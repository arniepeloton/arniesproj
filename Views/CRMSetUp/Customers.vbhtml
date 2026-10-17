@Code    
    ViewData("Title") = "CRM Setup"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code

    <script type="text/javascript" src="../../Scripts/sources/CRMSetup/Customers.js"></script>    

    <div id="Welcome" data-role="content">
	    <div data-role="header" data-theme="d">
		    <h1>Customers</h1>
	    </div><!-- /header -->
        <div style="font-weight:lighter
	    <div data-role="content" data-theme="d">
           <div data-role="fieldcontain">
	            <label for="txtCustomer"> <b> Add : </b></label>
	            <input type="text" id="txtCustomer" placeholder="Enter Customer Name"/> 
            </div>

            <a href="#" id="btnGo" data-role="button"> Go </a> 
@*
           <div class="ui-grid-b">
                <div class="ui-block-a" style="text-align:center; margin-bottom:5px"> </div>
                <div class="ui-block-b" style="margin-top:15px; text-align:right"> </div>
                <div class="ui-block-c"> </div>
            </div>
*@
            <div>
                <ul id="lstCustomers" data-role="listview" data-theme="d" data-divider-theme="d" style="margin:3px">
                 </ul>
            </div>
            
            <br /> <br />

            <div style="text-align:center"> <a href="#" id="btnNext" data-role="button">Next</a> </div>

	    </div> <!-- /content -->	
    </div>           

</body>
</html>

