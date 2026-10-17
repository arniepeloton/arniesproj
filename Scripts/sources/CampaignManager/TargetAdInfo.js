require([
    'common'
    ], function () {

        $(function () {
            GetCampaignDetails();

            getCityName();
            var d = new Date();
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

            $('#txtMessage').change(function () {
                if ($.trim($(this).val()) == '') {
                    $('#spanMessage').show();
                    $(this).addClass('form-validation-error');

                } else {
                    $('#spanMessage').hide();
                    $(this).removeClass('form-validation-error');
                }

            });


            $('#selAdDuration').change(function () {
                if ($.trim($(this).val()) == '') {
                    $('#spanAdDuration').show();
                    $('#selAdDuration').addClass('form-validation-error');

                } else {

                    $('#spanAdDuration').hide();
                    $('#selAdDuration').removeClass('form-validation-error');

                    if ($.trim($(this).val()) == 'Fixed') {
                        $('.fixed').show();
                    } else {
                        $('.fixed').hide();
                    }


                }

            });

            $('#txtAgeFrom').change(function () {
                $('#spanAge').hide();
                $('#spanAgeValid').hide();
                $(this).removeClass('form-validation-error');
                $('#txtAgeTo').removeClass('form-validation-error');

                if ($.trim($(this).val()) == '') {
                    $('#spanAge').show();
                    $(this).addClass('form-validation-error');

                } else {

                    if (!validateAge($.trim($(this).val()))) {
                        $('#spanAgeValid').show();
                        $(this).addClass('form-validation-error');
                    }

                }

            });

            $('#txtAgeTo').change(function () {
                $('#spanAge').hide();
                $('#spanAgeValid').hide();
                $(this).removeClass('form-validation-error');
                $('#txtAgeFrom').removeClass('form-validation-error');

                if ($.trim($(this).val()) == '') {
                    $('#spanAge').show();
                    $(this).addClass('form-validation-error');

                } else {

                    if (!validateAge($.trim($(this).val()))) {
                        $('#spanAgeValid').show();
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
                if ($.trim($('#txtMessage').val()) == '') {
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

                CreateNewTargetedAd();
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
            if ($.trim($('#txtMessage').val()) == '') {
                $('#txtMessage').addClass('form-validation-error');
                $('#spanMessage').show();
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
                if (!validateAge($.trim($('#txtAgeFrom').val()))) {
                    $('#txtAgeFrom').addClass('form-validation-error');
                    $('#spanAgeValid').show();
                }
            }

            if ($.trim($('#txtAgeTo').val()) == '') {
                $('#txtAgeTo').addClass('form-validation-error');
                $('#spanAge').show();
            } else {
                if (!validateAge($.trim($('#txtAgeTo').val()))) {
                    $('#txtAgeTo').addClass('form-validation-error');
                    $('#spanAgeValid').show();
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
                                //                                $('#lblCampaignName').text(vData.CampaignData.Rows[0].CampaignName);
                                //                                $('#lblCampaignCode').text(vData.CampaignData.Rows[0].CampaignCode);
                                //                                $('#lblStartDate').text(vData.CampaignData.Rows[0].CampaignStartDate.replace('12:00:00 AM', ''));
                                //                                $('#lblEndDate').text(vData.CampaignData.Rows[0].CampaignEndDate.replace('12:00:00 AM', ''));
                                //                                //                        $('#lblCampaignDuration').text(vData.CampaignData.Rows[0].CampaignType);
                                //                                $('#lblLastUpdated').text(vData.CampaignData.Rows[0].LastUpdated.replace('12:00:00 AM', ''));


                                SetStartDate(vData.CampaignData.Rows[0].CampaignStartDate);

                            }
                        }
                    });
                } else {
                    alert('Invalid CampaignID');
                }
            }


        }


        function SetStartDate(startDate) {
            setMonthCollection('selStartDateMonth');
            //set year
            var d = new Date();
            $('#selStartDateYear').append(new Option(d.getFullYear() + 2, d.getFullYear() + 2));
            $('#selStartDateYear').append(new Option(d.getFullYear() + 1, d.getFullYear() + 1));
            $('#selStartDateYear').append(new Option(d.getFullYear(), d.getFullYear()));

            //set start date to campaign start date by default
            $('#selStartDateMonth').val(new Date(startDate).getMonth() + 1);
            $('#selStartDateYear').val(new Date(startDate).getFullYear());

            setDayCollection('selStartDateMonth', 'selStartDateDay', 'selStartDateYear', false);

            $('#selStartDateDay').val(new Date(startDate).getDate());

        }

        function CreateNewTargetedAd() {
            var lastDay = "NULL";

            if ($.trim($('#selAdDuration').val()) == 'Fixed') {
                var lastDay = $("#selEndDateMonth").val() + '/' + $("#selEndDateDay").val() + '/' + $("#selEndDateYear").val()
            }

            var campaignId = querySt('id');

            var pData = {};

            pData = {
                CampaignID: campaignId,
                AdName: $.trim($("#txtAdName").val()),
                AdDescription: $.trim($("#txtAdDescription").val()),
                AdType: $.trim($('input[name="AdType"]:checked').val()),
                Message: $.trim($("#txtMessage").val()),
                AdDuration: $.trim($("#selAdDuration").val()),
                CouponCodeLength: $.trim($("#selCouponCodeLength").val()),
                CouponCodeType: $.trim($("#selCouponCodeType").val()),
                NoOfCoupons: $.trim($("#txtNoOfCoupons").val()),
                IsCouponAutoAdd: $.trim($('input[name="autoAdd"]:checked').val()),
                AgeFrom: $.trim($("#txtAgeFrom").val()),
                AgeTo: $.trim($("#txtAgeTo").val()),
                Gender: $.trim($("#selGender").val()),
                CityID: $.trim($("#selCity").val()),
                AdStartDate: $("#selStartDateMonth").val() + '/' + $("#selStartDateDay").val() + '/' + $("#selStartDateYear").val(),
                AdEndDate: lastDay

            };

            AjaxSend({
                url: 'CampaignManager/CreateNewTargetedAd',
                data: pData,
                success: function (vData) {
                    //if successful
                    if (vData.TargetAdData != '' || vData.TargetAdData != 'Error' || vData.TargetAdData != 'DateError') {
                        window.open(GetPathFormat('/campaignmanager/couponlist?id=' + campaignId), '_self');
                    } else {
                        if (vData.TargetAdData == 'DateError')
                            alert('Invalid Date');

                        if (vData.TargetAdData == 'Error')
                            alert('There was an error in processing your data. Please contact Mobiz Support.');

                    }

                } // success
            }); // AjaxSend

        }

    });


