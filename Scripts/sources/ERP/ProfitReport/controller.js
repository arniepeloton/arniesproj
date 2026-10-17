define([
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/ProfitReport/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.ProfitReport.controller = ERP.ProfitReport.controller(); // singleton
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.ProfitReport.controller', null, function () {

        var mOut = {};
        var mElement;
        var mDataTable;
        var mGrouping;
        var mGrouping2;

        mElement = $(mHtml);
        $.mobile.activePage.append(mElement);

        mOut.popup = function() {
            mElement.modal('show');
            LoadData();
        };

        mElement.find('[type=radio]')//.checkboxradio().checkboxradio('refresh');
        mElement.find('[type=button]')//.button().button('refresh');

        function LoadData() {
            var pData = {
                GroupBy: $('[name=radGroup]:checked').val()
            };
            mGrouping2 = null;
            if (pData.GroupBy == 'Customer') 
                mGrouping = 'Customer';
            else if (pData.GroupBy == 'Sku') {
                mGrouping = 'Product Name';
                mGrouping2 = 'Product No.';
            }

            AjaxSend({
                url: '/DashboardsAndReports/GetProfitData',
                data: pData,
                success: function(vData) {
                    DisplayData(vData);
                }
            });
        }

        function DisplayData(vData) {
            if (mDataTable != null) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                mElement.find('[name=table]').empty();
                mDataTable.destroy();
                mElement.find('[name=table] thead').empty(); // this is important
            }

            var pColumns = []
            if (mGrouping2 != null)
                pColumns.push(
                    {
                        title: mGrouping2,
                        data: function (vDR) {
                            return vDR.Grouping2 || '';
                        }
                    }
                );

            pColumns.push(
                {
                    title: mGrouping,
                    data: function (vDR) {
                        return vDR.Grouping || '';
                    }
                }
            );

            for (var pYear = vData.YearTo; vData.YearFrom <= pYear; pYear--) {
                (function(vYear) {
                    pColumns.push(
                        {
                            title: 'Net Sales ' + vYear,
                            render: function (v1, vType, vDR) {
                                // test display 1st before else https://stackoverflow.com/a/45037039/334045
                                if (vType == 'display')
                                    return '<a name="NetSales-' + vYear + '" href=#>' + AccountingFormat(vDR['NetSales-' + vYear]) + '</a>';
                                return CDec2(vDR['NetSales-' + vYear]);
                            },
                            className: 'dt-body-right dt-head-center'
                        }
                    );
                    pColumns.push(
                        {
                            title: 'Gross Profit ' + vYear,
                            render: function (v1, vType, vDR) {
                                if (vType == 'display')
                                    return AccountingFormat(vDR['GrossProfit-' + vYear]);
                                return CDec2(vDR['GrossProfit-' + vYear]);
                            },
                            className: 'dt-body-right dt-head-center'
                        }
                    );
                    pColumns.push(
                        {
                            title: 'GPM ' + vYear,
                            render: function (v1, vType, vDR) {
                                if (vType == 'display') {
                                    if (CDec2(vDR['GPM-' + vYear]) != 0)
                                        return AccountingFormat(CDec2(vDR['GPM-' + vYear]) * 100) + '%';
                                    else
                                        return '';
                                }
                                return CDec2(vDR['GPM-' + vYear]);
                            },
                            className: 'dt-body-right dt-head-center'
                        }
                    );
                })(pYear);
            }
          
            //

            mDataTable = mElement.find('[name=table]');

            mDataTable = mDataTable.DataTable({
                //order: [[0, 'asc']], //vParams[0].OrderBy is array . use only in display table
                pageLength: 25,
                "ordering": true,
                paging: true,
                data: vData.Table.Rows,
                columns: pColumns,
                initComplete: function () {
                    //PopupReposition()
                },
                "aaSorting": [],
                order: [],
                buttons: [
                    {
                        text: 'Refresh',
                        action: function (e, dt, node, config) {
                            LoadData();
                        }
                    },
                    { extend: 'csvHtml5', text: 'Download CSV', title: 'Profitability Report' }
                ],
                sDom: '<"top"B>rt<"bottom"ip><"clear">',
                rowCallback: function(row, data, dataIndex){
                    var pLinks = $(row).find('[name^=NetSales-]');
                    for (var pLinkIndex = 0; pLinkIndex <= pLinks.length - 1; pLinkIndex++) {
                        var pLink = pLinks[pLinkIndex];
                        pLink = $(pLink);
                        (function(vLink) {
                            var pDashIndex = vLink.attr('name').indexOf('-');
                            var pYear = vLink.attr('name').substr(pDashIndex + 1);
                            var pParams = {
                                InvoiceYear: pYear,
                                Click: function () {

                                }
                            };
                            if (mGrouping == 'Customer')
                                pParams.CustomerID = data.GroupingID;
                            else
                                pParams.ProductID = data.GroupingID;
                    
                            vLink.click(function() {
                                ShowSkuDetails(pParams);
                            });
                        })(pLink);
                        
                    }
                    
                }, initComplete: function () {
                    mElement.find('[name=table]').removeClass('dataTable');
                }
            })

        }

        function ShowSkuDetails(vParams) {
            
            var pColumns = [
                {
                    Label: 'Customer Name',
                    Column: 'CustomerName'
                },
                {
                    Label: 'Invoice No.',
                    Column: 'FullInvoiceNumber'
                },
                {
                    Label: 'Invoice Date',
                    Column: 'InvoiceDate'
                },
                {
                    Label: 'Product No.',
                    Column: 'ProductNo'
                },
                {
                    Label: 'Product Name',
                    Column: 'ProductName'
                },
                {
                    Label: 'Served Quantity',
                    Column: function(vData) {
                        return AccountingFormat(vData.Quantity, 0);
                    }
                },
                {
                    Label: 'Unit Price',
                    Column: function(vData) {
                        return AccountingFormat(vData.UnitPrice);
                    }
                },
                {
                    Label: 'Subtotal',
                    Column: function(vData) {
                        return AccountingFormat(vData.SubTotal);
                    }
                },
                {
                    Label: 'Line Cost',
                    Column: function(vData) {
                        return AccountingFormat(vData.LineCost);
                    }
                },
                {
                    Label: 'Line Gross Profit',
                    Column: function(vData) {
                        return AccountingFormat(vData.LineGP);
                    }
                },
                {
                    Label: 'Gross Profit Margin',
                    Column: function(vData) {
                        return AccountingFormat(CDec2(vData.UnitGPM) * 100) + '%';
                    }
                }
            ];

            BrowsePopup('ProfitDetails', 'Item Details',
                [
                    {
                        WithSearchbox: false,
                        ModalSize: 'xl',
                        StyleFormat: 'table',
                        AutoSearch: true,
                        DisplayText: pColumns,
                        Click: vParams.Click,
                        Validation: vParams.Validation,
                        Sorting: true,
                        //OrderBy: [1, 'asc']
                        DownloadCsvTitle: 'Profitability Report Item Details'
                    }
                ], { 
                    CustomerID: vParams.CustomerID,
                    ProductID: vParams.ProductID,
                    InvoiceYear: vParams.InvoiceYear
                }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        }

        var mFilterText = '';
        setInterval(function() {
            var pTxt = mElement.find('#txtProfitReportFilter');
            if (pTxt.length == 0)
                return;
            var pVal = $.trim(pTxt.val());
            if (mFilterText == pVal)
                return;
            mDataTable.search(pVal).draw();
            mFilterText = pVal;
        }, 500);
        return mOut;
    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});