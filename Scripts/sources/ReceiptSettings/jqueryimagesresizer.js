require([
    'common'
    ], function () {

        $(function () {

            var IsNewReceiptSetMessage = true;
            var arReceiptSetMessage = new Array();
            var intReceiptSetMessageCntr = 0;
            var intReceiptSetMessageIndex = 0;

            var intReceiptSetMessageRecID = 0;
            var intReceiptSetMessageFooter = 1;
            var intReceiptSetSortOrder = 2;
            var intReceiptSetMessageState = 3;

            var intReceiptSetPlacesCntr = -1;

            var mLoaded = false;

            getReceiptSetOrgTradeName();

            $.mobile.activePage.bind('pageshow', PageShow);
            function PageShow() {
                mLoaded = true;

                if (!window.IsPageLoaded) {

                    localStorage.removeItem('imgReceiptSetLogo');

                    AjaxSend({
                        url: 'ReceiptSettings/IsNewEntry',
                        success: function (vData) {

                            ShowReceiptSettingsInfo(vData.ReceiptSettingsID || '')
                        }
                    });

                    window.IsPageLoaded = true;
                }
            };
            if (!mLoaded) PageShow();

            document.getElementById('fileReceiptLogo').addEventListener('change', handleImageSelected, false);

            $('#btnReceiptSetInfoAddMessage').click(function (e) {
                e.preventDefault();

                IsNewReceiptSetMessage = true
                intReceiptSetMessageCntr = intReceiptSetMessageCntr + 1;
                intReceiptSetMessageIndex = intReceiptSetMessageCntr;

                $.mobile.changePage('#pageReceiptSettingsInfoMessages');
                ClearReceiptSetMessageFields();

            });

            $('#btnReceiptSettingsInfoMessageOk').click(function (e) {
                e.preventDefault();

                if ($.trim($("#txtReceiptSettingsInfoMessageFooter").val()) == "") {
                    MessageBox("Please enter message.",
                           "Message",
                            [
                                { Text: 'Ok'
                                }
                            ]
                    );
                    return;
                }

                SaveReceiptSetMessage();

            });

            $('#btnReceiptSettingsInfoMessageRemove').click(function (e) {
                e.preventDefault();

                MessageBox("Remove this record?",
                    'Remove',
                    [
                        { Text: 'Yes',
                            Click: function () {
                                RemoveReceiptSetMessage();
                            }
                        }
                    ,
                        { Text: 'No'
                        }
                    ]
                );

            });

            $('#btnReceiptSettingsInfoMessageCancel').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pageReceiptSettings');

            });

            $('#btnReceiptSettingsInfoDone').click(function (e) {
                e.preventDefault();

                if ($.trim($("#cmbReceiptSetInfoOrgTradeName").val()) == "") {
                    MessageBox("Please select business or trade name.",
                            "Business or Trade Name",
                            [
                                { Text: 'Ok' }
                            ]
                    );
                    return;
                }

                if ($.trim($("#cmbReceiptSetInfoOrgPaperSize").val()) == "") {
                    MessageBox("Please select paper size.",
                            "Paper Size",
                            [
                                { Text: 'Ok' }
                            ]
                    );
                    return;
                }

                SaveReceiptSettings();

            });

            function LoadReceiptLogo() {

                if (localStorage.imgReceiptSetLogo) {

                    var span = document.createElement('span');
                    span.innerHTML += ['<img class="thumb" src="', localStorage.imgReceiptSetLogo,
                            '" title="test"/>'].join('');

                    $("#divReceiptLogoList").html('');
                    document.getElementById('divReceiptLogoList').insertBefore(span, null);
                }
            }

            function handleImageSelected(evt) {

                var files = evt.target.files;

                localStorage.removeItem('imgReceiptSetLogo');
                for (var i = 0, f; f = files[i]; i++) {

                    if (!f.type.match('image.*')) {
                        continue;
                    }

                    var reader = new FileReader();

                    reader.onload = (function (theFile) {
                        return function (e) {
                            localStorage.setItem('imgReceiptSetLogo', e.target.result);
                            LoadReceiptLogo();
                        };
                    })(f);

                    reader.readAsDataURL(f);
                }
            }

            function getReceiptSetOrgTradeName() {

                AjaxSend({
                    url: "ReceiptSettings/setSubscriberOrganization",
                    success: function (vData) {

                        AjaxSend({
                            url: 'Organization/getOrgTradeNameList',
                            success: function (vData) {

                                var vRows = vData.OrganizationData.Rows;

                                $('#cmbReceiptSetInfoOrgTradeName').empty();

                                $('#cmbReceiptSetInfoOrgTradeName').append(new Option('Please Select', ''));
                                for (var p = 0; p <= vRows.length - 1; p++) {
                                    $('#cmbReceiptSetInfoOrgTradeName').append(new Option(vRows[p].BusinessTradeName, vRows[p].ID, false, false));
                                }
                            },
                            error: function (vError) {
                                var pDummy;
                            }
                        });

                    }
                });
            }

            function ShowReceiptSettingsInfo(strID) {
                var pData = {};
                pData.OrganizationID = strID;

                ClearReceiptSettingsFields();

                AjaxSend({
                    url: "ReceiptSettings/getReceiptSettingsInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.ReceiptSettingsData.Rows.length > 0) {

                            $("#txtReceiptSetInfoID").val(vData.ReceiptSettingsData.Rows[0].ID || '');

                            $("#txtReceiptSetInfoOrganization").val(vData.ReceiptSettingsData.Rows[0].CompanyName || '');
                            $("#cmbReceiptSetInfoOrgTradeName").val(vData.ReceiptSettingsData.Rows[0].OrgTradeNameID || '').selectmenu('refresh');
                            $("#cmbReceiptSetInfoOrgPaperSize").val(vData.ReceiptSettingsData.Rows[0].PaperSize || '').selectmenu('refresh');
                            $("#txtReceiptSetInfoTIN").val(vData.ReceiptSettingsData.Rows[0].TIN || '');

                            AjaxSend({
                                url: "ReceiptSettings/getMessageList",
                                data: pData,
                                success: function (vDataMessage) {

                                    LoadReceiptSetMessage(vDataMessage.ReceiptSettingsData.Rows);

                                }
                            });

                            AjaxSend({
                                url: "ReceiptSettings/getPlacesList",
                                data: pData,
                                success: function (vDataPlaces) {

                                    LoadReceiptSetPlacesList(vDataPlaces.ReceiptSettingsData.Rows);

                                }
                            });

                            LoadReceiptLogo();

                        }
                    }
                });

            };

            function LoadReceiptSetMessage(vRows) {

                for (var p = 0; p <= vRows.length - 1; p++) {

                    arReceiptSetMessage = new Array();
                    arReceiptSetMessage[intReceiptSetMessageRecID] = vRows[p].ID;
                    arReceiptSetMessage[intReceiptSetMessageFooter] = vRows[p].MessageFooter;
                    arReceiptSetMessage[intReceiptSetSortOrder] = vRows[p].SortOrder;
                    arReceiptSetMessage[intReceiptSetMessageState] = "";

                    sessionStorage["ReceiptSetMessage-" + p] = JSON.stringify(arReceiptSetMessage);

                };

                intReceiptSetMessageCntr = vRows.length;
                LoadReceiptSetMessageList();

            };

            function ShowReceiptSetMessageInfo(arData, index) {

                $.mobile.changePage('#pageReceiptSettingsInfoMessages');

                ClearReceiptSetMessageFields();

                intReceiptSetMessageIndex = index;

                $("#txtReceiptSettingsInfoMessageID").val(arData[intReceiptSetMessageRecID] || '');
                $("#txtReceiptSettingsInfoMessageFooter").val(arData[intReceiptSetMessageFooter] || '');
                $("#txtReceiptSettingsInfoSortOrder").val(arData[intReceiptSetSortOrder] || '');

                IsNewReceiptSetMessage = false;

                $("#btnReceiptSettingsInfoMessageRemove").show();

            };

            function ClearReceiptSettingsFields() {

                sessionStorage.clear();

                $("#txtReceiptSetInfoID").val('');

                $("#txtReceiptSetInfoOrganization").val('');
                $("#cmbReceiptSetInfoOrgTradeName").val('').selectmenu('refresh');
                $("#cmbReceiptSetInfoOrgPaperSize").val('').selectmenu('refresh');
                $("#txtReceiptSetInfoTIN").val('');

                LoadReceiptSetMessageList();

            }

            function ClearReceiptSetMessageFields() {
                $("#txtReceiptSettingsInfoMessageID").val('');
                $("#txtReceiptSettingsInfoMessageFooter").val('');
                $("#txtReceiptSettingsInfoSortOrder").val('');

                $("#btnReceiptSettingsInfoMessageRemove").hide();

            }

            function SaveReceiptSettings() {
                var pData = [];
                var pDataMessage = [];
                var pDataPlaces = [];
                var intIndex;

                //Messages
                intIndex = 0;
                for (var i = 0; i <= intReceiptSetMessageCntr; i++) {

                    if (sessionStorage["ReceiptSetMessage-" + i] != undefined) {

                        arReceiptSetMessage = new Array();
                        arReceiptSetMessage = JSON.parse(sessionStorage["ReceiptSetMessage-" + i]);

                        pDataMessage[intIndex] = {
                            ID: arReceiptSetMessage[intReceiptSetMessageRecID],
                            MessageFooter: arReceiptSetMessage[intReceiptSetMessageFooter],
                            SortOrder: arReceiptSetMessage[intReceiptSetSortOrder],
                            RecState: arReceiptSetMessage[intReceiptSetMessageState]
                        };

                        intIndex = intIndex + 1;

                    }
                }

                //Places
                intIndex = 0;
                for (var i = 0; i <= intReceiptSetPlacesCntr; i++) {

                    var pPlacesData = $("#chkPlaces-" + i).data('Data');

                    pDataPlaces[intIndex] = {
                        ID: pPlacesData.ID,
                        OrganizationBranchID: pPlacesData.OrganizationBranchID,
                        RecState: ($("#chkPlaces-" + i).is(":checked") ? "New" : "Remove")
                    };

                    intIndex = intIndex + 1;
                }

                pData = {
                    ID: $.trim($("#txtReceiptSetInfoID").val()),
                    Organization: $.trim($("#txtOrgCorporateName").val()),
                    OrganizationTradeNameID: $.trim($("#cmbReceiptSetInfoOrgTradeName").val()),
                    PaperSize: $.trim($("#cmbReceiptSetInfoOrgPaperSize").val()),

                    MessageList: pDataMessage,
                    PlacesList: pDataPlaces
                };

                AjaxSend({
                    url: "ReceiptSettings/SaveReceiptInfoName",
                    data: pData,
                    success: function (vData) {

                        $("#txtReceiptSetInfoID").val(vData.ReceiptSettingsData || '');

                        window.open("../ReceiptSettings", "_self");

                    }
                });

            };

            function SaveReceiptSetMessage() {

                arReceiptSetMessage = new Array();
                arReceiptSetMessage[intReceiptSetMessageRecID] = $("#txtReceiptSettingsInfoMessageID").val();
                arReceiptSetMessage[intReceiptSetMessageFooter] = $("#txtReceiptSettingsInfoMessageFooter").val();
                arReceiptSetMessage[intReceiptSetSortOrder] = $("#txtReceiptSettingsInfoSortOrder").val();
                arReceiptSetMessage[intReceiptSetMessageState] = (IsNewReceiptSetMessage ? "New" : "Edit");

                sessionStorage["ReceiptSetMessage-" + intReceiptSetMessageIndex] = JSON.stringify(arReceiptSetMessage);

                $.mobile.changePage('#pageReceiptSettings');
                LoadReceiptSetMessageList();
            };

            function RemoveReceiptSetMessage() {

                arReceiptSetMessage = new Array();
                arReceiptSetMessage[intReceiptSetMessageRecID] = $("#txtReceiptSettingsInfoMessageID").val();
                arReceiptSetMessage[intReceiptSetMessageFooter] = $("#txtReceiptSettingsInfoMessageFooter").val();
                arReceiptSetMessage[intReceiptSetSortOrder] = $("#txtReceiptSettingsInfoSortOrder").val();
                arReceiptSetMessage[intReceiptSetMessageState] = "Remove";

                sessionStorage["ReceiptSetMessage-" + intReceiptSetMessageIndex] = JSON.stringify(arReceiptSetMessage);

                $.mobile.changePage('#pageReceiptSettings');
                LoadReceiptSetMessageList();
            };

            function LoadReceiptSetMessageList() {
                var pLV = $('#lstReceiptSetInfoMessages');
                var pItem;

                pLV.html('');
                for (var i = 0; i <= intReceiptSetMessageCntr; i++) {

                    if (sessionStorage["ReceiptSetMessage-" + i] != undefined) {

                        arReceiptSetMessage = new Array();
                        arReceiptSetMessage = JSON.parse(sessionStorage["ReceiptSetMessage-" + i]);

                        if (arReceiptSetMessage[intReceiptSetMessageState] != "Remove") {

                            pItem = $('<li id="lst' + i + '">')
                                    .append($('<a>')
                                    .attr('ID', i)
                                    .data('Data', arReceiptSetMessage)
                                    .click(function (e) {
                                        e.preventDefault();
                                        ShowReceiptSetMessageInfo($(this).data('Data'), $(this).attr('ID'));
                                    })
                                    .append($('<h3>')
                                        .attr("style", "white-space:normal")
                                        .text(arReceiptSetMessage[intReceiptSetMessageFooter])
                                    )
                            );


                            pLV.append(pItem);

                        }

                    }
                }

                pLV.listview('refresh');
            };

            function LoadReceiptSetPlacesList(vRows) {
                var pLV = $('#fldsetPlaces');
                var pItem;

                intReceiptSetPlacesCntr = vRows.length - 1;

                pLV.html('');
                for (var i = 0; i <= vRows.length - 1; i++) {

                    pItem = $('<input>')
                                .attr("type", 'checkbox')
                                .attr('id', 'chkPlaces-' + i)
                                .attr('checked', ((vRows[i].ID || '') == '' ? false : true))
                                .data('Data', vRows[i])
                            .append($('<label>')
                                .attr('for', 'chkPlaces-' + i)
                                .text((vRows[i].BranchName || 'No Branch Name'))
                            );

                    pLV.append(pItem);
                }

                $('#divReceiptSettingsPlaces').trigger("create");
            };










        });
    });