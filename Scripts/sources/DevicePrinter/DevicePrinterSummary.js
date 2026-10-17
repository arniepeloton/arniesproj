require([
    'common'
    ], function () {

        $(function () {

            var mLoaded = false;
            var isIE = false || !!document.documentMode;
            var DevicePrinterInfoPage = "";

            $.mobile.activePage.bind('pageshow', PageShow);
            function PageShow() {
                mLoaded = true;

                if (!window.IsPageLoaded) {

                    AjaxSend({
                        url: 'DevicePrinter/IsNewEntry',
                        success: function (vData) {
                            if (vData.IsNew == 'FALSE') {
                                sessionStorage.PrinterID = vData.DevicePrinterID;
                            }

                            ShowSumDevicePrinterInfo(vData.DevicePrinterID || '');

                        }
                    });

                    window.IsPageLoaded = true;
                }
            };
            if (!mLoaded) PageShow();

            window.PrinterSummary = {
                ShowSumDevicePrinterInfo: ShowSumDevicePrinterInfo
            }

            $('body').on('click', '#btnDevicePrinterSummaryBack', function () {
                if (GetUrlParameters().mode == 'wizard') {
                    if ($.trim($('#txtSumDevicePrinterType').val()) == '') {
                        $('#TitleOrgEmp').text('Printer Setup');
                        $('#MsgOrgEmp').text('Please select a printer type');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#btnEditDevicePrinter').click();
                        })
                        $('#divBtnCancel').hide();
                    }
                    else {
                        $('#TitleOrgEmp').text('Device Printer Setup');
                        $('#MsgOrgEmp').text('Next step: Please select a FOWST and add a printer. Scroll to the bottom of the FOWST page to view the Printer section.');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            AjaxSend({
                                url: 'Login3/TestCookies',
                                success: function (vData) {
                                    window.pageSecuritySourcePage = $.mobile.activePage;
                                    PromptUserAccess('Company Places (FOWST)', {
                                        OnClickOKSuccess: function () {
                                            ShowOrganizationPlaces();
                                        }
                                    });
                                },
                                error: function (vError) {
                                    $('#TitleOrgEmp').text('Requires Internet Connection');
                                    $('#MsgOrgEmp').text('This feature requires internet connection.');
                                    $('#panOrgEmpMessage').popup('open');
                                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                                        e.preventDefault();
                                        $('#panOrgEmpMessage').popup('close');
                                    })
                                    $('#divBtnCancel').hide();
                                    return;
                                }

                            });
                        })
                        $('#divBtnCancel').hide();
                    }
                } else {
                    //window.location = GetPathFormat("/DevicePrinter?OrgID=" + sessionStorage.OrgID);
                    window.location.hash = 'LibraryPage';
                }
            })
            .on('click', '#btnEditDevicePrinter', function (e) {
                e.preventDefault();
                CreateNewEntry(0, sessionStorage.PrinterID);
            })

            $('#btnEditDevicePrinterDevice').click(function (e) {
                e.preventDefault();

                window.location = GetPathFormat("/DevicePrinter/DevicePrinterDevice?OrgID=" + sessionStorage.OrgID);
            });

            $('#btnEditDevicePrinterProduct').click(function (e) {
                e.preventDefault();

                //            window.open(GetPathFormat("DeviceFowst"), "_self");
            });

            function ShowOrganizationPlaces() {
                var pData = {};
                pData.IsNew = 0;
                pData.ID = sessionStorage.OrgID;

                AjaxSend({
                    url: 'Organization/setCreateNew',
                    data: pData,
                    success: function (vData) {
                        //window.location = MergeUrlParameters({
                        //    mode: 'wizard',
                        //    OrgID: sessionStorage.OrgID,
                        //    IsMobile: CBool(IsMobile) ? 1 : 0
                        //}, '/Organization/OrganizationAddress');
                        var IsMobile = CBool(IsMobile) ? 1 : 0;
                        AjaxSend({
                            ExactUrl: '/Organization/OrganizationAddress?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                            type: 'GET',
                            dataType: 'text',
                            success: function (vData) {
                                $('#UniversalDiv').empty();
                                $('#UniversalDiv').append(vData).trigger('create');

                                setTimeout(function () {
                                    if ($('#selOrgFOWSTType').val() == "" || $('#selOrgFOWSTTradeName').val() == "") {
                                        setMonthCollection("selOrgFOWSTContractStartMonth");
                                        setYearCollection("selOrgFOWSTContractStartYear");

                                        setMonthCollection("selOrgFOWSTContractEndMonth");
                                        setYearCollection("selOrgFOWSTContractEndYear");

                                        setMonthCollection("selOrgFOWSTTRegIssuedMonth");
                                        setYearCollection("selOrgFOWSTTRegIssuedYear");

                                        setMonthCollection("selOrgFOWSTTRegExpirationMonth");
                                        setYearCollection("selOrgFOWSTTRegExpirationYear");

                                        setMonthCollection("selOrgFOWSTBPermitDateOfPaymentMonth");
                                        setYearCollection("selOrgFOWSTBPermitDateOfPaymentYear");

                                        setMonthCollection("selOrgFOWSTBPermitBPlateIssuedMonth");
                                        setYearCollection("selOrgFOWSTBPermitBPlateIssuedYear");

                                        setHoursCollection("selOrgFOWSTOptOpenHR");
                                        setMinutesCollection("selOrgFOWSTOptOpenMin");
                                        setHoursCollection("selOrgFOWSTOptCloseHR");
                                        setMinutesCollection("selOrgFOWSTOptCloseMin");

                                        $("#selOrgFOWSTIsMortgaged").val('1').slider("refresh");

                                        getOrgAddTradeName();
                                        getOrgAddFowstType();
                                        getOrgAddWarehouseType();
                                        getOrgAddRetailStoreType();
                                    }
                                }, 1000)
                                App.Deferred.fulfill('FQ26E5TP76');
                            }
                        });
                    }
                });
            }


            function ShowSumDevicePrinterInfo(strID) {
                var pData = {};
                pData.DevicePrinterID = strID;

                ClearSumDevicePrinterFields();

                AjaxSend({
                    url: "DevicePrinter/getDevicePrinterInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.DevicePrinterData.Rows.length > 0) {

                            $("#txtSumDevicePrinterID").val(vData.DevicePrinterData.Rows[0].ID || '');
                            $("#txtSumDevicePrinterRole").val(vData.DevicePrinterData.Rows[0].PrinterRole || '');
                            $("#txtSumDevicePrinterType").val(vData.DevicePrinterData.Rows[0].PrinterType || '');

                            $('#chkSumIsActive').attr("checked", (vData.DevicePrinterData.Rows[0].IsActive == 0 ? false : true));//.checkboxradio("refresh");

                            getSumDevicePrinterCategories(vData.DevicePrinterData.Rows[0].ID);

                            if (vData.DevicePrinterData.Rows[0].PrinterRole == 'Point of Sale (POS)' || vData.DevicePrinterData.Rows[0].PrinterRole == 'Sales Order Taker')
                                $('#divSumCategoriesList').hide();
                            else
                                $('#divSumCategoriesList').show();

                        }
                    }
                });

            };

            function ClearSumDevicePrinterFields() {

                $("#txtSumDevicePrinterID").val('');

                $("#txtSumDevicePrinterRole").val('');
                $("#txtSumDevicePrinterType").val('');
                $('#chkSumIsActive').attr("checked", true);//.checkboxradio("refresh");

            }

            function getSumDevicePrinterDeviceList() {
                AjaxSend({
                    url: 'DevicePrinter/getDevicePrinterDeviceList',
                    success: function (vData) {
                        LoadSumDevicePrinterDeviceList(vData.DevicePrinterData.Rows);
                    }
                });
            }

            function LoadSumDevicePrinterDeviceList(vRows) {
                var pLV = $('#lstSumDevicePrinterDevice');
                var pItem;

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<a href="#" class="list-group-item list-group-item-action">')
                        .append($('<h3>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].PhoneName)
                        )
                        .append($('<div>')
                            .text('Phone Model : ' + vRows[p].PhoneModel)
                        )
                        .append($('<div>')
                            .text('IMEI : ' + vRows[p].IMEI)
                        )
                        .append($('<div>')
                            .text('UUID : ' + vRows[p].UUID)
                        )
                        .append($('<div>')
                            .text('Platform : ' + vRows[p].OS)
                        )
                        .append($('<div>')
                            .text('Active : ' + (vRows[p].IsActive == 1 ? "Yes" : "No"))
                        );

                    pLV.append(pItem);
                };

                pLV;//.listview('refresh');
            };

            function getSumDevicePrinterCategories(id) {

                AjaxSend({
                    url: 'DevicePrinter/getDevicePrinterCategoryList',
                    data: { PrinterID: id },
                    success: function (vData) {
                        LoadSumDevicePrinterCategories(vData.Categories.Rows);
                    }
                });
            }

            function LoadSumDevicePrinterCategories(vRows) {
                var pLV = $('#lstSumDevicePrinterCategories');
                var pItem;

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<a href="#" class="list-group-item list-group-item-action">')
                        .append($('<h3>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].CategoryName)
                        );

                    pLV.append(pItem);
                };

                pLV;//.listview('refresh');
            }

            function CreateNewEntry(IsNew, strID) {
                var pData = {
                    IsNew: IsNew,
                    ID: strID
                };
                AjaxSend({
                    url: 'Device/setCreateNew',
                    data: pData,
                    success: function (vData) {

                        //                        if ((vData.IsNew || '') == "TRUE") {
                        //                            PromptUserAccess('Devices', {
                        //                                OnClickOKSuccess: function () {
                        //                                    //window.open(GetPathFormat("/Device/DeviceInfo"), "_self");
                        //                                    location = MergeUrlParameters({
                        //                                        OrgID: sessionStorage.OrgID,
                        //                                        IsMobile: CBool(IsMobile) ? 1 : 0
                        //                                    }, '/Device/DeviceInfo');
                        //                                }
                        //                            });
                        //                        }
                        //                        else {
                        //                            window.open(GetPathFormat("/Device/DeviceInfoSummary"), "_self");
                        //                        }
                        if (GetUrlParameters().mode == 'wizard') {
                            location = MergeUrlParameters({
                                mode: 'wizard',
                                OrgID: sessionStorage.OrgID,
                                IsMobile: CBool(IsMobile) ? 1 : 0
                            }, '/DevicePrinter/DevicePrinterInfo');
                        } else {
                            if (DevicePrinterInfoPage == "") {
                                AjaxSend({
                                    ExactUrl: '/DevicePrinter/DevicePrinterInfo?v=' + assetversion,
                                    type: 'GET',
                                    dataType: 'text',
                                    success: function (vData) {
                                        DevicePrinterInfoPage = vData;
                                        $('#UniversalDiv').html('');
                                        $('#UniversalDiv').append(vData).trigger('create');
                                        //DevicePrinter.DevicePrinterInfo();
                                    }
                                });
                            }
                            else {
                                $('#UniversalDiv').html('');
                                $('#UniversalDiv').append(DevicePrinterInfoPage).trigger('create');
                                DevicePrinter.DevicePrinterInfo();
                            }

                        } //end if mode
                    }
                });

            }

            function getOrgAddTradeName() {

                AjaxSend({
                    url: 'Organization/getOrgTradeNameList',
                    success: function (vData) {

                        var vRows = vData.OrganizationData.Rows;

                        $('#selOrgFOWSTTradeName').empty();

                        $('#selOrgFOWSTTradeName').append(new Option('Please Select', ''));
                        for (var p = 0; p <= vRows.length - 1; p++) {
                            $('#selOrgFOWSTTradeName').append(new Option(vRows[p].BusinessTradeName, vRows[p].ID, false, false));
                        }
                    },
                    error: function (vError) {
                        var pDummy;
                    }
                });

            }

            function getOrgAddFowstType() {

                AjaxSend({
                    url: 'Browser/LibraryFowstType',
                    success: function (vData) {
                        var vRows = vData.Results.Rows;

                        $('#selOrgFOWSTType').empty();

                        $('#selOrgFOWSTType').append(new Option('Please Select', ''));
                        for (var p = 0; p <= vRows.length - 1; p++) {
                            $('#selOrgFOWSTType').append(new Option(vRows[p].Name, vRows[p].ID, false, false));
                        }
                    },
                    error: function (vError) {
                        var pDummy;
                    }
                });
            }

            function getOrgAddWarehouseType() {
                AjaxSend({
                    url: 'Browser/LibraryFOWSTWarehouseType',
                    success: function (vData) {
                        var vRows = vData.Results.Rows;

                        $('#selOrgFOWSTWarehouseType').empty();

                        $('#selOrgFOWSTWarehouseType').append(new Option('Please Select', ''));
                        for (var p = 0; p <= vRows.length - 1; p++) {
                            $('#selOrgFOWSTWarehouseType').append(new Option(vRows[p].Name, vRows[p].ID, false, false));
                        }
                    },
                    error: function (vError) {
                        var pDummy;
                    }
                });
            }

            function getOrgAddRetailStoreType() {
                AjaxSend({
                    url: 'Browser/LibraryFOWSTRetailStoreType',
                    success: function (vData) {
                        var vRows = vData.Results.Rows;

                        $('#selOrgFOWSTRetailStoreType').empty();

                        $('#selOrgFOWSTRetailStoreType').append(new Option('Please Select', ''));
                        for (var p = 0; p <= vRows.length - 1; p++) {
                            $('#selOrgFOWSTRetailStoreType').append(new Option(vRows[p].Name, vRows[p].ID, false, false));
                        }
                    },
                    error: function (vError) {
                        var pDummy;
                    }
                });
            }

        });
    });


