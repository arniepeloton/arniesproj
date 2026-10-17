require([
    'common'
    ], function () {

        $(function () {

            $('#frmCountryBrowser').submit(function (e) {
                e.preventDefault();

                ShowCountry();

            });

            $('#btnAddNewCountry').click(function () {

                CreateNewCountryEntry(1, '');

            });

            function ShowCountry() {

                var pData = {};
                pData.SearchString = $("#txtCountrySearch").val();

                AjaxSend({
                    url: 'Country/getFullCountryList',
                    data: pData,
                    success: function (vData) {
                        LoadCountryList(vData.CountryData.Rows);
                    }
                });

            }

            function LoadCountryList(vRows) {
                var pLV = $('#lstCountry');
                var pItem;

                $("#divCountrySearchResults").hide();
                $("#divCountrySearchNoResults").hide();
                $("#btnAddNewCountry").hide();

                if (vRows.length > 0) {
                    $("#divCountrySearchResults").show();
                    $("#divCountryRecordCount").text(" Number of record(s) found: " + vRows.length);

                    pLV.html('');
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        pItem = $('<li>')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();

                                CreateNewCountryEntry(0, $(this).attr('ID'));

                            })
                            .append($('<h3>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].Code + " - " + vRows[p].Country)
                            )
                        );
                        pLV.append(pItem);
                    };

                    pLV.listview('refresh');
                }
                else {
                    $("#divCountrySearchNoResults").show();
                    $("#btnAddNewCountry").show();
                    $("#lblCountryNotFound").text('The country "' + $("#txtCountrySearch").val() + '" does not exist. Please consider to search thoroughly using an alternate spelling before creating a new country to avoid duplication.');
                }
            };

            function CreateNewCountryEntry(IsNew, strID) {
                var pData = {};
                pData.IsNew = IsNew;
                pData.ID = strID;

                window.pageOrig = $.mobile.activePage;

                AjaxSend({
                    url: 'Country/setCreateNew',
                    data: pData,
                    success: function (vData) {

                        if ((vData.IsNew || '') == "TRUE") {
                            ChangePage('/Country/CountryInfo');
                        }
                        else {
                            ChangePage('/Country/CountrySummary');
                        }
                    }
                });

            }





        });
    });