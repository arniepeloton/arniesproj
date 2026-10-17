@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/DevicePrinter/DevicePrinterSummary.js?crc=')]);
    </script>
    
End Section

<div id="pageSumDevicePrinter" data-role="page">

    <div id"divHeader" data-role="header">
        <h1>Printer Information Summary</h1>
    </div>

    <div data-role="content" >

        <div>
            <ul data-role="listview" data-inset="true">

				<li data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Printer Information
                        <a href='#' id='btnEditDevicePrinter' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li>
                    <div data-role="fieldcontain" style="display:none;">
	                    <input type="text" readonly id="txtSumDevicePrinterID"/>                
                    </div>

                    <div data-role="fieldcontain">
                     
	                        <label for="txtSumDevicePrinterRole">Printer Role: </label>
	                        <input type="text" readonly id="txtSumDevicePrinterRole"/>
     
                    </div>

                    <div data-role="fieldcontain">
               
	                        <label for="txtSumDevicePrinterType">Printer Type: </label>
	                        <input type="text" readonly id="txtSumDevicePrinterType"/>

                    </div>

                    <div id="divSumCategoriesList" style="display:none">
                 
                            <div><label for="txtSumDevicePrinterType">Categories to Print: </label></div>
                           
                                <div>
                                    <ul data-role="listview" id="lstSumDevicePrinterCategories" data-inset="true">
                                    </ul>
                                </div>
                          
                  
                        <div data-role="fieldcontain">
                            <label><input type="checkbox" disabled id="chkSumIsActive" /> Active</label>
                        </div>

                    </div>
                </li>

              
			</ul>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align:center"> <a id="btnDevicePrinterSummaryBack" data-role="button">Done</a> </div>
        </div>
    </div>

</div>

