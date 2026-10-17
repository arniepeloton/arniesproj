require(['common'], 
    function () {
        $(document).ready(function () {
            var arrStage = [];
            var isNewStage = false;
            var stageIndex = 0, stageState = '';;

            window.ClientStage = {
                LoadData: function () {
                    LoadRequiredData();
                },
                ShowData: function () {
                    LoadRequiredData();
                }
            }

            LoadRequiredData();

            //Client Stage
            $('body')
            .on('click', '#btnAddNewStage', function () {
                $('#popStage').modal('show');
                $('#txtStageName').val('').focus();
                $('#txtStageDescription').val('').removeAttr('style');
                isNewStage = true;
            })
            .on('click', '#btnOkAddStage', function () {
                if ($.trim($('#txtStageName').val()) == "") {
                    WebToast('Please enter stage name');
                    return;
                }

                if(isNewStage == true){
                    arrStage.push({
                        ID: $('#txtStageID').val(),
                        Stage: $('#txtStageName').val(),
                        Description: $('#txtStageDescription').val(),
                        State: 'new'
                    })
                } else {
                    arrStage[stageIndex].ID = $('#txtStageID').val();
                    arrStage[stageIndex].Stage = $('#txtStageName').val();
                    arrStage[stageIndex].Description = $('#txtStageDescription').val();
                    arrStage[stageIndex].State = stageState;
                }

                LoadStage();
                $('#popStage').modal('hide');
            })
            .on('click', '#btnCancelStage', function () {
                window.location.hash = 'LibraryPage';
            })
            .on('click', '#btnSaveStage', function () {
                if ($.trim($('#txtGroupStageName').val()) == "") {
                    WebToast('Please enter group stage name')
                    return;
                }

                //if(arrStage.length == 0){
                //    WebToast('Please add at least one stage')
                //    return;
                //}

                SaveStage();
            })
            .on('click', '#btnRemoveStage', function () {
                NewMessageBox('Are you sure you want to remove this group stage?', 'Remove',
                    [
                        {
                            Text: 'Yes',
                            Click: function () {
                                RemoveGroupStage(sessionStorage.ViewID)
                            }
                        }, { Text: 'No' }
                    ]
                    )
            })

            function RemoveGroupStage(id) {
                $('#btnSaveStage, #btnCancelStage, #btnRemoveStage').button('disable')
                AjaxSend({
                    url: 'CRMDashboard/RemoveGroupStage',
                    data: { ID: id },
                    success: function (vData) {
                        $('#btnSaveStage, #btnCancelStage, #btnRemoveStage').button('enable');
                        WebToast('Successfully Deleted');
                        window.location.hash = 'LibraryPage';
                        LibraryTable.ClientStage();
                    }
                })
            }

            function GetStage(ID) {
                AjaxSend({
                    url: 'CRMDashboard/GetStage',
                    data: { ID: ID },
                    success: function (vData) {
                        var vRows = vData.Results.Rows;

                        for (var i = 0; i < vRows.length; i++) {
                            arrStage.push({
                                ID: vRows[i].ID,
                                Stage: vRows[i].StageName,
                                Description: vRows[i].Description,
                                State: 'edit'
                            })
                        }

                        LoadStage();
                    }
                })
            }

            function SaveStage() {
                $('#btnSaveStage, #btnCancelStage, #btnRemoveStage').button('disable')
                AjaxSend({
                    url: 'CRMDashboard/SaveStage',
                    data: { Stage: arrStage, Date: $.format.date(new Date(), 'MMM dd, yyyy hh:mm a'), GroupName: $('#txtGroupStageName').val(), ID: $('#txtGroupStageID').val() },
                    success: function (vData) {
                        $('#btnSaveStage, #btnCancelStage, #btnRemoveStage').button('enable');
                        if (vData.Results == "saved") {
                            arrStage = [];
                            WebToast('Successfully Saved');
                            window.location.hash = 'LibraryPage';
                            LibraryTable.ClientStage();
                        }
                        else {
                            NewMessageBox(vData.Results)
                        }
                    }
                })
            }

            function LoadStage() {
                var listStages = $('#listStages');
                listStages.empty();

                if(arrStage.length > 0){
                    for (var i = 0; i < arrStage.length; i++) {
                        arrStage[i].Index = i;
                        if(arrStage[i].State != 'remove'){
                            listStages.append($('<li class="list-group-item">')
                                    .data(arrStage[i])
                                    .append($('<table style="width:100%">')
                                        .append($('<tr>')
                                            .append($('<td style="width:50px;">')
                                                .text((i + 1) + '.')
                                            )
                                            .append($('<td>')
                                                .html(arrStage[i].Description.replace(/\r\n|\r|\n/g, "<br />"))
                                            )
                                            .append($('<td class="text-center" style="width:50px;">')
                                                .append($('<button data-role="none" class="btn btn-danger">')
                                                    .append($('<span class="bi-trash-fill">'))
                                                    .data('data', arrStage[i])
                                                    .click(function (e) {
                                                        e.preventDefault();
                                                        e.stopPropagation();

                                                        var data = $(this).data('data');

                                                        NewMessageBox('Are you sure you want to remove this Stage?', 'Confirmation Dialog', [
                                                            {
                                                                Text: 'Yes', Click: function () {
                                                                    if (data.State == 'new') {
                                                                        arrStage = $.grep(arrStage, function (e) {
                                                                            return e.Index != data.Index;
                                                                        })
                                                                    } else {
                                                                        arrStage[data.Index].State = 'remove';
                                                                    }

                                                                    LoadStage();
                                                                }
                                                            }, { Text: 'No' }
                                                        ])
                                                    })
                                                )
                                            )
                                        )
                                    )
                                    .click(function () {
                                        isNewStage = false;
                                        var data = $(this).data();
                                        stageIndex = data.Index;
                                        stageState = data.State;
                                        $('#txtStageID').val(data.ID);
                                        $('#txtStageName').val(data.Stage);
                                        $('#txtStageDescription').val(data.Description)
                                        $('#popStage').modal('show');
                                    })
                                )
                        }
                    }
                } else {
                    listStages.append($('<li class="list-group-item">').text('Nothing to show'))
                }

                //listStages.listview('refresh');
            }

            function LoadRequiredData() {
                $('#divMainClientStagePage').pageshow();
                ClearFields();

                if (sessionStorage.AddViewLibBCName == "View Stage") {
                    arrStage = [];
                    $('#listStages').empty();                    
                    $('#txtGroupStageID').val(sessionStorage.ViewID);
                    $('#txtGroupStageName').val(sessionStorage.GroupStageName);
                    $('#divRemoveStage').show();
                    GetStage(sessionStorage.ViewID);
                } else {
                    $('#divRemoveStage').hide();
                }
            }

            function ClearFields() {
                $('#txtStageName').val('').focus();
                $('#txtStageDescription').val('').removeAttr('style');
                $('#txtGroupStageID').val('');
                $('#txtStageID').val('');
                $('#txtGroupStageName').val('');                
                isNewStage = false;
            }
        })
    }
)