define([
    
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/ChildSos/view.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.ChildSos.controller', null, function () {

        var mOut = {};
        var mElement = $(mHtml);
        var mInvoiceID;
        var mDataTable;

        $('body').append(mElement);//.trigger('create');
        // mElement.trigger('create'); // does not work
        var mTable = mElement.find('[name=mainTable]');

        mOut.Show = function(vParams) {
            vParams = $.extend({
                InvoiceID: null
            }, vParams);

            mInvoiceID = vParams.InvoiceID;
            NewMessageBoxClose();
            GetData();
            $('#panChildSos').show();
        };

        function GetData() {
            AjaxSend({
                url: '/ERP/ChildSos',
                data: {
                    InvoiceID: mInvoiceID
                },
                success: function(vData) {
                    DisplayData(vData.Rows);
                }
            });
        }

        function DisplayData(vRows) {
            ClearTable();
            mDataTable = mTable.DataTable({
                data: vRows,
                columns: [
                    {
                        data: function(vDR) {
                            return vDR.SoMin;
                        }
                    },
                    {
                        data: function(vDR) {
                            return vDR.Name;
                        }
                    },
                    {
                        data: function(vDR) {
                            return CStr2(vDR.DeliveryMethod);
                        }
                    },
                    {
                        data: function(vDR) {
                            return vDR.ProductName;
                        }
                    },
                    {
                        data: function(vDR) {
                            return AccountingFormat(vDR.OrderedQuantity, 0);
                        }
                    }
                ],
                sDom: '<"top">rt<"bottom"ip><"clear">'
            });
        }

        function ClearTable() {
            if ($.fn.DataTable.isDataTable(mTable)) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                mTable.empty();
                mDataTable.destroy();
            }
        }
        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});