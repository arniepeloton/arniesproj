require(['common'], function () {
    $(document).ready(function () {
        var arrSettings = ["FOWST", "Authorized Devices", "Receipt Settings", "Security Roles", "Employees",
                            "Printer Roles", "Hot Button Settings", "Group Settings", "Customer List", "Supplier List",
                            "Sales Team"];

        window.VDMGeneralSettings = {
            Load: function () {
                //GetGeneralSettings();
            }
        }

        $('body')
            .on('click', '.btnCancelGeneralSettings', function () {
                window.location.hash = 'SystemAdministration';
            })
            .on('click', '#btnSaveGeneralSettings', function () {
                var arrAds = [], ctrSave = 0, ctrGuides = $('.vdmproductguides').length;

                $('.vdmproductguides').each(function () {
                    var dis = $(this);
                    var fileinfo = dis.data('fileinfo');
                    var filedata = dis.data('filedata');
                    var psgid = dis.attr('psgid');
                    var formdata = new FormData();
                    
                    if (filedata != undefined) {
                        formdata.append(psgid, filedata);                        
                    }

                    if (fileinfo != undefined) {
                        formdata.append('CustomName', fileinfo.CustomName);
                        formdata.append('Description', fileinfo.Description);
                    }

                    if (CBool(dis.attr('isfromexistingmedia'))) {
                        formdata.append('MediaRefID', dis.attr('MediaRefID'));
                    } else formdata.append('MediaRefID', '');

                    formdata.append('ID', psgid);
                    formdata.append('IsMedia', dis.attr('ismedia'));
                    formdata.append('IsNew', dis.attr('isnew'));
                    formdata.append('Page', dis.attr('page'));
                    formdata.append('IsRemove', dis.hasClass('isremove'));
                    formdata.append('Media', dis.attr('media'));                    

                    AjaxSend({
                        url: 'VendingMachine/SaveProductSelectionGuides?OrgID=' + sessionStorage.OrgID,
                        type: 'POST',
                        data: formdata,
                        async: true,
                        cache: false,
                        contentType: false,
                        processData: false,
                        success: function (vData) {
                            ctrSave += 1;

                            if (ctrSave == ctrGuides) {
                                NewMessageBox('Successfully saved');
                                GetGeneralSettings();
                            }

                            console.log(vData);
                        }
                    });
                    //arrAds.push({
                    //    ID: psgid,
                    //    IsMedia: dis.attr('ismedia'),
                    //    Page: dis.attr('page'),
                    //    IsRemove: dis.hasClass('isremove')
                    //})
                })

                //Bs.Loading('show');
            })
            .on('click', '#btnVDMMenuPageExistingFiles', function () {
                SelectExistingMedia($('#divMenuPageGuides'), 'menu');
            })
            .on('click', '#btnVDMMenuPageUpload', function () {
                SelectFile($('#divMenuPageGuides'), 'menu');
            })
            .on('click', '#btnVDMProductDetailsPageUpload', function () {
                SelectFile($('#divProductDetailsPageGuides'), 'productdetails');
            })
            .on('click', '#btnVDMCartPageUpload', function () {
                SelectFile($('#divCartPageGuides'), 'cart');
            })
            .on('click', '#btnVDMPaymentPageUpload', function () {
                SelectFile($('#divPaymentPageGuides'), 'payment');
            })
            .on('click', '.txtdenom', function () {
                var dis = $(this);

                BrowsePopup('LibraryPimProduct', 'Select Denomination', [
                    {
                        DisplayText: [{ Column: 'ProductNo', Label: 'Product No.' }, { Column: 'ProductName', Label: 'Description' }],
                        Click: function (e) {
                            dis.attr('denomid', e.ID).val(e.ProductNo + ' - ' + e.ProductName);
                        }
                    }
                ])
            })
            .on('click', '#btnSaveDefaultCoins', function () {
                var denom = [25, 1, 5, 10, 20, 50, 100, 200, 500, 1000];
                var data = {};

                for (var i in denom) {
                    data[denom[i] + '_' + (denom[i] == 25 ? 'cent' : 'peso')] = CStr2($('.denom_' + denom[i]).attr('denomid'));
                }

                AjaxSend({
                    url: 'VendingMachine/SaveDefaultCoins',
                    data: { data: data },
                    success: function (e) {
                        if (e.Result == 'saved') {
                            NewMessageBox('Successfully saved');
                            GetGeneralSettings();
                        } else {
                            NewMessageBox(e.Error, 'Error');
                        }
                    }
                })
            })

        function SelectExistingMedia(container, page) {
            BrowsePopup('LibraryVDMMedia', 'Select from uploaded media', [
                {
                    DisplayText: [{ Column: 'Name' }, { Column: 'Description' }],
                    SelectMultiple: true,
                    Click: function (e) {
                        for (var i in e) {
                            var fileContainer = DisplayFile(container, page);

                            fileContainer.attr('isfromexistingmedia', 1)
                            fileContainer.attr('ismedia', 1)
                            fileContainer.attr('isnew', 0)
                            fileContainer.attr('media', e[i].Media)
                            fileContainer.attr('MediaRefID', e[i].ID)
                            fileContainer.find('[name="MediaFileName"]').attr('title', e[i].Name).text(e[i].Name);

                            if (e[i].Media == 'video') {
                                fileContainer.find('video').removeClass('d-none').attr('src', '/Content/VendingMachine/' + sessionStorage.OrgID + '/' + e[i].FileName);
                            } else {
                                fileContainer.find('img').removeClass('d-none').attr('src', '/Content/VendingMachine/' + sessionStorage.OrgID + '/' + e[i].FileName);
                            }
                        }
                    }
                }
            ])
        }

        function SelectFile(container, page) {
            $('#txtAdFileName, #txtAdFileDescription, #fileInputVDMPSGuides').val('');

            $('#butAddAds').unbind().click(function () {
                var files = Array.from($('#fileInputVDMPSGuides')[0].files);

                files.forEach(function (file) {
                    var fileContainer = DisplayFile(container, page);
                    var customFileName = $('#txtAdFileName').val();

                    fileContainer.data('fileinfo', {
                        CustomName: customFileName,
                        Description: $('#txtAdFileDescription').val()
                    });
                    fileContainer.data('filedata', file);
                    fileContainer.attr('isfromexistingmedia', 0);
                    fileContainer.attr('ismedia', 1)
                    fileContainer.attr('isnew', 1)
                    fileContainer.attr('media', file.type.split('/')[0])

                    fileContainer.find('[name="MediaFileName"]').attr('title', customFileName).text(customFileName)

                    if (file.type.split('/')[0] == 'video') {
                        var vidurl = URL.createObjectURL(file);
                        fileContainer.find('video').removeClass('d-none').attr('src', vidurl);
                    } else {
                        var reader = new FileReader();
                        reader.onload = function () {
                            fileContainer.find('img').removeClass('d-none').attr('src', reader.result);
                        };
                        reader.readAsDataURL(file);
                    }
                })

                $(this).val('');
                $('#popVDMAds').modal('hide');
            })

            $('#fileInputVDMPSGuides').unbind().change(function (e) {
                var files = Array.from(e.target.files);

                files.forEach(function (file) {
                    $('#txtAdFileName').val(file.name);
                })
            })

            $('#popVDMAds').modal('show');
        }

        function DisplayFile(container, page, data) {
            var col = $('<div class="col m-2 vdmproductguides position-relative">')
                .attr('page', page)
                .append($('<div class="rounded-3 border p-2 bg-white shadow-sm">')
                    .append($('<button data-role="none" class="btn bg-white shadow-sm text-danger position-absolute" style="top:20px; right:20px; z-index:1;">')
                        .append('<span class="bi-trash-fill"></span>')
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
                    .append($('<img class="rounded-2 d-none w-100" style="height: 300px; object-fit:cover;">'))
                    .append($('<video controls class="rounded-2 d-none w-100" style="height: 300px; object-fit:cover;"></video>'))
                    .append('<div name="MediaFileName" class="text-secondary mt-2 text-truncate" style="font-size:14px;"></div>')
                )                

            if (data == undefined) {
                col.attr('psgid', NewID());
            } else {
                col.attr('psgid', data.ID);
                col.data('data', data);
            }

            container.append(col);
            return col;
        }

        function GetGeneralSettings() {
            AjaxSend({
                url: 'VendingMachine/GetGeneralSettings',
                success: function (e) {
                    var guides = e.Guides.Rows;
                    var denomsku = e.Denom.Rows;
                    var denom = [25, 1, 5, 10, 20, 50, 100, 200, 500, 1000];

                    $('.divPSGuidesContainer').empty();
                    for (var i in guides) {
                        var gData = guides[i], container;

                        if (gData.Page == 'menu') {
                            container = '#divMenuPageGuides';
                        } else if (gData.Page == 'productdetails') {
                            container = '#divProductDetailsPageGuides';
                        } else if (gData.Page == 'cart') {
                            container = '#divCartPageGuides';
                        } else if (gData.Page == 'payment') {
                            container = '#divPaymentPageGuides';
                        }

                        var html = DisplayFile($(container), gData.Page, gData);

                        if (CStr2(gData.FileName) != '') {
                            var med;
                            if (gData.Media == 'image') {
                                med = html.find('img');
                            } else {
                                med = html.find('video');
                            }
                            html.data('fileinfo', {
                                CustomName: gData.CustomName,
                                Description: gData.Description
                            })
                            html.find('[name="MediaFileName"]').attr('title', gData.FileName).text(gData.FileName)
                            html.attr('ismedia', 1);
                            html.attr('isnew', 0);
                            html.attr('media', gData.Media);
                            med.attr('src', '/Content/VendingMachine/' + sessionStorage.OrgID + '/' + gData.FileName).removeClass('d-none');
                        }
                    }

                    for (var d in denom) {
                        for (var i in denomsku) {
                            if (denom[d] == CStr2(denomsku[i].Description.replace('VDM ', '')).split('_')[0]) {
                                $('.denom_' + denom[d]).attr('denomid', denomsku[i].DefaultValue).val(denomsku[i].ProductNo + ' - ' + denomsku[i].ProductName);
                                break;
                            }
                        }                        
                    }
                }
            })
        }

        GetGeneralSettings();
    })
})