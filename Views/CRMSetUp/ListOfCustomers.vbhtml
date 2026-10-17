@Code    
    ViewData("Title") = "CRM Setup"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code

    <script type="text/javascript" src="../../Scripts/sources/CRMSetup/ListOfCustomers.js"></script>    

    <div id="Welcome" data-role="content">
	    <div data-role="header" data-theme="d">
		    <h1>Customers</h1>
	    </div><!-- /header -->

	    <div data-role="content" data-theme="d">            
            <div>
                <ul id="lstCustomers" data-role="listview" data-theme="d" data-divider-theme="d" style="margin:3px">
                 </ul>
            </div>

            <br /> <br />

            <div style="text-align:center"> <a href="#" id="btnBack" data-role="button">Back</a> </div>
	    </div> <!-- /content -->	
    </div>           

</body>
</html>

