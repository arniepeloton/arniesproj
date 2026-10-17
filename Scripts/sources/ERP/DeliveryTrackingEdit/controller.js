define([
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/DeliveryTrackingEdit/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.DeliveryTrackingEdit.controller = ERP.DeliveryTrackingEdit.controller(); // singleton
                // mHtml.trigger('create');
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.DeliveryTrackingEdit.controller', null, function () {

        var mOut = {};
        var mElement;
        var mHasLoaded;
        var mOutgoingData;
        var mOnSave;

        mElement = $(mHtml);
        mElement.attr('id', 'popActivityTrackerEdit' + NewID());
        $('body').append(mElement);
        //mElement.trigger('create');
        mOut.element = mElement;
        mElement.find('[name=butSave]').click(SetDeliver);

        var listDeliveryChecker = mElement.find('[name="listDeliveryChecker"]');
        var listDeliveryDriver = mElement.find('[name="listDeliveryDriver"]');
        var listDeliveryHelper = mElement.find('[name="listDeliveryHelper"]');

        mOut.popup = function(vParams, details) {
            Clear();
            
            mOutgoingData.InvoiceIDs = vParams.InvoiceIDs;
            mOnSave = vParams.OnSave;

            if (!mHasLoaded) 
                //NewMessageBox('Loading. Please wait');

            if (!mHasLoaded) {
                mHasLoaded = true;
                NewMessageBoxClose();
            }

            try {
                mElement.modal('show');
                
            } catch (ex) {
                // just ignore the error
            }

            if (mOutgoingData.InvoiceIDs.length == 1) {
                mElement.find('[name=lblCustomerName]').text(details.CompanyName);
                mElement.find('[name=lblInvoiceDate]').text(details.InvoiceDate);
                mElement.find('[name=lblInvoiceNo]').text(details.ActualInvoiceNumber);
                mElement.find('[name=divInvoiceDetails]').show();
                GetDeliver();
            } else mElement.find('[name=divInvoiceDetails]').hide();
                
        };

        mOut.SetUndeliveredAgain = function (vParams) {
            Clear();

            mOutgoingData.Status = 'Undelivered';
            mOutgoingData.InvoiceIDs = vParams.InvoiceIDs;
            mOutgoingData.Reason = vParams.Reason;

            SetDeliver();
        };

        var mCbDeliveryMode = mElement.find('[name=cbDeliveryMode]');
        mCbDeliveryMode.click(mCbDeliveryMode_Click);

        function mCbDeliveryMode_Click() {
            HideDivs();
            mOutgoingData = {
                InvoiceIDs: mOutgoingData.InvoiceIDs
            }
            mOutgoingData.DeliveryMethod = mCbDeliveryMode.val();
            switch (mOutgoingData.DeliveryMethod) {
                case 'Inhouse Delivery':
                    mElement.find('[name=divInhouseDelivery]').show();
                    break;

                case '3rd Party Delivery':
                    mElement.find('[name=div3rdPartyDelivery]').show();
                    break;

                case 'Customer Pickup':
                    mElement.find('[name=divCustomerPickup]').show();
                    break;

                default:
                    
            }
        }

        function HideDivs() {
            mElement.find('[name=divInhouseDelivery]').hide();
            mElement.find('[name=div3rdPartyDelivery]').hide();
            mElement.find('[name=divCustomerPickup]').hide();

            mElement.find('input, textarea').val('');
        }

        mElement.find('[name=butSearchTruck], [name=txtTruck]').click(BrowseTruck);
     
        function BrowseTruck(vParams) {
            BrowsePopup('LibraryFowst', 'Select Truck',
                [
                    {
                        AutoSearch: true,
                        DisplayText: [
                            {
                                Column: 'FowstName'
                            }
                        ],
                        Click: function (e) {
                            mOutgoingData.DeliveryTruckFowstID = e.ID;
                            mElement.find('[name=txtTruck]').val(e.FowstName);
                        },
                        Close: function() {
                            mElement.modal('show');
                        }
                    }
                ], {
                    FowstTypeID: 'CE791943-9D80-42FC-BEBE-5BB52A5792E8'
                }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        }

        mElement.find('[name=butSearchCheckedBy], [name=txtCheckedBy]').click(BrowseCheckedBy);
      
        function BrowseCheckedBy() {
            BrowsePopup('LibraryOrganizationEmployee', 'Checked By',
                [
                    {
                        // Search: mTxtCanvass.val(),
                        AutoSearch: true,
                        SelectMultiple: true,
                        DisplayText: [
                            {
                                Column: 'PeopleName'
                            }
                        ],
                        Click: function (e) {
                            mOutgoingData.DeliveryChecker = $.merge(mOutgoingData.DeliveryChecker || [], e);

                            DisplayDeliveryDetails(listDeliveryChecker, mOutgoingData.DeliveryChecker, 'checker');

                            //mOutgoingData.DeliveryCheckedByID = e.ID;
                            //mElement.find('[name=txtCheckedBy]').val(e.PeopleName);
                        },
                        Close: function() {
                            mElement.modal('show');
                        }
                    }
                ], { 
                }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        }

        mElement.find('[name=butSearchDriver], [name=txtDriver]').click(BrowseDriver);
     
        function BrowseDriver() {
            BrowsePopup('LibraryOrganizationEmployee', 'Driver',
                [
                    {
                        // Search: mTxtCanvass.val(),
                        AutoSearch: true,
                        SelectMultiple: true,
                        DisplayText: [
                            {
                                Column: 'PeopleName'
                            }
                        ],
                        Click: function (e) {
                            mOutgoingData.DeliveryDriver = $.merge(mOutgoingData.DeliveryDriver || [], e);

                            DisplayDeliveryDetails(listDeliveryDriver, mOutgoingData.DeliveryDriver, 'driver');

                            //mOutgoingData.DeliveryDriverID = e.ID;
                            //mElement.find('[name=txtDriver]').val(e.PeopleName);
                        },
                        Close: function() {
                            mElement.modal('show');
                        }
                    }
                ], { 
                }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        }

        mElement.find('[name=butSearchHelper], [name=txtHelper]').click(BrowseHelper);
     
        function BrowseHelper() {
            BrowsePopup('LibraryOrganizationEmployee', 'Helper',
                [
                    {
                        // Search: mTxtCanvass.val(),
                        AutoSearch: true,
                        SelectMultiple: true,
                        DisplayText: [
                            {
                                Column: 'PeopleName'
                            }
                        ],
                        Click: function (e) {
                            mOutgoingData.DeliveryHelper = $.merge(mOutgoingData.DeliveryHelper || [], e);

                            DisplayDeliveryDetails(listDeliveryHelper, mOutgoingData.DeliveryHelper, 'helper');

                            //mOutgoingData.DeliveryHelperID = e.ID;
                            //mElement.find('[name=txtHelper]').val(e.PeopleName);
                        },
                        Close: function() {
                            mElement.modal('show');
                        }
                    }
                ], { 
                }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        }

        mOut.DisplayDeliveryDetails = DisplayDeliveryDetails
        function DisplayDeliveryDetails(container, data, designation, rsStatus) {
            for (var i in data) {
                data[i].Index = i;
                data[i].Designation = designation;

                if (container.find('li[empid="' + data[i].ID + '"]').length > 0) continue;

                var li = $('<li class="list-group-item">')
                    .attr('empid', data[i].ID)
                    .append($('<div class="row align-items-center">')
                        .append($('<div class="col">').text(data[i].PeopleName))
                        .append($('<div class="col-auto text-right">')
                            .append($('<button data-role="none" class="btn btn-danger">')
                                .data('data', data[i])
                                .append($('<span class="bi-trash">'))
                                .click(function () {
                                    var data = $(this).data('data');

                                    if (data.Designation == 'helper') {
                                        mOutgoingData.DeliveryHelper.splice(data.Index, 1);
                                        listDeliveryHelper.empty();
                                        DisplayDeliveryDetails(listDeliveryHelper, mOutgoingData.DeliveryHelper, 'helper');
                                    } else if (data.Designation == 'driver') {
                                        mOutgoingData.DeliveryDriver.splice(data.Index, 1);
                                        listDeliveryDriver.empty();
                                        DisplayDeliveryDetails(listDeliveryDriver, mOutgoingData.DeliveryDriver, 'driver');
                                    } else if (data.Designation == 'checker') {
                                        mOutgoingData.DeliveryChecker.splice(data.Index, 1);
                                        listDeliveryChecker.empty();
                                        DisplayDeliveryDetails(listDeliveryChecker, mOutgoingData.DeliveryChecker, 'checker');
                                    }
                                })
                            )
                        )
                )

                if (rsStatus == 'finalized') li.find('.col-auto').remove();

                container.append(li)
            }
        }

        mElement.find('[name=butSearch3plCompanyName], [name=txt3plCompanyName]').click(BrowseSupplier);

        function BrowseSupplier() {
            BrowsePopup('LibraryOrganizationSupplier', '3PL Company',
                [
                    {
                        // Search: mTxtCanvass.val(),
                        AutoSearch: true,
                        DisplayText: [
                            {
                                Column: 'SupplierName'
                            }
                        ],
                        Click: function (e) {
                            mOutgoingData.Delivery3PlCompanyID = e.ID;
                            mElement.find('[name=txt3plCompanyName]').val(e.SupplierName);
                        },
                        Close: function () {
                            mElement.modal('show');
                        }
                    }
                ], {
            }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        }

        function Clear() {
            HideDivs();
            mCbDeliveryMode.val('');
            listDeliveryChecker.empty();
            listDeliveryDriver.empty();
            listDeliveryHelper.empty();
            
            mOutgoingData = {
                InvoiceIDs: []
            };
        }

        mOut.Init = function() {
        };

        function SetDeliver() {
            if ($('[name="cbDelivered"]').val() == 'Undelivered') {
                mOutgoingData.Delivery3PlCompanyName = mElement.find('[name=txt3plCompanyName]').val();
                mOutgoingData.Delivery3PlContactPerson = mElement.find('[name=txt3plContactPerson]').val();
                mOutgoingData.DeliveryWaybillNumber = mElement.find('[name=txtWaybillNumber]').val();
                mOutgoingData.DeliveryCustomerRepresentative = mElement.find('[name=txtCustomerRepresentative]').val();
                mOutgoingData.DeliveryRemarks = mElement.find('[name=txtRemarks]').val().trim();
                mOutgoingData.IsNewRoutingSheet = true;
                mOutgoingData.Status = 'For Delivery';
            }
            
            mElement.modal('hide');

            AjaxSend({
                url: '/SalesMenu/SetDeliver',
                data: mOutgoingData,
                success: function(vData) {
                    if (vData.Result == 'ok') {
                        NewMessageBox('Data was successfully saved');
                        mOnSave();
                    }                        
                    else if (vData.Error) 
                        NewMessageBox(vData.Error);
                }
            });
        }

        function GetDeliver() {
            mElement.find('[name=butSave]').addClass('disabled');
            AjaxSend({
                url: '/SalesMenu/GetDeliver',
                data: {
                    ID: mOutgoingData.InvoiceIDs[0]
                },
                success: function (vData) {
                    if (vData.Error) 
                        NewMessageBox(vData.Error);
                    else
                        DisplayData(vData);
                }
            });
        }

        function DisplayData(vData) {
            var headerInfo = vData.data;
            mCbDeliveryMode.val(headerInfo.DeliveryMethod);         
            mCbDeliveryMode_Click();
            mElement.find('[name=txtEncodingDate]').val(headerInfo.DeliveryEncodingDate);

            // A. Inhouse Delivery
            mOutgoingData.DeliveryTruckFowstID = headerInfo.DeliveryTruckFowstID;
            mElement.find('[name=txtTruck]').val(headerInfo.DeliveryTruckFowstName);

            mOutgoingData.DeliveryCheckedByID = headerInfo.DeliveryCheckedByID;
            mElement.find('[name=txtCheckedBy]').val(headerInfo.DeliveryCheckedByName);

            mOutgoingData.DeliveryDriverID = headerInfo.DeliveryDriverID;
            mElement.find('[name=txtDriver]').val(headerInfo.DeliveryDriverName);

            mOutgoingData.DeliveryHelperID = headerInfo.DeliveryHelperID;
            mElement.find('[name=txtHelper]').val(headerInfo.DeliveryHelperName);
            
            // B. 3rd Party Delivery
            mOutgoingData.Delivery3PlCompanyID = CStr2(headerInfo.Delivery3PlCompanyID);
            mElement.find('[name=txt3plCompanyName]').val(headerInfo.Delivery3PlCompanyName);
            mElement.find('[name=txt3plContactPerson]').val(headerInfo.Delivery3PlContactPerson);
            mElement.find('[name=txtWaybillNumber]').val(headerInfo.DeliveryWaybillNumber);

            // C. Customer Pickup
            mElement.find('[name=txtCustomerRepresentative]').val(headerInfo.DeliveryCustomerRepresentative);

            mElement.find('[name=txtRemarks]').val(CStr2(headerInfo.DeliveryRemarks));
            if (CStr2(headerInfo.DeliveryEncodingDate).trim() == '')
                mElement.find('[name=butSave]').removeClass('disabled');

            mOutgoingData.DeliveryChecker = vData.Checker;
            DisplayDeliveryDetails(listDeliveryChecker, mOutgoingData.DeliveryChecker, 'checker');

            mOutgoingData.DeliveryDriver = vData.Driver;
            DisplayDeliveryDetails(listDeliveryDriver, mOutgoingData.DeliveryDriver, 'driver');

            mOutgoingData.DeliveryHelper = vData.Helper;
            DisplayDeliveryDetails(listDeliveryHelper, mOutgoingData.DeliveryHelper, 'helper');
        }

        mOut.Init();

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});