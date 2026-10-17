<script>
    require([GetPathFormat('/Scripts/sources/CRMDashboard/ActivityQuota.js?crc=' + assetversion)]);
</script>

<div id="divLoadingActivityQuota" subpage="pageActivityQuota" style="display:block;"><div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div></div></div>

<div id="divMainActivityQuotaPage" subpage="pageActivityQuota" style="display:none;">
    <div class="mb-3 row align-items-center">
        <label class="col-md-2" for="selActivityTeamName">Team:</label>
        <div class="col-md">
            <select id="selActivityTeamName" data-role="none" class="form-select">
                <option value="">Select Team</option>
            </select>
        </div>
    </div>

    @*<div class="mb-3 row align-items-center">
        <label class="col-md-2" for="selActivityTerritory">Territory:</label>
        <div class="col-md">
            <select id="selActivityTerritory" data-role="none" class="form-select">
                <option value="">Select Territory</option>
                <option value="North">North</option>
                <option value="South">South</option>
            </select>
        </div>
    </div>

    <div class="mb-3 row align-items-center">
        <label class="col-md-2" for="selActivityTeamUser">User:</label>
        <div class="col-md">
            <select id="selActivityTeamUser" data-role="none" class="form-select">
                <option value="">Select User</option>
            </select>
        </div>
    </div>*@

    <div class="mb-3 row align-items-center">
        <label class="col-md-2" for="butActivityDateRange">Date Period:</label>
        <div class="col-md">
            <button id="butActivityDateRange" data-role="none" class="btn btn-secondary btn-block">Set Date</button>
        </div>
    </div>

    @*<div class="mb-3 row align-items-center">
        <label class="col-md-2" for="butActivityGenerate"></label>
        <div class="col-md">
            <button id="butActivityGenerate" data-role="none" class="btn btn-success btn-block">Generate</button>
        </div>
    </div>*@

    <div class="table-responsive">
        <table data-role="none" class="table table-light table-striped table-hover">
            <thead>
                <tr class="text-center align-top">
                    <th> Target Daily </th>
                    <th> Target </th>
                </tr>
            </thead>
            <tbody id="tbodyActivityQuota">
                <tr>
                    <td>Calls</td>
                    <td><input data-role="none" type="number" class="form-control" id="txtActivityQuotaCallsMade" onclick="this.select()" /></td>
                </tr>
                <tr>
                    <td>Notes</td>
                    <td><input data-role="none" type="number" class="form-control" id="txtActivityQuotaNotesTaken" onclick="this.select()" /></td>
                </tr>
                <tr>
                    <td>Physical Meetings</td>
                    <td><input data-role="none" type="number" class="form-control" id="txtActivityQuotaPhysicalMeetingAttended" onclick="this.select()" /></td>
                </tr>
                <tr>
                    <td>Virtual Meetings</td>
                    <td><input data-role="none" type="number" class="form-control" id="txtActivityQuotaVirtualMeetingAttended" onclick="this.select()" /></td>
                </tr>
                <tr>
                    <td>Sent Emails</td>
                    <td><input data-role="none" type="number" class="form-control" id="txtActivityQuotaEmailsSent" onclick="this.select()" /></td>
                </tr>
                <tr>
                    <td>Sent SMS</td>
                    <td><input data-role="none" type="number" class="form-control" id="txtActivityQuotaSMSSent" onclick="this.select()" /></td>
                </tr>
                <tr>
                    <td>Scheduled Calls</td>
                    <td><input data-role="none" type="number" class="form-control" id="txtActivityQuotaScheduledCalls" onclick="this.select()" /></td>
                </tr>
                <tr>
                    <td>Scheduled Physical Meetings</td>
                    <td><input data-role="none" type="number" class="form-control" id="txtActivityQuotaScheduledPhysicalMeeting" onclick="this.select()" /></td>
                </tr>
                <tr>
                    <td>Scheduled Virtual Meetings</td>
                    <td><input data-role="none" type="number" class="form-control" id="txtActivityQuotaScheduledVirtualMeeting" onclick="this.select()" /></td>
                </tr>
                <tr>
                    <td>Scheduled Emails</td>
                    <td><input data-role="none" type="number" class="form-control" id="txtActivityQuotaScheduledEmails" onclick="this.select()" /></td>
                </tr>
                <tr>
                    <td>Scheduled SMS</td>
                    <td><input data-role="none" type="number" class="form-control" id="txtActivityQuotaScheduledSMS" onclick="this.select()" /></td>
                </tr>
            </tbody>
        </table>
    </div>

    <div class="mt-5 mb-5 text-center">
        <button data-role="none" class="btn btn-secondary btn-lg" id="btnCancelActivityQuota">Cancel</button>
        <button data-role="none" class="btn btn-success btn-lg" id="btnSaveActivityQuota">Save</button>
    </div>
</div>