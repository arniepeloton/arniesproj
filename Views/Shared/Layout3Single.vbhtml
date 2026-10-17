@Code
    If Common.WebRequest.QueryString("Partial") <> "1" Then
        Layout = "~/Views/Shared/Layout3.vbhtml"
    End If
End Code
@Section head
    @RenderSection("head", False)
End Section
<div class="hide1st" data-role="page">
    <div data-role="header">
        <a href="#" data-icon="arrow-l" data-rel="back" style="display:none;">Back</a>
        <h1 id="lblPageHeader">@ViewData("Title")</h1>
    </div>
    <div data-role="content">
        @RenderBody()
    </div>
    @If IsSectionDefined("footer") Then
        @RenderSection("footer")
    Else
        @<div data-role="footer_no_theme">
            <div style="text-align: center; padding-bottom: 15px">
                This mobile site is powered by Mobiz&#8482;.<br />
                Copyright 2013, 1Mobiz Inc., All Rights Reserved.<br />
                www.mobiz-apps.com
            </div>
        </div>
    End If
</div>
@RenderSection("OtherPages", False)
