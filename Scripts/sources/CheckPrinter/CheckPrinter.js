require([
    'common'
    ], function () {
        //CREATED BY Janine Aug 24, 2013 1pm
        require([(OfflineMode ? '/Scripts/sources/People/PeopleMenu_offline.js?crc=' : ''), (OfflineMode ? '/Scripts/sources/CheckPrinter/CheckPrinter_offline.js?crc=' : '')
    ], function () {
        $(function () {
            getOrgFowstPrinter();

            $('#btnContinue').click(function (e) {

                e.preventDefault();

                window.location = '/People/PeopleMenu?IsMobile=1';
            });


            $('#btnDiagnose').click(function (e) {

                e.preventDefault();

                $.mobile.changePage('#pageDiagnose');
            });

            function getOrgFowstPrinter() {
                if (OfflineMode) {
                    var vData = PeopleMenu_offline().getPeopleOrganizationList();


                    if (vData != undefined) {

                        for (var p = 0; p <= vData.Rows.length - 1; p++) {

                            var vDataIP = CheckPrinter_offline().getPrinterFowstIP(vData.Rows[p].OrganizationID);

                            if (vDataIP != undefined) {
                                var pData = [];
                                var intIndex = 0;

                                if (vDataIP.Rows.length < 1) {

                                    $('#divWait').hide();
                                    $('#btnContinue').show();
                                } else {

                                    for (var j = 0; j <= vDataIP.Rows.length - 1; j++) {

                                        pData[intIndex] = {
                                            PrinterName: vDataIP.Rows[j].PrinterName,
                                            PrinterIPAddress: vDataIP.Rows[j].PrinterIPAddress,
                                            PrinterType: vDataIP.Rows[j].PrinterType,
                                            PrinterRole: vDataIP.Rows[j].PrinterRole,
                                            CompanyName: vDataIP.Rows[j].CompanyName,
                                            FowstName: vDataIP.Rows[j].FowstName
                                        };

                                        intIndex = intIndex + 1;


                                        if (j == vDataIP.Rows.length - 1) {
                                            $('#divWait').hide();
                                            $('#btnContinue').show();

                                            if (pData.length > 0)
                                                PingPrinterIPAddress(pData);
                                        } //end if j

                                    } //end for
                                }// end if < 1


                            } //end if vdata
                        } //end for


                    } else {
                        MessageBox('You should be a member of an organization/company in order to use this program');
                        return;
                    }
                }
                else {
                    //do nothing. this page is only for offline anyway
                }
            }



            function PingPrinterIPAddress(vRows) {

                for (var p = 0; p <= vRows.length - 1; p++) {

                    var connectedIP = [];
                    var disconnectedIP = [];
                    AjaxSend({
                        type: 'GET',
                        url: 'http://' + vRows[p].PrinterIPAddress,
                        success: function () {

                            connectedIP[p - 1] = {
                                PrinterName: vRows[p - 1].PrinterName,
                                PrinterIPAddress: vRows[p - 1].PrinterIPAddress,
                                PrinterType: vRows[p - 1].PrinterType,
                                PrinterRole: vRows[p - 1].PrinterRole,
                                CompanyName: vRows[p - 1].CompanyName,
                                FowstName: vRows[p - 1].FowstName
                            };

                            if (p == vRows.length) {

                                if (connectedIP.length > 0) {
                                    DisplayPrintersConnected(connectedIP);
                                    if (disconnectedIP.length < 1)
                                        $('#divConnDisplay').show();
                                }

                                if (disconnectedIP.length > 0) {
                                    $('#divDisconDisplay').show();
                                    DisplayPrintersDisconnected(disconnectedIP);
                                }

                            } //end if j


                        },
                        error: function () {

                            disconnectedIP[p - 1] = {
                                PrinterName: vRows[p - 1].PrinterName,
                                PrinterIPAddress: vRows[p - 1].PrinterIPAddress,
                                PrinterType: vRows[p - 1].PrinterType,
                                PrinterRole: vRows[p - 1].PrinterRole,
                                CompanyName: vRows[p - 1].CompanyName,
                                FowstName: vRows[p - 1].FowstName
                            };

                            if (p == vRows.length) {

                                if (connectedIP.length > 0) {
                                    DisplayPrintersConnected(connectedIP);
                                    if (disconnectedIP.length < 1)
                                        $('#divConnDisplay').show();
                                }

                                if (disconnectedIP.length > 0) {
                                    $('#divDisconDisplay').show();
                                    DisplayPrintersDisconnected(disconnectedIP);
                                }



                            } //end if j

                        }
                    });



                }



            }


            function DisplayPrintersConnected(vRows) {
                var pLV = $('#lvDevicePrinterConnected');
                var pItem;

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li>')
                        .append($('<h3>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].PrinterName)
                        )
                        .append($('<h3>')
                            .attr("style", "white-space:normal")
                            .text('IP Address: ' + vRows[p].PrinterIPAddress)
                        )
                        .append($('<p>')
                            .text('Printer Type: ' + vRows[p].PrinterType)
                        )
                        .append($('<p>')
                            .text('Printer Role: ' + vRows[p].PrinterRole)
                        )
                        .append($('<p>')
                            .text('Organization: ' + vRows[p].CompanyName)
                        )
                        .append($('<p>')
                            .text('FOWST: ' + vRows[p].FowstName)
                        );

                    pLV.append(pItem);
                };

                pLV.listview('refresh');
            };

            function DisplayPrintersDisconnected(vRows) {

                var pLV = $('#lvDevicePrinterDisconnected');
                var pItem;

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li>')
                        .append($('<h3>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].PrinterName)
                        )
                        .append($('<h3>')
                            .attr("style", "white-space:normal")
                            .text('IP Address: ' + vRows[p].PrinterIPAddress)
                        )
                        .append($('<p>')
                            .text('Printer Type: ' + vRows[p].PrinterType)
                        )
                        .append($('<p>')
                            .text('Printer Role: ' + vRows[p].PrinterRole)
                        )
                        .append($('<p>')
                            .text('Organization: ' + vRows[p].CompanyName)
                        )
                        .append($('<p>')
                            .text('FOWST: ' + vRows[p].FowstName)
                        );

                    pLV.append(pItem);
                };

                pLV.listview('refresh');
            };

        }); //End Inner function
    }); //End Inner function

    });


