@Code
    ViewData("Title") = "Data Integrity Test"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
<script>
    require([GetPathFormat('/Scripts/sources/SelfTest/SelfTest.js?crc=')]);
</script>
<div id="pageMain" data-role="page">
    <div data-role="header">        
        <h1>@ViewData("Title")</h1>
    </div>
    <div data-role="content" >
       <div>
        
            <div id="msg1" style="display:none;">
                <div class="ui-grid-a" >
                    <div class="ui-block-a" style="width: 90%">
                        Initializing data integrity test ...
                        
                    </div>
                    <div class="ui-block-b" style="text-align:right;width: 10%">
                        <img id="imgLoading1" src="/Content/images/ajax-loader.gif"/>
                        <img id="imgSuccess1" src="/Content/images/tick.png" style="display:none;"/>
                        <img id="imgFail1" src="/Content/images/cross.png" style="display:none;"/>
                    </div>
                
                </div>

                <br />
            </div>

            <div id="msg2" style="display:none;">
                <div class="ui-grid-a"> 
                    <div class="ui-block-a" style="width: 90%">
                        Generating random transaction data ...
                        
                    </div>
                    <div class="ui-block-b" style="text-align:right;width: 10%">
                        <img id="imgLoading2" src="/Content/images/ajax-loader.gif"/>
                        <img id="imgSuccess2" src="/Content/images/tick.png" style="display:none;"/>
                        <img id="imgFail2" src="/Content/images/cross.png" style="display:none;"/>
                    </div>
               
                </div>
                <br />
            </div>

            <div id="msg3" style="display:none;">
                <div class="ui-grid-a">
                    <div class="ui-block-a" style="width: 90%">
                        Processing data ...
                    </div>
                    <div class="ui-block-b" style="text-align:right;width: 10%">
                        <img id="imgLoading3" src="/Content/images/ajax-loader.gif"/>
                        <img id="imgSuccess3" src="/Content/images/tick.png" style="display:none;"/>
                        <img id="imgFail3" src="/Content/images/cross.png" style="display:none;"/>
                    </div>
                
                </div>
                <br />
         </div>

           <div id="msg4" style="display:none;">
                <div class="ui-grid-a">
                    <div class="ui-block-a" style="width: 90%">
                        Comparing local data ...
                        
                    </div>
                    <div class="ui-block-b" style="text-align:right;width: 10%">
                        <img id="imgLoading4" src="/Content/images/ajax-loader.gif"/>
                        <img id="imgSuccess4" src="/Content/images/tick.png" style="display:none;"/>
                        <img id="imgFail4" src="/Content/images/cross.png" style="display:none;"/>
                    </div>
                </div>
                <br />
            </div>

            <div id="msg5" style="display:none;">
                <div class="ui-grid-a">
                    <div class="ui-block-a" style="width: 90%">
                        Result: Passed
                    
                    </div>
                    <div class="ui-block-b" style="text-align:right;width: 10%">
                        <img id="imgSuccess5" src="/Content/images/tick.png"/>
                    </div>
           
                </div>
                <br />
           </div>

           <div id="msg6" style="display:none;">
                <div class="ui-grid-a">
                    <div class="ui-block-a" style="width: 90%">
                        Result: Failed
                    
                    </div>
                    <div class="ui-block-b" style="text-align:right;width: 10%">
                        <img id="imgFail6" src="/Content/images/cross.png""/>
                    </div>
       
                </div>
                <br />
            </div>
    
            <div id="msg8" style="display:none;">
                <div class="ui-grid-a">
                    <div class="ui-block-a" style="width: 90%">
                        Flushing data to server ...
                        
                    </div>
                    <div class="ui-block-b" style="text-align:right;width: 10%">
                        <img id="imgLoading8" src="/Content/images/ajax-loader.gif"/>
                        <img id="imgSuccess8" src="/Content/images/tick.png" style="display:none;"/>
                        <img id="imgFail8" src="/Content/images/cross.png" style="display:none;"/>
                    </div>
           
                </div>
                <br />
          </div>
          
            <div id="msg9" style="display:none;">
                <div class="ui-grid-a">
                    <div class="ui-block-a" style="width: 90%">
                        Comparing local data versus server data ...
                      
                    </div>
                    <div class="ui-block-b" style="text-align:right;width: 10%">
                        <img id="imgLoading9" src="/Content/images/ajax-loader.gif"/>
                        <img id="imgSuccess9" src="/Content/images/tick.png" style="display:none;"/>
                        <img id="imgFail9" src="/Content/images/cross.png" style="display:none;"/>
                    </div>
         
                </div>
                  <br />
            </div>

            <div id="msg10" style="display:none;">

                <div class="ui-grid-a">
                    <div class="ui-block-a" style="width: 90%">
                        Result: Passed
                       
                    </div>
                    <div class="ui-block-b" style="text-align:right;width: 10%">
                        <img id="imgSuccess10" src="/Content/images/tick.png" />
                    </div>
            
                </div>
                 <br />
            </div>
       
            <div id="msg11" style="display:none;">

                <div class="ui-grid-a">
                    <div class="ui-block-a" style="width: 90%">
                        Result: Failed
                    
                    </div>
                    <div class="ui-block-b" style="text-align:right;width: 10%">
                        <img id="imgFail11" src="/Content/images/cross.png"/>
                    </div>
             
                </div>
                <br />
              </div>
            <br />
            <div id ="divSuccessTest" style="display:none;">Thank you for waiting. The system has successfully tested your data and now you're good to go! Please sign in to continue.
                <br />
                <br />

                <div style="text-align: center">
                    <a id="btnSignInSelfTest" data-role="button">
                        <h3>Sign In</h3>
                    </a>
                </div>
            </div>
            <div id ="divFailTest" style="display:none;">
                Thank you for waiting. The system has found some issues with your data. As a result, we have sent a report to Mobiz to look into the issue and fix it as soon as possible. 
                <br /><br />
                You are not advised to continue unless the application installed in your device has passed the test. <br /><br />
                To contact Mobiz, please email customercare@mobiz-apps.com or call (082) 286 - 9579. Thank you.
            
            </div>
            <div id ="divFailTestNoData" style="display:none;">
               Oops, it seems that your data is incomplete. As a result, the system is unable to process your data. Please make sure that you have 
               completed setting up your company information such as FOWST (Factory, Office, Warehouse, Store, Truck), products, and prices.
               <br />
               <br />
                We require those things to be completed first before we can test your data. 
               <br /><br />
               To set up your company information and upload your products, please sign in to <a href="http://mobiz-apps.com">mobiz-apps.com</a>, then proceed to System Administration. 
               If you need help in setting up your account, please email customercare@mobiz-apps.com or call (082) 286 - 9579. Thank you.

            </div>
       </div>
        <div id="divResults" style="display:none;">
            <h3 id="hdrResults">Results</h3>
             <b>Expected Results</b><br />
            <div class="ui-grid-a">
                <div class="ui-block-a" style="width: 60%">
                    Total Amount: 
                </div>
                <div class="ui-block-b" style="text-align:right;width: 40%">
                    <label id ="lblResultsTestExpected"></label>
                </div>
            </div>
            <div class="ui-grid-a">
                <div class="ui-block-a" style="width: 60%">
                   No. of Transactions: 
                </div>
                <div class="ui-block-b" style="text-align:right;width: 40%">
                    <label id ="lblResultsCountExpected"></label>
                </div>
            </div>
            <div class="ui-grid-a">
                <div class="ui-block-a" style="width: 60%">
                   No. of Invoice Items: 
                </div>
                <div class="ui-block-b" style="text-align:right;width: 40%">
                    <label id ="lblItemCountExpected"></label>
                </div>
            </div>
            <div class="ui-grid-a">
                <div class="ui-block-a" style="width: 60%">
                   Has Duplicates?
                </div>
                <div class="ui-block-b" style="text-align:right;width: 40%">
                    <label id ="lblDuplicateExpected"></label>
                </div>
            </div>
            <br /><br />
            <b>Local vs Local</b><br /> 
            <div class="ui-grid-a">
                <div class="ui-block-a" style="width: 60%">
                    Actual Invoice Items Total Amount:  
                </div>
                <div class="ui-block-b" style="text-align:right;width: 40%">
                    <label id ="lblResultsActual"></label>
                </div>
            </div>

             <div class="ui-grid-a">
                <div class="ui-block-a" style="width: 60%">
                    Discrepancy:  
                </div>
                <div class="ui-block-b" style="text-align:right;width: 40%">
                    <label id ="lblResultsDiscrepancy"></label>
                </div>
            </div>

            <div class="ui-grid-a">
                <div class="ui-block-a" style="width: 60%">
                   No. of Transactions: 
                </div>
                <div class="ui-block-b" style="text-align:right;width: 40%">
                    <label id ="lblResultsCount"></label>
                </div>
            </div>

            <div class="ui-grid-a">
                <div class="ui-block-a" style="width: 60%">
                   No. of Invoice Items: 
                </div>
                <div class="ui-block-b" style="text-align:right;width: 40%">
                    <label id ="lblItemCount"></label>
                </div>
            </div>
            <div class="ui-grid-a">
                <div class="ui-block-a" style="width: 60%">
                   Has Duplicates?
                </div>
                <div class="ui-block-b" style="text-align:right;width: 40%">
                    <label id ="lblDuplicate"></label>
                </div>
            </div>
            <br /><br />

            <b>Local vs Server</b><br />
             <div class="ui-grid-a">
                <div class="ui-block-a" style="width: 60%">
                    Actual Invoice Items Total Amount (Server Data):  
                </div>
                <div class="ui-block-b" style="text-align:right;width: 40%">
                    <label id ="lblResultsServer"></label>
                </div>
            </div>

            <div class="ui-grid-a">
                <div class="ui-block-a" style="width: 60%">
                    Discrepancy:  
                </div>
                <div class="ui-block-b" style="text-align:right;width: 40%">
                    <label id ="lblResultsDiscrepancyServer"></label>
                </div>
            </div>

            <div class="ui-grid-a">
                <div class="ui-block-a" style="width: 60%">
                   No. of Transactions: 
                </div>
                <div class="ui-block-b" style="text-align:right;width: 40%">
                    <label id ="lblResultsCountServer"></label>
                </div>
            </div>
            <div class="ui-grid-a">
                <div class="ui-block-a" style="width: 60%">
                   No. of Invoice Items: 
                </div>
                <div class="ui-block-b" style="text-align:right;width: 40%">
                    <label id ="lblItemCountServer"></label>
                </div>
            </div>
            <div class="ui-grid-a">
                <div class="ui-block-a" style="width: 60%">
                   Has Duplicates?
                </div>
                <div class="ui-block-b" style="text-align:right;width: 40%">
                    <label id ="lblDuplicateServer"></label>
                </div>
            </div>
            <div data-role="popup" id="popupMenuTest" >
                <div data-role="popup" id="dvpopupTest" data-theme="a" class="ui-corner-all" data-dismissible="false" data-overlay-theme="a">
                    <p id="popTestMessage" style="padding:10px 20px;text-align:center"></p>
                </div>
            </div>

             <div data-role="popup" id="popupMenuWait" >
                <div data-role="popup" id="dvpopupWait" data-theme="a" class="ui-corner-all" data-dismissible="false" data-overlay-theme="a">
                    <p style="padding:20px 20px;text-align:center">Please Wait ...</p>
                </div>
            </div>
        </div>
    </div>
</div>
