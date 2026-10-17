require([
    'common',
    //GetPathFormat('/Scripts3/Browser/LibraryFowst.js?crc=')
    ], function () {

        $(document).ready(function () {

            if (sessionStorage.FowstID == undefined || sessionStorage.FowstID == "" || sessionStorage.FowstID == null) {
                GetFowst();
            }
        });

        function GetFowst() {
            var pData = { FilterFowst: 1 };
            AjaxSend({
                url: 'Browser/LibraryFowst',
                data: pData,
                success: function (vFowstData) {

                    if (vFowstData.Results.Rows.length == 1) {
                        sessionStorage.FowstID = vFowstData.Results.Rows[0].ID;
                    } else {
                        BrowseAddresses();
                    }
                }
            });
        }; // function GetFowst() {

        function BrowseAddresses() {
            BrowsePopup('LibraryFowst', 'Select FOWST', [
                {
                    AutoSearch: true,
                    DisplayText: [{ Column: 'FowstName' }],
                    Click: function (e) {
                        sessionStorage.FowstID = e.ID;
                    }
                }
            ], {
                OrganizationID: sessionStorage.OrgID,
                FilterFowst: 1
            })
            return;
            Browser.LibraryFowst.InputData.OrganizationID = sessionStorage.OrgID;
            Browser.LibraryFowst.InputData.FilterFowst = 1;
            Browser.LibraryFowst.Click = function (e) {
                sessionStorage.FowstID = e.ID;
            };
            Browser.LibraryFowst.Show();

        }; // function BrowseAddresses() {

        $(function () {

            setMonthCollection("selAyalaReportByDateMonth");
            setYearCollection("selAyalaReportByDateYear");

            $('#selAyalaReportByDateMonth').change(function (e) {
                e.preventDefault();

                setDayCollection('selAyalaReportByDateMonth', 'selAyalaReportByDateDay', 'selAyalaReportByDateYear', false);

            });

            $('#selAyalaReportByDateYear').change(function (e) {
                e.preventDefault();

                setDayCollection('selAyalaReportByDateMonth', 'selAyalaReportByDateDay', 'selAyalaReportByDateYear', false);

            });

            $('#selAyalaReportRange').change(function () {

                $("#divAyalaReportByDate").hide();

                if ($('#selAyalaReportRange').val() == "By Date") {
                    $("#divAyalaReportByDate").show();
                }

            });

            $('#btnAyalaReportGenerate').click(function (e) {

                e.preventDefault();

                GenerateReport();

            });

            $('#lnkReportLink').click(function (e) {
                e.preventDefault();

                window.location = GetPathFormat("/MallLessorAyala/DownloadReportFile?ReportLink=" + $(this).data("ReportFileLink"));

            });

            $('#btnAyalaReportDone').click(function (e) {

                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu"), "_self");
            });

            function GenerateReport() {
                var strDate = '';
                var pData = {};

                //Validate
                if ($("#selAyalaReportRange").val() == "") {
                    NewMessageBox("Please select report range.",
                            "Report Range",
                            [
                                { Text: 'Ok'
                                }
                            ]
                        );
                    return;
                }

                if ($("#selAyalaReportRange").val() == "By Date") {
                    if ($.trim($("#selAyalaReportByDateMonth").val()) == "" ||
                    $.trim($("#selAyalaReportByDateDay").val()) == "" ||
                    $.trim($("#selAyalaReportByDateYear").val()) == "") {

                        NewMessageBox("Please enter the date of report.",
                                "Date",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                        );
                        return;
                    }

                }

                strDate = $("#selAyalaReportByDateMonth").val() + "/" + $("#selAyalaReportByDateDay").val() + "/" + $("#selAyalaReportByDateYear").val();

                pData.RangeType = $('#selAyalaReportRange').val();
                pData.SalesDate = strDate;

                AjaxSend({
                    url: "MallLessorAyala/GenerateReportFile",
                    data: pData,
                    success: function (vDataGen) {

                        if (vDataGen.Error) {
                            NewMessageBox(vDataGen.Error);
                            return;
                        };

                        if (vDataGen.GeneratedReport != "") {

                            $("#divReportLink").show();
                            $("#lnkReportLink").data("ReportFileLink", vDataGen.GeneratedReport);

                            NewMessageBox('Report file successfully generated. Please download the file by clicking the "Download Report File" link.',
                                "Generate Report",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                            );

                        } else {

                            $("#divReportLink").hide();

                            NewMessageBox('Report file generation failed.',
                                "Generate Report",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                            );

                        }


                    }
                });



            }




        });
    });