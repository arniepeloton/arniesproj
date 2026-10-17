define([
    
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/CreditAndCollection/settings/GrantCredit/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.CreditAndCollection.settings.GrantCredit = ERP.CreditAndCollection.settings.GrantCredit.controller(); // singleton
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.CreditAndCollection.settings.GrantCredit.controller', null, function () {

        var mOut = {};
        var mElement;
        var mCustomers = [];
        var mRemovedCustomers = [];
        mElement = $(mHtml);
        mOut.element = mElement;
        var mTable = mElement.find('table');
        var mDataTable;

        mElement.find('[name=butAddCustomer]').click(function() {
            BrowsePopup('LibraryOrganization', 'Customer',
                [
                    {
                        // Search: mTxtCustomer.val(), // commented to prevent auto search upon popup
                        DisplayText: [{ Column: 'CompanyName' }],
                        Click: function (e) {
                            var pID = ReadyGuid(e.ID);
                            if (CustomerAlreadyAdded(pID)) {
                                NewMessageBox(e.CompanyName + ' is already in the list.', 'Already Listed');
                                return;
                            }
                            mCustomers.push({
                                ID: pID,
                                CompanyName: e.CompanyName
                            });
                            mRemovedCustomers = mRemovedCustomers.filter(function(value) {
                                return pID != value;
                            });
                            DisplayTable();
                        }
                    }
                ], {
                    FilterCustomers: 1
                }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        });

        function CustomerAlreadyAdded(vCustomerID) {
            for (var pIndex = 0; pIndex <= mCustomers.length - 1; pIndex++) {
                var pItem = mCustomers[pIndex];
                if (pItem.ID == vCustomerID)
                    return true;
            }
            return false;
        }

        function DisplayTable() {

            ClearTable();
      
            var pColumns = [
                {
                    title: 'Customer',
                    render: function(v1, v2, vData) {
                        return CStr2(vData.CompanyName);
                    }
                },
                {
                    title: 'Remove',
                    render: function(v1, v2, vData) {
                        return '<div name=divRemove></div>';
                    }
                }
            ];
          
            mDataTable = mTable.DataTable({
                data: mCustomers,
                columns: pColumns,
                order: [],
                rowCallback: function(row, data, dataIndex){
                    var pDiv = $(row).find('[name=divRemove]');
                    if (pDiv.children().length > 0)
                        return;

                    var pTotal = CDec2(data.VendorPoSummaryNotFinalized) + CDec2(data.VendorPoSummaryFinalized)
                    var pLink = $('<a>')
                        .text('Remove')
                        .attr('href', '#')
                        .click(function(e) {
                            e.preventDefault();
                            mCustomers = mCustomers.filter(function(value) {
                                return data.ID != value.ID;
                            });
                            mRemovedCustomers = mRemovedCustomers.filter(function(value) {
                                return data.ID != value;
                            });
                            mRemovedCustomers.push(data.ID);
                            $(row).remove();
                            return false;
                        });
                    pDiv.append(pLink);
                },
                sDom: '<"top">rt<"bottom"ip><"clear">', //hide search label and field
            });
            mTable.addClass('table-stripe').css('width', '100%')
            mTable.find('thead tr').addClass('custom-theme-f');

            $('[name="txtSearchGrantCustomer"]').unbind('keydown').keydown(function () {
                mDataTable.search($.trim($(this).val())).draw();
            })
        }

        function ClearTable() {
            if ($.fn.DataTable.isDataTable(mTable)) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                mTable.empty();
                mDataTable.destroy();
            }
        }

        mOut.GetInputData = function() {
            var pOut = {};
            pOut.GrantCreditOffline = mElement.find('#selGrantCredit').val();
            pOut.GrantCreditAllCustomers = mElement.find('#selGrantCreditAllCustomers').val();
            pOut.CustomerIDs = [];
            for (var pIndex = 0; pIndex <= mCustomers.length - 1; pIndex++) 
                pOut.CustomerIDs.push(mCustomers[pIndex].ID);
            pOut.RemovedCustomers = mRemovedCustomers
            return pOut;
        };

        mOut.LoadData = function LoadData() {
            AjaxSend({
                url: 'CreditAndCollection/GetSettings',
                success: function(vData) {
                    if (vData.Error) {
                        NewMessageBox(vData.Error);
                    } else {
                        mElement.find('#selGrantCredit').val(CBool(vData.GrantCreditOffline) ? 1 : 0);//.slider("refresh");
                        mElement.find('#selGrantCreditAllCustomers').val(CBool(vData.GrantCreditAllCustomers) ? 1 : 0);//.slider("refresh");
                        $('[name="expirationdaysforsoalink"]').val(vData.ExpirationDaysForSOALink || 7);
                        $('[name="soalinkdomain"]').val(vData.SOALinkDomainName);
                        $('[name="soalinkinternalipaddress"]').val(vData.SOALinkDomainInternalIPAddress);
                        $('[name="soalinkexternalipaddress"]').val(vData.SOALinkDomainExternalIPAddress);
                    }
                    mCustomers = vData.Customers.Rows;
                    mRemovedCustomers = [];
                    for (var pIndex = 0; pIndex <= mCustomers.length - 1; pIndex++) {
                        mCustomers[pIndex].ID = ReadyGuid(mCustomers[pIndex].ID);
                    }
                    DisplayTable();
                }
            });
        };

        return mOut;
    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});