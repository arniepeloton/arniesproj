@Code    
    ViewData("Title") = "CRM Setup"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code

    <script type="text/javascript" src="../../Scripts/sources/CRMSetup/EditItem.js"></script>

    <div id="Welcome" data-role="content">
	    <div data-role="header" data-theme="d">
		    <h4>Edit Item Parameters</h4>
	    </div><!-- /header -->

	    <div data-role="content">
            <div data-role="fieldcontain">
                <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
                    <div role="heading" class="ui-controlgroup-label"><b> Item : </b></div>
				    <div id="lblItem"> </div> 
		        </fieldset>
            </div>
            
            <div data-role="fieldcontain">
                <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
                    <div role="heading" class="ui-controlgroup-label"><b>SKU Sales Quota By Qty Volume :</b></div>
				    <input type="text" id="txtQty" style="width:50%"/>                    
		        </fieldset>
            </div>
            
            <div data-role="fieldcontain">
                <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
                    <div role="heading" class="ui-controlgroup-label"><b>Unit Measure :</b></div>
				    <div id="lblUnitMeasure"> </div> 
		        </fieldset>
            </div>

            <div data-role="fieldcontain">
	            <label for="txtIncentive"><b>SKU Incentive (%):</b></label>
	            <input type="text" id="txtIncentive" style="width:30%"/>                
            </div>

			<div style="text-align:center"> <a href="#" id="btnDone" data-role="button">Done</a> </div>

	    </div> <!-- /content -->	
    </div>           

</body>
</html>

