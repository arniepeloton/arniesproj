require([
    'common'
    ], function () {

        $(function () {

            $(document).ready(function () {

                ShowUserAccountDetails();

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

            $('#btnPeopleUserAccountPopUpMenu').click(function (e) {
                e.preventDefault();

                $("#divPeopleUserAccountPopUpMenu").popup("open")

            });

            $('#popPeopleUserAccountPeopleInfoName').click(function (e) {
                e.preventDefault();

                window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoName')), "_self");

            });

            $('#popPeopleUserAccountPeopleInfoAlias').click(function (e) {
                e.preventDefault();

                window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoAlias')), "_self");

            });

            $('#popPeopleUserAccountPeopleInfoAddress').click(function (e) {
                e.preventDefault();

                window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoAddress')), "_self");

            });

            $('#popPeopleUserAccountPeoplePersonalInfo').click(function (e) {
                e.preventDefault();

                window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeoplePersonalInfo')), "_self");

            });

            $('#popPeopleUserAccountPeopleContact').click(function (e) {
                e.preventDefault();

                window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleContactNo')), "_self");

            });

            $('#popPeopleUserAccountPeopleEducation').click(function (e) {
                e.preventDefault();

                window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleEducation')), "_self");

            });

            $('#popPeopleUserAccountPeopleEmpHistory').click(function (e) {
                e.preventDefault();

                window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleEmpHistory')), "_self");

            });

            $('#popPeopleUserAccountPeopleRelatedPerson').click(function (e) {
                e.preventDefault();

                window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleRelatedPerson')), "_self");

            });

            $('#popPeopleUserAccountPeoplePrimeIDReference').click(function (e) {
                e.preventDefault();

                window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeoplePrimeIDReference')), "_self");

            });

            $('#btnPeopleNameDone').click(function (e) {
                e.preventDefault();

                if ($.trim($("#txtPIN1").val()) == "") {
                    MessageBox("Please enter PIN.",
                                "PIN",
                                [
                                    { Text: 'Ok' }
                                ]
                    );
                    return;
                }

                if ($.trim($("#txtPIN2").val()) == "") {
                    MessageBox("Please re-enter PIN.",
                                "PIN",
                                [
                                    { Text: 'Ok' }
                                ]
                    );
                    return;
                }


                if ($.trim($("#txtPIN1").val()) != $.trim($("#txtPIN2").val())) {
                    MessageBox("PIN mismatched. Please re-enter PIN.",
                            "PIN",
                            [
                                { Text: 'Ok' }
                            ]
                );
                    return;
                }

                SavePersonPIN();

            });

            $('#btnUserAccountCancel').click(function (e) {
                e.preventDefault();

                history.back();

            });

            function ShowUserAccountDetails() {
                GetPeopleID();
            };

            function GetPeopleID() {

                AjaxSend({
                    url: "People/getPeopleSecurityQuestionByPeopleIDList",
                    success: function (vData) {
                        if (vData.PeopleSecurityData.Rows.length > 0) {
                            $("#txtID").val(vData.PeopleSecurityData.Rows[0].PeopleID);
                        }
                    }
                });

            }
            
            function validateAlphanumeric(string) {
                var re = /^[a-zA-Z0-9\_]+$/;

                return re.test(string);
            }

            function SavePersonPIN() {

                var pData = {};
                pData.PeopleID = $.trim($("#txtID").val());
                pData.PIN = $("#txtPIN1").val();

                AjaxSend({
                    url: "People/SavePeopleDTRPIN",
                    data: pData,
                    success: function (vData) {

                        window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoSummary')), "_self");

                    }
                });

            };
        });
    });