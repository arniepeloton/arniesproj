$(function () {

    setAdminMode();
    getTeams();
    //getWheather("Davao");
    getDashBoardData();
    getTODO("0");

    $("#btnGo").click(function () {

        getTeamInfo($("#optTeamsValue").val())

        var options = {
            Team: $("#optTeamsValue").val(),
            Month: $("#optMonthValue").val(),
            Year: $("#optYearValue").val()
        };

        getSalesDashBoard(options, 0);
    });
})

function getTODO(option) {

    var options = {
        m_Option: option
    };

    $.ajax({
        url: "getTODO/",
        data: JSON.stringify(options),
        dataType: "json",
        type: "POST",
        err: function (err) {
            alert("Error: " + err.toString());
        },
        success: function (data) {
            $('#tblTODO').dataTable(data);
        }
    });
}

function getSalesDashBoard(option, isOwn) {
    $.ajax({
        url: "getSalesByMonthly/",
        data: JSON.stringify(option),
        dataType: "json",
        type: "POST",
        err: function (err) {
            alert("Error: " + err.toString());
        },
        success: function (data) {
            var myData = data.d;

            if (isOwn == 0) {
                var currentTime = $("#optYearValue").val();
                var lastYear = $("#optYearValue").val() - 1;
            }
            else {
                var today = new Date();
                var currentTime = today.getFullYear();
                var lastYear = today.getFullYear() - 1;
            }

            var i = 0;
            var totalIncentive;
            var Quota1;
            var Quota2;
            var Incentive1;
            var Incentive2;

            var gData = [[lastYear.toString()], [currentTime.toString()], ["Quota1"], ["Quota2"], ["Quota3"]];

            $.each(myData, function (key, val) {

                if (i < 5) {

                    if (val["Data"] == "") {
                        gData[i][1] = 0;
                    }
                    else {
                        gData[i][1] = parseFloat(val["Data"]);
                    }
                }

                if (i == 1) {
                    totalIncentive = val["Data"];
                }
                else if (i == 2) {
                    Quota1 = val["Data"];
                }
                else if (i == 3) {
                    Quota2 = val["Data"];
                }
                else if (i == 5) {
                    Incentive1 = val["Data"];
                }
                else if (i == 6) {
                    Incentive2 = val["Data"];
                }

                i += 1;
            });

            plotDashBoard(gData);
            //getCollectionDashBoard(option); 
            createSalesGroupQuotaTable(Incentive1, Incentive2, lastYear, totalIncentive, Quota1, Quota2, Incentive1, Incentive2);
        }
    });
}

function getCollectionDashBoard(options, isOwn) {


    $.ajax({
        url: "getCollection/",
        data: JSON.stringify(options),
        dataType: "json",
        type: "POST",
        err: function (err) {
            alert("Error: " + err.toString());
        },
        success: function (data) {
            var myData = data.d;

            if (isOwn == 0) {
                var currentTime = $("#optYearValue").val();
                var lastYear = $("#optYearValue").val() - 1;
            }
            else {
                var today = new Date();
                var currentTime = today.getFullYear();
                var lastYear = today.getFullYear() - 1;
            }

            var i = 0;
            var totalIncentive;
            var Quota1;
            var Quota2;
            var Incentive1;
            var Incentive2;

            var gDataLastTwoMonths1 = [0, 0, 0, 0, 0, 0];
            var gDataLastTwoMonths2 = [0, 0, 0, 0, 0, 0];
            var gDataLastTwoMonths3 = [0, 0, 0, 0, 0, 0];
            var gDataLastTwoMonthsTotal = [0, 0, 0, 0, 0, 0];

            var gDataLastMonth1 = [0, 0, 0, 0, 0, 0];
            var gDataLastMonth2 = [0, 0, 0, 0, 0, 0];
            var gDataLastMonth3 = [0, 0, 0, 0, 0, 0];
            var gDataLastMonthsTotal = [0, 0, 0, 0, 0, 0];

            var gDataThisMonth1 = [0, 0, 0, 0, 0, 0];
            var gDataThisMonth2 = [0, 0, 0, 0, 0, 0];
            var gDataThisMonth3 = [0, 0, 0, 0, 0, 0];
            var gDataThisMonthTotal = [0, 0, 0, 0, 0, 0];
            var lastTwoMonths;
            var lastMonth;
            var thisMonth;

            $.each(myData, function (key, val) {

                if (i == 0) {
                    gDataLastTwoMonths1[0] = parseFloat(val["Value"]); //total
                    lastTwoMonths = val["Data"];
                }
                else if (i == 1) {
                    gDataLastTwoMonths1[1] = parseFloat(val["Value"]); //others
                }
                else if (i == 2) {
                    gDataLastTwoMonths2[1] = parseFloat(val["Value"]); //30
                }
                else if (i == 3) {
                    gDataLastTwoMonths3[1] = parseFloat(val["Value"]); //15
                }
                else if (i == 4) {
                    gDataLastMonth1[2] = parseFloat(val["Value"]); //total
                    lastMonth = val["Data"];
                }
                else if (i == 5) {
                    gDataLastMonth1[3] = parseFloat(val["Value"]); //others
                }
                else if (i == 6) {
                    gDataLastMonth2[3] = parseFloat(val["Value"]); //30
                }
                else if (i == 7) {
                    gDataLastMonth3[3] = parseFloat(val["Value"]); //15
                }
                else if (i == 8) {
                    gDataThisMonth1[4] = parseFloat(val["Value"]); //total
                    thisMonth = val["Data"];
                }
                else if (i == 9) {
                    gDataThisMonth1[5] = parseFloat(val["Value"]); //others
                }
                else if (i == 10) {
                    gDataThisMonth2[5] = parseFloat(val["Value"]); //30
                }
                else if (i == 11) {
                    gDataThisMonth3[5] = parseFloat(val["Value"]); //15
                }
                i++;
            });

            //uncollected
            gDataLastTwoMonthsTotal[1] = gDataLastTwoMonths1[0] - (gDataLastTwoMonths1[1] + gDataLastTwoMonths2[1] + gDataLastTwoMonths3[1]);
            gDataLastMonthsTotal[3] = gDataLastMonth1[2] - (gDataLastMonth1[3] + gDataLastMonth2[3] + gDataLastMonth3[3]);
            gDataThisMonthTotal[5] = gDataThisMonth1[4] - (gDataThisMonth1[5] + gDataThisMonth2[5] + gDataThisMonth3[5]);

            plotCollectionDashBoard(gDataLastTwoMonths1, gDataLastTwoMonths2, gDataLastTwoMonths3, gDataLastTwoMonthsTotal,
                                     gDataLastMonth1, gDataLastMonth2, gDataLastMonth3, gDataLastMonthsTotal,
                                     gDataThisMonth1, gDataThisMonth2, gDataThisMonth3, gDataThisMonthTotal,
                                     lastTwoMonths, lastMonth, thisMonth);
        }

    });
}

function getTeams() {

    $.ajax({
        url: "getTeams/",
        dataType: "json",
        type: "POST",
        err: function (err) {
            alert("Error: " + err.toString());
        },
        beforeSend: function () {
            $("#bd-loading").html("<img src=\"/Content/WhiteLabel/light/images/loading.gif\" />");
        },

        success: function (data) {

            $("#bd-loading").html("");

            var myData = data.d;
            var strTeams;

            strTeams = "Teams : <select id='optTeamsValue'> " +
                        "<option> -Select Value-</option>";

            $.each(myData, function (key, val) {
                strTeams += "<option value='" + val["ID"] + "'> " + val["TeamName"] + "</option>";
            });

            strTeams += "</select>";

            $("#Teams").html(strTeams)

        }
    });
}

function setAdminMode() {

    var strYear;
    var currentYear = new Date();
    var strMonths;
    var MonthList = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
    var ctr = 0;
    var myYear = 0;

    strYear = "Year : <select id='optYearValue'> " +
                  "<option> -Select Value-</option>";

    strMonths = "Month : <select id='optMonthValue'> " +
                  "<option> -Select Value-</option>";
    while (ctr < 12) {
        myYear = currentYear.getFullYear() - ctr;
        strYear += "<option value='" + myYear + "'> " + myYear + "</option>";
        strMonths += "<option value='" + parseInt(ctr + 1) + "'> " + MonthList[ctr] + "</option>";
        ctr += 1;
    }

    strYear += "</select>";
    strMonths += "</select>";

    $("#Year").html(strYear);
    $("#Month").html(strMonths);

}

function getWheather(city) {

    var options = {
        City: city
    };

    $.ajax({
        url: "getWheather/",
        data: JSON.stringify(options),
        dataType: "json",
        type: "POST",
        err: function (err) {
            alert("Error: " + err.toString());
        },
        beforeSend: function () {
            $("#bd-loading").html("<img src=\"/Content/WhiteLabel/light/images/loading.gif\" />");
        },

        success: function (data) {

            var myData = data.d;

            var ctr = 0;
            var degree = 0;
            var tr1a;
            var tr1b;
            var tr1c;
            var tr2a;
            var tr2b;
            var tr2c;

            var strHTML = "<table> <tr> <td style = 'font-size:Large; padding-left:10px'> <b> Real-time Weather </b> </td>" +
                                "<td style = 'font-size:Large; padding-left:10px'> <b> Whole-day Weather </b> </td> </tr>";
            strHTML += "<tr><td style = 'font-size:medium; padding-left:10px' colspan='2'> <br> <b> City: &nbsp; </b> " +
                            "<input id='city' value='' type='text'> &nbsp; <input id='Go' value='Go' type='button'> </td> </tr>";
            $("#bd-loading").html("");

            $.each(myData, function (key, val) {

                if (ctr == 0) {
                    tr1a = "<tr> <td style = 'font-size:medium; padding-left:10px;background-color:#EEF2F6'> <br/>" + val["Values"] + ", </td>";
                }
                if (ctr == 1) {
                    tempValue = val["Values"];
                }
                else if (ctr == 3) {
                    degree = val["Values"]
                }
                else if (ctr == 5) {
                    tr2a = "<td style = 'font-size:xx-Large; padding-left:10px; background-color:#EEF2F6'> <b>" + val["Values"] + " " + degree + " &deg;C, " + tempValue + " </b> </td>";
                }
                else if (ctr == 7) {
                    tempValue = val["Values"] + ". ";
                }
                else if (ctr == 8) {
                    degree = val["Values"];
                }
                else if (ctr == 9) {
                    tempValue += convertToDegrees(degree) + "&deg;C - " + convertToDegrees(val["Values"]) + "&deg;C, ";
                }
                else if (ctr == 10) {
                    //first forecast                        
                    tr1b = "<td style = 'font-size:medium; padding-left:10px; background-color:#EEF2F6'>" + val["Values"] + " " + tempValue;
                }
                else if (ctr == 11) {
                    tr1b += val["Values"] + "</td>";
                }
                else if (ctr == 12) {
                    tempValue = val["Values"] + ". ";
                }
                else if (ctr == 13) {
                    degree = val["Values"];
                }
                else if (ctr == 14) {
                    tempValue += convertToDegrees(degree) + "&deg;C - " + convertToDegrees(val["Values"]) + "&deg;C, ";
                }
                else if (ctr == 15) {
                    //second forecast
                    tr2b = "<td style = 'font-size:medium; padding-left:10px; background-color:#EEF2F6'>" + val["Values"] + " " + tempValue;
                }
                else if (ctr == 16) {
                    tr2b += val["Values"] + "</td>";
                }
                else if (ctr == 17) {
                    tempValue = val["Values"] + ". ";
                }
                else if (ctr == 18) {
                    degree = val["Values"];
                }
                else if (ctr == 19) {
                    tempValue += convertToDegrees(degree) + "&deg;C - " + convertToDegrees(val["Values"]) + "&deg;C, ";
                }
                else if (ctr == 20) {
                    //third forecast                        
                    tr1c = "<td style = 'font-size:medium; padding-left:10px; background-color:#EEF2F6'>" + val["Values"] + " " + tempValue;
                }
                else if (ctr == 21) {
                    tr1c += val["Values"] + "</td> </tr>";
                }
                else if (ctr == 22) {
                    tempValue = val["Values"] + ". ";
                }
                else if (ctr == 23) {
                    degree = val["Values"];
                }
                else if (ctr == 24) {
                    tempValue += convertToDegrees(degree) + "&deg;F - " + convertToDegrees(val["Values"]) + "&deg;F, ";
                }
                else if (ctr == 25) {
                    //fourth forecast
                    tr2c = "<td style = 'font-size:medium; padding-left:10px; background-color:#EEF2F6'>" + val["Values"] + " " + tempValue;
                }
                else if (ctr == 26) {
                    tr2c += val["Values"] + "</td> </tr>";
                }

                ctr += 1;
            });

            strHTML += tr1a + tr1b + tr1c + tr2a + tr2b + tr2c;

            strHTML += "</table>"

            $("#wheatherReport").html(strHTML);

            $("#city").keydown(function (e) {
                if (e.keyCode == 13) {
                    $("#wheatherReport").html("");
                    getWheather($(this).val());
                }
            });

            $("#Go").click(function (e) {
                getWheather($("#city").val());
            });
        }
    });
}

function convertToDegrees(mValue) {
    return parseInt((5 / 9) * (mValue - 32))
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
    return (((sign) ? '' : '-') + num + '.' + cents);
}
//  End -->

