@Code
    ViewData("Title") = "Welcome"
    Layout = "~/Views/Main/Layout.vbhtml"
End Code

    <script type="text/javascript">
    
    </script>
    @If ProWeb.Common.UseMinifiedJavascripts Then
        @<text>
        </text>
    Else
        @<text>
        </text>
    End If
    <style type="text/css">
        
    </style>

<div>
    Hello @ViewData("FirstName")</div>
<div>
How can we help you today?
</div>