define([
    
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/Invoicing/settings/AutoCreate/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.Invoicing.settings.AutoCreate = ERP.Invoicing.settings.AutoCreate.controller(); // singleton
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.Invoicing.settings.AutoCreate.controller', null, function () {

        var mOut = {};
        var mElement;
        var mVendors = [];
        var mRemovedVendors = [];
        mElement = $(mHtml);
        mOut.element = mElement;
        var mTable = mElement.find('[name=tableAutoCreateInvoiceVendors]');
        var mDataTable;
        var mSelAutoCreateInvoiceType = mElement.find('#selAutoCreateInvoiceType');
        var mFowstID;
        var mDeviceID;

        mElement.find('#butAutoCreateInvoiceFowst').click(function() {
            BrowsePopup('LibraryFowst', 'Invoice FOWST',
                [
                    {
                        AutoSearch: true,
                        DisplayText: [{ Column: 'FowstName' }],
                        Click: function (e) {
                            mElement.find('#butAutoCreateInvoiceFowst').text(e.FowstName);//.button('refresh');
                            mFowstID = e.ID;
                        }
                    }
                ], { 
                    OrganizationID: sessionStorage.OrgID,
                    HideNoAddress: 1
                }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        });

        mElement.find('#butAutoCreateInvoiceDevice').click(function() {
            BrowsePopup('LibraryDevice', 'Invoice Device', [
                {
                    AutoSearch: true,
                    DisplayText: [{ Column: 'PhoneName' }],
                    Click: function (e) {
                        mElement.find('#butAutoCreateInvoiceDevice').text(e.PhoneName);//.button('refresh');
                        mDeviceID = e.ID;
                    }
                }
            ], { IsAll: true })
        });

        mElement.find('[name=butAddAutoCreateInvoiceVendor]').click(function() {
            BrowsePopup('LibraryOrganization', 'Vendors',
                [
                    {
                        DisplayText: [{ Column: 'CompanyName' }],
                        Click: function (e) {
                            var pID = ReadyGuid(e.ID);
                            if (VendorAlreadyAdded(pID)) {
                                NewMessageBox(e.CompanyName + ' is already in the list.', 'Already Listed');
                                return;
                            }
                            mVendors.push({
                                ID: pID,
                                CompanyName: e.CompanyName
                            });
                            mRemovedVendors = mRemovedVendors.filter(function(value) {
                                return pID != value;
                            });
                            DisplayTable();
                        }
                    }
                ], {
                    FilterSupplier: 1
                }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        });

        function VendorAlreadyAdded(vVendorID) {
            for (var pIndex = 0; pIndex <= mVendors.length - 1; pIndex++) {
                var pItem = mVendors[pIndex];
                if (pItem.ID == vVendorID)
                    return true;
            }
            return false;
        }

        function DisplayTable() {

            ClearTable();
      
            var pColumns = [
                {
                    title: 'Vendor',
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
                data: mVendors,
                columns: pColumns,
                order: [],
                rowCallback: function(row, data, dataIndex){
                    var pDiv = $(row).find('[name=divRemove]');
                    if (pDiv.children().length > 0)
                        return;

                    var pLink = $('<a>')
                        .text('Remove')
                        .attr('href', '#')
                        .click(function(e) {
                            e.preventDefault();
                            mVendors = mVendors.filter(function(value) {
                                return data.ID != value.ID;
                            });
                            mRemovedVendors = mRemovedVendors.filter(function(value) {
                                return data.ID != value;
                            });
                            mRemovedVendors.push(data.ID);
                            $(row).remove();
                            return false;
                        });
                    pDiv.append(pLink);
                }
            });
            mTable.addClass('table-stripe').css('width', '100%')
            mTable.find('thead tr').addClass('custom-theme-f');
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
            pOut.AutoCreateInvoiceTypeID = mSelAutoCreateInvoiceType.val();
            pOut.AutoCreateInvoiceFowstID = mFowstID;
            pOut.AutoCreateInvoiceDeviceID = mDeviceID;
            pOut.AutoCreateInvoiceCondition = mElement.find('[name=radAutoCreateInvoiceCondition]:checked').val();

            pOut.VendorIDs = [];
            for (var pIndex = 0; pIndex <= mVendors.length - 1; pIndex++) 
                pOut.VendorIDs.push(mVendors[pIndex].ID);
            pOut.RemovedVendors = mRemovedVendors
            return pOut;
        };

        mOut.LoadData = function LoadData(vData) {
          
            DisplayInvoiceTypes(vData.InvoiceTypes);
            vData.AutoCreateInvoiceTypeID = ReadyGuid(vData.AutoCreateInvoiceTypeID);
            mSelAutoCreateInvoiceType.val(vData.AutoCreateInvoiceTypeID);//.selectmenu("refresh");

            mFowstID = ReadyGuid(vData.AutoCreateInvoiceFowstID);
            mElement.find('#butAutoCreateInvoiceFowst').text(vData.AutoCreateInvoiceFowstName);//.button("refresh");

            mDeviceID = ReadyGuid(vData.AutoCreateInvoiceDeviceID);
            mElement.find('#butAutoCreateInvoiceDevice').text(vData.AutoCreateInvoiceDeviceName);//.button("refresh");

            mElement.find('[name=radAutoCreateInvoiceCondition][value=' + vData.AutoCreateInvoiceCondition + ']').prop('checked', true);
            mElement.find('[name=radAutoCreateInvoiceCondition]');//.checkboxradio("refresh");

            mVendors = vData.Vendors.Rows;
            mRemovedVendors = [];
            for (var pIndex = 0; pIndex <= mVendors.length - 1; pIndex++) {
                mVendors[pIndex].ID = ReadyGuid(mVendors[pIndex].ID);
            }
            DisplayTable();

        };

        function DisplayInvoiceTypes(vData) {
            var pRows = vData.Rows;
            var pOption;
            var pDR;
            mSelAutoCreateInvoiceType.html('');
            for (var p = 0; p <= pRows.length - 1; p++) {
                pDR = pRows[p];
                if (pDR.DocumentName == 'STI')
                    continue;
                pOption = new Option(pDR.DocumentName, ReadyGuid(pDR.ID));
                mSelAutoCreateInvoiceType.append(pOption);    
            }
        }

        function ShowHideTable() {
            var pSelected = mElement.find('[name=radAutoCreateInvoiceCondition]:checked').val();
            pSelected = CStr2(pSelected).trim().toLowerCase();
            if (pSelected.indexOf('selected') == -1)
                mElement.find('[name=divAutoCreateInvoiceVendors]').hide();
            else
                mElement.find('[name=divAutoCreateInvoiceVendors]').show();
        }

        mElement.find('[name=radAutoCreateInvoiceCondition]').click(function() {
            ShowHideTable();
        });

        return mOut;
    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});