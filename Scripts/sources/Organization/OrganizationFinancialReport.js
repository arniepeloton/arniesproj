require([
    'common'
    ], function () {

        $(function () {

            var mLoaded = false;
            var isIE = false || !!document.documentMode;

            setMonthCollection("selOrgFiscalMonthEnd");

            $.mobile.activePage.bind('pageshow', PageShow);
            function PageShow() {
                mLoaded = true;

                if (!window.IsPageLoaded) {
                    $('body').scrollTop(0);
                    ShowOrgFinancialReportDetails();

                    window.IsPageLoaded = true;
                }
            };
            if (!mLoaded) PageShow();

            window.OrgFinRep = {
                ShowOrgFinRep: function () {
                    $('body').scrollTop(0);
                    setMonthCollection("selOrgFiscalMonthEnd");
                    ShowOrgFinancialReportDetails();
                }
            }

            $('body').on('change', '#selOrgFiscalMonthEnd', function (e) {
                e.preventDefault();
                setDayCollection('selOrgFiscalMonthEnd', 'selOrgFiscalDayEnd', '', false);
            })
            .on('click', '#btnOrgFinancialReportDone', function (e) {
                e.preventDefault();

                if ($.trim($("#selOrgFiscalMonthEnd").val()) == "" || $.trim($("#selOrgFiscalDayEnd").val()) == "") {
                    NewMessageBox('Please select valid fiscal year end.');
                    return;
                }

                SaveOrgFinancialReportDetails();
            })
            .on('click', '#btnOrgFinancialReportCancel', function (e) {
                $('body').scrollTop(0);
                $('#UniversalDiv').html('');
                LibraryTable.OrganizationPage();
            })

            function ShowOrgFinancialReportDetails() {

                ClearOrgFinancialReportFields();

                AjaxSend({
                    url: "Organization/getOrgFinancialReportInfo",
                    success: function (vData) {

                        if (vData.OrganizationData.Rows.length > 0) {

                            $("#txtOrgFinancialReportID").val(vData.OrganizationData.Rows[0].ID || '');

                            $("#selOrgFiscalMonthEnd").val(vData.OrganizationData.Rows[0].FiscalMonthEnd || '');//.selectmenu('refresh');
                            setDayCollection('selOrgFiscalMonthEnd', 'selOrgFiscalDayEnd', '', false);
                            $("#selOrgFiscalDayEnd").val(vData.OrganizationData.Rows[0].FiscalDayEnd || '');//.selectmenu('refresh');

                            $("#txtOrgExternalAuditor").val(vData.OrganizationData.Rows[0].NameOfExternalAuditor || '');
                            $("#txtOrgSingningPartnerExternalAuditor").val(vData.OrganizationData.Rows[0].SingningPartnerOfExternalAuditor || '');
                            $("#txtOrgSECAccreditationNoOfAuditor").val(vData.OrganizationData.Rows[0].SECAccreditationNoOfExternalAuditor || '');
                            $("#selOrgVATType").val(vData.OrganizationData.Rows[0].VATType || '');//.selectmenu('refresh');
                            $("#selOrgBIRTaxGroup").val(vData.OrganizationData.Rows[0].BIRTaxGroup || '');//.selectmenu('refresh');

                        }
                    }
                });

            };

            function ClearOrgFinancialReportFields() {
                $("#txtOrgFinancialReportID").val('');

                $("#selOrgFiscalMonthEnd").val('');//.selectmenu('refresh');
                $("#selOrgFiscalDayEnd").val('');//.selectmenu('refresh');
                $("#txtOrgExternalAuditor").val('');
                $("#txtOrgSingningPartnerExternalAuditor").val('');
                $("#txtOrgSECAccreditationNoOfAuditor").val('');
                $("#selOrgVATType").val('');//.selectmenu('refresh');
                $("#selOrgBIRTaxGroup").val('');//.selectmenu('refresh');
            }

            function SaveOrgFinancialReportDetails() {
                var pData = {};
                pData.FinancialReportID = $.trim($("#txtOrgFinancialReportID").val());
                pData.FiscalMonthEnd = $.trim($("#selOrgFiscalMonthEnd").val());
                pData.FiscalDayEnd = $.trim($("#selOrgFiscalDayEnd").val());
                pData.NameOfExternalAuditor = $.trim($("#txtOrgExternalAuditor").val());
                pData.SingningPartnerOfExternalAuditor = $.trim($("#txtOrgSingningPartnerExternalAuditor").val());
                pData.SECAccreditationNoOfExternalAuditor = $.trim($("#txtOrgSECAccreditationNoOfAuditor").val());
                pData.VATType = $.trim($("#selOrgVATType").val());
                pData.BIRTaxGroup = $.trim($("#selOrgBIRTaxGroup").val());

                AjaxSend({
                    url: "Organization/SaveOrgFinancialReport",
                    data: pData,
                    success: function (vData) {

                        //                        if (window.pageSourcePage != undefined) {
                        //                            $.mobile.changePage("#" + $(window.pageSourcePage).attr('id'));
                        //                        }
                        //                        else {
                        //                            //  ChangePage('/Organization/OrganizationSummary');

                        //                            window.location = MergeUrlParameters({
                        //                                OrgID: sessionStorage.OrgID,
                        //                                IsMobile: CBool(IsMobile) ? 1 : 0
                        //                            }, '/Organization/OrganizationSummary');

                        //                        }
                        $('body').scrollTop(0);
                        $('#UniversalDiv').html('');
                        LibraryTable.OrganizationPage();
                    }
                });

            };

        });
    });