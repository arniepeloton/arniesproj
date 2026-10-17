require([
    'common'
    ], function () {

        $(function () {

            $(document).ready(function () {

                AjaxSend({
                    url: 'CampaignManager/IsNewEntry',
                    success: function (vData) {

                        if (vData.IsNew == "FALSE") {
                            if (vData.PeopleID != null && vData.PeopleID != '') {

                                ShowPersonInfo(vData.PeopleID);
                                $("#txtPeopleID").val(vData.PeopleID);
                            }
                        }

                    }
                });

                GetOrganizationName();

                setMonthCollection('selStartDateMonth');
                //set year
                var d = new Date();
                $('#selStartDateYear').append(new Option(d.getFullYear() + 2, d.getFullYear() + 2));
                $('#selStartDateYear').append(new Option(d.getFullYear() + 1, d.getFullYear() + 1));
                $('#selStartDateYear').append(new Option(d.getFullYear(), d.getFullYear()));
            });

            //            $('div[data-role="dialog"]').live('pagebeforeshow', function (e, ui) {
            //                // make parent page transparent
            //                ui.prevPage.addClass("ui-dialog-background ");
            //            });

            //            $('div[data-role="dialog"]').live('pagehide', function (e, ui) {
            //                $(".ui-dialog-background ").removeClass("ui-dialog-background ");
            //            });

            //            $('#txtSearch').keypress(function (e) {
            //                if (e.keyCode == 13){
            //                    SearchDashboard();
            //                }
            //            });

            //CHANGE EVENTS
            $('#selStartDateDay').change(function () {

                $('#spanStartDate').hide();
                $('#selStartDateDay').removeClass('form-validation-error');
                $('#selStartDateMonth').removeClass('form-validation-error');
                $('#selStartDateYear').removeClass('form-validation-error');

                if ($.trim($(this).val()) == '') {
                    $('#spanStartDate').show();
                    $('#selStartDateDay').addClass('form-validation-error');
                }
            });


            $('#selStartDateMonth').change(function () {

                $('#spanStartDate').hide();
                $('#selStartDateDay').removeClass('form-validation-error');
                $('#selStartDateMonth').removeClass('form-validation-error');
                $('#selStartDateYear').removeClass('form-validation-error');

                if ($.trim($(this).val()) == '') {
                    $('#spanStartDate').show();
                    $('#selStartDateMonth').addClass('form-validation-error');
                }

                setDayCollection('selStartDateMonth', 'selStartDateDay', 'selStartDateYear', false);

            });

            $('#selStartDateYear').change(function () {
                $('#selStartDateDay').removeClass('form-validation-error');
                $('#selStartDateMonth').removeClass('form-validation-error');
                $('#selStartDateYear').removeClass('form-validation-error');

                $('#spanStartDate').hide();
                setDayCollection('selStartDateMonth', 'selStartDateDay', 'selStartDateYear', false);
                $('#selStartDateYear').removeClass('form-validation-error');
            });

            $('#selCampaignCode').change(function () {
                if ($.trim($(this).val()) == '') {
                    $('#spanCampaignCode').show();
                    $('#selCampaignCode').addClass('form-validation-error');

                } else {
                    $('#spanCampaignCode').hide();
                    $('#selCampaignCode').removeClass('form-validation-error');
                }

            });

            $('#txtCampaignName').change(function () {
                if ($.trim($(this).val()) == '') {
                    $('#spanCampaignName').show();
                    $('#txtCampaignName').addClass('form-validation-error');

                } else {
                    $('#spanCampaignName').hide();
                    $('#txtCampaignName').removeClass('form-validation-error');
                }

            });

            $('#selCampaignDuration').change(function () {
                if ($.trim($(this).val()) == '') {
                    $('#spanCampaignDuration').show();
                    $('#selCampaignDuration').addClass('form-validation-error');

                } else {
                    $('#spanCampaignDuration').hide();
                    $('#selCampaignDuration').removeClass('form-validation-error');
                }

            });



            //CLICK EVENTS
            $('#btnCreateCampaign').click(function (e) {
                e.preventDefault();
                validateFields();

                if ($.trim($('#txtCampaignName').val()) == '') {
                    return;
                }

                if ($.trim($('#selStartDateMonth').val()) == '') {
                    return;
                }

                if ($.trim($('#selStartDateDay').val()) == '') {
                    return;
                }

                if ($.trim($('#selStartDateYear').val()) == '') {
                    return;
                }

                if ($.trim($('#selCampaignDuration').val()) == '') {
                    return;
                }

                CreateNewCampaign();
            });

            $('#lnkSignOut').click(function () {
                SignOut();

            });

        }); //End Inner function


        function SignOut() {

            AjaxSend({
                url: "People/setSignOut",
                success: function (vData) {
                    window.open(GetPathFormat("/campaignmanager/signin?mode=logout"), "_self"); // window.open(GetPathFormat("/People"), "_self"); EDITED BY: JANINE , Aug 3 4pm
                }
            });

        };

        function validateFields() {

            if ($.trim($('#txtCampaignName').val()) == '') {
                $('#txtCampaignName').addClass('form-validation-error');
                $('#spanCampaignName').show();
            }

            if ($.trim($('#selStartDateMonth').val()) == '') {
                $('#selStartDateMonth').addClass('form-validation-error');
                $('#spanStartDate').show();
            }

            if ($.trim($('#selStartDateDay').val()) == '') {
                $('#selStartDateDay').addClass('form-validation-error');
                $('#spanStartDate').show();
            }

            if ($.trim($('#selStartDateYear').val()) == '') {
                $('#selStartDateYear').addClass('form-validation-error');
                $('#spanStartDate').show();
            }

            if ($.trim($('#selCampaignDuration').val()) == '') {
                $('#selCampaignDuration').addClass('form-validation-error');
                $('#spanCampaignDuration').show();
            }

        }

        function CreateNewCampaign() {

            var firstDay = new Date($("#selStartDateYear").val() + '/' + $("#selStartDateMonth").val() + '/' + $("#selStartDateDay").val());

            var days = 0;
            if ($.trim($('#selCampaignDuration').val()) != '')
                days = $('#selCampaignDuration').val() * 7; //7 days in a week. so that calls for 1 week

            var theDate = new Date(firstDay.getTime() + days * 24 * 60 * 60 * 1000);

            var endDate = (theDate.getMonth() + 1) + '/' + theDate.getDate() + '/' + theDate.getFullYear(); //strip off time. month starts at 0

            var pData = {};

            pData = {
                CampaignName: $.trim($("#txtCampaignName").val()),
                CampaignDescription: $.trim($("#txtCampaignDescription").val()),
                CampaignStartDate: $("#selStartDateMonth").val() + '/' + $("#selStartDateDay").val() + '/' + $("#selStartDateYear").val(),
                CampaignEndDate: endDate

            };

            AjaxSend({
                url: 'CampaignManager/CreateNewCampaign',
                data: pData,
                success: function (vData) {
                    //if successful
                    if (vData.CampaignData != '' || vData.CampaignData != 'Error' || vData.CampaignData != 'DateError') {
                        window.open(GetPathFormat('/campaignmanager'), '_self');
                    } else {
                        if (vData.CampaignData == 'DateError')
                            alert('Invalid Date');

                        if (vData.CampaignData == 'Error')
                            alert('There was an error in processing your data. Please contact Mobiz Support.');

                    }

                } // success
            }); // AjaxSend

        }

        function ShowPersonInfo(strID) {
            var pData = {};
            pData.ID = strID;

            AjaxSend({
                url: "SignIn/CheckPeopleIsFirstPartySignUp",
                data: pData,
                success: function (vData) {
                    if (vData.PeopleFirstPartySignUpData.Rows[0].IsFirstPartySignUp != '0') {

                        $('#lblName').text(vData.PeopleFirstPartySignUpData.Rows[0].FirstName);

                        $('#menuSignIn').hide();
                        $('#menuSignOut').show();
                        $('#lblMsg').hide();
                    } else {
                        pUrl = '/accountmanagement';
                        window.open(GetPathFormat(pUrl), '_self');

                    }
                }
            });

        }


        function SearchDashboard() {

            AjaxSend({
                url: "Dashboard/getSearchResult",
                data: { 'Keywords': $("#txtSearch").val() },
                success: function (vData) {
                    DisplayDashboard(vData.DashboardSearchResults.Rows)
                    $("#divRecordCount").text("No. of records: " + vData.DashboardSearchResults.Rows.length);
                }
            });

        }

        function GetOrganizationName() {

            AjaxSend({
                url: 'CampaignManager/getOrganization',
                success: function (vData) {
                    if (vData.CompanyName != '') {
                        $('#hdrCompanyName').text(vData.CompanyName);
                        GetListOfCampaigns();
                    }
                    //                    else {
                    //                        alert('Access Denied. You do not belong to any organization.');
                    //                        pUrl = '/accountmanagement/signin';
                    //                        window.open(GetPathFormat(pUrl), '_self');
                    //                    }
                }
            });
        }

        function GetListOfCampaigns() {

            AjaxSend({
                url: 'CampaignManager/getListOfCampaigns',
                success: function (vData) {
                    if (vData.DashboardList != '')
                        DisplayDashboard(vData.DashboardList.Rows);

                }
            });
        }

        function DisplayDashboard(vRows) {
            var pLV = $('#tblDashboardBody');

            pLV.html('');

            for (var i = 0; i <= vRows.length - 1; i++) {
                pItem = $('<tr>')
                            .append($('<td>')
                                .text(vRows[i].CampaignCode)
                            )
                            .append($('<td>')
                                .text(vRows[i].CampaignName)
                            )
                            .append($('<td>')
                                .text(vRows[i].CampaignStartDate.replace('12:00:00 AM', '')) //remove time
                            )
                            .append($('<td>')
                                .text(vRows[i].CampaignEndDate.replace('12:00:00 AM', '')) //remove time
                            )
                            .append($('<td>')
                                .text(vRows[i].LastUpdated.replace('12:00:00 AM', '')) //remove time
                            )
                            .append($('<td>')
                                .append($('<a href="/campaignmanager/couponlist?id=' + vRows[i].ID + '" id="' + vRows[i].ID + '">')
                                    .text('Manage')
                //                                    .click(function () {
                //                                        DisplayDetails($(this).attr('id'));
                //                                    })
                                )
                            )
                pLV.append(pItem)
            }
            //            $('#tblDashboard').table('refresh');
        }


    });


