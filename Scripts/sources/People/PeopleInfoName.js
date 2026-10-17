require([
    'common',
    GetPathFormat('/Scripts3/Browser/LibraryPeoplePreSuffix.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryPeoplePostSuffix.js?crc=')
    ], function() {

    $(function () {

        var IsNewPostSuffix = true;
        var arPostSuffix = new Array();
        var intPostSuffixCntr = 0;
        var intPostSuffixIndex = 0;

        var intPostSuffixRecID = 0;
        var intPostSuffixID = 1;
        var intPostSuffix = 2;
        var intPostSuffixState = 3;

        $(document).ready(function () {

            AjaxSend({
                url: 'People/IsNewEntry',
                success: function (vData) {

                    setMonthCollection("selDOBMonth");
                    setYearCollection("selDOBYear");

                    ShowPersonNameInfo(vData.PeopleID)

                }
            });
        });

        $('.btnProfileBackToHome').click(function () {
            if (GetUrlParameters().p == 'eas') {
                location.href = MergeUrlParameters({
                    OrgID: sessionStorage.OrgID,
                    IsMobile: 0
                }, '/erp')
            } else if (GetUrlParameters().p == 'crm') {
                location.href = MergeUrlParameters({
                    OrgID: sessionStorage.OrgID,
                    IsMobile: 0
                }, '/CRMDashboard')
            } else {
                location.href = '/People/PeopleMenu';
            }
        })

        $('#btnPeopleInfoPopUpMenu').click(function (e) {
            e.preventDefault();

            $("#divPeopleInfoPopUpMenu").popup("open")

        });

        $('#popPeopleInfoPeopleInfoAlias').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoAlias')), "_self");

        });

        $('#popPeopleInfoPeopleInfoAddress').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoAddress')), "_self");

        });

        $('#popPeopleInfoPeoplePersonalInfo').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeoplePersonalInfo')), "_self");

        });

        $('#popPeopleInfoPeopleContact').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleContactNo')), "_self");

        });

        $('#popPeopleInfoPeopleEducation').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleEducation')), "_self");

        });

        $('#popPeopleInfoPeopleEmpHistory').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleEmpHistory')), "_self");

        });

        $('#popPeopleInfoPeopleRelatedPerson').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleRelatedPerson')), "_self");

        });

        $('#popPeopleInfoPeoplePrimeIDReference').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeoplePrimeIDReference')), "_self");

        });

        $('#popPeopleInfoPeopleUserAccount').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleUserAccount')), "_self");

        });

        $('#btnTitle').click(function () {

            Browser.LibraryPeoplePreSuffix.Click = function (e) {

                $('#txtTitleID').val(e.ID)
                $('#btnTitle').text(e.Code)
                    .button('refresh');

            };
            Browser.LibraryPeoplePreSuffix.Show();

        });

        $('#btnAddPostSuffix').click(function (e) {

            Browser.LibraryPeoplePostSuffix.Click = function (e) {

                IsNewPostSuffix = true
                intPostSuffixCntr = intPostSuffixCntr + 1;
                intPostSuffixIndex = intPostSuffixCntr;

                arPostSuffix = new Array();
                arPostSuffix[intPostSuffixRecID] = "";
                arPostSuffix[intPostSuffixID] = e.ID;
                arPostSuffix[intPostSuffix] = e.Code;
                arPostSuffix[intPostSuffixState] = (IsNewPostSuffix ? "New" : "Edit");

                sessionStorage["PostSuffix-" + intPostSuffixIndex] = JSON.stringify(arPostSuffix);

                LoadPostSuffixList();

            };

            Browser.LibraryPeoplePostSuffix.Show();

        });

        $('#btnPeopleNameDone').click(function (e) {
            e.preventDefault();

            if ($.trim($("#txtSurname").val()) == "" || $.trim($("#txtSurname").val()).length <= 1) {
                MessageBox("Please enter surname.",
                            "Surname",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }

            if ($.trim($("#txtFirstName").val()) == "" || $.trim($("#txtFirstName").val()).length <= 1) {
                MessageBox("Please enter first name.",
                            "First Name",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }

            if ($.trim($("#txtMiddleName").val()) == "" || $.trim($("#txtMiddleName").val()).length <= 1) {
                MessageBox("Please enter middle name.",
                            "Middle Name",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }

            if ($.trim($("#selDOBMonth").val()) == "") {
                MessageBox("Please enter valid date of birth.",
                            "Date Of Birth",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }
            if ($.trim($("#selDOBDay").val()) == "") {
                MessageBox("Please enter valid date of birth.",
                            "Date Of Birth",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }
            if ($.trim($("#selDOBYear").val()) == "") {
                MessageBox("Please enter valid date of birth.",
                            "Date Of Birth",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }

            if ($.trim($("#txtMotherMaidenSurname").val()) == "" || $.trim($("#txtMotherMaidenSurname").val()).length <= 1) {
                MessageBox("Please enter mother maiden surname.",
                            "Mother Maiden Surname",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }
            if ($.trim($("#txtMotherMaidenFirstName").val()) == "" || $.trim($("#txtMotherMaidenFirstName").val()).length <= 1) {
                MessageBox("Please enter mother first name.",
                            "First Name",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }

            ValidateUniquePersonNameInfo();

        });

        $('#selDOBMonth').change(function (e) {
            e.preventDefault();

            setDayCollection('selDOBMonth', 'selDOBDay', 'selDOBYear', true);

        });

        $('#selDOBYear').change(function (e) {
            e.preventDefault();

            setDayCollection('selDOBMonth', 'selDOBDay', 'selDOBYear', true);

        });

        $('#selDOBYear').click(function (e) {
            e.preventDefault();

            if ($.trim($('#selDOBYear').val()) == "") {
                $('#selDOBYear').val('1980');
                $('#selDOBYear').selectmenu('refresh');
            }

        });

        $('#btnPeopleConflictYes').click(function (e) {
            e.preventDefault();

            $('.ui-dialog').dialog('close');
            SavePersonNameInfo();

        });

        $('#btnPeopleConflictNo').click(function (e) {
            e.preventDefault();

            $('.ui-dialog').dialog('close');

        });

        function ShowPersonNameInfo(strID) {
            var pData = {};
            pData.PeopleID = strID;

            ClearPeopleNameFields();

            AjaxSend({
                url: "People/getPeopleInfoName",
                data: pData,
                success: function (vData) {

                    if (vData.PeopleNameData.Rows.length > 0) {

                        $("#txtID").val(vData.PeopleNameData.Rows[0].ID || '');

                        $("#txtTitleID").val(vData.PeopleNameData.Rows[0].TitleID || '');
                        $("#btnTitle").text(vData.PeopleNameData.Rows[0].Title || 'Please Select').button('refresh');
                        $("#txtSurname").val(vData.PeopleNameData.Rows[0].Surname || '');
                        $("#txtFirstName").val(vData.PeopleNameData.Rows[0].FirstName || '');
                        $("#txtMiddleName").val(vData.PeopleNameData.Rows[0].MiddleName || '');

                        $("#selPersonalSuffixName").val(vData.PeopleNameData.Rows[0].PersonalSuffixName || '');
                        $("#selPersonalSuffixName").selectmenu('refresh');

                        $("#selDOBMonth").val(vData.PeopleNameData.Rows[0].DateOfBirthMonth || '');
                        $("#selDOBMonth").selectmenu('refresh');
                        $("#selDOBYear").val(vData.PeopleNameData.Rows[0].DateOfBirthYear || '');
                        $("#selDOBYear").selectmenu('refresh');
                        setDayCollection('selDOBMonth', 'selDOBDay', 'selDOBYear', true);
                        $("#selDOBDay").val(vData.PeopleNameData.Rows[0].DateOfBirthDay || '');
                        $("#selDOBDay").selectmenu('refresh');

                        $("#txtNickName").val(vData.PeopleNameData.Rows[0].NickName || '');

                        $("#txtMotherMaidenSurname").val(vData.PeopleNameData.Rows[0].MotherMaidenSurname || '');
                        $("#txtMotherMaidenFirstName").val(vData.PeopleNameData.Rows[0].MotherMaidenFirstName || '');
                        $("#txtMotherMaidenMiddleName").val(vData.PeopleNameData.Rows[0].MotherMaidenMiddleName || '');

                        AjaxSend({
                            url: "People/getPeoplePostSuffixList",
                            data: pData,
                            success: function (vDataPostSuffix) {

                                LoadPeoplePostSuffixList(vDataPostSuffix.PeopleNameData.Rows);

                            }
                        });

                        $("#btnGoTo").show();

                    }
                }
            });

        };

        function LoadPeoplePostSuffixList(vRows) {

            for (var p = 0; p <= vRows.length - 1; p++) {

                arPostSuffix = new Array();
                arPostSuffix[intPostSuffixRecID] = vRows[p].ID;
                arPostSuffix[intPostSuffixID] = vRows[p].PostSuffixID;
                arPostSuffix[intPostSuffix] = vRows[p].PostSuffixCode;
                arPostSuffix[intPostSuffixState] = "";

                sessionStorage["PostSuffix-" + p] = JSON.stringify(arPostSuffix);

            };

            intPostSuffixCntr = vRows.length;
            LoadPostSuffixList();

        };

        function ClearPeopleNameFields() {

            // dont clear
            // sessionStorage.clear();
            proj.ClearSessionStorage();

            $("#txtID").val('');

            $("#txtTitleID").val('');
            $("#btnTitle").text('Please Select').button('refresh');

            $("#txtSurname").val('');
            $("#txtFirstName").val('');
            $("#txtMiddleName").val('');

            $("#selPersonalSuffixName").val('');
            $("#selPersonalSuffixName").selectmenu('refresh');

            $("#selDOBMonth").val('');
            $("#selDOBMonth").selectmenu('refresh');
            $("#selDOBDay").val('');
            $("#selDOBDay").selectmenu('refresh');
            $("#selDOBYear").val('');
            $("#selDOBYear").selectmenu('refresh');

            $("#txtNickName").val('');

            $("#txtMotherMaidenSurname").val('');
            $("#txtMotherMaidenFirstName").val('');
            $("#txtMotherMaidenMiddleName").val('');

            LoadPostSuffixList();

            $("#btnGoTo").hide();
        }

        function ValidateUniquePersonNameInfo() {

            var pData = {};
            pData.PeopleID = $.trim($("#txtID").val());
            pData.TitleID = $.trim($("#txtTitleID").val());
            pData.Surname = $.trim($("#txtSurname").val());
            pData.FirstName = $.trim($("#txtFirstName").val());
            pData.MiddleName = $.trim($("#txtMiddleName").val());
            pData.PersonalSuffixName = $.trim($("#selPersonalSuffixName").val());
            pData.DateOfBirth = $("#selDOBMonth").val() + '/' + $("#selDOBDay").val() + '/' + $("#selDOBYear").val();
            pData.NickName = $.trim($("#txtNickName").val());

            pData.MotherMaidenSurname = $.trim($("#txtMotherMaidenSurname").val());
            pData.MotherMaidenFirstName = $.trim($("#txtMotherMaidenFirstName").val());
            pData.MotherMaidenMiddleName = $.trim($("#txtMotherMaidenMiddleName").val());

            pData.FullCheckName = "0";

            AjaxSend({
                url: "People/CheckPeopleUniqueName",
                data: pData,
                success: function (vData) {

                    if (vData.PeopleNameData.Rows.length > 0) {
                        $.mobile.changePage('#pagePeopleConflictList', { transition: 'pop', role: 'dialog' });
                        LoadPeopleConflictList(vData.PeopleNameData.Rows);
                    }
                    else {

                        SavePersonNameInfo();

                    }

                }
            });

        };

        function SavePersonNameInfo() {
            var pData = [];
            var intIndex;

            intIndex = 0;
            for (var i = 0; i <= intPostSuffixCntr; i++) {

                if (sessionStorage["PostSuffix-" + i] != undefined) {

                    arPostSuffix = new Array();
                    arPostSuffix = JSON.parse(sessionStorage["PostSuffix-" + i]);

                    pData[intIndex] = {
                        ID: arPostSuffix[intPostSuffixRecID],
                        PostSuffixID: arPostSuffix[intPostSuffixID],
                        RecState: arPostSuffix[intPostSuffixState]
                    };

                    intIndex = intIndex + 1;

                }
            }

            pData = {
                PeopleID: $.trim($("#txtID").val()),
                TitleID: $.trim($("#txtTitleID").val()),
                Surname: $.trim($("#txtSurname").val()),
                FirstName: $.trim($("#txtFirstName").val()),
                MiddleName: $.trim($("#txtMiddleName").val()),
                PersonalSuffixName: $.trim($("#selPersonalSuffixName").val()),
                DateOfBirth: $("#selDOBMonth").val() + '/' + $("#selDOBDay").val() + '/' + $("#selDOBYear").val(),
                NickName: $.trim($("#txtNickName").val()),

                MotherMaidenSurname: $.trim($("#txtMotherMaidenSurname").val()),
                MotherMaidenFirstName: $.trim($("#txtMotherMaidenFirstName").val()),
                MotherMaidenMiddleName: $.trim($("#txtMotherMaidenMiddleName").val()),

                FullCheckName: "1",

                PostSuffixList: pData
            };


            AjaxSend({
                url: "People/CheckPeopleUniqueName",
                data: pData,
                success: function (vData) {

                    if (vData.PeopleNameData.Rows.length > 0) {
                        MessageBox("Person with the same name, date of birth and mother maiden name already exist.",
                            "Person Unique Name",
                            [
                                { Text: 'Ok',
                                    Click: function () {
                                        $('.ui-dialog').dialog('close');
                                    }
                                }
                            ]
                        );

                    }
                    else {
                        AjaxSend({
                            url: "People/SavePeopleInfoName",
                            data: pData,
                            success: function (vSaveData) {

                                if (vSaveData.PeopleNameData == "DateOfBirthError") {
                                    MessageBox("Please enter valid date of birth.",
                                    "Date Of Birth",
                                    [
                                        { Text: 'Ok' }
                                    ]
                                    );
                                }
                                else {

                                    $("#txtID").val(vSaveData.PeopleNameData || '');

                                    window.open(MergeUrlParameters(GetUrlParameters(), '/People/PeopleInfoSummary'), "_self");

                                }

                            }
                        });

                    }

                }
            });

        };

        function LoadPeopleConflictList(vRows) {
            var pLV = $('#lstPeopleConflict');
            var pItem;

            $("#divConflictRecordCount").text(" Person with the same name record(s) found: " + vRows.length);

            pLV.html('');

            pItem = $('<li>')
                    .attr('data-role', "list-divider")
                    .text('Conflict Person List');
            pLV.append(pItem);

            for (var p = 0; p <= vRows.length - 1; p++) {

                pItem = $('<li>')
                    .append($('<a>')
                        .append($('<h3>')
                            .text(vRows[p].Surname + ', ' + vRows[p].FirstName + ' ' + vRows[p].MiddleName)
                        )
                        .append($('<p>')
                            .text('Date Of Birth : ' + $.format.date(new Date(vRows[p].DateOfBirth), 'MM/dd/yyyy'))
                        )
                        .append($('<p>')
                            .text('Mother Maiden Name : ' + vRows[p].MotherMaidenSurname + ', ' + vRows[p].MotherMaidenFirstName + ' ' + vRows[p].MotherMaidenMiddleName)
                        )
                    );
                pLV.append(pItem);
            };

            pLV.listview('refresh');
        };

        function LoadPostSuffixList() {
            var pLV = $('#lstPeoplePostSuffix');
            var pItem;

            pLV.html('');
            for (var i = 0; i <= intPostSuffixCntr; i++) {

                if (sessionStorage["PostSuffix-" + i] != undefined) {

                    arPostSuffix = new Array();
                    arPostSuffix = JSON.parse(sessionStorage["PostSuffix-" + i]);

                    if (arPostSuffix[intPostSuffixState] != "Remove") {

                        pItem = $('<li id="lst' + i + '">')
                            .append($('<a>')
                                    .attr('ID', i)
                                    .data('Data', arPostSuffix)
                                    .append($('<h3>')
                                            .text(arPostSuffix[intPostSuffix])
                                    )
                            )
                            .append($('<a>')
                                    .attr('ID', i)
                                    .attr('href', "#")
                                    .data('Data', arPostSuffix)
                                    .click(function (e) {
                                        e.preventDefault();

                                        sessionStorage["PostSuffixToRemoveIndex"] = $(this).attr('ID');
                                        sessionStorage["PostSuffixToRemove"] = JSON.stringify($(this).data('Data'));

                                        MessageBox("Remove this record?",
                                            'Remove',
                                            [
                                                { Text: 'Yes',
                                                    Click: function () {

                                                        var arPostSuffixToRemove = new Array();
                                                        arPostSuffixToRemove = JSON.parse(sessionStorage["PostSuffixToRemove"]);
                                                        intPostSuffixIndex = sessionStorage["PostSuffixToRemoveIndex"];

                                                        arPostSuffixToRemove[intPostSuffixState] = "Remove";
                                                        sessionStorage["PostSuffix-" + intPostSuffixIndex] = JSON.stringify(arPostSuffixToRemove);

                                                        LoadPostSuffixList();

                                                        sessionStorage["PostSuffixToRemove"] = null;
                                                        $('.ui-dialog').dialog('close');
                                                    }
                                                }
                                            ,
                                                { Text: 'No'
                                                }
                                            ]
                                        );

                                    })
                            );

                        pLV.append(pItem);

                    }

                }
            }

            pLV.listview('refresh');
        };
    });
});