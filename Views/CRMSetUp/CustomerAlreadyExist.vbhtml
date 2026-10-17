@Code    
    ViewData("Title") = "CRM Setup"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code

    <script type="text/javascript" src="../../Scripts/sources/CRMSetup/CustomerAlreadyExist.js"></script>

    <div id="Welcome" data-role="content">
	    <div data-role="header" data-theme="d">
		    <h4>Customer Already Assigned</h4>
	    </div><!-- /header -->

	    <div data-role="content">
            <div>
                Client <span id="lblCustomer1"> </span> has already been assigned to the <span id="lblTeam1"> </span> team. <br />
                This team is composed of the following: 
                    <div id="ListOfMembers">
                    
                    </div>

                    <br /> <br />

                You may not assign <span id="lblCustomer2"> </span> to  <span id="lblTeamName"> </span> while this client is still assigned to another team.

                    <br /> <br />

                We prevent internal client sniping within this company to preserve
                smooth inter-team relations.
                In case you strongly believe that you or your team can do a better
                job in serving this client due to several reasons, you may request for
                a client re-assignment from your sales manager. Please bear in
                mind that requests are subject to approval and discussion with the
                team that has current ownership of the client’s account.
                <br /> <br />
                You may also contact <span id="lblTeam2"> </span> at
                <span id="lblEmail"> </span> to discuss things first.
                Thank you for your cooperation.
            </div>

            <br /> <br />

            @*<div style="text-align:center"> <a href="#" id="btnAdd" data-role="button">Continue Adding this Customer</a> </div>
            <div style="text-align:center"> <a href="#" id="btnCancel" data-role="button">Select Different Customer</a> </div>*@

            <div style="text-align:center"> <a href="#" id="btnOK" data-role="button">OK</a> </div>
	    </div> <!-- /content -->	
    </div>           

</body>
</html>

