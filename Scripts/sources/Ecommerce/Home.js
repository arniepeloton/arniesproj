require(['common'], function () {
    require(['promise!/Scripts/jquery.bxslider.min.js'], function () {
        $(document).ready(function () {
            var divHomeAllProducts = $('#divHomeAllProducts');

            GetAllProducts(1, 20)
            GetBanners();

            function GetAllProducts(min, max) {
                Ecommerce.Loading('#divHomeAllProducts');

                AjaxSend({
                    url: 'Ecommerce/GetAllProducts',
                    data: {
                        min: min, max: max
                    },
                    success: function (e) {
                        var vRows = e.Results.Rows;
                        $('#divHomeAllProducts').find('.divspinner').remove();

                        if (vRows.length > 0) {
                            Ecommerce.DisplayProduct({
                                Container: $('#divHomeAllProducts'),
                                data: vRows,
                                LoadMore: function (e) {
                                    GetAllProducts(min + 20, max + 20)
                                    $(e).remove();
                                }
                            })
                        } else {
                            divHomeAllProducts.append($('<div style="text-align:center;">').text('No product categories found. You may search the product at the search bar.'))
                        }
                    }
                })
            }

            function GetBanners() {
                AjaxSend({
                    url: 'Ecommerce/GetBanner',
                    success: function (e) {
                        var vRows = e.Results.Rows;
                        var listHomeBanner = $('#listHomeBanner');

                        listHomeBanner.empty();

                        if (vRows.length > 0) {
                            for (var i = 0; i < vRows.length; i++) {
                                listHomeBanner.append($('<li>')
                                        .append($('<img>')
                                            .attr('src', '/Content/Ecommerce/banners/' + (sessionStorage.OrgID).toLowerCase() + '/' + vRows[i].FileName)
                                        )
                                    )
                            }

                            listHomeBanner.bxSlider({
                                auto: true,
                                pager: true,
                                touchEnabled: false,
                                responsive: true
                            });
                        } else {
                            listHomeBanner.hide();
                        }
                    }
                })
            }
        })
    })
})