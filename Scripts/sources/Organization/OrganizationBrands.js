require([
    'common'
    ], function () {

        $(function () {

            var mLoaded = false;
            var isIE = false || !!document.documentMode;

            $.mobile.activePage.bind('pageshow', PageShow);
            function PageShow() {
                mLoaded = true;

                if (!window.IsPageLoaded) {

                    getOrgBrandList();

                    window.IsPageLoaded = true;
                }

            };
            if (!mLoaded) PageShow();

            window.OrgBrand = {
                GetOrgBrand: function () {
                    getOrgBrandList();
                }
            }

            $('body').on('click', '#btnOrgBrandAdd', function (e) {
                e.preventDefault();

                //$.mobile.changePage('#pageOrgBrandInfo');
                $('#pageOrgBrand').hide();
                $('#pageOrgBrandInfo').show();
                ClearOrgBrandFields();
            })
            .on('click', '#btnOrgBrandInfoSave', function (e) {
                e.preventDefault();

                if ($.trim($("#txtOrgBrandName").val()) == "") {
                    NewMessageBox('Please enter brand name.');
                    return;
                }

                SaveOrganizationBrand();
            })
            .on('click', '#btnOrgBrandInfoCancel', function (e) {
                e.preventDefault();
                //$.mobile.changePage('#pageOrgBrand');
                $('#pageOrgBrand').show();
                $('#pageOrgBrandInfo').hide();
            })
            .on('click', '#btnOrgBrandDone', function (e) {
                e.preventDefault();
                $('body').scrollTop(0);
                $('#UniversalDiv').html('');
                LibraryTable.OrganizationPage();
//                if (window.pageSourcePage != undefined) {
//                    $.mobile.changePage("#" + $(window.pageSourcePage).attr('id'));
//                }
//                else {
//                    //ChangePage('/Organization/OrganizationSummary');

//                    window.location = MergeUrlParameters({
//                        OrgID: sessionStorage.OrgID,
//                        IsMobile: CBool(IsMobile) ? 1 : 0
//                    }, '/Organization/OrganizationSummary');
//                }
            })

            function getOrgBrandList() {
                AjaxSend({
                    url: 'Organization/getOrganizationBrandList',
                    success: function (vData) {
                        LoadOrgBrandList(vData.OrgBrandData.Rows);
                    }
                });
            }

            function LoadOrgBrandList(vRows) {
                var pLV = $('#lstOrgBrand');
                var pItem;

                $("#divOrgBrandRecordCount").text(" Number of record(s) found: " + vRows.length);

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li class="list-group-item">')
                    .append($('<a>')
                        .attr('ID', vRows[p].ID)
                        .click(function (e) {
                            e.preventDefault();
                            //$.mobile.changePage('#pageOrgBrandInfo');
                            $('#pageOrgBrand').hide();
                            $('#pageOrgBrandInfo').show();
                            ShowOrgBrandInfo($(this).attr('ID'));
                        })
                        .append($('<h3 class="m-0">')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].BrandName)
                        )
                    );

                    pLV.append(pItem);
                };

                //pLV.listview('refresh');
            };

            function ShowOrgBrandInfo(strID) {
                var pData = {};
                pData.ID = strID;

                ClearOrgBrandFields();

                AjaxSend({
                    url: "Organization/getOrganizationBrandInfo",
                    data: pData,
                    success: function (vData) {
                        if (vData.OrgBrandData.Rows.length > 0) {
                            $("#txtOrgBrandRecID").val(vData.OrgBrandData.Rows[0].ID || '');
                            $("#txtOrgBrandName").val(vData.OrgBrandData.Rows[0].BrandName || '');

                        }
                    }
                });

            };

            function SaveOrganizationBrand() {
                var pData = [];

                pData = {
                    OrgBrandRecID: $.trim($("#txtOrgBrandRecID").val()),
                    BrandName: $.trim($("#txtOrgBrandName").val())
                };

                AjaxSend({
                    url: "Organization/CheckOrganizationBrand",
                    data: pData,
                    success: function (vData) {

                        if (vData.OrgBrandData.Rows.length > 0) {
                            NewMessageBox('Brand being added already exist.');
                        }
                        else {
                            AjaxSend({
                                url: "Organization/SaveOrganizationBrand",
                                data: pData,
                                success: function (vSaveData) {

                                    //$.mobile.changePage('#pageOrgBrand');
                                    $('#pageOrgBrand').show();
                                    $('#pageOrgBrandInfo').hide();
                                    getOrgBrandList();

                                }
                            });

                        }

                    }
                });

            };

            function ClearOrgBrandFields() {
                $("#txtOrgBrandRecID").val('');
                $("#txtOrgBrandName").val('');
            };

        });
    });