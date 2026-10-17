<script>
    require([GetPathFormat('/Scripts/sources/CRM/crm.js?crc=' + assetversion)]);
</script>

<div id="pageDeviceMenu" style="display:none;">
    @*<div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>*@

    <div data-role="content">
        <div>
            <ul data-role="listview" id="lstOrgEmployeeContainer" data-inset="true">   
				<li data-role="list-divider">Team List</li>
                <li>
                    <div data-role="fieldcontain"><a id="btnOrgAddTeam" data-role="button" data-icon="plus" data-inline="true">Add Team</a></div>

                    @*<form id="frmOrgEmployeeSearch">
                        <div><input id="txtOrgTeamSearch" type="search" value="" /></div>
                    </form>
*@
                    <div data-role="fieldcontain" id="divOrgRecordCountTeam"></div>
                    <div>
                        <ul data-role="listview" id="lstOrgTeams" data-inset="true">   
                        </ul>
                    </div>
                </li>
            </ul>
        </div>
        
        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnOrgTeamDone" data-role="button">Done</a> </div>
        </div>

    </div>

</div>

<div id="pageAddTeam" style="display:block;">
    @*<div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>*@

    <div data-role="content">
        <div class="mb-3 row align-items-center">
            <label class="col-md-2" for="txtTeamName">Team Name: <span style="color:#ff0000">*</span></label>
            <div class="col-md">
                <input data-role="none" type="text" class="form-control" id="txtTeamName" />
            </div>
        </div>

        <div class="mb-3 row align-items-center">
            <label class="col-md-2" for="txtTeamDescription">Description:</label>
            <div class="col-md">
                <input data-role="none" type="text" class="form-control" id="txtTeamDescription" />
            </div>
        </div>

        <div class="mb-3 row align-items-center">
            <div class="col">
                <h4>Team Member(s)</h4>
            </div>
            <div class="col text-right">
                <button data-role="none" class="btn btn-success" id="btnOrgAddTeamMember">Add Team</button>
            </div>
        </div>

        <ul id="lstTeamMembers" data-role="none" class="list-group"></ul>

        @*<div>            
            <div>
                <ul data-role="listview" id="lstOrgCustomerContainer" data-inset="true">   
				    <li data-role="list-divider">Team Customer(s)</li>
                    <li>
                        <div data-role="fieldcontain"><a id="btnOrgAddCustomer" data-role="button" data-icon="plus" data-inline="true">Add Team Customer</a></div>

                        <div data-role="fieldcontain" id="divOrgRecordCountCustomer"></div>
                        <div>
                            <ul data-role="listview" id="lstOrgCustomer" data-inset="true">   
                            </ul>
                        </div>
                    </li>
                </ul>
            </div>
        </div>

        <div>            
            <div>
                <ul data-role="listview" id="lstOrgQuotaContainer" data-inset="true">   
				    <li data-role="list-divider">Team Quota</li>
                    <li>
                        <table data-role="table" data-mode="reflow" class="ui-responsive" style="width:100%; text-align:center" id="tblGrossSalesLabels">
                            <thead> 
                                <th> Month </th>                               
                                <th> Quota </th>
                                <th> Incentive (%) </th>
                            </thead>
                            <tbody>
                                <tr>
                                    <td> January <//td>                                    
                                    <td> <input id="txtOrgTeamQuotaTier1Jan" value="" />  </td>
                                    <td> <input id="txtOrgTeamIncentiveTier1Jan" value="" /> </td>                            
                                </tr>
                                <tr>
                                    <td> February <//td>                                    
                                    <td> <input id="txtOrgTeamQuotaTier1Feb" value="" />  </td>
                                    <td> <input id="txtOrgTeamIncentiveTier1Feb" value="" /> </td>                            
                                </tr>
                                <tr>
                                    <td> March <//td>                                    
                                    <td> <input id="txtOrgTeamQuotaTier1Mar" value="" />  </td>
                                    <td> <input id="txtOrgTeamIncentiveTier1Mar" value="" /> </td>                            
                                </tr>
                                <tr>
                                    <td> April <//td>                                    
                                    <td> <input id="txtOrgTeamQuotaTier1Apr" value="" />  </td>
                                    <td> <input id="txtOrgTeamIncentiveTier1Apr" value="" /> </td>                            
                                </tr>
                                <tr>
                                    <td> May <//td>                                    
                                    <td> <input id="txtOrgTeamQuotaTier1May" value="" />  </td>
                                    <td> <input id="txtOrgTeamIncentiveTier1May" value="" /> </td>                            
                                </tr>
                                <tr>
                                    <td> June <//td>                                    
                                    <td> <input id="txtOrgTeamQuotaTier1Jun" value="" />  </td>
                                    <td> <input id="txtOrgTeamIncentiveTier1Jun" value="" /> </td>                            
                                </tr>
                                <tr>
                                    <td> July <//td>                                    
                                    <td> <input id="txtOrgTeamQuotaTier1Jul" value="" />  </td>
                                    <td> <input id="txtOrgTeamIncentiveTier1Jul" value="" /> </td>                            
                                </tr>
                                <tr>
                                    <td> August <//td>                                    
                                    <td> <input id="txtOrgTeamQuotaTier1Aug" value="" />  </td>
                                    <td> <input id="txtOrgTeamIncentiveTier1Aug" value="" /> </td>                            
                                </tr>
                                <tr>
                                    <td> September <//td>                                    
                                    <td> <input id="txtOrgTeamQuotaTier1Sept" value="" />  </td>
                                    <td> <input id="txtOrgTeamIncentiveTier1Sept" value="" /> </td>                            
                                </tr>
                                <tr>
                                    <td> October <//td>                                    
                                    <td> <input id="txtOrgTeamQuotaTier1Oct" value="" />  </td>
                                    <td> <input id="txtOrgTeamIncentiveTier1Oct" value="" /> </td>                            
                                </tr>
                                <tr>
                                    <td> November <//td>                                    
                                    <td> <input id="txtOrgTeamQuotaTier1Nov" value="" />  </td>
                                    <td> <input id="txtOrgTeamIncentiveTier1Nov" value="" /> </td>                            
                                </tr>
                                <tr>
                                    <td> December <//td>                                    
                                    <td> <input id="txtOrgTeamQuotaTier1Dec" value="" />  </td>
                                    <td> <input id="txtOrgTeamIncentiveTier1Dec" value="" /> </td>                            
                                </tr>
                            </tbody>                    
                       </table>
                    </li>
                </ul>
            </div>
        </div>*@

         <div class="mt-5 mb-5 text-center">
             <button data-role="none" class="btn btn-secondary btn-lg" id="btnOrgTeamCancel">Cancel</button>
             <button data-role="none" class="btn btn-success btn-lg" id="btnOrgTeamSave">Save</button>
        </div>
    </div>
</div>




