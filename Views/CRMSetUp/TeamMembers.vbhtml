@Code    
    ViewData("Title") = "Team Members"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code

    <script type="text/javascript" src="../../Scripts/sources/CRMSetup/TeamMembers.js"></script>    

	    <div data-role="content" data-theme="d">
         
            <div data-role="fieldcontain">
	            <label for="txtEmployee"> <b> Search : </b> </label>
	            <input type="text" id="txtEmployee"/>                
            </div>

            <div style="text-align:center"> <a href="#" id="btnAdd" data-role="button">Go</a> </div>

            <br />

            <div>
                <ul id="lstTeamMembers" data-role="listview">
                 </ul>
            </div>
            <br /><br /> 
            <b>Legend : </b><br /> <br /> 
            
            @*<a href="#" data-role="button" data-icon="delete" data-iconpos="notext" class="ui-btn ui-shadow ui-btn-corner-all ui-mini ui-btn-inline ui-btn-icon-notext ui-btn-up-c">
                <span class="ui-btn-inner ui-btn-corner-all"><span class="ui-btn-text">Delete</span><span class="ui-icon ui-icon-delete ui-icon-shadow">&nbsp;</span></span></a>*@
            
            <a href="#" data-role="button" data-icon="star" data-iconpos="notext" class="ui-btn-inline"></a>
            Team Leader 
            <a href="#" data-role="button" data-icon="gear" data-iconpos="notext" class="ui-btn-inline"></a>
            Assign as team leader
             
            <br /> <br />
            <div style="text-align:center"> <a href="#" id="btnNext" data-role="button">Next</a> </div>
	    </div> <!-- /content -->	
    