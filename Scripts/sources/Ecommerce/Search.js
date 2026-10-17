require(['common'],
function () {    
    $(document).ready(function () {

        Ecommerce.DisplaySearch = LoadData;
        Ecommerce.SearchKey = CStr2(GetUrlParameters().keywords);

        $(function () {
            Ecommerce.Loading('#divSearchedProduct', true);

            AjaxSend({
                url: MergeUrlParameters(GetUrlParameters(), '/Ecommerce/GetProductSearch'),
                success: function (e) {
                    LoadData(e.SubGroup, e.MoreProducts);
                }
            })
        })        

        function LoadData(subg, moreprod) {
            var SubGroup = subg;
            var MoreProducts = moreprod;
            var divSearchedProduct = $('#divSearchedProduct');
            var divRow = $('<div class="row row-cols-2 row-cols-sm-2 row-cols-md-4">');
            var arrCat = [];

            divSearchedProduct.empty();
            $('#txtHeaderSearchField').val(CStr2(GetUrlParameters().keywords))

            if (SubGroup != undefined) {
                if(SubGroup.length > 0){
                    for (var i = 0; i < SubGroup.length; i++) { // SUB LEVEL 1                        
                        var pSubGroup = SubGroup[i];
                        var divContainer = $('<div>').attr('subgroupid', pSubGroup.ID);
                        var ctrCell = 0, ctrProduct = 0, divCol, hasSubgroup = false;
                        divContainer.append($('<div style="font-size: 25px;" class="mb-3">').text(pSubGroup.Name))
                        divSearchedProduct.append(divContainer);
                        divRow = $('<div class="row row-cols-2 row-cols-sm-2 row-cols-md-4">');

                        for (var p = 0; p < pSubGroup.SubGroup.length; p++) { // SUB LEVEL 2
                            var pProduct = pSubGroup.SubGroup[p];
                            var tempPic = '/Content/Ecommerce/images/products/p1.jpg'

                            if (CStr2(pProduct.FileName) != '') {
                                tempPic = '/Content/MobizImagePIM/' + sessionStorage.OrgID + '/' + pProduct.FileSize + '/' + pProduct.FileName;
                            }

                            if (CBool(pProduct.IsGroup)) {
                                hasSubgroup = true;
                                divContainer.append($('<div style="color:#5f5f5f;">').append($('<span class="bi-chevron-right">')).append('&nbsp;').append(pProduct.Name))
                                Ecommerce.DisplayProduct({
                                    Container: divContainer,
                                    ParentData: pProduct,
                                    data: pProduct.SubGroup,
                                    LoadMore: function (e) {
                                        LoadMoreProducts($(e));
                                    }
                                })
                            } else {
                                if (hasSubgroup) {
                                    divContainer.append($('<div style="color:#5f5f5f;" class="mb-3">').append('More Products for ' + pSubGroup.Name));
                                    hasSubgroup = false;
                                }

                                divCol = GenerateProductCol(pProduct, tempPic);

                                ctrCell += 1;
                                divRow.append(divCol)

                                if (ctrCell == 4 || pSubGroup.SubGroup.length == (p + 1)) {
                                    divContainer.append(divRow)
                                    divRow = $('<div class="row row-cols-2 row-cols-sm-2 row-cols-md-4">');
                                    ctrCell = 0;
                                }

                                ctrProduct += 1;
                            }
                        }

                        if (ctrProduct == 20) {
                            divContainer.append($('<div style="text-align:center;">').append('<br>').append($('<a data-role="none" class="btn btn-upper btn-primary outer-left-xs btn-less-width">')
                                    .data('data', pSubGroup)
                                    .attr({
                                        href: MergeUrlParameters({
                                            OrgID: GetUrlParameters().OrgID,
                                            gid: pSubGroup.ID
                                        }, location.origin + '/Ecommerce/SearchMore')
                                    })
                                    .text('LOAD MORE ' + pSubGroup.Name)
                                    .click(function (e) {
                                        e.stopPropagation();
                                        e.preventDefault();
                                        LoadMoreProducts($(this));
                                    })
                                ).append('<hr>')
                            )
                        }
                    }
                }
            }         
            
            divRow = $('<div class="row row-cols-2 row-cols-sm-2 row-cols-md-4">');

            if (MoreProducts.length > 0) {
                if ((SubGroup || []).length > 0) {
                    divSearchedProduct.append($('<div style="color:#5f5f5f;" class="mb-3">').append('More Products for ' + GetUrlParameters().cat));
                }

                Ecommerce.DisplayProduct({
                    Container: divSearchedProduct,
                    data: MoreProducts,
                    LoadMore: function (e) {
                        LoadMoreProducts($(e));
                    }
                })
            } else {
                divSearchedProduct.append($('<h1>').text('No Result(s) Found'))
            }            
        }

        function GenerateProductCol(pProduct, tempPic) {
            return $('<div class="col mb-3">')
                        .append($('<div class="p-2 rounded bg-white eProducts border-success">')
                            .data('data', pProduct)
                            .append($('<img class="img-thumbnail rounded float-start mb-2">')
                                .attr('src', tempPic)
                                .attr('onerror', 'this.onerror=null;this.src="/Content/Ecommerce/images/products/p1.jpg"')
                            )
                            .append($('<div class="text-primary mb-2 productname">').text((pProduct.ShortName || pProduct.ProductName)))
                            .append($('<div class="mb-2">').text('P ' + AccountingFormat(pProduct.Price) + ' per ' + pProduct.UnitMeasure))
                            .append($('<button data-role="none" class="btn btn-outline-success btn-block">').append($('<i class="fa fa-shopping-cart">')).append(' Add to cart')
                                .data('data', pProduct)
                                .click(function (e) {
                                    e.stopPropagation();
                                    e.preventDefault();
                                    var data = $(this).data('data');
                                    Ecommerce.AddToCart($(this));
                                })
                            )
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
        }

        function LoadMoreProducts(dis) {
            var data = dis.data('data');
            var parentDiv = dis.parent();
            var params = {};

            Ecommerce.Loading(dis.parent(), true);

            if(CStr2(data.ID) == ''){
                params.keywords = Ecommerce.SearchKey;
            } else {
                params.gid = data.ID;
            }

            params.From = CInt2(data.To || 20) + 1;
            params.To = CInt2(data.To || 20) + 20;

            AjaxSend({
                url: MergeUrlParameters(params, '/Ecommerce/GetProductSearchMore'),
                data: params,
                success: function (e) {
                    var vRows = (e.MoreProducts.Rows || e.MoreProducts);                    

                    parentDiv.empty();

                    Ecommerce.DisplayProduct({
                        Container: parentDiv,
                        ParentData: data,
                        data: vRows,
                        LoadMore: function (e) {
                            LoadMoreProducts($(e));
                        }
                    })
                    return

                    var divRow = $('<div class="row">'), ctrCell = 0, ctrProduct = 1, divCol, hasSubgroup = false;

                    for (var s = 0; s < vRows.length; s++) { // SUB LEVEL 3
                        var pProductLvlThree = vRows[s];
                        tempPic = '/Content/Ecommerce/images/products/p1.jpg';

                        if (CStr2(pProductLvlThree.FileName) != '') {
                            tempPic = '/Content/MobizImagePIM/' + sessionStorage.OrgID + '/' + pProductLvlThree.FileSize + '/' + pProductLvlThree.FileName;
                        }

                        divCol = GenerateProductCol(pProductLvlThree, tempPic);

                        ctrCell += 1;
                        divRow.append(divCol)

                        if (ctrCell == 4 || vRows.length == (s + 1)) {
                            parentDiv.append(divRow)
                            divRow = $('<div class="row">');
                            ctrCell = 0;
                        }
                    }

                    if (vRows.length == 20) {
                        parentDiv.append($('<div style="text-align:center;">').append('<br>').append($('<a data-role="none" class="btn btn-upper btn-primary outer-left-xs btn-less-width">')
                                    .data('data', data)
                                    //.attr({
                                    //    href: MergeUrlParameters({
                                    //        OrgID: GetUrlParameters().OrgID,
                                    //        gid: pProduct.ID
                                    //    }, location.origin + '/Ecommerce/SearchMore')
                                    //})
                                    .text('LOAD MORE ' + data.Name)
                                    .click(function (e) {
                                        e.stopPropagation();
                                        e.preventDefault();
                                        LoadMoreProducts($(this));
                                    })
                                )
                            )
                    } else {
                        parentDiv.append('<br>')
                    }
                }
            })
        }
    })
})