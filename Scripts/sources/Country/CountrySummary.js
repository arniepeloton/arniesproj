require([
    'common'
    ], function () {

        $(function () {

            var mLoaded = false;

            $.mobile.activePage.bind('pageshow', PageShow);
            function PageShow() {
                mLoaded = true;
                AjaxSend({
                    url: 'Country/IsNewEntry',
                    success: function (vData) {

                        ShowSumCountryInfo(vData.CountryID)

                    }
                });
            };
            if (!mLoaded) PageShow();

            $('#btnEditSumCountryInfo').click(function (e) {
                e.preventDefault();

                window.IsPageLoaded = false;
                ChangePage('/Country/CountryInfo');
            });

            $('#btnEditSumCountryLanguage').click(function (e) {
                e.preventDefault();

                ChangePage('/Country/CountryLanguage');
            });

            $('#btnSummaryCountryDone').click(function (e) {
                e.preventDefault();

                $.mobile.changePage("#" + $(window.pageOrig).attr('id'));
            });

            function ShowSumCountryInfo(strID) {
                var pData = {};
                pData.CountryID = strID;

                ClearSumCountryFields();

                AjaxSend({
                    url: "Country/getCountryInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.CountryData.Rows.length > 0) {

                            $("#txtSumCountryID").val(vData.CountryData.Rows[0].ID || '');

                            $("#txtSumCountryName").val(vData.CountryData.Rows[0].Country || '');

                            $("#txtSumCountryCode").val(vData.CountryData.Rows[0].Code || '');
                            $("#txtSumCountryAlpha2Code").val(vData.CountryData.Rows[0].Alpha2Code || '');
                            $("#txtSumCountryNumericCode").val(vData.CountryData.Rows[0].NumericCode || '');
                            $("#txtSumCountryISOCode").val(vData.CountryData.Rows[0].ISOCode || '');
                            $("#txtSumCountryCallingCode").val(vData.CountryData.Rows[0].CallingCode || '');

                            getSumCountryLanguageList();
                        }
                    }
                });

            };

            function ClearSumCountryFields() {

                $("#txtSumCountryID").val('');

                $("#txtSumCountryName").val('');

                $("#txtSumCountryCode").val('');
                $("#txtSumCountryAlpha2Code").val('');
                $("#txtSumCountryNumericCode").val('');
                $("#txtSumCountryISOCode").val('');
                $("#txtSumCountryCallingCode").val('');

            };

            function getSumCountryLanguageList() {
                AjaxSend({
                    url: 'Country/getCountryLanguageList',
                    success: function (vData) {
                        LoadSumCountryLanguageList(vData.CountryData.Rows);
                    }
                });
            }

            function LoadSumCountryLanguageList(vRows) {
                var pLV = $('#lstSumCountryLanguage');
                var pItem;

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li>')
                        .append($('<h3>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].Language)
                        );
                    pLV.append(pItem);
                };

                pLV.listview('refresh');
            };











        });
    });


