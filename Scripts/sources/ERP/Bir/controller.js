define([
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/Bir/view.html?crc=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.Bir = ERP.Bir.controller(); // singleton
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.Bir.controller', null, function () {

        var mOut = {};
        var mElement;
        var mPurchasing

        mElement = $(mHtml);
        $.mobile.activePage.append(mElement);
        mOut.element = mElement;
        var mInvoiceTypes;

        var mCbInvoiceType = mElement.find('[name=cbInvoiceType]');
        var mTable = mElement.find('[name=tableData]');
        var mDataTable;

        mOut.Popup = function(vPurchasing) {
            mPurchasing = vPurchasing;
            InitInvoiceTypes();
            $('#popupBir').modal('show');
        };

        mElement.find('[name=butRefresh]').click(function() {
            LoadData();
        });

        var mTxtCustomer = mElement.find('[name=txtCustomer]');
        mTxtCustomer.on('keypress', SearchCustomer); // END mTxtCustomer.on('keypress', function(e) {
        mTxtCustomer.click(function() {
            SearchCustomer({
                key: 'Enter'
            });
        });
        mElement.find('[name=butCustomer]').click(function() {
            SearchCustomer({
                key: 'Enter'
            });
        });

        var mCustomerID;
        function SearchCustomer(e) {

            if (e.key == 'Enter') {
                DoBrowsePopup();
            } // END if (e.key == 'Enter') {

            function DoBrowsePopup() {
                var pTarget;
                var pOptions;

                pTarget = 'Customer';
                pOptions = { 
                    FilterCustomers: 1
                };
                BrowsePopup('LibraryOrganization', pTarget,
                    [
                        {
                            // Search: mTxtCustomer.val(), // commented to prevent auto search upon popup
                            DisplayText: [{ Column: 'CompanyName' }],
                            Click: function (e) {
                                mTxtCustomer.val(e.CompanyName);
                                mCustomerID = e.ID;
                            }
                        }
                    ], pOptions
                ); // END BrowsePopup('LibraryFowst', 'Fowst',
            }
        } // END function SearchCustomer() {

        function InitInvoiceTypes() {   
            if (mPurchasing) {
                mCbInvoiceType.html('');
                pOption = new Option('Purchase Orders', 'po');
                mCbInvoiceType.append(pOption);    
                return;
            }
            if (mInvoiceTypes == null) 
                AjaxSend({
                    url: '/Browser/LibraryDocumentControl',
                    data: {
                        ModuleSource: 'Revenue > Invoicing'
                    },
                    success: function(vData) {  
                        mInvoiceTypes = vData.Results.Rows;
                        DisplayInvoiceTypes();
                    }
                });
            else
                DisplayInvoiceTypes();
            
            function DisplayInvoiceTypes() {
                var pDivInvoiceTypes = mElement.find('[name=divInvoiceTypes]');
                var pDivInvoiceType = mElement.find('[name=divInvoiceType]');
                pDivInvoiceTypes.html('');
                for (var p = 0; p <= mInvoiceTypes.length - 1; p++) {
                    var pDR = mInvoiceTypes[p];
                    var pClone = pDivInvoiceType.clone();
                    var pID = 'chkBirInvoiceType-' + pDR.ID;
                    pClone.find('label')
                        .text(pDR.DocumentName)
                        .attr('for', pID);
                    pClone.find('input[type=checkbox]')
                        .val(pDR.ID)
                        .attr('id', pID);
                    pClone.show();
                    pDivInvoiceTypes.append(pClone);
                }
            }
        }

        var mInvoiceDateFrom = new Date();
        var mInvoiceDateTo = new Date();
        mElement.find('[name=butDateRange]')
            .text(FormatDate(new Date(), {
                    Format: 'MMM dd, yyyy'
                }) + ' to ' +
                FormatDate(new Date(), {
                    Format: 'MMM dd, yyyy'
                }))
            .click(function () {
                DualCalendar({
                    Click: function (e) {
                        mElement.find('[name=butDateRange]').text(FormatDate(e.From, {
                                Format: 'MMM dd, yyyy'
                            }) + ' to ' +
                            FormatDate(e.To, {
                                Format: 'MMM dd, yyyy'
                            }));
                        mInvoiceDateFrom = e.From;
                        mInvoiceDateTo = e.To;
                    }
                }, new Date(), new Date());    
            }
        );

        function LoadData(vParams) {
            vParams = $.extend({
                DateFrom: new Date(),
                DateTo: new Date()
            }, vParams)

            var pColumns = [
                {
                    title: 'Product Number',
                    render: function(v1, v2, vData) {
                        return CStr2(vData.ProductNo);
                    }
                },
                {
                    title: 'Product Name / Description',
                    render: function(v1, v2, vData) {
                        return CStr2(vData.ProductName);
                    }
                },
                {
                    title: 'Brand',
                    render: function(v1, v2, vData) {
                        return CStr2(vData.Brand);
                    }
                },
                {
                    title: 'Category',
                    render: function(v1, v2, vData) {
                        return CStr2(vData.Category);
                    }
                },
                {
                    title: 'Average Selling Price (in base unit)',
                    render: function(v1, v2, vData) {
                        return AccountingFormat(vData.UnitPrice);
                    }
                },
                {
                    title: 'Average COGS (in base unit)',
                    render: function(v1, v2, vData) {
                        return AccountingFormat(vData.AvgCogs);
                    }
                },
                {
                    title: 'Average Unit Profit',
                    render: function(v1, v2, vData) {
                        return AccountingFormat(vData.AvgGP);
                    }
                },
                {
                    title: 'Average Unit Profit Margin',
                    render: function(v1, v2, vData) {
                        return AccountingFormat(vData.GPM);
                    }
                },
                {
                    title: 'Total Units Sold (in base UoM)',
                    render: function(v1, v2, vData) {
                        return AccountingFormat(vData.UnitsSold, 0);
                    }
                },
                {
                    name: 'SubTotal',
                    title: 'Total Sales',
                    render: function(v1, v2, vData) {
                        return AccountingFormat(vData.SubTotal);
                    }
                },
                {
                    name: 'ContributionToSales',
                    title: 'Contribution to Sales',
                    render: function(v1, v2, vData) {
                        return AccountingFormat(vData.ContributionToSales) + '%';
                    }
                },
                {
                    name: 'Cogs',
                    title: 'Total COGS',
                    render: function(v1, v2, vData) {
                        return AccountingFormat(vData.Cogs);
                    }
                },
                {
                    name: 'ContributionToCogs',
                    title: 'Contribution to COGS',
                    render: function(v1, v2, vData) {
                        return AccountingFormat(vData.ContributionToCogs) + '%';
                    }
                },
                {
                    name: 'GP',
                    title: 'Total Gross Profit',
                    render: function(v1, v2, vData) {
                        return AccountingFormat(vData.GP);
                    }
                },
                {
                    name: 'ContributionToGp',
                    title: 'Contribution to Gross Profit',
                    render: function(v1, v2, vData) {
                        return AccountingFormat(vData.ContributionToGp) + '%';
                    }
                },
            ];

            ClearTable();

            var pFooter = $('<tfoot>');
            var pTrFooter = $('<tr>');
            pFooter.append(pTrFooter);
            mTable.append(pFooter);
            pColumns.every(function () {
                pTrFooter.append($('<th>'));
                return true;
            });

            AjaxSend({
                url: '/ERP/Bir',
                data: vParams,
                success: function(vData) {
                    var pData = vData.Data.Rows;
                    mDataTable = mTable.DataTable({
                        data: pData,
                        columns: pColumns,
                        fixedHeader: true,
                        order: [],
                        sDom: '<"top"Bf>rt<"bottom"ip><"clear">', //hide search label and field
                        buttons: [{ extend: 'csvHtml5', text: 'Download CSV', title: 'Top Selling And Top Profitable SKUs Report' }],
                        initComplete: function () {
                            mTable.removeClass('dataTable');
                        }
                    });
                    mTable.addClass('table-stripe').css('width', '100%')
                    mTable.find('thead tr').addClass('text-center align-top').css('font-size', '12px');

                    var pDrFooter = vData.Footer.Rows;
                    if (pDrFooter.length > 0)
                        pDrFooter = pDrFooter[0];
                    else
                        pDrFooter = {};
                    $(mDataTable.column('SubTotal:name').footer()).text(AccountingFormat(pDrFooter.SubTotal));
                    $(mDataTable.column('Cogs:name').footer()).text(AccountingFormat(pDrFooter.Cogs));
                    $(mDataTable.column('GP:name').footer()).text(AccountingFormat(pDrFooter.GP));
                    $(mDataTable.column('ContributionToSales:name').footer()).text(AccountingFormat(pDrFooter.ContributionToSales) + '%');
                    $(mDataTable.column('ContributionToCogs:name').footer()).text(AccountingFormat(pDrFooter.ContributionToCogs) + '%');
                    $(mDataTable.column('ContributionToGp:name').footer()).text(AccountingFormat(pDrFooter.ContributionToGp) + '%');

                    //PopupReposition();
                } // END success: function(vData) {
            });
        }

        function ClearTable() {
            if ($.fn.DataTable.isDataTable(mTable)) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                mTable.empty();
                mDataTable.destroy();
            }
        }

        mElement.find('[name=butExport]').click(function() {

            var pAllCustomers = $('#chkBirAllCustomers').is(":checked");
            var pGroupByCustomer = $('#chkBirGroupByCustomer').is(":checked");

            if (!pAllCustomers && (CStr2(mCustomerID) == '')) {
                NewMessageBox('Please select a customer first.', 'No selected customer');
                return;
            }

            var pCheckedInvoiceTypes = mElement.find('.chkBirInvoiceType:checked');
            if (pCheckedInvoiceTypes.length == 0) {
                NewMessageBox('Please select an invoice type first.', 'No selected invoice type');
                return;
            }

            var pInvoiceTypeIDs = [];
            for (var pIndex = 0; pIndex <= pCheckedInvoiceTypes.length - 1; pIndex++) {
                var pElement = $(pCheckedInvoiceTypes[pIndex]);
                pInvoiceTypeIDs.push(pElement.val());
            }

            AjaxSend({
                url: '/Erp/CreateBirExcel',
                data: {
                    CustomerID: mCustomerID,
                    InvoiceTypeIDs: pInvoiceTypeIDs,
                    InvoiceDateFrom: mInvoiceDateFrom,
                    InvoiceDateTo: mInvoiceDateTo,
                    AllCustomers: pAllCustomers,
                    GroupByCustomer: pGroupByCustomer
                },
                success: function(vData) {  
                    if (vData.Error) {
                        NewMessageBox(vData.Error);
                        return;
                    }
                    if (CStr2(vData.Key) == '') {
                        NewMessageBox('Unexpected response');
                        return;
                    }

                    WebToast('Downloading spreadsheet.');
                    location = MergeUrlParameters({
                        Key: vData.Key
                    }, '/Erp/DownloadBirExcel');
                }
            });
        });

        mElement.find('[name=butExportPdf]').click(function() {

            var pInvoiceNumbers = mElement.find('[name=txtInvoiceNumbers]').val();

            if ((CStr2(pInvoiceNumbers).trim() == '')) {
                NewMessageBox('Please type at least 1 invoice number first.', 'No invoice number');
                return;
            }

            Bs.Loading('show');
            AjaxSend({
                url: '/Erp/CreateInvoicesPdfs',
                data: {
                    InvoiceNumbers: pInvoiceNumbers
                },
                success: function(vData) {  
                    if (vData.Error) {
                        NewMessageBox(vData.Error);
                        return;
                    }
                    if (CInt2(vData.OutputCount) == 0) {
                        NewMessageBox('No invoices matched the inputted invoice numbers');
                        return;
                    }

                    WebToast('Downloading zip file.');
                    var pUrl = MergeUrlParameters({
                        TimeStamp: vData.TimeStamp
                    }, '/Erp/DownloadInvoicesSinglePdf');
                    window.open(pUrl, '_blank', 
                        'width=' + window.innerWidth + 
                        ',height=' + window.innerHeight + 
                        ',scrollbars=auto');
                    NewMessageBox('You should be viewing the PDF file by now. If not, please check your web browser\'s popup settings.');
                },
                complete: function() {
                    Bs.Loading('hide');
                }
            });
        });

        return mOut;
    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});