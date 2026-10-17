
require([
    'common',
    GetPathFormat('/Scripts3/Browser/LibraryCities.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryCountries.js?crc=')
    ], function() {

    $(function () {

        $(document).ready(function () {
            getPeopleAddressList();
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

        $('#btnPeopleAddressPopUpMenu').click(function (e) {
            e.preventDefault();

            $("#divPeopleAddressPopUpMenu").popup("open")

        });

        $('#popPeopleAddressPeopleInfoName').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoName')), "_self");

        });

        $('#popPeopleAddressPeopleInfoAlias').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoAlias')), "_self");

        });

        $('#popPeopleAddressPeoplePersonalInfo').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeoplePersonalInfo')), "_self");

        });

        $('#popPeopleAddressPeopleContact').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleContactNo')), "_self");

        });

        $('#popPeopleAddressPeopleEducation').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleEducation')), "_self");

        });

        $('#popPeopleAddressPeopleEmpHistory').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleEmpHistory')), "_self");

        });

        $('#popPeopleAddressPeopleRelatedPerson').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleRelatedPerson')), "_self");

        });

        $('#popPeopleAddressPeoplePrimeIDReference').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeoplePrimeIDReference')), "_self");

        });

        $('#popPeopleAddressPeopleUserAccount').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleUserAccount')), "_self");

        });

        $('#btnCity').click(function () {

            Browser.LibraryCities.Click = function (e) {

                $('#txtCityID').val(e.ID)
                $('#btnCity').text(e.Name)
                    .button('refresh');

            };
            Browser.LibraryCities.Show();

        });

        $('#btnCountry').click(function () {

            Browser.LibraryCountries.Click = function (e) {

                $('#txtCountryID').val(e.ID)
                $('#btnCountry').text(e.Name)
                    .button('refresh');

            };
            Browser.LibraryCountries.Show();

        });

        $('#btnPeopleAddressAdd').click(function (e) {
            e.preventDefault();

            $.mobile.changePage('#pagePeopleAddressName');
            ClearPeopleAddressFields();

        });

        $('#btnPeopleAddressSave').click(function (e) {
            e.preventDefault();

            if ($.trim($("#txtStreet").val()) == "") {
                MessageBox("Please enter street.",
                            "Street",
                            [
                                { Text: 'Ok' }
                            ]
                );

                return;
            }

            if ($.trim($("#txtCityID").val()) == "") {
                MessageBox("Please select city.",
                            "City",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }

            if ($.trim($("#txtCountryID").val()) == "") {
                MessageBox("Please select country.",
                            "Country",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }

            if ($.trim($("#selTypeOfAddress").val()) == "") {
                MessageBox("Please select type of address.",
                            "Type Of Address",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }

            SavePersonAddressInfo();

        });

        $('#btnPeopleAddressRemove').click(function (e) {
            e.preventDefault();

            MessageBox("Remove this record?",
                'Remove',
                [
                    { Text: 'Yes',
                        Click: function () {
                            RemovePersonAddressInfo();
                        }
                    }
                ,
                    { Text: 'No'
                    }
                ]
            );

        });

        $('#btnPeopleAddressCancel').click(function (e) {
            e.preventDefault();

            $.mobile.changePage('#pagePeopleAddressList');

        });

        $('#btnPeopleAddressDone').click(function (e) {
            e.preventDefault();

            window.open(MergeUrlParameters(GetUrlParameters(), GetPathFormat('PeopleInfoSummary')), "_self");

        });

        function getPeopleAddressList() {
            AjaxSend({
                url: 'People/getPeopleAddressList',
                success: function (vData) {
                    LoadPeopleAddressList(vData.PeopleAddressList.Rows);
                }
            });
        }

        function LoadPeopleAddressList(vRows) {
            var pLV = $('#lstPeopleAddress');
            var pItem;

            $("#divRecordCount").text(" Number of record(s) found: " + vRows.length);

            pLV.html('');

            pItem = $('<li>')
                    .attr('data-role', "list-divider")
                    .text('Address List');
            pLV.append(pItem);

            for (var p = 0; p <= vRows.length - 1; p++) {
                pItem = $('<li>')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();
                                $.mobile.changePage('#pagePeopleAddressName');
                                ShowPersonAddressInfo($(this).attr('ID'));
                            })
                            .append($('<h3>')
                                .attr("style", "white-space:normal")
                                .text((vRows[p].HouseBuildingNo == "" ? "" : vRows[p].HouseBuildingNo + ", ") + vRows[p].Street)
                            )
                            .append($('<p>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].City + ', ' + vRows[p].Country)
                            )
                            .append($('<p>')
                                .attr("style", "white-space:normal")
                                .text('(' + vRows[p].AddressType + ')')
                            )
                        );
                pLV.append(pItem);
            };

            pLV.listview('refresh');
        };

        function ShowPersonAddressInfo(strID) {
            var pData = {};
            pData.ID = strID;

            ClearPeopleAddressFields();

            AjaxSend({
                url: "People/getPeopleAddressInfo",
                data: pData,
                success: function (vData) {

                    if (vData.PeopleAddressInfo.Rows.length > 0) {

                        $("#txtAddressID").val(vData.PeopleAddressInfo.Rows[0].ID || '');

                        $("#txtHouseBuildingNo").val(vData.PeopleAddressInfo.Rows[0].HouseBuildingNo || '');
                        $("#txtStreet").val(vData.PeopleAddressInfo.Rows[0].Street || '');
                        $("#txtCityID").val(vData.PeopleAddressInfo.Rows[0].CityID || '');
                        $("#btnCity").text(vData.PeopleAddressInfo.Rows[0].City || '');
                        $("#btnCity").button('refresh');
                        $("#txtCountryID").val(vData.PeopleAddressInfo.Rows[0].CountryID || '');
                        $("#btnCountry").text(vData.PeopleAddressInfo.Rows[0].Country || '');
                        $("#btnCountry").button('refresh');
                        $("#selTypeOfAddress").val(vData.PeopleAddressInfo.Rows[0].AddressType || '');
                        $('#selTypeOfAddress').selectmenu('refresh');

                        $("#btnPeopleAddressRemove").show();
                    }
                }
            });

        };

        function ClearPeopleAddressFields() {
            $("#txtAddressID").val('');

            $("#txtHouseBuildingNo").val('');
            $("#txtStreet").val('');
            $("#txtCityID").val('');
            $("#btnCity").text('Please Select');
            $("#btnCity").button('refresh');
            $("#txtCountryID").val('');
            $("#btnCountry").text('Please Select');
            $("#btnCountry").button('refresh');
            $("#selTypeOfAddress").val('');
            $('#selTypeOfAddress').selectmenu('refresh');

            $("#btnPeopleAddressRemove").hide();

        }

        function SavePersonAddressInfo() {

            var pData = {};
            pData.AddressID = $("#txtAddressID").val();

            pData.AddressHouseBuildingNo = $("#txtHouseBuildingNo").val();
            pData.AddressStreet = $("#txtStreet").val();
            pData.AddressCityID = $("#txtCityID").val();
            pData.AddressCountryID = $("#txtCountryID").val();
            pData.AddressType = $("#selTypeOfAddress").val();

            AjaxSend({
                url: "People/SavePeopleAddressInfo",
                data: pData,
                success: function (vData) {

                    $.mobile.changePage('#pagePeopleAddressList');
                    getPeopleAddressList();

                }
            });

        };

        function RemovePersonAddressInfo() {

            var pData = {};
            pData.AddressID = $("#txtAddressID").val();

            AjaxSend({
                url: "People/RemovePeopleAddress",
                data: pData,
                success: function (vData) {

                    $.mobile.changePage('#pagePeopleAddressList');
                    getPeopleAddressList();

                }
            });

        };
    });
});