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
    <script type="text/javascript" src="../../Scripts/sources/BackOrder/Wait.js"></script>

    <div data-role="page" data-add-back-btn="true">  
        <div data-role="header" data-theme="d">                    
	        <h1> <div id="pageHeader"> WAIT? </div> </h1> 
        </div>

        <div data-role="content">
            <div class="content-primary">
                <div style="text-align:center">
                    <h2> ORDERED ITEM </h2> 
                </div>
                
                <div class="ui-bar-c" style="border:none">
                    <div class="ui-grid-a">                    
	                    <div class="ui-block-a" style="margin-top:10px; width:43%;text-align:center"> <img src="../../Content/images/box.png" />  </div>
                        <div class="ui-block-b" style="width:57%"> 
                            <h3> SKU #: <span id="WaitSKUNo"> </span> <br />
                            <span id="WaitSKUDescription"></span>
                            <a href="#">(MORE INFO)</a> </h3> </div>  
                    </div>                    

                    <br /> 

                    <div class="ui-grid-a">
                        <div class="ui-block-a"> 
                            <a href="" data-role="button" data-theme="c">
                                QTY Ordered : <br />
                                <h1> <span id="WaitQtyOrdered"> </span> </h1>
                                <h3> <span id="WaitQtyUnitMeasure"> </span> </h3>
                            </a>
                        </div>

                        <div class="ui-block-b"> 
                            <a href="" data-role="button" data-theme="c">
                                Price in <b> PhP </b> : <br />
                                <div> <h1> <span id="WaitPrice"> </span> </h1> </div>
                                <h3> <span id="WaitQtyUnitMeasure2"> </span> </h3>
                            </a>                        
                        </div>  
                    </div>  
                </div>

                <div class="ui-bar-c" style="border:none">                    
	                <div style="text-align:center; vertical-align:middle"> <h3> Sub Total : PhP <span id="WaitSubTotal"> </span> </h3> </div>
                </div>
                
               
                <div class="ui-bar-c" style="border:none">                    
                    <a href="#" id="btnWait" data-role="button" data-rel="dialog" data-theme="c"> 
                    <h3> WAIT FOR ORDERED ITEM </h3> </a>                     
                </div>

                <div class="ui-bar-c" style="border:none">
                    <a href="#" id="btnCancel" data-role="button" data-theme="c">
                    <h3> CANCEL ORDERED ITEM </h3> </a>
                </div>
            </div> 
        </div>
    </div>

</body>
</html>
