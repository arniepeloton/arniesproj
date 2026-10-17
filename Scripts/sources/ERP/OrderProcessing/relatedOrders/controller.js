define([
    '/Scripts/sources/ERP/OrderProcessing/product/model.js'
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/OrderProcessing/relatedOrders/view.html?v=' + assetversion,
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

    CreateStringObject('ERP.OrderProcessing.relatedOrders.controller', null, function () {

        var mOut = {};
        var mElement;
        var mSalesOrderID;
        var mOnOk;
        var mOnCancel;
        var mData;
        var mDontAutoClose;
        var mIncludeInvoiced;
        var mDataTable;

        //$('body').append($(mHtml));
        //mElement = $('[idx="popOrderInput"]').attr('id', popupid); // popOrderInput only

        mElement = $(mHtml);
        mElement.attr('id', 'popRelatedOrders' + NewID());
        $('body').append(mElement);

        mOut.element = mElement;
        var mTable = mElement.find('table');
        
       //mOut.element.trigger('create');        

        mOut.popup = function(vParams) {
            
            mData = [];
            mSalesOrderID = vParams.ID;
            mOnOk = vParams.OnOk;
            mOnCancel = vParams.OnCancel;
            mDontAutoClose = vParams.DontAutoClose;
            mIncludeInvoiced = vParams.IncludeInvoiced;
            if (mIncludeInvoiced) {
                mElement.find('[name=butCancel]').hide();
                mElement.find('[name=divInvoiceNotes]').hide();
            } else {
                mElement.find('[name=butCancel]').show();
                mElement.find('[name=divInvoiceNotes]').show();
            }
            ClearTable();
            DisplayTable();
            mElement.find('[name=butOk]').hide();
             
        };        

        mElement.find('[name=butOk]').click(function() {
            if (mDontAutoClose)
                mElement.modal('hide');
            else
                onOk();
        });

        function onOk(vOnlyCloseIf1Finalized) {
            var pFinalized = getFinalized(mData);

            if ((CBool(mDontAutoClose))
                || (
                    (vOnlyCloseIf1Finalized)
                    && (mData.length != 1))
                )
                return false;

            mOnOk({
                ID: mSalesOrderID,
                Data: mData,
                IDs: pFinalized.IDs,
                OrderNumbers: pFinalized.OrderNumbers
            });
            mElement.modal('hide');

            return true;
        }
        
        function getFinalized(vRows) {
            var pIDs = '';
            var pOrderNumbers = '';
            var pIDsArray = [];

            for (var pRowIndex = 0; pRowIndex <= mData.length - 1; pRowIndex++) {
                var pRow = mData[pRowIndex];
                if (pRow.FulfillmentStatus != 'Finalized')
                    continue;
                if (pIDs != '') {
                    pIDs += ',';
                    pOrderNumbers += ',';
                }
                pIDs += pRow.ID;
                pIDsArray.push(pRow.ID);
                pOrderNumbers += pRow.SoMin;
            }

            return {
                IDs: pIDs,
                OrderNumbers: pOrderNumbers,
                IDsArray: pIDsArray
            };
        };

        mElement.find('[name=butCancel]').click(function() {
            mElement.modal('hide');
            mOnCancel();
        });

        function DisplayTable() {

            var pColumns = [
                {
                    title: 'Reference No.',
                    render: function(v1, v2, vData) {
                        return vData.SoMin;
                    }
                },
                {
                    title: 'SO Type',
                    render: function(v1, v2, vData) {
                        return vData.SoType;
                    }
                },
                {
                    title: 'Date Created',
                    render: function(v1, v2, vData) {
                        return FormatDate(vData.DateImported);
                    }
                },
                {
                    title: 'Fulfillment Status',
                    render: function(v1, v2, vData) {
                        return vData.FulfillmentStatus;
                    }
                },
                {
                    title: 'Location and Sub Area',
                    render: function(v1, v2, vData) {
                        return vData.LocationAndSubArea;
                    }
                }
            ];

            AjaxSend({
                url: '/Erp/RelatedSalesOrders',
                data: {
                    ID: mSalesOrderID,
                    IncludeInvoiced: mIncludeInvoiced
                },
                success: function(vData) {
                    mData = vData.Rows;

                    // BEGIN comment this to force showing the dialog
                    if (onOk(true))
                        return;
                    // END comment this to force showing the dialog

                    function showPopup() {
                        setTimeout(function() {
                            try {
                                mElement.modal('show');
                                //mElement.show().popup('open');
                            } catch (ex) {
                                // just ignore the error
                                showPopup();
                            }
                        }, 500);
                    }
                    showPopup();

                    //try {
                    //    mElement.show().popup('open');
                    //} catch (ex) {
                    //    // just ignore the error
                    //}

                    mDataTable = mTable.DataTable({
                        data: mData,
                        columns: pColumns
                    });
                    mTable.addClass('table-stripe').css('width', '100%')
                    mTable.find('thead tr').addClass('custom-theme-f');
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

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});