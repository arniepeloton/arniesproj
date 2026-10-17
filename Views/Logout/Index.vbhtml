@Code
    ViewData("Title") = "Logout"
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

<p>@ViewData("Message")
</p>