require([
    'common'
    ], function () {

        $(function () {

            var IsNewOrgRepTelContact = true;
            var arOrgRepTel = new Array();
            var intOrgRepTelCntr = 0;
            var intOrgRepTelIndex = 0;

            var intOrgRepTelRecID = 0;
            var intOrgRepTelCountryID = 1;
            var intOrgRepTelCountryCode = 2;
            var intOrgRepTelCallingCode = 3;
            var intOrgRepTelAreaCodeID = 4;
            var intOrgRepTelArea = 5;
            var intOrgRepTelAreaCode = 6;
            var intOrgRepTelContactNo = 7;
            var intOrgRepTelExtNo = 8;
            var intOrgRepTelContactType = 9;
            var intOrgRepTelState = 10;

            var IsNewOrgRepMobileContact = true;
            var arOrgRepMobile = new Array();
            var intOrgRepMobileCntr = 0;
            var intOrgRepMobileIndex = 0;

            var intOrgRepMobileRecID = 0;
            var intOrgRepMobileCountryID = 1;
            var intOrgRepMobileCountryCode = 2;
            var intOrgRepMobileCallingCode = 3;
            var intOrgRepMobileTelCoPrefixID = 4;
            var intOrgRepMobilePrefix = 5;
            var intOrgRepMobileTelCo = 6;
            var intOrgRepMobileContactNo = 7;
            var intOrgRepMobileContactType = 8;
            var intOrgRepMobileState = 9;


            var IsNewOrgRepEmailContact = true;
            var arOrgRepEmail = new Array();
            var intOrgRepEmailCntr = 0;
            var intOrgRepEmailIndex = 0;

            var intOrgRepEmailRecID = 0;
            var intOrgRepEmailAddress = 1;
            var intOrgRepEmailType = 2;
            var intOrgRepEmailState = 3;

            var mLoaded = false;
            var isIE = false || !!document.documentMode;

            var libname, lbllibname, addlibname, repid;

            var NameOfBtnForLibrary;

            $.mobile.activePage.bind('pageshow', PageShow);
            function PageShow() {
                mLoaded = true;
                lbllibname = sessionStorage.LabelLibraryName;
                libname = sessionStorage.LibraryName;
                addlibname = sessionStorage.AddViewLibBCName;
                repid = sessionStorage.ViewID;
                getOrgRepresentativeList();
            };
            if (!mLoaded) PageShow();

            window.OrgShortRep = {
                ShowOrgRep: function () {
                    lbllibname = sessionStorage.LabelLibraryName;
                    libname = sessionStorage.LibraryName;
                    addlibname = sessionStorage.AddViewLibBCName;
                    repid = sessionStorage.ViewID;
                    getOrgRepresentativeList();
                }
            }

            $('body').on('click', '#btnOrgRepAdd', function (e) {
                e.preventDefault();

                //$.mobile.changePage('#pageOrgRepInfo');
                $('#pageRepList').hide();
                $('#pageOrgRepInfo').show();
                $('#pageOrgRepContactTelInfo').hide();
                $('#pageOrgRepContactMobileInfo').hide();
                $('#pageOrgRepContactEmail').hide();
                ClearOrgRepFields();
            })
            .on('click', '#btnOrgRepTelCountryCode', function (e) {
                $('#TitleLibrary').text('COUNTRY');
                $('#txtSearchKeyword').val('').attr('placeholder', 'Type name of Country');
                $('#panSearchLibrary').popup('open');
                $('#lstOfLibrary').empty();
                NameOfBtnForLibrary = 'Telephone';
                //                Browser.LibraryCountries.Click = function (e) {

                //                    $('#txtOrgRepTelCountryID').val(e.ID)
                //                    $('#btnOrgRepTelCountryCode').text(e.Code + " - " + e.CallingCode)
                //                    .button('refresh');

                //                };
                //                Browser.LibraryCountries.Show();
            })
            .on('click', '#btnOrgRepTelAreaCode', function (e) {
                $('#TitleLibrary').text('AREA CODE');
                $('#txtSearchKeyword').val('').attr('placeholder', 'Type name of area');
                $('#panSearchLibrary').popup('open');
                $('#lstOfLibrary').empty();
                NameOfBtnForLibrary = 'Telephone';
                //                Browser.LibraryAreaCodes.Click = function (e) {

                //                    $('#txtOrgRepTelAreaCodeID').val(e.ID)
                //                    $('#btnOrgRepTelAreaCode').text(e.Area + ' - ' + e.CallingCode)
                //                    .button('refresh');

                //                };
                //                Browser.LibraryAreaCodes.Show();
            })
            .on('keydown', '#txtOrgRepContactNoTel', function (e) {
                if (!NumberOnly(e)) {
                    return;
                }
            })
            .on('keydown', '#txtOrgRepContactExtensionTel', function (e) {
                if (!NumberOnly(e)) {
                    return;
                }
            })
            .on('click', '#btnOrgRepMobileCountryCode', function (e) {
                $('#TitleLibrary').text('COUNTRY');
                $('#txtSearchKeyword').val('').attr('placeholder', 'Type name of Country');
                $('#panSearchLibrary').popup('open');
                $('#lstOfLibrary').empty();
                NameOfBtnForLibrary = 'Mobile';
                //                Browser.LibraryCountries.Click = function (e) {

                //                    $('#txtOrgRepMobileCountryID').val(e.ID)
                //                    $('#btnOrgRepMobileCountryCode').text(e.Code + " - " + e.CallingCode)
                //                    .button('refresh');

                //                };
                //                Browser.LibraryCountries.Show();
            })
            .on('click', '#btnOrgRepMobileTelCoPrefixCode', function (e) {
                $('#TitleLibrary').text('TELCO CODE');
                $('#txtSearchKeyword').val('').attr('placeholder', 'Type name or prefix number of sim provider');
                $('#panSearchLibrary').popup('open');
                $('#lstOfLibrary').empty();
                NameOfBtnForLibrary = 'Mobile';
                //                Browser.LibraryTelcoPrefixes.Click = function (e) {

                //                    $('#txtOrgRepTelCoPrefixID').val(e.ID)
                //                    $('#btnOrgRepMobileTelCoPrefixCode').text(e.TelCo + " - " + e.Prefix)
                //                    .button('refresh');

                //                };
                //                Browser.LibraryTelcoPrefixes.Show();
            })
            .on('keydown', '#txtOrgRepContactNoMobile', function (e) {
                if (!NumberOnly(e)) {
                    return;
                }
            })
            .on('click', '#btnOrgRepContactTelAdd', function (e) {
                e.preventDefault();

                //$.mobile.changePage('#pageOrgRepContactTelInfo');
                $('body').animate({ scrollTop: 0 });
                $('#pageRepList').hide();
                $('#pageOrgRepInfo').hide();
                $('#pageOrgRepContactTelInfo').show();
                $('#pageOrgRepContactMobileInfo').hide();
                $('#pageOrgRepContactEmail').hide();

                IsNewOrgRepTelContact = true
                intOrgRepTelCntr = intOrgRepTelCntr + 1;
                intOrgRepTelIndex = intOrgRepTelCntr;

                ClearOrgRepContactTelFields();
            })
            .on('click', '#btnOrgRepContactMobAdd', function (e) {
                e.preventDefault();

                //$.mobile.changePage('#pageOrgRepContactMobileInfo');
                $('body').animate({ scrollTop: 0 });
                $('#pageRepList').hide();
                $('#pageOrgRepInfo').hide();
                $('#pageOrgRepContactTelInfo').hide();
                $('#pageOrgRepContactMobileInfo').show();
                $('#pageOrgRepContactEmail').hide();

                IsNewOrgRepMobileContact = true
                intOrgRepMobileCntr = intOrgRepMobileCntr + 1;
                intOrgRepMobileIndex = intOrgRepMobileCntr;

                ClearOrgRepContactMobileFields();
            })
            .on('click', '#btnOrgRepContactEmailAdd', function (e) {
                e.preventDefault();

                //$.mobile.changePage('#pageOrgRepContactEmail');
                $('body').animate({ scrollTop: 0 });
                $('#pageRepList').hide();
                $('#pageOrgRepInfo').hide();
                $('#pageOrgRepContactTelInfo').hide();
                $('#pageOrgRepContactMobileInfo').hide();
                $('#pageOrgRepContactEmail').show();

                IsNewOrgRepEmailContact = true
                intOrgRepEmailCntr = intOrgRepEmailCntr + 1;
                intOrgRepEmailIndex = intOrgRepEmailCntr;

                ClearOrgRepContactEmailFields();
            })
            .on('click', '#btnOrgRepContactTelSave', function (e) {
                e.preventDefault();

                if ($.trim($("#txtOrgRepTelCountryID").val()) == "") {
                    $('#TitleOrgEmp').text('Country Code');
                    $('#MsgOrgEmp').text('Please select country code.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                if ($.trim($("#txtOrgRepTelAreaCodeID").val()) == "") {
                    $('#TitleOrgEmp').text('Area Code');
                    $('#MsgOrgEmp').text('Please select area code.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                if ($.trim($("#txtOrgRepContactNoTel").val()) == "") {
                    $('#TitleOrgEmp').text('Tel. No.');
                    $('#MsgOrgEmp').text('Please enter 7 digit telephone number.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                if ($.trim($("#txtOrgRepContactNoTel").val()).length != 7) {
                    $('#TitleOrgEmp').text('Tel. No.');
                    $('#MsgOrgEmp').text('Please enter 7 digit telephone number.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                if (!$.isNumeric($("#txtOrgRepContactNoTel").val())) {
                    $('#TitleOrgEmp').text('Tel. No.');
                    $('#MsgOrgEmp').text('Please enter valid telephone number.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                if ($.trim($("#txtOrgRepContactExtensionTel").val()).length > 0) {
                    if (!$.isNumeric($("#txtOrgRepContactExtensionTel").val())) {
                        $('#TitleOrgEmp').text('Tel. No.');
                        $('#MsgOrgEmp').text('Please enter valid extension or local number.');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').popup('close');
                        })
                        $('#divBtnCancel').hide();
                        return;
                    }
                }

                if ($.trim($("#selOrgRepContactTypeTel").val()) == "") {
                    $('#TitleOrgEmp').text('Telephone Contact Type');
                    $('#MsgOrgEmp').text('Please select telephone contact type.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                SaveOrgRepContactTel();
            })
            .on('click', '#btnOrgRepContactTelRemove', function (e) {
                e.preventDefault();
                $('#TitleOrgEmp').text('Remove');
                $('#MsgOrgEmp').text('Remove this record?');
                $('#panOrgEmpMessage').popup('open');
                $('#btnOKOrgEmp').text('Yes').click(function (e) {
                    e.preventDefault();
                    RemoveOrgRepContactTel();
                    $('#panOrgEmpMessage').popup('close');
                })
                $('#divBtnCancel').show();
                $('#btnCancelOrgEmp').text('No').show().click(function () {
                    $('#panOrgEmpMessage').popup('close');
                })
            })
            .on('click', '#btnOrgRepContactTelCancel', function (e) {
                e.preventDefault();

                //$.mobile.changePage('#pageOrgRepInfo');
                $('#pageRepList').hide();
                $('#pageOrgRepInfo').show();
                $('#pageOrgRepContactTelInfo').hide();
                $('#pageOrgRepContactMobileInfo').hide();
                $('#pageOrgRepContactEmail').hide();
            })
            .on('click', '#btnOrgRepContactMobileSave', function (e) {
                e.preventDefault();

                if ($.trim($("#txtOrgRepMobileCountryID").val()) == "") {
                    $('#TitleOrgEmp').text('Country Code');
                    $('#MsgOrgEmp').text('Please select country code.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                if ($.trim($("#txtOrgRepTelCoPrefixID").val()) == "") {
                    $('#TitleOrgEmp').text('Prefix Number');
                    $('#MsgOrgEmp').text('Please select prefix number.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                if ($.trim($("#txtOrgRepContactNoMobile").val()) == "") {
                    $('#TitleOrgEmp').text('Mobile No.');
                    $('#MsgOrgEmp').text('Please enter 7 digit mobile number.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                if ($.trim($("#txtOrgRepContactNoMobile").val()).length != 7) {
                    $('#TitleOrgEmp').text('Mobile No.');
                    $('#MsgOrgEmp').text('Please enter 7 digit mobile number.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                if (!$.isNumeric($("#txtOrgRepContactNoMobile").val())) {
                    $('#TitleOrgEmp').text('Mobile No.');
                    $('#MsgOrgEmp').text('Please enter valid mobile number.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                if ($.trim($("#selOrgRepContactTypeMobile").val()) == "") {
                    $('#TitleOrgEmp').text('Mobile Contact Type');
                    $('#MsgOrgEmp').text('Please select mobile contact type.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                SaveOrgRepContactMobile();
            })
            .on('click', '#btnOrgRepContactMobileRemove', function (e) {
                e.preventDefault();
                $('#TitleOrgEmp').text('Remove');
                $('#MsgOrgEmp').text('Remove this record?');
                $('#panOrgEmpMessage').popup('open');
                $('#btnOKOrgEmp').text('Yes').click(function (e) {
                    e.preventDefault();
                    RemoveOrgRepContactMobile();
                    $('#panOrgEmpMessage').popup('close');
                })
                $('#divBtnCancel').show();
                $('#btnCancelOrgEmp').text('No').show().click(function () {
                    $('#panOrgEmpMessage').popup('close');
                })
            })
            .on('click', '#btnOrgRepContactMobileCancel', function (e) {
                e.preventDefault();

                //$.mobile.changePage('#pageOrgRepInfo');
                $('#pageRepList').hide();
                $('#pageOrgRepInfo').show();
                $('#pageOrgRepContactTelInfo').hide();
                $('#pageOrgRepContactMobileInfo').hide();
                $('#pageOrgRepContactEmail').hide();
            })
            .on('click', '#btnOrgRepContactEmailSave', function (e) {
                e.preventDefault();

                if ($.trim($("#txtOrgRepContactEmail").val()) == "") {
                    $('#TitleOrgEmp').text('Email Address');
                    $('#MsgOrgEmp').text('Please enter email address.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                if (IsEmail($.trim($("#txtOrgRepContactEmail").val())) == false) {
                    $('#TitleOrgEmp').text('Email Address');
                    $('#MsgOrgEmp').text('Please enter valid email address.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                if ($.trim($("#selOrgRepContactTypeEmail").val()) == "") {
                    $('#TitleOrgEmp').text('Email Address');
                    $('#MsgOrgEmp').text('Please select email type.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                SaveOrgRepContactEmail();
            })
            .on('click', '#btnOrgRepContactEmailRemove', function (e) {
                e.preventDefault();
                $('#TitleOrgEmp').text('Remove');
                $('#MsgOrgEmp').text('Remove this record?');
                $('#panOrgEmpMessage').popup('open');
                $('#btnOKOrgEmp').text('Yes').click(function (e) {
                    e.preventDefault();
                    RemoveOrgRepContactEmail();
                    $('#panOrgEmpMessage').popup('close');
                })
                $('#divBtnCancel').show();
                $('#btnCancelOrgEmp').text('No').show().click(function () {
                    $('#panOrgEmpMessage').popup('close');
                })
            })
            .on('click', '#btnOrgRepContactEmailCancel', function (e) {
                e.preventDefault();

                //$.mobile.changePage('#pageOrgRepInfo');
                $('#pageRepList').hide();
                $('#pageOrgRepInfo').show();
                $('#pageOrgRepContactTelInfo').hide();
                $('#pageOrgRepContactMobileInfo').hide();
                $('#pageOrgRepContactEmail').hide();
            })
            .on('click', '#btnOrgRepSave', function (e) {
                e.preventDefault();

                if ($.trim($("#txtOrgRepLastName").val()) == "") {
                    $('#TitleOrgEmp').text('Last Name');
                    $('#MsgOrgEmp').text('Please enter last name.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                if ($.trim($("#txtOrgRepFirstName").val()) == "") {
                    $('#TitleOrgEmp').text('First Name');
                    $('#MsgOrgEmp').text('Please enter first name.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                SaveOrgRep();
            })
            .on('click', '#btnOrgRepCancel', function (e) {
                e.preventDefault();
                $('#pageRepList').show();
                $('#pageOrgRepInfo').hide();
                $('#pageOrgRepContactTelInfo').hide();
                $('#pageOrgRepContactMobileInfo').hide();
                $('#pageOrgRepContactEmail').hide();
            })
            .on('click', '#btnOrgRepDone', function (e) {
                e.preventDefault();
                window.location.hash = '';
                window.location.hash = 'UniversalPage';
                LibraryTable.ViewCustomerPage();

            })
            .on('keypress', '#txtSearchKeyword', function (e) {
                if (e.keyCode == 13) {
                    if ($('#TitleLibrary').text() == "COUNTRY") {
                        SearchCountryPopUp($(this).val());
                    }
                    else if ($('#TitleLibrary').text() == "AREA CODE") {
                        SearchAreaCodePopUp($(this).val());
                    }
                    else if ($('#TitleLibrary').text() == "TELCO CODE") {
                        SearchTelcoCodePopUp($(this).val());
                    }
                }
            })
            function SearchCountryPopUp(s) {
                var pData = {};

                pData.keywords = s;

                AjaxSend({
                    url: 'Browser/LibraryCountries',
                    data: pData,
                    success: function (vData) {
                        $('#lstOfLibrary').empty();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstOfLibrary').append($('<li>')
                                    .append($('<a>')
                                        .attr('ID', vData.Results.Rows[p].ID)
                                        .append($('<br><p style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].Name)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function (e) {
                                            if (NameOfBtnForLibrary == "Telephone") {
                                                $('#txtOrgRepTelCountryID').val($(this).data('data').ID);
                                                $('#btnOrgRepTelCountryCode').text($(this).data('data').Code + ' - ' + $(this).data('data').CallingCode).button('refresh');
                                            }
                                            else if (NameOfBtnForLibrary == "Mobile") {
                                                $('#txtOrgRepMobileCountryID').val($(this).data('data').ID);
                                                $('#btnOrgRepMobileCountryCode').text($(this).data('data').Code + ' - ' + $(this).data('data').CallingCode).button('refresh');
                                            }

                                            $('#panSearchLibrary').popup('close');
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#lstOfLibrary').append($('<li>').text('No Result(s) Found'));
                        }

                        $('#lstOfLibrary').listview('refresh');
                    }
                });
            }

            function SearchAreaCodePopUp(s) {
                var pData = {};

                pData.keywords = s;

                AjaxSend({
                    url: 'Browser/LibraryAreaCodes',
                    data: pData,
                    success: function (vData) {
                        $('#lstOfLibrary').empty();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstOfLibrary').append($('<li>')
                                    .append($('<a>')
                                        .attr('ID', vData.Results.Rows[p].ID)
                                        .append($('<br><p style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].Area + ' - ' + vData.Results.Rows[p].CallingCode)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function (e) {
                                            if (NameOfBtnForLibrary == "Telephone") {
                                                $('#txtOrgRepTelAreaCodeID').val($(this).data('data').ID);
                                                $('#btnOrgRepTelAreaCode').text($(this).data('data').Area + ' - ' + $(this).data('data').CallingCode).button('refresh');
                                            }

                                            $('#panSearchLibrary').popup('close');
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#lstOfLibrary').append($('<li>').text('No Result(s) Found'));
                        }

                        $('#lstOfLibrary').listview('refresh');
                    }
                });
            }

            function SearchTelcoCodePopUp(s) {
                var pData = {};

                pData.keywords = s;

                AjaxSend({
                    url: 'Browser/LibraryTelcoPrefixes',
                    data: pData,
                    success: function (vData) {
                        $('#lstOfLibrary').empty();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstOfLibrary').append($('<li>')
                                    .append($('<a>')
                                        .attr('ID', vData.Results.Rows[p].ID)
                                        .append($('<br><p style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].Prefix)
                                        )
                                        .append($('<p>')
                                            .text(vData.Results.Rows[p].TelCo)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function (e) {
                                            if (NameOfBtnForLibrary == "Mobile") {
                                                $('#txtOrgRepTelCoPrefixID').val($(this).data('data').ID);
                                                $('#btnOrgRepMobileTelCoPrefixCode').text($(this).data('data').TelCo + ' - ' + $(this).data('data').Prefix).button('refresh');
                                            }

                                            $('#panSearchLibrary').popup('close');
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#lstOfLibrary').append($('<li>').text('No Result(s) Found'));
                        }

                        $('#lstOfLibrary').listview('refresh');
                    }
                });
            }


            function getOrgRepresentativeList() {
                AjaxSend({
                    url: 'OrganizationShort/getOrgRepList',
                    success: function (vData) {
                        LoadOrgRepList(vData.OrganizationRepData.Rows);
                    }
                });
            }

            function LoadOrgRepList(vRows) {
                var pLV = $('#lstOrgRepresentative');
                var pItem;

                $("#divOrgRepRecordCount").text(" Number of record(s) found: " + vRows.length);

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li>')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();
                                //$.mobile.changePage('#pageOrgRepInfo');
                                $('#pageRepList').hide();
                                $('#pageOrgRepInfo').show();
                                $('#pageOrgRepContactTelInfo').hide();
                                $('#pageOrgRepContactMobileInfo').hide();
                                $('#pageOrgRepContactEmail').hide();
                                ShowOrgRepInfo($(this).attr('ID'));
                            })
                            .append($('<h3>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].RepresentativeName)
                            )
                        );
                    pLV.append(pItem);
                };

                pLV.listview('refresh');
            };

            function ShowOrgRepInfo(strID) {
                var pData = {};
                pData.ID = strID;

                ClearOrgRepFields();

                AjaxSend({
                    url: "OrganizationShort/getOrgRepInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.OrganizationRepData.Rows.length > 0) {

                            $("#txtOrgRepID").val(vData.OrganizationRepData.Rows[0].ID || '');

                            $("#txtOrgRepTitle").val(vData.OrganizationRepData.Rows[0].Title || '');
                            $("#txtOrgRepLastName").val(vData.OrganizationRepData.Rows[0].LastName || '');
                            $("#txtOrgRepFirstName").val(vData.OrganizationRepData.Rows[0].FirstName || '');
                            $("#txtOrgRepMiddleName").val(vData.OrganizationRepData.Rows[0].MiddleName || '');
                            $("#selOrgRepGenSuffixName").val(vData.OrganizationRepData.Rows[0].GenerationalSuffix || '').selectmenu('refresh');

                            $("#selOrgRepIsActive").val(vData.OrganizationRepData.Rows[0].IsActive || '1').slider("refresh");

                            pData.OrganizationID = vData.OrganizationRepData.Rows[0].ID;

                            //Get Tel
                            AjaxSend({
                                url: 'OrganizationShort/getOrgRepContactTelList',
                                data: pData,
                                success: function (vData) {
                                    LoadOrgRepContactTel(vData.OrganizationRepData.Rows);
                                }
                            });

                            //Get mobile
                            AjaxSend({
                                url: 'OrganizationShort/getOrgRepContactMobileList',
                                data: pData,
                                success: function (vData) {
                                    LoadOrgRepContactMobile(vData.OrganizationRepData.Rows);
                                }
                            });

                            //Get Email
                            AjaxSend({
                                url: 'OrganizationShort/getOrgRepContactEmailList',
                                data: pData,
                                success: function (vData) {
                                    LoadOrgRepContactEmail(vData.OrganizationRepData.Rows);
                                }
                            });
                        }

                    }
                });

            };

            function LoadOrgRepContactTel(vRows) {

                for (var p = 0; p <= vRows.length - 1; p++) {

                    arOrgRepTel = new Array();
                    arOrgRepTel[intOrgRepTelRecID] = vRows[p].ID;
                    arOrgRepTel[intOrgRepTelCountryID] = vRows[p].CountryID;
                    arOrgRepTel[intOrgRepTelCountryCode] = vRows[p].Country;
                    arOrgRepTel[intOrgRepTelCallingCode] = vRows[p].CountryCode;
                    arOrgRepTel[intOrgRepTelAreaCodeID] = vRows[p].AreaCodeID;
                    arOrgRepTel[intOrgRepTelArea] = vRows[p].Area;
                    arOrgRepTel[intOrgRepTelAreaCode] = vRows[p].AreaCode;
                    arOrgRepTel[intOrgRepTelContactNo] = vRows[p].ContactNo;
                    arOrgRepTel[intOrgRepTelExtNo] = vRows[p].ExtensionNo;
                    arOrgRepTel[intOrgRepTelContactType] = vRows[p].ContactType;
                    arOrgRepTel[intOrgRepTelState] = "";

                    sessionStorage["OrgRepTel-" + p] = JSON.stringify(arOrgRepTel);

                };

                intOrgRepTelCntr = vRows.length;
                LoadOrgRepContactTelList();

            };

            function LoadOrgRepContactMobile(vRows) {

                for (var p = 0; p <= vRows.length - 1; p++) {

                    arOrgRepMobile = new Array();
                    arOrgRepMobile[intOrgRepMobileRecID] = vRows[p].ID;
                    arOrgRepMobile[intOrgRepMobileCountryID] = vRows[p].CountryID;
                    arOrgRepMobile[intOrgRepMobileCountryCode] = vRows[p].Country;
                    arOrgRepMobile[intOrgRepMobileCallingCode] = vRows[p].CountryCode;
                    arOrgRepMobile[intOrgRepMobileTelCoPrefixID] = vRows[p].TelCoPrefixID;
                    arOrgRepMobile[intOrgRepMobilePrefix] = vRows[p].Prefix;
                    arOrgRepMobile[intOrgRepMobileTelCo] = vRows[p].TelCo;
                    arOrgRepMobile[intOrgRepMobileContactNo] = vRows[p].ContactNo;
                    arOrgRepMobile[intOrgRepMobileContactType] = vRows[p].ContactType;
                    arOrgRepMobile[intOrgRepMobileState] = "";

                    sessionStorage["OrgRepMobile-" + p] = JSON.stringify(arOrgRepMobile);

                };

                intOrgRepMobileCntr = vRows.length;
                LoadOrgRepContactMobileList();

            };

            function LoadOrgRepContactEmail(vRows) {

                for (var p = 0; p <= vRows.length - 1; p++) {

                    arOrgRepEmail = new Array();
                    arOrgRepEmail[intOrgRepEmailRecID] = vRows[p].ID;
                    arOrgRepEmail[intOrgRepEmailAddress] = vRows[p].EmailAddress;
                    arOrgRepEmail[intOrgRepEmailType] = vRows[p].EmailType;
                    arOrgRepEmail[intOrgRepEmailState] = "";

                    sessionStorage["OrgRepEmail-" + p] = JSON.stringify(arOrgRepEmail);

                };

                intOrgRepEmailCntr = vRows.length;
                LoadOrgRepContactEmailList();

            };

            function ShowOrgRepContactTelInfo(arData, index) {

                //$.mobile.changePage('#pageOrgRepContactTelInfo');
                $('#pageRepList').hide();
                $('#pageOrgRepInfo').hide();
                $('#pageOrgRepContactTelInfo').show();
                $('#pageOrgRepContactMobileInfo').hide();
                $('#pageOrgRepContactEmail').hide();

                ClearOrgRepContactTelFields();

                intOrgRepTelIndex = index;

                $("#txtOrgRepContactTelID").val(arData[intOrgRepTelRecID]);

                $("#txtOrgRepTelCountryID").val(arData[intOrgRepTelCountryID]);
                $("#btnOrgRepTelCountryCode").text(arData[intOrgRepTelCountryCode] + ' - ' + arData[intOrgRepTelCallingCode]).button('refresh');

                $("#txtOrgRepTelAreaCodeID").val(arData[intOrgRepTelAreaCodeID]);
                $("#btnOrgRepTelAreaCode").text(arData[intOrgRepTelArea] + ' - ' + arData[intOrgRepTelAreaCode]).button('refresh');

                $("#txtOrgRepContactNoTel").val(arData[intOrgRepTelContactNo]);
                $("#txtOrgRepContactExtensionTel").val(arData[intOrgRepTelExtNo]);

                $("#selOrgRepContactTypeTel").val(arData[intOrgRepTelContactType]).selectmenu('refresh');

                IsNewOrgRepTelContact = false;

                $("#btnOrgRepContactTelRemove").show();

            };

            function ShowOrgRepContactMobileInfo(arData, index) {

                //$.mobile.changePage('#pageOrgRepContactMobileInfo');
                $('#pageRepList').hide();
                $('#pageOrgRepInfo').hide();
                $('#pageOrgRepContactTelInfo').hide();
                $('#pageOrgRepContactMobileInfo').show();
                $('#pageOrgRepContactEmail').hide();

                ClearOrgRepContactMobileFields();

                intOrgRepMobileIndex = index;

                $("#txtOrgRepContactMobileID").val(arData[intOrgRepMobileRecID] || '');

                $("#txtOrgRepMobileCountryID").val(arData[intOrgRepMobileCountryID] || '');
                $("#btnOrgRepMobileCountryCode").text((arData[intOrgRepMobileCountryCode] + ' - ' + arData[intOrgRepMobileCallingCode]) || 'Country Code').button('refresh');

                $("#txtOrgRepTelCoPrefixID").val(arData[intOrgRepMobileTelCoPrefixID] || '');
                $("#btnOrgRepMobileTelCoPrefixCode").text((arData[intOrgRepMobileTelCo] + ' - ' + arData[intOrgRepMobilePrefix]) || 'Telco Code').button('refresh');

                $("#txtOrgRepContactNoMobile").val(arData[intOrgRepMobileContactNo] || '');
                $("#selOrgRepContactTypeMobile").val(arData[intOrgRepMobileContactType] || '').selectmenu('refresh');

                IsNewOrgRepMobileContact = false;

                $("#btnOrgRepContactMobileRemove").show();

            };

            function ShowOrgRepContactEmailInfo(arData, index) {

                //$.mobile.changePage('#pageOrgRepContactEmail');
                $('#pageRepList').hide();
                $('#pageOrgRepInfo').hide();
                $('#pageOrgRepContactTelInfo').hide();
                $('#pageOrgRepContactMobileInfo').hide();
                $('#pageOrgRepContactEmail').show();

                ClearOrgRepContactEmailFields();

                intOrgRepEmailIndex = index;

                $("#txtOrgRepContactEmailID").val(arData[intOrgRepEmailRecID] || '');

                $("#txtOrgRepContactEmail").val(arData[intOrgRepEmailAddress] || '');
                $("#selOrgRepContactTypeEmail").val(arData[intOrgRepEmailType] || '').selectmenu('refresh');

                IsNewOrgRepEmailContact = false;

                $("#btnOrgRepContactEmailRemove").show();


            };

            function ClearOrgRepFields() {

                // dont clear
                // sessionStorage.clear();
                proj.ClearSessionStorage();
                sessionStorage["LibraryName"] = libname;
                sessionStorage["LabelLibraryName"] = lbllibname;
                sessionStorage["AddViewLibBCName"] = addlibname;
                sessionStorage["ViewID"] = repid;

                $("#txtOrgRepID").val('');

                $("#txtOrgRepTitle").val('');
                $("#txtOrgRepLastName").val('');
                $("#txtOrgRepFirstName").val('');
                $("#txtOrgRepMiddleName").val('');
                $("#selOrgRepGenSuffixName").val('').selectmenu('refresh');

                $("#selOrgRepIsActive").val('1').slider("refresh");

            };

            function ClearOrgRepContactTelFields() {

                $("#txtOrgRepContactTelID").val('');

                $("#txtOrgRepTelCountryID").val('');
                $("#btnOrgRepTelCountryCode").text('Country Code').button('refresh');

                $("#txtOrgRepTelAreaCodeID").val('');
                $("#btnOrgRepTelAreaCode").text('Area Code').button('refresh');

                $("#txtOrgRepContactNoTel").val('');
                $("#txtOrgRepContactExtensionTel").val('');

                $("#selOrgRepContactTypeTel").val('').selectmenu('refresh');

                $("#btnOrgRepContactTelRemove").hide();

            }

            function ClearOrgRepContactMobileFields() {
                $("#txtOrgRepContactMobileID").val('');

                $("#txtOrgRepMobileCountryID").val('');
                $("#btnOrgRepMobileCountryCode").text('Country Code').button('refresh');

                $("#txtOrgRepTelCoPrefixID").val('');
                $("#btnOrgRepMobileTelCoPrefixCode").text('Telco Code').button('refresh');

                $("#txtOrgRepContactNoMobile").val('');
                $("#selOrgRepContactTypeMobile").val('').selectmenu('refresh');

                $("#btnOrgRepContactMobileRemove").hide();

            }

            function ClearOrgRepContactEmailFields() {
                $("#txtOrgRepContactEmailID").val('');

                $("#txtOrgRepContactEmail").val('');
                $("#selOrgRepContactTypeEmail").val('').selectmenu('refresh');

                $("#btnOrgRepContactEmailRemove").hide();

            }

            function SaveOrgRep() {
                var pData = [];
                var pDataTel = [];
                var pDataMobile = [];
                var pDataEmail = [];
                var intIndex;

                //Tel
                intIndex = 0;
                for (var i = 0; i <= intOrgRepTelCntr; i++) {

                    if (sessionStorage["OrgRepTel-" + i] != undefined) {

                        arOrgRepTel = new Array();
                        arOrgRepTel = JSON.parse(sessionStorage["OrgRepTel-" + i]);

                        pDataTel[intIndex] = {
                            ID: arOrgRepTel[intOrgRepTelRecID],
                            CountryID: arOrgRepTel[intOrgRepTelCountryID],
                            AreaCodeID: arOrgRepTel[intOrgRepTelAreaCodeID],
                            ContactNo: arOrgRepTel[intOrgRepTelContactNo],
                            ExtensionNo: arOrgRepTel[intOrgRepTelExtNo],
                            ContactType: arOrgRepTel[intOrgRepTelContactType],
                            RecState: arOrgRepTel[intOrgRepTelState]
                        };

                        intIndex = intIndex + 1;

                    }
                }

                //Mobile
                intIndex = 0;
                for (var i = 0; i <= intOrgRepMobileCntr; i++) {

                    if (sessionStorage["OrgRepMobile-" + i] != undefined) {

                        arOrgRepMobile = new Array();
                        arOrgRepMobile = JSON.parse(sessionStorage["OrgRepMobile-" + i]);

                        pDataMobile[intIndex] = {
                            ID: arOrgRepMobile[intOrgRepMobileRecID],
                            CountryID: arOrgRepMobile[intOrgRepMobileCountryID],
                            TelCoPrefixID: arOrgRepMobile[intOrgRepMobileTelCoPrefixID],
                            ContactNo: arOrgRepMobile[intOrgRepMobileContactNo],
                            ContactType: arOrgRepMobile[intOrgRepMobileContactType],
                            RecState: arOrgRepMobile[intOrgRepMobileState]
                        };

                        intIndex = intIndex + 1;

                    }
                }

                //Email
                intIndex = 0;
                for (var i = 0; i <= intOrgRepEmailCntr; i++) {

                    if (sessionStorage["OrgRepEmail-" + i] != undefined) {

                        arOrgRepEmail = new Array();
                        arOrgRepEmail = JSON.parse(sessionStorage["OrgRepEmail-" + i]);

                        pDataEmail[intIndex] = {
                            ID: arOrgRepEmail[intOrgRepEmailRecID],
                            EmailAddress: arOrgRepEmail[intOrgRepEmailAddress],
                            EmailType: arOrgRepEmail[intOrgRepEmailType],
                            RecState: arOrgRepEmail[intOrgRepEmailState]
                        };

                        intIndex = intIndex + 1;

                    }
                }

                pData = {
                    OrgRepID: $("#txtOrgRepID").val(),
                    LastName: $("#txtOrgRepLastName").val(),
                    FirstName: $.trim($("#txtOrgRepFirstName").val()),
                    MiddleName: $.trim($("#txtOrgRepMiddleName").val()),
                    Title: $.trim($("#txtOrgRepTitle").val()),
                    GenerationalSuffix: $.trim($("#selOrgRepGenSuffixName").val()),
                    IsActive: $("#selOrgRepIsActive").val(),

                    OrgRepContactTelList: pDataTel,
                    OrgRepContactMobileList: pDataMobile,
                    OrgRepContactEmailList: pDataEmail

                };

                AjaxSend({
                    url: "OrganizationShort/SaveOrgRep",
                    data: pData,
                    success: function (vData) {
                        //                        window.IsPageLoaded = false;

                        //                        window.location = MergeUrlParameters({
                        //                            OrgID: sessionStorage.OrgID,
                        //                            IsMobile: CBool(IsMobile) ? 1 : 0
                        //                        }, '/OrganizationShort/OrganizationRepresentative');

                        $('#pageRepList').show();
                        $('#pageOrgRepInfo').hide();
                        $('#pageOrgRepContactTelInfo').hide();
                        $('#pageOrgRepContactMobileInfo').hide();
                        $('#pageOrgRepContactEmail').hide();
                        getOrgRepresentativeList();
                        //////                        ChangePage('/OrganizationShort/OrganizationRepresentative');
                        //////                        getOrgRepresentativeList();

                    }
                });

            };

            function SaveOrgRepContactTel() {
                var arTmp;

                arOrgRepTel = new Array();
                arOrgRepTel[intOrgRepTelRecID] = $("#txtOrgRepContactTelID").val();

                arTmp = $("#btnOrgRepTelCountryCode").text().split("-")
                arOrgRepTel[intOrgRepTelCountryID] = $("#txtOrgRepTelCountryID").val();
                arOrgRepTel[intOrgRepTelCountryCode] = $.trim(arTmp[0]);
                arOrgRepTel[intOrgRepTelCallingCode] = $.trim(arTmp[1]);

                arTmp = $("#btnOrgRepTelAreaCode").text().split("-")
                arOrgRepTel[intOrgRepTelAreaCodeID] = $("#txtOrgRepTelAreaCodeID").val();
                arOrgRepTel[intOrgRepTelArea] = $.trim(arTmp[0]);
                arOrgRepTel[intOrgRepTelAreaCode] = $.trim(arTmp[1]);

                arOrgRepTel[intOrgRepTelContactNo] = $("#txtOrgRepContactNoTel").val();
                arOrgRepTel[intOrgRepTelExtNo] = $("#txtOrgRepContactExtensionTel").val();
                arOrgRepTel[intOrgRepTelContactType] = $("#selOrgRepContactTypeTel").val();
                arOrgRepTel[intOrgRepTelState] = (IsNewOrgRepTelContact ? "New" : "Edit");

                sessionStorage["OrgRepTel-" + intOrgRepTelIndex] = JSON.stringify(arOrgRepTel);

                //$.mobile.changePage('#pageOrgRepInfo');
                $('#pageRepList').hide();
                $('#pageOrgRepInfo').show();
                $('#pageOrgRepContactTelInfo').hide();
                $('#pageOrgRepContactMobileInfo').hide();
                $('#pageOrgRepContactEmail').hide();

                LoadOrgRepContactTelList();

            };

            function SaveOrgRepContactMobile() {
                var arTmp;

                arOrgRepMobile = new Array();

                arOrgRepMobile[intOrgRepMobileRecID] = $("#txtOrgRepContactMobileID").val();
                arOrgRepMobile[intOrgRepMobileCountryID] = $("#txtOrgRepMobileCountryID").val();
                arTmp = $("#btnOrgRepMobileCountryCode").text().split("-")
                arOrgRepMobile[intOrgRepMobileCountryID] = $("#txtOrgRepMobileCountryID").val();
                arOrgRepMobile[intOrgRepMobileCountryCode] = $.trim(arTmp[0]);
                arOrgRepMobile[intOrgRepMobileCallingCode] = $.trim(arTmp[1]);

                arTmp = $("#btnOrgRepMobileTelCoPrefixCode").text().split("-")
                arOrgRepMobile[intOrgRepMobileTelCoPrefixID] = $("#txtOrgRepTelCoPrefixID").val();
                arOrgRepMobile[intOrgRepMobilePrefix] = $.trim(arTmp[1]);
                arOrgRepMobile[intOrgRepMobileTelCo] = $.trim(arTmp[0]);

                arOrgRepMobile[intOrgRepMobileContactNo] = $("#txtOrgRepContactNoMobile").val();
                arOrgRepMobile[intOrgRepMobileContactType] = $("#selOrgRepContactTypeMobile").val();
                arOrgRepMobile[intOrgRepMobileState] = (IsNewOrgRepMobileContact ? "New" : "Edit");

                sessionStorage["OrgRepMobile-" + intOrgRepMobileIndex] = JSON.stringify(arOrgRepMobile);

                //$.mobile.changePage('#pageOrgRepInfo');
                $('#pageRepList').hide();
                $('#pageOrgRepInfo').show();
                $('#pageOrgRepContactTelInfo').hide();
                $('#pageOrgRepContactMobileInfo').hide();
                $('#pageOrgRepContactEmail').hide();

                LoadOrgRepContactMobileList();
            };

            function SaveOrgRepContactEmail() {

                arOrgRepEmail = new Array();
                arOrgRepEmail[intOrgRepEmailRecID] = $("#txtOrgRepContactEmailID").val();
                arOrgRepEmail[intOrgRepEmailAddress] = $("#txtOrgRepContactEmail").val();
                arOrgRepEmail[intOrgRepEmailType] = $("#selOrgRepContactTypeEmail").val();
                arOrgRepEmail[intOrgRepEmailState] = (IsNewOrgRepEmailContact ? "New" : "Edit");

                sessionStorage["OrgRepEmail-" + intOrgRepEmailIndex] = JSON.stringify(arOrgRepEmail);

                //$.mobile.changePage('#pageOrgRepInfo');
                $('#pageRepList').hide();
                $('#pageOrgRepInfo').show();
                $('#pageOrgRepContactTelInfo').hide();
                $('#pageOrgRepContactMobileInfo').hide();
                $('#pageOrgRepContactEmail').hide();

                LoadOrgRepContactEmailList();
            };

            function RemoveOrgRepContactTel() {
                var arTmp;

                arOrgRepTel = new Array();
                arOrgRepTel[intOrgRepTelRecID] = $("#txtOrgRepContactTelID").val();

                arTmp = $("#btnOrgRepTelCountryCode").text().split("-")
                arOrgRepTel[intOrgRepTelCountryID] = $("#txtOrgRepTelCountryID").val();
                arOrgRepTel[intOrgRepTelCountryCode] = $.trim(arTmp[0]);
                arOrgRepTel[intOrgRepTelCallingCode] = $.trim(arTmp[1]);

                arTmp = $("#btnOrgRepTelAreaCode").text().split("-")
                arOrgRepTel[intOrgRepTelAreaCodeID] = $("#txtOrgRepTelAreaCodeID").val();
                arOrgRepTel[intOrgRepTelArea] = $.trim(arTmp[0]);
                arOrgRepTel[intOrgRepTelAreaCode] = $.trim(arTmp[1]);

                arOrgRepTel[intOrgRepTelContactNo] = $("#txtOrgRepContactNoTel").val();
                arOrgRepTel[intOrgRepTelExtNo] = $("#txtOrgRepContactExtensionTel").val();
                arOrgRepTel[intOrgRepTelContactType] = $("#selOrgRepContactTypeTel").val();
                arOrgRepTel[intOrgRepTelState] = "Remove";

                sessionStorage["OrgRepTel-" + intOrgRepTelIndex] = JSON.stringify(arOrgRepTel);

                //$.mobile.changePage('#pageOrgRepInfo');
                $('#pageRepList').hide();
                $('#pageOrgRepInfo').show();
                $('#pageOrgRepContactTelInfo').hide();
                $('#pageOrgRepContactMobileInfo').hide();
                $('#pageOrgRepContactEmail').hide();

                LoadOrgRepContactTelList();
            };

            function RemoveOrgRepContactMobile() {
                var arTmp;

                arOrgRepMobile = new Array();

                arOrgRepMobile[intOrgRepMobileRecID] = $("#txtOrgRepContactMobileID").val();

                arTmp = $("#btnOrgRepMobileCountryCode").text().split("-")
                arOrgRepMobile[intOrgRepMobileCountryID] = $("#txtOrgRepMobileCountryID").val();
                arOrgRepMobile[intOrgRepMobileCountryCode] = $.trim(arTmp[0]);
                arOrgRepMobile[intOrgRepMobileCallingCode] = $.trim(arTmp[1]);

                arTmp = $("#btnOrgRepMobileTelCoPrefixCode").text().split("-")
                arOrgRepMobile[intOrgRepMobileTelCoPrefixID] = $("#txtOrgRepTelCoPrefixID").val();
                arOrgRepMobile[intOrgRepMobilePrefix] = $.trim(arTmp[1]);
                arOrgRepMobile[intOrgRepMobileTelCo] = $.trim(arTmp[0]);

                arOrgRepMobile[intOrgRepMobileContactNo] = $("#txtOrgRepContactNoMobile").val();
                arOrgRepMobile[intOrgRepMobileContactType] = $("#selOrgRepContactTypeMobile").val();
                arOrgRepMobile[intOrgRepMobileState] = "Remove";

                sessionStorage["OrgRepMobile-" + intOrgRepMobileIndex] = JSON.stringify(arOrgRepMobile);

                //$.mobile.changePage('#pageOrgRepInfo');
                $('#pageRepList').hide();
                $('#pageOrgRepInfo').show();
                $('#pageOrgRepContactTelInfo').hide();
                $('#pageOrgRepContactMobileInfo').hide();
                $('#pageOrgRepContactEmail').hide();

                LoadOrgRepContactMobileList();
            };

            function RemoveOrgRepContactEmail() {

                arOrgRepEmail = new Array();
                arOrgRepEmail[intOrgRepEmailRecID] = $("#txtOrgRepContactEmailID").val();
                arOrgRepEmail[intOrgRepEmailAddress] = $("#txtOrgRepContactEmail").val();
                arOrgRepEmail[intOrgRepEmailType] = $("#selOrgRepContactTypeEmail").val();
                arOrgRepEmail[intOrgRepEmailState] = "Remove";

                sessionStorage["OrgRepEmail-" + intOrgRepEmailIndex] = JSON.stringify(arOrgRepEmail);

                //$.mobile.changePage('#pageOrgRepInfo');
                $('#pageRepList').hide();
                $('#pageOrgRepInfo').show();
                $('#pageOrgRepContactTelInfo').hide();
                $('#pageOrgRepContactMobileInfo').hide();
                $('#pageOrgRepContactEmail').hide();

                LoadOrgRepContactEmailList();
            };

            function LoadOrgRepContactTelList() {
                var pLV = $('#lstOrgRepContactTel');
                var pItem;

                pLV.html('');
                for (var i = 0; i <= intOrgRepTelCntr; i++) {

                    if (sessionStorage["OrgRepTel-" + i] != undefined) {

                        arOrgRepTel = new Array();
                        arOrgRepTel = JSON.parse(sessionStorage["OrgRepTel-" + i]);

                        if (arOrgRepTel[intOrgRepTelState] != "Remove") {

                            pItem = $('<li id="lst' + i + '">')
                            .append($('<a>')
                                    .attr('ID', i)
                                    .data('Data', arOrgRepTel)
                                    .click(function (e) {
                                        e.preventDefault();
                                        ShowOrgRepContactTelInfo($(this).data('Data'), $(this).attr('ID'));
                                    })
                                    .append($('<h3>')
                                        .attr("style", "white-space:normal")
                                        .text("+" + arOrgRepTel[intOrgRepTelCallingCode] + "." + arOrgRepTel[intOrgRepTelAreaCode] + '.' + arOrgRepTel[intOrgRepTelContactNo] + ((arOrgRepTel[intOrgRepTelExtNo] || '') == '' ? '' : ", Ext. " + arOrgRepTel[intOrgRepTelExtNo]))
                                    )
                                    .append($('<p>')
                                        .attr("style", "white-space:normal")
                                        .text(arOrgRepTel[intOrgRepTelArea] + ", " + arOrgRepTel[intOrgRepTelCountryCode])
                                    )
                                    .append($('<p>')
                                        .attr("style", "white-space:normal")
                                        .text(arOrgRepTel[intOrgRepTelContactType])
                                    )
                            );

                            pLV.append(pItem);

                        }

                    }
                }

                pLV.listview('refresh');
            };

            function LoadOrgRepContactMobileList() {
                var pLV = $('#lstOrgRepContactMob');
                var pItem;

                pLV.html('');
                for (var i = 0; i <= intOrgRepMobileCntr; i++) {

                    if (sessionStorage["OrgRepMobile-" + i] != undefined) {

                        arOrgRepMobile = new Array();
                        arOrgRepMobile = JSON.parse(sessionStorage["OrgRepMobile-" + i]);

                        if (arOrgRepMobile[intOrgRepMobileState] != "Remove") {

                            pItem = $('<li id="lst' + i + '">')
                            .append($('<a>')
                                    .attr('ID', i)
                                    .data('Data', arOrgRepMobile)
                                    .click(function (e) {
                                        e.preventDefault();
                                        ShowOrgRepContactMobileInfo($(this).data('Data'), $(this).attr('ID'));
                                    })
                                    .append($('<h3>')
                                        .attr("style", "white-space:normal")
                                        .text("+" + arOrgRepMobile[intOrgRepMobileCallingCode] + "." + arOrgRepMobile[intOrgRepMobilePrefix] + "." + arOrgRepMobile[intOrgRepMobileContactNo])
                                    )
                                    .append($('<p>')
                                        .attr("style", "white-space:normal")
                                        .text(arOrgRepMobile[intOrgRepMobileTelCo] + " (" + arOrgRepMobile[intOrgRepMobileCountryCode] + ")")
                                    )
                                    .append($('<p>')
                                        .attr("style", "white-space:normal")
                                        .text(arOrgRepMobile[intOrgRepMobileContactType])
                                    )
                            );


                            pLV.append(pItem);

                        }

                    }
                }

                pLV.listview('refresh');
            };


            function LoadOrgRepContactEmailList() {
                var pLV = $('#lstOrgRepContactEmail');
                var pItem;

                pLV.html('');
                for (var i = 0; i <= intOrgRepEmailCntr; i++) {

                    if (sessionStorage["OrgRepEmail-" + i] != undefined) {

                        arOrgRepEmail = new Array();
                        arOrgRepEmail = JSON.parse(sessionStorage["OrgRepEmail-" + i]);

                        if (arOrgRepEmail[intOrgRepEmailState] != "Remove") {

                            pItem = $('<li id="lst' + i + '">')
                            .append($('<a>')
                                    .attr('ID', i)
                                    .data('Data', arOrgRepEmail)
                                    .click(function (e) {
                                        e.preventDefault();
                                        ShowOrgRepContactEmailInfo($(this).data('Data'), $(this).attr('ID'));
                                    })
                                    .append($('<h3>')
                                        .attr("style", "white-space:normal")
                                        .text(arOrgRepEmail[intOrgRepEmailAddress])
                                    )
                                    .append($('<p>')
                                        .attr("style", "white-space:normal")
                                        .text(arOrgRepEmail[intOrgRepEmailType])
                                    )
                            );


                            pLV.append(pItem);

                        }

                    }
                }

                pLV.listview('refresh');
            };




        });
    });
