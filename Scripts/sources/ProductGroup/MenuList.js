require([
    'common',
    GetPathFormat('/Scripts3/Browser/LibraryFowst.js')
    ], function () {

        var IsNew = 1;
        var IsConfirm = 0;
        var OrgName = '';
        var ID = '';

        $(function () {

            var mLoaded = false;
            var IsEnableVDMFeatures;

            $.mobile.activePage.bind('pageshow', PageShow);

            function PageShow() {
                mLoaded = true;
                //getPimMenu();

                proj.CheckIfVDMFeaturesIsActivated(function (e) {
                    IsEnableVDMFeatures = CBool(e.EnableVDMFeatures);
                    if (IsEnableVDMFeatures) {
                        $('#selTemplateVDMModel').append(new Option('Please select', ''));
                        for (var i in e.VDMMachines.Rows) {
                            $('#selTemplateVDMModel').append(new Option(e.VDMMachines.Rows[i].Name, e.VDMMachines.Rows[i].ID));
                        }
                        $('#divTemplateVDMModel').show();
                    } else {
                        $('#divTemplateVDMModel').remove();
                    }
                })
            };
            if (!mLoaded) PageShow();

            $('body').on('click', '#btnCreateTemplate', function (e) {
                e.preventDefault();

                if ($.trim($("#txtTemplateName").val()) == "") {
                    NewMessageBox("Please enter a template name.");
                    return;
                }

                if (IsEnableVDMFeatures) {
                    if ($('#selTemplateVDMModel').val() == '') {
                        NewMessageBox("Please select VDM model for this template");
                        return;
                    }
                }

                SaveTemplateName();
            })
            .on('click', '#btnCancelCreateTemplate', function (e) {
                window.location.hash = 'LibraryPage';
            })
            .on('change', '#selTemplateVDMActivate', function () {
                if ($(this).is(':checked')) {
                    $('#divTemplateVDMModelOption').show();
                } else {
                    $('#selTemplateVDMModel').val('');
                    $('#divTemplateVDMModelOption').hide();
                }
            })

            $('#frmHotButtonSearch').submit(function (e) {
                e.preventDefault();

                getPimMenu();

                return false;
            });


            $('#btnHotButtonsDone').click(function (e) {
                e.preventDefault();

                window.location = '/People/PeopleMenu';

            });

            $('#btnAddHotButtonTemplate').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pageCreateName');

            });

            $('#btnOrgEmployee').click(function (e) {

                Browser.LibraryPeople.Click = function (e) {

                    $('#txtOrgPeopleID').val(e.ID)
                    $('#btnOrgEmployee').text(e.PeopleName)
                    .button('refresh');
                    $('#btnOrgEmpInfoSaveAndInvite .ui-btn-text').text('Send ' + e.PeopleName + ' an Employment Invite')

                    GetAllFowst()
                };

                Browser.LibraryPeople.Show();

            });

            $('#btnOrgEmpInfoSave').click(function (e) {
                e.preventDefault();

                if ($.trim($("#txtOrgPeopleID").val()) == "") {
                    NewMessageBox("Please select employee.",
                            "Employee",
                            [
                                { Text: 'Ok'
                                }
                            ]
                );
                    return;
                }

                if ($.trim($("#selOrgDateHiredMonth").val()) == "" || $.trim($("#selOrgDateHiredDay").val()) == "" || $.trim($("#selOrgDateHiredYear").val()) == "") {
                    NewMessageBox("Please enter valid date hired.",
                            "Date Hired",
                            [
                                { Text: 'Ok' }
                            ]
                );
                    return;
                }

                if ($.trim($("#selOrgDeactivate").val()) == "0") {

                    ClearOrgEmployeeInActiveFields();

                }
                else {

                    if ($.trim($("#selOrgDateInActiveMonth").val()) == "" || $.trim($("#selOrgDateInActiveDay").val()) == "" || $.trim($("#selOrgDateInActiveYear").val()) == "") {
                        NewMessageBox("Please enter valid deactivation date.",
                                "Deactivation Date",
                                [
                                    { Text: 'Ok' }
                                ]
                    );
                        return;
                    }

                }

                SaveOrganizationEmployee();

            });

            $('#btnOrgEmpInfoSaveAndInvite').click(function (e) {
                e.preventDefault();

                if ($.trim($("#txtOrgPeopleID").val()) == "") {
                    NewMessageBox('An employment invite will be sent to ' + $('#btnOrgEmployee').text() + ' through the Mobiz Profile app and to any available email address. ' +
                                $('#btnOrgEmployee').text() + ' will need to confirm employment to ' + OrgName + ' before being added to your employee list.',
                            "Employee",
                            [
                                { Text: 'Ok',
                                    Click: function () {
                                        SaveOrganizationEmployee();
                                    }
                                }
                            ]
                );
                    return;
                }

                if ($.trim($("#selOrgDateHiredMonth").val()) == "" || $.trim($("#selOrgDateHiredDay").val()) == "" || $.trim($("#selOrgDateHiredYear").val()) == "") {
                    NewMessageBox("Please enter valid date hired.",
                            "Date Hired",
                            [
                                { Text: 'Ok' }
                            ]
                );
                    return;
                }

                if ($.trim($("#selOrgDeactivate").val()) == "0") {

                    ClearOrgEmployeeInActiveFields();

                }
                else {

                    if ($.trim($("#selOrgDateInActiveMonth").val()) == "" || $.trim($("#selOrgDateInActiveDay").val()) == "" || $.trim($("#selOrgDateInActiveYear").val()) == "") {
                        NewMessageBox("Please enter valid deactivation date.",
                                "Deactivation Date",
                                [
                                    { Text: 'Ok' }
                                ]
                    );
                        return;
                    }

                }

                SaveOrganizationEmployee();

            });

            $('#btnOrgEmpInfoSaveAndConfirm').click(function (e) {
                e.preventDefault();

                if ($.trim($("#txtOrgPeopleID").val()) == "") {
                    NewMessageBox("Please select employee.",
                            "Employee",
                            [
                                { Text: 'Ok'
                                }
                            ]
                );
                    return;
                }

                if ($.trim($("#selOrgDateHiredMonth").val()) == "" || $.trim($("#selOrgDateHiredDay").val()) == "" || $.trim($("#selOrgDateHiredYear").val()) == "") {
                    NewMessageBox("Please enter valid date hired.",
                            "Date Hired",
                            [
                                { Text: 'Ok' }
                            ]
                );
                    return;
                }


                NewMessageBox('Let ' + $('#btnOrgEmployee').text() + ' confirm his employment to ' + OrgName + ' now by signing in.',
                        "Sign In",
                        [
                            { Text: 'Ok',
                                Click: function () {
                                    $.mobile.changePage('#pageOrgEmployeeSignIn');
                                }
                            }
                        ]
                );


            });


            $('#btnOrgEmpInfoCancel').click(function (e) {
                e.preventDefault();

                ChangePage('/Organization/OrganizationEmployee');

            });



            $('#btnOrgEmployeeDone').click(function (e) {
                e.preventDefault();

                if (window.pageSourcePage != undefined) {
                    $.mobile.changePage("#" + $(window.pageSourcePage).attr('id'));
                }
                else {
                    ChangePage('/Organization/OrganizationSummary');
                }

            });

            $('#selOrgDeactivate').change(function (e) {
                e.preventDefault();

                ShowHideOrgInActiveDetails();

            });

            $('#btnOrgAddEmployeeFowst').click(function () {
                GetLibraryFowst();

            });

            function SaveTemplateName() {
                var data = {
                    Name: $.trim($("#txtTemplateName").val()),
                    VDMID: $('#selTemplateVDMModel').val()
                };
                AjaxSend({
                    url: 'ProductGroup/CreateTemplate',
                    data: data,
                    success: function (vData) {
                        if (vData.Result == 'ok') {

                            //location = MergeUrlParameters({
                            //    OrgID: sessionStorage.OrgID,
                            //    IsMobile: CBool(IsMobile) ? 1 : 0
                            //}, '/ProductGroup');
                            $('#tblBody').html('');
                            window.location.hash = '';
                            window.location.hash = 'LibraryPage';
                            LibraryTable.HotButtons();

                        }
                        else {
                            NewMessageBox(vData.Error);
                        }
                    }
                });
            }

            function GetLibraryFowst() {
                Browser.LibraryFowst.Click = function (e) {
                    var pLV = $("#lstOrgEmployeeFowst");

                    pItem = $('<li>')
                    .attr('ID', e.ID)
                    .attr('href', "#")
                    .append($('<a>')
                        .attr('ID', e.ID)
                        .data('data', e)
                        .click(function (e) {
                        })
                        .append($('<h3>')
                            .text(e.FowstName)
                        )
                        .append($('<p>')
                            .text(e.Address)
                        )
                    )
                    .append($('<a>')
                        .attr('ID', e.ID)
                        .attr('href', "#")
                        .click(function () {
                            $('#' + $(this).attr('ID')).remove();
                        })
                    )
                    pLV.append(pItem);

                    pLV.listview('refresh');
                };
                Browser.LibraryFowst.Show();
            }

            function GetAllFowst() {
                AjaxSend({
                    url: 'Browser/LibraryFowst',
                    success: function (vData) {
                        DisplayFOWST(vData.Results.Rows);
                    }
                });
            }

            function DisplayFOWST(vRows) {
                var pLV = $("#lstOrgEmployeeFowst");

                pLV.empty();

                for (var i = 0; i <= vRows.length - 1; i++) {
                    //unit measure
                    pItem = $('<li>')
                    .attr('ID', vRows[i].ID)
                    .append($('<a>')
                        .attr('ID', vRows[i].ID)
                        .data('data', vRows[i])
                        .click(function (e) {
                            window.location = MergeUrlParameters({
                                OrgID: sessionStorage.OrgID,
                                IsMobile: CBool(IsMobile) ? 1 : 0,
                                EmployeeName: $('#btnOrgEmployee').text(),
                                EmployeeID: ID,
                                FOWSTID: $(this).attr('ID'),
                                FOWSTName: $(this).text()
                            }, 'OrganizationEmployeeAssignWorkSchedule');
                        })
                        .append($('<h3>')
                            .text(vRows[i].FowstName)
                        )
                        .append($('<p>')
                            .text(vRows[i].Address)
                        )
                    )
                    .append($('<a>')
                        .attr('ID', vRows[i].ID)
                        .click(function () {
                            $('#' + $(this).attr('ID')).remove();
                        })
                    )
                    pLV.append(pItem)
                }

                pLV.listview('refresh');
            }

            function ShowHideOrgInActiveDetails() {

                if ($.trim($("#selOrgDeactivate").val()) == "1") {
                    $("#divOrgDeactivateDetails").show();
                }
                else {
                    $("#divOrgDeactivateDetails").hide();
                }
            }

            function getPimMenu() {
                var pData = {};
                pData.SearchKeyword = $.trim($("#txtHotButtonSearch").val());

                AjaxSend({
                    url: 'ProductGroup/GetPimMenu',
                    data: pData,
                    success: function (vData) {

                        LoadPimMenu(vData.PimMenuData.Rows);
                    }
                });
            }

            function LoadPimMenu(vRows) {
                var pLV = $('#lstHotButtonTemplate');
                var pItem;

                $("#divHotButtonTemplate").text(" Number of record(s) found: " + vRows.length);

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li>')
                    .append($('<a>')
                        .attr('ID', vRows[p].ID)
                        .click(function (e) {
                            e.preventDefault();
                            location = MergeUrlParameters({
                                OrgID: sessionStorage.OrgID,
                                MenuID: $(this).attr('ID'), //add menu id to query string
                                IsMobile: CBool(IsMobile) ? 1 : 0
                            }, '/ProductGroup/MenuTemplate');


                        })
                        .append($('<h3>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].Name)
                        )
                        .append($('<p>')
                            .attr("style", "white-space:normal")
                            .text("Created By: " + vRows[p].Surname + ', ' + vRows[p].FirstName + ' ' + vRows[p].MiddleName)
                        )
                        .append($('<p>')
                            .attr("style", "white-space:normal")
                            .text("Last Updated: " + $.format.date(new Date(vRows[p].LastUpdated), 'MMM dd, yyyy'))
                        )
                    );

                    pLV.append(pItem);
                };

                pLV.listview('refresh');
            };


            function ShowOrgEmployeeInfo(strID) {
                var pData = {};
                pData.ID = strID;

                ClearOrgEmployeeFields();
                ClearOrgEmployeeInActiveFields();

                AjaxSend({
                    url: "Organization/getOrganizationEmpInfo",
                    data: pData,
                    success: function (vData) {
                        if (vData.OrgEmployeeData.Rows.length > 0) {
                            $("#txtOrgOrgEmpID").val(vData.OrgEmployeeData.Rows[0].ID || '');

                            $("#txtOrgPeopleID").val(vData.OrgEmployeeData.Rows[0].PeopleID || '');
                            $("#btnOrgEmployee").text(vData.OrgEmployeeData.Rows[0].PeopleName || 'Please Select').button('refresh');

                            $("#selOrgDateHiredMonth").val(vData.OrgEmployeeData.Rows[0].DateHiredMonth || '').selectmenu('refresh');
                            $("#selOrgDateHiredYear").val(vData.OrgEmployeeData.Rows[0].DateHiredYear || '').selectmenu('refresh');
                            setDayCollection('selOrgDateHiredMonth', 'selOrgDateHiredDay', 'selOrgDateHiredYear', true);
                            $("#selOrgDateHiredDay").val(vData.OrgEmployeeData.Rows[0].DateHiredDay || '').selectmenu('refresh');

                            $("#selOrgDeactivate").val(vData.OrgEmployeeData.Rows[0].DeActivated || '0').slider("refresh");

                            if ($("#selOrgDeactivate").val() != "0") {
                                $("#selOrgDateInActiveMonth").val(vData.OrgEmployeeData.Rows[0].DeactivationDateMonth || '').selectmenu('refresh');
                                $("#selOrgDateInActiveYear").val(vData.OrgEmployeeData.Rows[0].DeactivationDateYear || '').selectmenu('refresh');
                                setDayCollection('selOrgDateInActiveMonth', 'selOrgDateInActiveDay', 'selOrgDateInActiveYear', true);
                                $("#selOrgDateInActiveDay").val(vData.OrgEmployeeData.Rows[0].DeactivationDateDay || '').selectmenu('refresh');

                                $("#txtOrgInActiveReason").val(vData.OrgEmployeeData.Rows[0].DeactivationReason || '').keyup();
                                $("#txtOrgComments").val(vData.OrgEmployeeData.Rows[0].Comments || '').keyup();

                                $("#selOrgBan").val(vData.OrgEmployeeData.Rows[0].IsBan || '0').slider("refresh");
                            }

                            GetEmployeeFowst(vData.OrgEmployeeData.Rows[0].PeopleID);

                            ShowHideOrgInActiveDetails();

                            $('#btnOrgEmpInfoSave').show()
                            $('#btnOrgEmpInfoCancel').show()
                            $('#btnOrgEmpInfoSaveAndInvite').hide()
                            $('#btnOrgEmpInfoSaveAndConfirm').hide()

                        }
                    }
                });

            };

            function GetEmployeeFowst(EmployeeID) {
                data = { EmployeeID: EmployeeID }

                AjaxSend({
                    url: 'Organization/GetEmployeeFowst',
                    data: data,
                    success: function (vData) {
                        DisplayFOWST(vData.Results.Rows);
                    }
                });
            }

            function SaveOrganizationEmployee() {
                var pData = [];
                var ctr = 0;
                var Fowst = [];

                $('#lstOrgEmployeeFowst li a').each(function (m_index) {
                    if ($.trim($(this).text()) != '') {
                        Fowst[ctr] = { FowstID: $(this).attr('ID') };
                        ctr++;
                    }
                });

                pData = {
                    ID: ID,
                    OrgEmployeeID: $.trim($("#txtOrgEmpID").val()),
                    PeopleID: $.trim($("#txtOrgPeopleID").val()),
                    DateHired: $("#selOrgDateHiredMonth").val() + '/' + $("#selOrgDateHiredDay").val() + '/' + $("#selOrgDateHiredYear").val(),
                    IsDeactivated: $("#selOrgDeactivate").val(),
                    DeactivationDate: $("#selOrgDateInActiveMonth").val() + '/' + $("#selOrgDateInActiveDay").val() + '/' + $("#selOrgDateInActiveYear").val(),
                    DeactivationReason: $("#txtOrgInActiveReason").val(),
                    Comments: $("#txtOrgComments").val(),
                    IsBan: $("#selOrgBan").val(),
                    IsNew: IsNew,
                    Fowst: Fowst,
                    IsConfirm: IsConfirm
                };

                AjaxSend({
                    url: "Organization/CheckOrganizationEmployee",
                    data: pData,
                    success: function (vData) {

                        if (vData.OrgEmployeeData.Rows.length > 0 && IsNew == 1) {
                            NewMessageBox("Employee being added already exist.",
                            "Employee Exist",
                            [
                                { Text: 'Ok' }
                            ]
                        );
                        }
                        else {
                            AjaxSend({
                                url: "Organization/SaveOrganizationEmployee",
                                data: pData,
                                success: function (vSaveData) {

                                    if (vSaveData.OrgEmployeeData == "DateHiredError") {
                                        NewMessageBox("Please enter valid date hired information.",
                                        "Date Hired",
                                        [
                                            { Text: 'Ok' }
                                        ]
                                    );
                                    }
                                    else if (vSaveData.OrgEmployeeData == "DeactivationDateError") {
                                        NewMessageBox("Please enter valid deactivation date.",
                                        "Deactivation Date",
                                        [
                                            { Text: 'Ok' }
                                        ]
                                    );
                                    }
                                    else {
                                        $.mobile.changePage('#pageOrgEmployee');
                                        getOrgEmployeeList();
                                    }

                                }
                            });

                        }

                    }
                });

            };

            function ClearOrgEmployeeFields() {

                $("#txtOrgEmpID").val('');

                $("#txtOrgPeopleID").val('');
                $("#btnOrgEmployee").text('Please Select').button('refresh');

                $("#selOrgDateHiredMonth").val('').selectmenu('refresh');
                $("#selOrgDateHiredDay").val('').selectmenu('refresh');
                $("#selOrgDateHiredYear").val('').selectmenu('refresh');

            };

            function ClearOrgEmployeeInActiveFields() {

                $("#selOrgDeactivate").val('0').slider("refresh");

                $("#selOrgDateInActiveMonth").val('').selectmenu('refresh');
                $("#selOrgDateInActiveDay").val('').selectmenu('refresh');
                $("#selOrgDateInActiveYear").val('').selectmenu('refresh');

                $("#txtOrgInActiveReason").val('');
                $("#txtOrgComments").val('');

                $("#selOrgBan").val('0').slider("refresh");
            };

        });
    });