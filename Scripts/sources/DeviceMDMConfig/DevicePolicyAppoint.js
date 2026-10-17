require([
    'common'
    ], function () {

        $(document).ready(function () {
            var paramDeviceID = getParameterByName("DeviceID");
            DisplayPolicyChoices();
            if (paramDeviceID.length > 0) {
                SelectPolicyFromChoices(paramDeviceID);
            }

            $("#btnSaveDevicePolicyAppointment").click(function () {
                var selValue = $("#selPolicyAppoint").val()

                if (paramDeviceID.length > 0) {
                    UpdatePolicyWithSelectedChoice(selValue, paramDeviceID);
                }
            });


            $("#btnBackDevicePolicyAppointment").click(function () {
                    window.location = GetPathFormat("/DeviceDashboard?OrgID=" + sessionStorage.OrgID);
            });

            


        });


        //        function GetListOfActiveDevices() {
        //            var pData = {};
        //            pData.KeyWord = $("#txtSearch").val();

        //            AjaxSend({
        //                url: 'Browser/LibraryDevicePolicyAppoint',
        //                data: pData,
        //                success: function (vData) {
        //                    DisplayDevices(vData);
        //                }
        //            });
        //        }

        //        function DisplayDevices(vData) {
        //            pLV = $('#lstDevicePolicyAppoint');
        //            pLV.empty();

        //            var vRows = vData.Results.Rows;

        //            if (vRows.length > 0) {
        //                for (var ctrDevices = 0; ctrDevices <= vRows.length - 1; ctrDevices++) {
        //                    var m_ID = vRows[ctrDevices].ID;
        //                    pItem = $('<li>').append($('<a>').attr("name", vRows[ctrDevices].PhoneName).attr('ID', vRows[ctrDevices].ID).click(function (e) {
        //                        e.preventDefault();
        //                        $.mobile.changePage('#pagePolicyChoices');
        //                        $("#headPolicyDevice").text($(this).attr('name'));
        //                        SelectPolicyFromChoices($(this).attr('ID'));
        //                        //                        DisplayPolicyChoices($(this).attr('ID'));
        //                    }).append($('<h3>').attr("style", "white-space:normal").text(vRows[ctrDevices].PhoneName)).append($('<p>').attr("style", "white-space:normal").text(vRows[ctrDevices].UUID)));
        //                    pLV.append(pItem);
        //                }
        //                pLV.listview('refresh');
        //            }
        //        }


        function DisplayPolicyChoices() {
            AjaxSend({
                url: 'DevicePolicyAppoint/getPolicyProfile',
                error: function () {
                },
                success: function (vData) {

                    var vRows = vData.Results.Rows;


                    for (var ctrDevices = 0; ctrDevices <= vRows.length - 1; ctrDevices++) {
                        var o = new Option(vRows[ctrDevices].ID, vRows[ctrDevices].ID);
                        /// jquerify the DOM object 'o' so we can use the html method
                        $(o).html(vRows[ctrDevices].DevicePolicyProfileName);
                        $("#selPolicyAppoint").append(o);
                    }

                } //function
            }); //end ajax  
        }



        function SelectPolicyFromChoices(id) {
            var data = { ID: id };

            AjaxSend({
                url: 'DevicePolicyAppoint/getPolicyProfileIDByID',
                error: function () {
                },
                data: data,
                success: function (vData) {

                    var vDevicePolicyID = vData.Results;
                    $('#selPolicyAppoint').val(vDevicePolicyID);

                    $("#btnSaveDevicePolicyAppointment").attr('name', id);
                    $('#selPolicyAppoint').selectmenu('refresh');

                } //function
            }); //end ajax
        }



        function UpdatePolicyWithSelectedChoice(devicePolicyProfileID, id) {
            var data = { DevicePolicyProfileID: devicePolicyProfileID, ID: id };

            AjaxSend({
                url: 'DevicePolicyAppoint/updateDevicePolicyAppointment',
                error: function () {
                },
                data: data,
                success: function () {
                    MessageBox("Successfully saved.",
                           "Device Policy",
                            [
                                { Text: 'Ok'
                                }
                            ]);
                } //function
            }); //end ajax
        }


        function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
            return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }

    });