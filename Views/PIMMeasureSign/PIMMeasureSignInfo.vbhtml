@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/PIMMeasureSign/PIMMeasureSignInfo.js?crc=')
        ]);
    </script>
    
End Section

<div id="pagePIMMeasureSignInfo" data-role="page">

    <div data-role="header">
        <h1>Sign Info</h1>
    </div>

    <div data-role="content" >

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtPIMMeasureSignID"/>                
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
	            <div><label for="txtPIMMeasureSign">Sign: <span style="color: #FF0000;" >*</span></label></div>
	            <div><input type="text" id="txtPIMMeasureSign"/></div>
            </fieldset>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnPIMMeasureSignInfoDone" data-role="button">Done</a> </div>
        </div>

    </div>

</div>

