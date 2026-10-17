require([
    'common',
    GetPathFormat('/Scripts3/Browser/LibraryPIMProduct.js?crc=')
    ], function () {

        $(function () {

            //ADDITIONAL BY JANINE Sept. 9, 2013 1pm
            CheckIfBackIsClicked();

            $('#btnOnlineStorePopUpMenu').click(function (e) {
                e.preventDefault();

                $("#divOnlineStoreMenuPopUp").popup("open")

            });

            $('#btnMyCompanyList').click(function () {

                window.location = '/OnlineStoreMyCompanyList';

            });

            $('#btnMyList').click(function () {

                window.location = '/OnlineStoreMyList';

            });

            $('#btnOrders').click(function () {

                window.location = '/OnlineStoreOrders';

            });

            $('#popOnlineStoreProfile').click(function () {

                window.location = '/People/PeopleInfoSummary';

            });

            $('#btnOrganizationFOWST').click(function () {

                var pData = {};
                pData.OrgID = sessionStorage["OrgID"];

                window.pageSourcePage = $.mobile.activePage;

                AjaxSend({
                    url: 'Organization/getSubscriberOrganization',
                    data: pData,
                    success: function (vDataOrg) {

                        if (vDataOrg.OrganizationID != "") {
                            pData.IsNew = 0;
                            pData.ID = vDataOrg.OrganizationID;

                            AjaxSend({
                                url: 'Organization/setCreateNew',
                                data: pData,
                                success: function (vData) {

                                    window.location = MergeUrlParameters({
                                        OrgID: sessionStorage.OrgID,
                                        IsMobile: CBool(IsMobile) ? 1 : 0
                                    }, '/Organization/OrganizationAddress');
                                }
                            });
                        }
                    },
                    error: function (vError) {
                        MessageBox("This feature requires internet connection.",
                                   "Requires Internet Connection",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                            );
                        return;
                    }

                });

            });

            $('#btnOnlineStoreSignOut').click(function () {

                AjaxSend({
                    url: "People/setSignOut",
                    success: function (vData) {

                        window.location = MergeUrlParameters({
                            IsMobile: 0,
                            moduletype: 'onlinestore'
                        }, '/Signin');

                    }
                });

            });

            $('#formSearchProduct').submit(function (e) {
                e.preventDefault();
                SearchProduct();
                return false;
            });

            $('#butProductBarCode').click(function () {
                if ((CBool(window.IsMobile)) && (!CBool(window.InWindows))) {
                    mobile.Scan({
                        Success: function (vData) {
                            $('#txtSearchProduct').val(vData.Value);
                            SearchProduct();
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

            function CheckIfBackIsClicked() {
                if (location.hash.length < 1) {

                    ShowPeopleMenu(); //if true, it means it's safe to reload people menu, if false, dont reload so the orgID will not be removed!

                }
            }

            function ShowPeopleMenu() {

                AjaxSend({
                    url: 'People/IsNewEntry',
                    success: function (vData) {
                        if (vData.PeopleID == '') {
                            $('#btnOnlineStoreSignOut').click();
                        } else {

                            sessionStorage.removeItem("OrgID");
                            sessionStorage.removeItem("FowstID");
                            sessionStorage["OrgID"] = '9DA344F3-DADB-4675-B723-15F0C591DA3C'; // Assigned Temporary

                            ShowPersonNameInfo(vData.PeopleID);

                        }

                    }
                });

            }

            function ShowPersonNameInfo(strID) {
                var pData = {};
                pData.PeopleID = strID;
                pData.OrgID = sessionStorage["OrgID"];

                ClearPersonFields();

                AjaxSend({
                    url: "People/getPeopleInfoName",
                    data: pData,
                    success: function (vData) {

                        if (vData.PeopleNameData.Rows.length > 0) {

                            //Header Info
                            $("#lblPersonName").text(vData.PeopleNameData.Rows[0].PeopleName || '');

                        }
                    }
                });
            };

            function ClearPersonFields() {

                //Header Info
                $("#lblName").text('');
                $("#lblEmployer").text('');

            }

            function SearchProduct() {

                window.IsBrowserloaded = false;
                Browser.LibraryPIMProduct.InputData.IsSearchMore = 1;
                Browser.LibraryPIMProduct.Click = function (e, vOpt) {
                    proj.ProductID = e.ID;
                    vOpt.DontClose = true;

                    window.NewStockBrowser = true;
                    ChangePage('StockInfo');

                };
                Browser.LibraryPIMProduct.Show($('#txtSearchProduct').val());

                $('#txtSearchProduct').val('');

            };





        });

    });