@Code    
    ViewData("Title") = "Sales Quota"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code 
    <script type="text/javascript" src="../../Scripts/sources/CRMSetup/Quota.js"></script>


	    <div data-role="content" data-theme="c">
            <h4> Please review the sales quota and sales incentive listed below </h4>
            <div style="text-align:center"> <a href="#" id="btnCalculate" data-role="button">Recalculate all Tier 1 Monthly Sales Quota</a> </div>

            <br />

            <ul id="lstQuota" data-role="listview">
            </ul>

                    <br />
                
			<div style="text-align:center"> <a href="#" id="btnNext" data-role="button">Next</a> </div>

	    </div> <!-- /content -->	