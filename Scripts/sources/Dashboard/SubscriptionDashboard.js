require([
    'common'
    ], function () {

        $(function () {
            //GetListOfSubscribers();

            $('div[data-role="dialog"]').live('pagebeforeshow', function (e, ui) {
                // make parent page transparent
                ui.prevPage.addClass("ui-dialog-background ");
            });

            $('div[data-role="dialog"]').live('pagehide', function (e, ui) {
                $(".ui-dialog-background ").removeClass("ui-dialog-background ");
            });

            $('#txtSearch').keypress(function (e) {
                if (e.keyCode == 13) {
                    SearchDashboard();
                }
            });

        }); //End Inner function

        function SearchDashboard() {

            AjaxSend({
                url: "Dashboard/getSearchResult",
                data: { 'Keywords': $("#txtSearch").val() },
                success: function (vData) {
                    if (CStr2(vData.Error).length > 0) {
                        alert(vData.Error);
                    } else {
                        DisplayDashboard(vData.DashboardSearchResults.Rows)
                        $("#divRecordCount").text("No. of records: " + vData.DashboardSearchResults.Rows.length);
                    }

                }
            });

        }

        function GetListOfSubscribers() {

            AjaxSend({
                url: 'Dashboard/getListOfSubscribers',
                success: function (vData) {
                    if (CStr2(vData.Error).length > 0) {
                        alert(vData.Error);
                    } else {
                        DisplayDashboard(vData.DashboardList.Rows);
                    }
                }
            });
        }

        function DisplayDetails(id) {
            var data = { ID: id }

            AjaxSend({
                url: 'Dashboard/getDetails',
                data: data,
                success: function (vData) {
                    if (CStr2(vData.Error).length > 0) {
                        alert(vData.Error);
                    } else {

                        var pLV = $('#tblDashboardBodyDetails');

                        pLV.html('');

                        for (var i = 0; i <= vData.Details.Rows.length - 1; i++) {

                            $('#hdrCompanyName').text(vData.Details.Rows[i].CompanyName);
                            $('#lblOfficeAddress').text(vData.Details.Rows[i].OfficeAddressBuildingNo + '  ' + vData.Details.Rows[i].OfficeAddressStreet + ', ' + vData.Details.Rows[i].OfficeCityName);
                            $('#lblBusinessAddress').text(vData.Details.Rows[i].BusinessAddressBuildingNo + '  ' + vData.Details.Rows[i].BusinessAddressStreet + ', ' + vData.Details.Rows[i].BusinessCityName);
                            $('#lblPromoCode').text(vData.Details.Rows[i].PromoCode);
                            $('#lblAgentName').text(vData.Details.Rows[i].AgentName);
                            $('#lblSubscriptionDate').text(vData.Details.Rows[i].SubscriptionDate);

                            pItem = $('<tr>')
                                            .append($('<th>')
                                                .text(vData.Details.Rows[i].ProductName)
                                            )
                                            .append($('<td>')
                                                .text(vData.Details.Rows[i].Qty)
                                            )
                                            .append($('<td>')
                                                .text(vData.Details.Rows[i].SubscriptionStatus)
                                            )
                                            .append($('<td>')
                                                .text(vData.Details.Rows[i].ActivationDate)
                                            )
                            pLV.append(pItem)
                        } //end for
                        //                        $('#tblDashboardDetails').table('refresh');
                        $.mobile.changePage('#dialog', 'pop', true, true);

                    } //end if
                } //end success
            });
        }


        function DisplayDashboard(vRows) {
            var pLV = $('#tblDashboardBody');

            pLV.html('');

            for (var i = 0; i <= vRows.length - 1; i++) {
                var emailVerification = 'Not Verified';

                if (vRows[i].EmailVerification == '1') {
                    emailVerification = 'Verified';
                }

                var title;

                if (vRows[i].Gender == 'Male')
                    title = 'Mr.';
                else if (vRows[i].Gender == 'Female')
                    title = 'Ms.';
                else
                    title = '';

                var prefix = vRows[i].Prefix;
                var countryCodeMobile = '+' + vRows[i].CountryCode;

                if (vRows[i].MobileNo == '' || vRows[i].MobileNo == null) {
                    countryCodeMobile = '';
                    prefix = '';
                } else {
                    if (vRows[i].CountryCode == '' || vRows[i].CountryCode == null) {
                        countryCodeMobile = '+--';
                        prefix = '--';
                    }
                }

                pItem = $('<tr>')
                            .append($('<th>')
                                .text(vRows[i].SubscriptionNo)
                            )
                            .append($('<td>')
                                .text(title + ' ' + vRows[i].LastName + ', ' + vRows[i].FirstName + ' ' + vRows[i].MiddleName + ' ' + vRows[i].PersonalSuffixName)
                            )
                            .append($('<td>')
                                .text(vRows[i].EmailAddress)
                            )
                            .append($('<td>')
                                .text(countryCodeMobile + prefix + vRows[i].MobileNo)
                            )
                            .append($('<td>')
                                .text(vRows[i].CompanyName)
                            )
                            .append($('<td>')
                                .text(vRows[i].CityName)
                            )
                            .append($('<td>')
                                .text(vRows[i].Class)
                            )
                            .append($('<td>')
                                .text(vRows[i].SourceOfInfo)
                            )
                            .append($('<td>')
                                .text(emailVerification)
                            )
                            .append($('<td>')
                                .text(vRows[i].LastUpdated)
                            )
                            .append($('<td>')
                                .append($('<a href="" id="' + vRows[i].ID + '">')
                                    .text('Show Subscriptions')
                                    .click(function () {
                                        DisplayDetails($(this).attr('id'));
                                    })
                                )
                            )
                pLV.append(pItem)
            }
            $('#tblDashboard').table('refresh');
        }


    });


