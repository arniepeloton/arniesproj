require(['common'], function () {
    require(['/Content/bootstrap/js/bootstrap.min.js'], function () {
        $(document).ready(function () {
            var respondentid = '', arrExistingFiles = [];

            $('body')
            .on('click', '#btnMyProfile', function () {
                location = location.origin + '/Konstruksyon/MyProfile'
            })
            .on('click', '#btnLogout', function () {
                SignOut();
            })
            .on('click', '#btnTakeSurvey', function () {
                BrowsePopup('Survey/GetListOfSurvey', 'Select Permit', [
                    {
                        DisplayText: [{ Column: 'Title' }],
                        Click: function (e) {
                            window.open(location.origin + '/Survey/Questionaire?q=' + e.Link + '&ClientID=' + sessionStorage.PeopleID, 'open');
                            //AjaxSend({
                            //    url: 'Konstruksyon/CheckIfUserAlreadyTakeSurvey',
                            //    data: {
                            //        QID: e.ID,
                            //        ClientID: sessionStorage.PeopleID
                            //    },
                            //    success: function (a) {
                            //        if(a.Results == 'done'){
                            //            WebToast('Sorry! You have already requested this permit.');
                            //        } else {
                                        
                            //        }
                            //    }
                            //})                            
                        },
                        AutoSearch: true,
                        WithSearchbox: false
                    }
                ])
            })
            .on('click', '#btnKonstruksyonAttachFiles', function () {
                arrExistingFiles = [];
                $('#fileKonstruksyonFiles').click();
            })
            .on('change', '#fileKonstruksyonFiles', function (e) {
                e.preventDefault();
                var fileUpload = $(this).get(0);
                var files = fileUpload.files;
                var data = new FormData();

                for (var i = 0; i < files.length; i++) {
                    data.append(files[i].ID, files[i]);
                }

                CheckFileName(data, data)
                $(this).val('');
            })

            $(function () {
                LoginDetails();                
            })

            function CheckFileName(data, formData) {
                NewMessageBox('Checking file ...', 'Message');
                AjaxSend({
                    url: '/Konstruksyon/CheckFileName?OrgID=' + sessionStorage.OrgID + '&respondentid=' + respondentid,
                    type: 'POST',
                    data: data,
                    async: true,
                    cache: false,
                    contentType: false,
                    processData: false,
                    success: function (vData) {
                        RenameExistingFile(vData, formData);
                    }
                });
            }

            function RenameExistingFile(vData, formData) {
                var vRows = vData.data, cnt = 1;
                var divExistingFile = $('#divExistingFile');

                divExistingFile.empty();

                if (vRows.length > 0) {
                    for (var i = 0; i < vRows.length; i++) {
                        var isExist = false, ind, data;

                        $.grep(arrExistingFiles, function (e, inx) {
                            if (e.FileName == vRows[i].FileName) {
                                isExist = true;
                                ind = inx;
                                return false; //stop loop
                            }
                        })

                        if (!isExist) {
                            data = vRows[i];
                            arrExistingFiles.push(vRows[i]);
                        } else {
                            data = arrExistingFiles[ind];
                        }

                        divExistingFile.append($('<div class="row">')
                            .append($('<div class="col-sm-1">')
                                .text(cnt)
                            )
                            .append($('<div class="col-sm-9">')
                                .append($('<input data-role="none" type="text" class="form-control">')
                                    .data('data', data)
                                    .val(data.FileName)
                                    .blur(function () {
                                        var data = $(this).data('data');
                                        var ind;

                                        $.grep(arrExistingFiles, function (e, inx) {
                                            if (e.FileName == data.FileName) {
                                                ind = inx;
                                                return false; //stop loop
                                            }
                                        })

                                        vRows[ind].NewName = $.trim($(this).val());
                                    })
                                )
                            )
                            .append($('<div class="col-sm-1">')
                                .append($('<button data-role="none" class="btn custom-btn-theme-d btnCircle-xs">')
                                    .data('data', data)
                                    .append('<span class="glyphicon glyphicon-remove"><span>')
                                    .click(function () {
                                        var data = $(this).data('data');
                                        var ind;

                                        $.grep(arrExistingFiles, function (e, inx) {
                                            if (e.FileName == data.FileName) {
                                                ind = inx;
                                                return false; //stop loop
                                            }
                                        })

                                        vRows[ind].State = 'remove'
                                        $(this).parent().parent().remove();
                                    })
                                )
                            ).append('<br><br><br>')
                        )

                        cnt += 1;
                    }

                    $('#btnCRMContinueUpload').unbind().click(function () {
                        CheckFileName({ Files: vRows }, formData);
                        $('#popCRMFileExist').popup('close');
                    })
                    NewMessageBoxClose();
                    $('#popCRMFileExist').popup('open');
                } else {
                    NewMessageBox('Uploading file(s). Please wait ....', 'Message');
                    
                    AjaxSend({
                        url: '/Konstruksyon/UploadFiles?OrgID=' + sessionStorage.OrgID + '&respondentid=' + respondentid + '&rename=' + JSON.stringify(arrExistingFiles) + '&DateNow=' + $.format.date(new Date(), 'MMMM dd,yyyy hh:mm:ss a'),
                        type: 'POST',
                        data: formData,
                        async: true,
                        cache: false,
                        contentType: false,
                        processData: false,
                        success: function (vData) {
                            if (vData.Results == 'uploaded') {
                                NewMessageBoxClose();
                                WebToast('File(s) has been successfully uploaded', 'top');
                                GetFiles();
                            } else {
                                NewMessageBox(vData.Results, 'Message');
                            }
                        }
                    });
                }
            }

            function GetFiles() {
                AjaxSend({
                    url: 'Konstruksyon/GetFiles',
                    data: {
                        RespondentID: respondentid
                    },
                    success: function (e) {
                        var tbodyKonstruksyonFile = $('#tbodyKonstruksyonFile');
                        var vRows = e.Results.Rows;

                        tbodyKonstruksyonFile.empty();

                        if(vRows.length > 0){
                            for (var i = 0; i < vRows.length; i++){
                                tbodyKonstruksyonFile.append($('<tr>')
                                    .append($('<td>').text(i + 1))
                                    .append($('<td>').text(vRows[i].FileName))
                                    .append($('<td>').text($.format.date(new Date(vRows[i].LastUpdated), 'MM-dd-yyyy hh:mm a')))
                                    .append($('<td>').append($('<button data-role="none" class="btn custom-btn-theme-d btnCircle-xs">')
                                        .append($('<img class="img-download">'))
                                        .data('data', vRows[i])
                                        .click(function (e) {
                                            var data = $(this).data('data');

                                            window.location = MergeUrlParameters({
                                                RespondentID: respondentid,
                                                FileName: data.FileName
                                            }, '/Konstruksyon/Download');
                                        })
                                    ))
                                )
                            }
                        } else {
                            tbodyKonstruksyonFile.append($('<tr>')
                                    .append($('<td colspan="4">').text('No result(s) found'))
                                )
                        }
                    }
                })
            }

            function GetCustomerBuildingPermit() {
                var tbodyCustomerBuildingPermit = $('#tbodyCustomerBuildingPermit');

                tbodyCustomerBuildingPermit.empty();

                AjaxSend({
                    url: 'Konstruksyon/GetCustomerBuildingPermit',
                    success: function (e) {
                        var vRows = e.data;

                        if(vRows.length > 0){
                            for (var i = 0; i < vRows.length; i++){
                                tbodyCustomerBuildingPermit.append($('<tr>')
                                        .data('data', vRows[i])
                                        .append($('<td>')
                                            .text((i + 1) + '.')
                                        )
                                        .append($('<td>')
                                            .text(vRows[i].ProjectName)
                                        )
                                        .append($('<td>')
                                            .text(vRows[i].Title)
                                        )
                                        .append($('<td>')
                                            .text($.format.date(new Date(vRows[i].DateResponded), 'MMM dd, yyyy'))
                                        )
                                        .append($('<td>')
                                            .text((CStr2(vRows[i].Status) == '' ? 'Pending' : vRows[i].Status))
                                        )
                                        .click(function () {
                                            var data = JSON.parse(JSON.stringify($(this).data('data')));
                                            respondentid = data.RespondentID;
                                            $('#lblKonstruksyonROQTitle').text(data.Title);
                                            $('#btnKonstruksyonRODLReq').unbind('click').click(function () {
                                                AjaxSend({
                                                    url: '/Konstruksyon/CheckIfReqExist?QID=' + data.ID + '&OrgID=' + sessionStorage.OrgID + '&RespondentID=' + data.RespondentID,
                                                    success: function (e) {
                                                        if (e.Results == 1) {
                                                            NewMessageBox('No requirements available', 'Message');
                                                        } else {
                                                            window.location = '/Survey/ZipFiles?QID=' + data.ID + '&OrgID=' + sessionStorage.OrgID + '&RespondentID=' + data.RespondentID;
                                                            NewMessageBoxClose();
                                                        }
                                                    }
                                                })                                                
                                            })
                                            $('#btnKonstruksyonEditForm').unbind('click').click(function () {
                                                window.open(location.origin + '/Survey/Questionaire?q=' + data.Link + '&ClientID=' + sessionStorage.PeopleID + '&edit=1&QID=' + data.ID + '&RespondentID=' + data.RespondentID + '&OrgID=' + sessionStorage.OrgID, 'open');
                                            })
                                            var arr = [];
                                            for (var i = 0; i < data.Pages.length; i++) {
                                                arr = $.merge(data.Pages[i].Questions, arr);
                                            }
                                            DisplayReadOnlyQuestion(arr);
                                            GetResponse(data.ID, data.RespondentID);
                                            GetFiles();
                                        })
                                    )
                            }
                        } else {
                            tbodyCustomerBuildingPermit.append($('<tr>')
                                    .append($('<td colspan="5" style="text-align:center;">').text('No result(s) found'))
                                )
                        }
                    }
                })
            }

            var IsUseCheckbox = false;
            function DisplayReadOnlyQuestion(arrQuestions) {
                var listKonstruksyonQuestionsReadOnly = $('#listKonstruksyonQuestionsReadOnly');
                listKonstruksyonQuestionsReadOnly.empty();

                for (var i = 0; i < arrQuestions.length; i++) {
                    if (arrQuestions[i].QuestionType == 'Page Text') {
                        continue;
                    }

                    if (arrQuestions[i].QuestionType != 'Matrix') {
                        listKonstruksyonQuestionsReadOnly.append($('<li>')
                            .append($('<div>')
                                .css('font-weight', 'bold')
                                .html(HtmlDecode(arrQuestions[i].Question))
                            )
                            .append($('<div qid="' + arrQuestions[i].ID + '" class="divResponse" style="margin-left:20px;">'))
                        )
                    } else {
                        IsUseCheckbox = arrQuestions[i].IsUseCheckbox;
                        listKonstruksyonQuestionsReadOnly.append($('<li>')
                            .append($('<div>')
                                .css('font-weight', 'bold')
                                .html(HtmlDecode(arrQuestions[i].Question))
                            )
                            .append('<br>')
                            .append($('<div style="overflow:auto;">')
                                .append(DisplayMatrix(arrQuestions[i], true))
                            )
                        )
                    }
                }
                $('#popKonstruksyonPermit').show();
            }

            function GetResponse(questionaireID, respondentID) {
                AjaxSend({
                    url: 'Konstruksyon/GetResponse',
                    data: { QID: questionaireID, RID: respondentID },
                    success: function (e) {
                        var vRows = e.data;
                        $('.tdresponse').empty();

                        for (var i = 0; i < vRows.length; i++) {
                            if (vRows[i].Type != 'Matrix') {
                                var divAns = $('div[qid="' + vRows[i].QuestionID + '"]');

                                divAns.append((divAns.text().length == 0 ? '<br>' : '<br><br>')
                                        + (vRows[i].Type == "Multiple Inputs" ? vRows[i].Label + ': ' : '- ') + vRows[i].Answer
                                    )
                            } else {
                                var td = $('tr[rowid="' + vRows[i].MatrixRowID + '"]').find('td[colid="' + vRows[i].FieldID + '"]')
                                    .empty()
                                if (vRows[i].Control == 'text') {
                                    td.text(vRows[i].Answer)
                                } else {
                                    td.append('<span class="glyphicon ' + (CBool(IsUseCheckbox) ? 'glyphicon-ok' : 'glyphicon-record') + '"></span>')
                                }   
                            }
                        }
                    }
                })
            }

            function DisplayMatrix(data, isReadOnly) {
                var table = $('<table style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe">'),
                    tHead = $('<thead>'), tHeadRow = $('<tr class="custom-theme-f">'), tBody = $('<tbody>'), matrixChoices = '';
                var choiceType = (CBool(data.IsUseCheckbox) ? 'checkbox' : 'radio');
                tHeadRow.append($('<th style="min-width:280px;">'))
                for (var i = 0; i < data.Fields.Columns.length; i++) {
                    tHeadRow.append($('<th class="txt-center" style="min-width:100px;">')
                            .data('data', data.Fields.Columns[i])
                            .text(data.Fields.Columns[i].Label)
                        )
                    matrixChoices += '<td class="tdresponse" colid="' + data.Fields.Columns[i].ID + '"></td>'
                }
                tHead.append(tHeadRow)

                for (var i = 0; i < data.Fields.Rows.length; i++) {
                    var row = $('<tr>')
                            .data('data', data.Fields.Rows[i])
                            .attr('rowid', data.Fields.Rows[i].ID)
                            .append($('<td>')
                                .text(data.Fields.Rows[i].Label)
                            )
                            .append(matrixChoices)
                    tBody.append(row)
                }

                table.append(tHead).append(tBody);
                return table;
            }

            function LoginDetails() {
                AjaxSend({
                    dontHandleSessionExpired: true,
                    url: 'Konstruksyon/IsNewEntry',
                    success: function (vData) {
                        if (vData.PeopleID == '' || vData.Error != undefined) { //if people ID is blank, then redirect to sign in                            
                            setTimeout(function () {
                                window.open(GetPathFormat('/Konstruksyon'), '_self');
                            }, 2000);
                            alert('Your session has expired. Redirecting to home page ...');
                        }
                        else {
                            var pData = {};
                            pData.ID = vData.PeopleID;

                            AjaxSend({
                                url: "SignIn/CheckPeopleIsFirstPartySignUp",
                                data: pData,
                                success: function (vDataFirstParty) {
                                    sessionStorage.PeopleID = vDataFirstParty.PeopleFirstPartySignUpData.Rows[0].ID;
                                    $('#lblUserName').text(vDataFirstParty.PeopleFirstPartySignUpData.Rows[0].FirstName);
                                    GetCustomerBuildingPermit();
                                }
                            });
                        }
                    }
                })
            }

            function SignOut() {
                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {
                        AjaxSend({
                            url: "Konstruksyon/setSignOut",
                            success: function (vData) {
                                window.open(GetPathFormat("/Konstruksyon?mode=logout"), "_self"); // window.open(GetPathFormat("/People"), "_self"); EDITED BY: JANINE , Aug 3 4pm
                            }
                        });
                    },
                    error: function (vError) {
                        window.open(GetPathFormat("/Konstruksyon?mode=logout"), "_self");
                    }
                })
            }
        }) // end document
    })    
})