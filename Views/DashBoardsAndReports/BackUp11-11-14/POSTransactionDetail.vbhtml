@Code
    ViewData("Title") = "POS Transaction Detail"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
@Section head
    @*<link rel="stylesheet" href="/Content/css/jquery.jqplot.min.css" />
    <link rel="stylesheet" href="/Content/css/ui.dynatree.css" />
    <link rel="stylesheet" href="/Content/css/qunit.css" />*@
    <script>
        require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/POSTransactionDetail.js?crc=')]);
    </script>
End Section
<div data-role="page">
    <div data-role="header">
        <h1>@ViewData("Title")</h1>
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
    </div>
    <div data-role="content">
        <ul data-role="listview" data-inset="true">
            <li data-role="list-divider" id="liMainChart">POS Transaction Detail Parameters</li>
            <li>
                <div data-role="fieldcontain">
                    <label for="selTransactionRangeType">
                        Transaction Range Type :
                    </label>
                    <select id="selTransactionRangeType">
                        <option value="" selected="selected"> Select Range Type </option>
                        <option value="optMonth">Month </option>
                        <option value="optDate">Date </option>
                    </select>
                </div>
                <div id="dvTransactionRangeType" style="visibility:hidden; display:none">
                    <div data-role="fieldcontain">
                        <label for="selPOSTransactionFOWST">
                            FOWST :
                        </label>
                        <select id="selPOSTransactionFOWST">
                            <option value="" selected="selected"> Select FOWST</option>
                        </select>
                    </div>

                    <div data-role="fieldcontain" id="dvMonth" style="visibility:hidden; display:none">
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <legend>Month :</legend>                        
                            <select id="selMonth">
                                <option value="" selected="selected">Select Month </option>
                                <option value="1">January </option>
                                <option value="2">February </option>
                                <option value="3">March </option>
                                <option value="4">April </option>
                                <option value="5">May </option>
                                <option value="6">June </option>
                                <option value="7">July </option> 
                                <option value="8">August</option>
                                <option value="9">September </option>
                                <option value="10">October </option>
                                <option value="11">November </option>
                                <option value="12">December </option>
                            </select>
                            <select id="selYear">
                                <option value="" selected="selected">Select Year </option>
                                <option value="2013">2013 </option>
                                <option value="2014">2014 </option>
                                <option value="2015">2015 </option>
                            </select>
                        </fieldset>
                    </div>
                    <div id="dvDay" style="visibility:hidden; display:none"> 
                        <div data-role="fieldcontain">
                            <fieldset data-role="controlgroup">
                                <legend>Date From : </legend>
                                <fieldset id="txtDateFrom" data-role="DateCombo" data-usecurrent="1">
                                </fieldset>
                            </fieldset>
                        </div>
                        <div data-role="fieldcontain" data-role="controlgroup">
                            <fieldset data-role="controlgroup">
                                <legend>Date To : </legend>
                                <fieldset id="txtDateTo" data-role="DateCombo" data-usecurrent="1">
                                </fieldset>
                            </fieldset>
                        </div>
                    </div>
                    <a id="btnGenerateData" href="#" data-role="button">Download Excel Report File</a>
                </div>
            </li>
        </ul>
        

        <ul data-role="listview" data-inset="true" style="display:none;">
            <li data-role="list-divider">Raw Data</li>
            <li>
                <table data-role="table" id="tblPOSRawDataTable" data-mode="reflow" class="ui-responsive"> 
                    <thead>
                        <tr>
                            <th> Account ID </th>
                            <th> Store ID </th>        
                            <th> MIN </th>            
                            <th> Transaction </th> 
                            <th> Transaction Date </th>
                            <th> UPC Code </th>
                            <th> Sold Qty </th>
                            <th> Sold Amt </th>
                            <th> Sold Amt Discount </th>
                            <th> Transaction Type </th>
                            <th> MONTH </th>
                            <th> YEAR </th>
                            <th> Reference SI </th>                            
                        </tr>
                    </thead>
                    <tbody id="tblPOSRawDataBody">
                   
                    </tbody>
                </table>            
            </li>
        </ul>
        <ul data-role="listview" data-inset="true" style="display:none;">
            <li data-role="list-divider">Summary</li>
            <li>
                <table data-role="table" id="tblPOSSummary" data-mode="reflow" class="ui-responsive"> 
                    <thead>
                        <tr>
                            <th> Company Name </th>
                            <th> FOWST </th>        
                            <th> MIN </th>            
                            <th> Total Sales </th> 
                            <th> Total Discounts </th> 
                            <th> Transaction Type </th>
                            <th> MONTH </th>
                            <th> YEAR </th>
                            <th> OR Start Range </th>
                            <th> OR End Range </th>
                            <th> Is Series Skip?</th>
                        </tr>
                    </thead>
                    <tbody id="tblPOSSummaryBody">
                   
                    </tbody>
                </table>
            </li>
        </ul>
    </div>
</div>
