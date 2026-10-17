require([
    'common',
    GetPathFormat('/Scripts3/Browser/LibraryPeople.js'),
    GetPathFormat('/Scripts3/Browser/LibraryOrganization.js')
], function () {
    var ID = '';

    $(function () {

        PageShow();

        window.CRM = {
            ShowCRMData: function () {
                if (sessionStorage.LibraryName == 'Departments') {
                    $('.lblCRMType').text('Department');
                    $('#divForTeam').hide();
                } else {
                    $('.lblCRMType').text('Team');
                    $('#divForTeam').show();
                }
                
                if (CStr2(sessionStorage.ViewID) == '') {
                    return;
                }
                ID = sessionStorage.ViewID || '';
                GetTeamInfo();
                GetTeamMembers();
                GetTeamCustomers();
                GetTeamQuota();
            }
        }
        $('body').on('click', '#btnOrgAddTeamMember', function () {
            BrowsePopup('LibraryPeople', 'Team Member',
                [
                    {
                        DisplayText: [{ Column: 'PeopleName' }],
                        Click: function (e) {
                            AddToTeam(e.ID, e.PeopleName);
                        }
                    }
                ]
            )
        })
        .on('click', '#btnOrgAddCustomer', function () {
            BrowsePopup('LibraryOrganization', 'Team Customer',
                [
                    {
                        DisplayText: [{ Column: 'CompanyName' }],
                        Click: function (e) {
                            AddToCustomer(e.ID, e.CompanyName)
                        }
                    }
                ]
            )
        })
        .on('click', '#btnOrgTeamDone', function () {
            //window.open(GetPathFormat("/People/PeopleMenu"), "_self");
            window.location.hash = 'LibraryPage';
        })
        .on('click', '#btnOrgTeamSave', function () {
            if ($('#txtTeamName').val() == "") {
                WebToast('Please enter team name.', 'top');
                return;
            }
            SaveCrmTeam();
        })
        .on('keypress', '#txtSearchKeyword', function (e) {
            if (e.keyCode == 13) {
                if ($('#TitleLibrary').text() == "TEAM MEMBER") {
                    SearchEmployee($(this).val());
                }
                else {
                    SearchCustomer($(this).val());
                }
            }
        })
        .on('click', '#btnOrgTeamCancel', function () {
            window.location.hash = 'LibraryPage';
        })

        $('#btnOrgAddTeam').click(function () {
            ClearData();
            $.mobile.changePage('#pageAddTeam');
        });
    });

    function PageShow() {
        if (sessionStorage.LibraryName == 'Departments') {
            $('.lblCRMType').text('Department');
            $('#divForTeam').hide();
        } else {
            $('.lblCRMType').text('Team');
            $('#divForTeam').show();
        }

        if (ValidPrevPage(arguments)) {
            GetAllTeams();
        };

        if (sessionStorage.ViewID != undefined) {
            ID = sessionStorage.ViewID;
            GetTeamInfo();
            GetTeamMembers();
            GetTeamCustomers();
            GetTeamQuota();
        }

        function ValidPrevPage(args) {
            try {
                return args[1].prevPage.attr('data-role') != 'dialog';
            } catch (e) {
                return true;
            };
        };
    }

    function ClearData() {

        ID = '';

        $('#txtTeamName').val('');
        $('#txtTeamDescription').val('');
        $('#lstTeamMembers').empty();
        $('#lstOrgCustomer').empty();

        $('#txtOrgTeamQuotaTier1Jan').val('');
        $('#txtOrgTeamIncentiveTier1Jan').val('');
        $('#txtOrgTeamQuotaTier1Feb').val('');
        $('#txtOrgTeamIncentiveTier1Feb').val('');
        $('#txtOrgTeamQuotaTier1Mar').val('');
        $('#txtOrgTeamIncentiveTier1Mar').val('');
        $('#txtOrgTeamQuotaTier1Apr').val('');
        $('#txtOrgTeamIncentiveTier1Apr').val('');
        $('#txtOrgTeamQuotaTier1May').val('');
        $('#txtOrgTeamIncentiveTier1May').val('');
        $('#txtOrgTeamQuotaTier1Jun').val('');
        $('#txtOrgTeamIncentiveTier1Jun').val('');
        $('#txtOrgTeamQuotaTier1Jul').val('');
        $('#txtOrgTeamIncentiveTier1Jul').val('');
        $('#txtOrgTeamQuotaTier1Aug').val('');
        $('#txtOrgTeamIncentiveTier1Aug').val('');
        $('#txtOrgTeamQuotaTier1Sept').val('');
        $('#txtOrgTeamIncentiveTier1Sept').val('');
        $('#txtOrgTeamQuotaTier1Oct').val('');
        $('#txtOrgTeamIncentiveTier1Oct').val('');
        $('#txtOrgTeamQuotaTier1Nov').val('');
        $('#txtOrgTeamIncentiveTier1Nov').val('');
        $('#txtOrgTeamQuotaTier1Dec').val('');
        $('#txtOrgTeamIncentiveTier1Dec').val('');
    }

    function SaveCrmTeam() {
        var mEmployees = [];
        var mCustomers = [];
        var mCounter = 0;

        $('#lstTeamMembers a').each(function () {
            mEmployees[mCounter] = { Employee: $(this).attr('ID') };
            mCounter++;
        });

        mCounter = 0;

        $('#lstOrgCustomer a').each(function () {
            mCustomers[mCounter] = { Customer: $(this).attr('ID') };
            mCounter++;
        });

        var data = {
            ID: ID,
            TeamName: $('#txtTeamName').val(),
            Description: $('#txtTeamDescription').val(),
            Employees: mEmployees,
            Customers: mCustomers,
            JanQuota: $('#txtOrgTeamQuotaTier1Jan').val(),
            JanIncentive: $('#txtOrgTeamIncentiveTier1Jan').val(),
            FebQuota: $('#txtOrgTeamQuotaTier1Feb').val(),
            FebIncentive: $('#txtOrgTeamIncentiveTier1Feb').val(),
            MarQuota: $('#txtOrgTeamQuotaTier1Mar').val(),
            MarIncentive: $('#txtOrgTeamIncentiveTier1Mar').val(),
            AprQuota: $('#txtOrgTeamQuotaTier1Apr').val(),
            AprIncentive: $('#txtOrgTeamIncentiveTier1Apr').val(),
            MayQuota: $('#txtOrgTeamQuotaTier1May').val(),
            MayIncentive: $('#txtOrgTeamIncentiveTier1May').val(),
            JunQuota: $('#txtOrgTeamQuotaTier1Jun').val(),
            JunIncentive: $('#txtOrgTeamIncentiveTier1Jun').val(),
            JulQuota: $('#txtOrgTeamQuotaTier1Jul').val(),
            JulIncentive: $('#txtOrgTeamIncentiveTier1Jul').val(),
            AugQuota: $('#txtOrgTeamQuotaTier1Aug').val(),
            AugIncentive: $('#txtOrgTeamIncentiveTier1Aug').val(),
            SepQuota: $('#txtOrgTeamQuotaTier1Sept').val(),
            SepIncentive: $('#txtOrgTeamIncentiveTier1Sept').val(),
            OctQuota: $('#txtOrgTeamQuotaTier1Oct').val(),
            OctIncentive: $('#txtOrgTeamIncentiveTier1Oct').val(),
            NovQuota: $('#txtOrgTeamQuotaTier1Nov').val(),
            NovIncentive: $('#txtOrgTeamIncentiveTier1Nov').val(),
            DecQuota: $('#txtOrgTeamQuotaTier1Dec').val(),
            DecIncentive: $('#txtOrgTeamIncentiveTier1Dec').val(),
            Type: (sessionStorage.LibraryName == 'Departments' ? 'department' : 'team')
        }

        AjaxSend({
            url: 'crm/SaveCRMTeam',
            data: data,
            success: function (vData) {
                if (IsGuid(vData.Result)) {
                    //history.back();
                    //GetAllTeams();
                    WebToast('Successfully saved', 'top');
                    $('#tblBody').html('');
                    window.location.hash = 'LibraryPage';
                    if (sessionStorage.SystemAdminFor == 'dtr') {
                        DTRLibraryTable.SalesTeam('', (sessionStorage.LibraryName == 'Departments' ? 'department' : 'team'));
                    } else {
                        LibraryTable.SalesTeam();
                    }                    
                }
            }
        });

    }

    function GetAllTeams() {
        AjaxSend({
            url: 'crm/GetAllTeams',
            data: { Type: 'team' },
            success: function (vData) {
                DisplayTeams(vData.Results.Rows);
            }
        });
    }

    function DisplayTeams(vRows) {
        var pLV = $("#lstOrgTeams");

        pLV.empty();

        for (i = 0; i < vRows.length; i++) {
            pItem = $('<li class="list-group-item">')
                .attr('ID', vRows[i].ID)
                .attr('href', "#")
                .append($('<div>')
                    .attr('ID', vRows[i].ID)
                    .data('data', vRows[i])
                    .click(function (e) {
                        ClearData();
                        ID = $(this).attr('ID');
                        $.mobile.changePage('#pageAddTeam');
                        GetTeamInfo();
                        GetTeamMembers();
                        GetTeamCustomers();
                        GetTeamQuota();
                    })
                    .append($('<p>')
                        .text(vRows[i].TeamName)
                    )
                    .append($('<div>')
                        .text(vRows[i].Description)
                    )
                )

            pLV.append(pItem);
        }

        //try {
        //    pLV.listview('refresh');
        //}
        //catch (e) {
        //    pLV.listview();
        //}
    }

    function GetTeamInfo() {
        AjaxSend({
            url: 'crm/GetTeamInfo',
            data: { ID: ID },
            success: function (vData) {
                $('#txtTeamName').val(vData.Results.Rows[0].TeamName);
                $('#txtTeamDescription').val(vData.Results.Rows[0].Description);
            }
        });
    }

    function GetTeamMembers() {
        AjaxSend({
            url: 'crm/GetTeamMembers',
            data: { ID: ID },
            success: function (vData) {
                DisplayTeamMembers(vData.Results.Rows);
            }
        });
    }

    function DisplayTeamMembers(vRows) {
        var pLV = $("#lstTeamMembers");
        pLV.empty();

        if (vRows.length > 0) {
            for (i = 0; i < vRows.length; i++) {
                pItem = $('<a href="#" class="list-group-item list-group-item-action">')
                            .attr('ID', vRows[i].ID)
                            .append($('<table style="width:100%;">')
                                .append($('<tr>')
                                    .append($('<td style="word-break:break-all;">').text(vRows[i].Name)
                                    )
                                    .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                        .append($('<span class="bi-trash-fill" aria-hidden="true">'))
                                        .attr('ID', vRows[i].ID)
                                        .click(function (e) {
                                            e.preventDefault();

                                            $('a#' + $(this).attr('ID')).remove();
                                            //$("#lstTeamMembers").listview('refresh');
                                        })
                                    ))
                                )
                            )

                pLV.append(pItem);


                //try {
                //    pLV.listview('refresh');
                //}
                //catch (e) {
                //    pLV.listview();
                //}
            }
        }
    }

    function GetTeamCustomers() {
        AjaxSend({
            url: 'crm/GetTeamCustomers',
            data: { ID: ID },
            success: function (vData) {
                DisplayTeamCustomers(vData.Results.Rows);
            }
        });
    }

    function DisplayTeamCustomers(vRows) {
        var pLV = $("#lstOrgCustomer");
        pLV.empty();

        if (vRows.length > 0) {
            for (i = 0; i < vRows.length; i++) {
                pItem = $('<a href="#" data-role="none" class="list-group-item list-group-item-action">')
                            .attr('ID', vRows[i].ID)
                            .append($('<table style="width:100%;">')
                                .append($('<tr>')
                                    .append($('<td style="word-break:break-all;">').text(vRows[i].CompanyName)
                                    )
                                    .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                        .append($('<span class="bi-trash-fill" aria-hidden="true">'))
                                        .attr('ID', vRows[i].ID)
                                        .click(function (e) {
                                            e.preventDefault();

                                            $('a#' + $(this).attr('ID')).remove();
                                            //$("#lstTeamMembers").listview('refresh');
                                        })
                                    ))
                                )
                            )

                pLV.append(pItem);

                //try {
                //    pLV.listview('refresh');
                //}
                //catch (e) {
                //    pLV.listview();
                //}
            }
        }
    }

    function GetTeamQuota() {
        AjaxSend({
            url: 'crm/GetTeamQuota',
            data: { ID: ID },
            success: function (vData) {
                DisplayTeamQuota(vData.Results.Rows);
            }
        });
    }

    function DisplayTeamQuota(vRows) {

        for (i = 0; i < vRows.length; i++) {
            var month = ParseDate(vRows[i].MonthYear).getMonth() + 1 //cause getMonth start @ 0

            switch (month) {
                case 1:
                    $('#txtOrgTeamQuotaTier1Jan').val(vRows[i].Tier1Quota);
                    $('#txtOrgTeamIncentiveTier1Jan').val(vRows[i].Tier1Incentive);
                    break;
                case 2:
                    $('#txtOrgTeamQuotaTier1Feb').val(vRows[i].Tier1Quota);
                    $('#txtOrgTeamIncentiveTier1Feb').val(vRows[i].Tier1Incentive);
                    break;
                case 3:
                    $('#txtOrgTeamQuotaTier1Mar').val(vRows[i].Tier1Quota);
                    $('#txtOrgTeamIncentiveTier1Mar').val(vRows[i].Tier1Incentive);
                    break;
                case 4:
                    $('#txtOrgTeamQuotaTier1Apr').val(vRows[i].Tier1Quota);
                    $('#txtOrgTeamIncentiveTier1Apr').val(vRows[i].Tier1Incentive);
                    break;
                case 5:
                    $('#txtOrgTeamQuotaTier1May').val(vRows[i].Tier1Quota);
                    $('#txtOrgTeamIncentiveTier1May').val(vRows[i].Tier1Incentive);
                    break;
                case 6:
                    $('#txtOrgTeamQuotaTier1Jun').val(vRows[i].Tier1Quota);
                    $('#txtOrgTeamIncentiveTier1Jun').val(vRows[i].Tier1Incentive);
                    break;
                case 7:
                    $('#txtOrgTeamQuotaTier1Jul').val(vRows[i].Tier1Quota);
                    $('#txtOrgTeamIncentiveTier1Jul').val(vRows[i].Tier1Incentive);
                    break;
                case 8:
                    $('#txtOrgTeamQuotaTier1Aug').val(vRows[i].Tier1Quota);
                    $('#txtOrgTeamIncentiveTier1Aug').val(vRows[i].Tier1Incentive);
                    break;
                case 9:
                    $('#txtOrgTeamQuotaTier1Sept').val(vRows[i].Tier1Quota);
                    $('#txtOrgTeamIncentiveTier1Sept').val(vRows[i].Tier1Incentive);
                    break;
                case 10:
                    $('#txtOrgTeamQuotaTier1Oct').val(vRows[i].Tier1Quota);
                    $('#txtOrgTeamIncentiveTier1Oct').val(vRows[i].Tier1Incentive);
                    break;
                case 11:
                    $('#txtOrgTeamQuotaTier1Nov').val(vRows[i].Tier1Quota);
                    $('#txtOrgTeamIncentiveTier1Nov').val(vRows[i].Tier1Incentive);
                    break;
                case 12:
                    $('#txtOrgTeamQuotaTier1Dec').val(vRows[i].Tier1Quota);
                    $('#txtOrgTeamIncentiveTier1Dec').val(vRows[i].Tier1Incentive);
                    break;
            }

        }
    }

    function GetEmployee() {
        Browser.LibraryPeople.Click = function (e) {

            var pLV = $("#lstTeamMembers");

            pItem = $('<li>')
                .attr('data-icon', 'delete')
                .attr('ID', e.ID)
                .attr('href', "#")
                .append($('<a>')
                    .attr('ID', e.ID)
                    .data('data', e)
                    .click(function (e) {
                    })
                    .append($('<h3>')
                        .text(e.PeopleName)
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


            //try {
            //    pLV.listview('refresh');
            //}
            //catch (e) {
            //    pLV.listview();
            //}
        };

        Browser.LibraryPeople.Show();
    }

    function SearchEmployee(s) {
        var pData = {};

        pData.keywords = s;

        AjaxSend({
            url: 'Browser/LibraryPeople',
            data: pData,
            success: function (vData) {
                $('#lstOfLibrary').empty();

                if (vData.Results.Rows.length > 0) {
                    for (var p = 0; p < vData.Results.Rows.length; p++) {
                        $('#lstOfLibrary').append($('<li>')
                                .append($('<a>')
                                    .attr('ID', vData.Results.Rows[p].ID)
                                    .append($('<br><p style="font-weight:bold;">')
                                        .text(vData.Results.Rows[p].PeopleName)
                                    )
                                    .data('data', vData.Results.Rows[p])
                                    .click(function (e) {
                                        AddToTeam($(this).data('data').ID, $(this).data('data').PeopleName)
                                    })
                                ));
                    }

                }
                else {
                    $('#lstOfLibrary').append($('<li>').text('No Result(s) Found'));
                }

                $('#lstOfLibrary').listview('refresh');
            }
        });
    }

    function AddToTeam(id, name) {
        var IsRoleExist = false;

        $('#lstTeamMembers li a').each(function (m_index) {
            if ($.trim($(this).attr('ID')) == id) {
                IsRoleExist = true;
            }
        });

        if (IsRoleExist) {
            alert(name + ' already exist in a list.')
            return;
        }

        //$('#panSearchLibrary').popup('close');
        $('#txtSearchKeyword').val('');
        $('#lstTeamMembers').append($('<a href="#" data-role="none" class="list-group-item list-group-item-action">')
                            .attr('ID', id)
                            .append($('<table style="width:100%;">')
                                .append($('<tr>')
                                    .append($('<td style="word-break:break-all;">').text(name)
                                    )
                                    .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                        .append($('<span class="bi-trash-fill" aria-hidden="true">'))
                                        .attr('ID', id)
                                        .click(function (e) {
                                            e.preventDefault();

                                            $('a#' + $(this).attr('ID')).remove();
                                            //$("#lstTeamMembers").listview('refresh');
                                        })
                                    ))
                                )
                            )
                        )
    }

    function GetCustomers() {
        Browser.LibraryOrganization.Click = function (e) {

            var pLV = $("#lstOrgCustomer");

            AjaxSend({
                url: 'crm/IsCustomerOwnedByOtherTeam',
                data: { CustomerID: e.ID },
                success: function (vData) {
                    if (vData.Results.Rows.length > 0) {
                        NewMessageBox("This customer is already owned by another team. Would you like to continue adding this customer?",
                            "Customer",
                            [
                                { Text: 'Yes',
                                    Click: function () {
                                        AddCustomerToList();
                                    }
                                },
                                {
                                    Text: 'No'
                                }

                            ]
                        );
                    }
                    else {
                        AddCustomerToList();
                    }
                }
            });

            function AddCustomerToList() {
                pItem = $('<li>')
                .attr('ID', e.ID)
                .attr('href', "#")
                .append($('<a>')
                    .attr('ID', e.ID)
                    .data('data', e)
                    .click(function (e) {
                    })
                    .append($('<h3>')
                        .text(e.CompanyName)
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


                try {
                    pLV.listview('refresh');
                }
                catch (e) {
                    pLV.listview();
                }
            }
        };

        Browser.LibraryOrganization.Show();
    }

    function SearchCustomer(s) {
        var pData = {};

        pData.keywords = s;

        AjaxSend({
            url: 'Browser/LibraryOrganization',
            data: pData,
            success: function (vData) {
                $('#lstOfLibrary').empty();

                if (vData.Results.Rows.length > 0) {
                    for (var p = 0; p < vData.Results.Rows.length; p++) {
                        $('#lstOfLibrary').append($('<li>')
                                .append($('<a>')
                                    .attr('ID', vData.Results.Rows[p].ID)
                                    .append($('<br><p style="font-weight:bold;">')
                                        .text(vData.Results.Rows[p].CompanyName)
                                    )
                                    .data('data', vData.Results.Rows[p])
                                    .click(function (e) {
                                        AddToCustomer($(this).data('data').ID, $(this).data('data').CompanyName)
                                    })
                                ));
                    }

                }
                else {
                    $('#lstOfLibrary').append($('<li>').text('No Result(s) Found'));
                }

                $('#lstOfLibrary').listview('refresh');
            }
        });
    }

    function AddToCustomer(id, name) {
        var pLV = $("#lstOrgCustomer");

        AjaxSend({
            url: 'crm/IsCustomerOwnedByOtherTeam',
            data: { CustomerID: id },
            success: function (vData) {
                if (vData.Results.Rows.length > 0) {
                    //                        MessageBox("This customer is already owned by another team. Would you like to continue adding this customer?",
                    //                                "Customer",
                    //                                [
                    //                                    { Text: 'Yes',
                    //                                        Click: function () {                                            
                    //                                            AddCustomerToList(id, name);
                    //                                            $('.ui-dialog').dialog('close');
                    //                                        }
                    //                                    },
                    //                                    {
                    //                                        Text: 'No'
                    //                                    }

                    //                                ]
                    //                            );
                    var IsRoleExist = false;
                    $('#lstOrgCustomer li a').each(function (m_index) {
                        if ($.trim($(this).attr('ID')) == id) {
                            IsRoleExist = true;
                        }
                    });

                    if (IsRoleExist) {
                        alert(name + ' already exist in a list.')
                        return;
                    }

                    NewMessageBox('This customer is already owned by another team. Would you like to continue adding this customer?', 'Message', [
                        {
                            Text: 'Yes', Click: function () {
                                AddCustomerToList(id, name);
                            }
                        }, { Text: 'No' }
                    ])
                }
                else {
                    AddCustomerToList(id, name);
                }
            }
        });
    }

    function AddCustomerToList(id, name) {

        //$('#panSearchLibrary').popup('close');
        $('#txtSearchKeyword').val('');
        $('#lstOrgCustomer').append($('<a href="#" data-role="none" class="list-group-item list-group-item-action">')
                            .attr('ID', id)
                            .append($('<table style="width:100%;">')
                                .append($('<tr>')
                                    .append($('<td style="word-break:break-all;">').text(name)
                                    )
                                    .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                        .append($('<span class="bi-trash-fill" aria-hidden="true">'))
                                        .attr('ID', id)
                                        .click(function (e) {
                                            e.preventDefault();

                                            $('a#' + $(this).attr('ID')).remove();
                                            //$("#lstOrgCustomer").listview('refresh');
                                        })
                                    ))
                                )
                            )
                        )
    }

    //        function showHideAdmin(isShow) {
    //            $("#dvTeamsValue").css({ 'visibility': '' + isShow + '' });
    //        }

    //        function isAdmin() {

    //            AjaxSend({
    //                url: "crm/IsAdmin",
    //                success: function (data) {
    //                    if (data.IsAdmin == "True") {
    //                        showHideAdmin("visible");
    //                    }

    //                    else {
    //                        showHideAdmin("hidden");
    //                    }
    //                }
    //            });
    //        }

    //        function saveCustomerValue(options) {

    //            AjaxSend({
    //                url: "crm/saveValue",
    //                data: options,
    //                success: function (vdata) {
    //                }
    //            });
    //        }

    //        function getTeams() {

    //            AjaxSend({
    //                url: "crm/getTeams/",
    //                success: function (data) {
    //                    var strTeams;

    //                    $.each(data, function (key, val) {
    //                        strTeams += "<option value='" + val["ID"] + "'> " + val["TeamName"] + "</option>";
    //                    });

    //                    strTeams += "</select>";

    //                    $("#optTeamsValue").html(strTeams);
    //                    $('#optTeamsValue').selectmenu('refresh');

    //                    var currentTime = new Date();
    //                    var thisyr = currentTime.getFullYear();
    //                    var thisMonth = currentTime.getMonth() + 1;

    //                    var options = {
    //                        Team: 3, //parseInt($("#cmbTeam option:selected").val()), 
    //                        Month: thisMonth.toString(),
    //                        Year: thisyr.toString()
    //                    };

    //                    getDashBoard(options, 1)
    //                }
    //            });
    //        }

    //        function GetCustomers(type) {

    //            var data = { Type: type,
    //                SearchString: $('#txtBrowser').val()
    //            }

    //            AjaxSend({
    //                url: 'crm/GetCustomers',
    //                data: data,
    //                success: function (vData) {

    //                    ShowCustomerInfo(vData.Customers.Rows);

    //                }
    //            });
    //        }

    //        function ShowCustomersRepresentative(vRows) {
    //            var pLV = $('#lstCustomersRepresentative');
    //            var pItem;

    //            intDevicePlacesCntr = vRows.length - 1;

    //            pLV.html('');
    //            for (var i = 0; i < vRows.length; i++) {

    //                pItem = $('<li>')
    //                        .append($('<a>')
    //                            .attr('href', "#")
    //                            .attr('ID', vRows[i].ID)
    //                            .append($('<h3>')
    //                                .attr("style", "white-space:normal")
    //                                .text(vRows[i].Name)
    //                            )
    //                        )

    //                pLV.append(pItem);
    //            }
    //            pLV.listview('refresh');
    //        }

    //        function ShowCustomerInfo(vRows) {

    //            var pLV = $('#lstCustomers');
    //            var pItem;

    //            intDevicePlacesCntr = vRows.length - 1;

    //            pLV.html('');
    //            for (var i = 0; i < vRows.length; i++) {

    //                pItem = $('<li>')
    //                        .append($('<a>')
    //                            .attr('href', "#PageCustomers")
    //                            .attr('ID', vRows[i].ID)
    //                            .append($('<h3>')
    //                                .attr("style", "white-space:normal")
    //                                .text(vRows[i].CompanyName)
    //                            )
    //                            .append($('<p style=font-size:medium>')
    //                                    .text('Days Since Last Purchase: ' + vRows[i].DaysSinceLastPurchase + 'day(s)')
    //                            )
    //                            .append($('<p style=font-size:medium>')
    //                                    .text('Business Type: ' + vRows[i].BusinessType)
    //                            )
    //                            .append($('<p style=font-size:medium>')
    //                                    .text('Distribution Channel: ' + vRows[i].DistributionChannel)
    //                            )
    //                            .append($('<p style=font-size:medium>')
    //                                    .text('Estimated Value: ' + vRows[i].EstimatedValue)
    //                            )
    //                            .append($('<p style=font-size:medium>')
    //                                    .text('Est. Close Deal   Date: ' + vRows[i].EstimatedClosedDate)
    //                            )
    //                            .append($('<p style=font-size:medium>')
    //                                    .text('Rating: ' + vRows[i].Rating)
    //                            )
    //                            .click(function () {
    //                                GetCustomerInfo($(this).attr("ID"))
    //                            })
    //                        )

    //                pLV.append(pItem);
    //            }

    //            $('lstMain').listview('refresh');
    //            pLV.listview('refresh');

    //            //window.location = GetPathFormat("/SalesMenu?mode=pos");

    //        }

    //        function GetCustomerInfo(CustomerID) {
    //            var data = { CustomerID: CustomerID }

    //            AjaxSend({
    //                url: 'crm/GetCustomers',
    //                data: data,
    //                success: function (vData) {
    //                    $('#lblCompanyName').data(data)
    //                    $('#lblCompanyName').text(vData.Customers.Rows[0].CompanyName);
    //                    $('#lblAddress').text(vData.Customers.Rows[0].Street + ' ' + vData.Customers.Rows[0].District + ', ' + vData.Customers.Rows[0].City + ' ' + vData.Customers.Rows[0].Country);
    //                    $('#lblBusinessType').text(vData.Customers.Rows[0].BusinessType);
    //                    $('#lblDistributionChannel').text(vData.Customers.Rows[0].DistributionChannel);
    //                    $('#lblContactNumber').text(vData.Customers.Rows[0].ContactNumber);
    //                    $('#cmbValue').val(vData.Customers.Rows[0].EstimatedValue);
    //                    $("#cmbValue").selectmenu('refresh');
    //                }
    //            });

    //            AjaxSend({
    //                url: 'crm/GetCustomersContactDetails',
    //                data: data,
    //                success: function (vData) {
    //                    ShowCustomersRepresentative(vData.Customers.Rows)
    //                }
    //            });
    //        }

    //        //Dashboard
    //        function getDashBoard(option, isOwn) {
    //            AjaxSend({
    //                url: "crm/getSalesByMonthly",
    //                data: option,
    //                success: function (data) {
    //                    var myData = data;
    //                    var currentTime;
    //                    var lastYear;

    //                    if (isOwn == 0) {
    //                        currentTime = $("#optYearValue").val();
    //                        lastYear = $("#optYearValue").val() - 1;
    //                    }
    //                    else {
    //                        var today = new Date();
    //                        currentTime = today.getFullYear();
    //                        lastYear = today.getFullYear() - 1;
    //                    }

    //                    var i = 0;
    //                    var totalIncentive;
    //                    var Quota1;
    //                    var Quota2;
    //                    var Incentive1;
    //                    var Incentive2;

    //                    var gData = [[lastYear.toString()], [currentTime.toString()], ["Quota1"], ["Quota2"], ["Quota3"]];

    //                    $.each(myData, function (key, val) {

    //                        if (i < 5) {

    //                            if (val["Data"] == "") {
    //                                gData[i][1] = 0;
    //                            }
    //                            else {
    //                                gData[i][1] = parseFloat(val["Data"]);
    //                            }
    //                        }

    //                        if (i == 1) {
    //                            totalIncentive = val["Data"];
    //                        }
    //                        else if (i == 2) {
    //                            Quota1 = val["Data"];
    //                        }
    //                        else if (i == 3) {
    //                            Quota2 = val["Data"];
    //                        }
    //                        else if (i == 5) {
    //                            Incentive1 = val["Data"];
    //                        }
    //                        else if (i == 6) {
    //                            Incentive2 = val["Data"];
    //                        }

    //                        i += 1;
    //                    });

    //                    plotDashBoard(gData);
    //                    //getCollectionDashBoard(option); 
    //                    //createSalesGroupQuotaTable(Incentive1, Incentive2, lastYear, totalIncentive, Quota1, Quota2, Incentive1, Incentive2);
    //                }
    //            });
    //        }

    //        var plot1;

    //        function plotDashBoard(gData) {
    //            if (plot1) plot1.destroy();

    //            plot1 = $.jqplot('tabDashBoard', [gData], {
    //                title: 'Sales to Quota Graph (Month on Month)',
    //                animate: true,
    //                animateReplot: true,
    //                //stackSeries: true,
    //                axesDefaults: {
    //                    tickRenderer: $.jqplot.CanvasAxisTickRenderer,
    //                    tickOptions: {
    //                        angle: -30,
    //                        fontSize: '10pt'
    //                    }
    //                },
    //                seriesColors: ['#c8c8c8', '#c8c8c8', '#7f7f7f', '#7f7f7f', '#7f7f7f'],
    //                seriesDefaults: {
    //                    renderer: $.jqplot.BarRenderer,
    //                    rendererOptions: { barMargin: 25,
    //                        varyBarColor: true,
    //                        animation: {
    //                            speed: 2000
    //                        }
    //                    },
    //                    pointLabels: { show: true }
    //                },
    //                axes: {
    //                    xaxis: {
    //                        renderer: $.jqplot.CategoryAxisRenderer
    //                    },
    //                    yaxis: {
    //                        tickOptions: {
    //                            formatString: "P%'d"
    //                        },
    //                        rendererOptions: {
    //                            forceTickAt0: true
    //                        }
    //                    }
    //                }
    //            });
    //        }


});
