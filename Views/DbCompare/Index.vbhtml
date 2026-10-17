@Code
    ViewData("Title") = "DB Compare"
    Layout = "~/Views/IT/Layout.vbhtml"
End Code
    <script type="text/javascript">
    
    </script>
    @If ProWeb.Common.UseMinifiedJavascripts Then
        @<text>
        <script src="/Scripts/minified/CommonAdmin.js" type="text/javascript"></script>
        </text>
    Else
        @<text>
        <script src="/Scripts/sources/CommonAdmin/DbCompare.js" type="text/javascript"></script>
        </text>
    End If
    <style type="text/css">
        .tableProGrid td
        {
            padding: 10px;
        }
    </style>
<h1>@ViewData("Title")</h1>
<h2>
    DBCompare</h2>
<a href="/DbCompare?create=1">Create a downloadable XML of the database's current schema</a>
<div>
    <h3>
        Schema history</h3>
    <table class="tableProGrid">
        @For Each pDRV As DataRowView In CType(ViewData("Data"), DataTable).DefaultView
            @<tr>
                <td class="ui-widget-content">
                    <a href="/DbCompare/download/@pDRV("Name")">@pDRV("Name")</a>
                </td>
                <td class="ui-widget-content">
                    @pDRV("DateModified")
                </td>
            </tr>
        Next pDRV
    </table>
</div>
