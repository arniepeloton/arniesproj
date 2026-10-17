@Code
    ViewData("Title") = "Confirm Employment"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
@Section head
    <script>
        require([GetPathFormat('/Scripts/sources/ConfirmEmployment/ConfirmEmployment.js?crc=')
            ]);
    </script>
End Section

<style>
    .sample {
    -webkit-transition: text-shadow 1s linear;
    -moz-transition: text-shadow 1s linear;
    -ms-transition: text-shadow 1s linear;
    -o-transition: text-shadow 1s linear;
    transition: text-shadow 1s linear;
    }
    .sample:hover,
    .sample.glow {
        text-shadow: 0 0 8px red;
    }
</style>

<div id="pageSalesPaymentDuplicateReceipt" data-role="page">
    @*<div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>*@
    
    @*<div class="sample" style="overflow-x: hidden; overflow-y: hidden; opacity: 1; font-size:80px; text-align:center;">Sample Glow</div>*@
    <span id="msg">
        <strong style="font-size:20px;">Processing your confirmation of employment.</strong> 
        Please wait ...
    </span>
</div>