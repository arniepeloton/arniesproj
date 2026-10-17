@Code
    ViewData("Title") = "Date Time Setup"
    Layout = "~/Views/IT/Layout.vbhtml"
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

<h1>@ViewData("Title")</h1>
<h2>
    Date Time</h2>
@If CStr2(ViewData("Message")).Length > 0 Then
    @<div class="divMessage">
        @ViewData("Message")
    </div>
End If
<form action="" method="post">
<table>
    <tbody>
        <tr>
            <td>
                Server datetime:
            </td>
            <td>
                @ViewData("ServerDateTime")
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td>
                Local datetime:
            </td>
            <td>
                @ViewData("LocalDateTime")
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td>
                Database server datetime:
            </td>
            <td>
                @ViewData("DatabaseServerDateTime")
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td>
                Database local datetime:
            </td>
            <td>
                @ViewData("DatabaseLocalDateTime")
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td>
                Change local datetime:
            </td>
            <td>
                <input id="txtDateTime" name="txtDateTime" />
            </td>
            <td>
                <input type="submit" value="Set" />
            </td>
        </tr>
    </tbody>
</table>
</form>
