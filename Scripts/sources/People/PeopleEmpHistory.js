require([
    'common',
    GetPathFormat('/Scripts3/Browser/LibraryJobOccupation.js?crc=')
    ], function() {

    $(function () {

        var IsNewJobOcc = true;
        var arJobOccupation = new Array();
        var intJobOccupationCntr = 0;
        var intJobOccupationIndex = 0;

        var intJobOccupationRecID = 0;
        var intJobOccupationID = 1;
        var intJobOccupation = 2;
        var intJobOccupationDateStartMonth = 3;
        var intJobOccupationDateStartDay = 4;
        var intJobOccupationDateStartYear = 5;
        var intJobOccupationDateEndMonth = 6;
        var intJobOccupationDateEndDay = 7;
        var intJobOccupationDateEndYear = 8;
        var intJobOccupationIsUntilPresent = 9;
        var intJobOccupationState = 10;


        $(document).ready(function () {

            setMonthCollection("selDateStartMonth");
            setYearCollection("selDateStartYear");

            setMonthCollection("selDateEndMonth");
            setYearCollection("selDateEndYear");

            getPeopleEmpHistList();
        });

        $('.btnProfileBackToHome').click(function () {
            if (GetUrlParameters().p == 'eas') {
                location.href = MergeUrlParameters({
                    OrgID: sessionStorage.OrgID,
                    IsMobile: 0
                }, '/erp')
            } else if (GetUrlParameters().p == 'crm') {
                location.href = MergeUrlParameters({
                    OrgID: sessionStorage.OrgID,
                    IsMobile: 0
                }, '/CRMDashboard')
            } else {
                location.href = '/People/PeopleMenu';
            }
        })

        $('#btnPeopleEmpHistPopUpMenu').click(function (e) {
            e.preventDefault();

            $("#divPeopleEmpHistPopUpMenu").popup("open")

        });

        $('#popPeopleEmpHistPeopleInfoName').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoName')), "_self");

        });

        $('#popPeopleEmpHistPeopleInfoAlias').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoAlias')), "_self");

        });

        $('#popPeopleEmpHistPeopleInfoAddress').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoAddress')), "_self");

        });

        $('#popPeopleEmpHistPeoplePersonalInfo').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeoplePersonalInfo')), "_self");

        });

        $('#popPeopleEmpHistPeopleContactNo').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleContactNo')), "_self");

        });

        $('#popPeopleEmpHistPeopleEducation').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleEducation')), "_self");

        });

        $('#popPeopleEmpHistPeopleRelatedPerson').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleRelatedPerson')), "_self");

        });

        $('#popPeopleEmpHistPeoplePrimeIDReference').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeoplePrimeIDReference')), "_self");

        });

        $('#popPeopleEmpHistPeopleUserAccount').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleUserAccount')), "_self");

        });

        $('#btnPeopleEmpHistAdd').click(function (e) {
            e.preventDefault();

            $.mobile.changePage('#pagePeopleEmpHistInfo');

            // dont clear
            // sessionStorage.clear();
            proj.ClearSessionStorage();
            ClearPeopleEmpHistFields();

        });

        $('#btnAddOccupation').click(function (e) {

            $.mobile.changePage('#pagePeopleEmpHistJobOcc');

            IsNewJobOcc = true
            intJobOccupationCntr = intJobOccupationCntr + 1;
            intJobOccupationIndex = intJobOccupationCntr;

            ClearPeopleEmpHistJobOccFields();

        });

        $('#btnJobOccupation').click(function (e) {

            Browser.LibraryJobOccupation.Click = function (e) {

                $('#txtJobOccupationID').val(e.ID)
                $('#btnJobOccupation').text(e.JobOccupation)
                    .button('refresh');

            };
            Browser.LibraryJobOccupation.Show();

        });

        $('#selDateStartMonth').change(function (e) {
            e.preventDefault();

            setDayCollection('selDateStartMonth', 'selDateStartDay', 'selDateStartYear', true);

        });

        $('#selDateStartYear').change(function (e) {
            e.preventDefault();

            setDayCollection('selDateStartMonth', 'selDateStartDay', 'selDateStartYear', true);

        });

        $('#selDateStartYear').click(function (e) {
            e.preventDefault();

            if ($.trim($('#selDateStartYear').val()) == "") {
                $('#selDateStartYear').val('1980');
                $('#selDateStartYear').selectmenu('refresh');
            }

        });

        $('#selDateEndMonth').change(function (e) {
            e.preventDefault();

            setDayCollection('selDateEndMonth', 'selDateEndDay', 'selDateEndYear', true);

        });

        $('#selDateEndYear').change(function (e) {
            e.preventDefault();

            setDayCollection('selDateEndMonth', 'selDateEndDay', 'selDateEndYear', true);

        });

        $('#selDateEndYear').click(function (e) {
            e.preventDefault();

            if ($.trim($('#selDateEndYear').val()) == "") {
                $('#selDateEndYear').val('1980');
                $('#selDateEndYear').selectmenu('refresh');
            }

        });

        $('#chkIsUntilPresent').click(function () {

            setDateEnded($(this).is(":checked"));

        });

        $('#btnPeopleEmpHistSave').click(function (e) {
            e.preventDefault();

            if ($.trim($("#txtCompanyName").val()) == "") {
                MessageBox("Please enter company name.",
                            "Company Name",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }

            if (intJobOccupationCntr <= 0) {
                MessageBox("Please enter job occupation.",
                            "Job Occupation",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }

            SavePeopleEmpHist();

        });

        $('#btnPeopleEmpHistRemove').click(function (e) {
            e.preventDefault();

            MessageBox("Remove this record?",
                'Remove',
                [
                    { Text: 'Yes',
                        Click: function () {
                            RemovePeopleEmpHist();
                        }
                    }
                ,
                    { Text: 'No'
                    }
                ]
            );

        });

        $('#btnPeopleEmpHistCancel').click(function (e) {
            e.preventDefault();

            $.mobile.changePage('#pagePeopleEmpHistList');

        });

        $('#btnPeopleEmpHistJobOccSave').click(function (e) {
            e.preventDefault();

            if ($.trim($("#txtJobOccupationID").val()) == "") {
                MessageBox("Please select job occupation.",
                            "Job Occupation",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }

            if ($.trim($("#selDateStartMonth").val()) == "") {
                MessageBox("Please enter valid start date of employment.",
                                "Date Start",
                                [
                                    { Text: 'Ok' }
                                ]
                    );
                return;
            }
            if ($.trim($("#selDateStartDay").val()) == "") {
                MessageBox("Please enter valid start date of employment.",
                                "Date Start",
                                [
                                    { Text: 'Ok' }
                                ]
                    );
                return;
            }
            if ($.trim($("#selDateStartYear").val()) == "") {
                MessageBox("Please enter valid start date of employment.",
                                "Date Start",
                                [
                                    { Text: 'Ok' }
                                ]
                    );
                return;
            }

            if (!$("#chkIsUntilPresent").is(":checked")) {
                if ($.trim($("#selDateEndMonth").val()) == "") {
                    MessageBox("Please enter valid end date of employment.",
                                    "Date End",
                                    [
                                        { Text: 'Ok' }
                                    ]
                        );
                    return;
                }
                if ($.trim($("#selDateEndDay").val()) == "") {
                    MessageBox("Please enter valid end date of employment.",
                                    "Date End",
                                    [
                                        { Text: 'Ok' }
                                    ]
                        );
                    return;
                }
                if ($.trim($("#selDateEndYear").val()) == "") {
                    MessageBox("Please enter valid end date of employment.",
                                    "Date End",
                                    [
                                        { Text: 'Ok' }
                                    ]
                        );
                    return;
                }
            }


            var dtDateStart = $.trim($("#selDateStartMonth").val()) + '/' + $.trim($("#selDateStartDay").val()) + '/' + $.trim($("#selDateStartYear").val());
            var dtDateEnd = $.trim($("#selDateEndMonth").val()) + '/' + $.trim($("#selDateEndDay").val()) + '/' + $.trim($("#selDateEndYear").val());

            if ((new Date(dtDateStart)) > (new Date(dtDateEnd))) {
                MessageBox('Time travelling is not allowed. You cannot end earlier than when you started. Please review your “Date Started” and “Date Ended” information.',
                                "Date Started and Date Ended",
                                [
                                    { Text: 'Ok' }
                                ]
                    );
                return;
            }


            SavePeopleEmpHistJobOcc();

        });

        $('#btnPeopleEmpHistJobOccRemove').click(function (e) {
            e.preventDefault();

            MessageBox("Remove this record?",
                'Remove',
                [
                    { Text: 'Yes',
                        Click: function () {
                            RemovePeopleEmpHistJobOcc();
                        }
                    }
                ,
                    { Text: 'No'
                    }
                ]
            );

        });

        $('#btnPeopleEmpHistJobOccCancel').click(function (e) {
            e.preventDefault();

            $.mobile.changePage('#pagePeopleEmpHistInfo');

        });

        $('#btnPeopleEmpHistDone').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoSummary')), "_self");

        });

        function getPeopleEmpHistList() {
            AjaxSend({
                url: 'People/getPeopleEmpHistList',
                success: function (vData) {
                    LoadPeopleEmpHistList(vData.PeopleEmpHistList.Rows);
                }
            });
        }

        function LoadPeopleEmpHistList(vRows) {
            var pLV = $('#lstPeopleEmpHist');
            var pItem;

            $("#divRecordCount").text(" Number of record(s) found: " + vRows.length);

            pLV.html('');

            pItem = $('<li>')
                    .attr('data-role', "list-divider")
                    .text('Employment History List');
            pLV.append(pItem);

            for (var p = 0; p <= vRows.length - 1; p++) {

                pItem = $('<li>')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();
                                $.mobile.changePage('#pagePeopleEmpHistInfo');
                                ShowPersonEmpHistInfo($(this).attr('ID'));
                            })
                            .append($('<h3>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].CompanyName)
                            )
                            .append($('<p>')
                                .attr("style", "white-space:normal")
                                .text($.format.date(new Date(vRows[p].DateStart), 'MMM dd, yyyy') + ' - ' + (vRows[p].IsUntilPresent == "0" ? $.format.date(new Date(vRows[p].DateEnd), 'MMM dd, yyyy') : 'Present'))
                            )
                        );
                pLV.append(pItem);
            };

            pLV.listview('refresh');
        };

        function ShowPersonEmpHistInfo(strID) {
            var pLV = $('#lstPeopleEmpHistOcc');
            var pItem;
            var pData = {};
            pData.ID = strID;

            // dont clear
            // sessionStorage.clear();
            proj.ClearSessionStorage();
            ClearPeopleEmpHistFields();

            AjaxSend({
                url: "People/getPeopleEmpHistInfo",
                data: pData,
                success: function (vData) {

                    if (vData.PeopleEmpHistInfo.Rows.length > 0) {

                        $("#txtEmpHistID").val(vData.PeopleEmpHistInfo.Rows[0].ID || '');

                        $("#txtCompanyName").val(vData.PeopleEmpHistInfo.Rows[0].CompanyName || '');

                        AjaxSend({
                            url: "People/getPeopleEmpHistJobOccList",
                            data: pData,
                            success: function (vDataJobOcc) {

                                LoadPeopleEmpHistJobOccList(vDataJobOcc.PeopleEmpHistJobOccInfo.Rows);

                            }
                        });

                        $("#btnPeopleEmpHistRemove").show();
                    }
                }
            });

        };

        function LoadPeopleEmpHistJobOccList(vRows) {

            for (var p = 0; p <= vRows.length - 1; p++) {

                arJobOccupation = new Array();
                arJobOccupation[intJobOccupationRecID] = vRows[p].ID;
                arJobOccupation[intJobOccupationID] = vRows[p].OccupationID;
                arJobOccupation[intJobOccupation] = vRows[p].JobOccupation;
                arJobOccupation[intJobOccupationDateStartMonth] = vRows[p].DateStartMonth;
                arJobOccupation[intJobOccupationDateStartDay] = vRows[p].DateStartDay;
                arJobOccupation[intJobOccupationDateStartYear] = vRows[p].DateStartYear;
                arJobOccupation[intJobOccupationDateEndMonth] = vRows[p].DateEndMonth;
                arJobOccupation[intJobOccupationDateEndDay] = vRows[p].DateEndDay;
                arJobOccupation[intJobOccupationDateEndYear] = vRows[p].DateEndYear;
                arJobOccupation[intJobOccupationIsUntilPresent] = (vRows[p].IsUntilPresent == 0 ? false : true);
                arJobOccupation[intJobOccupationState] = "";

                sessionStorage["Job-" + p] = JSON.stringify(arJobOccupation);

            };

            intJobOccupationCntr = vRows.length;
            LoadJobOccList();

        };

        function ShowPersonEmpHistJobOccInfo(arData, index) {

            $.mobile.changePage('#pagePeopleEmpHistJobOcc');

            ClearPeopleEmpHistJobOccFields();

            intJobOccupationIndex = index;

            $("#txtEmpHistJobOccID").val(arData[intJobOccupationRecID]);

            $("#txtJobOccupationID").val(arData[intJobOccupationID]);
            $("#btnJobOccupation").text(arData[intJobOccupation]);
            $("#btnJobOccupation").button('refresh');

            $("#selDateStartMonth").val(arData[intJobOccupationDateStartMonth]);
            $('#selDateStartMonth').selectmenu('refresh');
            $("#selDateStartYear").val(arData[intJobOccupationDateStartYear]);
            $('#selDateStartYear').selectmenu('refresh');
            setDayCollection('selDateStartMonth', 'selDateStartDay', 'selDateStartYear', true);
            $("#selDateStartDay").val(arData[intJobOccupationDateStartDay]);
            $('#selDateStartDay').selectmenu('refresh');

            $("#selDateEndMonth").val(arData[intJobOccupationDateEndMonth]);
            $('#selDateEndMonth').selectmenu('refresh');
            $("#selDateEndYear").val(arData[intJobOccupationDateEndYear]);
            $('#selDateEndYear').selectmenu('refresh');
            setDayCollection('selDateEndMonth', 'selDateEndDay', 'selDateEndYear', true);
            $("#selDateEndDay").val(arData[intJobOccupationDateEndDay]);
            $('#selDateEndDay').selectmenu('refresh');

            $('#chkIsUntilPresent').attr("checked", arData[intJobOccupationIsUntilPresent]).checkboxradio("refresh");
            setDateEnded(arData[intJobOccupationIsUntilPresent]);

            IsNewJobOcc = false;

            $("#btnPeopleEmpHistJobOccRemove").show();

        };

        function setDateEnded(IsUntilPresent) {

            $('#selDateEndMonth').attr("disabled", IsUntilPresent);
            $('#selDateEndDay').attr("disabled", IsUntilPresent);
            $('#selDateEndYear').attr("disabled", IsUntilPresent);

            if (IsUntilPresent) {
                $('#selDateEndMonth').val('');
                $('#selDateEndMonth').selectmenu('refresh');
                $('#selDateEndDay').val('');
                $('#selDateEndDay').selectmenu('refresh');
                $('#selDateEndYear').val('');
                $('#selDateEndYear').selectmenu('refresh');
            }

        };

        function ClearPeopleEmpHistFields() {

            intJobOccupationCntr = 0;

            $("#txtEmpHistID").val('');
            $("#txtCompanyName").val('');

            LoadJobOccList();

            $("#btnPeopleEmpHistRemove").hide();

        }

        function ClearPeopleEmpHistJobOccFields() {

            $("#txtEmpHistJobOccID").val('');

            $("#txtJobOccupationID").val('');
            $("#btnJobOccupation").text('Please Select');
            $("#btnJobOccupation").button('refresh');

            $("#selDateStartMonth").val('');
            $('#selDateStartMonth').selectmenu('refresh');
            $("#selDateStartDay").val('');
            $('#selDateStartDay').selectmenu('refresh');
            $("#selDateStartYear").val('');
            $('#selDateStartYear').selectmenu('refresh');

            $("#selDateEndMonth").val('');
            $('#selDateEndMonth').selectmenu('refresh');
            $("#selDateEndDay").val('');
            $('#selDateEndDay').selectmenu('refresh');
            $("#selDateEndYear").val('');
            $('#selDateEndYear').selectmenu('refresh');

            $('#chkIsUntilPresent').attr("checked", false).checkboxradio("refresh"); ;
            setDateEnded(false);

            $("#btnPeopleEmpHistJobOccRemove").hide();

        }

        function SavePeopleEmpHist() {
            var pData = [];
            var intIndex;

            intIndex = 0;
            for (var i = 0; i <= intJobOccupationCntr; i++) {

                if (sessionStorage["Job-" + i] != undefined) {

                    arJobOccupation = new Array();
                    arJobOccupation = JSON.parse(sessionStorage["Job-" + i]);

                    pData[intIndex] = {
                        ID: arJobOccupation[intJobOccupationRecID],
                        JobOccupationID: arJobOccupation[intJobOccupationID],
                        DateStart: arJobOccupation[intJobOccupationDateStartMonth] + '/' + arJobOccupation[intJobOccupationDateStartDay] + '/' + arJobOccupation[intJobOccupationDateStartYear],
                        DateEnd: arJobOccupation[intJobOccupationDateEndMonth] + '/' + arJobOccupation[intJobOccupationDateEndDay] + '/' + arJobOccupation[intJobOccupationDateEndYear],
                        IsUntilPresent: (arJobOccupation[intJobOccupationIsUntilPresent] ? 1 : 0),
                        RecState: arJobOccupation[intJobOccupationState]
                    };

                    intIndex = intIndex + 1;

                }
            }

            pData = {
                PersonEmpHistID: $("#txtEmpHistID").val(),
                CompanyName: $("#txtCompanyName").val(),
                JobOccList: pData
            };


            AjaxSend({
                url: "People/SavePeopleEmpHistory",
                data: pData,
                success: function (vData) {

                    // dont clear
                    // sessionStorage.clear()
                    proj.ClearSessionStorage();
                    $.mobile.changePage('#pagePeopleEmpHistList');
                    getPeopleEmpHistList();

                }
            });

        };

        function RemovePeopleEmpHist() {

            var pData = {};
            pData.PersonEmpHistID = $("#txtEmpHistID").val();

            AjaxSend({
                url: "People/RemovePeopleEmpHistory",
                data: pData,
                success: function (vData) {

                    $.mobile.changePage('#pagePeopleEmpHistList');
                    getPeopleEmpHistList();

                }
            });

        };

        function SavePeopleEmpHistJobOcc() {

            arJobOccupation = new Array();
            arJobOccupation[intJobOccupationRecID] = $("#txtEmpHistJobOccID").val();
            arJobOccupation[intJobOccupationID] = $("#txtJobOccupationID").val();
            arJobOccupation[intJobOccupation] = $("#btnJobOccupation").text();
            arJobOccupation[intJobOccupationDateStartMonth] = $("#selDateStartMonth").val();
            arJobOccupation[intJobOccupationDateStartDay] = $("#selDateStartDay").val();
            arJobOccupation[intJobOccupationDateStartYear] = $("#selDateStartYear").val();
            arJobOccupation[intJobOccupationDateEndMonth] = $("#selDateEndMonth").val();
            arJobOccupation[intJobOccupationDateEndDay] = $("#selDateEndDay").val();
            arJobOccupation[intJobOccupationDateEndYear] = $("#selDateEndYear").val();
            arJobOccupation[intJobOccupationIsUntilPresent] = $("#chkIsUntilPresent").is(":checked")

            arJobOccupation[intJobOccupationState] = (IsNewJobOcc ? "New" : "Edit");

            sessionStorage["Job-" + intJobOccupationIndex] = JSON.stringify(arJobOccupation);

            $.mobile.changePage('#pagePeopleEmpHistInfo');
            LoadJobOccList();

        };

        function RemovePeopleEmpHistJobOcc() {

            arJobOccupation = new Array();
            arJobOccupation[intJobOccupationRecID] = $("#txtEmpHistJobOccID").val();
            arJobOccupation[intJobOccupationID] = $("#txtJobOccupationID").val();
            arJobOccupation[intJobOccupation] = $("#btnJobOccupation").text();
            arJobOccupation[intJobOccupationDateStartMonth] = $("#selDateStartMonth").val();
            arJobOccupation[intJobOccupationDateStartDay] = $("#selDateStartDay").val();
            arJobOccupation[intJobOccupationDateStartYear] = $("#selDateStartYear").val();
            arJobOccupation[intJobOccupationDateEndMonth] = $("#selDateEndMonth").val();
            arJobOccupation[intJobOccupationDateEndDay] = $("#selDateEndDay").val();
            arJobOccupation[intJobOccupationDateEndYear] = $("#selDateEndYear").val();
            arJobOccupation[intJobOccupationIsUntilPresent] = $("#chkIsUntilPresent").is(":checked")
            arJobOccupation[intJobOccupationState] = "Remove";

            sessionStorage["Job-" + intJobOccupationIndex] = JSON.stringify(arJobOccupation);

            $.mobile.changePage('#pagePeopleEmpHistInfo');
            LoadJobOccList();
        };

        function LoadJobOccList() {
            var pLV = $('#lstPeopleEmpHistOcc');
            var pItem;

            pLV.html('');
            for (var i = 0; i <= intJobOccupationCntr; i++) {

                if (sessionStorage["Job-" + i] != undefined) {

                    arJobOccupation = new Array();
                    arJobOccupation = JSON.parse(sessionStorage["Job-" + i]);

                    if (arJobOccupation[intJobOccupationState] != "Remove") {

                        var dtDateStart = arJobOccupation[intJobOccupationDateStartMonth] + '/' + arJobOccupation[intJobOccupationDateStartDay] + '/' + arJobOccupation[intJobOccupationDateStartYear];
                        var dtDateEnd = arJobOccupation[intJobOccupationDateEndMonth] + '/' + arJobOccupation[intJobOccupationDateEndDay] + '/' + arJobOccupation[intJobOccupationDateEndYear];

                        pItem = $('<li id="lst' + i + '">')
                        .append($('<a>')
                            .attr('ID', i)
                            .data('Data', arJobOccupation)
                            .append($('<h3>')
                                .attr("style", "white-space:normal")
                                .text(arJobOccupation[intJobOccupation])
                            )
                            .append($('<p>')
                                .attr("style", "white-space:normal")
                                .text($.format.date(new Date(dtDateStart), 'MMM dd, yyyy') + ' - ' + (arJobOccupation[intJobOccupationIsUntilPresent] ? 'Present' : $.format.date(new Date(dtDateEnd), 'MMM dd, yyyy')))
                            )
                            .click(function (e) {
                                e.preventDefault();

                                ShowPersonEmpHistJobOccInfo($(this).data('Data'), $(this).attr('ID'));

                            })
                         )

                        pLV.append(pItem);

                    }

                }
            }

            pLV.listview('refresh');
        };

    });
});