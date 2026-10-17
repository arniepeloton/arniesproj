@Code
    ViewData("Title") = "Report to Principal"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head
    <script>
        require([GetPathFormat('/Scripts/sources/ReportToPrincipal/ReportToPrincipal.js?crc=')]);        
    </script>
End Section

<div id="pageMain" data-role="page">
    <div data-role="header">        
        <h1>@ViewData("Title")</h1>
    </div>

    <div data-role="content">       
        <ul data-role="listview" data-inset="true">
            <li data-role="list-divider"> Nike SEA Report </li> 
            <li> 
                <div data-role="fieldcontain">	            
                    <fieldset data-role="controlgroup" data-type="horizontal">
                        <legend id="lblPOSReportRangeByDateFrom"> From : </legend>                
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

                <div data-role="fieldcontain" id="divPOSReportRangeByDateTo">	            
                    <fieldset data-role="controlgroup" data-type="horizontal">
                        <legend> To : </legend>                
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
            </li> 
        </ul> 
        <div> <a id="btnGenerateWeeklySalesReport" data-role="button">Generate Sales Report</a> </div>
        <div> <a id="btnGenerateWeeklyInventoryReport" data-role="button">Generate Weekly Inventory Report</a> </div>
        <div> <a id="btnGenerateMonthlyInventoryReport" data-role="button">Generate Monthly Inventory Report</a> </div>
    </div>
</div>