define(['common'
    ], function () {
    
        //metro.min.js needs jquery to load first before it can load itself

        require([
                '/Scripts/jquery.widget.min.js'
        ],
            function () {
                require([
                        '/Scripts/metro.min.js'
                ],
                    function () {
                        //metro-calendar.js, metro-datepicker.js needs jquery.widget.min.js andmetro.min.js  to load first before it can load itself
                        require([
                                '/Scripts/metro-calendar.js',
                                '/Scripts/metro-datepicker.js'
                        ],
                            function () {
                                $(".fromCalendar").calendar({
                                    format: "mmmm dd, yyyy",
                                    multiSelect: false,
                                    otherDays: true,
                                    getDates: function (data) {
                                        $("#txtDateFrom").text(data);
                                    }
                                });

                                $(".toCalendar").calendar({
                                    format: "mmmm dd, yyyy",
                                    multiSelect: false,
                                    otherDays: true,
                                    getDates: function (data) {
                                        $("#txtDateTo").text(data);
                                    }
                                });

                                $('#btnGetData').click(function () {
                                    getJournal();
                                })
                            })

                        function getJournal() {

                            var pData = {};

                            pData.OrgID = 'CFE39DF6-8809-4973-9515-810DF75AED12'
                            pData.RangeType = 'Today';
                            pData.ZDate = $('#txtDateFrom').text();
                            pData.DateFrom = $('#txtDateFrom').text();
                            pData.DateTo = $('#txtDateTo').text();
                            pData.FOWSTID = $('#txtBranch').val();
                            pData.DeviceID = $('#txtDevice').val();

                            AjaxSend({
                                url: "ReportsPOS/getEJournal",
                                data: pData,
                                success: function (vData) {
                                    if (OfflineMode) {
                                        Transfers.Download({
                                            Source: 'ReportsPOS/DownloadReportFile?OrgID=' + sessionStorage.OrgID + '&ReportLink=' + vData.ReportPOS,
                                            Destination: vData.ReportPOS,
                                            Success: function (vData) {
                                                alert('Downloading EJournal Successful');
                                            }
                                        });
                                    } else {
                                        window.location = GetPathFormat('../ReportsPOS/DownloadReportFile?OrgID=' + sessionStorage.OrgID + '&ReportLink=' + vData.ReportPOS);
                                    }
                                }
                            });
                        }
                    })
            })
});