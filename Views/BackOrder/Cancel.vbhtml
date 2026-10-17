<!DOCTYPE html>
<html>
    <head>
        <meta charset="utf-8">
        <meta name="viewport" content="width=device-width, initial-scale=1">

        <title>Cancel</title>

        <link rel="stylesheet" href="../../Scripts3/others/themes/default/jquery.mobile.css" />
        <link rel="stylesheet" href="test1/jqm-docs.css"/>
        <script type="text/javascript" src="../../Scripts/jquery-1.6.1.min.js"></script>
        <script type="text/javascript" src="../../Scripts3/others/jquery.mobile.js"></script>        
    </head>
<body>  
    <script type="text/javascript" src="../../Scripts/sources/BackOrder/cancel.js"></script>

    <div id="main" data-role="page" data-add-back-btn="true"> 
        <div data-role="header" data-theme="d">                
	            <h1> <div id="pageHeader"> CANCEL? </div> </h1> 
            </div>            

        <div data-role="content">
                <div class="ui-bar-c" style="border:none">
                <h2> <div style="text-align:center"> Are you sure you want to cancel your back order for this item? </div> </h2> 
                </div>
                    
                    <div class="ui-bar-c" style="border:none">
                    <div class="ui-grid-a">                    
	                    <div class="ui-block-a" style="margin-top:10px; width:43%;text-align:center"> <img src="../../Content/images/box.png" /> </div>
                        <div class="ui-block-b" style="width:57%"> 
                            <h3> SKU #: <span id="SKUNo"> </span> <br /> 
                            <span id="SKUDescription"></span> 
                            <a href="#">(MORE INFO)</a> </h3> </div>  
                    </div>                    

                    <div class="ui-grid-a">
                        <div class="ui-block-a"> 
                            <a href="" id="btnWaitWait" data-role="button" data-theme="c">
                                QTY Ordered : <br />
                                <h1> <span id="QtyOrdered"> </span> </h1>
                                <h3> <span id="QtyUnitMeasure"> </span> </h3>
                            </a>
                        </div>

                        <div class="ui-block-b"> 
                            <a href="" id="btnWaitWait" data-role="button" data-theme="c">
                                Price <b> PhP </b> : <br />
                                <h1> <span id="Price"> </span> </h1>
                                <h3> <span id="QtyUnitMeasure2"> </span> </h3>
                            </a>                        
                        </div>  
                        </div>  
                    </div>

                    <div class="ui-bar-c" style="border:none">                    
	                    <div style="text-align:center"> <h3> Sub Total : PhP <span id="SubTotal"> </span> </h3> </div>
                    </div>

                    <div class="ui-bar-c" style="border:none">
                        <a href="#" id="btnCancel" data-role="button" data-theme="c"> 
                            <h3> CANCEL ORDERED ITEM </h3> </a>                         
                    </div>

                    <div class="ui-bar-c" style="border:none">                        
                        <a href="#" id="btnWait" data-role="button" data-theme="c">
                            <h3> WAIT FOR ORDERED ITEM </h3> </a>                         
                    </div>
       </div>
    </div>     
</body>
</html>

