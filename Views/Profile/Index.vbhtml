@Code
    ViewData("Title") = "User Profile"
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

<table class="TableForm">
    <tr>
        <td>
            Last name:
        </td>
        <td>
            @ProWeb.Common.WebSession.LastName
        </td>
    </tr>
    <tr>
        <td>
            First name:
        </td>
        <td>
            @ProWeb.Common.WebSession.FirstName
        </td>
    </tr>
    <tr>
        <td>
            Username:
        </td>
        <td>
            @ProWeb.Common.WebSession.Username
        </td>
    </tr>
    <tr>
        <td>
            Access level:
        </td>
        <td>
            @WebSession.Instance.UserAccess.LevelType
        </td>
    </tr>
</table>
