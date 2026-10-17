require([
    'common'
    ], function () {

        $(function () {

            var mLoaded = false;
            var mCreditTermID;
            var mBeginningBalanceAsOf;
            var mSupplierActivationDate;

            setMonthCollection("selOrgSupDateInActiveMonth");
            setYearCollection("selOrgSupDateInActiveYear");
            
            $.mobile.activePage.bind('pageshow', PageShow);

            window.OrgSup = {
                SetReqData: function () {
                    setMonthCollection("selOrgSupDateInActiveMonth");
                    setYearCollection("selOrgSupDateInActiveYear");
                },
                ShowOrgSupplierInfo: function () {
                    setMonthCollection("selOrgSupDateInActiveMonth");
                    setYearCollection("selOrgSupDateInActiveYear");
                    ShowOrgSupplierInfo(sessionStorage.ViewID);
                }
            }

            function PageShow() {
                mLoaded = true;
                //getOrgSupplierList();
                if (sessionStorage.ViewID != undefined) {
                    ShowOrgSupplierInfo(sessionStorage.ViewID);
                }
            };
            if (!mLoaded) PageShow();

            $('body').on('click', '#btnOrgSupplierInfoSave', function (e) {
                e.preventDefault();

                if ($.trim($("#txtOrgSupSupplierID").val()) == "") {
                    //                    MessageBox("Please select supplier.",
                    //                                "Supplier",
                    //                                [
                    //                                    { Text: 'Ok'
                    //                                    }
                    //                                ]
                    //                    );
                    $('#TitleOrgEmp').text('Supplier');
                    $('#MsgOrgEmp').text('Please select supplier.');
                    $('#panOrgEmpMessage').popup('open');
                    return;
                }

                if (!IsDate(mSupplierActivationDate)) {
                    //                    MessageBox("Please enter valid activation date.",
                    //                            "Activation Date",
                    //                            [
                    //                                { Text: 'Ok' }
                    //                            ]
                    //                        );
                    $('#TitleOrgEmp').text('Activation Date');
                    $('#MsgOrgEmp').text('Please enter valid activation date.');
                    $('#panOrgEmpMessage').popup('open');
                    return;
                }

                if (!$("#selOrgSupDeactivate").is(':checked')) {

                    ClearOrgSupplierInActiveFields();

                }
                else {

                    if ($.trim($("#selOrgSupDateInActiveMonth").val()) == "" || $.trim($("#selOrgSupDateInActiveDay").val()) == "" || $.trim($("#selOrgSupDateInActiveYear").val()) == "") {
                        //                        MessageBox("Please enter valid deactivation date.",
                        //                                "Deactivation Date",
                        //                                [
                        //                                    { Text: 'Ok' }
                        //                                ]
                        //                            );
                        $('#TitleOrgEmp').text('Deactivation Date');
                        $('#MsgOrgEmp').text('Please enter valid deactivation date.');
                        $('#panOrgEmpMessage').popup('open');
                        return;
                    }

                }

                SaveOrganizationSupplier();
            })
        .on('change', '#selOrgSupDateInActiveMonth', function (e) {
            e.preventDefault();

            setDayCollection('selOrgSupDateInActiveMonth', 'selOrgSupDateInActiveDay', 'selOrgSupDateInActiveYear', true);
        })
        .on('change', '#selOrgSupDateInActiveYear', function (e) {
            e.preventDefault();

            setDayCollection('selOrgSupDateInActiveMonth', 'selOrgSupDateInActiveDay', 'selOrgSupDateInActiveYear', true);
        })
        .on('change', '#selOrgSupDeactivate', function (e) {
            e.preventDefault();

            ShowHideOrgSupInActiveDetails();
        })
        .on('click', '#btnOrgSupplierInfoCancel', function (e) {
            e.preventDefault();

            //$.mobile.changePage('#pageOrgSupplier');
            window.location.hash = 'LibraryPage';
        })
        .on('click', '#btnOrgSupSupplier', function () {
            BrowsePopup('LibraryOrganization', 'Supplier',
                    [
                        {
                            AutoSearch: true,
                            DisplayText: [{ Column: 'CompanyName'}],
                            Click: function (e) {
                                $('#txtOrgSupSupplierID').val(e.ID)
                                $('#btnOrgSupSupplier').text(e.CompanyName)
                                ;//.button('refresh');
                            }
                        }
                    ], { ShowAdd: true })

            //                Browser.LibraryOrganization.InputData.ShowAdd = true;
            //                Browser.LibraryOrganization.Click = function (e) {

            //                    $('#txtOrgSupSupplierID').val(e.ID)
            //                    $('#btnOrgSupSupplier').text(e.CompanyName)
            //                ;//.button('refresh');

            //                };

            //                Browser.LibraryOrganization.Show();
        })
        .on('click', '#butOrgSupplierCreditTerm', function () {
            BrowsePopup('LibraryPaymentTerms', 'Search Payment Terms',
                [
                    {
                        DisplayText: [{ Column: 'Name' }],
                        AutoSearch: true,
                        Click: function (e) {
                            mCreditTermID = e.ID;
                            $('#butOrgSupplierCreditTerm').text(e.Name);//.button('refresh');
                        }
                    }
                ]);
        })
        .on('click', '#butOrgSupplierBeginningBalanceAsOf', function () {
            SingleCalendar({
                Click: function(e){
                    $('#butOrgSupplierBeginningBalanceAsOf').text(FormatDate(e, {DontDisplay12Am: true}));//.button('refresh');
                    mBeginningBalanceAsOf = e;
                }
            });
        })
        .on('click', '#butOrgSupActivationDate', function () {
            SingleCalendar({
                Click: function(e){
                    $('#butOrgSupActivationDate').text(FormatDate(e, {DontDisplay12Am: true}));//.button('refresh');
                    mSupplierActivationDate = e;
                }
            });
        });

            //Navigation Menu
            $('#btnHome').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu"), "_self");
            });

            $('#btnSystemAdmin').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu#pagePopUpMenuSettings"), "_self");
            });

            $('#btnDataLibraries').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu#pagePopDataLibraries"), "_self");
            });
            //End Navigation Menu

            $('#btnOrgSupAddSupplier').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pageOrgSupplierInfo');
                ClearOrgSupplierFields();
                ClearOrgSupplierInActiveFields();

                ShowHideOrgSupInActiveDetails();
            });

            $('#btnOrgSupplierDone').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu"), "_self");

                //////            if (window.pageSourcePage != undefined) {
                //////                $.mobile.changePage("#" + $(window.pageSourcePage).attr('id'));
                //////            }
                //////            else {
                //////              //  ChangePage('/Organization/OrganizationSummary');
                //////                window.location = MergeUrlParameters({
                //////                    OrgID: sessionStorage.OrgID,
                //////                    IsMobile: CBool(IsMobile) ? 1 : 0
                //////                }, '/Organization/OrganizationSummary');
                //////            }

            });

            $('#btnOrgSupplierGoToSetupChecklist').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/SetupChecklist"), "_self");

            });

            function ShowHideOrgSupInActiveDetails() {

                if ($("#selOrgSupDeactivate").is(':checked')) {
                    $("#divOrgSupDeactivateDetails").show();
                }
                else {
                    $("#divOrgSupDeactivateDetails").hide();
                }
            }

            function getOrgSupplierList() {
                AjaxSend({
                    url: 'Organization/getOrganizationSupplierList',
                    success: function (vData) {
                        LoadOrgSupplierList(vData.OrgSupplierData.Rows);
                    }
                });
            }

            function LoadOrgSupplierList(vRows) {
                var pLV = $('#lstOrgSupplier');
                var pItem;

                $("#divOrgSupRecordCountSupplier").text(" Number of record(s) found: " + vRows.length);

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li>')
                    .append($('<a>')
                        .attr('ID', vRows[p].ID)
                        .click(function (e) {
                            e.preventDefault();
                            $.mobile.changePage('#pageOrgSupplierInfo');
                            ShowOrgSupplierInfo($(this).attr('ID'));
                        })
                        .append($('<h3>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].SupplierName)
                        )
                        .append($('<p>')
                            .attr("style", "white-space:normal")
                            .text("Activation Date: " + $.format.date(new Date(vRows[p].ActivationDate), 'MMM dd, yyyy'))
                        )
                        .append($('<p>')
                            .attr("style", "white-space:normal")
                            .text("Status: " + (vRows[p].DeActivated != '0' ? 'Deactivated ( ' + $.format.date(new Date(vRows[p].DeactivationDate), 'MMM dd, yyyy') + ' )' : 'Active'))
                        )
                        .append($('<p>')
                            .attr("style", "white-space:normal")
                            .text("Banned: " + (vRows[p].IsBan != '0' ? 'Yes' : 'No'))
                        )
                    );

                    pLV.append(pItem);
                };

                pLV.listview('refresh');
            };

            function ShowOrgSupplierInfo(strID) {
                var pData = {};
                pData.ID = strID;

                ClearOrgSupplierFields();
                ClearOrgSupplierInActiveFields();

                AjaxSend({
                    url: "Organization/getOrganizationSupplierInfo",
                    data: pData,
                    success: function (vData) {
                        if (vData.OrgSupplierData.Rows.length > 0) {
                            var pDR = vData.OrgSupplierData.Rows[0];
                            DisplayData(pDR);

                        }
                    }
                });

            };

            proj.SupplierDisplayData = DisplayData;
            function DisplayData(vDR) {
                var pDR = vDR;
                if (pDR == null)
                    return;
                $("#txtOrgSupplierID").val(pDR.ID || '');

                $("#txtOrgSupSupplierID").val(pDR.SupplierID || '');
                $("#btnOrgSupSupplier").text(pDR.SupplierName || 'Please Select');//.button('refresh');

                mSupplierActivationDate = pDR.ActivationDate;
                if (IsDate(mSupplierActivationDate))
                    $('#butOrgSupActivationDate').text(FormatDate(mSupplierActivationDate, {DontDisplay12Am: true}));//.button('refresh');
                    
                $("#selOrgSupDeactivate").prop('checked', CInt2(pDR.DeActivated || '0'));//.slider("refresh");

                if ($("#selOrgSupDeactivate").is(':checked')) {
                    $("#selOrgSupDateInActiveMonth").val(pDR.DeactivationDateMonth || '');//.selectmenu('refresh');
                    $("#selOrgSupDateInActiveYear").val(pDR.DeactivationDateYear || '');//.selectmenu('refresh');
                    setDayCollection('selOrgSupDateInActiveMonth', 'selOrgSupDateInActiveDay', 'selOrgSupDateInActiveYear', true);
                    $("#selOrgSupDateInActiveDay").val(pDR.DeactivationDateDay || '');//.selectmenu('refresh');

                    $("#txtOrgSupInActiveReason").val(pDR.DeactivationReason || '').keyup();
                    $("#txtOrgSupComments").val(pDR.Comments || '').keyup();

                    $("#selOrgSupBan").prop('checked', CInt2(pDR.IsBan || '0'));//.slider("refresh");
                }

                mCreditTermID = pDR.CreditTermsID;
                $('#butOrgSupplierCreditTerm').text(pDR.CreditTermsName);//.button('refresh');
                $("#txtOrgSupplierCreditLimit").val(AccountingFormat(pDR.CreditLimit));
                $("#txtOrgSupplierBeginningBalance").val(AccountingFormat(pDR.CreditBegBalance));
                mBeginningBalanceAsOf = pDR.CreditBalanceAsOf;
                if (IsDate(mBeginningBalanceAsOf))
                    $('#butOrgSupplierBeginningBalanceAsOf').text(FormatDate(mBeginningBalanceAsOf, {DontDisplay12Am: true}));//.button('refresh');

                ShowHideOrgSupInActiveDetails();
            }

            proj.GetSupplierInfoFromHtml = GetSupplierInfoFromHtml;
            function GetSupplierInfoFromHtml() {
                var pData = {
                    OrgSupplierID: $.trim($("#txtOrgSupplierID").val()),
                    SupplierID: $.trim($("#txtOrgSupSupplierID").val()),
                    ActivationDate: (mSupplierActivationDate || new Date()),
                    IsDeactivated: CInt2($("#selOrgSupDeactivate").is(':checked')),
                    DeactivationDate: $("#selOrgSupDateInActiveMonth").val() + '/' + $("#selOrgSupDateInActiveDay").val() + '/' + $("#selOrgSupDateInActiveYear").val(),
                    DeactivationReason: $("#txtOrgSupInActiveReason").val(),
                    Comments: $("#txtOrgSupComments").val(),
                    IsBan: CInt2($("#selOrgSupBan").is(':checked')),
                    CreditTermsID: mCreditTermID,
                    CreditLimit: CDec2($("#txtOrgSupplierCreditLimit").val()),
                    CreditBegBalance: CDec2($("#txtOrgSupplierBeginningBalance").val()),
                    CreditBalanceAsOf: mBeginningBalanceAsOf
                };

                return pData;
            }

            function SaveOrganizationSupplier() {
                var pData = [];

                pData = GetSupplierInfoFromHtml();

                AjaxSend({
                    url: "Organization/CheckOrganizationSupplier",
                    data: pData,
                    success: function (vData) {

                        if (vData.OrgSupplierData.Rows.length > 0) {
                            //                        MessageBox("Supplier being added already exist.",
                            //                            "Supplier Exist",
                            //                            [
                            //                                { Text: 'Ok' }
                            //                            ]
                            //                        );
                            $('#TitleOrgEmp').text('Supplier Exist');
                            $('#MsgOrgEmp').text('Supplier being added already exist.');
                            $('#panOrgEmpMessage').popup('open');
                        }
                        else {
                            AjaxSend({
                                url: "Organization/SaveOrganizationSupplier",
                                data: pData,
                                success: function (vSaveData) {

                                    if (vSaveData.OrgSupplierData == "ActivationDateError") {
                                        //                                        MessageBox("Please enter valid activation date.",
                                        //                                            "Activation Date",
                                        //                                            [
                                        //                                                { Text: 'Ok' }
                                        //                                            ]
                                        //                                        );
                                        $('#TitleOrgEmp').text('Activation Date');
                                        $('#MsgOrgEmp').text('Please enter valid activation date.');
                                        $('#panOrgEmpMessage').popup('open');
                                    }
                                    else if (vSaveData.OrgSupplierData == "DeactivationDateError") {
                                        //                                        MessageBox("Please enter valid deactivation date.",
                                        //                                            "Deactivation Date",
                                        //                                            [
                                        //                                                { Text: 'Ok' }
                                        //                                            ]
                                        //                                        );
                                        $('#TitleOrgEmp').text('Deactivation Date');
                                        $('#MsgOrgEmp').text('Please enter valid deactivation date.');
                                        $('#panOrgEmpMessage').popup('open');
                                    }
                                    else {
                                        //$.mobile.changePage('#pageOrgSupplier');
                                        //getOrgSupplierList();
                                        $('#tblBody').html('');
                                        window.location.hash = '';
                                        window.location.hash = 'LibraryPage';
                                        LibraryTable.SupplierList();
                                    }

                                }
                            });

                        }

                    }
                });

            };

            proj.SupplierClearFields = function() {
                ClearOrgSupplierFields();
                ClearOrgSupplierInActiveFields();
            };

            proj.VerifySupplier = function() {
                if (sessionStorage.LibraryName != 'Supplier / Vendor List')
                    return true;

                // dapat dili ni required by arnie 5-2-2018
                //if (!IsDate(mSupplierActivationDate)) {
                //    NewMessageBox('Please enter valid activation date.', 'Activation Date',
                //        [
                //            {
                //                Text: 'Ok',
                //                Click: function () {
                //                    $(window).scrollTop(0);
                //                }
                //            }
                //        ]
                //    );
                //    return false;
                //}

                if (CStr2(mSupplierActivationDate) == '') {
                    mSupplierActivationDate = $.format.date(new Date(), "MM/dd/yyyy"); // set default activation date
                }                

                return true;
            };

            function ClearOrgSupplierFields() {

                mCreditTermID = null;
                mBeginningBalanceAsOf = null;
                mSupplierActivationDate = null;
                $("#txtOrgSupplierID").val('');

                $("#txtOrgSupSupplierID").val('');
                $("#btnOrgSupSupplier").text('Please Select');//.button('refresh');

                $('#butOrgSupplierCreditTerm').text('Please select');//.button('refresh');
                $("#txtOrgSupplierCreditLimit").val('');
                $("#txtOrgSupplierBeginningBalance").val('');
                $('#butOrgSupplierBeginningBalanceAsOf').text('Please Select');//.button('refresh');

            };

            function ClearOrgSupplierInActiveFields() {

                $("#selOrgSupDeactivate").prop('checked', 0);//.slider("refresh");

                $("#selOrgSupDateInActiveMonth").val('');//.selectmenu('refresh');
                $("#selOrgSupDateInActiveDay").val('');//.selectmenu('refresh');
                $("#selOrgSupDateInActiveYear").val('');//.selectmenu('refresh');

                $("#txtOrgSupInActiveReason").val('');
                $("#txtOrgSupComments").val('');

                $("#selOrgSupBan").prop('checked', 0);//.slider("refresh");
            };

        });
    });