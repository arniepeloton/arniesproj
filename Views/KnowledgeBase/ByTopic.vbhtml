@Code
    ViewData("Title") = "Knowledge Base"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head
    <link rel="stylesheet" href="/Content/flatui-css-custom.css" />
    <link rel="stylesheet" href="/Content/bootstrap/css/bootstrap.min.css" />
    <link rel="stylesheet" href="/Content/bootstrap/css/bootstrap-theme.min.css" />
    <script src=""></script>
    <script>
        require([GetPathFormat('/Scripts/sources/KnowledgeBase/ByTopic.js?v=' + assetversion),
            GetPathFormat('')
        ]);
    </script>

    <style type="text/css">
        .divSearchBox {
            background-color:#335F8E;
            padding:10px 0px 10px 0px;
            width:100%;
        }

        .stuck {
            position:fixed;
            top:0;
            box-shadow:0 2px 4px rgba(0, 0, 0, .3);
            z-index:1;            
        }

        .searchResult {
            border:1px solid #f5f5f5;
            width:100%;
            height:100px;
            background-color:#f9f9f9;
            border-radius:3px;
            padding:10px;
            cursor:pointer;
        }

        .searchResult:hover{
            border-color:#e0e0e0;
        }

        .searchResult > div {
            border-bottom:1px solid #efefef;
            font-size:18px;
            color:#0081e8;
        }

        .searchResult > div > span{
            color:#CCC;
            font-size:12px;
        }

        .searchResult > p {
            color: #585858;
            font-size:15px;
            line-height:1.8;
            margin-bottom: 0px;
        }

        .modal-body {
            max-height: calc(100vh - 200px);
            overflow-y: auto;
        }

        .kbTopicBtn{
            height: 100px;
            text-align: center;
            vertical-align: middle;
            line-height: 100px;
            font-size: 18px;
            border-radius: 3px;
            cursor:pointer;
            border: 1px solid #f2f2f2;
        }
    </style>
End Section

<div data-role="page">
    <div>
        <div class="modal fade" id="modFilter" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title" id="myModalLabel">Filter Results By</h4>
                    </div>
                    <div class="modal-body">
                        <form id="formFilter" class="form-horizontal">
                            <div class="row">
                                <div class="col-md-2">
                                    <label for="inputEmail3" class=" control-label">Topic</label>
                                </div>
                                <div class="col-md-10">
                                    <select id="selFilterTopic" data-role="none" class="form-control">
                                        <option>Please Select</option>
                                    </select>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-md-2">
                                    <label for="inputEmail3" class=" control-label">Sub Topic</label>
                                </div>
                                <div class="col-md-10">
                                    <select id="selFilterSubTopic" data-role="none" class="form-control">
                                        <option>Please Select</option>
                                    </select>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-md-2">
                                    <label for="inputEmail3" class=" control-label">Product</label>
                                </div>
                                <div class="col-md-10">
                                    <select id="selFilterProduct" data-role="none" class="form-control">
                                        <option>Please Select</option>
                                    </select>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-md-2">
                                    <label for="inputEmail3" class=" control-label">Language</label>
                                </div>
                                <div class="col-md-10">
                                    <select id="selFilterLanguage" data-role="none" class="form-control">
                                        <option>Please Select</option>
                                    </select>
                                </div>
                            </div>
                        </form>
                    </div>
                    <div class="modal-footer">
                        <button data-role="none" type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                        <button id="btnOkFilter" data-role="none" type="button" class="btn btn-success">Filter</button>
                    </div>
                </div>
            </div>
        </div>

        <div class="modal fade" id="modViewTopic" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
            <div class="modal-dialog modal-lg" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title" id="myTopicLabel">&nbsp;</h4>
                    </div>
                    <div class="modal-body">
                        <div subpage="pageAnswers" id="divMultiAnswers" style="display:block;">
                            <span class="glyphicon glyphicon-info-sign"></span> Click to view more details.
                            <ul id="listAnswers" class="list-unstyled"></ul>
                        </div>

                        <div subpage="pageAnswers" id="divAnswerDetails" style="display:none;">
                            <button id="btnAnswerDetailsBack" data-role="none" class="btn btn-default" style="margin-bottom:10px;"><span class="glyphicon glyphicon-chevron-left"></span> Back</button>
                            
                            <div id="listAnswerDetails">

                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button data-role="none" type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div id="divMainSearch" custom-data-role="page">
        <div style="width: 100%; background-color:#335F8E; color:#fff;">
            <div>
                <nav class="navbar" style="background-color:#FFF; border-radius:0;">
                    <div class="container">
                        <div class="navbar-header">
                            @*<button data-role="none" type="button" class="navbar-toggle collapsed" data-toggle="collapse" data-target="#navbar" aria-expanded="false" aria-controls="navbar">
                                <span class="sr-only">Toggle navigation</span>
                                <span class="icon-bar"></span>
                                <span class="icon-bar"></span>
                                <span class="icon-bar"></span>
                            </button>*@
                            <a id="btnHome" data-role="none" class="navbar-brand" href="#" style="color:#000;">1Mobiz Inc.</a>
                        </div>
                        @*<div id="navbar" class="navbar-collapse collapse">
                            <ul class="nav navbar-nav">
                                <li><a>Home</a></li>
                            </ul>
                        </div>*@
                    </div>
                </nav>
            </div>
            <div class="container theme-showcase" role="main">
                <div class="row txt-center">
                    <div class="col-sm-12">
                        <h2>Knowledgebase</h2>
                        <i>We’re here to help. Type your question in the search box <br />or click on the tiles below for a guided reading list of help topics.</i>
                        <br /><br />
                    </div>
                </div>
            </div>
            <div class="divSearchBox">
                <div class="container">
                    <div class="row">
                        <div class="col-sm-12">
                            <div class="input-group">
                                <input type="text" id="txtMainSearch" data-role="none" class="form-control" placeholder="Type your question here ..." onclick="this.select()">
                                <span class="input-group-btn">
                                    <button id="btnMainSearch" data-role="none" class="btn btn-primary" type="button"><span class="glyphicon glyphicon-search" aria-hidden="true"></span></button>
                                </span>
                            </div>                            
                        </div>
                    </div>
                </div>
            </div>
            <br /><br />
        </div>
        
        <div class="container">
            <div id="pageMain" subpage="divMainSearch" style="display:none;">
                <div id="divStartup">
                </div>                                
                @*<div style="padding-top:50px; text-align:center">
                    <img alt="Mobiz Logo" src="/Content/images/MobizIcon96x96.png" />
                    <br />
                    Powered by <b>Mobiz &trade;</b>
                </div>*@
            </div>
            <br />
            <div id="pageByTopic" subpage="divMainSearch" style="display:block;">
                <ul class="custom-list"><li><h3>MCR > Troubleshooting > Printers</h3></li><ol><li>Cannot Print</li><li>sgerghrfrgsdgsdgsdg</li><li>sdfsdfsd</li><li>Hello World</li><li>Feb 27</li><li>hgjgjhgjgh</li><li>Testing a long case</li><li>dfgertt</li></ol></ul>
            </div>

            <div id="pageSearchList" subpage="divMainSearch" style="display:none;">
                <div id="pageInfo" subpage="pageSearchList" style="display:block; font-size: 20px; font-style: italic;">
                    Searching keywords ...
                </div>

                <div id="pageSuccess" subpage="pageSearchList" style="display:none;">
                    <div class="row">
                        <div class="col-md-2">
                            <button data-role="none" data-toggle="modal" data-target="#modFilter" type="button" class="btn btn-warning"><span class="glyphicon glyphicon-filter" aria-hidden="true"></span> Filter Results</button>
                        </div>
                        <div id="listFilteredBy" class="col-md-10">

                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-md-8">
                            <ul id="listKbSearchResult" class="list-unstyled list-padding draggableList"></ul>
                        </div>
                    </div>
                </div>                
            </div>

            <div id="pageViaLink" subpage="divMainSearch" style="display:none;">
                <div class="row">
                    <div class="col-md-8">
                        <h2 id="txtProblem"></h2>

                        <div id="linkDetails" style="font-size:16px;"></div>
                    </div>
                </div>
            </div>
        </div>
        
        @*<div style="width:auto; position: absolute; left:50%; top:50%; transform:translate(-50%, -50%);">
            <div class="col-lg-6">
                <div class="input-group">
                    <input type="text" id="txtMainSearch" data-role="none" class="form-control" placeholder="Search for...">
                    <span class="input-group-btn">
                    <button id="btnMainSearch" data-role="none" class="btn btn-primary" type="button"><span class="glyphicon glyphicon-search" aria-hidden="true"></span></button>
                    </span>
                </div>
            </div>
        </div>*@
    </div>

    <div id="divMainSearchResults" custom-data-role="page" style="display:none;">
        <div class="custom-theme-a position-fixed" id="divHeaderForProjects">
            <div id="HeaderDesktopVersion">
                <div class="ui-grid-solo ui-responsive" style="height:53px;">
                    <div class="ui-block-a" style="padding-right:9px; overflow:hidden; display:flex;">
                        <div style="float:left; margin-left:10px; margin-top:5px; cursor:pointer;">
                            @*<a class="btnBack" data-iconpos="notext" data-theme="b" data-role="button" data-icon="back" title="Back">Back</a>
                            <a style="margin-top:8px;" class="btnHome" data-iconpos="notext" data-theme="b" data-role="button" data-icon="home" title="Home">Home</a>*@
                        </div>

                        <div style="float:left; margin-left:25px; margin-bottom:0px; width:50%;">                            
                            <input type="text" placeholder="Search Here" class="txtMainSearchResult" style="width:60%;" />
                        </div>
                    </div>
                </div>
            </div>        
        </div>

        <div id="mainContainer">
            
        </div>
        <div data-role="content">
            <div id="divSearchingIndicator" subpage="searchresult">
                Searching ...
            </div>

            <div id="divSearchResult" subpage="searchresult" style="display:none;">
                <ul id="lstSearchResults" style="list-style-type:none; padding:0;">
                </ul>
            </div>

            <div id="divViewSearchResult" subpage="searchresult" style="display:block;">
                <h2><label id="txtTopic">asd</label></h2>
                <br />

                <span id="txtProblem" style="font-size:18px;">jaghjgasd</span>

                <ol id="lstViewResults">
                    
                </ol>
            </div>
        </div>
        
    </div>
</div>
