require([
    'common',
    '/Scripts3/Browser/LibraryPropertyAndPlant.js'
    ], function () {

        $(function () {

            $(document).ready(function () {

                $("#btnAddPlaces > span").attr("style", "text-align:left;white-space:normal");

                getDevicePlaceList();
            });

            $('#btnAddPlaces').click(function (e) {

                Browser.LibraryPropertyAndPlant.Click = function (e) {

                    SaveDevicePlace(e.ID);

                    getDevicePlaceList();

                };

                Browser.LibraryPropertyAndPlant.Show();

            });

            $('#btnDevicePlacesBack').click(function (e) {
                e.preventDefault();

                window.open("DeviceInfo", "_self");

            });

            $('#btnDevicePlacesDone').click(function (e) {
                e.preventDefault();

                window.open("DeviceInfoSummary", "_self");

            });

            function getDevicePlaceList() {
                AjaxSend({
                    url: 'Device/getDevicePlaceList',
                    success: function (vData) {
                        LoadDevicePlaceList(vData.DevicePlacesData.Rows);
                    }
                });
            }

            function LoadDevicePlaceList(vRows) {
                var pLV = $('#lstPlaces');
                var pItem;

                $("#divRecordCountPlaces").text(" Number of record(s) found: " + vRows.length);

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li>')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .append($('<h3>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].PropertyAndPlant)
                            )
                        )
                        .append($('<a>')
                                .attr('ID', vRows[p].ID)
                                .attr('href', "#")
                                .click(function (e) {
                                    e.preventDefault();

                                    sessionStorage["DevicePlaceToRemove"] = $(this).attr('ID');

                                    MessageBox("Remove this record?",
                                        'Remove',
                                        [
                                            { Text: 'Yes',
                                                Click: function () {

                                                    RemoveDevicePlace(sessionStorage["DevicePlaceToRemove"]);

                                                }
                                            }
                                        ,
                                            { Text: 'No'
                                            }
                                        ]
                                    );

                                })
                        );

                    pLV.append(pItem);
                };

                pLV.listview('refresh');
            };

            function RemoveDevicePlace(strID) {

                var pData = {};
                pData.DevicePlaceID = strID;

                AjaxSend({
                    url: "Device/RemoveDevicePlace",
                    data: pData,
                    success: function (vData) {

                        getDevicePlaceList();

                        sessionStorage["DevicePlaceToRemove"] = null;
                        $('.ui-dialog').dialog('close');

                    }
                });

            };

            function SaveDevicePlace(PropertyAndPlantID) {
                var pData = [];

                pData = {
                    DevicePlaceID: "",
                    PropertyAndPlantID: PropertyAndPlantID
                };

                AjaxSend({
                    url: "Device/CheckDevicePlace",
                    data: pData,
                    success: function (vData) {

                        if (vData.DevicePlacesData.Rows.length > 0) {
                            MessageBox("Place being added already exist.",
                            "Place Exist",
                            [
                                { Text: 'Ok' }
                            ]
                        );

                        }
                        else {
                            AjaxSend({
                                url: "Device/SaveDevicePlace",
                                data: pData,
                                success: function (vSaveData) {

                                    getDevicePlaceList();

                                }
                            });

                        }

                    }
                });

            };

        });
    });