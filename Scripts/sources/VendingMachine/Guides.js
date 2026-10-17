require(['common'], function () {
    $(document).ready(function () {
        var stepctr = 1;
        var templateID = '';
        var arrAssignedFowst = [], arrSelectedValidFowst = [], arrSelectedInvalidFowst = [], arrNewlyAddedFowst = [];

        if (typeof VDM == 'undefined') {
            window.VDM = {};
        }

        VDM.GuidesInit = function () {
            stepctr = 1;
            templateID = '';
            arrAssignedFowst = [];
            arrSelectedValidFowst = [];
            arrSelectedInvalidFowst = [];
            arrNewlyAddedFowst = [];
            if (CStr2(sessionStorage.ViewID) != '') {
                GetGuidesInfo();
            }
        }        

        $('body')
            .on('click', '#btnVDMAddStep', function () {
                if ($('#selVDMModelBTemplate option:selected').val() == '') {
                    NewMessageBox('Please select Model-B Template first before you can add new page.')
                } else {
                    AddStep();
                }
            })
            .on('click', '#btnVDMSavePage', function () {
                var txtVDMGuideTemplateName = $.trim($('#txtVDMGuideTemplateName').val());
                var txtVDMGuideTemplateDescription = $.trim($('#txtVDMGuideTemplateDescription').val());
                var selVDMGuideDispensingType = $.trim($('#selVDMGuideDispensingType').val());
                var arrSteps = [], templateerror = false;

                if (txtVDMGuideTemplateName == '') {
                    NewMessageBox('Please enter template name')
                    return;
                }

                if (selVDMGuideDispensingType == '') {
                    NewMessageBox('Please select dispensing type')
                    return;
                }

                $('.vdmsteps').each(function () {
                    var dis = $(this);                    
                    var stepid = dis.attr('stepid');
                    var txtStep = $.trim(dis.find('.txtStep').val());
                    var selOnScreenTime = $.trim(dis.find('.selOnScreenTime option:selected').val());
                    var chbIsDisplayInfo = dis.find('.chbIsDisplayInfo').is(':checked')
                    var img = dis.find('.fileimg')[0].files[0];
                    var formdata = new FormData();

                    if (img != undefined) {
                        formdata.append(img.name, img);
                    }                    

                    if (txtStep != '') {
                        arrSteps.push({
                            ID: stepid,
                            Step: txtStep,
                            Img: formdata,
                            HasAnImage: dis.find('.fileimg').attr('hasanimage'),
                            OnScreenTime: selOnScreenTime,
                            IsDisplayInfo: chbIsDisplayInfo,
                            IsRemove: dis.hasClass('isremove')
                        })
                    }
                })

                if (arrSteps.length == 0) {
                    NewMessageBox('Please enter at least one step.')
                    return false;
                }

                var arrTemplate = {
                    TemplateID: templateID,
                    Name: txtVDMGuideTemplateName,
                    Description: txtVDMGuideTemplateDescription,
                    DispensingType: selVDMGuideDispensingType,
                    Steps: arrSteps,
                    AssignedFowst: arrAssignedFowst
                }
                Bs.Loading('show');
                AjaxSend({
                    url: 'VendingMachine/SaveGuides',
                    data: arrTemplate,
                    type: 'POST',
                    async: true,
                    cache: false,
                    contentType: false,
                    processData: false,
                    success: function (e) {
                        if (e.Results == 'saved') {
                            var fileimg = $('.fileimg'), ctrFile = 0, hasFile = false, ctrUploaded = 0;
                            fileimg.each(function () {
                                var dis = $(this);
                                if (dis.val() != '') {
                                    ctrFile += 1;
                                    hasFile = true;
                                    var img = dis[0].files[0];
                                    var formdata = new FormData();

                                    formdata.append('StepID', dis.parents().closest('div.vdmsteps').attr('stepid'))
                                    formdata.append('Media', img.type.split('/')[0]);
                                    formdata.append(img.name, img);
                                    Bs.Loading('show');
                                    AjaxSend({
                                        url: 'VendingMachine/SaveImage?OrgID=' + sessionStorage.OrgID,
                                        type: 'POST',
                                        data: formdata,
                                        async: true,
                                        cache: false,
                                        contentType: false,
                                        processData: false,
                                        success: function (vData) {                                            
                                            ctrUploaded += 1;
                                            if (vData.Results == "saved" && ctrUploaded == ctrFile) {
                                                Bs.Loading('hide');
                                                LibraryTable.VDMRiceDispensingGuides();
                                                window.location.hash = 'LibraryPage';
                                            }

                                            if (vData.Error != "") {
                                                Bs.Loading('hide');
                                                NewMessageBox(vData.Error, "Message");
                                            }
                                        }
                                    });
                                }
                            })

                            if (!hasFile) {
                                Bs.Loading('hide');
                                LibraryTable.VDMRiceDispensingGuides();
                                window.location.hash = 'LibraryPage';
                            }
                        } else {
                            Bs.Loading('hide');
                            NewMessageBox(e.Results, "Message");
                        }
                    }
                })
            })
            .on('click', '#btnVDMSCancelPage', function () {
                window.location.hash = 'LibraryPage';
            })
            .on('click', '#btnVDMAddFOWST', function () {
                arrSelectedValidFowst = [];
                arrSelectedInvalidFowst = [];
                function validate(e) {
                    for (var i = 0; i < e.length; i++) {
                        e[i].State = 'new';

                        //if (CStr2(e[i].HBTemplateID) == '') {
                            arrSelectedValidFowst.push(e[i]);
                        //} else {
                        //    arrSelectedInvalidFowst.push(e[i]);
                        //}
                    }

                    if (arrSelectedInvalidFowst.length > 0) {
                        NewMessageBox('Some of selected FOWST locations are already assigned to another Hot Button Template.<br><br> What would you like to do?', 'Message', [
                            {
                                Text: 'Add all selected FOWST locations to this template',
                                Click: function () {
                                    ValidateSelectedFowst($.merge(arrSelectedValidFowst, arrSelectedInvalidFowst));
                                }
                            },
                            {
                                Text: 'Do not add selected FOWST locations that already have another assigned Hot Button template',
                                Click: function () {
                                    ValidateSelectedFowst(arrSelectedValidFowst);
                                }
                            },
                            {
                                Text: 'Cancel'
                            }
                        ])
                    } else {
                        ValidateSelectedFowst(arrSelectedValidFowst);
                    }
                }
                PopupButtonList('', 'Options', [
                    {
                        Text: 'Select from Company\'s FOWST list', Click: function () {
                            BrowsePopup('LibraryFOWST', 'Select FOWST Locations', [
                                {
                                    DisplayText: [{ Column: 'FowstName' }],
                                    AutoSearch: true,
                                    SelectMultiple: true,
                                    Message: 'You can select multiple FOWST locations. Click OK when you\'re done.',
                                    Click: function (vData) {
                                        validate(vData);
                                    }
                                }
                            ], { ShowHBTemplate: true, IsAll: true })
                        }
                    }, {
                        Text: 'Select from Group\'s FOWST list', Click: function () {
                            BrowsePopup('LibraryFranchiseGroup', 'Select Group', [
                                {
                                    DisplayText: [{ Column: 'GroupName' }],
                                    AutoSearch: true,
                                    Message: '<b>Note:</b> FOWST location(s) of the selected group will be added on the list.',
                                    Click: function (e) {
                                        Bs.Loading('show');

                                        AjaxSend({
                                            url: 'ItemModifiers/GetFowstOfSelectedGroup',
                                            data: {
                                                GroupID: e.ID,
                                                ShowHBTemplate: true,
                                            }, success: function (e) {
                                                Bs.Loading('hide');
                                                validate(e.Results.Rows);
                                            }
                                        })
                                    }
                                }
                            ])
                        }
                    }
                ])
            })

        function ValidateSelectedFowst(data) {
            if (arrAssignedFowst.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    var isExist = false, index;

                    $.grep(arrAssignedFowst, function (a, idx) {
                        if (a.ID == data[i].ID) {
                            isExist = true;
                            index = idx;
                            return false;
                        }
                    })

                    if (!isExist) {
                        arrNewlyAddedFowst.push(data[i]);
                    } else {
                        arrAssignedFowst[index].State = 'edit';
                    }
                }
            } else {
                $('#listVDMGuideFowst').empty();
                arrNewlyAddedFowst = data;
            }

            arrAssignedFowst = $.merge(arrAssignedFowst, arrNewlyAddedFowst);

            AssignedFOWST(arrAssignedFowst);
        }

        function AssignedFOWST(data) {
            var listVDMGuideFowst = $('#listVDMGuideFowst');

            if (data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    data[i].Index = i;
                    if (listVDMGuideFowst.find('a[fid="' + data[i].ID + '"]').length == 0) {
                        listVDMGuideFowst.append($('<a href="#" class="list-group-item list-group-item-action">')
                            .attr('fid', data[i].ID)
                            .append($('<table style="width:100%;">')
                                .append($('<tr>')
                                    .append($('<td style="word-break:break-all;">')
                                        .data('data', data[i])
                                        .append($('<h5>')
                                            .attr("style", "white-space:normal")
                                            .text(data[i].Name || data[i].FowstName)
                                        )
                                    )
                                    .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                        .append($('<span class="bi-trash-fill" aria-hidden="true">'))
                                        .data('data', data[i])
                                        .click(function (e) {
                                            e.preventDefault();
                                            var dis = $(this);
                                            var data = dis.data('data');

                                            NewMessageBox('Are you sure you want to remove this FOWST location to this template?', 'Message', [
                                                {
                                                    Text: 'Yes', Click: function () {
                                                        dis.parents().closest('a').remove();
                                                        if (data.State == 'new') {
                                                            arrAssignedFowst.splice(data.Index, 1);
                                                        } else {
                                                            data.State = 'delete';
                                                        }
                                                    }
                                                }, { Text: 'No' }
                                            ])
                                        })
                                    )
                                    )
                                )
                            )
                        )
                    }
                }
            } else {
                listVDMGuideFowst.append($('<a href="#" class="list-group-item list-group-item-action">').text('No result(s) found'));
            }

            listVDMGuideFowst;//.listview('refresh');
        }

        function AddStep(pageData) {
            var newPage = $('#dvStepTemplate').clone();

            if (typeof pageData == 'undefined') {
                newPage.attr('stepid', NewID())
            } else {
                newPage.attr('stepid', pageData.ID);
                newPage.data('data', pageData);
            }

            newPage.find('input.fileimg').attr('hasanimage', 0).change(function (e) {
                $(this).parents().closest('li').find('a:eq(0)').removeData('data');
                $(this).attr('hasanimage', 1);
                $(this).attr('media', e.target.files[0].type.split('/')[0])
                PreviewImage(newPage, null, e.target.files[0]);
            })

            newPage.find('.btndelete').click(function () {
                var parentDiv = $(this).parents().closest('li'), pData = parentDiv.find('.vdmsteps').data('data');

                if (pData == undefined) {
                    parentDiv.remove();
                } else {
                    parentDiv.addClass('d-none isremove');
                    parentDiv.find('.vdmsteps').addClass('d-none isremove');
                }
            })
            
            newPage.attr('stepctr', stepctr).addClass('vdmsteps').removeAttr('id').removeClass('d-none');
            $('#divVDMSteps').append($('<li class="mb-5">').append(newPage));
            //PopulateSelectionButton(newPage.find('.selPagePurpose').val(), newPage, model);
            stepctr += 1;
            return newPage
        }

        function GetGuidesInfo() {
            AjaxSend({
                url: 'VendingMachine/GetGuidesInfo',
                data: { ID: sessionStorage.ViewID },
                success: function (e) {
                    var info = e.Info[0];
                    var steps = e.Steps;

                    templateID = info.ID;
                    $('#txtVDMGuideTemplateName').val(info.Name);
                    $('#txtVDMGuideTemplateDescription').val(info.Description);
                    $('#selVDMGuideDispensingType').val(info.DispensingType || 'Rice');
                    arrAssignedFowst = e.AssignedMachine;
                    AssignedFOWST(e.AssignedMachine);
                    for (var i in steps) {
                        var html = AddStep(steps[i]);
                        html.find('textarea').val(steps[i].Description);
                        html.find('select').val(steps[i].OnScreenTime);                        
                        html.find('.chbIsDisplayInfo').prop('checked', CInt2(steps[i].IsDisplayInfo));

                        if (CStr2(steps[i].Filename) != '') {
                            html.find('.fileimg').attr('hasanimage', 1);
                            html.find('.fileimg').attr('media', steps[i].Media);
                            PreviewImage(html, steps[i]);
                        }
                    }
                }
            })
        }

        function PreviewImage(html, data, imgdata) {
            html.find('a:eq(0)').data('data', data).text('Click here to view picture').unbind().click(function () {
                var popImagePreview = $('#popImagePreview'), vdata = $(this).data('data');

                popImagePreview.find('img, video').addClass('d-none');

                if (vdata == null) {
                    var file = $(this).parents().closest('li').find('.fileimg')[0].files[0];
                    console.log(file)
                    if (file.type.split('/')[0] == 'video') {
                        var vidurl = URL.createObjectURL(file);
                        popImagePreview.find('video').removeClass('d-none').attr('src', vidurl);
                    } else {
                        var reader = new FileReader();
                        reader.onload = function () {
                            popImagePreview.find('img').attr('src', reader.result).removeClass('d-none');
                        };
                        reader.readAsDataURL(file);
                    }
                } else {
                    if (vdata.Media == 'video') {
                        popImagePreview.find('video').attr('src', '/Content/VendingMachine/' + sessionStorage.OrgID + '/' + vdata.Filename).removeClass('d-none');
                    } else {
                        popImagePreview.find('img').attr('src', '/Content/VendingMachine/' + sessionStorage.OrgID + '/' + vdata.Filename).removeClass('d-none');
                    }
                }
                
                popImagePreview.modal('show');
            })

            html.find('a:eq(1)').html('<span class="badge bg-light text-dark"><span class="bi bi-x-lg"></span></span>').click(function () {
                html.find('.fileimg').attr('hasanimage', 0);
                html.find('a').text('');
            })
        }

        VDM.GuidesInit();
    })
})