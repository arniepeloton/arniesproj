require([
    'common'
    ], function () {

        $(function () {

            var mLoaded = false;

            $.mobile.activePage.bind('pageshow', PageShow);
            function PageShow() {
                mLoaded = true;
                getSecRoleUserList();
            };
            if (!mLoaded) PageShow();

            window.SecurityRoleUser = {
                UserList: function () {
                    getSecRoleUserList();
                }
            }

            $('body').on('click', '#btnSecRoleUserAddUser', function (e) {
                BrowsePopup('LibraryPeople', 'List of Employee',
                    [
                        {
                            DisplayText: [{ Column: 'PeopleName' }],
                            SelectMultiple: true,
                            AllowToSelectAll: true,
                            Message: 'You can select multiple records. Click OK when you\'re done.',
                            Click: function (e) {
                                for (var i = 0; i < e.length; i++) {
                                    SaveSecRoleUser(e[i]);
                                }                                
                            }
                        }
                    ], { EmployeesOnly: 1 }
                )
            })
            .on('keypress', '#txtSearchKeyword', function (e) {
                if (e.keyCode == 13) {
                    SearchEmployeePopUp($(this).val());
                }
            })
            .on('click', '#btnSecRoleUserDone', function (e) {
                e.preventDefault();

                //window.location = GetPathFormat("/SecurityRole/SecurityRoleSummary?OrgID=" + sessionStorage.OrgID);
                sessionStorage["AddViewLibBCName"] = "View Security Role";
                window.location.hash = '';
                window.location.hash = 'UniversalPage';                

                if(!CBool(sessionStorage.IsCrm)){
                    LibraryTable.ViewSecurityRolePage();
                } else {
                    CRMLibraryTable.ViewSecurityRolePage();
                }
            })

            function SearchEmployeePopUp(s) {
                var pData = {};

                pData.keywords = s;
                pData.EmployeesOnly = 1;

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
                                            SaveSecRoleUser($(this).data('data'));
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

            function getSecRoleUserList() {
                AjaxSend({
                    url: 'SecurityRole/getSecurityRoleUserList',
                    success: function (vData) {
                        LoadSecRoleUserList(vData.SecurityRoleData.Rows);
                    }
                });
            }

            function LoadSecRoleUserList(vRows) {
                var pLV = $('#lstSecRoleUser');
                var pItem;

                $("#divSecRoleUserRecordCount").text(" Number of record(s) found: " + vRows.length);

                pLV.html('');
                for (var i = 0; i <= vRows.length - 1; i++) {

                    pItem = $('<a href="#" class="list-group-item list-group-item-action">')
                                .append($('<table style="width:100%;">')
                                    .append($('<tr>')
                                        .append($('<td style="word-break:break-all;">')
                                            .attr('ID', vRows[i].ID)
                                            .append($('<h4>')
                                                    .text(vRows[i].PeopleName)
                                            )
                                        )
                                        .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                                .append($('<span class="bi-trash-fill" aria-hidden="true">'))
                                                .attr('ID', vRows[i].ID)
                                                .attr('href', "#")
                                                .click(function (e) {
                                                    e.preventDefault();

                                                    sessionStorage["SecRoleUserToRemove"] = $(this).attr('ID');

                                                    NewMessageBox('Remove this record?', 'Record', [
                                                        {
                                                            Text: 'Yes', Click: function () {
                                                                RemoveSecRoleUser(sessionStorage["SecRoleUserToRemove"]);
                                                                sessionStorage["SecRoleUserToRemove"] = null;
                                                            }
                                                        }, { Text: 'No' }
                                                    ])
                                                })
                                            )
                                        )
                                    )
                                )

                    pLV.append(pItem);

                }

                //pLV.listview('refresh');
            };

            function SaveSecRoleUser(data) {
                var pData = [];

                pData = {
                    PeopleID: data.ID
                };

                AjaxSend({
                    url: "SecurityRole/CheckUniqueSecurityRoleUser",
                    data: pData,
                    success: function (vData) {

                        if (vData.SecurityRoleData.Rows.length > 0) {
                            WebToast(data.PeopleName + ' has already been added.', 'top');
                        }
                        else {
                            AjaxSend({
                                url: "SecurityRole/SaveSecurityRoleUser",
                                data: pData,
                                success: function (vData) {
                                    getSecRoleUserList();
                                    //$('#panSearchLibrary').popup('close');
                                }
                            });
                        }
                    }
                });
            };

            function RemoveSecRoleUser(strID) {
                var pData = {};
                pData.SecRoleUserID = strID;

                AjaxSend({
                    url: "SecurityRole/RemoveSecurityRoleUser",
                    data: pData,
                    success: function (vData) {

                        getSecRoleUserList();

                        sessionStorage["PostSuffixToRemove"] = null;
                        //$('#panOrgEmpMessage').popup('close');

                    }
                });
            };

        });
    });