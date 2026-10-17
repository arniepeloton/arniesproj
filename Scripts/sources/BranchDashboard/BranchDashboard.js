$(function () {
    var currentDate = new Date();
    var month = currentDate.getMonth() + 1;
    var day = currentDate.getDate();
    var yday = currentDate.getDate() - 1;
    var oday = currentDate.getDate() - 2;
    var year = currentDate.getFullYear();

    setLevel0("Employees", "../welcome", 1);
    setLevel1("Branch", "../BranchDashboard/", 1);
    setLevel2("Dashboard", "../BranchDashboard/", 1);
    //  $("#bd-title-header").DailySales();
    //$("#bd-title-header").LoadSales();
    $("#bd-trailing").LoadSales();
    //$("#bd-trailing-branch").DailySalesGraphPerBranch();
    $("#sales-dt-start").attr("value", month + "/" + day + "/" + year);
    $("#sales-dt-end").attr("value", month + "/" + day + "/" + year);
    $("#sales-dt-start").datepicker();
    $("#sales-dt-end").datepicker();

    $(this).LoadBranches();

    $(this).LoadOBDRComparison({
        "datestart": month + "/" + day + "/" + year,
        "dateend": month + "/" + day + "/" + year,
        "dateString": "Today"
    });

    $("#btnToday").click(function () {
        $(this).LoadOBDRComparison({
            "datestart": month + "/" + day + "/" + year,
            "dateend": month + "/" + day + "/" + year,
            "dateString": "Today"
        });
    });

    $("#btnYesterday").click(function () {
        $(this).LoadOBDRComparison({
            "datestart": month + "/" + day + "/" + year,
            "dateend": month + "/" + day + "/" + year,
            "dateString": "Yesterday"
        });
    });

    $("#btnOnTheOtherDay").click(function () {
        $(this).LoadOBDRComparison({
            "datestart": month + "/" + day + "/" + year,
            "dateend": month + "/" + day + "/" + year,
            "dateString": "Otherday"
        });
    });
});

/* 
Trailing Sales
#################################################################################
Daily Sales         -   (Today, Yesterday, On the Other Day)
Weekly Sales        -   (Current Week, Last Week, On the Other Week)
Monthly Sales       -   (Current Month, Last Month, On the Other Month)
Quarterly Sales     -   (Current Quarter, Last Quarter, On the Other Quarter)
#################################################################################

This program is using calendar date, date will begin on January 01 and end at December 31.
*/

(function ($) {
    var bdChart;
    var mServerDate;

    $.fn.extend({
        DailySales: function () {
            return $(this).each(function () {
                //Ajax Function
                LoadDailySales($(this));
            });
        },
        LoadSales: function(){
            return $(this).each(function(){
                LoadSalesToTable($(this));

                $(this).change(function(){
                    LoadSalesToTable($(this));
                });
            });
        },
        DailySalesGraphPerBranch: function(){
            return $(this).each(function(){
                $(this).change(function(){
                    SalesReportChart("TrailingSalesDailyPerBranchGraph/", $("#bd-trailing-branch").val());
                });
            });
        }, 
        LoadOBDRComparison: function(obj){
            var defaults = obj;
            obj = $.extend(defaults, obj);
            return $(this).each(function(){
               LoadOBDRComparison(obj); 
            });
        },
        LoadBranches: function(){
            return $(this).each(function(){
                LoadBranches();
            });
        }
    });

    /* 
        Select Type of Sales to be Displayed
    */
    function LoadSalesToTable(obj){
        switch($("#bd-trailing").val()){
            case "daily":
                //LoadDailySales(obj);
                $("#widget-title").html("Store Sales: Daily Sales Comparison");
                SalesReports(obj, "TrailingSalesDaily/");
                break;
            case "weekly":
                $("#widget-title").html("Store Sales: Weekly Sales Comparison");
                SalesReports(obj,"TrailingSalesWeekly/");
                break;
            case "monthly":
                $("#widget-title").html("Store Sales: Monthly Sales Comparison");
                SalesReports(obj,"TrailingSalesMonthly/");
                break;
            case "quarterly":
                $("#widget-title").html("Store Sales: Quarterly Sales Comparison");
                SalesReports(obj,"TrailingSalesQuaterly/");
                break;
        }
    }

    function SalesReportChart(url, BranchName){
        SalesReportChartAjax(url, BranchName,$("#bd-trailing").val());
    }

    /* Sales Reports */
    function SalesReports(obj, url, ReportType){
        $.ajax({
            type: "POST",
            url: url,
            dataType: "json",
            beforeSend: function(){
                $("#bd-loading").html("<img src=\"/Content/WhiteLabel/light/images/loading.gif\" />");
            },
            success: function (mData){
                //Fetch All records
                $("#bd-loading").html("");
                var pData, Counter;
                var DisplayHeader = "";
                var DisplayContent = "";
                var TopHeaderTitle = new Array();
                var TopDateTime = new Array();
                var BranchNames = new Array();

                mServerDate = mData.d.time;
                pData = mData.d.sales.Rows;

                //########################################################################################################
                //Initialize Header
                $.each(pData, function(pKey, pVal){
                    if(TopHeaderTitle.indexOf(pVal["Trailing"]) == -1){
                        TopHeaderTitle.push(pVal["Trailing"]);
                        TopDateTime.push(pVal["Date"]);
                    }
                });

                //Loop Header
                Counter = 0;
                DisplayHeader = "<tr>" +
                                "   <th></th>";
                while(Counter < TopHeaderTitle.length){
                    DisplayHeader += "<th colspan='2'>" + TopHeaderTitle[Counter] + "<br/>" + TopDateTime[Counter] + "</th>";
                    Counter++;
                }
                DisplayHeader += "</tr>";
                DisplayHeader += "<tr>" +
                                  "     <th>Branch Name</th>" +
                                  "     <th>Actual Sales</th>" +
                                  "     <th>Target Sales</th>" +
                                  "     <th>Actual Sales</th>" +
                                  "     <th>Target Sales</th>" +
                                  "     <th>Actual Sales</th>" +
                                  "     <th>Target Sales</th>" +
                                  "</tr>";
                $("#tbl_header").html(DisplayHeader);
                //########################################################################################################

                //Fetch Of Branch Names and Records
                Counter = 0;
                var SubCounter;
                var ActualTotalOne, ActualTotalTwo, ActualTotalThree;
                var TargetTotalOne, TargetTotalTwo, TargetTotalThree;

                ActualTotalOne = 0;
                ActualTotalTwo = 0;
                ActualTotalThree = 0;

                TargetTotalOne = 0;
                TargetTotalTwo = 0;
                TargetTotalThree = 0;
                $.each(pData, function (pKey, pVal){
                    if (BranchNames.indexOf(pVal["BranchName"]) == -1){
                        BranchNames.push(pVal["BranchName"]);
                        DisplayContent += "<tr>" +
                                          "     <td>" + '<div class=divLastImportDate>' + pVal['LastImportDate'] + '</div>' + pVal["BranchName"] + "</td>"; 
                        SubCounter = 0;
                        $.each(pData, function(sKey, sVal){
                            if (BranchNames[Counter] == sVal["BranchName"]){
                                DisplayContent += "<td align='center'>" + formatCurrency(sVal["ActualSales"]) + "</td>" +
                                                  "<td align='center'>" + formatCurrency(sVal["TargetSales"]) + "</td>";
                                if (SubCounter == 0){
                                    ActualTotalOne += parseFloat(sVal["ActualSales"]);
                                    TargetTotalOne += parseFloat(sVal["TargetSales"]);
                                }else if(SubCounter == 1){
                                    ActualTotalTwo += parseFloat(sVal["ActualSales"]);
                                    TargetTotalTwo += parseFloat(sVal["TargetSales"]);
                                }else if(SubCounter == 2){
                                    ActualTotalThree += parseFloat(sVal["ActualSales"]);
                                    TargetTotalThree += parseFloat(sVal["TargetSales"]);
                                }
                                SubCounter++;
                            }
                        });
                        DisplayContent += "</tr>";
                        Counter++;
                    }
                });
                DisplayContent += "<tr>" +
                              "     <td style='font-weight: bolder;' align='left'>Total Sales</td>" +
                              "     <td style='font-weight: bolder;' align='center'>" + formatCurrency(ActualTotalOne) + "</td>" +
                              "     <td style='font-weight: bolder;' align='center'>" + formatCurrency(TargetTotalOne) + "</td>" +
                              "     <td style='font-weight: bolder;' align='center'>" + formatCurrency(ActualTotalTwo) + "</td>" +
                              "     <td style='font-weight: bolder;' align='center'>" + formatCurrency(TargetTotalTwo) + "</td>" +
                              "     <td style='font-weight: bolder;' align='center'>" + formatCurrency(ActualTotalThree) + "</td>" +
                              "     <td style='font-weight: bolder;' align='center'>" + formatCurrency(TargetTotalThree) + "</td>" +
                              "</tr>";

                $("#tbl_content").html(DisplayContent);
                SalesReportChartAjax("TrailingSalesDailyPerBranchGraph/",BranchNames[0], $("#bd-trailing").val());

                var DisplayBranchList = "";
                Counter = 0;
                while (Counter < BranchNames.length) {
                    DisplayBranchList += "<option value='" + BranchNames[Counter] + "'>" + BranchNames[Counter] + "</option>";
                    Counter++;
                }
                $("#bd-trailing-branch").html(DisplayBranchList); 
                //$.uniform.update();
                FormatLastImportDate();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(errorThrown);
            }
        });
    }

    function SalesReportChartAjax(url, BranchName, ReportType){
        var sData = {BranchName: BranchName,ReportType: ReportType};
        $.ajax({
            type: "POST",
            url: url,
            data: JSON.stringify(sData),
            dataType: "json",
            beforeSend: function(){
                $("#bd-loading-1").html("<img src=\"/Content/WhiteLabel/light/images/loading.gif\" />");

            },
            success: function(mData){
                $("#bd-loading-1").html("");
                var pData, Counter, tData;
                var ActualSales = new Array();
                var TargetSales = new Array();
                var Titles = new Array();

                tData = mData.d.time;
                $.each(mData.d.sales, function(pKey, pVal){
                    pData = pVal;
                });

                Counter = 0;
                $.each(pData, function(sKey, sVal){
                    if(Titles.indexOf(sVal["Trailing"]) == -1){
                        Titles.push(sVal["Trailing"]);
                        $.each(pData, function(vKey, vVal){
                            if (Titles[Counter] == vVal["Trailing"]){
                                ActualSales.push(parseFloat(vVal["ActualSales"]));
                                TargetSales.push(parseFloat(vVal["TargetSales"]));
                            }
                        });
                        Counter++;
                    }
                });

                if(bdChart) bdChart.destroy();

               bdChart = $.jqplot('Chart', [ActualSales, TargetSales], {
                       seriesDefaults: {
                           renderer: $.jqplot.BarRenderer,
                           pointLabels: { show: false },
                           rendererOptions:{
                                barWidth: 30
                           }
                       },
                       series: [
                            {
                                label: 'Actual Sales',
                                color: '#888888'
                            },
                            {
                                label: 'Target Sales',
                                color: '#C0C0C0'
                            }
                       ],
                       axes: {
                           xaxis: {
                               renderer: $.jqplot.CategoryAxisRenderer,
                               ticks: Titles
                           }
                       },
                       legend: {
                            show: true,
                            placement: 'outsideGrid'
                        },
                       highlighter: { show: true,
                           showMarker: false,
                           tooltipContentEditor: function (str, si, pi, plot) {
                               //return HtmlEncode(plot.data[si][pi][0]) + '<br />' + HtmlEncode(plot.series[si].label) + ': ' + HtmlEncode(plot.data[si][pi][1]);
                              return HtmlEncode(plot.series[si].label + " : " + formatCurrency(plot.data[si][pi]));
                           }
                       }
                   });

            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(errorThrown);
            }
        });
    }
    
    function FormatLastImportDate() {
        
        $('#spanServerTime').text(mServerDate);
        $('.divLastImportDate').each(function () {
            var $this = $(this);
            var pDiv;

            pDiv = $('<div>')
                
                .addClass('LastImportBulb')
                .tipsy({gravity: 'w'})
                .append($('<a>')
                    .attr('href', '/Reports/QrBranchesStatus')
                    .css('display', 'block')
                    .html('&nbsp;')
                );
            if (IsDate($this.text())) {
                if (new Date(mServerDate).getTime() - new Date($this.text()).getTime() >= 10 * 60 * 1000) 
                    pDiv.css('background-color', 'red');
                else
                    pDiv.css('background-color', 'green');
                pDiv.attr('title', 'Last import date: ' + $this.text());
            } else {
                pDiv.css('background-color', 'gray');
                pDiv.attr('title', 'No data');
            }

            $this.replaceWith(pDiv);
        });

    }; // END FormatLastImportDate

    //OBDR and POS Sales Comparison

    function LoadOBDRComparison(obj){
        $.ajax({
            type: "POST",
            url: "DepositSalesComparison/",
            dataType: "json",
            data: JSON.stringify(obj),
            beforeSend: function(){
                $("#bd-loading-2").html("<img src=\"/Content/WhiteLabel/light/images/loading.gif\" />");
            }, success: function(mData){
                $("#bd-loading-2").html("");
                var pData;
                var vHtml = "";
                var TotalDeposit, TotalEOD, TotalVarD, TotalVarE, TotalVarF;

                TotalDeposit = 0;
                TotalEOD = 0;
                TotalVarD = 0;
                TotalVarE = 0;
                TotalVarF = 0;
                pData = mData.d.obdr.Rows;

                $.each(pData, function(pKey, pVal){   
                    vHtml += "<tr>";
                    vHtml += "  <td>" + pVal["BranchDescriptions"] + "</td>";
                    vHtml += "  <td align='center'>" + pVal["DepositedDate"] + "</td>";
                    vHtml += "  <td align='right'>" + formatCurrency(pVal["DepositedAmount"]) + "</td>";
                    vHtml += "  <td align='center'>" + pVal["SalesDate"] + "</td>";
                    vHtml += "  <td align='right'>" + formatCurrency(pVal["EndOfDaySales"]) + "</td>";
                    vHtml += "  <td align='right'>" + formatCurrency(pVal["SalesDashboard"]) + "</td>";
                    vHtml += "  <td align='right'>" + formatCurrency(pVal["VarianceD"]) + "</td>";
                    vHtml += "  <td align='right'>" + formatCurrency(pVal["VarianceE"]) + "</td>";
                    vHtml += "  <td align='right'>" + formatCurrency(pVal["VarianceF"]) + "</td>";
                    vHtml += "</tr>"; 
                    TotalDeposit += parseFloat(pVal["DepositedAmount"]);
                    TotalEOD +=  parseFloat(pVal["EndOfDaySales"]);
                    TotalVarD +=  parseFloat(pVal["VarianceD"]);
                    TotalVarE +=  parseFloat(pVal["VarianceE"]);
                    TotalVarF +=  parseFloat(pVal["VarianceF"]);
                });

                vHtml += "<tr>";
                vHtml += "  <td colspan='2'></td>";
                vHtml += "  <td align='right'>" + formatCurrency(TotalDeposit) + "</td>";
                vHtml += "  <td></td>";
                vHtml += "  <td align='right'>" + formatCurrency(TotalEOD) + "</td>";
                vHtml += "  <td align='right'>0.00</td>";
                vHtml += "  <td align='right'>" + formatCurrency(TotalVarD) + "</td>";
                vHtml += "  <td align='right'>" + formatCurrency(TotalVarE) + "</td>";
                vHtml += "  <td align='right'>" + formatCurrency(TotalVarF) + "</td>";
                vHtml += "</tr>";
                $("#obdr-results").html(vHtml);
            }, error: function(){
            }
        });
    }

    function showVarianceGraph(url, BranchName, ReportType){
        if(bdChart) bdChart.destroy();

    }

    function LoadBranches(){
        $.ajax({
            type: "POST",
            url: "LoadBranches/",
            dataType: "json",
            beforeSend: function(){
            }, success: function(mData){
                var vData = mData.d.branches.Rows;
                var vHtml = "";
                $.each(vData, function(key, data){
                    vHtml += "<li id='" + data["BranchID"] + "'>" + data["Name"] + "</li>";
                });
                $("#branch-list ul").html(vHtml);
            }, error: function(){
            }
        });
    }
})(jQuery);

    
//<!-- Original:  Cyanide_7 (leo7278@hotmail.com) -->
//<!-- Web Site:  http://www7.ewebcity.com/cyanide7 -->
//<!-- This script and many more are available free online at -->
//<!-- The JavaScript Source!! http://javascript.internet.com -->
//<!-- Begin
function formatCurrency(num) {
    num = num.toString().replace(/\$|\,/g,'');
    if(isNaN(num))
    num = "0";
    sign = (num == (num = Math.abs(num)));
    num = Math.floor(num*100+0.50000000001);
    cents = num%100;
    num = Math.floor(num/100).toString();
    if(cents<10)
    cents = "0" + cents;
    for (var i = 0; i < Math.floor((num.length-(1+i))/3); i++)
    num = num.substring(0,num.length-(4*i+3))+','+
    num.substring(num.length-(4*i+3));
    return (((sign)?'':'-') + num + '.' + cents);
}
//  End -->

 