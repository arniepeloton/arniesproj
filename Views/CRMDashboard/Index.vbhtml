@Code
    ViewData("Title") = "CRM Dashboard"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code
@Section head
    @*<link rel="stylesheet" href="/Content/flatui-css-custom.css" />
    <link rel="stylesheet" href="/Content/bootstrap-custom.css" />
    <link rel="stylesheet" href="/Content/jquery.dataTables2.css" />*@
    <script src="/Scripts/sources/CRMDashboard/customcalendar.js"></script>
    <script src="/Scripts/sources/crm/qrcodeNew.js"></script>
    
    <script>
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
                require([GetPathFormat('/Scripts/sources/CRMDashboard/db.js?crc=' + assetversion)]);
            })
        })        
    </script>

    <style type="text/css">
        .ItemsBreadCrumbs {
            padding: 10px 0px 7px 0px;
        }
    </style>

End Section
<div id="divDBLoading" style="display:block; padding:10px;">Loading, please wait ...</div>
@*<div id="custom_datatables_processing" style="padding: 10px; position: absolute; z-index: 1100; background-color: rgba(0,0,0, 0.5); width: 100%; top: 0; bottom: 0; text-align: center; color: #FFF; text-shadow: none;">
    <div style="transform: translate(-50%, -50%); left: 50%; top: 50%; position: fixed;">Loading, please wait ...</div>
</div>*@
<div id="divDBMainPage" style="display:none">
    <div id="divCRM" style="display:block;">
        <nav class="bg-primary text-white mb-3 sticky-top">
            <div class="container-fluid">
                <table style="width:100%;">
                    <tr>
                        <td><div><h5 class="m-0 OrganizationNameHeader"></h5></div></td>
                        <td style="width:50px;">
                            <div class="dropdown">
                                <button type="button" data-role="none" class="btn btn-primary btn-lg btn-block text-white dropdown-toggle dropdown-toggle-split" data-toggle="dropdown" aria-expanded="false">
                                    Hi <span id="txtGreetName"></span>
                                </button>
                                <ul class="dropdown-menu dropdown-menu-right">
                                    <li><a class="dropdown-item" id="popPeopleMenuPeopleInfoSummary" href="#"><span class="bi-person-fill"></span> View Profile</a></li>
                                    <li><a class="dropdown-item" id="btnSignOut" href="#"><span class="bi-box-arrow-left"></span> Logout</a></li>
                                </ul>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>

            <div class="row" style="margin:0px; width:100%; color:#FFF; background-color:#1969de; font-weight:bold;">
                <div class="BreadCrumbs">
                    <div class="ItemsBreadCrumbs"></div>
                </div>
            </div>
        </nav>
    </div>   

    <div id="mainContainer">
        <div style="width:100%">
            <div id="divPanelMenu" style="min-width:200px; width:200px; float:none; top: 0; bottom: 0; height: 100%; padding:0px 15px 0px 15px; vertical-align:top; z-index: 1001; background-color: #F5F5F5; box-shadow: 1px 0px 8px #313131; overflow:auto !important; position:fixed; display: none;">
                <br />
                <img class="btnPanel" src="/Content/images/close-black.png" style="cursor:pointer;" />
                <br /><br />
                <ul id="listMenuPIM" class="custom-list-menu txt-left" panel-menu="true">
                    @*<li><a id="btnListDashboard" data-gotopage="pageDashboard" class="active-menu ui-link">Dashboard</a></li>*@
                    <li>
                        <a data-gotopage="pageLeadGen" class="active-menu ui-link">Marketing</a>
                        <ol>
                            <li id="btnListLeadGen">Lead Generation</li>
                        </ol>
                    </li>
                    @*<li>
                        <a class="ui-link">Sales</a>
                        <ol>
                            <li id="btnListSalesGrinder">Sales Grinder</li>
                        </ol>
                    </li>
                    <li>
                        <a id="btnListCac" data-gotopage="pageCac" class="ui-link">Credit And Collection</a>
                        <ol>
                                <li id="btnListCacBilling">Billing Statement</li>
                                <li id="btnListCacPayments">Customer Payments</li>
                            </ol>
                    </li>
                    <li><a id="btnDBSettings" class="ui-link">Settings</a></li>*@
                    <li>
                        <a data-gotopage="pageActivityReport" class="ui-link">Report</a>
                        <ol>
                            <li id="btnListActivityReport">Activity Report</li>
                        </ol>
                    </li>
                </ul>
            </div>

            <div class="container-fluid">
                @*<div custom-data-role="page" id="pageDashboard" style="display:block;">
                        <div subpage="pageDashboard" id="divMainDB">
                            <div class="ui-grid-b ui-responsive">
                                <div class="ui-block-a" style="padding:0px 5px 5px 5px">
                                    <div class="cards">
                                        <div class="cardsheader theme-orange">
                                            <div subpage="pageopentodo" class="divopentodolist">
                                                <div style="display:table; width:100%;">
                                                    <div style="display:table-cell; vertical-align: middle;">
                                                        Open To-Do Items
                                                    </div>
                                                    <div id="btnOpenToDoSearch" style="display:table-cell; width:25px;">
                                                        <img alt="search" src="/Content/images/search-white.png" style="width:18px; float:right;" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div subpage="pageopentodo" class="divopentodosearch" style="display:none;">
                                                <div style="display:table; width:100%;">
                                                    <div style="display:table-cell; vertical-align: middle;"><input data-role="none" type="text" id="txtSearchOpenToDo" style="width:100%;" /></div>
                                                    <div id="btnBackOpenToDoSearch" style="display:table-cell; width:25px;"><span class="contactclosebtn">×</span></div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="cardsbody theme-orange">
                                            <div subpage="pageopentodo" class="divopentodolist">
                                                <ul id="lstOpenToDo" data-role="none" class="custom-list list-badge"></ul>
                                                <button data-role="none" id="btnShowMoreOpenToDo">Show More</button>
                                            </div>
                                            <div subpage="pageopentodo" class="divopentodosearch" style="display:none;">
                                                <ul id="lstOpenToDoSearch" data-role="none" class="custom-list list-badge"></ul>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="ui-block-b" style="padding:0px 5px 5px 5px">
                                    <div class="cards">
                                        <div class="cardsheader theme-blue">
                                            <div subpage="pagescheduledtask" class="divscheduledtasklist">
                                                <div style="display:table; width:100%;">
                                                    <div style="display:table-cell; vertical-align: middle;">
                                                        Scheduled Tasks
                                                    </div>
                                                    <div id="btnScheduledSearch" style="display:table-cell; width:25px;">
                                                        <img alt="search" src="/Content/images/search-white.png" style="width:18px; float:right;" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div subpage="pagescheduledtask" class="divscheduledtasksearch" style="display:none;">
                                                <div style="display:table; width:100%;">
                                                    <div style="display:table-cell; vertical-align: middle;"><input data-role="none" type="text" id="txtSearchScheduledTask" style="width:100%;" /></div>
                                                    <div id="btnBackScheduleSearch" style="display:table-cell; width:25px;"><span class="contactclosebtn">×</span></div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="cardsbody theme-blue">
                                            <div subpage="pagescheduledtask" class="divscheduledtasklist">
                                                <ul id="lstSchedule" data-role="none" class="custom-list list-badge"></ul>
                                                <button data-role="none" id="btnShowMoreSched">Show More</button>
                                            </div>
                                            <div subpage="pagescheduledtask" class="divscheduledtasksearch" style="display:none;">
                                                <ul id="lstScheduleSearch" data-role="none" class="custom-list list-badge"></ul>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="ui-block-c" style="padding:0px 5px 5px 5px">
                                    <div class="cards">
                                        <div class="cardsheader theme-green">
                                            <div style="display:table; width:100%;">
                                                <div style="display:table-cell; vertical-align: middle;">
                                                    Special Events
                                                </div>
                                                <div style="display:table-cell; width:25px;">
                                                    <img alt="search" src="/Content/images/search-white.png" style="width:18px; float:right;" />
                                                </div>
                                            </div>
                                        </div>
                                        <div class="cardsbody theme-green">
                                            <ul data-role="none" class="custom-list list-badge">
                                                No Special Events
                                            </ul>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div subpage="pageDashboard" id="divDashboardOpenToDo" style="display:none;">
                            <div class="">
                                <div class="cardsheader theme-orange">
                                    To-Do Items of <span id="txtOpenToDoName"></span>
                                </div>
                                <div class="cardsbody theme-orange">
                                    <div>
                                        <ul id="listDashboardToDo" data-role="none" style="list-style-type: none; margin:0 auto; padding:0;"></ul>
                                    </div>
                                </div>
                            </div>
                            <div class="floatingDiv-bottomcenter">
                                <div>
                                    <button data-role="none" id="btnBackDBOpenToDo" data-title="Back" class="btn custom-btn-theme-c btnCircle btnShadow">
                                        <img alt="Cancel" src="/Content/images/arrow-left.png" style="width:18px;" />
                                    </button>
                                </div>
                            </div>
                        </div>

                        <div subpage="pageDashboard" id="divDashboardSched" style="display:none;">
                            Schedule List
                        </div>
                    </div>*@

                <div subpage="maindb" id="pageLeadGen" style="display:block;">
                    Loading, please wait ...
                </div>

                <div subpage="maindb" id="pageCac" style="display:none;">
                    Loading, please wait ...
                    @*<div id="pageCacBilling" subpage="pageCac" style="display:none;">
                Loading, please wait ...
            </div>

            <div id="pageCacPayment" subpage="pageCac" style="display:none;">
                Loading, please wait ...
            </div>*@
                </div>

                <div subpage="maindb" id="pageSalesGrinder" style="display:none;">
                    <button data-role="none" id="btnRequestData">Request Data</button>
                    <form id="formcustomerupload" enctype="multipart/form-data" method="post" style="display:block;">
                        <input type="text" id="txtUploadSalesGrinderOrgID" name="OrgID" data-role="none" style="display:none;">
                        <input type="text" id="txtUploadSalesGrinderDateTime" name="DateTime" data-role="none" style="display:none;">
                        <input type="file" id="objFileCustomer" name="objFile">
                        <input type="submit" id="btnUploadCustomerExcel" value="Upload File">
                    </form>
                    Loading, please wait ...
                </div>

                <div subpage="maindb" id="pageActivityReport" style="display:none;">
                    Loading, please wait ...
                </div>
            </div>
        </div>
    </div>
</div>