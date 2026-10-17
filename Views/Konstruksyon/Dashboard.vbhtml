@Code
    ViewData("Title") = "Konstruksyon"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
@Section head
    <link rel="stylesheet" href="/Content/flatui-css-custom.css" />
    <link rel="stylesheet" href="/Content/bootstrap-custom.css" />
    <link rel="stylesheet" href="/Content/bootstrap.css" />
    <link rel="stylesheet" href="/Content/jquery.dataTables2.css" />
    <link rel="stylesheet" href="/Content/jquery.buttons.dataTables.min.css" />    
    <script>
        require([GetPathFormat('/Scripts/sources/Konstruksyon/Dashboard.js?crc=')]);
    </script>

    <style type="text/css">
        .buttonKonstruksyon{
            width:200px; 
            height:200px;
            border-radius:50%;
            display: inline-block;
            border:3px solid none;
            background-color: #03A9F4;
            color: #FFF;
            text-shadow: none;
            font-size: 24px;
            cursor:pointer;
        }

        .buttonKonstruksyon:hover{
            border:3px solid #03A9F4;
            background-color: #FFF;
            color: #03A9F4;
        }
    </style>    
End Section

<div style="background-color:#2C3E50">
    <div class="container">
        <nav>
            <div class="container-fluid">
                <!-- Brand and toggle get grouped for better mobile display -->
                <div class="navbar-header">
                    <button data-role="none" type="button" class="navbar-toggle collapsed" data-toggle="collapse" data-target="#bs-example-navbar-collapse-1" aria-expanded="false">
                        <span class="sr-only">Toggle navigation</span>
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>
                    </button>
                    <label data-role="none" class="navbar-brand" href="#" style="height:auto !important; color:#FFF; text-shadow:none;">Dashboard</label>
                </div>

                <div class="collapse navbar-collapse" id="bs-example-navbar-collapse-1">
                    <ul class="nav navbar-nav navbar-right navbar-form">
                        <li class="dropdown">
                            <button data-role="none" href="#" class="dropdown-toggle btn custom-btn-theme-a" data-toggle="dropdown" role="button" aria-haspopup="true" aria-expanded="false">Hi <span id="lblUserName"></span> <span class="caret"></span></button>
                            <ul class="dropdown-menu">
                                <li><a id="btnMyProfile" data-role="none" href="#"><span class="glyphicon glyphicon-user"></span> Profile</a></li>
                                <li><a id="btnLogout" data-role="none" href="#"><span class="glyphicon glyphicon-log-out"></span> Logout</a></li>
                            </ul>
                        </li>
                    </ul>
                </div>
            </div><!-- /.container-fluid -->
        </nav>
    </div>
</div>
<br />
<div class="container">
    <div style="text-align:right;">
        <button data-role="none" class="btn custom-btn-theme-b btn-less-width btn-sm" id="btnTakeSurvey">
            Fill In an Application Form
        </button>
    </div>
    <br />
    <table data-role="table" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
        <thead>
            <tr class="custom-theme-f">
                <th style="cursor:pointer;"> # </th>
                <th style="cursor:pointer;"> Project Name </th>
                <th style="cursor:pointer;"> Application Form </th>
                <th style="cursor:pointer;"> Date Requested </th>
                <th style="cursor:pointer;"> Status </th>
            </tr>
        </thead>
        <tbody id="tbodyCustomerBuildingPermit"></tbody>
    </table>
</div>

<div class="modal fade" id="modalViewResponse" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                <h4 class="modal-title" id="myModalLabel">Log In</h4>
            </div>
            <div class="modal-body">
                <div class="row">
                    <div class="col-sm-3">Questionaire Title:</div>
                    <div class="col-sm-5 txt-bold" id="lblROQTitle"></div>
                    <div class="col-sm-4">
                        <button data-role="none" id="btnCRMRODLReq" class="btn custom-btn-theme-b btn-less-width btn-sm">Download Req(s)</button>
                    </div>
                </div>
                <br />
                <ul id="listCRMClientQuestionsReadOnly" class="list-padding" style="list-style-type:decimal;"></ul>
            </div>
            <div class="modal-footer">
                <button data-role="none" type="button" class="btn custom-btn-theme-c btn-less-width btn-sm" data-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>

<div id="popKonstruksyonPermit" custom-data-role="popup" style="display: none;">
    <div class="popup">
        <div class="popup-header">
            View Response
            <a class="popup-close ui-link">x</a>
        </div>

        <div class="popup-content">
            <div class="row">
                <div class="col-sm-6">Form Title: <b id="lblKonstruksyonROQTitle"></b></div>
                <div class="col-sm-6">
                    <button data-role="none" id="btnKonstruksyonRODLReq" class="btn custom-btn-theme-b btn-less-width btn-sm">Download Req(s)</button>
                    <button data-role="none" id="btnKonstruksyonEditForm" class="btn custom-btn-theme-b btn-less-width btn-sm">Edit Form</button>
                    <button data-role="none" id="btnKonstruksyonAttachFiles" class="btn custom-btn-theme-b btn-less-width btn-sm">Attach Files</button>
                    <input data-role="none" type="file" id="fileKonstruksyonFiles" accept=".xlsx,.xls,image/*,.doc, .docx,.ppt, .pptx,.txt,.pdf" multiple="" style="display:none;">
                </div>
            </div>
            <br><br />
            <div id="tabKonstruksyon" class="custom-tab">
                <ul>
                    <li class="active" custom-tab-content="#tabKonstruksyonDetails">Details</li>
                    <li custom-tab-content="#tabKonstruksyonFiles">Attachments</li>
                </ul>
            </div>
            <div id="tabKonstruksyonDetails" custom-tab-role="content" style="display:block;">
                <ul id="listKonstruksyonQuestionsReadOnly" class="list-padding" style="list-style-type:decimal;">></ul>
            </div>
            <div id="tabKonstruksyonFiles" custom-tab-role="content" style="display:none;">
                <table data-role="table" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
                    <thead>
                        <tr class="custom-theme-f">
                            <th style="width:20px !important;"> # </th>
                            <th style="cursor:pointer;"> File Name </th>
                            <th style="cursor:pointer; width:150px"> Date Uploaded </th>
                            <th style="cursor:pointer; width:50px"> </th>
                        </tr>
                    </thead>
                    <tbody id="tbodyKonstruksyonFile"></tbody>
                </table>
            </div>
        </div>

        <div class="popup-footer align-right">
            <button id="" data-role="none" class="btn custom-btn-theme-c btnCircle popup-close">
                <img class="img-cancel">
            </button>
        </div>
    </div>
</div>

<div data-role="popup" id="popCRMFileExist" data-dismissible="false" data-overlay-theme="a" data-theme="a" data-corners="false">
    <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
    <center><h3>Rename Existing File(s)</h3></center>
    <div data-role="content" class="ui-content ui-body-d jPopupContent">
        <div id="divExistingFile">

        </div>

        <br />
        <button id="btnCRMContinueUpload" data-role="none" class="btn custom-btn-theme-b">Continue Upload</button>
        <br /><br />
        <a href="#" data-rel="back" data-role="none" class="btn custom-btn-theme-c">Cancel</a>
    </div>
</div>