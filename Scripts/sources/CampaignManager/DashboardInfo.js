require([
    'common'
    ], function () {

        $(function () {

            $('div[data-role="dialog"]').live('pagebeforeshow', function (e, ui) {
                // make parent page transparent
                ui.prevPage.addClass("ui-dialog-background ");
            });

            $('div[data-role="dialog"]').live('pagehide', function (e, ui) {
                $(".ui-dialog-background ").removeClass("ui-dialog-background ");
            });

            $('#txtSearch').keypress(function (e) {
                if (e.keyCode == 13){
                    SearchDashboard();
                }
            });

        }); //End Inner function

        function SearchDashboard() {

            AjaxSend({
                url: "Dashboard/getSearchResult",
                data: { 'Keywords': $("#txtSearch").val() },
                success: function (vData) {
                    DisplayDashboard(vData.DashboardSearchResults.Rows)
                    $("#divRecordCount").text("No. of records: " + vData.DashboardSearchResults.Rows.length);
                }
            });

        }

        
        function DisplayDetails(id) {
            var data = { ID: id }

            AjaxSend({
                url: 'Dashboard/getDetails',
                data: data,
                success: function (vData) {
//                    var prefix = vData.Details.Rows[0].Prefix;
//                    var areaCode = '(' + vData.Details.Rows[0].AreaCode + ') ';
//                    var countryCodeTel = '(' + vData.Details.Rows[0].CallingCode + ') ';
//                    var countryCodeMobile = '+' + vData.Details.Rows[0].CountryCode;
//                    var productName = '';

//                    $('#lblBusinessName').text(vData.Details.Rows[0].BusinessName);
//                    $('#lblBusinessIndustry').text(vData.Details.Rows[0].BusinessIndustry);
//                    $('#lblCountry').text(vData.Details.Rows[0].Country);
//                    $('#lblCity').text(vData.Details.Rows[0].City);
//                    $('#lblContactPerson').text(vData.Details.Rows[0].LastName + ', ' + vData.Details.Rows[0].FirstName + ' ' + vData.Details.Rows[0].MiddleName);


//                    $('#lblTel').text(countryCodeTel + areaCode + vData.Details.Rows[0].TelephoneNo);

//                    if (vData.Details.Rows[0].MobileNo == '' || vData.Details.Rows[0].MobileNo == null) {
//                        countryCodeMobile = '';
//                        prefix = '';
//                    } else {
//                        if (vData.Details.Rows[0].CountryCode == '' || vData.Details.Rows[0].CountryCode == null) {
//                            countryCodeMobile = '+--';
//                            prefix = '--';
//                        }
//                    }

//                    $('#lblMobile').text(countryCodeMobile + prefix + vData.Details.Rows[0].MobileNo);

//                    $('#lblEmail').text(vData.Details.Rows[0].EmailAddress);
//                    $('#lblProduct').text(productName);
//                    $('#lblDate').text(vData.Details.Rows[0].LastUpdated);

                }
            });
        }


        function DisplayDashboard(vRows) {
            var pLV = $('#tblDashboardBody');

            pLV.html('');

            for (var i = 0; i <= vRows.length - 1; i++) {
                pItem = $('<tr>')
                            .append($('<td>')
                                .text(vRows[i].Name)
                            )
                            .append($('<td>')
                                .text(vRows[i].Age)
                            )
                            .append($('<td>')
                                .text(vRows[i].Gender)
                            )
                            .append($('<td>')
                                .text(vRows[i].City)
                            )
                            .append($('<td>')
                                .append($('<a href="#dialog" id="' + vRows[i].ID + '">')
                                    .text('Show Details')
                                    .click(function () {
                                        DisplayDetails($(this).attr('id'));
                                    })
                                )
                            )
                pLV.append(pItem)
            }
            $('#tblDashboard').table('refresh');
        }


    });


