require(['common', 'promise!/Scripts/jquery.bxslider.min.js', '/Content/Ecommerce/js/lightbox-2.11.1.js'], function () {
    var arrListOfCategories, userdetails, arrAllProducts, orgtradename;

    require(['promise!/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion,'promise!/Scripts/sources/Ecommerce/ImageViewer.js?crc=' + assetversion,
        'promise!bootstrap-tooltip'], function () {
            $(document).ready(function () {                               

                $(function () {
                    AjaxSend({
                        url: MergeUrlParameters(GetUrlParameters(), '/Ecommerce/GetGlobalData'),
                        success: function (e) {
                            if (e.Error != undefined) {
                                var tempinterval = setInterval(function () {
                                    if (typeof NewMessageBox != 'undefined') {
                                        clearInterval(tempinterval)
                                        NewMessageBox(e.Error, 'Message', undefined, {
                                            Close: function () {
                                                location.href = '/Ecommerce/Logout?OrgID=' + sessionStorage.OrgID + '&url=' + location.origin;
                                            }
                                        })
                                    }
                                }, 100)
                            }
                            sessionStorage.OrgID = e.OrgID;
                            localStorage.removeItem("LoginUrl");
                            arrListOfCategories = e.Categories;
                            userdetails = e.UserDetails;
                            arrAllProducts = e.AllProducts;
                            orgtradename = e.TradeName;
                            LoadData();

                            if (location.pathname.toLowerCase() != '/ecommerce/signin') {
                                DisplayDropdownCartTotal();
                            }
                        }
                    })
                })

                //LoadData();

                $('body')
                .tooltip({ selector: '[data-toggle="tooltip"]', trigger: 'hover' }).on('click mousedown mouseup', '[data-toggle="tooltip"], [title]:not([data-toggle="popover"])', function () {
                    $('[data-toggle="tooltip"], [title]:not([data-toggle="popover"])').tooltip('destroy');
                })
                .on('click', '#btnOrgLogo', function () {
                    location.href = '/Ecommerce?OrgID=' + GetUrlParameters().OrgID;
                })
                .on('keypress', '#txtHeaderSearchField', function (e) {
                    if (e.which == 13) {
                        if (location.pathname.toLowerCase() == '/ecommerce/search') {
                            window.history.pushState($.trim($(this).val()), 'Ecommerce', MergeUrlParameters({
                                OrgID: GetUrlParameters().OrgID,
                                keywords: $.trim($(this).val())
                            }, '/Ecommerce/Search'))
                            SearchProduct($.trim($(this).val()))
                        } else {
                            location.href = MergeUrlParameters({
                                OrgID: GetUrlParameters().OrgID,
                                keywords: $.trim($(this).val())
                            }, '/Ecommerce/Search')
                        }
                    }
                })
                .on('click', '#btnEcommerceLogin', function () {
                    location.href = MergeUrlParameters({
                        OrgID: GetUrlParameters().OrgID
                    }, '/Ecommerce/Signin')
                })
                .on('click', '#btnUserLogout', function () {
                    location.href = '/Ecommerce/Logout?OrgID=' + sessionStorage.OrgID + '&url=' + location.origin;
                })
                .on('click', '#btndropdownMyAccount', function () {
                    location.href = '/Ecommerce/MyAccount?OrgID=' + sessionStorage.OrgID;
                })
                .on('click', '#btndropdownAddressBook', function () {
                    location.href = '/Ecommerce/MyAccount?OrgID=' + sessionStorage.OrgID + '#addressbook';
                })
                .on('click', '#btndropdownOrderHistory', function () {
                    location.href = '/Ecommerce/MyAccount?OrgID=' + sessionStorage.OrgID + '#orderhistory';
                })
                .on('click', '#btndropdownAccountsPayable', function () {
                    location.href = '/Ecommerce/MyAccount?OrgID=' + sessionStorage.OrgID + '#accountspayable';
                })
                .on('shown.bs.dropdown', '#btnCartDropdown', function () {
                    DisplayDropdownCartTotal();
                })
                .on('click', '.divfloatingcart', function () {
                    if (sessionStorage.PeopleUserID == undefined) {
                        NewMessageBox('Before you can add an item to your cart, you must login first.', 'Message', [
                            {
                                Text: 'Go to login',
                                Click: function () {
                                    location.href = '/Ecommerce/Signin?OrgID=' + sessionStorage.OrgID
                                }
                            }
                        ])
                        return;
                    }

                    window.location.href = '/Ecommerce/ShoppingCart?OrgID=' + sessionStorage.OrgID;
                })                
            })
        })

    window.Ecommerce = {
        DisplayDropdownCartTotal: DisplayDropdownCartTotal,
        DisplayQtyInterval: function (data) {
            var qty = data.QtyChoices + (CStr2(data.Alias) == '' ? '' : ' (' + data.Alias + ')');
            return $('<a data-role="none" href="#" class="list-group-item">')
                    .data('data', data)
                    .text(qty)
                    .click(function (e) {
                        e.stopPropagation();
                        e.preventDefault();
                        $(this).parent().prev().val($(this).data('data').QtyChoices);
                        $(this).parent().hide();
                    })
        },
        ApplyUpcharge: function (data) {
            var upcharge = 1; // 1 coz in case walay upcharge, dapat same total gihapon

            if (CDec2(data.Qty || data.OrderedQuantity) < 1) { // base packing size qty is always 1 daw, so default to 1
                if ($.isArray(data.Upcharge)) {
                    upcharge = ((100 + CDec2(data.Upcharge.length == 0 ? 0 : data.Upcharge[0].Upcharge)) / 100);
                } else {
                    upcharge = ((100 + CDec2(data.Upcharge)) / 100);
                }
            }

            return (CDec2(data.Qty || data.OrderedQuantity) * CDec2(data.Price || data.UnitPrice)) * upcharge;
        },
        AddToCart: function (dis) {
            var data = dis.data('data')
            if (sessionStorage.PeopleUserID == undefined) {
                NewMessageBox('Before you can add an item to your cart, you must login first.', 'Message', [
                    {
                        Text: 'Go to login',
                        Click: function () {
                            location.href = '/Ecommerce/Signin?OrgID=' + sessionStorage.OrgID
                        }
                    }
                ])
                return;
            }
            
            dis.empty().attr('disabled', 'disabled').append($('<div class="spinner-border spinner-border-sm">')).append(' Adding ...');
            
            setTimeout(function () {
                var pData = {
                    FromEcommerce: 1,
                    //CustomerID: mOut.Model.Data.CustomerID,
                    TransType: 'SalesOrder',
                    DeleteExistingProduct: 0,
                    DontCheckCartValidity: 1,
                    //OrderID: mOut.Model.Data.ID,
                    //NewCartMode: !IsGuid(mOut.Model.Data.ID),
                    StockID: data.ProductID,
                    SalesRep: [],
                    ProductSerial: [],
                    UpdateOrderedQty: 1,
                    Stocks: [
                        {
                            Qty: 1,
                            UnitQty: 1
                        }
                    ]
                };

                AjaxSend({
                    url: '/Stock/UpdatePos',
                    data: pData,
                    NoSplash: true,
                    success: function (vData) {
                        dis.empty().removeAttr('disabled').append($('<i class="bi-cart4">')).append(' Add to cart');

                        if (vData.Result == 'ok') {
                            Ecommerce.DisplayDropdownCartTotal();
                            dis.empty().removeAttr('disabled').append($('<i class="bi-check2">')).append(' Added');
                            setTimeout(function () {
                                dis.empty().removeAttr('disabled').append($('<i class="bi-cart4">')).append(' Add to cart');                                
                            }, 1000)
                        } else {
                            NewMessageBox('Sorry! There was a problem while adding the item to your cart. Please try again later!');
                        }
                    }
                });
            }, 500)
        },
        DisplayProduct: function (params) {
            var ctrCell = 0, ctrProduct = 1;
            var divRow = $('<div class="row row-cols-2 row-cols-sm-2 row-cols-md-4">');

            for (var p = 0; p < params.data.length; p++) {
                var pProduct = params.data[p];
                var tempPic = '/Content/Ecommerce/images/products/p1.jpg'

                if (CStr2(pProduct.FileName) != '') {
                    tempPic = '/Content/MobizImagePIM/' + sessionStorage.OrgID + '/' + pProduct.FileSize + '/' + pProduct.FileName;
                }

                divRow.append($('<div class="col mb-3">')
                        .append($('<div class="p-2 rounded bg-white eProducts border-success">')
                            .data('data', pProduct)
                            .append($('<img class="img-thumbnail rounded float-start mb-2">')
                                .attr('src', tempPic)
                                .attr('onerror', 'this.onerror=null;this.src="/Content/Ecommerce/images/products/p1.jpg"')
                            )
                            .append($('<div class="text-primary mb-2 productname">').text(pProduct.ShortName || pProduct.ProductName))
                            .append($('<div class="mb-2">').text('P ' + AccountingFormat(pProduct.Price) + ' per ' + pProduct.UnitMeasure))
                            .append($('<button data-role="none" class="btn btn-outline-success btn-block">').append($('<i class="bi-cart4">')).append(' Add to cart')
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
                    )


                ctrCell += 1;

                if (ctrCell == 4 || p == params.data.length - 1) {
                    params.Container.append(divRow)
                    divRow = $('<div class="row row-cols-2 row-cols-sm-2 row-cols-md-4">');
                    ctrCell = 0;
                }

                if (ctrProduct == 20) {
                    params.Container.append($('<div style="text-align:center;">').append($('<a data-role="none" class="btn btn-upper btn-primary outer-left-xs btn-less-width">')
                                .text('LOAD MORE')
                                .data('data', (params.ParentData || pProduct))
                                .click(function (e) {
                                    e.stopPropagation();
                                    e.preventDefault();
                                    if (typeof params.LoadMore != 'undefined') {
                                        params.LoadMore(this);
                                    }
                                })
                            )
                        )
                    break;
                }

                ctrProduct += 1;
            }
        },
        Loading: function (container, isclear, msg) {
            if (CBool(isclear)) {
                $(container).empty();
            }
            $(container).append($('<div class="text-center divspinner">')
                    .append($('<div class="spinner-border text-primary" role="status">'))
                    .append('<br>').append((CStr2(msg) == '' ? '' : msg))
                );
        }
    }

    function SearchProduct(s) {
        Ecommerce.Loading('#divSearchedProduct', true);

        AjaxSend({
            url: MergeUrlParameters({
                OrgID: GetUrlParameters().OrgID,
                keywords: s
            }, '/Ecommerce/GetProductSearch'),
            data: { keywords: s },
            success: function (e) {
                //sessionStorage.Products = JSON.stringify({d : e});
                Ecommerce.SearchKey = s;
                Ecommerce.DisplaySearch(undefined, e.MoreProducts);
            }
        })
    }

    function LoadData() {
        var cat = arrListOfCategories; // arrListOfCategories declared in EcommerceLayout.vbhtml
        var listSidebarCategories = $('#listSidebarCategories');
        var listMainNavBarMenu = $('#listMainNavBarMenu');
        listSidebarCategories.empty();
        listMainNavBarMenu.empty();

        $('#imgOrgLogo').attr('src', '/Content/OrganizationLogo/' + sessionStorage.OrgID + '.png')
                    .attr('alt', orgtradename)
                    .attr('onload', 'logoload(this)')
                    .attr('onerror', 'logoerror(this)')

        if (window.location.pathname.toLowerCase() == '/ecommerce' ||
            window.location.pathname.toLowerCase() == '/ecommerce/search' ||
            window.location.pathname.toLowerCase() == '/ecommerce/productdetails') {
            $('.divfloatingcart').show();
        }

        listMainNavBarMenu.append($('<li class="nav-item mr-1">')
                    .append($('<button data-role="none" class="btn btn-primary">').text('Home')
                        .attr({
                            href: location.origin + '/Ecommerce?OrgID=' + GetUrlParameters().OrgID
                        })
                        .click(function (e) {
                            e.stopPropagation();
                            e.preventDefault();
                            location.href = '/Ecommerce?OrgID=' + GetUrlParameters().OrgID;
                        })
                    )
                )

        if (cat.length == 0) {
            listSidebarCategories.append($('<a data-role="none" href="#" class="list-group-item list-group-item-light">').text('No Product Categories available'))
        }

        for (var i = 0; i < cat.length; i++) {
            var ul = $('<ul class="dropdown-menu" style="max-height:300px; overflow:auto;">').attr('catid', cat[i].ID),
                li = $('<li>'),
                container = $('<div class="yamm-content">');

            //if (cat[i].SubGroup.length == 0 && CBool(cat[i].IsGroup)) {
            //    continue;
            //}

            if (cat[i].SubGroup.length > 0) {
                GenerateSubGroupList(container, cat[i].SubGroup, 2);
                li.append(container);
                ul.append(li);
            }

            listSidebarCategories.append($('<a data-role="none" href="#" class="list-group-item list-group-item-light">')
                    .data('data', cat[i])
                    .text((CBool(cat[i].IsGroup) ? cat[i].Name : cat[i].ShortName))
                    .click(function (e) {
                        e.stopPropagation();
                        e.preventDefault();
                        //GetSubGroup($(this));
                        var data = $(this).data('data');

                        if (CBool(data.IsGroup)) {
                            GetCategoriesProducts(data);
                        } else {
                            sessionStorage.ProductDetails = JSON.stringify(data);
                            location.href = MergeUrlParameters({
                                OrgID: GetUrlParameters().OrgID,
                                pname: data.ProductName, // Product Name
                                pid: data.ProductID
                            }, '/Ecommerce/ProductDetails')
                        }
                    })//.append(ul.clone(true).addClass('mega-menu'))                    
            )

            if (CBool(cat[i].IsFeature)) {
                listMainNavBarMenu.append($('<li class="nav-item mr-1">')
                    .data('data', cat[i])
                    .append($('<button data-role="none" class="btn btn-primary">')
                        .text(cat[i].Name)
                    )//.append(ul.clone(true).addClass('container'))
                    .click(function (e) {
                        e.stopPropagation();
                        e.preventDefault();
                        var data = $(this).data('data');

                        if (CBool(data.IsGroup)) {
                            if (location.pathname.toLowerCase() == '/ecommerce' || location.pathname.toLowerCase() == '/ecommerce/search') {
                                GetCategoriesProducts(data);
                            } else {
                                location.href = MergeUrlParameters({
                                    OrgID: GetUrlParameters().OrgID,
                                    gid: data.ID,
                                    cat: data.Name
                                }, '/Ecommerce/Search')
                            }
                        } else {
                            sessionStorage.ProductDetails = JSON.stringify(data);
                            location.href = MergeUrlParameters({
                                OrgID: GetUrlParameters().OrgID,
                                pname: data.ProductName, // Product Name
                                pid: data.ProductID
                            }, '/Ecommerce/ProductDetails')
                        }
                    })
                )
            }
        }

        LoadUser();
        //DisplayDropdownCartTotal();
    }

    function GetSubGroup(dis) {
        var data = dis.data('data');

        if (dis.hasClass('open')) {
            dis.removeClass('open');
        } else {
            $('.nav li').removeClass('open');
            dis.addClass('open');

            if (data.SubGroup.length == 0) {
                $('[catid="' + data.ID + '"]').append($('<li>').append($('<a>').text('Getting products .... please wait')))

                AjaxSend({
                    url: 'Ecommerce/GetSubGroup',
                    data: {
                        OrgID: sessionStorage.OrgID,
                        MenuID: data.MenuID,
                        GroupID: data.ID,
                        Level: 2
                    }, success: function (e) {
                        data.SubGroup = e.Results;
                        var container = $('<div class="yamm-content">');
                        GenerateSubGroupList(container, data.SubGroup, 2);
                        $('[catid="' + data.ID + '"]').empty().append($('<li>').append(container));
                    }
                })
            }
        }
    }

    function GenerateSubGroupList(container, data, subgrouplvl, ul) {
        var ctrCell = 1, ctrSecondLevel = 1, ctrUngroup = 1, isSeparateUngroup = false;
        var subgRow = $('<div class="row">'), unGroupSku = $('<div class="row">');

        if (subgrouplvl == 2) {
            if (data.length == 0) {
                container.append('No products found')
                return
            }

            for (var i = 0; i < data.length; i++) {
                var ul2 = $('<ul>');
                var subul = $('<ul style="margin-left:20px;">');

                if (ctrCell == 5 && CBool(data[i].IsGroup)) {
                    ctrCell = 1;
                    subgRow = $('<div class="row">');
                    container.append('<hr>')
                }

                if (CBool(data[i].IsGroup)) {
                    isSeparateUngroup = true;
                    if (data[i].SubGroup.length == 0) {
                        ctrSecondLevel += 1;
                        continue;
                    }
                    for (var s = 0 ; s < data[i].SubGroup.length; s++) {
                        if (CBool(data[i].SubGroup[s].IsGroup)) {
                            GenerateSubGroupList(subgRow, data[i].SubGroup[s], 3, subul);
                            ul2.append($('<li>')
                                    .append($('<a data-role="none">')
                                        .append('<span style="color:red;">*</span> ' + data[i].SubGroup[s].Name)
                                        .attr({
                                            href: MergeUrlParameters({
                                                OrgID: GetUrlParameters().OrgID,
                                                gid: data[i].SubGroup[s].ID,
                                                cat: data[i].SubGroup[s].Name
                                            }, location.origin + '/Ecommerce/Search')
                                        })
                                    ).append(subul)
                                    .data('data', data[i].SubGroup[s])
                                    .click(function (e) {
                                        e.stopPropagation();
                                        e.preventDefault();
                                        var data = $(this).data('data');
                                        location.href = MergeUrlParameters({
                                            OrgID: GetUrlParameters().OrgID,
                                            gid: data.ID,
                                            cat: data.Name
                                        }, '/Ecommerce/Search')
                                    })
                                )
                        } else {
                            ul2.append($('<li>')
                                    .data('data', data[i].SubGroup[s])
                                    .append($('<a data-role="none">')
                                        .text(data[i].SubGroup[s].Name)
                                        .attr({
                                            href: MergeUrlParameters({
                                                OrgID: GetUrlParameters().OrgID,
                                                pname: data[i].SubGroup[s].ShortName, // Product Name
                                                pid: data[i].SubGroup[s].ProductID
                                            }, location.origin + '/Ecommerce/ProductDetails')
                                        })
                                    )
                                    .click(function (e) {
                                        e.stopPropagation();
                                        e.preventDefault();
                                        var data = $(this).data('data');
                                        location.href = MergeUrlParameters({
                                            OrgID: GetUrlParameters().OrgID,
                                            pname: data.ShortName, // Product Name
                                            pid: data.ProductID
                                        }, '/Ecommerce/ProductDetails')
                                    })
                                )
                        }
                    }

                    subgRow.append($('<div class="col-xs-12 col-sm-6 col-md-3 col-menu">')
                        .append($('<a data-role="none">')
                                .attr({
                                    href: MergeUrlParameters({
                                        OrgID: GetUrlParameters().OrgID,
                                        gid: data[i].ID,
                                        cat: data[i].Name
                                    }, location.origin + '/Ecommerce/Search')
                                })
                                .append($('<h2 class="title">').text(data[i].Name))
                                .data('data', data[i])
                                .click(function (e) {
                                    e.stopPropagation();
                                    e.preventDefault();
                                    var data = $(this).data('data');
                                    location.href = MergeUrlParameters({
                                        OrgID: GetUrlParameters().OrgID,
                                        gid: data.ID,
                                        cat: data.Name
                                    }, '/Ecommerce/Search')
                                })
                        )
                        .append(ul2)
                    )

                    container.append(subgRow);

                    ctrCell += 1;
                    ctrSecondLevel += 1;
                } else {
                    if (isSeparateUngroup) {
                        container.append('<hr>');
                        isSeparateUngroup = false;
                    }

                    if (ctrUngroup == 5) {
                        ctrUngroup = 1;
                        unGroupSku = $('<div class="row">');
                    }

                    unGroupSku.append($('<div class="col-sm-3">')
                            .data('data', data[i])
                            .append($('<a data-role="none">').text(data[i].Name)
                                .attr({
                                    href: MergeUrlParameters({
                                        OrgID: GetUrlParameters().OrgID,
                                        pname: data[i].Name, // Product Name
                                        pid: data[i].ProductID
                                    }, location.origin + '/Ecommerce/ProductDetails')
                                })
                            )
                            .click(function (e) {
                                e.stopPropagation();
                                e.preventDefault();
                                var data = $(this).data('data');
                                location.href = MergeUrlParameters({
                                    OrgID: GetUrlParameters().OrgID,
                                    pname: data.Name, // Product Name
                                    pid: data.ProductID
                                }, '/Ecommerce/ProductDetails')
                            })
                        )

                    container.append(unGroupSku)

                    ctrUngroup += 1;
                }
            }

        } else {
            for (var s = 0 ; s < data.SubGroup.length; s++) {
                ul.append($('<li>')
                            .data('data', data.SubGroup[s])
                            .append($('<a data-role="none">')
                                .text(data.SubGroup[s].Name)
                                .attr({
                                    href: MergeUrlParameters({
                                        OrgID: GetUrlParameters().OrgID,
                                        pname: data.SubGroup[s].ShortName, // Product Name
                                        pid: data.SubGroup[s].ProductID
                                    }, location.origin + '/Ecommerce/ProductDetails')
                                })
                            )
                            .click(function (e) {
                                e.stopPropagation();
                                e.preventDefault();
                                var data = $(this).data('data');
                                location.href = MergeUrlParameters({
                                    OrgID: GetUrlParameters().OrgID,
                                    pname: data.ShortName, // Product Name
                                    pid: data.ProductID
                                }, '/Ecommerce/ProductDetails')
                            })
                        )

                if (s == 9 && data.SubGroup.length > 10) {
                    ul.append($('<li style="text-align:center;">')
                            .data('data', data)
                            .append($('<a data-role="none" href="#">')
                                .attr({
                                    href: MergeUrlParameters({
                                        OrgID: GetUrlParameters().OrgID,
                                        gid: data.ID,
                                        cat: data.name
                                    }, location.origin + '/Ecommerce/Search')
                                })
                                .text('-- See More --')
                            )
                            .click(function (e) {
                                e.stopPropagation();
                                e.preventDefault();
                                var data = $(this).data('data');
                                location.href = MergeUrlParameters({
                                    OrgID: GetUrlParameters().OrgID,
                                    gid: data.ID,
                                    cat: data.Name
                                }, '/Ecommerce/Search')
                            })
                        )
                    break
                }
            }
        }
    }

    function GetCategoriesProducts(data) {
        Ecommerce.Loading('#divHomeAllProducts, #divSearchedProduct', true, 'Getting products for ' + data.Name);

        if (location.pathname.toLowerCase() == '/ecommerce') {
            location.hash = '#divHomeAllProducts';
        } else {
            location.hash = '#divSearchedProduct';
        }

        AjaxSend({
            url: 'Ecommerce/GetProductSearch',
            data: { gid: data.ID },
            success: function (e) {
                $('#divHomeAllProducts, #divSearchedProduct').empty().append($('<div style="font-size:30px;">').text('Category: ' + data.Name));
                DisplayCategoriesProducts(e.SubGroup, e.MoreProducts, data);
            }
        })
    }

    function DisplayCategoriesProducts(subg, moreprod, categorydata) {
        var SubGroup = subg;
        var MoreProducts = moreprod;
        var divSearchedProduct = $('#divHomeAllProducts, #divSearchedProduct');
        var divRow = $('<div class="row">');
        var arrCat = [];

        //divSearchedProduct.empty();
        $('#txtHeaderSearchField').val(CStr2(GetUrlParameters().keywords))

        if (SubGroup != undefined) {
            if (SubGroup.length > 0) {
                for (var i = 0; i < SubGroup.length; i++) { // SUB LEVEL 1                        
                    var pSubGroup = SubGroup[i];
                    var divContainer = $('<div>').attr('subgroupid', pSubGroup.ID);
                    var ctrCell = 0, ctrProduct = 1, divCol, hasSubgroup = false;
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
                            divContainer.append($('<div style="color:#5f5f5f;">').append($('<span class="glyphicon glyphicon-chevron-right">')).append('&nbsp;').append(pProduct.Name))

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

                            if (pSubGroup.SubGroup.length == (p + 1)) {
                                divContainer.append('<hr>')
                            }

                            ctrProduct += 1;
                        }
                    }

                    if (ctrProduct == 20) {
                        divContainer.append($('<div style="text-align:center;">').append('<br>').append($('<a data-role="none" class="btn btn-upper btn-primary outer-left-xs btn-less-width">')
                                .data('data', pSubGroup)
                                //.attr({
                                //    href: MergeUrlParameters({
                                //        OrgID: GetUrlParameters().OrgID,
                                //        gid: pSubGroup.ID
                                //    }, location.origin + '/Ecommerce/SearchMore')
                                //})
                                .text('LOAD MORE ' + pSubGroup.Name)
                                .click(function (e) {
                                    e.stopPropagation();
                                    e.preventDefault();
                                    LoadMoreProducts($(this));
                                })
                            )
                        )
                    }
                }
            }
        }

        divRow = $('<div class="row">');

        if (MoreProducts.length > 0) {
            if ((SubGroup || []).length > 0) {
                divSearchedProduct.append($('<div style="color:#5f5f5f;">').append('More Products for ' + categorydata.Name));
            }

            Ecommerce.DisplayProduct({
                Container: divSearchedProduct,
                ParentData: categorydata,
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

        Ecommerce.Loading(dis.parent(), true);

        data.From = CInt2(data.To || 20) + 1;
        data.To = CInt2(data.To || 20) + 20;

        AjaxSend({
            url: 'Ecommerce/GetProductSearchMore',
            data: { From: data.From, To: data.To, gid: data.ID },
            success: function (e) {
                var vRows = e.MoreProducts.Rows;

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
                }
            }
        })
    }

    function LoadUser() {
        var data = userdetails;

        if (data.UserFullName == '') {
            $('#btnEcommerceLogin').show();
            $('#btnUserOptions').hide();
            $('#lblUser').text('');
            sessionStorage.removeItem("PeopleUserID");
            sessionStorage.removeItem("CurrentUserFullName");
            sessionStorage.removeItem("sonum");
        } else {
            $('#btnEcommerceLogin').hide();
            $('#btnUserOptions').show();
            $('#lblUser').text('Hello ' + data.FirstName);
        }
    }

    function DisplayDropdownCartTotal() {
        function GetCartData() {
            AjaxSend({
                url: '/Ecommerce/EcommerceCart',
                data: {
                    FromEcommerce: 1
                },
                success: function (vData) {
                    if (vData.Error == undefined) {
                        DisplayCartData(GetStringObject('Result.Rows', vData));
                    }
                }
            });
        }

        function DisplayCartData(arrMyCart) {
            var listCartDropdown = $('#listCartDropdown');
            //var arrMyCart = JSON.parse(JSON.parse(JSON.stringify(sessionStorage.MyCart || "[]")));
            var grandtotal = 0;

            listCartDropdown.empty();

            if (arrMyCart.length > 0) {
                for (var i = 0; i < arrMyCart.length; i++) {
                    var subtotal = Ecommerce.ApplyUpcharge(arrMyCart[i]);
                    grandtotal += subtotal;

                    var img = '';

                    if (arrMyCart[i].VendorID != undefined) {
                        if (arrMyCart[i].FileName != undefined) {
                            img = ImageViewer.DefaultImage('/Content/MobizImagePIM/' + sessionStorage.OrgID.toLowerCase() + '/Medium/' + arrMyCart[i].VendorID + '/' + arrMyCart[i].FileName, arrMyCart[i]).css('width', '100%');
                        }
                    } else {
                        img = ImageViewer.DefaultImage((arrMyCart[i].FileName == undefined ? '/Content/Ecommerce/images/products/p1.jpg' : "/Content/MobizImagePIM/" + sessionStorage.OrgID + '/medium/' + arrMyCart[i].FileName), arrMyCart[i]).css('width', '100%');
                    }

                    listCartDropdown.append($('<li>')
                            .append($('<div class="cart-item product-summary p-2">')
                                .append($('<div class="row">')
                                    .append($('<div class="col-3">')
                                        .append($('<div class="image">')
                                            .append(img)
                                        )
                                    )
                                    .append($('<div class="col-9">')
                                        .append($('<span class="text-secondary">')
                                            .text(arrMyCart[i].ProductName)
                                        )
                                        .append($('<div class="price">').text($.format.number(CDec2(arrMyCart[i].Qty), '#,###.##') + ' @ P ' + AccountingFormat(arrMyCart[i].Price))
                                            .append($('<div id="divupcharge-' + arrMyCart[i].ID + '">').append(arrMyCart[i].Upcharge.length == 0 ? '' : ' + ' + arrMyCart[i].Upcharge[0].Upcharge + '% upcharge'))
                                        )
                                    )
                                )
                            )
                        )
                }
                listCartDropdown.append('<li><hr class="dropdown-divider"></li>')
                listCartDropdown.append($('<div class="clearfix">'))
                    .append($('<hr>'))
                    .append($('<div class="clearfix cart-total p-2">')
                        .append('<div class="pull-right mb-3 font-weight-bold"> <span class="text">Sub Total :</span><span class="price">P ' + AccountingFormat(grandtotal) + '</span> </div>')
                        .append('<div class="clearfix"></div>')
                        .append($('<a data-role="none" href="#" class="btn btn-upper btn-warning btn-block m-t-20">')
                            .text('CHECKOUT')
                            .click(function (e) {
                                window.location.href = '/Ecommerce/Checkout?OrgID=' + sessionStorage.OrgID;
                            })
                        )
                        .append($('<a data-role="none" href="#" class="btn btn-upper btn-success btn-block m-t-20">')
                            .text('View Cart')
                            .click(function (e) {
                                window.location.href = '/Ecommerce/ShoppingCart?OrgID=' + sessionStorage.OrgID;
                            })
                        )
                    )

                $('#lblCartDropdownTotal').text(AccountingFormat(grandtotal));
                $('.lblCountItems').text(arrMyCart.length);
                $('.floatingcart').addClass('addedtocart').delay(1500).queue(function () {
                    $(this).removeClass('addedtocart').dequeue();
                });
            } else {
                listCartDropdown.append($('<li class="p-2">').text('Cart is empty'));
                $('#lblCartDropdownTotal').text('0.00');
                $('.lblCountItems').text(0);
            }
        } // END function DisplayCartData() {
        GetCartData();
    } // END function DisplayDropdownCartTotal() {
})