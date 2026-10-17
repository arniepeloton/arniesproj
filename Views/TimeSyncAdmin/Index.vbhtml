@Code
    ViewData("Title") = "DRP Tables"
    Layout = "~/Views/Shared/Layout2.vbhtml"
End Code
<script>
    require(['TimeSyncAdmin']);
</script>
 <h2>
        Time Sync Admin</h2>
    <div id="divTimeSyncAdmin">
        <form id="formTimeSyncAdmin" dojotype="dijit.form.Form">
        <table>
            <tr>
                <td>
                    Threshold of non-connection (in minutes):
                </td>
                <td>
                    <input id="txtThreshold" dojotype="dijit.form.ValidationTextBox" data-dojo-props="regExp:'\\d{1,4}', invalidMessage:'This must be a number. 1 to 4 digits'" required="true"
                    value="@ViewData("Threshold")"
                     />
                </td>
            </tr>
            
            <tr>
                <td>
                    Emails that will receive alerts:
                </td>
                <td>
                    <textarea id="txtEmails" data-dojo-type="dijit.form.Textarea">@ViewData("Emails")</textarea>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <div>
                        <button id="butSubmit" data-dojo-type="dijit.form.Button" type="submit">Update</button>
                    </div>
                </td>
            </tr>
        </table>
        </form>
    </div>