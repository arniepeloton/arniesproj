require(['common', '/Scripts/sources/Survey/CreatorDesigns.js?v=' + assetversion]
, function () { //'/Content/bootstrap/js/bootstrap.js'
    require(['/Scripts/jquery-ui-sortable.js'], function () {
        $(document).ready(function () {
            var selQuestionType = $('#selQuestionType');
            var listQuestions = $('#listQuestions');
            var qType = selQuestionType.val(), arrSurveyPage = [], ctrPage = 0, ctrPageIndex = 0, arrQuestions = [], questionaireID = '', titleState = 'new';
            var dtSurvey, IsUseCheckbox = false, curPageArr, arrReadOnlyQuestion = [], reArrangePageID;
            var arrListOfAllQuestions = [], arrTunnelQuestion = [], arrResponse = [], arrPageQuestionsOrdering = [];
            var arrReqGroup = [], editReqID, reqState;

            $('#divDescription').summernote({
                placeholder: 'Write someting here ...',
                minHeight: '300',
                toolbar: [
                    ['style', ['bold', 'italic', 'underline', 'clear']],
                    ['fontsize', ['fontsize']],
                    ['color', ['color']],
                    ['insert', ['link']],
                    ['para', ['paragraph']],
                    ['table', ['table']]
                ],
                dialogsInBody: true
            });

            $('[data-toggle="tooltip"]').tooltip()

            $('#listrearrangequestions').sortable({
                delay: 150,
                update: function (e, ui) {
                    var orderno = 1;
                    arrPageQuestionsOrdering = [];
                    $('#listrearrangequestions li').each(function () {
                        var data = $(this).data('data');
                        data.OrderNo = orderno;
                        reArrangePageID = data.PageID;
                        arrPageQuestionsOrdering.push(data);
                        orderno += 1;
                    })
                }
            })

            $('#divReqGroup').sortable({
                delay: 150,
                update: function (e, ui) {
                    var orderno = 1;
                    $('#divReqGroup > div').each(function () {
                        $(this).data('data').OrderNo = orderno;
                        orderno += 1;
                    })
                }
            })

            GetSurvey();

            $('body')
            .on('click', '#btnSurveyUrl', function () {
                CopyToClipboard('#txtSurveyUrl');
            })
            .on('click', '#btnSurveyAdd', function () {
                ClearPages();
                $('#popQuestionaire').modal('show');                
                $('#txtQuestionaireTitle').val('').focus();
                $('#listQuestions').empty();
                $('#tabSurveyDesign').opentabmenu();
                $('#divTabSurvey, #divSurveyUrl').hide();
                $('#divTabSurvey').addClass('hidetab');
                AddSurveyPage();
                //Creator.QuestionaireReq = [];
            })
            .on('click', '#btnSurveyAddPage', function () {
                AddSurveyPage();
                WebToast('New page has been added');
            })
            .on('change', '#selQuestionType', function () {
                qType = selQuestionType.val();
                AddNewQuestion();
            })
            .on('blur', '#txtQuestion', function () {
                if($.trim($(this).val()) != ""){
                    $(this).parent().removeClass('has-error');
                } else {
                    $(this).parent().addClass('has-error');
                }                
            })
            .on('click', '#btnOkQuestion', function () {
                var isOkay;
                qType = selQuestionType.val();
                if (qType == 'Multiple Inputs') {
                    isOkay = Creator.MultipleInputs.OnComplete();                    
                } else if (qType == 'Single Input') {
                    isOkay = Creator.SingleInput.OnComplete();
                } else if (qType == 'radio' || qType == 'checkbox' || qType == 'radioinput' || qType == 'checkboxinput') {
                    isOkay = Creator.CheckboxRadio.OnComplete();
                } else if (qType == 'Matrix') {
                    isOkay = Creator.Matrix.OnComplete();
                } else if(qType == 'Page Text'){
                    isOkay = Creator.PageText.OnComplete();
                } else if(qType == 'file'){
                    isOkay = Creator.File.OnComplete();
                }

                if (isOkay != undefined) {
                    if(isOkay.EditQuestion){
                        arrQuestions[CInt2(isOkay.Index)] = isOkay;
                    } else {
                        arrQuestions.push(isOkay);                        
                    }                    
                    $('#popNewQuestion').modal('hide');
                    DisplayQuestions();
                    PopulateSurveyQuestions();
                }                
            })
            .on('click', '#btnSurveyCancelQuestion', function () {
                $('#pageListOfSurvey').pageshow();
            })
            .on('click', '#btnSurveySaveQuestion', function () {
                SaveQuestionaire();
            })
            .on('click', '#btnSurveyRefresh', function () {
                GetSurvey();
            })
            .on('click', '#btnOkRearrange', function () {
                var pageIndex;
                $.grep(arrSurveyPage, function (e, i) {
                    if(e.ID == reArrangePageID){
                        pageIndex = i;
                        return false; //stop
                    }
                })
                curPageArr = {}
                curPageArr.ID = reArrangePageID;
                arrQuestions = arrPageQuestionsOrdering;
                arrSurveyPage[pageIndex].Questions = arrQuestions;
                DisplayQuestions();
                $('#popRearrangeQuestions').modal('hide');
            })
            .on('click', '#btnSurveyAddRequirement', function () {
                $('#popRequirement').modal('show');
                ClearRequirementsFields();
                reqState = 'new';
            })
            .on('change', '#selReqType', function () {
                if ($(this).val() == "1") {
                    $('#divReqFile').show();
                } else {
                    $('#divReqFile').hide();
                }
            })
            .on('click', '#btnOkCloseRequirement', function () {
                AddSurveyRequirement();
            })
            .on('click', '#btnOkAddRequirement', function () {
                if (AddSurveyRequirement()) {
                    $('#btnSurveyAddRequirement').click();
                }
            })
            .on('keydown', '#txtRequirementQty', function (e) {
                if(!NumberOnly(e)){
                    return
                }
            })

            .on('click', '#btnDownloadMrt', function () {
                if ($.trim($('.selRespondent').val()) == '') {
                    WebToast('Please select respondent', 'top');
                    return;
                }

                AjaxSend({
                    url: '/Survey/GenerateMrt',
                    data: {
                        ClientID: $.trim($('.selRespondent').val()),
                        MrtFile: 'BuildingPermit',
                        QID: questionaireID
                    },
                    success: function (e) {
                        var pUrl = MergeUrlParameters({
                            Filename: e.Filename
                        }, '/Survey/GetSampleMrt');
                        window.open(pUrl, '_blank',
                            'width=' + window.innerWidth +
                            ',height=' + window.innerHeight +
                            ',scrollbars=auto');
                    }
                })
            })
            .on('keydown', '#txtReqGroupName', function (e) {
                if (e.which == 13) {
                    $('#btnReqGroup').click();
                }                    
            })
            .on('click', '#btnReqGroup', function () {
                var val = $.trim($('#txtReqGroupName').val());

                if (val != '') {
                    var data = {
                        ID: NewID(),
                        Name: val,
                        State: 'new',
                        OrderNo: 0
                    }
                    arrReqGroup.push(data)
                    DisplayReqGroup(data);
                    $('#txtReqGroupName').val('');
                }
            })
            .on('click', '#btnZip', function () {
                window.location = '/Survey/ZipFiles';
            })
            .on('click', '#btnSurveyPreview', function () {
                window.open($('#txtSurveyUrl').val() + '&mode=preview', '_blank');
            })

            function DisplayReqGroup(data) {
                $('#divReqGroup').append($('<div class="form-check" id="divReqGroup-' + data.ID + '">')
                        .data('data', data)
                        .append($('<label class="form-check-label">')                            
                            .append($('<input type="radio" name="chbReqGroup" value="' + data.ID + '" class="chbReqGroup form-check-input" data-role="none">')
                                .data('data', data)
                            )
                            .append($('<div style="display:flex; word-break: break-word;">')
                                .append($('<span>').text(data.Name))
                                //.append('&nbsp; &nbsp;')
                                .append($('<a data-role="none" class="btnGroupEdit-' + data.ID + ' btnOptionReqGroup" title="edit" class="custom-link" style="color:#ff8d00;">').append($('<span class="bi bi-pencil">'))
                                    .data('data', data).click(function () {
                                        var span = $(this).parent().find('> span');
                                        sessionStorage.IsEditReqGroup = true;
                                        $(this).parent().prepend($('<input data-role="none" class="form-control" value="' + span.text() + '">')
                                            .focus()
                                            .blur(function () {
                                                $(this).nextAll('.btnGroupRename').click();
                                            })
                                            .keydown(function (e) {
                                                if (e.which == 13) {
                                                    $(this).nextAll('.btnGroupRename').click();
                                                }
                                            })
                                        );
                                        span.remove();
                                        $('.btnGroupEdit-' + data.ID).hide();
                                        $('.btnGroupRename-' + data.ID).show();
                                    })
                                )
                                .append($('<a data-role="none" class="btnGroupRename-' + data.ID + ' btnOptionReqGroup" title="rename" class="custom-link" style="color:green; display:none;">').append($('<span class="bi bi-check-lg">'))
                                    .data('data', data).click(function () {
                                        var data = $(this).data('data');
                                        var input = $(this).parent().find('> input'), ind;
                                        sessionStorage.IsEditReqGroup = false;
                                        $(this).parent().prepend($('<span>').text($.trim(input.val())));
                                        input.remove();
                                        $('.btnGroupEdit-' + data.ID).show();
                                        $('.btnGroupRename-' + data.ID).hide();

                                        $.grep(arrReqGroup, function (e, i) {
                                            if (e.ID == data.ID) {
                                                ind = i;
                                                return false; // stop
                                            }
                                        })

                                        arrReqGroup[ind].Name = $.trim(input.val());
                                    })
                                )
                                .append('&nbsp;')
                                .append($('<a data-role="none" class="custom-link btnOptionReqGroup" style="color:#ff5050;">').append($('<span class="bi bi-trash">'))
                                    .data('data', data)
                                    .click(function () {
                                        var data = $(this).data('data');
                                        if (CBool(sessionStorage.IsEditReqGroup)) {
                                            var input = $(this).parent().find('> input')
                                            $(this).parent().prepend($('<span>').text($.trim(input.val())));
                                            input.remove();
                                            $('.btnGroupEdit-' + data.ID).show();
                                            $('.btnGroupRename-' + data.ID).hide();
                                        } else {
                                            NewMessageBox('Are you sure you want to remove this record?', 'Message', [
                                                {
                                                    Text: 'Yes', Click: function () {
                                                        var ind;

                                                        $.grep(arrReqGroup, function (e, i) {
                                                            if (e.ID == data.ID) {
                                                                ind = i;
                                                                return false; // stop
                                                            }
                                                        })

                                                        if (data.State == 'new') {
                                                            (arrReqGroup).splice(ind, 1)
                                                        } else {
                                                            arrReqGroup[ind].State = 'remove';
                                                        }

                                                        $('#divReqGroup-' + data.ID).remove();
                                                    }
                                                }, { Text: 'No' }
                                            ])
                                        }
                                    })
                                )
                            )
                        )
                    )

                $('.chbReqGroup');//.checkboxradio();//.checkboxradio('refresh')
            }

            function PopulateReqGroup() {
                var divReqGroup = $('#divReqGroup');

                divReqGroup.empty();

                for (var i = 0; i < arrReqGroup.length; i++){
                    DisplayReqGroup(arrReqGroup[i])
                }
            }

            function AddSurveyRequirement() {
                var type = $('#selReqType').val(), ind = null;
                var txtRequirementName = $.trim($('#txtRequirementName').val()),
                    txtRequirementDescription = $.trim($('#txtRequirementDescription').val()),
                    txtRequirementQty = $.trim($('#txtRequirementQty').val()),
                    files = $('#fileReqMrt').get(0).files,
                    chbIsUniversal = $('#chbIsUniversal').is(':checked'),
                    ReqGroupID = $('.chbReqGroup:checked').val(),
                    groupname = $('.chbReqGroup:checked').data('data').Name;

                if(txtRequirementName == ''){
                    WebToast('Please enter name', 'top');
                    return false;
                }

                if (CInt2(txtRequirementQty) == 0) {
                    WebToast('Please enter number of copies', 'top');
                    return false;
                }

                if (CBool(type) && files.length == 0 && CStr2(editReqID) == '') { // not required for edit
                    WebToast('Please select MRT file', 'top');
                    return false;
                }

                if (CStr2(ReqGroupID) == '') {
                    WebToast('Please select group', 'top');
                    return false;
                }

                if (reqState == 'new') {
                    var data = {
                        ID: NewID(),
                        IsSysGen: type,
                        Name: txtRequirementName,
                        NoOfCopies: txtRequirementQty,
                        State: reqState,
                        Files: (CBool(type) ? [files[0]] : []),
                        IsUniversal: chbIsUniversal,
                        ReqGroupID: ReqGroupID,
                        RequiredBy: groupname
                    }

                    Creator.QuestionaireReq.push(data)
                } else {
                    $.grep(Creator.QuestionaireReq, function (e, i) {
                        if (e.ID == editReqID) {
                            ind = i;
                            return false; // stop
                        }
                    })
                    editReqID = '';
                    Creator.QuestionaireReq[ind].IsSysGen = type;
                    Creator.QuestionaireReq[ind].Name = txtRequirementName;
                    Creator.QuestionaireReq[ind].NoOfCopies = txtRequirementQty;
                    Creator.QuestionaireReq[ind].IsUniversal = chbIsUniversal;
                    Creator.QuestionaireReq[ind].ReqGroupID = ReqGroupID;
                    Creator.QuestionaireReq[ind].RequiredBy = groupname;
                    Creator.QuestionaireReq[ind].Files = (CBool(type) ? [files[0]] : []);
                }

                DisplaySurveyRequirement();
                $('#popRequirement').modal('hide');

                return true;
            }

            var dtSurveyReq;
            function DisplaySurveyRequirement() {
                var tblDataTableSurveyRequirement = $('#tblDataTableSurveyRequirement'),
                    ctr = 0;

                if ($.fn.DataTable.isDataTable(tblDataTableSurveyRequirement)) {
                    //empty the table first before destroying datatabase para dili magdoble ang header
                    tblDataTableSurveyRequirement.empty();
                    dtSurveyReq.destroy();
                }

                dtSurveyReq = tblDataTableSurveyRequirement.DataTable({
                    "data": Creator.QuestionaireReq,
                    "pageLength": 50,
                    "columns": [
                        {
                            "data": function (data, type, full, meta) {
                                ctr += 1;
                                return '<b class="ui-table-cell-label">#</b>' + ctr;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Name:</b>' + data.Name;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">No. of Copies</b>' + data.NoOfCopies;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Universal Req.:</b>' + (CBool(data.IsUniversal) ? 'Yes' : 'No')
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Required by:</b>' + data.RequiredBy
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Last Updated:</b>' + $.format.date(new Date(data.ReqLastUpdated), 'MMM dd, yyyy hh:mm a');
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Action:</b> <button data-role="none" class="btn custom-btn-theme-b btnCircle-xs" title="Remove"><span class="bi bi-remove"></span></button>'
                            }
                        }
                    ],
                    //"order": [[1, "asc"]],
                    "aoColumnDefs": [{ "bSortable": false, "aTargets": [0, 2, 3, 5] }],
                    "responsive": true,
                    "columnDefs": [{ "targets": 1, "orderable": false }],
                    "sDom": '<"top">rt<"bottom"ip><"clear">',
                    initComplete: function () {
                        tblDataTableSurveyRequirement.css('width', '100%').removeClass('dataTable');
                    }
                    //"stateSave": true
                })

                dtSurveyReq.on('click', 'td', function () {
                    if (dtSurveyReq.row(this).data() != undefined) {
                        var dis = this;
                        var data = dtSurveyReq.row(dis).data();
                        if (dtSurveyReq.cell(dis).index().columnVisible == 5) {
                            var ind = null;

                            NewMessageBox('Are you sure you want to remove this record?', 'Message', [
                                {
                                    Text: 'Yes', Click: function () {
                                        $.grep(Creator.QuestionaireReq, function (e, i) {
                                            if (e.ID == data.ID) {
                                                ind = i;
                                                return false; // stop
                                            }
                                        })

                                        if (data.State == 'new') {
                                            (Creator.QuestionaireReq).splice(ind, 1);
                                        } else {
                                            Creator.QuestionaireReq[ind].State = 'remove';
                                        }
                                        dtSurveyReq.row($(dis).parents('tr')).remove().draw();
                                        //DisplaySurveyRequirement();                                        
                                    }
                                }, { Text: 'No' }
                            ])
                        } else {
                            $('#popRequirement').modal('show');
                            ClearRequirementsFields();
                            editReqID = data.ID;
                            reqState = 'edit';
                            $('#selReqType').val(CInt2(CBool(data.IsSysGen)));
                            $('#txtRequirementName').val(data.Name).focus();
                            $('#txtRequirementQty').val(data.NoOfCopies);
                            $('.chbReqGroup[value="' + data.ReqGroupID + '"]').prop('checked', true);//.checkboxradio('refresh');

                            if (CBool(data.IsUniversal)) {
                                $('#chbIsUniversal').prop('checked', true);//.checkboxradio('refresh');
                            }

                            if (CBool(data.IsSysGen)) {
                                $('#reqMrtFile').hide();
                                $('#divReqFile').show();
                            } else {
                                $('#divReqFile').hide();
                            }
                        }
                    }
                });
            }

            function ClearRequirementsFields() {
                $('#selReqType').val('1');
                $('#txtRequirementName').val('').focus();
                $('#txtRequirementQty').val('');
                $('#fileReqMrt').val('');
                $('#divReqFile, #reqMrtFile').show();
                $('#chbIsUniversal').removeAttr('checked');//.checkboxradio('refresh');
                $('.chbReqGroup').removeAttr('checked');//.checkboxradio('refresh');
            }

            function GetSurvey() {
                var url = '/Survey/GetSurvey?OrgID=' + sessionStorage.OrgID + '&mod=' + (sessionStorage.SystemAdminFor || 'crm');
                if ($.fn.DataTable.isDataTable('#tblDataTableSurvey')) {
                    dtSurvey = $('#tblDataTableSurvey').DataTable().ajax.url(url).load(function (json) {
                        OnCompleteSurvey(json);
                    });
                    return;
                }

                dtSurvey = $('#tblDataTableSurvey').DataTable({
                    retrieve: true,
                    "order": [[1, "asc"]],
                    "pageLength": 10,
                    "bProcessing": true,
                    "bServerSide": true,
                    "ajax": {
                        url: url,
                        type: 'POST',
                        data: function (d) {
                            d.Status = $('#selStatusSurvey option:selected').val();
                        },
                        error: function (e) {
                            NewMessageBox('ERROR: ' + e.responseText, 'Message')
                        }
                    },
                    "autoWidth": false,
                    "columns": [
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">#</b>' + data.RowNum;
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Questionaire</b>' + data.Title;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Responses</b>' + data.Respondents;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Modified By</b>' + data.CreatedByName;
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Modified Date</b>' + $.format.date(new Date(data.LastUpdated), 'MMM dd, yyyy hh:mm a');
                            }
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Status</b>' + (CBool(data.IsActive) ? 'Active' : 'Inactive');
                            }
                        }
                    ],
                    "aoColumnDefs": [{ "bSortable": false, "aTargets": [4] }],
                    "responsive": true,
                    "columnDefs": [{ "targets": 0, "orderable": true }],
                    "sDom": '<"top">rt<"bottom"ip><"clear">',
                    "initComplete": function (settings, json) {
                        OnCompleteSurvey(json);
                        $('#tblDataTableSurvey').removeClass('dataTable');
                    }
                    //"stateSave": true
                })

                dtSurvey.on('click', 'td', function () {
                    if (dtSurvey.row(this).data() != undefined) {
                        var data = JSON.parse(JSON.stringify(dtSurvey.row(this).data()));
                        ClearPages();
                        arrSurveyPage = data.Pages;
                        arrReqGroup = data.ReqGroups;
                        questionaireID = data.ID;
                        titleState = 'edit';
                        ctrPage = arrSurveyPage.length;
                        ctrPageIndex = arrSurveyPage.length;
                        arrReadOnlyQuestion = [];
                        arrListOfAllQuestions = [];
                        Creator.QuestionaireReq = data.Requirements;
                        DisplaySurveyRequirement();
                        PopulateReqGroup();
                        $('#popQuestionaire').modal('show');                        
                        $('#txtQuestionaireTitle').val(data.Title);
                        $('#selSurveyStatus').val((CBool(data.IsActive) ? '1' : '0'));//.slider('refresh');
                        $('#tabSurveyDesign').opentabmenu();
                        $('#divTabSurvey, #divSurveyUrl, #btnSurveyStatus').show();
                        $('#divTabSurvey').removeClass('hidetab');
                        $('#txtSurveyUrl').val(location.origin + '/Survey/Questionaire?q=' + data.Link);
                        
                        for (var p = 0; p < arrSurveyPage.length; p++) {
                            curPageArr = arrSurveyPage[p];
                            arrQuestions = curPageArr.Questions;
                            arrReadOnlyQuestion.push(arrQuestions);
                            DisplayPage([curPageArr]);
                            DisplayQuestions();
                            $('#prevLabel-' + curPageArr.ID).val($.trim(CStr2(curPageArr.PrevLabel)));
                            $('#prevAction-' + curPageArr.ID).val($.trim(CStr2(curPageArr.PrevAction)));
                            $('#prevGotoPage-' + curPageArr.ID).val($.trim(CStr2(curPageArr.PrevGoto)));
                            ShowListPages('prev', curPageArr);
                            $('#nextLabel-' + curPageArr.ID).val($.trim(CStr2(curPageArr.NextLabel)));
                            $('#nextAction-' + curPageArr.ID).val($.trim(CStr2(curPageArr.NextAction)));
                            $('#nextGotoPage-' + curPageArr.ID).val($.trim(CStr2(curPageArr.NextGoto)));
                            $('#nextGotoLink-' + curPageArr.ID).val($.trim(CStr2(curPageArr.NextGotoLink)));
                            ShowListPages('next', curPageArr);
                        }
                        $('#' + arrSurveyPage[0].ID).opentabmenu();
                        //RemoveSpecialNextAction();
                        //AddSpecialNextAction();
                        GetListOfRespondents();
                        DisplayReadOnlyQuestion();
                        PopulateSurveyQuestions();
                    }
                });

                $("#txtSearchSurvey").unbind().keypress(function (e) {
                    if (e.keyCode == 13) {
                        dtSurvey.search($.trim($(this).val())).draw();
                    }
                });

                $('#selStatusSurvey').unbind('change').change(function () {
                    dtSurvey.columns(0)
                    .search($(this).val())
                    .draw()
                })

                function OnCompleteSurvey(data) {
                    if (typeof DisplayFilterSurvey != "undefined") {
                        DisplayFilterSurvey(data.data) // CRMDashboard.js
                    }                    
                }
            }

            function PopulateSurveyQuestions() {
                var select = $('<select data-role="none" class="form-control selTunnel">');
                var q = arrListOfAllQuestions;

                select.append(new Option("None", ""));

                for (var i = 0; i < q.length; i++){
                    //if ((q[i].QuestionType == 'checkbox' || q[i].QuestionType == 'radio'
                    //        || q[i].QuestionType == 'checkboxinput' || q[i].QuestionType == 'radioinput')
                    //        && select.find('[value="' + (q[i].ID || q[i].TempID) + '"]').length == 0) {
                    //    select.append(new Option(q[i].Question, (q[i].ID || q[i].TempID)));
                    //}

                    if (q[i].QuestionType != 'Page Text' && select.find('[value="' + (q[i].ID || q[i].TempID) + '"]').length == 0) {
                        select.append(new Option(q[i].Question, (q[i].ID || q[i].TempID)));
                    }
                }

                Creator.SurveyAllQuestions = select;
            }

            function DisplayReadOnlyQuestion() {
                var listQuestionsReadOnly = $('#listQuestionsReadOnly');
                listQuestionsReadOnly.empty();

                for (var q = 0; q < arrReadOnlyQuestion.length; q++){
                    for (var i = 0; i < arrReadOnlyQuestion[q].length; i++) {
                        if (arrReadOnlyQuestion[q][i].TunnelCount == 0) {
                            if (arrReadOnlyQuestion[q][i].QuestionType != 'Page Text') {
                                if (arrReadOnlyQuestion[q][i].QuestionType != 'Matrix') {
                                    listQuestionsReadOnly.append($('<li>')
                                        .attr('liqid', arrReadOnlyQuestion[q][i].ID)
                                        .append($('<div>')
                                            .css('font-weight', 'bold')
                                            .html(arrReadOnlyQuestion[q][i].Question)
                                        )
                                        .append($('<div qid="' + arrReadOnlyQuestion[q][i].ID + '" class="divResponse" style="margin-left:20px;">'))
                                    )
                                } else {
                                    IsUseCheckbox = arrReadOnlyQuestion[q][i].IsUseCheckbox;
                                    listQuestionsReadOnly.append($('<li>')
                                        .attr('liqid', arrReadOnlyQuestion[q][i].ID)
                                        .append($('<div>')
                                            .css('font-weight', 'bold')
                                            .text(arrReadOnlyQuestion[q][i].Question)
                                        )
                                        .append('<br>')
                                        .append($('<div style="overflow:auto;">')
                                            .append(DisplayMatrix(arrReadOnlyQuestion[q][i], true))
                                        )
                                    )
                                }
                            }
                        } else {
                            arrTunnelQuestion.push(arrReadOnlyQuestion[q][i]);
                        }
                    }
                }
            }

            function GetListOfRespondents() {
                var divRespondents = $('#divRespondents');
                divRespondents.empty();
                AjaxSend({
                    url: '/Survey/GetListOfRespondents',
                    data: { QID: questionaireID },
                    success: function (e) {
                        var vRows = e.Results.Rows;
                        if (vRows.length > 0) {
                            var sel = $('<select data-role="none" class="form-select fixed selRespondent">')
                            sel.append(new Option('Please select', ''))
                            for (var i = 0; i < vRows.length; i++){
                                sel.append(new Option((vRows[i].ClientID == undefined ? 'Respondent No. ' + (i + 1) : vRows[i].ClientName), vRows[i].ID))
                            }

                            divRespondents.append($('<div class="row align-items-center">')
                                    .append($('<div class="col-md-auto">')
                                        .text('Respondent: ')
                                        
                                        .append('&nbsp; &nbsp;')
                                        
                                    )
                                    .append($('<div class="col-md-auto">').append(sel))
                                    .append($('<div class="col-md-auto">').append($('<button data-role="none" class="btn btn-secondary">')
                                                .text('Download Requirement(s)')
                                                .click(function () {
                                                    var respondentID = $('.selRespondent').val();

                                                    if (CStr2(respondentID) == '') {
                                                        WebToast('Please select respondent', 'top');
                                                        return;
                                                    }
                                                    NewMessageBox('Please wait ...', 'Message');

                                                    AjaxSend({
                                                        url: '/Survey/CheckIfReqExist?QID=' + questionaireID + '&OrgID=' + sessionStorage.OrgID + '&RespondentID=' + respondentID,
                                                        success: function (e) {
                                                            if (e.Results == 1) {
                                                                NewMessageBox('No requirements available', 'Message');
                                                            } else {
                                                                window.location.href = '/Survey/ZipFiles?QID=' + questionaireID + '&OrgID=' + sessionStorage.OrgID + '&RespondentID=' + respondentID;
                                                                NewMessageBoxClose();
                                                            }
                                                        }
                                                    })
                                                })
                                            )
                                        )
                                );
                            sel.change(function (e) {
                                $('.divResponse').empty();
                                if ($(this).val() != "") {
                                    GetResponse($(this).val());
                                }
                            })
                        } else {
                            divRespondents.append('<h3>No Respondents</h3>')
                        }
                    }
                })
            }

            function GetResponse(respondentID) {
                AjaxSend({
                    url: '/Survey/GetResponse',
                    data: {QID: questionaireID, RID: respondentID},
                    success: function (e) {
                        var vRows = e.data;
                        arrResponse = e.data;
                        $('.tdresponse').empty();

                        for (var i = 0; i < vRows.length; i++) {
                            DisplayResponse(vRows[i], false);
                        }
                    }
                })
            }

            function DisplayResponse(vRows, isFromTunnel) {
                if (vRows.Type != 'Matrix') {
                    var divAns = $('div[qid="' + vRows.QuestionID + '"]');

                    divAns.append((divAns.text().length == 0 ? '<br>' : '<br><br>'))

                    if(vRows.Type == 'file'){
                        divAns.append($('<a data-role="none" class="custom-link">')
                                .data('data', vRows)
                                .attr('href', MergeUrlParameters({
                                    ClientID: vRows.ClientID,
                                    FileName: vRows.Description,
                                    DownloadFileName: vRows.Answer
                                }, '/Survey/Download'))
                                .append('<span class="bi bi-download"></span> &nbsp;' + vRows.Answer)
                            )
                    } else {
                        divAns.append((vRows.Type == "Multiple Inputs" ? vRows.Label + ': ' : '- ') + vRows.Answer + (CStr2(vRows.Description) != "" ? ' : ' + CStr2(vRows.Description) : ''))
                    }

                    if (CStr2(vRows.Tunnel) != "") {
                        var tunnelQ, answer;

                        $.grep(arrTunnelQuestion, function (e) {
                            if (e.ID == CStr2(vRows.Tunnel)) {
                                tunnelQ = e;
                                return false;
                            }
                        })

                        if(isFromTunnel){
                            $.grep(arrResponse, function (e) {
                                if (e.QuestionID == CStr2(tunnelQ.ID)) {
                                    answer = e;
                                    return false;
                                }
                            })
                        }                        

                        $('[qid="' + vRows.QuestionID + '"]').append('<br><br>')
                            .append($('<ul>')
                                .append($('<li>')
                                    .attr('liqid', tunnelQ.ID)
                                    .append($('<div style="font-weight:bold;">')
                                        .text(tunnelQ.Question)
                                    )
                                    .append($('<div qid="' + tunnelQ.ID + '" class="divResponse" style="margin-left:20px;">'))
                                )
                            )

                        if (answer != undefined) {
                            tunnelQ.Answer = answer.Answer;
                            tunnelQ.Description = answer.Description;
                            tunnelQ.Label = answer.Label;
                            tunnelQ.Type = answer.Type;
                            tunnelQ.QuestionID = answer.QuestionID;
                        }
                        
                        DisplayResponse(tunnelQ, true)
                    }
                } else {
                    var td = $('tr[rowid="' + vRows.MatrixRowID + '"]').find('td[colid="' + vRows.FieldID + '"]')
                        .empty()

                    if(vRows.Control == 'text'){
                        td.append(vRows.Answer);
                    } else {
                        td.append('<span class="bi ' + (CBool(vRows.IsUseCheckbox) ? 'bi-ok' : 'bi-record') + '"></span>');
                    }                        
                }
            }

            var ctrFiles = 1; ctrUpload = 0;
            function SaveQuestionaire() {
                var txtQuestionaireTitle = $.trim($('#txtQuestionaireTitle').val());
                var finalArrSurveyPage = [], isSave = true;

                if (txtQuestionaireTitle == '') {
                    $('#txtQuestionaireTitle').focus();
                    WebToast('Please enter questionaire title', 'top');
                    return;
                }

                //if (arrQuestions.length == 0) {
                //    WebToast('Please add at least one question', 'top');
                //    return;
                //}

                for (var i = 0; i < arrSurveyPage.length; i++){
                    if(arrSurveyPage[i].Questions.length > 0){ // only save page that has content on it
                        
                    }

                    arrSurveyPage[i].PrevLabel = ($.trim($('#prevLabel-' + arrSurveyPage[i].ID).val()) == '' ? 'Previous' : $.trim($('#prevLabel-' + arrSurveyPage[i].ID).val()));
                    arrSurveyPage[i].PrevAction = $.trim($('#prevAction-' + arrSurveyPage[i].ID).val());
                    arrSurveyPage[i].PrevGoto = $.trim($('#prevGotoPage-' + arrSurveyPage[i].ID).val());
                    arrSurveyPage[i].NextLabel = ($.trim($('#nextLabel-' + arrSurveyPage[i].ID).val()) == '' ? 'Next' : $.trim($('#nextLabel-' + arrSurveyPage[i].ID).val()));
                    arrSurveyPage[i].NextAction = $.trim($('#nextAction-' + arrSurveyPage[i].ID).val());
                    arrSurveyPage[i].NextGoto = $.trim($('#nextGotoPage-' + arrSurveyPage[i].ID).val());
                    arrSurveyPage[i].NextGotoLink = $.trim($('#nextGotoLink-' + arrSurveyPage[i].ID).val());
                    arrSurveyPage[i].IsShowButton = $('#chbShowNavBtn-' + arrSurveyPage[i].ID).is(':checked');

                    if (arrSurveyPage[i].NextAction == 'savefinishgotolink') {
                        if (!IsUrl(arrSurveyPage[i].NextGotoLink)) {
                            $('#' + arrSurveyPage[i].ID).opentabmenu();
                            WebToast('Please enter valid URL', 'top');
                            isSave = false;
                            break;
                        }
                    }

                    finalArrSurveyPage.push(arrSurveyPage[i]);
                }

                if(!isSave){
                    return false; // do not save
                }

                if (finalArrSurveyPage.length == 0) {
                    WebToast('No page content found', 'top');
                    return;
                }

                var pData = {
                    TitleID: questionaireID,
                    Title: txtQuestionaireTitle,
                    Pages: finalArrSurveyPage,
                    State: titleState,
                    Date: $.format.date(new Date(), 'MMMM dd,yyyy hh:mm a'),
                    IsActive: $('#selSurveyStatus').val(),
                    Requirements: Creator.QuestionaireReq,
                    ReqGroup: arrReqGroup
                }
                //console.log(arrReqGroup)
                //return;
                Bs.Loading('show', 'Please wait while saving survey form and do not reload the page', 'Message');
                $('#btnSurveySaveQuestion').prop('disabled', true);
                AjaxSend({
                    url: '/Survey/SaveQuestionaire?mod=' + (sessionStorage.SystemAdminFor || 'crm'),
                    data: pData,
                    success: function (e) {
                        $('#btnSurveySaveQuestion').removeAttr('disabled');
                        Bs.Loading('hide');
                        if (e.Results == 'saved') {
                            if (Creator.QuestionaireReq.length > 0) {
                                ctrFiles = 1;
                                ctrUpload = 0;
                                Bs.Loading('show','Please wait while updating 1 of ' + Creator.QuestionaireReq.length + ' requirement(s) and do not reload the page', 'Message');
                                SaveRequirement(function () {
                                    NewMessageBoxClose();
                                    success(e);
                                })
                            } else {
                                NewMessageBoxClose();
                                success(e);
                            }                            
                        } else {                            
                            NewMessageBox(e.Results, 'Message');
                        }
                    }
                })

                function success(e) {
                    arrQuestions = [];
                    questionaireID = '';
                    titleState = 'new';                    
                    $('#popQuestionaire').modal('hide');
                    GetSurvey();
                    Bs.Loading('hide');
                    if(e.code != ""){
                        NewMessageBox('New survey has been successfully created. Do you want to view your created survey?', 'Message', [
                            {
                                Text: 'Yes, show preview',
                                Click: function () {
                                    window.open(location.origin + '/Survey/Questionaire?q=' + e.code + '&mode=preview', '_blank');
                                }
                            }, { Text: 'No, thanks' }
                        ]);
                    } else {
                        WebToast('Successfully saved', 'top');
                    }
                }
            }
            
            function SaveRequirement(callback) {
                var formdata = new FormData();

                if (Creator.QuestionaireReq[ctrUpload].Files != undefined) {
                    if (Creator.QuestionaireReq[ctrUpload].Files.length > 0) {
                        formdata.append(Creator.QuestionaireReq[ctrUpload].Files[0].name, Creator.QuestionaireReq[ctrUpload].Files[0]);
                        Creator.QuestionaireReq[ctrUpload].Files = [];
                    }                    
                }                
                
                Creator.QuestionaireReq[ctrUpload].OrgID = sessionStorage.OrgID;
                Creator.QuestionaireReq[ctrUpload].QID = questionaireID;
                Creator.QuestionaireReq[ctrUpload].Date = $.format.date(new Date(), 'MMM dd, yyyy hh:mm a')
                var url = MergeUrlParameters(Creator.QuestionaireReq[ctrUpload], '/Survey/UploadRequirements');

                AjaxSend({
                    url: url,
                    type: 'POST',
                    data: formdata,
                    async: true,
                    cache: false,
                    contentType: false,
                    processData: false,
                    success: function (vData) {
                        Bs.Loading('hide');
                        if (ctrFiles == Creator.QuestionaireReq.length) {
                            if (vData.Results == 'saved') {
                                NewMessageBoxClose();
                                callback();
                            } else {
                                console.log(vData.Results)
                                NewMessageBox('There\'s an error uploading your file.', 'Message');
                            }
                        } else {
                            ctrFiles += 1;
                            ctrUpload += 1;
                            Bs.Loading('show', 'Please wait while updating '+ ctrFiles +' of ' + Creator.QuestionaireReq.length + ' requirement(s) and do not reload the page', 'Message');
                            SaveRequirement(callback);
                        }
                    }
                });
            }

            function AddNewQuestion() {
                $('#divOptionForChbRad, #divOptionForMatrix').hide();
                $('[name="choiceRadPresentation"]').removeAttr('checked');//.checkboxradio('refresh');
                $('[name="choiceRadPresentation"][value="0"]').prop('checked', true);//.checkboxradio('refresh');
                $('#chbQuestionRequired').removeAttr('checked');//.checkboxradio('refresh');
                $('.checkbox-only').removeClass('ui-btn-corner-all')
                $('#lblQuestion').text('Question');
                $('#divForQuestions').pageshow();
                $('#divRequired').show();
                if(qType == 'Single Input'){
                    Creator.SingleInput.init();
                }
                else if (qType == 'Multiple Inputs') {
                    Creator.MultipleInputs.init();
                }
                else if (qType == 'radio' || qType == 'checkbox'
                    || qType == 'radioinput' || qType == 'checkboxinput') {
                    Creator.CheckboxRadio.init(qType);
                }
                else if (qType == 'Matrix') {
                    $('#lblQuestion').text('Instruction');
                    Creator.Matrix.init();
                } else if (qType == 'Page Text') {
                    Creator.PageText.init();                    
                } else if (qType == "file") {
                    $('#lblQuestion').text('File Description');
                    Creator.File.init();
                }

                if (qType == 'radio') {
                    $('#divOptionForRad').show();
                } else {
                    $('#divOptionForRad').hide();
                }                
            }

            function DisplayQuestions() {
                listQuestions = $('#list-' + curPageArr.ID);
                listQuestions.empty();

                for (var i = 0; i < arrQuestions.length; i++){
                    arrQuestions[i].Index = i;
                    arrListOfAllQuestions.push(arrQuestions[i]);
                    if(arrQuestions[i].State != 'remove'){
                        if (arrQuestions[i].QuestionType == 'Single Input') {
                            DisplaySingleInput(arrQuestions[i]);
                        } else if (arrQuestions[i].QuestionType == 'Multiple Inputs') {
                            DisplayMultipleInputs(arrQuestions[i]);
                        } else if (arrQuestions[i].QuestionType == 'radio' || arrQuestions[i].QuestionType == 'checkbox'
                            || arrQuestions[i].QuestionType == 'radioinput' || arrQuestions[i].QuestionType == 'checkboxinput') {
                            DisplayMultipleChoice(arrQuestions[i]);
                        } else if (arrQuestions[i].QuestionType == 'Matrix') {
                            DisplayMatrix(arrQuestions[i]);
                        } else if (arrQuestions[i].QuestionType == 'Page Text') {
                            DisplayPageText(arrQuestions[i]);
                        } else if (arrQuestions[i].QuestionType == 'file') {
                            DisplayFile(arrQuestions[i]);
                        }
                    }
                }

                $('.checkbox-only').removeClass('ui-btn-corner-all')
            }

            function DisplayFile(data) {
                listQuestions.append($('<li>')
                        .append($('<div class="row">')
                            .append($('<div class="col-md-9">')
                                .append($('<span class="bi bi-upload">'))
                                .append('&nbsp;')
                                .append(data.Question)
                            )
                            .append($('<div class="col-md-3">')
                                .append(AddMenuOption(data))
                            )
                        )
                        .css({
                            padding: '15px'
                        })
                        .hover(function () {
                            ShowOption($(this), 'show');
                        }, function () {
                            ShowOption($(this), 'hide');
                        })
                    ).append('<hr>');
            }

            function DisplayPageText(data) {
                listQuestions.append($('<li class="list-group-item">')
                        .append($('<div class="row">')
                            .append($('<div class="col-md-9">')
                                .append(HTMLDecode(data.Question))
                            )
                            .append($('<div class="col-md-3">')
                                .append(AddMenuOption(data))
                            )
                        )
                        .css({
                            padding: '15px'
                        })
                        .hover(function () {
                            ShowOption($(this), 'show');
                        }, function () {
                            ShowOption($(this), 'hide');
                        })
                    )//.append('<hr>');
            }

            function HTMLDecode(str) {
                var htmlstr = $('<textarea />').html(str).text();
                return htmlstr;
            }

            function DisplayMatrix(data, isReadOnly) {
                var table = $('<table style="margin-right:5px; display:table;" class="table table-light table-striped table-hover">'),
                    tHead = $('<thead>'), tHeadRow = $('<tr class="custom-theme-f align-top text-center">'), tBody = $('<tbody>'), matrixChoices = '';
                var choiceType = (CBool(data.IsUseCheckbox) ? 'checkbox' : 'radio');
                tHeadRow.append($('<th style="min-width:280px;">'))
                for (var i = 0; i < data.Fields.Columns.length; i++) {
                    if(data.Fields.Columns[i].State != 'remove'){
                        tHeadRow.append($('<th class="txt-center" style="min-width:100px;">')
                            .data('data', data.Fields.Columns[i])
                            .html(data.Fields.Columns[i].Label + '<br><span style="font-weight: normal;"> (score: ' + CDec2(data.Fields.Columns[i].Score) + ')</span>')
                        )
                        if (!CBool(isReadOnly)) {
                            if (data.Fields.Columns[i].Control == 'radio') {
                                matrixChoices += '<td>' +
                                            '<div class="form-check">' +
                                                '<label for="chbChbRadMatrixRO" class="form-check-label">' +
                                                    '<input type="' + choiceType + '" data-role="' + choiceType + '" name="' + i + '" class="matrixChoices" id="chbChbRadMatrixRO" class="form-check-input">' +
                                                '</label>' +
                                                '<div></div>' +
                                            '</div>' +
                                        '</td>'
                            } else {
                                matrixChoices += '<td>' +
                                            '<input type="text" data-role="none" class="form-control">' +
                                        '</td>'
                            }
                        } else {
                            matrixChoices += '<td class="tdresponse" colid="' + data.Fields.Columns[i].ID + '"></td>'
                        }
                    }                    
                }
                tHead.append(tHeadRow)

                for (var i = 0; i < data.Fields.Rows.length; i++) {
                    if(data.Fields.Rows[i].State != 'remove'){
                        var row = $('<tr>')
                            .data('data', data.Fields.Rows[i])
                            .attr('rowid', data.Fields.Rows[i].ID)
                            .append($('<td>')
                                .text(data.Fields.Rows[i].Label)
                            )
                            .append(matrixChoices)
                        if (!CBool(isReadOnly)) {
                            row.find('input[type="' + choiceType + '"]').each(function () {
                                $(this).attr('name', i);
                                $(this).click(function (e) {
                                    e.stopPropagation();
                                    e.preventDefault();
                                })
                            })
                        }
                        tBody.append(row)
                    }
                }

                table.append(tHead).append(tBody);

                if (CBool(isReadOnly)) {
                    return table;
                }

                listQuestions.append($('<li class="list-group-item">')
                        .append($('<div class="row">')
                            .append($('<div class="col-md-9">')
                                .append($('<span style="font-weight:bold; font-size:20px;">').html((CBool(data.Required) ? '<span class="required">*</span> ' : '') + data.Question))
                            )
                            .append($('<div class="col-md-3">')
                                .append(AddMenuOption(data))
                            )
                        ).append('<br>')
                        .append($('<div style="width:100%; overflow-x: auto;">')
                            .append(table)
                        ).css({
                            padding: '15px'
                        })
                        .hover(function () {
                            ShowOption($(this), 'show');
                        }, function () {
                            ShowOption($(this), 'hide');
                        })
                    )//.append('<hr>');

                //$('.matrixChoices');//.checkboxradio();//.checkboxradio('refresh')
            }

            function DisplayMultipleChoice(data) {
                var textInputs = $('<div>'), type, sel = $('<select data-role="none" class="form-select fixed">').append(new Option('Please select', ''));

                if (CBool(data.IsWithField)) {
                    if (data.QuestionType == 'radioinput') {
                        type = 'radio';
                    } else {
                        type = 'checkbox'
                    }
                } else {
                    type = data.QuestionType;
                }

                for (var i = 0; i < data.Fields.length; i++) {
                    if (CBool(data.IsRadioButtonPresentAsSelectMenu)) {
                        sel.append(new Option(data.Fields[i].Label, data.Fields[i].Label))
                    } else {
                        var row = $('<div class="row">');
                        data.Fields[i].Index = i;
                        if (data.Fields[i].State != 'remove') {
                            row.append($('<div class="col-md-' + (CBool(data.IsWithField) ? 3 : 12) + '">')
                                .data('data', data.Fields[i])
                                .append($('<div class="form-check">')
                                    .append($('<label class="form-check-label">')
                                        .text(data.Fields[i].Label + ' (score: ' + CDec2(data.Fields[i].Score) + ')')
                                        .append($('<input type="' + type + '" id="chbrad-' + data.Index + '-' + data.Fields[i].Index + '" value="' + data.Fields[i].Value + '" name="chbradChoices' + data.Index + '" data-role="checkbox" class="form-check-input"/>'))
                                    )
                                )
                            )

                            if (CBool(data.IsWithField)) {
                                row.append($('<div class="col-md-3">')
                                        .append($('<input type="text" data-role="none" class="form-control">')
                                            .data('data', data.Fields[i])
                                            .attr('pInd', data.Index)
                                            .focus(function () {
                                                var radBtn = $('#chbrad-' + $(this).attr('pInd') + '-' + $(this).data('data').Index);
                                                if (radBtn.attr('type') == 'radio') {
                                                    $('[name="' + radBtn.attr('name') + '"]').removeAttr('checked');//.checkboxradio('refresh');
                                                }
                                                radBtn.prop('checked', true);//.checkboxradio('refresh');
                                            })
                                        )
                                    )
                            }

                            row.append('<br><br>')
                            textInputs.append(row);
                        }
                    }
                }

                if (CBool(data.IsRadioButtonPresentAsSelectMenu)) {
                    textInputs.append(sel);
                } else {
                    if (data.Other != undefined) {
                        if (data.Other.AsType == 'answer') {
                            textInputs.append($('<div class="row">')
                                .append($('<div class="col-md-12">')
                                    .data('data', data.Other)
                                    .append($('<div class="form-check">')
                                        .append($('<label class="form-check-label">')
                                            .text(data.Other.Label)
                                            .append($('<input type="' + type + '" value="Other" name="chbradChoices' + data.Index + '" data-role="checkbox" class="form-check-input"/>'))
                                        )
                                        .append('<br><br>')
                                        .append('<textarea data-role="none" class="form-control" style="height:100px; resize:none;"></textarea>')
                                    )
                                )
                            )
                        } else {
                            textInputs.append($('<div class="row">')
                                .append($('<div class="col-md-12">')
                                    .data('data', data.Other)
                                    .append($('<div>').text(data.Other.Label))
                                    .append('<br>')
                                    .append('<textarea data-role="none" class="form-control" style="height:100px; resize:none;"></textarea>')
                                )
                            )
                        }
                    }
                }                               

                listQuestions.append($('<li class="list-group-item">')
                        .append($('<div class="row mb-3">')
                            .append($('<div class="col-md-9">')
                                .append($('<span style="font-weight:bold; font-size:20px;">').html((CBool(data.Required) ? '<span class="required">*</span> ' : '') + data.Question))
                            )
                            .append($('<div class="col-md-3">')
                                .append(AddMenuOption(data))
                            )
                        )
                        .append(textInputs)
                        .css({
                            padding: '15px'
                        })
                        .hover(function () {
                            ShowOption($(this), 'show');
                        }, function () {
                            ShowOption($(this), 'hide');
                        })
                    )//.append('<hr>');

                //$('[name="chbradChoices' + data.Index + '"]').trigger('create');//.checkboxradio();//.checkboxradio('refresh');
            }

            function DisplaySingleInput(data) {
                listQuestions.append($('<li class="list-group-item">')
                        .append($('<div class="row">')
                            .append($('<div class="col-md">')
                                .append($('<span style="font-weight:bold; font-size:20px;">').html((CBool(data.Required) ? '<span class="required">*</span> ' : '') + data.Question))
                            )
                            .append($('<div class="col-md-3">')
                                .append(AddMenuOption(data))
                            )
                        )
                        .append('<br>')
                        .append($('<div class="row">')
                                .append($('<div class="col-md-12">')
                                    .append($('<textarea data-role="none" type="text" class="form-control txtInputs" id="txtLabel-0" style="height:50px;">'))
                            ).append('<br><br><br>')
                        )
                        .css({
                            padding: '15px'
                        })
                        .hover(function () {
                            ShowOption($(this), 'show');
                        }, function () {
                            ShowOption($(this), 'hide');
                        })
                    )//.append('<hr>');
            }

            function DisplayMultipleInputs(data) {
                var textInputs = $('<div>');

                for (var i = 0; i < data.Fields.length; i++) {
                    if (data.Fields[i].State != 'remove') {
                        textInputs.append($('<div class="row">')
                            .append($('<div class="col-md-2">')
                                .append($('<label for="txtLabel-' + data.Index + '-' + i + '">').html((CBool(data.Fields[i].IsRequired) ? '<span class="required">*</span> ' : '') + data.Fields[i].Label + ':'))
                            )
                            .append($('<div class="col-md">')
                                .data('data', data.Fields[i])
                                .append($('<input data-role="none" type="text" class="form-control txtInputs" id="txtLabel-' + data.Index + '-' + i + '">'))
                            ).append('<br><br><br>')
                        )
                    }                    
                }
                listQuestions.append($('<li class="list-group-item">')
                        .append($('<div class="row">')
                            .append($('<div class="col-md">')
                                .append($('<span style="font-weight:bold; font-size:20px;">').html((CBool(data.Required) ? '<span class="required">*</span> ' : '') + data.Question))
                            )
                            .append($('<div class="col-md-3">')
                                .append(AddMenuOption(data))
                            )
                        )
                        .append('<br>')
                        .append(textInputs)                        
                        .css({
                            padding: '15px'
                        })
                        .hover(function () {
                            ShowOption($(this), 'show');
                        }, function () {
                            ShowOption($(this), 'hide');
                        })
                    )//.append('<hr>');
            }

            function AddMenuOption(data) {
                return $('<div class="row txt-right divMenuOption" style="visibility:hidden;">')
                            .append($('<div class="col-md-12">')
                                .append($('<button data-role="none" title="Edit" class="btn btn-warning">')
                                    .data('data', data)
                                    .data('page', curPageArr)
                                    .append('<span class="bi bi-pencil">')
                                    .click(function () {
                                        var data = JSON.parse(JSON.stringify($(this).data('data')));
                                        curPageArr = $(this).data('page')
                                        arrQuestions = curPageArr.Questions;
                                        PopulateSurveyQuestions();
                                        EditQuestion(data)
                                    })
                                )
                                .append('&nbsp;')
                                .append($('<button data-role="none" title="Remove" class="btn btn-danger">')
                                    .data('data', data)
                                    .append('<span class="bi bi-trash">')
                                    .click(function () {
                                        var dis = $(this);
                                        var data = dis.data('data');
                                        NewMessageBox('Are you sure you want to delete this question? If yes, it will be deleted permanently.', 'Warning',
                                            [
                                                {
                                                    Text: 'Yes', Click: function () {
                                                        var pageIndex;

                                                        if(data.PageID == undefined){
                                                            pageIndex = curPageArr.Index;
                                                            data.PageID = curPageArr.ID;
                                                        } else {
                                                            $.grep(arrSurveyPage, function (e, i) {
                                                                if (e.ID == data.PageID) {
                                                                    pageIndex = i;
                                                                    return false; // stop
                                                                }
                                                            })
                                                        }                                                        

                                                        if (data.State == 'new') {                                                            
                                                            (arrSurveyPage[pageIndex].Questions).splice(CInt2(data.Index), 1)
                                                        } else {
                                                            arrSurveyPage[pageIndex].Questions[CInt2(data.Index)].State = 'remove';
                                                        }
                                                        curPageArr = {}
                                                        curPageArr.ID = data.PageID;
                                                        arrQuestions = arrSurveyPage[pageIndex].Questions;
                                                        DisplayQuestions();
                                                    }
                                                }, { Text: 'No' }
                                            ]
                                        )                                        
                                    })
                                )
                            )
            }

            function ShowOption(dis, display) {
                if(display == 'show'){
                    dis.find('.divMenuOption').css('visibility', 'visible');
                    dis.css('background-color', '#f5f5f5');
                } else {
                    dis.find('.divMenuOption').css('visibility', 'hidden');
                    dis.css('background-color', '#fff');
                }                
            }

            function EditQuestion(data) {
                var data = JSON.parse(JSON.stringify(data));

                $('#popNewQuestion').modal('show');
                $('#selQuestionType').attr('disabled', true);
                $('#divForQuestions').pageshow();

                if(data.QuestionType == 'Single Input'){
                    Creator.SingleInput.Edit(data);
                } else if (data.QuestionType == 'Multiple Inputs') {
                    Creator.MultipleInputs.Edit(data);
                } else if (data.QuestionType == 'radio' || data.QuestionType == 'checkbox'
                    || data.QuestionType == 'radioinput' || data.QuestionType == 'checkboxinput') {
                    Creator.CheckboxRadio.Edit(data);
                } else if (data.QuestionType == 'Matrix') {
                    Creator.Matrix.Edit(data);
                } else if (data.QuestionType == 'Page Text') {
                    Creator.PageText.Edit(data);
                } else if (data.QuestionType == 'file') {
                    Creator.File.Edit(data);
                }
            }

            function AddSurveyPage() {
                ctrPage += 1;
                var page = {
                    ID: NewID(),
                    Title: 'Page ' + ctrPage,
                    Questions: [],
                    State: 'new',
                    Index: ctrPageIndex,
                    IsShowButton: true
                }
                arrSurveyPage.push(page)
                ctrPageIndex += 1;
                RefreshPageList(page);
                //RemoveSpecialNextAction();
                DisplayPage([page]);                
            }

            function DisplayPage(pages) {
                var lisSurveyPage = $('#lisSurveyPage');
                var divSurveyPageContent = $('#divSurveyPageContent');

                for (var i = 0; i < pages.length; i++) {
                    lisSurveyPage.append($('<li class="nav-item" role="presentation">')
                            .append($('<button data-role="none" class="nav-link" data-toggle="tab" data-target="#' + pages[i].ID + '">').text(pages[i].Title)
                                .click(function () {
                                    var tabid = $($(this).attr('data-target'));
                                    divSurveyPageContent.find('> div').removeClass('show active');
                                    tabid.opentabmenu();
                                    tabid.addClass('show active');
                                })
                            )
                        )
                    divSurveyPageContent.append($('<div id="' + pages[i].ID + '" class="tab-pane fade show active">')
                            .append($('<div style="float:left;">')
                                .append($('<button data-role="none" title="Add Content" class="btn btn-success">')
                                    .data('data', pages[i])
                                    .text('Add Content')
                                    .click(function () {
                                        $('#selQuestionType').removeAttr('disabled');
                                        $('#popNewQuestion').modal('show');
                                        PopulateSurveyQuestions();
                                        AddNewQuestion();
                                        curPageArr = $(this).data('data');
                                        arrQuestions = curPageArr.Questions;
                                    })
                                )
                                .append('&nbsp;')
                                .append($('<button data-role="none" title="Edit Page Name" class="btn btn-warning">')
                                    .data('data', pages[i])
                                    .text('Edit Page Name')
                                    .click(function () {
                                        var data = $(this).data('data');
                                        curPageArr = $(this).data('data');
                                        arrQuestions = curPageArr.Questions;
                                        $('#popSurveyPageName').modal('show');;
                                        $('#txtSurveyPageName').val('').focus();
                                        $('#btnOkSurveyPageName').unbind('click').click(function () {
                                            arrSurveyPage[data.Index].Title = $.trim($('#txtSurveyPageName').val());
                                            $('[data-target="#' + data.ID + '"], [value="' + data.ID + '"]').text($.trim($('#txtSurveyPageName').val()));
                                            $('#popSurveyPageName').modal('hide');;
                                        })
                                    })
                                )
                                .append('&nbsp;')
                                .append($('<button data-role="none" title="Rearrange List" class="btn btn-info">')
                                    .data('data', pages[i])
                                    .text('Rearrange List')
                                    .click(function () {
                                        var data = $(this).data('data');
                                        RearrangeQuestions(data);
                                        $('#popRearrangeQuestions').modal('show');
                                    })
                                )
                                .append('&nbsp;')
                                .append($('<button data-role="none" title="Delete Page" class="btn btn-danger">')
                                    .data('data', pages[i])
                                    .text('Delete Page')
                                    .click(function () {
                                        var data = $(this).data('data');
                                        NewMessageBox('Are you sure you want to delete <b>' + data.Title + '</b> and its content? If yes, it cannot be undone.', 'Message', [
                                            {
                                                Text: 'Yes', Click: function () {
                                                    ctrPage -= 1;
                                                    if(data.State == 'new'){
                                                        arrSurveyPage.splice(data.Index, 1);
                                                    } else {
                                                        arrSurveyPage[data.Index].State = 'remove';
                                                    }
                                                    $('#lisSurveyPage li:first > button').click()
                                                    $('[data-target="#' + data.ID + '"], #' + data.ID + ', [value="' + data.ID + '"]').remove();

                                                    //AddSpecialNextAction();
                                                }
                                            }, { Text: 'No' }
                                        ])
                                    })
                                )                                
                            )
                            .append('<br><br>')
                            .append($('<ul id="list-' + pages[i].ID + '" class="list-group list-group-flush">'))
                            .append('<br>')
                            .append($('<div class="row">')
                                .append($('<div class="col-md-2">')
                                    .text('Previous button:')
                                )
                                .append($('<div class="col-md-3">')
                                    .append($('<input data-role="none" id="prevLabel-' + pages[i].ID + '" type="text" class="form-control" placeholder="Enter previous button label">'))
                                )
                                .append($('<div class="col-md-3">')
                                    .append($('<select data-role="none" id="prevAction-' + pages[i].ID + '" class="form-select">')
                                        .data('data', pages[i])
                                        .append($('<optgroup label="Please choose your action">')
                                            .append($('<option value="prev">').text('Previous page'))
                                            .append($('<option value="goto">').text('Go to specific page'))
                                            .append($('<option value="cancel">').text('Cancel'))                                            
                                        )
                                        .change(function () {
                                            var dis = $(this);
                                            ShowListPages('prev', dis.data('data'));
                                        })
                                    )
                                )
                                .append($('<div class="col-md-3" id="prevPageList-' + pages[i].ID + '" style="display:none;">')
                                    .append(GeneratePageList('prevGotoPage-' + pages[i].ID))
                                )
                            )
                            .append('<br>')
                            .append($('<div class="row">')
                                .append($('<div class="col-md-2">')
                                    .text('Next button:')
                                )
                                .append($('<div class="col-md-3">')
                                    .append($('<input data-role="none" id="nextLabel-' + pages[i].ID + '" type="text" class="form-control" placeholder="Enter next button label.">')
                                    )
                                )
                                .append($('<div class="col-md-3">')
                                    .append($('<select data-role="none" id="nextAction-' + pages[i].ID + '" class="form-select selectAction" style="width:100%;">')
                                        .data('data', pages[i])
                                        .append($('<optgroup class="optgroup" label="Please choose your action">')
                                            .append($('<option value="next">').text('Next page'))
                                            .append($('<option value="goto">').text('Go to specific page'))
                                            // commented by arnie 02-27-2019. When using Next and GoTo actions, data must save as draft
                                            //.append($('<option value="savenext">').text('Save & Go to next page'))
                                            //.append($('<option value="savegoto">').text('Save & Go to specific page'))
                                            //.append($('<option value="savefinish">').text('Save and Finish'))
                                            .append($('<option value="savefinishnext">').text('Save, Finish & Go to next page'))
                                            .append($('<option value="savefinishgoto">').text('Save, Finish & Go to specific page'))
                                            .append($('<option value="savefinishgotolink">').text('Save, Finish & Go to link'))                                            
                                        )
                                        .change(function () {
                                            var dis = $(this);
                                            ShowListPages('next', dis.data('data'));
                                        })
                                    )
                                )
                                .append($('<div class="col-md-3" id="nextPageList-' + pages[i].ID + '" style="display:none;">')
                                    .append(GeneratePageList('nextGotoPage-' + pages[i].ID))
                                )
                                .append($('<div class="col-md-3" id="gotolink-' + pages[i].ID + '" style="display:none;">')
                                    .append($('<input data-role="none" id="nextGotoLink-'+ pages[i].ID +'" class="form-control" placeholder="Enter URL here">'))
                                )
                            )
                            .append('<br>')
                            .append($('<div>')
                                .append($('<div class="form-check" title="Is this required?" style="margin-left:-5px; margin-top:-5px;">')
                                    .append($('<label class="form-check-lable">')
                                        .text('Show navigation button')
                                        .append($('<input data-role="none" class="form-check-input" type="checkbox" id="chbShowNavBtn-' + pages[i].ID + '" ' + (CBool(pages[i].IsShowButton) ? 'checked' : '') + '>'))
                                    )
                                )
                            )
                        )
                }
                //$('#chbShowNavBtn-' + pages[0].ID);//.checkboxradio();//.checkboxradio('refresh');
                //$('#' + pages[0].ID).opentabmenu();
                //$('.checkbox-only').removeClass('ui-btn-corner-all')
            }

            function RearrangeQuestions(data) {
                var listrearrangequestions = $('#listrearrangequestions');

                listrearrangequestions.empty();

                for (var i = 0; i < data.Questions.length; i++) {
                    data.Questions[i].PageID = data.ID;
                    listrearrangequestions.append($('<li>')
                            .css({
                                'padding': '10px 10px',
                                cursor: 'move'
                            })
                            .data('data', data.Questions[i])
                            .html(HTMLDecode(data.Questions[i].Question))
                        )
                }
            }

            function ShowListPages(type, dis) {
                var val;
                if (type == 'prev') {
                    val = $('#prevAction-' + dis.ID).val();
                    if (val == 'goto') {
                        $('#prevPageList-' + dis.ID).show();
                    } else {
                        $('#prevPageList-' + dis.ID).hide();
                    }
                } else {
                    val = $('#nextAction-' + dis.ID).val();
                    if (val == 'goto' || val == 'savegoto' || val == 'savefinishgoto') {
                        $('#nextPageList-' + dis.ID).show();
                        $('#gotolink-' + dis.ID).hide();
                    } else if (val == 'savefinishgotolink') {
                        $('#gotolink-' + dis.ID).show();
                        $('#nextPageList-' + dis.ID).hide();
                    } else {
                        $('#nextPageList-' + dis.ID).hide();
                        $('#gotolink-' + dis.ID).hide();
                    }
                }
            }

            function GeneratePageList(id) {
                var sel = $('<select data-role="none" id="'+ id +'" class="form-control fixed selPages">');
                for (var i = 0; i < arrSurveyPage.length; i++){
                    if(arrSurveyPage[i].State != 'remove'){
                        sel.append($('<option value="' + arrSurveyPage[i].ID + '">').text(arrSurveyPage[i].Title))
                    }                    
                }
                return sel;
            }

            function RefreshPageList(page) {
                $('.selPages').each(function () {
                    if($(this).find('[option="'+ page.ID +'"]').length == 0){
                        $(this).append($('<option value="' + page.ID + '">').text(page.Title));
                    }
                })
            }

            function AddSpecialNextAction() {
                if ($('.selectAction:last').find('option[value="savefinishnext"]').length == 0) {
                    $('.selectAction:last').find('.optgroup').append('<option value="savefinishnext">Save, Finish & Go to next page</option>')
                        .append('<option value="savefinishgoto">Save, Finish & Go to specific page</option>')
                }
            }

            function RemoveSpecialNextAction() {
                //remove the save finish option in all NEXT action . only last page must have this option
                $('.selectAction').find('option').each(function () {
                    if ($(this).val() == 'savefinishnext' || $(this).val() == 'savefinishgoto') {
                        $(this).remove()
                    }
                })
            }

            function ClearPages() {
                questionaireID = '';
                titleState = 'new';
                Creator.QuestionaireReq = [];
                arrSurveyPage = [];
                arrQuestions = [];
                arrReqGroup = [];
                ctrPage = 0;
                ctrPageIndex = 0;
                arrListOfAllQuestions = [];
                $('#selSurveyStatus').val('1');//.slider('refresh');
                $('#lisSurveyPage, #divSurveyPageContent, #lstSurveyRequirement, #divReqGroup').empty();
            }
        })        
    })
})