@Code
    ViewData("Title") = "Initializing Subscriber Data Storage"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head
    <script>
        require([GetPathFormat('/Scripts/sources/Subscription/Subscription.js?crc=')
            ]);
    </script>

    <script>
        require([
            ]);
    </script>
    <style>
        .ui-grid-a .ui-block-a
        {
            width: 75%;
        }
        .ui-grid-a .ui-block-b
        {
            width: 25%;
        }
    </style>
End Section

<div id="pageSubscriber" data-role="page">
    <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>

    <div data-role="content">
        <div id="divSubscriber">
            Initializing subscriber data... Please wait...
        </div>
        <div id="divSubscriberDone" style="display:none;">
            <p>Subscriber(s) data has been initialized successfully. Please <a id="lnkSignin" style="cursor:pointer; font-size:16px;">click here</a> to login to your account.</p>
        </div>
    </div>

</div>
