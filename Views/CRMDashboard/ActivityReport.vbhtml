<script>
    require([GetPathFormat('/Scripts/sources/CRMDashboard/ActivityReport.js?crc=')]);
</script>

<style>
    .horizontal-bar {
        width:100%;
        height: 30px; 
        /*background: linear-gradient(90deg, #FFEB3B 80%, #f3f3f3 0%);*/
        background: #f3f3f3;
    }

    .horizontal-bar.important {
        background: linear-gradient(90deg, #FFEB3B 80%, #cfcfcf 0%) !important;
    }
</style>

<div id="divLoadingActivityReport" subpage="pageActivityReport" style="display:block;">Loading, please wait ...</div>

<div id="divMainActivityReportPage" subpage="pageActivityReport" style="display:none;">
    <div class="row mb-3 align-items-center">
        <div class="col-md-auto">
            <label for="selActivityTeamName" class="form-label">Team:</label>
            <select id="selActivityTeamName" data-role="none" class="form-select">
                <option value="">Select Team</option>
            </select>
        </div>
        @*<div class="col-md-auto">
            <label for="selActivityTerritory" class="form-label">Territory:</label>
            <select id="selActivityTerritory" data-role="none" class="form-select">
                <option value="">Select Territory</option>
                <option value="North">North</option>
                <option value="South">South</option>
            </select>
        </div>*@
        <div class="col-md-auto">
            <label for="selActivityTeamUser" class="form-label">User:</label>
            <select id="selActivityTeamUser" data-role="none" class="form-select">
                <option value="">Select User</option>
            </select>
        </div>
        <div class="col-md-auto">
            <label for="butActivityDateRange" class="form-label">Date Range:</label>
            <button id="butActivityDateRange" data-role="none" class="btn btn-secondary btn-block">Set Date</button>
        </div>
        <div class="col-md-auto">
            <div class="form-check">
                <label class="form-check-label">
                    <input type="checkbox" name="chbExclude" data-role="none" class="form-check-input" />
                    Do not include days without activity quota
                </label>
            </div>
        </div>
    </div>

    <div class="table-responsive">
        <table data-role="none" data-mode="reflow" style="display:table;" class="table table-light table-striped table-hover">
            <thead>
                <tr class="custom-theme-f">
                    <th> Target </th>
                    <th style="width: 150px;"> Actual </th>
                    <th style="width: 150px;"> Target </th>
                    <th style="width: 150px;"> Actual-Target % </th>
                    <th style="width: 150px;">  </th>
                </tr>
            </thead>
            <tbody id="tbodyActivityReport">
                <tr>
                    <td>Calls Made</td>
                    <td id="tdCallActual">-</td>
                    <td id="tdCallTarget">-</td>
                    <td id="tdCallActualTarget">NA</td>
                    <td>
                        <div id="tdCallBar" class="horizontal-bar"></div>
                    </td>
                </tr>
                <tr>
                    <td>Notes Taken</td>
                    <td id="tdNoteActual">-</td>
                    <td id="tdNoteTarget">-</td>
                    <td id="tdNoteActualTarget">NA</td>
                    <td>
                        <div id="tdNoteBar" class="horizontal-bar"></div>
                    </td>
                </tr>
                <tr>
                    <td>Physical Meetings Attended</td>
                    <td id="tdPhysicalAttendedActual">-</td>
                    <td id="tdPhysicalAttendedTarget">-</td>
                    <td id="tdPhysicalAttendedActualTarget">NA</td>
                    <td>
                        <div id="tdPhysicalAttendedBar" class="horizontal-bar"></div>
                    </td>
                </tr>
                <tr>
                    <td>Virtual Meetings Attended</td>
                    <td id="tdVirtualAttendedActual">-</td>
                    <td id="tdVirtualAttendedTarget">-</td>
                    <td id="tdVirtualAttendedActualTarget">NA</td>
                    <td>
                        <div id="tdVirtualAttendedBar" class="horizontal-bar"></div>
                    </td>
                </tr>
                <tr>
                    <td>Emails Sent</td>
                    <td id="tdEmailActual">-</td>
                    <td id="tdEmailTarget">-</td>
                    <td id="tdEmailActualTarget">NA</td>
                    <td>
                        <div id="tdEmailBar" class="horizontal-bar"></div>
                    </td>
                </tr>
                <tr>
                    <td>SMS Sent</td>
                    <td id="tdSmsActual">-</td>
                    <td id="tdSmsTarget">-</td>
                    <td id="tdSmsActualTarget">NA</td>
                    <td>
                        <div id="tdSmsBar" class="horizontal-bar"></div>
                    </td>
                </tr>
                <tr>
                    <td>Scheduled Calls</td>
                    <td id="tdSchedCallActual">-</td>
                    <td id="tdSchedCallTarget">-</td>
                    <td id="tdSchedCallActualTarget">NA</td>
                    <td>
                        <div id="tdSchedCallBar" class="horizontal-bar"></div>
                    </td>
                </tr>
                <tr>
                    <td>Scheduled Physical Meetings</td>
                    <td id="tdSchedPhysicalActual">-</td>
                    <td id="tdSchedPhysicalTarget">-</td>
                    <td id="tdSchedPhysicalActualTarget">NA</td>
                    <td>
                        <div id="tdSchedPhysicalBar" class="horizontal-bar"></div>
                    </td>
                </tr>
                <tr>
                    <td>Scheduled Virtual Meetings</td>
                    <td id="tdSchedVirtualActual">-</td>
                    <td id="tdSchedVirtualTarget">-</td>
                    <td id="tdSchedVirtualActualTarget">NA</td>
                    <td>
                        <div id="tdSchedVirtualBar" class="horizontal-bar"></div>
                    </td>
                </tr>
                <tr>
                    <td>Scheduled Emails</td>
                    <td id="tdSchedEmailActual">-</td>
                    <td id="tdSchedEmailTarget">-</td>
                    <td id="tdSchedEmailActualTarget">NA</td>
                    <td>
                        <div id="tdSchedEmailBar" class="horizontal-bar"></div>
                    </td>
                </tr>
                <tr>
                    <td>Scheduled SMS</td>
                    <td id="tdSchedSmsActual">-</td>
                    <td id="tdSchedSmsTarget">-</td>
                    <td id="tdSchedSmsActualTarget">NA</td>
                    <td>
                        <div id="tdSchedSmsBar" class="horizontal-bar"></div>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</div>