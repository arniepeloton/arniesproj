define([
    'promise!/Scripts/sources/ERP/Deposits/work/model.js?v=' + assetversion
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/Deposits/work/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.Deposits.work.controller', null, function (vParams) {

        vParams = $.extend({
            mode: 'revenue',
            dontLoadData: false
        }, vParams);
        var mOut = {};
        var mElement;
        var mWorkModel = new ERP.Deposits.work.model({
            mode: vParams.mode
        });
        var mFilterCustomerID;
        var mFilterPaymentFowstID;
        var mCheckedData;
        var mIncludeCash = false;
        var mIncludeChecks = true;
        var mIncludeCreditCards = false;
        var mIncludeDebitCards = false;
        var mIncludeDeposited = false;
        var mIncludeReturned = false;
        var mIncludeUndeposited = true;
        var mDepositDate = FormatDate(new Date(), {Format: 'MMM d, yyyy'});
        var mBankID;
        var mDepositStatus;
        var mAllRows;
        var mDepositGroupID;

        var mCashCountMark = 0;
        var mCashAmountMark = 0;
        var mCashCountAll = 0;
        var mCashAmountAll = 0;

        var mCheckCountMark = 0;
        var mCheckAmountMark = 0;
        var mCheckCountAll = 0;
        var mCheckAmountAll = 0;

        var mCreditCardCountMark = 0;
        var mCreditCardAmountMark = 0;
        var mCreditCardCountAll = 0;
        var mCreditCardAmountAll = 0;

        var mDebitCardCountMark = 0;
        var mDebitCardAmountMark = 0;
        var mDebitCardCountAll = 0;
        var mDebitCardAmountAll = 0;

        var mMode = vParams.mode; // revenue or disbursement
        mElement = $(mHtml);
        mOut.element = mElement;
        var mTable = mElement.find('#tableDeposits');
        var mButDateDeposited = mElement.find('[name=butDateDeposited]');
        mButDateDeposited.text(mDepositDate);
        var mTxtBankBranch = mElement.find('[name=txtBankBranch]');
        var mTxtAccountName = mElement.find('[name=txtAccountName]');
        var mTxtAccountNumber = mElement.find('[name=txtAccountNumber]');
        var mTableTotals = mElement.find('[name=tableTotals]');
        var mTxtSearch = mElement.find('[name=butLookup]');
        mTxtSearch.unbind('keydown').keydown(function (e) {
            if (e.which == 13) {
                LoadData();
            }
        });
        var mButLookup = mElement.find('[name=butLookup]');
        mButLookup.click(Lookup);

        var mDataTable;

        $('#tabDepositsWork').append(mElement);//.trigger('create');

        mOut.LoadData = LoadData;
        function LoadData(vDepositGroupID) {
            mDepositGroupID = vDepositGroupID;
            var pData;
            if (CStr2(vDepositGroupID) == '') {
                pData = {
                    EncodingDateRange: $('#butDepositsEncodingDate').text(),
                    DoNotFilterEncodingDate: $('#chkDepositsEncodingDateDoNotFilter').prop('checked'),
                    MaturityDateRange: $('#butDepositsPaymentMaturityDate').text(),
                    DoNotFilterMaturityDate: $('#chkDepositsPaymentMaturityDateDoNotFilter').prop('checked'),
                    DepositDateRange: $('#butDepositsDepositDate').text(),
                    DoNotFilterDepositDate: $('#chkDepositsDepositDateDoNotFilter').prop('checked'),
                    IncludeCash: mIncludeCash,
                    IncludeChecks: mIncludeChecks,
                    IncludeCreditCards: mIncludeCreditCards,
                    IncludeDebitCards: mIncludeDebitCards,
                    IncludeDeposited: mIncludeDeposited,
                    IncludeReturned: mIncludeReturned,
                    IncludeUndeposited: mIncludeUndeposited,
                    SearchKeywords: mTxtSearch.val()
                };
                if (IsGuid(mFilterPaymentFowstID))
                    pData.PaymentFowstID = mFilterPaymentFowstID;
                if ($('#chkDepositsCustomerDoNotFilter').prop('checked') == false)
                    pData.CustomerID = mFilterCustomerID;
            } else {
                pData = {
                    DepositGroupID: vDepositGroupID,
                    DoNotFilterEncodingDate: 1,
                    DoNotFilterMaturityDate: 1,
                    DoNotFilterDepositDate: 1,
                    IncludeCash: 1,
                    IncludeChecks: 1,
                    IncludeCreditCards: 1,
                    IncludeDebitCards: 1,
                    IncludeDeposited: 1,
                    IncludeReturned: 1,
                    IncludeUndeposited: 1
                };

                mTxtSearch.hide();
                $('#divDepositsMainTab').hide();
                mElement.find('[name=butTagAsDeposited]').hide();
                mElement.find('[name=divDepositsEncodingDate]').hide();
            }
            
            ClearTable();
            //ActivateTab.call($('#divDepositsWorkMarkTab').find('#chkShowDepositsAll'));
            mWorkModel.GetData({
                data: pData,
                success: function(vRows) {
                    mAllRows = vRows;
                    DisplayTable(vRows);
                }
            });
        } // END function LoadData() {

        mElement.find('[name=butDepositsRefresh]').click(function() {
            LoadData();
        }); // END mElement.find('#butDepositsRefresh').click(function() {

        function Lookup() {
            var pColumns = [
                {
                    Label: 'Depository Bank Name',
                    Column: 'DepositoryBankName'
                },
                {
                    Label: 'Depository Bank Branch',
                    Column: 'DepositoryBankBranch'
                },
                {
                    Label: 'Depository Account Name',
                    Column: 'DepositoryAccountName'
                },
                {
                    Label: 'Depository Account Number',
                    Column: 'DepositoryAccountNumber'
                },
                {
                    Label: 'Deposit Date',
                    Column: function(vDR) {
                        return FormatDate(vDR.DepositDate, {DontDisplay12Am: true});
                    }
                },
                {
                    Label: 'Amount Deposited',
                    Column: function(vDR) {
                        return AccountingFormat(vDR.AmountDeposited);
                    },
                    className: 'dt-body-right'
                },
                {
                    Label: 'Date Encoded',
                    Column: 'TimeChecked'
                }
            ];
            BrowsePopup('CustomerPaymentReport', "Deposits",
                [
                    {
                        ModalSize: 'fullwidth',
                        StyleFormat: 'table',
                        AutoSearch: false,
                        DisplayText: pColumns,
                        Click: function(vData) {
                            //LoadData(vData.DepositGroupID);
                            var win = window.open(location.origin + '/erp/deposits?ID=' + vData.DepositGroupID,
                                '_blank',
                                'width=' + screen.availWidth + ',height=' + screen.availHeight);
                        },
                        InitComplete: function(vTable) {
                            //vTable.find('input').click(function(e) {
                            //    e.stopPropagation();
                            //});
                        },
                        Sorting: true,
                        NoInitialSorting: true
                    }
                ], { 
                    GroupByHeader: 1,
                    DoNotFilterEncodingDate: 1,
                    DoNotFilterMaturityDate: 1,
                    DoNotFilterDepositDate: 1,
                    IncludeCash: 1,
                    IncludeChecks: 1,
                    IncludeCreditCards: 1,
                    IncludeDebitCards: 1,
                    IncludeDeposited: 1,
                    IncludeReturned: 1,
                    IncludeUndeposited: 1
                }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        }

        function ClearTable() {
            if ($.fn.DataTable.isDataTable(mTable)) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                mTable.empty();
                mDataTable.destroy();
            }
        }

        function DisplayTable(vRows) {
            mDataTable = mTable.DataTable({
                data: vRows,
                columns: [
                    {
                        'searchable':false,
                        'orderable':false,
                        'width':'1%',
                        'className': 'text-center',
                        'render': function (data, type, full, meta){
                            return '<input type="checkbox" class="form-check-input" name="chkSelector">';
                        }
                    },
                    {
                        'className': 'text-center',
                        render: function(v1, v2, vData) {
                            if (vData.DepositStatus == '1')
                                return 'Deposited';
                            else if (vData.DepositStatus == '2')
                                return 'Returned';
                            else
                                return '';
                        }
                    },
                    {
                        'className': 'text-center',
                        data: function(vData) {
                            return CStr2(vData.DepositedByName);
                        }
                    },
                    {
                        'className': 'text-center',
                        data: function(vData) {
                            if (vData.DepositDate)
                                return FormatDate(vData.DepositDate);
                            else
                                return '';
                        }
                    },
                    {

                        data: function(vData) {
                            return vData.DepositoryBankName || '';
                        }
                    },
                    {data: 'CompanyName'},
                    {
                        'className': 'text-center',
                        data: function(vData) {
                            return $.format.date(new Date(vData.PaymentDate), 'MMM dd, yyyy');
                        }
                    },
                    {data: 'PaymentType'},
                    {
                        'className': 'text-center',
                        data: function(vData) {
                            return vData.RefNum || '';
                        }
                    },
                    {
                        data: function(vData) {
                            return vData.BankName || '';
                        }
                    },
                    {
                        'className': 'text-center',
                        data: function(vData) {
                            if (IsDate(vData.MaturityDate))
                                return $.format.date(new Date(vData.MaturityDate), 'MMM dd, yyyy');
                            else
                                return '';
                        }
                    },
                    {
                        'className': 'text-right',
                        data: function(vData) {
                            return AccountingFormat(vData.AmountPaid);
                        }
                    }
                ],
                'rowCallback': function(row, data, dataIndex) {
                    $(row).data('data', data);
                    $(row).find('[name=chkSelector]').prop('checked', data.Checked);
                    $(row).find('[name=chkSelector]').unbind('click').click(CheckBox_OnClick);
                },
                "initComplete": function () {
                    mTable.removeClass('dataTable');
                }
            });

            mTable.find('[name=select_all]').unbind('click').click(function(e) {
                UnbindCheckBoxes();

                if(this.checked){
                    mTable.find('tbody input[type="checkbox"]:not(:checked)').trigger('click');
                    mCashCountMark = mCashCountAll;
                    mCashAmountMark = mCashAmountAll;
                    mCheckCountMark = mCheckCountAll;
                    mCheckAmountMark = mCheckAmountAll;
                    mCreditCardCountMark = mCreditCardCountAll;
                    mCreditCardAmountMark = mCreditCardAmountAll;
                    mDebitCardCountMark = mDebitCardCountAll;
                    mDebitCardAmountMark = mDebitCardAmountAll;
                } else {
                    mTable.find('tbody input[type="checkbox"]:checked').trigger('click');
                    mCashCountMark = 0;
                    mCashAmountMark = 0;
                    mCheckCountMark = 0;
                    mCheckAmountMark = 0;
                    mCreditCardCountMark = 0;
                    mCreditCardAmountMark = 0;
                    mDebitCardCountMark = 0;
                    mDebitCardAmountMark = 0;
                }
                var pData = mDataTable.data();
                for (var p = 0; p <= pData.length - 1; p++) {
                    var pDR = pData[p];
                    pDR.Checked = this.checked;
                }
                DisplayTotals();
                BindCheckBoxes();
                e.stopPropagation();
            });

            mCashCountMark = 0;
            mCashAmountMark = 0;
            mCashCountAll = 0;
            mCashAmountAll = 0;

            mCheckCountMark = 0;
            mCheckAmountMark = 0;
            mCheckCountAll = 0;
            mCheckAmountAll = 0;

            mCreditCardCountMark = 0;
            mCreditCardAmountMark = 0;
            mCreditCardCountAll = 0;
            mCreditCardAmountAll = 0;

            mDebitCardCountMark = 0;
            mDebitCardAmountMark = 0;
            mDebitCardCountAll = 0;
            mDebitCardAmountAll = 0;

            for (var p = 0; p <= vRows.length - 1; p++) {
                var pDR = vRows[p];
                if (pDR.PaymentType == 'Cash') {
                    mCashCountAll++;
                    mCashAmountAll += CDec2(pDR.AmountPaid);
                } else if (pDR.PaymentType == 'Check') {
                    mCheckCountAll++;
                    mCheckAmountAll += CDec2(pDR.AmountPaid);
                } else if (pDR.PaymentType == 'Credit Card') {
                    mCreditCardCountAll++;
                    mCreditCardAmountAll += CDec2(pDR.AmountPaid);
                } else if (pDR.PaymentType == 'Debit Card') {
                    mDebitCardCountAll++;
                    mDebitCardAmountAll += CDec2(pDR.AmountPaid);
                }
                ComputeTotalByRow(pDR, true);
            }
            DisplayTotals();
        }

        function ComputeTotalByRow(vDR, vDontProcessUnmarked) {
            vDontProcessUnmarked = CBool(vDontProcessUnmarked);

            if (vDR.PaymentType == 'Cash') {
                if (vDR.Checked) {
                    mCashCountMark++;
                    mCashAmountMark += CDec2(vDR.AmountPaid);
                } else if (!vDontProcessUnmarked) {
                    mCashCountMark--;
                    mCashAmountMark -= CDec2(vDR.AmountPaid);
                }
            } else if (vDR.PaymentType == 'Check') {
                if (vDR.Checked) {
                    mCheckCountMark++;
                    mCheckAmountMark += CDec2(vDR.AmountPaid);
                } else if (!vDontProcessUnmarked) {
                    mCheckCountMark--;
                    mCheckAmountMark -= CDec2(vDR.AmountPaid);
                }
            } else if (vDR.PaymentType == 'Credit Card') {
                if (vDR.Checked) {
                    mCreditCardCountMark++;
                    mCreditCardAmountMark += CDec2(vDR.AmountPaid);
                } else if (!vDontProcessUnmarked) {
                    mCreditCardCountMark--;
                    mCreditCardAmountMark -= CDec2(vDR.AmountPaid);
                }
            } else if (vDR.PaymentType == 'Debit Card') {
                if (vDR.Checked) {
                    mDebitCardCountMark++;
                    mDebitCardAmountMark += CDec2(vDR.AmountPaid);
                } else if (!vDontProcessUnmarked) {
                    mDebitCardCountMark--;
                    mDebitCardAmountMark -= CDec2(vDR.AmountPaid);
                }
            }
        } 

        function DisplayTotals() {
            var pCountMark = mCashCountMark + mCheckCountMark + mCreditCardCountMark + mDebitCardCountMark;
            var pAmountMark = mCashAmountMark + mCheckAmountMark + mCreditCardAmountMark + mDebitCardAmountMark;
            var pCountAll = mCashCountAll + mCheckCountAll + mCreditCardCountAll + mDebitCardCountAll;
            var pAmountAll = mCashAmountAll + mCheckAmountAll + mCreditCardAmountAll + mDebitCardAmountAll;

            mTableTotals.find('[name=CashCountMark]').text(AccountingFormat(mCashCountMark, 0));
            mTableTotals.find('[name=CashAmountMark]').text(AccountingFormat(mCashAmountMark));
            mTableTotals.find('[name=CashCountAll]').text(AccountingFormat(mCashCountAll, 0));
            mTableTotals.find('[name=CashAmountAll]').text(AccountingFormat(mCashAmountAll));

            mTableTotals.find('[name=CheckCountMark]').text(AccountingFormat(mCheckCountMark, 0));
            mTableTotals.find('[name=CheckAmountMark]').text(AccountingFormat(mCheckAmountMark));
            mTableTotals.find('[name=CheckCountAll]').text(AccountingFormat(mCheckCountAll, 0));
            mTableTotals.find('[name=CheckAmountAll]').text(AccountingFormat(mCheckAmountAll));

            mTableTotals.find('[name=CreditCardCountMark]').text(AccountingFormat(mCreditCardCountMark, 0));
            mTableTotals.find('[name=CreditCardAmountMark]').text(AccountingFormat(mCreditCardAmountMark));
            mTableTotals.find('[name=CreditCardCountAll]').text(AccountingFormat(mCreditCardCountAll, 0));
            mTableTotals.find('[name=CreditCardAmountAll]').text(AccountingFormat(mCreditCardAmountAll));

            mTableTotals.find('[name=DebitCardCountMark]').text(AccountingFormat(mDebitCardCountMark, 0));
            mTableTotals.find('[name=DebitCardAmountMark]').text(AccountingFormat(mDebitCardAmountMark));
            mTableTotals.find('[name=DebitCardCountAll]').text(AccountingFormat(mDebitCardCountAll, 0));
            mTableTotals.find('[name=DebitCardAmountAll]').text(AccountingFormat(mDebitCardAmountAll));

            mTableTotals.find('[name=GrandTotalCountMark]').text(AccountingFormat(pCountMark, 0));
            mTableTotals.find('[name=GrandTotalAmountMark]').text(AccountingFormat(pAmountMark));
            mTableTotals.find('[name=GrandTotalCountAll]').text(AccountingFormat(pCountAll, 0));
            mTableTotals.find('[name=GrandTotalAmountAll]').text(AccountingFormat(pAmountAll));
        }

        function BindCheckBoxes() {
            mTable.find('[name=chkSelector]').unbind('click').click(CheckBox_OnClick);
        }

        function CheckBox_OnClick() {
            var pCheckedBox = $(this);
            var pTr = $(pCheckedBox).closest('tr');
            pTr = $(pTr);
            var pDR = pTr.data('data');
            pDR.Checked = this.checked;
            pDR.TimeChecked = FormatDate(new Date(), {Format: 'yyyy-MM-dd HH:mm:ss.SSS'});

            ComputeTotalByRow(pDR);
            DisplayTotals();
        }

        function UnbindCheckBoxes() {
            mTable.find('[name=chkSelector]').unbind('click');
        }

        mElement.find('#butDepositsEncodingDate, #butDepositsPaymentMaturityDate, #butDepositsDepositDate').click(function (vEl) {
            PopupPeriod({
                DisplayTodayOrEarlier: true,
                Click: function (e) {
                    DisplayDepositsEncodingDate($(vEl.target).attr('id'), e);
                    LoadData();
                }
            });        
        });

        function DisplayDepositsEncodingDate(vID, vData) {
            if (IsTodayOrEarlier(vData)) {
                mElement.find('#' + vID).text(vData);
            } else {
                var dateNow = $.format.date(new Date(), 'MMM dd, yyyy');
                var from = (vData == undefined ? dateNow : vData.From);
                var to = (vData == undefined ? dateNow : vData.To);
                mElement.find('#' + vID).text(from + ' to ' + to);
            }
        }

        mElement.find('[name=butTagAsDeposited]').click(function() {
            PromptSaveCheckedItems('deposit',1);
        });

        mElement.find('[name=butTagAsReturned]').click(function() {
            PromptSaveCheckedItems('return', 2);
        });

        mElement.find('[name=butUntag]').click(function() {
            PromptSaveCheckedItems('untag', 0);
        });

        function PromptSaveCheckedItems(vText, vValue) {
            
            var pMsg;

            mDepositStatus = vValue;
            mCheckedData = [];
            
            var pData = mDataTable.data();
            for (var p = 0; p <= pData.length - 1; p++) {
                var pDR = pData[p];
                if (pDR.Checked)
                    mCheckedData.push({
                        ID: pDR.ID,
                        PaymentType: pDR.PaymentType,
                        TimeChecked: pDR.TimeChecked
                    });
            }

            if (mCheckedData.length == 0) {
                NewMessageBox('Please mark at least 1 check box.');
                return;
            }

            pMsg = 'Are you sure you want to ' + vText + ' the ' + mCheckedData.length + ' marked items?';
            $('#popDepositsSubmit').find('[name=message]').text(pMsg);
            $('#popDepositsSubmit').modal('show');
            
            //NewMessageBox(pMsg, 'Take action to checked items', [
            //    {
            //        Text: 'Yes',
            //        Click: function() {
            //            $('#popDepositsSubmit').modal('show');
            //            // SaveCheckedItems(vValue);
            //        }
            //    },
            //    {
            //        Text: 'No'
            //    }
            //]);
        }

        mElement.find('[name=butPaymentTypes]').click(function() {
            $('#chkDepositsCash').prop('checked', mIncludeCash);
            $('#chkDepositsCheck').prop('checked', mIncludeChecks);
            $('#chkDepositsCreditCard').prop('checked', mIncludeCreditCards);
            $('#chkDepositsDebitCard').prop('checked', mIncludeDebitCards);
            var p = $('#popupDepositsPaymentTypes');
            //p = p.popup();
            //p = p.show();
            p = p.modal('show');
        });

        mElement.find('[name=butPaymentTypesOk]').click(function() {
            var pIncludeCash = $('#chkDepositsCash').prop('checked');
            var pIncludeChecks = $('#chkDepositsCheck').prop('checked');
            var pIncludeCreditCards = $('#chkDepositsCreditCard').prop('checked');
            var pIncludeDebitCards = $('#chkDepositsDebitCard').prop('checked');
            var pTotal = 0;

            pTotal += pIncludeCash ? 1 : 0;
            pTotal += pIncludeChecks ? 1 : 0;
            pTotal += pIncludeCreditCards ? 1 : 0;
            pTotal += pIncludeDebitCards ? 1 : 0;

            if (pTotal == 0) {
                NewMessageBox('Please select at least 1 payment type.');
                return;
            }

            if (pTotal == 4)
                mElement.find('[name=butPaymentTypes]').text('All');
            else if (pTotal == 1) {
                if (pIncludeCash)
                    mElement.find('[name=butPaymentTypes]').text('Cash');
                else if (pIncludeChecks)
                    mElement.find('[name=butPaymentTypes]').text('Check');
                else if (pIncludeCreditCards)
                    mElement.find('[name=butPaymentTypes]').text('Credit Card');
                else if (pIncludeDebitCards)
                    mElement.find('[name=butPaymentTypes]').text('Debit Card');
            } else 
                mElement.find('[name=butPaymentTypes]').text('Some');

            mIncludeCash = pIncludeCash;
            mIncludeChecks = pIncludeChecks;
            mIncludeCreditCards = pIncludeCreditCards;
            mIncludeDebitCards = pIncludeDebitCards;

            $('#popupDepositsPaymentTypes').modal('hide');
            LoadData();
        });

        mElement.find('[name=butDepositDownload]').click(function (e) {
            e.preventDefault();
            var pData;
            if (mDepositGroupID)
                pData = {
                    DepositGroupID: mDepositGroupID,
                    DoNotFilterEncodingDate: 1,
                    DoNotFilterMaturityDate: 1,
                    DoNotFilterDepositDate: 1,
                    IncludeCash: 1,
                    IncludeChecks: 1,
                    IncludeCreditCards: 1,
                    IncludeDebitCards: 1,
                    IncludeDeposited: 1,
                    IncludeReturned: 1,
                    IncludeUndeposited: 1,
                    DownloadAsDepositsPdf: 1
                };
            else
                pData = {
                    EncodingDateRange: $('#butDepositsEncodingDate').text(),
                    DoNotFilterEncodingDate: $('#chkDepositsEncodingDateDoNotFilter').prop('checked'),
                    MaturityDateRange: $('#butDepositsPaymentMaturityDate').text(),
                    DoNotFilterMaturityDate: $('#chkDepositsPaymentMaturityDateDoNotFilter').prop('checked'),
                    DepositDateRange: $('#butDepositsDepositDate').text(),
                    DoNotFilterDepositDate: $('#chkDepositsDepositDateDoNotFilter').prop('checked'),
                    IncludeCash: mIncludeCash,
                    IncludeChecks: mIncludeChecks,
                    IncludeCreditCards: mIncludeCreditCards,
                    IncludeDebitCards: mIncludeDebitCards,
                    IncludeDeposited: mIncludeDeposited,
                    IncludeReturned: mIncludeReturned,
                    IncludeUndeposited: mIncludeUndeposited,
                    DownloadAsDepositsPdf: 1
                };

            if ($('#chkDepositsCustomerDoNotFilter').prop('checked') == false)
                pData.CustomerID = mFilterCustomerID;

            AjaxSend({
                url: '/Browser/CustomerPaymentReport',
                data: pData,
                success: function(vData) {
                    if (vData.Error)
                        NewMessageBox(vData.Error);
                    else if (vData.Filename) 
                        ViewReport(vData.Filename) ;
                }
            });

            function ViewReport(vFilename) {
                
                var pUrl = MergeUrlParameters({
                    Filename: vFilename
                }, '/ERP/DownloadPrintablePdf');
                window.open(pUrl, '_blank', 
                    'width=' + window.innerWidth + 
                    ',height=' + window.innerHeight + 
                    ',scrollbars=auto');
                NewMessageBox('You should be viewing the PDF by now. If you are unable to view the PDF, please check your web browser\'s popup settings.');
            } // END function ViewReport() {
        });

        mElement.find('#butDepositsCustomer').click(function (vEl) {
            BrowsePopup('LibraryOrganization', 'Customer',
                [
                    {
                        //Search: mTxtCustomer.val(),
                        DisplayText: [{ Column: 'CompanyName' }],
                        Click: function (e) {
                            mElement.find('#butDepositsCustomer').text(e.CompanyName);
                            mFilterCustomerID = e.ID;
                        }
                    }
                ], {
                    FilterCustomers: 1
                }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        });

        mElement.find('[name=butPaymentFowst]').click(function (vEl) {
            BrowsePopup('LibraryFowst', 'FOWST',
                [
                    {
                        //Search: mTxtCustomer.val(),
                        DisplayText: [{ Column: 'FowstName' }],
                        Click: function (e) {
                            mElement.find('[name=butPaymentFowst]').text(e.FowstName);
                            mFilterPaymentFowstID = e.ID;
                        }
                    }
                ], {
                    IncludeAllFowstOptions: 1
                }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        });

        mElement.find('[name=butDepositStatus]').click(function() {
            $('#chkDepositsStatusDeposited').prop('checked', mIncludeDeposited);
            $('#chkDepositsStatusReturned').prop('checked', mIncludeReturned);
            $('#chkDepositsStatusUndeposited').prop('checked', mIncludeUndeposited);
            $('#popupDepositsStatus').modal('show');
            LoadData();
        });

        mElement.find('[name=butDepositsStatusOk]').click(function() {
            var pIncludeDeposited = $('#chkDepositsStatusDeposited').prop('checked');
            var pIncludeReturned = $('#chkDepositsStatusReturned').prop('checked');
            var pIncludeUndeposited = $('#chkDepositsStatusUndeposited').prop('checked');
            var pTotal = 0;

            pTotal += pIncludeDeposited ? 1 : 0;
            pTotal += pIncludeReturned ? 1 : 0;
            pTotal += pIncludeUndeposited ? 1 : 0;

            if (pTotal == 0) {
                NewMessageBox('Please select at least 1 deposit status.');
                return;
            }

            mIncludeDeposited = pIncludeDeposited;
            mIncludeReturned = pIncludeReturned;
            mIncludeUndeposited = pIncludeUndeposited;

            RefreshDepositedStatusFilterLabel();

            $('#popupDepositsStatus').modal('hide');
            LoadData();
        });

        function RefreshDepositedStatusFilterLabel() {

            var pTotal = 0;
       
            pTotal += mIncludeDeposited ? 1 : 0;
            pTotal += mIncludeReturned ? 1 : 0;
            pTotal += mIncludeUndeposited ? 1 : 0;

            if (pTotal == 3)
                mElement.find('[name=butDepositStatus]').text('All');
            else if (pTotal == 1) {
                if (mIncludeDeposited)
                    mElement.find('[name=butDepositStatus]').text('Deposited');
                else if (mIncludeReturned)
                    mElement.find('[name=butDepositStatus]').text('Returned');
                else if (mIncludeUndeposited)
                    mElement.find('[name=butDepositStatus]').text('Undeposited');
            } else 
                mElement.find('[name=butDepositStatus]').text('Some');
        }

        mButDateDeposited.unbind('click').click(function (e) {
            e.preventDefault();
            SingleCalendar({
                Click: function (e) {
                    $('#popDepositsSubmit').modal('show');
                    mButDateDeposited.text(e);
                    mDepositDate = e;
                    //Popup.ChequeGC.txtCGCValue.focus();
                },
                Close: function () {
                    $('#popDepositsSubmit').modal('show');
                    //$('#popFMCGChequeGC').modal('show');
                    //Popup.ChequeGC.txtCGCValue.focus();
                    //Popup.popupname = 'cgc';
                }
            });
        });

        mElement.find('[name=lnkBank]').click(function(e) {
            BrowsePopup('LibraryOrganizationBanks', 'Depository Bank',
                [
                    {
                        //Search: mTxtCustomer.val(),
                        AutoSearch: true,
                        DisplayText: [{ Column: 'CompanyName' }],
                        Click: function (e) {
                            mBankID = e.ID;
                            mElement.find('[name=lnkBank]').text(e.CompanyName);
                            $('#popDepositsSubmit').modal('show');
                        }
                    }
                ], {
                }
            ); // END BrowsePopup
          
            e.preventDefault();
        });

        mElement.find('[name=butSave]').click(function(e) {
            SaveCheckedItems();
        });

        mElement.find('[name=butSaveAndPrint]').click(function(e) {
            SaveCheckedItems(true);
        });
        
        function SaveCheckedItems(vViewAsPdf) {
            
            if (mBankID == null) {
                AfterPopupClose = function() {
                    $('#popDepositsSubmit').modal('show');
                };
                NewMessageBox('Depository bank is required.', '', [
                    {
                        Text: 'Ok',
                        Click: function() {
                            $('#popDepositsSubmit').modal('show');
                        }
                    }
                ]);
                return;
            }

            mTxtBankBranch.val(mTxtBankBranch.val().trim());
            if (mTxtBankBranch.val() == '') {
                AfterPopupClose = function() {
                    $('#popDepositsSubmit').modal('show');
                };
                NewMessageBox('Depository bank branch is required.', '', [
                    {
                        Text: 'Ok',
                        Click: function() {
                            $('#popDepositsSubmit').modal('show');
                        }
                    }
                ]);
                return;
            }

            mTxtAccountName.val(mTxtAccountName.val().trim());
            if (mTxtAccountName.val() == '') {
                AfterPopupClose = function() {
                    $('#popDepositsSubmit').modal('show');
                };
                NewMessageBox('Account name is required.', '', [
                    {
                        Text: 'Ok',
                        Click: function() {
                            $('#popDepositsSubmit').modal('show');
                        }
                    }
                ]);
                return;
            }

            mTxtAccountNumber.val(mTxtAccountNumber.val().trim());
            if (mTxtAccountNumber.val() == '') {
                AfterPopupClose = function() {
                    $('#popDepositsSubmit').modal('show');
                };
                NewMessageBox('Account number is required.', '', [
                    {
                        Text: 'Ok',
                        Click: function() {
                            $('#popDepositsSubmit').modal('show');
                        }
                    }
                ]);
                return;
            }

            mWorkModel.SaveData({
                DepositoryBankID: mBankID,
                DepositoryBankBranch: mTxtBankBranch.val(),
                AccountName: mTxtAccountName.val(),
                AccountNumber: mTxtAccountNumber.val(),
                DepositDate: mDepositDate,
                value: mDepositStatus,
                data: mCheckedData,
                success: function(vData) {
                    if (vData.Error)
                        NewMessageBox(vData.Error);
                    else if (vData.Results == 1) {
                        LoadData();
                        NewMessageBox('Marked items were successfully tagged.', 'Success', null, {
                            Close: function() {
                                $('#popDepositsSubmit').modal('hide');
                            }
                        });
                        if (vViewAsPdf) {
                            if (!IsGuid(vData.DepositGroupID)) {
                                NewMessageBox('The replied DepositGroupID is not a valid GUID.');
                                return;
                            }
                            AjaxSend({
                                url: '/Browser/CustomerPaymentReport',
                                data: {
                                    DepositGroupID: vData.DepositGroupID,
                                    DownloadAsDepositsPdf: 1
                                },
                                success: function(vData) {
                                    if (vData.Error)
                                        NewMessageBox(vData.Error);
                                    else if (vData.Filename) 
                                        ViewReport(vData.Filename) ;
                                }
                            });
                        }
                    }
                }
            });

            function ViewReport(vFilename) {
                
                var pUrl = MergeUrlParameters({
                    Filename: vFilename
                }, '/ERP/DownloadPrintablePdf');
                window.open(pUrl, '_blank', 
                    'width=' + window.innerWidth + 
                    ',height=' + window.innerHeight + 
                    ',scrollbars=auto');
                NewMessageBox('You should be viewing the PDF by now. If you are unable to view the PDF, please check your web browser\'s popup settings.');
            } // END function ViewReport() {
        }

        $('#popDepositsSubmit').find('[name=butSearchAccountName]').click(function() {
            AfterPopupClose = function() {
                $('#popDepositsSubmit').modal('show');
            };
            SearchAccounts();
        });

        $('#popDepositsSubmit').find('[name=butAddAccountName]').click(function() {
            AfterPopupClose = function() {
                $('#popDepositsSubmit').modal('show');
            };
            mWorkModel.SaveAccountNameAndNumber({
                AccountName: mTxtAccountName.val(),
                AccountNumber: mTxtAccountNumber.val(),
                success: function(e) {
                    var pMsg;

                    if (e.Error)
                        pMsg = e.Error;
                    else if (e.Result == 'ok')
                        pMsg = 'Account Name and Account Number were successfully saved.';
                    else
                        pMsg = 'Unknown result. ref: js74Z3JSPLN6!';

                    AfterPopupClose = function() {
                        $('#popDepositsSubmit').modal('show');
                    };
                    NewMessageBox(pMsg, '', [
                        {
                            Text: 'Ok',
                            Click: function() {
                                $('#popDepositsSubmit').modal('show');
                            }
                        }
                    ]);
                }
            });
        });

        function SearchAccounts() {
            BrowsePopup('LibraryDepositAccounts', 'Account Name/Number',
                [
                    {
                        Search: mTxtAccountName.val(),
                        DisplayText: [
                            { 
                                Column: 'AccountName', 
                                Label: 'Account Name' 
                            },
                            { 
                                Column: 'AccountNumber', 
                                Label: 'Account Number' 
                            }
                        ],
                        Click: function (e) {
                            mTxtAccountName.val(e.AccountName);
                            mTxtAccountNumber.val(e.AccountNumber);
                            $('#popDepositsSubmit').modal('show');
                        },
                        StyleFormat: 'table'
                    }
                ], {
                }
            ); // END BrowsePopup
        }

        mElement.find('#chkShowDepositsAll').change(function (e) {
            if (!$(this).is(':checked'))
                return;
            ClearTable();
            DisplayTable(mAllRows);
            //ActivateTab.call(this, e);
        });

        mElement.find('#chkShowDepositsMarked').change(function (e) {
            if (!$(this).is(':checked'))
                return;
            var pNewRows = [];

            //ActivateTab.call(this, e);
            for (var p = 0; p <= mAllRows.length - 1; p++) {
                var pDR = mAllRows[p];
                if (pDR.Checked)
                    pNewRows.push(pDR);
            }
            ClearTable();
            DisplayTable(pNewRows);
        });

        mElement.find('#chkShowDepositsUnmarked').change(function (e) {
            if (!$(this).is(':checked'))
                return;
            var pNewRows = [];

            //ActivateTab.call(this, e);
            for (var p = 0; p <= mAllRows.length - 1; p++) {
                var pDR = mAllRows[p];
                if (!pDR.Checked)
                    pNewRows.push(pDR);
            }
            ClearTable();
            DisplayTable(pNewRows);
        });

        function ActivateTab(e) {
            if (e)
                e.stopPropagation();
            var pTabs = $(this).closest('.custom-tab');
            pTabs.find('li').removeClass('active');
            $(this).addClass('active');
        }

        var pDateTo = new Date();
        var pDateFrom = new Date(pDateTo.getFullYear(), pDateTo.getMonth() - 2, pDateTo.getDate());
        pDateFrom = FormatDate(pDateFrom, {Format: 'MMM d, yyyy'});
        pDateTo = FormatDate(pDateTo, {Format: 'MMM d, yyyy'});
        DisplayDepositsEncodingDate('butDepositsEncodingDate', {From: pDateFrom, To: pDateTo});
        DisplayDepositsEncodingDate('butDepositsPaymentMaturityDate', 'Today or Earlier');
        DisplayDepositsEncodingDate('butDepositsDepositDate', 'Today or Earlier');
        if (!vParams.dontLoadData)
            LoadData();
        RefreshDepositedStatusFilterLabel();

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});