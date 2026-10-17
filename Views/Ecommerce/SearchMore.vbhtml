@Code
    ViewData("Title") = "Ecommerce"
    Layout = "~/Views/Shared/EcommerceLayout.vbhtml"
End Code
@Section head
    <script type="text/javascript">
        //sessionStorage.MoreProducts = @Html.Raw(Json.Encode(ViewData("MoreProducts")));
        require(['promise!/Scripts/sources/Ecommerce/Ecommerce.js?crc=' + assetversion], function () {
            require(['/Scripts/sources/Ecommerce/SearchMore.js?crc=' + assetversion])
        });
</script>
End Section
<div class="body-content outer-top-xs" id="top-banner-and-menu">
    <div class="container">
        <div id="divSearchedMoreProduct">

        </div>
    </div>
</div>