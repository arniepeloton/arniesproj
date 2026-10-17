@Code
    ViewData("Title") = "DTR"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
@Section head
    <link rel="stylesheet" href="/Content/flatui-css-custom.css" />
    <link rel="stylesheet" href="/Content/bootstrap-custom.css" />
    <link rel="stylesheet" href="/Content/jquery.dataTables2.css" />
    <link rel="stylesheet" href="/Content/jquery.buttons.dataTables.min.css" />
    <script>
        require(['common'], function () {
            require(['datatables.net'], function () {
                var htmlDTR = '';
                AjaxSend({
                    url: '/UpdateServer/GetMobizDTRPortNo',
                    success: function (vData) {

                        if (vData.Error || vData.MobizDTRPort == '')
                            sessionStorage.dtrhostname = location.hostname + ':9090'
                        else
                            sessionStorage.dtrhostname = location.hostname + ':' + vData.MobizDTRPort

                        //location.hostname + ':9090'
                        sessionStorage.SystemAdminFor = 'dtr';

                        if (CStr2(htmlDTR) != '') {
                            $('#divDailyTimeRecord').pageshow();
                            $('#tabDTRDashboard').opentabmenu();
                            return;
                        }

                        CallPage('#divDailyTimeRecord', htmlDTR, location.protocol + '//' + sessionStorage.dtrhostname + '/DTRReports/Main', function (e) {
                            htmlDTR = e;
                            $('#divDailyTimeRecord').find('#MainDiv').css('margin-top', '0px'); //naa sa dtr page
                            $('#divDailyTimeRecord').find('#HeaderDesktopVersion, #HeaderMobileVersion, .BreadCrumbs').remove();
                            $('#divDailyTimeRecord').find('#divDTRDashboardTab, .elementsForErp').removeClass('hidetab').show();

                            if (typeof dtr !== 'undefined') {
                                dtr.RefreshLogs();
                            }
                        })
                    }
                });
            })            
        });
    </script>

    <style type="text/css">
        .samplebtn {
            height: 100px;
            text-align: center;
            vertical-align: middle;
            font-size: 18px;
            border-radius: 3px;
            cursor: pointer;
            border: 1px solid #f2f2f2;
            display: flex;
            justify-content: center;
            align-items: center;
        }

        .erp-main-grid {
            text-align: center;
            margin-top: 15px;
            padding: 5px;
            border: 3px solid transparent;
        }

            .erp-main-grid:hover {
                border: 3px solid #CCC;
                border-radius: 2px;
                cursor: pointer;
            }

        .btn-trail {
            padding: 3px;
            display: inline-block;
            margin-right: 5px;
            text-align: center;
            text-shadow: none !important;
            font-weight: bold;
            color: #000;
            background-color: #FFF;
        }

            .btn-trail > span {
                font-size: 10px;
            }

            .btn-trail:hover {
                background-color: #3498db;
                color: #fff;
                cursor: pointer;
            }

        .elementsForErp {
            display: inline !important;
        }

        .dashboardGrid {
            background-color: #fffafa;
            height: 150px;
            padding: 10px;
            border-radius: 2px;
            border: 2px solid rgba(0,0,0, 0);
        }

            .dashboardGrid:hover {
                border: 2px solid #3498db;
                cursor: pointer;
            }

            .dashboardGrid > .gHeader {
                font-weight: bold;
                color: #3498db;
            }

            .dashboardGrid > .gBody {
                font-size: 25px;
                word-break: break-word;
                margin: 15px 0;
                text-align: center;
                color: #2c3e50;
            }

            .dashboardGrid > .gFooter {
                color: #b5b5b5;
                text-align: center;
                /*bottom:10px;
            position:absolute;*/
            }

            .modal {
                display:none !important;
            }
    </style>
End Section

<div>
    <div class="custom-theme-a">
        <div id="HeaderDesktopVersion2" class="container-fluid">
            <table>
                <tr>
                    <td>
                        <a data-role="none" href="javascript:history.back()" class="btn custom-btn-theme-a btn-less-width"><span class="glyphicon glyphicon-chevron-left"></span></a>
                    </td>
                    <td>
                        <div style="text-overflow:ellipsis; font-size:25px; white-space:nowrap; overflow:hidden;">
                            DTR and HR
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </div>

    @*<div class="BreadCrumbs">
        <div class="container-fluid">
            <div class="ItemsBreadCrumbs">
                
            </div>
        </div>
    </div>*@

    <br />
</div>
<div id="divDailyTimeRecord" subpage="divErpMain" class="container-fluid" style="display:none;">

</div>