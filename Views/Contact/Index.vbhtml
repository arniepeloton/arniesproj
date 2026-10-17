@Code
    ViewData("Title") = "Contact Us"
    Layout = "~/Views/Main/Layout.vbhtml"
End Code
    <script type="text/javascript">
    
    </script>
    @If ProWeb.Common.UseMinifiedJavascripts Then
        @<text>
        <script src="/Scripts/minified/Contact.js" type="text/javascript"></script>
        </text>
    Else
        @<text>
        <script src="/Scripts/sources/Project/Contact.js" type="text/javascript"></script>
        </text>
    End If
    <style type="text/css">
        
    </style>
<div>Latest News</div>
<div id="divTweetsResult"></div>
<div id="divTweets"></div>
<div id="divCommentPreview" contenteditable="true"></div>
 