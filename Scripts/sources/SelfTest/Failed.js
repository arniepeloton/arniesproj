require(['common', '/Scripts/sources/ImportExportData/ImportExportData.js?crc=',
    '/Scripts3/Offline/Browser/common.js?crc='
    ], function () {

        require([(OfflineMode ? '/Scripts/sources/SelfTest/SelfTest_offline.js?crc=' : '')], function () {

            $(function () {

                PageShow();

                function PageShow() {

                    //get org name
                    var pData = SelfTest_offline().getTestDetails();
                    sessionStorage.OrgID = pData.Data.Rows[0].OrganizationID;
                    sessionStorage.DeviceID = pData.Data.Rows[0].DeviceID;
                    sessionStorage.PeopleID = pData.Data.Rows[0].PeopleID;

                    if (pData.Data.Rows[0].DataSet == 'incomplete') {

                        $('#divFailTestNoData').show();

                    } else {
                        $('#divFailTest').show();
                    }

                }

            });

            $('#btnSync').click(function (e) {
                e.preventDefault();



                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {
                        window.pageSecuritySourcePage = $.mobile.activePage;
                        IsSyncDatabase();
                        return;
                    },
                    error: function (vError) {
                        MessageBox("This feature requires internet connection.",
                                   "Requires Internet Connection",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                            );
                    }
                });
            });



        });
    });