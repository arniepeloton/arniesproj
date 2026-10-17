@Code
    ViewData("Title") = "Dashboard Settings"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/Settings.js?crc=')]);
    </script>
    
End Section

<div data-role="page">
    <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>

    <div data-role="content">        
        <ul data-role="listview" id="lstOrganizationProductFeature" data-inset="true" data-split-icon="delete">   
            
        </ul>

        <br />
        <div> <a id="btnAddCompanyProductFeatures" data-role="button" data-icon="plus">Add Product Feature</a> </div>
        <div> <a id="btnDone" data-role="button">Done</a> </div>

        <div data-role="popup" id="panProductFeatures" data-theme="b" class="ui-corner-all" data-overlay-theme="a" > 
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Product Features</h3></center>
            <div data-role="content">
                <form>
                    <table id="tblProductFeature" style="width:100%">                    
                    </table>    
                    <a id="btnAddSelectedFeatures" data-role="button" href="#" data-theme="e">Add Selected Features</a>                    
                    @*<ul id="lstProductFeature" data-role="listview" data-filter="true">
                    </ul>*@
                </form>
            </div>       
        </div>

    </div>

</div>





