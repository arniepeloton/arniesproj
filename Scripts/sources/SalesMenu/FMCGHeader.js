require([
    'common'    
], function () {
    $(document).ready(function () {
        window.FmcgHeader = {
            Home: function () {
                window.location = '/People/PeopleMenu';
            },
            Fowst: function () {
                BrowseFowst();
            },
            NewCart: function () {
                if (FmcgBasicCart.CartTotalQty == 0) {
                    sessionStorage.IsNewCart = true;
                    sessionStorage.removeItem('CustomerOrganizationID');
                    Popup.amounttendered = 0;
                    NewCart();
                }
                else {
                    NewMessageBox('You have ' + FmcgBasicCart.CartTotalQty + ' items on your current cart. Are you sure you want to create a New Cart? If yes, the contents of your current cart will be placed “On Hold”. We recommend that you complete your current cart first before creating a new cart.', 'New Cart',
                        [
                            {
                                Text: 'Continue with Current Cart',
                                Click: function () {
                                    // do nothing
                                }
                            },
                            {
                                Text: 'Create new Cart',
                                Click: function () {
                                    sessionStorage.IsNewCart = true;
                                    sessionStorage.removeItem('CustomerOrganizationID');
                                    Popup.amounttendered = 0;
                                    NewCart();
                                }
                            }
                        ]
                    )
                }
            },
            Mode: function () {
                NewMessageBox('', 'POS Mode', [
                        {
                            Text: '<b>RETAIL MODE</b> <br>' +
                                '<span style="font-size:14px;">Makes it easy for retailers to search for SKUs using keyboard entry or bar code / QR code scanning (best for retailers with hundreds or thousands of SKUs).</span>',
                            Size: 4,
                            Click: function () {
                                var pParams = GetUrlParameters();

                                location = MergeUrlParameters({
                                    mode: pParams.mode,
                                    OrgID: sessionStorage.OrgID,
                                    rsd: pParams.rsd,
                                    IsMobile: CBool(IsMobile) ? 1 : 0
                                }, '/SalesMenu');
                            }
                        },
                        {
                            Text: '<b>HOT BUTTON MODE</b> <br>' +
                                '<span style="font-size:14px;">Shows predefined buttons sorted by categories (best for fast food outlets and retailers whose items do not have UPC or bar code labels). Administrators can customize these buttons online.</span>',
                            Size: 4,
                            Click: function () {
                                sessionStorage.PosMode = 'Hot Button Mode';

                                if ($(window).width() > proj.WideWidth) {
                                    location = MergeUrlParameters({
                                        OrgID: sessionStorage["OrgID"],
                                        mode: 'pos',
                                        IsMobile: IsMobile ? 1 : 0
                                    }, '/SalesMenu/POSViewAll');
                                } else {
                                    location = MergeUrlParameters({
                                        HotButtons: 1,
                                        OrgID: sessionStorage["OrgID"],
                                        mode: 'pos',
                                        IsMobile: IsMobile ? 1 : 0
                                    }, '/SalesMenu');
                                }
                            }
                        },
                        {
                            Text: '<b>TABLE VIEW MODE</b> <br>' +
                                '<span style="font-size:14px;">Lets you view dining patrons as they are seated in tables (best for fine dining restaurants that require a high-touch service).</span>',
                            Size: 4,
                            Click: function () {
                                if (OfflineMode) {
                                    if (window.location.pathname.toLowerCase() == '/tableview/NewTable') {
                                        Android.Toast('You are already in Table View Mode');
                                    } else {
                                        sessionStorage.PosMode = 'Table View Mode';
                                        location = MergeUrlParameters({
                                            OrgID: sessionStorage["OrgID"],
                                            mode: 'pos',
                                            IsMobile: IsMobile ? 1 : 0
                                        }, '/TableView/NewTable');
                                    }
                                } else {
                                    MessageBox("Table View Mode is only available on the mobile app",
                                                "Table View Mode",
                                                [
                                                    { Text: 'Ok' }
                                                ]
                                        );
                                }
                            }
                        },
                        {
                            Text: '<b>FMCG MODE (Modern)</b> <br>' +
                                    '<span style="font-size:14px;">Fast Moving Consumer Goods</span>',
                            Size: 4,
                            Click: function () {
                                if(window.location.pathname == '/SalesMenu/FMCG'){
                                    if (OfflineMode) {
                                        Android.Toast('You are already in FMCG (Modern) Mode');
                                    }
                                    else {
                                        WebToast('You are already in FMCG (Modern) Mode');
                                    }
                                } else {
                                    location = MergeUrlParameters({
                                        OrgID: sessionStorage["OrgID"],
                                        mode: 'pos',
                                        IsMobile: IsMobile ? 1 : 0
                                    }, '/SalesMenu/FMCG');
                                }
                            }
                        },
                        {
                            Text: '<b>FMCG MODE (DOS)</b> <br>' +
                                    '<span style="font-size:14px;">Fast Moving Consumer Goods</span>',
                            Size: 4,
                            Click: function () {
                                if (window.location.pathname == '/SalesMenu/FMCGBasic') {
                                    if (OfflineMode) {
                                        Android.Toast('You are already in FMCG (DOS) Mode');
                                    }
                                    else {
                                        WebToast('You are already in FMCG (DOS) Mode');
                                    }
                                } else {
                                    location = MergeUrlParameters({
                                        OrgID: sessionStorage["OrgID"],
                                        mode: 'pos',
                                        IsMobile: IsMobile ? 1 : 0
                                    }, '/SalesMenu/FMCGBasic');
                                }
                            }
                        }
                ], {AutoDismiss : true})
            }
        }

        $('#btnFMCGHome').on('click', function () {
            FmcgHeader.Home();
        })

        $('#btnFMCGFowst').on('click', function () {
            FmcgHeader.Fowst();
        })

        $('#btnFMCGNewCart').on('click', function () {
            FmcgHeader.NewCart();
        })

        $('#btnFMCGMode').on('click', function () {
            FmcgHeader.Mode();
        })
    })
    .on('keydown', function (e) {
        if (e.which == '72' && ( e.metaKey || e.ctrlKey)) {
            e.preventDefault();
            // CTRL + H for Home
            FmcgHeader.Home();
        }
        else if (e.which == '78' && (e.shiftKey || e.metaKey)) {
            // Shift + N for New Cart
            e.preventDefault();
            FmcgHeader.NewCart();
        }
        else if (e.which == '70' && (e.metaKey || e.ctrlKey)) {
            e.preventDefault();
            // CTRL + F for Fowst
            BrowseFowst(); // FMCG.js
        }
        else if (e.which == '77' && (e.metaKey || e.ctrlKey)) {
            e.preventDefault();
            // CTRL + M for POS MODE
            FmcgHeader.Mode();
        }
    })
})