require([
    'common'
    ], function () {

        $(function () {

            var mLoaded = false;

            $.mobile.activePage.bind('pageshow', PageShow);
            function PageShow() {
                mLoaded = true;

                AjaxSend({
                    url: 'DevicePrinter/IsNewEntry',
                    success: function (vData) {
                        if (vData.IsNew == 'TRUE') {

                            $("#divCategories").show();
                            GetAllCategories('');

                        }
                        else {
                            ShowDevicePrinterInfo(vData.DevicePrinterID || '');
                        }
                    }
                });
            };
            if (!mLoaded) PageShow();
            //ShowDevicePrinterInfo(sessionStorage.ViewID);
            window.DevicePrinter = {
                DevicePrinterInfo: function () {
                    AjaxSend({
                        url: 'DevicePrinter/IsNewEntry',
                        success: function (vData) {
                            if (vData.IsNew == 'TRUE') {

                                $("#divCategories").show();
                                GetAllCategories('');

                            }
                            else {
                                ShowDevicePrinterInfo(vData.DevicePrinterID || '');
                            }
                        }
                    });
                }
            }

            $('body').on('click', '#btnDevicePrinterInfoDone', function () {
                if ($.trim($("#selPrinterRole").val()) == "") {
                    NewMessageBox('Please select printer role.', 'Printer Role');
                    return;
                }

                if ($.trim($("#selPrinterType").val()) == "") {
                    NewMessageBox('Please select printer type.', 'Printer Role');
                    return;
                }


                SaveDevicePrinterInfo();
            })
            .on('click', '#btnDevicePrinterInfoCancel', function () {
                //history.back();
                if (sessionStorage.AddViewLibBCName == "View Printer Role") {
                    window.location.hash = '';
                    window.location.hash = 'UniversalPage';
                    LibraryTable.ViewPrinterRolesPage();
                }
                else {
                    if (sessionStorage.DevicePrinterInfoUsedby == "Printer Roles") {
                        window.location.hash = 'LibraryPage';
                    }
                    else if (sessionStorage.DevicePrinterInfoUsedby == "FOWST") {
                        $('#pageOrgFOWSTInfo').hide();
                        $('#pageOrgFOWSTTruckRegInfo').hide();
                        $('#pageOrgFOWSTSectionInfo').hide();
                        $('#pageOrgFOWSTBusinessPermitInfo').hide();
                        $('#pageOrgFOWSTTelInfo').hide();
                        $('#pageOrgFOWSTMobileInfo').hide();
                        $('#pageOrgFOWSTEmailInfo').hide();
                        $('#pageOrgFOWSTOperationHRInfo').hide();
                        $('#pageOrgFOWSTPrinterInfo').show();
                        $('#pageOrgFOWSTPrinterIP').hide();
                        $('#pageAddDevicePrinter').hide();
                    }
                }
            })

            $('#btnBackPrinterRole').click(function (e) {

                history.back();
            });

            $('#btnDevicePrinterFowst').click(function (e) {
                e.preventDefault();

                Browser.LibraryFowst.InputData.OrganizationID = sessionStorage.OrgID;
                Browser.LibraryFowst.Click = function (e) {
                    $("#txtDevicePrinterFowstID").val(e.ID);
                    $("#btnDevicePrinterFowst").text(e.FowstName);//.button('refresh');

                };
                Browser.LibraryFowst.Show();

            });

            function ShowDevicePrinterInfo(strID) {
                var pData = {};
                pData.DevicePrinterID = strID;

                ClearDevicePrinterFields();

                AjaxSend({
                    url: "DevicePrinter/getDevicePrinterInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.DevicePrinterData.Rows.length > 0) {

                            $("#txtDevicePrinterID").val(vData.DevicePrinterData.Rows[0].ID || '');

                            if (vData.DevicePrinterData.Rows[0].PrinterRole == 'Point of Sale (POS)' || vData.DevicePrinterData.Rows[0].PrinterRole == 'Sales Order Taker') {
                                $('#selPrinterRole').append(new Option(vData.DevicePrinterData.Rows[0].PrinterRole, vData.DevicePrinterData.Rows[0].PrinterRole, false, false))
                                $("#selPrinterRole").val(vData.DevicePrinterData.Rows[0].PrinterRole);//.selectmenu('refresh');
                                $('#selPrinterRole').prop('disabled', 'disabled');
                                $("#divCategories").hide();
                                $("#divActive").hide();

                            } else {
                                $("#selPrinterRole").val(vData.DevicePrinterData.Rows[0].PrinterRole || '');//.selectmenu('refresh');
                                $('#selPrinterRole').prop('disabled', false);
                                $("#divCategories").show();
                                $('#chkIsActive').prop("checked", (vData.DevicePrinterData.Rows[0].IsActive == 0 ? 0 : 1));//.checkboxradio("refresh");

                                $("#divActive").show();
                            }

                            $("#selPrinterType").val(vData.DevicePrinterData.Rows[0].PrinterType || '');//.selectmenu('refresh');


                            GetAllCategories(vData.DevicePrinterData.Rows[0].ID);
                        } else {
                            GetAllCategories('');
                        }


                    }
                });

            };

            function ClearDevicePrinterFields() {

                $("#txtDevicePrinterID").val('');
                $("#txtDevicePrinterName").val('');
                $("#selPrinterType").val('');//.selectmenu('refresh');

                $("#txtDevicePrinterFowstID").val('');
                $("#btnDevicePrinterFowst").text('Please Select');//.button('refresh');

                $("#txtDevicePrinterIP").val('');
                $('#chkIsActive').prop("checked", 1);//.checkboxradio("refresh");

            }

            function SaveDevicePrinterInfo() {
                var Categories = '';
                var i = 0;
                $('#tblCategories tr td input').each(function (m_index) {
                    if ($(this).attr('checked') == 'checked') {
                        Categories += $(this).data('data') + '|';
                    }
                });

                var pData = [];

                pData = {
                    DevicePrinterID: $("#txtDevicePrinterID").val(),
                    PrinterRole: $.trim($("#selPrinterRole").val()),
                    PrinterType: $.trim($("#selPrinterType").val()),
                    IsActive: ($("#chkIsActive").is(":checked") ? 1 : 0),
                    Categories: Categories
                };

                AjaxSend({
                    url: "DevicePrinter/CheckUniquePrinterRole",
                    data: { PrinterID: $("#txtDevicePrinterID").val(), PrinterRole: $.trim($("#selPrinterRole").val()) },
                    success: function (vData) {

                        if (vData.DevicePrinterData.Rows.length > 0) {
                            NewMessageBox('There is already an existing record with the same printer role.', 'Printer Role');
                        } else {
                            AjaxSend({
                                url: "DevicePrinter/SaveDevicePrinterInfo",
                                data: pData,
                                success: function (vSaveData) {

                                    $("#txtDevicePrinterID").val(vSaveData.DevicePrinterData || '');

                                    //                                    window.open(GetPathFormat("/DevicePrinter/DevicePrinterSummary"), "_self");
                                    // 
                                    //To add printers to a FOWST, go to System Administration -> Data Libraries -> FOWST. After selecting a FOWST, you will find the 'Add Printer' section at the bottom of the page.

                                    if (GetUrlParameters().mode == 'wizard') {
                                        window.open(GetPathFormat("/DevicePrinter/DevicePrinterSummary?mode=wizard"), "_self");

                                    } else {
                                        NewMessageBox('Successfully saved printer role. Do you want to add printers to a FOWST?', 'Printer Role', [
                                            {
                                                Text: 'Yes', Click: function () {
                                                    ShowOrganizationPlaces();
                                                }
                                            }, {
                                                Text: 'No', Click: function () {
                                                    if (sessionStorage.AddViewLibBCName == "View Printer Role") {
                                                        sessionStorage["AddViewLibBCName"] = "View Printer Role";
                                                        window.location.hash = '';
                                                        window.location.hash = 'UniversalPage';
                                                        LibraryTable.ViewPrinterRolesPage();
                                                    }
                                                    else {
                                                        if (sessionStorage.DevicePrinterInfoUsedby == "Printer Roles") {
                                                            $('#tblBody').html('');
                                                            LibraryTable.PrinterRoles();
                                                            window.location.hash = '';
                                                            window.location.hash = 'LibraryPage';
                                                        }
                                                        else if (sessionStorage.DevicePrinterInfoUsedby == "FOWST") {
                                                            $('#pageOrgFOWSTInfo').hide();
                                                            $('#pageOrgFOWSTTruckRegInfo').hide();
                                                            $('#pageOrgFOWSTSectionInfo').hide();
                                                            $('#pageOrgFOWSTBusinessPermitInfo').hide();
                                                            $('#pageOrgFOWSTTelInfo').hide();
                                                            $('#pageOrgFOWSTMobileInfo').hide();
                                                            $('#pageOrgFOWSTEmailInfo').hide();
                                                            $('#pageOrgFOWSTOperationHRInfo').hide();
                                                            $('#pageOrgFOWSTPrinterInfo').show();
                                                            $('#pageOrgFOWSTPrinterIP').hide();
                                                            $('#pageAddDevicePrinter').hide();
                                                        }
                                                    }
                                                }
                                            }
                                        ]);
                                    }
                                }
                            });
                        }
                    }
                });




            };

            function ShowOrganizationPlaces() {
                var pData = {};
                pData.OrgID = sessionStorage["OrgID"];

                window.pageSourcePage = $.mobile.activePage;

                AjaxSend({
                    url: 'Organization/getSubscriberOrganization',
                    data: pData,
                    success: function (vDataOrg) {

                        if (vDataOrg.OrganizationID != "") {
                            pData.IsNew = 0;
                            pData.ID = vDataOrg.OrganizationID;

                            AjaxSend({
                                url: 'Organization/setCreateNew',
                                data: pData,
                                success: function (vData) {
                                    //ChangePage('/Organization/OrganizationAddress');
                                    //window.location = MergeUrlParameters({
                                    //    OrgID: sessionStorage.OrgID,
                                    //    IsMobile: CBool(IsMobile) ? 1 : 0
                                    //}, '/Organization/OrganizationAddress');
                                    window.location.hash = 'LibraryPage';
                                    sessionStorage["LibraryName"] = "FOWST";
                                    $('#tblHeader').empty();
                                    var HeaderData = ['#', 'Name', 'Address'];
                                    for (var i = 0; i < HeaderData.length; i++) {
                                        $('#tblHeader').append('<th>' + HeaderData[i] + '</th>');
                                    }
                                    LibraryTable.Fowst();
                                }
                            });
                        }
                    },
                    error: function (vError) {
                        NewMessageBox('This feature requires internet connection.', 'Requires Internet Connection');
                    }

                });

            }

            function GetAllCategories(printerID) {
                data = { OrgID: sessionStorage.OrgID };

                AjaxSend({
                    url: 'DevicePrinter/GetAllCategories',
                    data: data,
                    success: function (vData) {
                        if (vData.Results.Rows.length > 0) {

                            if (printerID != '') {
                                AjaxSend({
                                    url: 'DevicePrinter/GetDevicePrinterCategories',
                                    data: { PrinterID: printerID },
                                    success: function (vDataCat) {

                                        if (vDataCat.Results.Rows.length > 0) {

                                            DisplayAllCategories(vData.Results.Rows, vDataCat.Results.Rows, printerID);
                                        } else {
                                            DisplayAllCategories(vData.Results.Rows, '', printerID);
                                        }
                                    }
                                });
                            } else {
                                DisplayAllCategories(vData.Results.Rows, '', '');
                            }

                        }
                    }
                });
            }

            var arrCategory = [];

            function DisplayAllCategories(vRows, vRowsCat, printerID) {
                var pLV = $("#tblCategories");

                $('#tblCategories').empty();

                var m_Ctr = 0;
                var categories = [];

                if (vRowsCat != '') {
                    for (var j = 0; j <= vRowsCat.length - 1; j++) {
                        categories[j] = vRowsCat[j].CategoryName;
                    }

                }


                for (var i = 0; i <= vRows.length - 1; i++) {

                    var checked = undefined;

                    if ((i % 2) == 0) {
                        pItem = $('<tr>')
                    }

                    if (categories.indexOf(vRows[i].Category) >= 0) {
                        checked = true;
                        pItem = pItem
                        .append($('<td>')
                            .append($('<div class="form-check mb-3" style="font-size:20px;">')
                                .append($('<input type="checkbox" class="form-check-input" data-role="none">')
                                    .attr('ID', i + 'chb')
                                    .prop('checked', CInt2(checked))
                                    .data('data', vRows[i].Category)
                                    .click(function () {
                                        if ($(this).is(':checked')) {
                                            AjaxSend({
                                                url: 'DevicePrinter/IsCategoryExist',
                                                data: { PrinterID: printerID, CategoryName: $(this).data('data') },
                                                success: function (vDataCat) {

                                                    if (vDataCat.Results.Rows.length > 0) {
                                                        NewMessageBox('This category already belongs to a different printer role. Saving this will overwrite the previous setting.', 'Printer Role');

                                                    }
                                                }
                                            });
                                        }
                                    })
                                ).append($('<label class="form-check-label">')
                                    .attr('for', i + 'chb')
                                    .text(vRows[i].Category)
                                )
                            )
                        )//td                    


                        pLV.append(pItem);

                        try {
                            $('#' + i + 'chb');//.checkboxradio('refresh');
                        } catch (ex) {
                            $('#' + i + 'chb');//.checkboxradio();
                        }
                    } else {
                        pItem = pItem
                        .append($('<td>')
                            .append($('<div class="form-check mb-3" style="font-size:20px;">')
                                .append($('<input type="checkbox" class="form-check-input" data-role="none">')
                                    .attr('ID', i + 'chb')
                                    .prop('checked', CInt2(checked))
                                    .data('data', vRows[i].Category)
                                    .click(function () {
                                        if ($(this).is(':checked')) {
                                            AjaxSend({
                                                url: 'DevicePrinter/IsCategoryExist',
                                                data: { PrinterID: printerID, CategoryName: $(this).data('data') },
                                                success: function (vDataCat) {

                                                    if (vDataCat.Results.Rows.length > 0) {
                                                        NewMessageBox('This category already belongs to a different printer role. Saving this will overwrite the previous setting.', 'Printer Role');

                                                    }
                                                }
                                            });
                                        }
                                    })
                                ).append($('<label class="form-check-label">')
                                    .attr('for', i + 'chb')
                                    .text(vRows[i].Category)
                                )
                            )
                        )//td                    


                        pLV.append(pItem);

                        try {
                            $('#' + i + 'chb');//.checkboxradio('refresh');
                        } catch (ex) {
                            $('#' + i + 'chb');//.checkboxradio();
                        }
                    }

                } //for

                try {
                    $('#lstCategoriesContainer');//.listview('refresh');
                }
                catch (e) {
                    $('#lstCategoriesContainer');//.listview();
                }
            }



        });
    });