define([
    'promise!/Scripts3/Browser/LibraryFowst.js?crc='
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/DropShipOrders/view.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.DropShipOrders = ERP.DropShipOrders.controller(); // singleton
                // mHtml.trigger('create');
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.DropShipOrders.controller', null, function () {

        var mOut = {};
        var mElement;
        var mSalesOrderID;
        var mOnOk;
        var mOnCancel;
        var mData;
        var mDontAutoClose;
        var mIncludeInvoiced;
        var mDropShip;
        var mOrderFulfillmentWorkLoaded;
        var mDateNeededFrom = $.format.date(new Date(), 'MM-0' + 1 + '-yy');
        var mDateNeededTo = $.format.date(new Date(), 'MM-dd-yy');
        var mSalesOrderFrom = $.format.date(new Date(), 'MM-0' + 1 + '-yy');
        var mSalesOrderTo = $.format.date(new Date(), 'MM-dd-yy');
        var mFowstID = sessionStorage.FowstID;

        //$('body').append($(mHtml));
        //mElement = $('[idx="popOrderInput"]').attr('id', popupid); // popOrderInput only

        mElement = $(mHtml);
        mElement.attr('id', 'popDropShipOrders' + NewID());
        $('body').append(mElement);

        mOut.element = mElement;
        var mTable = mElement.find('#tabDropShipSalesOrders table');
        var mDataTable;

       //mOut.element.trigger('create');        

        mOut.popup = function(vParams) {
            
            vParams = $.extend({
                dropShip: mDropShip
            }, vParams);
            mDropShip = vParams.dropShip;
            if (mDropShip)
                mElement.find('[name=title]').text('Drop Ship Orders Pending Fulfillment by Vendors');
            else
                mElement.find('[name=title]').text('SO Ready to Invoice');
            DisplayTable();
            mElement.find('[name=butOk]').hide();
            $('[custom-tab-content="#tabDropShipSalesOrders"]').click();

            try {
                mElement.modal('show');
            } catch (ex) {
                // just ignore the error
            }
             
        };        

        function DisplayTable() {

            var pColumns = [
                {
                    title: 'Sales Order No.',
                    render: function(v1, v2, vData) {
                        var pOut = CStr2(vData.MIN).trim();
                        if (pOut != '')
                            pOut = vData.MIN + '-';
                        if (vData.TransType == 'StockTransfer')
                            pOut = 'STR-' + pOut + vData.StrNum;
                        else
                            pOut = 'SO-' + pOut + vData.SalesOrderNo;
                            
                        return pOut;
                    }
                },
                {
                    title: 'Sales Order Date',
                    render: function(v1, v2, vData) {
                        return CStr2(vData.SalesOrderDate);
                    }
                },
                {
                    title: 'Date Needed',
                    render: function(v1, v2, vData) {
                        return CStr2(vData.DateNeeded);
                    }
                },
                {
                    title: 'Parent Sales Order No.',
                    render: function(v1, v2, vData) {
                        return CStr2(vData.ParentSalesOrderNo);
                    }
                },
                {
                    title: 'Serving Location',
                    render: function(v1, v2, vData) {
                        return CStr2(vData.FOWSTName);
                    }
                },
                {
                    title: 'Fulfillment Status',     
                    render: function(v1, v2, vData) {
                        return CStr2(vData.FulfillmentStatus);
                    }
                },
                {
                    title: 'Order Entry Location',
                    render: function(v1, v2, vData) {
                        if (CStr2(vData.EncodingFOWSTName).trim() == '')
                            return CStr2(vData.FOWSTName);
                        else
                            return CStr2(vData.EncodingFOWSTName);
                    }
                },
                {
                    title: 'Customer',
                    render: function(v1, v2, vData) {
                        return CStr2(vData.CompanyName);
                    }
                },
                {
                    title: 'Ordered Amount',
                    render: function(v1, v2, vData) {
                        return AccountingFormat(vData.OrderedAmount);
                    }
                },
                {
                    title: 'Served Amount',
                    render: function(v1, v2, vData) {
                        return AccountingFormat(vData.ServedAmount);
                    }
                }
            ];

            if (mDropShip)
                pColumns = pColumns.concat([
                    {
                        title: 'Drop Shipment?',
                        render: function(v1, v2, vData) {
                            return (vData.VendorPoSummaryNotFinalized == null) ? 'No' : 'Yes'; // you may also use the VendorPoSummaryFinalized field
                        }
                    },
                    {
                        title: 'No. of Unserved POs',
                        render: function(v1, v2, vData) {
                            var pTotal = CDec2(vData.VendorPoSummaryNotFinalized) + CDec2(vData.VendorPoSummaryFinalized)
                            if (pTotal == 0)
                                return '';
                            return '<div name=divUnservedPos></div>';
                        }
                    }
                ]);

            pColumns = pColumns.concat([
                {
                    title: 'SKU Count',
                    render: function(v1, v2, vData) {
                        return CStr2(vData.SkuCount);
                    }
                },
                {
                    title: 'Date Created',
                    render: function(v1, v2, vData) {
                        return CStr2(vData.DateCreated);
                    }
                },
                {
                    title: 'Last Updated',
                    render: function(v1, v2, vData) {
                        return CStr2(vData.LastUpdated);
                    }
                },
                {
                    title: 'Updated By',
                    render: function(v1, v2, vData) {
                        return CStr2(vData.EmployeeFullName);
                    }
                }
            ]);

            ClearTable();
            var pData = {
                FilterSalesOrders: 1,
                ExcludeInvoiced: 1,
                InvoiceFowstID: mFowstID,
                DateNeededFrom: mDateNeededFrom,
                DateNeededTo: mDateNeededTo,
                SalesOrderDateFrom: mSalesOrderFrom,
                SalesOrderDateTo: mSalesOrderTo,
                NonZeroServedSkuCount: true
            };
            if (mDropShip)
                pData.DisplayUnservedCount = 1;
            else 
                pData.FinalizedFcOnly = 1;
            AjaxSend({
                url: '/Browser/TransInvoices',
                data: pData,
                success: function(vData) {
                    mData = vData.Results.Rows;
                    mDataTable = mTable.DataTable({
                        data: mData,
                        columns: pColumns,
                        order: [],
                        rowCallback: function(row, data, dataIndex){
                            var pDiv = $(row).find('[name=divUnservedPos]');
                            if (pDiv.children().length > 0)
                                return;

                            var pTotal = CDec2(data.VendorPoSummaryNotFinalized) + CDec2(data.VendorPoSummaryFinalized)
                            var pLink = $('<a>')
                                .text(AccountingFormat(data.VendorPoSummaryNotFinalized, 0, '0') + ' of ' + AccountingFormat(pTotal, 0, '0'))
                                .attr('href', '#')
                                .click(function(e) {
                                    e.preventDefault();
                                    ShowOrderFulfillment({
                                        CustomerID: data.CustomerID,
                                        InvoiceID: data.ID,
                                        FowstID: sessionStorage.FowstID
                                    });
                                    return false;
                                });
                            pDiv.append(pLink);
                        },
                        initComplete: function () {
                            mTable.removeClass('dataTable')
                        }
                    });
                    mTable.addClass('table table-light table-striped table-hover').css('width', '100%')
                    mTable.find('thead tr').addClass('custom-theme-f').css({
                        fontSize: '12px',
                        verticalAlign: 'top',
                        textAlign: 'center'
                    });
                    mElement.find('[name=butOk]').show();
                    PopupReposition();
                }
            });
        }

        function ClearTable() {
            if ($.fn.DataTable.isDataTable(mTable)) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                mTable.empty();
                mDataTable.destroy();
            }
        }

        function ShowOrderFulfillment(vParams) {
            
            if (mOrderFulfillmentWorkLoaded) {
                $('[custom-tab-content="#tabDropShipPurchaseOrders"]').click();
                proj.ShowFulfillmentCenterTransaction({
                    CustomerID: vParams.CustomerID,
                    InvoiceID: vParams.InvoiceID,
                    FowstID: vParams.FowstID
                });
            } else {
                $('[custom-tab-content="#tabDropShipPurchaseOrders"]').click();
                PromptUserAccess('Access Order Fulfillment Module*', {
                    OnClickOKSuccess: function () {
                        AjaxSend({
                            url: '/DashboardsAndReports/SalesOrder?OrgID=' + sessionStorage.OrgID,
                            type: 'GET',
                            dataType: 'text',
                            success: function (vData) {
                                require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/SalesOrder.js?v='+assetversion)],
                                    function() {
                                        var p = $(vData);
                                        p = p.find('#MainNav1a');
                                        $('#tabDropShipPurchaseOrders').html('').append(p);
                                        p.trigger('create');
                                        mOrderFulfillmentWorkLoaded = true;
                                        proj.ShowFulfillmentCenterTransaction({
                                            CustomerID: vParams.CustomerID,
                                            InvoiceID: vParams.InvoiceID,
                                            FowstID: vParams.FowstID
                                        });
                                    }
                                ); // END require
                            } // END success: function (vData) {
                        }); // END AjaxSend({
                    } // END OnClickOKSuccess: function () {
                }); // END PromptUserAccess('Sales Order Processing and Analytics', {
            } // END ELSE if (mOrderFulfillmentWorkLoaded) {
        } // END function ShowOrderFulfillment

        mElement.find('[name=butDateNeeded]')
            .text(mDateNeededFrom + ' to ' + mDateNeededTo)
            .click(function () {
            DualCalendar({
                Format: 'MM-dd-yy',
                Click: function (e) {
                    mDateNeededFrom = e.From;
                    mDateNeededTo = e.To;
                    mElement.find('[name=butDateNeeded]').text(mDateNeededFrom + ' to ' + mDateNeededTo);
                    mOut.popup();
                }
            })
        });

        mElement.find('[name=butSalesOrderDate]')
            .text(mSalesOrderFrom + ' to ' + mSalesOrderTo)
            .click(function () {
                DualCalendar({
                    Format: 'MM-dd-yy',
                    Click: function (e) {
                        mSalesOrderFrom = e.From;
                        mSalesOrderTo = e.To;
                        mElement.find('[name=butSalesOrderDate]').text(mSalesOrderFrom + ' to ' + mSalesOrderTo);
                        mOut.popup();
                    }
                }
            )
        });

        mElement.find('[name=butServingLocation]')
            .text(sessionStorage.FowstName)
            .click(function () {
                BrowsePopup('LibraryFowst', 'FOWST',
                    [
                        {
                            AutoSearch: true,
                            DisplayText: [{ Column: 'FowstName' }],
                            Click: function (e) {
                                mFowstID = e.ID;
                                mElement.find('[name=butServingLocation]').text(e.FowstName);
                                mOut.popup();
                            }
                        }
                    ], {
                        FilterFowst: 1
                    }
                ); // END BrowsePopup('LibraryFowst', 'Fowst',
            });

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});