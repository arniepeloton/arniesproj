define([
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/AverageWeeklyDemand/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                //ERP.PurchaseOrder.poBySku.controller = ERP.PurchaseOrder.poBySku.controller(); // singleton
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.AverageWeeklyDemand.controller', null, function () {

        var mOut = {};
        var mElement;
        var mData = [];
        var mSelectedTr;
        var mFowstID = sessionStorage.FowstID;
        var mExpanded = false;
        var mProductID;

        mElement = $(mHtml);
        mOut.element = mElement;
        mOut.ItemRequisitionMode = false;

        var mTable = mElement.find('[name=myTable]');
        var mTableBody = mTable.find('tbody');
        var mTr;

        mOut.Initialize = function(vParams) {
            vParams = $.extend({
                ProductID: null
            }, vParams);

            mProductID = vParams.ProductID;
            mTable.hide();
            if (CStr2(mProductID) == '') {
                mElement.find('[name=divLoading]').text('Please select a product first.');
                mElement.find('[name=divLoading]').show();
                return;            
            }
            mElement.find('[name=divLoading]').text('Loading data. Please wait.');
            if (mOut.ItemRequisitionMode)
                mElement.find('[name=butNew],[name=butSaveAsDraft]').hide();
            else {
                mFowstID = '';
                mElement.find('#butPoBySkuFowst')
                    .text('[All FOWSTs]');
            }
            DisplayData();
            LoadData();
        };

        function LoadData() {
            mElement.find('[name=divLoading]').show();
            mTable.hide();
            AjaxSend({
                url: '/ERP/GetAverageWeeklyDemand',
                data: {
                    SelectedFowstID: mFowstID,
                    ProductID: mProductID
                },
                minimumDuration: 1000,
                success: function(vData) {
                    DisplayData(vData);
                    mElement.find('[name=divLoading]').hide();
                    mTable.show();
                }
            });
        }

        function DisplayData(vData) {
            
            //if (s != undefined) {
            //    tblSysAdminDataTable.search($.trim(s)).draw();
            //    return;
            //}

            var pData = {
                columns: []
            };

            pData.columns.push(
                {
                    title: 'Period',
                    "data": function (data, type, full, meta) {
                        if (vData == null)
                            return '';
                        return CStr2(data.SalesOrderYear);
                    },
                    "width": "100px",
                    className: 'dt-head-center'
                }
            );

            pData.columns.push(
                {
                    title: 'Average Weekly Demand',
                    "data": function (data, type, full, meta) {
                        if (vData == null)
                            return '';
                        return AccountingFormat(data.AverageWeeklyOrderQty, 4) + ' ' + vData.Unit + '/week';
                    },
                    "width": "500px",
                    className: 'dt-head-center'
                }
            );

            if (vData == null)
                pData.Rows = [];
            else
                pData.Rows = vData.data.Rows;

            SysAdminDataTable(pData);
        }

        function SysAdminDataTable(vData) {
            mTableBody.empty();
            
            if ($.fn.DataTable.isDataTable(mTable)) {
                //check if datatable is created 
                mTable.DataTable().clear().destroy();
            }

            mTable.DataTable({
                //retrieve: true,
                data: vData.Rows,
                "bdestroy": true,
                "pageLength": 50,
                "paging": false,
                //"order": [[1, "asc"]],
                "bProcessing": true,
                //"bServerSide": true,
                //"ajax": vData.ajax,
                "autoWidth": false,
                "columns": vData.columns,
                "aoColumnDefs": [{ "bSortable": true, "aTargets": vData.disablecols }],
                "responsive": true,
                "columnDefs": [{ "targets": 0, "orderable": true }],
                "sDom": '<"top">rt<"bottom"ip><"clear">',
                rowCallback: function(row, data, dataIndex){
                    $(row).data('data', data)
                       .click(function(e) {
                            if (!$(e.target).parent().is(row))
                                return;
                            // no code for now 2022_02_07__10_55_28_4379
                       });
                },
                "initComplete": function () {
                    mTable.removeClass('dataTable');
                    mTable.find('thead tr').addClass('align-top text-center')
                }
                //"stateSave": true
            })
            if (mTr == null)
                mTr = mTableBody.find('tr').clone();

        }

        // mOut.Initialize();

        return mOut;
    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});