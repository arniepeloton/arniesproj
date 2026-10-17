require(['common'],
    function () {
        $(document).ready(function () {
            var arrFile, filecount, ctr = 0;
            LoadBanner();

            window.EcommerceBanner = {
                LoadBanner: LoadBanner
            }

            $('body')
            .on('click', '#btnCancelEcommerceBannerSettings', function () {
                window.location.hash = 'SystemAdministration';
            })
            .on('click', '#btnAddBanner', function () {
                $('#fileUploadOrgLogo').click();
            })
            .on('change', '#fileUploadOrgLogo', function () {
                arrFile = $(this)[0].files
                filecount = $(this)[0].files.length;
                ctr = 0;
                CheckFile();
            })

            function CheckFile() {
                var file = arrFile[ctr];
                var reader = new FileReader();
                var imgTempOrgLogo = $('#imgTempOrgLogo');
                var formdata = new FormData();
                ctr += 1;
                formdata.append(file.name, file);

                reader.onload = function () {
                    var dataURL = reader.result;
                    imgTempOrgLogo.attr('src', dataURL);

                    setTimeout(function () {
                        if (file.type != "image/png" && file.type != "image/jpeg") {
                            NewMessageBox(file.name + ' must be a PNG file', 'Message');
                            LoadBanner();
                            $(this).val('');
                            return;
                        }

                        if (imgTempOrgLogo.innerWidth() != 850 || imgTempOrgLogo.innerHeight() != 360) {
                            NewMessageBox('Banner size of <b>' + file.name + '</b> must be 850px X 360px', 'Message');
                            LoadBanner();
                            $(this).val('');
                            return;
                        }

                        //if (file.size > 512000) { // 500kb
                        //    WebToast('File size must not be more than 500kb', 'top');
                        //    $(this).val('');
                        //    return;
                        //}

                        Bs.Loading('show', 'Please wait while uploading your banner ' + file.name, 'Message');

                        AjaxSend({
                            url: 'Ecommerce/SaveBanner?OrgID=' + sessionStorage.OrgID,
                            type: 'POST',
                            data: formdata,
                            async: true,
                            cache: false,
                            contentType: false,
                            processData: false,
                            success: function (vData) {
                                if (vData.Results == "saved") {
                                    if (ctr == filecount) {
                                        WebToast('Banner has been uploaded', 'top');
                                        $('#fileUploadOrgLogo').val('');
                                        Bs.Loading('hide');
                                        LoadBanner();
                                    } else {
                                        CheckFile();
                                    }

                                } else {
                                    Bs.Loading('hide');
                                    NewMessageBox(vData.Results, "Message");
                                    LoadBanner();
                                }
                            }
                        });
                    }, 200)
                };
                reader.readAsDataURL(file);
            }

            function LoadBanner() {
                AjaxSend({
                    url: 'Ecommerce/GetBanner',
                    success: function (e) {
                        var vRows = e.Results.Rows;
                        var divListBanners = $('#divListBanners');

                        divListBanners.empty();

                        if (vRows.length) {
                            var cnt = 0, divRow = $('<div class="row">');
                            for (var i = 0; i < vRows.length; i++){
                                var divCol = $('<div class="col-sm-4">');

                                divCol.append($('<img style="width:100%;">').attr('src', '/Content/Ecommerce/banners/' + (sessionStorage.OrgID).toLowerCase() + '/' + vRows[i].FileName))
                                    .append($('<div style="width:100%; font-size:inherit; background-color: #eaeaea; padding: 10px 0px;">')
                                        .append($('<div class="row">')
                                            .append($('<div class="col-10">').text(vRows[i].FileName))
                                            .append($('<div class="col-2" style="text-align:center;">')
                                                .append($('<button data-role="none" class="btn btn-danger btn-sm">')
                                                    .append($('<span class="bi-trash">'))
                                                    .data('data', vRows[i])
                                                    .click(function () {
                                                        var data = $(this).data('data');

                                                        NewMessageBox('Are you sure you want remove this banner <b>' + data.FileName + '</b>?', 'Message', [
                                                            {
                                                                Text: 'Yes',
                                                                Click: function () {
                                                                    AjaxSend({
                                                                        url: 'Ecommerce/RemoveBanner',
                                                                        data: { ID: data.ID },
                                                                        success: function (e) {
                                                                            if (e.Results == 'deleted') {
                                                                                WebToast('Banner has been deleted', 'top');
                                                                                LoadBanner();
                                                                            } else {
                                                                                NewMessageBox(e.Results);
                                                                            }
                                                                        }
                                                                    })
                                                                }
                                                            }, { Text: 'No' }
                                                        ])
                                                    })
                                                )
                                            )
                                        )
                                    )

                                if (cnt == 3) {
                                    divListBanners.append('<br><br>');
                                    divRow = $('<div class="row">');
                                    cnt = 1;
                                }

                                divRow.append(divCol);
                                divListBanners.append(divRow);
                                cnt += 1;
                            }
                        } else {
                            divListBanners.append('No results found')
                        }
                    }
                })
            }
        })
    })