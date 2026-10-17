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
            getPeopleAliasList();
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

        $('#btnPeopleAliasPopUpMenu').click(function (e) {
            e.preventDefault();

            $("#divPeopleAliasPopUpMenu").popup("open")

        });

        $('#popPeopleAliasPeopleInfoName').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoName')), "_self");

        });

        $('#popPeopleAliasPeopleInfoAddress').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoAddress')), "_self");

        });

        $('#popPeopleAliasPeoplePersonalInfo').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeoplePersonalInfo')), "_self");

        });

        $('#popPeopleAliasPeopleContact').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleContactNo')), "_self");

        });

        $('#popPeopleAliasPeopleEducation').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleEducation')), "_self");

        });

        $('#popPeopleAliasPeopleEmpHistory').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleEmpHistory')), "_self");

        });

        $('#popPeopleAliasPeopleRelatedPerson').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleRelatedPerson')), "_self");

        });

        $('#popPeopleAliasPeoplePrimeIDReference').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeoplePrimeIDReference')), "_self");

        });

        $('#popPeopleAliasPeopleUserAccount').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleUserAccount')), "_self");

        });

        $('#btnPeopleNameAliasAdd').click(function (e) {
            e.preventDefault();

            $.mobile.changePage('#pagePeopleAliasName');
            ClearPeopleAliasFields();

        });

        $('#btnAliasTitle').click(function () {

            Browser.LibraryPeoplePreSuffix.Click = function (e) {

                $('#txtAliasTitleID').val(e.ID)
                $('#btnAliasTitle').text(e.Code)
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

        $('#btnPeopleAliasNameSave').click(function (e) {
            e.preventDefault();

            if ($.trim($("#txtAliasSurname").val()) == "") {
                MessageBox("Please enter surname.",
                            "Surname",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }

            if ($.trim($("#txtAliasFirstName").val()) == "") {
                MessageBox("Please enter first name.",
                            "First Name",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }

            if ($.trim($("#txtAliasMiddleName").val()) == "") {
                MessageBox("Please enter middle name.",
                            "Middle Name",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }

            SavePersonAliasInfo();

        });

        $('#btnPeopleAliasNameRemove').click(function (e) {
            e.preventDefault();

            MessageBox("Remove this record?",
                'Remove',
                [
                    { Text: 'Yes',
                        Click: function () {
                            RemovePersonAliasInfo();
                        }
                    }
                ,
                    { Text: 'No'
                    }
                ]
            );

        });

        $('#btnPeopleAliasNameCancel').click(function (e) {
            e.preventDefault();

            $.mobile.changePage('#pagePeopleAliasNameList');

        });

        $('#btnPeopleAliasDone').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoSummary')), "_self");

        });

        function getPeopleAliasList() {
            AjaxSend({
                url: 'People/getPeopleAliasList',
                success: function (vData) {
                    LoadPeopleAliasList(vData.PeopleAliasList.Rows);
                }
            });
        }

        function LoadPeopleAliasList(vRows) {
            var pLV = $('#lstPeopleAlias');
            var pItem;

            $("#divRecordCount").text(" Number of record(s) found: " + vRows.length);

            pLV.html('');

            pItem = $('<li>')
                    .attr('data-role', "list-divider")
                    .text('Alias List');
            pLV.append(pItem);

            for (var p = 0; p <= vRows.length - 1; p++) {
                pItem = $('<li>')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();
                                $.mobile.changePage('#pagePeopleAliasName');
                                ShowPersonAliasInfo($(this).attr('ID'));
                            })
                            .append($('<h3>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].AliasName)
                            )
                        );
                pLV.append(pItem);
            };

            pLV.listview('refresh');
        };

        function ShowPersonAliasInfo(strID) {
            var pData = {};
            pData.ID = strID;

            ClearPeopleAliasFields();

            AjaxSend({
                url: "People/getPeopleAliasInfo",
                data: pData,
                success: function (vData) {

                    if (vData.PeopleAliasInfo.Rows.length > 0) {

                        $("#txtAliasID").val(vData.PeopleAliasInfo.Rows[0].ID || '');

                        $("#txtAliasTitleID").val(vData.PeopleAliasInfo.Rows[0].TitleID || '');
                        $("#btnAliasTitle").text(vData.PeopleAliasInfo.Rows[0].Title || 'Please Select').button('refresh');

                        $("#txtAliasSurname").val(vData.PeopleAliasInfo.Rows[0].Surname || '');
                        $("#txtAliasFirstName").val(vData.PeopleAliasInfo.Rows[0].FirstName || '');
                        $("#txtAliasMiddleName").val(vData.PeopleAliasInfo.Rows[0].MiddleName || '');

                        $("#selAliasPersonalSuffixName").val(vData.PeopleAliasInfo.Rows[0].PersonalSuffixName || '');
                        $("#selAliasPersonalSuffixName").selectmenu('refresh');

                        AjaxSend({
                            url: "People/getPeopleAliasPostSuffixList",
                            data: pData,
                            success: function (vDataPostSuffix) {

                                LoadPeoplePostSuffixList(vDataPostSuffix.PeopleAliasData.Rows);

                            }
                        });

                        $("#btnPeopleAliasNameRemove").show();
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

        function ClearPeopleAliasFields() {

            // dont clear
            // sessionStorage.clear();
            proj.ClearSessionStorage();

            $("#txtAliasID").val('');

            $("#txtAliasTitleID").val('');
            $("#btnAliasTitle").text('Please Select').button('refresh');

            $("#txtAliasSurname").val('');
            $("#txtAliasFirstName").val('');
            $("#txtAliasMiddleName").val('');

            $("#selAliasPersonalSuffixName").val('');
            $("#selAliasPersonalSuffixName").selectmenu('refresh');

            LoadPostSuffixList();

            $("#btnPeopleAliasNameRemove").hide();

        }

        function SavePersonAliasInfo() {

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
                AliasID : $("#txtAliasID").val(),
                AliasTitleID : $.trim($("#txtAliasTitleID").val()),
                AliasSurname : $("#txtAliasSurname").val(),
                AliasFirstName : $("#txtAliasFirstName").val(),
                AliasMiddleName : $("#txtAliasMiddleName").val(),
                AliasPersonalSuffixName : $("#selAliasPersonalSuffixName").val(),

                PostSuffixList: pData
            };

            AjaxSend({
                url: "People/CheckPeopleUniqueAliasName",
                data: pData,
                success: function (vData) {

                    if (vData.PeopleAliasData.Rows.length > 0) {
                        MessageBox("Alias with the same name already exist.",
                            "Alias Unique Name",
                            [
                                { Text: 'Ok' }
                            ]
                        );
                    }
                    else {

                        AjaxSend({
                            url: "People/SavePeopleAliasName",
                            data: pData,
                            success: function (vData) {

                                $.mobile.changePage('#pagePeopleAliasNameList');
                                getPeopleAliasList();

                            }
                        });

                    }

                }
            });

        };

        function RemovePersonAliasInfo() {

            var pData = {};
            pData.AliasID = $("#txtAliasID").val();

            AjaxSend({
                url: "People/RemovePeopleAliasName",
                data: pData,
                success: function (vData) {

                    $.mobile.changePage('#pagePeopleAliasNameList');
                    getPeopleAliasList();

                }
            });

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
                                            .attr("style", "white-space:normal")
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