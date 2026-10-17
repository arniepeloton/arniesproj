@Code
    'ViewData("Title") = "Survey Creator"
    'Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
@Section head
    @*<link rel="stylesheet" href="/Content/flatui-css-custom.css" />*@
    @*<link rel="stylesheet" href="/Content/bootstrap/css/bootstrap.css" />*@
    @*<link rel="stylesheet" href="/Content/bootstrap.css" />
    <link rel="stylesheet" href="/Content/bootstrap-custom.css" />
    <link rel="stylesheet" href="/Content/jquery.dataTables2.css" />*@
    

    @*<script>
        require([GetPathFormat('/Scripts/sources/Survey/Creator.js?crc='),
            GetPathFormat('/Scripts/bootstrap.min.js?crc='),
        GetPathFormat('/Scripts/summernote.js?crc=')]);
    </script>*@



End Section
<link rel="stylesheet" href="/Content/summernote.css" />

<style type="text/css">
    /*.ItemsBreadCrumbs {
        padding: 10px 0px 7px 0px;
    }*/

    /*@@font-face {
            font-family: "myfont";
            src: url("/Content/fonts/summernote.woff") format('woff');
        }*/
</style>

<div>
    <div id="popQuestionaire" class="modal fade" tabindex="-1" data-backdrop="static">
        <div class="modal-dialog modal-fullscreen">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Survey</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div id="divTabSurvey">
                        <ul class="nav nav-tabs" role="tablist" style="flex-wrap:nowrap;">
                            <li class="nav-item" role="presentation">
                                <button data-role="none" class="nav-link active" data-toggle="tab" data-target="#tabSurveyDesign">Design</button>
                            </li>
                            <li class="nav-item" role="presentation">
                                <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabSurveyRequirements">Requirements</button>
                            </li>
                            <li class="nav-item" role="presentation">
                                <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabSurveyRespondents">Respondents</button>
                            </li>
                        </ul>
                    </div>

                    <div class="tab-content p-3">
                        <div id="tabSurveyDesign" class="tab-pane fade show active">
                            <div class="row mb-3" id="divSurveyUrl">
                                <div class="col-md">
                                    <div class="input-group">
                                        <input data-role="none" type="text" id="txtSurveyUrl" class="form-control" onclick="this.select()" readonly placeholder="Link here">
                                        <button data-role="none" id="btnSurveyUrl" class="btn btn-secondary" type="button">Copy Link</button>
                                        <button data-role="none" id="btnSurveyPreview" class="btn btn-secondary">Preview</button>
                                    </div>
                                </div>
                            </div>

                            <div class="row mb-3">
                                <div class="col-md-2 col-form-label"><label for="txtQuestionaireTitle">Survey Title:</label></div>
                                <div class="col-md">
                                    <input type="text" data-role="none" class="form-control" id="txtQuestionaireTitle" />
                                </div>
                            </div>
                            <div class="row mb-3">
                                <div class="col-md-2 col-form-label"><label for="selSurveyStatus">Active survey?</label></div>
                                <div class="col-md">
                                    <select data-role="none" class="form-select fixed" id="selSurveyStatus">
                                        <option value="0">No</option>
                                        <option value="1">Yes</option>
                                    </select>
                                </div>
                                <div class="col-md-auto text-right">
                                    <button title="Add Question" id="btnSurveyAddPage" data-role="none" class="btn btn-success">
                                        Add Page
                                    </button>
                                </div>
                            </div>

                            <div id="divSurveyPage">
                                <ul id="lisSurveyPage" class="nav nav-tabs" role="tablist" style="flex-wrap:nowrap;"></ul>
                            </div>
                            <div id="divSurveyPageContent" class="tab-content p-3"></div>
                            <ul id="listQuestions" class="custom-list"></ul>
                        </div>

                        <div id="tabSurveyRequirements" class="tab-pane fade">
                            <div class="mb-3 text-right">
                                <button title="Add Requirement" id="btnSurveyAddRequirement" data-role="none" class="btn btn-success">
                                    Add Requirement
                                </button>
                            </div>
                            
                            <div class="table-responsive">
                                <table id="tblDataTableSurveyRequirement" data-role="none" style="width:100%; display:table;" class="table table-light table-striped table-hover">
                                    <thead>
                                        <tr class="custom-theme-f">
                                            <th style="cursor:pointer; width:20px;"> # </th>
                                            <th style="cursor:pointer;"> Name </th>
                                            <th style="cursor:pointer; width:100px;"> No. of Copies </th>
                                            <th style="cursor:pointer; width:100px;"> Universal Requirement </th>
                                            <th style="cursor:pointer; width:100px;"> Required by </th>
                                            <th style="cursor:pointer; width:100px;"> Last Updated </th>
                                            <th style="cursor:pointer; width:100px;"> </th>
                                        </tr>
                                    </thead>
                                    <tbody></tbody>
                                </table>
                            </div>                            
                        </div>

                        <div id="tabSurveyRespondents" class="tab-pane fade">
                            <div id="divRespondents"></div>
                            @*<button data-role="none" id="btnDownloadMrt" class="btn btn-secondary">Download</button>*@
                            <ul id="listQuestionsReadOnly" class="list-padding" style="list-style-type:decimal;"></ul>
                        </div>
                    </div>                    
                </div>
                <div class="modal-footer justify-content-center">
                    <button data-role="none" type="button" id="btnSurveyCancelQuestion" class="btn btn-secondary btn-lg" data-dismiss="modal">Cancel</button>
                    <button data-role="none" type="button" id="btnSurveySaveQuestion" class="btn btn-success btn-lg">Save</button>
                </div>
            </div>
        </div>
    </div>

    <div id="popNewQuestion" class="modal fade" tabindex="-1" data-backdrop="static">
        <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Question</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-md-12">
                            <div class="row">
                                <div class="col-md-2 col-form-label"><label for="selQuestionType">Type:</label></div>
                                <div class="col-md-9">
                                    <select id="selQuestionType" data-role="none" class="form-select">
                                        <option value="Single Input">Single Input</option>
                                        <option value="Page Text">Page Text</option>
                                        <option value="Multiple Inputs">Multiple Inputs</option>
                                        <option value="radio">Multiple Choice (Single Answer)</option>
                                        <option value="radioinput">Multiple Choice with input (Single Answer)</option>
                                        <option value="checkbox">Multiple Choice (Multiple Answers)</option>
                                        <option value="checkboxinput">Multiple Choice with input (Multiple Answers)</option>
                                        <option value="Matrix">Matrix</option>
                                        <option value="file">Upload File</option>
                                    </select>
                                </div>
                            </div>
                            <br />
                            <div id="divForQuestions" subpage="pageContent" style="display:block;">
                                <div class="row">
                                    <div class="col-md-2 col-form-label"><span class="required">*</span><label id="lblQuestion" for="txtQuestion">Description:</label></div>
                                    <div class="col-md-9"><input data-role="none" required type="text" name="txtQuestion" class="form-control" id="txtQuestion"></div>
                                </div>
                                <div id="divForQuestionTypeField"></div>
                                <div id="divOptionForRad" class="divTable" style="display:none; font-size:inherit !important; width:auto !important;">
                                    <div class="divTableRow">
                                        <div class="divTableCell txt-middle">
                                            Present list as :
                                        </div>
                                        <div class="divTableCell txt-middle">
                                            <div style="display:inline-flex">
                                                <div class="form-check">
                                                    <label class="form-check-label">
                                                        <input type="radio" class="form-check-input" name="choiceRadPresentation" value="0" />
                                                        Radio button
                                                    </label>
                                                </div>
                                                <div class="form-check">
                                                    <label class="form-check-label">
                                                        <input type="radio" class="form-check-input" name="choiceRadPresentation" value="1" />
                                                        Select Menu
                                                    </label>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div id="divRequired" class="form-check">
                                    <label for="chbQuestionRequired" class="form-check-label">
                                        <input type="checkbox" data-role="none" id="chbQuestionRequired" class="form-check-input">
                                        Require an answer to this question
                                    </label>
                                </div>
                                <div id="divOptionForChbRad" style="display:none;">
                                    <div class="form-check">
                                        <label for="chbChbRadOtherField" class="form-check-label">
                                            <input type="checkbox" data-role="none" id="chbChbRadOtherField" class="form-check-input">
                                            Add an "<b>Other</b>" answer option or comment field
                                        </label>
                                    </div>
                                    <div id="divOtherField" style="display:none;">
                                        <div style="margin-left:30px;">
                                            <div class="form-check">
                                                <label for="chbChbRadAsAnswer" class="form-check-label">
                                                    <input type="radio" data-role="none" name="radOther" value="answer" id="chbChbRadAsAnswer" class="form-check-input">
                                                    Display as answer choice
                                                </label>
                                            </div>
                                            <div class="form-check">
                                                <label for="chbChbRadAsComment" class="form-check-label">
                                                    <input type="radio" data-role="none" name="radOther" value="comment" id="chbChbRadAsComment" class="form-check-input">
                                                    Display as comment field
                                                </label>
                                            </div>
                                            <div class="form-inline">
                                                <div class="form-group">
                                                    <label>Enter Label:</label> <input type="text" data-role="none" onfocus="this.select()" id="txtOtherField" class="form-control" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div id="divOptionForMatrix" style="display:none;">
                                    <div class="form-check">
                                        <label for="chbChbRadMatrix" class="form-check-label">
                                            <input type="checkbox" data-role="none" id="chbChbRadMatrix" class="form-check-input">
                                            Allow multiple responses per row (use checkboxes)
                                        </label>
                                    </div>
                                </div>
                            </div>
                            <div id="divForDescription" subpage="pageContent" style="display:none;">
                                <div id="divDescription"></div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col-md order-1 order-md-0">
                            <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Close</button>
                        </div>
                        <div class="col-md order-1 order-md-0">
                            <button data-role="none" type="button" id="btnOkQuestion" class="btn btn-success btn-block btn-lg">Ok</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popSurveyPageName" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Page Name</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <input data-role="none" type="text" class="form-control form-control-lg" id="txtSurveyPageName" placeholder="Enter page name" />
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col-md order-1 order-md-0">
                            <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Close</button>
                        </div>
                        <div class="col-md order-1 order-md-0">
                            <button data-role="none" type="button" id="btnOkSurveyPageName" class="btn btn-success btn-block btn-lg">Ok</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popRequirement" class="modal fade" tabindex="-1" data-backdrop="static">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Requirement</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-md-4">
                            <div class="mb-3">
                                <label class="form-label">Required by: <span class="required">*</span></label>
                                <div class="input-group" id="divSurveyUrl">
                                    <input data-role="none" type="text" id="txtReqGroupName" placeholder="Enter new group name" class="form-control" onclick="this.select()">
                                    <button data-role="none" id="btnReqGroup" class="btn btn-secondary" type="button">Add</button>
                                </div>
                            </div>
                            <div id="divReqGroup"></div>
                        </div>
                        <div class="col-md-8">
                            <div class="mb-3">
                                <label class="form-label" for="selReqType">Requirement Type:</label>
                                <select id="selReqType" data-role="none" class="form-select">
                                    <option value="1">System Generated</option>
                                    <option value="0">Non-System Generated</option>
                                </select>
                            </div>
                            
                            <div class="mb-3">
                                <label class="form-label" for="txtRequirementName">Requirement Description: <span class="required">*</span></label>
                                <input type="text" id="txtRequirementName" data-role="none" class="form-control" />
                            </div>
                            
                            <div class="mb-3">
                                <label class="form-label" for="txtRequirementQty">No. of Copies: <span class="required">*</span></label>
                                <input type="text" id="txtRequirementQty" data-role="none" class="form-control" />
                            </div>

                            <div id="divReqFile" style="display:block;" class="mb-3">
                                <label class="form-label" for="txtRequirementQty">Upload MRT File: <span id="reqMrtFile" class="required">*</span></label>
                                <input type="file" id="fileReqMrt" placeholder="Select only .mrt file" data-role="none" class="form-control" accept=".mrt" />
                            </div>

                            <div class="form-check">
                                <label class="form-check-label">
                                    <input type="checkbox" id="chbIsUniversal" class="form-check-input" value="1" />
                                    This is a universal requirement
                                </label>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col-md order-1 order-md-0">
                            <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Close</button>
                        </div>
                        <div class="col-md order-1 order-md-0">
                            <button data-role="none" type="button" id="btnOkCloseRequirement" class="btn btn-success btn-block btn-lg">Ok</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popListOfRequirements" class="modal fade" tabindex="-1" data-backdrop="static">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">List Of Requirements</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <ul id="listofreq" data-role="none" class="list-group"></ul>
                </div>
            </div>
        </div>
    </div>

    <div id="popRearrangeQuestions" class="modal fade" tabindex="-1" data-backdrop="static">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Rearranging Questions</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <b class="mb-3">NOTE:</b> Please drag and drop the items into correct order.
                    <ul id="listrearrangequestions" data-role="none" style="list-style-type:decimal;"></ul>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col-md order-1 order-md-0">
                            <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Close</button>
                        </div>
                        <div class="col-md order-1 order-md-0">
                            <button data-role="none" type="button" id="btnOkRearrange" class="btn btn-success btn-block btn-lg">Ok</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popSurveyMoreOptions" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">More Options</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <label>Character Length of <span id="lblFieldName"></span></label>
                    <br /><br />
                    <div class="row">
                        <div class="col-md-2 col-form-label">Min</div>
                        <div class="col-md-10"><input data-role="none" type="text" class="form-control" id="txtCharMin" placeholder="Enter character minimum length" /></div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-md-2 col-form-label">Max</div>
                        <div class="col-md-10"><input data-role="none" type="text" class="form-control" id="txtCharMax" placeholder="Enter character maximum length" /></div>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col-md order-1 order-md-0">
                            <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Close</button>
                        </div>
                        <div class="col-md order-1 order-md-0">
                            <button data-role="none" type="button" id="btnOkSurveyMoreOptions" class="btn btn-success btn-block btn-lg">Ok</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<div id="divSurveyLoading" style="display:none; padding:10px;">Loading, please wait ...</div>
@*<div id="custom_datatables_processing" style="padding: 10px; position: absolute; z-index: 1100; background-color: rgba(0,0,0, 0.5); width: 100%; top: 0; bottom: 0; text-align: center; color: #FFF; text-shadow: none;">
        <div style="transform: translate(-50%, -50%); left: 50%; top: 50%; position: fixed;">Loading, please wait ...</div>
    </div>*@
<div id="divSurveyMainPage" style="display:block">
    <div >
        <div id="pageListOfSurvey" subpage="divSurveyMainPage" style="display:block;">
            <div class="row mb-3 align-items-center">
                <div class="col-md-auto">
                    Show:                    
                </div>
                <div class="col-md-4">
                    <select data-role="none" class="form-select fixed" id="selStatusSurvey">
                        <option value="All">All</option>
                        <option value="1" selected>Active</option>
                        <option value="0">Inactive</option>
                    </select>
                </div>
                <div class="col-md">
                    <input type="text" id="txtSearchSurvey" data-role="none" placeholder="Search here" class="form-control" />
                </div>
            </div>
            
            <div class="table-responsive mb-5">
                <table id="tblDataTableSurvey" data-role="none" style="width:100%; display:table;" class="table table-light table-striped table-hover">
                    <thead>
                        <tr class="custom-theme-f align-top text-center">
                            <th style="cursor:pointer; width:20px;"> # </th>
                            <th style="cursor:pointer;"> Questionaire Title </th>
                            <th style="cursor:pointer; width:100px;"> Responses </th>
                            <th style="cursor:pointer; width:150px;"> Modified By </th>
                            <th style="cursor:pointer; width:170px;"> Modified Date </th>
                            <th style="cursor:pointer; width:100px;"> Status </th>
                        </tr>
                    </thead>
                    <tbody id="tbodySurvey"></tbody>
                </table>
            </div>            

            <br />
    @*<input data-role="none" type="text" placeholder="Enter Full Name" class="form-control col-md-4" id="txtMrtFullName" /><br /><br />
    <input data-role="none" type="text" placeholder="Enter Address" class="form-control col-md-4" id="txtMrtAddress" /><br /><br />*@
            @*<button id="btnZip" data-role="none" class="btn btn-secondary">Download Zip</button>*@

            <div class="fixed-ActionBtn" style="display:block;">
                <button data-role="none" id="btnSurveyAdd" class="btn btn-danger rounded-circle">
                    <span class="bi-plus-lg"></span>
                </button>
                <ol class="secondaryActionBtn">
                    <li>
                        <button id="btnSurveyRefresh" data-toggle="tooltip" data-placement="top" title="Refresh" data-role="none" class="btn btn-dark">
                            <span class="bi-arrow-clockwise"></span>
                        </button>
                    </li>
                    <li>
                        <button data-toggle="tooltip" data-placement="top" title="Help" data-role="none" class="btn btn-dark btnKbHelp">
                            <span class="bi-question-lg"></span>
                        </button>
                    </li>
                </ol>
            </div>
        </div>       
    </div>
</div>