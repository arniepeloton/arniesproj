require(['common', '/Scripts/jquery-ui-sortable.js'],
    function () {
        $(document).ready(function () {
            var tempState, isAddNewStage = true, specialStageIDForEditing = '', isAddNewChildStage = true, specialStageChildIDForEditing = '';

            window.FunnelTemplate = {
                ShowData: function () {
                    LoadRequiredData();
                }
            }

            LoadRequiredData();

            //Funnel Template
            $('body')
            .on('click', '#btnNewStage', function () {
                isAddNewStage = true;
                $('#popNewStage').modal('show');
                $('#txtStageDescription').val('').focus();
            })
            .on('click', '#btnOkAddNewStage', function () {
                $('#btnOkNewStage').click();
                $('#btnNewStage').click();
            })
            .on('click', '#btnOkNewStage', function () {
                var txtStageDescription = $.trim($('#txtStageDescription').val())

                if (txtStageDescription == '') {
                    WebToast('Please enter stage name.', 'top');
                    return;
                }

                //its either adding or editing stage name
                if (isAddNewStage) {
                    AddStage({
                        SpecialID: NewID(),
                        Name: txtStageDescription,
                        State: 'new'
                    });
                } else {
                    EditStage();
                }

                $('#popNewStage').modal('hide');
            })
            .on('click', '#btnOkAddChildNewStage', function () {
                $('#btnOkNewChildStage').click();
                $('#popNewChildStage').modal('show');
                $('#txtChildStageDescription').val('').focus();
            })
            .on('click', '#btnOkNewChildStage', function () {
                var txtChildStageDescription = $.trim($('#txtChildStageDescription').val())

                if (txtChildStageDescription == '') {
                    WebToast('Please enter child stage name.', 'top');
                    return;
                }

                //its either adding or editing child stage name
                if (isAddNewChildStage) {
                    AddChildStage({
                        SpecialID: NewID(),
                        Name: txtChildStageDescription,
                        State: 'new'
                    });
                } else {
                    EditChildStage();
                }

                $('#popNewChildStage').modal('hide');
            })
            .on('click', '#btnCancelFunnelTemplate', function () {
                window.location.hash = 'LibraryPage';
            })
            .on('click', '#btnSaveFunnelTemplate', function () {
                SaveFunnelTemplate();
            })
            .on('click', '#btnDeleteFunnelTemplate', function () {
                NewMessageBox('Are you sure you want to disable this template?', 'Message',[
                        {
                            Text: 'Yes',
                            Click: function(){
                                tempState = 'delete'
                                SaveFunnelTemplate();
                            }
                        }, {Text: 'No'}
                    ])                
            })

            function SaveFunnelTemplate() {
                var txtFunnelTemplateName = $.trim($('#txtFunnelTemplateName').val());
                var listFunnelTemplate = $('#listFunnelTemplate > li');
                var stageorderno = 1;
                var arrStage = [];

                if (txtFunnelTemplateName == "") {
                    WebToast('Please enter funnel template name', 'top');
                    return;
                }

                if (listFunnelTemplate.length == 0) {
                    WebToast('Please add at least one (1) stage', 'top');
                    return;
                }

                listFunnelTemplate.each(function () {
                    var childstageorderno = 1;
                    var childstage = []
                    $(this).find('ol > li').each(function () {
                        if ($(this).data('data').No == undefined) {
                            $(this).data('data').No = childstageorderno;
                        }
                        childstage.push($(this).data('data'));
                        childstageorderno += 1;
                    })

                    if ($(this).data('data').No == undefined) {
                        $(this).data('data').No = stageorderno;
                    }
                    $(this).data('data').Child = childstage;
                    arrStage.push($(this).data('data'));
                    stageorderno += 1;
                })

                var pData = {
                    ID: $.trim($('#txtFunnelTemplateID').val()),
                    TemplateName: txtFunnelTemplateName,
                    Stages: arrStage,
                    DateNow: $.format.date(new Date(), 'MMMM dd,yyyy hh:mm:ss a'),
                    State: tempState
                }

                AjaxSend({
                    url: 'CRMDashboard/SaveFunnelTemplate',
                    data: pData,
                    success: function (vData) {
                        if (vData.Results == 'saved') {
                            if(tempState == 'delete'){
                                WebToast('Successfully Saved', 'top');
                            } else {
                                WebToast('Successfully Deleted', 'top');
                            }
                            ClearDataFields();
                            CRMLibraryTable.FunnelTemplates();
                            location.hash = 'LibraryPage';
                        } else {
                            NewMessageBox(vData.Results, 'Message');
                        }
                    }
                })
            }

            function AddChildStage(data) {
                var listFunnelTemplateChild = $('[id="childlist-' + specialStageIDForEditing + '"]');

                listFunnelTemplateChild.append($('<li>')
                        .css('margin-top', '15px')
                        .attr('sid', 'child-' + data.SpecialID)
                        .data('data', data)
                        .append($('<span style="font-size: 20px;">').text(data.Name))
                        .append(' ')
                        .append($('<a class="custom-link">')
                            .text('edit')
                            .click(function () {
                                var data = $(this).parent().data('data');
                                isAddNewChildStage = false;
                                specialStageChildIDForEditing = data.SpecialID;
                                $('#popNewChildStage').modal('show');
                                $('#txtChildStageDescription').val(data.Name).focus();
                                $('#txtTitleSubList').text('Edit Sublist');
                            })
                        )
                        .append(' | ')
                        .append($('<a class="custom-link">')
                            .text('delete')
                            .click(function () {
                                var data = $(this).parent().data('data');

                                NewMessageBox('Are you sure you want to permanently delete "' + data.Name + '" from ' +
                                        $('[sid="child-' + data.SpecialID + '"]').parent().parent().find('> span').text() + '?','Message', [
                                            {
                                                Text: 'Yes',
                                                Click: function () {
                                                    if (data.State == "new") {
                                                        $('[sid="child-' + data.SpecialID + '"]').remove();
                                                    } else {
                                                        $('[sid="child-' + data.SpecialID + '"]').hide();
                                                        $('[sid="child-' + data.SpecialID + '"]').data('data').State = 'delete';
                                                    }
                                                }
                                            }, { Text: 'No' }
                                        ])                                
                            })
                        )
                    )
            }

            function EditChildStage() {
                var li = $('[sid="child-' + specialStageChildIDForEditing + '"]');
                var newName = $.trim($('#txtChildStageDescription').val());
                var curData = li.data('data');

                curData.Name = newName;

                li.removeData('data').data('data', curData);
                li.find('> span').text(newName)
            }

            function AddStage(data) {
                var listFunnelTemplate = $('#listFunnelTemplate');

                listFunnelTemplate.append($('<li>')
                        .css('margin-bottom', '15px')
                        .attr('sid', data.SpecialID)
                        .data('data', data)
                        .append($('<span style="font-size: 20px;">').text(data.Name))
                        .append(' ')
                        .append($('<a class="custom-link">')
                            .text('edit')
                            .click(function () {
                                var data = $(this).parent().data('data');
                                isAddNewStage = false;
                                specialStageIDForEditing = data.SpecialID;
                                $('#popNewStage').modal('show');
                                $('#txtStageDescription').val(data.Name).focus();
                            })
                        )
                        .append(' | ')
                        .append($('<a class="custom-link">')
                            .text('add sublist')
                            .click(function () {
                                var data = $(this).parent().data('data');
                                isAddNewChildStage = true;
                                specialStageIDForEditing = data.SpecialID;
                                $('#txtTitleSubList').text('New Sublist for ' + data.Name);
                                $('#popNewChildStage').modal('show');
                                $('#txtChildStageDescription').val('').focus();
                            })
                        )
                        .append(' | ')
                        .append($('<a class="custom-link">')
                            .text('delete')
                            .click(function () {
                                var data = $(this).parent().data('data');

                                NewMessageBox('Are you sure you want to permanently delete "' + data.Name +
                                        ($('#childlist-' + data.SpecialID + '> li').length > 0 ? ' and its sub list?' : '?'), 'Message', [
                                            {
                                                Text: 'Yes',
                                                Click: function () {
                                                    if (data.State == "new") {
                                                        $('[sid="' + data.SpecialID + '"]').remove();
                                                    } else {
                                                        $('[sid="' + data.SpecialID + '"]').hide();
                                                        $('[sid="' + data.SpecialID + '"]').data('data').State = 'delete';
                                                    }
                                                }
                                            }, { Text: 'No' }
                                        ])
                            })
                        )
                        .append($('<ol>')
                            .attr('id', 'childlist-' + data.SpecialID) // csid stands for child-special-id
                        )
                    )

                SortList('listFunnelTemplate');
                SortList('childlist-' + data.SpecialID);
            }

            function EditStage() {
                var li = $('#listFunnelTemplate > li[sid="' + specialStageIDForEditing + '"]');
                var newName = $.trim($('#txtStageDescription').val());
                var curData = li.data('data');

                curData.Name = newName;

                li.removeData('data').data('data', curData);
                li.find('> span').text(newName)
            }

            function SortList(id) {
                setTimeout(function (e) {
                    $('[id="' + e + '"]').sortable({
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
                            $('[id="' + id + '"]').find('> li').each(function () {
                                $(this).data('data').No = newOrderNo;
                                newOrderNo += 1;
                            })
                        }
                    });
                }, 100, id)
            }

            function LoadRequiredData() {
                $('#divMainFunnelTemplatePage').pageshow();
                ClearDataFields();
                tempState = 'new';

                if (sessionStorage.ViewID != undefined) {
                    $('#btnDeleteFunnelTemplate').show();
                    GetTemplateDetails(sessionStorage.ViewID)
                }
            }

            function GetTemplateDetails(id) {
                AjaxSend({
                    url: 'CRMDashboard/GetTemplateDetails',
                    data: { ID: id },
                    success: function (vData) {
                        var vRows = vData.data;
                        tempState = 'edit';
                        $('#txtFunnelTemplateID').val(vRows.ID);
                        $('#txtFunnelTemplateName').val(vRows.TemplateName);

                        for (var i = 0; i < vRows.Stages.length; i++){
                            AddStage({
                                SpecialID: vRows.Stages[i].SpecialID,
                                Name: vRows.Stages[i].Name,
                                State: vRows.Stages[i].State,
                                No: vRows.Stages[i].No
                            })

                            specialStageIDForEditing = vRows.Stages[i].SpecialID;

                            for (var m = 0; m < vRows.Stages[i].Child.length; m++){
                                AddChildStage({
                                    SpecialID: vRows.Stages[i].Child[m].SpecialID,
                                    Name: vRows.Stages[i].Child[m].Name,
                                    State: vRows.Stages[i].Child[m].State,
                                    No: vRows.Stages[i].Child[m].No
                                })
                            }
                        }
                    }
                })
            }

            function ClearDataFields() {
                $('#txtFunnelTemplateID').val('');
                $('#txtFunnelTemplateName').val('');
                $('#listFunnelTemplate').empty();
                $('#btnDeleteFunnelTemplate').hide();
                arrStage = [];
                isAddNewStage = true;
                specialStageIDForEditing = '';
            }
        })
    }
)