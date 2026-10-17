<!DOCTYPE html>
<html>
    <head>
        <meta charset="utf-8">
        <meta name="viewport" content="width=device-width, initial-scale=1">

        <title>Replace</title>

        <link rel="stylesheet" href="../../Scripts3/others/themes/default/jquery.mobile.css" />
        <link rel="stylesheet" href="test1/jqm-docs.css"/>
        <script type="text/javascript" src="../../Scripts/jquery-1.6.1.min.js"></script>
        <script type="text/javascript" src="../../Scripts3/others/jquery.mobile.js"></script>
        <script type="text/javascript" src="../../Scripts/sources/BackOrder/Replace.js"></script>
    </head>

<body>
    <div data-role="page" class="type-interior">  
        <div data-role="header" data-theme="d">
	        <h1> REPLACE? </h1> 
        </div>

        <div data-role="content">
            <div class="content-primary">
                <div style="text-align:center">
                    <h2> REPLACEMENT ITEM </h2> 
                </div>

                <div class="ui-bar-c" style="border:none">
                    <div class="ui-grid-a">                    
	                    <div class="ui-block-a" style="width:43%; text-align:center"> <img src="../../Content/images/box.png" />  </div>
                        <div class="ui-block-b" style="width:57%"> 
                            <h3> SKU #: <span id="ReplaceSKUNo"> </span> <br />
                            <span id="ReplaceSKUDescription"></span> 
                            <a href="#">(MORE INFO)</a> </h3> </div>  
                    </div>                    

                    <div class="ui-grid-a">
                        <div class="ui-block-a"> 
                            <a href="" data-role="button" data-theme="c">
                                QTY Ordered : <br />
                                <h1> <span id="ReplaceQtyOrdered"> </span> </h1>
                                <h3> <span id="ReplaceQtyUnitMeasure"> </span> </h3>
                            </a>
                        </div>

                        <div class="ui-block-b"> 
                            <a href="" data-role="button" data-theme="c">
                                Price in <b> (PhP) </b> : <br />
                                <h1> <span id="ReplacePrice"> </span> </h1>
                                <h3> <span id="ReplaceQtyUnitMeasure2"> </span> </h3>
                            </a>                        
                        </div>  
                    </div>  
                </div>

                <div class="ui-bar-c" style="border:none">                    
	                <div style="text-align:center"> <h3> Sub Total : PhP <span id="ReplaceSubTotal"> </span> </h3> </div>
                </div>

                <br /> <br /> 

                <div style="text-align:center">
                    <h2> ORDERED ITEM </h2> 
                </div>
                
                <div class="ui-bar-c" style="border:none">
                    <div class="ui-grid-a">                    
	                    <div class="ui-block-a" style="width:43%; text-align:center"> <img src="../../Content/images/box.png" /> </div>
                        <div class="ui-block-b" style="width:57%"> 
                            <h3> SKU #: <span id="OriginalSKUNo"> </span> <br /> 
                            <span id="OriginalSKUDescription"></span>
                            <a href="#">(MORE INFO)</a> </h3> </div>  
                    </div>                    

                    <div class="ui-grid-a">
                        <div class="ui-block-a"> 
                            <a href="" data-role="button" data-theme="c">
                                QTY Ordered : <br />
                                <h1> <span id="OriginalQtyOrdered"> </span> </h1>
                                <h3> <span id="OriginalQtyUnitMeasure"> </span> </h3>
                            </a>
                        </div>

                        <div class="ui-block-b"> 
                            <a href="" data-role="button" data-theme="c">
                                Price in <b> (PhP) </b> : <br />
                                <h1> <span id="OriginalPrice"> </span> </h1>
                                <h3> <span id="OriginalQtyUnitMeasure2"> </span> </h3>
                            </a>                        
                        </div>  
                    </div>  
                </div>

                <div class="ui-bar-c" style="border:none">                    
	                <div style="text-align:center"> <h3> Sub Total : PhP <span id="OriginalSubTotal"> </span> </h3> </div>
                </div>
                
                <br /> <br /> 

                <div class="ui-bar-c" style="border:none">                    
                    <a href="#" id="btnReplace" data-role="button" data-theme="c"> 
                    <h3> REPLACE ORDERED ITEM </h3> </a>
                </div>

                <div class="ui-bar-c" style="border:none">                    
                    <a href="#" id="btnWait" data-role="button" data-theme="c"> 
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
