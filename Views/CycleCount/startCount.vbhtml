<html>
<head>
    <title>
        @Code
            ViewData("Title") = "Count"
            Layout = "~/Views/CycleCount/CycleCountMaster.vbhtml"
        End Code
    </title>
    
    <script type="text/javascript" src="../../Scripts/sources/CycleCount/startCount.js"></script>

    <style> 
            .ui-mblock-a {width: 25%; float:left;}
            .ui-mblock-b {width: 37.5%; float:left;}
            .ui-mblock-c {width: 50%; float:left;}
            .ui-mblock-d {width: 75%; float:left;}
    </style>
</head>

<body id="skuID">
        <div id="main" data-role="page">
            <div data-role="controlgroup" data-type="horizontal">
                <a id="btnBackSpace" class="ui-mblock-a" href=""#"" data-role="button">Backspace</a>
	            <input type="text"  name="name" id="txtQuantity" value="0" style="text-align:right; font-weight:bold; width:73%; padding:10px;">
            </div>
            @*value disabled*@
            <div data-role="controlgroup" data-type="horizontal">
                <a id="btnRemarks" class="ui-mblock-b" href="#remarks" data-rel="dialog" data-transition="pop" data-role="button" data-theme="b">Remarks</a>
                <a id="btnExpDate" class="ui-mblock-b" href="#expDate" data-rel="dialog" data-transition="pop" data-role="button" data-theme="b"> Exp. Date </a>
                <a id="btnReviewCount" class="ui-mblock-a" href="#" data-role="button" data-theme="b">Review Count</a>            
            </div>        

            <div data-role="controlgroup" data-type="horizontal">
                <a id="btn7" class="ui-mblock-a" href=""#"" data-role="button">7</a>
                <a id="btn8" class="ui-mblock-a" href=""#"" data-role="button">8</a>
                <a id="btn9" class="ui-mblock-a" href=""#"" data-role="button">9</a>
                <a class="ui-mblock-a" href=""#"" data-role="button" data-theme="e"> <div id="btnUnitMeasure1"> </div> </a>
            </div>

            <div data-role="controlgroup" data-type="horizontal">
                <a id="btn4" class="ui-mblock-a" href=""#"" data-role="button">4</a>
                <a id="btn5" class="ui-mblock-a" href=""#"" data-role="button">5</a>
                <a id="btn6" class="ui-mblock-a" href=""#"" data-role="button">6</a>
                <a class="ui-mblock-a" href=""#"" data-role="button" data-theme="e"> <div id="btnUnitMeasure2"> </div> </a>
            </div>

            <div data-role="controlgroup" data-type="horizontal">
                <a id="btn1" class="ui-mblock-a" href=""#"" data-role="button">1</a>
                <a id="btn2" class="ui-mblock-a" href=""#"" data-role="button">2</a>
                <a id="btn3" class="ui-mblock-a" href=""#"" data-role="button">3</a>
                <a class="ui-mblock-a" href="#unitMeasure" data-rel="dialog" data-transition="pop" data-role="button" data-theme="e"> <div id="btnUnitMeasure3"> </div> </a>
            </div>

            <div data-role="controlgroup" data-type="horizontal">
                <a id="btn." class="ui-mblock-a" href=""#"" data-role="button"> . </a>
                <a id="btn0" class="ui-mblock-a" href=""#"" data-role="button">0</a>
                <a id="btnDone" class="ui-mblock-c" href=""#"" data-role="button">Done</a>            
            </div>

        </div>

        <div id="unitMeasure" id="popup" data-url="popup">
	        <div data-role="header" data-theme="e">
		        <h1>UnitMeasures</h1>
	        </div><!-- /header -->

	        <div data-role="content" data-theme="d">	
		        <div id="lstUnitMeasures" data-role="listview"> </div>                
	        </div><!-- /content -->
	
	        <div data-role="footer">
		        <h4>Cycle Count</h4>
	        </div><!-- /footer -->
        </div>

        <div id="remark data-role="page" id="popup" data-url="popup">
	        <div data-role="header" data-theme="e">
		        <h1>Remarks</h1>
	        </div><!-- /header -->

	        <div data-role="content" data-theme="d">			        
                
	                <div> <textarea name="textarea" id="textarea"></textarea> </div>
                    <div style="text-align:center"> <a id="btnRemarks" href="#main" data-role="button">Done</a> </div>
                
	        </div><!-- /content -->
	
	        <div data-role="footer">
		        <h4>Cycle Count</h4>
	        </div><!-- /footer -->
        </div>

        <div id="expDate" data-role="page" id="popup" data-url="popup">
	        <div data-role="header" data-theme="e">
		        <h1>Expiration Date</h1>
	        </div><!-- /header -->

	        <div data-role="content" data-theme="d">                
	     	        <input id="datepicker" type="text">
			        <div style="text-align:center"> <a id="btnDatePicker" href="#main" data-role="button">Done</a> </div>
	        </div> <!-- /content -->
	
	        <div data-role="footer">
		        <h4>Cycle Count</h4> 
	        </div><!-- /footer -->
        </div>

        <a href="#startCounting" id="lnkCounting" data-rel="dialog" data-transition="pop"></a>

        <div id="startCounting" data-role="page" id="popup" data-url="popup">
	        <div data-role="header" data-theme="e">
		        @*<h1>Start</h1> *@
	        </div><!-- /header -->

	        <div data-role="content" data-theme="d">	 
                <br /> <br /> 
			        <div style="text-align:center"> <a id="btnStartCounting" href="#main" data-role="button">Start Counting</a> </div>
                <br /> <br /> 
	        </div> <!-- /content -->
	
	        <div data-role="footer">
		        <h4>Cycle Count</h4>
	        </div> <!-- /footer -->
        </div>

        <div id="reviewCount" data-role="page" id="popup" data-url="popup">
	        <div data-role="header" data-theme="e">
		        <h1>Review Count</h1>
	        </div><!-- /header -->

	        <div data-role="content" data-theme="d">	 
                <div>
                
                </div>
	        </div> <!-- /content -->
	
	        <div data-role="footer">
		        @*<h4>Cycle Count</h4> *@
	        </div><!-- /footer -->
        </div>
</body>
</ht>

