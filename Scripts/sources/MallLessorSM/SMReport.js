require([
    'common',
    GetPathFormat('/Scripts3/Browser/LibraryFowst.js?crc=')
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
        }; // function BrowseAddresses() {

        $(function () {

            setMonthCollection("selSMReportByDateMonth");
            setYearCollection("selSMReportByDateYear");

            $('#selSMReportByDateMonth').change(function (e) {
                e.preventDefault();

                setDayCollection('selSMReportByDateMonth', 'selSMReportByDateDay', 'selSMReportByDateYear', true);

            });

            $('#selSMReportByDateYear').change(function (e) {
                e.preventDefault();

                setDayCollection('selSMReportByDateMonth', 'selSMReportByDateDay', 'selSMReportByDateYear', true);

            });

            $('#selSMReportRange').change(function () {

                $("#divSMReportByDate").hide();

                if ($('#selSMReportRange').val() == "By Date") {
                    $("#divSMReportByDate").show();
                }

            });

            $('#btnSMReportGenerate').click(function (e) {

                e.preventDefault();

                GenerateReport();

            });

            //commented by janine june 17 2015
//            $('#lnkSmReportLink').click(function (e) {
//                e.preventDefault();

//                if (!IsMobile) {
//                    window.location = GetPathFormat("/MallLessorSM/DownloadReportFile?ReportOrgID=" + sessionStorage.OrgID + "&ReportFowstID=" + sessionStorage.FowstID + "&ReportLink=" + $(this).data("ReportFileLink"));
//                } else {
//                    var strFileName = $(this).data("ReportFileLink");
//                    Transfers.Download({
//                        Source: MergeUrlParameters({
//                            ReportOrgID: sessionStorage.OrgID
//                                                    , ReportFowstID: sessionStorage.FowstID
//                                                    , ReportLink: $(this).data("ReportFileLink")
//                        }, '/MallLessorSM/DownloadReportFile'),
//                        Destination: "SMCOIN/" + sessionStorage.OrgID + "/" + sessionStorage.FowstID + "/" + $(this).data("ReportFileLink"),
//                        Success: function (vData) {

//                            MessageBox("Downloaded file can be viewed in SMCOIN/" + sessionStorage.OrgID + "/" + sessionStorage.FowstID + "/" + strFileName,
//                                    "Download SM COIN Report",
//                                    [
//                                        { Text: 'Ok'
//                                        }
//                                    ]
//                            );
//                            return;

//                        },
//                        Error: function (e) {
//                            Android.Toast('Error while downloading file');
//                        }
//                    });
//                }

//            });

//edited by janine. download csv automatically not zipped
            $('#lnkSmReportLink').click(function (e) {
                e.preventDefault();

                if (!IsMobile) {
                    window.location = GetPathFormat("/MallLessorSM/DownloadReportFile?ReportOrgID=" + sessionStorage.OrgID + "&ReportFowstID=" + sessionStorage.FowstID + "&ReportLink=" + $(this).data("ReportFileLink"));
                } else {
                    var strFileName = $(this).data("ReportFileLink");
                    Transfers.Download({
                        Source: MergeUrlParameters({
                            ReportOrgID: sessionStorage.OrgID
                                                    , ReportFowstID: sessionStorage.FowstID
                                                    , ReportLink: $(this).data("ReportFileLink")
                        }, '/MallLessorSM/DownloadReportFile'),
                        Destination: "SMCOIN/" + sessionStorage.OrgID + "/" + sessionStorage.FowstID + "/" + $(this).data("ReportFileLink"),
                        Success: function (vData) {
                            Transfers.ExtractZipFile({
                                Source: vData.Destination,
                                Destination: "SMCOIN",
                                ExtractToFiles: true,
                                Success: function (vDataExtractFiles) {
                                    MessageBox("Downloaded file can be viewed in SMCOIN/" + sessionStorage.OrgID + "/" + sessionStorage.FowstID + "/" + strFileName,
                                             "Download SM COIN Report",
                                             [
                                                 {
                                                     Text: 'Ok'
                                                 }
                                             ]
                                     );
                                    return;

                                }
                            })






                        },
                        Error: function (e) {
                            Android.Toast('Error while downloading file');
                        }
                    });
                }

            });

            $('#btnSMReportDone').click(function (e) {

                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu"), "_self");
            });

            function GenerateReport() {
                var strDate = '';
                var pData = {};

                //Validate
                if ($("#selSMReportRange").val() == "") {
                    MessageBox("Please select report range.",
                            "Report Range",
                            [
                                { Text: 'Ok'
                                }
                            ]
                        );
                    return;
                }

                if ($("#selSMReportRange").val() == "By Date") {
                    if ($.trim($("#selSMReportByDateMonth").val()) == "" ||
                    $.trim($("#selSMReportByDateDay").val()) == "" ||
                    $.trim($("#selSMReportByDateYear").val()) == "") {

                        MessageBox("Please enter the date of report.",
                                "Date",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                        );
                        return;
                    }

                }

                strDate = $("#selSMReportByDateMonth").val() + "/" + $("#selSMReportByDateDay").val() + "/" + $("#selSMReportByDateYear").val();

                pData.RangeType = $('#selSMReportRange').val();
                pData.SalesDate = strDate;
                pData.FlagClose = 1;
                Bs.Loading('show');
                AjaxSend({
                    url: "MallLessorSM/GenerateReportFile",
                    data: pData,
                    success: function (vDataGen) {
                        Bs.Loading('hide');
                        if (vDataGen.Error) {
                            MessageBox(vDataGen.Error);
                            return;
                        };

                        if (vDataGen.GeneratedReport != "") {

                            $("#divSmReportLink").show();
                            $("#lnkSmReportLink").data("ReportFileLink", vDataGen.GeneratedReport);

                            MessageBox('Report file successfully generated. Please download the file by clicking the "Download Report File" link.',
                                "Generate Report",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                            );

                        } else {

                            $("#divSmReportLink").hide();

                            MessageBox('Report file generation failed.',
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

            $('#formImportMallLessorSummaryFile').submit(function(e) {
                var p;

                e.preventDefault();
                var formData = new FormData($(this)[0]);
                formData.append('OrgID', sessionStorage.OrgID);

                $.ajax({
                    url: '/ERP/ImportMallLessorSummaryFile',
                    type: 'POST',
                    data: formData,
                    async: false,
                    cache: false,
                    contentType: false,
                    processData: false,
                    success: function (vData) {
                        try {
                            vData = JSON.parse(vData);
                            if (vData.d)
                                vData = vData.d;
                        } catch (e) {
                            NewMessageBox('An error occured while parsing the server response to JSON: ' + e.message);
                            return;
                        }
                        if (vData.Error)
                            NewMessageBox(vData.Error);
                        else if (vData.Warning)
                            NewMessageBox(vData.Warning);
                        else if (vData.Result == 'ok')
                            NewMessageBox('The data was successfully saved.');
                        else
                            NewMessageBox('Unknown status. ref jsHCTRGFCAGZ!');
                        mOut.AfterSave();
                    }
                });
            });


        });
    });