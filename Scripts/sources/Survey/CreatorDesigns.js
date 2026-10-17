window.Creator = {
    ctrQuestion: 1,
    QuestionaireReq: [],
    SurveyAllQuestions: null,
    SingleInput: {
        isEditQuestion: false,
        pData: {},
        state: 'new',
        tempID: NewID(),
        init: function () {
            this.pData = {};
            this.state = 'new';
            this.isEditQuestion = false;
            this.tempID = NewID();
            $('#txtQuestion').val('');
            $('#selQuestionType').val('Single Input');
            $('#divForQuestionTypeField').empty()
                .append('<hr>')
        },
        Edit: function(data){
            this.init();
            this.state = data.State;
            this.pData = data;
            this.isEditQuestion = true;

            if (this.state == "new") {
                this.tempID = data.TempID;
            }

            $('#txtQuestion').val(data.Question);
            $('#chbQuestionRequired').prop('checked', data.Required);//.checkboxradio('refresh');
        },
        OnComplete: function () {
            var txtQuestion = $('[name="txtQuestion"]');

            if ($.trim(txtQuestion.val()) == "") {
                txtQuestion.parent().addClass('has-error').focus();
                return;
            } else {
                txtQuestion.parent().removeClass('has-error');
            }

            if (this.state == 'new') {
                this.pData.TempID = this.tempID;
            }

            this.pData.QuestionType = $('#selQuestionType').val();
            this.pData.Question = $.trim(txtQuestion.val());
            this.pData.Required = $('#chbQuestionRequired').is(':checked');
            this.pData.State = this.state;
            this.pData.EditQuestion = this.isEditQuestion;

            return this.pData;
        }
    },
    MultipleInputs: {
        isEditQuestion: false,
        pData: {},
        arrQuestionFields: [],
        ctrTextInput: 1,
        state: 'new',
        init: function () {
            this.state = 'new';
            this.AddNew();
            this.AddTextInput();
            this.Sortable();
            $('#divRequired').hide();
        },
        AddNew: function NewMultipleText() {
            var ctrQuestion = Creator.ctrQuestion;
            this.ctrTextInput = 1;
            this.pData = {},
            this.arrQuestionFields = [];
            this.isEditQuestion = false;
            $('#txtQuestion').val('');
            $('#selQuestionType').val('Multiple Inputs');
            $('#divForQuestionTypeField').empty()
                .append('<hr>')
                .append($('<div class="row align-items-center">')
                    .append($('<div class="col-md-11">')
                        .append('<b data-role="none">Text Input</b>')
                    )
                    .append($('<div class="col-md-1">')
                        .append($('<button data-role="none" class="btn btn-success">')
                            .append($('<span class="bi bi-plus-lg">'))
                            .click(function () {
                                var field = $('.txtMultipleInputs:last');

                                if (field.length == 0) {
                                    Creator.MultipleInputs.AddTextInput();
                                }

                                if ($.trim(field.val()) != "") {
                                    Creator.MultipleInputs.AddTextInput();
                                } setTimeout(function () { $('.txtMultipleInputs:last').focus() })
                            })
                        )
                    )
                )
                .append('<br>')
                .append($('<div id="divTextInputs">'))
                .append('<hr>')            
        },
        Edit: function(data){
            this.AddNew();
            $('#txtQuestion').val(data.Question);
            this.state = data.State;
            this.pData = data;
            this.isEditQuestion = true,
            this.arrQuestionFields = data.Fields;
            $('#chbQuestionRequired').prop('checked', data.Required);//.checkboxradio('refresh');
            for (var i = 0; i < data.Fields.length; i++) {
                data.Fields[i].Index = i;
                if (CStr2(data.Fields[i].State) != 'remove') {
                    Creator.MultipleInputs.AddTextInput(data.Fields[i]);
                }                
            }
            this.Sortable();
        },
        AddTextInput: function (data) {
            var stateAdd, stateDelete, lastTextInput = $('.txtMultipleInputs:last');
            if(this.ctrTextInput > 1){
                stateDelete = 'block';
                stateAdd = 'none';
                if (lastTextInput.length > 0) {
                    if ($.trim(lastTextInput.val()) == '') {
                        setTimeout(function () { lastTextInput.focus() })
                        return; // do not add new text input if the last one is empty
                    }
                }                
            } else {
                stateDelete = 'none';
                stateAdd = 'block';
            }
            $('#divTextInputs').append($('<div class="row mb-3 align-items-center">')
                        .append($('<div class="col-md-3">')
                            .append($('<span class="bi bi-grip-vertical sortInputs" style="cursor:move;"></span>'))
                            .append('&nbsp; &nbsp;')
                            .append($('<label for="txtLabel-' + this.ctrTextInput + '">').text('Label ' + this.ctrTextInput + ':'))
                        )
                        .append($('<div class="col-md-3">')
                            .append($('<input data-role="none" ctr="' + this.ctrTextInput + '" type="text" class="form-control txtMultipleInputs" id="txtLabel-' + this.ctrTextInput + '" placeholder="Enter label name">')
                                .attr({
                                    charmin: (data == undefined ? 0 : data.CharMin),
                                    charmax: (data == undefined ? 0 : data.CharMax)
                                })
                                .data('data', (data == undefined ? '' : data))
                                .val((data == undefined ? '' : data.Label))
                                .blur(function () {
                                    if ($.trim($(this).val()) != "") {
                                        Creator.MultipleInputs.AddTextInput();
                                        setTimeout(function () { $('.txtMultipleInputs:last').focus() })
                                    }
                                })
                                .keypress(function (e) {
                                    if(e.which == 13 && $.trim($(this).val()) != ""){
                                        Creator.MultipleInputs.AddTextInput();
                                        setTimeout(function () { $('.txtMultipleInputs:last').focus() })
                                    }
                                })
                            )
                        )
                        .append($('<div class="col-md-2">')
                            .append($('<div class="form-check" title="Is this required?">')
                                .append($('<label class="form-check-label">')
                                    .text('Required')
                                    .append($('<input data-role="none" type="checkbox" id="txtLabelRequired-' + this.ctrTextInput + '" class="form-check-input" ' + (data == undefined ? '' : (CBool(data.IsRequired) ? 'checked' : '')) + '>'))
                                )
                            )
                        )
                        .append($('<div class="col-md-2">')
                            .append(Creator.PopulateDataType(this.ctrTextInput))                            
                        )
                        .append($('<div class="col-md-2 text-center">')
                            .append($('<div class="btn-group">')
                                .append($('<button data-role="none" margin-left:-10px;" ctr="' + this.ctrTextInput + '" title="More Options" class="btn btn-secondary">')
                                    .data('data', (data == undefined ? '' : data))
                                    .append($('<span class="bi bi-gear">'))
                                    .click(function () {
                                        var txtfield = $('#txtLabel-' + $(this).attr('ctr'));

                                        if ($.trim(txtfield.val()) == '') {
                                            WebToast('Please enter label name first', 'top');
                                            return
                                        }

                                        Creator.MoreOptions({
                                            Min: CInt2(txtfield.attr('charmin')),
                                            Max: CInt2(txtfield.attr('charmax')),
                                            Label: $.trim(txtfield.val())
                                        }, function (e) {
                                            txtfield.attr({
                                                'charmin': e.Min,
                                                'charmax': e.Max
                                            })
                                        })
                                    })
                                )
                                .append($('<button data-role="none" margin-left:-10px;" class="btn btn-danger">')
                                    .data('data', (data == undefined ? '' : data))
                                    .append($('<span class="bi bi-trash">'))
                                    .click(function () {
                                        var data = $(this).data('data');
                                        if (CStr2($(this).data('data')) != '') {
                                            $(this).data('data').State = 'remove';
                                            $(this).parent().parent().hide();
                                        } else {
                                            $(this).parent().parent().remove();
                                        }
                                    })
                                )
                            )
                        )
                    )
            $('#txtLabelRequired-' + this.ctrTextInput);//.checkboxradio();//.checkboxradio('refresh');
            $('#datatype-' + this.ctrTextInput).val((data == undefined ? 'Alphanumeric' : data.DataType));
            this.ctrTextInput += 1;            
        },
        OnComplete: function () {
            var txtQuestion = $('[name="txtQuestion"]');
            var txtMultipleInputs = $('.txtMultipleInputs');
            var isTextInputEmpty = true, orderno = 1;

            txtMultipleInputs.each(function () {
                var val = $.trim($(this).val());
                var data = $(this).data('data');
                var datatype = $('#datatype-' + $(this).attr('ctr')).val();
                var charmin = CInt2($(this).attr('charmin'));
                var charmax = CInt2($(this).attr('charmax'));

                if (val != "") {
                    isTextInputEmpty = false;
                    if (CStr2(data) != '') {
                        if (data.State == 'remove') {
                            //do nothing
                            //(Creator.MultipleInputs.arrQuestionFields).splice(CInt2(data.Index), 1);
                        } else {
                            Creator.MultipleInputs.arrQuestionFields[CInt2(data.Index)].DataType = datatype;
                            Creator.MultipleInputs.arrQuestionFields[CInt2(data.Index)].Label = val;
                            Creator.MultipleInputs.arrQuestionFields[CInt2(data.Index)].State = data.State;
                            Creator.MultipleInputs.arrQuestionFields[CInt2(data.Index)].IsRequired = $(this).parent().next().find('[type="checkbox"]').is(':checked');
                            Creator.MultipleInputs.arrQuestionFields[CInt2(data.Index)].CharMax = charmax;
                            Creator.MultipleInputs.arrQuestionFields[CInt2(data.Index)].CharMin = charmin;
                            Creator.MultipleInputs.arrQuestionFields[CInt2(data.Index)].OrderNo = orderno;
                        }                        
                    } else {
                        Creator.MultipleInputs.arrQuestionFields.push({
                            DataType: datatype,
                            Label: $.trim(val),
                            State: 'new',
                            IsRequired: $(this).parent().next().find('[type="checkbox"]').is(':checked'),
                            CharMin: charmin,
                            CharMax: charmax,
                            OrderNo: orderno
                        })
                    }

                    orderno += 1;
                }
            })

            if ($.trim(txtQuestion.val()) == "") {
                txtQuestion.parent().addClass('has-error').focus();
                return;
            } else {
                txtQuestion.parent().removeClass('has-error');
            }

            if(isTextInputEmpty){
                txtMultipleInputs.first().parent().addClass('has-error').focus();
                return;
            } else {
                txtMultipleInputs.first().parent().removeClass('has-error');
            }

            this.arrQuestionFields = this.arrQuestionFields.sort(function(a, b) {
                return CDec2(a.OrderNo) - CDec2(b.OrderNo);
            })

            this.pData.QuestionType = $('#selQuestionType').val();
            this.pData.Question = $.trim(txtQuestion.val());
            this.pData.Fields = this.arrQuestionFields;
            this.pData.Required = $('#chbQuestionRequired').is(':checked');
            this.pData.State = this.state;
            this.pData.EditQuestion = this.isEditQuestion;
            return this.pData;
        },
        Sortable: function () {
            $('#divTextInputs').sortable({
                delay:150,
                handle: '.sortInputs',
                update: function (e, ui) {
                }
            })
        }
    },
    CheckboxRadio: {
        isEditQuestion: false,
        pData: {},
        pOther: {},
        arrQuestionFields: [],
        ctrTextInput: 1,
        type: null,
        state: 'new',
        isWithField: '',
        tempID: NewID(),
        init: function (type) {
            this.type = type;
            this.state = 'new';
            this.isEditQuestion = false;
            this.isWithField = '';
            this.tempID = NewID();

            $('[name="choiceRadPresentation"]').removeAttr('checked');//.checkboxradio('refresh');
            $('[name="choiceRadPresentation"][value="0"]').prop('checked', true);//.checkboxradio('refresh');

            if (this.type == 'radio') {
                $('#divOptionForRad').show();
            } else {
                $('#divOptionForRad').hide();
            }

            if(type == 'radioinput'){
                this.type = 'radio';
                this.isWithField = type;
            } else if(type == 'checkboxinput'){
                this.type = 'checkbox';
                this.isWithField = type;
            }

            this.AddNew();
            this.AddChbRad();
            this.Sortable();
        },
        AddNew: function(){
            var ctrQuestion = Creator.ctrQuestion;
            this.pData = {};
            this.pOther = {};
            this.arrQuestionFields = [];
            this.ctrTextInput = 1;
            $('#txtQuestion').val('');
            $('#selQuestionType').val((CStr2(this.isWithField) != '' ? this.isWithField : this.type));
            $('#divForQuestionTypeField').empty()
                .append('<hr>')
                .append($('<div class="row align-items-center">')
                    .append($('<div class="col-md-11">')
                        .append('<b data-role="none">Choices</b>')
                    )
                    .append($('<div class="col-md-1">')
                        .append($('<button data-role="none" class="btn btn-success">')
                            .append($('<span class="bi bi-plus-lg">'))
                            .click(function () {
                                var field = $('.txtChbRadLabel:last');

                                if (field.length == 0) {
                                    Creator.CheckboxRadio.AddChbRad();
                                }

                                if ($.trim(field.val()) != "") {
                                    Creator.CheckboxRadio.AddChbRad();
                                } setTimeout(function () { $('.txtChbRadLabel:last').focus() })
                            })
                        )
                    )
                )
                .append('<br>')
                .append($('<div class="row">')
                    .append($('<div class="col-md-1">'))
                    .append($('<div class="col-md-' + (CStr2(this.isWithField) != '' ? 2 : 3) + ' text-center">').text('Choice'))
                    .append((this.isWithField == 'radioinput' || this.isWithField == 'checkboxinput' ? $('<div class="' + (CStr2(this.isWithField) != '' ? 'col-md-1' : 'col-md-2') + '" text-center">').text('') : $('<div class="' + (CStr2(this.isWithField) != '' ? 'col-md-1' : 'col-md-2') + ' text-center">').text('Score')))
                    .append($('<div class="col-md-2 text-center">').text('Requirements')
                        .append('&nbsp;')
                        //.append($('<div class="btn-group">')
                        //    .append($('<button data-role="none" type="button" class="btn btn-default btn-xs dropdown-toggle" data-toggle="dropdown">')
                        //        .append($('<span class="glyphicon glyphicon-cog">'))
                        //    )
                        //    .append($('<ul class="dropdown-menu">')
                        //        .append($('<li>')
                        //            .append($('<a href="#">').text('Add Requirement'))
                        //            .click(function () {
                        //                var txtRequirementName = $('#txtRequirementName');
                        //                $('#popRequirement').modal('show');

                        //                txtRequirementName.val('').focus();
                        //                $('#btnOkAddRequirement').show();
                        //                $('#btnOkCloseRequirement').unbind('click').click(function () {
                        //                    if ($.trim(txtRequirementName.val()) == '') {
                        //                        WebToast('Please enter description', 'top');
                        //                        return
                        //                    }

                        //                    var data = {
                        //                        ID: NewID(),
                        //                        Description: $.trim(txtRequirementName.val()),
                        //                        State: 'new'
                        //                    }
                        //                    Creator.QuestionaireReq.push(data);
                        //                    Creator.AddRequirements(data, true);
                        //                    $('#popRequirement').modal('hide');
                        //                })

                        //                $('#btnOkAddRequirement').unbind('click').click(function () {
                        //                    $('#btnOkCloseRequirement').click();
                        //                    $('#popRequirement').modal('show');
                        //                    txtRequirementName.val('').focus();
                        //                })
                        //            })
                        //        )
                        //        .append($('<li>')
                        //            .append($('<a href="#">').text('Edit Requirements')
                        //                .click(function () {
                        //                    Creator.EditReq();
                        //                })
                        //            )
                        //        )
                        //    )
                        //)                        
                    )
                    .append($('<div class="' + (CStr2(this.isWithField) != '' ? 'col-md-2' : 'col-md-3') + ' text-center">').text('Tunnel To Question'))
                    .append((CStr2(this.isWithField) != '' ? $('<div class="col-md-2 text-center">').text('Data type') : ''))
                )
                .append($('<div id="divChoices">'))
                .append('<hr>')            
            
            $('#divOptionForChbRad').show();
            $('#divOtherField').hide();
            $('#chbChbRadOtherField').removeAttr('checked');//.checkboxradio('refresh');
            $('[name="radOther"]').removeAttr('checked');//.checkboxradio('refresh');
            $('[name="radOther"][value="answer"]').prop('checked', true);//.checkboxradio('refresh');

            $('#chbChbRadOtherField').unbind('change').change(function () {
                if($(this).is(':checked')){
                    $('#divOtherField').show();
                    $('#txtOtherField').val('Others (please specify)');
                } else {
                    $('#divOtherField').hide();
                }
            })
        },
        Edit: function (data) {
            if(data.QuestionType == 'radioinput'){
                this.type = 'radio';
                this.isWithField = data.QuestionType;
            } else if(data.QuestionType == 'checkboxinput') {
                this.type = 'checkbox';
                this.isWithField = data.QuestionType;
            } else {
                this.type = data.QuestionType;
                this.isWithField = '';
            }
            
            this.AddNew();            
            this.pData = data;
            this.state = data.State;
            this.pOther = data.Other;
            this.arrQuestionFields = data.Fields;
            this.isEditQuestion = true;            

            if(this.state == "new"){
                this.tempID = data.TempID;
            }

            if (data.QuestionType == 'radio') {
                $('#divOptionForRad').show();
            } else {
                $('#divOptionForRad').hide();
            }

            $('[name="choiceRadPresentation"]').removeAttr('checked');//.checkboxradio('refresh');            
            $('#selQuestionType').val(data.QuestionType);
            $('#txtQuestion').val(data.Question);
            $('#chbQuestionRequired').prop('checked', data.Required);//.checkboxradio('refresh');
            $('#txtOtherField').val((typeof this.pOther != "undefined" ? this.pOther.Label : 'Others (plese specify)'));
            $('[name="choiceRadPresentation"][value="' + data.IsRadioButtonPresentAsSelectMenu + '"]').prop('checked', true);//.checkboxradio('refresh')

            for (var i = 0; i < data.Fields.length; i++) {
                data.Fields[i].Index = i;
                if (CStr2(data.Fields[i].State) != 'remove') {
                    Creator.CheckboxRadio.AddChbRad(data.Fields[i]);
                }
            }

            this.Sortable();

            if(data.Other != undefined){
                $('#chbChbRadOtherField').prop('checked', true);//.checkboxradio('refresh');
                $('#divOtherField').show();
                $('[name="radOther"]').removeAttr('checked');//.checkboxradio('refresh');
                $('[name="radOther"][value="' + data.Other.AsType + '"]').prop('checked', true);//.checkboxradio('refresh');
            }            
        },
        AddChbRad: function (data) {
            var colSize = 4, stateAdd, stateDelete, lastTextInput = $('.txtChbRadLabel:last');
            var divRow = $('<div class="row mb-3 align-items-center">').data('data', (data == undefined ? '' : data));

            if (this.ctrTextInput > 1) {
                stateDelete = 'block';
                stateAdd = 'none';
                if (lastTextInput.length > 0) {
                    if ($.trim(lastTextInput.val()) == '') {
                        setTimeout(function () { lastTextInput.focus() })
                        return; // do not add new text input if the last one is empty
                    }
                }
            } else {
                stateDelete = 'none';
                stateAdd = 'block';
            }

            divRow
                .append($('<div class="col-md-1">')
                    .append($('<span class="bi bi-grip-vertical sortCheckbox" style="cursor:move;"></span>'))
                    .append($('<input type="' + this.type + '" name="chbradChoices" data-role="none" class="form-check-input"/>'))
                )
                .append($('<div class="col-md-' + (CStr2(this.isWithField) != '' ? 2 : 3) + '">')
                    .append($('<input data-role="none" type="text" class="form-control txtChbRadLabel" id="txtChbRadLabel-' + this.ctrTextInput + '" placeholder="Enter label">')
                        .attr({
                            charmin: (data == undefined ? 0 : data.CharMin),
                            charmax: (data == undefined ? 0 : data.CharMax)
                        })
                        .data('data', (data == undefined ? '' : data))
                        .val((data == undefined ? '' : data.Label))
                        //.blur(function () {
                        //    if ($.trim($(this).val()) != "") {
                        //        Creator.CheckboxRadio.AddChbRad();
                        //        setTimeout(function () { $('.txtChbRadLabel:last').focus() })
                        //    }
                        //})
                        .keypress(function (e) {
                            if (e.which == 13 && $.trim($(this).val()) != "") {
                                Creator.CheckboxRadio.AddChbRad();
                                setTimeout(function () { $('.txtChbRadLabel:last').focus() })
                            }
                        })
                    )
                )

            if (CStr2(this.isWithField) != '') {
                divRow
                    .append($('<div class="col-md-1">')
                        .append($('<input data-role="none" type="text" class="form-control" disabled readonly>'))
                    )
            } else {
                divRow
                    .append($('<div class="col-md-2">')
                        .append($('<input data-role="none" type="text" class="form-control txtChbRadScore" placeholder="Score">')
                            .val((data == undefined ? '' : CDec2(data.Score)))
                            .keydown(function (e) {
                                if (!NumberDotOnly($(this), e)) {
                                    return;
                                }
                            })
                        )
                    )
            }

            var listOfReq = $('<ul class="dropdown-menu listReq">')
                .css({
                    'max-height': '300px',
                    'overflow': 'auto',
                    'font-size': '13px'
                })
                .data('data', (data == undefined ? [] : data.Requirements));

            Creator.PopulateReqPerNewChoice(listOfReq);

            divRow
                .append($('<div class="col-md-2 text-center">')
                    .append($('<div class="btn-group">')
                        .append($('<button data-role="none" type="button" class="btn btn-secondary dropdown-toggle" data-toggle="dropdown">')
                            .append($('<span class="bi bi-list-check">'))
                        )                        
                        .append(listOfReq)
                    )                    
                )
                .append($('<div class="' + (CStr2(this.isWithField) != '' ? 'col-md-2' : 'col-md-3') + '">')
                    .append((Creator.SurveyAllQuestions).clone()
                        .val((data == undefined ? '' : CStr2(data.TunneledTo)))                        
                    )
                )

            var butgroupcol = $('<div class="text-center ' + (CStr2(this.isWithField) != '' ? 'col-md-2' : 'col-md-1') + '">'); butgroup = $('<div class="btn-group">')

            if (CStr2(this.isWithField) != '') {
                butgroup.append($('<button data-role="none" margin-left:-10px;" ctr="' + this.ctrTextInput + '" title="More Options" class="btn btn-secondary">')
                    .data('data', (data == undefined ? '' : data))
                    .append($('<span class="bi bi-gear">'))
                    .click(function () {
                        var txtfield = $('#txtChbRadLabel-' + $(this).attr('ctr'));

                        if ($.trim(txtfield.val()) == '') {
                            WebToast('Please enter label name first', 'top');
                            return
                        }

                        Creator.MoreOptions({
                            Min: CInt2(txtfield.attr('charmin')),
                            Max: CInt2(txtfield.attr('charmax')),
                            Label: $.trim(txtfield.val())
                        }, function (e) {
                            txtfield.attr({
                                'charmin': e.Min,
                                'charmax': e.Max
                            })
                        })
                    })
                )
                
                divRow.append($('<div class="col-md-2">')
                        .append(Creator.PopulateDataType(this.ctrTextInput))
                    )
            }

            butgroup.append($('<button data-role="none" class="btn btn-danger">')
                .data('data', (data == undefined ? '' : data))
                .append($('<span class="bi bi-trash">'))
                .click(function () {
                    var parent = $(this).parent().parent();
                    var data = parent.data('data');
                    if (CStr2($(this).data('data')) != '') {
                        data.State = 'remove';
                        parent.hide();
                    } else {
                        parent.remove();
                    }
                })
            )

            butgroupcol.append(butgroup);
            divRow.append(butgroupcol);

            $('#divChoices').append(divRow
                        //.append('<br><br><br>')
                    )
            
            //$('[name="chbradChoices"]').trigger('create');//.checkboxradio();//.checkboxradio('refresh');
            $('#datatype-' + this.ctrTextInput).val((data == undefined ? 'Alphanumeric' : data.DataType));
            this.ctrTextInput += 1;

            Creator.CheckReq(listOfReq);

            $('.selTunnel > option[value="' + (this.pData.ID || this.pData.TempID) + '"]').remove();
            $('.checkbox-only').removeClass('ui-btn-corner-all')
        },
        OnComplete: function () {
            var txtQuestion = $('[name="txtQuestion"]');
            var txtChbRadValue = $('.txtChbRadValue');
            var txtChbRadLabel = $('.txtChbRadLabel');
            var orderno = 1;

            if ($.trim(txtQuestion.val()) == "") {
                txtQuestion.parent().addClass('has-error').focus();
                return;
            } else {
                txtQuestion.parent().removeClass('has-error');
            }

            txtChbRadLabel.each(function () {
                var val = $(this);
                var charmin = CInt2(val.attr('charmin'));
                var charmax = CInt2(val.attr('charmax'));
                var rowData = val.parent().parent().data('data');
                var score = CDec2(val.closest('div').next('div').find('.txtChbRadScore').val());
                var req = val.closest('div').next('div').next('div').find('ul').data('data');
                var tunnelto = val.closest('div').next('div').next('div').next('div').find('.selTunnel').val();
                var datatype = CStr2(val.closest('div').next('div').next('div').next('div').next('div').find('select').val());
                if ($.trim(val.val()) != '') {
                    if (CStr2(rowData) != '') {
                        if (rowData.State == 'remove') {
                            //do nothing
                            //(Creator.CheckboxRadio.arrQuestionFields).splice(CInt2(rowData.Index), 1);
                        } else {
                            Creator.CheckboxRadio.arrQuestionFields[CInt2(rowData.Index)].Value = $.trim(val.val());
                            Creator.CheckboxRadio.arrQuestionFields[CInt2(rowData.Index)].Label = $.trim(val.val());
                            Creator.CheckboxRadio.arrQuestionFields[CInt2(rowData.Index)].Score = score;
                            Creator.CheckboxRadio.arrQuestionFields[CInt2(rowData.Index)].Requirements = req;
                            Creator.CheckboxRadio.arrQuestionFields[CInt2(rowData.Index)].TunneledTo = tunnelto;
                            Creator.CheckboxRadio.arrQuestionFields[CInt2(rowData.Index)].State = rowData.State;
                            Creator.CheckboxRadio.arrQuestionFields[CInt2(rowData.Index)].DataType = datatype;
                            Creator.CheckboxRadio.arrQuestionFields[CInt2(rowData.Index)].CharMax = charmax;
                            Creator.CheckboxRadio.arrQuestionFields[CInt2(rowData.Index)].CharMin = charmin;
                            Creator.CheckboxRadio.arrQuestionFields[CInt2(rowData.Index)].OrderNo = orderno;
                        }
                    } else {
                        Creator.CheckboxRadio.arrQuestionFields.push({
                            Label: $.trim(val.val()),
                            Value: $.trim(val.val()),
                            Score: score,
                            Requirements: req,
                            TunneledTo: tunnelto,
                            State: 'new',
                            DataType: datatype,
                            CharMin: charmin,
                            CharMax: charmax,
                            OrderNo: orderno
                        })
                    }

                    orderno += 1;
                } else {
                    val.parent().addClass('has-error').focus();
                    return false;
                }
            })

            if ($('#chbChbRadOtherField').is(':checked')) {
                if (typeof this.pOther == 'undefined') {
                    this.pOther = {}
                }
                this.pOther.AsType = $('[name="radOther"]:checked').val();
                this.pOther.Label = $.trim($('#txtOtherField').val());
                this.pOther.Score = 0;
                this.pData.Other = this.pOther;
            } else {
                delete this.pData.Other;
            }

            if(this.state == 'new'){
                this.pData.TempID = this.tempID;
            }

            this.arrQuestionFields = this.arrQuestionFields.sort(function (a, b) {
                return CDec2(a.OrderNo) - CDec2(b.OrderNo);
            })

            this.pData.IsRadioButtonPresentAsSelectMenu = $('[name="choiceRadPresentation"]:checked').val();
            this.pData.QuestionType = $('#selQuestionType').val();
            this.pData.Question = $.trim(txtQuestion.val());
            this.pData.Fields = this.arrQuestionFields;
            this.pData.Required = $('#chbQuestionRequired').is(':checked');
            this.pData.State = this.state;
            this.pData.IsWithField = (this.isWithField != '' ? true : false);
            
            this.pData.EditQuestion = this.isEditQuestion;

            return this.pData;
        },
        Sortable: function () {
            $('#divChoices').sortable({
                delay: 150,
                handle: '.sortCheckbox',
                update: function (e, ui) {
                }
            })
        }
    },
    Matrix: {
        isEditQuestion: false,
        pData: {},
        ctrTextInputRow: 1,
        ctrTextInputColumn: 1,
        arrColumn: [],
        arrRow: [],
        state: 'new',
        init: function () {
            this.state = 'new';
            this.isEditQuestion = false;
            this.arrColumn = [];
            this.arrRow = [];
            this.pData = {};
            this.AddNew();
            this.ctrTextInputRow = 1;
            this.ctrTextInputColumn = 1;
            this.AddField('Row');
            this.AddField('Column');
        },
        AddNew: function () {
            $('#txtQuestion').val('').focus();
            $('#selQuestionType').val('Matrix');
            $('#divForQuestionTypeField').empty()
                .append('<hr>')
                .append($('<div class="row align-items-center">')
                    .append($('<div class="col-md-11">')
                        .append('<b>Rows</b>')
                    )
                    .append($('<div class="col-md-1">')
                        .append($('<button data-role="none" class="btn btn-success">')
                            .append($('<span class="bi bi-plus-lg">'))
                            .click(function () {
                                var field = $('.txtMatrixRow:last');

                                if (field.length == 0) {
                                    Creator.Matrix.AddField('Row');
                                }

                                if ($.trim(field.val()) != "") {
                                    Creator.Matrix.AddField('Row');
                                } setTimeout(function () { $('.txtMatrixRow:last').focus() })
                            })
                        )
                    )
                )
                .append('<br>')
                .append($('<div id="divMatrixRow">'))
                .append('<hr>')
                .append($('<div class="row">')
                    .append($('<div class="col-md-5">')
                        .append('<b>Column</b>')
                    )
                    .append($('<div class="col-md-1">')
                        .append('<b>Score</b>')
                    )
                    .append($('<div class="col-md-2">')
                        .append('<b>Control Type</b>')
                    )
                    .append($('<div class="col-md-3">')
                        .append('<b>Data Type</b>')
                    )
                    .append($('<div class="col-md-1">')
                        .append($('<button data-role="none" class="btn btn-success">')
                            .append($('<span class="bi bi-plus-lg">'))
                            .click(function () {
                                var field = $('.txtMatrixColumn:last');

                                if (field.length == 0) {
                                    Creator.Matrix.AddField('Column');
                                }

                                if ($.trim(field.val()) != "") {
                                    Creator.Matrix.AddField('Column');
                                } setTimeout(function () { $('.txtMatrixColumn:last').focus() })
                            })
                        )
                    )
                )
                .append('<br>')
                .append($('<div id="divMatrixColumns">'))
                .append('<hr>')
            $('#divOptionForMatrix').show();
            $('#chbChbRadMatrix').remove('checked');//.checkboxradio('refresh');
        },
        Edit: function(data){
            this.AddNew();
            $('#txtQuestion').val(data.Question);
            this.state = data.State;
            this.ctrTextInputRow = 1;
            this.ctrTextInputColumn = 1;
            this.isEditQuestion = true;
            this.pData = data;
            this.arrRow = data.Fields.Rows;
            this.arrColumn = data.Fields.Columns;
            $('#chbQuestionRequired').prop('checked', data.Required);//.checkboxradio('refresh');
            $('#chbChbRadMatrix').prop('checked', data.IsUseCheckbox);//.checkboxradio('refresh');

            for (var i = 0; i < this.arrRow.length; i++) {
                Creator.Matrix.arrRow[i].Index = i;
                if (Creator.Matrix.arrRow[i].State != 'remove') {
                    Creator.Matrix.AddField('Row', Creator.Matrix.arrRow[i]);
                }                
            }

            for (var i = 0; i < this.arrColumn.length; i++) {
                Creator.Matrix.arrColumn[i].Index = i;
                if (Creator.Matrix.arrColumn[i].State != 'remove') {
                    Creator.Matrix.AddField('Column', Creator.Matrix.arrColumn[i]);
                }                
            }
        },
        AddField: function (type, data) {
            var divContainer, stateAdd, stateDelete, txtField;
            if (type == 'Row') {
                divContainer = 'divMatrixRow';
                txtField = 'txtMatrixRow';
                if (this.ctrTextInputRow > 1) {
                    stateAdd = 'none';
                    stateDelete = 'block';
                } else {
                    stateAdd = 'block';
                    stateDelete = 'none';
                }
                this.ctrTextInputRow += 1
            } else {
                divContainer = 'divMatrixColumns';
                txtField = 'txtMatrixColumn';
                if (this.ctrTextInputColumn > 1) {
                    stateAdd = 'none';
                    stateDelete = 'block';
                } else {
                    stateAdd = 'block';
                    stateDelete = 'none';
                }
                this.ctrTextInputColumn += 1
            }

            var lastInputField = $('.' + txtField + ':last');
            if(lastInputField.length > 0){
                if($.trim(lastInputField.val()) == ""){
                    setTimeout(function () { lastInputField.focus() })
                    return;
                }
            }

            var field = $('<div class="row">')
                            .append($('<div class="col-md-2">')
                                .text(type + ' Label:')
                            )
                            .append($('<div class="col-md-' + (type == 'Row' ? 8 : 3) + '">')
                                .append($('<input data-role="none" type="text" matrix="' + type + '" class="form-control ' + txtField + '" id="'+ txtField +'-'+ this.ctrTextInputColumn +'">')
                                    .attr({
                                        charmin: (data == undefined ? 0 : data.CharMin),
                                        charmax: (data == undefined ? 0 : data.CharMax),
                                        fieldname: txtField
                                    })
                                    .data('data', (data == undefined ? '' : data))
                                    .val((data == undefined ? '' : data.Label))
                                    .blur(function () {
                                        if ($.trim($(this).val()) != "") {
                                            Creator.Matrix.AddField($(this).attr('matrix'));
                                        }
                                    })
                                    .keypress(function (e) {
                                        var fname = $(this).attr('fieldname');
                                        if (e.which == 13 && $.trim($(this).val()) != "") {
                                            Creator.Matrix.AddField($(this).attr('matrix'));
                                            setTimeout(function () { $('.' + fname + ':last').focus() })
                                        }
                                    })
                                )
                            )

            if(type == 'Column'){
                field.append($('<div class="col-md-1">')
                        .append($('<input type="text" data-role="none" class="form-control" placeholder="Score">')
                            .data('data', (data == undefined ? '' : data))
                            .val((data == undefined ? '' : CDec2(data.Score)))
                            .keydown(function (e) {
                                if (!NumberDotOnly($(this), e)) {
                                    return;
                                }
                            })
                        )
                    )
                    .append($('<div class="col-md-2">')
                        .append($('<select data-role="none" class="form-control">')
                            .attr('id', 'controltype-' + this.ctrTextInputColumn)
                            .attr('ctr', this.ctrTextInputColumn)
                            .data('data', (data == undefined ? '' : data))
                            .append($('<option value="radio">').text('Radio Button'))
                            .append($('<option value="text">').text('Text field'))
                            .val((data == undefined ? 'radio' : data.Control))
                            .change(function () {
                                var val = $(this).val();

                                if(val == 'text'){
                                    $('#datatype-' + $(this).attr('ctr')).removeAttr('disabled');
                                } else {
                                    $('#datatype-' + $(this).attr('ctr')).attr('disabled', true);
                                }
                            })
                        )
                    )
                    .append($('<div class="col-md-2">')
                        .append(Creator.PopulateDataType(this.ctrTextInputColumn).attr('disabled', (data == undefined ? true : (data.Control == 'text' ? false : true))))
                    )
                    .append($('<div class="col-md-1 text-center">')
                        .append($('<button data-role="none" margin-left:-10px;" ctr="' + this.ctrTextInputColumn + '" title="More Options" class="btn btn-secondary">')
                            .data('data', (data == undefined ? '' : data))
                            .append($('<span class="bi bi-gear">'))
                            .click(function () {
                                var txtfield = $('#txtMatrixColumn-' + $(this).attr('ctr'));

                                if ($('#controltype-' + $(this).attr('ctr')).val() != 'text') {
                                    WebToast('Applicable only for control type "Text Field"', 'top');
                                    return;
                                }

                                if ($.trim(txtfield.val()) == '') {
                                    WebToast('Please enter label name first', 'top');
                                    return
                                }

                                Creator.MoreOptions({
                                    Min: CInt2(txtfield.attr('charmin')),
                                    Max: CInt2(txtfield.attr('charmax')),
                                    Label: $.trim(txtfield.val())
                                }, function (e) {
                                    txtfield.attr({
                                        'charmin': e.Min,
                                        'charmax': e.Max
                                    })
                                })
                            })
                        )
                    )
            }

            var action = $('<div class="col-md-1">')
                                //.append($('<button data-role="none" matrix="' + type + '" style="display:' + stateAdd + '" class="btn btn-success">')
                                //    .append($('<span class="bi bi-plus-lg">'))
                                //    .click(function () {
                                //        var field = ($(this).attr('matrix') == 'Row' ? 'txtMatrixRow' : 'txtMatrixColumn');
                                //        if ($.trim($('.' + field + ':last').val()) != "") {
                                //            Creator.Matrix.AddField();
                                //        } setTimeout(function () { $('.' + field + ':last').focus() })
                                //    })
                                //)
                                .append($('<button matrix="' + type + '" data-role="none" class="btn btn-danger">')
                                    .data('data', (data == undefined ? '' : data))
                                    .append($('<span class="bi bi-trash">'))
                                    .click(function () {
                                        var parent = $(this).parent().parent();
                                        var data = $(this).data('data');
                                        if (CStr2(data) != '') {
                                            data.State = 'remove';
                                            parent.hide();
                                        } else {
                                            parent.remove();
                                        }
                                    })
                                )

            $('#' + divContainer).append(field.append(action).append('<br><br><br>'));
            $('.checkbox-only').removeClass('ui-btn-corner-all');
            $('#datatype-' + this.ctrTextInputColumn).val((data == undefined ? 'Alphanumeric' : data.DataType));
        },
        OnComplete: function () {
            var txtQuestion = $('[name="txtQuestion"]');
            var txtRow = $('.txtMatrixRow');
            var txtColumn = $('.txtMatrixColumn');
            
            if ($.trim(txtQuestion.val()) == "") {
                txtQuestion.parent().addClass('has-error').focus();
                return;
            } else {
                txtQuestion.parent().removeClass('has-error');
            }

            txtRow.each(function () {
                var dis = $(this);
                var data = dis.data('data')
                if($.trim(dis.val()) != ""){
                    if (CStr2(data) != '') {
                        if (data.State == 'remove') {
                            //do nothing
                            //(Creator.Matrix.arrRow).splice(CInt2(data.Index), 1)
                        } else {
                            Creator.Matrix.arrRow[CInt2(data.Index)].Label = $.trim(dis.val());
                            Creator.Matrix.arrRow[CInt2(data.Index)].State = data.State;
                            Creator.Matrix.arrRow[CInt2(data.Index)].Type = data.Type;
                        }
                    } else {
                        Creator.Matrix.arrRow.push({
                            Label: $.trim(dis.val()),
                            State: 'new',
                            Type: 'Row'
                        })
                    }
                }
            })

            txtColumn.each(function () {
                var dis = $(this);
                var charmin = CInt2(dis.attr('charmin'));
                var charmax = CInt2(dis.attr('charmax'));
                var data = dis.data('data');
                var score = CDec2(dis.closest('div').next('div').find('[type="text"]').val());
                var controltype = CStr2(dis.closest('div').next('div').next('div').find('select').val());
                var datatype = CStr2(dis.closest('div').next('div').next('div').next('div').find('select').val());
                
                if ($.trim(dis.val()) != "") {
                    if (CStr2(data) != '') {
                        if (data.State == 'remove') {
                            //do nothing
                            //(Creator.Matrix.arrColumn).splice(CInt2(data.Index), 1)
                        } else {
                            Creator.Matrix.arrColumn[CInt2(data.Index)].Label = $.trim(dis.val());
                            Creator.Matrix.arrColumn[CInt2(data.Index)].State = data.State;
                            Creator.Matrix.arrColumn[CInt2(data.Index)].Type = data.Type;
                            Creator.Matrix.arrColumn[CInt2(data.Index)].Score = score;
                            Creator.Matrix.arrColumn[CInt2(data.Index)].Control = controltype;
                            Creator.Matrix.arrColumn[CInt2(data.Index)].DataType = datatype;
                            Creator.Matrix.arrColumn[CInt2(data.Index)].CharMax = charmax;
                            Creator.Matrix.arrColumn[CInt2(data.Index)].CharMin = charmin;
                        }
                    } else {
                        Creator.Matrix.arrColumn.push({
                            Label: $.trim(dis.val()),
                            State: 'new',
                            Type: 'Column',
                            Score: score,
                            Control: controltype,
                            DataType: datatype,
                            CharMax: charmax,
                            CharMin: charmin
                        })
                    }
                }
            })

            this.pData.QuestionType = $('#selQuestionType').val();
            this.pData.Question = $.trim(txtQuestion.val());
            this.pData.Fields = {
                Rows: this.arrRow,
                Columns: this.arrColumn
            }
            this.pData.Required = $('#chbQuestionRequired').is(':checked');
            this.pData.IsUseCheckbox = $('#chbChbRadMatrix').is(':checked');
            this.pData.State = this.state;
            this.pData.EditQuestion = this.isEditQuestion;

            return this.pData;
        }
    },
    PageText: {
        isEdit: false,
        pData: {},
        state: 'new',
        init: function () {
            this.pData = {};
            this.state = 'new';
            this.isEdit = false;
            $('#divForDescription').pageshow();
            $('#divDescription').summernote('reset');
            $('#divDescription').summernote('focus');
        },
        Edit: function (data) {
            this.init();
            this.state = data.State;
            this.isEdit = true;
            this.pData = data;
            $('#divDescription').summernote('code', HTMLDecode(data.Question));
            $('#divDescription').summernote('focus');
            $('#selQuestionType').val('Page Text');

        },
        OnComplete: function () {
            this.pData.State = this.state;
            this.pData.Question = HTMLEncode($('#divDescription').summernote('code'));
            this.pData.QuestionType = 'Page Text';
            this.pData.EditQuestion = this.isEdit;

            return this.pData;
        }
    },
    File: {
        isEdit: false,
        pData: {},
        state: 'new',
        init: function () {
            this.isEdit = false;
            this.pData = {};
            this.state = 'new';
            $('#txtQuestion').val('').focus();
            $('#selQuestionType').val('file');
            $('#divForQuestionTypeField').empty()
                .append('<hr>')
        },
        Edit: function (data) {
            this.init();
            this.state = data.State;
            this.isEdit = true;
            this.pData = data;
            $('#txtQuestion').val(data.Question).focus();
        },
        OnComplete: function () {
            this.pData.State = this.state;
            this.pData.Question = $.trim($('#txtQuestion').val());
            this.pData.QuestionType = 'file';
            this.pData.EditQuestion = this.isEdit;

            return this.pData;
        }
    },
    EditReq: function () {
        var listofreq = $('#listofreq'), ctr = 1;

        listofreq.empty();

        for (var i = 0; i < this.QuestionaireReq.length; i++){
            this.QuestionaireReq[i].Index = i;

            if (this.QuestionaireReq[i].State != 'remove') {
                listofreq.append($('<li class="list-group-item">')
                        .append(ctr + '. <span reqid="txtReq-' + this.QuestionaireReq[i].ID + '" data-value="' + this.QuestionaireReq[i].ID + '">' + this.QuestionaireReq[i].Description + '</span> &nbsp; &nbsp;')
                        .append($('<span style="display:none">')
                            .append($('<button data-toggle="tooltip" data-placement="bottom" title="Edit" data-role="none" type="button" class="btn btn-warning">')
                                .append($('<span class="bi bi-pencil">'))
                                .data('data', this.QuestionaireReq[i])
                                .click(function () {
                                    var data = $(this).data('data');
                                    var txtRequirementName = $('#txtRequirementName');

                                    $('#popRequirement').modal('show');
                                    txtRequirementName.val(data.Description).focus();
                                    $('#btnOkAddRequirement').hide().unbind('click');

                                    $('#btnOkCloseRequirement').unbind('click').click(function () {
                                        if ($.trim(txtRequirementName.val()) == '') {
                                            WebToast('Please enter description', 'top');
                                            return
                                        }

                                        Creator.QuestionaireReq[data.Index].Description = $.trim(txtRequirementName.val());
                                        Creator.RenameRequirement(Creator.QuestionaireReq[data.Index]);
                                        $('#popRequirement').modal('hide');
                                    })                                    
                                })
                            )
                            .append('&nbsp;')
                            .append($('<button data-toggle="tooltip" data-placement="bottom" title="Delete" data-role="none" type="button" class="btn btn-default btn-xs">')
                                .append($('<span class="bi bi-trash">'))
                                .data('data', this.QuestionaireReq[i])
                                .click(function () {
                                    var data = $(this).data('data');
                                    Creator.QuestionaireReq[data.Index].State = 'remove';
                                    Creator.DeleteRequirement(Creator.QuestionaireReq[data.Index]);
                                    Creator.EditReq();
                                })
                            )
                        ).hover(function () {
                            $(this).find('span:last-child').show();
                        }, function () {
                            $(this).find('span:last-child').hide();
                        })
                    )

                ctr += 1;
            }
        }

        $('#popListOfRequirements').modal('show');
    },
    CheckReq: function(list){
        var data = list.data('data');

        for (var i = 0; i < data.length; i++){
            list.find('li').find('a[data-value="' + data[i] + '"] > input').prop('checked', true);
        }
    },
    PopulateReqPerNewChoice: function(container){
        for (var i = 0; i < this.QuestionaireReq.length; i++) {
            this.QuestionaireReq[i].Index = i;
            if (this.QuestionaireReq[i].State != 'remove' && CBool(this.QuestionaireReq[i].IsUniversal) == false) {
                container.append(this.AddRequirements(this.QuestionaireReq[i], false))
            }
        }
    },
    PopulateRequirements: function () {
        for (var i = 0; i < this.QuestionaireReq.length; i++) {
            this.QuestionaireReq[i].Index = i;
            if (this.QuestionaireReq[i].State == 'new' && CBool(this.QuestionaireReq[i].IsUniversal) == false) {
                this.AddRequirements(this.QuestionaireReq[i], true)
            } else if (this.QuestionaireReq[i].State == 'edit' && CBool(this.QuestionaireReq[i].IsUniversal) == false) {
                this.RenameRequirement(this.QuestionaireReq[i])
            } else {
                this.DeleteRequirement(this.QuestionaireReq[i])
            }
        }
    },
    AddRequirements: function (data, addToAll) {
        var li = $('<li>')
                .append($('<a class="dropdown-item" href="#" data-role="none">')
                    .css('white-space', 'normal')
                    .attr({
                        'href': '#',
                        'data-role': 'none',
                        'data-value': data.ID
                    })
                    .append($('<input data-role="none" class="form-check-input" type="checkbox">'))
                    .append('&nbsp;')
                    .append($('<span reqid="txtReq-' + data.ID + '">').text(data.Name))
                    .click(function (event) {
                        event.preventDefault();
                        event.stopPropagation();
                        var data = $(this).parents().closest('ul').data('data');
                        //var arrReq = [];

                        //if (CStr2(data) != '') {
                        //    arrReq = data.Requirements;
                        //}

                        var $target = $(event.currentTarget),
                        val = $target.attr('data-value'),
                        $inp = $target.find('input'),
                        idx;

                        if ((idx = data.indexOf(val)) > -1) {
                            data.splice(idx, 1);
                            setTimeout(function () { $inp.prop('checked', false) }, 0);
                        } else {
                            data.push(val);
                            setTimeout(function () { $inp.prop('checked', true) }, 0);
                        }

                        //$(event.target).blur();

                        console.log(data);
                    })
                )                

        if(addToAll){
            $('.listReq').append(li);
        }        

        return li
    },
    RenameRequirement: function (data) {
        $('[reqid="txtReq-' + data.ID + '"]').text(data.Description);
    },
    DeleteRequirement: function (data) {
        $('[data-value="' + data.ID + '"]').parent().remove();
    },
    PopulateDataType: function (ctr) {
        var arrDataType = ["Alphanumeric", "Numeric", "Date", "Email", "URL"];
        var sel = $('<select data-role="none" class="form-select" id="datatype-' + ctr + '">');
        var group = $('<optgroup label="Please select data type">');

        for (var i = 0; i < arrDataType.length; i++){
            group.append(new Option(arrDataType[i], arrDataType[i]))
        }

        return sel.append(group);
    },
    MoreOptions: function (data, callback) {
        var opt = {};
        $('#popSurveyMoreOptions').modal('show');

        $('#lblFieldName').text(data.Label);
        $('#txtCharMin').val(CInt2(data.Min));
        $('#txtCharMax').val(CInt2(data.Max));

        $('#txtCharMin, #txtCharMax').unbind('keydown').keydown(function (e) {
            if(!NumberDotOnly($(this), e)){
                return;
            }
        })

        $('#btnOkSurveyMoreOptions').unbind('click').click(function () {
            opt = {
                Min: CInt2($('#txtCharMin').val()),
                Max: CInt2($('#txtCharMax').val())
            }

            if(opt.Min >= opt.Max){
                WebToast('Minimum character length must lesser than maximum character length', 'top');
                return false;
            }

            callback(opt);
            $('#popSurveyMoreOptions').modal('hide');
        })
    }
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