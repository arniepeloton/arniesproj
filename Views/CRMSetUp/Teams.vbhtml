@Code    
    ViewData("Title") = "Teams"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code

    <script type="text/javascript" src="../../Scripts/sources/CRMSetup/Teams.js"></script>

	    <div data-role="content" data-theme="d">
            <div data-role="fieldcontain">
                <label for="txtLocation"><b>Office/Branch Location :</b></label>
                <input type="text" id="txtLocation" placeholder="Enter Team Location"/>
            </div>

            <div data-role="fieldcontain" class="ui-field-contain ui-body ui-br">
                <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
                    <div role="heading" class="ui-controlgroup-label"> </div>
                        <div class="ui-controlgroup-controls">			
                            <a href="#" id="btnLocation" data-role="button">Search</a>
		                </div>
                </fieldset>
            </div>

            <div data-role="fieldcontain">
                <label for="txtTeam"><b>Team Name:</b></label>
                <input type="text" id="txtTeam" placeholder="Enter Team Name"/>                                 
            </div>

            <div data-role="fieldcontain" class="ui-field-contain ui-body ui-br">
                <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
                    <div role="heading" class="ui-controlgroup-label"> </div>
                        <div class="ui-controlgroup-controls">			
                            <a href="#" id="btnTeams" data-role="button">Search</a>
		                </div>
                </fieldset>
            </div>


            <div data-role="fieldcontain">
                <label for="txtDescription"><b>Team Objective :</b></label>
                <input type="text" id="txtDescription" placeholder="Enter Team Description"/> 
            </div>

            <div data-role="fieldcontain">
                <label for="txtObjective"><b>Team Description :</b></label>
                <textarea name="textarea" id="txtObjective"> </textarea>
            </div>
                
			<div style="text-align:center"> <a href="#" id="btnNext" data-role="button">Next</a> </div>

	    </div> <!-- /content -->	