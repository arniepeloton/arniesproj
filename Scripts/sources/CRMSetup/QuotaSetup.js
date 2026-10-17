require([
    'common'
    ], function () {
        $(function () {
            getQuotaInfo();

            $("#Done").click(function () {
                saveQuotaInfo();
            })

            $("#txtCommision1").focusout(function () {
                adjustValues();
            });

            $("#txtCommision2").focusout(function () {
                adjustValues();
            });

            $("#txtCommision3").focusout(function () {
                adjustValues();
            });

            $("#txtQuotaFrom1").focusout(function () {
                $("#mValueQuotaFrom1").val($("#txtQuotaFrom1").val().replace(/,/g, "").replace("PhP ", ""))
                adjustValues();
            });

            $("#txtGrowthFromQuota1").focusout(function () {
                $("#mValueQuotaFrom2").val($("#lblQuotaFrom2").val().replace(/,/g, "").replace("PhP ", ""))
                adjustValues();
            })

            $("#txtGrowthFromQuota2").focusout(function () {
                $("#mValueQuotaFrom3").val($("#lblQuotaFrom3").val().replace(/,/g, "").replace("PhP ", ""))
                adjustValues();
            })
        })

        function adjustValues() {
            var GFQ1 = $("#txtGrowthFromQuota1").val().replace(/%/g, "");
            var GFQ2 = $("#txtGrowthFromQuota2").val().replace(/%/g, "");

            $("#mValueQuotaFrom2").val((parseFloat(GFQ1) / 100) * parseFloat($("#mValueQuotaFrom1").val()) + (parseFloat($("#mValueQuotaFrom1").val())));
            $("#mValueQuotaFrom3").val((parseFloat(GFQ2) / 100) * parseFloat($("#mValueQuotaFrom1").val()) + (parseFloat($("#mValueQuotaFrom1").val())));

            $("#txtQuotaFrom1").val("PhP " + formatCurrency($("#mValueQuotaFrom1").val()));
            $("#lblQuotaFrom2").html("PhP " + formatCurrency($("#mValueQuotaFrom2").val()));
            $("#lblQuotaFrom3").html("PhP " + formatCurrency($("#mValueQuotaFrom3").val()));

            $("#txtGrowthFromQuota1").val(GFQ1 + '%')
            $("#txtGrowthFromQuota2").val(GFQ2 + '%')

            $("#txtCommision1").val($("#txtCommision1").val().replace(/%/g, "") + '%')
            $("#txtCommision2").val($("#txtCommision2").val().replace(/%/g, "") + '%')
            $("#txtCommision3").val($("#txtCommision3").val().replace(/%/g, "") + '%')
        }

        function saveQuotaInfo() {
            var option = {
                RangeQuota: $("#mValueQuotaFrom1").val() + ',' + $("#txtCommision1").val().replace(/%/g, "") + ','
                    + $("#mValueQuotaFrom1").val() + ',' + $("#mValueQuotaFrom2").val() + ',' + $("#txtCommision1").val().replace(/%/g, "") + ','
                    + $("#mValueQuotaFrom2").val() + ',' + $("#mValueQuotaFrom3").val() + ',' + $("#txtCommision2").val().replace(/%/g, "") + ','
                    + $("#mValueQuotaFrom3").val() + ',10000000' + ',' + $("#txtCommision3").val().replace(/%/g, "") + ','
                    + $("#txtGrowthFromQuota1").val().replace(/%/g, "") + "," + $("#txtGrowthFromQuota2").val().replace(/%/g, "")
            };

            $.ajax({
                url: "saveQuotaInfo/",
                dataType: "json",
                data: JSON.stringify(option),
                type: "POST",
                err: function (err) {
                    alert("Error: " + err.toString());
                },
                beforeSend: function () {
                },
                success: function (data) {
                    window.open("Quota/", "_self");
                }
            });
        }


        function getQuotaInfo() {

            AjaxSend({
                url: 'crmsetup/GetQuotaInfo/',
                success: function (vData) {
                    $("#txtQuotaFrom1").val("PhP " + formatCurrency(vData.data.Rows[0].From));
                    $("#mValueQuotaFrom1").val(vData.data.Rows[0].From);
                    $("#txtCommision1").val(vData.data.Rows[0].Incentive);

                    $("#txtCommision2").val(vData.data.Rows[2].Incentive);
                    $("#txtCommision3").val(vData.data.Rows[3].Incentive);

                    $("#txtGrowthFromQuota1").val(vData.data.Rows[4].From);
                    $("#txtGrowthFromQuota2").val(vData.data.Rows[4].From);

                    $("#mValueQuotaFrom1").val($("#txtQuotaFrom1").val().replace(/,/g, "").replace("PhP ", ""));
                    adjustValues();

                    //            $("#txtQuotaPerMember").val('PhP ' + formatCurrency(vData.data.Rows[0].QuotaPerMember));
                    //            $("#txtTier1IncentiveRate").val(vData.data.Rows[0].Tier1IncentiveRate + "%");
                    //            $("#txtTier2RequiredGrowth").val(vData.data.Rows[0].Tier2RequiredGrowth + "%");
                    //            $("#txtTier2IncentiveRate").val(vData.data.Rows[0].Tier2IncentiveRate + "%");
                    //            $("#txtTier3RequiredGrowth").val(vData.data.Rows[0].Tier3RequiredGrowth + "%");
                    //            $("#txtTier3IncentiveRate").val(vData.data.Rows[0].Tier3IncentiveRate + "%");
                },
                error: function (vError) {
                    var pDummy;
                }
            });


            //    $.ajax({
            //        url: "GetQuotaInfo/",
            //        dataType: "json",
            //        type: "POST",
            //        err: function (err) {
            //            alert("Error: " + err.toString());
            //        },
            //        beforeSend: function () {
            //        },
            //        success: function (data) {
            //            var myData = data.d;
            //            var strHTML = "";
            //            var ctr = 0;

            //            $.each(myData, function (key, value) {
            //                if (ctr == 0) {
            //                    $("#txtQuotaFrom1").val("PhP " + formatCurrency(value["From"]));
            //                    $("#mValueQuotaFrom1").val(value["From"]);
            //                    $("#txtCommision1").val(value["Incentive"]);
            //                    //                    $("#txtQuotaFrom1").val(value["From"]);
            //                    //                    $("#txtCommision1").val(value["Incentive"]);
            //                }
            //                else if (ctr == 1) {                    
            //                                        
            //                }
            //                else if (ctr == 2) {
            //                    $("#txtCommision2").val(value["Incentive"]);
            //                }
            //                else if (ctr == 3) {
            //                    $("#txtCommision3").val(value["Incentive"]);
            //                }
            //                else {
            //                    $("#txtGrowthFromQuota1").val(value["From"]);
            //                    $("#txtGrowthFromQuota2").val(value["To"]);
            //                }
            //                ctr += 1;
            //            });
            //            $("#mValueQuotaFrom1").val($("#txtQuotaFrom1").val().replace(/,/g, "").replace("PhP ", ""))
            //            adjustValues();
            //        }
            //    });
        }

        //<!-- Original:  Cyanide_7 (leo7278@hotmail.com) -->
        //<!-- Web Site:  http://www7.ewebcity.com/cyanide7 -->
        //<!-- This script and many more are available free online at -->
        //<!-- The JavaScript Source!! http://javascript.internet.com -->
        //<!-- Begin
        function formatCurrency(num) {
            num = num.toString().replace(/\$|\,/g, '');
            if (isNaN(num))
                num = "0";
            sign = (num == (num = Math.abs(num)));
            num = Math.floor(num * 100 + 0.50000000001);
            cents = num % 100;
            num = Math.floor(num / 100).toString();
            if (cents < 10)
                cents = "0" + cents;
            for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3); i++)
                num = num.substring(0, num.length - (4 * i + 3)) + ',' +
    num.substring(num.length - (4 * i + 3));
            return (((sign) ? '' : '-') + num);
        }
        //  End -->
    });