require([
    'common'
    ], function () {

        $(function () {
            GetCampaignDetails();
            getCityName();

            setMonthCollection('selStartDateMonth');
            //set year
            var d = new Date();
            $('#selStartDateYear').append(new Option(d.getFullYear() + 2, d.getFullYear() + 2));
            $('#selStartDateYear').append(new Option(d.getFullYear() + 1, d.getFullYear() + 1));
            $('#selStartDateYear').append(new Option(d.getFullYear(), d.getFullYear()));

            setMonthCollection('selEndDateMonth');
            $('#selEndDateYear').append(new Option(d.getFullYear() + 2, d.getFullYear() + 2));
            $('#selEndDateYear').append(new Option(d.getFullYear() + 1, d.getFullYear() + 1));
            $('#selEndDateYear').append(new Option(d.getFullYear(), d.getFullYear()));


            //CHANGE EVENTS
            $('#rbPromotionalCampaign').change(function () {

                if ($.trim($('input[name="AdType"]:checked').val()) == 'Coupon') {
                    $('.coupon').show();
                } else {
                    $('.coupon').hide();
                }
            });
            $('#rbCoupon').change(function () {

                if ($.trim($('input[name="AdType"]:checked').val()) == 'Coupon') {
                    $('.coupon').show();
                } else {
                    $('.coupon').hide();
                }
            });

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
            }).click(function () {
                if ($.trim($('#selStartDateYear').val()) == "") {
                    $('#selStartDateYear').val('1980');
                    //                $('#selDOBYear').selectmenu('refresh');
                }

                $('#selStartDateYear').removeClass('form-validation-error');
            });

            $('#selEndDateDay').change(function () {

                $('#spanEndDate').hide();
                $('#selEndDateDay').removeClass('form-validation-error');
                $('#selEndDateMonth').removeClass('form-validation-error');
                $('#selEndDateYear').removeClass('form-validation-error');

                if ($.trim($(this).val()) == '') {
                    $('#spanEndDate').show();
                    $('#selEndDateDay').addClass('form-validation-error');
                }
            });


            $('#selEndDateMonth').change(function () {

                $('#spanEndDate').hide();
                $('#selEndDateDay').removeClass('form-validation-error');
                $('#selEndDateMonth').removeClass('form-validation-error');
                $('#selEndDateYear').removeClass('form-validation-error');

                if ($.trim($(this).val()) == '') {
                    $('#spanEndDate').show();
                    $('#selEndDateMonth').addClass('form-validation-error');
                }

                setDayCollection('selEndDateMonth', 'selEndDateDay', 'selEndDateYear', false);

            });
            $('#selEndDateYear').change(function () {
                $('#selEndDateDay').removeClass('form-validation-error');
                $('#selEndDateMonth').removeClass('form-validation-error');
                $('#selEndDateYear').removeClass('form-validation-error');

                $('#spanEndDate').hide();
                setDayCollection('selEndDateMonth', 'selEndDateDay', 'selEndDateYear', false);
                $('#selEndDateYear').removeClass('form-validation-error');
            }).click(function () {
                if ($.trim($('#selEndDateYear').val()) == "") {
                    $('#selEndDateYear').val('1980');
                    //                $('#selDOBYear').selectmenu('refresh');
                }

                $('#selEndDateYear').removeClass('form-validation-error');
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

            $('#txtAdName').change(function () {
                if ($.trim($(this).val()) == '') {
                    $('#spanAdName').show();
                    $('#txtAdName').addClass('form-validation-error');

                } else {
                    $('#spanAdName').hide();
                    $('#txtAdName').removeClass('form-validation-error');
                }

            });

            $('#txtAdText').change(function () {
                if ($.trim($(this).val()) == '') {
                    $('#spanAdText').show();
                    $('#txtAdText').addClass('form-validation-error');

                } else {
                    $('#spanAdText').hide();
                    $('#txtAdText').removeClass('form-validation-error');
                }

            });


            $('#selAdDuration').change(function () {
                if ($.trim($(this).val()) == '') {
                    $('#spanAdDuration').show();
                    $('#selAdDuration').addClass('form-validation-error');

                } else {

                    $('#spanAdDuration').hide();
                    $('#selAdDuration').removeClass('form-validation-error');

                    if ($.trim($(this).val()) == 'fixed') {
                        $('.fixed').show();
                    } else {
                        $('.fixed').hide();
                    }


                }

            });

            $('#txtAgeFrom').change(function () {
                $('#spanAge').hide();
                $(this).removeClass('form-validation-error');

                if ($.trim($(this).val()) == '') {
                    $('#spanAge').show();
                    $(this).addClass('form-validation-error');

                } else {

                    if (!validateAge($.trim($(this).val()))) {
                        $('#spanAge').show();
                        $(this).addClass('form-validation-error');
                    }

                }

            });

            $('#txtAgeTo').change(function () {
                $('#spanAge').hide();
                $(this).removeClass('form-validation-error');

                if ($.trim($(this).val()) == '') {
                    $('#spanAge').show();
                    $(this).addClass('form-validation-error');

                } else {

                    if (!validateAge($.trim($(this).val()))) {
                        $('#spanAge').show();
                        $(this).addClass('form-validation-error');
                    }

                }

            });

            $('#selGender').change(function () {
                if ($.trim($(this).val()) == '') {
                    $('#spanGender').show();
                    $(this).addClass('form-validation-error');

                } else {
                    $('#spanGender').hide();
                    $(this).removeClass('form-validation-error');
                }

            });

            $('#selCity').change(function () {
                if ($.trim($(this).val()) == '') {
                    $('#spanCity').show();
                    $(this).addClass('form-validation-error');

                } else {
                    $('#spanCity').hide();
                    $(this).removeClass('form-validation-error');
                }

            });

            //CLICK EVENTS
            $('#btnCreateAd').click(function (e) {
                e.preventDefault();
                validateFields();

                if ($.trim($('#txtAdName').val()) == '') {
                    return;
                }
                if ($.trim($('#txtAdText').val()) == '') {
                    return;
                }


                if ($.trim($('input[name="AdType"]:checked').val()) == 'Coupon') {
                    if ($.trim($('#selCouponCodeLength').val()) == '') {
                        return;
                    }

                    if ($.trim($('#selCouponCodeType').val()) == '') {
                        return;
                    }

                    if ($.trim($('#txtNoOfCoupons').val()) == '') {
                        return;
                    }

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

                if ($.trim($('#selAdDuration').val()) == '') {
                    return;
                } else {
                    if ($.trim($('#selAdDuration').val()) == 'fixed') {
                        if ($.trim($('#selEndDateMonth').val()) == '') {
                            return;
                        }

                        if ($.trim($('#selEndDateDay').val()) == '') {
                            return;
                        }

                        if ($.trim($('#selEndDateYear').val()) == '') {
                            return;
                        }
                    }
                }

                if ($.trim($('#txtAgeFrom').val()) == '') {
                    return;
                } else {
                    if (!validateAge($.trim($('#txtAgeFrom').val()))) {
                        return;
                    }
                }

                if ($.trim($('#txtAgeTo').val()) == '') {
                    return;
                } else {
                    if (!validateAge($.trim($('#txtAgeTo').val()))) {
                        return;
                    }
                }

                if ($.trim($('#selGender').val()) == '') {
                    return;
                }
                if ($.trim($('#selCity').val()) == '') {
                    return;
                }
                alert('ok');
                //                CreateNewTargetedAd();
            });

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


        function validateAge(number) {
            var re = /^[0-9]+$/;

            return re.test(number);
        }

        function validateFields() {

            if ($.trim($('#txtAdName').val()) == '') {
                $('#txtAdName').addClass('form-validation-error');
                $('#spanAdName').show();
            }
            if ($.trim($('#txtAdText').val()) == '') {
                $('#txtAdText').addClass('form-validation-error');
                $('#spanAdText').show();
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

            if ($.trim($('#selAdDuration').val()) == '') {
                $('#selAdDuration').addClass('form-validation-error');
                $('#spanAdDuration').show();
            } else {
                if ($.trim($('#selAdDuration').val()) == 'fixed') {
                    if ($.trim($('#selEndDateMonth').val()) == '') {
                        $('#selEndDateMonth').addClass('form-validation-error');
                        $('#spanEndDate').show();
                    }

                    if ($.trim($('#selEndDateDay').val()) == '') {
                        $('#selEndDateDay').addClass('form-validation-error');
                        $('#spanEndDate').show();
                    }

                    if ($.trim($('#selEndDateYear').val()) == '') {
                        $('#selEndDateYear').addClass('form-validation-error');
                        $('#spanEndDate').show();
                    }
                }
            }

            if ($.trim($('input[name="AdType"]:checked').val()) == 'Coupon') {

                if ($.trim($('#selCouponCodeLength').val()) == '') {
                    $('#selCouponCodeLength').addClass('form-validation-error');
                    $('#spanCouponCodeLength').show();
                }

                if ($.trim($('#selCouponCodeType').val()) == '') {
                    $('#selCouponCodeType').addClass('form-validation-error');
                    $('#spanCouponCodeType').show();
                }

                if ($.trim($('#txtNoOfCoupons').val()) == '') {
                    $('#txtNoOfCoupons').addClass('form-validation-error');
                    $('#spanNoOfCoupons').show();
                }
            }

            if ($.trim($('#txtAgeFrom').val()) == '') {
                $('#txtAgeFrom').addClass('form-validation-error');
                $('#spanAge').show();
            } else {
                if (!validateAge($.trim($('#txtAgeTo').val()))) {
                    $('#txtAgeFrom').addClass('form-validation-error');
                    $('#spanAge').show();
                }
            }

            if ($.trim($('#txtAgeTo').val()) == '') {
                $('#txtAgeTo').addClass('form-validation-error');
                $('#spanAge').show();
            } else {
                if (!validateAge($.trim($('#txtAgeTo').val()))) {
                    $('#txtAgeTo').addClass('form-validation-error');
                    $('#spanAge').show();
                }
            }

            if ($.trim($('#selGender').val()) == '') {
                $('#selGender').addClass('form-validation-error');
                $('#spanGender').show();
            }
            if ($.trim($('#selCity').val()) == '') {
                $('#selCity').addClass('form-validation-error');
                $('#spanCity').show();
            }

        }

        function getCityName() {

            AjaxSend({
                url: 'CampaignManager/getCityNameList',
                success: function (vData) {

                    var vRows = vData.CityData.Rows;

                    $('#selCity').empty();

                    $('#selCity').append(new Option('Please Select', ''));
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        $('#selCity').append(new Option(vRows[p].Name, vRows[p].ID, false, false));
                    }
                },
                error: function (vError) {
                    var pDummy;
                }
            });

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
                                SetStartDate(vData.CampaignData.Rows[0].CampaignStartDate.replace('12:00:00 AM', ''));
                            }
                        }
                    });
                } else {
                    alert('Invalid CampaignID');
                }
            }


        }

        function SetStartDate(startDate) {
//            var monthNames = ["January", "February", "March", "April", "May", "June",
//            "July", "August", "September", "October", "November", "December"];

//            month = monthNames[startDate.getMonth()];

            $('#selStartDateMonth').val(startDate.getMonth());
            $('#selStartDateDay').val(startDate.getFullYear());
            $('#selStartDateYear').val(startDate.getDay());
        }

        function CreateNewTargetedAd() {
            if ($.trim($('#selAdDuration').val()) == 'fixed') {
                var lastDay = new Date($("#selEndDateYear").val() + '/' + $("#selEndDateMonth").val() + '/' + $("#selEndDateDay").val());
            }
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
                                .text(vRows[i].StartAge)
                            )
                            .append($('<td>')
                                .text(vRows[i].Gender)
                            )
                            .append($('<td>')
                                .text(vRows[i].LastUpdated.replace('12:00:00 AM', '')) //remove time
                            )
                            .append($('<td>')
                                .append($('<a href="/campaignmanager/coupon?id=' + vRows[i].ID + '" id="' + vRows[i].ID + '">')
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


