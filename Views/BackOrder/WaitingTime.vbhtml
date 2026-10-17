<!DOCTYPE html>
<html>
    <head>
        <meta charset="utf-8">
        <meta name="viewport" content="width=device-width, initial-scale=1">

        <title>Wait</title>

        <link rel="stylesheet" href="../../Scripts3/others/themes/default/jquery.mobile.css" />
        <link rel="stylesheet" href="test1/jqm-docs.css"/>
        <script type="text/javascript" src="../../Scripts/jquery-1.6.1.min.js"></script>
        <script type="text/javascript" src="../../Scripts3/others/jquery.mobile.js"></script>        
    </head>
<body>  
    <script type="text/javascript" src="../../Scripts/sources/BackOrder/WaitingTime.js"></script>
    <script type="text/javascript" src="../../Scripts/sources/BackOrder/root.js"></script>

    <div id="reviewCount" data-role="page" data-url="popup">
	    <div data-role="header" data-theme="d">
		    <h1>WAIT</h1>
	    </div><!-- /header -->

	    <div data-role="content" data-theme="d">	 
             <div class="ui-bar-c" style="border:none">
                    I can wait until: <br />
                    <select id="optWaitingOptions">
                        <option>-Select Value-</option>
				        <option value="1">Tomorrow</option>
				        <option value="2">2 days from today</option>
                        <option value="3">3 days from today</option>
                        <option value="4">Next week</option>
                        <option value="5">2 Weeks from today</option>
                        <option value="6">Next Month </option>
                        <option value="7">Custom</option>
			        </select>
                </div>

                <br />

                <div class="ui-bar-c" style="border:none">
                    ... or specify a custom due date for waiting (Please pick a date after today): 
                            <select id="Smonth">
	                            <option>Month</option>
	                            <option value="0">January</option>
					            <option value="1">February</option>
					            <option value="2">March</option>
					            <option value="3">April</option>
                                <option value="4">May</option>
                                <option value="5">June</option>
                                <option value="6">July</option>
                                <option value="7">August</option>
                                <option value="8">September</option>
                                <option value="9">October</option>
                                <option value="10">November</option>
                                <option value="11">December</option>
                            </select>
                </div>

	            <div class="ui-bar-c" style="border:none">
                    <div class="ui-grid-a">
                        <div class="ui-block-a" >
                            <select id="Sday">	                            
                            </select>
                        </div>
                        <div class="ui-block-b">
                            <select id="Syear">	                            
                            </select>
                        </div>
                    </div>
                </div>

                <div id="Waitingday"> </div>

                <br /> <br />

                <div class="ui-bar-c" style="border:none">
                    <a href="#popup" id="btnWait" data-role="button" data-theme="c"> 
                    <h3> DONE </h3> </a>                     
                </div>
	    </div> <!-- /content -->
	
	    <div data-role="footer">
		    @*<h4>Cycle Count</h4> *@
	    </div><!-- /footer -->
    </div>
    </body>
</html>