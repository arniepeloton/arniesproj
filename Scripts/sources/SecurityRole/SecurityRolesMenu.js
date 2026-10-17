require([
    'common'
    ], function () {

        $(function () {

            ShowSecurityRoleList();

            //Navigation Menu
            $('#btnHome').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu"), "_self");
            });

            $('#btnSystemAdmin').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu#pagePopUpMenuSettings"), "_self");
            });

            $('#btnSystemSettings').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu#pagePopSystemSettings"), "_self");
            });

            $('#btnSecuritySettings').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu#pagePopUpMenuSecuritySettings"), "_self");
            });
            //End Navigation Menu

            $('#frmSecurityRoleBrowser').submit(function (e) {
                e.preventDefault();

                ShowSecurityRoleList();


            });

            $('#btnAddNewSecurityRole').click(function () {

                CreateNewSecurityRoleEntry(1, '');

            });

            $('#btnSecurityRoleBackToHome').click(function () {

                window.open(GetPathFormat("/People/PeopleMenu"), "_self");

            });

            $('#btnSecurityRoleGoToSetupChecklist').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/SetupChecklist"), "_self");

            });

            btnSecurityRoleGoToSetupChecklist

            function ShowSecurityRoleList() {

                var pData = {};
                pData.SearchString = $("#txtSecurityRoleSearch").val();

                AjaxSend({
                    url: 'SecurityRole/getSecurityRoleList',
                    data: pData,
                    success: function (vData) {
                        LoadSecurityRoleList(vData.SecurityRoleData.Rows);
                    }
                });

            }

            function LoadSecurityRoleList(vRows) {
                var pLV = $('#lstSecurityRole');
                var pItem;

                $("#divSecurityRoleSearchResults").hide();
                $("#divSecurityRoleSearchNoResults").hide();
//                $("#btnAddNewSecurityRole").hide();

                if (vRows.length > 0) {
                    $("#divSecurityRoleSearchResults").show();
                    $("#divSecurityRoleRecordCount").text(" Number of record(s) found: " + vRows.length);

                    pLV.html('');
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        pItem = $('<li>')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();

                                CreateNewSecurityRoleEntry(0, $(this).attr('ID'));

                            })
                            .append($('<h3>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].Code + ' - ' + vRows[p].Role)
                            )
                            .append($('<p>')
                                .attr("style", "white-space:normal")
                                .text('Description : ' + vRows[p].Description)
                            )
                            .append($('<p>')
                                .attr("style", "white-space:normal")
                                .text('Status : ' + (vRows[p].IsActive == 1 ? "Active" : "Inactive"))
                            )
                        );
                        pLV.append(pItem);
                    };

                    pLV.listview('refresh');
                }
                else {
                    $("#divSecurityRoleSearchNoResults").show();
                    $("#btnAddNewSecurityRole").show();
                    $("#lblSecurityRoleNotFound").text('The role "' + $("#txtSecurityRoleSearch").val() + '" does not exist. Please consider to search thoroughly using an alternate spelling before creating a new role to avoid duplication.');
                }
            };

            function CreateNewSecurityRoleEntry(IsNew, strID) {
                var pData = {};
                pData.IsNew = IsNew;
                pData.ID = strID;

                window.pageOrig = $.mobile.activePage;

                AjaxSend({
                    url: 'SecurityRole/setCreateNew',
                    data: pData,
                    success: function (vData) {

                        window.IsPageLoaded = false;
                        if ((vData.IsNew || '') == "TRUE") {
                            PromptUserAccess('Security Roles', {
                                OnClickOKSuccess: function () {
                                    location = MergeUrlParameters({
                                        OrgID: sessionStorage.OrgID,
                                        IsMobile: CBool(IsMobile) ? 1 : 0
                                    }, '/SecurityRole/SecurityRoleInfo');
                                }
                            });
                        }
                        else {
                            window.open(GetPathFormat("/SecurityRole/SecurityRoleSummary"), "_self");
                        }


//                        if ((vData.IsNew || '') == "TRUE") {
//                            ChangePage('/SecurityRole/SecurityRoleInfo');
//                        }
//                        else {
//                            ChangePage('/SecurityRole/SecurityRoleSummary');
//                        }

                    }
                });

            }





        });
    });