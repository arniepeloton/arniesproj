require(['common'], function () {
    $(document).ready(function () {
        var divVDMPortraitAds = $('#divVDMPortraitAds').empty();
        var divVDMIdleAds = $('#divVDMIdleAds').empty();
        var templateID = '';
        var arrAssignedFowst = [], arrSelectedValidFowst = [], arrSelectedInvalidFowst = [], arrNewlyAddedFowst = [];

        if (typeof VDM == 'undefined') {
            window.VDM = {};
        }

        VDM.AdsInit = function () {
            divVDMPortraitAds = $('#divVDMPortraitAds').empty();
            divVDMIdleAds = $('#divVDMIdleAds').empty();
            arrAssignedFowst = [];
            arrSelectedValidFowst = [];
            arrSelectedInvalidFowst = [];
            arrNewlyAddedFowst = [];

            if (CStr2(sessionStorage.ViewID) != '') {
                GetAdsInfo();
            }
        }        

        $('body')
            .on('click', '#btnVDMAdsUpload', function () {
                AddAds(divVDMPortraitAds, 'Portrait');
            })
            .on('click', '#btnVDMIdleAdsUpload', function () {
                AddAds(divVDMIdleAds, 'Landscape');
            })
            .on('click', '#btnVDMSaveAds', function () {
                var txtVDMAdsName = $('#txtVDMAdsName').val();
                var txtVDMAdsDescription = $('#txtVDMAdsDescription').val();
                var arrAds = [];

                if ($.trim(txtVDMAdsName) == '') {
                    NewMessageBox('Please enter template name');
                    return
                }

                $('.vdmads').each(function () {
                    var dis = $(this);
                    var adid = dis.attr('adid');
                    var img = dis.find('.fileads')[0].files[0];
                    var formdata = new FormData();

                    if (img != undefined) {
                        formdata.append(img.name, img);                        
                    }

                    arrAds.push({
                        ID: adid,
                        Img: formdata,
                        IsMedia: dis.find('.fileads').attr('ismedia'),
                        Orientation: dis.find('.fileads').attr('orientation'),
                        IsRemove: dis.hasClass('isremove')
                    })
                })

                Bs.Loading('show');

                AjaxSend({
                    url: 'VendingMachine/SaveAds',
                    data: {
                        TemplateID: templateID,
                        Name: $.trim(txtVDMAdsName),
                        Description: $.trim(txtVDMAdsDescription),
                        Ads: arrAds,
                        AssignedFowst: arrAssignedFowst
                    }, success: function (e) {
                        if (e.Results == 'saved') {
                            var fileimg = $('.fileads'), ctrFile = 0, hasFile = false, ctrUploaded = 0;
                            fileimg.each(function () {
                                var dis = $(this);
                                if (dis.val() != '') {
                                    ctrFile += 1;
                                    hasFile = true;
                                    var img = dis[0].files[0];
                                    var formdata = new FormData();

                                    formdata.append('AdID', dis.parents().closest('div.vdmads').attr('adid'));
                                    formdata.append('Media', img.type.split('/')[0]);
                                    formdata.append(img.name, img);
                                    Bs.Loading('show');
                                    AjaxSend({
                                        url: 'VendingMachine/SaveImageAds?OrgID=' + sessionStorage.OrgID,
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
                                                LibraryTable.VDMAds();
                                                window.location.hash = 'LibraryPage';
                                            }

                                            if (CStr2(vData.Error) != "") {
                                                Bs.Loading('hide');
                                                NewMessageBox(vData.Error, "Message");
                                            }
                                        }
                                    });
                                }
                            })

                            if (!hasFile) {
                                Bs.Loading('hide');
                                LibraryTable.VDMAds();
                                window.location.hash = 'LibraryPage';
                            }
                        } else {
                            Bs.Loading('hide');
                            NewMessageBox(e.Results, "Message");
                        }
                    }
                })
            })
            .on('click', '#btnVDMAdsAddFOWST', function () {
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
                $('#listVDMAdsFowst').empty();
                arrNewlyAddedFowst = data;
            }

            arrAssignedFowst = $.merge(arrAssignedFowst, arrNewlyAddedFowst);

            AssignedFOWST(arrAssignedFowst);
        }

        function AssignedFOWST(data) {
            var listVDMAdsFowst = $('#listVDMAdsFowst');

            if (data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    data[i].Index = i;
                    if (listVDMAdsFowst.find('a[fid="' + data[i].ID + '"]').length == 0) {
                        arrAssignedFowst.push(data[i]);
                        listVDMAdsFowst.append($('<a href="#" class="list-group-item list-group-item-action">')
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
                                                            Bs.Loading('show');
                                                            AjaxSend({
                                                                url: '/ProductGroup/RemoveFOWSTHB',
                                                                data: { FOWSTID: data.ID },
                                                                success: function (e) {
                                                                    Bs.Loading('hide');
                                                                    if (e.Result == 'removed') {
                                                                        dis.remove();
                                                                        WebToast('Successfully removed')
                                                                    } else {
                                                                        NewMessageBox(e.Error, 'Error')
                                                                    }
                                                                }
                                                            })
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
                listVDMAdsFowst.append($('<a href="#" class="list-group-item list-group-item-action">').text('No result(s) found'));
            }

            listVDMAdsFowst;//.listview('refresh');
        }
        function AddAds(container, orientation, data) {
            var col = $('<div class="col mb-3 vdmads">')                
                    .append($('<img class="rounded d-none w-100" style="height: 300px; object-fit:cover;">'))
                    .append($('<video controls class="rounded d-none w-100" style="height: 300px; object-fit:cover;"></video>'))
                    .append($('<div class="input-group">')
                    .append($('<input data-role="none" type="file" accept="image/*, video/mp4" orientation="' + orientation + '" class="form-control fileads" ismedia="0">')
                        .change(function (e) {
                            var dis = $(this), file = e.target.files[0];

                            if (file.type.split('/')[0] == 'video') {
                                var vidurl = URL.createObjectURL(file);
                                dis.attr('ismedia', 1)
                                dis.parent().parent().find('video').removeClass('d-none').attr('src', vidurl);
                            } else {
                                var reader = new FileReader();
                                reader.onload = function () {
                                    dis.attr('ismedia', 1)
                                    dis.parent().parent().find('img').removeClass('d-none').attr('src', reader.result);
                                };
                                reader.readAsDataURL(file);
                            }
                        })
                )
                    .append($('<button data-role="none" class="btn btn-light border text-dark" title="Remove this ad">')
                        .html('<span class="bi-trash-fill"></span>')
                        .click(function () {
                            var parentDiv = $(this).parent().parent();

                            NewMessageBox('Are you sure you want to remove this ad?', 'Confirmation Dialog', [
                                {
                                    Text: 'Yes', Click: function () {
                                        if (parentDiv.data('data') != undefined) {
                                            parentDiv.addClass('d-none isremove');
                                        } else {
                                            parentDiv.remove();
                                        }
                                    }
                                }, { Text: 'No' }
                            ])
                        })
                    )
                )                

            if (data == undefined) {
                col.attr('adid', NewID());
            } else {
                col.attr('adid', data.ID);
                col.data('data', data);
            }

            container.append(col);
            return col;
        }

        function GetAdsInfo() {
            AjaxSend({
                url: 'VendingMachine/GetAdsInfo',
                data: { ID: sessionStorage.ViewID },
                success: function (e) {
                    var info = e.Info[0];
                    var ads = e.Ads;

                    templateID = info.ID;
                    $('#txtVDMAdsName').val(info.Name);
                    $('#txtVDMAdsDescription').val(info.Description);
                    arrAssignedFowst = e.AssignedMachine;
                    AssignedFOWST(e.AssignedMachine);
                    for (var i in ads) {
                        var html = AddAds((ads[i].Orientation == 'Portrait' ? divVDMPortraitAds : divVDMIdleAds), ads[i].Orientation, ads[i]);

                        if (CStr2(ads[i].FileName) != '') {
                            var med;
                            if (ads[i].Media == 'image') {
                                med = html.find('img');
                            } else {
                                med = html.find('video');
                            }
                            html.find('.fileads').attr('ismedia', 1);
                            med.attr('src', '/Content/VendingMachine/' + sessionStorage.OrgID + '/' + ads[i].FileName).removeClass('d-none');
                        }
                    }
                }
            })
        }

        VDM.AdsInit();
    })
})