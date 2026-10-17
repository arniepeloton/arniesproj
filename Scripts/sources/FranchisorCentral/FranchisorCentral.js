require([
    'common',
    GetPathFormat('/Scripts3/Browser/LibraryOrganizationEmployee.js'),
    GetPathFormat('/Scripts3/Browser/LibraryFowst.js'),
    GetPathFormat('/Scripts3/Browser/LibraryFranchisee.js'),
    GetPathFormat('/Scripts3/Browser/LibraryFowstAll.js')

], function () {

    var ID = '';
    var PeopleID = '';
    var FowstID = '';
    var FranchiseFOWSTRows = [];
    var IsApplyToFOWST = false;
    var SelectedFOWSTID = '';

    $(function () {
        //ClearData();
        //GetAllFranchisorGroup();        

        window.Franchisor = {
            ShowFranchisorInfo: function () {
                FranchiseFOWSTRows = [];
                ID = '';
                PeopleID = '';
                IsApplyToFOWST = false;
                SelectedFOWSTID = '';
                GetOrganizationPriceLevels();
                if (sessionStorage.ViewID != undefined) {
                    ClearData();
                    GetFranchisorGroupInfo(sessionStorage.ViewID);
                    GetGroupFranchiseeFOWST(sessionStorage.ViewID);
                    GetGroupSchedule(sessionStorage.ViewID);
                    GetGroupHeads(sessionStorage.ViewID);
                    GetOtherPriceLevels(sessionStorage.ViewID);
                }
            }
        }

        Franchisor.ShowFranchisorInfo();

        $('body').on('click', '#btnSearchFOWST', function () {
            //SearchFOWST($('#txtGroupFOWST').val());
            //            $('#TitleLibrary').text('FOWST');
            //            $('#txtSearchKeyword').attr('placeholder', 'Type name of FOWST');
            //            $('#panSearchLibrary').modal('show');
            //            $('#lstOfLibrary').empty();
            //            $('#txtSearchKeyword').val('');
            BrowsePopup('LibraryFowst', 'Group Distribution Center', [
                {
                    AutoSearch: true,
                    DisplayText: [
                        { Column: 'FowstName' },
                        { Column: 'Address' }
                    ],
                    Click: function (vData) {
                        FowstID = vData.ID;
                        $('#txtGroupFOWST').val(vData.FowstName);
                    }
                }
            ])
        })
        .on('click', '#btnAddGroupHead', function () {
            //SearchEmployee();
            BrowsePopup('LibraryOrganizationEmployee', 'Employee', [
                {
                    AutoSearch: true,
                    DisplayText: [
                        { Column: 'PeopleName' }
                    ],
                    SelectMultiple: true,
                    AllowToSelectAll: true,
                    Message: 'You can select multiple records. Click OK when you\'re done.',
                    Click: function (vData) {
                        for (var i in vData) {
                            var id = vData[i].ID;
                            var peoplename = vData[i].PeopleName;
                            var IsRoleExist = false;

                            if ($('#lstGroupHeads a[id="' + id + '"]').length > 0) return;

                            vData[i].CompanyEmailAddress = (vData[i].EmailAddress || '-');
                            AddGroupHeads(vData[i]);

                            //AjaxSend({
                            //    url: 'Franchisor/CheckIfHaveEmailAddress',
                            //    data: { ID: id },
                            //    success: function (vEmailAdd) {
                            //        var vRows = vEmailAdd.Results.Rows;
                            //        vData.CompanyEmailAddress = '-';
                            //        if (vRows.length > 0) {
                            //            vData.CompanyEmailAddress = $.trim(vRows[0].EmailAddress);
                            //            AddGroupHeads(vData)
                            //        }
                            //        else {
                            //            NewMessageBox('Enter your work email address: <br> <input type="text" id="txtCompanyEmailAddress" data-role="none" class="form-control"/>', 'Email Address',
                            //            [
                            //                {
                            //                    Validation: function () {
                            //                        if (IsEmail($.trim($("#txtCompanyEmailAddress").val())) == false) {
                            //                            alert('Please enter valid email address');
                            //                            $("#txtCompanyEmailAddress").focus();
                            //                            return false;
                            //                        }

                            //                        return true;
                            //                    },
                            //                    Text: 'Save',
                            //                    Click: function () {
                            //                        vData.CompanyEmailAddress = $.trim($('#txtCompanyEmailAddress').val());
                            //                        AddGroupHeads(vData);
                            //                    }
                            //                },
                            //                {
                            //                    Text: 'I don\'t have work email address',
                            //                    Click: function () {
                            //                        vData.CompanyEmailAddress = '-';
                            //                        AddGroupHeads(vData);
                            //                    }
                            //                },
                            //                { Text: 'Cancel' }
                            //            ])
                            //        }
                            //    }
                            //})
                        }                        
                    }
                }
            ])
        })
        .on('click', '#btnAddFranchisee', function () {
            //SearchFranchisee();
            BrowsePopup('LibraryFowstAll', 'Franchisee', [
                {
                    AutoSearch: true,
                    StyleFormat: 'table',
                    ModalSize: 'xl',
                    DisplayText: [
                        { Column: 'FowstName', Label: 'FOWST Name' },
                        { Column: 'FowstOrgName', Label: 'Company Name' },
                        { Column: 'Address', Label: 'FOWST Address' }
                    ],
                    Click: function (vData) {
                        if (CStr2(vData.ID) == '')
                            return;

                        CheckIfOwnedFowst(vData);
                    }
                }
            ], { IsSubscriber: 1 });
        })
        .on('click', '#btnAddCutOffToList', function () {
            AddCutOffToList();
        })
        .on('click', '#btnOrgGroupSave', function () {
            SaveGroup();
        })
        .on('click', '#btnOrgGroupCancel', function () {
            window.location.hash = 'LibraryPage';
        })
        .on('keypress', '#txtGroupFOWST', function (e) {
            if (e.keyCode == 13) {
                $('#TitleLibrary').text('FOWST');
                $('#panSearchLibrary').modal('show');
                SearchFOWSTPopUp($(this).val());
            }
        })
        .on('keypress', '#txtSearchKeyword', function (e) {
            if (e.keyCode == 13) {
                if ($('#TitleLibrary').text() == "FOWST") {
                    SearchFOWSTPopUp($(this).val());
                }
                else if ($('#TitleLibrary').text() == "EMPLOYEE") {
                    SearchEmployeePopUp($(this).val());
                }
                else if ($('#TitleLibrary').text() == "FRANCHISEE") {
                    SearchFranchiseePopUp($(this).val());
                }
            }
        })
        .on('click', '#btnFranchiseeGroupDone', function (e) {
            e.preventDefault();

            window.location = '/People/PeopleMenu';
        })
        .on('click', '#btnFranchiseeGroup', function (e) {
            ClearData();
            $.mobile.changePage('#pageFrancishorGroup');
        })
        .on('keypress', '#txtGroupHead', function (e) {
            if (e.keyCode == 13) {
                SearchEmployee($('#txtGroupHead').val());
            }
        })
        .on('click', '#btnSearchEmployee', function (e) {
            SearchEmployee($('#txtGroupHead').val());
        })
        .on('click', '#btnApplyToAllFOWST', function () {
            if (IsNumeric($('#txtMaxInventoryValue').val())) {
                for (var i = 0; i <= FranchiseFOWSTRows.length - 1; i++) {
                    FranchiseFOWSTRows[i].MaxInventoryValue = $('#txtMaxInventoryValue').val();
                    FranchiseFOWSTRows[i].InventoryValuePriceLevel = $('#selInventoryValuePriceLevel').val();

                    FranchiseFOWSTRows[i].DefaultSellingPriceLevelID = $('input[name="radPriceLevel"]:checked').val(); //$('#selApplyToAllDefaultSellingPrice').val();
                    FranchiseFOWSTRows[i].DefaultSellingPriceLevel = $('input[name="radPriceLevel"]:checked').attr('text'); //$('#selApplyToAllDefaultSellingPrice :selected').text();

                    FranchiseFOWSTRows[i].POPriceLevelID = $('#selApplyToAllDefaultPOPrice').val();
                    FranchiseFOWSTRows[i].POPriceLevel = $('#selApplyToAllDefaultPOPrice :selected').text();
                    FranchiseFOWSTRows[i].IsPOExclusiveToPrincipal = CInt2($('#optIsExclusivePO').is(':checked'));

                    DisplayFranchiseeFOWST(FranchiseFOWSTRows);
                    IsApplyToFOWST = true
                }
            } else {
                NewMessageBox("Please put proper \'Max Inventory Value\' ");
            }
        })
        .on('click', '#btnSpecificDone', function () {
            var mID = '';
            var mName = '';
            var mAddress = '';
            var mOrgName = '';
            var mOrgOwner = '';
            var mOwnershipType = '';

            for (var i = 0; i <= FranchiseFOWSTRows.length - 1; i++) {

                if (FranchiseFOWSTRows[i].ID == SelectedFOWSTID) {
                    mID = FranchiseFOWSTRows[i].ID; ;
                    mName = FranchiseFOWSTRows[i].FowstName;
                    mAddress = FranchiseFOWSTRows[i].Address;
                    mOrgName = FranchiseFOWSTRows[i].FowstOrgName;
                    mOrgOwner = FranchiseFOWSTRows[i].FowstOrgOwner;
                    mOwnershipType = FranchiseFOWSTRows[i].OwnershipType;
                    FranchiseFOWSTRows.splice(i, 1);
                }
            }

            FranchiseFOWSTRows.push({
                ID: mID,
                FowstName: mName,
                Address: mAddress,
                FowstOrgName: mOrgName,
                FowstOrgOwner: mOrgOwner,
                OwnershipType: $('#selOwnershipType').val(),
                MaxInventoryValue: $('#txtSpecificMaxInventoryValue').val(),
                InventoryValuePriceLevel: $('#selSpecificInventoryValuePriceLevel').val(),
                DefaultSellingPriceLevel: $('#selSpecificApplyToAllDefaultSellingPrice :selected').text(),
                DefaultSellingPriceLevelID: $('#selSpecificApplyToAllDefaultSellingPrice').val(),
                POPriceLevel: $('#selSpecificApplyToAllDefaultPOPrice :selected').text(),
                POPriceLevelID: $('#selSpecificApplyToAllDefaultPOPrice').val(),
                IsPOExclusiveToPrincipal: CInt2($('#optSpecificIsExclusivePO').is(':checked'))
            })

            DisplayFranchiseeFOWST(FranchiseFOWSTRows);

            $('#panMaxInventoryValue').modal('hide');
        })
        .on('click', '#btnAddOtherPriceLevel', function () {
            BrowsePopup('LibraryPIMPriceLevel', 'Price Level', [
                {
                    AutoSearch: true,
                    DisplayText: [
                        { Column: 'PriceLevel' }
                    ],
                    Validation: function (e) {
                        if ($('#tbodyOtherPriceLevel tr[id="' + e.ID + '"]').length > 0) {
                            return e.PriceLevel + " already exist";
                        }

                        return true;
                    },
                    Click: function (e) {
                        var pData = [];

                        pData.push({
                            ID: e.ID,
                            PriceLevel: e.PriceLevel
                        })

                        DisplayOtherPriceLevel(pData);
                    }
                }
            ])
        })
        .on('keyup', '#txtGroupSearchFranchise', function () {
            var value = $(this).val().toLowerCase();

            $('#tblFOWSTListBody tr').filter(function () {
                $(this).toggle(
                    $(this).text().toLowerCase().indexOf(value) > -1
                );
            });
        })
    });

    function AddGroupHeads(vData) {
        $('#lstGroupHeads').append($('<a href="#" class="list-group-item list-group-item-action">')
                        .data(vData)
                        .attr('ID', vData.ID)
                        .append($('<table style="width:100%;">')
                            .append($('<tr>')
                                .append($('<td style="word-break:break-all;">')
                                    .data('data', vData)
                                    .append($('<h5>')
                                        .text(vData.PeopleName)
                                    )
                                    .append($('<div>')
                                        .html('Work Email: <span>' + (vData.CompanyEmailAddress == undefined ? '-' : vData.CompanyEmailAddress) + '</span>')
                                    )
                                    .click(function () {
                                        var vData = $(this).data();

                                        NewMessageBox('Enter your work email address: <br> <input type="text" id="txtCompanyEmailAddress" data-role="none" class="form-control"/>', 'Email Address',
                                        [
                                            {
                                                Validation: function () {
                                                    if (IsEmail($.trim($("#txtCompanyEmailAddress").val())) == false) {
                                                        alert('Please enter valid email address');
                                                        $("#txtCompanyEmailAddress").focus();
                                                        return false;
                                                    }

                                                    return true;
                                                },
                                                Text: 'Save',
                                                Click: function () {
                                                    $('#' + vData.ID).remove();

                                                    vData.CompanyEmailAddress = $.trim($('#txtCompanyEmailAddress').val());
                                                    AddGroupHeads(vData);
                                                }
                                            },
                                            { Text: 'Cancel' }
                                        ])
                                    })
                                )
                                .append($('<td style="width:50px; text-align:center;">').append($('<button class="btn btn-xs btn-danger">')
                                        .append($('<span class="bi-trash-fill" aria-hidden="true">'))
                                        .attr('fID', vData.ID)
                                        .click(function (e) {
                                            e.preventDefault();
                                            $('#' + $(this).attr('fID')).remove();
                                        })
                                    )
                                )
                            )
                        )
                    )
        $('#lstGroupHeads');//.listview('refresh');
    }

    function GetAllFranchisorGroup() {
        AjaxSend({
            url: 'Franchisor/GetAllFranchisorGroup',
            success: function (vData) {
                DisplayFranchisorGroup(vData.Results.Rows);
            }
        });
    }

    function DisplayFranchisorGroup(vRows) {
        var pLV = $("#lstFranchiseeGroup");

        pLV.empty();

        for (i = 0; i < vRows.length; i++) {
            pItem = $('<li>')
                .attr('ID', vRows[i].ID)
                .attr('href', "#")
                .append($('<a>')
                    .attr('ID', vRows[i].ID)
                    .data('data', vRows[i])
                    .click(function (e) {
                        ClearData();
                        ID = $(this).attr('ID');
                        $.mobile.changePage('#pageFrancishorGroup');
                        GetFranchisorGroupInfo(ID);
                        GetGroupFranchiseeFOWST(ID);
                        GetGroupSchedule();
                        GetGroupHeads();
                    })
                    .append($('<h3>')
                        .text(vRows[i].GroupName)
                    )
                    .append($('<p>')
                        .text(vRows[i].Description)
                    )
                )

            pLV.append(pItem);
        }

        try {
            pLV;//.listview('refresh');
        }
        catch (e) {
            pLV.listview();
        }
    }

    function GetFranchisorGroupInfo(ID) {

        data = { ID: ID };

        AjaxSend({
            url: 'Franchisor/GetFranchisorGroupInfo',
            data: data,
            success: function (vData) {
                DisplayFranchisorGroupInfo(vData.Results.Rows);
            }
        });

    }

    function GetGroupFranchiseeFOWST(ID) {
        data = { ID: ID };

        AjaxSend({
            url: 'Franchisor/GetGroupFranchiseeFOWST',
            data: data,
            success: function (vData) {
                DisplayFranchiseeFOWST(vData.Results.Rows);
                FranchiseFOWSTRows = vData.Results.Rows;
            }
        });
    }

    function GetGroupSchedule(ID) {
        data = { ID: ID };

        AjaxSend({
            url: 'Franchisor/GetGroupSchedule',
            data: data,
            success: function (vData) {
                if (vData.Results.Rows.length > 0) {
                    DisplayGroupSchedule(vData.Results.Rows);
                }
            }
        });
    }

    function GetOtherPriceLevels(ID) {
        data = { ID: ID, OrgID: sessionStorage.OrgID };

        AjaxSend({
            url: 'Franchisor/GetOtherPriceLevels',
            data: data,
            success: function (vData) {
                if (vData.Results.Rows.length > 0) {
                    DisplayOtherPriceLevel(vData.Results.Rows, 1);
                }
            }
        });
    }

    function DisplayOtherPriceLevel(vRows, isFromOtherPriceLevel) {
        for (var i = 0; i < vRows.length; i++) {
            if (CBool(isFromOtherPriceLevel)) {
                if ($('#' + vRows[i].ID).length > 0) {
                    $('#' + vRows[i].ID).attr('state', 'edit').attr('sid', vRows[i].SID).attr('tormv', 0)
                    $('input[id="rad_' + vRows[i].ID + '"]').prop('checked', CBool(vRows[i].IsActive));
                }
                else {
                    DisplayOtherPriceLevelOnTable(vRows[i], 0);
                }
            }
            else {
                DisplayOtherPriceLevelOnTable(vRows[i], 1);
            }
        }

        if (CBool(isFromOtherPriceLevel)) {
            $('#tbodyOtherPriceLevel tr[tormv="1"]').remove();
        }
        else {
            $('input[name="radPriceLevel"]:first').prop('checked', true);
        }

        $('input[type="radio"]');//.checkboxradio('refresh');
    }

    function DisplayOtherPriceLevelOnTable(vRows, tormv) {
        $('#tbodyOtherPriceLevel').append($('<tr>')
            .attr('id', vRows.ID)
            .attr('sid', (vRows.SID || ''))
            .attr('state', (vRows.SID == undefined ? 'add' : 'edit'))
            .attr('tormv', tormv)
            .append($('<td>')
                .css('line-height', 3)
                .text(vRows.PriceLevel)
            )
            .append($('<td>')
                .append($('<div class="form-check mb-3" style="font-size:20px;">')
                    .append($('<input type="radio" class="form-check-input" name="radPriceLevel" text="' + vRows.PriceLevel + '" value="' + vRows.ID + '" id="rad_' + vRows.ID + '" data-iconpos="notext"/>'))
                )
            )
            .append($('<td>')
                .css('text-align', 'center')
                .append($('<button class="btn btn-xs btn-danger">')
                    .append($('<span class="bi-trash-fill" aria-hidden="true">'))
                )
                .data('data', vRows)
                .click(function (e) {
                    e.stopPropagation();
                    var dis = $(this).parents();

                    NewMessageBox('Are you sure you want to delete <b>' + $(this).data('data').PriceLevel + '</b>?', 'Price Level', [
                        {
                            Text: 'Yes',
                            Click: function () {
                                if (dis.attr('sid') == "") {
                                    dis[0].remove();
                                }
                                else {
                                    dis.closest('tr').attr('state', 'delete').hide();
                                }
                            }
                        },
                        { Text: 'No' }
                    ])
                })
            )
        )
        $('input[type="radio"]');//.checkboxradio();
    }

    function GetGroupHeads(ID) {
        data = { ID: ID };

        AjaxSend({
            url: 'Franchisor/GetGroupHeads',
            data: data,
            success: function (vData) {
                if (vData.Results.Rows.length > 0) {
                    DisplayGroupHeads(vData.Results.Rows);
                }
            }
        });
    }

    function DisplayGroupHeads(vRows) {
        var pLV = $("#lstGroupHeads");

        for (i = 0; i <= vRows.length - 1; i++) {
            data = {
                ID: vRows[i].ID,
                PeopleName: vRows[i].PeopleName,
                CompanyEmailAddress: (vRows[i].CompanyEmailAddress == undefined ? '-' : vRows[i].CompanyEmailAddress)
            };

            pItem = $('<a href="#" class="list-group-item list-group-item-action">')
                        .data(data)
                        .attr('ID', vRows[i].ID)
                        .append($('<table style="width:100%;">')
                            .append($('<tr>')
                                .append($('<td style="word-break:break-all;">')
                                    .data('data', data)
                                    .append($('<h5>')
                                        .text(vRows[i].PeopleName)
                                    )
                                    .append($('<div>')
                                        .html('Work Email: <span>' + (vRows[i].CompanyEmailAddress == undefined ? '-' : vRows[i].CompanyEmailAddress) + '</span>')
                                    )
                                    .click(function () {
                                        var vData = $(this).data('data');

                                        NewMessageBox('Enter your work email address: <br> <input type="text" id="txtCompanyEmailAddress" data-role="none" class="form-control" value="' + CStr2(vData.CompanyEmailAddress) + '"/>', 'Email Address',
                                        [
                                            {
                                                Validation: function () {
                                                    if (IsEmail($.trim($("#txtCompanyEmailAddress").val())) == false) {
                                                        alert('Please enter valid email address');
                                                        $("#txtCompanyEmailAddress").focus();
                                                        return false;
                                                    }

                                                    return true;
                                                },
                                                Text: 'Save',
                                                Click: function () {
                                                    $('#' + vData.ID).remove();

                                                    vData.CompanyEmailAddress = $.trim($('#txtCompanyEmailAddress').val());
                                                    AddGroupHeads(vData);
                                                }
                                            },
                                            { Text: 'Cancel' }
                                        ])
                                    })
                                )
                                .append($('<td style="width:50px; text-align:center;">').append($('<button class="btn btn-xs btn-danger">')
                                        .append($('<span class="bi-trash-fill" aria-hidden="true">'))
                                        .attr('fID', vRows[i].ID)
                                        .click(function (e) {
                                            e.preventDefault();
                                            $('#' + $(this).attr('fID')).remove();
                                        })
                                    )
                                )
                            )
                        )

            pLV.append(pItem);
        }

        try {
            pLV;//.listview('refresh');
        }
        catch (e) {
            pLV.listview();
        }
    }

    function GetOrganizationPriceLevels() {
        data = { OrgID: sessionStorage.OrgID };

        AjaxSend({
            url: 'Franchisor/GetOrganizationPriceLevels',
            data: data,
            success: function (vData) {
                if (vData.Results.Rows.length > 0) {
                    DisplayPriceLevels(vData.Results.Rows);
                    DisplayOtherPriceLevel(vData.Results.Rows, 0);
                }
            }
        });
    }

    function DisplayPriceLevels(vRows) {
        var pLV1 = $('#selSpecificApplyToAllDefaultSellingPrice');
        var pLV2 = $('#selApplyToAllDefaultSellingPrice');
        var pLV3 = $('#selApplyToAllDefaultPOPrice');
        var pLV4 = $('#selSpecificApplyToAllDefaultPOPrice');

        for (var i = 0; i <= vRows.length - 1; i++) {
            pLV1.append(new Option(vRows[i].PriceLevel, vRows[i].ID, false));
            pLV2.append(new Option(vRows[i].PriceLevel, vRows[i].ID, false));
            pLV3.append(new Option(vRows[i].PriceLevel, vRows[i].ID, false));
            pLV4.append(new Option(vRows[i].PriceLevel, vRows[i].ID, false));
        }

        try {
            pLV1//.selectmenu('refresh');
            pLV2//.selectmenu('refresh');
            pLV3//.selectmenu('refresh');
            pLV4//.selectmenu('refresh');
        } catch (e) {
            pLV1//.selectmenu();
            pLV2//.selectmenu();
            pLV3//.selectmenu();
            pLV4//.selectmenu();
        }
    }


    function DisplayGroupSchedule(vRows) {
        var pLV = $("#lstSchedule");

        var Weekday = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"]

        for (i = 0; i <= vRows.length - 1; i++) {
            data = {
                ID: vRows[i].ID,
                ScheduleType: $('#selSchedule').val(),
                ScheduleName: vRows[i].CutOffName,
                ScheduleDay: Weekday[vRows[i].WeekDay],
                ScheduleTime: $.format.date(new Date('1/1/2014 ' + vRows[i].CutOffTime), 'hh:mm a')
            };

            pItem = $('<li class="list-group-item">')
                .attr('ID', vRows[i].ID)
                .data('data', data)
                .append($('<table style="width:100%">')
                    .append($('<tr>')
                        .append($('<td>')
                            .append($('<h3>')
                                .text(vRows[i].CutOffName)
                            )
                            .append($('<p>')
                                .text($('#selSchedule').val() + ' - ' + Weekday[vRows[i].WeekDay] + ' - ' + $.format.date(new Date('1/1/2014 ' + vRows[i].CutOffTime), 'hh:mm a'))
                            )
                        )
                        .append($('<td class="text-center" style="width:50px;">')
                            .append($('<button data-role="none" class="btn btn-danger">')
                                .attr('ID', vRows[i].ID)
                                .append($('<span class="bi-trash">'))
                                .data('data', data)
                                .click(function () {
                                    var data = $(this).data('data');

                                    $('#' + $(this).attr('ID')).remove();

                                    if (IsGuid($(this).attr('ID'))) {
                                        DisableSchedule($(this).attr('ID'));
                                    }
                                })
                            )
                        )
                    )
                )


            pLV.append(pItem);
        }

        try {
            pLV;//.listview('refresh');
        }
        catch (e) {
            pLV.listview();
        }
    }


    function DisableSchedule(ID) {
        data = { ID: ID };

        AjaxSend({
            url: 'Franchisor/DisableSchedule',
            data: data,
            success: function (vData) {
            }
        });
    }

    function DisplayFranchiseeFOWST(vRows) {

        $('#dvFOWSTList').css('visibility', 'visible');
        $('#dvFOWSTList').css('display', 'block');

        var pLV = $("#tblFOWSTListBody");
        var IsPOExclusiveToPrincipal = false;

        pLV.empty();

        if (vRows.length > 0) {
            for (i = 0; i <= vRows.length - 1; i++) {

                IsPOExclusiveToPrincipal = 'No';

                if (CBool(vRows[i].IsPOExclusiveToPrincipal)) {
                    IsPOExclusiveToPrincipal = 'Yes';
                }

                pItem = $('<tr>')
                        .attr('ID', vRows[i].ID)
                        .data('data', vRows[i])
                        .css('cursor', 'pointer')
                        .append($('<td>')
                            .text((CStr2(vRows[i].FowstCode) == '' ? '' : vRows[i].FowstCode + ' - ') + vRows[i].FowstName)
                        )
                        .append($('<td>')
                            .text(vRows[i].Address)
                        )
                        .append($('<td>')
                            .text((vRows[i].FowstOrgName || ''))
                        )
                        .append($('<td>')
                            .text((vRows[i].FowstOrgOwner || ''))
                        )
                        .append($('<td>')
                            .text((vRows[i].OwnershipType || ''))
                        )
                        .append($('<td>')
                            .text((vRows[i].MaxInventoryValue || '-'))
                        )
                        .append($('<td>')
                            .text((vRows[i].InventoryValuePriceLevel || '-'))
                        )
                        .append($('<td>')
                            .text((vRows[i].DefaultSellingPriceLevel || '-'))
                        )
                        .append($('<td>')
                            .text((vRows[i].POPriceLevel || '-'))
                        )
                        .append($('<td>')
                            .text(IsPOExclusiveToPrincipal)
                        )
                        .click(function () {
                            SelectedFOWSTID = $(this).attr("ID");
                            $('#panMaxInventoryValue').modal('show');
                            $('#selOwnershipType').empty();

                            $('#txtSpecificMaxInventoryValue').val(CDec2($(this).data('data').MaxInventoryValue));

                            if ($(this).data('data').OwnershipType == "") {
                                $('#selOwnershipType').append('<option value="Company-Owned">Company-Owned</option><option value="Franchised">Franchised</option><option value="Co-Owned">Co-Owned</option>');
                            }
                            else if ($(this).data('data').OwnershipType == "Company-Owned") {
                                $('#selOwnershipType').append('<option value="Company-Owned">Company-Owned</option>');
                            }
                            else {
                                $('#selOwnershipType').append('<option value="Franchised">Franchised</option><option value="Co-Owned">Co-Owned</option>');
                            }
                            $('#selOwnershipType').val($(this).data('data').OwnershipType);
                            $('#selOwnershipType')//.selectmenu()//.selectmenu('refresh');

                            $('#selSpecificApplyToAllDefaultSellingPrice').empty();
                            $('#tbodyOtherPriceLevel tr').each(function () {
                                $('#selSpecificApplyToAllDefaultSellingPrice').append(new Option($(this).find('td:eq(0)').text(), $(this).attr('id')))
                            })
                            $('#selSpecificApplyToAllDefaultSellingPrice')//.selectmenu('refresh');
                        })
                        .append($('<td>')
                            .append($('<button data-role="none" class="btn btn-danger">')
                                            .append($('<span class="bi-trash">'))
                                            //.button()
                                            .attr('ID', vRows[i].ID)
                                            .data('data', vRows[i])
                                            .click(function (e) {
                                                e.stopPropagation();

                                                for (var i = 0; i <= FranchiseFOWSTRows.length - 1; i++) {
                                                    if (FranchiseFOWSTRows[i].ID == $(this).attr("ID")) {
                                                        FranchiseFOWSTRows.splice(i, 1);
                                                    }
                                                }

                                                DisplayFranchiseeFOWST(FranchiseFOWSTRows);
                                            })
                                        )
                        )
                pLV.append(pItem);
            }
        }
        else {
            pItem = $('<tr>')
                        .append($('<td>')
                            .attr('colspan', 11)
                            .css('text-align', 'center')
                            .text('No Result(s) Found')
                        )
            pLV.append(pItem);
        }

        //try {
        //    pLV;//.listview('refresh');
        //}
        //catch (e) {
        //    pLV.listview();
        //}
    }

    function RemoveFranchiseeFromGroup(vID) {
        var data = { FOWSTID: vID }

        AjaxSend({
            url: 'Franchisor/RemoveFranchiseeFromGroup',
            data: data,
            success: function (vData) {
            }
        });
    }

    function DisplayFranchisorGroupInfo(vRows) {
        ID = vRows[0].ID;
        $('#txtGroupName').val(vRows[0].GroupName);
        $('#txtGroupDescription').val(vRows[0].Description);
        FowstID = vRows[0].FranchisorFowstID;
        $('#txtGroupFOWST').val(vRows[0].FOWSTName);
        //$('#txtGroupHead').val(vRows[0].FullName);

        PeopleID = vRows[0].FranchisorGroupHead;

        $('#txtRoyaltyFee').val(vRows[0].RoyaltyFee);
        $('#txtMarketingFee').val(vRows[0].MarketingFee);
        $('#chbIsGroupAFranchisee').prop('checked', CInt2(vRows[0].IsFranchisee))
        $('#chbIsGroupActive').prop('checked', CInt2(vRows[0].IsActive))
    }



    function SearchFOWST(vFOWST) {
        Browser.LibraryFowst.Click = function (e) {
            FowstID = e.ID;
            $('#txtGroupFOWST').val(e.FowstName);
        }
        Browser.LibraryFowst.Show(vFOWST);

    }

    function SearchFOWSTPopUp(s) {
        var pData = {};

        pData.keywords = s;

        AjaxSend({
            url: 'Browser/LibraryFowst',
            data: pData,
            success: function (vData) {
                $('#lstOfLibrary').empty();

                if (vData.Results.Rows.length > 0) {
                    for (var p = 0; p < vData.Results.Rows.length; p++) {
                        $('#lstOfLibrary').append($('<li>')
                                .append($('<a>')
                                    .attr('ID', vData.Results.Rows[p].ID)
                                    .append($('<br><p style="font-weight:bold;">')
                                        .text(vData.Results.Rows[p].FowstName)
                                    )
                                    .append($('<p>')
                                        .text(vData.Results.Rows[p].Address)
                                    )
                                    .data('data', vData.Results.Rows[p])
                                    .click(function (e) {
                                        FowstID = $(this).data('data').ID;
                                        $('#txtGroupFOWST').val($(this).data('data').FowstName);
                                        $('#panSearchLibrary').modal('hide');
                                    })
                                ));
                    }

                }
                else {
                    $('#lstOfLibrary').append($('<li>').text('No Result(s) Found'));
                }

                $('#lstOfLibrary');//.listview('refresh');
            }
        });
    }

    function SearchEmployeePopUp(s) {
        var pData = {};

        pData.keywords = s;

        AjaxSend({
            url: 'Browser/LibraryOrganizationEmployee',
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
                                        var id = $(this).data('data').ID;
                                        var peoplename = $(this).data('data').PeopleName;
                                        var IsRoleExist = false;

                                        $('#lstGroupHeads a').each(function (m_index) {
                                            if ($.trim($(this).attr('ID')) == id) {
                                                IsRoleExist = true;
                                            }
                                        });

                                        if (IsRoleExist) {
                                            alert(peoplename + ' already exist in a list.')
                                            return;
                                        }

                                        $('#panSearchLibrary').modal('hide');
                                        $('#txtSearchKeyword').val('');
                                        $('#lstGroupHeads').append($('<li>')
                                                            .attr('ID', id)
                                                            .append($('<a>')
                                                                .attr('ID', id)
                                                                .append($('<br><p style="font-weight:bold;">')
                                                                    .text(peoplename)
                                                                )
                                                            )
                                                            .append($('<a>')
                                                                .attr('ID', id)
                                                                .click(function () {
                                                                    $('#' + $(this).attr('ID')).remove();
                                                                    $('#lstGroupHeads');//.listview('refresh');
                                                                })
                                                            ));
                                        $('#lstGroupHeads');//.listview('refresh');
                                    })
                                ));
                    }

                }
                else {
                    $('#lstOfLibrary').append($('<li>').text('No Result(s) Found'));
                }

                $('#lstOfLibrary');//.listview('refresh');
            }
        });
    }

    function SearchFranchiseePopUp(s) {
        var pData = {};

        pData.keywords = s;
        pData.IsSubscriber = 1;

        AjaxSend({
            url: 'Browser/LibraryFowstAll',
            data: pData,
            success: function (vData) {
                $('#lstOfLibrary').empty();

                if (vData.Results.Rows.length > 0) {
                    for (var p = 0; p < vData.Results.Rows.length; p++) {
                        $('#lstOfLibrary').append($('<li>')
                                .append($('<a>')
                                    .attr('ID', vData.Results.Rows[p].ID)
                                    .append($('<br><p style="font-weight:bold;">')
                                        .text(vData.Results.Rows[p].FowstName)
                                    )
                                    .append($('<p>Corporate Name:')
                                        .text(vData.Results.Rows[p].FowstOrgName)
                                    )
                                    .append($('<p>Corporate Address:')
                                        .text(vData.Results.Rows[p].Address)
                                    )
                                    .data('data', vData.Results.Rows[p])
                                    .click(function (e) {
                                        if (CStr2($(this).data('data').ID) == '')
                                            return;

                                        CheckIfOwnedFowst($(this).data('data'));

                                    })
                                ));
                    }

                }
                else {
                    $('#lstOfLibrary').append($('<li>').text('No Result(s) Found'));
                }

                $('#lstOfLibrary');//.listview('refresh');
            }
        });
    }

    var isContinue = false;
    var isDone = false;
    var franchiseeId = '';
    var fName = '';
    var fAddress = '';
    var fOrgName = '';
    var fOrgOwner = '';
    var fOwnershipType = '';
    $('#btnOKOrgEmp').click(function (e) {
        e.preventDefault();

        if (isContinue) {

            AjaxSend({
                url: 'DashboardsAndReports/AddFranchisee',
                data: { FranchiseeID: franchiseeId, IsOwn: false },
                success: function (vData) {
                    if (vData.FranchiseData == "ok") {
                        NewMessageBox('Your franchisor request has been sent successfully.');
                        isDone = true;
                    }

                },
                error: function (vError) {
                    NewMessageBox('There was an error while processing your request.');
                }
            });

            isContinue = false;
            isDone = false;

        }


        if (isDone) {

            AddThisToFranchiseeList(franchiseeId, fName, fAddress, fOrgName, fOrgOwner, fOwnershipType);
            $('#panSearchLibrary').modal('hide');

            isContinue = false;
            isDone = false;

        }
    });

    function CheckIfOwnedFowst(vRows) {
        var pData = {};

        pData.OrgID = sessionStorage.OrgID;
        pData.FowstID = vRows.ID;

        AjaxSend({
            url: 'Franchisor/CheckIfOwnedFowst',
            data: pData,
            success: function (vData) {
                if (vData.IsOwnedFowst.Rows.length > 0) {
                    AddFranchisee(vRows, 'Company-Owned');
                }
                else {
                    //$('#panSearchLibrary').modal('hide');
                    PopupButtonList('', 'Ownership Type', [
                        {
                            Text: 'Co-owned', Click: function () {
                                AddFranchisee(vRows, 'Co-Owned');
                            }
                        }, {
                            Text: 'Franchised', Click: function () {
                                AddFranchisee(vRows, 'Franchised');
                            }
                        }
                    ])
                    //$('#panOwnershipTypeOption').modal('show');
                    //$('#btnCoOwned').click(function () {
                        
                    //    $('#panOwnershipTypeOption').modal('hide');
                    //})
                    //$('#btnFranchised').click(function () {
                        
                    //    $('#panOwnershipTypeOption').modal('hide');
                    //})
                }
            }
        })
    }

    function AddFranchisee(vRows, ownershiptype) {
        var fowstname = ' a FOWST of ' + vRows.FowstOrgName;
        var id = vRows.ID;
        var fowstaddress = vRows.Address;
        var fowstorgname = vRows.FowstOrgName;
        var fowstorgowner = vRows.FowstOrgOwner;

        if (CStr2(vRows.FowstName) != '')
            fowstname = vRows.FowstName;

        AjaxSend({
            url: 'Franchisor/CheckIfExistingFranchisee',
            data: { FowstID: id, FowstOrgID: vRows.OrganizationID },
            success: function (vData) {

                if (vData.Result == "True") {
                    AddThisToFranchiseeList(id, fowstname, fowstaddress, fowstorgname, fowstorgowner, ownershiptype);
                    //history.back();
                    $('#panSearchLibrary').modal('hide');
                } else if (vData.Result == "Own") {
                    if (IsApplyToFOWST) {
                        var data = {
                            FranchiseeID: id,
                            IsOwn: true,
                            MaxInventoryValue: $('#txtMaxInventoryValue').val(),
                            InventoryValuePriceLevel: $('#selInventoryValuePriceLevel').val(),
                            DefaultSellingPriceLevelID: $('input[name="radPriceLevel"]:checked').val(), //$('#selApplyToAllDefaultSellingPrice').val(),
                            POPriceLevelID: $('#selApplyToAllDefaultPOPrice').val(),
                            IsPOExclusiveToPrincipal: CInt2($('#optIsExclusivePO').is(':checked')),
                            OwnershipType: ownershiptype
                        }
                    } else {
                        var data = {
                            FranchiseeID: id,
                            IsOwn: true
                        }
                    }


                    AjaxSend({
                        url: 'DashboardsAndReports/AddFranchisee',
                        data: data,
                        success: function (vData) {
                            if (vData.FranchiseData == "ok") {
                                AddThisToFranchiseeList(id, fowstname, fowstaddress, fowstorgname, fowstorgowner, ownershiptype);
                                $('#panSearchLibrary').modal('hide');
                            }

                        },
                        error: function (vError) {
                            NewMessageBox('There was an error while processing your request.');
                        }
                    });



                } else {
                    //$('#panSearchLibrary').modal('hide');
                    function AddNow() {
                        if (isContinue) {

                            AjaxSend({
                                url: 'DashboardsAndReports/AddFranchisee',
                                data: { FranchiseeID: franchiseeId, IsOwn: false, AutoApproveFranchisorRequests: vData.AutoApproveFranchisorRequests },
                                success: function (e) {
                                    if (e.FranchiseData == "ok" && !vData.AutoApproveFranchisorRequests) {
                                        NewMessageBox('Your franchisor request has been sent successfully.');
                                        isDone = true;
                                    }

                                },
                                error: function (vError) {
                                    NewMessageBox('There was an error while processing your request.');
                                }
                            });

                            isContinue = false;
                            isDone = false;

                        }


                        if (isDone || vData.AutoApproveFranchisorRequests) {

                            AddThisToFranchiseeList(franchiseeId, fName, fAddress, fOrgName, fOrgOwner, fOwnershipType);
                            $('#panSearchLibrary').modal('hide');

                            isContinue = false;
                            isDone = false;

                        }
                    }                    
                    
                    franchiseeId = id;
                    fName = fowstname;
                    fAddress = fowstaddress;
                    fOrgName = fowstorgname;
                    fOrgOwner = fowstorgowner;
                    fOwnershipType = ownershiptype;
                    isContinue = true;

                    if (vData.AutoApproveFranchisorRequests) {
                        AddNow();
                    } else {
                        NewMessageBox('You are about to add ' + fowstname + ' as your franchisee. If you click Continue, a confirmation message will be sent to the administrator for approval.', 'Message', [
                            {
                                Text: 'Continue', Click: function () {
                                    AddNow();
                                }
                            }, { Text: 'Cancel' }
                        ]);
                    }
                    //$('#btnOKOrgEmp').text('Continue');
                    //$('#divBtnCancel').show();
                    //$('#btnCancelOrgEmp').text('No').show().click(function () {
                    //    $('#panOrgEmpMessage').modal('hide');
                    //});
                } // end if

            },
            error: function (vError) {
                NewMessageBox('There was an error while processing your request.');
            }
        });
    }

    function AddThisToFranchiseeList(id, name, address, orgname, orgowner, ownershiptype) {
        var IsRoleExist = false;

        if (IsApplyToFOWST) {
            FranchiseFOWSTRows.push({
                ID: id,
                FowstName: name,
                Address: address,
                FowstOrgName: orgname,
                FowstOrgOwner: orgowner,
                OwnershipType: ownershiptype,
                MaxInventoryValue: $('#txtMaxInventoryValue').val(),
                InventoryValuePriceLevel: $('#selInventoryValuePriceLevel').val(),
                DefaultSellingPriceLevel: $('input[name="radPriceLevel"]:checked').attr('text'), //$('#selApplyToAllDefaultSellingPrice :selected').text(),
                DefaultSellingPriceLevelID: $('input[name="radPriceLevel"]:checked').val(), //$('#selApplyToAllDefaultSellingPrice').val(),
                POPriceLevel: $('#selApplyToAllDefaultPOPrice :selected').text(),
                POPriceLevelID: $('#selApplyToAllDefaultPOPrice').val(),
                IsPOExclusiveToPrincipal: CInt2($('#optIsExclusivePO').is(':checked'))
            })
        } else {
            FranchiseFOWSTRows.push({
                ID: id,
                FowstName: name,
                Address: address,
                FowstOrgName: orgname,
                FowstOrgOwner: orgowner,
                OwnershipType: ownershiptype,
                MaxInventoryValue: '-',
                InventoryValuePriceLevel: $('#selInventoryValuePriceLevel').val(),
                DefaultSellingPriceLevel: $('input[name="radPriceLevel"]:checked').attr('text'), //$('#selApplyToAllDefaultSellingPrice :selected').text(),
                DefaultSellingPriceLevelID: $('input[name="radPriceLevel"]:checked').val(), //$('#selApplyToAllDefaultSellingPrice').val(),
                POPriceLevel: $('#selApplyToAllDefaultPOPrice :selected').text(),
                POPriceLevelID: $('#selApplyToAllDefaultPOPrice').val(),
                IsPOExclusiveToPrincipal: 'No'
            })
        }

        DisplayFranchiseeFOWST(FranchiseFOWSTRows);

        //$('#lstGroupMembers li a').each(function (m_index) {
        //    if ($.trim($(this).attr('ID')) == id) {
        //        IsRoleExist = true;
        //    }
        //});

        //if (IsRoleExist) {
        //    alert(name + ' already exist in a list.')
        //    return;
        //}

        //$('#panSearchLibrary').modal('hide');
        //$('#txtSearchKeyword').val('');
        //$('#lstGroupMembers').append($('<li>')
        //                    .attr('ID', id)
        //                    .append($('<a>')
        //                        .attr('ID', id)
        //                        .append($('<br><p style="font-weight:bold;">')
        //                            .text(name)
        //                        )
        //                    )
        //                    .append($('<a>')
        //                        .attr('ID', id)
        //                        .attr('href', "#")
        //                        .attr('data-icon', 'delete')
        //                        .click(function () {
        //                            $('#' + $(this).attr('ID')).remove();
        //                            $('#lstGroupMembers');//.listview('refresh');
        //                        })
        //                    ));
        //$('#lstGroupMembers');//.listview('refresh');
    }

    function SearchFranchisee() {

        //            Browser.LibraryFranchisee.Click = function (e) {
        //                AddToFranchiseeList(e);
        //            }

        //            Browser.LibraryFranchisee.Show();
        Browser.LibraryFowstAll.InputData.IsSubscriber = 1;
        Browser.LibraryFowstAll.Click = function (e) {
            if (CStr2(e.ID) == '')
                return;

            var fowstname = ' a FOWST of ' + e.FowstOrgName;

            if (CStr2(e.FowstName) != '')
                fowstname = e.FowstName;

            AjaxSend({
                url: 'Franchisor/CheckIfExistingFranchisee',
                data: { FowstID: e.ID },
                success: function (vData) {

                    if (vData.Result == "True") {
                        AddToFranchiseeList(e);
                        history.back();
                    } else if (vData.Result == "Own") {

                        AjaxSend({
                            url: 'DashboardsAndReports/AddFranchisee',
                            data: { FranchiseeID: e.ID, IsOwn: true },
                            success: function (vData) {
                                if (vData.FranchiseData == "ok") {
                                    AddToFranchiseeList(e);
                                    history.back();
                                }

                            },
                            error: function (vError) {
                                MessageBox("There was an error while processing your request.",
                                        "Error",
                                        [
                                            {
                                                Text: 'Ok'
                                            }
                                        ]
                                );

                            }
                        });



                    } else {
                        MessageBox("You are about to add " + fowstname + " as your franchisee. If you click Continue, " +
                            " a confirmation message will be sent to the administrator for approval.",
                            "Add New Franchisee",
                            [
                                {
                                    Text: 'Continue',
                                    Click: function () {
                                        AjaxSend({
                                            url: 'DashboardsAndReports/AddFranchisee',
                                            data: { FranchiseeID: e.ID, IsOwn: false },
                                            success: function (vData) {
                                                if (vData.FranchiseData == "ok") {
                                                    MessageBox("Your franchisor request has been sent successfully.",
                                                            "Add Franchisee", null, {
                                                                OnClose: function () {
                                                                    AddToFranchiseeList(e);
                                                                    history.back();
                                                                    history.back();
                                                                }
                                                            }
                                                    );
                                                }

                                            },
                                            error: function (vError) {
                                                MessageBox("There was an error while processing your request.",
                                                        "Error",
                                                        [
                                                            {
                                                                Text: 'Ok'
                                                            }
                                                        ]
                                                );

                                            }
                                        });
                                    }
                                },
                                    {
                                        Text: 'Cancel'
                                    }
                            ]
                        ); //end messagebox

                    } // end if

                },
                error: function (vError) {

                    MessageBox("There was an error while processing your request.",
                                                        "Error",
                                                        [
                                                            {
                                                                Text: 'Ok'
                                                            }
                                                        ]
                                                );


                }
            });

        };
        Browser.LibraryFowstAll.Show();
    }

    function SearchEmployee() {
        Browser.LibraryOrganizationEmployee.Click = function (e) {
            AddToGroupHeadList(e);
        }

        Browser.LibraryOrganizationEmployee.Show();
    }

    function CheckIfExistingFranchisee(fowstid) {

        AjaxSend({
            url: 'Franchisor/CheckIfExistingFranchisee',
            data: { FowstID: fowstid },
            success: function (vData) {
                if (vData.Result == "True") {
                    return true;
                } else {
                    return false;
                }

            },
            error: function (vError) {

                MessageBox("There was an error while processing your request.",
                                                        "Error",
                                                        [
                                                            {
                                                                Text: 'Ok'
                                                            }
                                                        ]
                                                );
                return false;


            }
        });

    }


    function AddToFranchiseeList(vData) {
        var pLV = $("#lstGroupMembers");
        var IsFranchiseeExist = false;

        $('#lstGroupMembers a').each(function () {

            if ($(this).data().ID == vData.ID) {
                IsFranchiseeExist = true;
            }
        })

        if (IsFranchiseeExist) {
            return;
        }

        pItem = $('<li>')
                .attr('ID', vData.ID)
                .attr('href', "#")
                .append($('<a>')
                    .data(vData)
                    .click(function (e) {
                    })
                    .append($('<h3>')
                        .text(vData.FowstName)
                    )
                    .append($('<p>')
                        .text(vData.Address)
                    )
                )
                .append($('<a>')
                    .attr('ID', vData.ID)
                    .attr('href', "#")
                    .click(function () {
                        $('#' + $(this).attr('ID')).remove();
                    })
                )


        pLV.append(pItem);


        try {
            pLV;//.listview('refresh');
        }
        catch (e) {
            pLV.listview();
        }
    }

    function AddToGroupHeadList(vData) {
        var pLV = $("#lstGroupHeads");
        var data;
        var IsEmployeeExist = false;

        $('#lstGroupHeads a').each(function () {
            if ($(this).data().ID == vData.ID) {
                IsEmployeeExist = true;

            }
        });

        if (IsEmployeeExist) {
            MessageBox(vData.PeopleName + ' already exist in Group Head\'s list')
            return;
        }


        pItem = $('<li>')
                .attr('ID', vData.ID)
                .attr('href', "#")
                .append($('<a>')
                    .data(vData)
                    .click(function (e) {
                    })
                    .append($('<h3>')
                        .text(vData.PeopleName)
                    )
                )
                .append($('<a>')
                    .attr('ID', vData.ID)
                    .attr('href', "#")
                    .click(function () {
                        $('#' + $(this).attr('ID')).remove();
                    })
                )


        pLV.append(pItem);


        try {
            pLV;//.listview('refresh');
        }
        catch (e) {
            pLV.listview();
        }
    }

    function AddCutOffToList() {
        var pLV = $("#lstSchedule");
        var data = '';
        var p = [];

        if ($('#selHourFrom').val() == '' || $('#selDayOfHour').val() == '') {
            NewMessageBox("Please indicate time for this cut-off.", "Franchisor Central");
            return;
        }
        data = {
            ID: $('#selSchedule').val() + $('#txtScheduleName').val() + $('#selScheduleDay').val(),
            ScheduleType: $('#selSchedule').val(),
            ScheduleName: $('#txtScheduleName').val(),
            ScheduleDay: $('#selScheduleDay').val(),
            ScheduleTime: $('#selHourFrom').val() + ' ' + $('#selDayOfHour').val()
        };

        pItem = $('<li class="list-group-item">')
                .attr('ID', ($('#selSchedule').val() + $('#txtScheduleName').val() + $('#selScheduleDay').val()).replace(/\s/g, "-"))
                .data('data', data)
                .append($('<table style="width:100%">')
                    .append($('<tr>')
                        .append($('<td>')
                            .append($('<h3>')
                                .text($('#txtScheduleName').val())
                            )
                            .append($('<p>')
                                .text($('#selSchedule').val() + ' - ' + $('#selScheduleDay').val() + ' - ' + $('#selHourFrom').val() + ' ' + $('#selDayOfHour').val())
                            )
                        )
                        .append($('<td class="text-center" style="width:50px;">')
                            .append($('<button data-role="none" class="btn btn-danger">')
                                .attr('ID', ($('#selSchedule').val() + $('#txtScheduleName').val() + $('#selScheduleDay').val()).replace(/\s/g, "-"))
                                .append($('<span class="bi-trash">'))
                                .data('data', data)
                                .click(function () {
                                    var data = $(this).data('data');

                                    $('#' + $(this).attr('ID')).remove();
                                })
                            )
                        )
                    )
                )


        pLV.append(pItem);


        try {
            pLV;//.listview('refresh');
        }
        catch (e) {
            pLV.listview();
        }
    }

    function SaveGroup() {
        var data;
        var mFranchisee = [];
        var mSchedule = [];
        var mGroupHeads = [];
        var mOtherPriceLevel = [];
        var mCounter = 0;

        if ($('#lstGroupHeads a').length == 0) {
            NewMessageBox('Please assign at least one group head');
            return;
        }

        if ($('#txtGroupName').val() == '') {
            NewMessageBox("Please enter group name.", "Franchisor Central");
            return;
        }

        if (FowstID == '') {
            NewMessageBox("Please enter reporting FOWST.", "Franchisor Central");
            return;
        }

        if ($('input[name="radPriceLevel"]:checked').length == 0) {
            NewMessageBox('Please select at least one default price level');
            return;
        }

        //$('#lstGroupMembers li').each(function () {
        //    mFranchisee[mCounter] = { Franchisee: $(this).attr('ID') };
        //    mCounter++;
        //});

        if ($('#selSchedule').val() == 'Daily') {
            mCounter = 0;
            $('#lstSchedule li').each(function () {
                if ($(this).data('data').ScheduleName != undefined) {
                    mSchedule[mCounter] = $(this).data('data');
                    mCounter++;
                }
            });
        }

        mCounter = 0;
        $('#lstGroupHeads a').each(function () {
            if ($(this).data().PeopleName != undefined) {
                mGroupHeads[mCounter] = $(this).data();
                mCounter++;
            }
        });

        $('#tbodyOtherPriceLevel tr').each(function () {
            mOtherPriceLevel.push({
                State: $(this).attr('state'),
                ID: $(this).attr('id'),
                SID: ($(this).attr('sid') || ''),
                IsActive: $(this).find('td:eq(1)').find('input[type="radio"]').is(':checked')
            })
        });

        data = {
            ID: ID,
            Name: $('#txtGroupName').val(),
            Description: $('#txtGroupDescription').val(),
            GroupReportingFOWST: FowstID,
            RoyaltyFee: $('#txtRoyaltyFee').val(),
            MarketingFee: $('#txtMarketingFee').val(),
            Franchisee: FranchiseFOWSTRows,
            Schedule: mSchedule,
            GroupHeads: mGroupHeads,
            OtherPriceLevel: mOtherPriceLevel,
            OrgID: sessionStorage.OrgID,
            IsFranchisee: $('#chbIsGroupAFranchisee').is(':checked'),
            IsActive: $('#chbIsGroupActive').is(':checked')
        };

        AjaxSend({
            url: 'Franchisor/SaveGroup',
            data: data,
            success: function (vData) {
                if (IsGuid(vData.Results)) {
                    //history.back();
                    //GetAllFranchisorGroup();
                    $('#tblBody').html('');
                    window.location.hash = '';
                    window.location.hash = 'LibraryPage';
                    LibraryTable.GroupSettings();
                }
            }
        });
    };

    function ClearData() {

        ID = '';

        $('#txtGroupName').val('');
        $('#txtGroupDescription').val('');
        $('#txtRoyaltyFee').val('');
        $('#txtMarketingFee').val('');
        $('#txtGroupHead').val('');
        $('#txtGroupFOWST').val('');
        $('#lstGroupMembers').empty();
        $('#txtScheduleName').val('');
        $('#lstSchedule').empty();
        $('#lstGroupHeads').empty();
        $('#selHourFrom').val('');
        $('#selDayOfHour').val('');
        $('#tblFOWSTListBody').empty();

        try {
            $('#selHourFrom')//.selectmenu('refresh');
            $('#selDayOfHour')//.selectmenu('refresh');
        }
        catch (e) {
            $('#selHourFrom')//.selectmenu();
            $('#selDayOfHour')//.selectmenu();
        }

    }


});