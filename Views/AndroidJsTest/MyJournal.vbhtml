@Code
    ViewData("Title") = "Home"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head
    <script>
        require([GetPathFormat('/Scripts3/MyJournal.js?crc=')]);
    </script>
    
End Section

<div data-role="page">
    <div>
        <div data-role="fieldcontain">
            <label for="txtBranch"> Branch ID </label>
            <input type="text" id="txtBranch" />
        </div>

            <div data-role="fieldcontain">
                <div class="metro" style="max-width:252px; overflow:auto;">
                    Date From <span id="txtDateFrom"></span>
                    <div class="fromCalendar calendar"></div>
                </div>
            </div>
            <div data-role="fieldcontain">
                <div class="metro" style="max-width:252px; overflow:auto;">
                    Date To <span id="txtDateTo"></span>
                    <div class="toCalendar calendar"></div>
                </div>
            </div>

        <div data-role="fieldcontain">
            <label for="txtDevice"> Device ID </label>
            <input type="text" id="txtDevice" />
        </div>
        
        <div style="text-align: center">
            <a id="btnGetData" data-role="button">
                <h3>
                    Get Data
                </h3>                
            </a>
        </div>
    </div>
</div>