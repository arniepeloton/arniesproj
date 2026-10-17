require([
    'common'
    ], function () {
        var ID = '';
        var IsEdit = 0;
        var ApplicationControl = 0;
        $(document).ready(function () {
            GetListOfConfigs();


            $('#pageConfigurationItems').bind('pageshow', ShowConfigurationItems)

            $('#btnAddNewPolicy').unbind().click(function () {

                $("#btnConfigUpdate").attr('name', '');
                $.mobile.changePage('#pageConfigurationItems');
                $("#txtPolicyName").val('');
                $("#txtPolicyDescription").val('');
                $('#chkRoleIsActive').prop('checked', false);
                $('#chkRoleIsActive').checkboxradio('refresh');
                ClearPolicyFields();
                ClearWhiteList();
            });


            $('#btnView').unbind().click(function () {
                GetListOfConfigs();
                $.mobile.changePage('#pageMain');

            });


            $('#btnWhiteListApps').unbind().click(function () {
                $("#btnAppsWhiteListBack").attr('name', '');
                $.mobile.changePage('#pageWhiteList');
                ClearWhiteListFields();
            });



            $('#txtSearch').keypress(function (e) {
                if (e.keyCode == 13) {
                    SearchListOfPolicies();
                }
            });



            var paramPIID = getParameterByName("PPID");
            if (paramPIID.length > 0) {
                DisplayPolicyDetails(paramPIID);
                DisplayPolicyItems(paramPIID);
                DisplayWhiteListApps(paramPIID);
            }




            UpdatePolicyItemsHandler();
            UpdateWhiteListAppsItemsHandler();
        });

        function ShowConfigurationItems() {
            DisplayWhiteListApps(ID);
        }

        function GetListOfConfigs() {
            AjaxSend({
                url: 'Browser/LibraryDevicePolicyProfile',
                success: function (vData) {
                    DisplayPolicies(vData);
                }
            });
        }


        function SearchListOfPolicies() {
            var pData = { KeyWord: $("#txtSearch").val() };
            AjaxSend({
                url: "DeviceMDMConfig/searchPolicyProfileByKeyword",
                data: pData,
                success: function (vData) {
                    DisplayPolicies(vData)
                    $("#divRecordCount").text("No. of records: " + vData.Results.Rows.length);
                }
            });

        }



        function DisplayPolicies(vData) {
            pLV = $('#lstPolicy');
            pLV.empty();

            var vRows = vData.Results.Rows;

            if (vRows.length > 0) {
                for (var ctrDevices = 0; ctrDevices <= vRows.length - 1; ctrDevices++) {
                    var m_ID = vRows[ctrDevices].ID;
                    pItem = $('<li>').append($('<a>').attr('ID', vRows[ctrDevices].ID).attr('name', vRows[ctrDevices].DevicePolicyProfileName).click(function (e) {
                        e.preventDefault();
                        $.mobile.changePage('#pageConfigurationItems');
                        ClearPolicyFields();
                        $('#hWLA').text('Add Application to White List (' + $(this).attr('name') + ')');
                        ID = $(this).attr('ID');
                        DisplayPolicyDetails($(this).attr('ID'));
                        DisplayPolicyItems($(this).attr('ID'));
                        DisplayWhiteListApps($(this).attr('ID'));

                    }).append($('<h3>').attr("style", "white-space:normal").text(vRows[ctrDevices].DevicePolicyProfileName)).append($('<p>').attr("style", "white-space:normal").text(vRows[ctrDevices].DevicePolicyDescription)));
                    pLV.append(pItem);
                }

                try {
                    pLV.listview('refresh');
                }
                catch (e) {
                    pLV.listview();
                }

            }
        }






        function DisplayPolicyDetails(id) {
            var data = { ID: id };
            AjaxSend({
                url: 'DeviceMDMConfig/getPolicyDetailsByProfileID',
                error: function (data) {
                },
                data: data,
                success: function (vData) {
                    var vRows = vData.Results.Rows;

                    $('#txtPolicyName').val(vRows[0].DevicePolicyProfileName);
                    $('#txtPolicyDescription').val(vRows[0].DevicePolicyDescription);
                    if (vRows[0].IsActive == '1') {

                        $('#chkRoleIsActive').prop('checked', true);
                        $('#chkRoleIsActive').checkboxradio('refresh');
                    } else {
                        $('#chkRoleIsActive').prop('checked', true);
                        $('#chkRoleIsActive').checkboxradio('refresh');
                    }
                }
            });


        }



        function DisplayPolicyItems(id) {
            var data = { ID: id };
            $("#btnConfigUpdate").attr('name', id);
            $("#btnAppsWhiteListSave").attr('name', id);


            AjaxSend({
                url: 'DeviceMDMConfig/getPolicyItemsByPolicyProfileID',
                error: function (data) {
                },
                data: data,
                success: function (vData) {
                    var vRows = vData.Results.Rows;
                    if (vRows.length > 0) {
                        for (var i = 0; i <= vRows.length - 1; i++) {
                            switch (vRows[i].ConfigCode) {

                                case "ALS":
                                    if (vRows[i].ConfigValue.toLowerCase() == "yes") {
                                        $('#rcAutoLockScreenYes').prop('checked', true);
                                        $('#rcAutoLockScreenYes').checkboxradio('refresh');
                                    }
                                    else {
                                        $('#rcAutoLockScreenNo').prop('checked', true);
                                        $('#rcAutoLockScreenNo').checkboxradio('refresh');
                                    }
                                    break;
                                case "RP":
                                    if (vRows[i].ConfigValue.toLowerCase() == "yes") {
                                        $('#rcRequirePasswordYes').prop('checked', true);
                                        $('#rcRequirePasswordYes').checkboxradio('refresh');
                                    }
                                    else {
                                        $('#rcRequirePasswordNo').prop('checked', true);
                                        $('#rcRequirePasswordNo').checkboxradio('refresh');
                                    }
                                    break;
                                case "EHD":
                                    if (vRows[i].ConfigValue.toLowerCase() == "yes") {
                                        $('#rcEncryptDataStorageYes').prop('checked', true);
                                        $('#rcEncryptDataStorageYes').checkboxradio('refresh');
                                    }
                                    else {
                                        $('#rcEncryptDataStorageNo').prop('checked', true);
                                        $('#rcEncryptDataStorageNo').checkboxradio('refresh');
                                    }
                                    break;
                                case "ALSD":
                                    //                                    $('#selAutoLockAfterInactivity  option').filter(function () {
                                    //                                        return ($(this).val() == vRows[i].ConfigValue);
                                    //                                    }).prop('selected', true);
                                    $('#selAutoLockAfterInactivity ').val(vRows[i].ConfigValue);
                                    $('#selAutoLockAfterInactivity ').selectmenu('refresh');
                                    break;
                                case "PS":
                                    //                                    $('#selAutoLockAfterInactivity  option').filter(function () {
                                    //                                        return ($(this).val() == vRows[i].ConfigValue);
                                    //                                    }).prop('selected', true)
                                    $('#selPasswordStrength').val(vRows[i].ConfigValue);
                                    $('#selPasswordStrength').selectmenu('refresh');
                                    break;
                                case "RPR":
                                    //                                    alert(vRows[i].ConfigValue.toLowerCase());
                                    //                                    $('#selAutoLockAfterInactivity  option').filter(function () {
                                    //                                        return ($(this).val() == vRows[i].ConfigValue);
                                    //                                    }).prop('selected', true);

                                    $('#selPasswordResetEvery').val(vRows[i].ConfigValue);
                                    $('#selPasswordResetEvery').selectmenu('refresh');
                                    break;
                                case "WOHD":
                                    //                                    $('#selAutoLockAfterInactivity  option').filter(function () {
                                    //                                        return ($(this).val() == vRows[i].ConfigValue);
                                    //       
                                    $('#selWipeOutData').val(vRows[i].ConfigValue);
                                    $('#selWipeOutData').selectmenu('refresh');
                                    break;

                                case "FP":
                                    //                                    $('#selAutoLockAfterInactivity  option').filter(function () {
                                    //                                        return ($(this).val() == vRows[i].ConfigValue);
                                    //       
                                    $('#selNoDaysCertificateWillExpire').val(vRows[i].ConfigValue);
                                    $('#selNoDaysCertificateWillExpire').selectmenu('refresh');
                                    break;


                                default:
                                    //default for the sake  
                                    break;
                            } //end case
                        } //end for

                    } //end if
                } //function
            }); //end ajax
        }

        function UpdatePolicyItemsHandler() {

            $("#btnConfigUpdate").unbind().click(function () {
                var fixIID = $(this).attr('name');

                ValidateFields();
                if ($('#txtPolicyName').val() == '') {
                    return;
                }
                if ($('#txtPolicyDescription').val() == '') {
                    return;
                }
                UpdatePolicyDetailsBatch(fixIID);
            });
        }



        function UpdateWhiteListAppsItemsHandler() {
            $("#btnAppsWhiteListSave").unbind().click(function () {
                var fixIID = $(this).attr('name');
                ValidateFieldsWhiteListed();
                if ($.trim($('#txtApplicationTitle').val()) == '') {
                    return;
                }
                if ($.trim($('#txtApplicationName').val()) == '') {
                    return;
                }
                UpdateDeviceAppWhiteList(fixIID);
            });
        }




        function setRadio(id) {
            var radio = $('#' + id);
            radio[0].checked = true;
            radio.button("refresh");
        }





        function UpdatePolicyDetailsBatch(dppid) {
            var isActive = 'True';

            $('#chkRoleIsActive').is(':checked') ? isActive = 'True' : isActive = 'False';


            var data = { DevicePolicyProfileID: dppid,
                DevicePolicyDescription: $("#txtPolicyDescription").val(),
                DevicePolicyProfileName: $("#txtPolicyName").val(), IsActive: isActive
            };

            AjaxSend({
                url: 'DeviceMDMConfig/savePolicyProfile',
                error: function (data) {
                },
                data: data,
                success: function (vData) {
                    var vipID = vData.Results;
                    $.mobile.changePage('#pageConfigurationItems');
                    $('#headerPolicyName').val($('#txtPolicyName').text());
                    UpdatePolicyItemsBatch(vipID);
                }
            });
        }


        function UpdatePolicyItemsBatch(fixIID) {
            if ($('#rcAutoLockScreenYes').is(':checked')) {
                UpdatePolicyItemsIndividual('ALS', 'Yes', fixIID);
            }
            else {
                UpdatePolicyItemsIndividual('ALS', 'No', fixIID);
            }


            if ($('#rcRequirePasswordYes').is(':checked')) {
                UpdatePolicyItemsIndividual('RP', 'Yes', fixIID);
            } else {
                UpdatePolicyItemsIndividual('RP', 'No', fixIID);
            }


            if ($('#rcEncryptDataStorageYes').is(':checked')) {
                UpdatePolicyItemsIndividual('EHD', 'Yes', fixIID);
            }
            else {
                UpdatePolicyItemsIndividual('EHD', 'No', fixIID);
            }
            UpdatePolicyItemsIndividual('ALSD', $('#selAutoLockAfterInactivity ').val(), fixIID);
            UpdatePolicyItemsIndividual('PS', $('#selPasswordStrength').val(), fixIID);
            UpdatePolicyItemsIndividual('RPR', $('#selPasswordResetEvery').val(), fixIID);
            UpdatePolicyItemsIndividual('WOHD', $('#selWipeOutData').val(), fixIID);
            UpdatePolicyItemsIndividual('FP', $('#selNoDaysCertificateWillExpire').val(), fixIID);



            var paramPIID = getParameterByName("PPID")
            var paramDeviceID = getParameterByName("DeviceID")

            if (paramDeviceID.length > 0) {
                DevicePolicyAppointment(paramDeviceID, fixIID);
            }

            MessageBox("Successfully saved.",
                           "Device Policy",
                            [
                                { Text: 'Ok'
                                }
                            ]);

            GetListOfConfigs();
        }


        function UpdatePolicyItemsIndividual(vConfigCode, vConfigValue, vDevicePolicyProfileID) {

            var data = { ConfigCode: vConfigCode, ConfigValue: vConfigValue, DevicePolicyProfileID: vDevicePolicyProfileID };
            AjaxSend({
                url: 'DeviceMDMConfig/updatePolicies',
                error: function (data) {
                },
                data: data,
                success: function (vData) {
                    //   UpdatePolicyMostStrict(vConfigCode, vConfigValue, vDevicePolicyProfileID);
                }
            });
        }




        function DevicePolicyAppointment(paramDeviceID, paramPIID) {
            var data = { ID: paramDeviceID, DevicePolicyProfileID: paramPIID };

            AjaxSend({
                url: 'DevicePolicyAppoint/updateDevicePolicyAppointment',
                error: function () {
                },
                data: data,
                success: function () {

                } //function
            }); //end ajax
        }




        //        function UpdatePolicyMostStrict(vConfigCode, vConfigValue, vDevicePolicyProfileID) {

        //            var data = { ConfigCode: vConfigCode, ConfigValue: vConfigValue, DevicePolicyProfileID: vDevicePolicyProfileID };
        //            AjaxSend({
        //                url: 'DeviceMDMConfig/updatePolicies_MostStrict',
        //                error: function (data) {
        //                },
        //                data: data,
        //                success: function (vData) {
        //                }
        //            });
        //        }





        function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
            return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }



        //        function UpdateProfileNameHandler() {
        //            $('#btnUpdateProfile').unbind().click(function () {
        //                $.mobile.changePage('#pageConfigurationProfile');
        //                $('#txtPolicyName').val($('#headerPolicyName').text());
        //                $('#txtPolicyDesc').val($('#headerPolicyName').attr('name'));
        //                EditPolicyProfile($("#btnConfigUpdate").attr('name'));
        //            });

        //        }


        function ClearPolicyFields() {
            $('#rcAutoLockScreenYes').prop('checked', false);
            $('#rcAutoLockScreenYes').checkboxradio('refresh');
            $('#rcAutoLockScreenNo').prop('checked', false);
            $('#rcAutoLockScreenNo').checkboxradio('refresh');

            $('#rcRequirePasswordYes').prop('checked', false);
            $('#rcRequirePasswordYes').checkboxradio('refresh');
            $('#rcRequirePasswordNo').prop('checked', false);
            $('#rcRequirePasswordNo').checkboxradio('refresh');


            $('#rcEncryptDataStorageYes').prop('checked', false);
            $('#rcEncryptDataStorageYes').checkboxradio('refresh');
            $('#rcEncryptDataStorageNo').prop('checked', false);
            $('#rcEncryptDataStorageNo').checkboxradio('refresh');

            $('#selAutoLockAfterInactivity ').val("0");
            $('#selAutoLockAfterInactivity ').selectmenu('refresh');

            $('#selPasswordStrength').val("0");
            $('#selPasswordStrength').selectmenu('refresh');

            $('#selPasswordResetEvery').val("0");
            $('#selPasswordResetEvery').selectmenu('refresh');


            $('#selWipeOutData').val("0");
            $('#selWipeOutData').selectmenu('refresh');


            $('#selNoDaysCertificateWillExpire').val("0");
            $('#selNoDaysCertificateWillExpire').selectmenu('refresh');
        } // end of clear






        function DisplayWhiteListApps(mPPID) {
            var data = { DevicePolicyProfileID: mPPID };

            AjaxSend({
                url: 'DeviceMDMConfig/getApplicationWhiteListByPolicyProfileID',
                error: function () {
                },
                data: data,
                success: function (vData) {
                    var vRows = vData.Results.Rows;
                    var pLVW;
                    pLVW = $('#lstWhiteListedApps');
                    pLVW.empty();



                    if (vRows.length > 0) {
                        for (var ctrDevices = 0; ctrDevices <= vRows.length - 1; ctrDevices++) {
                            var m_ID = vRows[ctrDevices].ID;
                            pItem = $('<li data-icon="false">')
                                        .append($('<a>')
                                        .attr('ID', vRows[ctrDevices].ID)
                                        .attr('name', vRows[ctrDevices].ApplicationLabel)
                                        .click(function (e) {
                                            e.preventDefault();
                                            $.mobile.changePage('#pageWhiteList');
                                            GetListOfWhiteAppListByID($(this).attr('ID'));
                                        })
                                        .append($('<h3>').attr("style", "white-space:normal")
                                            .text(vRows[ctrDevices].ApplicationLabel)).append($('<p>').attr("style", "white-space:normal").text(vRows[ctrDevices].ApplicationName)));
                            pLVW.append(pItem);
                        }
                        pLVW.listview('refresh');
                    }


                } //function
            }); //end ajax
        } // end of clear



        //  getApplicationWhiteListByPolicyProfileID











        function ValidateFields() {
            if ($.trim($('#txtPolicyName').val()) == '') {
                $('#spanPolicyName').show();
                $('#txtPolicyName').addClass('form-validation-error');
            }
            else {
                $('#spanPolicyName').hide();
                $('#txtPolicyName').removeClass('form-validation-error');
            }


            if ($.trim($('#txtPolicyDescription').val()) == '') {
                $('#spanPolicyDescription').show();
                $('#txtPolicyDescription').addClass('form-validation-error');
            }
            else {
                $('#spanPolicyDescription').hide();
                $('#txtPolicyDescription').removeClass('form-validation-error');
            }
        }












        function GetListOfWhiteAppListByID(id) {
            $("#btnAppsWhiteListBack").attr('name', id);
            var pData = { ID: id };

            AjaxSend({
                url: 'DeviceAppsWhiteList/getApplicationWhiteListByID',
                data: pData,
                success: function (vData) {
                    var vRows = vData.DeviceAppsWhiteList.Rows;
                    $('#txtApplicationTitle').val(vRows[0].ApplicationLabel);
                    $('#txtApplicationName').val(vRows[0].ApplicationName);
                    $('#txtKeywords').val(vRows[0].Directory);

                }
            });
        }






        function ValidateFieldsWhiteListed() {
            if ($.trim($('#txtApplicationTitle').val()) == '') {
                $('#spanApplicationTitle').show();
                $('#txtApplicationTitle').addClass('form-validation-error');
            }
            else {
                $('#spanApplicationTitle').hide();
                $('#txtApplicationTitle').removeClass('form-validation-error');
            }


            if ($.trim($('#txtApplicationName').val()) == '') {
                $('#spanApplicationName').show();
                $('#txtApplicationName').addClass('form-validation-error');
            }
            else {
                $('#spanApplicationName').hide();
                $('#txtApplicationName').removeClass('form-validation-error');
            }

            // checkApplicationIfExistByPolicyProfileID

        }



        function UpdateDeviceAppWhiteList(mPPID) {
            var data = { ID: $("#btnAppsWhiteListBack").attr('name'), ApplicationLabel: $("#txtApplicationTitle").val(), ApplicationName: $("#txtApplicationName").val(), Directory: $("#txtKeywords").val(), DevicePolicyProfileID: mPPID };
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
                    DisplayWhiteListApps($('#btnAppsWhiteListSave').attr('name'));
                }
            });
        }




        function ClearWhiteList() {
            var pLV = $('#lstWhiteListedApps');
            pLV.empty();
            try {
                pLV.listview('refresh');
            }
            catch (e) {
                pLV.listview();
            }


        }

        function ClearWhiteListFields() {
            $('#txtApplicationTitle').val('');
            $('#txtApplicationName').val('');
            $('#txtKeywords').val('');
        }


        function CheckIfApplicationExist() {

            var data = { ApplicationLabel: ($.trim($('#txtApplicationTitle').val())), ApplicationName: ($.trim($('#txtApplicationName').val())), DevicePolicyProfileID: mPPID };
            AjaxSend({
                url: 'DeviceAppsWhiteList/checkApplicationIfExistByPolicyProfileID',
                error: function (data) {
                },
                data: data,
                success: function (vData) {
                    var vRows = vData.Results.Rows;
                    if (vRows.length > 0) {
                        return true;
                    } else {
                        return false;
                    }
                }
            });
        }


    });


