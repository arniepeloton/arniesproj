require([
    'common',
    '/Content/Ecommerce/js/lightbox-2.11.1.js'
    //,'/Scripts/jquery.dataTables.min.js'
], function () {

    require(['promise!/Scripts/jquery-ui-sortable.js'], function () {

        var arrSolution = [];
        var ctrSolution = 0;
        var editSolution = false;
        var arrSolutionSteps = [];
        var arrtempSolutionSteps = [];
        var ctrSolutionStep = 0;
        var editSolutionStep = false;
        var isEditMode = false;
        var arrIndexSolution, arrIndexStep, arrAllowedTeam = [], arrFilter = [], arrNewImages = [];
        var dTableKb;
        var entities = { '<': '&lt;', '>': '&gt;' };
        var cntUploadPic = 0, cntUploadedPic = 0, allowedtoSave = true;
        var isAddNewStepsTruPic = false;
        var isSavingReadingList = false;
        var arrArticleTopic = [], articleState, articleID;

        $(document).ready(function () {
            LoadRequiredData();

            window.GetKnowledgeBase = function () {
                GetKnowledgeBase();
            }

            $('.btnHome').click(function () {
                SignOut();
            })

            $('#btnArrangeReadingList').click(function () {
                GetAllList();
            })

            $('#btnSaveKM').click(function () {
                if(allowedtoSave == true && cntUploadPic == 0){
                    //SaveCase();
                    GetCheckedTopic()
                } else {
                    WebToast('Pictures are being imported ... please wait', 'top');
                }
            });

            $('#btnCancelKM').click(function () {
                editSolution = false;
                $('#panMainDashboardKB').pageshow();

                if(location.pathname.toLowerCase() != '/erp'){
                    AddBreadCrumb({
                        text: 'Main Dashboard', Click: function () {
                            $('#panMainDashboardKB').pageshow();
                        }
                    }, '.ItemsBreadCrumbs')
                } else {
                    if (sessionStorage.SystemAdminFor != 'dtr') {
                        AddBreadCrumb({
                            text: 'Knowledge Base', Click: function () {
                                $('#divErpKnowledgeBase, #panMainDashboardKB').pageshow();
                            }
                        }, '.ItemsBreadCrumbs')
                    }                    
                }
            });

            $('#selLanguage').change(function (e) {
                if ($('#selLanguage').val() == 'AddNew') {
                    objSelected = 'Language';
                    isEditMode = false;
                    $('#lblAddObject').html('Add Language');
                    $('#lblAddObjectTitle').html('Language');
                    $('#txtObject').val('');
                    $('#panAddObject').modal('show');;
                    $('#btnSaveObject').unbind('click').click(function () {
                        SaveObject();
                    });
                }
            });

            $('#selProduct').change(function (e) {
                if ($('#selProduct').val() == 'AddNew') {
                    objSelected = 'Product';
                    isEditMode = false;
                    $('#lblAddObject').html('Add Product');
                    $('#lblAddObjectTitle').html('Product');                
                    $('#panAddObject').modal('show');;
                    $('#txtObject').val('').focus();
                    $('#btnSaveObject').unbind('click').click(function () {
                        SaveObject();
                    });
                }
            });

            $('#btnAddNewKbProduct').click(function (e) {
                objSelected = 'Product';
                isEditMode = false;
                $('#lblAddObject').html('Add Product');
                $('#lblAddObjectTitle').html('Product');
                $('#panAddObject').modal('show');;
                $('#txtObject').val('').focus();
                $('#btnSaveObject').unbind('click').click(function () {
                    SaveObject(function (e) {
                        $('#listProduct').append($('<div class="form-check">')
                                    .append($('<input class="chbKbProduct form-check-input" type="checkbox" id="chbProduct-' + e.ID + '" value="' + e.ID + '" data-iconpos="notext">')
                                            .data('data', e)
                                        )
                                    .append($('<label class="form-check-label">')
                                        .append($('<div>').text(e.Description))
                                    )
                                )
                    });
                });
            });

            $('#btnAddNewTopic').click(function () {
                objSelected = 'Topic';
                isEditMode = false;
                $('#lblAddObject').html('Add Topic');
                $('#lblAddObjectTitle').html('Topic');
                $('#panAddObject').modal('show');;
                $('#txtObject').val('').focus();

                $('#btnSaveObject').unbind('click').click(function () {
                    SaveObject(function (vData) {
                        var pData = {
                            ID: vData.ID,
                            Topic: vData.Description
                        }
                        DisplayManuallyAddedTopic($('<div topicid="' + vData.ID + '" class="divsubtopic" style="display:none; margin-left:30px;">'), pData);
                        $('.chbKb');//.checkboxradio();//.checkboxradio('refresh');
                    });
                });
            })

            $('#selTopic').change(function (e) {
                if ($(this).val() == 'AddNew') {
                    objSelected = 'Topic';
                    $('#lblAddObject').html('Add Topic');
                    $('#lblAddObjectTitle').html('Topic');
                    $('#txtObject').val('');
                    $('#panAddObject').modal('show');;
                } else {
                    GetAllSubTopics();
                }
            });

            $('#selFilterTopic').change(function (e) {
                if ($(this).val() != '') {
                    GetAllFilterSubTopics($(this).val());
                } else {
                    $('#selFilterSubTopic').empty();//.selectmenu('refresh');//.selectmenu('disable');
                }
            });

            $('#selSubTopic').change(function (e) {
                if ($('#selSubTopic').val() == 'AddNew') {
                    objSelected = 'SubTopic';
                    $('#lblAddObject').html('Add SubTopic');
                    $('#lblAddObjectTitle').html('SubTopic');
                    $('#txtObject').val('');
                    $('#panAddObject').modal('show');;
                }
            });

            $('#btnAddSolution').click(function () {
                $('#panAddSolution').modal('show');
                $('#txtSolutionAndAnswer').val('').height('50px').focus();
                $('#listSolutionSteps').empty();
            })

            $('#btnSteps').click(function () {
                $('#panAddSteps').modal('show');;
                $('#txtNewSteps').empty();
                placeCaretAtEnd($('#txtNewSteps')[0])
                arrNewImages = [];
            })

            $('#btnOkAndAddNewSteps').click(function () {
                NewSteps(function () {
                    $('#btnSteps').click();
                });
                //$('#txtNewSteps').val('').height('50px').focus();
            })

            $('#btnOkNewSteps').click(function () {
                NewSteps();                
            })

            $('#btnCancelNewSteps').click(function () {
                $('#panAddSteps').modal('hide');;
                $('#txtNewSteps').val('').height('50px');
                editSolutionStep = false;
            })

            $('#btnCancelSolution').click(function () {
                $('#panAddSolution').modal('hide');
                editSolution = false;
                ctrSolutionStep = 0;
                arrSolutionSteps = [];
                ClearSolutionField();
            })

            $('#btnOkSolution').click(function () {
                if ($('#chbPrivate').is(':checked') == true) {
                    if ($('.mobilecontainer').length == 0) {
                        WebToast('Please add team', 'top');
                        return;
                    }

                    if ($.trim($('#txtSolutionAndAnswer').val()) == '') {
                        $('#txtSolutionAndAnswer').focus();
                        return;
                    }
                }

                if ($('#txtSolutionAndAnswer').val().trim() == '') {
                    WebToast('Please enter solution or answer', 'top');
                    return;
                }

                $('#panAddSolution').modal('hide');
                ctrSolutionStep = 0;
                if (editSolution == false) {
                    ctrSolution = ctrSolution + 1;
                    arrSolution.push({
                        No: ctrSolution,
                        SolutionID: '',
                        Solution: $('#txtSolutionAndAnswer').val().replace(/[<>]/g, function (s) { return entities[s]; }),
                        Step: arrSolutionSteps,
                        State: 'Add',
                        AllowedTeam: arrAllowedTeam,
                        IsPublic: ($('input[name="chbPrivate"]').is(':checked') == true ? false : true)
                    })
                }
                else {
                    arrSolution[arrIndexSolution].Solution = $('#txtSolutionAndAnswer').val();
                    arrSolution[arrIndexSolution].Step = arrSolutionSteps;
                    arrSolution[arrIndexSolution].AllowedTeam = arrAllowedTeam;
                    arrSolution[arrIndexSolution].IsPublic = ($('input[name="chbPrivate"]').is(':checked') == true ? false : true)
                }
                //SortByNo()
                DisplaySolution();
                editSolution = false;
                arrSolutionSteps = [];
                arrAllowedTeam = [];
            })

            $('#btnPrivateSolution').click(function () {
                BrowsePopup('LibraryCRMTeam', 'Team',
                        [
                            {
                                AutoSearch: true,
                                AddButton: [
                                    {
                                        Text: 'Create New Team',
                                        Click: function () {
                                            $('#panNewTeam').modal('show');;
                                            $('#txtNewTeamName').val('').focus();
                                        }
                                    }
                                ],
                                DisplayText: [{ Column: 'Name'}],
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

            $('#btnOkNewTeam').click(function () {
                if ($.trim($('#txtNewTeamName').val()) == "") {
                    $('#txtNewTeamName').focus();
                    return;
                }

                AjaxSend({
                    url: 'KnowledgeBase/SaveNewTeam',
                    data: { TeamName: $('#txtNewTeamName').val(), DateNow: new Date() },
                    success: function (vData) {
                        if (vData.Results == "True") {
                            $('#txtNewTeamName').val('');
                            $('#panNewTeam').modal('hide');;
                            $('#btnPrivateSolution').click();
                        }
                        else {
                            NewMessageBox(vData.Results, 'ERROR');
                        }
                    }
                })
            })

            $('#chbPrivate').change(function () {
                if ($(this).is(':checked')) {
                    $('#divAllowedTeam').show();
                }
                else {
                    $('#divAllowedTeam').hide();
                }
            })

            $('.txtSearchKB').on('keyup', function (e) {
                if(e.which == 13){
                    dTableKb.search($.trim($(this).val())).draw();
                }
                return;
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

            $('#btnAddKB').click(function () {
                clearKMFields();
                GetKnowledgeManagementNumber()
                $('#divCaseUrl').hide();
                $('#txtCaseUrl').val('');
                $('#panAddKnowledgeManagement').pageshow();
                $('#txtSolution, #txtSteps').height('50px');

                if (sessionStorage.SystemAdminFor != 'dtr') {
                    AddBreadCrumb({
                        text: 'New Article', Click: function () {
                            $('#panAddKnowledgeManagement').pageshow();
                        }
                    }, '.ItemsBreadCrumbs')
                }                
            })

            $('#btnSaveObject').click(function () {
                SaveObject();
                isEditMode = false;
            });

            $('#btnKBFilter').click(function () {
                $('#panFilter').modal('show');
            })

            $('#btnOkFilter').click(function () {
                var isSelectEmpty = true
                $('#divKbFilter select').each(function () {
                    if($(this).val() != ""){
                        isSelectEmpty = false;
                    }
                })

                if(isSelectEmpty == true){
                    WebToast('Please fill at least one dropdown', 'top');
                    return;
                } else {
                    AddFilter()
                }

                $('#panFilter').modal('hide');
            })

            var curDragItemIndex = 0;
            var curDragItemNo = 0;
            $('#listSolutionSteps').sortable({
                delay: 150,
                update: function (event, ui) {
                    var newOrderNo = CInt2(1);
                    var newArrSteps = [];
                    $('#listSolutionSteps li').each(function () {
                        $(this).attr('no', CStr2(newOrderNo))
                        $(this).data().No = CStr2(newOrderNo)
                        curDragItemIndex += 1;
                        newOrderNo += 1;
                        $(this).data().sortableItem = "";
                        newArrSteps.push($(this).data())
                    })
                    ctrSolutionStep = newOrderNo - 1;
                    arrSolutionSteps = newArrSteps;
                    curDragItemIndex = 0;
                    curDragItemNo = 0;
                }
            })

            $('#listSolution').sortable({
                delay: 150,
                update: function (event, ui) {
                    var newOrderNo = CInt2(1);
                    var newArrSolution = [];
                    $('#listSolution > li').each(function () {
                        $(this).attr('no', CStr2(newOrderNo))
                        $(this).data().No = CStr2(newOrderNo)
                        newOrderNo += 1;
                        $(this).data().sortableItem = "";
                        newArrSolution.push($(this).data())
                    })
                    ctrSolution = newOrderNo - 1;
                    arrSolution = newArrSolution;
                }
            })

            var stepCaretPos = $('#txtSolutionAndAnswer').prop("selectionStart");
            var caretPos;
            $('#formStepPic').submit(function (e) {
                e.preventDefault();
                var formData = new FormData($(this)[0]);
                
                var reader = new FileReader();                

                reader.onload = function (e) {
                    //setCaretPosition(caretPos.row, caretPos.col)
                    var file;
                    var imgID = NewID();

                    placeCaretAtEnd($('#txtNewSteps')[0])
                    insertAtCursor('img', 'img', e.target.result, imgID, 'uploading', $('#fileStepPic')[0].files[0])
                    placeCaretAtEnd($('#txtNewSteps')[0])

                    cntUploadPic = cntUploadPic + 1;

                    $.ajax({
                        url: '/KnowledgeBase/UploadPicture?OrgID=' + sessionStorage.OrgID + '&caretpos=' + caretPos + '&imgid=' + imgID,
                        type: 'POST',
                        data: formData,
                        async: true,
                        cache: false,
                        contentType: false,
                        processData: false,
                        beforeSend: function(){
                            file = $('#fileStepPic')[0].files[0];
                        },
                        success: function (vData) {
                            $('#fileStepPic').val('');
                            $('[imgid="' + vData.ImgID + '"]').attr('status', 'done')

                            //vData.Location = HTMLEncode(vData.Location);
                            vData.file = file; //$('[imgid="' + vData.ImgID + '"]').data().file;
                            vData.Status = 'new';

                            arrNewImages.push(vData);

                            cntUploadedPic = cntUploadedPic + 1;

                            if (cntUploadedPic == cntUploadPic) {
                                allowedtoSave = true;
                                cntUploadPic = 0;
                                cntUploadedPic = 0;
                            }

                            var reader = new FileReader();
                            reader.onload = (function (theFile) {
                                var imgID = theFile.ImgID;
                                return function (e) {
                                    $('[imgid="' + imgID + '"]').attr('src', e.target.result)
                                };
                            })(vData);
                            reader.readAsDataURL(vData.file);
                        }
                    });
                };
                reader.readAsDataURL($('#fileStepPic')[0].files[0]);                
            })

            $('#fileStepPic').change(function (e) {
                e.preventDefault();
                if($(this).val() != ""){
                    $('#formStepPic').submit()
                }                
            })
            .click(function () {
                caretPos = getCaretPosition();
            })

            $('#txtNewSteps').mouseup(function () {
                caretPos = getCaretPosition();
            })

            $('#btnsample').click(function () {
                setSelectionRange($('#txtNewSteps')[0], caretPos)
                insertAtCursor('img', 'img', 'https://cnet4.cbsistatic.com/hub/i/2011/10/27/a66dfbb7-fdc7-11e2-8c7c-d4ae52e62bcc/android-wallpaper5_2560x1600_1.jpg')
                //insertAtCursor('text', 'sample')
            })

            $('#btnFileOpener').click(function () {
                $('#fileStepPic').click();
            })

            $('#btnCaseUrl').click(function () {
                copyToClipboard();
            })

            $('#btnSaveArrangeReadingList').click(function () {
                var lstArrangeList = $('#lstArrangeList');
                var arrProduct = [];
                isSavingReadingList = true;

                lstArrangeList.find('> li').each(function () {
                    //LOOP for Product
                    var productLI = $(this);
                    var productTopic = [];
                    var ctrTopic = 1;

                    if (productLI.find('> ol').length >= 1) {
                        productLI.find('> ol > li').each(function () {
                            //LOOP for Topic
                            var topic = $(this);
                            var arrSubTopic = [], ctrSubTopic = 1;

                            if (topic.find('> ol').length >= 1) {
                                topic.find('> ol > li').each(function () {
                                    //LOOP for subtopic
                                    var subtopic = $(this);
                                    var arrProblem = [], ctrProblem = 1;
                                    if (subtopic.find('> ol').length >= 1) {
                                        subtopic.data().Problem = [];
                                        subtopic.find('> ol > li').each(function () {
                                            //LOOP for Problem
                                            var problem = $(this);
                                            problem.data().OrderNo = ctrProblem;
                                            problem.data().sortableItem = "";
                                            arrProblem.push(problem.data())
                                            subtopic.data().Problem = arrProblem;
                                            ctrProblem += 1;
                                        })
                                    }
                                    subtopic.data().OrderNo = ctrSubTopic;
                                    subtopic.data().sortableItem = "";
                                    arrSubTopic.push(subtopic.data())
                                    topic.data().List = arrSubTopic;
                                    ctrSubTopic += 1;
                                })
                            } else {
                                topic.data().List = [];
                            }
                            topic.data().OrderNo = ctrTopic;
                            topic.data().sortableItem = "";
                            productTopic.push(topic.data())
                            ctrTopic += 1;
                        })
                    }
                    productLI.data().sortableItem = "";
                    productLI.data().Topic = productTopic;
                    arrProduct.push(productLI.data())
                })

                AjaxSend({
                    url: 'KnowledgeBase/SaveArrangement',
                    data: { ServerUrl: $.trim($('#txtKBServerIP').val()), data: arrProduct },
                    success: function (vData) {
                        isSavingReadingList = false;
                        if (vData.Results == 'saved') {
                            WebToast('Successfully Saved')
                        } else {
                            NewMessageBox(vData.Results, 'Something went wrong')
                        }
                    }
                })
            })

            function GetAllList() {
                var lstArrangeList = $('#lstArrangeList');

                lstArrangeList.empty().append('<h3>Please wait ...</h3>');

                AjaxSend({
                    url: 'KnowledgeBase/ArrangeList',
                    success: function (vData) {
                        lstArrangeList.empty()

                        if (vData.length > 0) {
                            SortList('lstArrangeList');

                            for (var i = 0; i < vData.length; i++) {
                                var product = vData[i];
                                var topic = vData[i].Topic;
                                var topicOL = $('<ol id="' + i + '" class="draggableList" style="display:none;">');
                                var productLI = $('<li>').data(product).append($('<h2>').text(product.Name + ' ').append($('<a ol="' + i + '" class="custom-link" style="font-size:14px;">').text('show child')
                                                        .click(function () {
                                                            if ($(this).text() == 'show child') {
                                                                $('#' + $(this).attr('ol')).show()
                                                                $(this).text('hide child')
                                                            } else {
                                                                $('#' + $(this).attr('ol')).hide()
                                                                $(this).text('show child')
                                                            }
                                                        })
                                                    ))

                                SortList(i)

                                if (topic.length > 0) {
                                    for (var t = 0; t < topic.length; t++) {
                                        var subtopicOL = $('<ol id="' + i + '' + t + '" class="draggableList" style="display:none;">');
                                        var topicLI = $('<li>').data(topic[t]).append($('<h3>').text(topic[t].Topic + ' ').append($('<a ol="' + i + '' + t + '" class="custom-link" style="font-size:14px;">').text('show child')
                                                        .click(function () {
                                                            if ($(this).text() == 'show child') {
                                                                $('#' + $(this).attr('ol')).show()
                                                                $(this).text('hide child')
                                                            } else {
                                                                $('#' + $(this).attr('ol')).hide()
                                                                $(this).text('show child')
                                                            }
                                                        })
                                                    ));
                                        var subtopic = topic[t].List;

                                        SortList(i + '' + t)

                                        //if (subtopic.NoSubTopic.length > 0) {
                                        //    for (s = 0; s < subtopic.NoSubTopic.length; s++) {
                                        //        var subtopicLI = $('<li>').data(subtopic.NoSubTopic[s]).append($('<h3>').text(subtopic.NoSubTopic[s].Problem));

                                        //        subtopicOL.append(subtopicLI);
                                        //        topicLI.append(subtopicOL);
                                        //    }
                                        //}

                                        for (s = 0; s < subtopic.SubTopic.length; s++) {
                                            var arrsubtopic = subtopic.SubTopic[s];
                                            if (arrsubtopic != null) {
                                                var subtopicLI = $('<li>').data(arrsubtopic).append($('<h3>').text(arrsubtopic.SubTopic + ' ').append($('<a ol="' + i + '' + t + '' + s + '" class="custom-link" style="font-size:14px;">').text('show child')
                                                        .click(function () {
                                                            if ($(this).text() == 'show child') {
                                                                $('#' + $(this).attr('ol')).show()
                                                                $(this).text('hide child')
                                                            } else {
                                                                $('#' + $(this).attr('ol')).hide()
                                                                $(this).text('show child')
                                                            }
                                                        })
                                                    ));
                                                var problem = arrsubtopic.Problem
                                                var problemOL = $('<ol id="' + i + '' + t + '' + s + '" class="draggableList" style="display:none;">');

                                                SortList(i + '' + t + '' + s)

                                                if (problem != undefined) {
                                                    for (var p = 0; p < problem.length; p++) {
                                                        var problemLI = $('<li>').data(problem[p]).append($('<h3>').text(problem[p].Problem));

                                                        problemOL.append(problemLI);
                                                        subtopicLI.append(problemOL);
                                                    }
                                                } else {

                                                }

                                                subtopicOL.append(subtopicLI);
                                                topicLI.append(subtopicOL);
                                            }
                                        }

                                        topicOL.append(topicLI);
                                        productLI.append(topicOL);
                                    }
                                }

                                lstArrangeList.append(productLI)
                            }
                        }
                    }
                })
            }

            function SortList(id) {
                setTimeout(function (e) {
                    $('#' + e).sortable({
                        list: '#' + e,
                        delay: 150,
                        start: function (event, ui) {
                            ui.item.parent().css('border', '1px solid #CCC');
                        },
                        stop: function (event, ui) {
                            ui.item.parent().css('border', '1px solid #FFF');
                        },
                        update: function (event, ui) {
                            var id = ui.item.parent().attr('id');
                            var newOrderNo = 1;
                            $('#' + id).find('> li').each(function () {
                                $(this).data().OrderNo = newOrderNo;
                                newOrderNo += 1;
                            })
                        }
                    });
                    //$('#' + e).disableSelection();
                }, 100, id)
            }

            function copyToClipboard() {
                $('#txtCaseUrl').select();
                document.execCommand("copy");
                WebToast('Copied to clipboard');
            }

            function HTMLDecode(str) {
                var htmlstr = $('<textarea />').html(str).text();
                return htmlstr;
            }

            function HTMLEncode(str) {
                var i = str.length,
                    aRet = [];

                while (i--) {
                    var iC = str[i].charCodeAt();
                    if (iC < 65 || iC > 127 || (iC > 90 && iC < 97)) {
                        aRet[i] = '&#' + iC + ';';
                    } else {
                        aRet[i] = str[i];
                    }
                }
                return aRet.join('');
            }

            function setSelectionRange(input, selectionStart, selectionEnd) {
                if (input.setSelectionRange) {
                    input.focus();
                    input.setSelectionRange(selectionStart, selectionEnd);
                } else if (input.createTextRange) {
                    var range = input.createTextRange();
                    range.collapse(true);
                    range.moveEnd('character', selectionEnd);
                    range.moveStart('character', selectionStart);
                    range.select();
                }
            }

            function placeCaretAtEnd(el) {
                el.focus();
                if (typeof window.getSelection != "undefined"
                        && typeof document.createRange != "undefined") {
                    var range = document.createRange();
                    range.selectNodeContents(el);
                    range.collapse(false);
                    var sel = window.getSelection();
                    sel.removeAllRanges();
                    sel.addRange(range);
                } else if (typeof document.body.createTextRange != "undefined") {
                    var textRange = document.body.createTextRange();
                    textRange.moveToElementText(el);
                    textRange.collapse(false);
                    textRange.select();
                }
            }

            function insertAtCursor(type, text, src, imgID, imgStatus, dataimg) {
                var sel = window.getSelection && window.getSelection();
                if (sel) {
                    if (type == 'img') {
                        var img = document.createElement(text)
                        var br = document.createElement("br");
                        var range = sel.getRangeAt(0);
                        range.deleteContents();
                        range.insertNode(img);
                        img.setAttribute('class', 'steppic')
                        img.setAttribute('src', src)
                        //img.setAttribute('style', 'width:100%')
                        img.setAttribute('imgid', imgID)
                        img.setAttribute('status', imgStatus)
                        img.setAttribute('alt', 'Loading Image ...')
                        //img.appendChild(br)
                        //img.appendChild(br)
                        $('[imgid="' + imgID + '"]').data('file', dataimg)

                    } else {
                        var txt = document.createTextNode(text)
                        var range = sel.getRangeAt(0);
                        range.deleteContents();
                        range.insertNode(txt);
                    }
                }
            }

            function setCaretPosition(r, c) {
                var el = document.getElementById("txtNewSteps");
                var range = document.createRange();
                var sel = window.getSelection();
                range.setStart(el.childNodes[r], c);
                range.collapse(true);
                sel.removeAllRanges();
                sel.addRange(range);
            }

            function getCaretPosition() {
                //var sel = document.getSelection(),
                //    nd = sel.anchorNode,
                //    text = nd.textContent.slice(0, sel.focusOffset);

                //var line = text.split("\n").length;
                //var col = text.split("\n").pop().length;
                var caretOffset = 0;
                if (typeof window.getSelection != "undefined") {
                    var range = window.getSelection().getRangeAt(0);
                    var preCaretRange = range.cloneRange();
                    preCaretRange.selectNodeContents($('#txtNewSteps')[0]);
                    preCaretRange.setEnd(range.endContainer, range.endOffset);
                    caretOffset = preCaretRange.toString().length;
                } else if (typeof document.selection != "undefined" && document.selection.type != "Control") {
                    var textRange = document.selection.createRange();
                    var preCaretTextRange = document.body.createTextRange();
                    preCaretTextRange.moveToElementText($('#txtNewSteps')[0]);
                    preCaretTextRange.setEndPoint("EndToEnd", textRange);
                    caretOffset = preCaretTextRange.text.length;
                }
                return caretOffset;
            }

            function AddFilter() {
                var topic = $('#selFilterTopic');
                var subtopic = $('#selFilterSubTopic');
                var product = $('#selFilterProduct');
                var language = $('#selFilterLanguage');

                var tempArr = [topic, subtopic, product, language];
                arrFilter = [];

                for (var i = 0; i < tempArr.length; i++) {
                    var isExist = false;
                    var type = '';

                    if (arrFilter.length > 0) {
                        $.grep(arrFilter, function (e) {
                            if (CStr2(e.ID) == tempArr[i].val()) {
                                isExist = true;
                            }
                        })
                    }

                    if (isExist == false && (tempArr[i].val() || '') != "") {
                        if (i == 0) {
                            type = 'topic';
                        } else if (i == 1) {
                            type = 'subtopic';
                        } else if (i == 2) {
                            type = 'product';
                        } else if (i == 3) {
                            type = 'language';
                        }
                        arrFilter.push({
                            id: tempArr[i].val(),
                            description: tempArr[i].find('option:selected').text(),
                            type: type
                        })
                    }
                }

                DisplayFilter();
                FilteredText();
            }

            function FilteredText() {
                filterVal = '';
                for (var i = 0; i < arrFilter.length; i++) {
                    filterVal = filterVal + (i > 0 ? '||' : ''); // for splitting purposes only
                    filterVal = filterVal + arrFilter[i].type + '\/' + arrFilter[i].description + '\/' + arrFilter[i].id
                }
                dTableKb.columns(1).search(filterVal).draw();
                return filterVal;
            }

            function DisplayFilter() {
                var listFilteredBy = $('#listFilteredBy');

                listFilteredBy.empty();

                for (var i = 0; i < arrFilter.length; i++) {
                    listFilteredBy.append($('<span class="badge text-dark bg-light border p-2 mr-2" style="font-size:14px;">')
                                    .text(arrFilter[i].description)
                                    .append($('<span class="closebtn bi bi-x ml-3">')
                                        .attr('i', i)
                                        .click(function () {
                                            arrFilter.splice($(this).attr('i'), 1);
                                            DisplayFilter();
                                            FilteredText();
                                        })
                                    )
                                )
                }

                if (arrFilter.length == 0) {
                    $('#divFilteredBy').hide();
                } else {
                    $('#divFilteredBy').show();
                }
            }

            function GetKnowledgeManagementNumber() {
                AjaxSend({
                    url: "KnowledgeBase/GetLastKMNumber",
                    success: function (vData) {
                        if (vData.Results.Rows.length > 0) {
                            $('#txtCaseID').val(vData.Results.Rows[0].LastNumber);
                        } else {
                            $('#txtCaseID').val(0);
                        }
                    }
                });
            }

            function LoadRequiredData() {
                var tempinterval = setInterval(function () {
                    if (typeof AddBreadCrumb != 'undefined') {
                        if (location.pathname.toLowerCase() != '/erp') {
                            $('.OrganizationNameHeader').text(sessionStorage.OrgName);
                            $(window).scrollTop(0);
                            //$('#mainContainer').css('margin-top', $('#divHeaderForProjects').height());

                            $('.ItemsBreadCrumbs').empty();
                            AddBreadCrumb({
                                text: 'Main Dashboard', Click: function () {
                                    $('#panMainDashboardKB').pageshow();
                                }
                            }, '.ItemsBreadCrumbs')
                        }
                        
                        GetKnowledgeBase();
                        GetAllLanguages();
                        GetAllProducts();
                        GetAllTopics();
                        GetGeneralSettings();
                        clearInterval(tempinterval)
                    }
                }, 100);
            }

            function GetGeneralSettings() {
                AjaxSend({
                    url: 'KnowledgeBase/GeneralSettings',
                    success: function (e) {
                        $('#txtKBServerIP').val(e.ServerUrl);
                    }
                })
            }

            function GetKnowledgeBase() {
                var tblKBbody = $('#tblKBbody');

                if ($.fn.DataTable.isDataTable("#dTableKb")) {
                    //check if datatable is created 
                    tblKBbody.empty()
                    dTableKb.destroy();
                }

                dTableKb = $('#dTableKb').DataTable({
                    retrieve: true,
                    "bdestroy": true,
                    "order": [[1, "asc"]],
                    "bProcessing": true,
                    "bServerSide": true,
                    "ajax": {
                        url: "/KnowledgeBase/GetAllKnowledgeManagementAndQuestionaire?OrgID=" + sessionStorage.OrgID,
                        type: 'POST'
                    },
                    "autoWidth": false,
                    "columns": [
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">#</b>' + data.RowNum + '.';
                            },
                            "width": "30px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Problem</b>' + data.Problem;
                            }
                        },
                        //{
                        //    "data": function (data, type, full, meta) {
                        //        return '<b class="ui-table-cell-label">Solution</b>' + (data.Solution || '');
                        //    }
                        //},
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Topic</b>' + (data.Topic || data.Topics || '');
                            },
                            "width": "150px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Product</b>' + (data.Product || data.Products || '');
                            },
                            "width": "150px"
                        },
                        //{
                        //    "data": function (data, type, full, meta) {
                        //        return '<b class="ui-table-cell-label">Language</b>' + (data.Language || '');
                        //    }
                        //},
                        {
                            "data": function (data, type, full, meta) {
                                return '<b class="ui-table-cell-label">Modified By</b>' + (data.EditedBy || '');
                            },
                            "width": "150px"
                        },
                        {
                            "data": function (data, type, full, meta) {
                                if (data.LastUpdated == null) {
                                    return "";
                                }
                                return '<b class="ui-table-cell-label">Last Interaction Date</b>' + $.format.date(new Date(data.LastUpdated), 'MMM dd, yyyy');
                            },
                            "width": "150px",
                            className: 'text-center'
                        },
                        {
                            "data": function (data, type, full, meta) {
                                return '<button data-role="none" class="btn btn-danger btnKbRemove" title="Remove"><span class="bi bi-trash"></span></button>'
                            }
                            ,
                            "width": "30px"
                        }
                    ],
                    "aoColumnDefs": [{ "bSortable": false, "aTargets": [0, 6] }],
                    "responsive": true,
                    "columnDefs": [{ "targets": 0, "orderable": false }],
                    "sDom": '<"top">rt<"bottom"ip><"clear">',
                    "initComplete": function () {
                        $('#dTableKb').removeClass('dataTable');
                    },
                    "fnDrawCallback": function (oSettings) {
                        //$('.btnKbRemove');//.button()
                    }
                    //"stateSave": true
                })

                $('#dTableKb tbody').unbind('click').on('click', 'td', function () {
                    var pData = dTableKb.row(this).data();
                    var kbserverurl = $('#txtKBServerIP').val();
                    kbserverurl = kbserverurl.slice(-1) == '/' ? kbserverurl.slice('/', -1) : kbserverurl;
                    if (dTableKb.cell(this).index().columnVisible == 6) {
                        RemoveProblem(pData.ProblemID)
                    } else {
                        clearKMFields();
                        GetSolution(pData);
                        $('#divCaseUrl').show();
                        $('#txtCaseUrl').val((kbserverurl == '' ? location.origin : kbserverurl) + '/Knowledgebase?OrgID=' + sessionStorage.OrgID + '&' + (pData.ProblemShortLink == null ? 't=' + pData.ProblemID : 't=' + pData.ProblemShortLink))
                        $('#panAddKnowledgeManagement').pageshow();

                        if (sessionStorage.SystemAdminFor != 'dtr') {
                            AddBreadCrumb({
                                text: 'View Article', Click: function () {
                                    $('#panAddKnowledgeManagement').pageshow();
                                }
                            }, '.ItemsBreadCrumbs')
                        }                        

                        $(window).scrollTop(0);
                    }
                })

                return;

                tblKBbody.empty().append('<tr><td colspan="9" style="text-align:center;">Please wait ...</td></tr>');

                AjaxSend({
                    url: 'KnowledgeBase/GetAllKnowledgeManagementAndQuestionaire',
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
                                        $('#panAddKnowledgeManagement').pageshow();
                                    })
                                )
                            }
                            //$('a');//.button('refresh')
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
                                        url: 'KnowledgeBase/RemoveCase',
                                        data: { ID: id },
                                        success: function (v) {
                                            if (v.Results == "removed") {
                                                WebToast('Successfully Removed');
                                                $('#dTableKb').DataTable().destroy();
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
                    url: 'KnowledgeBase/GetProblemSolution',
                    data: { ProblemID: data.ProblemID },
                    success: function (vData) {
                        $('#txtProblemDBID').val(data.ProblemID);
                        //$('#selTopic').val(data.TopicID);//.selectmenu('refresh');
                        GetAllSubTopics(data.SubTopicID);
                        //('#selProduct').val(data.ProductID);//.selectmenu('refresh');
                        $('#selLanguage').val(data.LanguageID);//.selectmenu('refresh');
                        $('#txtTags').val(data.Tags);
                        $('#txtCaseID').val(data.CaseNo);
                        $('#txtCase').val(data.Problem);
                        $('#txtProblemShortLink').val(data.ProblemShortLink);
                        arrSolution = (vData.Rows || []);
                        DisplaySolution();

                        $('#chbTopic-' + data.TopicID).prop('checked', true);
                        $('#chbSubTopic-' + data.SubTopicID).prop('checked', true);
                        $('#chbProduct-' + data.ProductID).prop('checked', true);

                        for (var i = 0; i < vData.Topics.length; i++){
                            $('#chbTopic-' + vData.Topics[i].TopicID).prop('checked', true);
                            $('#chbSubTopic-' + vData.Topics[i].SubTopicID).prop('checked', true);
                        }

                        for (var i = 0; i < vData.Products.length; i++) {
                            $('#chbProduct-' + vData.Products[i].ID).prop('checked', true);
                        }

                        $('.chbKb, .chbKbProduct');//.checkboxradio('refresh');
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
                        arrSolution[i].index = i;
                        ctrSolution = CInt2(arrSolution[i].No)
                        if (arrSolution[i].State != 'Remove') {
                            var ol = $('<ol>');
                            if (arrSolution[i].Step.length > 0) {
                                for (var m = 0; m < arrSolution[i].Step.length; m++) {
                                    if (arrSolution[i].Step[m].State != "Remove") {
                                        ol.append($('<li class="mb-3">')
                                            .html(HTMLDecode(arrSolution[i].Step[m].Step))
                                        )
                                    }
                                }
                            }
                            else {
                                ol = '';
                            }
                            listSolution.append($('<li class="list-group-item list-group-item-action">')
                                .data(arrSolution[i])
                                .append($('<div class="row">')
                                    .append($('<div class="col-auto">').text((i + 1) + '.'))
                                    .append($('<div class="col" style="overflow:hidden; word-break:break-word;">')
                                        .data(arrSolution[i])
                                        .attr('s', i)
                                        .html(arrSolution[i].Solution.replace(/\r\n|\r|\n/g, "<br />") + '<br><br>')
                                        .append(ol)
                                        .click(function () {
                                            arrAllowedTeam = [];
                                            arrIndexSolution = $(this).attr('s');
                                            editSolution = true;
                                            AddSolutionStep($(this).data().Step)
                                            arrSolutionSteps = (arrSolutionSteps.length > 0 ? arrSolutionSteps : $(this).data().Step);
                                            arrAllowedTeam = $(this).data().AllowedTeam;
                                            DisplayAllowedTeam(arrAllowedTeam);
                                            $('#panAddSolution').modal('show');
                                            $('#txtSolutionAndAnswer').val($(this).data().Solution).focus();
                                            $('input[name="chbPrivate"]').prop('checked', (CBool($(this).data().IsPublic) == true ? false : true));//.checkboxradio('refresh');

                                            if (CBool($(this).data().IsPublic) == false) {
                                                $('#divAllowedTeam').show();
                                            }
                                            else {
                                                $('#divAllowedTeam').hide();
                                            }
                                        })
                                    )
                                    .append($('<div class="col-auto">')
                                        .data(arrSolution[i])
                                        .append($('<button data-role="none" title="Remove" class="btn btn-danger">')
                                            .append('<span class="bi bi-trash"></span>')
                                        )
                                        .click(function () {
                                            var data = $(this).data();

                                            NewMessageBox('Are you sure you want to remove this record?', 'Message', [
                                                {
                                                    Text: 'Yes',
                                                    Click: function () {
                                                        var no = data.No;

                                                        if (data.State == 'Add') {
                                                            arrSolution = $.grep(arrSolution, function (e) {
                                                                return e.No != no;
                                                            })
                                                        }
                                                        else {
                                                            arrSolution[data.index].State = 'Remove';
                                                        }
                                                        DisplaySolution();
                                                    }
                                                }, { Text: 'No' }
                                            ])
                                        })
                                    )
                                )
                            )
                        }

                        if (arrSolution[i].Step.length > 0) {
                            for (var m = 0; m < arrSolution[i].Step.length; m++) {
                                LoadImages(arrSolution[i].Step[m])
                            }
                        }

                    }
                }
            }

            function NewSteps(callback) {
                if ($.trim($('#txtNewSteps').html()) == '') {
                    WebToast('Please enter step');
                    return;
                }

                var steps = $('#txtNewSteps');
                steps.find('img').removeAttr('src');

                if (editSolutionStep == false) {
                    ctrSolutionStep = ctrSolutionStep + 1;

                    arrSolutionSteps.push({
                        StepID: NewID(),
                        No: ctrSolutionStep,
                        Step: HTMLEncode(steps.html()),
                        State: 'Add',
                        Images: arrNewImages
                    })
                }
                else {
                    arrSolutionSteps[arrIndexStep].Step = HTMLEncode(steps.html())

                    //Check to see if images in textarea are deleted
                    if (arrSolutionSteps[arrIndexStep].Images != undefined) {
                        var arrImages = arrSolutionSteps[arrIndexStep].Images;
                        var newArrImages = arrImages;
                        for (var i = 0; i < arrImages.length; i++) {
                            if ($('#txtNewSteps').find('img[imgid="' + arrImages[i].ImgID + '"]').length == 0 && arrImages[i].Status == 'new') {
                                var id = arrImages[i].ImgID;
                                newArrImages = $.grep(newArrImages, function (e) {
                                    return e.ImgID != id;
                                })

                                arrSolutionSteps[arrIndexStep].Images = newArrImages;
                            }
                            else if ($('#txtNewSteps').find('img[imgid="' + arrImages[i].ImgID + '"]').length == 0 && arrImages[i].Status == 'old') {
                                arrImages[i].Status = "delete";
                            }
                        }
                    }
                }
                AddSolutionStep(arrSolutionSteps);
                $('#txtNewSteps').val('');
                editSolutionStep = false;
                $('#panAddSteps').modal('hide');

                if (typeof callback == 'function') {
                    setTimeout(function () {
                        callback();
                    }, 500)
                }
            }

            function SortByNo() {
                arrSolutionSteps.sort(function (a, b) {
                    var a1 = CStr2(a.No).toLowerCase(), b1 = CStr2(b.No).toLowerCase();
                    if (a1 == b1) return 0;
                    return a1 > b1 ? 1 : -1;
                });
            }

            function AddSolutionStep(arrSolutionSteps) {
                var listSolutionSteps = $('#listSolutionSteps');

                listSolutionSteps.empty();

                for (var i = 0; i < arrSolutionSteps.length; i++) {
                    //arrSolutionSteps[i].No = i + 1;
                    ctrSolutionStep = CInt2(arrSolutionSteps[i].No);
                    if (arrSolutionSteps[i].State != 'Remove') {
                        listSolutionSteps.append($('<li class="list-group-item">')
                            .data(arrSolutionSteps[i])
                            .append($('<div class="row">')
                                .css('cursor', 'pointer')
                                .append($('<div class="col-auto">').text((i + 1) + '.'))
                                .append($('<div class="col" style="overflow:hidden; word-break:break-word;">')
                                    .data(arrSolutionSteps[i])
                                    .attr('i', i)
                                    .html(HTMLDecode(arrSolutionSteps[i].Step)
                                    )
                                    .click(function () {
                                        arrNewImages = [];
                                        arrNewImages = $(this).data().Images;
                                        editSolutionStep = true;
                                        $('#panAddSteps').modal('show');;
                                        $('#txtNewSteps').html(HTMLDecode($(this).data().Step));
                                        LoadImages($(this).data())
                                        placeCaretAtEnd($('#txtNewSteps')[0])
                                        arrIndexStep = $(this).attr('i'); // index
                                    })
                                )
                                .append($('<div class="col-auto">')
                                    .data(arrSolutionSteps[i])
                                    .attr('x', i)
                                    .append($('<button data-role="none" title="Remove" class="btn btn-danger">')
                                        .append('<span class="bi bi-trash"></span>')
                                    )
                                    .click(function () {
                                        var data = $(this).data();
                                        var ind = $(this).attr('x');

                                        NewMessageBox('Are you sure you want to remove this record?', 'Message', [
                                            {
                                                Text: 'Yes',
                                                Click: function () {
                                                    var no = data.No;

                                                    if (data.State == 'Add') {
                                                        arrSolutionSteps = $.grep(arrSolutionSteps, function (e) {
                                                            return e.No != no;
                                                        })
                                                    }
                                                    else {
                                                        arrSolutionSteps[CInt2(ind)].State = 'Remove';
                                                    }
                                                    AddSolutionStep(arrSolutionSteps);
                                                }
                                            }, { Text: 'No' }
                                        ])
                                    })
                                )
                            )
                        )
                    }

                    LoadImages(arrSolutionSteps[i])
                }
            }

            function LoadImages(data) {
                if (data.Images != undefined) {
                    for (var m = 0; m < data.Images.length; m++) {
                        if (data.Images[m].Status == "new") {
                            var reader = new FileReader();
                            reader.onload = (function (theFile) {
                                var imgID = theFile.ImgID;
                                return function (e) {
                                    var src = e.target.result;
                                    $('[imgid="' + imgID + '"]').attr('src', src).click(function (e) {
                                        e.preventDefault();
                                        e.stopPropagation();
                                        $('[data-lightbox="' + $(this).attr('imgid') + '"]').click();
                                    });;

                                    if ($('[data-lightbox="' + imgID + '"]').length == 0) {
                                        $('#divlinkimg').append($('<a>')
                                            .attr({
                                                href: src,
                                                'data-lightbox': imgID
                                            })
                                        )
                                    }
                                };
                            })(data.Images[m]);
                            reader.readAsDataURL(data.Images[m].file);
                        } else {
                            var src = '../../../KBPictures/' + data.Images[m].Location;
                            $('[imgid="' + data.Images[m].ImgID + '"]').attr('src', src).click(function (e) {
                                e.preventDefault();
                                e.stopPropagation();
                                $('[imgid="' + $(this).attr('imgid') + '"][data-lightbox="' + $('#txtProblemDBID').val() + '"]').unbind('click').click();
                            });

                            if ($('[imgid="' + data.Images[m].ImgID + '"][data-lightbox="' + $('#txtProblemDBID').val() + '"]').length == 0) {
                                $('#divlinkimg').append($('<a>')
                                    .attr({
                                        href: src,
                                        'data-lightbox': $('#txtProblemDBID').val(),
                                        imgid: data.Images[m].ImgID
                                    })
                                )
                            }                            
                        }
                    }

                    lightbox.option({
                        'fadeDuration': 200,
                        'resizeDuration': 200,
                        'wrapAround': true,
                        'imageFadeDuration': 100,
                        'alwaysShowNavOnTouchDevices': true
                    })
                }
            }

            function SaveObject(callback) {
                objID = NewID();

                var pData = {};
                pData.ID = (isEditMode ? sessionStorage.ModifiedID : objID);
                pData.Description = $('#txtObject').val();
                pData.TopicID = sessionStorage.TopicID; //$('#selTopic').val();
                pData.IsEdit = isEditMode;

                var lnk = '';

                if (objSelected == 'Topic') {
                    lnk = 'SaveTopic';
                } else if (objSelected == 'Product') {
                    lnk = 'SaveProduct';
                } else if (objSelected == 'Language') {
                    lnk = 'SaveLanguage';
                } else if (objSelected == 'SubTopic') {
                    lnk = 'SaveSubTopic';
                }

                AjaxSend({
                    url: 'KnowledgeBase/' + lnk,
                    data: pData,
                    success: function (vData) {

                        if ((vData.Results || '') == "True") {
                            if (objSelected == 'Topic') {
                                //GetAllTopics();
                            } else if (objSelected == 'Product') {
                                //GetAllProducts();
                            }
                            else if (objSelected == 'Language') {
                                GetAllLanguages();
                            } else if (objSelected == 'SubTopic') {
                                //GetAllSubTopics();
                            }

                            if (callback != undefined) {
                                if (typeof callback != "function") {
                                    NewMessageBox('"Callback" must be a function');
                                } else {
                                    if (!isEditMode) {
                                        callback(pData);
                                    } else {
                                        callback();
                                    }
                                }
                            }

                            $('#panAddObject').modal('hide');;
                        }
                        else {
                            NewMessageBox('An error occured while saving Case. Please contact Mobiz ASAP. Thank you.')
                        }
                    }
                });
            }

            function GetCheckedTopic() {
                var topic = $('.chbTopic:checked');
                var chkTopic = [], ctrLoopTopic = 1;

                if (topic.length == 0) {
                    NewMessageBox('Please select at least one topic', 'Message');
                    return false;
                }

                topic.each(function () {
                    var data = $(this).data('data');
                    var subtopic = $('[topicid="' + data.ID + '"]').find('input[type="checkbox"]:checked');
                    var chkSubTopic = [];

                    if (subtopic.length == 0) {
                        NewMessageBox('Please select at least one subtopic for <b>' + data.Topic + '</b>', 'Message');
                        return false;
                    } else {
                        subtopic.each(function () {
                            chkSubTopic.push($(this).data('data'))
                        })
                    }
                    chkTopic.push({
                        TopicID: data.ID,
                        Topic: data.Topic,
                        SubTopic: chkSubTopic
                    })

                    if (topic.length == ctrLoopTopic) {
                        SaveCase(chkTopic);
                    }
                    ctrLoopTopic += 1;
                })
            }

            function SaveCase(chktopic) {
                var chbKbProduct = $('.chbKbProduct:checked'), arrProduct = [];                

                if ($('#selTopic').val() == "" || $('#selTopic').val() == "AddNew") {
                    WebToast('Please select topic.');
                    return;
                }

                if ($('#selProduct').val() == "" || $('#selProduct').val() == "AddNew") {
                    WebToast('Please select product.');
                    return;
                }

                if ($('#selLanguage').val() == "" || $('#selLanguage').val() == "AddNew") {
                    WebToast('Please select language.');
                    return;
                }

                if ($.trim($('#txtCase').val()) == "") {
                    WebToast('Please enter your question.');
                    return;
                }

                if (arrSolution.length == 0) {
                    WebToast('Please add at least one answer.');
                    return;
                }

                if (chbKbProduct.length == 0) {
                    WebToast('Please select topic.');
                    return;
                }

                chbKbProduct.each(function (e) {
                    arrProduct.push($(this).data('data'));
                })

                var pData = {};
                pData.CaseID = $('#txtCaseID').val();
                pData.Case = $('#txtCase').val();
                //pData.Solution = $('#txtSolution').val();
                //pData.Steps = $('#txtSteps').val();
                pData.Topic = 'NULL'; //$('#selTopic').val();
                pData.SubTopic = 'NULL'; //$('#selSubTopic').val();
                pData.Product = 'NULL'; //('#selProduct').val();
                pData.Language = $('#selLanguage').val();
                pData.Tags = $('#txtTags').val();
                pData.Solution = arrSolution;
                pData.ProblemID = $('#txtProblemDBID').val();
                pData.Date = $.format.date(new Date(), 'MMMM dd,yyyy hh:mm a');
                pData.ProblemShortLink = $.trim($('#txtProblemShortLink').val());
                pData.ModifiedBy = sessionStorage.PeopleID;
                pData.ArticleTopic = chktopic;
                pData.Products = arrProduct;
                console.log(pData)
                //return

                AjaxSend({
                    url: 'KnowledgeBase/SaveCase',
                    data: pData,
                    dontHandleSessionExpired: true,
                    success: function (vData) {
                        if ((vData.Results || '') == "True") {

                            editSolution = false;
                            $('#dTableKb').DataTable().destroy();
                            arrSolutionSteps = [];
                            cntUploadedPic = 0;
                            cntUploadPic = 0;
                            isAddNewStepsTruPic = false;
                            GetKnowledgeBase();
                            $('#panMainDashboardKB').pageshow();
                            WebToast('Successfully Saved.')
                        }
                        else {
                            NewMessageBox('An error occured while saving Case. Please contact Mobiz ASAP. Thank you.')
                        }

                    }
                });
            }

            var objSelected = '';
            var objID = '';

            function GetAllTopics() {
                AjaxSend({
                    url: 'KnowledgeBase/GetAllTopics',
                    success: function (vData) {
                        var vRows = vData.Results;

                        if (vRows.length > 0) {
                            $('#selTopic').empty();
                            $('#selFilterTopic').empty();

                            $('#selFilterTopic').append(new Option('Please Select', '', true));

                            for (var i = 0; i <= vRows.length - 1; i++) {
                                $('#selTopic').append(new Option(vRows[i].Topic, vRows[i].ID, false));
                                $('#selFilterTopic').append(new Option(vRows[i].Topic, vRows[i].ID, false));
                            }

                            if (objID != '') {
                                $('#selTopic').val(objID);
                                $('#selFilterTopic').val(objID);
                            } else {
                                $('#selTopic').val(vRows[0].ID);
                            }
                        }

                        $('#selTopic').append(new Option('Add New Topic', 'AddNew', false));

                        try {
                            //$('#selTopic');//.selectmenu('refresh');
                            $('#selFilterTopic');//.selectmenu('refresh');
                        } catch (e) {
                            //('#selTopic');//.selectmenu();
                            $('#selFilterTopic');//.selectmenu();
                        }

                        if ($('#selTopic').val() != "AddNew" && $('#selTopic').val() != "") {
                            GetAllSubTopics()
                        }

                        var listTopic = $('#listTopic');

                        for (var i = 0; i <= vRows.length - 1; i++) {
                            var ul = $('<div topicid="' + vRows[i].ID + '" class="divsubtopic" style="display:none; margin-left:30px;">')
                            for (var s = 0; s <= vRows[i].SubTopic.length - 1; s++) {
                                DisplayManuallyAddedSubTopic(ul, vRows[i].SubTopic[s])
                            }

                            DisplayManuallyAddedTopic(ul, vRows[i]);
                        }

                        $('.chbKb');//.checkboxradio();//.checkboxradio('refresh');
                    }
                });
            }

            function DisplayManuallyAddedTopic(ul, vData) {
                var listTopic = $('#listTopic');

                listTopic.append($('<div class="form-check mb-2">')
                                        .append($('<input class="chbKb chbTopic form-check-input" type="checkbox" id="chbTopic-' + vData.ID + '" value="' + vData.ID + '" data-iconpos="notext">')
                                            .data('data', vData)
                                            .change(function (e) {
                                                //if (!$(this).is(':checked')) {
                                                //    $('[topicid="' + $(this).data('data').ID + '"]').find('[type="checkbox"]').removeAttr('checked');//.checkboxradio('refresh');
                                                //}
                                            })
                                        )
                                        .append($('<label class="form-check-label" id="lbl-' + vData.ID + '" for="chbTopic-' + vData.ID + '">')
                                            .text(vData.Topic)
                                            .append($('<span class="menuoptions ml-3" style="display:none;">')
                                                .append($('<a data-role="none" href="#" class="custom-link">')
                                                    .data('data', vData)
                                                    .text('expand')
                                                    .click(function (e) {
                                                        e.stopPropagation();
                                                        e.preventDefault();
                                                        var dis = $(this);
                                                        if (dis.text() == "expand") {
                                                            dis.parent().parent().parent().next().show();
                                                            dis.text('collapse')
                                                        } else {
                                                            dis.parent().parent().parent().next().hide();
                                                            dis.text('expand')
                                                        }
                                                    })
                                                ).append('&nbsp; | &nbsp;')
                                                .append($('<a data-role="none" href="#" class="custom-link">')
                                                    .data('data', vData)
                                                    .text('new subtopic')
                                                    .click(function (e) {
                                                        e.stopPropagation();
                                                        e.preventDefault();
                                                        objSelected = 'SubTopic';
                                                        isEditMode = false;
                                                        var data = $(this).data('data');
                                                        sessionStorage.TopicID = data.ID;
                                                        $('#lblAddObject').html('Add SubTopic');
                                                        $('#lblAddObjectTitle').html('SubTopic');
                                                        $('#panAddObject').modal('show');;
                                                        $('#txtObject').val('').focus();

                                                        $('#btnSaveObject').unbind('click').click(function () {
                                                            SaveObject(function (vData) {
                                                                var pData = {
                                                                    ID: vData.ID,
                                                                    SubTopic: vData.Description
                                                                }
                                                                WebToast('Successfully saved', 'top');
                                                                DisplayManuallyAddedSubTopic($('[topicid="' + data.ID + '"]'), pData);
                                                                $('.chbKb');//.checkboxradio();//.checkboxradio('refresh');
                                                            })
                                                        })
                                                    })
                                                )
                                                .append('&nbsp; | &nbsp;')
                                                .append($('<a data-role="none" href="#" class="custom-link">')
                                                    .data('data', vData)
                                                    .text('edit name')
                                                    .click(function (e) {
                                                        e.stopPropagation();
                                                        e.preventDefault();
                                                        objSelected = 'Topic';
                                                        isEditMode = true;
                                                        var data = $(this).data('data');
                                                        sessionStorage.TopicID = data.ID;
                                                        sessionStorage.ModifiedID = data.ID;
                                                        $('#lblAddObject').html('Edit Topic');
                                                        $('#lblAddObjectTitle').html('Topic');
                                                        $('#panAddObject').modal('show');;
                                                        $('#txtObject').val(data.Topic).focus();

                                                        $('#btnSaveObject').unbind('click').click(function () {
                                                            SaveObject(function () {
                                                                WebToast('Successfully modified', 'top');
                                                                $('#lbl-' + data.ID).text($('#txtObject').val());
                                                                isEditMode = false;
                                                            })
                                                        })
                                                    })
                                                )
                                                .append('&nbsp; | &nbsp;')
                                                .append($('<a data-role="none" href="#" class="custom-link">')
                                                    .data('data', vData)
                                                    .text('remove')
                                                    .click(function (e) {
                                                        e.stopPropagation();
                                                        e.preventDefault();
                                                        var data = $(this).data('data');
                                                        Remove(data, 'topic');
                                                    })
                                                )
                                            )
                                        )
                                        .hover(function () {
                                            $(this).find('.menuoptions').show();
                                        }, function () {
                                            $(this).find('.menuoptions').hide();
                                        })
                                    ).append(ul)
            }

            function Remove(data, type) {
                AjaxSend({
                    url: '/KnowledgeBase/CountLinkedProblem',
                    data: { Type: type, ID: data.ID },
                    success: function (e) {
                        var msg = '';

                        if (CInt2(e.Results) > 0) {
                            msg += '<br><br>There is/are <b>' + e.Results + '</b> question(s) linked to this ' + type + ', if Yes, all questions will be unlinked to this ' + type + '.';
                        }

                        NewMessageBox('Are you sure you want to remove ' + type + ' <b>' + (data.Topic || data.SubTopic || data.Product) + '</b>?' + msg, 'Confirmation Dialog', [
                            {
                                Text: 'Yes', Click: function () {
                                    AjaxSend({
                                        url: '/KnowledgeBase/Remove',
                                        data: { Type: type, ID: data.ID },
                                        success: function (e) {
                                            if (e.Results != undefined) {
                                                $('#lbl-' + data.ID).parent().remove();
                                                $('[topicid="' + data.ID + '"]').remove();
                                            } else {
                                                NewMessageBox(e.Error, 'Error');
                                            }                                            
                                        }
                                    })
                                }
                            }, { Text: 'No' }
                        ])
                    }
                })
            }

            function DisplayManuallyAddedSubTopic(ul, vData) {
                ul.append($('<div class="form-check mb-2">')
                        .append($('<input class="chbKb form-check-input" type="checkbox" id="chbSubTopic-' + vData.ID + '" value="' + vData.ID + '" data-iconpos="notext">')
                                .data('data', vData)
                            )
                        .append($('<label class="form-check-label" id="lbl-' + vData.ID + '" for="chbSubTopic-' + vData.ID + '">')
                            .text(vData.SubTopic)
                            .append($('<span class="menuoptions" style="display:none;">')
                                .append('&nbsp; | &nbsp;')
                                .append($('<a data-role="none" href="#" class="custom-link">')
                                    .data('data', vData)
                                    .text('edit name')
                                    .click(function (e) {
                                        e.stopPropagation();
                                        e.preventDefault();
                                        objSelected = 'SubTopic';
                                        isEditMode = true;
                                        var data = $(this).data('data');
                                        sessionStorage.ModifiedID = data.ID;
                                        $('#lblAddObject').html('Edit Subtopic');
                                        $('#lblAddObjectTitle').html('Subtopic');
                                        $('#panAddObject').modal('show');;
                                        $('#txtObject').val(data.SubTopic).focus();

                                        $('#btnSaveObject').unbind('click').click(function () {
                                            SaveObject(function () {
                                                WebToast('Successfully modified', 'top');
                                                $('#lbl-' + data.ID).text($('#txtObject').val());
                                                isEditMode = false;
                                            })
                                        })
                                    })
                                )
                                .append('&nbsp; | &nbsp;')
                                .append($('<a data-role="none" href="#" class="custom-link">')
                                    .data('data', vData)
                                    .text('remove')
                                    .click(function (e) {
                                        e.stopPropagation();
                                        e.preventDefault();
                                        var data = $(this).data('data');
                                        Remove(data, 'subtopic');
                                    })
                                )
                            )
                        )
                        .hover(function () {
                            $(this).find('.menuoptions').show();
                        }, function () {
                            $(this).find('.menuoptions').hide();
                        })
                    )
            }

            function GetAllSubTopics(id) {
                var data = { TopicID: $('#selTopic option:selected').val() }
                AjaxSend({
                    url: 'KnowledgeBase/GetAllSubTopics',
                    data: data,
                    success: function (vData) {
                        var vRows = vData.Results;

                        $('#selSubTopic').empty();
                        $('#selSubTopic').append(new Option('Select Topic', '', false));

                        if (vRows.length > 0) {
                            for (var i = 0; i <= vRows.length - 1; i++) {
                                $('#selSubTopic').append(new Option(vRows[i].SubTopic, vRows[i].ID, false));
                            }

                            if (id != undefined) {
                                $('#selSubTopic').val(id);
                            }
                            else {
                                if (objID != '') {
                                    $('#selSubTopic').val(objID);
                                } else {
                                    $('#selSubTopic').val(vRows[0].ID);
                                }
                            }

                        } else {
                            $('#selSubTopic').append(new Option('Select Topic', '', false));
                        }

                        $('#selSubTopic').append(new Option('Add New SubTopic', 'AddNew', false));

                        try {
                            $('#selSubTopic');//.selectmenu('refresh');
                        } catch (e) {
                            $('#selSubTopic');//.selectmenu();
                        }
                    }
                });
            }

            function GetAllFilterSubTopics(id) {
                var data = { TopicID: id }
                AjaxSend({
                    url: 'KnowledgeBase/GetAllSubTopics',
                    data: data,
                    success: function (vData) {
                        var vRows = vData.Results.Rows;

                        $('#selFilterSubTopic').empty();

                        $('#selFilterSubTopic').append(new Option('Please Select', '', true));

                        if (vRows.length > 0) {
                            for (var i = 0; i <= vRows.length - 1; i++) {
                                $('#selFilterSubTopic').append(new Option(vRows[i].SubTopic, vRows[i].ID, false));
                            }

                            if (id != undefined) {
                                $('#selFilterSubTopic').val(id);
                            }
                            else {
                                if (objID != '') {
                                    $('#selFilterSubTopic').val(objID);
                                }
                            }
                            $('#selFilterSubTopic');//.selectmenu('enable');
                        } else {
                            $('#selFilterSubTopic');//.selectmenu('disable');
                        }

                        try {
                            $('#selFilterSubTopic');//.selectmenu('refresh');
                        } catch (e) {
                            $('#selFilterSubTopic');//.selectmenu();
                        }
                    }
                });
            }

            function GetAllProducts() {
                AjaxSend({
                    url: 'KnowledgeBase/GetAllProducts',
                    success: function (vData) {
                        var vRows = vData.Results.Rows;
                        var listProduct = $('#listProduct');

                        if (vRows.length > 0) {
                            $('#selProduct').empty();
                            $('#selFilterProduct').empty();

                            $('#selFilterProduct').append(new Option('Please Select', '', false));

                            for (var i = 0; i < vRows.length; i++) {
                                listProduct.append($('<div class="form-check">')
                                    .append($('<input class="chbKbProduct form-check-input" type="checkbox" id="chbProduct-' + vRows[i].ID + '" value="' + vRows[i].ID + '" data-iconpos="notext">')
                                        .data('data', vRows[i])
                                    )
                                    .append($('<label class="form-check-label" id="lbl-' + vRows[i].ID + '" for="chbProduct-' + vRows[i].ID + '">')
                                        .text(vRows[i].Product)
                                        .append($('<span class="menuoptions" style="display:none;">')
                                            .append('&nbsp; | &nbsp;')
                                            .append($('<a data-role="none" href="#" class="custom-link">')
                                                .data('data', vRows[i])
                                                .text('remove')
                                                .click(function (e) {
                                                    e.stopPropagation();
                                                    e.preventDefault();
                                                    var data = $(this).data('data');
                                                    Remove(data, 'product');
                                                })
                                            )
                                        )
                                    ).hover(function () {
                                        $(this).find('.menuoptions').show();
                                    }, function () {
                                        $(this).find('.menuoptions').hide();
                                    })
                                )
                                $('#selProduct').append(new Option(vRows[i].Product, vRows[i].ID, false));
                                $('#selFilterProduct').append(new Option(vRows[i].Product, vRows[i].ID, false));
                            }

                            if (objID != '') {
                                $('#selProduct').val(objID);
                                $('#selFilterProduct').val(objID);
                            } else {
                                $('#selProduct').val(vRows[0].ID);
                                $('#selFilterProduct').val(objID);
                            }
                        }

                        $('#selProduct').append(new Option('Add New Product', 'AddNew', false));
                    }
                });
            }

            function GetAllLanguages() {
                AjaxSend({
                    url: 'KnowledgeBase/GetAllLanguages',
                    success: function (vData) {
                        var vRows = vData.Results.Rows;

                        if (vRows.length > 0) {
                            $('#selLanguage').empty();
                            $('#selFilterLanguage').empty();

                            $('#selFilterLanguage').append(new Option('Please Select', '', true));

                            for (var i = 0; i <= vRows.length - 1; i++) {
                                $('#selLanguage').append(new Option(vRows[i].Language, vRows[i].ID, false));
                                $('#selFilterLanguage').append(new Option(vRows[i].Language, vRows[i].ID, false));
                            }

                            if (objID != '') {
                                $('#selLanguage').val(objID);
                                $('#selFilterLanguage').val(objID);
                            } else {
                                $('#selLanguage').val(vRows[0].ID);
                                //$('#selFilterLanguage').val(vRows[0].ID);
                            }
                        }

                        $('#selLanguage').append(new Option('Add New Language', 'AddNew', false));

                        try {
                            $('#selLanguage');//.selectmenu('refresh');
                            $('#selFilterLanguage');//.selectmenu('refresh');
                        } catch (e) {
                            $('#selLanguage');//.selectmenu();
                            $('#selFilterLanguage');//.selectmenu();
                        }
                    }
                });
            }

            function clearKMFields() {
                $('#txtProblemDBID').val('');
                $('#txtProblemShortLink').val('');
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
                isEditMode = false;
                $('.chbKb, .chbKbProduct').removeAttr('checked');//.checkboxradio('refresh')
            }

            function ClearSolutionField() {
                $('#txtSolutionAndAnswer').val('');
                $('#listSolutionSteps').empty();
            }

            function GotoSigninPage() {

                var pParam = {};
                if (IsMobile)
                    pParam.IsMobile = 1;
                pParam = $.param(pParam);
                window.location = '/SignIn/kb?' + pParam;

            }; // GotoSigninPage

            function SignOut() {
                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {

                        AjaxSend({
                            url: "CRMDashboard/setSignOut",
                            success: function (vData) {
                                window.open(GetPathFormat("/SignIn/kb?mode=logout"), "_self"); // window.open(GetPathFormat("/People"), "_self"); EDITED BY: JANINE , Aug 3 4pm
                            }
                        });

                    },
                    error: function (vError) {
                        GotoSigninPage();
                    }
                })
            }

        }) // end $(document)
    })
}) // end require