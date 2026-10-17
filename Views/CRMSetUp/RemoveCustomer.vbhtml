@Code    
    ViewData("Title") = "CRM Setup"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code
      
    <script type="text/javascript" src="../../Scripts/sources/CRMSetup/RemoveCustomer.js"></script>    

    <div id="Welcome" data-role="content">
	    <div data-role="header" data-theme="d">
		    <h1>Team Members</h1>
	    </div><!-- /header -->

	    <div data-role="content" data-theme="d">
            <div>
                Do you want to remove <span id="lblName"> </span> from <span id="lblBranch"> </span>  Team?
            </div>

            <br /> <br />            
            <div style="text-align:center"> <a href="#" id="btnRemove" data-role="button">Remove Customer from Team</a> </div>
            <div style="text-align:center"> <a href="#" id="btnCancel" data-role="button">Cancel</a> </div>
	    </div> <!-- /content -->	
    </div>           

</body>
</html>

