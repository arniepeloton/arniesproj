@Code    
    ViewData("Title") = "Sales Quota Settings"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code
    <script type="text/javascript" src="../../Scripts/sources/CRMSetup/SalesQuotaPerMember.js"></script>    

        <div data-role="content" data-theme="d">

        <div>
                <h4>TIERING SCHEME for Sales Quota and Sales Incentive Rates</h4> <br /> 
        </div>

        <div data-role="fieldcontain">
	        <label for="txtQuotaPerMember"><b> Minimum Sales Quota (MSQ) per Team Member : </b></label>
	        <input type="text" id="txtQuotaPerMember"/> 
        </div>
        
        <div>
            <br />
            * Minimum Sales Quota for Tier 1 is the max value of these 3 figures: <br /> <br /> <br />
            a.) Monthly historical sales 2 years ago <br /> <br />
            b.) Monthly historical sales 1 year ago <br /> <br />
            c.) Minimum sales quota per team member x number of team members <br /> <br />
        </div>

        <div data-role="fieldcontain">
	        <label for="txtTier1IncentiveRate"><b>Tier 1: Sales Incentive Rate (SIR) (%) : </b> </label>
	        <input type="text" id="txtTier1IncentiveRate"/>
        </div>

        <hr />

        <div data-role="fieldcontain">
	        <label for="txtTier2RequiredGrowth"><b>Tier 2: Required Growth Rate from Tier 1 MSQ (%) : </b> </label>
	        <input type="text" id="txtTier2RequiredGrowth"/> 
            <br /> <br />
            <label for="txtTier2IncentiveRate"><b>Tier 2: Sales Incentive Rate (SIR) (%) : </b> </label>
	        <input type="text" id="txtTier2IncentiveRate"/>
        </div>

        <hr />

        <div data-role="fieldcontain">
	        <label for="txtTier3RequiredGrowth"><b>Tier 3: Required Growth Rate from Tier 1 MSQ (%) : </b> </label>
	        <input type="text" id="txtTier3RequiredGrowth"/> 
            <br /> <br />
            <label for="txtTier3IncentiveRate"><b>Tier 3: Sales Incentive Rate (SIR) (%) : </b> </label>
	        <input type="text" id="txtTier3IncentiveRate"/>
        </div>

            <div style="text-align:center"> <a href="#" id="btnNext" data-role="button"> Next </a> </div>
	    </div> <!-- /content -->	
    
