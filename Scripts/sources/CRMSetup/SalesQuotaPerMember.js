require([
    'common'
    ], function () {
        $(function () {
            getSalesQuotaPerMember();

            $("#txtQuotaPerMember").focusout(function () {
                $("#txtQuotaPerMember").val('PhP ' + formatCurrency($("#txtQuotaPerMember").val().replace(/PhP/g, "")));
            });

            $("#txtTier1IncentiveRate").focusout(function () {
                $("#txtTier1IncentiveRate").val($("#txtTier1IncentiveRate").val() + "%");
            });

            $("#txtTier2RequiredGrowth").focusout(function () {
                $("#txtTier2RequiredGrowth").val($("#txtTier2RequiredGrowth").val() + "%");
            });

            $("#txtTier2IncentiveRate").focusout(function () {
                $("#txtTier2IncentiveRate").val($("#txtTier2IncentiveRate").val() + "%");
            });

            $("#txtTier3RequiredGrowth").focusout(function () {
                $("#txtTier3RequiredGrowth").val($("#txtTier3RequiredGrowth").val() + "%");
            });

            $("#txtTier3IncentiveRate").focusout(function () {
                $("#txtTier3IncentiveRate").val($("#txtTier3IncentiveRate").val() + "%");
            });

            $("#btnNext").click(function () {
                saveSalesQuotaPerMember();
            });
        })

        function getSalesQuotaPerMember() {

            AjaxSend({
                url: 'crmsetup/getTeamInfo/',
                success: function (vData) {
                    $("#txtQuotaPerMember").val('PhP ' + formatCurrency(vData.data.Rows[0].QuotaPerMember));
                    $("#txtTier1IncentiveRate").val(vData.data.Rows[0].Tier1IncentiveRate + "%");
                    $("#txtTier2RequiredGrowth").val(vData.data.Rows[0].Tier2RequiredGrowth + "%");
                    $("#txtTier2IncentiveRate").val(vData.data.Rows[0].Tier2IncentiveRate + "%");
                    $("#txtTier3RequiredGrowth").val(vData.data.Rows[0].Tier3RequiredGrowth + "%");
                    $("#txtTier3IncentiveRate").val(vData.data.Rows[0].Tier3IncentiveRate + "%");
                },
                error: function (vError) {
                    var pDummy;
                }
            });
        }

        function saveSalesQuotaPerMember() {
            var option = {
                QuotaPerMember: $("#txtQuotaPerMember").val().replace(/,/g, "").replace(/PhP/g, ""),
                Tier1IncentiveRate: $("#txtTier1IncentiveRate").val().replace(/%/g, ""),
                Tier2RequiredGrowth: $("#txtTier2RequiredGrowth").val().replace(/%/g, ""),
                Tier2IncentiveRate: $("#txtTier2IncentiveRate").val().replace(/%/g, ""),
                Tier3RequiredGrowth: $("#txtTier3RequiredGrowth").val().replace(/%/g, ""),
                Tier3IncentiveRate: $("#txtTier3IncentiveRate").val().replace(/%/g, "")
            }

            AjaxSend({
                url: 'crmsetup/saveSalesQuotaPerMember/',
                data: option,
                success: function (vData) {
                    window.open("../Quota/", "_self");
                },
                error: function (vError) {
                    var pDummy;
                }
            });

            //    $.ajax({
            //        url: "../saveSalesQuotaPerMember/",
            //        dataType: "json",
            //        data: JSON.stringify(option),
            //        type: "POST",
            //        err: function (err) {
            //            alert("Error: " + err.toString());
            //        },
            //        beforeSend: function () {
            //        },
            //        success: function (data) {
            //            window.open("../Quota/", "_self");
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