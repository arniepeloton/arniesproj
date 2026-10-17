@Code    
    ViewData("Title") = "CRM Setup"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code

    <script type="text/javascript" src="../../Scripts/sources/CRMSetup/RemoveEmployee.js"></script>    

    <div id="Welcome" data-role="content">
	    <div data-role="header" data-theme="d">
		    <h1>Remove Employee</h1>
	    </div><!-- /header -->

	    <div data-role="content" data-theme="d">
            <div>
                <h4> Please confirm that <span id='lblName'> </span> will be remove from <span id='lblTeamName'> </span> Team starting </h4> <br />  
            </div>

            <div class="ui-bar-c" style="border:none">                    
                <select id="SmonthEnd">
	                <option>Month</option>
	                <option value="0">January</option>
					<option value="1">February</option>
					<option value="2">March</option>
					<option value="3">April</option>
                    <option value="4">May</option>
                    <option value="5">June</option>
                    <option value="6">July</option>
                    <option value="7">August</option>
                    <option value="8">September</option>
                    <option value="9">October</option>
                    <option value="10">November</option>
                    <option value="11">December</option>
                </select>
            </div>

	        <div class="ui-bar-c" style="border:none">
                <div class="ui-grid-a">
                    <div class="ui-block-a" >
                        <select id="SdayEnd">	                            
                        </select>
                    </div>
                    <div class="ui-block-b">
                        <select id="SyearEnd">	                            
                        </select>
                    </div>
                </div>
            </div>

            <br /> <br />

			<div style="text-align:center"> <a href="#" id="btnConfirm" data-role="button">Confirm</a> </div>

            <div style="text-align:center"> <a href="#" id="btnCancel" data-role="button">Cancel</a> </div>

	    </div> <!-- /content -->	
    </div>           

</body>
</html>

