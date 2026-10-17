require([
    'common'
    ], function () {

        $(function () {

            var mLoaded = false;
            var IsCancelled = false;
            var isIE = false || !!document.documentMode;

            setMonthCollection("selOrgCustProductActivationDateMonth");
            setYearCollection("selOrgCustProductActivationDateYear");

            setMonthCollection("selOrgCustProductDateInActiveMonth");
            setYearCollection("selOrgCustProductDateInActiveYear");

            $.mobile.activePage.bind('pageshow', PageShow);
            function PageShow() {
                mLoaded = true;
                getOrgCustProductList();
            };
            if (!mLoaded) PageShow();

            window.OrgShortCusProd = {
                ShowOrgCustProd: function () {
                    setMonthCollection("selOrgCustProductActivationDateMonth");
                    setYearCollection("selOrgCustProductActivationDateYear");

                    setMonthCollection("selOrgCustProductDateInActiveMonth");
                    setYearCollection("selOrgCustProductDateInActiveYear");
                    getOrgCustProductList();
                }
            }

            $('body').on('submit', '#frmOrgCustProductSearch', function (e) {
                e.preventDefault();

                getOrgCustProductList(); ;

                return false;
            })
            .on('click', '#lnkCustProductList', function (e) {
                e.preventDefault();
                //////                window.location = GetPathFormat("/Organization/DownloadCompanyWideProductList");
            })
            .on('click', '#btnOrgCustProductAdd', function (e) {
                e.preventDefault();

                //$.mobile.changePage('#pageOrgCustProductInfo');
                $('#pageOrgCustProduct').hide();
                $('#pageOrgCustProductInfo').show();

                ClearOrgCustProductFields();
                ClearOrgCustProductInActiveFields();

                ShowHideOrgCustProductInActiveDetails();
            })
            .on('click', '#btnOrgCustProduct', function (e) {
                $('#TitleLibrary').text('PRODUCT');
                $('#txtSearchKeyword').val('').attr('placeholder', 'Type name of Product');
                $('#panSearchLibrary').popup('open');
                $('#lstOfLibrary').empty();
                //                Browser.LibraryPIMProduct.InternalInputData.IsSearchMore = 1;
                //                Browser.LibraryPIMProduct.Click = function (e) {

                //                    $('#txtOrgCustProductID').val(e.ID)
                //                    $('#btnOrgCustProduct').text(e.ProductName)
                //                .button('refresh');

                //                };

                //                Browser.LibraryPIMProduct.Show();
            })
            .on('click', '#btnOrgCustProductInfoSave', function (e) {
                if ($.trim($("#txtOrgCustProductID").val()) == "") {
                    $('#TitleOrgEmp').text('Product');
                    $('#MsgOrgEmp').text('Please select product.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                if ($.trim($("#selOrgCustProductActivationDateMonth").val()) == "" || $.trim($("#selOrgCustProductActivationDateDay").val()) == "" || $.trim($("#selOrgCustProductActivationDateYear").val()) == "") {
                    $('#TitleOrgEmp').text('Activation Date');
                    $('#MsgOrgEmp').text('Please enter valid activation date.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                if ($.trim($("#selOrgCustProductDeactivate").val()) == "0") {

                    ClearOrgCustProductInActiveFields();

                }
                else {

                    if ($.trim($("#selOrgCustProductDateInActiveMonth").val()) == "" || $.trim($("#selOrgCustProductDateInActiveDay").val()) == "" || $.trim($("#selOrgCustProductDateInActiveYear").val()) == "") {
                        $('#TitleOrgEmp').text('Deactivation Date');
                        $('#MsgOrgEmp').text('Please enter valid deactivation date.');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').popup('close');
                        })
                        $('#divBtnCancel').hide();
                        return;
                    }

                }

                SaveCustOrganizationProduct();
            })
            .on('click', '#btnOrgCustProductInfoCancel', function (e) {
                e.preventDefault();

                IsCancelled = true;
                $('#pageOrgCustProduct').show();
                $('#pageOrgCustProductInfo').hide();
            })
            .on('change', '#selOrgCustProductActivationDateMonth', function (e) {
                e.preventDefault();

                setDayCollection('selOrgCustProductActivationDateMonth', 'selOrgCustProductActivationDateDay', 'selOrgCustProductActivationDateYear', true);
            })
            .on('change', '#selOrgCustProductActivationDateYear', function (e) {
                e.preventDefault();

                setDayCollection('selOrgCustProductActivationDateMonth', 'selOrgCustProductActivationDateDay', 'selOrgCustProductActivationDateYear', true);
            })
            .on('change', '#selOrgCustProductDateInActiveMonth', function (e) {
                e.preventDefault();

                setDayCollection('selOrgCustProductDateInActiveMonth', 'selOrgCustProductDateInActiveDay', 'selOrgCustProductDateInActiveYear', true);
            })
            .on('change', '#selOrgCustProductDateInActiveYear', function (e) {
                e.preventDefault();

                setDayCollection('selOrgCustProductDateInActiveMonth', 'selOrgCustProductDateInActiveDay', 'selOrgCustProductDateInActiveYear', true);
            })
            .on('click', '#btnOrgCustProductDone', function (e) {
                e.preventDefault();

                //////                if (window.pageSourcePage != undefined) {
                //////                    $.mobile.changePage("#" + $(window.pageSourcePage).attr('id'));
                //////                }
                //////                else {
                //////                    window.open(GetPathFormat("/Organization/OrganizationCustomer") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                //////                }
                window.location.hash = '';
                window.location.hash = 'UniversalPage';
                LibraryTable.ViewCustomerPage();
            })
            .on('click', '#selOrgCustProductDeactivate', function (e) {
                e.preventDefault();
                ShowHideOrgCustProductInActiveDetails();
            })
            .on('keypress', '#txtSearchKeyword', function (e) {
                if (e.keyCode == 13) {
                    SearchProductPopUp($(this).val());
                }
            })

            function SearchProductPopUp(s) {
                var pData = {};

                pData.keywords = s;
                pData.IsSearchMore = 0;

                AjaxSend({
                    url: 'Browser/LibraryPIMProduct',
                    data: pData,
                    success: function (vData) {
                        $('#lstOfLibrary').empty();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstOfLibrary').append($('<li>')
                                    .append($('<a>')
                                        .attr('ID', vData.Results.Rows[p].ID)
                                        .append($('<br><p style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].ProductName)
                                        )
                                        .append($('<p>')
                                            .text('Published by: ' + vData.Results.Rows[p].PublishedBy)
                                        )
                                        .append($('<p>')
                                            .text('Uploaded by: ' + vData.Results.Rows[p].UploadedBy)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function (e) {
                                            $('#txtOrgCustProductID').val($(this).attr('ID'));
                                            $('#btnOrgCustProduct').text($(this).data('data').ProductName).button('refresh');

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

            function ShowHideOrgCustProductInActiveDetails() {

                if ($.trim($("#selOrgCustProductDeactivate").val()) == "1") {
                    $("#divOrgCustProductDeactivateDetails").show();
                }
                else {
                    $("#divOrgCustProductDeactivateDetails").hide();
                }
            }

            function getOrgCustProductList() {
                var pData = {};

                if (!IsCancelled) {
                    pData.SearchKeyword = $("#txtOrgCustProductSearch").val();

                    AjaxSend({
                        url: 'Organization/getOrganizationCustomerProductList',
                        data: pData,
                        success: function (vData) {
                            LoadOrgCustProductList(vData.OrgCustProductData.Rows);
                        }
                    });
                }

                IsCancelled = false;
            }

            function LoadOrgCustProductList(vRows) {
                var pLV = $('#lstOrgCustProduct');
                var pItem;

                AjaxSend({
                    url: 'Organization/getOrgCustomerProductCount',
                    success: function (vData) {

                        $("#lblCustProductRecordcount").text(" There are " + vRows.length + " results that match your keyword search. " + vData.OrgCustProductData + " products are listed for your company.");

                        pLV.html('');
                        for (var p = 0; p <= vRows.length - 1; p++) {
                            pItem = $('<li>')
                                .append($('<a>')
                                    .attr('ID', vRows[p].ID)
                                    .click(function (e) {
                                        e.preventDefault();
                                        //$.mobile.changePage('#pageOrgCustProductInfo');
                                        $('#pageOrgCustProduct').hide();
                                        $('#pageOrgCustProductInfo').show();
                                        ShowOrgCustProductInfo($(this).attr('ID'));
                                    })
                                    .append($('<h3>')
                                        .attr("style", "white-space:normal")
                                        .text(vRows[p].ProductName)
                                    )
                                    .append($('<p>')
                                        .attr("style", "white-space:normal")
                                        .text("Activation Date: " + $.format.date(new Date(vRows[p].ActivationDate), 'MMM dd, yyyy'))
                                    )
                                );

                            pLV.append(pItem);
                        };

                        pLV.listview('refresh');

                    }
                });

            };

            function ShowOrgCustProductInfo(strID) {
                var pData = {};
                pData.ID = strID;

                ClearOrgCustProductFields();
                ClearOrgCustProductInActiveFields();

                AjaxSend({
                    url: "Organization/getOrganizationCustomerProductInfo",
                    data: pData,
                    success: function (vData) {
                        if (vData.OrgCustProductData.Rows.length > 0) {
                            $("#txtOrgCustPIMProductID").val(vData.OrgCustProductData.Rows[0].ID || '');
                            $("#txtOrgCustomerID").val(vData.OrgCustProductData.Rows[0].CustomerID || '');

                            $('#btnOrgCustProduct').attr("disabled", true);
                            $("#txtOrgCustProductID").val(vData.OrgCustProductData.Rows[0].ProductID || '');
                            $("#btnOrgCustProduct").text(vData.OrgCustProductData.Rows[0].ProductName || 'Please Select').button('refresh');

                            $("#selOrgCustProductActivationDateMonth").val(vData.OrgCustProductData.Rows[0].ActivationDateMonth || '').selectmenu('refresh');
                            $("#selOrgCustProductActivationDateYear").val(vData.OrgCustProductData.Rows[0].ActivationDateYear || '').selectmenu('refresh');
                            setDayCollection('selOrgCustProductActivationDateMonth', 'selOrgCustProductActivationDateDay', 'selOrgCustProductActivationDateYear', true);
                            $("#selOrgCustProductActivationDateDay").val(vData.OrgCustProductData.Rows[0].ActivationDateDay || '').selectmenu('refresh');

                            $("#selOrgCustProductDeactivate").val(vData.OrgCustProductData.Rows[0].DeActivated || '0').slider("refresh");

                            if ($("#selOrgCustProductDeactivate").val() != "0") {
                                $("#selOrgCustProductDateInActiveMonth").val(vData.OrgCustProductData.Rows[0].DeactivationDateMonth || '').selectmenu('refresh');
                                $("#selOrgCustProductDateInActiveYear").val(vData.OrgCustProductData.Rows[0].DeactivationDateYear || '').selectmenu('refresh');
                                setDayCollection('selOrgCustProductDateInActiveMonth', 'selOrgCustProductDateInActiveDay', 'selOrgCustProductDateInActiveYear', true);
                                $("#selOrgCustProductDateInActiveDay").val(vData.OrgCustProductData.Rows[0].DeactivationDateDay || '').selectmenu('refresh');

                                $("#txtOrgCustProductInActiveReason").val(vData.OrgCustProductData.Rows[0].DeactivationReason || '').keyup();
                                $("#txtOrgCustProductComments").val(vData.OrgCustProductData.Rows[0].Comments || '').keyup();

                                $("#selOrgCustProductBan").val(vData.OrgCustProductData.Rows[0].IsBan || '0').slider("refresh");
                            }

                            ShowHideOrgCustProductInActiveDetails();

                        }
                    }
                });

            };

            function SaveCustOrganizationProduct() {
                var pData = [];

                pData = {
                    OrgProductID: $.trim($("#txtOrgCustPIMProductID").val()),
                    OrgCustomerID: $.trim($("#txtOrgCustomerID").val()),
                    ProductID: $.trim($("#txtOrgCustProductID").val()),
                    ActivationDate: $("#selOrgCustProductActivationDateMonth").val() + '/' + $("#selOrgCustProductActivationDateDay").val() + '/' + $("#selOrgCustProductActivationDateYear").val(),
                    IsDeactivated: $("#selOrgCustProductDeactivate").val(),
                    DeactivationDate: $("#selOrgCustProductDateInActiveMonth").val() + '/' + $("#selOrgCustProductDateInActiveDay").val() + '/' + $("#selOrgCustProductDateInActiveYear").val(),
                    DeactivationReason: $("#txtOrgCustProductInActiveReason").val(),
                    Comments: $("#txtOrgCustProductComments").val(),
                    IsBan: $("#selOrgCustProductBan").val()
                };

                AjaxSend({
                    url: "Organization/CheckOrganizationCustomerProduct",
                    data: pData,
                    success: function (vData) {

                        if (vData.OrgCustProductData.Rows.length > 0) {
                            $('#TitleOrgEmp').text('Product Exist');
                            $('#MsgOrgEmp').text('Product being added already exist.');
                            $('#panOrgEmpMessage').popup('open');
                            $('#btnOKOrgEmp').text('Ok').click(function (e) {
                                e.preventDefault();
                                $('#panOrgEmpMessage').popup('close');
                            })
                            $('#divBtnCancel').hide();
                        }
                        else {
                            AjaxSend({
                                url: "Organization/SaveOrganizationCustomerProduct",
                                data: pData,
                                success: function (vSaveData) {

                                    if (vSaveData.OrgCustProductData == "ActivationDateError") {
                                        $('#TitleOrgEmp').text('Activation Date');
                                        $('#MsgOrgEmp').text('Please enter valid activation date.');
                                        $('#panOrgEmpMessage').popup('open');
                                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                                            e.preventDefault();
                                            $('#panOrgEmpMessage').popup('close');
                                        })
                                        $('#divBtnCancel').hide();
                                    }
                                    else if (vSaveData.OrgCustProductData == "DeactivationDateError") {
                                        $('#TitleOrgEmp').text('Deactivation Date');
                                        $('#MsgOrgEmp').text('Please enter valid deactivation date.');
                                        $('#panOrgEmpMessage').popup('open');
                                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                                            e.preventDefault();
                                            $('#panOrgEmpMessage').popup('close');
                                        })
                                        $('#divBtnCancel').hide();
                                    }
                                    else {
                                        //                                    $.mobile.changePage('#pageOrgProduct');
                                        // ChangePage('/Organization/OrganizationCustomerProduct');

                                        //                                        window.location = MergeUrlParameters({
                                        //                                            OrgID: sessionStorage.OrgID,
                                        //                                            IsMobile: CBool(IsMobile) ? 1 : 0
                                        //                                        }, '/Organization/OrganizationCustomerProduct');

                                        $('#pageOrgCustProduct').show();
                                        $('#pageOrgCustProductInfo').hide();

                                        IsCancelled = false;
                                        getOrgCustProductList();
                                    }

                                }
                            });

                        }

                    }
                });

            };

            function ClearOrgCustProductFields() {

                $("#txtOrgCustPIMProductID").val('');
                $("#txtOrgCustomerID").val('');

                $('#btnOrgCustProduct').attr("disabled", false);
                $("#txtOrgCustProductID").val('');
                $("#btnOrgCustProduct").text('Please Select').button('refresh');

                $("#selOrgCustProductActivationDateMonth").val(parseInt(new Date().getMonth()) + 1).selectmenu('refresh');
                $("#selOrgCustProductActivationDateYear").val(new Date().getFullYear()).selectmenu('refresh');
                setDayCollection('selOrgCustProductActivationDateMonth', 'selOrgCustProductActivationDateDay', 'selOrgCustProductActivationDateYear', true);
                $("#selOrgCustProductActivationDateDay").val(new Date().getDate()).selectmenu('refresh');

            };

            function ClearOrgCustProductInActiveFields() {

                $("#selOrgCustProductDeactivate").val('0').slider("refresh");

                $("#selOrgCustProductDateInActiveMonth").val('').selectmenu('refresh');
                $("#selOrgCustProductDateInActiveDay").val('').selectmenu('refresh');
                $("#selOrgCustProductDateInActiveYear").val('').selectmenu('refresh');

                $("#txtOrgCustProductInActiveReason").val('');
                $("#txtOrgCustProductComments").val('');

                $("#selOrgCustProductBan").val('0').slider("refresh");
            };

        });
    });