@Code
    ViewData("Title") = "ERP Subscribers"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
@Section head
    <link rel="stylesheet" href="/Content/flatui-css-custom.css" />
    <link rel="stylesheet" href="/Content/bootstrap-custom.css" />
    <link rel="stylesheet" href="/Content/jquery.dataTables2.css" />
    
    <script>
        require([GetPathFormat('/Scripts/sources/Subscription/ERPSubscriberSettings.js?crc=')]);
    </script>

    <style type="text/css">
        .ItemsBreadCrumbs {
            padding: 10px 0px 7px 0px;
        }

        .orgGrid{
            height: 100px;
            border: 1px solid #CCC;
            text-align: center;
            line-height: 100px;
            background-color:#fff;
            border-radius:2px;
        }

        .orgGrid:hover{
            border-color:#3498db;
            cursor:pointer;
            color:#3498db;
        }

        .orgGrid > span{
            display:inline-block;
            vertical-align:middle;
            line-height:normal;
        }

        .ui-body-c{
            background-color:#f9f9f9 !important;
        }
    </style>

End Section

<div id="divDBMainPage">
    <div id="divCRM" style="display:block;">
        <div class="custom-theme-a position-fixed" id="divHeaderForProjects2">

            <div id="HeaderDesktopVersion2" class="container">
                <div class="ui-grid-a" style="height:53px;">
                    <div class="ui-block-a" style="padding-right:9px; overflow:hidden; display:flex;">
                        <div style="float:left; margin-top:10px; margin-bottom:0px; overflow:auto;">                            
                            <div class="OrganizationNameHeader" style="text-overflow:ellipsis; font-size:25px; white-space:nowrap; overflow:hidden;">
                                ERP Settings
                            </div>
                        </div>
                    </div>
                    <div class="ui-block-b" style="padding:0px 0px 0px 20px;">
                    </div>
                </div>
            </div>

            <div id="HeaderMobileVersion2" style="display:none;">
                <div class="ui-grid-solo">
                    <div class="ui-block-a OrganizationNameHeader txt-center" style="white-space:nowrap; overflow:hidden; text-overflow:ellipsis;">
                        ERP Settings
                    </div>
                </div>
                <div class="ui-grid-b">
                    <div class="ui-block-a">
                        <a class="btn custom-btn-theme-a btnHome">
                            <span class="ui-icon ui-icon-home" style="display:inline-block;">
                                <p style="margin:16px 0px 0px 0px; position:relative; transform: translate(-50%, 50%); font-size:12px;">Home</p>
                            </span>
                        </a>
                    </div>
                    <div class="ui-block-b">
                        <a id="btnSearchMobile" class="btn custom-btn-theme-a">
                            <span class="ui-icon ui-icon-search" style="display:inline-block;">
                                <p style="margin:16px 0px 0px 0px; position:relative; transform: translate(-50%, 50%); font-size:12px;">Search</p>
                            </span>
                        </a>
                    </div>
                    @*<div class="ui-block-c">
                        <a class="btn custom-btn-theme-a btnPeriod">
                            <span class="ui-icon ui-icon-grid" style="display:inline-block;">
                                <p style="margin:16px 0px 0px -7px; font-size:12px;">Period</p>
                            </span>
                        </a>
                    </div>*@
                    <div class="ui-block-c">
                        <a class="btn custom-btn-theme-a btnSettings">
                            <span class="ui-icon ui-icon-gear" style="display:inline-block;">
                                <p style="margin:16px 0px 0px 0px; position:relative; transform: translate(-50%, 50%); font-size:12px;">Settings</p>
                            </span>
                        </a>
                    </div>
                </div>
                <div class="contentSearchEngine">
                    <div class="ui-grid-solo ui-responsive">
                        <div class="ui-block-a" style="padding:6px;">
                            <input type="text" placeholder="Search Here" class="txtSearchProspect" />
                        </div>
                    </div>
                </div>
                @*<div class="divLblPeriod">
                    <div class="lblPeriod">
                        Today
                    </div>
                </div>
                <br />*@
            </div>
            
            <div class="BreadCrumbs">
                <div class="container">
                    <div class="ItemsBreadCrumbs">
                        &nbsp;
                    </div>
                </div>                
            </div>
        </div>
    </div>   
    <br />
    <div id="mainContainer" class="container">
        <div id="pageMainErpSettings" subpage="erpsettings" style="display:block;">
            <div class="row">
                <div class="col-sm-6 col-sm-push-6">
                    <input type="text" data-role="none" id="txtSearchMobizSubscriber" placeholder="Search here ..." class="form-control" />
                </div>
                <div class="col-sm-6 col-sm-pull-6">
                    <b>List of Mobiz subscribers</b>
                </div>
            </div>
            <br />
            <div id="divOrgList">

            </div>
        </div>
        <div id="pageSettings" subpage="erpsettings" style="display:none;">

        </div>
    </div>
</div>