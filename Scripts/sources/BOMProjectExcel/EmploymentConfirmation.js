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
                window.location.href = '/bomprojectexcel';
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
                            if (vDataOrg.PeopleData.Rows.length > 0) {
                                sessionStorage.OrgID = vDataOrg.PeopleData.Rows[0].OrganizationID;
                                sessionStorage.OrgName = vDataOrg.PeopleData.Rows[0].CompanyName;
                                sessionStorage.PeopleID = vDataOrg.PeopleData.Rows[0].PeopleID;

                                location = MergeUrlParameters({
                                    OrgID: vDataOrg.PeopleData.Rows[0].OrganizationID,
                                    IsMobile: CBool(IsMobile) ? 1 : '0',
                                    mp: 'mc'
                                }, '/SetupChecklist/BOMProjectExcel');
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