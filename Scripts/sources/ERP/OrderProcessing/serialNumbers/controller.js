define([
    '/Scripts/sources/ERP/OrderProcessing/serialNumbers/model.js'
], function () {

    var mDeferred = $.Deferred();
    var mHtml, isLoaded = false;
    var mOut = {};

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/OrderProcessing/serialNumbers/view.html',
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

    CreateStringObject('ERP.OrderProcessing.serialNumbers.controller', null, function () {
        if(isLoaded){
            return mOut;
        }
        
        var mElement = $(mHtml);
        var mTransType;
        var mProductNo;
        var mProductName;
        var mParentController;
        var mTxtAdd = mElement.find('[name=txtAdd]');
        var arrSerialNumber = [], arrImei = [], maxInput = 0, ctrSerialNumber = 0, ctrImei = 0;
        var popuOrderInputID = NewID();      

        $('body').append(mElement);

        if (proj.IsBootstrapDesign()){
            $('#divSerialNumberspopup').find('.modal').attr('id', 'popOrderInput' + popuOrderInputID);
            $('#divSerialNumberspopup').find('[data-role="popup"] > [data-role="content"] > *').appendTo('#popOrderInput' + popuOrderInputID + ' .modal-body');
            $('#divSerialNumberspopup').find('[data-role="popup"]').remove();
        } else {
            $('#divSerialNumberspopup').find('[data-role="popup"]').attr('id', 'popOrderInput' + popuOrderInputID);
            $('#divSerialNumberspopup').find('.modal').remove();
        }

        isLoaded = true;
        mOut.element = mElement;
        mOut.Model = null;
        mOut.AfterSave = function(){};
        var mTable = mElement.find('[name=tableSerials]');
        var mTR = mTable.find('tbody tr:first');

        //mOut.element.trigger('create');

        mElement.find('#txtOrderInputSerialNumber').keyup(function (e) {
            var val = CStr2($.trim($(this).val()));
            if (e.which == 13 && val != '') {                
                DisplaySnImei({ Serial: val, IsDelete: false }, 'serial')
                $(this).val('').focus();
                PopupReposition();
            }
        })

        mElement.find('#txtOrderInputIMEI').keyup(function (e) {
            var val = CStr2($.trim($(this).val()));
            if (e.which == 13 && val != '') {
                DisplaySnImei({ Imei: val, IsDelete: false }, 'imei')
                $(this).val('').focus();
                PopupReposition();
            }
        })

        function DisplaySnImei(data, type) {
            var ul, num;

            if(type == 'serial'){
                if(ctrSerialNumber == maxInput){
                    WebToast('Maximum of ' + maxInput + ' serial numbers only', 'top');
                    return;
                }
                ctrSerialNumber += 1;
                ul = $('#listOrderInputSerialNumber');
                arrSerialNumber.push(data)
                num = data.Serial;
                $('#lblTotalSerialNumber').text(ctrSerialNumber);
            } else {
                if(ctrImei == maxInput){
                    WebToast('Maximum of ' + maxInput + ' IMEIs only', 'top');
                    return;
                }
                ctrImei += 1;
                ul = $('#listOrderInputIMEI');
                arrImei.push(data);
                num = data.Imei;
                $('#lblTotalImei').text(ctrImei);
            }

            ul.append($('<li class="list-group-item">')
                    .append($('<div style="display:table; width:100%;">')
                        .append($('<div style="display:table-cell; vertical-align:middle; padding:5px;">')
                            .append(num)
                        )
                        .append($('<div style="display: table-cell; text-align:right; padding:5px; vertical-align:middle; width:50px;">')
                            .append($('<button data-role="none" style="visibility:hidden;" class="btn custom-btn-theme-d btn-danger btnCircle-xs">')
                                .data('data', data)
                                .append($('<span class="glyphicon glyphicon-remove bi-trash">'))
                                .click(function (e) {
                                    e.preventDefault();
                                    e.stopPropagation();

                                    var dis = $(this);
                                    var data = dis.data('data');

                                    NewMessageBox('Are you sure you want to remove record <b>"' + (data.Serial || data.Imei) + '"</b>? <br><br>' +
                                        'If Yes, you cannot undo this deletion.', 'Message', [
                                        {
                                            Text: 'Yes', Click: function () {
                                                data.IsDelete = true;
                                                dis.parents().closest('li').remove();

                                                if (CStr2(data.Serial) != ''){
                                                    ctrSerialNumber -= 1;
                                                    $('#lblTotalSerialNumber').text(ctrSerialNumber);
                                                } else {
                                                    ctrImei -= 1;
                                                    $('#lblTotalImei').text(ctrImei);
                                                }
                                            }
                                        }, { Text: 'No' }
                                        ], {
                                            Close: function(){
                                                if (proj.IsBootstrapDesign()){
                                                    $('#popupSalesReturn').modal('show');
                                                    setTimeout(function(){
                                                        $('#popOrderInput' + popuOrderInputID).modal('show');
                                                    }, 500)
                                                } else {
                                                    mElement.show().popup('open');
                                                }
                                                                                                                                                
                                            }
                                        })
                                })                                
                            )
                        )
                    )
                    .hover(function() {
                        $(this).find('button').css('visibility', 'visible');
                    }, function() {
                        $(this).find('button').css('visibility', 'hidden');
                    })
                )
        }

        mOut.popup = function(vParams) {
            mParentController = vParams.ParentController;
            mProductNo = vParams.ProductNo;
            mOut.Model.Data.ID = vParams.InvoiceID;
            mOut.Model.Data.ProductID = vParams.ProductID;
            mOut.Model.Data.CustomerID = vParams.CustomerID;
            //mTransType = vParams.TransType;
            //if ((mTransType == 'PurchaseOrder') 
            //    || (mTransType == 'AccountsPayable')) {

            //    mElement.find('.PriceLevel').hide();
            //    if (mTransType == 'AccountsPayable') {
            //        mElement.find('th[name=thOrderQty]').text('Qty to be Paid');
            //        mElement.find('th.FreeTypePrice').text('Unit Price to be Paid');
            //    } else {
            //        mElement.find('th.FreeTypePrice').text('PO Price');
            //    }
            //} else {
            //    mElement.find('.PriceLevel').show();
            //    mElement.find('th.FreeTypePrice').text('Net Price per Selling Unit');
            //}
            
            //switch (mTransType) {
            //    case 'Quote':
            //        proj.CacMode = 'AR';
            //        break;

            //    case 'SalesOrder':
            //        proj.CacMode = 'AR';
            //        break;
                
            //    case 'Invoice':
            //        proj.CacMode = 'AR';
            //        break;
                
            //    case 'PurchaseOrder':
            //        proj.CacMode = 'AP';
            //        break;

            //    case 'AccountsPayable':
            //        proj.CacMode = 'AP';
            //        break;

            //}

            mProductName = vParams.ProductName;
            mElement.find('[name=title]').text(mProductName);
            mElement.find('[name=spanProductNumber]').text(vParams.ProductNo);
            mTxtAdd.val('');

            Clear();
            
            //mOut.Model.GetData({
            //    Success: function(vData) {
            //        vParams.Success();
            //        DisplayData(vData);
            //        HandleFreeTypePriceElements();
            //        
            //    },
            //    Fail: function(vMessage) {
            //        NewMessageBox(vMessage);
            //    }
            //});
            ReloadData();

            if (proj.IsBootstrapDesign()){
                $('#popOrderInput' + popuOrderInputID).modal('show');
            } else {
                mElement.show().popup('open');
            }
            
            mElement.find('#txtOrderInputSerialNumber').val('').focus();
            mElement.find('#txtOrderInputIMEI').val('');
        }; // END mOut.popup = function(vParams) {
        
        function Clear() {            
            mTable.find('tbody tr').remove();
            mElement.find('#listOrderInputSerialNumber').empty();
            mElement.find('#listOrderInputIMEI').empty();
            arrImei = [];
            arrSerialNumber = [];
            ctrSerialNumber = 0;
            ctrImei = 0;
            maxInput = 0;
            $('#lblTotalSerialNumber, #lblTotalImei').text(0);
        }

        async function ReloadData() {
            var pResult = await mOut.Model.GetData();
            if (pResult.Result == 'ok') {
                maxInput = CDec2(pResult.TotalBaseUnitQty.Rows[0].TotalBaseUnitQty);
                DisplayData(pResult.data.Rows);
                $('.lblTotalBaseUnitQty').text(maxInput);
            } else
                NewMessageBox(pResult.Error, '', [
                    {
                        Text: 'Ok',
                        Click: function() {
                            setTimeout(function() {
                                if (proj.IsBootstrapDesign()){
                                    $('#popOrderInput' + popuOrderInputID).modal('show');
                                } else {
                                    mElement.show().popup('open');
                                }
                            }, 500);
                        }
                    }
                ]);
        }

        function DisplayData(vData) {            
            for (var pIndex = 0; pIndex <= vData.length - 1; pIndex++) {
                //var pDR = vData[pIndex];
                //AddRow(pDR);
                if(CStr2(vData[pIndex].Serial) != ''){
                    DisplaySnImei(vData[pIndex], 'serial');                    
                }

                if(CStr2(vData[pIndex].Imei) != ''){                    
                    DisplaySnImei(vData[pIndex], 'imei');                    
                }
            }

            PopupReposition();
        }


        mElement.find('[name=butAdd]').click(function() {
            if (mTxtAdd.val().trim() == '') {
                NewMessageBox('Please enter the serial number first.', '', [
                        {
                            Text: 'Ok',
                            Click: function() {
                                setTimeout(function() {
                                    if (proj.IsBootstrapDesign()){
                                        $('#popOrderInput' + popuOrderInputID).modal('show');
                                    } else {
                                        mElement.show().popup('open');
                                    }
                                }, 500);
                            }
                        }
                    ]);
                return;
            }
            var pDR = {
                SerialNumber: mTxtAdd.val().trim()
            };
            AddRow(pDR);
            mTxtAdd.val('');
        }); // END mElement.find('[name=butAdd]').click(function() {

        mElement.find('[name=butOk]').click(async function() {
            var pDRs = [];
            var pTRs = mTable.find('tr[name=data]');
            for (var pRowIndex = 0; pRowIndex <= pTRs.length - 1; pRowIndex++) {
                var pTR = $(pTRs[pRowIndex]);
                var pDR = {
                    Serial: pTR.find('[name=txtSerial]').val().trim()
                }
                pDRs.push(pDR);
            }

            var pResult = await mOut.Model.Save({
                InvoiceID: mOut.Model.Data.ID,
                ProductID: mOut.Model.Data.ProductID,
                DRs: pDRs,
                SerialNumbers: arrSerialNumber,
                IMEIs: arrImei
            });
            
            if (pResult.Result == 'ok') {
                //NewMessageBoxClose();
                WebToast('Serial numbers were successfully saved', 'top');
            } else
                NewMessageBox(pResult.Error, '', [
                    {
                        Text: 'Ok',
                        Click: function() {
                            setTimeout(function() {
                                if (proj.IsBootstrapDesign()){
                                    $('#popOrderInput' + popuOrderInputID).modal('show');
                                } else {
                                    mElement.show().popup('open');
                                }
                            }, 500);
                        }
                    }
                ]);
                
        });

        function AddRow(vDR) {
            var pTR = mTR.clone();
            var pDR = vDR;
            pTR.data('data', pDR);
            var pTxtSerial = pTR.find('[name=txtSerial]');
            pTxtSerial.val(vDR.SerialNumber);
            mTable.append(pTR);
        }

        mOut.Init = function() {
            mOut.Model = new ERP.OrderProcessing.serialNumbers.model();
        };

        mOut.Init();

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});