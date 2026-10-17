define([
    'promise!/Scripts/sources/ERP/Consignee/work/controller.js?v=' + assetversion
], function () {

    var mDeferred = $.Deferred();
    var mHtml;
    var mConsigneeHtml;    
    var mCustomerID;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/Consignee/main/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.Consignee.main.controller = ERP.Consignee.main.controller()
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.Consignee.main.controller', null, function () {

        var mOut = {};
        var mElement;

        //mElement = $("");

        //mElement = $(mHtmlConsignee);
        mElement = $(mHtml);
        mOut.element = mElement;

        //$('#tabReportsWork').append(mElementConsignee);//.trigger('create');      
        //$.mobile.activePage.append(mElementConsignee);
        $('#divConsignee').append(mElement);

        var ConsigneeWorkController = new ERP.Consignee.work.controller();
        
        //mOut.Show = function (vParam) {            

        //    mElementConsignee.show();
        //    $('.popup-content').scrollTop(0);
        //    mElementConsigneeInit();
            
        //}

        //$('#divInvoicingTab').find('[custom-tab-content="#tabInvoicingSettings"]').click(function(e) {

        //    if (mAllowedInInvoiceSettings)
        //        return;

        //    var pThis = this;

        //    e.stopPropagation();
        //    PromptUserAccess('Invoice Settings (EAS)', {
        //        OnClickOKSuccess: function () {
        //            mAllowedInInvoiceSettings = true;
        //            $(pThis).trigger('click');
        //        },
        //        OnFailed: function () {
        //            console.log('jsCFDTPD3GXC!');
        //        }
        //    });
        //});

        function mElementConsigneeInit() {
            GetAllTransferOut();

            $('#butSearchCustomer').click(function () {
                BrowsePopup('LibraryOrganization', 'Customers',
                    [
                        {
                            // Search: mTxtCustomer.val(), // commented to prevent auto search upon popup
                            DisplayText: [{ Column: 'CompanyName' }],
                            Click: function (e) {
                                $('#butSearchCustomer').text(e.CompanyName);
                                mCustomerID = e.ID;                                
                            }
                        }
                    ]
                ); // END BrowsePopup('LibraryFowst', 'Fowst',
            })

            $('#butConvertToInvoice').click(function () {
                ConvertTransferOutToInvoice();
            })
        }

        function GetAllTransferOut() {
            var data = {};

            AjaxSend({
                url: 'Receiving/GetStockTransferReport',
                data: data, 
                success: function (vData) {
                    DisplayAllTransferOut(vData.Results.Rows);
                }
            });
        }

        var datatableTransferOut;
        function DisplayAllTransferOut(vRows){
            if ($.fn.DataTable.isDataTable($('#tblConsignee'))) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                $('#tbodyConsignee').empty();
                datatableTransferOut.destroy();
            }

            var buttons = [];
            if (!IsMobile) {
                buttons.push({ extend: 'csvHtml5', text: 'Download CSV', title: 'Inv Details' })
            }

            var ctr = 0;
            datatableTransferOut = $('#tblConsignee').DataTable({
                pageLength: 25,
                "ordering": false,
                "data": vRows,
                "columns": [
                    {
                        "data": function (data) {
                            return CStr2(data.ProductNo)
                        }
                    },
                    {
                        "data": function (data) {
                            return CStr2(data.ProductName);
                        }
                    },
                    {
                        "data": function (data) {
                            return CDec2(data.GrossStockTransfer);
                        }
                    },
                    {
                        "data": function (data) {
                            return CDec2(data.GrossStockReceive)
                        }
                    },
                    {
                        "data": function (data) {
                            return CDec2(data.NetStockTransfer)
                        }
                    },
                     {
                         "data": function (data) {
                             return "<input type='number' ID=txt" + data.ID + " value=" + CDec2(data.NetStockTransfer) + " />"
                         }
                     },
                      {
                          "data": function (data) {
                              return AccountingFormat(data.Price)
                          }
                      },
                    {
                        "data": function (data) {
                            return AccountingFormat(data.LineTotal)
                        }
                    }
                ],
                dom: 'Brtip',
                buttons: buttons,
                initComplete: function () {
                    $('#tblConsignee').removeClass('dataTable');
                }
            });
        }

        function ConvertTransferOutToInvoice() {
            var items = [];

            $('#tblConsignee tbody tr').each(function () {
                items.push({
                    ProductID: datatableTransferOut.row(this).data().ID,
                    BillableQty: $('#txt' + datatableTransferOut.row(this).data().ID).val(),
                    UnitPrice: datatableTransferOut.row(this).data().Price,
                    UnitMeasureID: datatableTransferOut.row(this).data().UnitMeasure
                });
            });
            var data = {
                FOWSTID: sessionStorage.FowstID,
                CustomerID: mCustomerID,
                PeopleID: sessionStorage.PeopleID,
                Products: items
            };

            AjaxSend({
                url: 'Receiving/ConvertTransferOutToInvoice',
                data: data,
                success: function (vData) {
                    //DisplayAllTransferOut(vData.Results.Rows);
                }
            });
        }

        //$('#divOrderProcessingMain').append(ERP.OrderProcessing.work.controller.element);

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});