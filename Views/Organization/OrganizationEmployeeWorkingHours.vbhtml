<style>
    .ui-field-contain  label.ui-submit  {
        width: 100%;
    }
    .ui-field-contain  .ui-controlgroup-controls  {
        width: 100%;
    }
</style>

<script>
    require([GetPathFormat('/Scripts/sources/Organization/OrganizationEmployeeWorkingHours.js?crc=' + assetversion)]);
</script>

<div id="pageOrganizationEmployeeSchedule" style="display:block;">
    <div class="row mb-4">
        <div class="col-6">
            <h1 class="display-6">Work Schedule</h1>
        </div>
        <div class="col-6 text-right">
            <button id="btnNewEmployeeSchedule" data-role="none" class="btn btn-success">Create New Employee Schedule</button>
        </div>
    </div>

    <div id="lstWorkSchedule" data-role="none" class="list-group mb-3"></div>

    <div class="text-center mt-5">
        <button id="btnDone" data-role="none" class="btn btn-secondary btn-lg">Done</button>
    </div>
</div>

<div id="pageNewSchedule" style="display:none;">
    <h1 class="display-6 mb-4">Work Schedule</h1>

    <div data-role="content">
        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtName">Work Schedule Name: <span style="color:#F00">*</span></label>
            <div class="col-sm-10">
                <input type="text" id="txtName" data-role="none" class="form-control" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtDescription">Description:</label>
            <div class="col-sm-10">
                <input type="text" id="txtDescription" data-role="none" class="form-control" />
            </div>
        </div>

        <h5 class="mb-3">Effectivity Date To:</h5>

        <div class="text-right mb-3">
            <button id="btnClear" data-role="none" class="btn btn-secondary">Clear All Time</button>
            <button id="btnApplyTimeToDays" data-role="none" data-toggle="modal" data-target="#panApplyTime" class="btn btn-success">Apply time schedule to other day(s)</button>
        </div>

        <table style="width:100%; table-layout:fixed;" data-role="none" class="table table-striped table-hover">
            <thead>
                <tr class="text-center">
                    <th>
                        Day
                    </th>
                    <th>
                        From
                    </th>
                    <th>
                        To
                    </th>
                </tr>
            </thead>

            <tbody>
                @*monday*@
                <tr>
                    <th>
                        Monday
                    </th>
                    <td>
                        <div class="input-group mb-3">
                            <select id="selMondayFrom" class="form-select" data-role="none">
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
                            <select id="selMondayFromDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
                    </td>
                    <td>
                        <div class="input-group mb-3">
                            <select id="selMondayTo" class="form-select" data-role="none">
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
                            <select id="selMondayToDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
                    </td>
                </tr>
                @*monday lunch break*@
                <tr>
                    <th>
                        Lunch Break
                    </th>
                    <td>
                        <div class="input-group mb-3">
                            <select id="selMondayLunchBreakFrom" class="form-select" data-role="none">
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
                            <select id="selMondayLunchBreakFromDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
                    </td>
                    <td>
                        <div class="input-group mb-3">
                            <select id="selMondayLunchBreakTo" class="form-select" data-role="none">
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
                            <select id="selMondayLunchBreakToDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
                    </td>
                </tr>
                @*monday dinner break*@
                <tr>
                    <th>
                        Dinner Break
                    </th>
                    <td>
                        <div class="input-group mb-3">
                            <select id="selMondayDinnerBreakFrom" class="form-select" data-role="none">
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
                            <select id="selMondayDinnerBreakFromDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
                    </td>
                    <td>
                        <div class="input-group mb-3">
                            <select id="selMondayDinnerBreakTo" class="form-select" data-role="none">
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
                            <select id="selMondayLunchDinnerToDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
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
                        <div class="input-group mb-3">
                            <select id="selTuesdayFrom" class="form-select" data-role="none">
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
                            <select id="selTuesdayFromDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
                    </td>
                    <td>
                        <div class="input-group mb-3">
                            <select id="selTuesdayTo" class="form-select" data-role="none">
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
                            <select id="selTuesdayToDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
                    </td>
                </tr>
                @*tuesday lunch break*@
                <tr>
                    <th>
                        Lunch Break
                    </th>
                    <td>
                        <div class="input-group mb-3">
                            <select id="selTuesdayLunchBreakFrom" class="form-select" data-role="none">
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
                            <select id="selTuesdayLunchBreakFromDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
                    </td>
                    <td>
                        <div class="input-group mb-3">
                            <select id="selTuesdayLunchBreakTo" class="form-select" data-role="none">
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
                            <select id="selTuesdayLunchBreakToDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
                    </td>
                </tr>
                @*tuesday dinner break*@
                <tr>
                    <th>
                        Dinner Break
                    </th>
                    <td>
                        <div class="input-group mb-3">
                            <select id="selTuesdayDinnerBreakFrom" class="form-select" data-role="none">
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
                            <select id="selTuesdayDinnerBreakFromDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
                    </td>
                    <td>
                        <div class="input-group mb-3">
                            <select id="selTuesdayDinnerBreakTo" class="form-select" data-role="none">
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
                            <select id="selTuesdayLunchDinnerToDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
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
                        <div class="input-group mb-3">
                            <select id="selWednesdayFrom" class="form-select" data-role="none">
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
                            <select id="selWednesdayFromDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
                    </td>
                    <td>
                        <div class="input-group mb-3">
                            <select id="selWednesdayTo" class="form-select" data-role="none">
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
                            <select id="selWednesdayToDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
                    </td>
                </tr>
                @*wednesday lunch break*@
                <tr>
                    <th>
                        Lunch Break
                    </th>
                    <td>
                        <div class="input-group mb-3">
                            <select id="selWednesdayLunchBreakFrom" class="form-select" data-role="none">
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
                            <select id="selWednesdayLunchBreakFromDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
                    </td>
                    <td>
                        <div class="input-group mb-3">
                            <select id="selWednesdayLunchBreakTo" class="form-select" data-role="none">
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
                            <select id="selWednesdayLunchBreakToDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
                    </td>
                </tr>
                @*wednesday dinner break*@
                <tr>
                    <th>
                        Dinner Break
                    </th>
                    <td>
                        <div class="input-group mb-3">
                            <select id="selWednesdayDinnerBreakFrom" class="form-select" data-role="none">
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
                            <select id="selWednesdayDinnerBreakFromDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
                    </td>
                    <td>
                        <div class="input-group mb-3">
                            <select id="selWednesdayDinnerBreakTo" class="form-select" data-role="none">
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
                            <select id="selWednesdayLunchDinnerToDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
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
                        <div class="input-group mb-3">
                            <select id="selThursdayFrom" class="form-select" data-role="none">
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
                            <select id="selThursdayFromDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
                    </td>
                    <td>
                        <div class="input-group mb-3">
                            <select id="selThursdayTo" class="form-select" data-role="none">
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
                            <select id="selThursdayToDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
                    </td>
                </tr>
                @*thursday lunch break*@
                <tr>
                    <th>
                        Lunch Break
                    </th>
                    <td>
                        <div class="input-group mb-3">
                            <select id="selThursdayLunchBreakFrom" class="form-select" data-role="none">
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
                            <select id="selThursdayLunchBreakFromDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
                    </td>
                    <td>
                        <div class="input-group mb-3">
                            <select id="selThursdayLunchBreakTo" class="form-select" data-role="none">
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
                            <select id="selThursdayLunchBreakToDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
                    </td>
                </tr>
                @*thursday dinner break*@
                <tr>
                    <th>
                        Dinner Break
                    </th>
                    <td>
                        <div class="input-group mb-3">
                            <select id="selThursdayDinnerBreakFrom" class="form-select" data-role="none">
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
                            <select id="selThursdayDinnerBreakFromDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
                    </td>
                    <td>
                        <div class="input-group mb-3">
                            <select id="selThursdayDinnerBreakTo" class="form-select" data-role="none">
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
                            <select id="selThursdayLunchDinnerToDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
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
                        <div class="input-group mb-3">
                            <select id="selFridayFrom" class="form-select" data-role="none">
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
                            <select id="selFridayFromDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
                    </td>
                    <td>
                        <div class="input-group mb-3">
                            <select id="selFridayTo" class="form-select" data-role="none">
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
                            <select id="selFridayToDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
                    </td>
                </tr>
                @*friday lunch break*@
                <tr>
                    <th>
                        Lunch Break
                    </th>
                    <td>
                        <div class="input-group mb-3">
                            <select id="selFridayLunchBreakFrom" class="form-select" data-role="none">
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
                            <select id="selFridayLunchBreakFromDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
                    </td>
                    <td>
                        <div class="input-group mb-3">
                            <select id="selFridayLunchBreakTo" class="form-select" data-role="none">
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
                            <select id="selFridayLunchBreakToDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
                    </td>
                </tr>
                @*friday dinner break*@
                <tr>
                    <th>
                        Dinner Break
                    </th>
                    <td>
                        <div class="input-group mb-3">
                            <select id="selFridayDinnerBreakFrom" class="form-select" data-role="none">
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
                            <select id="selFridayDinnerBreakFromDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
                    </td>
                    <td>
                        <div class="input-group mb-3">
                            <select id="selFridayDinnerBreakTo" class="form-select" data-role="none">
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
                            <select id="selFridayLunchDinnerToDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
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
                        <div class="input-group mb-3">
                            <select id="selSaturdayFrom" class="form-select" data-role="none">
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
                            <select id="selSaturdayFromDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
                    </td>
                    <td>
                        <div class="input-group mb-3">
                            <select id="selSaturdayTo" class="form-select" data-role="none">
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
                            <select id="selSaturdayToDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
                    </td>
                </tr>
                @*saturday lunch break*@
                <tr>
                    <th>
                        Lunch Break
                    </th>
                    <td>
                        <div class="input-group mb-3">
                            <select id="selSaturdayLunchBreakFrom" class="form-select" data-role="none">
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
                            <select id="selSaturdayLunchBreakFromDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
                    </td>
                    <td>
                        <div class="input-group mb-3">
                            <select id="selSaturdayLunchBreakTo" class="form-select" data-role="none">
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
                            <select id="selSaturdayLunchBreakToDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
                    </td>
                </tr>
                @*saturday dinner break*@
                <tr>
                    <th>
                        Dinner Break
                    </th>
                    <td>
                        <div class="input-group mb-3">
                            <select id="selSaturdayDinnerBreakFrom" class="form-select" data-role="none">
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
                            <select id="selSaturdayDinnerBreakFromDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
                    </td>
                    <td>
                        <div class="input-group mb-3">
                            <select id="selSaturdayDinnerBreakTo" class="form-select" data-role="none">
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
                            <select id="selSaturdayLunchDinnerToDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
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
                        <div class="input-group mb-3">
                            <select id="selSundayFrom" class="form-select" data-role="none">
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
                            <select id="selSundayFromDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
                    </td>
                    <td>
                        <div class="input-group mb-3">
                            <select id="selSundayTo" class="form-select" data-role="none">
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
                            <select id="selSundayToDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
                    </td>
                </tr>
                @*sunday lunch break*@
                <tr>
                    <th>
                        Lunch Break
                    </th>
                    <td>
                        <div class="input-group mb-3">
                            <select id="selSundayLunchBreakFrom" class="form-select" data-role="none">
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
                            <select id="selSundayLunchBreakFromDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
                    </td>
                    <td>
                        <div class="input-group mb-3">
                            <select id="selSundayLunchBreakTo" class="form-select" data-role="none">
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
                            <select id="selSundayLunchBreakToDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
                    </td>
                </tr>
                @*sunday dinner break*@
                <tr>
                    <th>
                        Dinner Break
                    </th>
                    <td>
                        <div class="input-group mb-3">
                            <select id="selSundayDinnerBreakFrom" class="form-select" data-role="none">
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
                            <select id="selSundayDinnerBreakFromDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
                    </td>
                    <td>
                        <div class="input-group mb-3">
                            <select id="selSundayDinnerBreakTo" class="form-select" data-role="none">
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
                            <select id="selSundayLunchDinnerToDay" class="form-select" data-role="none">
                                <option value=""> -- </option>
                                <option value="AM"> AM </option>
                                <option value="PM"> PM </option>
                            </select>
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>

        <div class="mt-5 text-center">
            <button id="btnWorkSchedCancel" data-role="none" class="btn btn-secondary btn-lg">Cancel</button>
            <button id="btnSave" data-role="none" class="btn btn-success btn-lg">Save Schedule</button>
            <button id="btnAssignEmployees" data-role="none" class="btn btn-warning btn-lg">Assign Employees to this schedule</button>
        </div>
    </div>
</div>

<div id="panApplyTime" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title"></h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div class="mb-3">
                    <label class="form-label" for="selSelectDay">Select Day:</label>
                    <select id="selSelectDay" class="form-select" data-role="none">
                        <option value=""> -Select Day- </option>
                        <option value="optMonday"> Monday </option>
                        <option value="optTuesday"> Tuesday </option>
                        <option value="optWednesday"> Wednesday </option>
                        <option value="optThursday"> Thursday </option>
                        <option value="optFriday"> Friday </option>
                        <option value="optSaturday"> Saturday </option>
                        <option value="optSunday"> Sunday </option>
                    </select>
                </div>

                <div id="lblTimeRange" class="mb-3"> </div>
                <div id="lblLunchBreak" class="mb-3"> </div>
                <div id="lblDinnerBreak" class="mb-3"> </div>

                <div class="mb-3">
                    <label class="form-label" for="selSelectDay">Apply to:</label>
                    <div class="form-check">
                        <input type="checkbox" data-role="none" name="chkMonday" id="chkMonday" class="form-check-input" />
                        <label class="form-check-label" for="chkMonday">Monday</label>
                    </div>
                    <div class="form-check">
                        <input type="checkbox" data-role="none" name="chkTuesday" id="chkTuesday" class="form-check-input" />
                        <label class="form-check-label" for="chkTuesday">Tuesday</label>
                    </div>
                    <div class="form-check">
                        <input type="checkbox" data-role="none" name="chkWednesday" id="chkWednesday" class="form-check-input" />
                        <label class="form-check-label" for="chkWednesday">Wednesday</label>
                    </div>
                    <div class="form-check">
                        <input type="checkbox" data-role="none" name="chkThursday" id="chkThursday" class="form-check-input" />
                        <label class="form-check-label" for="chkThursday">Thursday</label>
                    </div>
                    <div class="form-check">
                        <input type="checkbox" data-role="none" name="chkFriday" id="chkFriday" class="form-check-input" />
                        <label class="form-check-label" for="chkFriday">Friday</label>
                    </div>
                    <div class="form-check">
                        <input type="checkbox" data-role="none" name="chkSaturday" id="chkSaturday" class="form-check-input" />
                        <label class="form-check-label" for="chkSaturday">Saturday</label>
                    </div>
                    <div class="form-check">
                        <input type="checkbox" data-role="none" name="chkSunday" id="chkSunday" class="form-check-input" />
                        <label class="form-check-label" for="chkSunday">Sunday</label>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <button data-role="none" type="button" id="btnApplyTime" class="btn btn-success btn-block">Apply time to selected day(s)</button>
                <button data-role="none" type="button" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
            </div>
        </div>
    </div>
</div>