require(['common', 'bootstrap-tooltip']
, function () { //'/Content/bootstrap/js/bootstrap.js'
    $(document).ready(function () {
        var selQuestionType = $('#selQuestionType');
        var listQuestions = $('#listQuestions');
        var qType = selQuestionType.val(), arrQuestions = [], orgID = '', qID = '', ctrQuestion = 1, clientID = '';
        var curPageArr, arrSurveyPage = [], ctrPage = 0, pageID = '', respondentID = '', isSaveFinish = false;
        var arrTunnelQuestions = [], arrFieldWithTunnel = [], arrResponse = [], arrListWithFiles = [], arrListOfRequirements = [];
        var isSaveAndExit = false, PageNo = 1;

        if(GetUrlParameters().mode == 'preview'){
            $('#divPreviewMessage').show();
        } else {
            $('#divPreviewMessage').remove();
        }

        GetLinkDetails();

        $('[data-toggle="tooltip"]').tooltip()

        $('body')
        .on('click', '.btnCloseAnswer', function () {
            parent.LeadGenSave.CloseSurvey();
        })
        .on('click', '#btnPayNow', function () {
            $('#pageDownloadFiles').pageshow();
        })
        .on('click', '#btnDownloadZipFiles', function () {
            if (GetUrlParameters().mode != 'preview') {
                window.location = '/Survey/ZipFiles?QID=' + qID + '&OrgID=' + orgID + '&RespondentID=' + respondentID;
            } else {
                NewMessageBox('You\'re not allowed to download requirements in preview mode', 'Message');
            }
        })

        function GetLinkDetails() {
            if (!CBool(sessionStorage.IsDone)) {
                AjaxSend({
                    url: 'Survey/GetLinkDetails',
                    data: { Link: GetUrlParameters().q },
                    success: function (e) {
                        if (e.Results.Rows.length > 0) {
                            orgID = e.Results.Rows[0].OrgID;
                            qID = e.Results.Rows[0].QuestionaireID;
                            AjaxSend({
                                url: 'Survey/GetQuestionaire',
                                data: {
                                    OrgID: orgID,
                                    QID: qID
                                },
                                success: function (q) {
                                    if (CBool(q.data.IsActive)) {
                                        $('#btnSubmitAnswer').show();
                                        $('#lblSurveyTitle').text(q.data.Title);
                                        //arrQuestions = q.data.Questions;
                                        //DisplayQuestions();
                                        PageNo = 1;
                                        arrSurveyPage = q.data.Pages;
                                        arrListOfRequirements = q.data.Requirements;
                                        for (var p = 0; p < q.data.Pages.length; p++) {
                                            curPageArr = q.data.Pages[p];
                                            arrQuestions = curPageArr.Questions;
                                            DisplayPage([curPageArr]);
                                            DisplayQuestions();                                            
                                        }
                                        $('.noofpages').text(ctrPage);
                                        GetResponseOfCustomer();
                                        $('#' + arrSurveyPage[0].ID).pageshow();
                                        PopulateTunneledQuestions();
                                        Paypal();
                                    } else {
                                        $('#lblErrorMsg').text('This survey was deactivated.');
                                        $('#pageError').pageshow();
                                    }
                                }
                            })
                        } else {
                            $('#lblErrorMsg').text('Survey does not exist');
                            $('#pageError').pageshow();
                        }
                    }
                })
            } else {
                $('#pageBan').pageshow()
            }

            if (GetUrlParameters().w != undefined) {
                $('.btnCloseAnswer').show();                
            }
        }

        function GetResponseOfCustomer() {
            var data = GetUrlParameters();

            if (CStr2(data.RespondentID) != '') {
                GetResponse();
            } else {
                AjaxSend({
                    url: '/CRMDashboard/CheckIfCustomerSurveyExist',
                    data: {
                        ClientID: data.ClientID,
                        QID: data.QID
                    },
                    success: function (res) {
                        if (res.Results.Rows.length > 0) {
                            data.RespondentID = res.Results.Rows[0].ID;
                        }

                        GetResponse(data);
                    }
                })
            }
        }

        function GetResponse(data) {
            if (CBool(data.edit)) {
                respondentID = data.RespondentID;
                AjaxSend({
                    url: 'Survey/GetResponse',
                    data: { QID: data.QID, RID: data.RespondentID },
                    success: function (e) {
                        var vRows = e.data;

                        for (var i = 0; i < vRows.length; i++){
                            if (vRows[i].Type == 'Single Input') {
                                $('[fieldid="' + vRows[i].QuestionID + '"]').val(vRows[i].Answer);
                            } else if (vRows[i].Type == 'Multiple Inputs') {
                                if(CStr2(vRows[i].DataType) == 'Date'){
                                    $('[fieldid="' + vRows[i].FieldID + '"]').val($.format.date(new Date(vRows[i].Answer), 'yyyy-MM-dd'));
                                } else {
                                    $('[fieldid="' + vRows[i].FieldID + '"]').val(vRows[i].Answer);
                                }
                                
                            } else if (vRows[i].Type == 'radio' || vRows[i].Type == 'checkbox') {
                                $('[fieldid="' + vRows[i].FieldID + '"]').attr('checked', true).checkboxradio('refresh');

                                if(CStr2(vRows[i].Tunnel) != ''){
                                    $('[divfieldidtunnel="' + vRows[i].FieldID + '"]').pageshow();
                                }

                                if (CBool(vRows[i].IsOther)) {
                                    $('[fieldid="' + vRows[i].QuestionID + '"]').attr('checked', true).checkboxradio('refresh');
                                    $('[name="txtChoiceOther-' + vRows[i].QuestionID + '"]').val(vRows[i].Answer);
                                }
                            } else if (vRows[i].Type == 'Matrix') {
                                if(vRows[i].Control == 'text'){
                                    $('[rowid="' + vRows[i].MatrixRowID + '"]').find('[fieldid="' + vRows[i].FieldID + '"]').val(vRows[i].Answer);
                                } else {
                                    $('[rowid="' + vRows[i].MatrixRowID + '"]').find('[fieldid="' + vRows[i].FieldID + '"]').attr('checked', true).checkboxradio('refresh');
                                }
                            } else if (vRows[i].Type == 'radioinput' || vRows[i].Type == 'checkboxinput') {
                                $('[fieldid="' + vRows[i].FieldID + '"]').attr('checked', true).checkboxradio('refresh');
                                $('#txtfield-' + vRows[i].FieldID).val(vRows[i].Description);

                                if (CBool(vRows[i].IsOther)) {
                                    $('[fieldid="' + vRows[i].QuestionID + '"]').attr('checked', true).checkboxradio('refresh');
                                    $('[name="txtChoiceOther-' + vRows[i].QuestionID + '"]').val(vRows[i].Answer);
                                }
                            }
                        }
                    }
                })
            }            
        }

        function PopulateTunneledQuestions() {
            for (var i = 0; i < arrFieldWithTunnel.length; i++){
                var tunneledQ;

                $.grep(arrTunnelQuestions, function (e) {
                    if (e.ID == arrFieldWithTunnel[i].TunneledTo) {
                        tunneledQ = e;
                        return false;
                    }
                })

                var tQ;

                if (tunneledQ.QuestionType == 'Single Input') {
                    tQ = DisplaySingleInput(tunneledQ, true);
                } else if (tunneledQ.QuestionType == 'Multiple Inputs') {
                    tQ = DisplayMultipleInputs(tunneledQ, true);
                } else if (tunneledQ.QuestionType == 'radio' || tunneledQ.QuestionType == 'checkbox'
                    || tunneledQ.QuestionType == 'radioinput' || tunneledQ.QuestionType == 'checkboxinput') {
                    tQ = DisplayMultipleChoice(tunneledQ, true);
                } else if (tunneledQ.QuestionType == 'Matrix') {
                    tQ = DisplayMatrix(tunneledQ, true);
                } else if (tunneledQ.QuestionType == 'Page Text') {
                    tQ = DisplayPageText(tunneledQ, true);
                } else if (tunneledQ.QuestionType == 'file') {
                    tQ = DisplayFile(tunneledQ, true);
                }

                $('[divFieldIDTunnel="' + arrFieldWithTunnel[i].ID + '"]')
                    .append(tQ)
                    .attr('tunneltype', tunneledQ.QuestionType);
            }

            $('input[type="checkbox"], input[type="radio"]').trigger('create').checkboxradio();
        }

        var price = 500;
        function DisplayQuestions() {
            listQuestions = $('#list-' + curPageArr.ID);
            listQuestions.empty();

            for (var i = 0; i < arrQuestions.length; i++) {
                arrQuestions[i].Index = i;
                arrQuestions[i].PageID = curPageArr.ID;
                if (arrQuestions[i].State != 'remove') {
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
                        arrQuestions[i].Files = [];
                        DisplayFile(arrQuestions[i]);
                    }

                    if (arrListOfRequirements.length > 0) {
                        if (!CBool(curPageArr.IsShowButton)) {
                            $('#list-' + curPageArr.ID).empty()
                            //.append($('#divPaymentOption').clone().html())
                            //$('#divPaymentOption').remove();
                            $('#divPaymentOption').appendTo($('#list-' + curPageArr.ID));
                            $('#divPaymentOption').show();

                            $('#btnOwnLegwork').unbind('click').click(function () {
                                //$('#pagePayment').pageshow();
                                price = 500;
                                $(this).addClass('selectedService');
                                $('#btnOthersLegwork').removeClass('selectedService');
                            })

                            $('#btnOthersLegwork').unbind('click').click(function () {
                                //$('#pagePayment').pageshow();
                                alert('This service is currently not yet available. \n\nWe will update this page as soon as we can offer this service to you. Thank you.');
                                return;
                                price = 4999;
                                $(this).addClass('selectedService');
                                $('#btnOwnLegwork').removeClass('selectedService');
                            })
                        }
                    }
                }
            }
        }

        function Paypal() {
            paypal.Buttons({
                //style: {
                //    color:  'blue',
                //    shape:  'pill',
                //    label:  'pay',
                //    height: 40
                //},
                createOrder: function (data, actions) {
                    // This function sets up the details of the transaction, including the amount and line item details.
                    return actions.order.create({
                        purchase_units: [{
                            amount: {
                                value: price
                            }
                        }]
                    });
                },
                onApprove: function (data, actions) {
                    // This function captures the funds from the transaction.
                    return actions.order.capture().then(function (details) {
                        // This function shows a transaction success message to your buyer.
                        $('#lblPaypalName').text(details.payer.name.given_name + ' ' + details.payer.name.surname);
                        $('#pageDownloadFiles').pageshow();
                    });
                }
            }).render('#paypal-button-container');
        }

        function DisplayFile(data, isTunnel) {
            if (CDec2(data.TunnelCount) > 0 && !CBool(isTunnel)) {
                arrTunnelQuestions.push(data); // do not display tunneled questions
                return;
            }

            var divQ = $('<li>')
                        .data('data', data)
                        .attr({
                            req: CBool(data.Required),
                            type: data.QuestionType
                        })
                        .append($('<div class="row">')
                            .append($('<div class="col-sm-12">')
                                .append($('<a data-role="none" class="custom-link">')
                                    .data('data', data)
                                    .append($('<span class="glyphicon glyphicon-upload">'))
                                    .append('&nbsp;')
                                    .append(data.Question)
                                    .click(function (e) {
                                        var data = $(this).data('data');
                                        $('[fileid="' + data.ID + '"]').click();
                                    })
                                )
                                .append($('<input data-role="none" multiple="multiple" type="file" fileid="' + data.ID + '" accept=".xlsx,.xls,image/*,.doc, .docx,.ppt, .pptx,.txt,.pdf" style="display:none;">')
                                    .data('data', data)
                                    .change(function (e) {
                                        var data = $(this).data('data');
                                        var selectedFiles = data.Files;
                                        var files = $(this).get(0).files;
                                        var ul = $('#file-' + data.ID);

                                        for (var i = 0; i < files.length; i++){
                                            var isExist = false, ind;

                                            $.grep(selectedFiles, function (e, inx) {
                                                if (e.name == files[i].name) {
                                                    ind = inx;
                                                    isExist = true;
                                                    return false; // stop loop
                                                }
                                            })

                                            if (!isExist) {
                                                ul.append($('<li>')
                                                        .attr('filename', files[i].name)
                                                        .append(files[i].name)
                                                        .append('&nbsp; &nbsp;')
                                                        .append($('<span class="glyphicon glyphicon-remove">')
                                                            .data('allfiles', selectedFiles)
                                                            .data('data', files[i])
                                                            .click(function (e) {
                                                                var allfiles = $(this).data('allfiles');
                                                                var data = $(this).data('data');
                                                                var ind;

                                                                $.grep(allfiles, function(e, i){
                                                                    if (e.Index == data.Index) {
                                                                        ind = i;
                                                                        return false;
                                                                    }
                                                                })
                                                                allfiles.splice(ind, 1);
                                                                $(this).parent().remove();
                                                            })
                                                        )
                                                    )
                                                selectedFiles.push(files[i]);
                                            } else {
                                                selectedFiles[ind] = files[i];
                                            }
                                        }
                                    })
                                )
                                .append('<br>')
                                .append($('<ul data-role="none" class="custom-list" style="padding:0px 50px; list-style-type:decimal;" id="file-'+ data.ID +'">'))
                            )                            
                        )

            if (CBool(isTunnel)) {
                return divQ;
            } else {
                listQuestions.append(divQ);
            }
        }

        function DisplayPageText(data, isTunnel) {
            var divQ = $('<li>')
                        .data('data', data)
                        .attr({
                            req: CBool(data.Required),
                            type: data.QuestionType
                        })
                        .append($('<div class="row">')
                            .append($('<div class="col-sm-12">')
                                .append(HTMLDecode(data.Question))
                            )
                        )

            if(CBool(isTunnel)){
                return divQ;
            } else {
                listQuestions.append(divQ);
            }            
        }

        function HTMLDecode(str) {
            var htmlstr = $('<textarea />').html(str).text();
            return htmlstr;
        }

        function DisplayMatrix(data, isTunnel) {
            var table = $('<table style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe">'),
                tHead = $('<thead>'), tHeadRow = $('<tr class="custom-theme-f">'), tBody = $('<tbody>'), matrixChoices = $('<tr>');
            var choiceType = (CBool(data.IsUseCheckbox) ? 'checkbox' : 'radio');
            tHeadRow.append($('<th style="min-width:280px;">'))
            for (var i = 0; i < data.Fields.Columns.length; i++) {
                tHeadRow.append($('<th class="txt-center" style="min-width:100px;">')
                        .data('data', data.Fields.Columns[i])
                        .text(data.Fields.Columns[i].Label)
                    )

                if (data.Fields.Columns[i].Control == 'radio') {
                    matrixChoices.append($('<td>')
                            .append($('<div class="custom-' + choiceType + '">')
                                .append($('<label class="' + choiceType + '-only">')
                                    .append($('<input type="' + choiceType + '" controltype="' + choiceType + '" data-role="' + choiceType + '" score="' + data.Fields.Columns[i].Score + '" fieldid="' + data.Fields.Columns[i].ID + '" colname="' + data.Fields.Columns[i].Label + '" class="matrixChoices control" data-iconpos="notext">'))
                                ).append($('<div>'))
                            )
                        )
                } else {
                    var attrMaxLength = '', attrMinLength = '', inputtype;

                    if (data.Fields[i] != undefined) {
                        if (CInt2(data.Fields[i].CharMax) != 0) {
                            attrMaxLength = ' maxlength=' + CInt2(data.Fields[i].CharMax);
                        }

                        if (CInt2(data.Fields[i].CharMin) != 0) {
                            attrMinLength = ' minlength=' + CInt2(data.Fields[i].CharMin);
                        }
                    }

                    if (data.Fields.Columns[i].DataType == 'Alphanumeric') {
                        inputtype = 'text';
                    } else if (data.Fields.Columns[i].DataType == 'Numeric') {
                        inputtype = 'number';
                    } else if (data.Fields.Columns[i].DataType == 'Date') {
                        inputtype = 'date';
                    } else if (data.Fields.Columns[i].DataType == 'Email') {
                        inputtype = 'email';
                    } else if (data.Fields.Columns[i].DataType == 'URL') {
                        inputtype = 'url';
                    }
                    matrixChoices.append($('<td>')
                            .append($('<input type="' + inputtype + '" ' + attrMaxLength + attrMinLength + ' data-role="none" class="form-control control" controltype="text" score="' + data.Fields.Columns[i].Score + '" fieldid="' + data.Fields.Columns[i].ID + '" colname="' + data.Fields.Columns[i].Label + '">')
                                .data('data', data.Fields.Columns[i])
                                .keydown(function (e) {
                                    var data = $(this).data('data');

                                    if (data.DataType == 'Numeric') {
                                        if (!NumberDotOnly($(this), e)) {
                                            return;
                                        }
                                    }

                                    $(this).removeClass('has-error');
                                })
                                .blur(function (e) {
                                    var data = $(this).data('data');
                                    ValidateCharLength($(this), data);
                                })
                            )
                        )
                }
            }
            tHead.append(tHeadRow)

            for (var i = 0; i < data.Fields.Rows.length; i++) {
                var row = $(matrixChoices.clone(true))
                            .data('data', data.Fields.Rows[i])
                            .attr('rowid', data.Fields.Rows[i].ID)
                            .attr('choicetype', choiceType)
                            .prepend($('<td>')
                                .text(data.Fields.Rows[i].Label)
                            )
                row.find('input[type="' + choiceType + '"]').each(function () {
                    $(this).attr('name', data.Fields.Rows[i].ID);
                    $(this).click(function (e) {
                        e.stopPropagation();
                        e.preventDefault();
                    })
                })
                tBody.append(row)
            }

            table.append(tHead).append(tBody);

            var divQ = $('<li>')
                    .data('data', data)
                    .attr({
                        req: CBool(data.Required),
                        type: data.QuestionType
                    })
                    .append($('<div>').append($('<div class="row">')
                            .append($('<div class="col-sm-12 divQuestion">')
                                .append($('<span style="font-weight:bold; font-size:20px;">').html((CBool(data.Required) ? '<span class="required">*</span> ' : '') + data.Question))
                            )
                        ).append('<br>')
                        .append($('<div style="width:100%; overflow-x: auto;">')
                            .append(table)
                        )
                    )

            if(CBool(isTunnel)){
                return divQ;
            } else {
                listQuestions.append(divQ);

                $('.matrixChoices').checkboxradio().checkboxradio('refresh')
            }            
        }

        function DisplayMultipleChoice(data, isTunnel) {
            var textInputs = $('<div>'), type, sel = $('<select fieldid="'+ data.ID +'" id="radSelect-' + data.ID + '" data-role="none" class="form-control">').data('data', data).append(new Option('Please select', ''));

            if (CDec2(data.TunnelCount) > 0 && !CBool(isTunnel)) {
                arrTunnelQuestions.push(data); // do not display tunneled questions
                return;
            }

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
                data.Fields[i].Index = i;
                data.Fields[i].Type = type;
                data.Fields[i].QID = data.ID;
                data.Fields[i].TunnelAnswer = [];

                if (CBool(data.IsRadioButtonPresentAsSelectMenu)) {
                    sel.append($('<option>')
                            .data('data', data.Fields[i])
                            .attr('value', data.Fields[i].Label)
                            .text(data.Fields[i].Label)
                        )
                } else {
                    var row = $('<div class="row">');
                    row.append($('<div class="col-sm-' + (CBool(data.IsWithField) ? 3 : 12) + '">')
                            .data('data', data.Fields[i])
                            .append($('<div class="custom-' + type + '">')
                                .append($('<label class="' + type + '-only">')
                                    .append($('<input type="' + type + '" data-role="checkbox" data-iconpos="notext"/>')
                                        .attr({
                                            'id': 'chbrad-' + data.Index + '-' + data.Fields[i].Index,
                                            'value': data.Fields[i].Value,
                                            'fieldid': data.Fields[i].ID,
                                            'name': 'chbradChoices-' + data.ID,
                                            'hasTunnel': (CStr2(data.Fields[i].TunneledTo) == '' ? 0 : 1)
                                        })
                                        .data('data', data.Fields[i])
                                        .change(function () {
                                            var data = $(this).data('data');

                                            if (data.Type == 'checkbox') {
                                                if ($(this).is(':checked')) {
                                                    $('[divFieldIDTunnel="' + data.ID + '"]').show();
                                                } else {
                                                    $('[divFieldIDTunnel="' + data.ID + '"]').hide();
                                                }
                                            } else {
                                                $('[divQIDTunnel="' + data.QID + '"]').hide();
                                                $('[divFieldIDTunnel="' + data.ID + '"]').show();
                                            }

                                            $(this).parents().closest('li').find('.chbradValidation').remove();
                                        })
                                    )
                                )
                                .append($('<div>').text(data.Fields[i].Label))
                            )
                        )

                    if (CBool(data.IsWithField)) {
                        var attrMaxLength = '', attrMinLength = '', inputtype;

                        if (data.Fields[i] != undefined) {
                            if (CInt2(data.Fields[i].CharMax) != 0) {
                                attrMaxLength = ' maxlength=' + CInt2(data.Fields[i].CharMax);
                            }

                            if (CInt2(data.Fields[i].CharMin) != 0) {
                                attrMinLength = ' minlength=' + CInt2(data.Fields[i].CharMin);
                            }
                        }

                        if(data.Fields[i].DataType == 'Alphanumeric'){
                            inputtype = 'text';
                        } else if (data.Fields[i].DataType == 'Numeric') {
                            inputtype = 'number';
                        } else if (data.Fields[i].DataType == 'Date') {
                            inputtype = 'date';
                        } else if (data.Fields[i].DataType == 'Email') {
                            inputtype = 'email';
                        } else if (data.Fields[i].DataType == 'URL') {
                            inputtype = 'url';
                        }

                        row.append($('<div class="col-sm-9">')
                                .append($('<input type="' + inputtype + '" '+ attrMaxLength + attrMinLength +' data-role="none" id="txtfield-' + data.Fields[i].ID + '" class="form-control">')
                                    .data('data', data.Fields[i])
                                    .attr({
                                        'pInd': data.Index,
                                        'qid' : data.ID
                                    })
                                    .keydown(function (e) {
                                        var data = $(this).data('data');

                                        if(data.DataType == 'Numeric'){
                                            if (!NumberDotOnly($(this), e)) {
                                                return;
                                            }
                                        }
                                    })
                                    .focus(function () {
                                        var radBtn = $('#chbrad-' + $(this).attr('pInd') + '-' + $(this).data('data').Index);
                                        if (radBtn.attr('type') == 'radio') {
                                            $('[name="' + radBtn.attr('name') + '"]').removeAttr('checked').checkboxradio('refresh');
                                            $('[qid="' + $(this).attr('qid') + '"]').val('');
                                        }
                                        radBtn.prop('checked', true).checkboxradio('refresh');
                                    })
                                    .blur(function () {
                                        var data = $(this).data('data');
                                        var val = $.trim($(this).val());

                                        if (val == '') {
                                            //$('[fieldid="' + data.ID + '"]').removeAttr('checked').checkboxradio('refresh');                                            
                                        } else {
                                            ValidateCharLength($(this), data);
                                        }
                                    })
                                )
                            )
                    }

                    CheckIfFieldHasTunnel(data.Fields[i], data.ID, row);

                    //row.append('<br>')
                    textInputs.append(row);
                }
            }

            if (data.Other != undefined) {
                if (data.Other.AsType == 'answer') {
                    textInputs.append($('<div class="row">')
                            .append($('<div class="col-sm-9">')
                                .data('data', data.Other)
                                .append($('<div class="custom-' + type + '">')
                                    .append($('<label class="' + type + '-only">')
                                        .append($('<input type="' + type + '" value="Other" fieldid="' + data.ID + '" name="chbradChoices-' + data.ID + '" data-role="checkbox" data-iconpos="notext"/>')
                                            .data('data', data)
                                        )
                                    )
                                    .append($('<div>')
                                        .text(data.Other.Label)
                                        .append('<br><br>')
                                        .append($('<textarea data-role="none" name="txtChoiceOther-' + data.ID + '" class="form-control" style="height:100px; resize:none;">')
                                            .data('dataOther', data.Other)
                                            .attr('type', type)
                                            .click(function (e) {
                                                e.stopPropagation()
                                                e.preventDefault();
                                                var type = $(this).attr('type');
                                                var rc = $(this).parent().prev().find('input[type="' + type + '"]');
                                                if (type == 'radio') {
                                                    $(this).parent().closest('li').find('input[type="' + type + '"]').removeAttr('checked').checkboxradio('refresh');
                                                }
                                                rc.prop('checked', true).checkboxradio('refresh');
                                            })
                                        )
                                    )
                                ).append('<br>')
                            )
                        )
                } else {
                    textInputs.append($('<div class="row">')
                        .append($('<div class="col-sm-9">')
                            .append($('<div>').text(data.Other.Label))
                            .append('<br>')
                            .append($('<textarea data-role="none" name="txtChoiceOther-' + data.ID + '" class="form-control" style="height:100px; resize:none;">')
                                .data('dataOther', data.Other)
                                .data('data', data)
                            )
                            .append('<br>')
                        )
                    )
                }
            }

            var btnClearSelection = $('<a data-role="none" class="custom-link" data-toggle="tooltip" data-placement="bottom" title="Clear Selection">')
                                    .append($('<span class="glyphicon glyphicon-erase">'))
                                    .tooltip()
                                    .data('data', data)
                                    .click(function (e) {
                                        $('[name="chbradChoices-' + $(this).data('data').ID + '"]').removeAttr('checked').checkboxradio('refresh');
                                        $('[qid="' + $(this).data('data').ID + '"]').val('').removeClass('has-error');
                                        $('[divQIDTunnel="' + $(this).data('data').ID + '"]').hide();
                                    })

            var divQ = $('<li>')
                    .data('data', data)
                    .attr({
                        req: CBool(data.Required),
                        type: data.QuestionType
                    })

            if (CBool(data.IsRadioButtonPresentAsSelectMenu)) {
                sel.change(function () {
                    var seldata = $(this).data('data');
                    var optdata = $(this).find('option:selected').data('data');
                    $('ul[divqidtunnel="' + seldata.ID + '"]').hide();
                    if($(this).val() != ''){
                        $('ul[divfieldidtunnel="' + optdata.ID + '"]').show();
                    }
                })
                var row = $('<div class="row">')
                        .append($('<div class="col-sm-3 divQuestion">')
                            .append($('<span style="font-weight:bold; font-size:16px;">').html((CBool(data.Required) ? '<span class="required">*</span> ' : '') + data.Question))
                        ).append($('<div class="col-sm-9">')
                            .append(sel)
                        )
                divQ.append($('<div>')
                        .append(row)
                    );

                for (var i = 0; i < data.Fields.length; i++){
                    CheckIfFieldHasTunnel(data.Fields[i], data.ID, row);
                }
            } else {
                divQ.append($('<div>')
                        .append($('<div class="row">')
                            .append($('<div class="col-sm-12 divQuestion">')
                                .append($('<span style="font-weight:bold; font-size:16px;">').html((CBool(data.Required) ? '<span class="required">*</span> ' : '') + data.Question))
                                .append('&nbsp; &nbsp;')
                                .append(btnClearSelection)
                            ).append('<br><br>')
                        ).append(textInputs)
                    );
            }

            if(CBool(isTunnel)){
                return divQ;
            } else {
                listQuestions.append(divQ);

                $('input[type="checkbox"], input[type="radio"]').trigger('create').checkboxradio();
            }
        }

        function DisplaySingleInput(data, isTunnel) {

            if (CDec2(data.TunnelCount) > 0 && !CBool(isTunnel)) {
                arrTunnelQuestions.push(data); // do not display tunneled questions
                return;
            }

            var divQ = $('<li>')
                    .data('data', data)
                    .attr({
                        req: CBool(data.Required),
                        type: data.QuestionType
                    })
                    .append($('<div>').append($('<div class="row">')
                            .append($('<div class="col-sm-12 divQuestion">')
                                .append($('<span style="font-weight:bold; font-size:16px;">').html((CBool(data.Required) ? '<span class="required">*</span> ' : '') + data.Question))
                            )
                            .append('<br><br>')
                        )
                        .append($('<textarea data-role="none" type="text" class="form-control txtInputs" style="max-height:300px;">')
                            .attr('fieldid', data.ID) //field id
                            .data('data', data)
                        )
                    )

            if(CBool(isTunnel)){
                return divQ;
            } else {
                listQuestions.append(divQ)
            }            
        }

        function DisplayMultipleInputs(data, isTunnel) {
            if (CDec2(data.TunnelCount) > 0 && !CBool(isTunnel)) {
                arrTunnelQuestions.push(data); // do not display tunneled questions
                return;
            }

            var textInputs = $('<div>');

            for (var i = 0; i < data.Fields.length; i++) {
                var attrMaxLength = '', attrMinLength = '', inputtype;

                if (data.Fields[i] != undefined) {
                    if (CInt2(data.Fields[i].CharMax) != 0) {
                        attrMaxLength = ' maxlength=' + CInt2(data.Fields[i].CharMax);
                    }

                    if (CInt2(data.Fields[i].CharMin) != 0) {
                        attrMinLength = ' minlength=' + CInt2(data.Fields[i].CharMin);
                    }
                }

                if (data.Fields[i].DataType == 'Alphanumeric') {
                    inputtype = 'text';
                } else if (data.Fields[i].DataType == 'Numeric') {
                    inputtype = 'number';
                } else if (data.Fields[i].DataType == 'Date') {
                    inputtype = 'date';
                } else if (data.Fields[i].DataType == 'Email') {
                    inputtype = 'email';
                } else if (data.Fields[i].DataType == 'URL') {
                    inputtype = 'url';
                }

                textInputs.append($('<div class="row">')
                    .append($('<div class="col-sm-3">')
                        .append($('<label for="txtLabel-' + data.Index + '-' + i + '">').html((CBool(data.Fields[i].IsRequired) ? '<span class="required">*</span> ' : '') + data.Fields[i].Label + ':'))
                    )
                    .append($('<div class="col-sm-9">')
                        .data('data', data.Fields[i])
                        .append($('<input data-role="none" ' + attrMaxLength + attrMinLength + ' type="' + inputtype + '" required class="form-control txtInputs" id="txtLabel-' + data.Index + '-' + i + '">')
                            .attr('fieldid', data.Fields[i].ID) //field id
                            .data('data', data.Fields[i])
                            .blur(function () {
                                var data = $(this).data('data');
                                ValidateCharLength($(this), data);
                            })
                        )
                    )
                ).append('<br>')
            }

            var divQ = $('<li>')
                    .data('data', data)
                    .attr({
                        req: CBool(data.Required),
                        type: data.QuestionType
                    })
                    .append($('<div>').append($('<div class="divQuestion">')
                        .append($('<span style="font-weight:bold; font-size:16px;">').html(data.Question))
                        )
                        .append('<br><br>')
                        .append(textInputs)
                    )

            if(CBool(isTunnel)){
                return divQ;
            } else {
                listQuestions.append(divQ);
            }            
        }

        function DisplayPage(pages) {
            var lisSurveyPage = $('#divPageList');
            var divSurveyPageContent = $('#divPageList');

            for (var i = 0; i < pages.length; i++) {
                var content = $('<div id="' + pages[i].ID + '" pno="'+ pages[i].PageNo +'" subpage="survey" style="display:none;">');
                content.append($('<ul id="list-' + pages[i].ID + '" class="custom-list list-unstyled surveylist">'))
                content.append(DisplayNavigationButton(pages[i]));
                divSurveyPageContent.append(content)
            }
        }

        function CheckIfFieldHasTunnel(fielddata, qID, row) {
            if (CStr2(fielddata.TunneledTo) != '') {
                var isExist;

                $.grep(arrFieldWithTunnel, function (e) {
                    if (e.ID == fielddata.ID) {
                        isExist = true;
                        return false; // stop
                    }
                })

                if (!CBool(isExist)) {
                    row.append($('<ul class="custom-list list-unstyled">')
                        .attr({
                            'divFieldIDTunnel': fielddata.ID,
                            'divQIDTunnel': qID
                        })
                        .css({ display: 'none', 'margin-left': '55px' })
                    )

                    arrFieldWithTunnel.push(fielddata); // store field with tunnel
                }
            }
        }

        function DisplayNavigationButton(data) {
            var div = $('<div id="divnav-'+ data.ID +'" style="text-align:center;">');
 
            if(CBool(data.IsShowButton)){
                if (CInt2(data.PageNo) == 1) { // show only NEXT button in page 1
                    div.append($('<button data-role="none" class="btn custom-btn-theme-b btn-less-width">')
                            .data('data', data)
                            .text(data.NextLabel)
                            .click(function () {
                                isSaveAndExit = false;
                                NextAction($(this).data('data'), $(this));
                            })
                        )
                        .append('<br><br>')
                        .append($('<a data-role="none" class="custom-link">')
                            .data('data', data)
                            .text('Save and Exit')
                            .click(function () {
                                SaveAndExit($(this));
                            })
                        )
                } else if (CInt2(data.PageNo) != 1) {
                    div.append($('<button data-role="none" class="btn custom-btn-theme-c btn-less-width">')
                            .data('data', data)
                            .text(data.PrevLabel)
                            .click(function () {
                                var data = $(this).data('data');

                                if (data.PrevAction == 'prev') {
                                    $('[pno="' + (CInt2(data.PageNo) - 1) + '"]').pageshow();
                                } else if (data.PrevAction == 'goto') {
                                    $('#' + data.PrevGoto).pageshow();
                                } else {
                                    NewMessageBox('Cancel');
                                }
                                isSaveAndExit = false;
                            })
                        )
                        .append('&nbsp;&nbsp;')
                        .append($('<button data-role="none" class="btn custom-btn-theme-b btn-less-width">')
                            .data('data', data)
                            .text(data.NextLabel)
                            .click(function () {
                                isSaveAndExit = false;
                                NextAction($(this).data('data'), $(this));
                            })
                        )
                        .append('<br><br>')
                        .append($('<a data-role="none" class="custom-link">')
                            .data('data', data)
                            .text('Save and Exit')
                            .click(function () {
                                SaveAndExit($(this));
                            })
                        )
                }

                div.append('<br><br>')
                    .append('Page ' + PageNo + ' of <span class="noofpages"></span>')

                PageNo += 1;
                ctrPage += 1;
            } else {
                //if (arrListOfRequirements.length > 0) {                    
                //    div.append($('<button data-role="none" class="btn custom-btn-theme-b btn-less-width">')
                //            .data('data', data)
                //            .text('Go to Payment\'s Page')
                //            .click(function () {
                //                $('#pagePayment').pageshow();
                //            })
                //        )
                //}                
            }

            if (GetUrlParameters().w == 'crm' && !CBool(GetUrlParameters().viabrowser)) {
                div.append('<br><br>')
                    .append($('<a data-role="none" class="custom-link">').text('Close')
                        .click(function () {
                            parent.LeadGenSave.CloseSurvey();
                        })
                    )
            }

            return div;
        }

        function SaveAndExit(dis) {
            NewMessageBox('You have left some required fields blank. Are you sure you want to save and exit this building permit application form? <br><br>' +
                'You can still edit this form at a later time even after saving and exiting. <br><br>' + 
                'Please note that you may not download the completed forms until you have fully filled in all the required fields.',
                'Save and Exit', [
                    {
                        Text: 'Save this form and exit. I will complete this at a later time.', Click: function () {
                            isSaveAndExit = true;
                            var tempdata = JSON.parse(JSON.stringify(dis.data('data')));
                            tempdata.NextAction = 'savefinish';
                            NextAction(tempdata, dis);
                        }
                    }, { Text: 'Oops, wrong press. I will continue filling in this form.' }
                ])
        }

        function NextAction(data, btn) {
            listQuestions = $('#list-' + data.ID).find('> li');
            pageID = btn.data('data').ID;
            isSaveFinish = false;
            
            if (data.NextAction == 'next' && ValidateSurvey(data.NextAction)) {
                $('[pno="' + (CInt2(data.PageNo) + 1) + '"]').pageshow();
                $(window).scrollTop(0);
                return true;
            } else if (data.NextAction == 'goto' && ValidateSurvey(data.NextAction)) {
                $('#' + data.NextGoto).pageshow();
                $(window).scrollTop(0);
                return true;
            } else

            if ((data.NextAction == 'next' || data.NextAction == 'savenext') && ValidateSurvey(data.NextAction)) {
                SaveSurvey(btn, function () {
                    $('[pno="' + (CInt2(data.PageNo) + 1) + '"]').pageshow();
                })
                return true;
            } else if ((data.NextAction == 'next' || data.NextAction == 'savegoto') && ValidateSurvey(data.NextAction)) {
                SaveSurvey(btn, function () {
                    $('#' + data.NextGoto).pageshow();
                })
                return true;
            } else if (data.NextAction == 'savefinishnext') {
                listQuestions = $('.surveylist > li');
                pageID = '';
                isSaveFinish = true;
                if (ValidateSurvey(data.NextAction)) {
                    SaveSurvey(btn, function () {
                        $('[pno="' + (CInt2(data.PageNo) + 1) + '"]').pageshow();
                    })
                    return true;
                }
            } else if (data.NextAction == 'savefinishgoto') {
                listQuestions = $('.surveylist > li');
                pageID = '';
                isSaveFinish = true;
                if (ValidateSurvey(data.NextAction)) {
                    SaveSurvey(btn, function () {
                        $('#' + data.NextGoto).pageshow();
                    })
                    return true;
                }
            } else if (data.NextAction == 'savefinishgotolink') {
                listQuestions = $('.surveylist > li');
                pageID = '';
                isSaveFinish = true;
                if (ValidateSurvey(data.NextAction)) {
                    SaveSurvey(btn, function () {
                        window.location = 'http://' + data.NextGotoLink;
                    })
                    return true;
                }
            } else if (data.NextAction == 'savefinish') {
                listQuestions = $('.surveylist > li');
                pageID = '';
                isSaveFinish = true;
                if (ValidateSurvey(data.NextAction)) {
                    SaveSurvey(btn, function () {
                        $('#pageTY').pageshow();
                    })
                    return true;
                }                
            } else if (data.NextAction == 'cancel') {
                NewMessageBox('Cancel');
                return true;
            }

            WebToast('Please fill in the required field(s)', 'top');
            return false;
        }

        function ValidateSingleInput(dis, data, isRequired, arrExistingResponse) {
            var inp = dis.find('textarea');
            dis.find('.divQuestion').find('span.chbradValidation').remove();
            if (($.trim(inp.val()) == "" && CBool(isRequired) == true) && isSaveAndExit == false) {
                inp.parent().addClass('has-error');
                dis.find('.divQuestion').append($('<span class="required chbradValidation">').append('<br>Please fill the required field'))
                //isOkay = false;
                return false;
            } else {
                var isExist = false, ind;

                $.grep(arrExistingResponse, function (e, i) {
                    if (e.ID == data.ID) {
                        isExist = true;
                        ind = i;
                        return false; //stop
                    }
                })

                if (!isExist) {
                    data.Answer = $.trim(inp.val());
                    arrExistingResponse.push(data)
                } else {
                    if ($.trim(inp.val()) != "" || isSaveAndExit) { //  // allow required fields with empty value
                        arrExistingResponse[ind].Answer = $.trim(inp.val());
                    } else {
                        arrExistingResponse[ind].State = 'delete';
                    }
                }
                inp.parent().removeClass('has-error');
                return true;
            }
        }

        function ValidateMultipleInputs(dis, data, isRequired, arrExistingResponse) {
            var inps = dis.find('input');
            var isValid = true;

            if (data.Answer == undefined) {
                data.Answer = [];
            }

            var isExist = false, indR; // for response

            $.grep(arrExistingResponse, function (e, i) {
                if (e.ID == data.ID) {
                    isExist = true;
                    indR = i;
                    return false; //stop
                }
            })

            inps.each(function () {
                var inp = $(this);
                var inpdata = inp.data('data');
                dis.find('.divQuestion').find('span.chbradValidation').remove();
                inp.parent().find('span.chbradValidation').remove();

                if($.trim(inp.val()) != ''){
                    if (inpdata.DataType == 'Email') {
                        if (!IsEmail($.trim(inp.val()))) {
                            inp.parent().addClass('has-error');
                            isValid = false;
                            inp.parent().append($('<span class="required chbradValidation">').append('<br>Please enter valid email address.'))
                            return false;
                        }
                    } else if (inpdata.DataType == 'Date') {
                        if (!IsDate($.trim(inp.val()))) {
                            inp.parent().addClass('has-error');
                            isValid = false;
                            inp.parent().append($('<span class="required chbradValidation">').append('<br>Please enter valid date.'))
                            return false;
                        }
                    } if (inpdata.DataType == 'Numeric') {
                        if (!IsNumeric($.trim(inp.val()))) {
                            inp.parent().addClass('has-error');
                            isValid = false;
                            inp.parent().append($('<span class="required chbradValidation">').append('<br>Please enter valid number.'))
                            return false;
                        }
                    }
                }

                var isExistA = false, indA = 0; // for answer
                if (isExist) {
                    $.grep(arrExistingResponse[indR].Answer, function (e, i) {
                        if (e.ID == inpdata.ID) {
                            isExistA = true;
                            indA = i;
                            return false; //stop
                        }
                    })
                }

                if (($.trim(inp.val()) == "" && CBool(inpdata.IsRequired) == true) && isSaveAndExit == false) { // allow required fields with empty value
                    inp.parent().addClass('has-error');
                    dis.find('.divQuestion').append($('<span class="required chbradValidation">').append('<br>Please fill the required fields'))
                    isValid = false;
                    return false;
                } else {
                    if ($.trim(inp.val()) != '') {
                        if (ValidateCharLength(inp, inpdata)) {
                            var isExistA = false, indA;

                            if (isExist) {
                                $.grep(arrExistingResponse[indR].Answer, function (e, i) {
                                    if (e.ID == inpdata.ID) {
                                        isExistA = true;
                                        indA = i;
                                        return false; //stop
                                    }
                                })
                            }

                            if ($.trim(inp.val()) == "" && isSaveAndExit == false) { // allow required fields with empty value
                                if (isExistA) {
                                    arrExistingResponse[indR].Answer[indA].State = 'delete';
                                }
                            } else {
                                if (!isExistA) {
                                    $(this).data('data').Answer = $.trim($(this).val());
                                    data.Answer.push($(this).data('data'));
                                } else {
                                    if ($.trim(inp.val()) != "") { // allow required fields with empty value
                                        arrExistingResponse[indR].Answer[indA].Answer = $.trim(inp.val());
                                        arrExistingResponse[indR].Answer[indA].State = 'edit';
                                    } else {
                                        arrExistingResponse[indR].Answer[indA].State = 'delete';
                                    }
                                }
                            }

                            inp.parent().removeClass('has-error');
                        } else {
                            isValid = false;
                        }
                    } else {
                        $(this).data('data').State = 'delete';
                        data.Answer.push($(this).data('data'));
                    }
                }
            })

            if (!isExist) {
                arrExistingResponse.push(data);
            }

            return isValid;
        }

        function ValidateCheckboxRadio(dis, data, isRequired, arrExistingResponse) {
            var type, isValid = true;

            if (dis.attr('type') == 'radioinput') {
                type = 'radio';
            } else if (dis.attr('type') == 'checkboxinput') {
                type = 'checkbox';
            } else {
                type = dis.attr('type');
            }

            var inp = dis.find('[name="chbradChoices-'+ data.ID +'"]') //dis.find('input[type="' + type + '"]');
            var txtarea = dis.find('[name="txtChoiceOther-' + data.ID + '"]') //dis.find('textarea');
            var checked = dis.find('[name="chbradChoices-' + data.ID + '"]:checked') //dis.find('input[type="' + type + '"]:checked');
            var isExist = false, indR; // for response

            $.grep(arrExistingResponse, function (e, i) {
                if (e.ID == data.ID) {
                    isExist = true;
                    indR = i;
                    return false; //stop
                }
            })

            dis.find('.divQuestion:first').find('span.chbradValidation').remove();

            if (data.Answer == undefined) {
                data.Answer = [];
            }

            if (CBool(data.IsRadioButtonPresentAsSelectMenu)) {
                data.Answer = [];
                var sel = $('#radSelect-' + data.ID + ' option:selected');
                var val = sel.val();
                var optdata = sel.data('data');

                if (CBool(isRequired) && val == "" && isSaveAndExit == false) { // allow required fields with empty value
                    dis.find('.divQuestion:first').append($('<span class="required chbradValidation">').append('<br>Please select at least one choice'))
                    isValid = false;
                } else {
                    if (val != "" || val == "") { //allow required fields with empty value
                        optdata.IsOther = false;
                        optdata.Answer = val;
                        data.Answer.push(optdata);
                        if (isExist) {
                            arrExistingResponse[indR].Answer[0] = optdata;
                        } else {
                            arrExistingResponse.push(data);
                        }

                        isValid = true;

                        if (CStr2(optdata.TunneledTo) != '') {
                            var tunnel = $('[divFieldIDTunnel="' + optdata.ID + '"]')
                            var tunnelLI = tunnel.find('li:first');
                            var tunnelType = tunnel.attr('tunneltype');
                            var isTunnelRequired = tunnelLI.attr('req');
                            var tunnelData = tunnelLI.data('data');

                            isValid = TunnelingCheckboxRadio(tunnelType, tunnelLI, tunnelData, isTunnelRequired, optdata.TunnelAnswer)
                        }
                    }
                }
            } else {
                if (false) { //checked.length == 0 && CBool(isRequired) == true //allow required fields with empty value
                    //isOkay = false;
                    dis.find('.divQuestion:first').append($('<span class="required chbradValidation">').append('<br>Please select at least one choice'))
                    isValid = false;
                } else {
                    inp.each(function () {
                        var chbrad = $(this);
                        var chbraddata = chbrad.data('data');
                        var tunnelType = $('[divFieldIDTunnel="' + chbraddata.ID + '"]').attr('tunneltype');
                        var isExistA = false, indA = 0; // for answer

                        if (CBool($(this).attr('hasTunnel'))) {
                            var tunnelLI = $('[divFieldIDTunnel="' + chbraddata.ID + '"]').find('li:first');
                            var isTunnelRequired = tunnelLI.attr('req');
                            var tunnelData = tunnelLI.data('data');

                            isValid = TunnelingCheckboxRadio(tunnelType, tunnelLI, tunnelData, isTunnelRequired, chbraddata.TunnelAnswer)
                        }

                        if (isExist) {
                            $.grep(arrExistingResponse[indR].Answer, function (e, i) {
                                if (chbrad.val() == "Other") {
                                    if (e.ID == chbraddata.Other.ID) {
                                        isExistA = true;
                                        indA = i;
                                        return false; //stop
                                    }
                                } else {
                                    if (e.ID == chbraddata.ID) {
                                        isExistA = true;
                                        indA = i;
                                        return false; //stop
                                    }
                                }
                            })
                        }

                        if (chbrad.is(':checked')) {
                            if (chbrad.val() == "Other") {
                                var other = chbrad.parent().next().find('textarea');
                                if ($.trim(other.val()) == "") {
                                    isValid = false;
                                    chbrad.parent().next().addClass('has-error');
                                    return false;
                                } else {
                                    chbrad.parent().next().removeClass('has-error');
                                    chbraddata.Other.IsOther = true;
                                    chbraddata.Other.Answer = $.trim(other.val());
                                    chbraddata.Other.State = 'edit';
                                    if (isExist && isExistA) {
                                        arrExistingResponse[indR].Answer[indA].State = "edit";
                                    } else {
                                        data.Answer.push(chbraddata.Other)
                                    }
                                }
                            } else {
                                if (dis.attr('type') == 'radioinput' || dis.attr('type') == 'checkboxinput') {
                                    var txtfield = $('#txtfield-' + chbraddata.ID);

                                    if ($.trim(txtfield.val()) == '' && isSaveAndExit == false) { //allow required fields with empty value
                                        isValid = false;
                                        txtfield.addClass('has-error');
                                        return false;
                                    } else {
                                        if ($.trim(txtfield.val()) == '') {
                                            chbraddata.Description = "";
                                        } else {
                                            if (chbraddata.DataType == 'Numeric') {
                                                if (!IsNumeric($.trim(txtfield.val()))) {
                                                    isValid = false;
                                                    txtfield.addClass('has-error');
                                                    return false;
                                                }
                                            } else if (chbraddata.DataType == 'Date') {
                                                if (!IsDate($.trim(txtfield.val()))) {
                                                    isValid = false;
                                                    txtfield.addClass('has-error');
                                                    return false;
                                                }
                                            } else if (chbraddata.DataType == 'Email') {
                                                if (!IsEmail($.trim(txtfield.val()))) {
                                                    isValid = false;
                                                    txtfield.addClass('has-error');
                                                    return false;
                                                }
                                            } else if (chbraddata.DataType == 'URL') {
                                                if (!IsUrl($.trim(txtfield.val()))) {
                                                    isValid = false;
                                                    txtfield.addClass('has-error');
                                                    return false;
                                                }
                                            }

                                            chbraddata.Description = $.trim(txtfield.val());
                                        }                                        

                                        txtfield.removeClass('has-error');
                                        chbraddata.IsOther = false;
                                        chbraddata.Answer = $.trim(chbrad.val());

                                        if (isExist && isExistA) {
                                            arrExistingResponse[indR].Answer[indA].State = "edit";
                                        } else if (isExist && !isExistA) {
                                            arrExistingResponse[indR].Answer.push(chbraddata)
                                        } else {
                                            data.Answer.push(chbraddata)
                                        }
                                    }
                                } else {
                                    chbraddata.IsOther = false;
                                    chbraddata.Answer = $.trim(chbrad.val());

                                    if (isExist && isExistA) {
                                        arrExistingResponse[indR].Answer[indA].State = "edit";
                                    } else if (isExist && !isExistA) {
                                        arrExistingResponse[indR].Answer.push(chbraddata)
                                    } else {
                                        data.Answer.push(chbraddata)
                                    }
                                }
                            }
                        } else {
                            if (chbrad.val() == "Other") {
                                chbraddata.Other.State = 'delete';
                                data.Answer.push(chbraddata.Other)
                            } else {
                                chbraddata.State = 'delete';
                                data.Answer.push(chbraddata)
                            }                            
                            //if (isExist) {
                            //    if (isExistA) {
                            //        arrExistingResponse[indR].Answer[indA].State = 'delete';
                            //    }
                            //}
                        }
                    })

                    var txtareaval = $.trim(txtarea.val());
                    var isExistO = false, indO = 0;
                    if (txtarea.length > 0) {
                        var otherData = txtarea.data('dataOther');

                        if (txtarea.data('dataOther').AsType == 'comment') {
                            if (isExist) {
                                $.grep(arrExistingResponse[indR].Answer, function (e, i) {
                                    if (e.ID == txtarea.data('dataOther').ID) {
                                        isExistO = true;
                                        indO = i;
                                        return false; //stop
                                    }
                                })
                            }

                            otherData.IsOther = true;
                            otherData.Answer = txtareaval;
                            otherData.State = (txtareaval == '' ? 'delete' : 'edit');

                            if (isExist && isExistO) {
                                arrExistingResponse[indR].Answer[indO] = otherData;
                            } else if (isExist && !isExistO) {
                                arrExistingResponse[indR].Answer.push(otherData);
                            } else {
                                data.Answer.push(otherData);
                            }
                        }
                    }

                    if (!isExist) {
                        arrExistingResponse.push(data);

                        if (isExistO && txtareaval != '') {

                        }
                    }
                }
            }            

            return isValid;
        }

        function TunnelingCheckboxRadio(tunnelType, tunnelLI, tunnelData, isTunnelRequired, arrTunnelAnswer) {
            var isValid = true;

            if (tunnelType == 'Single Input') {
                isValid = ValidateSingleInput(tunnelLI, tunnelData, isTunnelRequired, arrTunnelAnswer)
            } else if (tunnelType == 'Multiple Inputs') {
                isValid = ValidateMultipleInputs(tunnelLI, tunnelData, isTunnelRequired, arrTunnelAnswer)
            } else if (tunnelType == 'radio' || tunnelType == 'checkbox'
                || tunnelType == 'radioinput' || tunnelType == 'checkboxinput') {
                isValid = ValidateCheckboxRadio(tunnelLI, tunnelData, isTunnelRequired, arrTunnelAnswer)
            } else if (tunnelType == 'Matrix') {
                isValid = ValidateMatrix(tunnelLI, tunnelData, isTunnelRequired, arrTunnelAnswer)
            } else if (tunnelType == 'file') {
                if (tunnelData.Files.length > 0) {
                    arrListWithFiles.push(tunnelData);
                }
            }

            return isValid;
        }

        function ValidateMatrix(dis, data, isRequired, arrExistingResponse) {
            var tr = dis.find('table').find('tbody').find('tr');
            var isRowUnchecked = false, isOkay = true;
            dis.find('.divQuestion').find('span.chbradValidation').remove();

            if (data.Answer == undefined) {
                data.Answer = [];
            }

            var isExist = false, indR; // for response

            $.grep(arrExistingResponse, function (e, i) {
                if (e.ID == data.ID) {
                    isExist = true;
                    indR = i;
                    return false; //stop
                }
            })

            tr.each(function () {
                if ($(this).find('input:checked').length == 0) {
                    isRowUnchecked = true;
                    return false;
                }
            })

            if ((isRowUnchecked && CBool(isRequired)) && isSaveAndExit == false) {
                //isOkay = false;
                dis.find('.divQuestion').append($('<span class="required chbradValidation">').append('<br>This is required.'))
                return false;
            } else {
                tr.each(function () {
                    var rowid = $(this).attr('rowid');
                    var choicetype = $(this).attr('choicetype');

                    $(this).find('td').find('.control').each(function () {
                        var fieldData = $(this).data('data');
                        var controltype = $(this).attr('controltype');
                        var val = CStr2($.trim($(this).val()));
                        var isSelected;
                        var pData = {
                            RowID: rowid,
                            ID: $(this).attr('fieldid'), // COLUMN ID
                            Answer: (controltype == 'text' ? val : $(this).attr('colname')),
                            Score: $(this).attr('score'),
                            State: 'edit'
                        };

                        if (controltype == 'text'){
                            isSelected = (CStr2($(this).val()) == '' ? false : true);

                            if (val != '') {
                                if (fieldData.DataType == 'Numeric') {
                                    if (!IsNumeric($.trim(val))) {
                                        isOkay = false;
                                        $(this).addClass('has-error');
                                        return false;
                                    }
                                } else if (fieldData.DataType == 'Date') {
                                    if (!IsDate($.trim(val))) {
                                        isOkay = false;
                                        $(this).addClass('has-error');
                                        return false;
                                    }
                                } else if (fieldData.DataType == 'Email') {
                                    if (!IsEmail($.trim(val))) {
                                        isOkay = false;
                                        $(this).addClass('has-error');
                                        return false;
                                    }
                                } else if (fieldData.DataType == 'URL') {
                                    if (!IsUrl($.trim(val))) {
                                        isOkay = false;
                                        $(this).addClass('has-error');
                                        return false;
                                    }
                                }
                            }
                        } else {
                            isSelected = $(this).is(':checked');
                        }

                        var isExistRo = false, indRo = 0; // for answer
                        if (isExist) {
                            $.grep(arrExistingResponse[indR].Answer, function (e, i) {
                                if (e.RowID == pData.RowID && e.ID == pData.ID) {
                                    isExistRo = true;
                                    indRo = i;
                                    return false; //stop
                                }
                            })
                        }

                        if (isExistRo) {
                            arrExistingResponse[indR].Answer[indRo] = pData;
                            arrExistingResponse[indR].Answer[indRo].State = (isSelected ? 'edit' : 'delete')
                        }
                        else {
                            pData.State = (isSelected ? 'edit' : 'delete');
                            if (isExist) {
                                arrExistingResponse[indR].Answer.push(pData)
                            } else {
                                data.Answer.push(pData)
                            }
                        }
                    })
                    
                    return isOkay
                })

                if (!isExist) {
                    arrExistingResponse.push(data);
                }
                //else {
                //    arrExistingResponse[indR] = data;
                //}
            }

            return isOkay;
        }
        
        function ValidateSurvey(nextaction) {
            var isOkay = true;
            arrListWithFiles = [];

            listQuestions.each(function () {
                var dis = $(this);
                var data = JSON.parse(JSON.stringify(dis.data('data')));
                var isRequired = dis.attr('req');
                //console.log(data);
                if (dis.attr('type') == 'Single Input') {
                    isOkay = ValidateSingleInput(dis, data, isRequired, arrResponse);
                } else if (dis.attr('type') == 'Multiple Inputs') {
                    isOkay = ValidateMultipleInputs(dis, data, isRequired, arrResponse);
                } else if (dis.attr('type') == 'radio' || dis.attr('type') == 'checkbox'
                        || dis.attr('type') == 'radioinput' || dis.attr('type') == 'checkboxinput') {
                    isOkay = ValidateCheckboxRadio(dis, data, isRequired, arrResponse);
                } else if (dis.attr('type') == 'Matrix') {
                    isOkay = ValidateMatrix(dis, data, isRequired, arrResponse);
                } else if (dis.attr('type') == 'file') {
                    if(dis.data('data').Files.length > 0){
                        arrListWithFiles.push(dis.data('data'));
                    }
                }

                return isOkay;
            })

            return isOkay;
        }

        function ValidateCharLength(dis, data) {
            var max = CInt2(data.CharMax), min = CInt2(data.CharMin);
            var curCharLength = dis.val().length;

            if ($.trim(dis.val()) == '' || (CBool(data.IsRequired) && $.trim(dis.val()) == '')) {
                return false; // allow required fields with empty value
            }

            if (max != 0 && min != 0) {
                if (curCharLength > max || curCharLength < min) {
                    WebToast('Enter character length between ' + min + ' and ' + max, 'top');
                    dis.addClass('has-error');
                    return false;
                }
            } else if (max != 0 && min == 0) {
                if (curCharLength > max) {
                    WebToast('Maximum character length is ' + max + '', 'top');
                    dis.addClass('has-error');
                    return false;
                }
            } else if (max == 0 && min != 0) {
                if (curCharLength > max) {
                    WebToast('Please enter character at least ' + min + '', 'top');
                    dis.addClass('has-error');
                    return false;
                }
            }
            dis.removeClass('has-error');
            return true;
        }

        var ctrFiles = 1; ctrUpload = 0;
        function SaveSurvey(btn, callback) {            
            if (GetUrlParameters().mode == 'preview') {
                callback();
            } else {
                //btn.prop('disabled', true);
                NewMessageBox('Submitting your answer, please wait.', 'Message');
                //console.log(arrResponse);
                //return;
                AjaxSend({
                    url: 'Survey/SaveResponse',
                    data: {
                        OrgID: orgID,
                        QID: qID,
                        ClientID: CStr2(GetUrlParameters().ClientID),
                        Response: arrResponse,
                        Date: $.format.date(new Date(), 'MMM dd, yyyy hh:mm a'),
                        PageID: pageID,
                        RespondentID: respondentID,
                        IsSaveFinish: isSaveFinish
                    }, success: function (e) {
                        ctrFiles = 1;
                        ctrUpload = 0;
                        NewMessageBoxClose();
                        if (e.Results == "saved") {
                            respondentID = e.RespondentID;
                            if (clientID == '') {
                                clientID = e.ClientID;
                            }
                            if (arrListWithFiles.length > 0) {
                                SaveFiles(clientID, function () {
                                    btn.removeAttr('disabled');
                                    callback();
                                })
                            } else {
                                btn.removeAttr('disabled');
                                callback();
                            }
                            $(window).scrollTop(0);
                            if (GetUrlParameters().w != undefined) {
                                parent.GetClientSurvey(); //CRMDashboard.js
                            }
                        } else {
                            NewMessageBox('Error: ' + e.Results);
                        }
                    }
                })
            }
        }
        
        function SaveFiles(clientID, callback) {
            NewMessageBox('Uploading file(s). Please wait ...', 'Message');
            AjaxSend({
                url: '/Survey/Upload?OrgID=' + orgID + '&clientID=' + clientID + '&respondentID=' + respondentID + '&questionID=' + arrListWithFiles[ctrUpload].ID + '&pageID=' + arrListWithFiles[ctrUpload].PageID,
                type: 'POST',
                data: FileFormData(ctrUpload),
                async: true,
                cache: false,
                contentType: false,
                processData: false,
                success: function (vData) {
                    if (ctrFiles == arrListWithFiles.length) {
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
                        SaveFiles(clientID, callback);
                    }
                }
            });
        }

        function FileFormData(index) {
            var data = new FormData();
            for (var m = 0; m < arrListWithFiles[index].Files.length; m++) {
                data.append(arrListWithFiles[index].Files[m].name, arrListWithFiles[index].Files[m]);
            }

            return data;
        }
    })
})