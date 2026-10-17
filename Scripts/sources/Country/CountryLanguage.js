require([
    'common'
    ], function() {

    $(function () {

        var mLoaded = false;

        $.mobile.activePage.bind('pageshow', PageShow);
        function PageShow() {
            mLoaded = true;
            getCountryLanguageList();
        };
        if (!mLoaded) PageShow();

        $('#btnCountryLanguageAdd').click(function (e) {
            e.preventDefault();

            $.mobile.changePage('#pageCountryLanguageDetails');
            ClearCountryLanguageFields();

        });

        $('#btnCountryLanguageSave').click(function (e) {
            e.preventDefault();

            if ($.trim($("#txtCountryLanguage").val()) == "") {
                MessageBox("Please enter country language.",
                            "Surname",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }

            SaveCountryLanguageInfo();

        });

        $('#btnCountryLanguageRemove').click(function (e) {
            e.preventDefault();

            MessageBox("Remove this record?",
                'Remove',
                [
                    { Text: 'Yes',
                        Click: function () {
                            RemoveCountryLanguage();
                        }
                    }
                ,
                    { Text: 'No'
                    }
                ]
            );

        });

        $('#btnCountryLanguageCancel').click(function (e) {
            e.preventDefault();

            ChangePage('/Country/CountryLanguage');

        });

        $('#btnCountryLanguageDone').click(function (e) {
            e.preventDefault();

            if (window.pageSourcePage != undefined) {
                $.mobile.changePage("#" + $(window.pageSourcePage).attr('id'));
            }
            else {
                ChangePage('/Country/CountrySummary');
            }

        });

        function getCountryLanguageList() {
            AjaxSend({
                url: 'Country/getCountryLanguageList',
                success: function (vData) {
                    LoadCountryLanguageList(vData.CountryData.Rows);
                }
            });
        }

        function LoadCountryLanguageList(vRows) {
            var pLV = $('#lstCountryLanguage');
            var pItem;

            $("#divCountryLanguageRecordCount").text(" Number of record(s) found: " + vRows.length);

            pLV.html('');

            pItem = $('<li>')
                    .attr('data-role', "list-divider")
                    .text('Language List');
            pLV.append(pItem);

            for (var p = 0; p <= vRows.length - 1; p++) {
                pItem = $('<li>')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();
                                $.mobile.changePage('#pageCountryLanguageDetails');
                                ShowCountryLanguageInfo($(this).attr('ID'));
                            })
                            .append($('<h3>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].Language)
                            )
                        );
                pLV.append(pItem);
            };

            pLV.listview('refresh');
        };

        function ShowCountryLanguageInfo(strID) {
            var pData = {};
            pData.ID = strID;

            ClearCountryLanguageFields();

            AjaxSend({
                url: "Country/getCountryLanguageInfo",
                data: pData,
                success: function (vData) {

                    if (vData.CountryData.Rows.length > 0) {

                        $("#txtCountryLanguageID").val(vData.CountryData.Rows[0].ID || '');
                        $("#txtCountryLanguage").val(vData.CountryData.Rows[0].Language || '');

//                        $("#btnCountryLanguageRemove").show();
                    }
                }
            });

        };

        function ClearCountryLanguageFields() {

            $("#txtCountryLanguageID").val('');
            $("#txtCountryLanguage").val('');

            $("#btnCountryLanguageRemove").hide();

        }

        function SaveCountryLanguageInfo() {

            var pData = [];
            var intIndex;

            pData = {
                ID: $("#txtCountryLanguageID").val(),
                CountryLanguage: $.trim($("#txtCountryLanguage").val())
            };

            AjaxSend({
                url: "Country/CheckCountryLanguage",
                data: pData,
                success: function (vData) {

                    if (vData.CountryData.Rows.length > 0) {
                        MessageBox("Language already exist.",
                            "Language",
                            [
                                { Text: 'Ok' }
                            ]
                        );
                    }
                    else {

                        AjaxSend({
                            url: "Country/SaveCountryLanguage",
                            data: pData,
                            success: function (vData) {

                                ChangePage('/Country/CountryLanguage');
                                getCountryLanguageList();

                            }
                        });

                    }

                }
            });

        };

        function RemoveCountryLanguage() {

            var pData = {};
            pData.ID = $("#txtCountryLanguageID").val();

            AjaxSend({
                url: "Country/RemoveCountryLanguage",
                data: pData,
                success: function (vData) {

                    ChangePage('/Country/CountryLanguage');
                    getCountryLanguageList();

                }
            });

        };


    });
});