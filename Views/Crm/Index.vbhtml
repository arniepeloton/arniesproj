<script>
    require([GetPathFormat('/Scripts/sources/CRM/crm.js?v=' + assetversion)]);
</script>

<div id="pageDeviceMenu" class="container-fluid" style="display:none;">
    <div class="row mb-4">
        <div class="col-6">
            <h1 class="display-6">Team List</h1>
        </div>
        <div class="col-6 text-right">
            <a id="btnOrgAddTeam" data-role="none" class="btn btn-success">Add Team</a>
        </div>
    </div>
    
    <ul data-role="none" id="lstOrgTeams" class="list-group"></ul>

    <div class="mt-4" style="text-align: center">
        <a id="btnOrgTeamDone" data-role="none" class="btn btn-secondary btn-lg">Done</a>
    </div>
</div>

<div id="pageAddTeam" style="display:block;">
    <div>
        <h1 class="display-6 mb-4"><span class="lblCRMType"></span> Information</h1>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3" for="txtTeamName"><span class="lblCRMType"></span> Name: <span style="color:#F00">*</span></label>
            <div class="col-sm-10">
                <input type="text" data-role="none" id="txtTeamName" class="form-control" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3" for="txtTeamDescription">Description:</label>
            <div class="col-sm-10">
                <input type="text" data-role="none" id="txtTeamDescription" class="form-control" />
            </div>
        </div>
    </div>
    
    <div>
        <div class="row mb-4">
            <div class="col-6">
                <h1 class="display-6"><span class="lblCRMType"></span> Member(s)</h1>
            </div>
            <div class="col-6 text-right">
                <a id="btnOrgAddTeamMember" data-role="none" class="btn btn-success">Add Member</a>
            </div>
        </div>
        
        <div data-role="none" class="list-group mb-3" id="lstTeamMembers">
            <a class="list-group-item list-group-item-action">No result(s) found</a>
        </div>
    </div>

    <div id="divForTeam" style="display:block;">
        <div class="row mb-4">
            <div class="col-6">
                <h1 class="display-6">Team Customer(s)</h1>
            </div>
            <div class="col-6 text-right">
                <a id="btnOrgAddCustomer" data-role="none" class="btn btn-success">Add Customer</a>
            </div>
        </div>

        <div data-role="none" class="list-group mb-3" id="lstOrgCustomer">
            <a class="list-group-item list-group-item-action">No result(s) found</a>
        </div>

        <h1 class="display-6 mb-4">Team Quota</h1>

        <table data-role="none" class="table table-striped table-hover" id="tblGrossSalesLabels">
            <thead>
                <tr>
                    <th> Month </th>
                    <th> Quota </th>
                    <th> Incentive (%) </th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td> January <//td>
                    <td> <input id="txtOrgTeamQuotaTier1Jan" value="" class="form-control" data-role="none" />  </td>
                    <td> <input id="txtOrgTeamIncentiveTier1Jan" value="" class="form-control" data-role="none" /> </td>
                </tr>
                <tr>
                    <td> February <//td>
                    <td> <input id="txtOrgTeamQuotaTier1Feb" value="" class="form-control" data-role="none" />  </td>
                    <td> <input id="txtOrgTeamIncentiveTier1Feb" value="" class="form-control" data-role="none" /> </td>
                </tr>
                <tr>
                    <td> March <//td>
                    <td> <input id="txtOrgTeamQuotaTier1Mar" value="" class="form-control" data-role="none" />  </td>
                    <td> <input id="txtOrgTeamIncentiveTier1Mar" value="" class="form-control" data-role="none" /> </td>
                </tr>
                <tr>
                    <td> April <//td>
                    <td> <input id="txtOrgTeamQuotaTier1Apr" value="" class="form-control" data-role="none" />  </td>
                    <td> <input id="txtOrgTeamIncentiveTier1Apr" value="" class="form-control" data-role="none" /> </td>
                </tr>
                <tr>
                    <td> May <//td>
                    <td> <input id="txtOrgTeamQuotaTier1May" value="" class="form-control" data-role="none" />  </td>
                    <td> <input id="txtOrgTeamIncentiveTier1May" value="" class="form-control" data-role="none" /> </td>
                </tr>
                <tr>
                    <td> June <//td>
                    <td> <input id="txtOrgTeamQuotaTier1Jun" value="" class="form-control" data-role="none" />  </td>
                    <td> <input id="txtOrgTeamIncentiveTier1Jun" value="" class="form-control" data-role="none" /> </td>
                </tr>
                <tr>
                    <td> July <//td>
                    <td> <input id="txtOrgTeamQuotaTier1Jul" value="" class="form-control" data-role="none" />  </td>
                    <td> <input id="txtOrgTeamIncentiveTier1Jul" value="" class="form-control" data-role="none" /> </td>
                </tr>
                <tr>
                    <td> August <//td>
                    <td> <input id="txtOrgTeamQuotaTier1Aug" value="" class="form-control" data-role="none" />  </td>
                    <td> <input id="txtOrgTeamIncentiveTier1Aug" value="" class="form-control" data-role="none" /> </td>
                </tr>
                <tr>
                    <td> September <//td>
                    <td> <input id="txtOrgTeamQuotaTier1Sept" value="" class="form-control" data-role="none" />  </td>
                    <td> <input id="txtOrgTeamIncentiveTier1Sept" value="" class="form-control" data-role="none" /> </td>
                </tr>
                <tr>
                    <td> October <//td>
                    <td> <input id="txtOrgTeamQuotaTier1Oct" value="" class="form-control" data-role="none" />  </td>
                    <td> <input id="txtOrgTeamIncentiveTier1Oct" value="" class="form-control" data-role="none" /> </td>
                </tr>
                <tr>
                    <td> November <//td>
                    <td> <input id="txtOrgTeamQuotaTier1Nov" value="" class="form-control" data-role="none" />  </td>
                    <td> <input id="txtOrgTeamIncentiveTier1Nov" value="" class="form-control" data-role="none" /> </td>
                </tr>
                <tr>
                    <td> December <//td>
                    <td> <input id="txtOrgTeamQuotaTier1Dec" value="" class="form-control" data-role="none" />  </td>
                    <td> <input id="txtOrgTeamIncentiveTier1Dec" value="" class="form-control" data-role="none" /> </td>
                </tr>
            </tbody>
        </table>
    </div>

    <div class="mt-4" style="text-align: center">
        <a id="btnOrgTeamCancel" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
        <a id="btnOrgTeamSave" data-role="none" class="btn btn-success btn-lg">Save</a>
    </div>
</div>