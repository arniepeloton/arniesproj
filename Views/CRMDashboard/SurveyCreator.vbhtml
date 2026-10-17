<script>
    require([GetPathFormat('/Scripts/sources/CRMDashboard/SurveyCreator.js?crc=')]);
</script>

<div id="divLoadingSurveyCreator" subpage="pageClientStage" style="display:block; text-align:center;">Loading, please wait ...</div>

<div id="divMainSurveyCreatorPage" subpage="pageClientStage" style="display:none;">
    <div>
        <div id="panAddSolution" custom-data-role="popup">
            <div class="popup-md">
                <div class="popup-header">
                    Question
                    <a class="popup-close">x</a>
                </div>

                <div class="popup-content">
                    <div class="custom-ui-label" style="display:none;">
                        <div>
                            Is this Private?
                        </div>
                        <div>
                            <label for="chbPrivate" class="checkbox-only">
                                <input type="checkbox" name="chbPrivate" id="chbPrivate" data-iconpos="notext" />
                                Private
                            </label>
                        </div>
                    </div>


                    @*<div id="divAllowedTeam" style="display:none">
        <button data-role="none" id="btnPrivateSolution" class="btn custom-btn-theme-b btn-less-width" style="float:right;">Add Team</button>
        <div style="clear:both;"></div>
        <br />
        Teams allowed to view:
        <div id="listofAllowedTeam" style="max-height: 150px; overflow:auto;">

        </div>
        <br />
    </div>
    <hr />
    <br />*@                    

                    <input type="text" id="txtSolutionID" style="display:none;" data-role="none" />
                    <textarea id="txtSolutionAndAnswer" placeholder="Enter your question"></textarea>
                    

                    <button data-role="none" id="btnSteps" title="Add Answer" class="btn custom-btn-theme-b btnCircle-xs">
                        <img class="img-plus-white" />
                    </button>

                    <ul id="listAnswer" data-role="none" style="list-style-type: none; margin:0 auto; padding:0;"></ul>
                </div>

                <div class="popup-footer align-right">
                    <div style="text-align:right;">
                        <button id="btnCancelSolution" data-role="none" class="btn custom-btn-theme-c btnCircle">
                            <img class="img-arrow-left" />
                        </button>
                        <button id="btnOkSolution" title="Ok" data-role="none" class="btn custom-btn-theme-b btnCircle">
                            <img class="img-check-white" />
                        </button>
                    </div>
                </div>
            </div>
        </div>

        <div data-role="popup" id="panNewTeam" data-overlay-theme="a" data-theme="a" data-corners="false">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3 style="margin-left:10px; margin-right:10px;">New Team</h3></center>
            <div data-role="content" style="width:300px; word-wrap: break-word;">
                <div class="custom-ui-label">
                    <div>Team Name:</div>
                    <div><input type="text" id="txtNewTeamName" /></div>
                </div>

                <br />
                <button data-role="none" id="btnOkNewTeam" class="btn custom-btn-theme-b">Ok</button>
                <br /><br />
                <a data-role="none" data-rel="back" id="btnCancelNewTeam" class="btn custom-btn-theme-c">Cancel</a>
            </div>
        </div>

        <div data-role="popup" id="panAddSteps" data-overlay-theme="a" data-theme="a" data-corners="false">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right" id="btnClosePopUp">Close</a>
            <center><h3 style="margin-left:10px; margin-right:10px;">Answer</h3></center>
            <div data-role="content" class="ui-content ui-body-d" style="width:300px; word-wrap: break-word;">
                <textarea id="txtAnswerDescription" placeholder="Enter decription"></textarea>
                <br />
                
                <div>
                    Control Type:
                    <select id="selControlType">
                        <option value="text">Text field</option>
                        <option value="radio">Radio</option>
                        <option value="checkbox">Checkbox</option>
                    </select>
                </div>

                <br />
                @*<button data-role="none" id="btnOkAndAddNewSteps" class="btn custom-btn-theme-b">Ok and Add another Step</button>
                <br /><br />*@
                <button data-role="none" id="btnOkAnswer" class="btn custom-btn-theme-g">Ok</button>
                <br /><br />
                <button data-role="none" id="btnCancelAnswer" class="btn custom-btn-theme-c">Cancel</button>
            </div>
        </div>

        <div data-role="popup" id="panAddObject" data-overlay-theme="a" data-theme="a" data-corners="false">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right" id="btnClosePopUp">Close</a>
            <center><h3 style="margin-left:10px; margin-right:10px;" id="lblAddObject">Add Case</h3></center>
            <div data-role="content" class="ui-content ui-body-d" style="word-wrap: break-word;" class="jPopupContent">

                <div id="lblAddObjectTitle">Case ID</div>
                <input type="text" id="txtObject" placeholder="" />

                <br />

                <button id="btnSaveObject" data-role="none" class="btn custom-btn-theme-b">
                    Save
                </button>
                <br /><br />
                <a href="#" data-rel="back" data-role="none" class="btn custom-btn-theme-c">Cancel</a>
            </div>
        </div>
    </div>
    <input type="text" id="txtProblemDBID" style="display:none;" data-role="none" />
    <div class="custom-ui-label">
        <div>Topic</div>
        <div>
            <select id="selTopic" data-inline="true">
                <option value="">Select Topic</option>
            </select>
        </div>
    </div>

    <div class="custom-ui-label">
        <div>SubTopic</div>
        <div>
            <select id="selSubTopic" data-inline="true">
                <option>Select SubTopic</option>
            </select>
        </div>
    </div>

    <div class="custom-ui-label">
        <div>Product</div>
        <div>
            <select id="selProduct" data-inline="true">
                <option>Select Product</option>
            </select>
        </div>
    </div>

    <div class="custom-ui-label">
        <div>Language</div>
        <div>
            <select id="selLanguage" data-inline="true">
                <option>Select Language</option>
            </select>
        </div>
    </div>

    <div class="custom-ui-label">
        <div>Tags</div>
        <div>
            <input type="text" id="txtTags" data-inline="true" placeholder="Enter Case Tags" />
        </div>
    </div>

    <div class="custom-ui-label">
        <div>Survey Name</div>
        <div>
            <input type="text" id="txtCase" data-inline="true" placeholder="Enter Survey Name" />
        </div>
    </div>

    <div class="panel">
        <div class="panel-title custom-theme-h">
            Questions
        </div>

        <div class="panel-content txt-left">
            <div style="text-align:right;">
                <button id="btnAddSolution" data-role="none" class="btn custom-btn-theme-b btnCircle-sm">
                    <img class="img-plus-white" />
                </button>
            </div>
            <div style="padding: 8px 15px 0px 15px;">
                <ol id="listSolution" data-role="none" style=" margin:0 auto; padding:0;"></ol>
            </div>
        </div>
    </div>
</div>