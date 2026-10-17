@Code
    ViewData("Title") = "SMS Usage"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
@Section head
    <link rel="stylesheet" href="/Content/flatui-css-custom.css" />
    <script src="/Scripts/sources/CRMDashboard/customcalendar.js"></script>
    <script src="/Scripts/sources/crm/qrcodeNew.js"></script>

    <script>
        require([GetPathFormat('/Scripts/sources/CRMDashboard/SmSUsage.js?crc=')
                ]);
    </script>

<div id="divMainPage">
    <div custom-data-role="page" id="pageMainDashboard">
        <div data-role="content">
            <div>
                <div style="float:left;">
                    
                    <select id="selFilterForm" data-role="none" class="custom-theme-c select">
                        <optgroup label="Filter By:">
                            <option value="a">All</option>
                            <option value="o">Telco</option>
                            <option value="p">People</option>
                            <option value="city">City</option>
                        </optgroup>
                    </select>
                    <select id="selCity" data-role="none" class="custom-theme-c select" style="display:none;"></select>
                </div>
                
                <div style="clear:both;"></div>
            </div>
            <br />

            <div id="divCountRow">SMS Sent</div>
            <br />

            <div style="overflow:auto;">
                <table data-role="table" data-role="reflow" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
                    <thead>
                        <tr class="custom-theme-f">
                            <th class="bulk" style="cursor:pointer; width:20px; display:none;">
                                <label class="checkbox-only" for="chbbulk">
                                    <input type="checkbox" name="chbbulk" id="chbbulk" data-iconpos="notext" />
                                </label>
                            </th>
                            
                            <th data-sort="string" style="cursor:pointer;">  </th>
                            <th data-sort="string" style="cursor:pointer;text-align:right"> Device To Modem </th>
                            <th data-sort="string" style="cursor:pointer;text-align:right"> Modem To Telco </th>
                            <th data-sort="string" style="cursor:pointer;text-align:right"> </th>
                            
                        </tr>
                    </thead>
                    <tbody id="tblSmSUsagebody"></tbody>
                </table>
            </div>

            <div style="overflow:auto">
                <table data-role="table" data-role="reflow" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
                    <thead>
                        <tr class="custom-theme-f">
                            <th class="bulk" style="cursor:pointer; width:20px; display:none;">
                                <label class="checkbox-only" for="chbbulk">
                                    <input type="checkbox" name="chbbulk" id="chbbulk" data-iconpos="notext" />
                                </label>
                            </th>

                            <th data-sort="string" style="cursor:pointer;"> Sent To </th>
                            <th data-sort="string" style="cursor:pointer;text-align:right"> Message </th>
                            <th data-sort="string" style="cursor:pointer;text-align:right"> Date and Time </th>
                            <th data-sort="string" style="cursor:pointer;text-align:right"> Sent By </th>                           

                        </tr>
                    </thead>
                    <tbody id="tblSmSUsageDetailbody"></tbody>
                </table>
            </div>
        </div>
    </div>
    </div>
End Section
