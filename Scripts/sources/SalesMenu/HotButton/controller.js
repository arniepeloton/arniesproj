define([
    '/Scripts/sources/SalesMenu/HotButton/model.js?v=' + assetversion,
    '/Scripts3/ItemModifiers.js?v=' + assetversion,
    '/Scripts3/Bs_SalesMenu.js',
    '/Scripts3/Bs_HotButtons.js',
], function () {

    var mDeferred = $.Deferred();
    var mHtml, isLoad = false;
    var mOut = {};

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/SalesMenu/HotButton/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.WebPOS.hb.controller = ERP.WebPOS.hb.controller()
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.WebPOS.hb.controller', null, function () {
                
        var mElement = $(mHtml), hbtemplateid = null, arrHotButtons, arrHBTemplates = [];
        var arrSelectedProducts = [];
        mOut.element = mElement;
        
        $('[name="maincontainer"]').append(mHtml);

        var listCartItem = $('[name="listCartItem"]');

        mOut.popup = function (vParams) {
            $('[name="divRetail"], [name="divPOSViewLoading"]').hide();
            $('[name="divLine"]').hide();
            $('#formPunchItem').parent().hide();
            $('[name="divHotButtons"]').show();

            mOut.HBParams = vParams;
            mOut.Clear();
            GetFowstHBTemplate();
            //$('#popWebPOSHB').modal('show');
        }

        mOut.Clear = function () {
            $('[name="navHeadHB"] button:first').click();
            $('[name="lblHBCartTotal"]').text('0.00');
            arrSelectedProducts = [];
            $('#tblSideCartItems tbody').remove();
        }

        $('[name="btnHBOk"]').click(function () {
            mOut.HBParams.success(arrSelectedProducts);
            $('#popWebPOSHB').modal('hide');
        })

        $('[name="btnHBSettings"]').click(function () {
            PopupButtonList('', 'Options', [
                {
                    Text: 'Change Hot Button Template', Click: function () {
                        var opt = [];
                        for (var i = 0; i < arrHBTemplates.length; i++) {
                            opt.push({
                                data: arrHBTemplates[i],
                                Text: arrHBTemplates[i].Name,
                                Click: function (e) {
                                    hbtemplateid = e.ID;
                                    NewGetHotButtons();
                                }
                            })
                        }
                        PopupButtonList('', 'Choose Hot Button Template', opt);
                    }
                }
            ])
        })

        function GetFowstHBTemplate() {
            if (hbtemplateid != null) return;

            AjaxSend({
                url: '/ProductGroup/GetFowstHBTemplate',
                success: function (e) {
                    if (e.DefaultTemplate != undefined) {
                        hbtemplateid = e.DefaultTemplate;                        
                    }

                    if (e.Templates != undefined) {
                        arrHBTemplates = e.Templates.Rows;
                        hbtemplateid = arrHBTemplates[0].ID;
                    }

                    if (hbtemplateid == null) {
                        $('[name="nohb"]').show();
                        $('[name="transhotbuttons"]').remove();
                    } else {
                        $('[name="nohb"]').remove();
                        $('[name="transhotbuttons"]').show();
                    }

                    NewGetHotButtons();
                }
            })
        }

        function NewGetHotButtons() {
            AjaxSend({
                url: '/ProductGroup/NewGetHotButtons',
                data: { MenuID: hbtemplateid, IncludeUnitPrice: true, FromWebPOS: true },
                success: function (e) {
                    arrHotButtons = e.Results;
                    $('[name="navHeadHB"]').empty();
                    PopulateHotButtons(arrHotButtons, 'Main');
                }
            })
        }

        function PopulateHotButtons(vData, navName) {
            var i = 0; len = vData.length;
            var navHeadHB = $('[name="navHeadHB"]'), navBodyHB = $('[name="navBodyHB"]');

            navHeadHB.find('button').removeClass('active');
            navHeadHB.append($('<div class="col-3">')
                        .append($('<button data-role="none" class="btn btn-success btn-block darker active border-dark rounded-0 h-100">')
                            .data('data', vData)
                            .text(navName)
                            .click(function () {
                                var dis = $(this), data = dis.data('data');
                                PopulateHotButtons(data, dis.text());
                                dis.parent().nextAll().remove();
                                dis.addClass('active');
                            })
                        )
                    )

            navBodyHB.empty();

            for (i; i < len; i++) {
                var data = vData[i];
                if (data.LineItemID == undefined)
                    data.LineItemID = NewID();
                navBodyHB.append($('<div class="col">')
                        .append($('<button data-role="none" class="btn btn-dark btn-block darker border-dark rounded-0 h-100">')
                            .data('data', data)
                            .text(data.Name + ' ' + (CBool(data.IsGroup) ? '*' : ''))
                            .click(function() {
                                var data = $(this).data('data');                                

                                if (CBool(data.IsGroup)) {
                                    sessionStorage.DefaultOtherPaymentID = data.DefaultOtherPaymentID;
                                    if (!CBool(data.IsFinishLoadData)) {
                                        Bs.Loading('show');
                                        AjaxSend({
                                            url: 'ProductGroup/GetSubGroup',
                                            data: {
                                                OrgID: sessionStorage.OrgID,
                                                MenuID: hbtemplateid,
                                                GroupID: data.ID,
                                                IncludeSubGroup: false,
                                                Level: CInt2(data.Level) + 1,
                                                IncludeUnitPrice: true,
                                                FromWebPOS: true
                                            }, success: function (e) {
                                                Bs.Loading('hide');
                                                data.SubGroup = $.merge(data.SubGroup, e.Results);
                                                data.IsFinishLoadData = true;
                                                PopulateHotButtons(data.SubGroup, data.Name);
                                            }
                                        })
                                    } else {
                                        PopulateHotButtons(data.SubGroup, data.Name);
                                    }
                                } else {
                                    function Go(data) {
                                        Save(data);
                                        proj.HotButtonDisplayStock(data);
                                        //ComputeCartTotal();                                        

                                        var isExist = false, index = null;

                                        $.grep(arrSelectedProducts, function (arr, ind) {
                                            if (arr.ID == data.ID) {
                                                index = ind;
                                                return false;
                                            }
                                        })

                                        if (index == null) {
                                            arrSelectedProducts.push(data);
                                        }
                                    }

                                    data.ProductName = data.Name; // use for Item Modifier;
                                    data.UnitPrice = data.RetailPrice;

                                    if (CBool(data.HasModifier)) {                                        
                                        NewItemModifier({
                                            AllowSKUQty: true,
                                            ProductData: data,
                                            success: function (e) {
                                                console.log(e);
                                                data.GroupIdOfItemModifiers = NewID();
                                                data.ItemModifier = e.ItemModifier;
                                                data.SelectedItemModifierID = e.SelectedItemModifierID;
                                                data.OrderQty = e.Quantity;
                                                data.Quantity = data.OrderQty;
                                                Go(data);
                                            }
                                        });
                                    } else {
                                        data.ItemModifier = [];
                                        data.OrderQty = CInt2(data.OrderQty) + 1;
                                        data.Quantity = data.OrderQty;
                                        Go(data);
                                    }                                    
                                }
                            })
                        )
                    )
            }
        }

        function DisplayInCart(data) {
            var pRow = $('<div class="row g-0">'), colSKU = $('<div class="col p-2">'),
                mainSKUTotal = data.OrderQty * data.RetailPrice;

            pRow.append($('<div class="col-2 align-self-center text-center">')
                    .append('<span style="font-size: 40px;">'+ data.OrderQty +'</span><br>' + data.UnitMeasure)
                )

            colSKU.append($('<div class="row g-0 mb-3" style="font-size:16px;">')
                    .append($('<div class="col border-right">').text(data.Name + ' @ ' + data.RetailPrice + ' each'))
                    .append($('<div class="col-4 cartListTotal text-right">').text(AccountingFormat(mainSKUTotal)))
                )

            if (data.SelectedItemModifierID.length > 0) {
                var imTotal = 0;
                colSKU.find('.cartListTotal').removeClass('cartListTotal');
                for (var i = 0; i < data.ItemModifier.length; i++) {
                    for (var m = 0; m < data.ItemModifier[i].SelectedModifier.length; m++) {
                        var im = data.ItemModifier[i].SelectedModifier[m], imsubtotal = CDec2(im.OrderQty) * im.UnitPrice;
                        imTotal += imsubtotal;
                        colSKU.append($('<div class="row g-0" style="font-size:16px;">')
                            .append($('<div class="col border-right">').html('<span class="required">+</span> ' + im.OrderQty + ' x ' + im.Description + ' @ ' + AccountingFormat(im.UnitPrice) + ' each'))
                            .append($('<div class="col-4 text-right">').text(AccountingFormat(imsubtotal)))
                        )
                    }
                }
                mainSKUTotal += imTotal;
                colSKU.append($('<div class="row g-0 font-weight-bold" style="font-size:16px;">')
                            .append($('<div class="col border-top text-right">').text('SUBTOTAL:'))
                            .append($('<div class="col-4 cartListTotal border-top text-right">').text(AccountingFormat(mainSKUTotal)))
                        )
            }

            pRow.append(colSKU).append($('<div class="col-auto p-1 align-self-center">')
                    .append($('<button data-role="none" class="btn btn-danger"><span class="bi bi-trash"></span></button>')
                        .data('data', data)
                        .click(function () {
                            var data = $(this).data('data');

                            NewMessageBox('Are you sure you want to remove item <b>' + data.Name + '</b>?', 'Confirmation Dialog', [
                                {
                                    Text: 'Yes', Click: function () {
                                        $('[lineitemid="' + data.LineItemID + '"]').remove();
                                        var isExist = false, index = null;

                                        $.grep(arrSelectedProducts, function (arr, ind) {
                                            if (arr.ID == data.ID) {
                                                index = ind;
                                                return false;
                                            }
                                        })

                                        if (index != null) {
                                            arrSelectedProducts.splice(index, 1);
                                        }
                                    }
                                }, { Text: 'No' }
                            ])                            
                        })
                    )
                )

            var li = $('li[productid="' + data.ID + '"]');
            if (li.length > 0) {
                li.remove();
            }

            listCartItem.prepend($('<li class="list-group-item list-group-item-action p-1">').attr({
                'productid': data.ID,
                'lineitemid': data.LineItemID
            }).append(pRow));
            $('[name="divCartItem"]').scrollTop(0);
        }

        mOut.ComputeCartTotal = ComputeCartTotal
        function ComputeCartTotal() {
            var total = 0;
            $('.cartListTotal').each(function () {
                total += CDec2($(this).text());
            })
            $('#webPosCartSubTotal').text(AccountingFormat(total));            

            if (CDec2($('#lblPaidDiscounts').text()) > 0) {
                //CacPopup.Discount.GetSelectedDiscount($.trim($('[name="selDiscountType"]').val()));
                CacPopup.Discount.ShowSalesPaymentsInfo();
            } else {
                $('#webPosCartTotal').text(AccountingFormat(total));
                ERP.Payment.controller.ComputeTotalPaymentRcv();
            }            
        }

        mOut.Save = Save
        function Save(vParams) {
            if (CStr2(mOut.HBParams.WebPos.Model.Data.ID) == '') {
                mOut.HBParams.WebPos.SaveAsDraft({
                    NoPrompt: true,
                    Success: function() {
                        Save(vParams);
                    }
                });
                return;
            } 

            var pData = {
                CustomerID: mOut.HBParams.WebPos.Model.Data.CustomerID,
                TransType: mOut.HBParams.WebPos.Model.Data.TransType,
                EditMode: 1,
                DontCheckCartValidity: 1,
                OrderID: mOut.HBParams.WebPos.Model.Data.ID,
                NewCartMode: !IsGuid(mOut.HBParams.WebPos.Model.Data.ID),
                ProductID: vParams.ProductID,
                StockID: vParams.ProductID,
                SalesRep: [],
                ProductSerial: [],
                UpdateOrderedQty: 1,
                IsPos: 1,
                Stocks: [
                    {
                        Qty: vParams.Quantity,
                        UnitQty: vParams.UnitQty,
                        EditPrice: vParams.UnitPrice,
                        UnitMeasure: vParams.UnitMeasure,
                        ItemModifier: vParams.ItemModifier
                    }
                ]
            };

            if (!IsGuid(pData.CustomerID)) {
                NewMessageBox('Invalid/missing CustomerID: ' + pData.CustomerID + '. This known bug is still unresolved. Workaround: reload this page and try again. ref: jsNGEPR7QUPZ');
                console.trace();
                return;
            }
            //Bs.Loading('show');
            AjaxSend({
                url: '/Stock/UpdatePosLite',
                data: pData,
                NoSplash: true,
                success: function (vData) {
                    if (vData.Error) {
                        vParams.Fail(vData.Error);
                    } else if (vData.Result == 'ok') {
                        mOut.HBParams.WebPos.Model.Data.ID = vData.OrderID;
                        if (vParams.Success != undefined) vParams.Success(vData);
                    } else {
                        //vParams.Fail('Unknown status. ref: jsCSLRU5TDDJ!');
                    }
                },
                complete: function() {
                    Bs.Loading('hide');
                }
            });
        }

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});