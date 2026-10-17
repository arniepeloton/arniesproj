require(['common'], function () {

    var mDeviceID;

    $('[name=radWebPosMachineType]').click(function() {
        var pValue = $('[name=radWebPosMachineType]:checked').val();
        HideDivs();
        $('[name=divWebPosMachineType' + pValue).show();
    });

    function HideDivs() {
        $('[name=divWebPosMachineType1').hide();
        $('[name=divWebPosMachineType2').hide();
        $('[name=divWebPosMachineType3').hide();
    }

    $('#butWebPosSettingSave').click(function() {
        var pData = {};
        pData.MachineType = $('[name=radWebPosMachineType]:checked').val();
        if (pData.MachineType == 2)
            pData.ExternalServerIp = $('#txtWebPosSettingExternalIp2').val();
        else if (pData.MachineType == 3)
            pData.ExternalServerIp = $('#txtWebPosSettingExternalIp3').val();
        pData.DeviceID = mDeviceID;
        pData.AfterSaving = $('[name=radWebPosAfterSaving]:checked').val();;
        AjaxSend({
            url: '/WebPosDbDownloader/SetWebPosSettings',
            data: pData,
            success: function(vData) {
                if (vData.Result == 'ok')
                    NewMessageBox('Settings were saved successfully.', 'Saved');
                else if (vData.Error)
                    NewMessageBox(vData.Error, 'Failed To Save');
            }
        });
    });

    $('#butDevice').click(function() {
        BrowsePopup('LibraryDevice', "Devices",
            [
                {
                    // Search: mTxtCustomer.val(), // commented to prevent auto search upon popup
                    DisplayText: [
                        {
                            Column: 'PhoneName',
                            Label: 'Phone Name'
                        },
                        {
                            Column: 'PhoneModel',
                            Label: 'Phone Model'
                        },
                        {
                            Column: 'IMEI',
                            Label: 'IMEI'
                        },
                        {
                            Column: 'UUID',
                            Label: 'UUID'
                        },
                        {
                            Column: 'DeviceNo',
                            Label: 'Device No.'
                        },
                        {
                            Column: 'LastUpdated',
                            Label: 'Last Updated'
                        }
                    ],
                    StyleFormat: 'table',
                    Click: function (e) {
                        Bs.Loading('show');
                        mDeviceID = e.ID;
                        $('#txtDevice').val(e.UUID);
                        Bs.Loading('hide');
                    }
                }
            ], null // pOptions
        ); // END BrowsePopup('LibraryFowst', 'Fowst',
    });

    function LoadSettings() {
        AjaxSend({
            url: '/WebPosDbDownloader/GetWebPosSettings',
            success: function(vData) {
                if (vData.Error) {
                    NewMessageBox(vData.Error);
                    return;
                }
                var pRadio = $('[name=radWebPosMachineType][value=' + CInt2(vData.MachineType) + ']');
                pRadio.prop('checked', true);
                pRadio.click();
                $('#txtWebPosSettingExternalIp' + CInt2(vData.MachineType)).val(vData.ExternalServerIp);
                mDeviceID = vData.DeviceID;
                $('#txtDevice').val(vData.UUID);

                pRadio = $('[name=radWebPosAfterSaving][value=' + CInt2(vData.AfterSaving) + ']');
                pRadio.prop('checked', true);
            }
        });
    }

    LoadSettings();
    HideDivs();
});