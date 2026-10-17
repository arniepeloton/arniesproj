define(['/Scripts/sources/ERP/UnapprovedCustomerCreditTerms/model.js?v=' + assetversion
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/UnapprovedCustomerCreditTerms/view.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.UnapprovedCustomerCreditTerms.controller = ERP.UnapprovedCustomerCreditTerms.controller(); // singleton
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.UnapprovedCustomerCreditTerms.controller', null, function () {

        var mOut = {
            Model: ERP.UnapprovedCustomerCreditTerms.model
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
        mElement.find('#selUnapprovedOrdersTransactionType option[value="' + mTransactionType + '"]').prop('checked', true);

        if (proj.IsBootstrapDesign()) {
            $('#popupUnapprovedCustomerCreditTerms .popup-content > *').appendTo('#popupUnapprovedCustomerCreditTerms-bs .modal-body');
            $('#popupUnapprovedCustomerCreditTerms-bs').attr('id', 'popupUnapprovedCustomerCreditTerms');
        }

        mOut.Popup = function() {
            mOut.LoadWithUnapproved();

            if (proj.IsBootstrapDesign()) {
                $('#popupUnapprovedCustomerCreditTerms').modal('show');
            } else {
                $('#popupUnapprovedCustomerCreditTerms').show();
            }
        };

        mOut.LoadWithUnapproved = function() {

            var pCb = mElement.find('[name=selUnapprovedOrdersStatus]');

            mOut.Model.GetUnapproved({
                IncludeUnapproved: pCb.val() == '',
                IncludeApproved: pCb.val() == 'approved',
                Success: function(vData) {
                    DisplayUnapproved(vData);
                },
                Fail: function(vMessage) {
                    NewMessageBox(vMessage);
                }
            });
        };

        var mUnapprovedTable = mElement.find('[name=tableUnapproved]');
        var mUnapprovedRow = mUnapprovedTable.find('tbody tr');
        mUnapprovedRow.remove();
        function DisplayUnapproved(vData) {
            var pTotalOrder = 0;
            mUnapprovedTable.find('tbody tr').remove();
            for (var p = 0; p <= vData.Rows.length - 1; p++) {
                var pDR = vData.Rows[p];
                var pTR = mUnapprovedRow.clone();
                var pDate = pDR.SalesOrderDate || pDR.LastUpdated;
                if (IsDate(pDate)) {
                    pDate = $.format.date(ParseDate(pDate), 'M/d/yyyy');
                }
                pTotalOrder += CDec2(pDR.TotalOrder);
                pTR.find('[name=CustomerName]').text(pDR.CustomerName);
                pTR.find('[name=Fowst]').text(pDR.FowstName);
                pTR.find('[name=SalesRep]').text(pDR.SalesRepName);
                pTR.find('[name=CreditLimit]').text(AccountingFormat(pDR.CreditLimit));
                pTR.find('[name=CreditTerms]').text(pDR.TermName);
                pTR.find('[name=SalesRep]').text(pDR.SalesRepName);
                pTR.find('[name=EncodedByDate]').text(pDR.EncodedByName);
                
                pTR.click(function() {
                    UnapprovedOrder_Click($(this));
                });
                pTR.data('data', pDR);
                mUnapprovedTable.append(pTR);
            }
            mUnapprovedTable.find('tfoot [name=CustomerName]').text(vData.Rows.length + ' record[s]');
            mUnapprovedTable.find('tfoot [name=OrderAmount]').text(AccountingFormat(pTotalOrder));
        } // END DisplayWithCnCUnapprovedReason

        function UnapprovedOrder_Click(vTR) {
            var pDR = vTR.data('data');
            PopupButtonList('', pDR.CustomerName + ' - ' + pDR.FowstName, [
                {
                    Text: 'Approve',
                    Click: function() {
                        mOut.Model.CncApproveSalesOrder({
                            ID: pDR.ID,
                            Success: function() {
                                NewMessageBox(pDR.TransNum + ' was successfully approved.', '', null, {
                                    Close: function () {
                                        if (proj.IsBootstrapDesign()) {
                                            $('#popupUnapprovedCustomerCreditTerms').modal('show');
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
                                            $('#popupUnapprovedCustomerCreditTerms').modal('show');
                                        }
                                    }
                                });
                                mOut.LoadWithCnCUnapprovedReason();
                            }
                        });
                    }
                },
                {
                    Text: 'View Customer Account Details',
                    Click: function() {
                        require(['/Scripts/sources/CreditAndCollection/CreditAndCollection.js?v=' + assetversion], function() {
                            proj.CustomerPaymentID = pDR.CustomerID;
                            ERP.AccountDetails.controller.GetCustomerCreditDetails(proj.CustomerPaymentID, 'has payable');
                            $('#popupAccountDetails').find('.lblCustomerName').text(pDR.CustomerName);                           

                            if (proj.IsBootstrapDesign()) {
                                Bs.NestedModal('#popupUnapprovedCustomerCreditTerms', '#popupAccountDetails');
                            } else {
                                $('#popupUnapprovedCustomerCreditTerms').hide();
                                $('#popupAccountDetails').show();
                            }
                        });
                    }
                }
            ]);
        }

        mElement.find('[name=butRefresh]').click(function() {
            mOut.LoadWithUnapproved();
        });

        return mOut;
    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});