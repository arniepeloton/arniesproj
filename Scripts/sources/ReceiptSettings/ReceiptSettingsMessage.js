require([
    'common'
    ], function () {

    $(function () {

        $(document).ready(function () {

            $("#btnAddMessage > span").attr("style", "text-align:left;white-space:normal");

            getMessageList();
        });

        $('#btnAddMessage').click(function (e) {
            e.preventDefault();

            $.mobile.changePage('#pageMessages');
            ClearMessageFields();

        });

        $("#txtSortOrder").keydown(function (e) {
            if (!NumberOnly(e)) {
                return;
            }
        });

        $('#btnMessageSave').click(function (e) {
            e.preventDefault();

            if ($.trim($("#txtMessageFooter").val()) == "") {
                MessageBox("Please enter message.",
                           "Message",
                            [
                                { Text: 'Ok'
                                }
                            ]
                );
                return;
            }

            if ($.trim($("#txtSortOrder").val()) == "") {
                MessageBox("Please enter sort order.",
                           "Sort Order",
                            [
                                { Text: 'Ok'
                                }
                            ]
                );
                return;
            }

            SaveMessage();

        });

        $('#btnMessageRemove').click(function (e) {
            e.preventDefault();

            MessageBox("Remove this record?",
                'Remove',
                [
                    { Text: 'Yes',
                        Click: function () {
                            RemoveMessage();
                        }
                    }
                ,
                    { Text: 'No'
                    }
                ]
            );

        });


        $('#btnMessageCancel').click(function (e) {
            e.preventDefault();

            $.mobile.changePage('#pageReceiptSettingsMessage');

        });

        $('#btnReceiptMsgSettingsBack').click(function (e) {
            e.preventDefault();

            if ($.trim($("#txtID").val()) == "") {
                window.open("ReceiptSettingsInfo", "_self");
            }
            else {
                window.open("ReceiptSettingsSummary", "_self");
            }

        });

        $('#btnReceiptMsgSettingsDone').click(function (e) {
            e.preventDefault();

            window.open("ReceiptSettingsSummary", "_self");

        });

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
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();
                                $.mobile.changePage('#pageMessages');
                                ShowMessageInfo($(this).attr('ID'));
                            })
                            .append($('<h3>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].MessageFooter)
                            )
                            .append($('<p>')
                                .attr("style", "white-space:normal")
                                .text("Sort Order : " + vRows[p].SortOrder)
                            )
                        );
                pLV.append(pItem);
            };

            pLV.listview('refresh');
        };

        function ShowMessageInfo(strID) {
            var pData = {};
            pData.ID = strID;

            ClearMessageFields();

            AjaxSend({
                url: "ReceiptSettings/getMessageInfo",
                data: pData,
                success: function (vData) {

                    if (vData.ReceiptSettingsMsgData.Rows.length > 0) {

                        $("#txtMessageID").val(vData.ReceiptSettingsMsgData.Rows[0].ID || '');

                        $("#txtMessageFooter").val(vData.ReceiptSettingsMsgData.Rows[0].MessageFooter || '');
                        $("#txtSortOrder").val(vData.ReceiptSettingsMsgData.Rows[0].SortOrder || '0');

                        $("#btnGoTo").show();

                    }
                }
            });

        };

        function ClearMessageFields() {

            $("#txtMessageID").val('');

            $("#txtMessageFooter").val('');
            $("#txtSortOrder").val('0');

            $("#btnGoTo").hide();
        }

        function SaveMessage() {
            var pData = [];

            pData = {
                MessageID: $.trim($("#txtMessageID").val()),
                Message: $.trim($("#txtMessageFooter").val()),
                MessageSortOrder: $.trim($("#txtSortOrder").val())
            };


            AjaxSend({
                url: "ReceiptSettings/SaveMessageInfo",
                data: pData,
                success: function (vSaveData) {

                    if (vSaveData.ReceiptSettingsMsgData == "Invalid Sort Order") {
                        MessageBox("Please enter valid sort order.",
                                    "Sort Order",
                                    [
                                        { Text: 'Ok' }
                                    ]
                                    );
                    }
                    else {

                        $("#txtMessageID").val(vSaveData.ReceiptSettingsMsgData || '');

                        $.mobile.changePage('#pageReceiptSettingsMessage');
                        getMessageList();

                    }

                }
            });

        };

        function RemoveMessage() {

            var pData = {};
            pData.MessageID = $("#txtMessageID").val();

            AjaxSend({
                url: "ReceiptSettings/RemoveMessage",
                data: pData,
                success: function (vData) {

                    $.mobile.changePage('#pageReceiptSettingsMessage');
                    getMessageList();

                }
            });

        };


    });
});