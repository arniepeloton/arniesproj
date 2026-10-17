@Code    
    ViewData("Title") = "CRM Setup"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code

<script type="text/javascript" src="../../Scripts/sources/CRMSetup/ItemIncentive.js"></script>    

    <div id="Welcome" data-role="content">
	    <div data-role="header" data-theme="d">
		    <h1>SKU's with Additional Incentive</h1>
	    </div><!-- /header -->

	    <div data-role="content" data-theme="d">
            <div data-role="fieldcontain">
	            <label for="txtItem"> <b> Search : </b> </label>
	            <input type="text" id="txtItem"/>                
            </div>

            <div style="text-align:center"> <a href="#" id="btnAdd" data-role="button">Go</a> </div>

            <div>
                <ul id="lstItems" data-role="listview" data-theme="d" data-divider-theme="d" style="margin:3px">
                 </ul>
            </div>

            <br /> <br />

            <div style="text-align:center"> <a href="#btnNext" id="btnNext" data-role="button">Next</a> </div>
	    </div> <!-- /content -->	
    </div>           

