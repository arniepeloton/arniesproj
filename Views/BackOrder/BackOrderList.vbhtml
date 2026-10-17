<!DOCTYPE html>
<html>
    <head>
        <meta charset="utf-8">
        <meta name="viewport" content="width=device-width, initial-scale=1">

        <title>Items</title>

        <link rel="stylesheet" href="../../Scripts3/others/themes/default/jquery.mobile.css" />
        <link rel="stylesheet" href="test1/jqm-docs.css"/>
        <script type="text/javascript" src="../../Scripts/jquery-1.6.1.min.js"></script>
        <script type="text/javascript" src="../../Scripts3/others/jquery.mobile.js"></script>
    </head>

<body>  
    <script type="text/javascript" src="../../Scripts/sources/BackOrder/index.js"></script> 

    <div id="main" data-role="page" class="type-interior"> 
        
            <div data-role="header" data-theme="d">                
	            <h1> Back Order Response </h1> 
            </div>    

        <div data-role="content">            
            <div class="content-primary">
                <br />
                <div class="ui-bar-c" style="border:none">
                    <div class="ui-grid-a">                        
	                    <div class="ui-block-a"> Customer: </div>
                        <div class="ui-block-b"> <span id="customer"> </span> </div> 
                        <div class="ui-block-a"> Tel: </div> 
                        <div class="ui-block-b"> <span id="telNo"> </span> </div> 
                        <div class="ui-block-a"> Mobile: </div> 
                        <div class="ui-block-b"> <span id="Mobile"> </span> </div>
                        <div class="ui-block-a"> Email: </div> 
                        <div class="ui-block-b"> <span id="Email"> </span ></div> 
                        <div class="ui-block-a"> Sales Order(SO): </div>
                        <div class="ui-block-b"> <span id="SalesOrder"> </span ></div> 
                        <div class="ui-block-a"> SO Date: </div> 
                        <div class="ui-block-b"> <span id="SODate"> </span ></div>                
                    </div>
                    <br />
                </div>

                <div class="ui-bar-c" style="border:none">
                    <div class="ui-grid-a">           
                            <h2> BACK ORDER ITEMS </h2>

                        <ul id="lstItems" data-role="listview" data-theme="d" data-divider-theme="d" style="margin:3px">
                        </ul>
                    </div>                    
                </div>

                <div class="ui-bar-c" style="border:none">
                    <a href="#" id="btnDone" data-role="button" data-theme="c"> 
                    <h3> REVIEW AND CONFIRM </h3> </a>  
                </div>
            </div>
        </div>
        </div>
</body>
</html>

