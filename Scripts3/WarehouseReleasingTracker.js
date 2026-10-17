require(['common', 'business',
    GetPathFormat('/Scripts3/Browser/LibraryOverallPeople.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryDocument.js?crc='),
    'promise!/Scripts3/Browser/LibraryFowst.js?crc='
    ], function () {

        require([(OfflineMode ? '/Scripts3/SalesMenu_offline.js?crc=' : ''), (OfflineMode ? '/Scripts3/Offline/Business/SystemDefaults.js?crc=' : '')], function () {

            $(function () {

                var IsManual = 0;
                var customerID;
                var freeTypeCustomer;

                (function Init() {

                    var pParams = GetUrlParameters();
                    
                    if (pParams.mode == 'wrt') {
                        $('#btnPOSMode').html('');
                        $('#btnMoreOptions').html('');
                        $('#lblLabel').html('New <br /> Releasing');
                        $('#lblPayNow').html('Search <br /> Released <br /> Records');
                        $('#mainContent').hide();
                        $('#divCartEmpty').hide();
                        $('#divReleasingPanel').show();
                        startclock('#releaseClock');
                        $("#releaseDate").selectmenu("refresh");
                    }

                    if (!GetUser())
                        return;

                    startclock('#clock');

                })(); // END (function Init() {

                startclock('#clock');

                function LoadDocuments() {
                    AjaxSend({
                        url: 'WarehouseReleasingTracker/GetDetails',
                        data: {
                            operationCase: 2
                        },
                        success: function (vData) {
                            $('#tblBody').html('');
                            var pStr="";
                            for (var i = 0; i < vData.Result.Rows.length; i++) {
                                pStr = pStr + "<tr>";
                                pStr = pStr + "<td>" + vData.Result.Rows[i].ReleaseDate + "</td>";
                                pStr = pStr + "<td>" + vData.Result.Rows[i].ReleaseDoc + "</td>";
                                if (vData.Result.Rows[i].ReleaseTo == null || vData.Result.Rows[i].ReleaseTo == '') {
                                    pStr = pStr + "<td>" + vData.Result.Rows[i].CustomerName + "</td>";
                                } else {
                                    pStr = pStr + "<td>" + vData.Result.Rows[i].ReleaseTo + "</td>";
                                }
                                if (vData.Result.Rows[i].ReleaseTo == null || vData.Result.Rows[i].ReleaseTo == '') {
                                    pStr = pStr + "<td>" + vData.Result.Rows[i].CustomerName + "</td>";
                                } else {
                                    pStr = pStr + "<td>" + vData.Result.Rows[i].ReleaseTo + "</td>";
                                }
                                pStr = pStr + "</tr>";
                            }
                            $('#tblBody').html(pStr);
                        }
                    });
                }

                //Function for comparing documents
                function compareReleaseDoc() {
                    var resultCount;

                    AjaxSend({
                        url: 'WarehouseReleasingTracker/CompareDocument',
                        data: {
                            docNum: $('#documentSearch').val()
                        },
                        success: function (vData) {
                            resultCount = vData.Result;
                            if (resultCount > 1) {
                                AjaxSend({
                                    url: 'WarehouseReleasingTracker/GetDetails',
                                    data: {
                                        docNum: $('#documentSearch').val(),
                                        operationCase: 1
                                    },
                                    success: function (vData) {
                                        $('#popupMessageHeader').html('WARNING');
                                        $('#refNum').html('<p style="font-weight: normal;width: 100%;">' + vData.Result.Rows[0].ReleaseDoc + ' has already been released</p>');
                                        if (vData.Result.Rows[0].ReleaseTo == null || vData.Result.Rows[0].ReleaseTo == '') {
                                            $('#releasedTo').html(vData.Result.Rows[0].CustomerName);
                                        } else {
                                            $('#releasedTo').html(vData.Result.Rows[0].ReleaseTo);
                                        }
                                        $('#releasedOn').html(vData.Result.Rows[0].ReleaseDate);
                                        $('#releasedBy').html(vData.Result.Rows[0].ReleasedBy);
                                        $('#errorNotice').show();
                                        $('#popupMessage').popup('open');
                                    }
                                });
                            } else {
                                AjaxSend({
                                    url: 'WarehouseReleasingTracker/GetDetails',
                                    data: {
                                        docNum: $('#documentSearch').val(),
                                        operationCase: 1
                                    },
                                    success: function (vData) {
                                        $('#popupMessageHeader').html('Warehouse releasing <br /> successfully recorded');
                                        $('#refNum').html('<div class=\"ui-block-a\" style=\"width: 35%;\"><p style=\"font-weight: bold;\">Ref Num: </p></div><div class=\"ui-block-b\" style=\"width: 65%;\"><p style="font-weight: normal;">' + vData.Result.Rows[0].ReleaseDoc + '</p></div>');
                                        if (vData.Result.Rows[0].ReleaseTo == null || vData.Result.Rows[0].ReleaseTo == '') {
                                            $('#releasedTo').html(vData.Result.Rows[0].CustomerName);
                                        } else {
                                            $('#releasedTo').html(vData.Result.Rows[0].ReleaseTo);
                                        }
                                        $('#releasedOn').html(vData.Result.Rows[0].ReleaseDate);
                                        $('#releasedBy').html(vData.Result.Rows[0].ReleasedBy);
                                        $('#errorNotice').hide();
                                        $('#popupMessage').popup('open');
                                    }
                                });
                            }
                        }
                    });
                }

                //Funtions beyond here are to get user and fowst details
                function GetUser() {
                    var pUser;
                    if (OfflineMode) {
                        pUser = SalesMenu_offline().GetUser();
                        if ((pUser != undefined) && (pUser.FowstID == undefined)) {
                            if (pUser.FowstCount == undefined) {
                                MessageBox('The device you are currently using is not registered to any FOWST in the current organization.', null, null, {
                                    OnClose: function () {
                                        var pParam = {};
                                        if (IsMobile)
                                            pParam.IsMobile = 1;
                                        pParam = $.param(pParam);
                                        window.location = "/People/PeopleMenu?" + pParam;
                                    }
                                });
                                return false;
                            }
                        }
                        LoadUser(pUser);
                        SalesMenu_offline().UpdateCounterFromOnline();
                    } else {
                        AjaxSend({
                            url: 'Login3/GetUser',
                            data: { FowstID: sessionStorage.FowstID },
                            success: function (vData) {
                                LoadUser(vData);
                            },
                            error: function (vError) {
                                console.log('Error getting Login/GetUser. Retrying...');
                                GetUser();
                            }
                        });
                    } 
                }

                function LoadUser(vData) {

                    var pUserMenu;
                    var pLink;

                    if (CStr2(vData).length == 0) return;
                    if (vData.Error) {
                        MessageBox(vData.Error);
                        MessageBox(vData.Error, '', null, {
                            OnClose: function () {
                                window.location = '/People/PeopleMenu';
                            }
                        });
                        return;
                    };

                    if (vData.FirstName != undefined) {
                        pUserMenu = $('#divUserMenu');
                        pUserMenu.html('');

                        pLink = $('<a>')
                    .attr('href', '#pageUserMenu')
                    .css('text-decoration', 'none')
                    .html('Hi, ' + HtmlEncode(vData.FirstName) + '&#x25BC;');
                        pUserMenu.append(pLink);
                        if (CStr2(vData.FowstID).length > 0) {
                            sessionStorage.FowstID = vData.FowstID;
                            VerifyDevice();
                            sessionStorage.FowstName = vData.FowstName;
                            if (IsMobile)
                                Transfers.WriteFile(sessionStorage.OrgID + 'LastFowst.flg', null, vData.FowstID);
                        } else {
                            if (OfflineMode) {
                                BrowseAddresses();
                            } else {
                                var pData = { FilterFowst: 1 };
                                AjaxSend({
                                    url: 'Browser/LibraryFowst',
                                    data: pData,
                                    success: function (vFowstData) {

                                        if (vFowstData.Results.Rows.length == 1) {
                                            sessionStorage.FowstID = vFowstData.Results.Rows[0].ID;
                                            VerifyDevice();
                                            sessionStorage.FowstName = vFowstData.Results.Rows[0].FowstName;
                                        } else {
                                            BrowseAddresses();
                                        }
                                    }
                                });
                            }
                        }
                    }
                }

                function VerifyDevice() {
                    if (OfflineMode) return;
                    AjaxSend({
                        url: '/Login3/VerifyDevice',
                        success: function (vData) {
                            if (vData.Error != undefined) {
                                MessageBox('This device is not assigned to this FOWST', 'FOWST',
                                            [
                                                {
                                                    Text: 'Ok',
                                                    Click: function () {
                                                        window.location = '/People/PeopleMenu';
                                                    }
                                                }
                                            ]
                                );
                            }
                        }
                    });
                }

                function BrowseAddresses() {

                    Browser.LibraryFowst.InputData.OrganizationID = sessionStorage.OrgID;
                    Browser.LibraryFowst.InputData.FilterFowst = 1;
                    Browser.LibraryFowst.AfterClick = function (e) {
                        sessionStorage.FowstID = e.ID;
                        VerifyDevice();
                        sessionStorage.FowstName = e.FowstName;
                        if (OfflineMode) {
                            Android.UpdateSessionData({
                                FowstID: sessionStorage.FowstID
                            }); // disconnected checkout
                            SalesMenu_offline().UpdateCounterFromOnline();
                        }
                    };
                    Browser.LibraryFowst.Show();

                }

                $('#lnkCheckout').click(function () {
                    LoadDocuments();
                    $('#divReleasingPanel').hide();
                    $('#divRecordsPanel').show();
                });

                $('#btnCustomer').click(function (e) {
                    e.preventDefault();
                    $('#divRecordsPanel').hide();
                    $('#divReleasingPanel').show();
                });

                $('#documentRecordsSearch').submit(function (e) {
                    e.preventDefault();
                    Browser.LibraryDocument.InputData.docNum = $('#recordsSearch').val();
                    Browser.LibraryDocument.Click = function (e) {
                        AjaxSend({
                            url: 'WarehouseReleasingTracker/GetDetails',
                            data: {
                                docNum: e.ReleaseDoc,
                                operationCase: 1
                            },
                            success: function (vData) {
                                $('#tblBody').html('');
                                var pStr = "";
                                pStr = pStr + "<tr>";
                                pStr = pStr + "<td>" + vData.Result.Rows[0].ReleaseDate + "</td>";
                                pStr = pStr + "<td>" + vData.Result.Rows[0].ReleaseDoc + "</td>";
                                if (vData.Result.Rows[0].ReleaseTo == null || vData.Result.Rows[0].ReleaseTo == '') {
                                    pStr = pStr + "<td>" + vData.Result.Rows[0].CustomerName + "</td>";
                                } else {
                                    pStr = pStr + "<td>" + vData.Result.Rows[0].ReleaseTo + "</td>";
                                }
                                if (vData.Result.Rows[0].ReleaseTo == null || vData.Result.Rows[0].ReleaseTo == '') {
                                    pStr = pStr + "<td>" + vData.Result.Rows[0].CustomerName + "</td>";
                                } else {
                                    pStr = pStr + "<td>" + vData.Result.Rows[0].ReleaseTo + "</td>";
                                }
                                pStr = pStr + "</tr>";
                                $('#tblBody').html(pStr);
                            }
                        });

                    };
                    Browser.LibraryDocument.Show();
                });

                $('#personSearch').keypress(function (e) {
                    customerID = "";
                    if (e.keyCode == 13) {
                        e.preventDefault();
                        localStorage.setItem("wrt", 1);
                        Browser.LibraryOverallPeople.Click = function (e) {
                            $('#personSearch').val(e.Name);
                            customerID = e.ID;
                            localStorage.removeItem("wrt");
                        };
                        Browser.LibraryOverallPeople.Show();
                    }
                    startclock('#releaseClock');
                    $("#releaseDate").selectmenu("refresh");
                });

                $('#butBarCodeWrt').click(function () {
                    if ((CBool(window.IsMobile)) && (!CBool(window.InWindows))) {
                        mobile.Scan({
                            Success: function (vData) {
                                $('#documentSearch').val(vData.Value);
                                $('#lblManual').hide();
                                $('#documentSearch').prop('readonly', true);
                                startclock('#releaseClock');
                            }
                        });
                    } else {
                        MessageBox('Scan Barcode / QR Code feature is only available on the mobile app',
                                    'Scan Barcode / QR Code',
                                    [
                                        { Text: 'Ok' }
                                    ]
                        );
                    }
                });

                $('#chkManual').change(function () {
                    if (this.checked) {
                        $('#documentSearch').prop('readonly', false);
                        IsManual = 1;
                    } else {
                        $('#documentSearch').prop('readonly', true);
                        IsManual = 0;
                    }
                });

                $('#btnSubmitWrt').click(function () {

                    //Check if all of the fields have a value
                    if ($('#documentSearch').val() == "") {
                        MessageBox('Releasing document should not be blank.', "Missing field",
                                    [
                                        { Text: 'Ok' }
                                    ]
                        );
                    } else if ($('#personSearch').val() == "" && $('#personName').val() == "") {
                        MessageBox('Person field should not be empty.', "Missing field",
                                    [
                                        { Text: 'Ok' }
                                    ]
                        );
                    } else {
                        if ($('#personSearch').val() == "" && $('#personName').val() != "") {
                            freeTypeCustomer = $('#personName').val();
                        } else {
                            freeTypeCustomer = $('#personSearch').val();
                        }
                        AjaxSend({
                            url: 'WarehouseReleasingTracker/PostReleaseDocument',
                            data: {
                                docNum: $('#documentSearch').val(),
                                PeopleID: customerID,
                                FowstID: sessionStorage.FowstID,
                                IsManual: IsManual,
                                CustomerName: freeTypeCustomer
                            },
                            success: function (vData) {
                                if (vData.Result == "ok") {
                                    compareReleaseDoc();
                                }
                            }
                        });
                    }
                });

                $('#btnSuccess').click(function () {

                    pParam = {
                        'OrgID': sessionStorage.OrgID,
                        'mode': 'wrt'
                    };
                    //if (IsMobile)
                    //pParam.IsMobile = 1;
                    pParam = $.param(pParam);
                    window.location = '/WarehouseReleasingTracker?' + pParam;

                });

                $('#btnHome').click(function (e) {
                    e.preventDefault();
                    window.location = '/People/PeopleMenu';
                });

            }); // $(function() {
        }); // require 
    });                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      // require

//Android.Download({
//    Source: 'http://thepapertree.com.ph/data_ba_ron?akong_query_string',
//    Destination: '/dri_bar_ron/kamo_na.zip',
//    Success: function() {
//        Android.Extract(this.Destination);
//        var pString = Android.ReadFile(this.Destination + '/sulod_sa_zip.json');
//    },
//    Error: function() {
//    },
//    Complete: function() {
//    }
//});