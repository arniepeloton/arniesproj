define([
], function () {

    var mDeferred = $.Deferred();
    var mHtml;
    var mHtmlSalesReportBySalesRepresentativeAndSupplier;
    var mHtmlSalesReportByCustomerAndSupplierProducts;
    var mHtmlSalesReportByProductQuantity;
    var mHtmlSalesReportByProductBaseQuantity;    
    var mHtmlSalesReportByProductQuantityPerMonth;
    var mHtmlInventoryMovementByFOWST;
    
    var mDownloadLink = '';

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/Reports/crossTab/view.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.Reports.crossTab.controller = new ERP.Reports.crossTab.controller();
                SalesReportBySalesRepresentativeAndSupplier();
            }
        });

        function SalesReportBySalesRepresentativeAndSupplier() {
            AjaxSend({
                ExactUrl: '/Scripts/sources/ERP/Reports/crossTab/salesreportbysalesrepresentativeandsupplier.html',
                type: 'GET',
                dataType: 'text',
                success: function (vDataSalesReportBySalesRepresentativeAndSupplier) {
                    mHtmlSalesReportBySalesRepresentativeAndSupplier = vDataSalesReportBySalesRepresentativeAndSupplier
                    SalesReportByCustomerAndSupplierProducts();
                }
            });
        }

        function SalesReportByCustomerAndSupplierProducts() {
            AjaxSend({
                ExactUrl: '/Scripts/sources/ERP/Reports/crossTab/salesreportbycustomerandsupplierproducts.html',
                type: 'GET',
                dataType: 'text',
                success: function (vDataSalesReportByCustomerAndSupplierProducts) {
                    mHtmlSalesReportByCustomerAndSupplierProducts = vDataSalesReportByCustomerAndSupplierProducts;
                    SalesReportByProductMeasurementQuantity();
                }
            });
        }

        function SalesReportByProductMeasurementQuantity() {
            AjaxSend({
                ExactUrl: '/Scripts/sources/ERP/Reports/crossTab/salesreportbyvendorandproductquantity.html',
                type: 'GET',
                dataType: 'text',
                success: function (vDataSalesReportByProductQuantity) {
                    mHtmlSalesReportByProductQuantity = vDataSalesReportByProductQuantity;
                    SalesReportByProductQuantityPerMonth();
                }
            });
        }

        function SalesReportByProductQuantityPerMonth() {
            AjaxSend({
                ExactUrl: '/Scripts/sources/ERP/Reports/crossTab/salesreportbyproductquantitypermonth.html',
                type: 'GET',
                dataType: 'text',
                success: function (vDataSalesReportByProductQuantityPerMonth) {
                    mHtmlSalesReportByProductQuantityPerMonth = vDataSalesReportByProductQuantityPerMonth;
                    SalesReportByProductBaseQuantity();                    
                }
            });
        }

        function SalesReportByProductBaseQuantity() {
            AjaxSend({
                ExactUrl: '/Scripts/sources/ERP/Reports/crossTab/salesreportbyvendorandproductbasequantity.html',
                type: 'GET',
                dataType: 'text',
                success: function (vDataSalesReportByProductBaseQuantity) {
                    mHtmlSalesReportByProductBaseQuantity = vDataSalesReportByProductBaseQuantity;
                    InventoryMovementByFOWST();
                }
            });
        }

        function InventoryMovementByFOWST() {
            AjaxSend({
                ExactUrl: '/Scripts/sources/ERP/Reports/crossTab/inventorymovementbyFOWST.html',
                type: 'GET',
                dataType: 'text',
                success: function (vDataInventoryMovementByFOWST) {
                    mHtmlInventoryMovementByFOWST = vDataInventoryMovementByFOWST;
                    mDeferred.resolve();
                }
            });
        }
    })();

    CreateStringObject('ERP.Reports.crossTab.controller', null, function () {

        var mOut = {};
        var mElement;
        var mElementSalesReportByCustomerAndSupplierProducts;
        var mElementSalesReportByProductQuantity;
        var mElementSalesReportByProductQuantityPerMonth;        
        var mElementSalesReportByProductBaseQuantity;
        var mElementInventoryMovementByFOWST;
        
        mElement = $("");
        //mOut.element = mElement;
        //var mTable = mElement.find('[name=tableReports]');
        var mDataTable;
        var mDateFrom = '', mDateTo = '';
        var mCustomerID = '';

        var mVendorID = '';
        var mFOWSTID = '';

        //var mDateNeededDateFrom, mDateNeededDateTo;
        //var mCbX = mElement.find('[name=cbXAxis]');
        //var mCbY = mElement.find('[name=cbYAxis]');
        //var mDateType;
        //var mSalesFigure;

        //$('#tabReportsWork').append(mElement);//.trigger('create');        

        //(function Init() {
        //    //var pAdjustments = $('body');
        //    var pAdjustments = $.mobile.activePage.find('#tabRevenueDashboard');
        //    if (pAdjustments.length)
        //        //mElement.insertBefore(pAdjustments);
        //        pAdjustments.append(mElement);
        //    else
        //        $.mobile.activePage.append(mElement);
        //    PopuplateCombobox(mCbX);
        //    PopuplateCombobox(mCbY); 
        //})();

        mOut.Show = function (vParam) {
            //mElement.popup();
            //mElement.popup('open');
            
            if (vParam == 'SalesReportBySalesRepresentativeAndSupplier') {
                mElement = $(mHtmlSalesReportBySalesRepresentativeAndSupplier);
                mOut.element = mElement;
                $('#tabReportsWork').append(mElement);//.trigger('create');      
                $.mobile.activePage.append(mElement);

                mElement.show();
            } else if (vParam == 'SalesReportByCustomerAndSupplierProducts') {
                mElementSalesReportByCustomerAndSupplierProducts = $(mHtmlSalesReportByCustomerAndSupplierProducts);
                mOut.element = mElementSalesReportByCustomerAndSupplierProducts;
                $('#tabReportsWork').append(mElementSalesReportByCustomerAndSupplierProducts);//.trigger('create');      
                $.mobile.activePage.append(mElementSalesReportByCustomerAndSupplierProducts);

                mElementSalesReportByCustomerAndSupplierProducts.show();

                mElementSalesReportByCustomerAndSupplierProductsInit();
            } else if (vParam == 'SalesReportByProductMeasurementQuantity') {
                mElementSalesReportByProductQuantity = $(mHtmlSalesReportByProductQuantity);
                mOut.element = mElementSalesReportByProductQuantity;
                $('#tabReportsWork').append(mElementSalesReportByProductQuantity);//.trigger('create');      
                $.mobile.activePage.append(mElementSalesReportByProductQuantity);

                mElementSalesReportByProductQuantity.show();

                mElementSalesReportByProductQuantityInit();
            } else if (vParam == 'SalesReportByProductQuantityPerMonth') {
                mElementSalesReportByProductQuantityPerMonth = $(mHtmlSalesReportByProductQuantityPerMonth);
                mOut.element = mElementSalesReportByProductQuantityPerMonth;
                $('#tabReportsWork').append(mElementSalesReportByProductQuantityPerMonth);//.trigger('create');      
                $.mobile.activePage.append(mElementSalesReportByProductQuantityPerMonth);

                mElementSalesReportByProductQuantityPerMonth.show();

                mElementSalesReportByProductQuantityPerMonthInit();                
            } else if (vParam == 'SalesReportByProductBaseQuantity') {
                mElementSalesReportByProductBaseQuantity = $(mHtmlSalesReportByProductBaseQuantity);
                mOut.element = mElementSalesReportByProductBaseQuantity;
                $('#tabReportsWork').append(mElementSalesReportByProductBaseQuantity);//.trigger('create');      
                $.mobile.activePage.append(mElementSalesReportByProductBaseQuantity);

                mElementSalesReportByProductBaseQuantity.show();

                mElementSalesReportByProductBaseQuantityInit();
            } else if (vParam == 'InventoryMovementByFOWST') {
                mElementInventoryMovementByFOWST = $(mHtmlInventoryMovementByFOWST);
                mOut.element = mElementInventoryMovementByFOWST;
                $('#tabReportsWork').append(mElementInventoryMovementByFOWST);//.trigger('create');      
                $.mobile.activePage.append(mElementInventoryMovementByFOWST);

                mElementInventoryMovementByFOWST.show();

                mElementInventoryMovementByFOWSTInit();
            }

            
        };

        function PopuplateCombobox(vCombobox) {
            vCombobox.append(new Option('Encoded By', 'Encoder'));
            vCombobox.append(new Option('Sales Rep', 'SalesRep'));
            vCombobox.append(new Option('Vendor', 'Vendor'));
            //vCombobox.append(new Option('Brand', 'vendor'));
        }

        function LoadData(vParams) {
            vParams = $.extend({
                Download: false
            }, vParams);
            var pData = {
                XAxis: mCbX.val(),
                YAxis: mCbY.val()
            };
            if (mDateType == 'dateneeded') {
                pData.DateNeededFrom = mDateFrom;
                pData.DateNeededTo = mDateTo;
            } else if (mDateType == 'dateencoded') {
                pData.DateEncodedFrom = mDateFrom;
                pData.DateEncodedTo = mDateTo;
            } else if (mDateType == 'invoicedate') {
                pData.DateFrom = mDateFrom;
                pData.DateTo = mDateTo;
            } else {
                NewMessageBox('Please choose the type of date first.');
                return;
            }
            pData.ShowProducts = CBool(mElement.find('[name=chkShowProducts]').prop('checked'));
            if (pData.ShowProducts
                && !vParams.Download) {

                NewMessageBox('Show Products is only available as a spreadsheet download.');
                return;
            }
            pData.SalesFigure = mSalesFigure;

            if (vParams.Download)
                pData.Download = true;
            ClearTable();
            AjaxSend({
                url: '/Browser/CrossTabReport',
                data: pData,
                success: function(vData) {
                    if (vData.Error) {
                        NewMessageBox(vData.Error);
                        return;
                    }
                    if (vParams.Download) {
                        var pUrl = MergeUrlParameters({
                            Key: vData.Key
                        }, '/Browser/DownloadCrossTabReport');
                        window.open(pUrl, '_blank', 
                            'width=' + window.innerWidth + 
                            ',height=' + window.innerHeight + 
                            ',scrollbars=auto');
                        NewMessageBox('You should be viewing/downloading the spreadsheet by now. If you are unable to view/download the spreadsheet, please check your web browser\'s popup settings.');

                    } 
                    DisplayTable(vData);
                }
            });
        } // END function LoadData() {

        mElement.find('[name=butRefresh]').click(function() {
            LoadData();
        }); // END mElement.find('#butDepositsRefresh').click(function() {

        mElement.find('[name=butDownload]').click(function() {
            LoadData({
                Download: true
            });
        }); // END mElement.find('#butDepositsRefresh').click(function() {

        function ClearTable() {
            if ($.fn.DataTable.isDataTable(mTable)) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                mTable.empty();
                mDataTable.destroy();
            }
        }

        function DisplayTable(vData) {
            var pColumns = [];
            var pYAxisColumnName = vData.ColumnsArray[0];
            mTable.html('');
            var pTD;
            var pTR = $('<tr>');
            for (var p  = 0; p <= vData.ColumnsArray.length - 1; p++) {
                var pColumn = vData.ColumnsArray[p];
                pTD = $('<th>');
                pTD.text(pColumn);
                pTR.append(pTD);

                (function(vColumnName, vIsNumeric) {
                    pColumns.push({
                        data: function(vData2) {
                            if (vIsNumeric)
                                return AccountingFormat(vData2[vColumnName]);
                            else
                                return vData2[vColumnName] + '';
                        },
                        render: function(v1, v2, vData) {
                            var pText;
                            if (vIsNumeric)
                                pText = AccountingFormat(vData[vColumnName]);
                            else
                                pText = vData[vColumnName] + '';
                            if (CDec2(pText) != 0)
                                return '<a href="#" name="cell", columnname="' + HtmlEncode(vColumnName) + '" yaxisvalue="' + HtmlEncode(vData[pYAxisColumnName]) + '">' + HtmlEncode(pText) + '</a>';
                            else
                                return pText;
                        }
                    });
                })(pColumn, p != 0);
            }
            var pTHead = $('<thead>');
            pTHead.append(pTR);
            mTable.append(pTHead);

            mDataTable = mTable.DataTable({
                data: vData.Rows,
                columns: pColumns
            });

            mTable.find('[name=cell]').click(function(e) {
                e.preventDefault();
                Find({
                    TransType: 'Invoice',
                    XAxisValue: $(this).attr('columnname'),
                    YAxisValue: $(this).attr('yaxisvalue'),

                    XAxisName: mCbX.val(),
                    YAxisName: mCbY.val()
                });
            });
        }

        function mElementSalesReportByProductQuantityInit() {

            $('#butSalesReportByProductQuantityDateRange').click(function () {
                DualCalendar({
                    Format: 'yyyy-MM-dd',
                    Click: function (e) {
                        mDateFrom = e.From;
                        mDateTo = e.To;
                        $('#butSalesReportByProductQuantityDateRange').text(FormatDate(e.From, { Format: 'MMM d, yyyy' }) + ' to ' + FormatDate(e.To, { Format: 'MMM d, yyyy' }));
                    }
                });
            });

            $('#butSalesReportByProductQuantityVendor').click(function () {
                BrowsePopup('LibraryVendor', 'Select Vendor', [
                  {
                      DisplayText: [{ Column: 'CompanyName' }],
                      Search: $.trim($(this).val()),
                      Click: function (e) {
                          mVendorID = e.ID;
                          $('#butSalesReportByProductQuantityVendor').text(e.CompanyName);
                      },
                      Close: function () {
                          $('#popupAssetNewCustomer').show()
                      }
                  }
                ]);
            });

            $('#butDownloadSalesReportByProductQuantity').click(function () {
                var mVendorName = $('#butSalesReportByProductQuantityVendor').text();

                //mVendorID = '0E5CCAA1-C4F9-49FD-AEFC-F3814C366558' //temporary since wala pay browser for vendor/supplier

                if (mVendorID  == '') {
                    NewMessageBox('Please Select Vendor/Supplier');
                    return;
                }

                if (mDateFrom == '') {
                    NewMessageBox('Please Select Date From');
                    return;
                }

                if (mDateTo == '') {
                    NewMessageBox('Please Select Date To');
                    return;
                }

                mDownloadLink = '';

                pData = {
                    VendorID: mVendorID,
                    VendorName: mVendorName,
                    DateFrom: mDateFrom,
                    DateTo: mDateTo,
                }

                AjaxSend({
                    url: '/Revenue/SalesReportByProductMeasurementQuantity',
                    data: pData,
                    success: function (vData) {
                        if (vData.Error) {
                            NewMessageBox(vData.Error);
                            return;
                        } else {
                            mDownloadLink = vData.Report;
                            var pUrl = MergeUrlParameters({
                                Key: mDownloadLink,
                                OrgID: sessionStorage.OrgID
                            }, '/Revenue/DownloadReport');

                            window.open(pUrl, '_blank',
                            'width=' + window.innerWidth +
                            ',height=' + window.innerHeight +
                            ',scrollbars=auto');

                            //DisplayTable(vData);
                        }
                    }
                });
            });
          
        }

        function mElementSalesReportByCustomerAndSupplierProductsInit() {

            $('#butSalesReportByCustomerAndSupplierProductsDateRange').click(function () {
                DualCalendar({
                    Format: 'yyyy-MM-dd',
                    Click: function (e) {
                        mDateFrom = e.From;
                        mDateTo = e.To;
                        $('#butSalesReportByCustomerAndSupplierProductsDateRange').text(FormatDate(e.From, { Format: 'MMM d, yyyy' }) + ' to ' + FormatDate(e.To, { Format: 'MMM d, yyyy' }));
                    }
                });
            })

            $('#butSalesReportByCustomerAndSupplierProductsCustomer').click(function () {
                BrowsePopup('LibraryOrganizationCustomer', 'Select Customer', [
               {
                   DisplayText: [{ Column: 'CustomerName' }],
                   Search: $.trim($(this).val()),
                   Click: function (e) {
                       mCustomerID = e.ID;
                       $('#butSalesReportByCustomerAndSupplierProductsCustomer').text(e.CustomerName);                      
                   },
                   Close: function () {
                       $('#popupAssetNewCustomer').show()
                   }
               }
                ], { FilterBy: 'Org' })
            })

            $('#butDownloadSalesReportByCustomerAndSupplierProductsCustomer').click(function () {
                var mCustomerName = $('#butSalesReportByCustomerAndSupplierProductsCustomer').text();
                var DateReference = $('#selSalesReportByCustomerAndSupplierProductsCustomer').val();

                if (mCustomerID == '') {
                    NewMessageBox('Please Select Customer');
                    return;
                }

                if (mDateFrom == '') {
                    NewMessageBox('Please Select Date From');
                    return;
                }

                if (mDateTo == '') {
                    NewMessageBox('Please Select Date To');
                    return;
                }

                if (DateReference == 'selSalesReportByCustomerAndSupplierProductsCustomerDateNeeded') {
                    DateReference = 'TransInvoice.DateNeeded'
                } else if (DateReference == 'selSalesReportByCustomerAndSupplierProductsCustomerEncodedDate') {
                    DateReference = 'TransInvoice.LastUpdated';
                } else {
                    DateReference = 'TransInvoice.InvoiceDate'
                }

                mDownloadLink = '';

                pData = {
                    CustomerID: mCustomerID,
                    CustomerName: mCustomerName,
                    DateFrom: mDateFrom,
                    DateTo: mDateTo,
                    DateReference: DateReference
                }

                AjaxSend({
                    url: '/Revenue/SalesReportByCustomerAndSupplierProducts',
                    data: pData,
                    success: function (vData) {
                        if (vData.Error) {
                            NewMessageBox(vData.Error);
                            return;
                        } else {
                            mDownloadLink = vData.Report;
                            var pUrl = MergeUrlParameters({
                                Key: mDownloadLink,
                                OrgID: sessionStorage.OrgID
                            }, '/Revenue/DownloadReport');

                            window.open(pUrl, '_blank',
                            'width=' + window.innerWidth +
                            ',height=' + window.innerHeight +
                            ',scrollbars=auto');

                            //DisplayTable(vData);
                        }
                    }
                });
            });
        }        

        function mElementSalesReportByProductBaseQuantityInit() {

            $('#butSalesReportByProductBaseQuantityDateRange').click(function () {
                DualCalendar({
                    Format: 'yyyy-MM-dd',
                    Click: function (e) {
                        mDateFrom = e.From;
                        mDateTo = e.To;
                        $('#butSalesReportByProductBaseQuantityDateRange').text(FormatDate(e.From, { Format: 'MMM d, yyyy' }) + ' to ' + FormatDate(e.To, { Format: 'MMM d, yyyy' }));
                    }
                });
            });

            $('#butSalesReportByProductBaseQuantityVendor').click(function () {
                BrowsePopup('LibraryVendor', 'Select Vendor', [
                  {
                      DisplayText: [{ Column: 'CompanyName' }],
                      Search: $.trim($(this).val()),
                      Click: function (e) {
                          mVendorID = e.ID;
                          $('#butSalesReportByProductQuantityVendor').text(e.CompanyName);
                      },
                      Close: function () {
                          $('#popupAssetNewCustomer').show()
                      }
                  }
                ]);
            });

            $('#butDownloadSalesReportByProductBaseQuantity').click(function () {
                var mVendorName = $('#butSalesReportByProductQuantityVendor').text();

                //mVendorID = '0E5CCAA1-C4F9-49FD-AEFC-F3814C366558' //temporary since wala pay browser for vendor/supplier

                if (mVendorID == '') {
                    NewMessageBox('Please Select Vendor/Supplier');
                    return;
                }

                if (mDateFrom == '') {
                    NewMessageBox('Please Select Date From');
                    return;
                }

                if (mDateTo == '') {
                    NewMessageBox('Please Select Date To');
                    return;
                }

                mDownloadLink = '';

                pData = {
                    VendorID: mVendorID,
                    VendorName: mVendorName,
                    DateFrom: mDateFrom,
                    DateTo: mDateTo,
                }

                AjaxSend({
                    url: '/Revenue/SalesReportByProductBaseQuantity',
                    data: pData,
                    success: function (vData) {
                        if (vData.Error) {
                            NewMessageBox(vData.Error);
                            return;
                        } else {
                            mDownloadLink = vData.Report;
                            var pUrl = MergeUrlParameters({
                                Key: mDownloadLink,
                                OrgID: sessionStorage.OrgID
                            }, '/Revenue/DownloadReport');

                            window.open(pUrl, '_blank',
                            'width=' + window.innerWidth +
                            ',height=' + window.innerHeight +
                            ',scrollbars=auto');

                            //DisplayTable(vData);
                        }
                    }
                });
            });

        }

        function mElementInventoryMovementByFOWSTInit() {

            $('#butInventoryMovementByFOWSTDateRange').click(function () {
                DualCalendar({
                    Format: 'yyyy-MM-dd',
                    Click: function (e) {
                        mDateFrom = e.From;
                        mDateTo = e.To;
                        $('#butInventoryMovementByFOWSTDateRange').text(FormatDate(e.From, { Format: 'MMM d, yyyy' }) + ' to ' + FormatDate(e.To, { Format: 'MMM d, yyyy' }));
                    }
                });
            });

            $('#butInventoryMovementByFOWSTFOWST').click(function () {
                BrowsePopup('LibraryFOWST', 'Select FOWST', [
                  {
                      DisplayText: [{ Column: 'FowstName' }],
                      Search: $.trim($(this).val()),
                      Click: function (e) {
                          mFOWSTID = e.ID;
                          $('#butInventoryMovementByFOWSTFOWST').text(e.FowstName);
                      },
                      Close: function () {
                          $('#popupAssetNewCustomer').show()
                      }
                  }
                ]);
            });

            $('#butDownloadInventoryMovementByFOWST').click(function () {
                var mFOWSTName = $('#butInventoryMovementByFOWST').text();

                //mVendorID = '0E5CCAA1-C4F9-49FD-AEFC-F3814C366558' //temporary since wala pay browser for vendor/supplier

                if (mFOWSTID == '') {
                    NewMessageBox('Please Select FOWST');
                    return;
                }

                if (mDateFrom == '') {
                    NewMessageBox('Please Select Date From');
                    return;
                }

                if (mDateTo == '') {
                    NewMessageBox('Please Select Date To');
                    return;
                }

                mDownloadLink = '';

                pData = {
                    FOWSTID: mFOWSTID,
                    FOWSTName: mFOWSTName,
                    DateFrom: mDateFrom,
                    DateTo: mDateTo,
                }

                AjaxSend({
                    url: '/Revenue/InventoryMovementByFOWST',
                    data: pData,
                    success: function (vData) {
                        if (vData.Error) {
                            NewMessageBox(vData.Error);
                            return;
                        } else {
                            mDownloadLink = vData.Report;
                            var pUrl = MergeUrlParameters({
                                Key: mDownloadLink,
                                OrgID: sessionStorage.OrgID
                            }, '/Revenue/DownloadReport');

                            window.open(pUrl, '_blank',
                            'width=' + window.innerWidth +
                            ',height=' + window.innerHeight +
                            ',scrollbars=auto');

                            //DisplayTable(vData);
                        }
                    }
                });
            });

        }        

        function mElementSalesReportByProductQuantityPerMonthInit() {
            $('#butMonthlyProductSalesReportDateRange').click(function () {
                DualCalendar({
                    Format: 'yyyy-MM-dd',
                    Click: function (e) {
                        mDateFrom = e.From;
                        mDateTo = e.To;
                        $('#butMonthlyProductSalesReportDateRange').text(FormatDate(e.From, { Format: 'MMM d, yyyy' }) + ' to ' + FormatDate(e.To, { Format: 'MMM d, yyyy' }));
                    }
                });
            })


            $('#butDownloadMonthlyProductSalesReport').click(function () {
                
                var DateReference = $('#selMonthlyProductSalesReport').val();
                
                if (mDateFrom == '') {
                    NewMessageBox('Please Select Date From');
                    return;
                }

                if (mDateTo == '') {
                    NewMessageBox('Please Select Date To');
                    return;
                }

                if (DateReference == 'selMonthlyProductSalesReportDateNeeded') {
                    DateReference = 'TransInvoice.DateNeeded'
                } else if (DateReference == 'selMonthlyProductSalesReportEncodedDate') {
                    DateReference
                } else {
                    DateReference = 'TransInvoice.InvoiceDate'
                }

                mDownloadLink = '';

                pData = {
                    DateFrom: mDateFrom,
                    DateTo: mDateTo,
                    DateReference: DateReference
                }

                AjaxSend({
                    url: '/Revenue/SalesReportByProductQuantityPerMonth',
                    data: pData,
                    success: function (vData) {
                        if (vData.Error) {
                            NewMessageBox(vData.Error);
                            return;
                        } else {
                            mDownloadLink = vData.Report;
                            var pUrl = MergeUrlParameters({
                                Key: mDownloadLink,
                                OrgID: sessionStorage.OrgID
                            }, '/Revenue/DownloadReport');

                            window.open(pUrl, '_blank',
                            'width=' + window.innerWidth +
                            ',height=' + window.innerHeight +
                            ',scrollbars=auto');

                            //DisplayTable(vData);
                        }
                    }
                });
            });
        }

        mElement.find('[name=butDateRange]').click(function() {
            DualCalendar({
                Format: 'yyyy-MM-dd',
                Click: function (e) {
                    mDateFrom = e.From;
                    mDateTo = e.To;
                    mElement.find('[name=butDateRange]').text(FormatDate(e.From, {Format: 'MMM d, yyyy'}) + ' to ' + FormatDate(e.To, {Format: 'MMM d, yyyy'}));
                }
            });
        });

        mElement.find('[name=butParameters]').click(function() {
            try {
                $('#popDateType').trigger('create').popup();
                $('#popDateType').popup('open');
            } catch (e) {
            }
        });
        
        mElement.find('[name=butOkParameter]').click(function() {
            try {
                var pDateType = mElement.find('[name=radCrossReportDateType]:checked').val();
                if (CStr2(pDateType) == '') {
                    NewMessageBox('Please choose the type of date first.');
                    return;
                }
                mDateType = pDateType;

                var pSalesFigure = mElement.find('[name=radCrossTabSalesFigure]:checked').val();
                if (CStr2(pSalesFigure) == '') {
                    NewMessageBox('Please choose the type of sales figure first.');
                    return;
                }
                mSalesFigure = pSalesFigure;

                $('#popDateType').popup('close');
            } catch (e) {
            }
        });

        function Find(vParams) {
            var pTitle;
            var pColumns = [];

            if (vParams.TransType == 'Quote') {
                pTitle = 'Quotations';
                pColumns = [
                    {
                        Label: 'Quotation No.',
                        Column: 'QuotationNo'
                    },
                    {
                        Label: 'Customer',
                        Column: 'CompanyName'
                    },
                    {
                        Label: 'Amount',
                        Column: function(vDR) {
                            return AccountingFormat(vDR.Total);
                        }
                    }
                ];
            } else if (vParams.TransType == 'SalesOrder') {
                pTitle = 'Sales Order';
                pColumns = [
                    {
                        Label: 'Sales Order No.',
                        Column: function(vDR) {
                            var pOut = CStr2(vDR.MIN).trim();
                            if (pOut != '')
                                pOut = vDR.MIN + '-';
                            pOut = 'SO-' + pOut + vDR.SalesOrderNo;
                            
                            return pOut;
                        }
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
                            return AccountingFormat(vDR.OrderedAmount);
                        }
                    },
                    {
                        Label: 'Served Amount',
                        Column: function(vDR) {
                            return AccountingFormat(vDR.ServedAmount);
                        }
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
                            return AccountingFormat(vDR.OrderedAmount);
                        }
                    },
                    {
                        Label: 'Served Amount',
                        Column: function(vDR) {
                            return AccountingFormat(vDR.ServedAmount);
                        }
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
                        }
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
                }
            ]);

            if ((vParams.TransType == 'Invoice')
                || (vParams.TransType == 'SalesOrder')){
                pColumns = pColumns.concat([
                    {
                        Label: 'Action',
                        Column: function () {
                            return '<div name="divAction" />';
                        }
                    }
                ]);
            }

            if (vParams.Click == null)
                vParams.Click = function (e) {
                    if (CBool(vParams.DontLoadCancelled))
                        if (CBool(e.IsCancelled)) {
                            NewMessageBox('The selected transaction was already cancelled. Please select a transaction that is still active.');
                            return;
                        }
                    if ((mOut.TransType == 'Invoice')
                        && (vParams.TransType == 'SalesOrder')) {

                        if ((CStr2(e.CnCUnapprovedReason).trim() != '')
                            && (CStr2(e.CncApprovedByID).trim() == '')) {
                            NewMessageBox('This transaction is awaiting approval from credit and collection and may not prepared and/or invoiced prior to said approval.');
                            return;
                        }

                        if (CStr2(e.FinalizePickListByID).trim() == '') {
                            NewMessageBox('This sales order cannot be invoiced at this time. It\'s pick list must be finalized via Fulfillment Center first.');
                            return;
                        }
                    }

                    var win = window.open(location.origin + location.pathname + location.search + '&IsReadOnly=1',
                        '_blank',
                        'width=' + window.innerWidth + ',height=' + window.innerHeight);
                    e.DontClose = true;
                    win.onload = function () {
                        function LoadRecord() {
                            if (win.Z9Q3C9ASDA)
                                win.Z9Q3C9ASDA.ShowInvoice({
                                    ID: e.ID,
                                    ReadOnly: false,
                                    ShowProductSuppliersColumn: true
                                });
                            else
                                setTimeout(LoadRecord, 1000);
                        }
                        LoadRecord();    
                    }
                        
                    return false; // para DILI magclose ang popup
                }; // END Click: function (e) {

            BrowsePopup('TransInvoices', pTitle, 
                [
                    {
                        StyleFormat: 'table',
                        AutoSearch: true,
                        DisplayText: pColumns,
                        Click: vParams.Click,
                        Validation: vParams.Validation,
                        rowCallback: function(row, data, dataIndex) {
                            var pCell = $(row).find('[name=divAction]');
                            if (pCell.length == 0)
                                return;
                            var pLink = $('<a>')
                                .text('Preview')
                                .attr('href', '#')
                                .click(function(e) {
                                    e.preventDefault();
                                    var win = window.open(location.origin + location.pathname + location.search + '&IsReadOnly=1',
                                        '_blank',
                                        'width=' + window.innerWidth + ',height=' + window.innerHeight);

                                    win.onload = function () {
                                        function LoadRecord() {
                                            if (win.Z9Q3C9ASDA)
                                                win.Z9Q3C9ASDA.ShowInvoice({
                                                    ID: data.ID,
                                                    ReadOnly: true,
                                                    ShowProductSuppliersColumn: true
                                                });
                                            else
                                                setTimeout(LoadRecord, 1000);
                                        }
                                        LoadRecord();    
                                    }
                        
                                    return false; // para DILI magclose ang popup
                                });
                            pCell.append(pLink);
                        },
                        Sorting: false,
                        //OrderBy: [1, 'asc']
                    }
                ], $.extend({ 
                    TransType: vParams.TransType,
                    AnyZero: 1,
                    AllowNoItems: true,
                    XAxisName: vParams.XAxisName,
                    XAxisValue: vParams.XAxisValue,
                    YAxisName: vParams.YAxisName,
                    YAxisValue: vParams.YAxisValue
                }, vParams.QueryParams)
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        } // END function Find(vParams) {

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

}); 