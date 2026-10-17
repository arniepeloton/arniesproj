require([
    'common'
    ], function () {

        $(function () {
            GetListOfDevices();

            $('div[data-role="dialog"]').live('pagebeforeshow', function (e, ui) {
                // make parent page transparent
                ui.prevPage.addClass("ui-dialog-background ");
            });

            $('div[data-role="dialog"]').live('pagehide', function (e, ui) {
                $(".ui-dialog-background ").removeClass("ui-dialog-background ");
            });

            $('#txtSearch').keypress(function (e) {
                if (e.keyCode == 13) {
                    SearchDashboard();
                }
            });


            $('#btnRefreshDashboard').unbind().click(function () {
                GetListOfDevices();
                $.mobile.changePage('#pageMain');

            });
        }); //End Inner function

        function SearchDashboard() {

            AjaxSend({
                url: "DeviceDashboard/getDeviceSearchResult",
                data: { 'Keywords': $("#txtSearch").val() },
                success: function (vData) {
                    DisplayDashboard(vData.DashboardSearchResults.Rows)
                    $("#divRecordCount").text("No. of records: " + vData.DashboardSearchResults.Rows.length);
                }
            });

        }

        function GetListOfDevices() {

            AjaxSend({
                url: 'DeviceDashboard/getDeviceList',
                success: function (vData) {
                    DisplayDashboard(vData.DashboardSearchResults.Rows)
                }
            });
        }

        function DisplayDetails(id) {
            var data = { ID: id }

            AjaxSend({
                url: 'Dashboard/getDetails',
                data: data,
                success: function (vData) {

                }
            });
        }




        function DisplayDashboard(vRows) {
            var pLV = $('#tblDashboardBody');
            var devID;
            pLV.html('');



            for (var i = 0; i <= vRows.length - 1; i++) {
                var pImage;
                var Status;
                if (vRows[i].IsActive == '0') {
                    Status = 'InActive'
                } else {
                    Status = 'Active'
                }



                if (vRows[i].IsCertificateValid == '0') {
                    pImage = '<img id="imgH" height="15px" src="/Content/images/icons/alert.png" >'

                } else {
                    pImage = '';
                }

                pItem = $('<tr>')
                            .append($('<td id="rowid">').attr('name', vRows[i].ID)
                                 .append($('<a href="">').click(function (e) {
                                     e.preventDefault();

                                     devID = $(this).closest('#rowid').attr('name');
                                     $.mobile.changePage('#pageCertificateReport');
                                     DisplayCertificateLog(devID);
                                     $(this).val('0');
                                     $('#txtSearchCertificateLog').unbind().keypress(function (e) {
                                         if (e.keyCode == 13) {
                                             DisplayCertificateLog(devID);
                                         }
                                     });
                                 })
                                 .append(pImage))

                            )
                            .append($('<th>')
                                .text(vRows[i].PhoneName)
                            )
                            .append($('<td>')
                                .text(vRows[i].UUID)
                            )
                            .append($('<td>')
                                .text(vRows[i].IMEI)
                            )
                //                            .append($('<td>')
                //                                .text(vRows[i].OS)
                //                            )
                //                            .append($('<td>')
                //                                .text(vRows[i].DateEnrolled)
                //                            )
                //                            .append($('<td>')
                //                                .text(vRows[i].UserFullName)
                //                            )
                            .append($('<td id="statAc">')
                                .text(Status)
                            )
                //                            .append($('<td>')
                //                                .text(vRows[i].DevicePolicyLastUpdate)
                //                            )
                            .append($('<td>')
                             .append($(' <div id="divDet" data-role="fieldcontain">').attr('name', vRows[i].ID)
                             .append($(' <select  id="' + vRows[i].ID + '">').attr('name', vRows[i].DevicePolicyProfileID).change(function () {
                                 if ($(this).val() == '1') {
                                     $.mobile.changePage('#pageWipeDetails');
                                     devID = $(this).closest('#divDet').attr('name');
                                     ShowDeviceDetailsForW(devID);
                                     $('#btnWipeOut').unbind().click(function () {
                                         if (confirm('You are sure you want to wipe the storage of this device?')) {

                                          
                                             if (TidyString($('#txtDeviceNameToWipe').val()) == TidyString($('#lblDeviceNameW').text())) {
                                                 $('#spanDeviceNameToWipe').hide();
                                                 SendWipeOutSignal(devID);
                                             } else {
                                                 $('#spanDeviceNameToWipe').show();
                                             }

                                         } else {
                                             // Do nothing!
                                         }
                                     });
                                     $(this).val('0');
                                 }
                                 if ($(this).val() == '2') {
                                     devID = $(this).closest('#divDet').attr('name');
                                     window.location = GetPathFormat("/GPS?DeviceID=" + devID);
                                     $(this).val('0');
                                 }

                                 if ($(this).val() == '3') {

                                     devID = $(this).closest('#divDet').attr('name');

                                     window.location = GetPathFormat("/DeviceMDMConfig?OrgID=" + sessionStorage.OrgID + "&PPID=" + $(this).attr('name') + "&DeviceID=" + devID + "#pageConfigurationItems");
                                     $(this).val('0');
                                 }
                                 if ($(this).val() == '4') {
                                     DisplayApplicationList($(this).find("option[value='4']").attr('name'));
                                     //   $('#pAppName').attr('name', $(this).attr('name'));
                                     devID = $(this).closest('#divDet').attr('name');
                                     $.mobile.changePage('#pageApplication');
                                     ShowDeviceDetails(devID);
                                     $(this).val('0');

                                     //about refreshing lang
                                     $('#btnWAppRefresh').attr('name', devID);
                                     $('#btnWAppRefresh').unbind().click(function () {
                                         DisplayApplicationList($('#btnWAppRefresh').attr('name'));
                                     });


                                 }

                                 if ($(this).val() == '5') {
                                     // InitializeMap();
                                     devID = $(this).closest('#divDet').attr('name');
                                     window.location = GetPathFormat("/DevicePolicyAppoint?OrgID=" + sessionStorage.OrgID + "&DeviceID=" + devID);
                                     $(this).val('0');

                                 }
                                 if ($(this).val() == '6') {
                                     // InitializeMap();
                                     devID = $(this).closest('#divDet').attr('name');
                                     window.location = GetPathFormat("/DeviceApproval?OrgID=" + sessionStorage.OrgID + "&DeviceID=" + devID);
                                     $(this).val('0');

                                 }
                                 if ($(this).val() == '7') {
                                     // InitializeMap();
                                     devID = $(this).closest('#divDet').attr('name');
                                     $.mobile.changePage('#pageCertificateReport');
                                     DisplayCertificateLogAll(devID);
                                     $(this).val('0');
                                     $('#txtSearchCertificateLog').unbind().keypress(function (e) {
                                         if (e.keyCode == 13) {
                                             DisplayCertificateLogAll(devID);
                                         }
                                     });
                                 }
                             })
                                  .append($('<option value="0">').text('Select Action'))
                                  .append($('<option value="1">').text('Wipe Out Device Storage'))
                                  .append($('<option value="2">').text('Find Device'))
                                  .append($('<option value="3">').text('View Policy Details'))
                                  .append($('<option value="4">').attr('name', vRows[i].ID).text('View Installed Apps'))
                                  .append($('<option value="5">').text('Change MDM Policy'))
                                  .append($('<option value="6" id="opApprove">').text('Approve Registration'))
                                  .append($('<option value="7">').text('View Log'))
                                  )));
                //                var varActive = $(this).closest('#statAc').text();
                //   
                //                if (varActive == 'Active') {
                //                    $('#opApprove').hide();
                //                }

                pLV.append(pItem);
            }

            $('#tblDashboard').table('refresh');

        }



        function InitializeMap() {
            var latlng = new google.maps.LatLng(-34.397, 150.644);
            var myOptions = {
                zoom: 8,
                center: latlng,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            };
            var map = new google.maps.Map(document.getElementById("divMap"), myOptions);
        }





        function ShowApplicationList(vRows) {
            var pLV = $('#divListApps');
            var pIcon;
            pLV.html('');
            $('#divNotif').show();





            if (vRows.length > 0) {
                $('#divNotif').hide();
                for (var i = 0; i <= vRows.length - 1; i++) {

                    if (vRows[i].Found == '1') {
                        pIcon = '<img class="ui-li-icon ui-li-thumb" id="imgH" src="/Content/images/icons/whitelist2.png" alt="This application is already in the Whitelist">';
                    } else {
                        pIcon = '';
                    }

                    pItem = ($('<li data-icon="add">').append($('<a>').attr('ID', vRows[i].ID).text(vRows[i].ApplicationLabel)
                     .click(function (e) {
                         GetApplicationListByID($(this).attr('ID'));
                         e.preventDefault();


                         $("#btnAppsWhiteListSave").unbind().click(function () {
                             CheckApplicationIfExistByPolicyProfileID('')
                         });

                         $("#btnAppsWhiteListSpecificSave").unbind().click(function () {
                             CheckApplicationIfExistByPolicyProfileID($('#lblCurrentPolicy').attr('name')) //if application is already in whitelist dont save it anymore
                         });

                         $.mobile.changePage('#pageAddWhiteList');
                     }).append(pIcon).append($('<p id="pAppName">').attr("style", " style='padding-top: 5px; margin-top: 5px;width: 10px; height: 10px;'").text(vRows[i].ApplicationName)))
              );




                    pLV.append(pItem);

                }
            }
            pLV.listview('refresh');
        }



        function GetApplicationListByID(id) {
            var data = { ID: id }

            AjaxSend({
                url: 'DeviceDashboard/getDeviceAppsListByID',
                data: data,
                success: function (vData) {

                    ShowToTextBox(vData);
                }
            });
        }


        function ShowToTextBox(vData) {
            var vRows = vData.DeviceAppsList.Rows;

            $('#txtApplicationName').val(vRows[0].ApplicationLabel);
            $('#txtApplicationDesc').val(vRows[0].ApplicationName);
            $('#txtKeywords').val(vRows[0].Directory);
        }


        function ShowDeviceDetails(DeviceID) {
            var data = { ID: DeviceID }

            AjaxSend({
                url: 'DeviceDashboard/getDeviceListByID',
                data: data,
                success: function (vData) {
                    var vRow = vData.Results.Rows;
                    if (vRow) {
                        $('#lblDeviceName').text(vRow[0].PhoneName);
                        $('#lblUUID').text(vRow[0].UUID);
                        $('#lblIMEI').text(vRow[0].IMEI);
                        $('#lblCurrentPolicy').attr('name', vRow[0].DevicePolicyProfileID);
                        $('#lblCurrentPolicy').text(vRow[0].DevicePolicyProfileName);
                    }
                }
            });
        }

        function ShowDeviceDetailsForW(DeviceID) {
            var data = { ID: DeviceID }

            AjaxSend({
                url: 'DeviceDashboard/getDeviceListByID',
                data: data,
                success: function (vData) {
                    var vRow = vData.Results.Rows;
                    if (vRow) {
                        $('#lblDeviceNameW').text(vRow[0].PhoneName);
                        $('#lblUUIDW').text(vRow[0].UUID);
                        $('#lblIMEIW').text(vRow[0].IMEI);
                        $('#lblOSW').text(vRow[0].OS);
                    }
                }
            });
        }



        function DisplayApplicationList(id) {
            var data = { ID: id }

            AjaxSend({
                url: 'DeviceDashboard/getDeviceAppsList',
                data: data,
                success: function (vData) {
                    ShowApplicationList(vData.DeviceAppsList.Rows);
                }
            });
        }



        function DisplayCertificateLog(devID) {
            $('#txtSearchCertificateLog').attr('name', devID);
            var data = { KeyWord: $('#txtSearchCertificateLog').val(), DeviceID: devID }
            AjaxSend({
                url: 'DeviceMDMConfig/getCertificateLog',
                data: data,
                success: function (vData) {
                    ShowCertificateLog(vData.Results.Rows);
                }
            });
        }



        function ShowCertificateLog(vRows) {


            var pLV = $('#tblCertBody');

            pLV.html('');



            for (var i = 0; i <= vRows.length - 1; i++) {
                pItem = $('<tr>')
                            .append($('<th>')
                                .text(vRows[i].DateReported)
                            )
                            .append($('<td>')
                                .text(vRows[i].ReasonForInvalidation)
                            )
                //                            .append($('<td>')
                //                                .text(vRows[i].UserFullName)
                //                            )
                //                            .append($('<td>')
                //                                .text(vRows[i].OrgName)
                //                            )



                pLV.append(pItem)

            }

            $('#tblCert').table('refresh');
        }


        function ClearCertificateLog() {
            var pLV = $('#tblCertBody');
            pLV.html('');
            $('#tblCert').table('refresh');
        }






        function DisplayCertificateLogAll(devID) {

            $('#txtSearchCertificateLog').attr('name', devID);
            var data = { KeyWord: $('#txtSearchCertificateLog').val(), DeviceID: devID }
            AjaxSend({
                url: 'DeviceMDMConfig/getCertificateLogAll',
                data: data,
                success: function (vData) {
                    ShowCertificateLogAll(vData.Results.Rows);
                }
            });
        }



        function ShowCertificateLogAll(vRows) {


            var pLV = $('#tblCertBody');

            pLV.html('');



            for (var i = 0; i <= vRows.length - 1; i++) {
                pItem = $('<tr>')
                            .append($('<th>')
                                .text(vRows[i].DateReported)
                            )
                            .append($('<td>')
                                .text(vRows[i].ReasonForInvalidation)
                            )
                //                            .append($('<td>')
                //                                .text(vRows[i].UserFullName)
                //                            )
                //                            .append($('<td>')
                //                                .text(vRows[i].OrgName)
                //                            )



                pLV.append(pItem)

            }

            $('#tblCert').table('refresh');
        }



        function ClearCertificateLogAll() {
            var pLV = $('#tblCertBody');
            pLV.html('');
            $('#tblCert').table('refresh');
        }







        //WAL
        function UpdateDeviceAppWhiteList(PPID) {

            var data = { ID: '', DevicePolicyProfileID: PPID, ApplicationLabel: $("#txtApplicationName").val(), ApplicationName: $("#txtApplicationDesc").val(), Directory: $("#txtKeywords").val() };
            AjaxSend({
                url: 'DeviceAppsWhiteList/saveApplicationWhiteList',
                error: function (data) {
                },
                data: data,
                success: function (vData) {
                    MessageBox("Successfully saved.",
                            "Device Application WhiteList",
                            [
                                { Text: 'Ok' }
                            ]
                );
                    $('#btnAWLBack').unbind().click(function () {
                        DisplayApplicationList($('#btnWAppRefresh').attr('name'));

                    });

                }
            });
        }




        //WAL
        function CheckApplicationIfExistByPolicyProfileID(PPID) {

            var data = { ID: '', DevicePolicyProfileID: PPID, ApplicationLabel: $("#txtApplicationName").val(), ApplicationName: $("#txtApplicationDesc").val() };
            AjaxSend({
                url: 'DeviceDashboard/checkApplicationIfExistByPolicyProfileID',
                error: function (data) {
                },
                data: data,
                success: function (vData) {
                    var answer = vData.Results;
                    if (answer.toLowerCase() == "false") { //if not duplicate
                        UpdateDeviceAppWhiteList(PPID);
                    }
                    else {
                        MessageBox("Not saved. The Application is already whitelisted.",
                            "Device Application WhiteList.",
                            [
                                { Text: 'Ok' }
                            ]
                          );
                        return false;
                    }
                }
            });
        }



        function SendWipeOutSignal(devID) {

            var data = { DeviceID: devID };
            AjaxSend({
                url: 'DeviceDashboard/sendWipeOutSignal',
                error: function (data) {
                },
                data: data,
                success: function (vData) {
                    MessageBox("Device will be wipe out.",
                            "Device Storage Wipe Out",
                            [
                                { Text: 'Ok' }
                            ]
                          );

                }
            });
        }






        function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
            return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }


        function TidyString(text) {
            return text.toLowerCase().trim();
        }




    });


