require([
    'common'
    ], function () {

        $(function () {

        $("#btnAddOrgAffiliates > span").attr("style", "text-align:left;white-space:normal")

        var mLoaded = false;
        var isIE = false || !!document.documentMode;

        $.mobile.activePage.bind('pageshow', PageShow);
        function PageShow() {
            mLoaded = true;

            if (!window.IsPageLoaded) {
                ShowOrgAffiliationInfo();
                window.IsPageLoaded = true;
            }
        };
        if (!mLoaded) PageShow();

        window.OrgAff = {
            ShowInfo: function () {
                ShowOrgAffiliationInfo();
            }
        }

        $('body')
        .on('click', '#btnOrgAffHoldingCompany', function () {
            BrowsePopup('LibraryOrganization', 'Affiliate or Subsidiary Company', [
                {
                    AutoSearch: true,
                    DisplayText: [
                        { Column: 'CompanyName' }
                    ],
                    Click: function (e) {
                        $('#txtOrgAffHoldingCompanyID').val(e.ID);
                        $('#btnOrgAffHoldingCompany').text(e.CompanyName);//.button('refresh');
                    }
                }
            ])
        })
        .on('click', '#btnAddOrgAffiliates', function () {
            //$.mobile.changePage('#pageOrgAffiliatesInfo');
            $('#pageOrgAffiliationInfo').hide();
            $('#pageOrgAffiliatesInfo').show();
            ClearOrgAffiliateFields();
        })
        .on('click', '#btnOrgAffiliateCompany', function () {
            BrowsePopup('LibraryOrganization', 'Affiliate or Subsidiary Company', [
                {
                    AutoSearch: true,
                    DisplayText: [
                        { Column: 'CompanyName' }
                    ],
                    Click: function (e) {
                        $('#txtOrgAffiliateCompanyID').val(e.ID);
                        $('#btnOrgAffiliateCompany').text(e.CompanyName);//.button('refresh');
                    }
                }
            ])
        })
        .on('click', '#btnOrgAffiliateSave', function (e) {
            e.preventDefault();

            if ($.trim($("#txtOrgAffiliateCompanyID").val()) == "") {
                NewMessageBox("Please select affiliate company.",
                            "Affiliate Company",
                            [
                                { Text: 'Ok' }
                            ]
                );
                return;
            }

            SaveOrgAffiliates();
        })
        .on('click', '#btnOrgAffiliateRemove', function (e) {
            e.preventDefault();

            NewMessageBox("Remove this record?",
                'Remove',
                [
                    { Text: 'Yes',
                        Click: function () {
                            RemoveOrgAffiliate();
                        }
                    }
                ,
                    { Text: 'No'
                    }
                ]
            );
        })
        .on('click', '#btnOrgAffiliateCancel', function (e) {
            e.preventDefault();

            //ChangePage('/Organization/OrganizationAffiliation');
            $('#pageOrgAffiliationInfo').show();
            $('#pageOrgAffiliatesInfo').hide();
        })
        .on('click', '#btnOrgAffiliationDone', function (e) {
            e.preventDefault();

            if ($.trim($("#selOrgAffHasParentCompany").val()) == "0") {

                $("#txtOrgAffHoldingCompanyID").val('');
                $("#btnOrgAffHoldingCompany").text('');//.button('refresh');

            }
            else {
                if ($.trim($("#txtOrgAffHoldingCompanyID").val()) == "") {
                    NewMessageBox("Please select parent or holding company.",
                                "Parent or Holding Company",
                                [
                                    { Text: 'Ok' }
                                ]
                    );
                    return;
                }
            }

            SaveOrgAffiliationInfo();
        })
        .on('change', '#selOrgAffHasParentCompany', function (e) {
            e.preventDefault();

            ShowHideOrgAffParentCompanyDetails();
        })

            function ShowHideOrgAffParentCompanyDetails() {

                if ($.trim($("#selOrgAffHasParentCompany").val()) == "1") {
                    $("#divOrgAffParentCompanyDetails").show();
                }
                else {
                    $("#divOrgAffParentCompanyDetails").hide();
                }
            }

            function ShowOrgAffiliationInfo() {

                ClearOrgAffiliationFields();

                AjaxSend({
                    url: "Organization/getOrgHoldingCompanyInfo",
                    success: function (vData) {

                        if (vData.OrgAffiliationData.Rows.length > 0) {

                            $("#selOrgAffHasParentCompany").val(vData.OrgAffiliationData.Rows[0].IsHasHoldingCompany || '0');
                            $("#selOrgAffHasParentCompany");//.slider("refresh");

                            $("#txtOrgAffHoldingCompanyID").val(vData.OrgAffiliationData.Rows[0].HoldingCompanyID || '');
                            $("#btnOrgAffHoldingCompany").text(vData.OrgAffiliationData.Rows[0].HoldingCompanyName || 'Please Select');//.button('refresh');

                        }

                        getOrgAffiliatesList();

                        ShowHideOrgAffParentCompanyDetails();
                    }
                });

            };

            function getOrgAffiliatesList() {
                AjaxSend({
                    url: "Organization/getOrgAffliatesList",
                    success: function (vData) {

                        LoadOrgAffiliatesList(vData.OrgAffiliationData.Rows);

                    }
                });
            }

            function LoadOrgAffiliatesList(vRows) {

                var pLV = $('#lstOrgAffiliates');
                var pItem;

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li class="list-group-item">')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();
                                //$.mobile.changePage('#pageOrgAffiliatesInfo');
                                $('#pageOrgAffiliationInfo').hide();
                                $('#pageOrgAffiliatesInfo').show();
                                ShowOrgAffiliateInfo($(this).attr('ID'));
                            })
                            .append($('<div>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].AffiliateCompanyName)
                            )
                            .append($('<div>')
                                .attr("style", "white-space:normal")
                                .text("Remarks : " + vRows[p].Remarks)
                            )
                        );
                    pLV.append(pItem);
                };

                //pLV.listview('refresh');

            };

            function ShowOrgAffiliateInfo(strID) {
                var pData = {};
                pData.ID = strID;

                ClearOrgAffiliateFields();

                AjaxSend({
                    url: "Organization/getOrgAffliatesInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.OrgAffiliationData.Rows.length > 0) {

                            $("#txtOrgAffiliateID").val(vData.OrgAffiliationData.Rows[0].ID || '');

                            $("#txtOrgAffiliateCompanyID").val(vData.OrgAffiliationData.Rows[0].OrgAffiliatesID || '');
                            $("#btnOrgAffiliateCompany").text(vData.OrgAffiliationData.Rows[0].AffiliateCompanyName || 'Please Select');//.button('refresh');

                            $("#txtOrgAffiliateRemarks").val(vData.OrgAffiliationData.Rows[0].Remarks || '');

                            $("#btnOrgAffiliateRemove").show();
                        }
                    }
                });

            };

            function ClearOrgAffiliationFields() {

                $("#selOrgAffHasParentCompany").val('0');//.slider("refresh");

                $("#txtOrgAffHoldingCompanyID").val('');
                $("#btnOrgAffHoldingCompany").text('Please Select');//.button('refresh');
                $("#txtOrgAffiliateRemarks").val('');

                $('#lstOrgAffiliates').empty().append('<li class="list-group-item">No result(s) found</li>');

                ShowHideOrgAffParentCompanyDetails();

            }

            function ClearOrgAffiliateFields() {

                $("#txtOrgAffiliateID").val('');

                $("#txtOrgAffiliateCompanyID").val('');
                $("#btnOrgAffiliateCompany").text('Please Select');//.button('refresh');
                $("#txtOrgAffiliateRemarks").val('');

                $("#btnOrgAffiliateRemove").hide();                

            }

            function SaveOrgAffiliationInfo() {
                var pData = {};

                pData.IsHasHoldingCompany = $("#selOrgAffHasParentCompany").val();
                pData.OrgHoldingCompanyID = $.trim($("#txtOrgAffHoldingCompanyID").val());

                AjaxSend({
                    url: "Organization/SaveOrgHoldingCompany",
                    data: pData,
                    success: function (vSaveData) {

                        if (vSaveData.OrgAffiliationData == "InvalidHoldingCompany") {

                            NewMessageBox($.trim($("#btnOrgAffiliateCompany").text()) + " is already a corporate name of this organization.",
                        "Parent or Holding Company",
                        [
                            { Text: 'Ok' }
                        ]
                        );
                        }
                        else {
//                            if (window.pageSourcePage != undefined) {
//                                $.mobile.changePage("#" + $(window.pageSourcePage).attr('id'));
//                            }
//                            else {
//                                // ChangePage('/Organization/OrganizationSummary');
//                                window.location = MergeUrlParameters({
//                                    OrgID: sessionStorage.OrgID,
//                                    IsMobile: CBool(IsMobile) ? 1 : 0
//                                }, '/Organization/OrganizationSummary');
                            //                            }
                            $('body').scrollTop(0);
                            $('#UniversalDiv').html('');
                            LibraryTable.OrganizationPage();
                        }

                    }
                });

            };

            function SaveOrgAffiliates() {

                var pData = {};

                pData.OrgAffiliationID = $.trim($("#txtOrgAffiliateID").val());
                pData.OrgAffiliatesID = $.trim($("#txtOrgAffiliateCompanyID").val());
                pData.Remarks = $.trim($("#txtOrgAffiliateRemarks").val());

                AjaxSend({
                    url: "Organization/SaveOrgAffiliates",
                    data: pData,
                    success: function (vSaveData) {

                        if (vSaveData.OrgAffiliationData == "InvalidAffiliateCompany") {

                            NewMessageBox($.trim($("#btnOrgAffiliateCompany").text()) + " is already a corporate name of this organization.",
                        "Affiliate Company",
                        [
                            { Text: 'Ok' }
                        ]
                        );
                        }
                        else {

                            //ChangePage('/Organization/OrganizationAffiliation');

                            $('#pageOrgAffiliationInfo').show();
                            $('#pageOrgAffiliatesInfo').hide();
                            getOrgAffiliatesList();

                        }

                    }
                });

            };

            function RemoveOrgAffiliate() {

                var pData = {};
                pData.OrgAffiliationID = $("#txtOrgAffiliateID").val();

                AjaxSend({
                    url: "Organization/RemoveOrgAffiliates",
                    data: pData,
                    success: function (vData) {

                        // ChangePage('/Organization/OrganizationAffiliation');

                        $('#pageOrgAffiliationInfo').show();
                        $('#pageOrgAffiliatesInfo').hide();
                        getOrgAffiliatesList();

                    }
                });
            };


        });
    });
