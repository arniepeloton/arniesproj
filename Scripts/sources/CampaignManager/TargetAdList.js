require([
    'common'
    ], function () {

        $(function () {
            GetCampaignDetails();

        }); //End Inner function

        //functions

        function querySt(Key) {
            var url = window.location.href;
            KeysValues = url.split(/[\?&]+/);
            for (i = 0; i < KeysValues.length; i++) {
                KeyValue = KeysValues[i].split("=");
                if (KeyValue[0] == Key) {
                    return KeyValue[1];
                }
            }
        }


        function GetCampaignDetails() {
            var cId = '';

            if (querySt('id')) {
                if (querySt('id') != '') {
                    cId = querySt('id');
                    var pData = {};
                    pData.CampaignID = cId;

                    AjaxSend({
                        url: 'CampaignManager/getCampaignByID',
                        data: pData,
                        success: function (vData) {
                            if (vData.CampaignData.Rows.length > 0) {
                                $('#lblCampaignName').text(vData.CampaignData.Rows[0].CampaignName);
                                $('#lblCampaignCode').text(vData.CampaignData.Rows[0].CampaignCode);
                                $('#lblStartDate').text(vData.CampaignData.Rows[0].CampaignStartDate.replace('12:00:00 AM', ''));
                                $('#lblEndDate').text(vData.CampaignData.Rows[0].CampaignEndDate.replace('12:00:00 AM', ''));
                                //                        $('#lblCampaignDuration').text(vData.CampaignData.Rows[0].CampaignType);
                                $('#lblLastUpdated').text(vData.CampaignData.Rows[0].LastUpdated.replace('12:00:00 AM', ''));

                                GetListOfTargetedAds(cId);
//                                SetStartDate(vData.CampaignData.Rows[0].CampaignStartDate);

                                $('#btnNewTargetedAd').click(function (e) {
                                    e.preventDefault();
      
                                    window.open(GetPathFormat('/campaignmanager/coupon?id=' + cId), '_self');
                                });

                            }
                        }
                    });
                } else {
                    alert('Invalid CampaignID');
                }
            }


        }


        function GetListOfTargetedAds(campaignID) {

            var pData = {};
            pData.CampaignID = campaignID;

            AjaxSend({
                url: 'CampaignManager/getListOfTargetedAds',
                data: pData,
                success: function (vData) {
                    if (vData.DashboardList != '')
                        DisplayDashboard(vData.DashboardList.Rows);

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


        function DisplayDetails(id) {
            var data = { ID: id }

            AjaxSend({
                url: 'Dashboard/getDetails',
                data: data,
                success: function (vData) {
                    //                    var prefix = vData.Details.Rows[0].Prefix;
                    //                    var areaCode = '(' + vData.Details.Rows[0].AreaCode + ') ';
                    //                    var countryCodeTel = '(' + vData.Details.Rows[0].CallingCode + ') ';
                    //                    var countryCodeMobile = '+' + vData.Details.Rows[0].CountryCode;
                    //                    var productName = '';

                    //                    $('#lblBusinessName').text(vData.Details.Rows[0].BusinessName);
                    //                    $('#lblBusinessIndustry').text(vData.Details.Rows[0].BusinessIndustry);
                    //                    $('#lblCountry').text(vData.Details.Rows[0].Country);
                    //                    $('#lblCity').text(vData.Details.Rows[0].City);
                    //                    $('#lblContactPerson').text(vData.Details.Rows[0].LastName + ', ' + vData.Details.Rows[0].FirstName + ' ' + vData.Details.Rows[0].MiddleName);


                    //                    $('#lblTel').text(countryCodeTel + areaCode + vData.Details.Rows[0].TelephoneNo);

                    //                    if (vData.Details.Rows[0].MobileNo == '' || vData.Details.Rows[0].MobileNo == null) {
                    //                        countryCodeMobile = '';
                    //                        prefix = '';
                    //                    } else {
                    //                        if (vData.Details.Rows[0].CountryCode == '' || vData.Details.Rows[0].CountryCode == null) {
                    //                            countryCodeMobile = '+--';
                    //                            prefix = '--';
                    //                        }
                    //                    }

                    //                    $('#lblMobile').text(countryCodeMobile + prefix + vData.Details.Rows[0].MobileNo);

                    //                    $('#lblEmail').text(vData.Details.Rows[0].EmailAddress);
                    //                    $('#lblProduct').text(productName);
                    //                    $('#lblDate').text(vData.Details.Rows[0].LastUpdated);

                }
            });
        }


        function DisplayDashboard(vRows) {
            var pLV = $('#tblDashboardBody');

            pLV.html('');

            for (var i = 0; i <= vRows.length - 1; i++) {
                pItem = $('<tr>')
                            .append($('<td>')
                                .text(vRows[i].AdName)
                            )
                            .append($('<td>')
                                .text(vRows[i].AdType)
                            )
                            .append($('<td>')
                                .text(vRows[i].AgeFrom + ' - ' + vRows[i].AgeTo)
                            )
                            .append($('<td>')
                                .text(vRows[i].Gender)
                            )
                            .append($('<td>')
                                .text(vRows[i].CityName)
                            )
                            .append($('<td>')
                                .text(vRows[i].LastUpdated.replace('12:00:00 AM', '')) //remove time
                            )
                            .append($('<td>')
                                .append($('<a href="#">') //.append($('<a href="/campaignmanager/coupon?id=' + vRows[i].ID + '" id="' + vRows[i].ID + '">')
                                    .text('View Coupons')
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


