require([
    'common'
    ], function () {

        $(function () {

            var mLoaded = false;
            var isIE = false || !!document.documentMode;

            $("#btnAddOrgOwner > span").attr("style", "text-align:left;white-space:normal")

            $.mobile.activePage.bind('pageshow', PageShow);
            function PageShow() {
                mLoaded = true;
                getOrgOwnerList();
            };
            if (!mLoaded) PageShow();

            window.OrgOwner = {
                GetOrgOwnerList: function () {
                    getOrgOwnerList();
                }
            }

            $('body')
            .on('click', '#btnAddOrgOwner', function (e) {
                e.preventDefault();

                //$.mobile.changePage('#pageOrgOwnerInfo');
                $('#pageOwnerList').hide();
                $('#pageOrgOwnerInfo').show();
                ClearOrgOwnerFields();
            })
            .on('click', '#btnOrgOwner', function (e) {
                BrowsePopup('LibraryPeople', 'Owner', [
                    {
                        DisplayText: [{
                            Column: 'PeopleName'
                        }],
                        Click: function (e) {
                            $('#txtOrgOwnerPeopleID').val(e.ID);
                            $('#btnOrgOwner').text(e.PeopleName);//.button('refresh');
                        }
                    }
                ])
                //                Browser.LibraryPeople.Click = function (e) {

                //                    $('#txtOrgOwnerPeopleID').val(e.ID)
                //                    $('#btnOrgOwner').text(e.PeopleName)
                //                    ;//.button('refresh');

                //                };

                //                Browser.LibraryPeople.Show();
            })
            .on('click', '#btnOrgOwnerJobOccupation', function (e) {
                BrowsePopup('LibraryJobOccupation', 'Position', [
                    {
                        DisplayText: [{
                            Column: 'JobOccupation'
                        }],
                        Click: function (e) {
                            $('#txtOrgOwnerJobOccupationID').val(e.ID);
                            $('#btnOrgOwnerJobOccupation').text(e.JobOccupation);//.button('refresh');
                        }
                    }
                ])
                //                Browser.LibraryJobOccupation.Click = function (e) {

                //                    $('#txtOrgOwnerJobOccupationID').val(e.ID)
                //                    $('#btnOrgOwnerJobOccupation').text(e.JobOccupation)
                //                    ;//.button('refresh');

                //                };
                //                Browser.LibraryJobOccupation.Show();
            })
            .on('click', '#btnOrgOwnerSave', function (e) {
                e.preventDefault();

                if ($.trim($("#txtOrgOwnerPeopleID").val()) == "") {
                    NewMessageBox("Please select owner.",
                            "Owner",
                            [
                                { Text: 'Ok' }
                            ]
                );
                    return;
                }

                if ($.trim($("#txtOrgOwnerJobOccupationID").val()) == "") {
                    NewMessageBox("Please select position.",
                            "Position",
                            [
                                { Text: 'Ok' }
                            ]
                );
                    return;
                }

                SaveOrgOwner();
            })
            .on('click', '#btnOrgOwnerRemove', function (e) {
                e.preventDefault();

                NewMessageBox("Remove this record?",
                'Remove',
                [
                    { Text: 'Yes',
                        Click: function () {
                            RemoveOrgOwner();
                        }
                    }
                ,
                    { Text: 'No'
                    }
                ]);
            })
            .on('click', '#btnOrgOwnerCancel', function (e) {
                e.preventDefault();

                // ChangePage('/Organization/OrganizationOwner');

                //                window.location = MergeUrlParameters({
                //                    OrgID: sessionStorage.OrgID,
                //                    IsMobile: CBool(IsMobile) ? 1 : 0
                //                }, '/Organization/OrganizationOwner');                
                $('#pageOwnerList').show();
                $('#pageOrgOwnerInfo').hide();

            })
            .on('click', '#btnOrgOwnerDone', function (e) {
                e.preventDefault();

                if (window.pageSourcePage != undefined) {
                    //$.mobile.changePage("#" + $(window.pageSourcePage).attr('id'));
                    $('body').scrollTop(0);
                    $('#UniversalDiv').html('');
                    LibraryTable.OrganizationPage();
                }
                else {
                    //   ChangePage('/Organization/OrganizationSummary');

                    //                    window.location = MergeUrlParameters({
                    //                        OrgID: sessionStorage.OrgID,
                    //                        IsMobile: CBool(IsMobile) ? 1 : 0
                    //                    }, '/Organization/OrganizationSummary');
                    $('body').scrollTop(0);
                    $('#UniversalDiv').html('');
                    LibraryTable.OrganizationPage();

                }
            })

            function RemoveOrgOwner() {
                var pData = { "OrgOwnerID": $('#txtOrgOwnerID').val() };

                AjaxSend({
                    url: 'Organization/RemoveOrgOwner',
                    data: pData,
                    success: function (vData) {
                        if (vData.OrgOwnerData == "TRUE") {
                            $('#pageOwnerList').show();
                            $('#pageOrgOwnerInfo').hide();
                            getOrgOwnerList();
                        }
                        else {
                            NewMessageBox('Unable to remove');
                        }
                    }
                })
            }

            function getOrgOwnerList() {
                AjaxSend({
                    url: 'Organization/getOrgOwnerList',
                    success: function (vData) {
                        LoadOrgOwnerList(vData.OrgOwnerList.Rows);
                    }
                });
            }

            function LoadOrgOwnerList(vRows) {
                var pLV = $('#lstOrgOwner');
                var pItem;

                $("#divOrgOwnerRecordCount").text(" Number of record(s) found: " + vRows.length);

                pLV.html('');

                //pItem = $('<li>')
                //    .attr('data-role', "list-divider")
                //    .text('Directors, Officers and Stockholders, Partners or Owners');
                //pLV.append(pItem);

                for (var p = 0; p <= vRows.length - 1; p++) {

                    var OwnershipType = (vRows[p].IsIncorporator == "1" ? "Incorporator" : "");
                    OwnershipType = OwnershipType + (vRows[p].IsDirector == "1" ? (OwnershipType == "" ? "Director" : ", Director") : "");
                    OwnershipType = OwnershipType + (vRows[p].IsStockHolder == "1" ? (OwnershipType == "" ? "Stock Holder" : ", Stock Holder") : "");
                    OwnershipType = OwnershipType + (vRows[p].IsOfficer == "1" ? (OwnershipType == "" ? "Officer" : ", Officer") : "");
                    OwnershipType = OwnershipType + (vRows[p].IsExecutiveCommittee == "1" ? (OwnershipType == "" ? "Executive Committee" : ", Executive Committee") : "");
                    OwnershipType = OwnershipType + (vRows[p].IsOwner == "1" ? (OwnershipType == "" ? "Owner or Proprietor" : ", Owner or Proprietor") : "");

                    pItem = $('<li class="list-group-item">')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();
                                //$.mobile.changePage('#pageOrgOwnerInfo');
                                $('#pageOwnerList').hide();
                                $('#pageOrgOwnerInfo').show();
                                ShowOrgOwnerInfo($(this).attr('ID'));
                            })
                            .append($('<div>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].PeopleName)
                            )
                            .append($('<div>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].JobOccupation)
                            )
                            .append($('<div>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].BusinessUnit)
                            )
                            .append($('<div>')
                                .attr("style", "white-space:normal")
                                .text(OwnershipType)
                            )
                        );
                    pLV.append(pItem);
                };

                //pLV.listview('refresh');
            };

            function ShowOrgOwnerInfo(strID) {
                var pData = {};
                pData.ID = strID;

                ClearOrgOwnerFields();

                AjaxSend({
                    url: "Organization/getOrgOwnerInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.OrgOwnerInfo.Rows.length > 0) {

                            $("#txtOrgOwnerID").val(vData.OrgOwnerInfo.Rows[0].ID || '');

                            $("#txtOrgOwnerPeopleID").val(vData.OrgOwnerInfo.Rows[0].PeopleID || '');
                            $("#btnOrgOwner").text(vData.OrgOwnerInfo.Rows[0].PeopleName);//.button('refresh');

                            $("#txtOrgOwnerJobOccupationID").val(vData.OrgOwnerInfo.Rows[0].JobOccupationID || '');
                            $("#btnOrgOwnerJobOccupation").text(vData.OrgOwnerInfo.Rows[0].JobOccupation || 'Please Select');//.button('refresh');
                            $("#txtOrgOwnerDepartmentBusinessUnit").val(vData.OrgOwnerInfo.Rows[0].BusinessUnit || '');

                            $('#chkOrgOwnerIsIncorporator').attr("checked", (vData.OrgOwnerInfo.Rows[0].IsIncorporator == 0 ? false : true));//.checkboxradio("refresh");
                            $('#chkOrgOwnerIsDirector').attr("checked", (vData.OrgOwnerInfo.Rows[0].IsDirector == 0 ? false : true));//.checkboxradio("refresh");
                            $('#chkOrgOwnerIsStockHolder').attr("checked", (vData.OrgOwnerInfo.Rows[0].IsStockHolder == 0 ? false : true));//.checkboxradio("refresh");
                            $('#chkOrgOwnerIsOfficer').attr("checked", (vData.OrgOwnerInfo.Rows[0].IsOfficer == 0 ? false : true));//.checkboxradio("refresh");
                            $('#chkOrgOwnerIsExecutiveCommittee').attr("checked", (vData.OrgOwnerInfo.Rows[0].IsExecutiveCommittee == 0 ? false : true));//.checkboxradio("refresh");
                            $('#chkOrgOwnerIsOwner').attr("checked", (vData.OrgOwnerInfo.Rows[0].IsOwner == 0 ? false : true));//.checkboxradio("refresh");

                            pData.PeopleID = vData.OrgOwnerInfo.Rows[0].PeopleID;

                            $("#btnOrgOwnerRemove").show();
                        }
                    }
                });

            };

            function ClearOrgOwnerFields() {

                $("#txtOrgOwnerID").val('');

                $("#txtOrgOwnerPeopleID").val('');

                $("#txtOrgOwnerJobOccupationID").val('');
                $("#btnOrgOwner, #btnOrgOwnerJobOccupation").text('Please Select');//.button('refresh');
                $("#txtOrgOwnerDepartmentBusinessUnit").val('');

                $('#chkOrgOwnerIsIncorporator').attr("checked", false);//.checkboxradio("refresh");
                $('#chkOrgOwnerIsDirector').attr("checked", false);//.checkboxradio("refresh");
                $('#chkOrgOwnerIsStockHolder').attr("checked", false);//.checkboxradio("refresh");
                $('#chkOrgOwnerIsOfficer').attr("checked", false);//.checkboxradio("refresh");
                $('#chkOrgOwnerIsExecutiveCommittee').attr("checked", false);//.checkboxradio("refresh");
                $('#chkOrgOwnerIsOwner').attr("checked", false);//.checkboxradio("refresh");

                $("#btnOwnerRemove").hide();

            }

            function SaveOrgOwner() {
                var pData = [];

                pData = {
                    OrgOwnerID: $("#txtOrgOwnerID").val(),

                    PeopleID: $("#txtOrgOwnerPeopleID").val(),
                    JobOccupationID: $("#txtOrgOwnerJobOccupationID").val(),
                    BusinessUnit: $("#txtOrgOwnerDepartmentBusinessUnit").val(),

                    IsIncorporator: ($("#chkOrgOwnerIsIncorporator").is(":checked") ? "1" : "0"),
                    IsDirector: ($("#chkOrgOwnerIsDirector").is(":checked") ? "1" : "0"),
                    IsStockHolder: ($("#chkOrgOwnerIsStockHolder").is(":checked") ? "1" : "0"),
                    IsOfficer: ($("#chkOrgOwnerIsOfficer").is(":checked") ? "1" : "0"),
                    IsExecutiveCommittee: ($("#chkOrgOwnerIsExecutiveCommittee").is(":checked") ? "1" : "0"),
                    IsOwner: ($("#chkOrgOwnerIsOwner").is(":checked") ? "1" : "0")

                };

                AjaxSend({
                    url: "Organization/SaveOrgOwner",
                    data: pData,
                    success: function (vData) {

                        //ChangePage('/Organization/OrganizationOwner');

                        //                        window.location = MergeUrlParameters({
                        //                            OrgID: sessionStorage.OrgID,
                        //                            IsMobile: CBool(IsMobile) ? 1 : 0
                        //                        }, '/Organization/OrganizationOwner');
                        $('#pageOwnerList').show();
                        $('#pageOrgOwnerInfo').hide();
                        getOrgOwnerList();

                    }
                });

            };




        });
    });


