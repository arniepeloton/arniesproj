@Code
    ViewData("Title") = "Bulk Price Adjustment"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/PIMProductPrice/UploadDownloadExcelFile.js?crc=')
            ]);
    </script>
    
End Section

<div id="pagePIMTemplate" data-role="page">
    <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>


    <div data-role="content">
    To change the prices of multiple products by category or brand, please download the product list below. 
    Once you have downloaded the file, you can update the values of the PRICE column. Save the file and upload it back to update the price.
    
    <h2> Download Price List</h2>
         <div data-role="fieldcontain">
            <label for="selFilter" class="select">Filter:</label>
            <select id="selFilter">
            <option value="">All Products</option>
            <option value="Brand">By Brand</option>
            <option value="Category">By Category</option>
            </select>
        </div>
	    <div data-role="fieldcontain">
            <label for="selPriceLevel" class="select">Price Level:</label>
            <select id="selPriceLevel">
            <option value="">All Price Levels</option>
            <option value="01000012-0005-0000-0000-000000000000">Retail Price</option>
            <option value="01000012-0004-0000-0000-000000000000">Wholesale Price</option>
            <option value="EEF716C6-B290-4B28-B2A6-0A4F52CF4989">SRP (Suggested Retail Price)</option>
            <option value="01000012-0001-0000-0000-000000000000">FREIGHT COST</option>
            <option value="01000012-0002-0000-0000-000000000000">COST FROM VENDOR</option>
            <option value="01000012-0003-0000-0000-000000000000">LANDED COST</option>
            </select>
        </div>          
            <div id="divResult" style="display:none;">
                <ul data-role="listview" id="lstResultContainer" data-inset="true">   
                    <li>
                        <table id="tblResult" style="width:100%">                    
                        </table>  
                    </li>
                </ul>
            </div>
                            
        <a data-role="button" id="btnDownloadByFilter" >Download Excel File</a>   
     <br /><br />           
                    
     <hr /><br />
     <h2> Upload Price List</h2>
    Note: This feature only updates the prices of EXISTING products in the organization with matching price levels. 
    <br /> <br />
        <form action="Index" enctype="multipart/form-data" method="post">

            <div style="display:none;"><input type="text" id="OrgID" name="OrgID" style="display:none;"></div>
            <input type="file" id="objFile" name="objFile">
            <input type="submit" value="Upload Excel File">
        </form>

        <br /><br />
        <div style="text-align: center">
            <a id="btnBackToHome" data-role="button"><h3>Back to Home</h3></a>
        </div>
   
    </div>
</div>

