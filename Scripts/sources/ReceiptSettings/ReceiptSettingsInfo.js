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

            var vDataMessBackUp, vDataPlaceBackUp;

            var libname, lbllibname, addlibname, recsetid;

            //getReceiptSetOrgTradeName();

            //$.mobile.activePage.bind('pageshow', PageShow);
            function PageShow() {
                getReceiptSetOrgTradeName();
                lbllibname = sessionStorage.LabelLibraryName;
                libname = sessionStorage.LibraryName;
                addlibname = sessionStorage.AddViewLibBCName;
                recsetid = sessionStorage.ViewID;
                ShowReceiptSettingsInfo(sessionStorage.ViewID);
            };
            if (!mLoaded) PageShow();

            //            document.getElementById('fileReceiptLogo').addEventListener('change', handleImageSelected, false);

            window.ReceiptSettings = {
                ShowReceiptInfo: function () {
                    AjaxSend({
                        url: 'ReceiptSettings/IsNewEntry',
                        success: function (vData) {
                            lbllibname = sessionStorage.LabelLibraryName;
                            libname = sessionStorage.LibraryName;
                            addlibname = sessionStorage.AddViewLibBCName;
                            recsetid = sessionStorage.ViewID;
                            getReceiptSetOrgTradeName();
                            ShowReceiptSettingsInfo(vData.ReceiptSettingsID);
                        }
                    });
                }
            }

            $('body').on('click', '#btnReceiptSetInfoAddMessage', function (e) {
                e.preventDefault();

                IsNewReceiptSetMessage = true
                intReceiptSetMessageCntr = intReceiptSetMessageCntr + 1;
                intReceiptSetMessageIndex = intReceiptSetMessageCntr;

                //$.mobile.changePage('#pageReceiptSettingsInfoMessages');
                $('#pageReceiptSettings').hide();
                $('#pageReceiptSettingsInfoMessages').show();
                ClearReceiptSetMessageFields();
            })
            .on('click', '#btnReceiptSettingsInfoMessageOk', function (e) {
                e.preventDefault();

                if ($.trim($("#txtReceiptSettingsInfoMessageFooter").val()) == "") {
                    NewMessageBox('Please enter message.', 'Message');
                    return;
                }

                SaveReceiptSetMessage();
            })
            .on('click', '#btnReceiptSettingsInfoMessageRemove', function (e) {
                e.preventDefault();
                NewMessageBox('Remove this message?', 'Message', [
                    {
                        Text: 'Yes', Click: function () {
                            RemoveReceiptSetMessage();
                        }
                    }, { Text: 'No' }
                ])
            })
            .on('click', '#btnReceiptSettingsInfoMessageCancel', function (e) {
                e.preventDefault();
                //$.mobile.changePage('#pageReceiptSettings');
                $('#pageReceiptSettings').show();
                $('#pageReceiptSettingsInfoMessages').hide();
            })
            .on('click', '#btnReceiptSettingsInfoDone', function (e) {
                e.preventDefault();
                if (CDec2(intReceiptSetMessageCntr) <= 0) {
                    NewMessageBox('Please provide at least 1 message', 'Message');
                    return;
                }

                if ($.trim($("#cmbReceiptSetInfoOrgTradeName").val()) == "") {
                    NewMessageBox('Please select business or trade name.', 'Business or Trade Name');
                    return;
                }

                if ($.trim($("#cmbReceiptSetInfoOrgPaperSize").val()) == "") {
                    NewMessageBox('Please select paper size.', 'Paper Size');
                    return;
                }

                SaveReceiptSettings();
            })
            .on('click', '#btnReceiptSettingsInfoCancel', function (e) {
                e.preventDefault();
                window.location.hash = 'LibraryPage';
            })

            function LoadReceiptLogo(imgObj) {

                var imgLogo = document.createElement("img");
                var canvas = document.getElementById('cnvReceiptlogo');
                var context = canvas.getContext('2d');

                imgLogo.src = imgObj;
                context.drawImage(imgLogo, 0, 0, 100, 100);
            }

            function handleImageSelected(evt) {

                var files = evt.target.files;

                for (var i = 0, f; f = files[i]; i++) {

                    if (!f.type.match('image.*')) {
                        continue;
                    }

                    var reader = new FileReader();

                    reader.onload = function (e) {
                        LoadReceiptLogo(e.target.result);
                    };
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
                            $("#cmbReceiptSetInfoOrgTradeName").val(vData.ReceiptSettingsData.Rows[0].OrgTradeNameID || '')//.selectmenu('refresh');
                            $("#cmbReceiptSetInfoOrgPaperSize").val(vData.ReceiptSettingsData.Rows[0].PaperSize || '')//.selectmenu('refresh');
                            $("#txtReceiptSetInfoTIN").val(vData.ReceiptSettingsData.Rows[0].TIN || '');

                            AjaxSend({
                                url: "ReceiptSettings/getMessageList",
                                data: pData,
                                success: function (vDataMessage) {
                                    vDataMessBackUp = vDataMessage.ReceiptSettingsData.Rows;
                                    LoadReceiptSetMessage(vDataMessage.ReceiptSettingsData.Rows);
                                }
                            });

                            AjaxSend({
                                url: "ReceiptSettings/getPlacesList",
                                data: pData,
                                success: function (vDataPlaces) {
                                    vDataPlaceBackUp = vDataPlaces.ReceiptSettingsData.Rows;
                                    LoadReceiptSetPlacesList(vDataPlaces.ReceiptSettingsData.Rows);

                                }
                            });

                            var datanew = vData.ReceiptSettingsData.Rows[0];
                            var load = setInterval(function () {
                                if ($("#txtReceiptSetInfoOrganization").val() == "") {
                                    $("#txtReceiptSetInfoID").val(vData.ReceiptSettingsData.Rows[0].ID || '');

                                    $("#txtReceiptSetInfoOrganization").val(vData.ReceiptSettingsData.Rows[0].CompanyName || '');
                                    $("#cmbReceiptSetInfoOrgTradeName").val(vData.ReceiptSettingsData.Rows[0].OrgTradeNameID || '')//.selectmenu('refresh');
                                    $("#cmbReceiptSetInfoOrgPaperSize").val(vData.ReceiptSettingsData.Rows[0].PaperSize || '')//.selectmenu('refresh');
                                    $("#txtReceiptSetInfoTIN").val(vData.ReceiptSettingsData.Rows[0].TIN || '');
                                    LoadReceiptSetMessage(vDataMessBackUp);
                                    LoadReceiptSetPlacesList(vDataPlaceBackUp)
                                }
                                else {
                                    clearInterval(load);
                                }
                            }, 1500)

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

                if (vRows.length != 0) {
                    LoadReceiptSetMessageList();
                }
            };

            function ShowReceiptSetMessageInfo(arData, index) {

                //$.mobile.changePage('#pageReceiptSettingsInfoMessages');
                $('#pageReceiptSettings').hide();
                $('#pageReceiptSettingsInfoMessages').show();

                ClearReceiptSetMessageFields();

                intReceiptSetMessageIndex = index;

                $("#txtReceiptSettingsInfoMessageID").val(arData[intReceiptSetMessageRecID] || '');
                $("#txtReceiptSettingsInfoMessageFooter").val(arData[intReceiptSetMessageFooter] || '');
                $("#txtReceiptSettingsInfoSortOrder").val(arData[intReceiptSetSortOrder] || '');

                IsNewReceiptSetMessage = false;

                $("#btnReceiptSettingsInfoMessageRemove").show();

            };

            function ClearReceiptSettingsFields() {

                // dont clear
                // sessionStorage.clear();
                //proj.ClearSessionStorage();
                sessionStorage["LibraryName"] = libname;
                sessionStorage["LabelLibraryName"] = lbllibname;
                sessionStorage["AddViewLibBCName"] = addlibname;
                sessionStorage["ViewID"] = recsetid;

                $("#txtReceiptSetInfoID").val('');

                $("#txtReceiptSetInfoOrganization").val('');
                $("#cmbReceiptSetInfoOrgTradeName").val('')//.selectmenu('refresh');
                $("#cmbReceiptSetInfoOrgPaperSize").val('')//.selectmenu('refresh');
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

                if (sessionStorage.AddViewLibBCName == "Add Receipt Template") {
                    AjaxSend({
                        url: "ReceiptSettings/CheckReceiptSettings",
                        data: pData,
                        success: function (result) {
                            if (result.CheckReceiptSettingsData.Rows.length >= 1) {
                                NewMessageBox('Selected Trade Name and Paper Size is already exist.', 'Receipt Settings');
                            }
                            else {
                                AjaxSend({
                                    url: "ReceiptSettings/SaveReceiptInfoName",
                                    data: pData,
                                    success: function (vData) {

                                        $("#txtReceiptSetInfoID").val(vData.ReceiptSettingsData || '');

                                        //window.location = GetPathFormat("/ReceiptSettings?OrgID=" + sessionStorage.OrgID);
                                        $('tblBody').html('');
                                        window.location.hash = '';
                                        window.location.hash = 'LibraryPage';
                                        LibraryTable.ReceiptSettings();
                                    }
                                });
                            }
                        }
                    })
                }
                else {
                    AjaxSend({
                        url: "ReceiptSettings/SaveReceiptInfoName",
                        data: pData,
                        success: function (vData) {

                            $("#txtReceiptSetInfoID").val(vData.ReceiptSettingsData || '');

                            //window.location = GetPathFormat("/ReceiptSettings?OrgID=" + sessionStorage.OrgID);
                            $('tblBody').html('');
                            window.location.hash = '';
                            window.location.hash = 'LibraryPage';
                            LibraryTable.ReceiptSettings();
                        }
                    });
                }
            };

            function SaveReceiptSetMessage() {

                arReceiptSetMessage = new Array();
                arReceiptSetMessage[intReceiptSetMessageRecID] = $("#txtReceiptSettingsInfoMessageID").val();
                arReceiptSetMessage[intReceiptSetMessageFooter] = $("#txtReceiptSettingsInfoMessageFooter").val();
                arReceiptSetMessage[intReceiptSetSortOrder] = $("#txtReceiptSettingsInfoSortOrder").val();
                arReceiptSetMessage[intReceiptSetMessageState] = (IsNewReceiptSetMessage ? "New" : "Edit");

                sessionStorage["ReceiptSetMessage-" + intReceiptSetMessageIndex] = JSON.stringify(arReceiptSetMessage);
                //$.mobile.changePage('#pageReceiptSettings');
                $('#pageReceiptSettings').show();
                $('#pageReceiptSettingsInfoMessages').hide();
                LoadReceiptSetMessageList();
            };

            function RemoveReceiptSetMessage() {

                arReceiptSetMessage = new Array();
                arReceiptSetMessage[intReceiptSetMessageRecID] = $("#txtReceiptSettingsInfoMessageID").val();
                arReceiptSetMessage[intReceiptSetMessageFooter] = $("#txtReceiptSettingsInfoMessageFooter").val();
                arReceiptSetMessage[intReceiptSetSortOrder] = $("#txtReceiptSettingsInfoSortOrder").val();
                arReceiptSetMessage[intReceiptSetMessageState] = "Remove";

                sessionStorage["ReceiptSetMessage-" + intReceiptSetMessageIndex] = JSON.stringify(arReceiptSetMessage);

                //$.mobile.changePage('#pageReceiptSettings');
                //history.back();
                $('#pageReceiptSettings').show();
                $('#pageReceiptSettingsInfoMessages').hide();
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

                            pItem = $('<a href="#" id="lst' + i + '" class="list-group-item list-group-item-action">')
                                    .append($('<div>')
                                    .attr('ID', i)
                                    .data('Data', arReceiptSetMessage)
                                    .click(function (e) {
                                        e.preventDefault();
                                        ShowReceiptSetMessageInfo($(this).data('Data'), $(this).attr('ID'));
                                    })
                                    .append($('<div>')
                                        .attr("style", "white-space:normal")
                                        .text(arReceiptSetMessage[intReceiptSetMessageFooter])
                                    )
                            );


                            pLV.append(pItem);

                        }

                    }
                }

                //pLV.listview('refresh');
            };

            function LoadReceiptSetPlacesList(vRows) {
                var pLV = $('#fldsetPlaces');
                var pItem;

                intReceiptSetPlacesCntr = vRows.length - 1;

                pLV.html('');
                for (var i = 0; i <= vRows.length - 1; i++) {
                    pItem = $('<div class="form-check mb-3" style="font-size:20px;">')
                                .append($('<input class="form-check-input" type="checkbox">')
                                    .css({
                                        height: '30px',
                                        width: '30px'
                                    })
                                    .attr('id', 'chkPlaces-' + i)
                                    .attr('checked', ((vRows[i].ID || '') == '' ? false : true))
                                    .data('Data', vRows[i])
                                ).append($('<label>')
                                    .css('margin-left', '10px')
                                    .attr('for', 'chkPlaces-' + i)
                                    .text((vRows[i].BranchName || 'No Branch Name'))
                                )

                    pLV.append(pItem);
                }

                //$('#divReceiptSettingsPlaces').trigger("create");
            };
        });
    });