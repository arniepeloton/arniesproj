require([
    'common'
    ], function () {
        var isAddNewFowst = true;
        $(function () {

            setMonthCollection("selDevFOWSTBegBalDateMonth");
            setYearCollection("selDevFOWSTBegBalDateYear");

            setMonthCollection("selDevFowstBIRApplicationDateMonth");
            setYearCollection("selDevFowstBIRApplicationBalDateYear");

            $(document).ready(function () {

                $("#btnAddDeviceFOWST > span").attr("style", "text-align:left;white-space:normal");

                getDeviceFowstList();
            });

            window.DeviceFowst = {
                DeviceFowstList: function () {
                    getDeviceFowstList();
                    setMonthCollection("selDevFOWSTBegBalDateMonth");
                    setYearCollection("selDevFOWSTBegBalDateYear");

                    setMonthCollection("selDevFowstBIRApplicationDateMonth");
                    setYearCollection("selDevFowstBIRApplicationBalDateYear");
                }
            }

            $('#btnDeviceFowstBack').click(function (e) {

                history.back();
            });

            $('body').on('click', '#btnAddDeviceFOWST', function () {
                isAddNewFowst = true;
                $('#pageDeviceFowstInfo').show();
                $('#pageDeviceFowst').hide();
                ClearDeviceFowstFields();
            })
            .on('click', '#btnFOWST', function () {
                //                $('#TitleLibrary').text('List of FOWST');
                //                $('#txtSearchKeyword').val('').focus().attr('placeholder', 'Type name of fowst');
                //                $('#panSearchLibrary').popup('open');
                //                $('#lstOfLibrary').empty();
                //                SearchFowstPopUp('');
                BrowsePopup('LibraryFowst', 'FOWST', [
                    {
                        DisplayText: [
                            { Column: 'FowstName' }
                        ],
                        AutoSearch: true,
                        Click: function (vData) {
                            var isExist = false;

                            $('#lstDeviceFowst li').each(function () {
                                if ($(this).attr('id') == vData.ID) {
                                    isExist = true;
                                }
                            })

                            if (isExist) {
                                NewMessageBox('FOWST was already exist', 'FOWST');
                                return false;
                            }

                            $('#txtFowstID').val(vData.ID)
                            $('#btnFOWST').text(vData.FowstName)
                                                //.button('refresh');

                            $('#divSMMachineNo').hide();
                            if (vData.FOWSTMallName == 'SM Mall') {
                                $('#divSMMachineNo').show()
                            }
                            //$('#panSearchLibrary').popup('close');
                            GetPrinterNames(vData.ID, '');
                        }
                    }
                ])
            })
            .on('keypress', '#txtSearchKeyword', function (e) {
                if (e.keyCode == 13) {
                    SearchFowstPopUp($(this).val());
                }
            })
            .on('change', '#selDevFOWSTBegBalDateMonth', function (e) {
                e.preventDefault();
                setDayCollection('selDevFOWSTBegBalDateMonth', 'selDevFOWSTBegBalDateDay', 'selDevFOWSTBegBalDateYear', false);
            })
            .on('change', '#selDevFOWSTBegBalDateYear', function (e) {
                e.preventDefault();
                setDayCollection('selDevFOWSTBegBalDateMonth', 'selDevFOWSTBegBalDateDay', 'selDevFOWSTBegBalDateYear', false);
            })
            .on('click', '#selDevFOWSTBegBalDateYear', function (e) {
                e.preventDefault();
                if ($.trim($('#selDevFOWSTBegBalDateYear').val()) == "") {
                    $('#selDevFOWSTBegBalDateYear').val('2013');
                    $('#selDevFOWSTBegBalDateYear')//.selectmenu('refresh');
                }
            })
            .on('keydown', '#txtDevFOWSTBegBalance', function (e) {
                if (e.keyCode != 110 && e.keyCode != 190) {
                    if (!NumberOnly(e)) return;
                }
            })
            .on('keydown', '#txtDevFOWSTInvoiceStartNo', function (e) {
                if (!NumberOnly(e)) {
                    return;
                }
            })
            .on('keydown', '#txtDevFOWSTSRVStartNo', function (e) {
                if (!NumberOnly(e)) {
                    return;
                }
            })
            .on('keydown', '#txtDevFOWSTRefStartNo', function (e) {
                if (!NumberOnly(e)) {
                    return;
                }
            })
            .on('click', '#btnDevFowstSave', function (e) {
                e.preventDefault();

                if ($.trim($("#txtFowstID").val()) == "") {
                    NewMessageBox('Please select device FOWST assignment.', 'FOWST');
                    return;
                }

                //                if ($.trim($("#selPrinterFowstName").val()) == "") {
                //                    NewMessageBox('Please select a printer.', 'Printer');
                //                    return;
                //                }

                if (CBool($("#selIsDeviceAuthorizedofBIR").val())) {

                    if ($.trim($("#txtPOSRegNo").val()) == "") {
                        NewMessageBox('Please enter POS registration number.', 'Printer');
                        return;
                    }

                    if (!IsDate($("#txtDevFowstBIRPTUDateIssued").val())) {
                        NewMessageBox('Please set PTU date issued.', 'Printer');
                        return;
                    }

                    if ($.trim($("#txtDevFowstBIRMachineNo").val()) == "") {
                        NewMessageBox('Please enter Machine Identification Number (MIN) issued by BIR.', 'MIN Issued by BIR');
                        return;
                    }

                }
                    if ($("#divSMMachineNo").is(':visible')) {
                        if ($.trim($("#txtDevFOWSTSMMachineNo").val()) == "") {
                            NewMessageBox('Please enter POS machine number assigned by SM Management.', 'SM POS Machine No.');
                            return;
                        }
                    }
                               

                //FOWST Beginning Balance
                if ($('#selDevFOWSTBegBalDateMonth').attr("disabled") == undefined) {

                    if ($.trim($("#selDevFOWSTBegBalDateMonth").val()) != "" ||
                            $.trim($("#selDevFOWSTBegBalDateDay").val()) != "" ||
                            $.trim($("#selDevFOWSTBegBalDateYear").val()) != "") {

                        if ($.trim($("#selDevFOWSTBegBalDateMonth").val()) == "" ||
                                $.trim($("#selDevFOWSTBegBalDateDay").val()) == "" ||
                                $.trim($("#selDevFOWSTBegBalDateYear").val()) == "") {
                            NewMessageBox('Please set a valid beginning balance date.', 'Beginning Balance Date');
                            return;
                        }
                        if ($.trim($("#txtDevFOWSTBegBalance").val()) == "") {
                            NewMessageBox('Please set a valid sales beginning balance.', 'Sales Beginning Balance');
                            return;
                        }
                        if ($.trim($("#txtDevFOWSTInvoiceStartNo").val()) == "") {
                            NewMessageBox('Please set the last sales invoice number.', 'Last Sales Invoice Number');
                            return;
                        }
                        if ($.trim($("#txtDevFOWSTSRVStartNo").val()) == "") {
                            NewMessageBox('Please set the last sales return voucher number (SRV).', 'Last Sales Invoice Number');
                            return;
                        }

                        if ($.trim($("#txtDevFOWSTRefStartNo").val()) == "") {
                            NewMessageBox('Please set the last sales refund number.', 'Last Sales Invoice Number');
                            return;
                        }

                        NewMessageBox('Beginning balance could be set at once only. Make sure you have entered the accurate information before you proceed. Do you want to continue?', 'Setting Beginning Balance',
                            [
                                {
                                    Text: 'Yes',
                                    Click: function () {
                                        SaveDeviceFowst();
                                    }
                                },
                                {
                                    Text: 'No'
                                }
                            ]);
                    } else {
                        SaveDeviceFowst();
                    }

                } else {
                    SaveDeviceFowst();
                }
            })
            .on('click', '#btnDevFowstCancel', function (e) {
                e.preventDefault();
                //$.mobile.changePage('#pageDeviceFowst');
                $('#pageDeviceFowstInfo').hide();
                $('#pageDeviceFowst').show();
            })
            .on('click', '#btnDeviceFowstDone', function (e) {
                e.preventDefault();
                //window.open("DeviceInfoSummary", "_self");
                sessionStorage["AddViewLibBCName"] = "View Device";
                window.location.hash = '';
                window.location.hash = 'UniversalPage';
                LibraryTable.ViewDevicePage();
            })
            .on('change', '#selIsDeviceAuthorizedofBIR', function (e) {
                e.preventDefault();
                if ($('#selIsDeviceAuthorizedofBIR').val() == '1') {
                    $('#dvBIRInfo').css('visibility', 'visible');
                    $('#dvBIRInfo').css('display', 'block');
                    $('#dvNoneBIRInfo').css('visibility', 'hidden');
                    $('#dvNoneBIRInfo').css('display', 'none');
                } else {
                    $('#dvNoneBIRInfo').css('visibility', 'visible');
                    $('#dvNoneBIRInfo').css('display', 'block');
                    $('#dvBIRInfo').css('visibility', 'hidden');
                    $('#dvBIRInfo').css('display', 'none');
                }
            })
            .on('change', '#NoneRegistrationReason1', function (e) {
                e.preventDefault();                
                $('#dvRegistrationInProgress').css('visibility', 'hidden');
                $('#dvRegistrationInProgress').css('display', 'none');
            })
            .on('change', '#NoneRegistrationReason2', function (e) {
                e.preventDefault();
                $('#dvRegistrationInProgress').css('visibility', 'visible');
                $('#dvRegistrationInProgress').css('display', 'block');
            })
            .on('change', '#selDevFowstBIRApplicationDateMonth', function (e) {
                e.preventDefault();
                setDayCollection('selDevFowstBIRApplicationDateMonth', 'selDevFowstBIRApplicationDateDay', 'selDevFowstBIRApplicationBalDateYear', false);
            })
            .on('change', '#selDevFowstBIRApplicationBalDateYear', function (e) {
                e.preventDefault();
                setDayCollection('selDevFowstBIRApplicationDateMonth', 'selDevFowstBIRApplicationDateDay', 'selDevFowstBIRApplicationBalDateYear', false);
            })
            .on('click', '#lnkZeroOutHistory', function(e) {
                e.preventDefault();
                PromptUserAccess('Zero out / clear transaction history for this Device+FOWST location', {
                    OnClickOKSuccess: function () {
                        NewMessageBox('Are you sure you want to zero out / clear transaction history for this device+FOWST location?', '', [
                            {
                                Text: 'Yes',
                                Click: function() {
                                    AjaxSend({
                                        url: 'Device/ZeroOutDeviceFowstTransactions',
                                        data: { DeviceFowstID: $("#txtDeviceFowstID").val()},
                                        success: function (vData) {
                                            if (vData.error)
                                                NewMessageBox(vData.error);
                                            else
                                                NewMessageBox('The transaction history of this device-FOWST was successfully zeroed out.', 'Zero Out');
                                        }
                                    });
                                }
                            },
                            {
                                Text: 'No'
                            }
                        ]);
                    }
                });
            });

            function SearchFowstPopUp(s) {
                var pData = {};

                pData.keywords = s;

                AjaxSend({
                    url: 'Browser/LibraryFowst',
                    data: pData,
                    success: function (vData) {
                        $('#lstOfLibrary').empty();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstOfLibrary').append($('<li>')
                                    .append($('<a>')
                                        .attr('ID', vData.Results.Rows[p].ID)
                                        .append($('<br><p style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].FowstName)
                                        )
                                        .append($('<p>')
                                            .text(vData.Results.Rows[p].Address)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function (e) {
                                            $('#txtFowstID').val($(this).attr('ID'))
                                            $('#btnFOWST').text($(this).data('data').FowstName)
                                                //.button('refresh');

                                            $('#divSMMachineNo').hide();
                                            if (e.FOWSTMallName == 'SM Mall') {
                                                $('#divSMMachineNo').show()
                                            }
                                            //$('#panSearchLibrary').popup('close');
                                            GetPrinterNames($(this).attr('ID'), '');
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#lstOfLibrary').append($('<li>').text('No Result(s) Found'));
                        }

                        $('#lstOfLibrary').listview('refresh');
                    }
                });
            }

            function getDeviceFowstList() {
                AjaxSend({
                    url: 'Device/getDeviceFOWSTList',
                    success: function (vData) {
                        LoadDeviceFowstList(vData.DeviceFowstData.Rows);
                    }
                });
            }

            function LoadDeviceFowstList(vRows) {
                var pLV = $('#lstDeviceFowst');
                var pItem;

                $("#divRecordCountDeviceFowst").text(" Number of record(s) found: " + vRows.length);

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<a href="#" class="list-group-item list-group-item-action">')
                                .append($('<table style="width:100%;">')
                                    .append($('<tr>')
                                        .append($('<td style="word-break:break-all;">')
                                            .attr('ID', vRows[p].ID)
                                            .data('DeviceID', vRows[p].DeviceID)
                                            .data('FowstID', vRows[p].FowstID)
                                            .append($('<h5>')
                                                .attr("style", "white-space:normal")
                                                .text(vRows[p].FOWSTName)
                                            )
                                            .append($('<div>')
                                                .text('POS Registration No. : ' + vRows[p].POSRegistrationNo)
                                            )
                                            .append($('<div>')
                                                .text('MIN : ' + vRows[p].FOWSTNo + '-' + vRows[p].DeviceNo)
                                            )
                                            .click(function (e) {
                                                e.preventDefault();
                                                //$.mobile.changePage('#pageDeviceFowstInfo');
                                                $('#pageDeviceFowstInfo').show();
                                                $('#pageDeviceFowst').hide();
                                                ShowDeviceFOWSTInfo($(this).attr('ID'), $(this).data('DeviceID'), $(this).data('FowstID'));
                                            })
                                        )
                                        .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                                .append($('<span class="bi-trash-fill" aria-hidden="true">'))
                                                .data('data', vRows[p])
                                                .click(function (e) {
                                                    e.preventDefault();
                                                    var id = $(this).data('data').ID;
                                                    NewMessageBox('Are you sure you want remove <strong>"' + $(this).data('data').FOWSTName + '"</strong> ?', 'Remove', [
                                                        {
                                                            Text: 'Yes',
                                                            Click: function () {
                                                                isAddNewFowst = false;
                                                                RemoveDeviceFowst(id);
                                                            }
                                                        },
                                                        {
                                                            Text: 'No'
                                                        }
                                                    ])
                                                })
                                            )
                                        )
                                    )
                                )

                    pLV.append(pItem);
                };

                //pLV.listview('refresh');
            };

            function RemoveDeviceFowst(id) {
                AjaxSend({
                    url: 'Device/RemoveDeviceFowst',
                    data: { ID: id },
                    success: function (vData) {
                        isAddNewFowst = true;
                        if (vData.Results == "ok") {
                            getDeviceFowstList();
                        }
                        else {
                            NewMessageBox('<strong>Error:</strong> ' + vData, 'Error');
                        }
                    }
                })
            }

            function GetPrinterNames(fowstid, printerfowstid) {
                var pData = {};
                pData.FowstID = fowstid;

                AjaxSend({
                    url: "Device/GetPrinterNames",
                    data: pData,
                    success: function (vData) {

                        $('#selPrinterFowstName').empty();

                        $('#selPrinterFowstName').append(new Option('Please Select Printer', ''));

                        if (vData.DeviceData.Rows.length > 0) {
                            $('#divPrinterFowstName').show();
                            var vRows = vData.DeviceData.Rows;


                            for (var p = 0; p <= vRows.length - 1; p++) {
                                $('#selPrinterFowstName').append(new Option(vRows[p].PrinterName + ' - ' + vRows[p].PrinterIPAddress, vRows[p].ID, false, false));

                            }

                            if (printerfowstid == '')
                                $('#selPrinterFowstName').val('')//.selectmenu('refresh');
                            else
                                $("#selPrinterFowstName").val(printerfowstid)//.selectmenu('refresh');

                        } else {
                            $('#selPrinterFowstName').val('');

                            NewMessageBox('There are no printers assigned to this FOWST. Would you like to add a printer?', 'Printer', [
                                {
                                    Text: 'Yes', Click: function () {
                                        AjaxSend({
                                            url: 'Login3/TestCookies',
                                            success: function (vData) {
                                                window.pageSecuritySourcePage = $.mobile.activePage;
                                                PromptUserAccess('Company Places (FOWST)', {
                                                    OnClickOKSuccess: function () {
                                                        ShowOrganizationPlaces(fowstid);
                                                    }
                                                });
                                            },
                                            error: function (vError) {
                                                NewMessageBox('This feature requires internet connection', 'Requires Internet Connection');
                                            }
                                        });
                                    }
                                }, { Text: 'No' }
                            ])
                        }
                    }
                });
            }


            function ShowOrganizationPlaces(fowstid) {
                var pData = {};
                pData.IsNew = 0;
                pData.ID = sessionStorage.OrgID;
                sessionStorage.ViewID = fowstid;
                $('#UniversalDiv').empty();
                AjaxSend({
                    url: 'Organization/setCreateNew',
                    data: pData,
                    success: function (vData) {
                        //                        window.location = MergeUrlParameters({
                        //                            mode: 'wizard',
                        //                            OrgID: sessionStorage.OrgID,
                        //                            IsMobile: CBool(IsMobile) ? 1 : 0
                        //                        }, '/Organization/OrganizationAddress');
                        AjaxSend({
                            ExactUrl: '/Organization/OrganizationAddress?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                            type: 'GET',
                            dataType: 'text',
                            success: function (vData) {
                                $('#UniversalDiv').empty();
                                $('#UniversalDiv').append(vData).trigger('create');
                                setTimeout(function () {
                                    Fowst.ShowFowstInfo(fowstid);
                                }, 5000);

                                App.Deferred.fulfill('FQ26E5TP76');
                            }
                        });
                    }
                });
            }


            function ShowDeviceFOWSTInfo(strID, deviceid, fowstid) {
                var pData = {};
                pData.DeviceFowstID = strID;
                pData.DeviceID = deviceid;
                pData.FowstID = fowstid;

                ClearDeviceFowstFields();

                AjaxSend({
                    url: "Device/getDeviceFOWSTInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.DeviceData.Rows.length > 0) {

                            $("#txtDeviceFowstID").val(vData.DeviceData.Rows[0].ID || '');

                            $("#txtFowstID").val(vData.DeviceData.Rows[0].FowstID || '');
                            $("#btnFOWST").text(vData.DeviceData.Rows[0].FOWSTName || '')//.button('refresh');


                            if (vData.DeviceData.Rows[0].PrinterFowstID == '')
                                GetPrinterNames(vData.DeviceData.Rows[0].FowstID, '');
                            else
                                GetPrinterNames(vData.DeviceData.Rows[0].FowstID, vData.DeviceData.Rows[0].PrinterFowstID);

                            $("#txtPOSRegNo").val(vData.DeviceData.Rows[0].POSRegistrationNo || '');
                            $("#txtDevFowstBIRPTUDateIssued").val($.format.date(new Date(vData.DeviceData.Rows[0].POSPTUDateIssued), 'yyyy-MM-dd'));
                            $("#txtDevFowstBIRMachineNo").val(vData.DeviceData.Rows[0].BIRMachineNo || '');
                            $("#txtDevFowstPrinterIP").val(vData.DeviceData.Rows[0].DevicePrinterIP || '');
                            //$("#txtPrintServer").val(vData.DeviceData.Rows[0].PrintServer || '');

                            $("#txtTerminalNo").val(vData.DeviceData.Rows[0].TerminalNo || '');
                            $("#txtDevFOWSTSMMachineNo").val(vData.DeviceData.Rows[0].SMMachineNo || '');

                            $("#selDevFOWSTBegBalDateMonth").val(vData.DeviceData.Rows[0].BegBalDateMonth || '')//.selectmenu('refresh');
                            $("#selDevFOWSTBegBalDateYear").val(vData.DeviceData.Rows[0].BegBalDateYear || '')//.selectmenu('refresh');
                            setDayCollection('selDevFOWSTBegBalDateMonth', 'selDevFOWSTBegBalDateDay', 'selDevFOWSTBegBalDateYear', false);
                            $("#selDevFOWSTBegBalDateDay").val(vData.DeviceData.Rows[0].BegBalDateDay || '')//.selectmenu('refresh');

                            if (parseFloat(vData.DeviceData.Rows[0].BegBalance) > 0) {
                                $("#txtDevFOWSTBegBalance").val(AccountingFormat(vData.DeviceData.Rows[0].BegBalance || '0.00'));
                            } else {
                                $("#txtDevFOWSTBegBalance").val(vData.DeviceData.Rows[0].BegBalance || '0.00');
                            }
                            $("#txtDevFOWSTInvoiceStartNo").val(vData.DeviceData.Rows[0].POSLastORNo || '');
                            $("#txtDevFOWSTSRVStartNo").val(vData.DeviceData.Rows[0].POSLastSRVNo || '');
                            $("#txtDevFOWSTRefStartNo").val(vData.DeviceData.Rows[0].POSLastRefNo || '');

                            //Show Hide SM Machine No
                            $('#divSMMachineNo').hide()
                            if (vData.DeviceData.Rows[0].FOWSTMallName == 'SM Mall') {
                                $('#divSMMachineNo').show();
                            }

                            //Disable if not editable
                            $('#selDevFOWSTBegBalDateMonth').attr("disabled", (vData.DeviceData.Rows[0].IsBegBalNotEditable == 1 ? true : false));
                            $('#selDevFOWSTBegBalDateDay').attr("disabled", (vData.DeviceData.Rows[0].IsBegBalNotEditable == 1 ? true : false));
                            $('#selDevFOWSTBegBalDateYear').attr("disabled", (vData.DeviceData.Rows[0].IsBegBalNotEditable == 1 ? true : false));
                            $("#txtOrgFOWSTBegBalance").attr("disabled", (vData.DeviceData.Rows[0].IsBegBalNotEditable == 1 ? true : false));
                            $("#txtOrgFOWSTInvoiceStartNo").attr("disabled", (vData.DeviceData.Rows[0].IsBegBalNotEditable == 1 ? true : false));

                            $('#selIsDeviceAuthorizedofBIR').val((CBool(vData.DeviceData.Rows[0].IsAuthorizedForPOS) == true ? "1" : "0"));

                            $("#selDevFowstBIRApplicationDateMonth").val(new Date(vData.DeviceData.Rows[0].ApplicationDate).getMonth() - 1 || '')//.selectmenu('refresh');
                            $("#selDevFowstBIRApplicationBalDateYear").val(new Date(vData.DeviceData.Rows[0].ApplicationDate).getYear() || '')//.selectmenu('refresh');
                            setDayCollection('selDevFOWSTBegBalDateMonth', 'selDevFOWSTBegBalDateDay', 'selDevFOWSTBegBalDateYear', false);
                            $("#selDevFowstBIRApplicationDateDay").val(new Date(vData.DeviceData.Rows[0].ApplicationDate).getDay() || '')//.selectmenu('refresh');

                            try {
                                $('#selIsDeviceAuthorizedofBIR')//.slider('refresh');
                            } catch (e) {
                                $('#selIsDeviceAuthorizedofBIR')//.slider();
                            }

                            $('#selIsDeviceAuthorizedofBIR').change();

                            if (CInt2(vData.DeviceData.Rows[0].ReasonForNonRegistration) == 0) {
                                $('#NoneRegistrationReason1').prop("checked", true);
                                $('#NoneRegistrationReason2').prop("checked", false);
                                $('#NoneRegistrationReason1').change();
                            } else {
                                $('#NoneRegistrationReason1').prop("checked", false);
                                $('#NoneRegistrationReason2').prop("checked", true);
                                $('#NoneRegistrationReason2').change();
                            }

                            $('#txtDevFowstBIRApplicationReferenceNo').val(vData.DeviceData.Rows[0].ApplicationReferenceNo);
                            $('#selIsShowCompanyHeader').val((CBool(vData.DeviceData.Rows[0].ShowCompanyDetailsInHeader) == true ? "1" : "0"));

                            try {
                                $('#NoneRegistrationReason1')//.checkboxradio('refresh'); //refresh here
                                $('#NoneRegistrationReason2')//.checkboxradio('refresh');                                
                                $('#selIsShowCompanyHeader')//.slider('refresh');
                            } catch (e) {
                                $('#NoneRegistrationReason1')//.checkboxradio();
                                $('#NoneRegistrationReason2')//.checkboxradio();
                                $('#selIsShowCompanyHeader')//.slider();
                            }
                        }

                    }
                });

            };

            function SaveDeviceFowst() {
                var pData = [];

                var dtBegBalDate = "";
                var dtApplicationDate = "";

                if ($('#selDevFOWSTBegBalDateMonth').val() != "" &&
                    $('#selDevFOWSTBegBalDateDay').val() != "" &&
                    $('#selDevFOWSTBegBalDateYear').val() != "") {

                    dtBegBalDate = $('#selDevFOWSTBegBalDateMonth').val() + "/" + $('#selDevFOWSTBegBalDateDay').val() + "/" + $('#selDevFOWSTBegBalDateYear').val();
                }

                if ($('#selDevFowstBIRApplicationDateMonth').val() != "" &&
                    $('#selDevFowstBIRApplicationDateDay').val() != "" &&
                    $('#selDevFowstBIRApplicationBalDateYear').val() != "") {

                    dtApplicationDate = $('#selDevFowstBIRApplicationDateMonth').val() + "/" + $('#selDevFowstBIRApplicationDateDay').val() + "/" + $('#selDevFowstBIRApplicationBalDateYear').val();
                }

                pData = {
                    DeviceFowstID: $("#txtDeviceFowstID").val(),
                    PrinterFowstID: $("#selPrinterFowstName").val(),
                    FowstID: $("#txtFowstID").val(),
                    POSRegistrationNo: $("#txtPOSRegNo").val(),
                    POSPTUDateIssued: $("#txtDevFowstBIRPTUDateIssued").val(),
                    BIRMachineNo: $("#txtDevFowstBIRMachineNo").val(),
                    DevFowstPrinterIP: $("#txtDevFowstPrinterIP").val(),
                    POSSMMachineNo: ($("#divSMMachineNo").is(':visible') ? $("#txtDevFOWSTSMMachineNo").val() : ""),
                    BegBalDate: dtBegBalDate,
                    BegBalance: $("#txtDevFOWSTBegBalance").val(),
                    POSLastORNo: $("#txtDevFOWSTInvoiceStartNo").val(),
                    POSLastSRVNo: $("#txtDevFOWSTSRVStartNo").val(),
                    POSLastRefNo: $("#txtDevFOWSTRefStartNo").val(),
                    IsAuthorizedForPOS: $("#selIsDeviceAuthorizedofBIR").val(),
                    ReasonForNonRegistration: ($("#NoneRegistrationReason1").prop('checked') ? 0 : 1),
                    ApplicationReferenceNo: $("#txtDevFowstBIRApplicationReferenceNo").val(),
                    ApplicationDate: dtApplicationDate,
                    ShowCompanyDetailsInHeader: $("#selIsShowCompanyHeader").val()
                    //PrintServer: $("#txtPrintServer").val()
                };

                AjaxSend({
                    url: "Device/CheckDeviceFowst",
                    data: pData,
                    success: function (vData) {

                        if (vData.DeviceData.Rows.length > 0) {
                            //                            if (!CBool(isAddNewFowst)) {
                            //                                pData.DeviceFowstID = vData.DeviceData.Rows[0].ID;
                            //                                SaveDeviceFOWSTNow(pData, vData);
                            //                            }
                            //                            else {
                            //                                NewMessageBox('FOWST was already exist', 'FOWST');
                            //                            }
                            pData.DeviceFowstID = vData.DeviceData.Rows[0].ID;
                            SaveDeviceFOWSTNow(pData, vData);
                        }
                        else {
                            if (GetUrlParameters().mode == 'wizard') {
                                if ($("#selPrinterFowstName").val() == '') {
                                    NewMessageBox('Please select a printer.', 'Printer');
                                    return;
                                }
                            } //end if mode                           
                            SaveDeviceFOWSTNow(pData, vData);
                        }

                    }
                });

            };

            function SaveDeviceFOWSTNow(pData, vData) {
                AjaxSend({
                    url: "Device/SaveDeviceFowst",
                    data: pData,
                    success: function (vSaveData) {

                        if (vData.DeviceData == "Invalid BegBalDate") {
                            NewMessageBox('Please set valid beginning balance date.', 'Beginning Balance Date');
                        }
                        else if (vData.DeviceData == "Invalid BegBal") {
                            NewMessageBox('Please set valid beginning balance.', 'Beginning Balance');
                        }
                        else if (vData.DeviceData == "Invalid LastORNo") {
                            NewMessageBox('Please set valid last Invoice Number.', 'Last Invoice Number');
                        }
                        else if (vData.DeviceData == "Invalid LastSRVNo") {
                            NewMessageBox('Please set valid last SRV Number.', 'Last SRV Number');
                        }
                        else if (vData.DeviceData == "Invalid LastRefNo") {
                            NewMessageBox('Please set valid last Refund Number.', 'Last Refund Number');
                        }
                        else {
                            if (GetUrlParameters().mode == 'wizard') {
                                NewMessageBox('You have completed the Device Printer Setup. Please sync your device to apply the changes.', 'Device Printer Setup');
                            } else {
                                $('#pageDeviceFowstInfo').hide();
                                $('#pageDeviceFowst').show();
                                //$.mobile.changePage('#pageDeviceFowst');
                                getDeviceFowstList();
                            } //end if mode
                        } //end if 

                    } //end success
                });
            }

            function ClearDeviceFowstFields() {

                $("#txtDeviceFowstID").val('');

                $("#txtFowstID").val('');
                $("#selPrinterFowstName").val('')//.selectmenu('refresh');
                $("#btnFOWST").text('Please Select');
                $("#btnFOWST")//.button('refresh');
                $("#txtDevFowstBIRMachineNo").val('');
                $("#txtPOSRegNo").val('');
                $("#txtDevFowstBIRPTUDateIssued").val('');
                $("#txtTerminalNo").val('');

                $("#selDevFOWSTBegBalDateMonth").val('')//.selectmenu('refresh');
                $("#selDevFOWSTBegBalDateDay").val('')//.selectmenu('refresh');
                $("#selDevFOWSTBegBalDateYear").val('')//.selectmenu('refresh');

                $("#txtDevFOWSTBegBalance").val('');
                $("#txtDevFOWSTInvoiceStartNo").val('');
                $("#txtDevFOWSTSRVStartNo").val('');
                $("#txtDevFOWSTRefStartNo").val('');
            }


        });
    });