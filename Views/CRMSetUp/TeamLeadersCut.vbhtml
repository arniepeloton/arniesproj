@Code    
    ViewData("Title") = "CRM Setup"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code

    <script type="text/javascript" src="../../Scripts/sources/CRMSetup/TeamsLeadersCut.js"></script>

    <div id="Welcome" data-role="content">
	    <div data-role="header" data-theme="d">
		    <h1>Team Leaders Cut</h1>
	    </div><!-- /header -->

	    <div data-role="content" data-theme="d">
            <div data-role="fieldcontain">
	        <label for="txtTeamLeadersCut"><b>Team Leader's Cut (%) :</b> </label>
	        <input type="text" id="txtTeamLeadersCut"/>
        </div>

        <div style="text-align:center"> <a href="#" id="btnNext" data-role="button">Next</a> </div>
	    </div> <!-- /content -->	
    </div>           

</body>
</html>

