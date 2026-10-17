@Code
    If Common.WebRequest.QueryString("Partial") <> "1" Then
        Layout = "~/Views/Shared/Layout3.vbhtml"
    End If
End Code
@Section head
    @RenderSection("head", False)
End Section

    <div id="h5FYX6Q8AA4" class="hide1st" data-role="page" id="pageMain">

        <div id="hU7FBL89B9X" data-role="content" style="padding: 0px;">
            @RenderBody()
        </div>
    @If IsSectionDefined("footer") Then
        @RenderSection("footer")
    Else
        @<div class="hide1st" data-role="footer_no_theme">
            <div style="text-align: center; padding-bottom: 15px">
                This mobile site is powered by Mobiz&#8482;.<br />
                Copyright 2013, 1Mobiz Inc., All Rights Reserved.<br />
                www.mobiz-apps.com
            </div>
        </div>
    End If
</div>





@RenderSection("OtherPages", False)
