require([
    'common'
    ], function () {

        $(function () {

            setMonthCollection("selShopperTrakByDateFromMonth");
            setYearCollection("selShopperTrakByDateFromYear");
            setMonthCollection("selShopperTrakByDateToMonth");
            setYearCollection("selShopperTrakByDateToYear");

            $('#selShopperTrakByDateFromMonth').change(function () {
                setDayCollection('selShopperTrakByDateFromMonth', 'selShopperTrakByDateFromDay', 'selShopperTrakByDateFromYear', false);
            });

            $('#selShopperTrakByDateFromYear').change(function () {
                setDayCollection('selShopperTrakByDateFromMonth', 'selShopperTrakByDateFromDay', 'selShopperTrakByDateFromYear', false);
            });

            $('#selShopperTrakByDateToMonth').change(function () {
                setDayCollection('selShopperTrakByDateToMonth', 'selShopperTrakByDateToDay', 'selShopperTrakByDateToYear', false);
            });

            $('#selShopperTrakByDateToYear').change(function () {
                setDayCollection('selShopperTrakByDateToMonth', 'selShopperTrakByDateToDay', 'selShopperTrakByDateToYear', false);
            });

            $('#selShopperTrakByDateFromMonth').val(new Date().getMonth() + 1);            
            $('#selShopperTrakByDateFromYear').val(new Date().getFullYear());
            setDayCollection('selShopperTrakByDateFromMonth', 'selShopperTrakByDateFromDay', 'selShopperTrakByDateFromYear', false);
            $('#selShopperTrakByDateFromDay').val(new Date().getDate());

            $('#selShopperTrakByDateToMonth').val(new Date().getMonth() + 1);
            $('#selShopperTrakByDateToYear').val(new Date().getFullYear());
            setDayCollection('selShopperTrakByDateToMonth', 'selShopperTrakByDateToDay', 'selShopperTrakByDateToYear', false);
            $('#selShopperTrakByDateToDay').val(new Date().getDate());

            $('#btnShopperTrakReportSalesData').click(function () {
                //var m_From = $('#txtDateFrom').DateCombo('val');
                //var m_To = $('#txtDateTo').DateCombo('val');
                var data = {
                    From: $('#selShopperTrakByDateFromMonth').val() + '/' + $('#selShopperTrakByDateFromDay').val() + '/' + $('#selShopperTrakByDateFromYear').val(),
                    To: $('#selShopperTrakByDateToMonth').val() + '/' + $('#selShopperTrakByDateToDay').val() + '/' + $('#selShopperTrakByDateToYear').val()
                }
                Bs.Loading('show');
                AjaxSend({
                    url: 'ShopperTrak/ShopperTrakSalesReport',
                    data: data,
                    success: function (vData) {
                        Bs.Loading('hide');
                        window.location = GetPathFormat('/ShopperTrak/DownloadShopperTrakReport?p=' + vData.Report);
                    }
                }); //end ajaxsend
            });

            $('#btnShopperTrakReportLaborData').click(function () {
                //var m_From = $('#txtDateFrom').DateCombo('val');
                //var m_To = $('#txtDateTo').DateCombo('val');
                var data = {
                    From: $('#selShopperTrakByDateFromMonth').val() + '/' + $('#selShopperTrakByDateFromDay').val() + '/' + $('#selShopperTrakByDateFromYear').val(),
                    To: $('#selShopperTrakByDateToMonth').val() + '/' + $('#selShopperTrakByDateToDay').val() + '/' + $('#selShopperTrakByDateToYear').val()
                }
                Bs.Loading('show');
                AjaxSend({
                    url: 'ShopperTrak/ShopperTrakReportLaborData',
                    data: data,
                    success: function (vData) {
                        Bs.Loading('hide');
                        window.location = GetPathFormat('/ShopperTrak/DownloadShopperTrakReport?p=' + vData.Report);
                    }
                }); //end ajaxsend
            });

        });

    });
