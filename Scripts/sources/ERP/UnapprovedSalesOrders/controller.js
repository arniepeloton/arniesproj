define(['/Scripts/sources/ERP/UnapprovedSalesOrders/model.js?v=' + assetversion,
    'promise!/Scripts/sources/ERP/AccountDetails/controller.js?v=' + assetversion,
    '/Scripts/sources/ERP/OrderProcessing/work/model.js?v=' + assetversion,
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/UnapprovedSalesOrders/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.UnapprovedSalesOrders.controller = ERP.UnapprovedSalesOrders.controller(); // singleton
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.UnapprovedSalesOrders.controller', null, function () {

        var mOut = {
            Model: ERP.UnapprovedSalesOrders.model
        };
        var mElement;
        var mTransactionType;

        mElement = $(mHtml);
        var pAdjustments = $.mobile.activePage.find('#popupAdjustment');
        if (pAdjustments.length)
            mElement.insertBefore(pAdjustments);
        else
            $.mobile.activePage.append(mElement);
        mOut.element = mElement;
        mTransactionType = localStorage.UnapprovedOrdersTransactionType;
        // mElement.find('#selUnapprovedOrdersTransactionType option[value="' + mTransactionType + '"]').prop('checked', true); // not working
        mElement.find('#selUnapprovedOrdersTransactionType').val(mTransactionType);

        if (proj.IsBootstrapDesign()) {
            $('#popupUnapprovedSalesOrders .popup-content > *').appendTo('#popupUnapprovedSalesOrders-bs .modal-body');
            $('#popupUnapprovedSalesOrders-bs').attr('id', 'popupUnapprovedSalesOrders');
        }

        mOut.Popup = function() {
            mOut.LoadWithCnCUnapprovedReason();

            if (proj.IsBootstrapDesign()) {
                $('#popupUnapprovedSalesOrders').modal('show');
            } else {
                $('#popupUnapprovedSalesOrders').show();
            }
        };

        mOut.LoadWithCnCUnapprovedReason = function() {

            var pCb = mElement.find('[name=selUnapprovedOrdersStatus]');
            
            mOut.Model.GetWithCnCUnapprovedReason({
                WithCnCUnapprovedReason: pCb.val() == '',
                WithApprovedCnC: pCb.val() == 'approved',
                WithCnCDisapproved: pCb.val() == 'disapproved',
                TransactionType: mTransactionType,
                Success: function(vData) {
                    DisplayWithCnCUnapprovedReason(vData.Results);
                },
                Fail: function(vMessage) {
                    NewMessageBox(vMessage);
                }
            });
        };

        var mWithCnCUnapprovedReasonTable = mElement.find('[name=tableUnapprovedOrders]');
        var mWithCnCUnapprovedReasonRow = mWithCnCUnapprovedReasonTable.find('tbody tr');
        mWithCnCUnapprovedReasonRow.remove();
        function DisplayWithCnCUnapprovedReason(vData) {
            var pTotalOrder = 0;
            mWithCnCUnapprovedReasonTable.find('tbody tr').remove();
            for (var p = 0; p <= vData.Rows.length - 1; p++) {
                var pDR = vData.Rows[p];
                var pTR = mWithCnCUnapprovedReasonRow.clone();
                var pDate = pDR.SalesOrderDate || pDR.LastUpdated;
                if (IsDate(pDate)) {
                    pDate = $.format.date(ParseDate(pDate), 'M/d/yyyy');
                }
                pTotalOrder += CDec2(pDR.TotalOrder);
                pTR.find('[name=CustomerName]').text(pDR.CompanyName);
                if (CInt2(pDR.SalesOrderNo) > 0)
                    pDR.TransNum = 'SO-' + pDR.MIN + '-' + pDR.SalesOrderNo;
                else if (CInt2(pDR.PurchaseOrderNo) > 0)
                    pDR.TransNum = 'PO-' + pDR.MIN + '-' +  pDR.PurchaseOrderNo;
                else if (CInt2(pDR.InvoiceNumber) > 0)
                    pDR.TransNum = 'SI-' + pDR.MIN + '-' +  pDR.InvoiceNumber;
                else
                    pDR.TransNum = pDR.MIN + '-' + pDR.InvoiceNumber;
                pTR.find('[name=OrderNum]').empty().append($('<a href="#">').text(pDR.TransNum).data('data', pDR).unbind('click').click(function (e) {
                    e.stopPropagation();
                    var vData = $(this).data('data');
                    PeekView({
                        ID: vData.ID,
                        DontReplaceCurrentID: false,
                        IncludeZeroQty: vData.TransType == 'PurchaseOrder',
                        TransType: vData.TransType
                    });
                }))
                pTR.find('[name=OrderDate]').text(pDate);
                pTR.find('[name=OrderAmount]').text(AccountingFormat(pDR.TotalOrder));
                pTR.find('[name=RequestedBy]').text(pDR.EmployeeFullName);
                pTR.find('[name=UnapprovedReason]').text(pDR.CnCUnapprovedReason);
                var pStatus = 'Unapproved';
                if (CStr2(pDR.CncApprovedByID) != '')
                    pStatus = 'Approved';
                if (CStr2(pDR.CncDisapprovedByID) != '')
                    pStatus = 'Disapproved';

                pTR.find('[name=Status]').text(pStatus);
                pTR.click(function() {
                    UnapprovedOrder_Click($(this));
                });
                pTR.data('data', pDR);
                mWithCnCUnapprovedReasonTable.append(pTR);
            }
            mWithCnCUnapprovedReasonTable.find('tfoot [name=CustomerName]').text(vData.Rows.length + ' record[s]');
            mWithCnCUnapprovedReasonTable.find('tfoot [name=OrderAmount]').text(AccountingFormat(pTotalOrder));
        } // END DisplayWithCnCUnapprovedReason

        function PeekView(vParams) {
            Bs.Loading('show');
            vParams = $.extend({
                ID: null,
                LoadProductsOnly: false,
                DontReplaceCurrentID: false,
                IncludeZeroQty: vParams.TransType == 'PurchaseOrder',
                AccountsPayableFollowCost: CBool(vParams.AccountsPayableFollowCost)
            }, vParams);
            new ERP.OrderProcessing.work.model().GetCart({
                ID: vParams.ID,
                IncludeZeroQty: vParams.IncludeZeroQty,
                IncludeSuppliers: mOut.ShowProductSuppliersColumn,
                AccountsPayableFollowCost: vParams.AccountsPayableFollowCost,
                IncludeChildOrderItems: vParams.TransType == 'SalesOrder',
                Success: function (vData) {
                    Bs.Loading('hide');
                    var msg = CopyTransToPlainText(vParams.TransType, vData, true);
                    NewMessageBox(msg.replace(/(?:\r\n|\r|\n)/g, '<br>'), 'Peek View');
                }
            });
        }

        function UnapprovedOrder_Click(vTR) {
            var pDR = vTR.data('data');
            var pButtons = [];
            if ((CStr2(pDR.CncApprovedByID) == '')
                && (CStr2(pDR.CncDisapprovedByID) == '')) {

                pButtons.push({
                    Text: 'Approve',
                    Click: function() {
                        mOut.Model.CncApproveSalesOrder({
                            ID: pDR.ID,
                            Success: function() {
                                NewMessageBox(pDR.TransNum + ' was successfully approved.', '', null, {
                                    Close: function () {
                                        if (proj.IsBootstrapDesign()) {
                                            $('#popupUnapprovedSalesOrders').modal('show');
                                        }
                                    }
                                });
                                mOut.LoadWithCnCUnapprovedReason();                                
                                //vTR.remove();
                            },
                            Fail: function(vMessage) {
                                NewMessageBox(vMessage, '', null, {
                                    Close: function () {
                                        if (proj.IsBootstrapDesign()) {
                                            $('#popupUnapprovedSalesOrders').modal('show');
                                        }
                                    }
                                });
                                mOut.LoadWithCnCUnapprovedReason();
                            }
                        });
                    }
                });
            }

            pButtons.push({
                Text: 'View Customer Account Details',
                Click: function() {
                    require(['/Scripts/sources/CreditAndCollection/CreditAndCollection.js?v=' + assetversion], function() {
                        proj.CustomerPaymentID = pDR.CustomerID;
                        ERP.AccountDetails.controller.GetCustomerCreditDetails(proj.CustomerPaymentID, 'has payable');
                        $('#popupAccountDetails').find('.lblCustomerName').text(pDR.CompanyName);                           

                        if (proj.IsBootstrapDesign()) {
                            Bs.NestedModal('#popupUnapprovedSalesOrders', '#popupAccountDetails');
                        } else {
                            $('#popupUnapprovedSalesOrders').hide();
                            $('#popupAccountDetails').show();
                        }
                    });
                }
            });

            if ((CStr2(pDR.CncApprovedByID) == '')
                && (CStr2(pDR.CncDisapprovedByID) == '')) {

                pButtons.push({
                    Text: 'Disapprove and Cancel Transaction',
                    Click: function() {
                        NewMessageBox('Are you sure you want to disapprove and cancel this transaction?', 'Disapprove and Cancel Transaction', [
                            {
                                Text: 'Yes',
                                Click: function() {
                                    Proceed();
                                }
                            },
                            {
                                Text: 'No'
                            }
                        ]);

                        function Proceed() {
                            mOut.Model.CncDisapproveSalesOrder({
                                ID: pDR.ID,
                                Success: function() {
                                    NewMessageBox(pDR.TransNum + ' was successfully disapproved and cancelled.', '', null, {
                                        Close: function () {
                                            if (proj.IsBootstrapDesign()) {
                                                $('#popupUnapprovedSalesOrders').modal('show');
                                            }
                                        }
                                    });
                                    mOut.LoadWithCnCUnapprovedReason();                                
                                    //vTR.remove();
                                },
                                Fail: function(vMessage) {
                                    NewMessageBox(vMessage, '', null, {
                                        Close: function () {
                                            if (proj.IsBootstrapDesign()) {
                                                $('#popupUnapprovedSalesOrders').modal('show');
                                            }
                                        }
                                    });
                                    mOut.LoadWithCnCUnapprovedReason();
                                }
                            });
                        }
                    }
                });
            }

            PopupButtonList('', pDR.TransNum, pButtons);
        }

        mElement.find('[name=butRefreshUnapprovedOrders]').click(function() {
            mTransactionType = $('#selUnapprovedOrdersTransactionType option:checked').val();
            localStorage.UnapprovedOrdersTransactionType = mTransactionType;
            mOut.LoadWithCnCUnapprovedReason();
        });

        return mOut;
    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});