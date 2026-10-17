<html>
<head>    
    <title>
        @Code
            ViewData("Title") = "Items"
            Layout = "~/Views/CycleCount/CycleCountMaster.vbhtml"
        End Code
    </title>

    <script type="text/javascript" src="../../Scripts/sources/CycleCount/SKU.js"></script>
</head>
<body>    <div>
        
        <div id="mainbody">

        @*<div id="result"></div>
        <div>Barcode/QrCode Scanner in this section</div> <br />*@

        <div data-role="footer" data-theme="a">            
           <div class="ui-block-a" style="margin-top:5px"> Search :  </div>
           <div class="ui-block-b"> <input type="text" style="font-weight:bold" id="txtSearch" placeholder="Item Description" data-theme="a"/> </div>                
           <div class="ui-block-c"> <button id="Search" data-theme="a" style="width:"10px"> Q </button> </div>
        </div>
        <div id="bd-loading"></div>
            <br />

        <div class="ui-grid-a">
            
            <div id="lstItems" data-role="listview">
                
            </div>

            </div>      
    </div>
    <div id="bID">

    </div>
</body>
</html>
