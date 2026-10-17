require([
    'common'
    ], function() {
    
    $(function () {

        var mLoaded = false;

        $.mobile.activePage.bind('pageshow', PageShow);
        function PageShow() {
            mLoaded = true;

            if (!window.IsPageLoaded) {

                AjaxSend({
                    url: 'Country/IsNewEntry',
                    success: function (vData) {

                        ShowCountryInfo(vData.CountryID || '')

                    }
                });

                window.IsPageLoaded = true;
            }
        };
        if (!mLoaded) PageShow();

        $('#btnCountryInfoDone').click(function (e) {
            e.preventDefault();

            if ($.trim($("#txtCountryName").val()) == "" || $.trim($("#txtCountryName").val()).length <= 1) {
                MessageBox("Please enter country name.",
                            "Country Name",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }

            if ($.trim($("#txtCountryCode").val()) == "" || $.trim($("#txtCountryCode").val()).length != 3) {
                MessageBox("Please enter alpha three (3) code.",
                            "Alpha Three (3) Code",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }

            if ($.trim($("#txtCountryCallingCode").val()) == "") {
                MessageBox("Please enter country calling code.",
                            "Calling Code",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }

            SaveCountryInfo();

        });

        $("#txtCountryNumericCode").keydown(function (e) {
            if (!NumberOnly(e)) {
                return;
            }
        });

        function ShowCountryInfo(strID) {
            var pData = {};
            pData.CountryID = strID;

            ClearCountryFields();

            AjaxSend({
                url: "Country/getCountryInfo",
                data: pData,
                success: function (vData) {

                    if (vData.CountryData.Rows.length > 0) {

                        $("#txtCountryID").val(vData.CountryData.Rows[0].ID || '');

                        $("#txtCountryName").val(vData.CountryData.Rows[0].Country || '');

                        $("#txtCountryCode").val(vData.CountryData.Rows[0].Code || '');
                        $("#txtCountryAlpha2Code").val(vData.CountryData.Rows[0].Alpha2Code || '');
                        $("#txtCountryNumericCode").val(vData.CountryData.Rows[0].NumericCode || '');
                        $("#txtCountryISOCode").val(vData.CountryData.Rows[0].ISOCode || '');
                        $("#txtCountryCallingCode").val(vData.CountryData.Rows[0].CallingCode || '');
                    }
                }
            });

        };

        function ClearCountryFields() {

            $("#txtCountryID").val('');

            $("#txtCountryName").val('');

            $("#txtCountryCode").val('');
            $("#txtCountryAlpha2Code").val('');
            $("#txtCountryNumericCode").val('');
            $("#txtCountryISOCode").val('');
            $("#txtCountryCallingCode").val('');
        }

        function SaveCountryInfo() {
            var pData = [];

            pData = {
                CountryID: $.trim($("#txtCountryID").val()),
                CountryName: $.trim($("#txtCountryName").val()),
                Code: $.trim($("#txtCountryCode").val()),
                Alpha2Code: $.trim($("#txtCountryAlpha2Code").val()),
                NumericCode: $.trim($("#txtCountryNumericCode").val()),
                ISOCode: $("#txtCountryISOCode").val(),
                CallingCode: $("#txtCountryCallingCode").val()
            };

            AjaxSend({
                url: "Country/CheckCountryUniqueName",
                data: pData,
                success: function (vData) {

                    if (vData.CountryData.Rows.length > 0) {
                        MessageBox("Country with the same name already exist.",
                            "Country Name",
                            [
                                { Text: 'Ok' }
                            ]
                        );

                    }
                    else {
                        AjaxSend({
                            url: "Country/SaveCountryInfo",
                            data: pData,
                            success: function (vSaveData) {

                                if (vSaveData.CountryData == "NumericCodeInvalid") {
                                    MessageBox("Please enter valid numeric code.",
                                        "Numeric Code",
                                        [
                                            { Text: 'Ok' }
                                        ]
                                    );
                                }
                                else {

                                    $("#txtCountryID").val(vSaveData.CountryData || '');

                                    if (window.pageSourcePage != undefined) {
                                        $.mobile.changePage("#" + $(window.pageSourcePage).attr('id'));
                                    }
                                    else {
                                        ChangePage('/Country/CountrySummary');
                                    }
                                }

                            }
                        });

                    }

                }
            });

        };


    });
});