require([
    'common'
    ], function () {

        $(function () {

            $('body').on('click', '#btnStartSetup', function (e) {
                e.preventDefault();
                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {

                        window.pageSecuritySourcePage = $.mobile.activePage;
                        PromptUserAccess('Printer Roles', {
                            OnClickOKSuccess: function () {
                                //location = MergeUrlParameters({
                                //    mode: 'wizard',
                                //    OrgID: sessionStorage.OrgID,
                                //    IsMobile: CBool(IsMobile) ? 1 : 0
                                //}, '/DevicePrinter');
                                AjaxSend({
                                    ExactUrl: '/DevicePrinter?cache=1&mode=wizard&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                                    type: 'GET',
                                    dataType: 'text',
                                    success: function (vData) {
                                        if (typeof (history.pushState) != "undefined") {
                                            var isMob = CBool(IsMobile) ? 1 : 0;
                                            var obj = { Page: 'page', Url: 'SystemAdministration?OrgID='+ sessionStorage.OrgID + '&IsMobile=' + isMob + '&mode=wizard#UniversalPage' };
                                            history.pushState(obj, obj.Page, obj.Url);
                                        } else {
                                            alert("Browser does not support HTML5.");
                                        }

                                        $('#UniversalDiv').empty();
                                        $('#UniversalDiv').append(vData).trigger('create');

                                        setTimeout(function(){
                                            if($('#lstDevicePrinter li').length == 0){
                                                ShowDevicePrinter();
                                            }
                                        }, 1000)
                                        App.Deferred.fulfill('FQ26E5TP76');
                                    }
                                });
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

            function ShowDevicePrinter() {

                var pData = {};
                pData.SearchString = $("#txtDevicePrinterSearch").val();

                AjaxSend({
                    url: 'DevicePrinter/getDevicePrinterList',
                    data: pData,
                    success: function (vData) {
                        LoadDevicePrinterList(vData.DevicePrinterData.Rows);
                    }
                });

            }

            function LoadDevicePrinterList(vRows) {
                var pLV = $('#lstDevicePrinter');
                var pItem;

                $("#divDevicePrinterResults").hide();
                $("#divDevicePrinterSearchNoResults").hide();
                //                $("#btnAddNewDevicePrinter").hide();

                if (vRows.length > 0) {
                    $("#divDevicePrinterResults").show();
                    $("#divDevicePrinterRecordCount").text(" Number of record(s) found: " + vRows.length);

                    pLV.html('');
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        pItem = $('<li>')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();

                                CreateNewDevicePrinterEntry(0, $(this).attr('ID'));

                            })
                            .append($('<h3>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].PrinterRole)
                            )
                            .append($('<p>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].PrinterType)
                            )
                            .append($('<p>')
                                .attr("style", "white-space:normal")
                                .text('Active : ' + (vRows[p].IsActive == 1 ? 'Yes' : 'No'))
                            )
                        );
                        pLV.append(pItem);
                    };

                    pLV.listview('refresh');
                }
                else {
                    $("#divDevicePrinterSearchNoResults").show();
                    //                    $("#btnAddNewDevicePrinter").show();
                    $("#lblDevicePrinterNotFound").text('');
                    if ($.trim($("#txtDevicePrinterSearch").val()) != '')
                        $("#lblDevicePrinterNotFound").text('The printer "' + $("#txtDevicePrinterSearch").val() + '" does not exist. Please consider to search thoroughly using an alternate spelling before creating a new printer to avoid duplication.');
                }
            }

            function CreateNewDevicePrinterEntry(IsNew, strID) {
                var pData = {};
                pData.IsNew = IsNew;
                pData.ID = strID;

                window.pageOrig = $.mobile.activePage;

                AjaxSend({
                    url: 'DevicePrinter/setCreateNew',
                    data: pData,
                    success: function (vData) {

                        if ((vData.IsNew || '') == "TRUE") {
                            //                            PromptUserAccess('DevicePrinter', {
                            //                                OnClickOKSuccess: function () {
                            //                                    window.IsPageLoaded = false;

                            location = MergeUrlParameters({
                                OrgID: sessionStorage.OrgID,
                                IsMobile: CBool(IsMobile) ? 1 : 0
                            }, '/DevicePrinter/DevicePrinterInfo');
                            //                                }
                            //                            });
                        }
                        else {
                            if (GetUrlParameters().mode == 'wizard') {
                                //window.open(GetPathFormat("/DevicePrinter/DevicePrinterSummary?mode=wizard"), "_self");
                                AjaxSend({
                                    ExactUrl: '/DevicePrinter/DevicePrinterSummary?mode=wizard',
                                    type: 'GET',
                                    dataType: 'text',
                                    success: function (vData) {
                                        AjaxSend({
                                            url: 'DevicePrinter/IsNewEntry',
                                            success: function (vData) {
                                                if (vData.IsNew == 'FALSE') {
                                                    sessionStorage.PrinterID = vData.DevicePrinterID;
                                                }

                                                ShowSumDevicePrinterInfo(vData.DevicePrinterID || '');

                                            }
                                        });
                                        $('#UniversalDiv').empty();
                                        $('#UniversalDiv').append(vData).trigger('create');
                                        App.Deferred.fulfill('FQ26E5TP76');
                                    }
                                });
                            } else
                                window.open(GetPathFormat("/DevicePrinter/DevicePrinterSummary"), "_self");
                        }

                    }
                });

            }

            function ShowSumDevicePrinterInfo(strID) {
                var pData = {};
                pData.DevicePrinterID = strID;

                //ClearSumDevicePrinterFields();

                AjaxSend({
                    url: "DevicePrinter/getDevicePrinterInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.DevicePrinterData.Rows.length > 0) {

                            $("#txtSumDevicePrinterID").val(vData.DevicePrinterData.Rows[0].ID || '');
                            $("#txtSumDevicePrinterRole").val(vData.DevicePrinterData.Rows[0].PrinterRole || '');
                            $("#txtSumDevicePrinterType").val(vData.DevicePrinterData.Rows[0].PrinterType || '');

                            $('#chkSumIsActive').attr("checked", (vData.DevicePrinterData.Rows[0].IsActive == 0 ? false : true)).checkboxradio("refresh");

                            getSumDevicePrinterCategories(vData.DevicePrinterData.Rows[0].ID);

                            if (vData.DevicePrinterData.Rows[0].PrinterRole == 'Point of Sale (POS)' || vData.DevicePrinterData.Rows[0].PrinterRole == 'Sales Order Taker')
                                $('#divSumCategoriesList').hide();
                            else
                                $('#divSumCategoriesList').show();

                        }
                    }
                });
            }

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
                    pItem = $('<li>')
                        .append($('<h3>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].CategoryName)
                        );

                    pLV.append(pItem);
                };

                pLV.listview('refresh');
            }
        });
    });