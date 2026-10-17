@Code
    ViewData("Title") = "Sales Adjustment Report"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/ReportsPOS/SalesAdjustmentReport.js?crc=')
            ]);
    </script>
    
End Section

<div id="pageSalesReport" data-role="page">
    <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>

    <div data-role="content">

   @*     @If Common.WebRequest.QueryString("IsMobile") <> "1" Then
            @<applet name="jzebra" code="jzebra.PrintApplet.class" archive="/Scripts/jzebra.jar" width="10" height="10">
            </applet>
        End If*@

        <div data-role="fieldcontain">
	        <label for="selPOSSalesReportRange" class="select">Show sales adjustment report range</label>
            <select id="selPOSSalesReportRange">
	            <option value="">Please Select</option>
				<option value="Today">Today</option>
				<option value="Yesterday">Yesterday</option>
				<option value="Day">Range by day</option>
				<option value="Week">Range by week</option>
				<option value="Month">Range by month</option>
				<option value="Quarter">Range by quarter</option>
				<option value="Year">Range by year</option>
            </select>
        </div>

        <div id="divPOSSalesReportRangeByDate" style="display:none;">

            <div data-role="fieldcontain">
	            <label>From: </label>
                <fieldset data-role="controlgroup" data-type="horizontal">
                    <select id="selPOSSalesReportByDateFromMonth">
	                    <option value="">MMM</option>
                    </select>
                    <select id="selPOSSalesReportByDateFromDay">
	                    <option value="">DD</option>
                    </select>                
                    <select id="selPOSSalesReportByDateFromYear">
	                    <option value="">YYYY</option>
                    </select>                
                </fieldset>
            </div>

            <div data-role="fieldcontain">
	            <label>To: </label>
                <fieldset data-role="controlgroup" data-type="horizontal">
                    <select id="selPOSSalesReportByDateToMonth">
	                    <option value="">MMM</option>
                    </select>
                    <select id="selPOSSalesReportByDateToDay">
	                    <option value="">DD</option>
                    </select>                
                    <select id="selPOSSalesReportByDateToYear">
	                    <option value="">YYYY</option>
                    </select>                
                </fieldset>
            </div>

        </div>

        <div id="divPOSSalesReportRangeByWeek" style="display:none;">

            <div data-role="fieldcontain">
	            <label for="selPOSSalesReportRangeByWeekFrom" class="select">From:</label>
                <select id="selPOSSalesReportRangeByWeekFrom">
	                <option value="">Please Select</option>
                </select>
            </div>

            <div data-role="fieldcontain">
	            <label for="selPOSSalesReportRangeByWeekTo" class="select">To:</label>
                <select id="selPOSSalesReportRangeByWeekTo">
	                <option value="">Please Select</option>
                </select>
            </div>

            <div data-role="fieldcontain">
	            <label for="selPOSSalesReportRangeByWeekYear" class="select">Year:</label>
                <select id="selPOSSalesReportRangeByWeekYear">
	                <option value="">Please Select</option>
	                <option value="This year.">This year.</option>
	                <option value="This year and last year.">This year and last year.</option>
	                <option value="This year and last 2 years.">This year and last 2 years.</option>
	                <option value="This year and last 3 years.">This year and last 3 years.</option>
                </select>
            </div>

        </div>

        <div id="divPOSSalesReportRangeByMonth" style="display:none;">

            <div data-role="fieldcontain">
	            <label for="selPOSSalesReportRangeByMonthFrom" class="select">From:</label>
                <select id="selPOSSalesReportRangeByMonthFrom">
	                <option value="">Please Select</option>
                </select>
            </div>

            <div data-role="fieldcontain">
	            <label for="selPOSSalesReportRangeByMonthTo" class="select">To:</label>
                <select id="selPOSSalesReportRangeByMonthTo">
	                <option value="">Please Select</option>
                </select>
            </div>

            <div data-role="fieldcontain">
	            <label for="selPOSSalesReportRangeByMonthYear" class="select">Year:</label>
                <select id="selPOSSalesReportRangeByMonthYear">
	                <option value="">Please Select</option>
	                <option value="This year.">This year.</option>
	                <option value="This year and last year.">This year and last year.</option>
	                <option value="This year and last 2 years.">This year and last 2 years.</option>
	                <option value="This year and last 3 years.">This year and last 3 years.</option>
                </select>
            </div>

        </div>

        <div id="divPOSSalesReportRangeByQuarter" style="display:none;">

            <div data-role="fieldcontain">
	            <label for="selPOSSalesReportRangeByQtrFrom" class="select">From:</label>
                <select id="selPOSSalesReportRangeByQtrFrom">
	                <option value="">Please Select</option>
	                <option value="1">1st Quarter</option>
	                <option value="2">2nd Quarter</option>
	                <option value="3">3rd Quarter</option>
	                <option value="4">4th Quarter</option>
                </select>
            </div>

            <div data-role="fieldcontain">
	            <label for="selPOSSalesReportRangeByQtrTo" class="select">To:</label>
                <select id="selPOSSalesReportRangeByQtrTo">
	                <option value="">Please Select</option>
	                <option value="1">1st Quarter</option>
	                <option value="2">2nd Quarter</option>
	                <option value="3">3rd Quarter</option>
	                <option value="4">4th Quarter</option>
                </select>
            </div>

            <div data-role="fieldcontain">
	            <label for="selPOSSalesReportRangeByQtrYear" class="select">Year:</label>
                <select id="selPOSSalesReportRangeByQtrYear">
	                <option value="">Please Select</option>
	                <option value="This year.">This year.</option>
	                <option value="This year and last year.">This year and last year.</option>
	                <option value="This year and last 2 years.">This year and last 2 years.</option>
	                <option value="This year and last 3 years.">This year and last 3 years.</option>
                </select>
            </div>
        </div>

        <div id="divPOSSalesReportRangeByYear" style="display:none;">

            <div data-role="fieldcontain">
	            <label for="selPOSSalesReportRangeByYear" class="select">Year:</label>
                <select id="selPOSSalesReportRangeByYear">
                </select>
            </div>

        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnPOSSalesAdjReportPrint" data-role="button"><h3>Print</h3></a> </div>
	        <div style="text-align:center"> <a id="btnPOSSalesAdjReportCancel" data-role="button" data-rel="back"><h3>Back To Home</h3></a></div>
        </div>

        <div data-role="popup" id="popupMenu" >
            <div data-role="popup" id="dvpopup" data-theme="a" class="ui-corner-all" data-dismissible="false" data-overlay-theme="a">
                <p id="popMessage" style="padding:10px 20px;text-align:center"></p>
            </div>
        </div>

    </div>

</div>

