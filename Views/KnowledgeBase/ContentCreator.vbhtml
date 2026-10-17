<script>
    require([GetPathFormat('/Scripts/sources/KnowledgeBase/ContentCreator.js?v=' + assetversion)]);
</script>

<style type="text/css">
    .art_option{
        display:none;
    }

    .steppic {
        max-width:150px;
    }
</style>

<div data-role="page">
    <div id="divKB" style="display:block;">
        <div class="custom-theme-a position-fixed" id="divHeaderForProjects">

            <div id="HeaderDesktopVersion" class="container">
                <div class="ui-grid-a ui-responsive" style="height:53px;">
                    <div class="ui-block-a" style="overflow:hidden; display:flex;">
                        <div style="float:left; margin-top:5px; cursor:pointer;">
                            <div custom-panel="divPanelMenu" style="height:40px; width:40px; float:right; margin-left: 10px; margin-top: 5px; margin-right:-5px;">
                                <div style="border:2px solid #FFF; margin-top:5px; width:30px;"></div>
                                <div style="border:2px solid #FFF; margin-top:5px; width:30px;"></div>
                                <div style="border:2px solid #FFF; margin-top:5px; width:30px;"></div>
                            </div>
                        </div>
                        @*<div style="float:left; margin-left:10px; margin-top:5px; cursor:pointer;">
                            <a class="btnBack" data-iconpos="notext" data-theme="b" data-role="button" data-icon="back" title="Back">Back</a>
                            <a style="margin-top:8px;" class="btnHome" data-iconpos="notext" data-theme="b" data-role="button" data-icon="home" title="Home">Home</a>
                        </div>*@

                        <div style="float:left; margin-top:10px; margin-left:25px; margin-bottom:0px;">                            
                            <div class="OrganizationNameHeader" style="text-overflow:ellipsis; font-size:25px; white-space:nowrap; overflow:hidden;">
                                Company Name
                            </div>
                        </div>
                    </div>
                    <div class="ui-block-b txt-right" style="padding:0px 0px 0px 20px;">
                        <div id="divSearchBox" style="margin-bottom:-10px; display:block;">
                            <div class="ui-grid-solo">
                                <div class="ui-block-a" style="width:100%">
                                    <input type="text" placeholder="Search Here" class="txtSearchKB" onclick="this.select()" />
                                </div>
                                @*<div class="ui-block-b" style="width:20%">
                                    <a class="btnSettings" style="margin-top:13px; float:right;" data-iconpos="notext" data-theme="b" data-role="button" data-icon="gear" title="Setting">Setting</a>
                                </div>*@
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div id="HeaderMobileVersion">
                <div class="ui-grid-solo">
                    <div class="ui-block-a OrganizationNameHeader txt-center" style="white-space:nowrap; overflow:hidden; text-overflow:ellipsis;">
                        Company Name Here
                    </div>
                </div>
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <a class="btn custom-btn-theme-a btnHome">
                            <span class="ui-icon ui-icon-home" style="display:inline-block;">
                                <p style="margin:16px 0px 0px 0px; position:relative; transform: translate(-50%, 50%); font-size:12px;">Home</p>
                            </span>
                        </a>
                    </div>
                    <div class="ui-block-b">
                        <a id="btnSearchMobile" class="btn custom-btn-theme-a">
                            <span class="ui-icon ui-icon-search" style="display:inline-block;">
                                <p style="margin:16px 0px 0px 0px; position:relative; transform: translate(-50%, 50%); font-size:12px;">Search</p>
                            </span>
                        </a>
                    </div>
                    @*<div class="ui-block-c">
                        <a class="btn custom-btn-theme-a btnPeriod">
                            <span class="ui-icon ui-icon-grid" style="display:inline-block;">
                                <p style="margin:16px 0px 0px -7px; font-size:12px;">Period</p>
                            </span>
                        </a>
                    </div>
                    <div class="ui-block-c">
                        <a class="btn custom-btn-theme-a btnSettings">
                            <span class="ui-icon ui-icon-gear" style="display:inline-block;">
                                <p style="margin:16px 0px 0px 0px; position:relative; transform: translate(-50%, 50%); font-size:12px;">Settings</p>
                            </span>
                        </a>
                    </div>*@
                </div>
                <div class="contentSearchEngine">
                    <div class="ui-grid-solo ui-responsive">
                        <div class="ui-block-a" style="padding:6px;">
                            <input type="text" placeholder="Search Here" class="txtSearchKB" />
                        </div>
                    </div>
                </div>
                @*<div class="divLblPeriod">
                    <div class="lblPeriod">
                        Today
                    </div>
                </div>
                <br />*@
            </div>

            <div class="BreadCrumbs">
                <div class="ItemsBreadCrumbs container">
                    Main Dashboard
                </div>
            </div>
        
        </div>
    </div>

    <div>
        <div id="panAddSolution" class="modal fade" data-backdrop="static" tabindex="-1">
            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Solution and Answer</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div style="display:none;">
                            <input type="text" id="txtSolutionID" style="display:none;" data-role="none" />
                        </div>

                        <div class="row mb-3">
                            <div class="col-md-2">
                                <label for="chbPrivate">Is this Private?</label>
                            </div>
                            <div class="col-md">
                                <input data-role="none" class="form-check-input" type="checkbox" name="chbPrivate" id="chbPrivate" />
                            </div>
                        </div>

                        <div id="divAllowedTeam" style="display:none">
                            <div class="row mb-3 align-items-center">
                                <div class="col-md">
                                    Teams allowed to view
                                </div>
                                <div class="col-md text-right">
                                    <button data-role="none" id="btnPrivateSolution" class="btn btn-success">Add Team</button>
                                </div>
                            </div>

                            <div id="listofAllowedTeam" style="max-height: 150px; overflow:auto;"></div>
                        </div>

                        <textarea id="txtSolutionAndAnswer" class="form-control mb-3" placeholder="Enter your solution or answer" style="min-height:200px; max-height:200px;"></textarea>

                        <div class="row mb-3 mt-5">
                            <div class="col-md"><h4>List of Steps</h4></div>
                            <div class="col-md text-right">
                                <button data-role="none" class="btn btn-success" id="btnSteps">Add Step</button>
                            </div>
                        </div>

                        <ul id="listSolutionSteps" data-role="none" class="draggableList list-group" style="list-style-type: none; margin:0 auto; padding:0;"></ul>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-md order-1 order-md-0">
                                <button data-role="none" type="button" id="btnCancelSolution" class="btn btn-secondary btn-block btn-lg">Cancel</button>
                            </div>
                            <div class="col-md order-1 order-md-0">
                                <button data-role="none" type="button" id="btnOkSolution" class="btn btn-success btn-block btn-lg">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="panNewTeam" class="modal fade" data-backdrop="static" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">New Team</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div class="mb-3">
                            <label for="txtNewTeamName">Team Name:</label>
                            <input data-role="none" class="form-control" type="text" id="txtNewTeamName" />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-md order-1 order-md-0">
                                <button data-role="none" type="button" id="btnCancelNewTeam" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-md order-1 order-md-0">
                                <button data-role="none" type="button" id="btnOkNewTeam" class="btn btn-success btn-block btn-lg">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="panAddSteps" class="modal fade" data-backdrop="static" tabindex="-1">
            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">New Steps</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <form method="post" enctype="multipart/form-data" id="formStepPic" style="display:none;">
                            <input data-role="none" type="file" accept="image/*" name="fileStepPic" id="fileStepPic" style="margin-left:-500px; position:absolute;" />
                        </form>

                        <div class="text-right mb-3">
                            <button id="btnFileOpener" data-role="none" class="btn btn-success">
                                Attach Image
                            </button>
                        </div>
                        
                        @*<button data-role="none" id="btnsample">sample</button>*@
                        <div id="txtNewSteps" contenteditable style="min-height:250px; max-height:250px;" placeholder="Type the step here ...">
                            <img id="imgpix" />
                        </div>
                        @*<textarea id="txtNewSteps2" placeholder="Enter your solution or answer" style="min-height:250px; max-height:250px;"></textarea>*@
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-auto">
                                <button data-role="none" type="button" id="btnCancelNewSteps" class="btn btn-secondary btn-block btn-lg">Cancel</button>
                            </div>
                            <div class="col">
                                <button data-role="none" type="button" id="btnOkNewSteps" class="btn btn-success btn-block btn-lg">Ok and Close</button>
                            </div>
                            <div class="col">
                                <button data-role="none" type="button" id="btnOkAndAddNewSteps" class="btn btn-success btn-block btn-lg">Ok and Add another Step</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="panAddObject" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="lblAddObject">Add Case</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div class="mb-3">
                            <label for="txtObject" class="form-label"><span id="lblAddObjectTitle">Case ID</span>:</label>
                            <input data-role="none" type="text" class="form-control" id="txtObject" placeholder="" onclick="this.select()" />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-md order-1 order-md-0">
                                <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-md order-1 order-md-0">
                                <button data-role="none" type="button" id="btnSaveObject" class="btn btn-success btn-block btn-lg">Save</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="panFilter" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Filter</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body" id="divKbFilter">
                        <div class="mb-3">
                            <label for="selFilterTopic" class="form-label">Topic:</label>
                            <select id="selFilterTopic" class="form-select"></select>
                        </div>

                        <div class="mb-3">
                            <label for="selFilterSubTopic" class="form-label">Sub Topic:</label>
                            <select id="selFilterSubTopic" class="form-select"></select>
                        </div>

                        <div class="mb-3">
                            <label for="selFilterProduct" class="form-label">Product:</label>
                            <select id="selFilterProduct" class="form-select"></select>
                        </div>

                        <div class="mb-3">
                            <label for="selFilterLanguage" class="form-label">Language:</label>
                            <select id="selFilterLanguage" class="form-select"></select>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-md order-1 order-md-0">
                                <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-md order-1 order-md-0">
                                <button data-role="none" type="button" id="btnOkFilter" class="btn btn-success btn-block btn-lg">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div>
        <div subpage="pagekb" id="panMainDashboardKB" style="display:block;">
            <div id="divKBTab" style="white-space:nowrap; overflow:auto;">
                <ul class="nav nav-tabs" role="tablist" style="flex-wrap:nowrap;">
                    <li class="nav-item" role="presentation">
                        <button data-role="none" class="nav-link active" id="btnKBDashboard" data-toggle="tab" data-target="#tabKBWork">Knowledge Base</button>
                    </li>
                    <li class="nav-item" role="presentation">
                        <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabKBSettings">Settings</button>
                    </li>
                </ul>
            </div>
            <div class="tab-content p-3">
                <div id="tabKBWork" class="tab-pane fade show active">
                    <div class="row">
                        <div class="col-md mb-3">
                            <input data-role="none" class="form-control txtSearchKB" placeholder="Search here" />
                        </div>
                        <div class="col-md text-right">
                            <button data-role="none" class="btn btn-secondary" id="btnKBFilter">Filter</button>
                            <button data-role="none" class="btn btn-success" id="btnAddKB">Add Knowledge Base</button>
                        </div>
                    </div>

                    <div id="divFilteredBy" class="row mb-3" style="display:none">
                        <div class="col-md-auto mb-3">Filtered By:</div>
                        <div id="listFilteredBy" class="col-md"></div>
                    </div>

                    <div style="overflow:auto;">
                        <table id="dTableKb" data-role="none" class="table table-light table-striped table-hover">
                            <thead>
                                <tr class="align-top text-center" style="font-size:14px;">
                                    <th data-sort="accounting" style="cursor:pointer;"> # </th>
                                    <th data-sort="string" style="cursor:pointer;"> Problem </th>
                                    @*<th data-sort="string" style="cursor:pointer;"> Solution </th>*@
                                    <th data-sort="string" style="cursor:pointer;"> Topic </th>
                                    <th data-sort="string" style="cursor:pointer;"> Product </th>
                                    @*<th data-sort="string" style="cursor:pointer;"> Language </th>*@
                                    <th data-sort="string" style="cursor:pointer;"> Modified By </th>
                                    <th data-sort="string" style="cursor:pointer;"> Last Modified </th>
                                    <th style="cursor:pointer;">  </th>
                                </tr>
                            </thead>
                            <tbody id="tblKBbody"></tbody>
                        </table>
                    </div>
                </div>

                <div id="tabKBSettings" class="tab-pane fade">
                    <div class="row wrapper min-vh-100 flex-column flex-md-row">
                        <div class="col-12 col-md-2 p-0 bg-light flex-shrink-1 mb-3">
                            <nav class="navbar navbar-expand-sm navbar-light bg-light align-items-start flex-md-column flex-row p-2">
                                <a data-role="none" class="navbar-brand" href="#">Menu</a>
                                <a data-role="none" href="#" class="navbar-toggler" data-toggle="collapse" data-target=".sidebarKBSettings">
                                    <span class="navbar-toggler-icon"></span>
                                </a>
                                <div class="collapse navbar-collapse sidebarKBSettings mt-2">
                                    <ul class="flex-column w-100 justify-content-between nav nav-pills">
                                        <li><a class="nav-link active" role="button" data-role="none" data-toggle="tab" href="#tabKBGeneralSettings">General Settings</a></li>
                                        <li><a class="nav-link" role="button" id="btnArrangeReadingList" data-role="none" data-toggle="tab" href="#tabKBReadingListArrangement">Reading List Arrangement</a></li>                                        
                                    </ul>
                                </div>
                            </nav>
                        </div>

                        <div class="col bg-faded">
                            <div class="tab-content">
                                <div class="tab-pane fade active show" id="tabKBGeneralSettings">
                                    <div class="row mb-3 align-items-center">
                                        <div class="col-md-auto mb-3 mb-sm-0">
                                            KB Server IP or URL:
                                        </div>
                                        <div class="col-md">
                                            <input data-role="none" class="form-control" id="txtKBServerIP" />
                                        </div>
                                    </div>
                                </div>

                                <div class="tab-pane fade" id="tabKBReadingListArrangement">
                                    <div class="mb-3">
                                        Notes: Drag and drop list into your desired order.
                                    </div>
                                    <ul id="lstArrangeList" class="custom-list draggableList"></ul>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="mt-4 text-center" style="position:fixed; bottom:20px; width:100%;">
                        <button id="btnSaveArrangeReadingList" data-role="none" class="btn btn-success btn-lg">Save</button>
                    </div>
                </div>
            </div>            
        </div>        

        <div subpage="pagekb" id="panAddKnowledgeManagement" style="display:none;">
            <div style="display:none;">
                <input type="text" id="txtProblemDBID" style="display:none;" data-role="none" />
                <input type="text" id="txtProblemShortLink" style="display:none;" data-role="none" />
            </div>

            <div class="input-group mb-3" id="divCaseUrl">
                <input data-role="none" id="txtCaseUrl" class="form-control" readonly type="text" onclick="this.select()" />
                <button data-role="none" id="btnCaseUrl" class="btn btn-secondary">Copy Link</button>
            </div>

            <div class="row mb-3">
                <div class="col-2">
                    <h5>Topic:</h5>
                </div>
                <div class="col">
                    <div style="display:none;">
                        <select id="selTopic" data-role="none" class="form-select">
                            <option value="">Select Topic</option>
                        </select>
                    </div>

                    <div id="listTopic" style="max-height:400px; overflow:auto; word-break:break-word;"></div>
                </div>
                <div class="col-auto">
                    <button data-role="none" class="btn btn-success" id="btnAddNewTopic">Add New Topic</button>
                </div>
            </div>

            <div class="custom-ui-label" style="display:none;">
                <div>SubTopic  </div>
                <div>
                    <select id="selSubTopic">
                        <option>Select SubTopic</option>
                    </select>
                </div>
            </div>
            <hr />

            <div class="row mb-3">
                <div class="col-2">
                    <h5>Product:</h5>
                </div>
                <div class="col">
                    <div style="display:none;">
                        <select id="selProduct" data-role="none" class="form-select">
                            <option value="">Select Topic</option>
                        </select>
                    </div>

                    <div id="listProduct" style="max-height:400px; overflow:auto; word-break:break-word;"></div>
                </div>
                <div class="col-auto">
                    <button data-role="none" class="btn btn-success" id="btnAddNewKbProduct">Add Product</button>
                </div>
            </div>

            <div class="row mb-3 align-items-center">
                <div class="col-2"><h5>Language:</h5></div>
                <div class="col">
                    <select data-role="none" id="selLanguage" class="form-select">
                        <option>Select Language</option>
                    </select>
                </div>
            </div>

            <div class="row mb-3 align-items-center">
                <div class="col-2"><h5>Tags:</h5></div>
                <div class="col">
                    <input data-role="none" class="form-control" type="text" id="txtTags" placeholder="Enter Case Tags" />
                </div>
            </div>

            <div class="row mb-3 align-items-center">
                <div class="col-2"><h5>Problem or Question:</h5></div>
                <div class="col">
                    <input data-role="none" class="form-control" type="text" id="txtCase" placeholder="Enter Problem or Question" />
                </div>
            </div>

            <div class="row mb-3 mt-5">
                <div class="col-md"><h4>List of Solutions or Answers</h4></div>
                <div class="col-md text-right">
                    <button data-role="none" class="btn btn-success" id="btnAddSolution">Add Solution/Answer</button>
                </div>
            </div>

            <ol id="listSolution" data-role="none" class="draggableList list-group"></ol>

            <div class="text-center mb-5 mt-5">
                <button id="btnCancelKM" data-role="none" class="btn btn-secondary btn-lg">
                    Cancel
                </button>
                <button id="btnSaveKM" data-role="none" class="btn btn-success btn-lg">
                    Save
                </button>
            </div>
        </div>
    </div>
    <div id="divlinkimg" class="d-none"></div>
</div>