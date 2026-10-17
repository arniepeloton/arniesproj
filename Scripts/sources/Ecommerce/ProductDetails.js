require(['common'], function(){
    $(document).ready(function () {
        var productDetails;

        $(function () {
            AjaxSend({
                url: MergeUrlParameters(GetUrlParameters(), '/Ecommerce/GetProductDetails'),
                success: function (e) {
                    GetProductPictures(e);
                }
            })
        })

        $('body')
        .on('keydown', '.txtQty', function (e) {
            if(!NumberOnly(e)){
                return;
            }
        })
        .on('click', '#btnAddToCart', function () {
            if (sessionStorage.PeopleUserID == undefined) {
                NewMessageBox('Before you can add an item to your cart, you must login first.', 'Message', [
                    {
                        Text: 'Go to login',
                        Click: function () {
                            location.href = '/Ecommerce/Signin?OrgID=' + sessionStorage.OrgID
                        }
                    }
                ], {

                })
                return;
            }
            productDetails.Qty = $.trim($('#txtProductQty').val());
            if (sessionStorage.MyCart == undefined) {                    
                sessionStorage.MyCart = JSON.stringify([productDetails]);
            } else {
                var tempArr = JSON.parse(JSON.parse(JSON.stringify(sessionStorage.MyCart)));
                var isExist = false, ind = 0;

                $.grep(tempArr, function (e, i) {
                    if (e.ProductID == productDetails.ProductID) {
                        isExist = true;
                        ind = i;
                        return false; // stop
                    }
                })

                if(isExist){
                    tempArr[ind].Qty = $.trim($('#txtProductQty').val());
                } else {
                    tempArr.push(productDetails);
                }
                    
                sessionStorage.MyCart = JSON.stringify(tempArr);

                $(this).text('ADDING TO CART ...').attr('disabled', true);

                Save({
                    ByVendor: false,
                    Qty: productDetails.Qty,
                    Success: function() {
                        location.href = MergeUrlParameters({
                            OrgID: GetUrlParameters().OrgID
                        }, '/Ecommerce/ShoppingCart');
                    },
                    Fail: function(vMessage) {
                        NewMessageBox(vMessage);
                    }
                });
            }
        })
        .on('click', '.btnIncreaseQty', function () {
            var txtProductQty = $(this).prev(); //('#txtProductQty');

            txtProductQty.val(CInt2(txtProductQty.val()) + 1).change();
        })
        .on('click', '.btnDecreaseQty', function () {
            var txtProductQty = $(this).next(); //('#txtProductQty');

            if (CInt2(txtProductQty.val() == 0)) {
                return;
            }

            txtProductQty.val(CInt2(txtProductQty.val()) - 1).change();
        })
        .on('click', '#btnVendorAddtoCart', function () {
            if (sessionStorage.PeopleUserID == undefined) {
                NewMessageBox('Before you can add an item to your cart, you must login first.', 'Message', [
                    {
                        Text: 'Go to login',
                        Click: function () {
                            location.href = '/Ecommerce/Signin?OrgID=' + sessionStorage.OrgID
                        }
                    }
                ], {

                })
                return;
            }

            var isValid = false, selectedVendor = [];

            $('#tbodySkuVendor').find('.txtQty').each(function (e) {
                if(CDec2($(this).val()) != 0){
                    isValid = true;
                    selectedVendor.push($(this).data('data'));
                }
            })

            if(!isValid){
                NewMessageBox('No item(s) will be added to your cart. Please choose a vendor and add at least 1 order quantity.')
            } else {
                $(this).text('ADDING TO CART ...').attr('disabled', true);
                var pSuccessCount = 0;
                for (var i = 0; i < selectedVendor.length; i++) {
                    Save({
                        ByVendor: true,
                        Qty: selectedVendor[i].OrderQty,
                        VendorID: selectedVendor[i].VendorID,
                        Price: selectedVendor[i].Price,
                        UnitMeasure: selectedVendor[i].UnitMeasure,
                        Success: function () {
                            pSuccessCount++;
                            if (pSuccessCount == selectedVendor.length)
                                location.href = MergeUrlParameters({
                                    OrgID: GetUrlParameters().OrgID
                                }, '/Ecommerce/ShoppingCart');
                        },
                        Fail: function (vMessage) {
                            NewMessageBox(vMessage);
                        }
                    });
                }
            }
        })

        function Save(vParams) {
            var pData = {
                FromEcommerce: 1,
                //CustomerID: mOut.Model.Data.CustomerID,
                TransType: 'SalesOrder',
                DeleteExistingProduct: 0,
                DontCheckCartValidity: 1,
                //OrderID: mOut.Model.Data.ID,
                //NewCartMode: !IsGuid(mOut.Model.Data.ID),
                StockID: productDetails.ProductID,
                SalesRep: [],
                ProductSerial: [],
                UpdateOrderedQty: 1,
                Stocks: [
                    {
                        Qty: vParams.Qty,
                        UnitQty: 1
                    }
                ]
            };

            if (vParams.ByVendor) {
                pData.Stocks[0].VendorID = vParams.VendorID;
                pData.Stocks[0].EditPrice = vParams.Price;
                pData.Stocks[0].UnitMeasure = vParams.UnitMeasure;
            }

            AjaxSend({
                url: '/Stock/UpdatePos',
                data: pData,
                NoSplash: true,
                success: function(vData) {
                    if (vData.Error) {
                        vParams.Fail(vData.Error);
                    } else if (vData.Result == 'ok') {
                        vParams.Success(vData);
                    } else {
                        vParams.Fail('Unknown status. ref: jsCSLRU5TDDJ!');
                    }

                }
            });
        }

        function GetProductPictures(e) {
            //var e = JSON.parse(sessionStorage.ProductDetails).d; // declared in ProductDetails.vbhtml

            var big = e.Big.Rows;
            var medium = e.Medium.Rows;
            var small = e.Small.Rows;
            var arrVendor = (e.Vendor || undefined);
            var qtychoices = e.QtyChoices.Rows;
            var upcharge = e.Upcharge.Rows;
            productDetails = e.Details.Rows[0];
            var singleProductPic = $('#singleProductPic');
            var singleProductPicThumbnails = $('#singleProductPicThumbnails');
            var pQty = CDec2(e.Qty);
            if (pQty <= 0)
                pQty = 1;
            $('#txtProductQty').val(pQty);
            $('#lblProductName').text(productDetails.ProductName);
            $('#lblProductPrice').text('P ' + AccountingFormat(productDetails.Price) + ' per ' + productDetails.UnitMeasure);
            var pMarketingText = productDetails.MarketingText;
            if (CStr2(pMarketingText).trim() == '')
                pMarketingText = '[This product has no marketing description]';
            pMarketingText = HtmlEncode(pMarketingText);
            pMarketingText = pMarketingText.replace(/\n/g, '<br />');
            $('#pMarketingText').html(pMarketingText);

            singleProductPic.empty().append($('<img src="/Content/Ecommerce/images/products/p1.jpg" style="max-width:100%; height:200px;">'));
            singleProductPicThumbnails.empty();

            if (big.length > 0) {
                singleProductPic.empty();
                for (var i = 0; i < big.length; i++) {                    
                    var fileLocation = "/Content/MobizImagePIM/" + sessionStorage.OrgID.toLowerCase() + '/big/' + big[i].FileName;
                    singleProductPic.append($('<div>')
                            .append($('<img class="img-responsive" alt="" src="' + fileLocation + '" data-echo="' + fileLocation + '">')
                                .attr('onerror', 'this.onerror=null;this.src="/Content/Ecommerce/images/products/p1.jpg"')
                                .css({ margin: 'auto' })
                            )
                        )
                }

                singleProductPic.bxSlider({
                    auto: false,
                    autoControls: true,
                    stopAutoOnClick: true,
                    pager: true,
                    slideWidth: 400,
                    adaptiveHeight: true,
                    touchEnabled: true,
                    responsive: true,
                    onSliderLoad: function () {
                        $('.bx-viewport').css('height', '326px');
                        $('.bxslider > div').css('width', '326px');
                    }
                });
            }

            if (medium.length == 0) {
                for (var i = 0; i < medium.length; i++) {
                    if (CBool(medium[i].IsDefault)) {
                        productDetails.FileName = medium[i].FileName;
                    }

                    var fileLocation = "/Content/MobizImagePIM/" + sessionStorage.OrgID + '/medium/' + medium[i].FileName;
                    singleProductPicThumbnails.append($('<div class="owl-item">')
                            .append($('<div class="item">')
                                .append($('<a data-role="none" class="horizontal-thumb active" data-target="#singleProductPic" data-slide="1" href="#' + big[i].ID + '">')
                                    .append($('<img class="img-responsive" width="85" src="' + fileLocation + '" data-echo="' + fileLocation + '">')
                                        .attr('onerror', 'this.onerror=null;this.src="/Content/Ecommerce/images/products/p1.jpg"')
                                    )
                                )
                            )
                        )
                }
            }

            var qtystyle = $('<div class="input-group mb-3">')
                                .append($('<button data-role="none" class="btn btn-danger btnDecreaseQty" data-toggle="tooltip" data-placement="bottom" title="" data-original-title="Decrease">').append($('<span class="bi-dash">')))
                                .append($('<input data-role="none" id="txtProductQty" type="text" value="1" class="form-control form-control-lg txtQty">'))
                                .append($('<button data-role="none" class="btn btn-success btnIncreaseQty" data-toggle="tooltip" data-placement="bottom" title="" data-original-title="Increase">').append($('<span class="bi-plus">')))

            if (qtychoices.length > 0) {
                qtystyle = $('<form autocomplete="off" style="width:100%; position:relative;">')
                qtystyle.append($('<input data-role="none" type="number" autocomplete="off" id="txtProductQty" class="form-control unicase-form-control" style="width:100%;">')
                        .data('choices', qtychoices)
                        .focusin(function () {
                            $(this).next().show();
                        })
                        .blur(function () {
                            var dis = $(this);
                            setTimeout(function () {
                                dis.next().hide();
                            }, 150)
                        })
                        .keyup(function () {
                            var keyword = $.trim($(this).val()).toLowerCase();
                            var choices = $(this).data('choices');
                            var listQtyInterval = $(this).next();
                            listQtyInterval.empty();

                            if (keyword != '') {
                                $.grep(choices, function (e) {
                                    if (e.QtyChoices.toLowerCase().indexOf(keyword) > -1) {
                                        listQtyInterval.append(Ecommerce.DisplayQtyInterval(e))
                                    }
                                });
                                listQtyInterval.show();
                            } else {
                                for (var i = 0; i < choices.length; i++) {
                                    divsuggestion.append(Ecommerce.DisplayQtyInterval(choices[i]))
                                }
                            }
                        })
                    );

                var divsuggestion = $('<div class="list-group list-autosuggest qtyintervallist">')

                for (var i = 0; i < qtychoices.length; i++) {
                    divsuggestion.append(Ecommerce.DisplayQtyInterval(qtychoices[i]));
                }

                qtystyle.append(divsuggestion);
            }            

            if (arrVendor != undefined) {
                arrVendor = arrVendor.Rows;

                var tbodySkuVendor = $('#tbodySkuVendor');
                tbodySkuVendor.empty();

                $('#divForVendorProduct').show();

                if (arrVendor.length > 0) {
                    $('#btnVendorAddtoCart').show();

                    if (arrVendor[0].VendorName == undefined) {
                        $('#tdVendorName').hide();
                    } else {
                        $('#tdVendorName').show();
                    }

                    for (var i = 0; i < arrVendor.length; i++) {
                        arrVendor[i].ProductID = productDetails.ProductID;
                        var img = '';

                        if (arrVendor[i].FileName != undefined) {
                            img = ImageViewer.DefaultImage('/Content/MobizImagePIM/' + sessionStorage.OrgID.toLowerCase() + '/Medium/' + arrVendor[i].VendorID + '/' + arrVendor[i].FileName, arrVendor[i]);
                        }

                        tbodySkuVendor.append($('<tr>')
                                .data('data', arrVendor[i])
                                .append($('<td>').append(img))
                                .append($('<td>').append($('<b class="ui-table-cell-label">').text('Vendor Code:')).append(arrVendor[i].VendorCode))
                                .append($('<td ' + (arrVendor[0].VendorName == undefined ? 'style="display:none"' : '') + '>').append($('<b class="ui-table-cell-label">').text('Vendor Name:')).append($.trim(arrVendor[i].VendorName || '')))
                                .append($('<td class="txt-center">').append($('<b class="ui-table-cell-label">').text('Price:')).append(AccountingFormat(arrVendor[i].Price)))
                                .append($('<td class="txt-center">').append($('<b class="ui-table-cell-label">').text('Unit Measure:')).append(arrVendor[i].UnitMeasure))
                                .append($('<td>').append($('<b class="ui-table-cell-label">').text('Order Quantity:')).append($('<div style="display:inline-flex;">')
                                        .append($('<button  data-role="none" id="" style="width:50px; height:45px;" class="btn custom-btn-theme-c btn-less-width btn-sm btnDecreaseQty">')
                                            .append($('<span class="glyphicon glyphicon-minus">'))
                                        )
                                        .append($('<input data-role="none" style="width:80px; height:45px;" type="text" value="0" class="form-control unicase-form-control txtQty">')
                                            .data('data', arrVendor[i])
                                            .change(function () {
                                                var qtyfield = $(this).val();
                                                var data = $(this).data('data');
                                                data.OrderQty = CDec2(qtyfield);
                                                $(this).parents().closest('tr').find('td.linetotal').text(AccountingFormat(CDec2(qtyfield) * CDec2(data.Price)));
                                            })
                                        )
                                        .append($('<button  data-role="none" id="" style="width:50px; height:45px;" class="btn custom-btn-theme-c btn-less-width btn-sm btnIncreaseQty">')
                                            .append($('<span class="glyphicon glyphicon-plus">'))
                                        )
                                    )
                                )
                                .append($('<td class="linetotal txt-center">').append($('<b class="ui-table-cell-label">').text('Line Total:')).append('-'))
                            )
                    }
                } else {
                    $('#btnVendorAddtoCart').hide();
                    tbodySkuVendor.append($('<tr>').append($('<td colspan="7" class="txt-center">').text('No vendors sell this product')));
                }
            } else {
                if (upcharge.length > 0) {
                    if (CDec2(upcharge[0].Upcharge) != 0) {
                        $('#divUpchargeNote').html('<b>Note:</b> Due to additional processing and handling, purchases less than 1 ' + productDetails.UnitMeasure + ' has a ' + CDec2(upcharge[0].Upcharge) + '% upcharge. Thank you.')
                    }
                }
                $('#divQtyStyleOption').append(qtystyle);
                $('#divForCompanyProduct').show();
            }
        }
    })          
})