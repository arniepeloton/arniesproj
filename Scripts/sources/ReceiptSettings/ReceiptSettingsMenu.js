require([
    'common'
    ], function () {

        $(function () {

            $(document).ready(function () {

                ShowReceiptSettings();

            });

            //Navigation Menu
            $('#btnHome').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu"), "_self");
            });

            $('#btnSystemAdmin').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu#pagePopUpMenuSettings"), "_self");
            });

            $('#btnSystemSettings').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu#pagePopSystemSettings"), "_self");
            });
            //End Navigation Menu

            $('#frmReceiptSettingsBrowser').submit(function (e) {
                e.preventDefault();

                ShowReceiptSettings();

            });

            $('#btnAddNewReceiptSettings').click(function () {

                CreateNewReceiptSetEntry(1, '');

            });

            $('#btnDoneReceiptSettings').click(function () {

                window.open(GetPathFormat("/People/PeopleMenu"), "_self");

            });

            $('#btnReceiptSettingsGoToSetupChecklist').click(function () {

                window.open(GetPathFormat("/SetupChecklist"), "_self");

            });

            function ShowReceiptSettings() {

                var pData = {};
                pData.SearchString = $("#txtReceiptSearch").val();

                AjaxSend({
                    url: 'ReceiptSettings/getReceiptSettingsList',
                    data: pData,
                    success: function (vData) {
                        LoadReceiptSettingsList(vData.ReceiptSettingsData.Rows);
                    }
                });

            }

            function LoadReceiptSettingsList(vRows) {
                var pLV = $('#lstReceiptSettings');
                var pItem;

                $("#divReceiptSearchResults").hide();
                $("#divReceiptSearchNoResults").hide();
                $("#btnAddNewReceiptSettings").hide();

                if (vRows.length > 0) {
                    $("#divReceiptSearchResults").show();
                    $("#divReceiptRecordCount").text(" Number of record(s) found: " + vRows.length);

                    pLV.html('');
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        pItem = $('<li>')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();

                                CreateNewReceiptSetEntry(0, $(this).attr('ID'));

                            })
                            .append($('<h3>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].CompanyName || '')
                            )
                            .append($('<p>')
                                .attr("style", "white-space:normal")
                                .text('Trade Name : ' + vRows[p].BusinessTradeName || '')
                            )
                            .append($('<p>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].BusinessTradeName)
                            )
                            .append($('<p>')
                                .attr("style", "white-space:normal")
                                .text('Paper Size : ' + vRows[p].PaperSize)
                            )
                        );
                        pLV.append(pItem);
                    };

                    pLV.listview('refresh');
                }
                else {
                    $("#divReceiptSearchNoResults").show();
                    $("#btnAddNewReceiptSettings").show();
                    $("#lblReceiptNotFound").text('The receipt template "' + $("#txtReceiptSearch").val() + '" does not exist. Please consider to search thoroughly using an alternate spelling before creating a new receipt template to avoid duplication.');
                }
            };

            function CreateNewReceiptSetEntry(IsNew, strID) {
                var pData = {};
                pData.IsNew = IsNew;
                pData.ID = strID;

                AjaxSend({
                    url: 'ReceiptSettings/setCreateNew',
                    data: pData,
                    success: function (vData) {

                        window.location = GetPathFormat("ReceiptSettings/ReceiptSettingsInfo");
                    }
                });


            }





        });
    });
