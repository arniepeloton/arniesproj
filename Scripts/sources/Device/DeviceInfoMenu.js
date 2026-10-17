require([
    'common',
    GetPathFormat('/Scripts3/Browser/LibraryDevice.js')
    ], function () {

        $(function () {

            GetAllDevices();

            //Navigation Menu
            $('#btnHome').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu"), "_self");
            });

            $('#btnSystemAdmin').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu#pagePopUpMenuSettings"), "_self");
            });

            $('#btnSystemSettings').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu#pagePopSystemSettings"), "_self");
            });

            $('#btnSecuritySettings').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu#pagePopUpMenuSecuritySettings"), "_self");
            });
            //End Navigation Menu

            $('#btnNewDeviceInfo').click(function (e) {
                e.preventDefault();
                CreateNewEntry(1, '');
            });

            $('#btnDoneDeviceInfo').click(function () {

                window.open(GetPathFormat("/People/PeopleMenu"), "_self");

            });

            $('#btnDeviceGoToSetupChecklist').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/SetupChecklist"), "_self");

            });

            $('#frmDeviceSearch').submit(function (e) {
                e.preventDefault();

                GetAllDevices();

                return false;
            });


        });

        function GetAllDevices() {
            var pData = {};
            pData.KeyWord = $("#txtSearchDevice").val();

            AjaxSend({
                url: 'Browser/LibraryDevice',
                data: pData,
                success: function (vData) {
                    DisplayDevices(vData);
                }
            });
        }

        function DisplayDevices(vData) {
            pLV = $('#lstDevices');
            pLV.empty();

            $("#divDeviceSearchResults").hide();
            $("#divDeviceSearchNoResults").hide();
            //$("#btnNewDeviceInfo").hide();

            var vRows = vData.Results.Rows;

            if (vRows.length > 0) {
                $("#divDeviceSearchResults").show();
                $("#divDeviceRecordCount").text(" Number of record(s) found: " + vRows.length);

                for (ctrDevices = 0; ctrDevices <= vRows.length - 1; ctrDevices++) {
                    var m_ID = vRows[ctrDevices].ID;
                    pItem = $('<li>')
                    .append($('<a>')
                        .attr('ID', vRows[ctrDevices].ID)
                        .attr('href', '#')
                        .append($('<h3>')
                            .text(vRows[ctrDevices].PhoneName)
                        )
                        .append($('<p>')
                            .text(vRows[ctrDevices].PhoneModel)
                        )
                        .append($('<p>')
                            .text(vRows[ctrDevices].IMEI + ' - ' + vRows[ctrDevices].UUID)
                        )
                        .append($('<p>')
                            .text('Device No. ' + vRows[ctrDevices].DeviceNo)
                        )
                        .append($('<p>')
                            .text(vRows[ctrDevices].DateEnrolled)
                        )
                        .click(function () {
                            CreateNewEntry(0, $(this).attr("ID"));
                        })
                    )
                    pLV.append(pItem);
                }

                pLV.listview('refresh');

            }
            else {
                $("#divDeviceSearchNoResults").show();
                $("#btnNewDeviceInfo").show();
                $("#lblDeviceNotFound").text('The device "' + $("#txtSearchDevice").val() + '" does not exist. Please consider to search thoroughly using an alternate spelling before adding a new device to avoid duplication.');
            }

        }

        function DeactivateDevice(ID) {
            var pData = { ID: ID };
            AjaxSend({
                url: 'Device/DeactivateDevice',
                data: pData,
                success: function (vData) {
                }
            });
        }

        function CreateNewEntry(IsNew, strID) {
            var pData = {
                IsNew: IsNew,
                ID: strID
            };
            AjaxSend({
                url: 'Device/setCreateNew',
                data: pData,
                success: function (vData) {

                    if ((vData.IsNew || '') == "TRUE") {
                        PromptUserAccess('Devices', {
                            OnClickOKSuccess: function () {
                                //window.open(GetPathFormat("/Device/DeviceInfo"), "_self");
                                location = MergeUrlParameters({
                                    OrgID: sessionStorage.OrgID,
                                    IsMobile: CBool(IsMobile) ? 1 : 0
                                }, '/Device/DeviceInfo');
                            }
                        });
                    }
                    else {
                     
                        if (GetUrlParameters().mode == 'wizard') {
                            window.open(GetPathFormat("/Device/DeviceInfoSummary?mode=wizard"), "_self");
                        } else
                            window.open(GetPathFormat("/Device/DeviceInfoSummary"), "_self");
                    }
                }
            });

        }
    });
