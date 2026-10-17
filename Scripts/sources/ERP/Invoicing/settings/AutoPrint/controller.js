define([
    
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/Invoicing/settings/AutoPrint/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.Invoicing.settings.AutoPrint = ERP.Invoicing.settings.AutoPrint.controller(); // singleton
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.Invoicing.settings.AutoPrint.controller', null, function () {

        var mOut = {};
        var mElement;
        mElement = $(mHtml);
        mOut.element = mElement;
        var mTable = mElement.find('[name=tableAutoPrintInvoiceUuid]');
        var mDataTable;

        function DisplayTable(vRows) {

            ClearTable();
      
            var pColumns = [
                {
                    title: 'Workstation Device Serial',
                    render: function(v1, v2, vData) {
                        return CStr2(vData.Uuid);
                    }
                },
                {
                    title: 'Requested By',
                    render: function(v1, v2, vData) {
                        return CStr2(vData.RequestedByName);
                    }
                },
                {
                    title: 'Date Requested',
                    render: function(v1, v2, vData) {
                        return CStr2(vData.DateRequested);
                    }
                },
                {
                    title: 'Status',
                    render: function(v1, v2, vData) {
                        return '<div name=divStatus></div>';
                    }
                }
            ];
          
            mDataTable = mTable.DataTable({
                data: vRows,
                columns: pColumns,
                order: [],
                rowCallback: function(row, data, dataIndex){
                    var pDiv = $(row).find('[name=divStatus]');
                    if (pDiv.children().length > 0)
                        return;

                    var pSelect = $('<select name=selStatus>');
                    pSelect.data('data', data);

                    var pOption = new Option('Pending', 'pending');
                    pSelect.append(pOption);

                    pOption = new Option('Approved', 'approved');
                    pSelect.append(pOption);

                    pOption = new Option('Disapproved', 'disapproved');
                    pSelect.append(pOption);

                    if (CStr2(data.Status) == '')
                        data.Status = 'pending';

                    pSelect.val(data.Status);
                    pDiv.append(pSelect);
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
            var pOut = {
                AutoPrintStatus: []
            };
            
            var pSelects = mElement.find('select[name=selStatus]');
            for (var pIndex = 0; pIndex <= pSelects.length - 1; pIndex++) {
                var pSelect = $(pSelects[pIndex]);
                var pDR = {
                    ID: pSelect.data('data').ID,
                    Status: pSelect.val()
                };
                pOut.AutoPrintStatus.push(pDR);
            }

            return pOut;
        };

        mOut.LoadData = function LoadData(vData) {
          
            DisplayTable(vData.AutoPrint.Rows);

        };

        return mOut;
    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});