@Code
    ViewData("Title") = "Questionaire"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
@Section head
    <link rel="stylesheet" href="/Content/flatui-css-custom.css" />
    <link rel="stylesheet" href="/Content/bootstrap.css" />
    <link rel="stylesheet" href="/Content/bootstrap-custom.css" />
    <link rel="stylesheet" href="/Content/jquery.dataTables2.css" />

    <script>
        require([GetPathFormat('/Scripts/sources/Survey/Questionaire.js?crc=')]);
    </script>

    <style type="text/css">
        .ItemsBreadCrumbs {
            padding: 10px 0px 7px 0px;
        }

        .spinner-border {
            display: inline-block;
            width: 2rem;
            height: 2rem;
            vertical-align: text-bottom;
            border: .25em solid currentColor;
            border-right-color: transparent;
            border-radius: 50%;
            -webkit-animation: spinner-border .75s linear infinite;
            animation: spinner-border .75s linear infinite; 
        }

        .text-primary {
            color: #007bff!important;
            box-sizing: border-box;
        }

        @@-webkit-keyframes spinner-border {
            100% {
                -webkit-transform: rotate(360deg);
                transform: rotate(360deg);
            }
        }

        .ownlegwork{
            padding:5px;
            width: 100%;
            position:relative;
            background-color:#e8e8e8;
            border-radius:5px;
            color:#565656;
            text-shadow: none;
        }

        .ownlegwork:hover{
            cursor:pointer;
            background: -webkit-radial-gradient(50% bottom, 255,255,255);
            background: radial-gradient(at 50% bottom, rgba(255,255,255,1) 0%, rgba(231,231,231,1) 35%, rgba(213,213,213,1) 100%);
        }

        .ownlegwork .dash{
            margin: 5px;
            border: 2px dashed #929292;            
            border-radius: 5px;
        }

        .ownlegwork .header{
            width:100%;
            font-size:22px;
            text-align:center;
            padding-top:20px;
            padding-bottom:20px;
            border-radius:5px 5px 0px 0px;
            font-weight:bold;
        }

        .ownlegwork .body{
            padding:15px;
            min-height: 310px;
        }

        .ownlegwork .footer{
            text-align:center;
            font-size:25px;
            padding-bottom: 20px;
            font-weight: bold;
        }

        .otherslegwork{
            padding:5px;
            width: 100%;
            position:relative;
            background-color:#4CAF50;
            border-radius:5px;
            color:#FFF;
            text-shadow: none;
        }

        .otherslegwork:hover{
            cursor:pointer;
            background: -webkit-radial-gradient(50% bottom,232,107);
            background: radial-gradient(at 50% bottom, rgba(102,232,107,1) 0%, rgba(76,175,80,1) 35%, rgba(62,142,65,1) 100%);
        }

        .otherslegwork .dash{
            margin: 5px;
            border: 2px dashed #fff;
            border-radius: 5px;
        }

        .otherslegwork .header{
            width:100%;
            font-size:22px;
            text-align:center;
            padding-top:20px;
            padding-bottom:20px;
            border-radius:5px 5px 0px 0px;
            font-weight:bold;
        }

        .otherslegwork .body{
            padding:15px;
            min-height: 310px;
        }

        .otherslegwork .footer{
            text-align:center;
            font-size:25px;
            padding-bottom: 20px;
            font-weight: bold;
        }

        .selectedService {
            border:2px solid #2196F3;
        }
        
    </style>

End Section
<div id="divSurveyLoading" style="display:none; padding:10px;">Loading, please wait ...</div>
@*<div id="custom_datatables_processing" style="padding: 10px; position: absolute; z-index: 1100; background-color: rgba(0,0,0, 0.5); width: 100%; top: 0; bottom: 0; text-align: center; color: #FFF; text-shadow: none;">
        <div style="transform: translate(-50%, -50%); left: 50%; top: 50%; position: fixed;">Loading, please wait ...</div>
    </div>*@
<div style="width: 100%; height: 300px; background-color: #335F8E; display:block; position:fixed;">
    
</div>
<div class="container" style="position:relative;">
    <div style="width: 100%; background-color: #fdfdfd; border-bottom-left-radius:5px; border-bottom-right-radius:5px; padding:10px; box-sizing:border-box;">
        <div id="divPreviewMessage" class="alert alert-success" role="alert" style="display:none;">
            Hello! Welcome to your <b>form preview</b>. <br /><br />
            Here you can test your form and check the navigation button of every page.
            Note that every information you enter here will <b>not be saved</b>.            
        </div>
        <div style="width: 100%; min-height: 500px; box-shadow: -10px 10px 9px 0px rgba(237, 237, 237, 0.6); border-radius: 5px; border-bottom: 1px solid #eaeaea; padding:10px; box-sizing:border-box;">
            <div class="row">
                <div class="col-sm-12">
                    <div id="pageMain" subpage="pageQuestionaire" style="display:block;">
                        <div id="divPaymentOption" style="display:none; text-align:center;">
                            <h2>
                                Whew! That was long! Now that you have completed the form, please choose the level of service you want for this building permit application:
                            </h2>

                            <div class="row">
                                <div class="col-sm-6">
                                    <div id="btnOwnLegwork" class="ownlegwork selectedService">
                                        <div class="dash">
                                            <div class="header">
                                                I’ll do the legwork
                                            </div>
                                            <div class="body">
                                                I’d like to download the filled-in forms and list of requirements and do the legwork myself.
                                                <br /><br />
                                                <b>Important Note:</b> The system will allow me to edit most of the details in the
                                                building permit forms even after I have downloaded them. I can re-download
                                                the updated forms at a later date to allow for changes in project details.
                                                The actual submission of documents, follow ups, and pick up of documents
                                                from various government units will be carried out by me or my company.
                                            </div>
                                            <div class="footer">
                                                P 500.00
                                            </div>
                                        </div>                                        
                                    </div>
                                </div>

                                <div class="col-sm-6">
                                    <div id="btnOthersLegwork" class="otherslegwork">
                                        <div class="dash">
                                            <div class="header">
                                                Konstruksyon.com does the legwork
                                            </div>
                                            <div class="body">
                                                I’d like to download the filled-in forms and list of requirements but I want konstruksyon.com to do the legwork.
                                                <br /><br />
                                                <b>Important Note:</b> The system will allow me to edit most of the details in the
                                                building permit forms even after I have downloaded them. I can re-download
                                                the updated forms at a later date to allow for changes in project details.
                                                However, once I have saved the forms as final, konstruksyon.com will start the
                                                submission of documents to the various government agencies involved.
                                                Modifications to the building permit forms after document submissions have been
                                                started will be subject to resubmission fees to compensate for the additional
                                                time and effort to resubmit the docs. I will be notified of the status of my
                                                building permit(s) via the konstruksyon.com website.
                                            </div>
                                            <div class="footer">
                                                P 4,999.00
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <br /><br />
                            <div id="paypal-button-container"></div>
                        </div>

                        <div class="row">
                            <div class="col-sm-3">
                                <img alt="Mobiz" src="/Content/images/MobizNewLogo.png" width="100px" />
                            </div>
                            <div class="col-sm-9">
                                <h1 id="lblSurveyTitle" style="padding:10px; margin:0; color:#335F8E;"></h1>
                            </div>
                        </div>                        

                        <div style="padding:10px; border:1px solid #f2f2f2; border-radius:2px;">                           
                            <div id="divPageList">

                            </div>
                            <ul id="listQuestions" class="custom-list list-padding" style=" margin:0;"></ul>
                            <br /><br />
                        </div>                        

                        @*<div class="txt-center" style="padding:10px; border-radius:0px 0px 5px 5px;">
                            <button data-role="none" id="btnSubmitAnswer" class="btn custom-btn-theme-a btn-less-width" style="display:none;">
                                <span class="glyphicon glyphicon-send"></span> Submit
                            </button>
                            <button data-role="none" class="btn custom-btn-theme-c btn-less-width btnCloseAnswer" style="display:none;">
                                <span class="glyphicon glyphicon-remove"></span> Close Popup
                            </button>
                        </div>*@
                    </div>
                    <div id="pageTY" subpage="pageQuestionaire" style="display:none; text-align:center;">
                        <br />
                        <span class="glyphicon glyphicon-ok-circle" style="font-size:100px; color:#16a085;"></span>
                        <br />
                        <span style="font-size:50px; color:#24675a;">Thank you for your response.</span>
                        <br /><br /><br /><br /><br /><br />
                        <button data-role="none" class="btn custom-btn-theme-c btn-less-width btnCloseAnswer" style="display:none;">
                            <span class="glyphicon glyphicon-remove"></span> Close Popup
                        </button>
                    </div>
                    <div id="pageBan" subpage="pageQuestionaire" style="display:none; text-align:center;">
                        <br />
                        <span class="glyphicon glyphicon-ban-circle" style="font-size:100px; color:#ff9800;"></span>
                        <br />
                        <span style="font-size:50px; color:#ca7a04;">You have already taken this survey .</span>
                        <br /><br /><br /><br /><br /><br />
                        <button data-role="none" class="btn custom-btn-theme-c btn-less-width btnCloseAnswer" style="display:none;">
                            <span class="glyphicon glyphicon-remove"></span> Close Popup
                        </button>
                    </div>
                    <div id="pageError" subpage="pageQuestionaire" style="display:none; text-align:center;">
                        <br />
                        <span class="glyphicon glyphicon-remove-circle" style="font-size:100px; color:#f44336;"></span>
                        <br />
                        <span id="lblErrorMsg" style="font-size:50px; color:#b71509;">Error</span>
                        <br /><br /><br /><br /><br /><br />
                        <button data-role="none" class="btn custom-btn-theme-c btn-less-width btnCloseAnswer" style="display:none;">
                            <span class="glyphicon glyphicon-remove"></span> Close
                        </button>
                    </div>
                    <div id="pagePayment" subpage="pageQuestionaire" style="display:none; text-align:center;">
                        <h1>Payment's Page</h1>
                        <br />
                        <button data-role="none" id="btnPayNow" class="btn custom-btn-theme-b btn-less-width">Pay Now</button>
                    </div>
                    <div id="pageDownloadFiles" subpage="pageQuestionaire" style="display:none; text-align:center;">
                        <h1>Thank you <b id="lblPaypalName"></b>! We received your payment.</h1>
                        <br />
                        <p>Click the button below to download your requirements.</p>
                        <br />
                        <button data-role="none" id="btnDownloadZipFiles" class="btn custom-btn-theme-b btn-less-width">Download Requirements</button>
                    </div>
                    <br />

                </div>
            </div>
        </div>
    </div>
    <br /><br />
    <div class="txt-center" style="padding:10px;">
        Powered by 1Mobiz Inc.
        <br /><br />
        For more information, visit us on <a data-role="none" href="http://mobiz-apps.com/" class="custom-link">www.mobiz-apps.com</a>
    </div>    
</div>
<div id="divSurveyMainPage" style="display:none;">
    <div class="container">
        
    </div>    
</div>
<script src="https://www.paypal.com/sdk/js?client-id=AfCfndCJqfA0OhGBEIUG1iFiuJqqqevAjapkaQ-tLFkZz_Uxu765GLebtYq_0TkaxjgJlrsJH9j4OQWR&currency=PHP">
    // Required. Replace SB_CLIENT_ID with your sandbox client ID.
</script>

<script>
    
    // This function displays Smart Payment Buttons on your web page.
</script>