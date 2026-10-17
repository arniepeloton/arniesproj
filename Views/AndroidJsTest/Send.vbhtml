@Code
    ViewData("Title") = "Send Test"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code
<script>
    require([GetPathFormat('/Scripts3/AndroidJsTestSend.js?crc=')]);
</script>
<a href="#" data-icon="arrow-l" data-rel="back">Back</a>
<div data-role="collapsible" data-collapsed="false" style="">
    <h3>
        Socket test
    </h3>
    <input id="txtUrl" placeholder="Type remote URL (ex: 192.168.1.1) here" />
    @*<button id="butConnectSocket">Connect</button>*@
    <div id="divLog"></div>
    <div id="divMessage">
        <textarea id="txtMessage" placeholder="Type message here then click Send"></textarea>
        <button id="butSend">Send</button>
        <div id="divButtons"></div>
    </div>
</div>
<div data-role="collapsible" data-collapsed="false" style="">
    local counter:<span id="spanLocalCounter"></span>
    <button id="butIncrementLocal">Increment local counter</button>
    server counter:<span id="spanServerCounter"></span>
    <button id="butIncrement">Increment server counter</button>
</div>
<div data-role="collapsible" data-collapsed="false" style="">
    <h3>Customer Screen Transmitter</h3>
    <button id="butCustomerTransmit">Transmit</button>
</div>

<a href="/signin">Sign in</a>
