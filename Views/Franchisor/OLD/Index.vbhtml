@Code
    ViewData("Title") = "Group Settings"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/FranchisorCentral/FranchisorCentral.js?crc=')]);
    </script>
    
End Section

<div data-role="page">
    <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>
     
    <div data-role="content">
        <div>
            <ul data-role="listview" data-inset="true">   
				<li data-role="list-divider">Group List</li>
                <li>
                    <div data-role="fieldcontain"><a id="btnFranchiseeGroup" data-role="button" data-icon="plus" data-inline="true">Add Group</a></div>
                                        
                    <div data-role="fieldcontain" id="divOrgRecordCountTeam"></div>
                    <div>
                        <ul data-role="listview" id="lstFranchiseeGroup" data-inset="true">   
                        </ul>
                    </div>
                </li>
            </ul>
        </div>
        
        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnFranchiseeGroupDone" data-role="button">Done</a> </div>
        </div>

    </div>

</div>

<div id="pageFrancishorGroup" data-role="page">
    <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>

    <div data-role="content">
      <div>
        <ul data-role="listview" data-inset="true">
            <li data-role="list-divider">Group Information</li>
            <li>                
                <div data-role="fieldcontain">
                <label for="txtGroupName">Group Name :</label>
                <input type="text" id="txtGroupName"/>
                </div>

                <div data-role="fieldcontain">
                    <label for="txtGroupDescription">Group Description :</label>
                    <input type="text" id="txtGroupDescription"/>
                </div>

                <div data-role="fieldcontain">
                    <label for="txtRoyaltyFee">Royalty Fee (%) :</label>
                    <input type="text" id="txtRoyaltyFee"/>
                </div>

                <div data-role="fieldcontain">
                    <label for="txtMarketingFee">Marketing Fee (%) :</label>
                    <input type="text" id="txtMarketingFee"/>
                </div>

                <br />
                
                @*<div data-role="fieldcontain">
                    <label for="txtGroupHead">Group Head(s) :</label>
                    <input type="text" id="txtGroupHead"/>
                </div>

                <div data-role="fieldcontain">                
                    <a id="btnSearchEmployee" data-role="button">Search Employee</a>
                </div>*@

             @*   <div>
                    <ul data-role="listview" data-inset="true" id="lstSchedule" data-icon="delete">
                    </ul>
                </div>*@

                <br />

                <div data-role="fieldcontain">
                    <label for="txtGroupFOWST">Group Distribution Center :</label>
                    <input type="text" id="txtGroupFOWST"/>
                </div>

                <div style="text-align:center"> <a id="btnSearchFOWST" data-role="button">Search FOWST</a> </div>
            </li>
        </ul>

      </div> 

      <div>
        <ul data-role="listview" data-inset="true">
            <li data-role="list-divider">Group Head(s)</li>
            <li>
                <div data-role="fieldcontain"><a id="btnAddGroupHead" data-role="button" data-icon="plus" data-inline="true">Add Group Head</a></div>
                    
                    <div>
                        <ul data-role="listview" data-inset="true" id="lstGroupHeads" data-icon="delete">
                        </ul>
                    </div>
            </li>
        </ul>
    </div>

      <div>
        <ul data-role="listview" data-inset="true">
            <li data-role="list-divider">Members (FOWSTS)</li>
            <li>
                <div data-role="fieldcontain"><a id="btnAddFranchisee" data-role="button" data-icon="plus" data-inline="true">Add Franchisee</a></div>
                    
                    <div>
                        <ul data-role="listview" data-inset="true" id="lstGroupMembers">
                        </ul>
                    </div>
            </li>
        </ul>
      </div>

        <div>
          <ul data-role="listview" data-inset="true">
            <li data-role="list-divider">Order Cut-Off</li>
            <li>                
                <div data-role="fieldcontain">
                    <label for="selSchedule">Cut-Off Type :</label>
                    <select id="selSchedule">
                        <option id="optDaily">Daily</option>
                        @*<option id="optWeekly">Weekly</option>
                        <option id="optBiWeekly">Every 2 Weeks (BiWeekly)</option>*@
                        @*<option>Every 4 Weeks</option>*@
                    </select>
                </div>

                <div data-role="fieldcontain">
                    <label for="txtScheduleName">Cut-Off Name :</label>
                    <input type="text" id="txtScheduleName"/>
                </div>

                <div data-role="fieldcontain">
                    <label for="selScheduleDay">Cut-Off Day :</label>
                    <select id="selScheduleDay">
                        <option id="optMonday">Monday</option>
                        <option id="optTuesday">Tuesday</option>
                        <option id="optWednesday">Wednesday</option>
                        <option id="optThursday">Thursday</option>
                        <option id="optFriday">Friday</option>
                        <option id="optSaturday">Saturday</option>
                        <option id="optSunday">Sunday</option>
                        @*<option>Every 4 Weeks</option>*@
                    </select>
                </div>

                <div data-role="fieldcontain">
                    <fieldset data-role="controlgroup" data-type="horizontal">
                        <legend> Time : </legend>
                            <select id="selHourFrom">
                                <option value=""> hh:mm </option>
                                <option value="01:00"> 1:00 </option>
                                <option value="01:15"> 1:15 </option>
                                <option value="01:30"> 1:30 </option>
                                <option value="01:45"> 1:45 </option>

                                <option value="02:00"> 2:00 </option>
                                <option value="02:15"> 2:15 </option>
                                <option value="02:30"> 2:30 </option>
                                <option value="02:45"> 2:45 </option>

                                <option value="03:00"> 3:00 </option>
                                <option value="03:15"> 3:15 </option>
                                <option value="03:30"> 3:30 </option>
                                <option value="03:45"> 3:45 </option>

                                <option value="04:00"> 4:00 </option>
                                <option value="04:15"> 4:15 </option>
                                <option value="04:30"> 4:30 </option>
                                <option value="04:45"> 4:45 </option>

                                <option value="05:00"> 5:00 </option>
                                <option value="05:15"> 5:15 </option>
                                <option value="05:30"> 5:30 </option>
                                <option value="05:45"> 5:45 </option>

                                <option value="06:00"> 6:00 </option>
                                <option value="06:15"> 6:15 </option>
                                <option value="06:30"> 6:30 </option>
                                <option value="06:45"> 6:45 </option>

                                <option value="07:00"> 7:00 </option>
                                <option value="07:15"> 7:15 </option>
                                <option value="07:30"> 7:30 </option>
                                <option value="07:45"> 7:45 </option>

                                <option value="08:00"> 8:00 </option>
                                <option value="08:15"> 8:15 </option>
                                <option value="08:30"> 8:30 </option>
                                <option value="08:45"> 8:45 </option>

                                <option value="09:00"> 9:00 </option>
                                <option value="09:15"> 9:15 </option>
                                <option value="09:30"> 9:30 </option>
                                <option value="09:45"> 9:45 </option>

                                <option value="10:00"> 10:00 </option>
                                <option value="10:15"> 10:15 </option>
                                <option value="10:30"> 10:30 </option>
                                <option value="10:45"> 10:45 </option>

                                <option value="11:00"> 11:00 </option>
                                <option value="11:15"> 11:15 </option>
                                <option value="11:30"> 11:30 </option>
                                <option value="11:45"> 11:45 </option>

                                <option value="12:00"> 12:00 </option>
                                <option value="12:15"> 12:15 </option>
                                <option value="12:30"> 12:30 </option>
                                <option value="12:45"> 12:45 </option>
                            </select>
                            <select id="selDayOfHour">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                         
                    @*<label for="txtTime"></label>
                    <input type="text" id="txtTime"/>*@
                </div>

                <div style="text-align:center"> <a id="btnAddCutOffToList" data-role="button">Add To Schedule</a> </div>

                <div>
                    <ul data-role="listview" data-inset="true" id="lstSchedule" data-icon="delete">
                    </ul>
                </div>
            </li>
         </ul>
        </div>

         <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnOrgGroupSave" data-role="button">Save</a> </div>
        </div>
    </div>
</div>




