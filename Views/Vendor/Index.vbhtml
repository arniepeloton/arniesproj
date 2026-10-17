@Code
    ViewData("Title") = "Vendor Information Sheet"
    Layout = "~/Views/Main/Layout.vbhtml"
End Code

      @If ProWeb.Common.UseMinifiedJavascripts Then
        @<text>
        </text>
    Else
        @<text>
        </text>
    End If
    

<h2>@ViewData("Title")</h2>
<a href="/vendor/edit" class="button ui-icon-plus">Add New Vendor </a>