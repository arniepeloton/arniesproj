@Code    
    ViewData("Title") = "Sales Quota"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code    
    <script type="text/javascript" src="../../Scripts/sources/CRMSetup/QuotaSetup.js"></script>


	    <div data-role="content" data-theme="c">
           
           <div>

            <h4> Basis for Tier 1 Minimum Sales Quota: </h4> <br />

            <div data-role="fieldcontain">
                <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
                    <div ID='Yr3' role="heading" class="ui-controlgroup-label"></div>
				    <div id="lblQuotaYr3"> </div> 
		        </fieldset>                	            
            </div>

            <div data-role="fieldcontain">
                <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
                    <div ID='Yr2' role="heading" class="ui-controlgroup-label"></div>
				    <div id="lblQuotaYr2"> </div> 
		        </fieldset>                	            
            </div>

            <div data-role="fieldcontain">
                <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
                    <div ID='Yr1' role="heading" class="ui-controlgroup-label"></div>
				    <div id="lblQuotaYr1"> </div> 
		        </fieldset>                	            
            </div>
            
            <div data-role="fieldcontain">
                <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
                    <div role="heading" class="ui-controlgroup-label"><b> Minimum Team Sales Quota </b></div>
				    <div id="lblMinTeamSalesQuota"> </div> 
		        </fieldset>                	            
            </div>

            @*<Month> <Y-3>	Sales		P3,000,000 <br />
            <Month> <Y-2>	Sales		P3,100,000 <br />
            <Month> <Y-1>	Sales		P3,000,000 <br />
            <Month> 	P300,000 *@

           </div>

           <div>
           
            <h4> TIER 1 </h4>

            <div data-role="fieldcontain">
	            <label for="txtQuotaFrom1"><b> Minimum Sales Quota (MSQ) (%) </b></label>
	            <input type="text" id="txtQuotaFrom1" />                
            </div>

            <div data-role="fieldcontain">
	            <label for="txtCommision1"><b> Sales Incentive Rate (SIR) (%)</b></label>
	            <input type="text" id="txtCommision1" />
            </div>
        </div>
            
        <div>
           
            <h4> TIER 2 </h4>

            <div data-role="fieldcontain">
	            <label for="txtGrowthFromQuota1"><b> Growth Rate from Tier 1 (%)</b></label>
	            <input type="text" id="txtGrowthFromQuota1" />
            </div>

            <div data-role="fieldcontain">
                <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
                    <div role="heading" class="ui-controlgroup-label"><b>Minimum Sales Quota (MSQ)</b></div>
				    <div id="lblQuotaFrom2"> </div> 
		        </fieldset>                	            
            </div>

            <div data-role="fieldcontain">
	            <label for="txtCommision2"><b> Sales Incentive Rate (SIR) (%)</b></label>
	            <input type="text" id="txtCommision2" />
            </div>            
       </div>
            
        <div>
           
            <h4> TIER 3 </h4>

            <div data-role="fieldcontain">
	            <label for="txtGrowthFromQuota1"><b> Growth Rate from Tier 1 (%)</b></label>
	            <input type="text" id="txtGrowthFromQuota2" />
            </div>

            <div data-role="fieldcontain">
                <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
                    <div role="heading" class="ui-controlgroup-label"><b>Minimum Sales Quota (MSQ)</b></div>
				    <div style="text-align:left" id="lblQuotaFrom3"> </div>
		        </fieldset>                	            
            </div>

            <div data-role="fieldcontain">
	            <label for="txtCommision2"><b> Sales Incentive Rate (SIR) (%)</b></label>
	            <input type="text" id="txtCommision3" />
            </div>            
       </div>

			<div style="text-align:center"> <a id="Done" data-role="button">Done</a> </div>

        <mValue id="mValueQuotaFrom1"> </mValue> 
        <mValue id="mValueQuotaFrom2"> </mValue> 
        <mValue id="mValueQuotaFrom3"> </mValue> 
	    </div> <!-- /content -->	
    

