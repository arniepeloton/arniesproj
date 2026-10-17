@Code
    ViewData("Title") = "This is the Online Store Home Page"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/OnlineStoreOrders/OnlineStoreOrders.js?crc=')]);
    </script>
    
End Section

<div id="pageOnlineStoreHome" data-role="page">
    <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>

    <div data-role="content">
        
        <div>Order List Here</div>

    </div>

</div>

