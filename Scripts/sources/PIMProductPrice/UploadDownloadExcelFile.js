require([
    'common',
    GetPathFormat('/Scripts3/Browser/LibraryFowst.js?crc=')
    ], function () {


        $(function () {
            $(document).ready(function () {
                //getPIMExcelColumns();
                $("#OrgID").val(sessionStorage.OrgID);

                AjaxSend({
                    url: '/Browser/LibraryPIMPriceLevel',
                    success: function (e) {
                        var selPriceLevel = $('#selPriceLevel').empty();
                        var vRows = e.Results.Rows;

                        selPriceLevel.append(new Option('Please select', ''));

                        for (var i = 0 in vRows) {
                            selPriceLevel.append(new Option(vRows[i].PriceLevel, vRows[i].ID));
                        }
                    }
                })
            });

            window.UpDownExcelFile = {
                SetReqData: function () {
                    $("#OrgID").val(sessionStorage.OrgID);
                }
            }

            $('#btnBackToHome').click(function (e) {

                e.preventDefault();

                //window.open(GetPathFormat("/People/PeopleMenu"), "_self");
                window.location.hash = 'LibraryPage';
            });

            $('body').on('click', '#btnDownloadByFilter', function (e) {
                e.preventDefault();
                //get selected
                var filter = $("input[name='filter']").serializeArray();
                var arrayList = [];

                if ($('#selPriceLevel :selected').val() == '') {
                    NewMessageBox('Please select Price Level');
                    return;
                }

                if ($('#selFilter').val() != '') {
                    if (filter.length > 0) {
                        for (var i = 0; i < filter.length; i++) {
                            arrayList.push(filter[i].value);
                        }
                    }

                    if (arrayList.length > 0) {
                        var pData = {
                            FilterName: $.trim($('#selFilter :selected').val()),
                            PriceLevelName: $.trim($('#selPriceLevel :selected').val()),
                            Keywords: arrayList
                        };

                        AjaxSend({
                            url: 'PIMProductPrice/GetProductList',
                            data: pData,
                            success: function (vData) {
                                if (vData.PIMProductList == "") {
                                    NewMessageBox("No record(s) found.",
                                        "PIM Product List",
                                        [
                                            { Text: 'Ok' }
                                        ]
                                    );
                                } else {
                                    window.location = GetPathFormat("/PIMTemplate/DownloadPIMProductListFile?FileName=" + $.trim(vData.PIMProductList));
                                }

                            }
                        });
                    }
                } else {

                NewMessageBox("Are you sure you want to download all of your organization's products?",
                        "PIM Product List",
                        [
                            { Text: 'Ok',
                                Click: function () {
                                    var pData = {
                                        FilterName: $.trim($('#selFilter :selected').val()),
                                        PriceLevelName: $.trim($('#selPriceLevel :selected').val()),
                                        Keywords: ''
                                    };

                                    AjaxSend({
                                        url: 'PIMProductPrice/GetProductList',
                                        data: pData,
                                        success: function (vData) {
                                            if (vData.PIMProductList == "") {

                                                NewMessageBox("No record(s) found.",
                                                    "PIM Product List",
                                                    [
                                                        { Text: 'Ok'
                                                        }
                                                    ]
                                                );
                                            } else {
                                                window.location = GetPathFormat("/PIMTemplate/DownloadPIMProductListFile?FileName=" + $.trim(vData.PIMProductList));

                                                history.back();
                                            }

                                        }
                                    });

                                } //click
                            },
                                { Text: 'Cancel'
                                } //

                        ]
                    );
                }
            })
            .on('change', '#selFilter', function (e) {
                e.preventDefault();
                //if not blank
                if ($('#selFilter').val() != '') {

                    var pData = {
                        FilterName: $('#selFilter :selected').val()
                    };

                    AjaxSend({
                        url: 'PIMProductPrice/GetFilterResult',
                        data: pData,
                        success: function (vData) {
                            if (vData.Filter == "") {
                                NewMessageBox("No record(s) found.",
                                        "PIM Product List",
                                        [
                                            { Text: 'Ok' }
                                        ]
                                    );
                            } else {
                                DisplayResults(vData.Filter.Rows);
                            }

                        }
                    });
                } else {
                    $('#tblResult').empty();
                    $("#divResult").hide();

                }
            })
            .on('submit', '#formupload', function (e) {
                e.preventDefault();

                //grab form data
                var formData = new FormData($(this)[0]);
                Bs.Loading('show');
                setTimeout(function () {
                    $.ajax({
                        url: '/PIMProductPrice/Index',
                        type: 'POST',
                        data: formData,
                        async: false,
                        cache: false,
                        contentType: false,
                        processData: false,
                        success: function (vData) {
                            getPIMExcelColumns(vData);                            
                        }
                    });
                }, 200)                
            })
            .on('change', '#txtBulkPriceobjFile', function (e) {
                e.preventDefault();
                $('#txtBulkPriceOrgID').val(sessionStorage.OrgID);
                $('#formupload').submit();
            })

            function DisplayResults(vRows) {

                $("#divResult").show();
                var pLV = $("#tblResult");

                $('#tblResult').empty();


                for (var i = 0; i <= vRows.length - 1; i++) {

                    var checked = undefined;

                    if ((i % 2) == 0) {
                        pItem = $('<tr>')
                    }

                    checked = false;
                    pItem = pItem
                    .append($('<td>')
                        .append($('<div class="row">')
                            .append($('<label class="col-sm-2 col-form-label">')
                                .attr('for', i + 'chb')
                                .text(vRows[i].Label)
                            )
                            .append($('<div class="col-sm-10">')
                                .append($('<div class="form-check">')
                                    .append($('<input type="checkbox" name="filter" class="form-check-input"/>')
                                        .attr('ID', i + 'chb')
                                        .attr('checked', checked)
                                        .attr('value', vRows[i].Label)

                                    )
                                )
                            )
                        )
                    )//td                    


                    pLV.append(pItem);

                    try {
                        $('#' + i + 'chb').checkboxradio('refresh');
                    } catch (ex) {
                        $('#' + i + 'chb').checkboxradio();
                    }


                } //for

                try {
                    $('#lstResultContainer').listview('refresh');
                }
                catch (e) {
                    $('#lstResultContainer').listview();
                }
            }





            function getPIMExcelColumns(vData) {
                var data = JSON.parse(vData);

                var pData = [];
                var pParams = GetUrlParameters();

                $("#divFailedMessages").hide();

                pData = {
                    OrgID: $.trim(sessionStorage.OrgID)
                };

                AjaxSend({
                    url: 'PIMProductPrice/IsProductPriceExcelUploaded',
                    data: pData,
                    success: function (vData) {

                        IsTempateExist = false;
                        if (data.d.Result == 'Incomplete') {
                            Bs.Loading('hide');
                            NewMessageBox("The system is unable to process your data. ProductNo, Quantity, UnitMeasure, PriceLevel, and Price columns are required. Please download your product list above to see the correct template.",
                                    "Excel Data",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                            );
                        } else if (data.d.Result == 'Ok') {

                            UploadProductPrice();

                        } else if (data.d.Result == 'Success') {
                            Bs.Loading('hide');
                            NewMessageBox("Excel Data successfully uploaded.",
                                    "Excel Data",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                            );
                        } else if (data.d.Result == 'Empty') {
                            Bs.Loading('hide');
                            NewMessageBox("Please attach an excel file.",
                                    "Excel Data",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                            );
                        }
                    }
                });
            }



            function UploadProductPrice() {
                pData = {
                    OrgID: $.trim(sessionStorage.OrgID)
                };

                AjaxSend({
                    url: "PIMProductPrice/SavePriceExcelFileData",
                    data: pData,
                    success: function (vFilekData) {

                        if (vFilekData.PIMExcelUpload == "Success") {
                            Bs.Loading('hide');
                            NewMessageBox("Excel Data successfully uploaded.",
                                                            "Excel Data",
                                                            [
                                                                { Text: 'Ok'
                                                                }
                                                            ]
                                                    );
                        } else {
                            if (vFilekData.PIMExcelUploadRequired != undefined) {
                                Bs.Loading('hide');
                                pDiv.html('');

                                for (var p = 0; p <= vFilekData.PIMExcelUploadRequired.Rows.length - 1; p++) {
                                    pItem = $('<div>')
                                                                .append($('<p>')
                                                                    .attr("style", "white-space:normal; color: #FF0000;")
                                                                    .text(vFilekData.PIMExcelUpload.Rows[p].ExcelColumn)
                                                                )
                                    pDiv.append(pItem);
                                };

                                pDiv.trigger('create');
                                $("#divFailedMessages").show();
                            } else if (vFilekData.PIMExcelUpload == 'Failed') {
                                Bs.Loading('hide');
                                NewMessageBox("Encountered error while uploading data from Excel File.",
                                                                "Excel Data",
                                                                [
                                                                    { Text: 'Ok'
                                                                    }
                                                                ]
                                                        );
                            } else if (vFilekData.PIMExcelUpload != '') {

                                setTimeout(FollowUpRequestFromServer, 5000);
                                Bs.Loading('show', vFilekData.PIMExcelUpload);
                            }
                        }
                    }
                });
            }





            function FollowUpRequestFromServer() {
                AjaxSend({
                    url: 'PIMProductPrice/UploadRequestFollowUp',
                    success: function (vData) {
                        $('#txtBulkPriceobjFile').val('');
                        if (vData.PIMExcelUpload == "Success") {
                            Bs.Loading('hide');
                            NewMessageBox("Excel Data successfully uploaded.",
                                    "Excel Data",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                            );

                        } else if (vData.PIMExcelUpload == 'Failed') {
                            Bs.Loading('hide');
                            NewMessageBox("Encountered error while uploading data from Excel File.",
                                    "Excel Data",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                            );
                        } else if (vData.PIMExcelUpload != '') {
                            setTimeout(FollowUpRequestFromServer, 5000);
                            Bs.Loading('show', vData.PIMExcelUpload);
                        }
                    }
                });
            }
        });
    });