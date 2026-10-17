define([
    '/Scripts/sources/ERP/OrderProcessing/backorders/model.js'
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/OrderProcessing/backorders/view.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                // ERP.OrderProcessing.product.controller = ERP.OrderProcessing.product.controller(); // singleton
                // mHtml.trigger('create');
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.OrderProcessing.backorders.controller', null, function () {

        var mOut = {};
        var mElement;
        var mDivUnit;
        var mTransType;
        var mProductName;

        mElement = $(mHtml);
        mOut.element = mElement;
        mOut.Model = null;
        var mTable = mElement.find('[name=table]');
        var mDataTable;

        mOut.Init = function() {
            mOut.Model = new ERP.OrderProcessing.backorders.model();
            // LoadData(); // 2023_05_03__16_43_47_5955
        };

        function ClearTable() {
            if ($.fn.DataTable.isDataTable(mTable)) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                mTable.empty();
                mDataTable.destroy();
            }
        }

        function LoadData() {
            ClearTable();
            if (!CBool(GetUrlParameters().IsReadOnly)) 
                mOut.Model.GetData({
                    Success: function (vData) {
                        DisplayTable(vData.Results.Rows);
                    }
                });
        }

        function DisplayTable(vRows) {
            var pButtons = [
                {
                    text: 'Refresh',
                    action: function() {
                        LoadData();
                    }
                }
            ];
            mDataTable = mTable.DataTable({
                data: vRows,
                columns: [
                    //{
                    //    'searchable':false,
                    //    'orderable':false,
                    //    'width':'1%',
                    //    'className': 'dt-body-center',
                    //    'render': function (data, type, full, meta){
                    //        return '<input type="checkbox" name="chkSelector">';
                    //    }
                    //},
                    //{
                    //    render: function(v1, v2, vData) {
                    //        if (vData.DepositStatus == '1')
                    //            return 'Deposited';
                    //        else if (vData.DepositStatus == '2')
                    //            return 'Returned';
                    //        else
                    //            return '';
                    //    }
                    //},
                    {
                        render: function(v1, vType, vData) {
                            if (vType == 'display') {
                                var pOut = $('<input name=chkInclude type=checkbox>');
                                pOut = pOut.get(0).outerHTML;
                                return pOut;
                            } else
                                return '';
                        }
                    },
                    {
                        data: function(vData) {
                            var pOut = CStr2(vData.MIN).trim();
                            if (pOut != '')
                                pOut = vData.MIN + '-';
                            pOut = 'Backorder-' + pOut + vData.SalesOrderNo;
                            
                            return pOut;
                        }
                    },
                    {
                        data: 'CompanyName'
                    },
                    {
                        data: function(vData) {
                            return AccountingFormat(vData.OrderedAmount);
                        }
                    },
                    {
                        data: 'SkuCount'
                    },
                    {
                        data: 'DateCreated'
                    },
                    {
                        data: 'LastUpdated'
                    },
                    {
                        data: 'EmployeeFullName'
                    },
                    {
                        data: function(vData) {
                            return CStr2(vData.BackorderSourceMinSalesOrderNo);
                        }
                    },
                    {
                        data: function(vData) {
                            return CStr2(vData.BackorderSourceSalesOrderDate);
                        }
                    },
                    {
                        render: function(v1, vType, vData) {
                            if (vType == 'display') {
                                var pOut = $('<div>')
                                    .attr('name', 'divAction');
                                pOut = pOut.get(0).outerHTML;
                                return pOut;
                            } else
                                return '';
                        }
                    }
                ],
                'rowCallback': function(row, data, dataIndex) {
                    $(row).data('data', data);
                    //$(row).find('[name=chkSelector]').prop('checked', data.Checked);
                    //$(row).unbind('click').click(function() {
                    //    var win = window.open(location.href,
                    //        '_blank',
                    //        'width=' + window.innerWidth + ',height=' + window.innerHeight);

                    //    win.onload = function () {
                    //        function LoadRecord() {
                    //            if (win.Z9Q3C9ASDA)
                    //                win.Z9Q3C9ASDA.ShowSalesOrder({
                    //                    ID: data.ID,
                    //                    ReadOnly: true
                    //                });
                    //            else
                    //                setTimeout(LoadRecord, 1000);
                    //        }
                    //        LoadRecord();
                    //    }
                    //});

                    var pOut = $('<div>');
                    var pLink = $('<a>')
                        .text('Edit')
                        .attr('href', '#')
                        .click(function() {
                            proj.LoadEasSalesOrderTrans({
                                ID: data.ID
                            });
                            $('#divOrderProcessingTab').find('[data-target=#tabOPWork]').click();
                        });
                    pOut.append(pLink);
                    pOut.append(' | ');
                    pLink = $('<a>')
                        .text('Preview')
                        .attr('href', '#')
                        .click(function() {
                            var win = window.open(location.origin + '/erp' + location.search + '&ShowSalesOrder=1&IsReadOnly=1&ID=' + data.ID,
                                '_blank',
                                'width=' + screen.availWidth + ',height=' + screen.availHeight);

                            win.onload = function () {
                                function LoadRecord() {
                                    if (win.Z9Q3C9ASDA)
                                        win.Z9Q3C9ASDA.ShowSalesOrder({
                                            ID: data.ID,
                                            ReadOnly: true
                                        });
                                    else
                                        setTimeout(LoadRecord, 1000);
                                }
                                LoadRecord();
                            }
                        });
                    pOut.append(pLink);
                    $(row).find('[name=divAction]').empty().append(pOut);
                },
                buttons: pButtons,
                sDom: '<"top"Bf>rt<"bottom"ip><"clear">',
                initComplete: function () {
                    mTable.removeClass('dataTable');
                }
            });
        }

        mElement.find('[name=butCreateItemRequisition]').click(function() {
            var pIncluded = mElement.find('[name=chkInclude]:checked');
            if (pIncluded.length == 0) {
                NewMessageBox('Please check at least 1 backorder.', 'No selected backorder');
                return;
            }

            var pIDs = [];
            for (var pIndex = 0; pIndex <= pIncluded.length - 1; pIndex++) {
                var pChk = $(pIncluded[pIndex]);
                var pTR = pChk.closest('tr');
                var pDR = pTR.data('data');
                pIDs.push(pDR.ID);
            }

            AjaxSend({
                url: '/Erp/UpdateItemRequisition',
                data: {
                    IDs: pIDs
                },
                success: function(vData) {
                    if (vData.Error)
                        NewMessage(vData.Error, 'Fail');
                    else if (vData.Result == 'ok')
                        NewMessageBox('Item requisition were updated successfully.', 'Success');
                }
            });
        });

        mOut.Init();

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});