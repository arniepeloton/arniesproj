require([
    'common'
    ], function () {

        $(function () {

            ShowDevicePrinter();

            //            $('#frmDevicePrinterBrowser').submit(function (e) {
            //                e.preventDefault();

            //                ShowDevicePrinter();

            //            });

            $('body').on('keypress', '#txtDevicePrinterSearch', function (e) {
                if (e.keyCode == 13 || $('#txtDevicePrinterSearch').val() == "") {
                    ShowDevicePrinter();
                }
            })

            $('#btnAddNewDevicePrinter').click(function () {

                CreateNewDevicePrinterEntry(1, '');

            });

            $('#btnDevicePrinterBackToHome').click(function () {

                window.open(GetPathFormat("/People/PeopleMenu"), "_self");

            });

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
            };

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





        });
    });