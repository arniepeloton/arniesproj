@Code
    ViewData("Title") = "Work Schedule"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head
    <style>
        .ui-field-contain  label.ui-submit  {
            width: 100%;
        }
        .ui-field-contain  .ui-controlgroup-controls  {
            width: 100%;
        }
    </style>

    <script>
        require([GetPathFormat('/Scripts/sources/Organization/OrganizationEmployeeWorkingHours.js?crc=')]);
    </script>

End Section

<div id="pageOrganizationEmployeeSchedule" data-role="page">
    <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>

    <div data-role="content">       
        <div>
            <ul id="lstWorkSchedule" data-role="listview" data-filter="true">
            </ul>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnNewEmployeeSchedule" data-role="button"><h3>Create New Employee Schedule</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnDone" data-role="button"><h3>Done</h3></a>
            </div>
        </div>
    </div>

</div>

<div id="pageNewSchedule" data-role="page">
    <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>

    <div data-role="content">       
        <div data-role="fieldcontain">
            <label for="txtName"> Work Schedule Name: </label>
            <input type="text" id="txtName"/>
        </div>

        <div data-role="fieldcontain">
            <label for="txtDescription"> Description: </label>
            <input type="text" id="txtDescription"/>
        </div>

        <div data-role="fieldcontain">
            <fieldset id="txtEffectivityDateto" data-role="DateCombo" data-usecurrent="1"> 
                <legend> Effectivity Date To : </legend>
            </fieldset>            
        </div> 
               
        @*<div data-role="fieldcontain">
            <fieldset>
                <legend>Schedule Type: </legend>
                <input type="radio" name="rdSort" id="rdWeekly" checked="checked" />
                <label for="rdWeekly">Weekly</label>
                <input type="radio" name="rdSort" id="rdDaily"/>
                <label for="rdDaily">Daily</label>
            </fieldset>
        </div>*@

        <div data-role="fieldcontain">
        <table style="width:100%" data-role="table" data-mode="reflow" class="ui-responsive">
            <thead>
                <th>
                    Day
                </th>
                <th>
                    From
                </th>
                <th>
                    To
                </th>
            </thead>
            
            <tbody>
                @*monday*@
                <tr>
                    <th>
                        Monday
                    </th>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selMondayFrom">
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
                            <select id="selMondayFromDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                        
                    </td>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selMondayTo">
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
                            <select id="selMondayToDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                    </td>
                </tr>
                @*monday lunch break*@
                <tr>
                    <th>
                        Lunch Break
                    </th>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selMondayLunchBreakFrom">
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
                            <select id="selMondayLunchBreakFromDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                        
                    </td>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selMondayLunchBreakTo">
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
                            <select id="selMondayLunchBreakToDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                    </td>
                </tr>
                 @*monday dinner break*@
                <tr>
                    <th>
                        Dinner Break
                    </th>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selMondayDinnerBreakFrom">
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
                            <select id="selMondayDinnerBreakFromDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                        
                    </td>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selMondayDinnerBreakTo">
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
                            <select id="selMondayLunchDinnerToDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <hr />
                    </td>
                </tr>

                @*tuesday*@
                <tr>
                    <th>
                        Tuesday
                    </th>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selTuesdayFrom">
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
                            <select id="selTuesdayFromDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                        
                    </td>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selTuesdayTo">
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
                            <select id="selTuesdayToDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                    </td>
                </tr>
                @*tuesday lunch break*@
                <tr>
                    <th>
                        Lunch Break
                    </th>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selTuesdayLunchBreakFrom">
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
                            <select id="selTuesdayLunchBreakFromDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                        
                    </td>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selTuesdayLunchBreakTo">
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
                            <select id="selTuesdayLunchBreakToDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                    </td>
                </tr>
                 @*tuesday dinner break*@
                <tr>
                    <th>
                        Dinner Break
                    </th>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selTuesdayDinnerBreakFrom">
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
                            <select id="selTuesdayDinnerBreakFromDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                        
                    </td>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selTuesdayDinnerBreakTo">
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
                            <select id="selTuesdayLunchDinnerToDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <hr />
                    </td>
                </tr>
                
                @*wednesday*@
                <tr>
                    <th>
                        Wednesday
                    </th>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selWednesdayFrom">
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
                            <select id="selWednesdayFromDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                        
                    </td>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selWednesdayTo">
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
                            <select id="selWednesdayToDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                    </td>
                </tr>
                @*wednesday lunch break*@
                <tr>
                    <th>
                        Lunch Break
                    </th>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selWednesdayLunchBreakFrom">
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
                            <select id="selWednesdayLunchBreakFromDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                        
                    </td>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selWednesdayLunchBreakTo">
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
                            <select id="selWednesdayLunchBreakToDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                    </td>
                </tr>
                 @*wednesday dinner break*@
                <tr>
                    <th>
                        Dinner Break
                    </th>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selWednesdayDinnerBreakFrom">
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
                            <select id="selWednesdayDinnerBreakFromDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                        
                    </td>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selWednesdayDinnerBreakTo">
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
                            <select id="selWednesdayLunchDinnerToDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <hr />
                    </td>
                </tr>

                @*thursday*@
                <tr>
                    <th>
                        Thursday
                    </th>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selThursdayFrom">
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
                            <select id="selThursdayFromDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                        
                    </td>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selThursdayTo">
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
                            <select id="selThursdayToDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                    </td>
                </tr>
                @*thursday lunch break*@
                <tr>
                    <th>
                        Lunch Break
                    </th>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selThursdayLunchBreakFrom">
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
                            <select id="selThursdayLunchBreakFromDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                        
                    </td>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selThursdayLunchBreakTo">
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
                            <select id="selThursdayLunchBreakToDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                    </td>
                </tr>
                 @*thursday dinner break*@
                <tr>
                    <th>
                        Dinner Break
                    </th>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selThursdayDinnerBreakFrom">
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
                            <select id="selThursdayDinnerBreakFromDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                        
                    </td>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selThursdayDinnerBreakTo">
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
                            <select id="selThursdayLunchDinnerToDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <hr />
                    </td>
                </tr>

                @*friday*@
                <tr>
                    <th>
                        Friday
                    </th>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selFridayFrom">
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
                            <select id="selFridayFromDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                        
                    </td>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selFridayTo">
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
                            <select id="selFridayToDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                    </td>
                </tr>
                @*friday lunch break*@
                <tr>
                    <th>
                        Lunch Break
                    </th>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selFridayLunchBreakFrom">
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
                            <select id="selFridayLunchBreakFromDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                        
                    </td>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selFridayLunchBreakTo">
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
                            <select id="selFridayLunchBreakToDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                    </td>
                </tr>
                 @*friday dinner break*@
                <tr>
                    <th>
                        Dinner Break
                    </th>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selFridayDinnerBreakFrom">
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
                            <select id="selFridayDinnerBreakFromDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                        
                    </td>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selFridayDinnerBreakTo">
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
                            <select id="selFridayLunchDinnerToDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <hr />
                    </td>
                </tr>

                @*saturday*@
                <tr>
                    <th>
                        Saturday
                    </th>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selSaturdayFrom">
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
                            <select id="selSaturdayFromDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                        
                    </td>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selSaturdayTo">
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
                            <select id="selSaturdayToDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                    </td>
                </tr>
                @*saturday lunch break*@
                <tr>
                    <th>
                        Lunch Break
                    </th>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selSaturdayLunchBreakFrom">
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
                            <select id="selSaturdayLunchBreakFromDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                        
                    </td>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selSaturdayLunchBreakTo">
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
                            <select id="selSaturdayLunchBreakToDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                    </td>
                </tr>
                 @*saturday dinner break*@
                <tr>
                    <th>
                        Dinner Break
                    </th>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selSaturdayDinnerBreakFrom">
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
                            <select id="selSaturdayDinnerBreakFromDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                        
                    </td>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selSaturdayDinnerBreakTo">
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
                            <select id="selSaturdayLunchDinnerToDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <hr />
                    </td>
                </tr>

                @*sunday*@
                <tr>
                    <th>
                        Sunday
                    </th>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selSundayFrom">
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
                            <select id="selSundayFromDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                        
                    </td>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selSundayTo">
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
                            <select id="selSundayToDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                    </td>
                </tr>
                @*sunday lunch break*@
                <tr>
                    <th>
                        Lunch Break
                    </th>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selSundayLunchBreakFrom">
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
                            <select id="selSundayLunchBreakFromDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                        
                    </td>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selSundayLunchBreakTo">
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
                            <select id="selSundayLunchBreakToDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                    </td>
                </tr>
                 @*sunday dinner break*@
                <tr>
                    <th>
                        Dinner Break
                    </th>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selSundayDinnerBreakFrom">
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
                            <select id="selSundayDinnerBreakFromDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                        
                    </td>
                    <td>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selSundayDinnerBreakTo">
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
                            <select id="selSundayLunchDinnerToDay">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </fieldset>
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <hr />
                    </td>
                </tr>

                <tr>
                    <th>
                        
                    </th>
                    <td>
                        <div> <a id="btnClear" data-role="button" data-inline="true">Clear All Time</a> </div>           
                    </td>
                    <td>
                        
                        <div> <a href="#panApplyTime" id="btnApplyTimeToDays" data-rel="popup" data-position-to="window" data-role="button">Apply time schedule to other day(s)</a> </div>           
                    </td>
                </tr>
            </tbody>
        </table>        
        </div>

        <div id="panApplyTime" div data-role="popup">
            <ul data-role="listview" data-inset="true">
                <li data-role="list-divider">Work Schedule</li>
                <li>            
                    <br />     
                    <fieldset data-role="controlgroup" data-type="horizontal">
                        <legend><b>Select Day: </b></legend>
                        <select id='selSelectDay'>
                            <option value=""> -Select Day- </option>
                            <option value="optMonday"> Monday </option>
                            <option value="optTuesday"> Tuesday </option>
                            <option value="optWednesday"> Wednesday </option>
                            <option value="optThursday"> Thursday </option>
                            <option value="optFriday"> Friday </option>
                            <option value="optSaturday"> Saturday </option>
                            <option value="optSunday"> Sunday </option>
                        </select>
                    </fieldset>
                    
                    <label id="lblTimeRange"> </label>
                    <br />
                    <label id="lblLunchBreak"> </label>
                    <br />
                    <label id="lblDinnerBreak"> </label>
                    <br /> <br /> 
                    <fieldset data-role="controlgroup">
                        <legend><b>Apply to: </b></legend>
                        
                        <input type="checkbox" name="chkMonday" id="chkMonday" class="custom" />
	                    <label for="chkMonday">Monday</label>

                        <input type="checkbox" name="chkTuesday" id="chkTuesday" class="custom" />
	                    <label for="chkTuesday">Tuesday</label>

                        <input type="checkbox" name="chkWednesday" id="chkWednesday" class="custom" />
	                    <label for="chkWednesday">Wednesday</label>

                        <input type="checkbox" name="chkThursday" id="chkThursday" class="custom" />
	                    <label for="chkThursday">Thursday</label>

                        <input type="checkbox" name="chkFriday" id="chkFriday" class="custom" />
	                    <label for="chkFriday">Friday</label>

                        <input type="checkbox" name="chkSaturday" id="chkSaturday" class="custom" />
	                    <label for="chkSaturday">Saturday</label>

                        <input type="checkbox" name="chkSunday" id="chkSunday" class="custom" />
	                    <label for="chkSunday">Sunday</label>
                        
                    </fieldset>
                </li>
            </ul>           

            <br /> <br />     
            <div> <a id="btnApplyTime" data-role="button" data-inline="true">Apply time to selected day(s)</a> </div>
            <br />
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnSave" data-role="button"><h3>Save Schedule</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnAssignEmployees" data-role="button"><h3>Assign Employees to this schedule</h3></a>
            </div>
        </div>

    </div>

</div>




