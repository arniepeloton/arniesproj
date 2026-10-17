require([
    'common'
    ], function () {
        var pLV;
        $(document).ready(function () {

            EditDeviceApps();

            $('#btnAddWhiteListApp').click(function () {
                $.mobile.changePage('#pageDetails');
            });


            $('#btnAppsWhiteListBack').unbind().click(function () {
                $.mobile.changePage('#pageMain');
                GetListOfWhiteAppList();
            });


//            $('#btnAWLBack').unbind().click(function () {
//                window.location = GetPathFormat("/DeviceDashboard?OrgID=" + sessionStorage.OrgID);
//            });



            //if other module call
            var paramAPPID = getParameterByName("APPID");
            if (paramAPPID.length > 0) {
                DisplayInstalledtoWhiteListApp(paramAPPID);
            }
            else {
                GetListOfWhiteAppList();
            }


            var paramPPID = getParameterByName("PPID");
            if (paramPPID.length > 0) {
                $('#btnAppsWhiteListSpecificSave').show();
                $("#btnAppsWhiteListSpecificSave").attr('name', paramPPID);
            }
        });


        function GetListOfWhiteAppList() {
            var pData = {};

            AjaxSend({
                url: 'DeviceAppsWhiteList/getApplicationWhiteList',
                success: function (vData) {
                    DisplayAppList(vData);
                }
            });
        }





        function DisplayAppListToWhiteList(vData) {
            var vRows = vData.DeviceAppsWhiteList.Rows;
            $("#btnAppsWhiteListSave").attr('name', '');
            $('#txtApplicationName').val(vRows[0].ApplicationLabel);
            $('#txtApplicationDesc').val(vRows[0].ApplicationName);
            $('#txtKeywords').val(vRows[0].Directory);
        }






        function GetListOfWhiteAppListByID(id) {
            var pData = { ID: id };

            AjaxSend({
                url: 'DeviceAppsWhiteList/getApplicationWhiteListByID',
                data: pData,
                success: function (vData) {
                    var vRows = vData.DeviceAppsWhiteList.Rows;
                    $('#txtApplicationName').val(vRows[0].ApplicationLabel);
                    $('#txtApplicationDesc').val(vRows[0].ApplicationName);
                    $('#txtKeywords').val(vRows[0].Directory);

                }
            });
        }

        function DisplayInstalledtoWhiteListApp(paramAPPID) {
            var pData = { ID: paramAPPID };

            AjaxSend({
                url: 'DeviceAppsWhiteList/getApplicationByAppID',
                data: pData,
                success: function (vData) {
                    DisplayAppListToWhiteList(vData);
                }
            });

        }



        function DisplayAppList(vData) {
            pLV = $('#lstAppWhiteList');
            pLV.empty();






            var vRows = vData.DeviceAppsWhiteList.Rows;

            if (vRows.length > 0) {
                for (var ctrDevices = 0; ctrDevices <= vRows.length - 1; ctrDevices++) {
                    var m_ID = vRows[ctrDevices].ID;

                    $('#txtApplicationName').attr('name', vRows[ctrDevices].ApplicationName);
                    $('#txtApplicationDesc').attr('name', vRows[ctrDevices].Directory);
                    $('#txtKeywords').attr('name', vRows[ctrDevices].Keywords);

                    pItem = $('<li>').append($('<a>').attr('ID', vRows[ctrDevices].ID).attr('name', vRows[ctrDevices].ApplicationName).click(function (e) {
                        e.preventDefault();

                        var retrievedID = $(this).attr('ID');

                        GetListOfWhiteAppListByID(retrievedID);
                        $.mobile.changePage('#pageDetails');
                        $('#btnAppsWhiteListDelete').unbind().click(function () {
                            DeleteDeviceAppWhiteList(retrievedID);
                           
                        });

                    }).append($('<h3>').attr("style", "white-space:normal").text(vRows[ctrDevices].ApplicationLabel)).append($('<p>').attr("style", "white-space:normal").text(vRows[ctrDevices].ApplicationName)));
                    pLV.append(pItem);
                }
                pLV.listview('refresh');
            }
        }








        function DeleteDeviceAppWhiteList(id) {

            var data = { ID: id };
            AjaxSend({
                url: 'DeviceAppsWhiteList/deleteWhiteApplicationByID',
                error: function (data) {
                },
                data: data,
                success: function (vData) {
                    MessageBox("Successfully deleted.",
                            "Device Application WhiteList",
                            [
                                { Text: 'Ok' }
                            ]
                );
                    $('#txtApplicationName').val("");
                    $('#txtApplicationDesc').val("");
                    $('#txtKeywords').val("");
                    GetListOfWhiteAppList(); 
                }
            });
        }










        function UpdateDeviceAppWhiteList() {

            var data = { ID: $("#btnAppsWhiteListSave").attr('name'), DevicePolicyProfileID: $("#btnAppsWhiteListSpecificSave").attr('name'), ApplicationLabel: $("#txtApplicationName").val(), ApplicationName: $("#txtApplicationDesc").val(), Directory: $("#txtKeywords").val() };
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
                   
               
                }
            });
        }



        function EditDeviceApps() {
            // var status = false;
            $("#btnAppsWhiteListSave").unbind().click(function () {
                //     status = !status;
                UpdateDeviceAppWhiteList();
            });


            $("#btnAppsWhiteListSpecificSave").unbind().click(function () {
                //     status = !status;
                UpdateDeviceAppWhiteList();
            });
        }




        function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
            return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }



    });


