require([
    'common'
    ], function () {
        $(function () {
            getTeamQuota();

            $("#btnNext").click(function () {
                window.open("../ItemIncentive/", "_self");
            });

            $("#btnCalculate").click(function () {
                recalculateBaseQuota();
            });
        })

        function getTeamQuota() {
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

            $.ajax({
                url: "../getTeamQuota/",
                dataType: "json",
                type: "POST",
                err: function (err) {
                    alert("Error: " + err.toString());
                },
                beforeSend: function () {
                },
                success: function (data) {
                    var myData = data.d;
                    var strHTML = "";
                    var months = ["", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
                    var ctr = 1;

                    $.each(myData, function (key, val) {

                        var Quota = val["RangeQuota"].split(',');

                        strHTML += "<li><a href='#' style='white-space:normal' class='btnMonth' ID='" + parseInt(val["MonthQuota"]) + "'> " + months[ctr] +
                "<span class='ui-li-count ui-btn-up-c ui-btn-corner-all' style='font-size:small'>";

                        switch (parseInt(val["MonthQuota"])) {
                            case 1: strHTML += "P" + formatCurrency(Quota[1]); break;
                            case 2: strHTML += "P" + formatCurrency(Quota[1]); break;
                            case 3: strHTML += "P" + formatCurrency(Quota[1]); break;
                            case 4: strHTML += "P" + formatCurrency(Quota[1]); break;
                            case 5: strHTML += "P" + formatCurrency(Quota[1]); break;
                            case 6: strHTML += "P" + formatCurrency(Quota[1]); break;
                            case 7: strHTML += "P" + formatCurrency(Quota[1]); break;
                            case 8: strHTML += "P" + formatCurrency(Quota[1]); break;
                            case 9: strHTML += "P" + formatCurrency(Quota[1]); break;
                            case 10: strHTML += "P" + formatCurrency(Quota[1]); break;
                            case 11: strHTML += "P" + formatCurrency(Quota[1]); break;
                            case 12: strHTML += "P" + formatCurrency(Quota[1]); break;
                        }

                        strHTML += "</span> </a></li>";
                        ctr += 1;
                    });

                    $("#lstQuota").html(strHTML);
                    $("#lstQuota").listview('refresh');

                    $(".btnMonth").click(function () {
                        window.open("../QuotaSetUp?Month=" + $(this).attr("ID"), "_self")
                    });
                }
            });
        }


        function recalculateBaseQuota() {
            $.ajax({
                url: "../ReCalculateBaseQuota/",
                dataType: "json",
                type: "POST",
                err: function (err) {
                    alert("Error: " + err.toString());
                },
                beforeSend: function () {
                },
                success: function (data) {
                    window.open("../Quota/", "_self")
                }
            });
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