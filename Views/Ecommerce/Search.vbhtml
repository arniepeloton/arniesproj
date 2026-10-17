@Code
    ViewData("Title") = "Ecommerce"
    Layout = "~/Views/Shared/EcommerceLayout.vbhtml"
End Code
@Section head
    <script type="text/javascript">
        //sessionStorage.Products = @Html.Raw(Json.Encode(ViewData("Products")));
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion, 'promise!/Scripts/sources/Ecommerce/ImageViewer.js?crc=' + assetversion], function () {
                require(['/Scripts/sources/Ecommerce/Ecommerce.js?crc=' + assetversion], function () {
                    require(['/Scripts/sources/Ecommerce/Search.js?crc=' + assetversion])
                });
            })
        })
</script>
End Section
<div class="body-content outer-top-xs" id="top-banner-and-menu">
    <div class="container">
        <div class="row">
            <div class="col-xs-12 col-sm-12 col-md-3 sidebar">
                <div class="side-menu animate-dropdown outer-bottom-xs">
                    <div class="head bg-warning p-2"><i class="bi-justify"></i> Categories</div>
                    <nav class="yamm megamenu-horizontal">
                        <div id="listSidebarCategories" class="list-group list-group-flush border"></div>
                    </nav>
                </div>
            </div>
            <div class="col-xs-12 col-sm-12 col-md-9 homebanner-holder">
                <div id="divSearchedProduct">

                </div>
            </div>
        </div>        
    </div>
</div>