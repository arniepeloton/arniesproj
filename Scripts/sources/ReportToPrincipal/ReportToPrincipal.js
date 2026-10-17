require([
    'common'
    ], function () {

        $(function () {
            SetDateDefaults();

            $('#btnGenerateWeeklySalesReport').click(function () {
                var data = { From: $('#selPOSSalesReportByDateFromMonth').val() + '/' + $('#selPOSSalesReportByDateFromDay').val() + '/' + $('#selPOSSalesReportByDateFromYear').val(),
                    To: $('#selPOSSalesReportByDateToMonth').val() + '/' + $('#selPOSSalesReportByDateToDay').val() + '/' + $('#selPOSSalesReportByDateToYear').val()
                }
                Bs.Loading('show');
                AjaxSend({
                    url: 'ReportToPrincipal/GetWeeklySalesReport',
                    data: data,
                    success: function (vData) {
                        Bs.Loading('hide');
                        window.location = GetPathFormat('/ReportToPrincipal/DownloadReportToPrincipal?p=' + vData.Report);
                    }
                }); //end ajaxsend
            });

            $('#btnGenerateWeeklyInventoryReport').click(function () {
                var data = { From: $('#selPOSSalesReportByDateFromMonth').val() + '/' + $('#selPOSSalesReportByDateFromDay').val() + '/' + $('#selPOSSalesReportByDateFromYear').val(),
                    To: $('#selPOSSalesReportByDateToMonth').val() + '/' + $('#selPOSSalesReportByDateToDay').val() + '/' + $('#selPOSSalesReportByDateToYear').val()
                }
                Bs.Loading('show');
                AjaxSend({
                    url: 'ReportToPrincipal/GenerateWeeklyInventoryReport',
                    data: data,
                    success: function (vData) {
                        Bs.Loading('hide');
                        window.location = GetPathFormat('/ReportToPrincipal/DownloadReportToPrincipal?p=' + vData.Report);
                    }
                }); //end ajaxsend
            });

            $('#btnGenerateMonthlyInventoryReport').click(function () {
                var data = { From: $('#selPOSSalesReportByDateFromMonth').val() + '/' + $('#selPOSSalesReportByDateFromDay').val() + '/' + $('#selPOSSalesReportByDateFromYear').val(),
                    To: $('#selPOSSalesReportByDateToMonth').val() + '/' + $('#selPOSSalesReportByDateToDay').val() + '/' + $('#selPOSSalesReportByDateToYear').val()
                }
                Bs.Loading('show');
                AjaxSend({
                    url: 'ReportToPrincipal/GenerateMonthlyInventoryReport',
                    data: data,
                    success: function (vData) {
                        Bs.Loading('hide');
                        window.location = GetPathFormat('/ReportToPrincipal/DownloadReportToPrincipal?p=' + vData.Report);
                    }
                }); //end ajaxsend
            });

            $('#selPOSSalesReportByDateFromMonth').change(function () {
                setDayCollection('selPOSSalesReportByDateFromMonth', 'selPOSSalesReportByDateFromDay', 'selPOSSalesReportByDateFromYear', false);
            });

            $('#selPOSSalesReportByDateFromYear').change(function () {
                setDayCollection('selPOSSalesReportByDateFromMonth', 'selPOSSalesReportByDateFromDay', 'selPOSSalesReportByDateFromYear', false);
            });

            $('#selPOSSalesReportByDateToMonth').change(function () {
                setDayCollection('selPOSSalesReportByDateToMonth', 'selPOSSalesReportByDateToDay', 'selPOSSalesReportByDateToYear', false);
            });

            $('#selPOSSalesReportByDateToYear').change(function () {
                setDayCollection('selPOSSalesReportByDateToMonth', 'selPOSSalesReportByDateToDay', 'selPOSSalesReportByDateToYear', false);
            });

            $('#selOnitsukaSalesReportByDateFromMonth').change(function () {
                setDayCollection('selOnitsukaSalesReportByDateFromMonth', 'selOnitsukaSalesReportByDateFromDay', 'selOnitsukaSalesReportByDateFromYear');
            });

            $('#selOnitsukaSalesReportByDateFromYear').change(function () {
                setDayCollection('selOnitsukaSalesReportByDateFromMonth', 'selOnitsukaSalesReportByDateFromDay', 'selOnitsukaSalesReportByDateFromYear');
            });

            $('#selOnitsukaSalesReportByDateToMonth').change(function () {
                setDayCollection('selOnitsukaSalesReportByDateToMonth', 'selOnitsukaSalesReportByDateToDay', 'selOnitsukaSalesReportByDateToYear');
            });

            $('#selOnitsukaSalesReportByDateToYear').change(function () {
                setDayCollection('selOnitsukaSalesReportByDateToMonth', 'selOnitsukaSalesReportByDateToDay', 'selOnitsukaSalesReportByDateToYear');
            });

            $('#btnGenerateOnitsukaWeeklySalesReport').click(function () {
                GenerateOnitsukaWeeklySalesReport(true);
            });

            $('#btnGenerateOnitsukaInventoryReport').click(function () {
                GenerateOnitsukaInventoryReport(true);
            });
            
            $('#btnGenerateOnitsukaInventoryReport').click(function () {
                GenerateOnitsukaInventoryReport(true);
            });

            $('#btnGenerateOnitsukaWeeklySalesReportforSportsTownInc').click(function () {
                GenerateOnitsukaWeeklySalesReport(false);
            });

            $('#btnGenerateOnitsukaInventoryReportforSportsTownInc').click(function () {
                GenerateOnitsukaInventoryReport(false);
            });

        });

        function GenerateOnitsukaWeeklySalesReport(IsForToraSports) {
            var data = {
                From: $('#selOnitsukaSalesReportByDateFromMonth').val() + '/' + $('#selOnitsukaSalesReportByDateFromDay').val() + '/' + $('#selOnitsukaSalesReportByDateFromYear').val(),
                To: $('#selOnitsukaSalesReportByDateToMonth').val() + '/' + $('#selOnitsukaSalesReportByDateToDay').val() + '/' + $('#selOnitsukaSalesReportByDateToYear').val(),
                IsForToraSports: IsForToraSports
            }
            Bs.Loading('show');
            AjaxSend({
                url: 'ReportToPrincipal/GetOnitsukaWeeklySalesReport',
                data: data,
                success: function (vData) {
                    Bs.Loading('hide');
                    window.location = GetPathFormat('/ReportToPrincipal/DownloadReportToPrincipal?p=' + vData.Report);
                }
            }); //end ajaxsend
        }

        function GenerateOnitsukaInventoryReport(IsForToraSports) {
            var data = {
                From: $('#selOnitsukaSalesReportByDateFromMonth').val() + '/' + $('#selOnitsukaSalesReportByDateFromDay').val() + '/' + $('#selOnitsukaSalesReportByDateFromYear').val(),
                To: $('#selOnitsukaSalesReportByDateToMonth').val() + '/' + $('#selOnitsukaSalesReportByDateToDay').val() + '/' + $('#selOnitsukaSalesReportByDateToYear').val(),
                IsForToraSports: IsForToraSports
            }
            Bs.Loading('show');
            AjaxSend({
                url: 'ReportToPrincipal/GetOnitsukaInventoryReport',
                data: data,
                success: function (vData) {
                    Bs.Loading('hide');
                    window.location = GetPathFormat('/ReportToPrincipal/DownloadReportToPrincipal?p=' + vData.Report);
                }
            }); //end ajaxsend
        }

        function SetDateDefaults() {
            setMonthCollection("selPOSSalesReportByDateFromMonth");
            setYearCollection("selPOSSalesReportByDateFromYear");
            setMonthCollection("selPOSSalesReportByDateToMonth");
            setYearCollection("selPOSSalesReportByDateToYear");
            setMonthCollection("selPOSSalesReportRangeByMonthFrom");
            setMonthCollection("selPOSSalesReportRangeByMonthTo");
            setYearCollection("selPOSSalesReportRangeByYear");

            setMonthCollection("selOnitsukaSalesReportByDateFromMonth");
            setYearCollection("selOnitsukaSalesReportByDateFromYear");
            setMonthCollection("selOnitsukaSalesReportByDateToMonth");
            setYearCollection("selOnitsukaSalesReportByDateToYear");
            setMonthCollection("selOnitsukaSalesReportRangeByMonthFrom");
            setMonthCollection("selOnitsukaSalesReportRangeByMonthTo");
            setYearCollection("selOnitsukaSalesReportRangeByYear");

            var m_Date = new Date();

            $('#selPOSSalesReportByDateFromMonth').val(m_Date.getMonth() + 1);
            $('#selPOSSalesReportByDateFromYear').val(m_Date.getFullYear());
            setDayCollection('selPOSSalesReportByDateFromMonth', 'selPOSSalesReportByDateFromDay', 'selPOSSalesReportByDateFromYear');
            $('#selPOSSalesReportByDateFromDay').val(m_Date.getDate());

            $('#selPOSSalesReportByDateToMonth').val(m_Date.getMonth() + 1);
            $('#selPOSSalesReportByDateToYear').val(m_Date.getFullYear());
            setDayCollection('selPOSSalesReportByDateToMonth', 'selPOSSalesReportByDateToDay', 'selPOSSalesReportByDateToYear');
            $('#selPOSSalesReportByDateToDay').val(m_Date.getDate());

            $('#selOnitsukaSalesReportByDateFromMonth').val(m_Date.getMonth() + 1);
            $('#selOnitsukaSalesReportByDateFromYear').val(m_Date.getFullYear());
            setDayCollection('selOnitsukaSalesReportByDateFromMonth', 'selOnitsukaSalesReportByDateFromDay', 'selOnitsukaSalesReportByDateFromYear');
            $('#selOnitsukaSalesReportByDateFromDay').val(m_Date.getDate());

            $('#selOnitsukaSalesReportByDateToMonth').val(m_Date.getMonth() + 1);
            $('#selOnitsukaSalesReportByDateToYear').val(m_Date.getFullYear());
            setDayCollection('selOnitsukaSalesReportByDateToMonth', 'selOnitsukaSalesReportByDateToDay', 'selOnitsukaSalesReportByDateToYear');
            $('#selOnitsukaSalesReportByDateToDay').val(m_Date.getDate());


            //try {
            //    $('#selPOSSalesReportByDateFromMonth').selectmenu('refresh');
            //    $('#selPOSSalesReportByDateFromYear').selectmenu('refresh');
            //    $('#selPOSSalesReportByDateFromDay').selectmenu('refresh');

            //    $('#selPOSSalesReportByDateToMonth').selectmenu('refresh');
            //    $('#selPOSSalesReportByDateToYear').selectmenu('refresh');
            //    $('#selPOSSalesReportByDateToDay').selectmenu('refresh');

            //    $('#selOnitsukaSalesReportByDateFromMonth').selectmenu('refresh');
            //    $('#selOnitsukaSalesReportByDateFromYear').selectmenu('refresh');
            //    $('#selOnitsukaSalesReportByDateFromDay').selectmenu('refresh');

            //    $('#selOnitsukaSalesReportByDateToMonth').selectmenu('refresh');
            //    $('#selOnitsukaSalesReportByDateToYear').selectmenu('refresh');
            //    $('#selOnitsukaSalesReportByDateToDay').selectmenu('refresh');
            //}
            //catch (e) {
            //    $('#selPOSSalesReportByDateFromMonth').selectmenu();
            //    $('#selPOSSalesReportByDateFromYear').selectmenu();
            //    $('#selPOSSalesReportByDateFromDay').selectmenu();

            //    $('#selPOSSalesReportByDateToMonth').selectmenu();
            //    $('#selPOSSalesReportByDateToYear').selectmenu();
            //    $('#selPOSSalesReportByDateToDay').selectmenu();

            //    $('#selOnitsukaSalesReportByDateFromMonth').selectmenu();
            //    $('#selOnitsukaSalesReportByDateFromYear').selectmenu();
            //    $('#selOnitsukaSalesReportByDateFromDay').selectmenu();

            //    $('#selOnitsukaSalesReportByDateToMonth').selectmenu();
            //    $('#selOnitsukaSalesReportByDateToYear').selectmenu();
            //    $('#selOnitsukaSalesReportByDateToDay').selectmenu();
            //}
        }
    });
