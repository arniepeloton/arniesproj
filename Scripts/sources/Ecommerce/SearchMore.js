require(['common'],
function () {    
    $(document).ready(function () {
        var from = 1, to = 20;
        var divContainer = $('<div>');
        var ctrCell = 0, divRow = $('<div class="row">');

        window.Ecommerce = {
            DisplaySearch: LoadData
        }

        $(function () {
            AjaxSend({
                url: MergeUrlParameters(GetUrlParameters(), '/Ecommerce/GetProductSearchMore'),
                success: function (e) {
                    LoadData(e.MoreProducts);
                }
            })
        })

        function LoadData(moreprod) {
            var MoreProducts = moreprod;
            var vRows = (MoreProducts.Rows || MoreProducts);
            var divSearchedMoreProduct = $('#divSearchedMoreProduct');
            var divRow = $('<div class="row">');
            var arrCat = [];

            divSearchedMoreProduct.empty();
            $('#txtHeaderSearchField').val(CStr2(GetUrlParameters().keywords))

            if (vRows.length >= 1) {
                for (var p = 0; p < vRows.length; p++) {
                    var pProduct = vRows[p];
                    var tempPic = '/Content/Ecommerce/images/products/p1.jpg'

                    if (CStr2(pProduct.FileName) != '') {
                        tempPic = '/Content/MobizImagePIM/' + sessionStorage.OrgID + '/' + pProduct.FileSize + '/' + pProduct.FileName;
                    }

                    divRow.append($('<div class="col-sm-3 txt-center">')
                            .append($('<div class="eProducts">')
                                .append($('<a data-role="none">')
                                    .attr({
                                        href: MergeUrlParameters({
                                            OrgID: GetUrlParameters().OrgID,
                                            pname: pProduct.ShortName, // Product Name
                                            pid: pProduct.ProductID
                                        }, location.origin + '/Ecommerce/ProductDetails')
                                    })
                                    .data('data', pProduct)
                                    .append($('<div class="product" style="overflow-wrap: break-word;">')
                                        .append($('<div class="product-image">')
                                            .append($('<img style="margin:auto; max-width:200px;">')
                                                .attr('src', tempPic)
                                                .attr('onerror', 'this.onerror=null;this.src="/Content/Ecommerce/images/products/p1.jpg"')
                                            )
                                        )
                                        .append($('<div class="product-info text-left" style="margin:0px 5px; word-break: break-word;">')
                                            .append($('<h3 class="name">')
                                                .append($('<a>').text((pProduct.ShortName || pProduct.ProductName)))
                                            )
                                            .append($('<div class="product-price">')
                                                .append($('<spa class="price">').text('P ' + AccountingFormat(pProduct.Price) + ' per ' + pProduct.UnitMeasure))
                                            )
                                        )
                                    )
                                    //.append($('<div>')
                                    //    .append($('<button data-role="none" class="btn btn-primary btn-less-width">').text('ADD TO CART')
                                    //        .click(function (e) {
                                    //            e.stopPropagation();
                                    //            e.preventDefault();
                                    //        })
                                    //    )
                                    //)
                                    .click(function (e) {
                                        e.stopPropagation();
                                        e.preventDefault();
                                        var data = $(this).data('data');
                                        sessionStorage.ProductDetails = JSON.stringify(data);
                                        location.href = MergeUrlParameters({
                                            OrgID: GetUrlParameters().OrgID,
                                            pname: data.ShortName, // Product Name
                                            pid: data.ProductID
                                        }, '/Ecommerce/ProductDetails')
                                    })
                                )
                            )
                        )

                    ctrCell += 1;

                    if (ctrCell == 4 || p == pProduct.length - 1) {
                        divContainer.append(divRow).append('<hr>')
                        divSearchedMoreProduct.append(divContainer);
                        divRow = $('<div class="row">');
                        ctrCell = 0;
                    }
                }

                if(vRows.length >= 20){
                    divSearchedMoreProduct.append($('<div style="text-align:center;">').append($('<a data-role="none" class="btn btn-upper btn-primary outer-left-xs btn-less-width">')
                                            .text('Load More')
                                            .click(function () {
                                                from += 20;
                                                to += 20;
                                                LoadMore();
                                                $(this).text('Loading more 20 products ...').attr('disabled', true)
                                            })
                                        ).append($('<hr>'))
                                    )
                }
            }
        }

        function LoadMore() {
            AjaxSend({
                url: '/Ecommerce/GetProductSearchMore?OrgID=' + sessionStorage.OrgID + (GetUrlParameters().keywords == undefined ? "" : "&keywords=" + GetUrlParameters().keywords),
                data: { From: from, To: to, gid: GetUrlParameters().gid},
                success: function (e) {
                    //sessionStorage.MoreProducts = JSON.stringify({ d: e });
                    LoadData(e.MoreProducts);
                }
            })
        }
    })
})