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
           <div class="ui-block-a" style="margin-top:5px"> Stock Number :  </div>
           <div class="ui-block-b"> Description : <div id=""> </div> </div>                           
        </div>
        <div id="bd-loading"></div>
            <br />

        <div class="ui-grid-a">            
            <div class="ui-grid-a">
	            <div class="ui-block-a">Qty</div>
	            <div class="ui-block-b">Unit Measure</div>	            
            </div>
        </div>      
    </div>
    <div id="bID">

    </div>
</body>
</html>
