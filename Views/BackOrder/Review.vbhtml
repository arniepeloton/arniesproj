<!DOCTYPE html>
<html>
    <head>
        <meta charset="utf-8">
        <meta name="viewport" content="width=device-width, initial-scale=1">

        <title>Review</title>

        <link rel="stylesheet" href="../../Scripts3/others/themes/default/jquery.mobile.css" />
        <link rel="stylesheet" href="test1/jqm-docs.css"/>
        <script type="text/javascript" src="../../Scripts/jquery-1.6.1.min.js"></script>
        <script type="text/javascript" src="../../Scripts3/others/jquery.mobile.js"></script>        
        <script type="text/javascript" src="../../Scripts/sources/BackOrder/Review.js"></script>
    </head>
<body>  
    <div id="main" data-role="page" data-add-back-btn="true"> 
        <div data-role="header" data-theme="d">                
	        <h1> <div id="pageHeader"> REVIEW </div> </h1> 
        </div>            

        <div data-role="content">            
            <div id="index">            
                <div>
                    <h1>
                        REVIEW & CONFIRM
                    </h1>
                </div>

                <div>
                    <h4>Please review your back order responses before clicking on the "Confirm Back Order Response" button below. <br /> <br />
                    If you would like to make changes to your response at this time, please press the back button on your phone or browser now. </h4>

                </div>
                <div class="ui-bar-c" style="border:none">
                    <div class="ui-grid-a">
                        <br />
                        <ul id="lstItems" data-role="listview" style="margin-right:3px;">
                            
                        </ul>
                    </div>                    
                </div>

                <br /> <br /> 

                <div class="ui-bar-c" style="border:none">
                    <i> Please note that you can no longer change your reponse after you click on this button </i> <br />
                    <i> Press back if you want to make changes. </i>
                </div>
                <br />

                <div class="ui-bar-c" style="border:none">
                    <a href="#" id="btnDone" data-role="button" data-theme="c"> 
                    <h3> Confirm Back Order Response </h3>                       
                    </a>  
                </div>

                

            </div>
        </div>
    </div>
</body>
</html>

