require(['common'], 
    function () {
        $(document).ready(function () {
            var arrSolution = [];
            var ctrSolution = 0;
            var editSolution = false;
            var arrSolutionSteps = [];
            var ctrSolutionStep = 0;
            var editSolutionStep = false;
            var arrIndexSolution, arrIndexStep, arrAllowedTeam = [];

            var arrQuestion = [];
            var arrAnswer = [];
            var editAnswer = false;
            var ctrAnswer = 1;
            var arrIndexStep;

            window.SurveyCreator = {
                LoadData: function () {
                    LoadRequiredData();
                }
            }

            LoadRequiredData();

            //Survey Stage
            $('body')
            .on('click', '#btnSaveKM', function () {
                SaveCase();
            })
            .on('click', '#btnCancelKM', function () {
                editSolution = false;
                $('#panAddKnowledgeManagement').hide();
            })
            .on('change', '#selLanguage', function () {
                if ($('#selLanguage').val() == 'AddNew') {
                    objSelected = 'Language';
                    $('#lblAddObject').html('Add Language');
                    $('#lblAddObjectTitle').html('Language');
                    $('#txtObject').val('');
                    $('#panAddObject').popup('open');
                }
            })
            .on('change', '#selProduct', function () {
                if ($('#selProduct').val() == 'AddNew') {
                    objSelected = 'Product';
                    $('#lblAddObject').html('Add Product');
                    $('#lblAddObjectTitle').html('Product');
                    $('#panAddObject').popup('open');
                    $('#txtObject').val('').focus();
                }
            })
            .on('change', '#selTopic', function () {
                if ($('#selTopic').val() == 'AddNew') {
                    objSelected = 'Topic';
                    $('#lblAddObject').html('Add Topic');
                    $('#lblAddObjectTitle').html('Topic');
                    $('#txtObject').val('');
                    $('#panAddObject').popup('open');
                } else {
                    GetAllSubTopics();
                }
            })
            .on('change', '#selSubTopic', function () {
                if ($('#selSubTopic').val() == 'AddNew') {
                    objSelected = 'SubTopic';
                    $('#lblAddObject').html('Add SubTopic');
                    $('#lblAddObjectTitle').html('SubTopic');
                    $('#txtObject').val('');
                    $('#panAddObject').popup('open');
                }
            })
            .on('click', '#btnAddSolution', function () {
                $('#panAddSolution').show();
                $('#txtSolutionAndAnswer').val('').height('50px').focus();
                $('#listAnswer').empty();
                ctrSolution = 0;
            })
            .on('click', '#btnSteps', function () {
                $('#panAddSteps').popup().popup('open');
                $('#txtAnswerDescription').focus();
            })
            .on('click', '#btnOkAndAddNewSteps', function () {
                NewSteps();
                $('#txtNewSteps').val('').height('50px').focus();
            })
            .on('click', '#btnOkNewSteps', function () {
                NewSteps();
                $('#panAddSteps').popup('close');
            })
            .on('click', '#btnCancelNewSteps', function () {
                $('#panAddSteps').popup('close');
                $('#txtNewSteps').val('').height('50px');
                editSolutionStep = false;
            })
            .on('click', '#btnCancelSolution', function () {
                $('#panAddSolution').hide();
                editSolution = false;
                ctrSolutionStep = 0;
                arrSolutionSteps = [];
                ClearSolutionField();
            })
            .on('click', '#btnOkSolution', function () {
                if ($('#chbPrivate').is(':checked') == true) {
                    if ($('.mobilecontainer').length == 0) {
                        alert('Please add team');
                        return;
                    }

                    if ($.trim($('#txtSolutionAndAnswer').val()) == '') {
                        $('#txtSolutionAndAnswer').focus();
                        return;
                    }
                }

                $('#panAddSolution').hide();
                ctrSolutionStep = 0;
                if (editSolution == false) {
                    ctrSolution = ctrSolution + 1;
                    arrSolution.push({
                        //No: ctrSolution,
                        SolutionID: '',
                        Solution: $('#txtSolutionAndAnswer').val(),
                        Step: arrAnswer,
                        State: 'Add',
                        AllowedTeam: arrAllowedTeam,
                        IsPublic: ($('input[name="chbPrivate"]').is(':checked') == true ? false : true)
                    })
                }
                else {
                    arrSolution[arrIndexSolution].Solution = $('#txtSolutionAndAnswer').val();
                    arrSolution[arrIndexSolution].Step = arrAnswer;
                    arrSolution[arrIndexSolution].AllowedTeam = arrAllowedTeam;
                    arrSolution[arrIndexSolution].IsPublic = ($('input[name="chbPrivate"]').is(':checked') == true ? false : true)
                }

                DisplaySolution();
                editSolution = false;
                arrAnswer = [];
                arrAllowedTeam = [];
            })
            .on('click', '#btnPrivateSolution', function () {
                BrowsePopup('LibraryCRMTeam', 'Team',
                        [
                            {
                                AutoSearch: true,
                                AddButton: [
                                    {
                                        Text: 'Create New Team',
                                        Click: function () {
                                            $('#panNewTeam').popup('open');
                                            $('#txtNewTeamName').val('').focus();
                                        }
                                    }
                                ],
                                DisplayText: [{ Column: 'Name' }],
                                Validation: function (e) {
                                    if ($('.mobilecontainer[id="' + e.ID + '"]').length >= 1) {
                                        return '"' + e.Name + '" already exist.'
                                    }
                                    return true;
                                },
                                Click: function (e) {
                                    arrAllowedTeam.push({
                                        State: 'Add',
                                        Name: e.Name,
                                        TeamID: e.ID
                                    });
                                    DisplayAllowedTeam();
                                }
                            }
                        ]
                    )
            })
            .on('click', '#btnOkNewTeam', function () {
                if ($.trim($('#txtNewTeamName').val()) == "") {
                    $('#txtNewTeamName').focus();
                    return;
                }

                AjaxSend({
                    url: 'Survey/SaveNewTeam',
                    data: { TeamName: $('#txtNewTeamName').val(), DateNow: new Date() },
                    success: function (vData) {
                        if (vData.Results == "True") {
                            $('#txtNewTeamName').val('');
                            $('#panNewTeam').popup('close');
                            $('#btnPrivateSolution').click();
                        }
                        else {
                            NewMessageBox(vData.Results, 'ERROR');
                        }
                    }
                })
            })
            .on('change', '#chbPrivate', function () {
                if ($(this).is(':checked')) {
                    $('#divAllowedTeam').show();
                }
                else {
                    $('#divAllowedTeam').hide();
                }
            })
            .on('keyup', '.txtSearchKB', function () {
                $(window).scrollTop(0);
                var term = $(this).val().toLowerCase();
                if (term != "") {
                    $("#tblKBbody > tr").hide();
                    $("#tblKBbody td").filter(function () {
                        return $(this).text().toLowerCase().indexOf(term) > -1
                    }).parent("tr").show();
                }
                else {
                    $("#tblKBbody >tr").show();
                }
                $('.txtSearchKB').val($(this).val())
            })
            .on('click', '#btnAddKB', function () {
                clearKMFields();
                GetKnowledgeManagementNumber()
                $('#panAddKnowledgeManagement').show();
                $('#txtSolution, #txtSteps').height('50px');
            })
            .on('click', '#btnSaveObject', function () {
                SaveObject();
            })
            .on('click', '#btnOkAnswer', function () {
                if ($('#selControlType').val() != "text") {
                    if ($.trim($('#txtAnswerDescription').val()) == "") {
                        $('#txtAnswerDescription').focus();
                    } else {
                        NewAnswers();
                    }
                } else {
                    NewAnswers();
                }
                $('#panAddSteps').popup('close');
            })
            
            function NewAnswers() {
                if(editAnswer == false){
                    arrAnswer.push({
                        Type: $('#selControlType').val(),
                        Description: $.trim($('#txtAnswerDescription').val()),
                        No: ctrAnswer,
                        State: 'add'
                    })

                    ctrAnswer = ctrAnswer + 1;
                } else {
                    arrAnswer[arrIndexStep].Type = $('#selControlType').val();
                    arrAnswer[arrIndexStep].Description = $.trim($('#txtAnswerDescription').val());
                }

                GroupAnswerByType();
            }

            function GroupAnswerByType() {
                function SortByName(a, b) {
                    var aName = a.Type.toLowerCase();
                    var bName = b.Type.toLowerCase();
                    return ((aName < bName) ? -1 : ((aName > bName) ? 1 : 0));
                }

                arrAnswer = arrAnswer.sort(SortByName);
                AddSolutionStep();
            }

            function LoadAnswer(){
                var listAnswer = $('#listAnswer');

                listAnswer.empty();

                for (var i = 0; i < arrAnswer.length; i++) {
                    arrAnswer[i].No = i + 1;
                    if (arrAnswer[i].State != 'Remove') {
                        listAnswer.append($('<li>')
                            .append($('<div class="todoContainer" style="display:table; width:100%;">')
                                .css('cursor', 'pointer')
                                .append($('<div style="float:none; display:table-cell; width:auto; overflow:hidden; padding:8px; word-break:break-word;">')
                                    .data(arrAnswer[i])
                                    .html(arrAnswer[i].Description)
                                    .click(function () {
                                        
                                    })
                                )
                                .append($('<div style="padding:10px; display:table-cell; width:30px; vertical-align:middle;">')
                                    .data(arrAnswer[i])
                                    .append($('<button data-role="none" title="Remove" class="btn custom-btn-theme-c btnCircle-xs btnShadow">')
                                        .append('<img alt="Remove" src="/Content/images/close-black.png" />')
                                    )
                                    .click(function () {
                                        var no = $(this).data().No;

                                        if ($(this).data().State == 'Add') {
                                            arrAnswer = $.grep(arrAnswer, function (e) {
                                                return e.No != no;
                                            })
                                        }
                                        else {
                                            arrAnswer[no - 1].State = 'Remove';
                                        }
                                        GroupAnswerByType();
                                    })
                                )
                            )
                        )
                    }
                }
            }

            function GetKnowledgeManagementNumber() {
                AjaxSend({
                    url: "Survey/GetLastKMNumber",
                    success: function (vData) {
                        if (vData.Results.Rows.length > 0) {
                            $('#txtCaseID').val(vData.Results.Rows[0].LastNumber);
                        } else {
                            $('#txtCaseID').val(0);
                        }
                    }
                });
            }

            function GetKnowledgeBase() {
                var tblKBbody = $('#tblKBbody');

                tblKBbody.empty().append('<tr><td colspan="9" style="text-align:center;">Please wait ...</td></tr>');

                AjaxSend({
                    url: 'Survey/GetAllKnowledgeManagementAndQuestionaire',
                    success: function (vData) {
                        var vRows = vData.Results.Rows;
                        tblKBbody.empty();
                        if (vRows.length > 0) {
                            for (var i = 0; i < vRows.length; i++) {
                                tblKBbody.append($('<tr>')
                                    .data(vRows[i])
                                    .append($('<td>')
                                        .text((i + 1))
                                    )
                                    .append($('<td>')
                                        .text(vRows[i].Problem)
                                    )
                                    .append($('<td>')
                                        .text(vRows[i].Solution)
                                    )
                                    .append($('<td>')
                                        .text(vRows[i].Topic)
                                    )
                                    .append($('<td>')
                                        .text(vRows[i].Product)
                                    )
                                    .append($('<td>')
                                        .text(vRows[i].Language)
                                    )
                                    .append($('<td>')
                                        .text(vRows[i].EditedBy)
                                    )
                                    .append($('<td>')
                                        .text($.format.date(new Date(vRows[i].LastUpdated), 'MMM dd, yyyy'))
                                    )
                                    .append($('<td>')
                                        .data(vRows[i])
                                        .append($('<a style="margin-top:8px;" class="btnHome" data-iconpos="notext" data-theme="b" data-role="button" data-icon="delete" title="Remove">')
                                            .text('Remove')
                                            .button()
                                        )
                                        .click(function (e) {
                                            e.stopPropagation();

                                            var id = $(this).data("ProblemID");
                                            RemoveProblem(id);
                                        })
                                    )
                                    .css('cursor', 'pointer')
                                    .click(function () {
                                        var pDR = $(this).data();
                                        clearKMFields();
                                        GetSolution(pDR);
                                        $('#panAddKnowledgeManagement').show();
                                    })
                                )
                            }
                            //$('a').button('refresh')
                        }
                        else {
                            $('#tblKBbody').append($('<tr>')
                                .append($('<td>')
                                    .css('text-align', 'center')
                                    .attr('colspan', '9')
                                    .text('No Result(s) Found')
                                )
                            );

                            $('#divNoResultsFound').show();
                        }
                        GetAllLanguages();
                        GetAllProducts();
                        GetAllTopics();
                    }
                });
            }

            function RemoveProblem(id) {
                NewMessageBox('Are you sure you want to remove this case? If yes, it will permanently deleted.', 'Warning',
                        [
                            {
                                Text: 'Yes',
                                Click: function () {
                                    AjaxSend({
                                        url: 'Survey/RemoveCase',
                                        data: { ID: id },
                                        success: function (v) {
                                            if (v.Results == "removed") {
                                                WebToast('Successfully Removed');
                                                GetKnowledgeBase();
                                            } else {
                                                NewMessageBox(v);
                                            }
                                        }
                                    })
                                }
                            }, { Text: 'No' }
                        ]
                    )
            }

            function GetSolution(data) {
                AjaxSend({
                    url: 'Survey/GetProblemSolution',
                    data: { ProblemID: data.ProblemID },
                    success: function (vData) {
                        $('#txtProblemDBID').val(data.ProblemID);
                        $('#selTopic').val(data.TopicID).selectmenu('refresh');
                        GetAllSubTopics(data.SubTopicID);
                        $('#selProduct').val(data.ProductID).selectmenu('refresh');
                        $('#selLanguage').val(data.LanguageID).selectmenu('refresh');
                        $('#txtTags').val(data.Tags);
                        $('#txtCaseID').val(data.CaseNo);
                        $('#txtCase').val(data.Problem);
                        arrSolution = (vData || []);
                        DisplaySolution();
                    }
                })
            }

            function DisplayAllowedTeam(e) {
                var listofAllowedTeam = $('#listofAllowedTeam');

                listofAllowedTeam.empty();

                for (var i = 0; i < arrAllowedTeam.length; i++) {
                    arrAllowedTeam[i].No = i + 1;
                    if (arrAllowedTeam[i].State != 'Remove') {
                        listofAllowedTeam.append($('<div class="mobilecontainer">')
                                .attr('id', arrAllowedTeam[i].ID)
                                .data(arrAllowedTeam[i])
                                .append($('<div class="mobilenumber">')
                                    .text(arrAllowedTeam[i].Name)
                                )
                                .append($('<div class="closemobilenumber">')
                                    .data(arrAllowedTeam[i])
                                    .text('x')
                                    .click(function () {
                                        var no = $(this).data().No;

                                        if ($(this).data().State == 'Add') {
                                            arrAllowedTeam = $.grep(arrAllowedTeam, function (e) {
                                                return e.No != no
                                            })
                                        }
                                        else {
                                            arrAllowedTeam[no - 1].State = 'Remove';
                                        }

                                        DisplayAllowedTeam()
                                    })
                                )
                            )
                    }
                }
            }

            function DisplaySolution() {
                var listSolution = $('#listSolution');

                listSolution.empty();

                if (arrSolution != null) {
                    for (var i = 0; i < arrSolution.length; i++) {
                        arrSolution[i].No = i + 1;
                        if (arrSolution[i].State != 'Remove') {
                            listSolution.append($('<li>')
                                .append($('<div class="todoContainer" style="display:table; width:100%;">')
                                    .append($('<div style="float:none; display:table-cell; width:auto; overflow:hidden; padding:8px; word-break:break-word;">')
                                        .data(arrSolution[i])
                                        .html(arrSolution[i].Solution.replace(/\r\n|\r|\n/g, "<br />"))
                                        .click(function () {
                                            console.log($(this).data())
                                            arrAllowedTeam = [];
                                            arrIndexSolution = CDec2($(this).data().No) - 1;
                                            editSolution = true;
                                            AddSolutionStep($(this).data().Step)
                                            arrSolutionSteps = $(this).data().Step;
                                            arrAllowedTeam = $(this).data().AllowedTeam;
                                            DisplayAllowedTeam(arrAllowedTeam);
                                            $('#panAddSolution').show();
                                            $('#txtSolutionAndAnswer').val($(this).data().Solution).focus();
                                            $('input[name="chbPrivate"]').prop('checked', (CBool($(this).data().IsPublic) == true ? false : true)).checkboxradio('refresh');

                                            if (CBool($(this).data().IsPublic) == false) {
                                                $('#divAllowedTeam').show();
                                            }
                                            else {
                                                $('#divAllowedTeam').hide();
                                            }
                                        })
                                    )
                                    .append($('<div style="padding:10px; display:table-cell; width:30px; vertical-align:middle;">')
                                    .data(arrSolution[i])
                                    .append($('<button data-role="none" title="Remove" class="btn custom-btn-theme-c btnCircle-xs btnShadow">')
                                        .append('<img alt="Remove" src="/Content/images/close-black.png" />')
                                    )
                                    .click(function () {
                                        var no = $(this).data().No;

                                        if ($(this).data().State == 'Add') {
                                            arrSolution = $.grep(arrSolution, function (e) {
                                                return e.No != no;
                                            })
                                        }
                                        else {
                                            arrSolution[no - 1].State = 'Remove';
                                        }
                                        DisplaySolution();
                                    })
                                )
                                )
                            )
                        }
                    }
                }
            }

            function NewSteps() {
                if ($.trim($('#txtNewSteps').val()) == '') {
                    $('#txtNewSteps').focus();
                    return;
                }

                if (editSolutionStep == false) {
                    ctrSolutionStep = ctrSolutionStep + 1;

                    arrSolutionSteps.push({
                        StepID: '',
                        No: ctrSolutionStep,
                        Step: $('#txtNewSteps').val(),
                        State: 'Add'
                    })
                }
                else {
                    arrSolutionSteps[arrIndexStep].Step = $('#txtNewSteps').val();
                }


                AddSolutionStep(arrSolutionSteps);
                $('#txtNewSteps').val('');
                editSolutionStep = false;
            }

            function AddSolutionStep(arrSolutionSteps) {
                var listAnswer = $('#listAnswer');

                listAnswer.empty();

                for (var i = 0; i < arrAnswer.length; i++) {
                    arrAnswer[i].No = i + 1;
                    ctrSolutionStep = i + 1;
                    if (arrAnswer[i].State != 'Remove') {
                        listAnswer.append($('<li>')
                            .append($('<div class="todoContainer" style="display:table; width:100%;">')
                                .css('cursor', 'pointer')
                                .append($('<div style="float:none; display:table-cell; width:auto; overflow:hidden; padding:8px; word-break:break-word;">')
                                    .data(arrAnswer[i])
                                    .html(arrAnswer[i].Description.replace(/\r\n|\r|\n/g, "<br />")
                                    )
                                    .click(function () {
                                        editSolutionStep = true;
                                        $('#panAddSteps').popup('open');
                                        $('#txtAnswerDescription').val($(this).data().Description).focus();
                                        $('#selControlType').val($(this).data().Type).selectmenu('refresh');
                                        arrIndexStep = CDec2($(this).data().No) - 1;
                                    })
                                )
                                .append($('<div style="padding:10px; display:table-cell; width:30px; vertical-align:middle;">')
                                    .data(arrAnswer[i])
                                    .append($('<button data-role="none" title="Remove" class="btn custom-btn-theme-c btnCircle-xs btnShadow">')
                                        .append('<img alt="Remove" src="/Content/images/close-black.png" />')
                                    )
                                    .click(function () {
                                        var no = $(this).data().No;

                                        if ($(this).data().State == 'Add') {
                                            arrAnswer = $.grep(arrAnswer, function (e) {
                                                return e.No != no;
                                            })
                                        }
                                        else {
                                            arrAnswer[no - 1].State = 'Remove';
                                        }
                                        GroupAnswerByType();
                                    })
                                )
                            )
                        )
                    }
                }
            }

            function SaveObject() {
                objID = NewID();

                var pData = {};
                pData.ID = objID;
                pData.Description = $('#txtObject').val();
                pData.TopicID = $('#selTopic').val();

                var lnk = '';

                if (objSelected == 'Topic') {
                    lnk = 'SaveTopic';
                } else if (objSelected == 'Product') {
                    lnk = 'SaveProduct';
                }
                else if (objSelected == 'Language') {
                    lnk = 'SaveLanguage';
                } else if (objSelected == 'SubTopic') {
                    lnk = 'SaveSubTopic';
                }

                AjaxSend({
                    url: 'Survey/' + lnk,
                    data: pData,
                    success: function (vData) {

                        if ((vData.Results || '') == "True") {
                            if (objSelected == 'Topic') {
                                GetAllTopics();
                            } else if (objSelected == 'Product') {
                                GetAllProducts();
                            }
                            else if (objSelected == 'Language') {
                                GetAllLanguages();
                            } else if (objSelected == 'SubTopic') {
                                GetAllSubTopics();
                            }

                            $('#panAddObject').popup('close');
                        }
                        else {
                            MessageBox('An error occured while saving Case. Please contact Mobiz ASAP. Thank you.')
                        }
                    }
                });
            }

            function SaveCase() {
                var pData = {};
                pData.CaseID = $('#txtCaseID').val();
                pData.Case = $('#txtCase').val();
                //pData.Solution = $('#txtSolution').val();
                //pData.Steps = $('#txtSteps').val();
                pData.Topic = $('#selTopic').val();
                pData.SubTopic = $('#selSubTopic').val();
                pData.Product = $('#selProduct').val();
                pData.Language = $('#selLanguage').val();
                pData.Tags = $('#txtTags').val();
                pData.Solution = arrSolution;
                pData.ProblemID = $('#txtProblemDBID').val();

                AjaxSend({
                    url: 'Survey/SaveCase',
                    data: pData,
                    success: function (vData) {
                        if ((vData.Results || '') == "True") {
                            $('#panAddKnowledgeManagement').hide();
                            editSolution = false;
                            GetKnowledgeBase();
                        }
                        else {
                            MessageBox('An error occured while saving Case. Please contact Mobiz ASAP. Thank you.')
                        }

                    }
                });
            }

            var objSelected = '';
            var objID = '';

            function GetAllTopics() {
                AjaxSend({
                    url: 'Survey/GetAllTopics',
                    success: function (vData) {
                        var vRows = vData.Results.Rows;

                        if (vRows.length > 0) {
                            $('#selTopic').empty();

                            for (var i = 0; i <= vRows.length - 1; i++) {
                                $('#selTopic').append(new Option(vRows[i].Topic, vRows[i].ID, false));
                            }

                            if (objID != '')
                                $('#selTopic').val(objID);
                            else
                                $('#selTopic').val(vRows[0].ID);
                        }

                        $('#selTopic').append(new Option('Add New Topic', 'AddNew', false));

                        try {
                            $('#selTopic').selectmenu('refresh');
                        } catch (e) {
                            $('#selTopic').selectmenu();
                        }

                        if ($('#selTopic').val() != "AddNew" && $('#selTopic').val() != "") {
                            GetAllSubTopics()
                        }
                    }
                });
            }

            function GetAllSubTopics(id) {
                var data = { TopicID: $('#selTopic option:selected').val() }
                AjaxSend({
                    url: 'Survey/GetAllSubTopics',
                    data: data,
                    success: function (vData) {
                        var vRows = vData.Results.Rows;

                        $('#selSubTopic').empty();

                        if (vRows.length > 0) {
                            for (var i = 0; i <= vRows.length - 1; i++) {
                                $('#selSubTopic').append(new Option(vRows[i].SubTopic, vRows[i].ID, false));
                            }

                            if (id != undefined) {
                                $('#selSubTopic').val(id);
                            }
                            else {
                                if (objID != '')
                                    $('#selSubTopic').val(objID);
                                else
                                    $('#selSubTopic').val(vRows[0].ID);
                            }

                        } else
                            $('#selSubTopic').append(new Option('Select Topic', '', false));

                        $('#selSubTopic').append(new Option('Add New SubTopic', 'AddNew', false));

                        try {
                            $('#selSubTopic').selectmenu('refresh');
                        } catch (e) {
                            $('#selSubTopic').selectmenu();
                        }
                    }
                });
            }

            function GetAllProducts() {
                AjaxSend({
                    url: 'Survey/GetAllProducts',
                    success: function (vData) {
                        var vRows = vData.Results.Rows;

                        if (vRows.length > 0) {
                            $('#selProduct').empty();

                            for (var i = 0; i <= vRows.length - 1; i++) {
                                $('#selProduct').append(new Option(vRows[i].Product, vRows[i].ID, false));
                            }

                            if (objID != '')
                                $('#selProduct').val(objID);
                            else
                                $('#selProduct').val(vRows[0].ID);
                        }

                        $('#selProduct').append(new Option('Add New Product', 'AddNew', false));

                        try {
                            $('#selProduct').selectmenu('refresh');
                        } catch (e) {
                            $('#selProduct').selectmenu();
                        }

                    }
                });
            }

            function GetAllLanguages() {
                AjaxSend({
                    url: 'Survey/GetAllLanguages',
                    success: function (vData) {
                        var vRows = vData.Results.Rows;

                        if (vRows.length > 0) {
                            $('#selLanguage').empty();

                            for (var i = 0; i <= vRows.length - 1; i++) {
                                $('#selLanguage').append(new Option(vRows[i].Language, vRows[i].ID, false));
                            }

                            if (objID != '')
                                $('#selLanguage').val(objID);
                            else
                                $('#selLanguage').val(vRows[0].ID);
                        }

                        $('#selLanguage').append(new Option('Add New Language', 'AddNew', false));

                        try {
                            $('#selLanguage').selectmenu('refresh');
                        } catch (e) {
                            $('#selLanguage').selectmenu();
                        }
                    }
                });
            }

            function clearKMFields() {
                $('#txtProblemDBID').val('');
                $('#txtCaseDBID').val('');
                $('#txtCase').val('');
                $('#txtSolution').val('');
                $('#txtSteps').val('');
                $('#selTopic').val('');
                $('#txtTags').val('');
                $('#txtSearch').val('');
                $('#listSolution').empty();
                arrSolution = [];
                arrSolutionSteps = [];
            }

            function ClearSolutionField() {
                $('#txtSolutionAndAnswer').val('');
                $('#listSolutionSteps').empty();
            }

            function LoadRequiredData() {
                $('#divMainSurveyCreatorPage').pageshow();
                GetKnowledgeBase();
                ClearFields();
            }

            function ClearFields() {
                
            }
        }).trigger('popup')
    }
)