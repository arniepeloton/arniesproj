@Code
    If Request.QueryString("Partial") <> "1" Then
        Layout = "~/Views/Shared/Layout3.vbhtml"
    End If
End Code
@Section head
    @RenderSection("head", False)
End Section
<div data-role="page">
    <div data-role="header" data-position="fixed1" style="background: none white; border: none">
        <div class="ui-grid-c" >
            <div class="ui-block-a">
                <div id="btnCustomer" style="margin:0px; height: 80px;" data-role="button" data-theme="gray" data-corners="false" class="HotButtons HotButtonsText">
                    <span class="SAMPLE">Customer</span>
                </div>
            </div>
            <div class="ui-block-b">
                <div id="btnPOSMode" style="margin:0px; height: 80px;" data-role="button" data-theme="gray" data-corners="false" class="HotButtons HotButtonsText">
                    <span>POS Mode</span>
                </div>
            </div>
            <div class="ui-block-c">
                <div id="btnMoreOptions" style="margin:0px; height: 80px;" data-role="button" data-theme="gray" data-corners="false" class="HotButtons HotButtonsText">
                    <span>More Options</span>
                </div>
            </div>
            <div class="ui-block-d">
                <div id="btnViewCart" style="margin:0px; height: 80px;" data-role="button" data-theme="green" data-corners="false" class="HotButtons HotButtonsText">
                    <span>View Cart</span>
                </div>
            </div>
        </div>
        <div class="ui-grid-b">
        <div class="ui-block-a" style="width: 50%;">
            <input name="txtOrder" style="margin:0px; height: 40px;" readonly/>
        </div>
        <div class="ui-block-b" style="width: 25%;">
            <div style="margin:0px; height: 70px;" data-role="button" data-theme="gray" data-corners="false" class="HotButtons HotButtonsText">
                
                <span>+</span>
            </div>
        </div>
        <div class="ui-block-c" style="width: 25%;">
            <div style="margin:0px; height: 70px;" data-role="button" data-theme="gray" data-corners="false" class="HotButtons HotButtonsText">
                <span>-</span>
            </div>
        </div>
    </div>

        @RenderSection("Header", False)
    </div>
    <div data-role="content" style="padding: 0px;">
        @RenderBody()
    </div>
    @If IsSectionDefined("footer") Then
        @RenderSection("footer")
    Else
        @<div data-role="footer_no_theme">
            <div style="text-align: center; padding-bottom: 15px">
                This mobile site is powered by Mobiz&#8482;.<br />
                Copyright 2013, 1Mobiz Inc., All Rights Reserved.<br />
                www.mobiz-apps.com</div>
        </div>
    End If
</div>
@RenderSection("OtherPages", False)
