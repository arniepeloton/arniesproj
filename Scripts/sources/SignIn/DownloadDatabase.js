require([
    'common',
    'qrcode',
    '/Scripts/sources/ImportExportData/ImportExportData.js?crc=',
    '/Scripts3/Offline/Browser/common.js?crc='
    ], function () {

        var mIsBusy = false;

        $(function () {
            var mUrlParams = GetUrlParameters();
            if (CStr2(mUrlParams.OrgID) == '')
                GetAllDatabases();
            else {
                if (!IsGuid(localStorage.PeopleUserAccountData)) {
                    NewMessageBox('localStorage.PeopleUserAccountData must be a GUID. This is a code error. js7W7GWZ9SS9!');
                    return;
                }
                sessionStorage.PeopleUserAccountData = localStorage.PeopleUserAccountData;
                sessionStorage.OrgID = mUrlParams.OrgID;
                WebPosDownloadDatabase();
            }

            $('#pageDownloadError').bind('pageshow', PageShowError)

            $('#btnDoneNotice').click(function () {
                if ($('#chkAgree').attr('checked') == 'checked') {
                    Transfers.WriteFile('IsFirstTime.flg', null, '');

                    location = MergeUrlParameters({
                        IsMobile: CBool(IsMobile) ? 1 : ''
                    }, '/People/PeopleMenu');

                }
                else if ($('#chkCancel').attr('checked') == 'checked') {
                    location = MergeUrlParameters({
                        IsMobile: CBool(IsMobile) ? 1 : ''
                    }, '/signin');
                }
                else {
                    MessageBox("Please click \'I Agree \' to proceed",
                            "Proceed?",
                            [
                                { Text: 'Ok' }
                            ]
                        );
                }
            });

            $('#btnDownloadDatabase').click(function () {
                var progressraw = ''
                var progress = [];
                var dbCount = 0;

                //Transfers.WriteFile('filename.txt', null, 'mao ni ang sulod')

                $('#tblDatabase tr td input').each(function () {
                    if ($(this).attr("checked") == "checked") {
                        if (mIsBusy) {
                            console.log('already downloading database jsHFN9GVD26K!');
                            return;
                        }
                        mIsBusy = true;
                        var OrgID = $(this).attr('ID');
                        var data = { 
                            OrgID: $(this).attr('ID')
                        };
                        sessionStorage.OrgID = OrgID;
                        dbCount++;
                        if (IsMobile)
                            data.PeopleID = Web.PeopleUserID();
                        else {
                            WebPosDownloadDatabase();
                            return;
                        }

                        //flushing
                        Transfers.WriteFile('DownloadingOrgID', null, ReadyGuid(OrgID));
                        var IsExists = 1;

                        IsExists = Transfers.ReadFile(OrgID + '.flg');

                        if (IsExists == undefined) {
                            IsExists = false;
                            Sql.DeleteIfExist(OrgID);
                        }
                        else {
                            if (Sql.IsExist(OrgID)) {
                                IsExists = true;
                            }
                            else {
                                IsExists = false;
                            }

                        }

                        if (IsExists) {
                            RequestDatabaseUpdate(OrgID);
                        }
                        else {
                            RequestImportData(OrgID, CDec2(IsExists));
                        }
                    }; //if
                });

                if (dbCount == 0) {
                    MessageBox("Please select database to download",
                            "Download Database",
                            [
                                { Text: 'Ok' }
                            ]
                        );
                }
            });

            $('#btnSendRequest').click(function () {
                var pData = [];
                var ctr = 0;

                if (IsMobile)
                    pData = {
                        DeviceID: '',
                        PhoneName: $.trim(Device.Name()),
                        PhoneModel: $.trim(Device.Name()),
                        IMEI: $.trim(Device.Imei()),
                        UUID: $.trim(Device.Serial()),
                        OS: $.trim(Device.Platform())
                    };
                else
                    pData = {
                        DeviceID: '',
                        PhoneName: $.trim('WebPOS'),
                        PhoneModel: $.trim('WebPOS'),
                        IMEI: $.trim('WebPOS'),
                        UUID: $.trim(localStorage.UUID),
                        OS: $.trim('WebPOS')
                    };

                AjaxSend({
                    url: CStr2(sessionStorage.RemoteServer) +  "Device/RequestDeviceApproval",
                    data: pData,
                    success: function (vSaveData) {
                        MessageBox(vSaveData.Result,
                            "Download Database",
                            [
                                { Text: 'Ok',
                                    Click: function () {
                                        window.location = MergeUrlParameters({
                                            IsMobile: CBool(IsMobile) ? 1 : ''
                                        }, '/SignIn/DownloadDatabase');
                                    }
                                }
                            ]
                        );
                    }
                });

            });

            $('#btnDone').click(function () {
                history.back();
            });

        });

        function GetAllDatabases() {
            if (IsMobile) {
                var vData = Android.GetSessionData().PeopleData;
                var pData = { Username: '', UserPassword: '' }
                if (vData == undefined) {
                    vData = Android.GetSessionData();

                    if (vData == undefined) {
                        proj.PromptLogin();
                        return;
                    }

                    pData = { Username: vData.Username, UserPassword: vData.Password }
                } else {
                    pData = { Username: vData.Username, UserPassword: vData.UserPassword }
                }

                AjaxSend({
                    url: 'People/CheckPeopleUserAccount',
                    data: pData,
                    success: function (vData) {
                        //log in                    
                        Login(vData);
                    }
                }); // AjaxSend url: 'People/CheckPeopleUserAccount'
            } else {
                Login({
                    PeopleUserAccountData: sessionStorage.PeopleUserAccountData,
                    RemoteServer: sessionStorage.RemoteServer
                });
            }

            function Login(vData) {
                var pLV = $("#tblDatabase");
                pLV.empty();
                if ($.trim(vData.PeopleUserAccountData) != '') {
                    var pData = { 
                        PeopleID: vData.PeopleUserAccountData,
                        DontRequireLogin: !CBool(IsMobile)
                    };
                    if ($.trim(vData.PeopleUserAccountData) != '') {
                        var pRemoteServer = vData.RemoteServer;
                        AjaxSend({
                            url: CStr2(pRemoteServer) + "People/getPeopleInfoName",
                            data: pData,
                            success: function (vData) {
                                //database
                                var pData2 = {};
                                if (!CBool(IsMobile))
                                    pData2 = pData;
                                AjaxSend({
                                    url: CStr2(pRemoteServer) + "People/getPeopleOrganizationList",
                                    data: pData2,
                                    success: function (vDataOrg) {
                                        var vRows = vDataOrg.PeopleData.Rows;
                                        if (vRows.length > 0) {
                                            for (i = 0; i < vRows.length; i++) {
                                                var checked = undefined;

                                                pItem = $('<tr>')
                                                .append($('<td>')
                                                    .append($('<div class="bg-white border rounded p-3 mb-3">')
                                                        .append($('<div class="form-check" style="font-size:23px;">')
                                                            .append($('<input data-role="none" class="form-check-input" type=radio />')
                                                                .attr('ID', vRows[i].OrganizationID)
                                                                // .attr('name', vRows[i].OrganizationID)
                                                                .attr('name', "optOrganization")
                                                                .attr('checked', checked)
                                                                .data('data', vRows[i])
                                                            )
                                                            .append($('<label class="form-check-label" style="font-size:18px; width:100%">')
                                                                .attr('for', vRows[i].OrganizationID)
                                                                .text(vRows[i].CompanyName)
                                                            )
                                                        )
                                                    )
                                                )//td          
                                                pLV.append(pItem);

                                                //$('#' + vRows[i].OrganizationID).checkboxradio();
                                            }; //for
                                            //try {
                                            //    $('#lstDatabaseFrame').listview('refresh');
                                            //}
                                            //catch (e) {
                                            //    $('#lstDatabaseFrame').listview();
                                            //}

                                        }; //if
                                    } //success
                                });
                            } //success
                        });
                    }
                } // success
            }
        }; //GetAllDatabases

        var mLastTimeStamp;

        function WebPosDownloadDatabase() {
            $('.container').hide();
            $('#divProgressLog').html('Downloading database for WebPOS');
            $('#pageProgress').show();
            mLastTimeStamp = new Date();
            mLastTimeStamp = FormatDate(mLastTimeStamp, {Format: 'yyyy-MM-dd HH:mm:ss.000'});
            GetRecent();
            var pData = {
                OrgID: sessionStorage.OrgID,
                PeopleID: sessionStorage.PeopleUserAccountData
            };

            setTimeout(function() {
                AjaxSend({
                    url: 'WebPosDbDownloader/RequestData',
                    data: pData,
                    success: function(vData) {
                        if (vData.Error) {
                            NewMessageBox(vData.Error);
                            AddMessage({
                                TimeStamp: new Date(),
                                Text: vData.Error
                            });
                        } else if (vData.Result == 'for follow up')
                            setTimeout(WebPosDownloadDatabase, 10000);
                        else if (vData.Result == 'unauthorized device') {
                            Bs.Loading('hide');
                            $('#pageDownloadError').pageshow();
                            setTimeout(proj.DownloadDatabasePageShowError, 736);
                        } else if (vData.Result == 'done')
                            NewMessageBox('Database has been successfully downloaded. Please go to sign in page', 'Download Success', null, {
                                Close: function() {
                                    location = localStorage.LoginUrl;
                                }
                            });

                    }
                });
            }, 1000);
        }

        function GetRecent() {
            AjaxSend({
                url: '/WebPosDbDownloader/GetRecentMessages',
                data: {
                    LastTimeStamp: mLastTimeStamp
                },
                success: function(vData) {
                    if ($.isArray(vData)) {
                        for (var pItem of vData) {
                            mLastTimeStamp = pItem.TimeStamp;
                            AddMessage(pItem);
                        }
                        if (vData.length > 0)
                            $('html,body').animate({scrollTop: document.body.scrollHeight},"slow"); // https://stackoverflow.com/a/35618752/334045
                    } else
                        AddMessage({
                            Text: vData
                        });
                },
                complete: function() {
                    setTimeout(GetRecent, 3000);
                }
            });
        }

        function AddMessage(vMsg) {
            var pParent = $('#divProgressLog');
            var pText = '[' + FormatDate(vMsg.TimeStamp, {Format: 'MM/dd/yyyy hh:mm:ss a'}) + '] ' + vMsg.Text;
            var pDiv = $('<div>')
                .text(pText);
            pParent.append(pDiv);
        }

        if (IsMobile)
            proj.DownloadDatabasePageShowError = PageShowError;
        else
            proj.DownloadDatabasePageShowError = PageShowErrorWebPos;

        function PageShowError() {
            $('#lblPhoneName').text(Device.Name()); // device.name
            $('#lblPhoneModel').text(Device.Name()); // device.platform
            $('#lblUUID').text(Device.Serial()); // device.uuid
            $('#lblOS').text(Device.Platform()); // device.platform
            $('#lblIMEI').text(Device.Imei()); // window.pPrint.GetIMEI()

            var data = { Name: Device.Name(),
                Serial: Device.Serial(),
                Platform: Device.Platform(),
                Imei: Device.Imei()
            }

            Sql.DeleteIfExist(sessionStorage.OrgID);

            $('#dvDeviceQrCode').html('');

            var qrcode = new QRCode("dvDeviceQrCode", {
                text: JSON.stringify(data)
            });

        }

        function PageShowErrorWebPos() {
            $('#lblPhoneName').text('WebPos'); // device.name
            $('#lblPhoneModel').text('WebPos'); // device.platform
            $('#lblUUID').text(localStorage.UUID); // device.uuid
            $('#lblOS').text('WebPos'); // device.platform
            $('#lblIMEI').text('WebPos'); // window.pPrint.GetIMEI()

            var data = { Name: 'WebPos',
                Serial: localStorage.UUID,
                Platform: 'WebPos',
                Imei: 'WebPos'
            }

            $('#dvDeviceQrCode').html('');
        }

        function SqlTest() {

            var pSql;

            pSql = ' SELECT LibraryOrganizationFOWST.Name AS FowstName' + '\n' +
                ' ,LibraryDeviceFowst.TerminalNo' + '\n' +
                ' FROM LibraryDeviceFowst' + '\n' +
                ' JOIN Mobiz.LibraryOrganizationFOWST LibraryOrganizationFOWST ON LibraryOrganizationFOWST.ID = LibraryDeviceFowst.FowstID';
            Sql.Execute(pSql);
        };

    });

    