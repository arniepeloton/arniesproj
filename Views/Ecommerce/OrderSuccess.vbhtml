@Code
    ViewData("Title") = "Ecommerce"
    Layout = "~/Views/Shared/EcommerceLayout.vbhtml"
End Code
@Section head
    <script type="text/javascript">
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion, 'promise!/Scripts/sources/Ecommerce/ImageViewer.js?crc=' + assetversion], function () {
                require(['/Scripts/sources/Ecommerce/Ecommerce.js?crc=' + assetversion], function () {
                    require(['/Scripts/sources/Ecommerce/OrderSuccess.js?crc=' + assetversion])
                });
            })
        })
    </script>
End Section
<div class="body-content outer-top-xs" id="top-banner-and-menu">
    <div class="container">
        <div class="bg-white border p-3 rounded">
            <div class="row">
                <div class="offset-sm-1 col-sm-11">
                    <h1 class="text-success">Thank you for your order.</h1>
                    <br />
                    Your Order Number Is:
                    <b>SO <label id="lblSONum"></label></b>
                    <br /><br />
                    You can track your order(s) online by going to “My Account > <a data-role="none" id="btnOSOrderHistory" class="custom-link">Order History</a>”.
                    <br /><br />
                    Have a nice and safe day ahead.
                    <br /><br />
                    <button data-role="none" id="btnOSHomepage" class="btn btn-primary btn-less-width">Go back to home page</button>
                </div>
            </div>
        </div>               
    </div>
</div>