require([
    'common'
    ], function () {
        $(document).ready(function () {
            var arrMsg = new Array();
            arrMsg = JSON.parse(sessionStorage.Message);

            $('#txtPeopleMsgT1ID').val(arrMsg.ID);
            $('#lblPeopleMsgT1Subject').text(arrMsg.Subject);
            $('#lblPeopleMsgT1From').text(arrMsg.SenderName);
            $('#lblPeopleMsgT1DateTime').text($.format.date(new Date(arrMsg.MessageDateTime), 'MMM dd, yyyy hh:mm a'));
            $('#lblPeopleMsgT1').text(arrMsg.Message);

            $('#btnPeopleMsgT1Confirm').click(function () {
                var pData = {};
                pData.MsgID = $("#txtPeopleMsgT1ID").val();

                //Get Messages Details
                AjaxSend({
                    url: "People/ConfirmPeopleMessageT1Details",
                    data: pData,
                    success: function (vDataMsgDetails) {
                        GetPeopleOrg();
                    }
                });
            })

            $('#btnPeopleMsgT1DenyX').click(function () {
                alert("Not yet implemented.");
            })

            $('#btnPeopleMsgT1Cancel').click(function () {
                if (sessionStorage.system == 'eas') {
                    window.location.href = '/signin/erp';
                } else {
                    window.location.href = '/signin/crm';
                }                
            })

            function GetPeopleOrg() {

                $('#divOrgApps').html('');

                if (OfflineMode) {
                    vData = PeopleMenu_offline().getPeopleOrganizationList();
                    DisplayPeopleMenu(vData, true); //Load First Org with DBase
                    DisplayPeopleMenu(vData, false);
                }
                else {
                    AjaxSend({
                        url: "People/getPeopleOrganizationList",
                        success: function (vDataOrg) {
                            if (vDataOrg.PeopleData.Rows.length > 1) {
                                var a = [];

                                for (var i = 0; i < vDataOrg.PeopleData.Rows.length; i++) {
                                    a.push({
                                        data: vDataOrg.PeopleData.Rows[i],
                                        Text: vDataOrg.PeopleData.Rows[i].CompanyName,
                                        Click: function (e) {
                                            sessionStorage.OrgID = e.OrganizationID;
                                            sessionStorage.OrgName = e.CompanyName;
                                            sessionStorage.PeopleID = e.PeopleID;

                                            if (sessionStorage.system == 'eas') {
                                                location = MergeUrlParameters({
                                                    OrgID: e.OrganizationID
                                                }, '/erp');
                                            } else {
                                                location = MergeUrlParameters({
                                                    OrgID: e.OrganizationID,
                                                    IsMobile: CBool(IsMobile) ? 1 : '0'
                                                }, '/CRMDashboard');
                                            }
                                        }
                                    })
                                }

                                PopupButtonList('', 'Select Organization', a);
                            }
                            else if (vDataOrg.PeopleData.Rows.length == 1) {
                                sessionStorage.OrgID = vDataOrg.PeopleData.Rows[0].OrganizationID;
                                sessionStorage.OrgName = vDataOrg.PeopleData.Rows[0].CompanyName;
                                sessionStorage.PeopleID = vDataOrg.PeopleData.Rows[0].PeopleID;

                                if (sessionStorage.system == 'eas') {
                                    location = MergeUrlParameters({
                                        OrgID: vDataOrg.PeopleData.Rows[0].OrganizationID
                                    }, '/erp');
                                } else {
                                    location = MergeUrlParameters({
                                        OrgID: vDataOrg.PeopleData.Rows[0].OrganizationID,
                                        IsMobile: CBool(IsMobile) ? 1 : '0'
                                    }, '/CRMDashboard');
                                }                                
                            }
                            else {
                                alert('Somethings went wrong');
                            }
                        }
                    });
                }
            }
        })
    })