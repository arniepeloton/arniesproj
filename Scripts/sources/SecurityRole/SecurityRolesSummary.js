require([
    'common'
    ], function () {

        $(function () {

            var mLoaded = false;
            var SecurityRoleInfoPage = "";
            var SecurityRoleUserPage = "";

            $.mobile.activePage.bind('pageshow', PageShow);
            function PageShow() {
                mLoaded = true;

                if (!window.IsPageLoaded) {

                    AjaxSend({
                        url: 'SecurityRole/IsNewEntry',
                        success: function (vData) {

                            ShowSumSecurityRoleInfo(vData.SecurityRoleID || '')

                        }
                    });

                    window.IsPageLoaded = true;
                }
            };
            if (!mLoaded) PageShow();

            window.SecurityRoleSum = {
                ShowSumSecurityRoleInfo: ShowSumSecurityRoleInfo
            }

            $('#btnBackSecurityRolesSummary').click(function () {
                history.back();
            });

            $('body').on('click', '#btnEditSumSecRoleInfo', function (e) {
                e.preventDefault();

                window.IsPageLoaded = false;
                var secroleid = $('#txtSumSecurityRoleID').val();
                var containerDiv = (CBool(sessionStorage.IsCrm) ? $('#CRMUniversalDiv') : $('#UniversalDiv'));
                //window.open(GetPathFormat("SecurityRoleInfo"), "_self");
                if (SecurityRoleInfoPage == "") {
                    AjaxSend({
                        ExactUrl: '/SecurityRole/SecurityRoleInfo',
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            SecurityRoleInfoPage = vData;
                            containerDiv.html('');
                            containerDiv.append(vData);//.trigger('create');
                            //setTimeout(function () {
                            //    if ($('#txtRoleCode').val() == "") {
                            //        SecurityRole.ShowSecurityInfo();
                            //    }
                            //}, 2000)
                            //SecurityRole.ShowSecurityInfo();
                        }
                    });
                }
                else {
                    containerDiv.html('');
                    containerDiv.append(SecurityRoleInfoPage);//.trigger('create');
                    SecurityRole.ShowSecurityInfo();
                }
            })
            .on('click', '#btnEditSumSecRoleUser', function (e) {
                e.preventDefault();

                window.IsPageLoaded = false;
                var secroleid = $('#txtSumSecurityRoleID').val();
                var containerDiv = (CBool(sessionStorage.IsCrm) ? $('#CRMUniversalDiv') : $('#UniversalDiv'));
                //window.open(GetPathFormat("SecurityRoleInfo"), "_self");
                if (SecurityRoleUserPage == "") {
                    AjaxSend({
                        ExactUrl: '/SecurityRole/SecurityRoleUsers',
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            SecurityRoleUserPage = vData;
                            containerDiv.html('');
                            containerDiv.append(vData);//.trigger('create');
                        }
                    });
                }
                else {
                    containerDiv.html('');
                    containerDiv.append(SecurityRoleUserPage);//.trigger('create');
                    SecurityRoleUser.UserList();
                }

            })
            .on('click', '#btnSecRoleSummaryDone', function (e) {
                e.preventDefault();                
                window.location.hash = 'LibraryPage';
                if (CBool(sessionStorage.IsCrm)) {
                    CRMLibraryTable.SecurityRoles();
                } else {
                    $('#tblBody').html('');
                    LibraryTable.SecurityRoles();
                }                
            })

            function ShowSumSecurityRoleInfo(strID) {
                var pData = {};
                pData.SecurityRoleID = strID;

                ClearSumSecurityRoleFields();

                AjaxSend({
                    url: "SecurityRole/getSecurityRoleInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.SecurityRoleData.Rows.length > 0) {

                            $("#txtSumSecurityRoleID").val(vData.SecurityRoleData.Rows[0].ID || '');
                            $("#txtSumRoleCode").val(vData.SecurityRoleData.Rows[0].Code || '');
                            $("#txtSumRole").val(vData.SecurityRoleData.Rows[0].Role || '');
                            $("#txtSumRoleDescription").val(vData.SecurityRoleData.Rows[0].Description || '');
                            $('#chkSumRoleIsActive').attr("checked", (vData.SecurityRoleData.Rows[0].IsActive == 1 ? true : false));//.checkboxradio("refresh");

                        }

                        getSumSecRoleUserList();
                    }
                });

            };

            function ClearSumSecurityRoleFields() {

                $("#txtSumSecurityRoleID").val('');
                $("#txtSumRoleCode").val('');
                $("#txtSumRole").val('');
                $("#txtSumRoleDescription").val('');
                $('#chkSumRoleIsActive').attr("checked", true);//.checkboxradio("refresh");

            }

            function getSumSecRoleUserList() {
                AjaxSend({
                    url: 'SecurityRole/getSecurityRoleUserList',
                    success: function (vData) {
                        LoadSumSecRoleUserList(vData.SecurityRoleData.Rows);
                    }
                });
            }

            function LoadSumSecRoleUserList(vRows) {
                var pLV = $('#lstSumSecurityRoleUser');
                var pItem;

                pLV.html('');
                for (var i = 0; i <= vRows.length - 1; i++) {

                    pItem = $('<a href="#" class="list-group-item list-group-item-action">')
                            .attr('ID', vRows[i].ID)
                            .append($('<h4>')
                                .text(vRows[i].PeopleName)
                            );

                    pLV.append(pItem);

                }

                pLV;//.listview('refresh');
            };
        });
    });