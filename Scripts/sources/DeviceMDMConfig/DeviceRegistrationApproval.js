require([
    'common'
    ], function () {

        $(document).ready(function () {
            GetListOfConfigs();

            var paramDeviceID = getParameterByName("DeviceID");
            if (paramDeviceID.length > 0) {

                DisplayDetailsApproval(paramDeviceID);
                $.mobile.changePage('#dialog');
            }

        });


        function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
            return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }




        function GetListOfConfigs() {
            var pData = {};
            pData.KeyWord = $("#txtSearch").val();

            AjaxSend({
                url: 'Browser/LibraryDeviceApproval',
                data: pData,
                success: function (vData) {
                    DisplayDashboard(vData);
                }
            });
        }








        function DisplayDashboard(vData) {
            pLV = $('#lstApproval');
            pLV.empty();

            var vRows = vData.Results.Rows;

            if (vRows.length > 0) {
                for (var ctrDevices = 0; ctrDevices <= vRows.length - 1; ctrDevices++) {
                    var m_ID = vRows[ctrDevices].ID;
                    pItem = $('<li>').append($('<a>').attr('ID', vRows[ctrDevices].ID).click(function (e) {
                        e.preventDefault();
                        $.mobile.changePage('#dialog');
                        DisplayDetailsApproval($(this).attr('ID'));
                    }).append($('<h3>').attr("style", "white-space:normal").text(vRows[ctrDevices].PhoneName)).append($('<p>').attr("style", "white-space:normal").text(vRows[ctrDevices].UUID)));
                    pLV.append(pItem);
                }
                pLV.listview('refresh');
            }
        }


        function DisplayDetailsApproval(id) {
            var data = { ID: id };
            AjaxSend({
                url: 'DeviceApproval/getDeviceByID',
                error: function (data) {
                },
                data: data,
                success: function (vData) {
                    var Phonename = vData.DeviceList.Rows[0].PhoneName;
                    var PhoneModel = vData.DeviceList.Rows[0].PhoneModel;
                    var IMEI = vData.DeviceList.Rows[0].IMEI;
                    var UUID = vData.DeviceList.Rows[0].UUID;
                    var OS = vData.DeviceList.Rows[0].OS;
                    var DeviceNo = vData.DeviceList.Rows[0].DeviceNo;
                    var IDd = vData.DeviceList.Rows[0].ID;
                    $('#lblPhoneName').text(Phonename);
                    $('#lblPhoneModel').text(PhoneModel);
                    $('#lblIMEI').text(IMEI);
                    $('#lblUUID').text(UUID);
                    $('#lblOS').text(OS);
                    $('#lblDeviceNo').text(DeviceNo);


                    $('#btnApprove').click(function () {
                      
                        ApproveDeviceRequest(IDd);

                    });

                    $('#btnApproveBack').click(function () {
                        window.location = GetPathFormat("/DeviceDashboard?OrgID=" + sessionStorage.OrgID);
                    });

                }
            }); //end ajax
        }




        function ApproveDeviceRequest(id) {
            var data = { ID: id };
            AjaxSend({
                url: 'DeviceApproval/approveDeviceRegistrationRequestByID',
                error: function (data) {
                },
                data: data,
                success: function (vData) {     
                    MessageBox("Successfully approved.",
                           "Device Policy",
                            [
                                { Text: 'Ok'
                                }
                            ]);
                }
            }); //end ajax
        }



        function setRadio(id) {
            var radio = $('#' + id);
            radio[0].checked = true;
            radio.button("refresh");
        }


      


    });