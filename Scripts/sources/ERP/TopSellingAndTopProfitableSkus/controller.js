define([
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/TopSellingAndTopProfitableSkus/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                // ERP.TopSellingAndTopProfitableSkus = ERP.TopSellingAndTopProfitableSkus.controller(); // singleton
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.TopSellingAndTopProfitableSkus.controller', null, function () {

        var mOut = {};
        var mElement;
        var mDateFrom;
        var mDateTo;
        var mFilter;
        var mFilterValue;

        mElement = $(mHtml);
        $.mobile.activePage.append(mElement);
        mOut.element = mElement;

        var mTable = mElement.find('[name=tableData]');
        var mDataTable;

        mOut.Popup = function(vParams) {
            vParams = $.extend({
                Filter: null,
                FilterValue: null,
                Group: 'product',
                DateFrom: new Date(),
                DateTo: new Date()
            }, vParams);

            mDateFrom = vParams.DateFrom;
            mDateTo = vParams.DateTo;
            mFilter = vParams.Filter;
            mFilterValue = vParams.FilterValue;

            mElement.find('[name=butDateRange]')
                .text(FormatDate(mDateFrom, {
                    Format: 'MMM dd, yyyy'
                }) + ' to ' +
                FormatDate(mDateTo, {
                    Format: 'MMM dd, yyyy'
                }));
            //mElement.find('[name=popupTopSellingAndTopProfitableSkus]').modal('show');
            mElement.modal('show');
            LoadData();
        };

        mElement.find('[name=butRefresh]').click(function() {
            LoadData();
        });

        mElement.find('[name=butDateRange]')
            .click(function () {
                DualCalendar({
                    Click: function (e) {
                        mElement.find('[name=butDateRange]').text(FormatDate(e.From, {
                                Format: 'MMM dd, yyyy'
                            }) + ' to ' +
                            FormatDate(e.To, {
                                Format: 'MMM dd, yyyy'
                            }));
                        mDateFrom = e.From;
                        mDateTo = e.To;
                        LoadData();
                    }
                }, new Date(), new Date());    
            }
        );

        var mSelGroup = mElement.find('[name=selGroup]')
            .change(function() {
                LoadData();
            });

        function LoadData(vParams) {
            vParams = $.extend({
                DateFrom: mDateFrom,
                DateTo: mDateTo,
                Filter: mFilter,
                FilterValue: mFilterValue,
                Group: mSelGroup.val()
            }, vParams)

            var pColumns = [];
            if (vParams.Group == 'product') {
                pColumns.push(
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
                    }
                );
            } else if (vParams.Group == 'brand') {
                pColumns.push(
                    {
                        title: 'Brand',
                        render: function(v1, v2, vData) {
                            return CStr2(vData.Brand);
                        }
                    }
                );
            } else if (vParams.Group == 'product category') {
                pColumns.push(
                    {
                        title: 'Category',
                        render: function(v1, v2, vData) {
                            return CStr2(vData.CategoryName);
                        }
                    }
                );
            } else if (vParams.Group == 'sales channel') {
                pColumns.push(
                    {
                        title: 'Sales Channel',
                        render: function(v1, v2, vData) {
                            return CStr2(vData.SalesChannelName);
                        }
                    }
                );
            } else if (vParams.Group == 'customer industry') {
                pColumns.push(
                    {
                        title: 'Industry Name',
                        render: function(v1, v2, vData) {
                            return CStr2(vData.UserDefinedIndustryName);
                        }
                    }
                );
            } else if (vParams.Group == 'customer') {
                pColumns.push(
                    {
                        title: 'Customer',
                        render: function(v1, v2, vData) {
                            return CStr2(vData.CompanyName);
                        }
                    }
                );
            }

            pColumns.push(
                {
                    title: 'Average Selling Price (in base unit)',
                    className: 'dt-head-body-center',
                    render: function(v1, v2, vData) {
                        return AccountingFormat(vData.UnitPrice);
                    }
                },
                {
                    title: 'Average COGS (in base unit)',
                    className: 'dt-head-body-center',
                    render: function(v1, v2, vData) {
                        return AccountingFormat(vData.AvgCogs);
                    }
                },
                {
                    title: 'Average Unit Profit',
                    className: 'dt-head-body-center',
                    render: function(v1, v2, vData) {
                        return AccountingFormat(vData.AvgGP);
                    }
                },
                {
                    title: 'Average Unit Profit Margin',
                    className: 'dt-head-body-center',
                    render: function(v1, v2, vData) {
                        return AccountingFormat(vData.GPM) + '%';;
                    }
                },
                {
                    title: 'Total Units Sold (in base UoM)',
                    className: 'dt-head-body-center',
                    render: function(v1, v2, vData) {
                        return AccountingFormat(vData.UnitsSold, 0);
                    }
                },
                {
                    name: 'SubTotal',
                    className: 'dt-head-body-center',
                    title: 'Total Sales',
                    render: function(v1, v2, vData) {
                        return AccountingFormat(vData.SubTotal);
                    }
                },
                {
                    name: 'ContributionToSales',
                    title: 'Contribution to Total Sales',
                    className: 'dt-head-body-center',
                    render: function(v1, v2, vData) {
                        return AccountingFormat(vData.ContributionToSales) + '%';
                    }
                },
                {
                    name: 'Cogs',
                    title: 'Total COGS',
                    className: 'dt-head-body-center',
                    render: function(v1, v2, vData) {
                        return AccountingFormat(vData.Cogs);
                    }
                },
                {
                    name: 'ContributionToCogs',
                    title: 'Contribution to Total COGS',
                    className: 'dt-head-body-center',
                    render: function(v1, v2, vData) {
                        return AccountingFormat(vData.ContributionToCogs) + '%';
                    }
                },
                {
                    name: 'GP',
                    title: 'Total Gross Profit',
                    className: 'dt-head-body-center',
                    render: function(v1, v2, vData) {
                        return AccountingFormat(vData.GP);
                    }
                },
                {
                    name: 'ContributionToGp',
                    title: 'Contribution to Total Gross Profit',
                    className: 'dt-head-body-center',
                    render: function(v1, v2, vData) {
                        return AccountingFormat(vData.ContributionToGp) + '%';
                    }
                }
            );

            AjaxSend({
                url: '/ERP/TopSellingAndTopProfitableSkus',
                data: vParams,
                success: function(vData) {
                    do {
                        var pData = vData.Data.Rows;
                        ClearTable();
                        if ($.fn.DataTable.isDataTable(mTable)) {
                            mDataTable = mTable.DataTable();
                            mDataTable.destroy();
                            mTable.empty();
                        }

                        var pFooter = $('<tfoot>');
                        var pTrFooter = $('<tr>');
                        pFooter.append(pTrFooter);
                        mTable.append(pFooter);
                        pColumns.every(function () {
                            pTrFooter.append($('<th>'));
                            return true;
                        });
                        try {
                            if ($.fn.DataTable.isDataTable(mTable)) {
                                mDataTable = mTable.DataTable();
                                mDataTable.destroy();
                            }
                            mDataTable = mTable.DataTable({
                                data: pData,
                                columns: pColumns,
                                fixedHeader: true,
                                order: [],
                                sDom: '<"top"Bf>rt<"bottom"ip><"clear">', //hide search label and field
                                buttons: [{ extend: 'csvHtml5', text: 'Download CSV', title: 'Top Selling And Top Profitable SKUs Report' }],
                                initComplete: function () {
                                    mTable.removeClass('dataTable');
                                },
                                createdRow: function (row, data, index) {
                                    $(row).click(function() {
                                        var pFilterValue;
                                        if (vParams.Group == 'product')
                                            pFilterValue = data.ProductID;
                                        else if (vParams.Group == 'brand')
                                            pFilterValue = data.Brand;
                                        else if (vParams.Group == 'product category')
                                            pFilterValue = data.CategoryName;
                                        else if (vParams.Group == 'sales channel')
                                            pFilterValue = data.SalesChannelID;
                                        else if (vParams.Group == 'customer industry')
                                            pFilterValue = data.UserDefinedIndustryID;
                                        else if (vParams.Group == 'customer')
                                            pFilterValue = data.CustomerID;
                                        new ERP.TopSellingAndTopProfitableSkus.controller().Popup({
                                            DateFrom: vParams.DateFrom,
                                            DateTo: vParams.DateTo,
                                            Filter: vParams.Group,
                                            FilterValue: pFilterValue
                                        });
                                    });
                                }
                            });
                        } catch(e) {
                            console.warn('jsYCRNVT3NRS ' + e.message);
                            continue;
                        }
                        mDataTable = mTable.DataTable();
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
                        break;
                    } while (true); 
                } // END success: function(vData) {
            });
        }

        function ClearTable() {
            if ($.fn.DataTable.isDataTable(mTable)) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                mTable.empty();
                if (mDataTable)
                    mDataTable.destroy();
            }
        }

        return mOut;
    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});