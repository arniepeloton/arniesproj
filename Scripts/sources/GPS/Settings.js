
require([
    'common'
    ], function () {
        var centerpoint;
        var marker;
        var map;
        var circle;
        
        $(document).ready(function () {

            GetGPSSettings();

            window.GPSSettings = {
                Load: GetGPSSettings
            }

            $('body')
            .on('click', '#butSaveGPSSettings', function () {
                SaveGPSSettings();
            })
            .on('click', '#btnGPSSettingsBack', function () {
                window.location.hash = 'SystemAdministration';
            })
        });

        function SaveGPSSettings() {
            var data = {
                IsRequireGPS: $('#selIsRequireGPS').is(':checked'),
                GPSInterval: $('#txtGPSInterval').val()
            }

            AjaxSend({
                url: '/GPS/SaveGPSSettings',
                data: data,
                success: function (vData) {
                    if (vData.Results = "True") {
                        WebToast('Successfully saved', 'top');
                        history.back();
                    } else {
                        MessageBox("Something went wrong while saving settings. Please contact Mobiz ASAP.",
                            "Settings",
                                [
                                    { Text: 'Ok' }
                                ]
                            );
                    }
                },
                error: function (vError) {

                }
            });
        }

        function GetGPSSettings() {
            AjaxSend({
                url: '/GPS/GetGPSSettings',
                success: function (vData) {
                    var vRows = vData.Results.Rows;

                    if (vRows.length > 0) {
                        if(vRows[0].Description=='IsRequireGPS'){
                            try{
                                $('#selIsRequireGPS').prop('checked', CBool(vRows[0].DefaultValue));
                                $('#txtGPSInterval').val(vRows[1].DefaultValue);
                            }catch(e){

                            }
                        }else{
                            try{
                                $('#selIsRequireGPS').prop('checked', CBool(vRows[1].DefaultValue));
                                $('#txtGPSInterval').val(vRows[0].DefaultValue);
                            }catch(e){

                            }
                        }

                        try {
                            //$('#selIsRequireGPS').slider('refresh');
                        } catch (e) {
                            //$('#selIsRequireGPS').slider();
                        }
                    }
                },
                error: function (vError) {

                }
            });
        }
    });