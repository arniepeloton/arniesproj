@Code
    ViewData("Title") = "Device App WhiteList"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
@Section head
    <script>
        require([GetPathFormat('/Scripts/sources/DeviceMDMConfig/DeviceAppsWhiteList.js?crc=')]);     
    </script>
</script>
End Section
<div id="pageMain" data-role="page">
    <div data-role="header">
        <h1>
            List of White Listed Applications</h1>
    </div>
    <ul id="lstAppWhiteList" data-role="listview" data-inset="true" style="min-width: 210px;">
    </ul>
    <a data-role="button" data-rel="back" data-theme="c">Back</a>
</div>
<div id="pageDetails" data-role="page">
    <div data-role="header">
        <h1 id="headerWL">
            Add White-Listed Applications</h1>
    </div>
    <div data-role="content">
        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
                <legend>Application Title <span style="color: #FF0000;">*</span></legend>
                <input type="text" id="txtApplicationName" data-role="field-contain" >
                @*<span class="smallfontRed" id="spanApplicationTitle" style="display: none;">This field
                    is required.</span>*@
            </fieldset>
        </div>
        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
                <legend>Application Name <span style="color: #FF0000;">*</span></legend>
                <input type="text" id="txtApplicationDesc" data-role="field-contain">
               @* <span class="smallfontRed" id="spanApplicationName" style="display: none;">This field
                    is required.</span>*@
            </fieldset>
        </div>
        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
                <legend>Directory </legend>
                <input type="text" id="txtKeywords" data-role="field-contain">
            </fieldset>
        </div>
    </div>
   @* <a data-role="button" id="btnAppsWhiteListSave" data-theme="c">Add this app to company-wide white list</a> 
*@
     <a data-role="button" id="btnAppsWhiteListDelete" data-theme="c">Delete this app from white list</a> 


    @*<a style="display:none;" data-role="button" id="btnAppsWhiteListSpecificSave" data-theme="c">Add this app to policy white list</a> *@
   @* <a data-role="button" id="btnAppsWhiteListBack" data-theme="c">View All White-Listed Application</a>*@
    <a data-role="button"  data-rel="back" data-theme="c">Back</a>
</div>
</div> 