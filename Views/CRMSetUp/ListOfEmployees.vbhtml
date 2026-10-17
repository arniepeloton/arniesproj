@Code    
    ViewData("Title") = "CRM Setup"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code
 
    <script type="text/javascript" src="../../Scripts/sources/CRMSetup/ListOfEmployees.js"></script>    

    <div id="Welcome" data-role="content">
	    <div data-role="header" data-theme="d">
		    <h1>Employees</h1>
	    </div><!-- /header -->

	    <div data-role="content" data-theme="d">
            @*<div class="ui-grid-b">
                <div class="ui-block-a" style="text-align:center; margin-bottom:5px; width:10%"> <h3> Search : </h3> </div>
                <div class="ui-block-b" style="margin-top:15px; text-align:right; width:80%"> <input type="text" id="txtEmployee" placeholder="Enter Employee Name"/> </div>
                <div class="ui-block-c" style="width:10%"> <a href="#" id="btnGo" data-role="button"> Go </a> </div>
            </div>*@

            @*<div data-role="fieldcontain">
	            <label for="txtEmployee"> <b> Search : </b> </label>
	            <input type="text" id="txtEmployee" style="width:50%"/>                
            </div>

            <div style="text-align:center"> <a href="#" id="btnGo" data-role="button">Go</a> </div>*@

            <br />

            <div>
                <ul id="lstEmployees" data-role="listview" data-theme="d" data-divider-theme="d">
                 </ul>
            </div>

            <br /> <br />

            <div style="text-align:center"> <a href="#" id="btnBack" data-role="button">Back</a> </div>
	    </div> <!-- /content -->	
    </div>           

</body>
</html>

