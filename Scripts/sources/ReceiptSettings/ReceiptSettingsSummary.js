require([
    'common'
    ], function () {

    $(function () {

        $(document).ready(function () {

            ShowReceiptSettingsInfo()

        });

        $('#btnReceiptSettingsSummaryBack').click(function (e) {
            e.preventDefault();

            window.open("../ReceiptSettings", "_self");
        });

        function ShowReceiptSettingsInfo() {

            ClearReceiptSettingFields();

            AjaxSend({
                url: "ReceiptSettings/getReceiptSettingsInfo",
                success: function (vData) {

                    if (vData.ReceiptSettingsData.Rows.length > 0) {

                        $("#txtID").val(vData.ReceiptSettingsData.Rows[0].ID || '');

                        $("#txtOrganizationID").val(vData.ReceiptSettingsData.Rows[0].OrganizationID || '');
                        $("#txtOrganization").val(vData.ReceiptSettingsData.Rows[0].CorporateName || '');
                        $("#txtOrganizationTradeNameID").val(vData.ReceiptSettingsData.Rows[0].OrgTradeNameID || '');
                        $("#txtOrganizationTradeName").val(vData.ReceiptSettingsData.Rows[0].BusinessTradeName || '');
                        $("#txtCorporateTIN").val(vData.ReceiptSettingsData.Rows[0].TIN || '');

                        getMessageList();

                    }
                }
            });

        };

        function ClearReceiptSettingFields() {

            $("#txtID").val('');

            $("#txtOrganizationID").val('');
            $("#txtOrganization").val('');
            $("#txtOrganizationTradeNameID").val('');
            $("#txtOrganizationTradeName").val('');
            $("#txtCorporateTIN").val('');

        }

        function getMessageList() {
            AjaxSend({
                url: 'ReceiptSettings/getMessageList',
                success: function (vData) {
                    LoadMessageList(vData.ReceiptSettingsMsgData.Rows);
                }
            });
        }

        function LoadMessageList(vRows) {
            var pLV = $('#lstMessages');
            var pItem;

            pLV.html('');
            for (var p = 0; p <= vRows.length - 1; p++) {
                pItem = $('<li>')
                        .append($('<h3>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].MessageFooter)
                        )
                        .append($('<p>')
                            .attr("style", "white-space:normal")
                            .text("Sort Order : " + vRows[p].SortOrder)
                        );
                pLV.append(pItem);
            };

            pLV.listview('refresh');
        };


    });
});


