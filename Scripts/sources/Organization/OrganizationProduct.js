require([
    'common',
    '/Content/Ecommerce/js/lightbox-2.11.1.js',
    'promise!/Scripts3/controller/Bom.js?crc=',
    '/Scripts3/Browser/LibraryPIMProductPublic.js?crc=',
     GetPathFormat('/Scripts3/Browser/LibraryOrganizationProductPublisher.js'),
     'bootstrap-tooltip'
    ], function () {

        $(function () {
            var arrKOGroupItems = [];

            var mLoaded = false;
            var IsCancelled = false;

            var IsNewOrgProductMeaBarcodeAdd = true;
            var arOrgProductMeaBarcode = new Array();
            var intOrgProductMeaBarcodeCntr = 0;
            var intOrgProductMeaBarcodeIndex = 0;

            var intOrgProductMeaBarcodeRecID = 0;
            var intOrgProductMeaBarcode = 1;
            var intOrgProductMeaState = 2;
            var selectedOrgPublisherID;
            var selectedOrgPublisherName;
            var mPriceBreakUnitID;
            var mProductName;
            var mPaymentTermID;

            var libname, lbllibname, addlibname, viewid, NameOfBtnForLibrary, productname, isAddRawMaterial = 0, unitmeasureid, unitmeasure, childProductBaseUnitMeasureID, qty, destinationMeasure, childProductID, isPriceEdit, jit;
            var isPageUnitOfMeasureLoaded = "", isPageProductPrices = "";
            var mPriceBreakHtml;
            var mBaseUnit;
            var mBaseUnitQty;
            var mUnitMeasures = [];

            setMonthCollection("selOrgProductActivationDateMonth");
            setYearCollection("selOrgProductActivationDateYear");

            setMonthCollection("selOrgProductDateInActiveMonth");
            setYearCollection("selOrgProductDateInActiveYear");

            var divProductBOM = $('#divProductBOM');
            var divProductsBOMSub = $('#divProductBOMSub');
            var isGrantAccessToModifyPIM = true;

            var isFinishedLoadIL = false;

            var maxreferenceskuid = '';
            var maxreferenceskuname = '';

            var mParentID;
            var mParentComponentName;
            var mBom = new App.controller.Bom();
            mBom.model.organizationID = sessionStorage.OrgID;

            $('#divProductBOMContainer')
                .html('')
                .append(mBom.element);
            //mBom.element;//.trigger('create'); // only needed in siesta
            $.mobile.activePage.bind('pageshow', PageShow);
            function PageShow() {
                mLoaded = true;
                lbllibname = sessionStorage.LabelLibraryName;
                libname = sessionStorage.LibraryName;
                addlibname = sessionStorage.AddViewLibBCName;
                viewid = sessionStorage.ViewID;
                //getOrgProductList();
                if (sessionStorage.ViewID != undefined) {
                    $("#divProductBOMContainer");//.trigger('create');
                    ShowOrgProductInfo(sessionStorage.ViewID);
                }

                lightbox.option({
                    'resizeDuration': 200,
                    'wrapAround': true,
                    'imageFadeDuration': 300,
                    'alwaysShowNavOnTouchDevices': true
                })
            };
            if (!mLoaded) PageShow();

            $('body').tooltip({ selector: '[data-toggle="tooltip"]' });

            window.OrgProd = {
                SetReqData: function () {
                    lbllibname = sessionStorage.LabelLibraryName;
                    libname = sessionStorage.LibraryName;
                    addlibname = sessionStorage.AddViewLibBCName;
                    setMonthCollection("selOrgProductActivationDateMonth");
                    setYearCollection("selOrgProductActivationDateYear");

                    setMonthCollection("selOrgProductDateInActiveMonth");
                    setYearCollection("selOrgProductDateInActiveYear");
                },
                GetAllOrgOwners: function () {
                    setMonthCollection("selOrgProductActivationDateMonth");
                    setYearCollection("selOrgProductActivationDateYear");

                    setMonthCollection("selOrgProductDateInActiveMonth");
                    setYearCollection("selOrgProductDateInActiveYear");
                    GetAllOrgOwners();
                },
                ShowProductInfo: function () {
                    mBom = new App.controller.Bom();
                    $('#divProductBOMContainer')
                    .html('')
                    .append(mBom.element)

                    $("input[type='radio']");//.checkboxradio();
                    $("input[type='radio']");//.checkboxradio("refresh");

                    setMonthCollection("selOrgProductActivationDateMonth");
                    setYearCollection("selOrgProductActivationDateYear");

                    setMonthCollection("selOrgProductDateInActiveMonth");
                    setYearCollection("selOrgProductDateInActiveYear");
                    ShowOrgProductInfo(sessionStorage.ViewID);
                },
                GetPIMPictures: GetPIMPictures
            }
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

            $('#btnPIM').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu#pagePopUpMenuProducts"), "_self");
            });

            //End Navigation Menu

            $('body')
                .on('click', '#btnPIMMenuPictureGallery, #btnCancelCrop', function () {
                    $('#divMainPicGallery').pageshow();
                })
            .on('click', '#listMenuPIM > li', function () {
                var listMenu = $(this);
                if (listMenu.find('a').hasClass('disabled')) {
                    return;
                }
                var CurrentPageShowed;
                var NameCurrentActiveMenu = $('#listMenuPIM').find('.active-menu').attr('id');
                var NameNewActiveMenu = listMenu.find('a').attr('id');
                var PageToShow = listMenu.find('a').attr('data-gotopage');
                $('#divContainerMenuPage > div').each(function () {
                    if ($(this).css('display') == 'block') {
                        CurrentPageShowed = $(this).attr('id');
                    }
                });

                $('#divContainerMenuPage').show();
                $('#listMenuPIM a').removeClass('active');
                $('#' + NameCurrentActiveMenu + '').removeClass('active-menu active');
                $('#' + NameNewActiveMenu + '').addClass('active-menu active');
                $('#' + CurrentPageShowed + '').hide();
                $('#' + PageToShow + '').show();//.trigger('create');

                $('#pageSKUMeasurement').hide();
                $('#pageSKUPrice').hide();
            })
            .on('click', '#btnProductInfo', function (e) {
                e.preventDefault();

                $("#divProductInfoContainer").show();
                $("#divProductMeasurementContainer").hide();
                $("#divProductPriceContainer").hide();
                $("#divProductBOMContainer").hide();
                $('#divProductInfoPopup').modal('hide');
            })
            .on('click', '#btnProductMeasurements', function (e) {
                e.preventDefault();

                $("#divProductInfoContainer").hide();
                $("#divProductMeasurementContainer").show();
                $("#divProductPriceContainer").hide();
                $("#divProductBOMContainer").hide();
                $('#divProductInfoPopup').modal('hide');
            })
            .on('click', '#btnProductPrices', function (e) {
                e.preventDefault();

                $("#divProductInfoContainer").hide();
                $("#divProductMeasurementContainer").hide();
                $("#divProductPriceContainer").show();
                $("#divProductBOMContainer").hide();
                $('#divProductInfoPopup').modal('hide');
            })
            .on('click', '#btnProductBOM', function (e) {
                e.preventDefault();

                $("#divProductInfoContainer").hide();
                $("#divProductMeasurementContainer").hide();
                $("#divProductPriceContainer").hide();
                $("#divProductBOMContainer");//.trigger('create').show();
                $('#divProductInfoPopup').modal('hide');
            })
            .on('submit', '#frmSearchPublicProduct', function (e) {
                e.preventDefault();
                //$('#TitleLibrary').text('PRODUCT');
                //$('#txtSearchKeyword').val('').val($('#txtPublicSearch').val()).attr('placeholder', 'Type name of product');
                //$('#panSearchLibrary').modal('show');
                //$('#lstOfLibrary').empty();
                SearchPublicProduct($('#txtSearchKeyword').val());

                return false;
            })
            .on('submit', '#frmOrgProductSearch', function (e) {
                e.preventDefault();

                getOrgProductList(); ;

                return false;
            })
            .on('click', '#butBarCode', function (e) {
                e.preventDefault();

                if ((CBool(window.IsMobile)) && (!CBool(window.InWindows))) {
                    mobile.Scan({
                        Success: function (vData) {
                            $('#txtPublicSearch').val(vData.Value);
                            SearchPublicProduct();
                        }
                    });
                } else {
                    NewMessageBox('Scan Barcode / QR Code feature is only available on the mobile app', 'Scan Barcode / QR Code');
                }
            })
            .on('click', '#lnkProductList', function (e) {
                e.preventDefault();

                window.location = GetPathFormat("/Organization/DownloadCompanyWideProductList?OrgID=" + sessionStorage["OrgID"]);
            })
            .on('click', '#btnOrgProductInfoSave', function (e) {
                e.preventDefault();

                if ($.trim($("#txtOrgProductID").val()) == "") {
                    NewMessageBox('Please select product.', 'Product');
                    return;
                }

                if ($.trim($("#selOrgProductActivationDateMonth").val()) == "" || $.trim($("#selOrgProductActivationDateDay").val()) == "" || $.trim($("#selOrgProductActivationDateYear").val()) == "") {
                    NewMessageBox('Please enter valid activation date.', 'Activation Date');
                    return;
                }

                if (!$("#selOrgProductDeactivate").is(':checked')) {
                    ClearOrgProductInActiveFields();
                }
                else {

                    if ($.trim($("#selOrgProductDateInActiveMonth").val()) == "" || $.trim($("#selOrgProductDateInActiveDay").val()) == "" || $.trim($("#selOrgProductDateInActiveYear").val()) == "") {
                        NewMessageBox('Please enter valid deactivation date.', 'Deactivation Date');
                        return;
                    }

                }

                SaveOrganizationProduct();
            })
            .on('click', '#btnOrgProductInfoSaveAs', function (e) {
                e.preventDefault();
                require(['promise!/Scripts/sources/PIM/pimSaveAs/controller.js?v=' + assetversion], function() {
                    new PIM.pimSaveAs.controller().popup({
                        ProductID: sessionStorage.ViewID,
                        ProductNum: $("#txtOrgProductNo").val(),
                        ProductName: $("#txtOrgProductName").val(),
                        ProductBrand: $('#txtOrgProductBrand').val()
                    });
                });
            })
            .on('click', '#btnPIMMenuRelatedProducts', function (e) {
                e.preventDefault();
                $(window).scrollTop(0);
                require(['promise!/Scripts/sources/PIM/relatedproducts/controller.js?v=' + assetversion], function () {
                    new PIM.relatedproducts.controller().Initialize(isGrantAccessToModifyPIM);
                });
            })
            .on('click', '#butProductLineItemPaymentTerm', function(e) {
                BrowsePopup('LibraryPaymentTerms', 'Search Payment Terms',
                    [
                        {
                            DisplayText: [{ Column: 'Name' }],
                            Click: function (e) {
                                mPaymentTermID = e.ID;
                                $('#txtOnlyOrgProductLineItemPaymentTerm, #txtOrgProductLineItemPaymentTerm').text(e.Name);
                            }
                        }
                    ]
                );
            })
            .on('click', '#btnOrgProductInfoCancel', function (e) {
                e.preventDefault();
                $(this).hide();
                $('#btnOrgProductInfoSave').hide();
                $('#btnOrgProductEdit').show();
                IsCancelled = true;
                //$.mobile.changePage('#pageOrgProduct');
                $('.statictext').removeClass('staticeditmodeinputtext').addClass('staticmodeinputtext');
                $('.inputtext').removeClass('editmodeinputtext').addClass('staticeditmodeinputtext');
                //$('#btnOrgProductEdit').removeClass('custom-btn-theme-c').addClass('custom-btn-theme-b').removeAttr('actionmode').attr('actionmode', 'edit').text('Edit Information');
                $('#divOrgProductInfoActionButton').hide();
                $(window).scrollTop(0);
            })
            .on('change', '#selOrgProductActivationDateMonth', function (e) {
                e.preventDefault();

                setDayCollection('selOrgProductActivationDateMonth', 'selOrgProductActivationDateDay', 'selOrgProductActivationDateYear', false);
            })
            .on('change', '#selOrgProductActivationDateYear', function (e) {
                e.preventDefault();

                setDayCollection('selOrgProductActivationDateMonth', 'selOrgProductActivationDateDay', 'selOrgProductActivationDateYear', false);
            })
            .on('change', '#selOrgProductDateInActiveMonth', function (e) {
                e.preventDefault();

                setDayCollection('selOrgProductDateInActiveMonth', 'selOrgProductDateInActiveDay', 'selOrgProductDateInActiveYear', false);
                $('#selOrgProductDateInActiveDay').val($.format.date(new Date(), 'd'));
            })
            .on('change', '#selOrgProductDateInActiveYear', function (e) {
                e.preventDefault();

                setDayCollection('selOrgProductDateInActiveMonth', 'selOrgProductDateInActiveDay', 'selOrgProductDateInActiveYear', false);
            })
            .on('click', '#btnOrgProductDone', function (e) {
                e.preventDefault();
                window.open(GetPathFormat("/People/PeopleMenu#pagePopUpMenuProducts"), "_self");
            })
            .on('click', '#btnOrgProductGoToSetupChecklist', function (e) {
                e.preventDefault();
                window.open(GetPathFormat("/SetupChecklist"), "_self");
            })
            .on('change', '#selOrgProductDeactivate', function (e) {
                e.preventDefault();
                ShowHideOrgProductInActiveDetails();
            })
            .on('click', '#btnOrgProductMeasurementAdd', function (e) {
                e.preventDefault();
                LoadUnitOfMeasurePage('add');
                $('#pageOrgProduct').hide();
                $('#divContainerMenuPage').hide();
                //$('#pageSKUMeasurement').show();
                $('#pageSKUPrice').hide();
                $('#pageBOMSKUInfo').hide();
                $('#pageBrowseByOrgOwner').hide();
                $('#pageBarcodeSearch').hide();
                $('#pageCustomAdd').hide();
                $('#pageProductNamePreview').hide();
                DisabledProductFields(false);
                ClearOrgProductMeasurementFields();
            })
            .on('click', '#btnOrgPriceBreakAdd', function (e) {
                e.preventDefault();
                LoadPriceBreakPage('add');
                
            })
            .on('click', '#btnPackingSizeUnit', function (e) {
                BrowsePopup('LibraryPIMMeasure', 'Packing Size Unit',
                    [
                        {
                            DisplayText: [{Column: 'Measurement'}],
                            Click: function(e){
                                $('#txtRecProductMeasurementPackingSizeUnitID').val(e.ID);
                                $('#btnPackingSizeUnit').text(e.Measurement);
                            }
                        }
                    ]
                )
                //                Browser.LibraryPIMMeasure.Click = function (e) {

                //                    $('#txtRecProductMeasurementPackingSizeUnitID').val(e.ID)
                //                    $('#btnPackingSizeUnit').text(e.Measurement);//.button('refresh');

                //                };

                //                Browser.LibraryPIMMeasure.Show();
            })
            .on('click', '#btnPriceBreakUnit', function (e) {
                BrowsePopup('LibraryPIMProductMeasure', 'Packing Size Unit',
                    [
                        {
                            DisplayText: [{Column: 'Measurement'}],
                            Click: function(e){
                                mPriceBreakUnitID = e.UnitMeasureID;
                                $('#btnPriceBreakUnit').text(e.Measurement);
                            }
                        }
                    ],
                    {
                        ProductID: $("#txtOrgProductID").val()
                    }
                );
            })
            //.on('keydown', '#txtPackingSizeQty', function (e) {
            //    if (e.keyCode != 110 && e.keyCode != 190) {
            //        if (!NumberOnly(e)) return;
            //    }
            //})
            //.on('keydown', '#txtPackingSizeVolDiscount', function (e) {
            //    if (e.keyCode != 110 && e.keyCode != 190) {
            //        if (!NumberOnly(e)) return;
            //    }
            //})
            //.on('keydown', '#txtPackingSizeContentQty', function (e) {
            //    if (e.keyCode != 110 && e.keyCode != 190) {
            //        if (!NumberOnly(e)) return;
            //    }
            //})
            .on('click', '#btnPackingSizeContentUnit', function (e) {
                BrowsePopup('LibraryPIMMeasure', 'Packing Size Unit',
                    [
                        {
                            DisplayText: [{Column: 'Measurement'}],
                            Click: function(e){
                                $('#btnPackingSizeContentUnit').text(e.Measurement);
                            }
                        }
                    ]
                )
                //                Browser.LibraryPIMMeasure.Click = function (e) {

                //                    $('#btnPackingSizeContentUnit').text(e.Measurement);//.button('refresh');

                //                };

                //                Browser.LibraryPIMMeasure.Show();
            })
            //.on('keydown', '#txtPackingSizeWeight', function (e) {
            //    if (e.keyCode != 110 && e.keyCode != 190) {
            //        if (!NumberOnly(e)) return;
            //    }
            //})
            .on('click', '#btnPackingSizeWeightUnit', function (e) {
                BrowsePopup('LibraryPIMMeasureWeight', 'Packing Size Weight Unit',
                    [
                        {
                            StyleFormat: 'table',
                            DisplayText: [
                                {
                                    Label: 'Measurement', 
                                    Column: 'Measurement'
                                }, {
                                    Label: 'Sign', 
                                    Column: 'Sign'
                                }
                            ],
                            Click: function(e){
                                $('#btnPackingSizeWeightUnit').text(e.Measurement);
                            }
                        }
                    ]
                );
                //                Browser.LibraryPIMMeasureWeight.Click = function (e) {

                //                    $('#btnPackingSizeWeightUnit').text(e.Measurement);//.button('refresh');

                //                };

                //                Browser.LibraryPIMMeasureWeight.Show();
            })
            //.on('keydown', '#txtPackingSizeDimensionLength', function (e) {
            //    if (e.keyCode != 110 && e.keyCode != 190) {
            //        if (!NumberOnly(e)) return;
            //    }
            //})
            .on('click', '#btnPackingSizeDimLengthUnit', function (e) {
                BrowsePopup('LibraryPIMMeasureLength', 'Packing Size Length Unit',
                    [
                        {
                            DisplayText: [{Column: 'Measurement'}, {Label: 'Sign', Column: 'Sign'}],
                            Click: function(e){
                                $('#btnPackingSizeDimLengthUnit').text(e.Measurement);
                            }
                        }
                    ]
                )
                //                Browser.LibraryPIMMeasureLength.Click = function (e) {

                //                    $('#btnPackingSizeDimLengthUnit').text(e.Measurement);//.button('refresh');

                //                };

                //                Browser.LibraryPIMMeasureLength.Show();
            })
            //.on('keydown', '#txtPackingSizeDimensionWidth', function (e) {
            //    if (e.keyCode != 110 && e.keyCode != 190) {
            //        if (!NumberOnly(e)) return;
            //    }
            //})
            .on('click', '#btnPackingSizeDimWidthUnit', function (e) {
                BrowsePopup('LibraryPIMMeasureLength', 'Packing Size Width Unit',
                    [
                        {
                            DisplayText: [{Column: 'Measurement'}, {Label: 'Sign', Column: 'Sign'}],
                            Click: function(e){
                                $('#btnPackingSizeDimWidthUnit').text(e.Measurement);
                            }
                        }
                    ]
                )
                //                Browser.LibraryPIMMeasureLength.Click = function (e) {

                //                    $('#btnPackingSizeDimWidthUnit').text(e.Measurement);//.button('refresh');

                //                };

                //                Browser.LibraryPIMMeasureLength.Show();
            })
            //.on('keydown', '#txtPackingSizeDimensionHeight', function (e) {
            //    if (e.keyCode != 110 && e.keyCode != 190) {
            //        if (!NumberOnly(e)) return;
            //    }
            //})
            .on('click', '#btnPackingSizeDimensionHeightUnit', function (e) {
                BrowsePopup('LibraryPIMMeasureLength', 'Packing Size Height Unit',
                    [
                        {
                            DisplayText: [{Column: 'Measurement'}, {Label: 'Sign', Column: 'Sign'}],
                            Click: function(e){
                                $('#btnPackingSizeDimensionHeightUnit').text(e.Measurement);
                //                Browser.LibraryPIMMeasureLength.Click = function (e) {

                //                    $('#btnPackingSizeDimensionHeightUnit').text(e.Measurement);//.button('refresh');

                //                };

                //                Browser.LibraryPIMMeasureLength.Show();
                            }
                        }
                    ]
                )
                //                Browser.LibraryPIMMeasureLength.Click = function (e) {

                //                    $('#btnPackingSizeDimensionHeightUnit').text(e.Measurement);//.button('refresh');

                //                };

                //                Browser.LibraryPIMMeasureLength.Show();
            })
            .on('keydown', '#txtProductMeasurementBarcode', function (e) {
                if (e.which == 13) {
                    EnlistMeasurementBarcode($('#txtProductMeasurementBarcode').val());
                };
            })
            .on('click', '#butMeasurementBarCode', function (e) {
                e.preventDefault();

                if ((CBool(window.IsMobile)) && (!CBool(window.InWindows))) {
                    mobile.Scan({
                        Success: function (vData) {
                            EnlistMeasurementBarcode(vData.Value);
                        }
                    });
                } else {
                    NewMessageBox('Scan Barcode / QR Code feature is only available on the mobile app', 'Scan Barcode / QR Code');
                }
            })
            .on('click', '#btnProductMeasurementSave', function (e) {
                e.preventDefault();

                if ($.trim($("#txtRecProductMeasurementPackingSizeUnitID").val()) == "") {
                    NewMessageBox('Please select packing size unit measurement.', 'Packing Size Unit');
                    return;
                }

                if (CDec2($.trim($("#txtPackingSizeQty").val().replace(/,/g, ""))) <= 0) {
                    NewMessageBox('Please enter packing size qty.', 'Packing Size Qty');
                    return;
                }

                if (CBool($('#selUoMTimeBase').is(':checked'))) {
                    if ($('[name="radTimeBase"]:checked').length == 0) {
                        NewMessageBox('Please select time unit.', 'Time-based Unit');
                        return;
                    }
                }

                SaveOrgProductMeasurement();
            })
            .on('click', '#btnProductMeasurementRemove', function (e) {
                e.preventDefault();
                NewMessageBox('Remove this record?', 'Remove', [
                    {
                        Text: 'Yes', Click: function () {
                            RemoveOrgProductMeasurement();
                        }
                    }, { Text: 'No' }
                ]);
            })
            .on('click', '#butPriceBreakSave', function (e) {
                e.preventDefault();

                //if ($.trim($("#txtRecProductMeasurementPackingSizeUnitID").val()) == "") {
                //    $('#TitleOrgEmp').text('Packing Size Unit');
                //    $('#MsgOrgEmp').text('Please select packing size unit measurement.');
                //    $('#panOrgEmpMessage').modal('show');
                //    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                //        e.preventDefault();
                //        $('#panOrgEmpMessage').modal('hide');
                //    })
                //    $('#divBtnCancel').hide();
                //    return;
                //}

                //if (CDec2($.trim($("#txtPackingSizeQty").val().replace(/,/g, ""))) <= 0) {
                //    $('#TitleOrgEmp').text('Packing Size Qty');
                //    $('#MsgOrgEmp').text('Please enter packing size qty.');
                //    $('#panOrgEmpMessage').modal('show');
                //    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                //        e.preventDefault();
                //        $('#panOrgEmpMessage').modal('hide');
                //    })
                //    $('#divBtnCancel').hide();
                //    return;
                //}

                SaveOrgProductPriceBreak();
            })
            .on('click', '#btnOrgProductPriceLevelAdd', function (e) {
                e.preventDefault();

                LoadProductPricesPage('add');
                $('#pageOrgProduct').hide();
                $('#divContainerMenuPage').hide();
                $('#pageSKUMeasurement').hide();
                //$('#pageSKUPrice').show();
                $('#pageBOMSKUInfo').hide();
                $('#pageBrowseByOrgOwner').hide();
                $('#pageBarcodeSearch').hide();
                $('#pageCustomAdd').hide();
                $('#pageProductNamePreview').hide();
                isPriceEdit = 0;
                ClearOrgProductPriceFields();
            })
            .on('click', '#btnProductPriceLevel', function (e) {
                BrowsePopup('LibraryPIMPriceLevel', 'Price Level',
                    [
                        {
                            DisplayText: [{ Column: 'PriceLevel' }],
                            Click: function (e) {
                                $('#txtRecProductPriceLevelID').val(e.ID);
                                $('#btnProductPriceLevel').text(e.PriceLevel);
                            }
                        }
                    ]
                    )
                return;
                $('#TitleLibrary').text('PRODUCT PRICE LEVEL');
                $('#txtSearchKeyword').val('').attr('placeholder', 'Search here');
                $('#panSearchLibrary').modal('show');
                $('#lstOfLibrary').empty(); SearchPriceLvlPopUp($(this).val());
                //                Browser.LibraryPIMPriceLevel.Click = function (e) {

                //                    $('#txtRecProductPriceLevelID').val(e.ID);
                //                    $('#btnProductPriceLevel').text(e.PriceLevel);//.button('refresh');

                //                };

                //                Browser.LibraryPIMPriceLevel.Show();
            })
            //.on('keydown', '#txtProductPrice', function (e) {
            //    if (e.keyCode != 110 && e.keyCode != 190) {
            //        if (!NumberOnly(e)) return;
            //    }
            //})
            .on('click', '#btnProductPriceSave', function (e) {
                e.preventDefault();

                if ($.trim($("#txtRecProductPriceLevelID").val()) == "") {
                    NewMessageBox('Please select price level.', 'Price Level');
                    return;
                }

                if (CDec2($.trim($("#txtProductPrice").val().replace(/,/g, ""))) <= 0) {
                    NewMessageBox('Please enter price', 'Price Level');
                    return;
                }

                if ($('#chbSetUpcomingPrice').is(':checked')) {
                    if (CDec2($('#txtProductUpcomingPrice').val()) <= 0) {
                        NewMessageBox('Please enter upcoming price');
                        return;
                    }

                    if ($('#txtProductUpcomingPriceEffectivityDate').val() == '') {
                        NewMessageBox('Please enter upcoming price effectivity date');
                        return;
                    }
                }

                SaveOrgProductPrice();
            })
            .on('click', '#btnProductPriceRemove', function (e) {
                e.preventDefault();
                NewMessageBox('Remove this record?', 'Remove', [
                    {
                        Text: 'Yes', Click: function () {
                            RemoveOrgProductPrice();
                        }
                    }, { Text: 'No' }
                ]);
            })
            .on('keyup', '#txtBOMComponentName', function (e) {
                $("#legendComponentMeasurement").text("How much " + $.trim($(this).val().replace(/,/g, "")) + " do you need to make " + mParentComponentName);
                $("#legendComponentMethod").text("How do you make " + $.trim($(this).val().replace(/,/g, "")) + ".");
            })
            .on('blur', '#txtBOMComponentName', function (e) {
                $("#legendComponentMeasurement").text("How much " + $("#txtBOMComponentName").val() + " do you need to make " + mParentComponentName);
                $("#legendComponentMethod").text("How do you make " + $("#txtBOMComponentName").val() + ".");
            })
            .on('click', '#btnBOMProduct', function (e) {
                $('#TitleLibrary').text('PRODUCTS');
                $('#txtSearchKeyword').val('').attr('placeholder', 'Search unit here');
                $('#panSearchLibrary').modal('show');
                $('#lstOfLibrary').empty();
                //                Browser.LibraryPIMProduct.InternalInputData.IsSearchMore = 1;
                //                Browser.LibraryPIMProduct.Click = function (e) {

                //                    $('#txtBOMProductID').val(e.ID)
                //                    $('#btnBOMProduct').text(e.ProductName);//.button('refresh');
                //                    getComponentMeasurement(e.ID);

                //                };

                //                Browser.LibraryPIMProduct.Show();
            })
            //.on('keydown', '#txtBOMProductQty', function (e) {
            //    if (e.keyCode != 110 && e.keyCode != 190) {
            //        if (!NumberOnly(e)) return;
            //    }
            //})
            .on('click', '#btnBOMProductUnitMeasure', function (e) {
                Browser.LibraryPIMProductMeasure.InputData.ProductID = $('#txtBOMProductID').val();
                Browser.LibraryPIMProductMeasure.Click = function (e) {

                    $('#txtBOMProductUnitMeasureID').val(e.UnitMeasureID)
                    $('#txtBOMProductUnitMeasureUnitQty').val(e.Quantity)
                    $('#btnBOMProductUnitMeasure').text(e.Measurement);//.button('refresh');

                };

                Browser.LibraryPIMProductMeasure.Show();
            })
            .on('click', '#btnProductBOMSave', function (e) {
                e.preventDefault();

                if ($.trim($("#txtOrgProductID").val()) == "") {
                    NewMessageBox('Please select product.', 'Product', [
                        {
                            Text: 'Ok', Click: function () {
                                $("#divProductInfoContainer").show();
                                $("#divProductBOMContainer").hide();
                            }
                        }
                    ]);
                    return;
                }

                if ($.trim($("#txtBOMProductID").val()) == "") {
                    NewMessageBox('Please select component SKU.', 'SKU', [
                        {
                            Text: 'Ok', Click: function () {
                                $("#divProductInfoContainer").hide();
                                $("#divProductBOMContainer").show();
                            }
                        }
                    ]);
                    return;
                }

                if ($.trim($('#txtBOMProductQty').val()) == "-") {
                    NewMessageBox('Please enter component quantity.', 'Quantity');
                    return;
                }

                if (parseFloat(CDec2($('#txtBOMProductQty').val().replace(/,/g, ""))) <= 0) {
                    NewMessageBox('Please enter component quantity.', 'Quantity');
                    return;
                }

                if ($.trim($("#selUnitMeasure").val()) == "") {
                    NewMessageBox('Please select unit of measurement.', 'Component');
                    return;
                }

                if ($.trim($("#selComponentMethod").val()) == "") {
                    NewMessageBox('Please specify how do you make this component.', 'Component');
                    return;
                }

                var pData = {};
                pData.OrgProductBOMRecID = $("#txtRecBOMProductID").val();
                pData.OrganizationID = sessionStorage["OrgID"];
                pData.ProductID = $("#txtOrgProductID").val();
                pData.ComponentName = $("#txtBOMComponentName").val();
                pData.BOMProductID = $("#txtBOMProductID").val();
                pData.Qty = $("#txtBOMProductQty").val();
                pData.UnitMeasureID = $("#selUnitMeasure").val();
                pData.ComponentMethod = $("#selComponentMethod").val();
                pData.ParentBOMProductID = mParentID;

                AjaxSend({
                    url: 'Organization/SaveOrganizationProductBOM',
                    data: pData,
                    success: function (vData) {

                        GetProductBOMData();
                        //$.mobile.changePage('#pageOrgProductInfo');
                        $('#pageOrgProduct').hide();
                        $('#pageOrgProductInfo').show();
                        $('#pageSKUMeasurement').hide();
                        $('#pageSKUPrice').hide();
                        $('#pageBOMSKUInfo').hide();
                        $('#pageBrowseByOrgOwner').hide();
                        $('#pageBarcodeSearch').hide();
                        $('#pageCustomAdd').hide();
                        $('#pageProductNamePreview').hide();
                    }
                });
            })
            .on('click', '#btnProductBOMRemove', function (e) {
                e.preventDefault();
                NewMessageBox('Remove this record?', 'Remove', [
                    {
                        Text: 'Yes', Click: function () {
                            RemoveProductBOMData();
                        }
                    }, { Text: 'No' }
                ]);
            })
            .on('click', '#btnProductBOMCancel', function (e) {
                e.preventDefault();
                //$.mobile.changePage('#pageOrgProductInfo');
                $('#pageOrgProduct').hide();
                $('#pageOrgProductInfo').show();
                $('#pageSKUMeasurement').hide();
                $('#pageSKUPrice').hide();
                $('#pageBOMSKUInfo').hide();
                $('#pageBrowseByOrgOwner').hide();
                $('#pageBarcodeSearch').hide();
                $('#pageCustomAdd').hide();
                $('#pageProductNamePreview').hide();
            })

            //additional by janine browse by brand owner
            //jan 21 2015

            .on('click', '#btnAddNewProducts', function (e) {
                e.preventDefault();
                $("#divAddProductPopup").popup("open");
            })
            .on('click', '#btnBarcodeSearch', function (e) {
                e.preventDefault();
                //$.mobile.changePage('#pageBarcodeSearch');
                $('#pageOrgProduct').hide();
                $('#pageOrgProductInfo').hide();
                $('#pageSKUMeasurement').hide();
                $('#pageSKUPrice').hide();
                $('#pageBOMSKUInfo').hide();
                $('#pageBrowseByOrgOwner').hide();
                $('#pageBarcodeSearch').show();
                $('#pageCustomAdd').hide();
                $('#pageProductNamePreview').hide();
            })
            .on('click', '#btnExcelFile', function (e) {
                e.preventDefault();
                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {
                        window.open(GetPathFormat("/PIMUpload") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                    },
                    error: function (vError) {
                        NewMessageBox('This feature requires internet connection.', 'Requires Internet Connection');
                        return;
                    }
                });
            })
            .on('click', '#btnSubscribe', function (e) {
                e.preventDefault();

                //$.mobile.changePage('#pageBrowseByOrgOwner');
                $('#pageOrgProduct').hide();
                $('#pageOrgProductInfo').hide();
                $('#pageSKUMeasurement').hide();
                $('#pageSKUPrice').hide();
                $('#pageBOMSKUInfo').hide();
                $('#pageBrowseByOrgOwner').show();
                $('#pageBarcodeSearch').hide();
                $('#pageCustomAdd').hide();
                $('#pageProductNamePreview').hide();

                GetAllOrgOwners();
            })
            .on('submit', '#frmOrgOwnerSearch', function (e) {
                e.preventDefault();
                GetAllOrgOwners();
            })
            .on('click', '#btnExpress', function (e) {
                e.preventDefault();
                NewMessageBox("Are you sure you want to add all of " + selectedOrgPublisherName + "'s products to your company-wide product list? The SRP (Suggested Retail Price) of the products set by the publisher will automatically become the default RETAIL PRICE of the products in your company. You may change the prices later.", 'Add to Company-Wide Product List', [
                    {
                        Text: 'Yes', Click: function () {
                            if (CStr2(selectedOrgPublisherID) != '') {
                                AddOrgOwnerProductList(selectedOrgPublisherID);
                            } else {
                                NewMessageBox('Invalid organization', 'Invalid');
                            }
                        }
                    }, { Text: 'No' }
                ]);
            })
            .on('click', '#btnCustom', function (e) {
                e.preventDefault();
                $('#divExpressCustomPopup').modal('hide');
                //$.mobile.changePage('#pageCustomAdd');
                $('#pageOrgProduct').hide();
                $('#pageOrgProductInfo').hide();
                $('#pageSKUMeasurement').hide();
                $('#pageSKUPrice').hide();
                $('#pageBOMSKUInfo').hide();
                $('#pageBrowseByOrgOwner').hide();
                $('#pageBarcodeSearch').hide();
                $('#pageCustomAdd').show();
                $('#pageProductNamePreview').hide();
                ClearCustomAddPage();

                $('#selFilter').change(function (e) {
                    e.preventDefault();
                    //if not blank
                    if ($('#selFilter').val() != '') {

                        var pData = {
                            FilterName: $('#selFilter :selected').val(),
                            ID: selectedOrgPublisherID
                        };

                        AjaxSend({
                            url: 'Organization/GetOrgProductFilterResult',
                            data: pData,
                            success: function (vData) {

                                if (vData.Filter.Rows.length > 0) {
                                    DisplayResults(vData.Filter.Rows);
                                    $("#divResult").show();
                                    $("#divNoResult").hide();
                                } else {
                                    $("#divResult").hide();
                                    $("#divNoResult").show();
                                }

                            }
                        });
                    } else {
                        ClearCustomAddPage();
                    }

                });
            })
            .on('click', '#btnAddProductsCustom', function (e) {
                e.preventDefault();
                //get selected
                var filter = $("input[name='filter']").serializeArray();
                var arrayList = [];


                if (filter.length > 0) {
                    for (var i = 0; i < filter.length; i++) {
                        arrayList.push(filter[i].value);
                    }
                }

                if (arrayList.length > 0) {
                    var pData = {
                        FilterName: $.trim($('#selFilter :selected').val()),
                        Keywords: arrayList,
                        ID: selectedOrgPublisherID
                    };
                    AjaxSend({
                        url: 'Organization/GetProductNames',
                        data: pData,
                        success: function (vData) {

                            //$.mobile.changePage('#pageProductNamePreview');
                            $('#pageOrgProduct').hide();
                            $('#pageOrgProductInfo').hide();
                            $('#pageSKUMeasurement').hide();
                            $('#pageSKUPrice').hide();
                            $('#pageBOMSKUInfo').hide();
                            $('#pageBrowseByOrgOwner').hide();
                            $('#pageBarcodeSearch').hide();
                            $('#pageCustomAdd').hide();
                            $('#pageProductNamePreview').show();
                            if (vData.OrgData != undefined) {
                                DisplayResultsProductName(vData.OrgData.Rows);
                            }

                        }
                    });

                } else {
                    NewMessageBox('Please select at least one.', 'Custom Add');
                }
            })
            .on('click', '#btnAddProductsCustomCancel', function () {
                $('#tblBody').empty();
                window.location.hash = 'LibraryPage';
                LibraryTable.PIMPage();
            })
            .on('click', '#btnAddProductsContinue', function (e) {
                e.preventDefault();
                //get selected
                var filter = $("input[name='filter']").serializeArray();
                var arrayList = [];


                if (filter.length > 0) {
                    for (var i = 0; i < filter.length; i++) {
                        arrayList.push(filter[i].value);
                    }
                }

                if (arrayList.length > 0) {
                    var pData = {
                        FilterName: $.trim($('#selFilter :selected').val()),
                        Keywords: arrayList,
                        ID: selectedOrgPublisherID
                    };

                    AddOrgOwnerProductListCustom(pData);

                } else {
                    NewMessageBox('Please select at least one.', 'Custom Add');
                }
            })
            .on('click', '#btnAddProductsCancel', function (e) {
                e.preventDefault();

                history.back();
            })
            .on('click', '#btnAddProductBOMMain', function (e) {
                e.preventDefault();

                mParentID = "";
                mParentComponentName = $("#spanProductShortName").text();
                ShowProductBOMInfo(null);
            })
            .on('keypress', '#txtSearchKeyword', function (e) {
                if (e.keyCode == 13) {
                    if ($('#TitleLibrary').text() == "PRODUCT") {
                        SearchPublicProduct($(this).val());
                    }
                    else if ($('#TitleLibrary').text() == "PRODUCTS") {
                        SearchProductPopUp($(this).val());
                    }
                    else if ($('#TitleLibrary').text() == "PRODUCT PRICE LEVEL") {
                        SearchPriceLvlPopUp($(this).val());
                    }
                    else if ($('#TitleLibrary').text() == "PACKING SIZE UNIT") {
                        SearchPackingSizeUnitPopUp($(this).val());
                    }
                    else if ($('#TitleLibrary').text() == "PACKING SIZE WEIGTH UNIT") {
                        SearchPackingSizeWeightUnitPopUp($(this).val());
                    }
                    else if ($('#TitleLibrary').text() == "PACKING SIZE LENGTH UNIT" || $('#TitleLibrary').text() == "PACKING SIZE WIDTH UNIT" || $('#TitleLibrary').text() == "PACKING SIZE HEIGHT UNIT") {
                        SearchPackingSizeLengthUnitPopUp($(this).val());
                    }
                    else if ($('#TitleLibrary').text() == "PIM") {
                        isAddRawMaterial = 1;
                        SearchProductPopUp($(this).val());
                    }
                }
            })
            .on('click', '#btnOrgProductEdit', function () {
                //if ($(this).attr('actionmode') == "edit") {
                $('.statictext').removeClass('staticmodeinputtext').addClass('staticeditmodeinputtext');
                $('.inputtext').removeClass('staticeditmodeinputtext').addClass('editmodeinputtext');
                //$('#btnOrgProductEdit').removeClass('custom-btn-theme-b').addClass('custom-btn-theme-c').removeAttr('actionmode').attr('actionmode', 'cancel').text('Cancel');
                $('#btnOrgProductInfoCancel, #btnOrgProductInfoSave').show();
                $(this).hide();
                EditModeProductInfo();
                //}
                //else {
                //    $('.statictext').removeClass('staticeditmodeinputtext').addClass('staticmodeinputtext');
                //    $('.inputtext').removeClass('editmodeinputtext').addClass('staticeditmodeinputtext');
                //    $('#btnOrgProductEdit').removeClass('custom-btn-theme-c').addClass('custom-btn-theme-b').removeAttr('actionmode').attr('actionmode', 'edit').text('Edit Information');
                //    $('#divOrgProductInfoActionButton').hide();
                //}
            })
            .on('click', '#btnOrgProductBOMInstructionEdit', function () {
                if ($(this).attr('actionmode') == "edit") {
                    $('.statictextarea').removeClass('staticmodetextarea').addClass('staticeditmodetextarea');
                    $('.inputtextarea').removeClass('staticeditmodetextarea').addClass('editmodetextarea');
                    $('#btnOrgProductBOMInstructionEdit').removeAttr('actionmode').attr('actionmode', 'cancel').text('Cancel');
                    $('#txtareaBomInstruction').focus();
                }
                else {
                    $('.statictextarea').removeClass('staticeditmodetextarea').addClass('staticmodetextarea');
                    $('.inputtextarea').removeClass('editmodetextarea').addClass('staticeditmodetextarea');
                    $('#btnOrgProductBOMInstructionEdit').removeAttr('actionmode').attr('actionmode', 'edit').text('Edit');
                }
            })
            .on('click', '#btnOrgProdSaveBomInstruction', function () {
                SaveOrgProdBomDefinition();
            })
            .on('click', '#btnOrgProdSaveSeperateIntoComponentDuringSalesReturns', function () {
                SaveOrgProdSeperateIntoComponentDuringSalesReturns();
            })
            .on('click', '#btnOrgProductBOMAdd', function () {
                BrowsePopup('LibraryPIMProduct', 'PIM',
                    [
                        {
                            Message: 'Please choose a content for <strong>' + productname + '</strong>',
                            DisplayText: [{ Label: 'Product No.', Column: 'ProductNo' }, { Label: 'Product Name', Column: 'ProductName' }, { Label: 'Published by', Column: 'PublishedBy' }, { Label: 'Uploaded by', Column: 'UploadedBy' }],
                            Click: function (e) {
                                $('#btnBackBomRawMaterials').show();
                                $('#panAddRawMaterials').modal('show').unbind('shown.bs.modal').on('shown.bs.modal', function () {
                                    $('#txtQty').val('').focus();
                                });
                                childProductID = e.ID;
                                GetBomPageContentData(e.ID);
                            }
                        }
                    ]
                    )
                return;
                $('#panSearchLibrary').modal('show');
                $('#TitleLibrary').text('PIM');
                $('.divBrowserContentTop').html('Please choose a content for <strong>' + productname + '</strong>');
                $('#txtSearchKeyword').attr('placeholder', 'Search Here').val($('#txtPublicSearch').val()).focus();                
                $('#lstOfLibrary').empty();
                PopupReposition();
            })
            .on('click', '#btnOrgProductBOMExcel', function () {
                // Import data from an Excel file
                window.location = '/PIMProduct/BomUpload';
            })
            .on('click', '#btnCancelBomContent', function () {
                $('#panAddRawMaterials').modal('hide');
            })
            .on('click', '#btnCancelBomUnitDefinition', function () {
                $('#divBomUnitDefinition').css({ visibility: 'hidden', display: 'none' });
                $('#divBomContent').css({ visibility: 'visible', display: 'block' });
                $('#BOMTitle').text('Content');
                $('#cbUnit').val($('#cbUnit option:first').val());//.selectmenu('refresh');
            })
            .on('change', '#cbUnit', function () {
                if ($(this).val() == "define") {
                    $('#divBomUnitDefinition').css({ visibility: 'visible', display: 'block' });
                    $('#divBomContent').css({ visibility: 'hidden', display: 'none' });
                    $('#BOMTitle').text('Unit Definition');
                    GetUnitDefinitionData();
                } else {
                    UpdateHeaderContent();
                }
            })
            .on('click', '#btnSaveBomUnitDefinition', function () {
                if ($('#txtQtyUD').val() == "" || $('#txtQtyUD').val() == "0") {
                    WebToast('Please enter quantity', 'top');
                    return;
                }
                SaveBomUnitDefinition();
            })
            .on('keydown', '#txtQty', function (e) {
                if (e.which == 13) {
                    $('#btnSaveBomContent').click();
                }
            })
            .on('click', '#btnSaveBomContent', function () {
                if ($('#txtQty').val() == "" || $('#txtQty').val() == "0") {
                    WebToast('Please enter quantity', 'top');
                    return;
                }
                SaveBomContent();
            })
            .on('click', '#btnBackBomRawMaterials', function () {
                $('#btnOrgProductBOMAdd').click();
                return;
                $('#panAddRawMaterials').modal('hide');
                $('#panSearchLibrary').modal('show');
            })
            .on('change', 'input:radio[name=optJit]', function () {
                $('#divActionButtonForJit').show();
            })
            .on('change', '#selSeperateIntoComponentDuringSalesReturns', function () {
                $('#divActionButtonForComponentsDuringSalesReturns').show();
            })
            .on('click', '#btnOrgProdSaveChangesBom', function () {
                SaveOrgProdBomDefinition();
            })            
            .on('click', '#btnOrgProdCancelSeperateIntoComponentDuringSalesReturns', function () {                    
                $('#divActionButtonForComponentsDuringSalesReturns').hide();
            })
            .on('click', '#btnOrgProdCancelChangesBom', function () {
                $('input:radio[name=optJit]').removeAttr('checked');//.checkboxradio('refresh');
                $('input:radio[name=optJit][value=' + jit + ']').prop('checked', 1);//.checkboxradio('refresh');
                $('#divActionButtonForJit').hide();
            })
            .on('click', '#btnOrgProdCancelBomInstruction', function () {
                $('.statictextarea').removeClass('staticeditmodetextarea').addClass('staticmodetextarea');
                $('.inputtextarea').removeClass('editmodetextarea').addClass('staticeditmodetextarea');
                $('#btnOrgProductBOMInstructionEdit').removeAttr('actionmode').attr('actionmode', 'edit').text('Edit');
            })
            .on('click', '#butOrgProductUnitOfMeasuresDelete', function () {
                NewMessageBox('Are you sure you want to delete this unit measure?', 'Delete?', [
                    {
                        Text: 'Yes', 
                        Click: function() {
                            AjaxSend({
                                url: "Organization/DeleteProductUnitMeasure",
                                data: {
                                    ID: $("#txtRecProductMeasurementID").val()
                                },
                                success: function(vData) {
                                    if (vData.Error)
                                        NewMessageBox(vData.Error);
                                    else if (vData.Result == 'ok') {
                                        NewMessageBox('The unit measure was successfully deleted.');
                                        $('#btnPIMMenuUnitOfMeasures').click();
                                        GetProductMeasurementList($("#txtOrgProductID").val());
                                    }
                                }
                            });
                        }
                    },
                    {
                        Text: 'No'
                    }
                ]);
            })
            .on('click', '#btnOrgProductUnitOfMeasures', function () {
                //if ($(this).attr('actionmode') == "edit") {
                $('.statictext.unitofmeasure').removeClass('staticmodeinputtext').addClass('staticeditmodeinputtext');
                $('.inputtext.unitofmeasure').removeClass('staticeditmodeinputtext').addClass('editmodeinputtext');
                $('#btnOrgProductUnitOfMeasures').hide(); //.removeClass('custom-btn-theme-b').addClass('custom-btn-theme-c').removeAttr('actionmode').attr('actionmode', 'cancel').text('Cancel');
                $('#divUnitOfMeasureActionButton').show();
                EditModeUnitOfMeasures();
                //}
                //else {
                //    $('.statictext.unitofmeasure').removeClass('staticeditmodeinputtext').addClass('staticmodeinputtext');
                //    $('.inputtext.unitofmeasure').removeClass('editmodeinputtext').addClass('staticeditmodeinputtext');
                //    $('#btnOrgProductUnitOfMeasures').removeClass('custom-btn-theme-c').addClass('custom-btn-theme-b').removeAttr('actionmode').attr('actionmode', 'edit').text('Edit Details');
                //    $('#divUnitOfMeasureActionButton').hide();
                //}
            })
            .on('click', '#btnProductMeasurementCancel, #butPriceBreakCancel', function () {
                $('.statictext.unitofmeasure').removeClass('staticeditmodeinputtext').addClass('staticmodeinputtext');
                $('.inputtext.unitofmeasure').removeClass('editmodeinputtext').addClass('staticeditmodeinputtext');
                $('#btnOrgProductUnitOfMeasures').removeClass('custom-btn-theme-c').addClass('custom-btn-theme-b').removeAttr('actionmode').attr('actionmode', 'edit').text('Edit Details');
                $('#divUnitOfMeasureActionButton').hide();
                $('#divContainerMenuPage').show();
                $('#pageSKUMeasurement').hide();
                $(window).scrollTop(0);
            })
            .on('click', '#btnOrgProductEditProductPrices', function () {
                //if ($(this).attr('actionmode') == "edit") {
                $('.statictext.productprices').removeClass('staticmodeinputtext').addClass('staticeditmodeinputtext');
                $('.inputtext.productprices').removeClass('staticeditmodeinputtext').addClass('editmodeinputtext');
                //$('#btnOrgProductEditProductPrices').removeClass('custom-btn-theme-b').addClass('custom-btn-theme-c').removeAttr('actionmode').attr('actionmode', 'cancel').text('Cancel');
                $('#divProductPricesActionButton, #btnProductPriceSave').show();
                $('#btnOrgProductEditProductPrices, #btnProductPriceBack').hide();
                $('#txtProductPrice').val($('#txtOnlyProductPrice').text());
                //}
                //else {
                //    $('.statictext.productprices').removeClass('staticeditmodeinputtext').addClass('staticmodeinputtext');
                //    $('.inputtext.productprices').removeClass('editmodeinputtext').addClass('staticeditmodeinputtext');
                //    $('#btnOrgProductEditProductPrices').removeClass('custom-btn-theme-c').addClass('custom-btn-theme-b').removeAttr('actionmode').attr('actionmode', 'edit').text('Edit Details');
                //    $('#divProductPricesActionButton').hide();
                //}
            })
            .on('click', '#btnProductPriceCancel', function (e) {
                $('#pageOrgProduct, #btnProductPriceSave, #btnProductPriceRemove').hide();
                $('#divContainerMenuPage').show();
                $('#pageSKUMeasurement').hide();
                $('#pageSKUPrice').hide();
                $('#pageBOMSKUInfo').hide();
                $('#pageBrowseByOrgOwner').hide();
                $('#pageBarcodeSearch').hide();
                $('#pageCustomAdd').hide();
                $('#pageProductNamePreview').hide();
            })
            .on('click', '#btnProductPriceBack', function () {
                $('#btnPIMMenuProductPrices').click();
            })
            .on('click', '#btnUploadProductImage', function () {
                $('#btnPIMMenuPictureGallery').click();
            })
            .on('click', '#btnChoosePicture', function () {
                if ($('#imgSelection img').length == 5) {
                    WebToast('Maximum of 5 pictures only', 'top');
                    return;
                }

                $('#fileUploadProductImage').click();
            })

                .on('click', '#btnPIMMenuFileAttachments', function () {
                    var listPimFileAttachments = $('#listPimFileAttachments').empty();

                    AjaxSend({
                        url: '/Organization/GetPimFileAttachments',
                        data: { ProductID: sessionStorage.ViewID },
                        success: function (e) {
                            var files = e.Results.Rows;

                            for (var i in files) {
                                listPimFileAttachments.append($('<li class="list-group-item p-3">')
                                    .append($('<div class="row align-items-center">')
                                        .append($('<div class="col text-dark">').text((CInt2(i) + 1) + '. ' + files[i].FileName))
                                        .append($('<div class="col-auto">')
                                            .append($('<div class="btn-group">')
                                                .append($('<button data-role="none" title="Download" class="btn btn-light border text-primary">')
                                                    .data('data', files[i])
                                                    .append('<span class="bi bi-download"></span>')
                                                    .click(function () {
                                                        var pUrl = MergeUrlParameters({
                                                            OrgID: sessionStorage.OrgID,
                                                            ID: $(this).data('data').ID
                                                        }, '/Organization/DownloadPimFileAttachment');
                                                        window.open(pUrl, '_blank');
                                                    })
                                                )
                                                .append($('<button data-role="none" title="Remove" class="btn btn-light border text-danger">')
                                                    .data('data', files[i])
                                                    .append('<span class="bi bi-trash-fill"></span>')
                                                    .click(function () {
                                                        var vID = $(this).data('data').ID;

                                                        NewMessageBox('Are you sure you want to remove this file "' + $(this).data('data').FileName + '"? Once removed, it cannot be undone.', 'Confirmation Dialog', [
                                                            {
                                                                Text: 'Yes', Click: function () {
                                                                    AjaxSend({
                                                                        url: '/Organization/RemovePimFileAttachment',
                                                                        data: { ID: vID },
                                                                        success: function () {
                                                                            WebToast('File has been removed', 'top');
                                                                            $('#btnPIMMenuFileAttachments').click();
                                                                        }
                                                                    })
                                                                }
                                                            }, { Text: 'No' }
                                                        ])
                                                    })
                                                )
                                            )                                            
                                        )
                                    )
                                )
                            }
                        }
                    })
                })
                .on('change', '#filePimAttachments', function () {
                    var filePimAttachments = $(this);
                    var formdata = new FormData();
                    var url = MergeUrlParameters({
                        OrgID: sessionStorage.OrgID,
                        ProductID: sessionStorage.ViewID
                    }, '/Organization/UploadPimAttachments')

                        for (var i = 0; i < filePimAttachments[0].files.length; i++) {
                            formdata.append(filePimAttachments[0].files[i].name, filePimAttachments[0].files[i]);
                    }

                    Bs.Loading('show', 'Please wait while uploading file(s) ...');

                    AjaxSend({
                        url: url,
                        type: 'POST',
                        data: formdata,
                        async: true,
                        cache: false,
                        contentType: false,
                        processData: false,
                        success: function (vData) {
                            filePimAttachments.val('');

                            if (vData.Results == 'saved') {
                                WebToast('File(s) were successfully uploaded', 'top');
                                $('#btnPIMMenuFileAttachments').click();
                            } else {
                                NewMessageBox('Thers\'s an error uploading attachment(s).', 'Message');
                            }
                        }, complete: function () {
                            Bs.Loading('hide');
                        }
                    })
                })
                //Buttons for Modifiers
            .on('click', '#btnKitOptions', function () {
                $('#pageKitOptions').pageshow();
            })
            .on('click', '#btnAddGroupKitOptions', function () {
                $('#pageAddGroup').pageshow();
            })
            .on('click', '#btnKOGroupItem', function () {
                BrowsePopup('LibraryPIMProduct', 'Item',
                        [
                            {
                                DisplayText: [{ Column: 'ProductName' }],
                                Click: function (e) {
                                    arrKOGroupItems.push(e)
                                    DisplayKOGroupItem(arrKOGroupItems)
                                }
                            }
                        ]
                    )
            })

            .on('change', '#selUoMTimeBase', function () {
                $('[name="radTimeBase"]').removeAttr('checked');//.checkboxradio('refresh');
                if(CBool($(this).is(':checked'))){
                    $('#divForTimeBase').show();
                } else {
                    $('#divForTimeBase').hide();
                }
            })
            .on('click', '#btnAddBarcode', function () {
                if (CDec2($.trim($("#txtPackingSizeQty").val().replace(/,/g, ""))) <= 0) {
                    NewMessageBox('Please enter first packing size qty.', 'Message');
                    return;
                }

                if ($.trim($("#txtRecProductMeasurementPackingSizeUnitID").val()) == "") {
                    NewMessageBox('Please select first packing size unit measurement.', 'Message');
                    return;
                }
                $('#popBarcode').modal('show');//.trigger('create').modal();
                $('#txtBarcodeID').val('');
                $('#txtBarcodeNumber').val('').focus();
                $('#selBarcodeType').val('f848c611-a271-4aea-94a3-1a5164333dd4');//.selectmenu('refresh');                
            })
            .on('click', '#btnBarcodeSave', function () {
                SaveBarcode()
            })
            .on('click', '#btnPIMMenuInventoryLedger', function () {

                mProductName = productname;
                
                sessionStorage.IsFromInventoryBalance = true;
                sessionStorage.InventoryBalanceProductID = sessionStorage.ViewID;
                sessionStorage.InventoryBalanceProductName = mProductName;

                //if (mInventoryLedgerLoaded) {
                //    InventoryLedger.CheckSource();
                //    return;
                //}
                    
                //mInventoryLedgerLoaded = true;

                AjaxSend({
                    ExactUrl: '/InventoryLedger?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=1',
                    type: 'GET',
                    dataType: 'text',
                    success: function (vData) {
                        sessionStorage["Inventory"] = "ledger";
                        $('#piminventoryledger').empty();
                        $('#piminventoryledger').append(vData);//.trigger('create');

                        if (isFinishedLoadIL) {
                            InventoryLedger.CheckSource();
                        }

                        isFinishedLoadIL = true;
                    }
                });
            })

            .on('click', '#btnEditUpcharge', function () {
                $('#popupUpcharge').modal('show');
                $('#txtUpcharge').val($('#txtEncodedUpcharge').val());
            })
            .on('keydown', '#txtUpcharge', function (e) {
                if (!NumberDotOnly($(this), e)) {
                    return;
                }
            })
            .on('click', '#btnSaveUpcharge', function () {
                var txtUpcharge = $('#txtUpcharge').val();

                AjaxSend({
                    url: 'Stock/SaveUpcharge',
                    data: {
                        ProductID: sessionStorage.ViewID,
                        Upcharge: txtUpcharge
                    }, success: function (e) {
                        if (e.Result == 'ok') {
                            GetProductContent(sessionStorage.ViewID);
                            WebToast('Upcharge was successfully saved', 'top');
                            $('#popupUpcharge').modal('hide');
                        } else {
                            NewMessageBox(e.Result);
                        }
                    }
                })
            })
            .on('click', '#btnAddVendorPrice', function () {
                $('#popEditVendorPrice .modal-title').text('Add Vendor Price');
                var pCB = $('[name=selVendorPriceUnitMeasure]');
                    pCB.html('');
                for (var pIndex = 0; pIndex <= mUnitMeasures.length - 1; pIndex++) {
                    var pDR = mUnitMeasures[pIndex];
                    var pText = pDR.UnitMeasure;
                    if (CDec2(pDR.Quantity) != mBaseUnitQty)
                        pText += ' (' + CDec2(pDR.Quantity) + ' ' + mBaseUnit + ')';
                    var pOption = new Option(pText, pDR.Quantity);
                    pOption = $(pOption);
                    pOption.data('data', pDR);
                    pCB.append(pOption);
                }
                $('[name="txtUnitMeasure"]').val(mBaseUnit); 
                $('[name="txtVendorPriceName"]').val('').unbind('click').click(function () {
                    BrowsePopup('LibraryOrganizationSupplier', 'Search Vendor', [
                        {
                            DisplayText: [{ Column: 'SupplierName' }],
                            Click: function(e) {
                                $('[name="txtVendorPriceName"]').val(e.SupplierName);

                                $('[name="butSaveVendorPrice"]').unbind('click').click(function() {
                                    var pOption = $('[name=selVendorPriceUnitMeasure] option:selected');
                                    if (pOption.length == 0) {
                                        WebToast('Please select a unit measure first.');
                                        return;
                                    }
                                    var pOptionData = pOption.data('data');
                                    AjaxSend({
                                        url: '/Organization/SaveVendorPrice',
                                        data: {
                                            VendorID: e.ID,
                                            Price: CDec2($('[name="txtVendorPrice"]').val()),
                                            ProductID: sessionStorage.ViewID,
                                            IsActive: 1,
                                            UnitMeasure: pOptionData.UnitMeasure,
                                            UnitQty: pOptionData.Quantity,
                                            Url: $('[name="selVendorPriceURL"]').val(),
                                            Note: $('[name="txtVendorPriceNote"]').val(),
                                        },
                                        success: function(e) {
                                            $('#popEditVendorPrice').modal('hide');
                                            if (e.Results == 'saved') {
                                                GetProductPriceList(sessionStorage.ViewID);
                                                WebToast('Vendor Price has been successfully saved');
                                            } else {
                                                NewMessageBox('There was an error while saving Vendor\'s price', 'Error');
                                            }
                                        }
                                    });
                                });
                            }
                    }]);
                });
                $('[name="txtVendorPrice"]').val('');
                $('#popEditVendorPrice').modal('show');
            })
            .on('change', '#selVendorPriceStatus', function () {
                GetProductPriceList(sessionStorage.ViewID);
            })
            .on('click', '#btnSearchProductBrand', function () {
                BrowsePopup('LibraryPimBrands', 'Product Brands', [
                    {
                        DisplayText: [{ Column: 'Brand' }],
                        Click: function (e) {
                            $('#txtOrgProductBrand').val(e.Brand);
                        }
                    }
                ])
            })
            .on('click', '#btnSearchProductGroupNo', function () {
                BrowsePopup('LibraryPimProductGroupNo', 'Product Group No', [
                    {
                        DisplayText: [{ Column: 'ProductGroupNo' }],
                        Click: function (e) {
                            $('#txtOrgProductGroupNo').val(e.ProductGroupNo);
                        }
                    }
                ])
            })
            .on('click', '#btnSearchProductCategory', function () {
                BrowsePopup('LibraryPimCategories', 'Product Categories', [
                    {
                        DisplayText: [{ Column: 'Category' }],
                        Click: function (e) {
                            $('#txtOrgProductCategory').val(e.Category);
                        }
                    }
                ])
            })

            .on('change', '#chbSetUpcomingPrice', function () {
                ($(this).is(':checked') ? $('#divUpcomingPrice').show() : $('#divUpcomingPrice').hide());
            })
            .on('click', '#txtProductUpcomingPriceEffectivityDate', function () {
                var dis = $(this);
                SingleCalendar({
                    MinDate: $.format.date(new Date(), 'MMM dd, yyyy'),
                    Click: function (e) {
                        dis.val(e);
                    }
                })
            })

                .on('change', '[name="chbMaxOption"]', function () {
                    if ($(this).val() == 'custom') {
                        $('#txtOrgMaxAllowableDiscountableAmount').show();
                        $('#divMaxReferenceSKU').hide();
                    } else {
                        $('#txtOrgMaxAllowableDiscountableAmount').hide();
                        $('#divMaxReferenceSKU').show();
                    }
                })
                .on('click', '#btnMaxReferenceSKUSearch', function () {
                    BrowsePopup('LibraryPIMProduct', 'Search Product', [
                        {
                            StyleFormat: 'table',
                            DisplayText: [{ Label: 'Product No.', Column: 'ProductNo' }, { Label: 'Description', Column: 'ProductName' }],
                            Click: function (e) {
                                maxreferenceskuid = e.ID;
                                $('#txtMaxReferenceSKUName').val(e.ProductNo + ' - ' + e.ProductName);
                            }
                        }
                    ])
                })

            function SaveBarcode() {
                if ($.trim($('#txtBarcodeNumber').val()) == '') {
                    WebToast('Please enter barcode number', 'top');
                    return;
                }
                $('#btnBarcodeSave').text('Saving ...').attr('disabled', true);
                var pData = {};

                pData.StockMeasurementsID = $("#txtRecProductMeasurementID").val();
                pData.BarcodeID = $('#txtBarcodeID').val();
                pData.Barcode = $.trim($('#txtBarcodeNumber').val());
                pData.BarcodeType = $('#selBarcodeType').val();
                pData.ProductID = $("#txtOrgProductID").val();
                pData.UnitMeasureID = $("#txtRecProductMeasurementPackingSizeUnitID").val();
                pData.Quantity = $("#txtPackingSizeQty").val();
                pData.Discount = $("#txtPackingSizeVolDiscount").val();
                pData.ContentQty = $("#txtPackingSizeContentQty").val();
                pData.ContentUnit = ($("#btnPackingSizeContentUnit").text() == "Please Select" ? "" : $.trim($("#btnPackingSizeContentUnit").text()));
                pData.ContentWeight = $("#txtPackingSizeWeight").val();
                pData.ContentWeightUnit = ($("#btnPackingSizeWeightUnit").text() == "Please Select" ? "" : $.trim($("#btnPackingSizeWeightUnit").text()));
                pData.DimensionLenght = $("#txtPackingSizeDimensionLength").val();
                pData.DimensionLenghtUnit = ($("#btnPackingSizeDimLengthUnit").text() == "Please Select" ? "" : $.trim($("#btnPackingSizeDimLengthUnit").text()));
                pData.DimensionWidth = $("#txtPackingSizeDimensionWidth").val();
                pData.DimensionWidthUnit = ($("#btnPackingSizeDimWidthUnit").text() == "Please Select" ? "" : $.trim($("#btnPackingSizeDimWidthUnit").text()));
                pData.DimensionHeight = $("#txtPackingSizeDimensionHeight").val();
                pData.DimensionHeightUnit = ($("#btnPackingSizeDimensionHeightUnit").text() == "Please Select" ? "" : $.trim($("#btnPackingSizeDimensionHeightUnit").text()));
                pData.Weight = $("#txtPackingSizeWeight").val();
                pData.WeightUnit = ($("#btnPackingSizeWeightUnit").text() == "Please Select" ? "" : $.trim($("#btnPackingSizeWeightUnit").text()));
                pData.IsTimeBaseUnit = $('#selUoMTimeBase').is(':checked');
                pData.TimeBaseUnit = $('[name="radTimeBase"]:checked').val();

                AjaxSend({
                    url: '/Organization/SaveBarcode',
                    data: pData,
                    success: function (e) {
                        $('#btnBarcodeSave').text('Save').removeAttr('disabled');
                        $('#popBarcode').modal('hide');

                        if (e.Results == "saved") {
                            WebToast('Successfully saved', 'top');
                            if (CStr2(e.StockMeasurementID) != '') {
                                ShowOrgProductMeasurementInfo(e.StockMeasurementID);
                                $('.statictext.unitofmeasure').removeClass('staticeditmodeinputtext').addClass('staticmodeinputtext');
                                $('.inputtext.unitofmeasure').removeClass('editmodeinputtext').addClass('staticeditmodeinputtext');
                            } else {
                                GetProductMeasurementBarcodeList(pData.StockMeasurementsID);
                            }
                            GetProductMeasurementList($("#txtOrgProductID").val());
                        } else {
                            NewMessageBox(e.Results, 'Message');
                        }
                    }
                })
            }

            function DisplayKOGroupItem(data) {
                var listKOGroupItems = $('#listKOGroupItems');

                listKOGroupItems.empty()

                if(data.length > 0){
                    for (var i = 0; i < data.length; i++){
                        listKOGroupItems.append($('<li>')
                                .append($('<a>')
                                    .text(data[i].ProductName)
                                )
                            )
                    }
                }

                listKOGroupItems;//.listview();//.listview('refresh');
            }

            function EditModeProductInfo() {
                $('#txtOrgProductName').val($('#txtOnlyOrgProductName').text().substring(14));
                $('#txtOrgProductBrand').val($('#txtOnlyOrgProductBrand').text());
                $('#txtOrgProductNo').val($('#txtOnlyOrgProductNo').text());
                $('#txtOrgProductModelNo').val($('#txtOnlyOrgProductModelNo').text());
                $('#txtOrgProductGroupNo').val($('#txtOnlyOrgProductGroupNo').text());
                $('#txtOrgProductDivision').val($('#txtOnlyOrgProductDivision').text());
                $('#txtOrgProductCategory').val($('#txtOnlyOrgProductCategory').text());
                $('#txtOrgProductMaketingText').val($('#txtOnlyOrgProductMaketingText').text());
                $('#txtOrgProductWarranty').val($('#txtOnlyOrgProductWarranty').text());
                $('#txtOrgProductPublishedBy').val($('#txtOnlyOrgProductPublishedBy').text());
                $('#txtOrgProductUploadedBy').val($('#txtOnlyOrgProductUploadedBy').text());
                if (CStr2(maxreferenceskuid) != '') {                    
                    $('[name="chbMaxOption"][value="reference"]').prop('checked', 1);
                    $('#txtMaxReferenceSKUName').val(maxreferenceskuname);
                    $('#txtOrgMaxAllowableDiscountableAmount').val('').hide();
                    $('#divMaxReferenceSKU').show();
                } else {
                    $('[name="chbMaxOption"][value="custom"]').prop('checked', 1);
                    $('#txtOrgMaxAllowableDiscountableAmount').val($('#txtOnlyMaxAllowableDiscountableAmount').text()).show();                    
                    $('#divMaxReferenceSKU').hide();
                }                
                $('[name="radRoundOptions"][value="' + $.trim($('#txtOnlySCPWDDiscountedPriceAction').text()) + '"]').prop('checked', 1)
            }

            function EditModeUnitOfMeasures() {
                $('#txtPackingSizeQty').val($('#txtOnlyPackingSizeQty').text());
                $('#txtPackingSizeVolDiscount').val($('#txtOnlyPackingSizeVolDiscount').text());
                $('#txtPackingSizeContentQty').val($('#txtOnlyPackingSizeContentQty').text());
                $('#txtPackingSizeWeight').val($('#txtOnlyPackingSizeWeight').text());
                $('#txtPackingSizeDimensionLength').val($('#txtOnlyPackingSizeDimensionLength').text());
                $('#txtPackingSizeDimensionWidth').val($('#txtOnlyPackingSizeDimensionWidth').text());
                $('#txtPackingSizeDimensionHeight').val($('#txtOnlyPackingSizeDimensionHeight').text());
                $('#txtPackingSizeWeight').val($('#txtOnlyPackingSizeWeight').text());
            }

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
                                            .text((vData.Results.Rows[p].ProductNo == undefined ? "" : vData.Results.Rows[p].ProductNo) + ' ' + vData.Results.Rows[p].ProductName)
                                        )
                                        .append($('<p>')
                                            .text('Published By:' + vData.Results.Rows[p].PublishedBy)
                                        )
                                        .append($('<p>')
                                            .text('Uploaded By:' + vData.Results.Rows[p].UploadedBy)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function (e) {
                                            $('#panSearchLibrary').modal('hide');
                                            if (isAddRawMaterial == 0) {
                                                $('#txtBOMProductID').val($(this).attr('ID'))
                                                $('#btnBOMProduct').text($(this).data('data').ProductName);//.button('refresh');
                                                getComponentMeasurement($(this).attr('ID'));
                                            }
                                            else {
                                                $('#btnBackBomRawMaterials').show();
                                                $('#panAddRawMaterials').modal('show');
                                                childProductID = $(this).attr('ID');
                                                GetBomPageContentData($(this).attr('ID'));
                                            }
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#lstOfLibrary').append($('<li>').text('No Result(s) Found'));
                        }

                        $('#lstOfLibrary');//.listview('refresh');
                        PopupReposition();
                    }
                });
            }

            function LoadUnitOfMeasurePage(v) {
                if (isPageUnitOfMeasureLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/SubPages/OrganizationProduct/UnitOfMeasure.html?v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isPageUnitOfMeasureLoaded = vData;
                            $('#pageSKUMeasurement').empty().html(vData).show();//.trigger('create');
                            if (v == "add") {
                                $('#btnOrgProductUnitOfMeasures').click();

                                // 2017_11_13__09_04_13_4061 SKU unit must be deletable and editable
                                //$('#divForAddingUnitOfMeasure').css({ visibility: 'visible', display: 'block' });
                                //$('#divForEditingUnitOfMeasure').css({ visibility: 'hidden', display: 'none' });

                                $('#divUnitOfMeasureEditBtn').hide();
                            }
                        }
                    });
                }
                else {
                    $('#pageSKUMeasurement').html(isPageUnitOfMeasureLoaded).show();//.trigger('create');
                }

                if (v == "add") {
                    $('#btnOrgProductUnitOfMeasures').click();
                    
                    // 2017_11_13__09_04_13_4061 SKU unit must be deletable and editable
                    //$('#divForAddingUnitOfMeasure').css({ visibility: 'visible', display: 'block' });
                    //$('#divForEditingUnitOfMeasure').css({ visibility: 'hidden', display: 'none' });

                    $('#divUnitOfMeasureEditBtn').hide();
                }
                else {
                    // 2017_11_13__09_04_13_4061 SKU unit must be deletable and editable
                    //$('#divForAddingUnitOfMeasure').css({ visibility: 'hidden', display: 'none' });
                    //$('#divForEditingUnitOfMeasure').css({ visibility: 'visible', display: 'block' });

                    $('#divUnitOfMeasureEditBtn').show();
                }
            }

            function LoadPriceBreakPage(v, vCallback) {
                // 
                if (mPriceBreakHtml == null) {
                    AjaxSend({
                        ExactUrl: '/SubPages/OrganizationProduct/PriceBreak.html?v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            mPriceBreakHtml = vData;
                            Show();
                        }
                    });
                } else
                    Show();

                function Show() {
                    $('#pageSKUMeasurement').empty().html(mPriceBreakHtml).show();//.trigger('create');
                    if (v == "add") {
                        //$('#divForAddingUnitOfMeasure').css({ visibility: 'visible', display: 'block' });
                        //$('#divForEditingUnitOfMeasure').css({ visibility: 'hidden', display: 'none' });
                        //$('#divPriceBreakButton').hide();
                        //$('#divUnitOfMeasureActionButton').show();
                        $('.statictext').removeClass('staticeditmodeinputtext').addClass('staticmodeinputtext');
                        $('.inputtext').removeClass('editmodeinputtext').addClass('staticeditmodeinputtext');                    
                    }
                    else {
                        $('.statictext').removeClass('staticmodeinputtext').addClass('staticeditmodeinputtext');
                        $('.inputtext').removeClass('staticeditmodeinputtext').addClass('editmodeinputtext');
                        //$('#divForAddingUnitOfMeasure').css({ visibility: 'hidden', display: 'none' });
                        //$('#divForEditingUnitOfMeasure').css({ visibility: 'visible', display: 'block' });
                        //$('#divUnitOfMeasureEditBtn').show();
                    }
                    $('#pageOrgProduct').hide();
                    $('#divContainerMenuPage').hide();
                    //$('#pageSKUMeasurement').show();
                    $('#pageSKUPrice').hide();
                    $('#pageBOMSKUInfo').hide();
                    $('#pageBrowseByOrgOwner').hide();
                    $('#pageBarcodeSearch').hide();
                    $('#pageCustomAdd').hide();
                    $('#pageProductNamePreview').hide();
                    $(':radio[name=radioPriceBreakDiscountType][value=0]').prop('checked', true);
                    $(':radio');//.checkboxradio('refresh');
                    DisabledProductFields(false);
                    ClearOrgProductMeasurementFields();
                    if ($.isFunction(vCallback))
                        vCallback();
                }
            } // END function LoadPriceBreakPage() {

            function LoadProductPricesPage(v) {
                if (isPageProductPrices == "") {
                    AjaxSend({
                        ExactUrl: '/SubPages/OrganizationProduct/ProductPrices.html?v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isPageProductPrices = vData;
                            $('#pageSKUPrice').empty().html(vData).show();//.trigger('create');
                            if (v == "add") {
                                $('#btnOrgProductEditProductPrices').click();
                                $('#divForAddingProductPrices').css({ visibility: 'visible', display: 'block' });
                                $('#divForEditingProductPrices').css({ visibility: 'hidden', display: 'none' });
                                $('#divProductPricesEditBtn').hide();
                                $('#btnProductPriceCancel').show();
                            }
                        }
                    });
                }
                else {
                    $('#pageSKUPrice').html(isPageProductPrices).show();//.trigger('create');
                }

                if (v == "add") {
                    $('#btnOrgProductEditProductPrices').click();
                    $('#divForAddingProductPrices').css({ visibility: 'visible', display: 'block' });
                    $('#divForEditingProductPrices').css({ visibility: 'hidden', display: 'none' });
                    $('#divProductPricesEditBtn').hide();
                    //$('#btnProductPriceCancel').show();
                }
                else {
                    $('#divForAddingProductPrices').css({ visibility: 'hidden', display: 'none' });
                    $('#divForEditingProductPrices').css({ visibility: 'visible', display: 'block' });
                    $('#divProductPricesEditBtn').show();
                    //$('#btnProductPriceCancel').hide();
                }
            }

            //************ START BOM FUNCTIONS **************

            function SaveBomContent() {
                Bs.Loading('show');
                AjaxSend({
                    url: 'Stock/SaveBomPageData',
                    data: {
                        ParentProductID: sessionStorage.ViewID,
                        ChildProductID: childProductID,
                        OrgID: sessionStorage.OrgID,
                        Qty: $('#txtQty').val(),
                        UnitMeasureID: $('#cbUnit').val(),
                        UnitMeasure: $('#cbUnit option:selected').text()
                    },
                    success: function (vData) {
                        Bs.Loading('hide');
                        if (vData.Error) {
                            NewMessageBox(vData.Error, 'Error');
                        } else if (vData.Result == 'ok') {
                            $('#panAddRawMaterials').modal('hide');
                            WebToast('Data was successfully saved', 'top');
                            GetProductContent(sessionStorage.ViewID);
                            GetProductPriceList(sessionStorage.ViewID);
                        } else {
                            NewMessageBox('Unknown error jsBH7RG4935Y!', 'Error');
                        }
                    },
                    error: function (vError) {
                        NewMessageBox(vError.statusText, 'Error');
                    }
                });
            }

            function SaveBomUnitDefinition() {
                AjaxSend({
                    url: 'Stock/SaveUnitDefinition',
                    data: {
                        ProductID: childProductID,
                        SourceMeasureID: childProductBaseUnitMeasureID,
                        OrgID: sessionStorage.OrgID,
                        Quantity: $('#txtQtyUD').val(),
                        DestinationMeasure: $('#cbUnitUD').val()
                    },
                    success: function (vData) {
                        if (vData.Error) {
                            NewMessageBox(vData.Error, 'Error', [
                                {
                                    Text: 'Ok', Click: function () {
                                        $('#panAddRawMaterials').modal('show');
                                    }
                                }
                            ]);
                        } else if (vData.Result == 'ok') {
                            NewMessageBox('Data was successfully saved', 'Unit Definition', [
                                {
                                    Text: 'Ok', Click: function () {
                                        $('#divBomUnitDefinition').css({ visibility: 'hidden', display: 'none' });
                                        $('#divBomContent').css({ visibility: 'visible', display: 'block' });
                                        $('#BOMTitle').text('Content');
                                        $('#cbUnit').val($('#cbUnit option:first').val());//.selectmenu('refresh');
                                        $('#panAddRawMaterials').modal('show');
                                    }
                                }
                            ]);
                            GetBomPageContentData(childProductID);
                        } else {
                            NewMessageBox('Unknown error jsBH7RG4935Y!', 'Error', [
                                {
                                    Text: 'Ok', Click: function () {
                                        $('#panAddRawMaterials').modal('show');
                                    }
                                }
                            ]);
                        }
                    },
                    error: function (vError) {
                        NewMessageBox(vError.statusText, 'Error');
                    }
                });
            }

            function GetUnitDefinitionData() {
                AjaxSend({
                    url: 'Stock/GetUnitDefinition',
                    data: {
                        ProductID: childProductID,
                        OrgID: sessionStorage.OrgID
                    },
                    success: function (vData) {
                        if (vData.Error != undefined) {
                            vParams.error(vData.Error);
                            return;
                        }

                        $('#spanParentUnitUD').text(vData.parent.BaseUnit);
                        $('#spanParentProductUD').text(vData.parent.ProductName);
                        if (vData.content == undefined) {
                            qty = null;
                            destinationMeasure = null;
                        } else {
                            qty = vData.content.Quantity;
                            destinationMeasure = vData.content.DestinationMeasure;
                        }
                        $('#txtQtyUD').val($.format.number(qty, proj.QuantityFormat));
                        LoadComboUnitDefinition(vData.families);
                    },
                    error: function (vData) {
                        vParams.error(vData.statusText);
                    }
                });
            }

            function LoadComboUnitDefinition(vData) {
                var pCbItem;
                var pDR;
                var pRows = GetStringObject('Rows', vData);
                var pSelectedUnitMeasureID;
                var pFamily;

                $('#cbUnitUD').html('');
                if (!$.isArray(pRows))
                    RaiseError('Rows is required and must be of type array jsL73KD8ZHFL!');

                for (var pIndex = 0; pIndex <= pRows.length - 1; pIndex++) {
                    pDR = pRows[pIndex];
                    if (pFamily != pDR.MeasurementFamily) {
                        pFamily = pDR.MeasurementFamily;
                        pCbItem = $('<option>')
                            .attr('disabled', 'disabled')
                            .css('font-weight', 'bold') // doesnt work
                            .text('[' + pFamily + ']');
                        $('#cbUnitUD').append(pCbItem);
                    }

                    pCbItem = $('<option>')
                        .val(pDR.DestMeasure)
                        .text(pDR.DestMeasure);
                    if (destinationMeasure)
                        if (pDR.DestMeasure == destinationMeasure)
                            pCbItem.attr('selected', true);
                    $('#cbUnitUD').append(pCbItem);
                }

                $('#cbUnitUD');//.selectmenu();
                $('#cbUnitUD');//.selectmenu('refresh', true);
            }

            function UpdateHeaderContent() {
                $('#spanQty').text($.format.number($('#txtQty').val()));
                $('#spanChildUnit').text($('#cbUnit option:selected').text());
            }

            function GetBomPageContentData(childProductID) {
                Bs.Loading('show');
                AjaxSend({
                    url: 'Stock/GetBomPageData',
                    data: {
                        ParentProductID: sessionStorage.ViewID,
                        ChildProductID: childProductID,
                        OrgID: sessionStorage.OrgID
                    },
                    success: function (vData) {
                        Bs.Loading('hide');
                        if (vData.Error != undefined) {
                            vParams.error(vData.Error);
                            return;
                        }
                        $('#spanParentUnit').text(vData.parent.BaseUnit);
                        $('#spanParentProduct').text(vData.parent.ProductName);
                        $('#spanChildProduct').text(vData.child.ProductName);

                        childProductBaseUnitMeasureID = vData.child.BaseUnitMeasureID;

                        if (vData.contents == undefined) {
                            $('#txtQty').val('');
                            unitmeasureid = null;
                            unitmeasure = null;
                        }
                        else {
                            $('#txtQty').val($.format.number(vData.contents.Quantity, proj.QuantityFormat));
                            unitmeasureid = vData.contents.UnitMeasureID;
                            unitmeasure = vData.contents.UnitMeasure;
                        }
                        LoadComboContent(vData.units);
                        UpdateHeaderContent();
                        PopupReposition();
                    },
                    error: function (vData) {
                        vParams.error(vData.statusText);
                    }
                });
            }

            function LoadComboContent(vData) {
                var pCbItem;
                var pDR;
                var pRows = GetStringObject('Rows', vData);
                var pSelectedUnitMeasureID;
                var pSelectedUnitMeasure;
                var pItem1; // 1st unit measure

                $('#cbUnit').html('');
                if (!$.isArray(pRows))
                    RaiseError('Rows is required and must be of type array js5PQRVKB5VA!');

                if (unitmeasureid != undefined)
                    pSelectedUnitMeasureID = ReadyGuid(unitmeasureid);

                if (unitmeasure != undefined)
                    pSelectedUnitMeasure = unitmeasure;

                for (var pIndex = 0; pIndex <= pRows.length - 1; pIndex++) {
                    pDR = pRows[pIndex];
                    if (pItem1 == undefined)
                        pItem1 = pDR;
                    pDR.ID = ReadyGuid(pDR.ID);
                    pCbItem = new Option(pDR.UnitMeasure, ReadyGuid(pDR.ID));

                    if ((pSelectedUnitMeasureID != undefined)
                        && (pSelectedUnitMeasureID == pDR.ID)) {

                        pCbItem.selected = true;
                    } else if ((pSelectedUnitMeasure != undefined)
                        && (pSelectedUnitMeasure == pDR.UnitMeasure)) {

                        pCbItem.selected = true;
                    }

                    $(pCbItem).data('data', pDR);
                    $('#cbUnit').append(pCbItem);
                }
                if (pItem1 != undefined) {
                    pCbItem = $('<option>')
                        .attr('disabled', 'disabled')
                        .css('border-bottom', '1px dotted #00') // doesnt work
                        .text('──────');
                    $('#cbUnit').append(pCbItem);

                    pCbItem = $('<option>')
                        .val('define')
                        .css('border-bottom', '1px dotted #00') // doesnt work
                        .text('Define ' + pItem1.UnitMeasure);
                    $('#cbUnit').append(pCbItem);

                }

                $('#cbUnit');//.selectmenu();
                $('#cbUnit');//.selectmenu('refresh', true);
            }

            //************** END BOM FUNCTIONS **********************

            function SearchPackingSizeLengthUnitPopUp(s) {
                var pData = {};

                pData.keywords = s;

                AjaxSend({
                    url: 'Browser/LibraryPIMMeasureLength',
                    data: pData,
                    success: function (vData) {
                        $('#lstOfLibrary').empty();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstOfLibrary').append($('<li>')
                                    .append($('<a>')
                                        .attr('ID', vData.Results.Rows[p].ID)
                                        .append($('<br><p style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].Measurement)
                                        )
                                        .append($('<p>')
                                            .text('Sign: ' + vData.Results.Rows[p].Sign)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function (e) {
                                            if (NameOfBtnForLibrary == "PackingSizeDimLengthUnit") {
                                                $('#btnPackingSizeDimLengthUnit').text($(this).data('data').Measurement);
                                            }
                                            else if (NameOfBtnForLibrary == "PackingSizeDimWidthUnit") {
                                                $('#btnPackingSizeDimWidthUnit').text($(this).data('data').Measurement);
                                            }
                                            else {
                                                $('#btnPackingSizeDimensionHeightUnit').text($(this).data('data').Measurement);
                                            }

                                            $('#panSearchLibrary').modal('hide');
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#lstOfLibrary').append($('<li>').text('No Result(s) Found'));
                        }

                        $('#lstOfLibrary');//.listview('refresh');
                    }
                });
            }

            function SearchPackingSizeWeightUnitPopUp(s) {
                var pData = {};

                pData.keywords = s;

                AjaxSend({
                    url: 'Browser/LibraryPIMMeasureWeight',
                    data: pData,
                    success: function (vData) {
                        $('#lstOfLibrary').empty();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstOfLibrary').append($('<li>')
                                    .append($('<a>')
                                        .attr('ID', vData.Results.Rows[p].ID)
                                        .append($('<br><p style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].Measurement)
                                        )
                                        .append($('<p>')
                                            .text('Sign: ' + vData.Results.Rows[p].Sign)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function (e) {
                                            $('#btnPackingSizeWeightUnit').text($(this).data('data').Measurement);

                                            $('#panSearchLibrary').modal('hide');
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#lstOfLibrary').append($('<li>').text('No Result(s) Found'));
                        }

                        $('#lstOfLibrary');//.listview('refresh');
                    }
                });
            }

            function SearchPackingSizeUnitPopUp(s) {
                var pData = {};

                pData.keywords = s;

                AjaxSend({
                    url: 'Browser/LibraryPIMMeasure',
                    data: pData,
                    success: function (vData) {
                        $('#lstOfLibrary').empty();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstOfLibrary').append($('<li>')
                                    .append($('<a>')
                                        .attr('ID', vData.Results.Rows[p].ID)
                                        .append($('<br><p style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].Measurement)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function (e) {
                                            if (NameOfBtnForLibrary == "PackingSizeUnit") {
                                                $('#txtRecProductMeasurementPackingSizeUnitID').val($(this).attr('ID'));
                                                $('#btnPackingSizeUnit').text($(this).data('data').Measurement);
                                            }
                                            else if (NameOfBtnForLibrary == "PackingSizeContentUnit") {
                                                $('#btnPackingSizeContentUnit').text($(this).data('data').Measurement);
                                            }

                                            $('#panSearchLibrary').modal('hide');
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#lstOfLibrary').append($('<li>').text('No Result(s) Found'));
                        }

                        $('#lstOfLibrary');//.listview('refresh');
                    }
                });
            }


            function SearchPriceLvlPopUp(s) {
                var pData = {};

                pData.keywords = s;

                AjaxSend({
                    url: 'Browser/LibraryPIMPriceLevel',
                    data: pData,
                    success: function (vData) {
                        $('#lstOfLibrary').empty();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstOfLibrary').append($('<li>')
                                    .append($('<a>')
                                        .attr('ID', vData.Results.Rows[p].ID)
                                        .append($('<br><p style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].PriceLevel)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function (e) {
                                            $('#txtRecProductPriceLevelID').val($(this).attr('ID'));
                                            $('#btnProductPriceLevel').text($(this).data('data').PriceLevel);
                                            $('#panSearchLibrary').modal('hide');
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#lstOfLibrary').append($('<li>').text('No Result(s) Found'));
                        }

                        $('#lstOfLibrary');//.listview('refresh');
                    }
                });
            }

            //
            function AddOrgOwnerProductListCustom(pData) {
                AjaxSend({
                    url: 'Organization/AddOrgOwnerProductListCustom',
                    data: pData,
                    success: function (vData) {
                        if (CDec2(vData.OrgData) > 0) {
                            NewMessageBox('Successfully added ' + CDec2(vData.OrgData) + ' new product/s to your company-wide product list. To change the prices of the product/s, please click Continue to download your product list.', 'Add to Company-Wide Product List',
                                [
                                    {
                                        Text: 'Continue',
                                        Click: function () {
                                            AjaxSend({
                                                url: 'Login3/TestCookies',
                                                success: function (vData) {
                                                    window.pageSecuritySourcePage = $.mobile.activePage;
                                                    PromptUserAccess('Product Price', {
                                                        OnClickOKSuccess: function () {
                                                            //window.open(GetPathFormat("/PIMProductPrice/UploadDownloadExcelFile") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                                                            window.location.hash = "";
                                                            window.location.hash = "UniversalPage";
                                                            sessionStorage["AddViewLibBCName"] = 'Bulk Price Adjustment';
                                                            LibraryTable.BulkPriceAdjustmentPage();
                                                        }
                                                    });
                                                },
                                                error: function (vError) {
                                                    NewMessageBox('This feature requires internet connection.', 'Requires Internet Connection');
                                                }

                                            });
                                        }
                                    },
                                    {
                                        Text: 'No',
                                        Click: function () {
                                            $('#tblBody').empty();
                                            window.location.hash = 'LibraryPage';
                                            LibraryTable.PIMPage();
                                        }
                                    }
                                ])
                            //  MessageBox('Successfully added ' + CDec2(vData.OrgData) + ' new product/s to your company-wide product list.');
                        } else {
                            NewMessageBox('No new product/s found.', 'Product');
                        }
                    }
                });

            }



            function ClearCustomAddPage() {
                $('#tblResult').empty();
                $('#divResult').hide();
                $("#divNoResult").hide();
                $('#selFilter').val('');//.selectmenu('refresh');
            }

            function DisplayResults(vRows) {

                //                $("#divResult").show();
                var pLV = $("#tblResult");

                $('#tblResult').empty();


                for (var i = 0; i <= vRows.length - 1; i++) {

                    var checked = undefined;

                    if ((i % 2) == 0) {
                        pItem = $('<tr>')
                    }

                    checked = false;
                    pItem = pItem
                    .append($('<td>')
                        .append($('<div class="form-check mb-3" style="font-size:20px;">')
                            .append($('<input type="checkbox" name="filter" class="form-check-input"/>')
                                .attr('ID', i + 'chb')
                                .attr('checked', checked)
                                .attr('value', vRows[i].Label)

                            )
                            .append($('<label class="form-check-label">')
                                .attr('for', i + 'chb')
                                .text(vRows[i].Label)
                            )
                        )                        
                    )//td                    


                    pLV.append(pItem);

                    try {
                        $('#' + i + 'chb');//.checkboxradio('refresh');
                    } catch (ex) {
                        $('#' + i + 'chb');//.checkboxradio();
                    }


                } //for

                try {
                    $('#lstResultContainer');//.listview('refresh');
                }
                catch (e) {
                    $('#lstResultContainer');//.listview();
                }
            }
            function DisplayResultsProductName(vRows) {

                //                $("#divResult").show();
                var pLV = $("#tblResultName");

                $('#tblResultName').empty();


                for (var i = 0; i <= vRows.length - 1; i++) {

                    if ((i % 2) == 0) {
                        pItem = $('<tr>')
                    }

                    pItem = pItem
                    .append($('<td>')
                        .append($('<label style="font-weight: normal;">')
                            .text(vRows[i].ProductNo + ', ' + vRows[i].Brand + ', ' + vRows[i].ProductName)
                        )
                    )//td                    

                    pLV.append(pItem);

                } //for

                try {
                    $('#lstResultContainerName');//.listview('refresh');
                }
                catch (e) {
                    $('#lstResultContainerName');//.listview();
                }
            }

            function SearchPublicProduct(s) {
                BrowsePopup('LibraryPIMProductPublic', 'Search Product', [
                    {
                        Search: s || '',
                        DisplayText: [{
                            Column: function (e) {
                                return e.ProductNo + ' ' + e.ProductName
                            }
                        }, { Label: 'Published by', Column: 'PublishedBy' }, { Label: 'Uploaded by', Column: 'UploadedBy' }],
                        Click: function (e) {
                            $('#pageOrgProduct').hide();
                            $('#pageOrgProductInfo').show();
                            $('#pageSKUMeasurement').hide();
                            $('#pageSKUPrice').hide();
                            $('#pageBOMSKUInfo').hide();
                            $('#pageBrowseByOrgOwner').hide();
                            $('#pageBarcodeSearch').hide();
                            $('#pageCustomAdd').hide();
                            $('#pageProductNamePreview').hide();
                            ClearOrgProductFields();
                            ClearOrgProductInActiveFields();
                            ShowHideOrgProductInActiveDetails();

                            $('#txtOrgProductID').val(e.ID);
                            $('#spanProductShortName').text(e.ProductName);

                            ShowOrgProductInfo(e.ID);
                        }
                    }
                ])
                return
                window.IsBrowserloaded = false;
                var pData = {};

                pData.keywords = s || '';

                AjaxSend({
                    url: 'Browser/LibraryPIMProductPublic',
                    data: pData,
                    success: function (vData) {
                        $('#lstOfLibrary').empty();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstOfLibrary').append($('<li>')
                                    .append($('<a>')
                                        .attr('ID', vData.Results.Rows[p].ID)
                                        .attr('title', vData.Results.Rows[p].ProductName)
                                        .append($('<br><p style="font-weight:bold;">')
                                            .text((vData.Results.Rows[p].ProductNo == undefined ? "" : vData.Results.Rows[p].ProductNo) + ' ' + vData.Results.Rows[p].ProductName)
                                        )
                                        .append($('<p>')
                                            .text('Published By:' + vData.Results.Rows[p].PublishedBy)
                                        )
                                        .append($('<p>')
                                            .text('Uploaded By:' + vData.Results.Rows[p].UploadedBy)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function (e) {
                                            $('#txtPublicSearch').val('');

                                            //$.mobile.changePage('#pageOrgProductInfo');
                                            

                                            $('#panSearchLibrary').modal('hide');
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#lstOfLibrary').append($('<li>').text('No Result(s) Found'));
                        }

                        $('#lstOfLibrary');//.listview('refresh');
                    }
                });

            }

            function ShowHideOrgProductInActiveDetails() {

                if ($("#selOrgProductDeactivate").is(':checked')) {
                    $(".divOrgProductDeactivateDetails").show();
                    $('.divProductAvailableToMobile').hide();
                    var d = new Date();
                    $('#selOrgProductDateInActiveMonth').val($.format.date(d, 'M'));
                    $('#selOrgProductDateInActiveYear').val($.format.date(d, 'yyyy'));
                    setDayCollection('selOrgProductDateInActiveMonth', 'selOrgProductDateInActiveDay', 'selOrgProductDateInActiveYear', false);
                    $('#selOrgProductDateInActiveDay').val($.format.date(d, 'd'));
                }
                else {
                    $(".divOrgProductDeactivateDetails").hide();
                    $('.divProductAvailableToMobile').show();
                }
            }

            function getOrgProductList() {
                var pData = {};

                if (!IsCancelled) {
                    pData.SearchKeyword = $("#txtOrgProductSearch").val();

                    AjaxSend({
                        url: 'Organization/getOrganizationProductList',
                        data: pData,
                        success: function (vData) {
                            LoadOrgProductList(vData.OrgProductData.Rows);
                        }
                    });
                }

                IsCancelled = false;
            }

            function LoadOrgProductList(vRows) {
                var pLV = $('#lstOrgProduct');
                var pItem;

                AjaxSend({
                    url: 'Organization/getOrgCompanyWideProductCount',
                    success: function (vData) {

                        $("#lblProductRecordcount").text(" There are " + vRows.length + " results that match your keyword search. " + vData.OrgProductData + " products are listed for your company.");

                        pLV.html('');
                        for (var p = 0; p <= vRows.length - 1; p++) {

                            var pDivOfficial;
                            if (CDec2(vRows[p].IsBrandOwner) != 0) {
                                pDivOfficial = $('<div>')
                                    .click(function () {
                                        NewMessageBox('SKUs with the "Official Item Record" icon are SKU records sourced and maintained by the official brand owner (or authorized agent). You get automatic updates to changes made by the brand owner such as new and/or modified item descriptions, pictures, SRP, packing sizes, dimensions, bar code numbers, and more. We strongly recommend that you use these official records in all of your transactions.', 'Official Item Record');
                                        return false;
                                    })
                                    .addClass('ui-block-b')
                                    .attr('style', 'width: 80px')
                                    .append($('<img>')
                                        .css({
                                            'max-width': '80px',
                                            'max-height': '80px',
                                            'margin-right': '9px'
                                        })
                                        .attr('src', '../Content/images/OfficialItem.gif')
                                    );
                            };



                            pItem = $('<li>')
                                .append($('<div style="cursor:pointer;">')
                                    .addClass('ui-grid-b')
                                    .attr('href', "#")
                                    .append($('<div>')
                                        .addClass('ui-block-a')
                                        .attr('style', 'width: 80px')
                                        .append($('<img>')
                                            .css({
                                                'max-width': '80px',
                                                'max-height': '80px',
                                                'margin-right': '9px'
                                            })
                                            .attr('src', '../Content/images/box.png')
                                        )
                                    )
                                    .append(pDivOfficial)
                                    .append($('<div style="cursor:pointer;">')
                                        .addClass('ui-block-c')
                                        .attr('style', 'width: 70%')
                                        .append($('<a>')
                                            .attr('ID', vRows[p].ID)
                                            .data('ProductID', vRows[p].ProductID)
                                            .click(function (e) {
                                                e.preventDefault();
                                                //$.mobile.changePage('#pageOrgProductInfo');
                                                $('#pageOrgProduct').hide();
                                                $('#pageOrgProductInfo').show();
                                                $('#pageSKUMeasurement').hide();
                                                $('#pageSKUPrice').hide();
                                                $('#pageBOMSKUInfo').hide();
                                                $('#pageBrowseByOrgOwner').hide();
                                                $('#pageBarcodeSearch').hide();
                                                $('#pageCustomAdd').hide();
                                                $('#pageProductNamePreview').hide();
                                                ShowOrgProductInfo($(this).data('ProductID'));
                                            })
                                            .append($('<h3>')
                                                .attr("style", "white-space:normal")
                                                .text(vRows[p].ProductName)
                                            )
                                            .append($('<p>')
                                                .attr("style", "white-space:normal")
                                                .text("Activation Date: " + $.format.date(new Date(vRows[p].ActivationDate), 'MMM dd, yyyy'))
                                            )
                                            .append($('<p>')
                                                .attr("style", "white-space:normal")
                                                .text("Do not share transaction data with the brand owner? " + (vRows[p].IsDoNotShareTransDataToBrandOwner != '0' ? 'Yes' : 'No'))
                                            )
                                            .append($('<p>')
                                                .attr("style", "white-space:normal")
                                                .attr("style", "color:Red")
                                                .text((vRows[p].IsPriceCount < 2 ? 'Please click to add price for use in POS and Order Taking.' : ''))
                                            )
                                        )
                                    )
                                );

                            pLV.append(pItem);
                        };

                        pLV;//.listview('refresh');

                    }
                });

            };

            function ShowOrgProductInfo(strID) {
                var pData = {};
                pData.ID = strID;

                mBom.model.productID = strID;
                mBom.getData({
                    success: function () { },
                    error: function () { }
                });
                $("#divProductInfoContainer").show();
                $("#divProductBOMContainer").hide();

                ClearOrgProductFields();
                ClearOrgProductInActiveFields();

                AjaxSend({
                    url: "SecurityRole/CheckSecurityCurrentUserAcccess",
                    data: { ModuleName: 'Grant access to modify PIM details' },
                    success: function (vData) {
                        if (vData.SecurityRoleData.Rows.length == 0) {
                            isGrantAccessToModifyPIM = false;
                            $('#btnOrgProductEdit, #btnOrgProductInfoSave, #btnOrgProductInfoSaveAs, ' +
                                '#btnOrgProductMeasurementAdd, #btnOrgPriceBreakAdd, #btnEditUpcharge, ' +
                                '#btnOrgProductPriceLevelAdd, #btnAddVendorPrice, #btnOrgProductBOMAdd, ' +
                                '#divActionButtonForJit, #btnOrgProductBOMInstructionEdit, #divActionButtonForComponentsDuringSalesReturns,' +
                                '#btnChoosePicture').remove();
                        }

                        $('#pageOrgProductInfoNew').pageshow();

                        AjaxSend({
                            url: "Organization/getOrganizationProductInfo",
                            data: pData,
                            success: function (vData) {
                                if (vData.OrgProductData.Rows.length > 0) {

                                    ///Hide if editing record
                                    $("#txtOrgPIMProductID").val(vData.OrgProductData.Rows[0].ID || '');

                                    $('#btnProductBrowser').hide();
                                    $('#btnOrgProduct').attr("disabled", true);
                                    $("#txtOrgProductID").val(vData.OrgProductData.Rows[0].ProductID || '');
                                    $("#btnOrgProduct").text(vData.OrgProductData.Rows[0].ProductName || 'Please Select');//.button('refresh');
                                    ////////////
                                    if ((IsGuid(vData.OrgProductData.Rows[0].UploadedByID))
                                        && ((vData.OrgProductData.Rows[0].UploadedByID || '') != sessionStorage["OrgID"]))

                                        DisabledProductFields(true);
                                    else
                                        DisabledProductFields(false);

                                    $('#txtOnlyOrgProductSellInEcommerce').text((CBool(vData.OrgProductData.Rows[0].IsEcommerceVendorAllowedToSell) ? 'Yes' : 'No'));
                                    $('#selProdSellInEcommerce').prop('checked', CInt2(CBool(vData.OrgProductData.Rows[0].IsEcommerceVendorAllowedToSell)));//.slider('refresh');
                                    $('#txtOnlyOrgProductIsAsset').text((CBool(vData.OrgProductData.Rows[0].IsAsset) ? 'Yes' : 'No'));
                                    $('#txtOnlyOrgProductIsPromoItem').text((CBool(vData.OrgProductData.Rows[0].IsPromoItem) ? 'Yes' : 'No'));
                                    $('#txtOnlyOrgProductIsInventoryCountItem').text((CBool(vData.OrgProductData.Rows[0].IsInventoryCountItem || true) ? 'Yes' : 'No'));
                                    $('#selProdIsAsset').prop('checked', CInt2(CBool(vData.OrgProductData.Rows[0].IsAsset)));//.slider('refresh');
                                    $('#selProdIsPromoItem').prop('checked', CInt2(CBool(vData.OrgProductData.Rows[0].IsPromoItem)));//.slider('refresh');
                                    $('#selProdIsInventoryCountItem').prop('checked', CInt2(CBool(vData.OrgProductData.Rows[0].IsInventoryCountItem || 1)));
                                    $("#txtOrgProductBrand").val(vData.OrgProductData.Rows[0].Brand || '');
                                    $('#txtOnlyOrgProductBrand').text(vData.OrgProductData.Rows[0].Brand || '');

                                    $("#txtOrgProductNo").val(vData.OrgProductData.Rows[0].ProductNo || '');
                                    $('#txtOnlyOrgProductNo').text(vData.OrgProductData.Rows[0].ProductNo || '');

                                    $("#txtOrgProductName").val(vData.OrgProductData.Rows[0].ProductName || '');
                                    mProductName = vData.OrgProductData.Rows[0].ProductName;
                                    $("#txtOnlyOrgProductName").text('Product Name: ' + mProductName || '');
                                    productname = vData.OrgProductData.Rows[0].ProductName || '';

                                    $("#txtOnlyOrgProductModelNo").text(vData.OrgProductData.Rows[0].ModelNo || '');
                                    $("#txtOrgProductModelNo").val(vData.OrgProductData.Rows[0].ModelNo || '');

                                    $("#txtOnlyOrgProductGroupNo").text(vData.OrgProductData.Rows[0].ProductGroupNo || '');
                                    $("#txtOrgProductGroupNo").val(vData.OrgProductData.Rows[0].ProductGroupNo || '');

                                    $("#txtOrgProductDivision").text(vData.OrgProductData.Rows[0].DivisionDescription || '');
                                    $("#txtOrgProductDivision").val(vData.OrgProductData.Rows[0].DivisionDescription || '');

                                    $("#txtOnlyOrgProductCategory").text(vData.OrgProductData.Rows[0].Category || '');
                                    $("#txtOrgProductCategory").val(vData.OrgProductData.Rows[0].Category || '');

                                    if (CStr2(vData.OrgProductData.Rows[0].MaxAllowableDiscountReferenceSKUID) != '') {
                                        maxreferenceskuid = CStr2(vData.OrgProductData.Rows[0].MaxAllowableDiscountReferenceSKUID);
                                        maxreferenceskuname = CStr2(vData.OrgProductData.Rows[0].MaxAllowableDiscountReferenceSKUName);
                                        $("#txtOnlyMaxAllowableDiscountableAmount").html('SKU based <i>(' + maxreferenceskuname + ')</i>')
                                    } else {
                                        maxreferenceskuid = '';
                                        maxreferenceskuname = '';
                                        $("#txtOnlyMaxAllowableDiscountableAmount").text(AccountingFormat(vData.OrgProductData.Rows[0].MaxAllowableDiscountableAmount));
                                    }
                                    
                                    $("#txtOnlySCPWDDiscountedPriceAction").text((CStr2(vData.OrgProductData.Rows[0].DiscountedPriceAction) == '' ? 'Exact Price' : $.trim(vData.OrgProductData.Rows[0].DiscountedPriceAction)));

                                    var pStr = vData.OrgProductData.Rows[0].MarketingText || '';
                                    pStr = HtmlEncode(pStr);
                                    pStr = pStr.replace(/\n/g, '<br />');
                                    $("#txtOnlyOrgProductMaketingText").html(pStr);
                                    $("#txtOrgProductMaketingText").html(pStr);

                                    $("#txtOnlyOrgProductWarranty").text(vData.OrgProductData.Rows[0].Warranty || '');
                                    $("#txtOrgProductWarranty").val(vData.OrgProductData.Rows[0].Warranty || '');

                                    $("#txtOnlyOrgProductLineItemPaymentTerm").text(vData.OrgProductData.Rows[0].PaymentTerm || '');
                                    $("#txtOrgProductLineItemPaymentTerm").text(vData.OrgProductData.Rows[0].PaymentTerm || '');
                                    mPaymentTermID = vData.OrgProductData.Rows[0].PaymentTermID || ''

                                    $("#txtOrgProductPublishedBy").data('ID', vData.OrgProductData.Rows[0].PublishedByID || '');
                                    $("#txtOnlyOrgProductPublishedBy").text(vData.OrgProductData.Rows[0].PublishedBy || '');
                                    $("#txtOrgProductPublishedBy").val(vData.OrgProductData.Rows[0].PublishedBy || '');

                                    $("#txtOrgProductUploadedBy").data('ID', vData.OrgProductData.Rows[0].UploadedByID || '');
                                    $("#txtOnlyOrgProductUploadedBy").text(vData.OrgProductData.Rows[0].UploadedBy || '');
                                    $("#txtOrgProductUploadedBy").val(vData.OrgProductData.Rows[0].UploadedBy || '');

                                    $('#spanProductShortName').text(vData.OrgProductData.Rows[0].ProductName || '')

                                    $("#selOrgProductDoNotShareData").prop('checked', CInt2(vData.OrgProductData.Rows[0].IsDoNotShareTransDataToBrandOwner || '0'));//.slider("refresh");
                                    $("#txtOrgProductDoNotShare").text(vData.OrgProductData.Rows[0].IsDoNotShareTransDataToBrandOwner == 0 ? "No" : "Yes")

                                    $("#selOrgProductActivationDateMonth").val(vData.OrgProductData.Rows[0].ActivationDateMonth || '');//.selectmenu('refresh');
                                    $("#selOrgProductActivationDateYear").val(vData.OrgProductData.Rows[0].ActivationDateYear || '');//.selectmenu('refresh');
                                    setDayCollection('selOrgProductActivationDateMonth', 'selOrgProductActivationDateDay', 'selOrgProductActivationDateYear', false);
                                    $("#selOrgProductActivationDateDay").val(vData.OrgProductData.Rows[0].ActivationDateDay || '');//.selectmenu('refresh');

                                    $('#txtOrgProductActivationDate').text(vData.OrgProductData.Rows[0].ActivationDateMonth + '.' + vData.OrgProductData.Rows[0].ActivationDateDay + '.' + vData.OrgProductData.Rows[0].ActivationDateYear);

                                    $("#selOrgVatExempt").prop('checked', CInt2(vData.OrgProductData.Rows[0].IsVatExempt || '0'));//.slider("refresh");
                                    $("#txtOrgVatExempt").text(vData.OrgProductData.Rows[0].IsVatExempt == 0 ? "No" : "Yes")

                                    $("#selOrgProductDeactivate").prop('checked', CInt2(vData.OrgProductData.Rows[0].DeActivated || '0'));//.slider("refresh");
                                    $("#txtOrgProductDeactivateRecord").text(vData.OrgProductData.Rows[0].DeActivated == 0 ? "No" : "Yes")

                                    $("#selOrgProductAvailableToMobile").prop('checked', CInt2(vData.OrgProductData.Rows[0].ProductAvailableToMobile || '0'));//.slider("refresh");
                                    $("#txtOrgProductAvailableToMobile").text(vData.OrgProductData.Rows[0].ProductAvailableToMobile == 0 ? "No" : "Yes")

                                    if ($("#selOrgProductDeactivate").is(':checked')) {
                                        $('.divProductAvailableToMobile').hide();
                                        $('#divOrgProductDeactivateDetails').show();
                                        $('#txtOrgProductDeactivateDate').text(vData.OrgProductData.Rows[0].DeactivationDateMonth + '.' + vData.OrgProductData.Rows[0].DeactivationDateDay + '.' + vData.OrgProductData.Rows[0].DeactivationDateYear);
                                        $("#selOrgProductDateInActiveMonth").val(vData.OrgProductData.Rows[0].DeactivationDateMonth || '');//.selectmenu('refresh');
                                        $("#selOrgProductDateInActiveYear").val(vData.OrgProductData.Rows[0].DeactivationDateYear || '');//.selectmenu('refresh');
                                        setDayCollection('selOrgProductDateInActiveMonth', 'selOrgProductDateInActiveDay', 'selOrgProductDateInActiveYear', false);
                                        $("#selOrgProductDateInActiveDay").val(vData.OrgProductData.Rows[0].DeactivationDateDay || '');//.selectmenu('refresh');

                                        $('#txtOnlyOrgProductInActiveReason').text(vData.OrgProductData.Rows[0].DeactivationReason);
                                        $("#txtOrgProductInActiveReason").val(vData.OrgProductData.Rows[0].DeactivationReason || '').keyup();

                                        $("#txtOrgProductComments").val(vData.OrgProductData.Rows[0].Comments || '').keyup();
                                        $('#txtOnlyOrgProductComments').text(vData.OrgProductData.Rows[0].Comments);

                                        $("#selOrgProductBan").prop('checked', CInt2(vData.OrgProductData.Rows[0].IsBan || '0'));//.slider("refresh");
                                        $('#txtOrgProductBan').text(vData.OrgProductData.Rows[0].IsBan == "1" ? "Yes" : "No");
                                    }

                                    GetParentProductInformation(strID);
                                    ShowHideOrgProductInActiveDetails();

                                    GetProductMeasurementList(strID);
                                    GetProductPriceBreakList(strID);
                                    GetPIMPictures(strID);
                                    GetBarcodeType();
                                }
                            }
                        });
                    }
                });
            };

            function GetBarcodeType() {
                AjaxSend({
                    url: 'Organization/GetBarcodeType',
                    success: function (e) {
                        var selBarcodeType = $('#selBarcodeType');
                        var arrBarcode = $.merge([
                            { ID: 'f848c611-a271-4aea-94a3-1a5164333dd4', BarcodeType: 'UPC' },
                            { ID: '25332b69-ab93-4742-b924-f5c05a7b26b7', BarcodeType: 'EAN' },
                            { ID: 'dad61485-803e-4f5f-ad6e-7a91952a94f0', BarcodeType: 'ISBN' }
                        ], e.Results.Rows);

                        selBarcodeType.empty();

                        for (var i = 0; i < arrBarcode.length; i++) {
                            selBarcodeType.append(new Option(arrBarcode[i].BarcodeType, arrBarcode[i].ID))
                        }

                        selBarcodeType;//.selectmenu('refresh');
                    }
                })                
            }

            var carousel = null;
            function GetPIMPictures(id) {
                AjaxSend({
                    url: 'Organization/GetPIMPictures',
                    data: { ProductID: id },
                    success: function (e) {
                        var vRows = e.Results.Rows;                        
                        var imgSelection = $("#imgSelection");
                        var divPIMPicPreview = $('#divPIMPicPreview');
                        var divPIMPicPreviewThumbnails = $('#divPIMPicPreviewThumbnails');
                        var table = $('<table>');

                        imgSelection.empty();
                        divPIMPicPreview.empty();
                        divPIMPicPreviewThumbnails.empty();

                        if(vRows.length > 0){
                            for (var i = 0; i < vRows.length; i++) {
                                vRows[i].Index = i;
                                var td = $('<td style="width:100px; padding:5px; text-align:center;">');
                                var srcMed = '/Content/MobizImagePIM/' + sessionStorage.OrgID + '/Medium/' + vRows[i].FileName;
                                var srcBig = '/Content/MobizImagePIM/' + sessionStorage.OrgID + '/Big/' + vRows[i].GroupImageID + '_big.jpg';
                                var img = $('<img>');

                                if (CBool(vRows[i].IsDefault)) {
                                    $('#finalImage').attr('src', srcBig);
                                    img.addClass('selected');
                                }

                                img.data('data', vRows[i]).attr('src', srcMed)
                                    //.click(function () {
                                    //    var data = $(this).data('data');
                                    //    carousel.goToSlide(data.Index);
                                    //    $('#panUploadProductImagePopup').modal('show');
                                    //})
                                
                                td.append($('<a data-role="none" data-lightbox="pimpictures">')
                                        .attr('href', srcBig)
                                        .append(img)
                                    ).append('<br><br>')
                                    .append($('<div class="divPIMPicOption" style="display:none;">')
                                        .append($('<button data-role="none" class="btn btn-info" title="Choose this as default picture">')
                                            .append($('<span class="bi-image-alt">'))
                                            .data('data', vRows[i])
                                            .click(function (e) {
                                                var data = $(this).data('data');
                                                data.OrgID = sessionStorage.OrgID;
                                                data.ViewID = sessionStorage.ViewID;
                                                SaveImagePIM(JSON.stringify(data), function () {
                                                    $('.tooltip').remove();
                                                    WebToast('Selected as default picture', 'top');
                                                    GetPIMPictures(sessionStorage.ViewID);
                                                })
                                            })
                                        ).append('&nbsp;')
                                        .append($('<button data-role="none" class="btn btn-danger" title="Remove this picture">')
                                            .append($('<span class="bi-trash">'))
                                            .data('data', vRows[i])
                                            .click(function (e) {
                                                var data = $(this).data('data');
                                                data.OrgID = sessionStorage.OrgID;
                                                data.ViewID = sessionStorage.ViewID;
                                                NewMessageBox('Are you sure you want to remove this picture?', 'Message', [
                                                    {
                                                        Text: 'Yes', Click: function () {
                                                            data.State = 'remove';
                                                            SaveImagePIM(JSON.stringify(data), function () {
                                                                WebToast('Deleted successfully', 'top');
                                                                GetPIMPictures(sessionStorage.ViewID);
                                                            })
                                                        }
                                                    }, { Text: 'No' }
                                                ]);
                                            })
                                        )
                                    ).hover(function () {
                                        $(this).find('.divPIMPicOption').show();
                                    }, function () {
                                        $(this).find('.divPIMPicOption').hide();
                                    })
                                table.append(td);

                                //divPIMPicPreview.append($('<div>')
                                //        .append($('<img>')
                                //            .data('data', vRows[i]).attr('src', srcBig)
                                //        )
                                //    )

                                divPIMPicPreviewThumbnails.append($('<img>')
                                    .data('data', vRows[i]).attr('src', srcMed)
                                    .click(function () {
                                        var data = $(this).data('data');
                                        carousel.goToSlide(data.Index);
                                    })
                                )                                
                                .append('&nbsp;')
                            }

                            //if(carousel){
                            //    carousel.destroySlider()
                            //}

                            //carousel = $('#divPIMPicPreview').bxSlider({
                            //    auto: false,
                            //    autoControls: true,
                            //    stopAutoOnClick: true,
                            //    pager: true,
                            //    slideWidth: 400,
                            //    touchEnabled: true,
                            //    responsive: true
                            //});

                            imgSelection.append(table);
                        } else {
                            imgSelection.append($('<h3>').text('Gallery is empty'));
                            $('#finalImage').attr('src', '/images/placeholder.png');
                        }                        
                    }
                })
            }

            function GetProductContent(strProductID) {
                var pData = [];

                pData = {
                    ProductID: strProductID
                };

                AjaxSend({
                    url: 'Stock/GetBomData',
                    data: pData,
                    success: function (vData) {
                        //LoadOrgProductPriceList(vData.OrgProductPriceData.Rows);
                        var count = 1;

                        $('#tblBodyBOM').empty();

                        $('#txtBomInstruction').text(vData.parent.BomInstruction || 'No instruction specified.');
                        $('#txtareaBomInstruction').text(vData.parent.BomInstruction || '');
                        $('#txtEncodedUpcharge').val(CStr2(vData.parent.Upcharge));

                        if (vData.parent.Jit != undefined) {
                            jit = CBool(vData.parent.Jit) ? '1' : '0';
                            $('input:radio[name=optJit][value=' + jit + ']').prop('checked', 1);
                        }
                        else {
                            $('input:radio[name=optJit]').removeAttr('checked');
                        }

                        var pTotalFactoryPrice = 0;
                        var pTotalFreightCost = 0;
                        var pTotalLandedCost = 0;

                        if (vData.contents.Rows.length == 0) {
                            $('#tblBodyBOM').append('<tr><td colspan="12" style="text-align:center;">No Result(s) Found</td></tr>')
                        }
                        else {
                            for (var i = 0; i < vData.contents.Rows.length; i++) {
                                pTotalFactoryPrice += CDec2(vData.contents.Rows[i].TotalFactoryPrice);
                                pTotalFreightCost += CDec2(vData.contents.Rows[i].TotalFreightCost);
                                pTotalLandedCost += CDec2(vData.contents.Rows[i].TotalLandedCost);

                                $('#tblBodyBOM').append($('<tr style="cursor:pointer;">')
                                    .attr('ID', vData.contents.Rows[i].ChildProductID)
                                    .attr('ProductName', vData.contents.Rows[i].ProductName)
                                    .append($('<td>')
                                        .text(count + '.')
                                    )
                                    .append($('<td>')
                                        .text(vData.contents.Rows[i].ProductNo)
                                    )
                                    .append($('<td>')
                                        .text(vData.contents.Rows[i].ProductName)
                                    )
                                    .append($('<td>')
                                        .text(AccountingFormat(vData.contents.Rows[i].Quantity))
                                    )
                                    .append($('<td>')
                                        .text(vData.contents.Rows[i].UnitMeasure)
                                    )
                                    .append($('<td>')
                                        .text(AccountingFormat(vData.contents.Rows[i].FactoryPrice))
                                    )
                                    .append($('<td>')
                                        .text(AccountingFormat(vData.contents.Rows[i].TotalFactoryPrice))
                                    )
                                    .append($('<td>')
                                        .text(AccountingFormat(vData.contents.Rows[i].FreightCost))
                                    )
                                    .append($('<td>')
                                        .text(AccountingFormat(vData.contents.Rows[i].TotalFreightCost))
                                    )
                                    .append($('<td>')
                                        .text(AccountingFormat(vData.contents.Rows[i].LandedCost))
                                    )
                                    .append($('<td>')
                                        .text(AccountingFormat(vData.contents.Rows[i].TotalLandedCost))
                                    )
                                    .append($('<td>')
                                        .append(
                                            $('<a data-role="none" class="btn btn-danger" title="Remove">')
                                            .append($('<span class="bi-trash">'))
                                        )
                                        .click(function (e) {
                                            e.preventDefault();
                                            var data = $(this).parents().data('data');
                                            NewMessageBox('Are you sure you want to remove ' + $(this).parents().attr('ProductName') + ' as a content of ' + productname + '?', 'Remove', [
                                                {
                                                    Text: 'Yes', Click: function () {
                                                        DeleteBomContent(data.ChildProductID);
                                                    }
                                                }, { Text: 'No' }
                                            ]);
                                        })
                                    )
                                    .data('data', vData.contents.Rows[i])
                                    .on('click', 'td:not(:last-child)', function (e) {
                                        e.preventDefault();
                                        $('#btnBackBomRawMaterials').hide();
                                        $('#panAddRawMaterials').modal('show').unbind('shown.bs.modal').on('shown.bs.modal', function () {
                                            $('#txtQty').val('').focus();
                                        });
                                        childProductID = $(this).parents().attr('ID');
                                        GetBomPageContentData(childProductID);
                                    })
                                )
                                count += 1;
                            }
                        }

                        $('#thBomTotalFactoryPrice').text(AccountingFormat(pTotalFactoryPrice));
                        $('#thBomTotalFreightCost').text(AccountingFormat(pTotalFreightCost));
                        $('#thBomTotalLandedCost').text(AccountingFormat(pTotalLandedCost));

                        $('#tblBodyBOM');//.trigger('create');
                        $('input:radio[name=optJit]');//.checkboxradio('refresh', true);
                        GetProductBOMData();
                    }
                });
            }

            function DeleteBomContent(id) {
                AjaxSend({
                    url: 'Stock/DeleteBomContent',
                    data: {
                        ParentProductID: sessionStorage.ViewID,
                        ChildProductID: id
                    },
                    success: function (vData) {
                        if (vData.Error != undefined) {
                            NewMessageBox(vData.Error, 'Error');
                            return;
                        }

                        GetProductContent(sessionStorage.ViewID);
                        GetProductPriceList(sessionStorage.ViewID);
                    },
                    error: function (vData) {
                        NewMessageBox(vError.statusText, 'Error');
                    }
                });
            }

            function SaveOrgProdBomDefinition() {

                AjaxSend({
                    url: 'Stock/SaveBomInstruction',
                    data: {
                        ProductID: sessionStorage.ViewID,
                        BomInstruction: $('#txtareaBomInstruction').val(),
                        Jit: $('input:radio[name=optJit]:checked').val()
                    },
                    success: function (vData) {
                        if (vData.Error != undefined) {
                            NewMessageBox(vData.statusText, 'Error');
                            return;
                        }
                        $('.statictextarea').removeClass('staticeditmodetextarea').addClass('staticmodetextarea');
                        $('.inputtextarea').removeClass('editmodetextarea').addClass('staticeditmodetextarea');
                        $('#btnOrgProductBOMInstructionEdit').removeAttr('actionmode').attr('actionmode', 'edit').text('Edit');
                        $('#divActionButtonForJit').hide();
                        GetProductContent(sessionStorage.ViewID);
                    },
                    error: function (vData) {
                        NewMessageBox(vData.statusText, 'Error');
                    }
                });
            }

            function SaveOrgProdSeperateIntoComponentDuringSalesReturns() {

                AjaxSend({
                    url: 'Stock/SaveSeperateIntoComponentDuringSalesReturns',
                    data: {
                        ProductID: sessionStorage.ViewID,
                        IsSeperateIntoComponentDuringSalesReturns: CInt2($('#selSeperateIntoComponentDuringSalesReturns').is(':checked')),
                    },
                    success: function (vData) {
                        $('#divActionButtonForComponentsDuringSalesReturns').hide();
                    },
                    error: function (vData) {
                        
                    }
                });
            }

            function SaveOrganizationProduct() {
                var pData = [];

                pData = {
                    OrgID: sessionStorage["OrgID"],
                    OrgProductID: $.trim($("#txtOrgPIMProductID").val()),
                    ProductID: $.trim($("#txtOrgProductID").val()),
                    IsAsset: CInt2($('#selProdIsAsset').is(':checked')),
                    IsPromoItem: CInt2($('#selProdIsPromoItem').is(':checked')),
                    IsInventoryCountItem: CInt2($('#selProdIsInventoryCountItem').is(':checked')),
                    IsEcommerceVendorAllowedToSell: CBool($('#selProdSellInEcommerce').val()),
                    Brand: $.trim($("#txtOrgProductBrand").val()),
                    ProductNo: $.trim($("#txtOrgProductNo").val()),
                    ProductName: $.trim($("#txtOrgProductName").val()),
                    ModelNo: $.trim($("#txtOrgProductModelNo").val()),
                    GroupNo: $.trim($("#txtOrgProductGroupNo").val()),
                    Division: $.trim($("#txtOrgProductDivision").val()),
                    Category: $.trim($("#txtOrgProductCategory").val()),
                    MarketingText: $.trim($("#txtOrgProductMaketingText").val()),
                    Warranty: $.trim($("#txtOrgProductWarranty").val()),
                    PaymentTermID: mPaymentTermID,
                    PublishedByID: $.trim($("#txtOrgProductPublishedBy").data('ID')),
                    UploadedByID: $.trim($("#txtOrgProductUploadedBy").data('ID')),
                    IsDoNotShareData: CInt2($("#selOrgProductDoNotShareData").is(':checked')),
                    ActivationDate: $("#selOrgProductActivationDateMonth").val() + '/' + $("#selOrgProductActivationDateDay").val() + '/' + $("#selOrgProductActivationDateYear").val(),
                    IsDeactivated: CInt2($("#selOrgProductDeactivate").is(':checked')),
                    ProductAvailableToMobile: CInt2($("#selOrgProductAvailableToMobile").is(':checked')),
                    DeactivationDate: $("#selOrgProductDateInActiveMonth").val() + '/' + $("#selOrgProductDateInActiveDay").val() + '/' + $("#selOrgProductDateInActiveYear").val(),
                    DeactivationReason: $("#txtOrgProductInActiveReason").val(),
                    Comments: $("#txtOrgProductComments").val(),
                    IsBan: CInt2($("#selOrgProductBan").is(':checked')),
                    IsVatExempt: CInt2($('#selOrgVatExempt').is(':checked')),
                    MaxAllowableDiscountableAmount: $('[name="chbMaxOption"]:checked').val() == 'custom' ? CDec2($("#txtOrgMaxAllowableDiscountableAmount").val()) : 0,
                    ItemBasedMaxAllowableDiscount: $('[name="chbMaxOption"]:checked').val() == 'reference' ? maxreferenceskuid : '',
                    DiscountedPriceAction: $("[name='radRoundOptions']:checked").val(),
                };

                Bs.Loading('show');

                AjaxSend({
                    url: "Organization/CheckOrganizationProduct",
                    data: pData,
                    success: function (vData) {

                        if (vData.OrgProductData.Rows.length > 0) {
                            Bs.Loading('hide');
                            NewMessageBox('Product being added already exist.', 'Product Exist');
                        }
                        else {
                            AjaxSend({
                                url: "Organization/SaveOrganizationProduct",
                                data: pData,
                                success: function (vSaveData) {
                                    Bs.Loading('hide');
                                    if (vSaveData.OrgProductData == "ActivationDateError") {
                                        NewMessageBox('Please enter valid activation date.', 'Activation Date');
                                    }
                                    else if (vSaveData.OrgProductData == "DeactivationDateError") {
                                        NewMessageBox('Please enter valid deactivation date.', 'Deactivation Date');
                                    }
                                    else {
                                        //$.mobile.changePage('#pageOrgProduct');
                                        //                                        ChangePage('/Organization/OrganizationProduct');
                                        IsCancelled = false;
                                        //getOrgProductList();
                                        ShowOrgProductInfo(sessionStorage.ViewID);
                                        $('.statictext').removeClass('staticeditmodeinputtext').addClass('staticmodeinputtext');
                                        $('.inputtext').removeClass('editmodeinputtext').addClass('staticeditmodeinputtext');
                                        //$('#btnOrgProductEdit').removeAttr('actionmode').attr('actionmode', 'edit').text('Edit Information');
                                        $('#divOrgProductInfoActionButton, #btnOrgProductInfoCancel, #btnOrgProductInfoSave').hide();
                                        $('#btnOrgProductEdit').show();
                                        $(window).scrollTop(0);
                                    }

                                }
                            });

                        }

                    }
                });

            };

            function ClearOrgProductFields() {

                $('#btnProductBrowser').show();

                $("#txtOrgPIMProductID").val('');

                $("#txtOrgProductID").val('');

                DisabledProductFields(false);
                $('#selProdIsAsset').prop('checked', 0);//.slider();//.slider('refresh');
                $('#selProdIsPromoItem').prop('checked', 0);//.slider();//.slider('refresh');
                $('#selProdIsInventoryCountItem').prop('checked', 1);
                $("#txtOrgProductBrand").val('');
                $("#txtOrgProductNo").val('');
                $("#txtOrgProductName").val('');
                $("#txtOrgProductModelNo").val('');
                $("#txtOrgProductGroupNo").val('');
                $("#txtOrgProductDivision").val('');
                $("#txtOrgProductCategory").val('');
                $("#txtOrgProductMaketingText").val('');
                $("#txtOrgProductWarranty").val('');
                $("#txtOrgProductPublishedBy").val('');
                $("#txtOrgProductUploadedBy").val('');

                $("#txtOnlyOrgProductIsAsset").text('');
                $("#txtOnlyOrgProductIsPromoItem").text('');
                $("#txtOnlyOrgProductSellInEcommerce").text('');
                $("#txtOnlyOrgProductBrand").text('');
                $("#txtOnlyOrgProductNo").text('');
                $("#txtOnlyOrgProductName").text('');
                $("#txtOnlyOrgProductModelNo").text('');
                $("#txtOnlyOrgProductGroupNo").text('');
                $("#txtOnlyOrgProductDivision").text('');
                $("#txtOnlyOrgProductCategory").text('');
                $("#txtOnlyOrgProductMaketingText").text('');
                $("#txtOnlyOrgProductWarranty").text('');
                $("#txtOnlyOrgProductPublishedBy").text('');
                $("#txtOnlyOrgProductUploadedBy").text('');

                $("#selOrgProductDoNotShareData").prop('checked', 0);//.slider();//.slider("refresh");

                $("#selOrgProductActivationDateMonth").val(parseInt(new Date().getMonth()) + 1);//.selectmenu();//.selectmenu('refresh');
                $("#selOrgProductActivationDateYear").val(new Date().getFullYear());//.selectmenu();//.selectmenu('refresh');
                setDayCollection('selOrgProductActivationDateMonth', 'selOrgProductActivationDateDay', 'selOrgProductActivationDateYear', false);
                $("#selOrgProductActivationDateDay").val(new Date().getDate());//.selectmenu();//.selectmenu('refresh');

            };

            function DisabledProductFields(vEnable) {

                $("#txtOrgProductBrand").attr("disabled", vEnable);
                $("#txtOrgProductNo").attr("disabled", vEnable);
                $("#txtOrgProductName").attr("disabled", vEnable);
                $("#txtOrgProductModelNo").attr("disabled", vEnable);
                $("#txtOrgProductGroupNo").attr("disabled", vEnable);
                $("#txtOrgProductDivision").attr("disabled", vEnable);
                $("#txtOrgProductCategory").attr("disabled", vEnable);
                $("#txtOrgProductMarketingText").attr("disabled", vEnable);
                $("#txtOrgProductWarranty").attr("disabled", vEnable);
                $("#txtOrgProductPublishedBy").attr("disabled", vEnable);
                $("#txtOrgProductUploadedBy").attr("disabled", vEnable);

                // Measurements
                $("#btnOrgProductMeasurementAdd").attr("disabled", vEnable);
                $("#txtRecProductMeasurementID").attr("disabled", vEnable);

                $("#txtRecProductMeasurementPackingSizeUnitID").attr("disabled", vEnable);
                $("#btnPackingSizeUnit").attr("disabled", vEnable);
                $("#txtPackingSizeQty").attr("disabled", vEnable);
                $("#txtPackingSizeVolDiscount").attr("disabled", vEnable);

                $("#txtPackingSizeContentQty").attr("disabled", vEnable);
                $("#btnPackingSizeContentUnit").attr("disabled", vEnable);

                $("#txtPackingSizeDimensionLength").attr("disabled", vEnable);
                $("#btnPackingSizeDimLengthUnit").attr("disabled", vEnable);

                $("#txtPackingSizeDimensionWidth").attr("disabled", vEnable);
                $("#btnPackingSizeDimWidthUnit").attr("disabled", vEnable);

                $("#txtPackingSizeDimensionHeight").attr("disabled", vEnable);
                $("#btnPackingSizeDimensionHeightUnit").attr("disabled", vEnable);

                $("#txtPackingSizeWeight").attr("disabled", vEnable);
                $("#btnPackingSizeWeightUnit").attr("disabled", vEnable);


                $("#btnProductMeasurementSave").attr("disabled", vEnable);

                //                // Prices
                //                $("#btnOrgProductPriceLevelAdd").attr("disabled", vEnable);

                //                $("#btnProductPriceLevel").attr("disabled", vEnable);
                //                $("#txtProductPrice").attr("disabled", vEnable);

            };

            function ClearOrgProductInActiveFields() {

                $("#selOrgProductDeactivate").prop('checked', 0);//.slider("refresh");
                
                $("#selOrgProductDateInActiveMonth").val('');//.selectmenu('refresh');
                $("#selOrgProductDateInActiveDay").val('');//.selectmenu('refresh');
                $("#selOrgProductDateInActiveYear").val('');//.selectmenu('refresh');

                $("#txtOrgProductInActiveReason").val('');
                $("#txtOrgProductComments").val('');

                $("#selOrgProductBan").prop('checked', 0);//.slider("refresh");
            };

            function GetProductMeasurementList(strProductID) {
                var pData = [];

                pData = {
                    ProductID: strProductID
                };

                AjaxSend({
                    url: 'Organization/getOrganizationProductMeasurementList',
                    data: pData,
                    success: function (vData) {
                        //LoadOrgProductMeaurementList(vData.OrgProductMeasurementData.Rows);
                        var count = 1;

                        $('#tblBodyMeaurement').empty();

                        mUnitMeasures = vData.OrgProductMeasurementData.Rows || [];
                        if (vData.OrgProductMeasurementData.Rows.length == 0) {
                            $('#tblBodyMeaurement').append('<tr><td colspan="5" style="text-align:center;">No Result(s) Found</td></tr>')
                        }
                        else {
                            for (var i = 0; i < vData.OrgProductMeasurementData.Rows.length; i++) {
                                $('#tblBodyMeaurement').append($('<tr style="cursor:pointer;">')
                                    .attr('ID', vData.OrgProductMeasurementData.Rows[i].ID)
                                    .append($('<td>')
                                        .text(count + '.')
                                    )
                                    .append($('<td style="text-align:left; vertical-align: middle;">')
                                        .text(vData.OrgProductMeasurementData.Rows[i].UnitMeasure)
                                    )
                                    .append($('<td style="text-align:center; vertical-align: middle;">')
                                        .text(vData.OrgProductMeasurementData.Rows[i].Quantity)
                                    )
                                    .append($('<td style="text-align:left; vertical-align: middle;">')
                                        .text(vData.OrgProductMeasurementData.Rows[i].BaseUnit)
                                    )
                                    .append($('<td style="text-align:center; vertical-align: middle;">')
                                        .text(AccountingFormat(vData.OrgProductMeasurementData.Rows[i].Discount) + '%')
                                    )
                                    .append($('<td style="text-align:center; vertical-align: middle;">')
                                        .text(AccountingFormat(vData.OrgProductMeasurementData.Rows[i].ContentQty))
                                    )
                                    .append($('<td style="text-align:center; vertical-align: middle;">')
                                        .text(vData.OrgProductMeasurementData.Rows[i].ContentUnit)
                                    )
                                    .append($('<td style="text-align:center; vertical-align: middle;">')
                                        .text(AccountingFormat(vData.OrgProductMeasurementData.Rows[i].ProdDimensionLength, 4, null, null, null, true) + ' ' + CStr2(vData.OrgProductMeasurementData.Rows[i].ProdDimensionLengthUnit))
                                    )
                                    .append($('<td style="text-align:center; vertical-align: middle;">')
                                        .text(AccountingFormat(vData.OrgProductMeasurementData.Rows[i].ProdDimensionWidth, 4, null, null, null, true) + ' ' + CStr2(vData.OrgProductMeasurementData.Rows[i].ProdDimensionWidthUnit))
                                    )
                                    .append($('<td style="text-align:center; vertical-align: middle;">')
                                        .text(AccountingFormat(vData.OrgProductMeasurementData.Rows[i].ProdDimensionHeight, 4, null, null, null, true) + ' ' + CStr2(vData.OrgProductMeasurementData.Rows[i].ProdDimensionHeightUnit))
                                    )
                                    .append($('<td style="text-align:center; vertical-align: middle;">')
                                        .text(AccountingFormat(vData.OrgProductMeasurementData.Rows[i].ProductWeight, 4, null, null, null, true) + ' ' + CStr2(vData.OrgProductMeasurementData.Rows[i].ProductWeightUnit))
                                    )
                                    .data('data', vData.OrgProductMeasurementData.Rows[i])
                                    .click(function (e) {
                                        if (!isGrantAccessToModifyPIM) return;
                                        LoadUnitOfMeasurePage();
                                        $('#divContainerMenuPage').hide();
                                        $('#pageSKUMeasurement, #btnProductMeasurementRemove, #btnOrgProductUnitOfMeasures').show();
                                        viewid = sessionStorage.ViewID;
                                        ShowOrgProductMeasurementInfo($(this).attr('ID'));
                                        GetBarcodeType();
                                    })
                                );
                                count += 1;
                            }
                        }

                        // GetProductPriceList(vData.OrgProductMeasurementData.Rows[0].ProductID); // attempt by Arvin. pero error japon
                        GetProductPriceList(sessionStorage.ViewID); // original parameter: 

                    }
                });
            }

            function GetProductPriceBreakList(strProductID) {
                var pData;

                $('#tblBodyPriceBreak').empty();
                pData = {
                    ProductID: strProductID
                };

                AjaxSend({
                    url: 'Organization/GetOrganizationProductPriceBreakList',
                    data: pData,
                    success: function (vData) {
                        var pDR;

                        if (vData.OrgProductPriceBreakData.Rows.length == 0) {
                            $('#tblBodyPriceBreak').append('<tr><td colspan="5" style="text-align:center;">No Result(s) Found</td></tr>')
                        }
                        else {
                            for (var i = 0; i < vData.OrgProductPriceBreakData.Rows.length; i++) {
                                pDR = vData.OrgProductPriceBreakData.Rows[i];
                                $('#tblBodyPriceBreak').append($('<tr style="cursor:pointer;">')
                                    .attr('ID', pDR.ID)
                                    .append($('<td>')
                                        .text((i + 1) + '.')
                                    )
                                    .append($('<td>')
                                        .text(pDR.Qty)
                                    )
                                    .append($('<td>')
                                        .text(pDR.UnitMeasure)
                                    )
                                    .append($('<td>')
                                        .text(pDR.Discount + '%')
                                    )
                                    .append($('<td>')
                                        .text('')
                                    )
                                    .data('data', pDR)
                                    .click(function (e) {
                                        var pID = $($(e.target).parent()).data('data');
                                        LoadPriceBreak(pID.ID);
                                    })
                                );
                            } // END for (var i = 0; i < vData.OrgProductPriceBreakData.Rows.length; i++) {
                        }

                    }
                });
            } // END GetProductPriceBreakList

            function LoadPriceBreak(vID) {
                
                GetPriceBreak({
                    ID: vID,
                    Success: function(vData) {
                        DisplayPriceBreak(vData);
                    }
                });

            } // END LoadPriceBreak

            function DisplayPriceBreak(vData) {
                
                LoadPriceBreakPage('add', function() {
                    mPriceBreakUnitID = vData.UnitMeasureID;
                    $('#btnPriceBreakUnit').text(vData.UnitMeasure);
                    $('#txtPriceBreakQty').val(vData.Qty);
                    $('#txtPriceBreakDiscount').val(vData.Discount);
                    $('[name=radioPriceBreakDiscountType]').removeAttr('checked');//.checkboxradio('refresh');
                    $(':radio[name=radioPriceBreakDiscountType][value="' + vData.DiscountType + '"]').prop('checked', true);//.checkboxradio('refresh');
                }); // END LoadPriceBreakPage

            } // END DisplayPriceBreak

            function GetPriceBreak(vParams) {
                //vParams = {
                //    ID: 'guid',
                //    Success: function(vData) {
                //    }
                //};
                var pData = {
                    ProductID: $("#txtOrgProductID").val(),
                    ID: vParams.ID
                };
                AjaxSend({
                    url: 'Organization/GetOrganizationProductPriceBreakList',
                    data: pData,
                    success: function(vData) {
                        vParams.Success(vData.OrgProductPriceBreakData.Rows[0]);
                    }
                });
            } // END GetPriceBreak

            function GetParentProductInformation(strID) {
                var pData = { ParentProductID: strID }
                AjaxSend({
                    url: "Stock/GetParentProductInformation",
                    data: pData,
                    success: function (vData) {
                        var vRows = vData.Result.Rows;

                        if (vRows.length > 0) {
                            if (CBool(vRows[0].IsSeperateIntoComponentDuringSalesReturns)) {
                                $('#selSeperateIntoComponentDuringSalesReturns').prop('checked', 1)
                            } else {
                                $('#selSeperateIntoComponentDuringSalesReturns').prop('checked', 0)
                            }
                        } else {
                            $('#selSeperateIntoComponentDuringSalesReturns').prop('checked', 0)
                        }
                        

                        try {
                            $('#selSeperateIntoComponentDuringSalesReturns');//.slider('refresh');
                        }
                        catch (e) {
                            $('#selSeperateIntoComponentDuringSalesReturns');//.slider();
                        }

                    }
                })
            }

            function LoadOrgProductMeaurementList(vRows) {
                var pLV = $('#lstOrgProductMeasurement');
                var pItem;

                $("#divOrgProductMeasurementRecordCount").text(" Number of record(s) found: " + vRows.length);

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li>')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();
                                //$.mobile.changePage('#pageSKUMeasurement');
                                $('#pageOrgProduct').hide();
                                $('#pageOrgProductInfo').hide();
                                $('#pageSKUMeasurement').show();
                                $('#pageSKUPrice').hide();
                                $('#pageBOMSKUInfo').hide();
                                $('#pageBrowseByOrgOwner').hide();
                                $('#pageBarcodeSearch').hide();
                                $('#pageCustomAdd').hide();
                                $('#pageProductNamePreview').hide();
                                ShowOrgProductMeasurementInfo($(this).attr('ID'));
                            })
                            .append($('<h3>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].Quantity + " / " + vRows[p].UnitMeasure)
                            )
                            .append($('<p>')
                                .attr("style", "white-space:normal")
                                .text("Volume Discount : " + AccountingFormat(vRows[p].Discount))
                            )
                            .append($('<p>')
                                .attr("style", "white-space:normal")
                                .text("Content : " + AccountingFormat(vRows[p].ContentQty) + ' ' + (vRows[p].ContentUnit || ''))
                            )
                        );
                    pLV.append(pItem);
                };

                pLV;//.listview('refresh');
            };

            function ShowOrgProductMeasurementInfo(strID) {
                var pData = {};
                pData.ID = strID;

                ClearOrgProductMeasurementFields();

                AjaxSend({
                    url: "Organization/getOrganizationProductMeasurementInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.OrgProductMeasurementData.Rows.length > 0) {

                            $("#txtRecProductMeasurementID").val(vData.OrgProductMeasurementData.Rows[0].ID || '');

                            $("#txtRecProductMeasurementPackingSizeUnitID").val(vData.OrgProductMeasurementData.Rows[0].UnitMeasureID || '');

                            $("#txtOnlyPackingSizeUnit").text(vData.OrgProductMeasurementData.Rows[0].UnitMeasure || '');
                            $("#txtPackingSizeUnit").val(vData.OrgProductMeasurementData.Rows[0].UnitMeasure || 'Please Select');
                            $("#btnPackingSizeUnit, #lblTimeBaseUoM").text(vData.OrgProductMeasurementData.Rows[0].UnitMeasure || 'Please Select'); // 2017_11_13__09_04_13_4061 SKU unit must be deletable and editable

                            $("#txtPackingSizeQty").val(vData.OrgProductMeasurementData.Rows[0].Quantity || '');
                            $("#txtOnlyPackingSizeQty").text(vData.OrgProductMeasurementData.Rows[0].Quantity || '');
                            $("[name=spanBaseUnit]").text(vData.OrgProductMeasurementData.Rows[0].BaseUnit || '');

                            $("#txtPackingSizeVolDiscount").val(vData.OrgProductMeasurementData.Rows[0].Discount || '');
                            $("#txtOnlyPackingSizeVolDiscount").text(vData.OrgProductMeasurementData.Rows[0].Discount || '');

                            $("#txtPackingSizeContentQty").val(vData.OrgProductMeasurementData.Rows[0].ContentQty || '');
                            $("#txtOnlyPackingSizeContentQty").text(vData.OrgProductMeasurementData.Rows[0].ContentQty || '');
                            $("#btnPackingSizeContentUnit").text(vData.OrgProductMeasurementData.Rows[0].ContentUnit || 'Please Select');
                            $("#spanPackingSizeContentUnit").text(vData.OrgProductMeasurementData.Rows[0].ContentUnit || '');
                            $("#txtPackingSizeContentUnit").text(vData.OrgProductMeasurementData.Rows[0].ContentUnit || '')

                            $("#txtPackingSizeDimensionLength").val(vData.OrgProductMeasurementData.Rows[0].ProdDimensionLength || '');
                            $("#txtOnlyPackingSizeDimensionLength").text(vData.OrgProductMeasurementData.Rows[0].ProdDimensionLength || '');
                            $("#btnPackingSizeDimLengthUnit").text(vData.OrgProductMeasurementData.Rows[0].ProdDimensionLengthUnit || 'Please Select');
                            $("#txtPackingSizeDimLengthUnit").text(vData.OrgProductMeasurementData.Rows[0].ProdDimensionLengthUnit || '');

                            $("#txtPackingSizeDimensionWidth").val(vData.OrgProductMeasurementData.Rows[0].ProdDimensionWidth || '');
                            $("#txtOnlyPackingSizeDimensionWidth").text(vData.OrgProductMeasurementData.Rows[0].ProdDimensionWidth || '');
                            $("#btnPackingSizeDimWidthUnit").text(vData.OrgProductMeasurementData.Rows[0].ProdDimensionWidthUnit || 'Please Select');
                            $("#txtPackingSizeDimWidthUnit").text(vData.OrgProductMeasurementData.Rows[0].ProdDimensionWidthUnit || '');

                            $("#txtPackingSizeDimensionHeight").val(vData.OrgProductMeasurementData.Rows[0].ProdDimensionHeight || '');
                            $("#txtOnlyPackingSizeDimensionHeight").text(vData.OrgProductMeasurementData.Rows[0].ProdDimensionHeight || '');
                            $("#btnPackingSizeDimensionHeightUnit").text(vData.OrgProductMeasurementData.Rows[0].ProdDimensionHeightUnit || 'Please Select');
                            $("#txtPackingSizeDimensionHeightUnit").text(vData.OrgProductMeasurementData.Rows[0].ProdDimensionHeightUnit || '');

                            $("#txtPackingSizeWeight").val(vData.OrgProductMeasurementData.Rows[0].ProductWeight || '');
                            $("#txtOnlyPackingSizeWeight").text(vData.OrgProductMeasurementData.Rows[0].ProductWeight || '');
                            $("#btnPackingSizeWeightUnit").text(vData.OrgProductMeasurementData.Rows[0].ProductWeightUnit || 'Please Select');
                            $("#txtPackingSizeWeightUnit").text(vData.OrgProductMeasurementData.Rows[0].ProductWeightUnit || '');

                            $('#selUoMTimeBase').prop('checked', CInt2(vData.OrgProductMeasurementData.Rows[0].IsTimeBaseUnit));//.slider('refresh');
                            $('#txtUoMTimeBase').text((CBool(vData.OrgProductMeasurementData.Rows[0].IsTimeBaseUnit) ? 'Yes' : 'No'));

                            if (CBool(vData.OrgProductMeasurementData.Rows[0].IsTimeBaseUnit)) {
                                $('#txtUoMTimeBaseDescription').text(vData.OrgProductMeasurementData.Rows[0].TimeBaseUnitDescription);
                                $('[name="radTimeBase"][value="' + vData.OrgProductMeasurementData.Rows[0].TimeBaseUnitDescription + '"]').attr('checked', true);//.checkboxradio('refresh');
                                $('#divForTimeBase').show();
                            } else {
                                $('#txtUoMTimeBaseDescription').text('');
                                $('[name="radTimeBase"').removeAttr('checked');//.checkboxradio('refresh');
                                $('#divForTimeBase').hide();
                            }

                            $("#btnProductMeasurementRemove").hide();

                            $('.statictext').removeClass('staticeditmodeinputtext').addClass('staticmodeinputtext');
                            $('.inputtext').removeClass('editmodeinputtext').addClass('staticeditmodeinputtext');

                            GetProductMeasurementBarcodeList(vData.OrgProductMeasurementData.Rows[0].ID);
                        }
                    }
                });

            };

            function GetProductMeasurementBarcodeList(strMeasurementID) {
                var pData = [];

                pData = {
                    MeasurementID: strMeasurementID
                };

                AjaxSend({
                    url: 'Organization/GetOrganizationProductMeasurementBarcode',
                    data: pData,
                    success: function (vData) {
                        LoadOrgProductMeaurementBarcode(vData.OrgProductData.Rows);
                    }
                });
            }

            function LoadOrgProductMeaurementBarcode(vRows) {
                var tbodyBarcode = $('#tbodyBarcode');

                tbodyBarcode.empty();

                if (vRows.length > 0) {
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        tbodyBarcode.append($('<tr>')
                                .data('data', vRows[p])
                                .append($('<td>')
                                    .text(vRows[p].Barcode)
                                )
                                .append($('<td class="txt-center">')
                                    .text(vRows[p].BarcodeType)
                                )
                                .append($('<td class="txt-center">')
                                    .text($.format.date(new Date(vRows[p].LastUpdated), 'MMM dd, yyyy hh:mm a'))
                                )
                                .append($('<td class="txt-center">')
                                    .data('data', vRows[p])
                                    .append($('<button data-role="none" class="btn custom-btn-theme-d btnCircle-xs">')
                                        .append($('<span class="glyphicon glyphicon-remove">'))
                                    )
                                    .click(function (e) {
                                        e.stopPropagation();
                                        e.preventDefault();

                                        if (!isGrantAccessToModifyPIM) {
                                            NewMessageBox('You\'re not allowed to modify product related info');
                                            return;
                                        }

                                        var data = $(this).data('data');
                                        NewMessageBox('Are you sure you want to remove this barcode?', 'Message', [
                                            {
                                                Text: 'Yes', Click: function () {
                                                    AjaxSend({
                                                        url: 'Organization/DeleteBarcode',
                                                        data: { BarcodeID: data.ID },
                                                        success: function (e) {
                                                            if(e.Results == 'saved'){
                                                                GetProductMeasurementBarcodeList($("#txtRecProductMeasurementID").val());
                                                            } else {
                                                                NewMessageBox(e.Results, 'Message');
                                                            }                                                            
                                                        }
                                                    })
                                                }
                                            }, { Text: 'No' }
                                        ])
                                    })
                                )
                                .click(function (e) {
                                    e.stopPropagation();
                                    e.preventDefault();

                                    var data = $(this).data('data');
                                    $('#popBarcode').modal('show');//.trigger('create').modal();
                                    $('#txtBarcodeID').val(data.ID);
                                    $('#txtBarcodeNumber').val(data.Barcode).focus();
                                    $('#selBarcodeType').val(data.BarcodeTypeID);//.selectmenu('refresh');                                    
                                })
                            )
                        //arOrgProductMeaBarcode = new Array();
                        //arOrgProductMeaBarcode[intOrgProductMeaBarcodeRecID] = vRows[p].ID;
                        //arOrgProductMeaBarcode[intOrgProductMeaBarcode] = vRows[p].Barcode;
                        //arOrgProductMeaBarcode[intOrgProductMeaState] = "";

                        //sessionStorage["OrgProductMeasurementBarcode-" + p] = JSON.stringify(arOrgProductMeaBarcode);

                    };
                } else {
                    tbodyBarcode.append($('<tr>')
                            .append($('<td colspan="4" style="text-align:center;">')
                                .text('No barcodes')
                            )
                        )
                }

                //intOrgProductMeaBarcodeCntr = vRows.length;
                //LoadOrgProductMeaurementBarcodeList();

            };

            function LoadOrgProductMeaurementBarcodeList() {
                var pLV = $('#lstProductMeasurementBarcode');
                var pItem;

                pLV.html('');
                for (var i = 0; i <= intOrgProductMeaBarcodeCntr; i++) {

                    if (sessionStorage["OrgProductMeasurementBarcode-" + i] != undefined) {

                        arOrgProductMeaBarcode = new Array();
                        arOrgProductMeaBarcode = JSON.parse(sessionStorage["OrgProductMeasurementBarcode-" + i]);

                        if (arOrgProductMeaBarcode[intOrgProductMeaState] != "Remove") {

                            pItem = $('<li id="lst' + i + '">')
                            .append($('<a>')
                                    .attr('ID', i)
                                    .attr('href', "#")
                                    .data('Data', arOrgProductMeaBarcode)
                                    .append($('<h3>')
                                        .attr("style", "white-space:normal")
                                        .text(arOrgProductMeaBarcode[intOrgProductMeaBarcode])
                                    )
                            )
                            .append($('<a>')
                                .attr('ID', i)
                                .attr('href', "#")
                                .attr('data-icon', "minus")
                                .data('Data', arOrgProductMeaBarcode)
                                .click(function (e) {
                                    e.preventDefault();

                                    sessionStorage["OrgProductMeasurementBarcodeToRemoveIndex"] = $(this).attr('ID');
                                    sessionStorage["OrgProductMeasurementBarcodeToRemove"] = JSON.stringify($(this).data('Data'));
                                    NewMessageBox('Are you sure you want to remove this record?', 'Message', [
                                        {
                                            Text: 'Yes', Click: function () {
                                                var arOrgProductMeaBarcodeToRemove = new Array();
                                                arOrgProductMeaBarcodeToRemove = JSON.parse(sessionStorage["OrgProductMeasurementBarcodeToRemove"]);
                                                intOrgProductMeaBarcodeIndex = sessionStorage["OrgProductMeasurementBarcodeToRemoveIndex"];

                                                arOrgProductMeaBarcodeToRemove[intOrgProductMeaState] = "Remove";
                                                sessionStorage["OrgProductMeasurementBarcode-" + intOrgProductMeaBarcodeIndex] = JSON.stringify(arOrgProductMeaBarcodeToRemove);

                                                LoadOrgProductMeaurementBarcodeList();

                                                //OLD sessionStorage - ERROR: Unexpected token u
                                                localStorage.removeItem("OrgProductMeasurementBarcodeToRemove");
                                                localStorage.removeItem("OrgProductMeasurementBarcodeToRemoveIndex");
                                                $('#panOrgEmpMessage').modal('hide');
                                                $('#divUnitOfMeasureActionButton').show();
                                            }
                                        }, { Text: 'No' }
                                    ])
                                })
                            );

                            pLV.append(pItem);

                        }

                    }

                    pLV.append(pItem);
                };

                pLV;//.listview('refresh');
            };

            function EnlistMeasurementBarcode(vBarcode) {
                if ($.trim(vBarcode) != "") {

                    IsNewOrgProductMeaBarcodeAdd = true
                    
                    intOrgProductMeaBarcodeIndex = intOrgProductMeaBarcodeCntr;

                    arOrgProductMeaBarcode = new Array();
                    arOrgProductMeaBarcode[intOrgProductMeaBarcodeRecID] = "";
                    arOrgProductMeaBarcode[intOrgProductMeaBarcode] = vBarcode;
                    arOrgProductMeaBarcode[intOrgProductMeaState] = (IsNewOrgProductMeaBarcodeAdd ? "New" : "Edit");

                    sessionStorage["OrgProductMeasurementBarcode-" + intOrgProductMeaBarcodeIndex] = JSON.stringify(arOrgProductMeaBarcode);

                    $('#txtProductMeasurementBarcode').val('');
                    $('#divUnitOfMeasureActionButton').show();
                    LoadOrgProductMeaurementBarcodeList();
                    intOrgProductMeaBarcodeCntr = intOrgProductMeaBarcodeCntr + 1;
                }
            }

            function ClearOrgProductMeasurementFields() {

                //proj.ClearSessionStorage();

                sessionStorage["LibraryName"] = libname;
                sessionStorage["LabelLibraryName"] = lbllibname;
                sessionStorage["AddViewLibBCName"] = addlibname;
                sessionStorage["ViewID"] = viewid;

                $("#txtRecProductMeasurementID").val('');

                $("#txtRecProductMeasurementPackingSizeUnitID").val('');
                $("#btnPackingSizeUnit").text('Please Select');
                $("#txtPackingSizeQty").val('1');
                $("#txtPackingSizeVolDiscount").val('0');

                $("#txtPackingSizeContentQty").val('');
                $("#btnPackingSizeContentUnit").text('Please Select');

                $("#txtPackingSizeDimensionLength").val('');
                $("#btnPackingSizeDimLengthUnit").text('Please Select');

                $("#txtPackingSizeDimensionWidth").val('');
                $("#btnPackingSizeDimWidthhUnit").text('Please Select');

                $("#txtPackingSizeDimensionHeight").val('');
                $("#btnPackingSizeDimensionHeightUnit").text('Please Select');

                $("#txtPackingSizeWeight").val('');
                $("#btnPackingSizeWeightUnit").text('Please Select');

                $('#lstProductMeasurementBarcode').empty();

                $("#btnProductMeasurementRemove").hide();

            }

            function SaveOrgProductMeasurement() {
                var pDataMeasurementBarcode = [];
                var pData = {};
                var intIndex;

                //Measurement Barcode
                intIndex = 0;
                for (var i = 0; i <= intOrgProductMeaBarcodeCntr; i++) {

                    if (sessionStorage["OrgProductMeasurementBarcode-" + i] != undefined) {

                        arOrgProductMeaBarcode = new Array();
                        arOrgProductMeaBarcode = JSON.parse(sessionStorage["OrgProductMeasurementBarcode-" + i]);

                        pDataMeasurementBarcode[intIndex] = {
                            ID: arOrgProductMeaBarcode[intOrgProductMeaBarcodeRecID],
                            Barcode: arOrgProductMeaBarcode[intOrgProductMeaBarcode],
                            RecState: arOrgProductMeaBarcode[intOrgProductMeaState]
                        };

                        intIndex = intIndex + 1;

                    }
                }


                pData.ID = $("#txtRecProductMeasurementID").val();
                pData.ProductID = $("#txtOrgProductID").val();
                pData.UnitMeasureID = $("#txtRecProductMeasurementPackingSizeUnitID").val();
                pData.Quantity = $("#txtPackingSizeQty").val();
                pData.Discount = $("#txtPackingSizeVolDiscount").val();
                pData.ContentQty = $("#txtPackingSizeContentQty").val();
                pData.ContentUnit = ($("#btnPackingSizeContentUnit").text() == "Please Select" ? "" : $.trim($("#btnPackingSizeContentUnit").text()));
                pData.ContentWeight = $("#txtPackingSizeWeight").val();
                pData.ContentWeightUnit = ($("#btnPackingSizeWeightUnit").text() == "Please Select" ? "" : $.trim($("#btnPackingSizeWeightUnit").text()));
                pData.DimensionLenght = $("#txtPackingSizeDimensionLength").val();
                pData.DimensionLenghtUnit = ($("#btnPackingSizeDimLengthUnit").text() == "Please Select" ? "" : $.trim($("#btnPackingSizeDimLengthUnit").text()));
                pData.DimensionWidth = $("#txtPackingSizeDimensionWidth").val();
                pData.DimensionWidthUnit = ($("#btnPackingSizeDimWidthUnit").text() == "Please Select" ? "" : $.trim($("#btnPackingSizeDimWidthUnit").text()));
                pData.DimensionHeight = $("#txtPackingSizeDimensionHeight").val();
                pData.DimensionHeightUnit = ($("#btnPackingSizeDimensionHeightUnit").text() == "Please Select" ? "" : $.trim($("#btnPackingSizeDimensionHeightUnit").text()));
                pData.OrgProductMeasurementBarcodeList = pDataMeasurementBarcode;
                pData.IsTimeBaseUnit = CInt2($('#selUoMTimeBase').is(':checked'));
                pData.TimeBaseUnit = $('[name="radTimeBase"]:checked').val();

                AjaxSend({
                    url: "Organization/SaveOrganizationProductMeasurement",
                    data: pData,
                    success: function (vData) {

                        //$.mobile.changePage('#pageOrgProductInfo');
                        $('#pageOrgProduct').hide();
                        $('#divContainerMenuPage').show();
                        $('#pageSKUMeasurement').hide();
                        $('#pageSKUPrice').hide();
                        $('#pageBOMSKUInfo').hide();
                        $('#pageBrowseByOrgOwner').hide();
                        $('#pageBarcodeSearch').hide();
                        $('#pageCustomAdd').hide();
                        $('#pageProductNamePreview').hide();
                        $('#btnProductMeasurements').click();
                        GetProductMeasurementList($("#txtOrgProductID").val());
                        $(window).scrollTop(0);
                    }
                });

            };

            function GetProductPriceList(strProductID) {
                var pData = [];

                pData = {
                    ProductID: strProductID,
                    IsActive: $('#selVendorPriceStatus').val()
                };

                AjaxSend({
                    url: 'Organization/getOrganizationProductPriceList',
                    data: pData,
                    success: function (vData) {
                        //LoadOrgProductPriceList(vData.OrgProductPriceData.Rows);
                        var count = 1;

                        $('#tblBodyProductPrice').empty();

                        if (vData.OrgProductPriceData.Rows.length == 0) {
                            $('#tblBodyProductPrice').append('<tr><td colspan="3" style="text-align:center;">No Result(s) Found</td></tr>')
                        }
                        else {
                            for (var i = 0; i < vData.OrgProductPriceData.Rows.length; i++) {
                                $('#tblBodyProductPrice').append($('<tr style="cursor:pointer;">')
                                                                .attr('ID', vData.OrgProductPriceData.Rows[i].ID)
                                                                .append($('<td>')
                                                                    .text(count + '.')
                                                                )
                                                                .append($('<td>')
                                                                    .text(proj.ChangePriceLevelDisplay(vData.OrgProductPriceData.Rows[i].PriceLevel))
                                                                )
                                                                .append($('<td style="text-align:right">')
                                                                    .text((vData.OrgProductPriceData.Rows[i].Price))
                                                                )
                                                                .append($('<td style="text-align:right">')
                                                                    .text((vData.OrgProductPriceData.Rows[i].GrossProfit))
                                                                )
                                                                .append($('<td style="text-align:right">')
                                                                    .text(AccountingFormat(vData.OrgProductPriceData.Rows[i].GrossProfitMargin) + '%')
                                                                )
                                                                .append($('<td>')
                                                                    .html((vData.OrgProductPriceData.Rows[i].UpcomingPrice == undefined ? '' : AccountingFormat(vData.OrgProductPriceData.Rows[i].UpcomingPrice) + ' on <br> ' + FormatDate(vData.OrgProductPriceData.Rows[i].UpcomingPriceEffectivityDate, { Format: 'MM/dd/yyyy' })))
                                                                )
                                                                .append($('<td>')
                                                                    .text(CStr2(vData.OrgProductPriceData.Rows[i].UpdaterName))
                                                                )
                                                                .append($('<td>')
                                                                    .text(FormatDate(vData.OrgProductPriceData.Rows[i].LastUpdated, {Format: 'MM/dd/yyyy hh:mm a'}))
                                                                )
                                                                .append($('<td class="text-center ' + (isGrantAccessToModifyPIM ? '' : 'd-none') + '">')
                                                                    .append($('<button data-role="none" class="btn btn-danger">').text('Delete')
                                                                        .data('data', vData.OrgProductPriceData.Rows[i])
                                                                        .click(function (e) {
                                                                            e.stopPropagation();
                                                                            e.preventDefault();

                                                                            if (!isGrantAccessToModifyPIM) return;

                                                                            var data = $(this).data('data');
                                                                            NewMessageBox('Are you sure you want to delete this product price?', 'Confirmation Message', [
                                                                                {
                                                                                    Text: 'Yes', Click: function () {
                                                                                        AjaxSend({
                                                                                            url: '/Organization/DeleteOrganizationProductPrice',
                                                                                            data: {
                                                                                                ID: data.ID
                                                                                            }, success: function (e) {
                                                                                                if (e.Error)
                                                                                                    NewMessageBox(e.Error);
                                                                                                else if (e.Result == 'ok') {
                                                                                                    GetProductPriceList(sessionStorage.ViewID);
                                                                                                    WebToast('Product price has beenn successfully deleted');
                                                                                                } else {
                                                                                                    NewMessageBox('There was an error while deleting product price', 'Error');
                                                                                                }
                                                                                            }
                                                                                        })
                                                                                    }
                                                                                }, { Text: 'No' }
                                                                            ])
                                                                        })
                                                                    )
                                                                )
                                                                .data('data', vData.OrgProductPriceData.Rows[i])
                                                                .click(function (e) {
                                                                    if (!isGrantAccessToModifyPIM) return;
                                                                    LoadProductPricesPage();
                                                                    $('#divContainerMenuPage').hide();
                                                                    $('#pageSKUPrice').show();
                                                                    isPriceEdit = 1;
                                                                    ShowOrgProductPriceInfo($(this).attr('ID'));
                                                                })
                                )
                                count += 1;
                            }

                            if ($.fn.DataTable.isDataTable($('#tblBodyProductPrice').closest('table'))) {
                                $('#tblBodyProductPrice').closest('table').DataTable({
                                    initComplete: function () {
                                        $('#tblBodyProductPrice').closest('table').removeClass('dataTable');
                                    }
                                });
                            }
                        }

                        $('#tblBodyProductVendorPrice').empty();
                        mBaseUnit = GetStringObject('Unit.UnitMeasure', vData);
                        mBaseUnitQty = CDec2(GetStringObject('Unit.Quantity', vData));
                        if (mBaseUnitQty != 1)
                            mBaseUnit += ' (' + mBaseUnitQty + ')';
                        if (vData.VendorPrices.Rows.length == 0) {
                            $('#tblBodyProductVendorPrice').append('<tr><td colspan="7" style="text-align:center;">No Result(s) Found</td></tr>')
                        }
                        else {
                            count = 1;
                            for (var i = 0; i < vData.VendorPrices.Rows.length; i++) {
                                var isactive = CBool(vData.VendorPrices.Rows[i].IsActive)
                                var pUnitMeasure = mBaseUnit;
                                if (CStr2(vData.VendorPrices.Rows[i].UnitMeasure) != '') {
                                    pUnitMeasure = vData.VendorPrices.Rows[i].UnitMeasure;
                                    if ((CDec2(vData.VendorPrices.Rows[i].UnitQty) > 0)
                                        && (CDec2(vData.VendorPrices.Rows[i].UnitQty) != 1))
                                        pUnitMeasure += ' (' + CDec2(vData.VendorPrices.Rows[i].UnitQty) + ')';
                                }
                                var pPrice = CDec2(vData.VendorPrices.Rows[i].UnitPrice);
                                if (pPrice == 0)
                                    pPrice = CDec2(vData.VendorPrices.Rows[i].Price);
                                $('#tblBodyProductVendorPrice').append($('<tr style="cursor:pointer;">')
                                    .data('data', vData.VendorPrices.Rows[i])
                                    .attr('ID', vData.VendorPrices.Rows[i].ID)
                                    .append($('<td>')
                                        .text(count + '.')
                                    )
                                    .append($('<td>')
                                        .text(vData.VendorPrices.Rows[i].CompanyName)
                                    )
                                    .append($('<td name=tdPrice style="text-align:right">')
                                        .text(AccountingFormat(pPrice))
                                    )
                                    .append($('<td style="text-align:center">')
                                        .text(pUnitMeasure)
                                    )
                                    .append($('<td class="text-center">')
                                        .text(FormatDate(vData.VendorPrices.Rows[i].LastUpdated, {Format: 'MM/dd/yyyy hh:mm a'}))
                                    )
                                    .append($('<td class="text-center">')
                                        .text(CStr2(vData.VendorPrices.Rows[i].UpdatedByName))
                                    )
                                    .append($('<td class="text-center ' + (isGrantAccessToModifyPIM ? '' : 'd-none') + '">')
                                        .append($('<button data-role="none" class="btn ' + (isactive ? 'btn-danger' : 'btn-success') + '">').text(isactive ? 'Deactivate' : 'Activate')
                                            .data('data', vData.VendorPrices.Rows[i])
                                            .click(function (e) {
                                                e.stopPropagation();
                                                e.preventDefault();

                                                if (!isGrantAccessToModifyPIM) return;

                                                var data = $(this).data('data');
                                                NewMessageBox('Are you sure you want to ' + (CBool(data.IsActive) ? 'deactivate' : 'activate' ) + ' this Vendor Price?', 'Confirmation Message', [
                                                    {
                                                        Text: 'Yes', Click: function () {
                                                            AjaxSend({
                                                                url: '/Organization/SaveVendorPrice',
                                                                data: {
                                                                    VendorID: data.VendorID,
                                                                    Price: data.Price,
                                                                    UnitQty: data.UnitQty,
                                                                    ProductID: sessionStorage.ViewID,
                                                                    IsActive: (CBool(data.IsActive) ? 0 : 1)
                                                                }, success: function (e) {
                                                                    $('#popEditVendorPrice').modal('hide');
                                                                    if (e.Results == 'saved') {
                                                                        GetProductPriceList(sessionStorage.ViewID);
                                                                        WebToast('Vendor Price has beenn successfully deactivated');
                                                                    } else {
                                                                        NewMessageBox('There was an error while deactivating Vendor\'s price', 'Error');
                                                                    }
                                                                }
                                                            })
                                                        }
                                                    }, { Text: 'No' }
                                                ])
                                            })
                                        )
                                    )
                                    .data('data', vData.VendorPrices.Rows[i])
                                    .click(function (e) {
                                        if (!isGrantAccessToModifyPIM) return;
                                        var data = $(this).data('data');
                                        $('#lblVendorPriceHistory').text('Vendor Price History Of This Product From ' + data.CompanyName);
                                        GetVendorPriceHistory(data.VendorID, sessionStorage.ViewID);
                                        if ($(e.target).attr('name') != 'tdPrice')
                                            return;
                                        $('#popEditVendorPrice .modal-title').text('Edit Vendor Price');
                                        $('[name="txtVendorPriceName"]').val(data.CompanyName).unbind('click');
                                        $('[name="txtVendorPrice"]').val(CDec2(data.Price));
                                        $('[name="selVendorPriceURL"]').val(data.Url);
                                        $('[name="txtVendorPriceNote"]').val(data.Note);
                                        $('[name="txtUnitMeasure"]').val(mBaseUnit); 
                                        $('#popEditVendorPrice').modal('show');

                                        $('[name="butSaveVendorPrice"]').unbind('click').click(function () {
                                            AjaxSend({
                                                url: '/Organization/SaveVendorPrice',
                                                data: {
                                                    VendorID: data.VendorID,
                                                    Price: CDec2($('[name="txtVendorPrice"]').val()),
                                                    ProductID: sessionStorage.ViewID,
                                                    IsActive: 1,
                                                    Url: $('[name="selVendorPriceURL"]').val(),
                                                    Note: $('[name="txtVendorPriceNote"]').val(),
                                                }, success: function (e) {
                                                    $('#popEditVendorPrice').modal('hide');
                                                    if (e.Results == 'saved') {
                                                        GetProductPriceList(sessionStorage.ViewID);
                                                        GetVendorPriceHistory(data.VendorID, sessionStorage.ViewID);
                                                        WebToast('Vendor Price has beenn successfully updated');
                                                    } else {
                                                        NewMessageBox('There was an error while updating Vendor\'s price', 'Error');
                                                    }
                                                }
                                            })
                                        })
                                    })
                                )
                                count += 1;
                            }

                            if ($.fn.DataTable.isDataTable($('#tblBodyProductVendorPrice').closest('table'))) {
                                $('#tblBodyProductVendorPrice').closest('table').DataTable({
                                    initComplete: function () {
                                        $('#tblBodyProductVendorPrice').closest('table').removeClass('dataTable');
                                    }
                                });
                            }                            
                        }
                        GetProductContent(sessionStorage.ViewID);
                    }
                });
            }

            function GetVendorPriceHistory(vVendorID, vProductID) {
                $('#divVendorPriceHistory').hide();
                AjaxSend({
                    url: 'ERP/VendorPriceHistory',
                    data: {
                        VendorID: vVendorID,
                        ProductID: vProductID
                    },
                    success: function(vData) {
                        $('#divVendorPriceHistory').show();
                        $('#tblBodyVendorPriceHistory').empty();
                        if (vData.Result.Rows.length == 0) {
                            $('#tblBodyVendorPriceHistory').append('<tr><td colspan="7" style="text-align:center;">No Result(s) Found</td></tr>');
                        }
                        else {
                            for (var pIndex = 0; pIndex < vData.Result.Rows.length; pIndex++) {
                                var pDR = vData.Result.Rows[pIndex];
                                var pSourceOfPriceChange = CStr2(pDR.TransType);
                                if (pSourceOfPriceChange == 'PurchaseOrder')
                                    pSourceOfPriceChange = 'Purchase Order';
                                else if (pSourceOfPriceChange == 'PimVendorEdit')
                                    pSourceOfPriceChange = 'PIM Vendor Price Edit';
                                $('#tblBodyVendorPriceHistory').append($('<tr style="cursor:pointer;">')
                                    .data('data', pDR)
                                    .append($('<td>')
                                        .text((pIndex + 1) + '.')
                                    )
                                    .append($('<td>')
                                        .text(FormatDate(pDR.TransDate, { Format: 'MM/dd/yyyy hh:mm a' }))
                                    )
                                    .append($('<td style="text-align:right">')
                                        .text(AccountingFormat(pDR.CurrentPrice))
                                    )
                                    .append($('<td style="text-align:right">')
                                        .text(AccountingFormat(pDR.PreviousPrice))
                                    )
                                    .append($('<td style="text-align:right">')
                                        .text(AccountingFormat(pDR.NominalChange))
                                    )
                                    .append($('<td style="text-align:right">')
                                        .text(AccountingFormat(pDR.PercentChange) + '%')
                                    )
                                    .append($('<td style="text-align:center">')
                                        .text(pSourceOfPriceChange)
                                    )
                                );
                            }

                            if ($.fn.DataTable.isDataTable($('#tblBodyVendorPriceHistory').closest('table'))) {
                                $('#tblBodyVendorPriceHistory').closest('table').DataTable({
                                    initComplete: function () {
                                        $('#tblBodyVendorPriceHistory').closest('table').removeClass('dataTable');
                                    }
                                });
                            }
                        }
                    }
                }); // END AjaxSend({
            } // END function GetVendorPriceHistory(vVendorID, vProductID) {

            function LoadOrgProductPriceList(vRows) {
                var pLV = $('#lstOrgProductPrice');
                var pItem;

                $("#divOrgProductPriceRecordCount").text(" Number of record(s) found: " + vRows.length);

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li>')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();
                                //$.mobile.changePage('#pageSKUPrice');
                                $('#pageOrgProduct').hide();
                                $('#pageOrgProductInfo').hide();
                                $('#pageSKUMeasurement').hide();
                                $('#pageSKUPrice').show();
                                $('#pageBOMSKUInfo').hide();
                                $('#pageBrowseByOrgOwner').hide();
                                $('#pageBarcodeSearch').hide();
                                $('#pageCustomAdd').hide();
                                $('#pageProductNamePreview').hide();
                                ShowOrgProductPriceInfo($(this).attr('ID'));
                            })
                            .append($('<h3>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].PriceLevel + " - " + AccountingFormat(vRows[p].Price))
                            )
                        );
                    pLV.append(pItem);
                };

                pLV;//.listview('refresh');
            };

            function ShowOrgProductPriceInfo(strID) {
                var pData = {};
                pData.ID = strID;

                ClearOrgProductPriceFields();

                AjaxSend({
                    url: "Organization/getOrganizationProductPriceInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.OrgProductPriceData.Rows.length > 0) {
                            var vPrice = vData.OrgProductPriceData.Rows[0];
                            $("#txtRecProductPriceID").val(vPrice.ID || '');

                            $("#txtRecProductPriceLevelID").val(vPrice.PriceLevelID || '');
                            $("#txtRecProductUpcomingPriceID").val(vPrice.UpcomingPriceID || '');
                            $("#btnProductPriceLevel").text(vPrice.PriceLevel || 'Please Select');
                            $("#txtProductPriceLevel").val(vPrice.PriceLevel || '');
                            $("#txtOnlyProductPriceLevel").text(vPrice.PriceLevel || '');

                            $("#txtProductPrice").val(AccountingFormat(vPrice.Price || '0.00'));
                            $("#txtOnlyProductPrice").text(AccountingFormat(vPrice.Price || '0.00'));
                            $("#txtOnlyProductUpcomingPrice").text(AccountingFormat(vPrice.UpcomingPrice || '0.00') + ' ' + (vPrice.UpcomingPriceEffectivityDate == undefined ? '' : 'will take effect on ' + $.format.date(new Date(vPrice.UpcomingPriceEffectivityDate), 'MM-dd-yyyy')));

                            if (vPrice.UpcomingPrice == undefined) {
                                $('#chbSetUpcomingPrice').prop('checked', false);
                                $('#divUpcomingPrice').hide();
                            } else {
                                $('#chbSetUpcomingPrice').prop('checked', true);
                                $('#divUpcomingPrice').show();
                            }

                            $('#txtProductUpcomingPrice').val(vPrice.UpcomingPrice == undefined ? '' : CDec2(vPrice.UpcomingPrice));
                            $('#txtProductUpcomingPriceEffectivityDate').val((vPrice.UpcomingPriceEffectivityDate == undefined ? '' : $.format.date(new Date(vPrice.UpcomingPriceEffectivityDate), 'MMM dd, yyyy')));

                            var pGP = CDec2(vData.OrgProductPriceData.Rows[0].Price) - CDec2(vData.OrgProductPriceData.Rows[0].LandedCost);
                            $("#txtOnlyGrossProfit").text(AccountingFormat(pGP));
                            if (CDec2(vData.OrgProductPriceData.Rows[0].Price) != 0) {
                                var pGpm = pGP / CDec2(vData.OrgProductPriceData.Rows[0].Price);
                                $("#txtOnlyGrossProfitMargin").text(AccountingFormat(pGpm));
                            } else
                                $("#txtOnlyGrossProfitMargin").text('N/A');

                            $("#btnProductPriceRemove").hide();
                        }
                    }
                });

            };

            function ClearOrgProductPriceFields() {

                $("#txtRecProductPriceID").val('');

                $("#txtRecProductPriceLevelID").val('');
                $("#btnProductPriceLevel").text('Please Select');
                $("#txtProductPrice").val('0.00');

                $("#btnProductPriceRemove").hide();

            }

            function SaveOrgProductPrice() {
                var pData = {};

                pData.ID = $("#txtRecProductPriceID").val();
                pData.ProductID = $("#txtOrgProductID").val();
                pData.PriceLevelID = $("#txtRecProductPriceLevelID").val();
                pData.Price = $("#txtProductPrice").val();
                pData.IsEdit = isPriceEdit;
                pData.UpcomingPriceID = $("#txtRecProductUpcomingPriceID").val();

                if ($('#chbSetUpcomingPrice').is(':checked')) {
                    pData.UpcomingPrice = $('#txtProductUpcomingPrice').val();
                    pData.UpcomingPriceEffectivityDate = $('#txtProductUpcomingPriceEffectivityDate').val();
                } else {
                    pData.UpcomingPrice = '';
                    pData.UpcomingPriceEffectivityDate = '';
                }

                AjaxSend({
                    url: "Organization/SaveOrganizationProductPrice",
                    data: pData,
                    success: function (vData) {

                        //$.mobile.changePage('#pageOrgProductInfo');
                        $('#pageOrgProduct, #btnProductPriceSave').hide();
                        $('#divContainerMenuPage, #btnOrgProductEditProductPrices, #btnProductPriceBack').show();
                        $('#pageSKUMeasurement').hide();
                        $('#pageSKUPrice').hide();
                        $('#pageBOMSKUInfo').hide();
                        $('#pageBrowseByOrgOwner').hide();
                        $('#pageBarcodeSearch').hide();
                        $('#pageCustomAdd').hide();
                        $('#pageProductNamePreview').hide();
                        $('#btnProductPrices').click();
                        GetProductPriceList($("#txtOrgProductID").val());
                        $(window).scrollTop(0);
                    }
                });

            };

            function GetProductBOMData() {

                if ($.trim($("#txtOrgProductID").val()) != '') {
                    var pData = {};
                    pData.OrganizationID = sessionStorage["OrgID"];
                    pData.ProductID = $.trim($("#txtOrgProductID").val());
                    pData.ID = "";

                    AjaxSend({
                        url: 'Organization/GetOrganizationProductBOM',
                        data: pData,
                        success: function (vData) {

                            //Clear Existing BOM
                            //0 - is the cloned div
                            while ($("#divProductBOMContainer").find('[data-role=collapsible_clone]').length > 1) {
                                if ($("#divProductBOMContainer").find('[data-role=collapsible_clone]')[1] != undefined) {
                                    $("#divProductBOMContainer").find('[data-role=collapsible_clone]')[1].remove();
                                }
                            }

                            LoadProductBOM(null, vData);
                        }
                    }); // END Ajax
                }

            }; // END LoadData

            function LoadProductBOM(vParentDiv, vData, level) {
                var pDiv;
                var pIndex;

                if (vData.ProductBOM != undefined) {
                    if (vData.ProductBOM.Rows.length > 0) {

                        for (var p = 0; p <= vData.ProductBOM.Rows.length - 1; p++) {

                            pDiv = divProductBOM.clone();
                            pDiv.show();

                            pDiv.find('[data-role=collapsible_clone]')
                            .collapsible({
                                collapsed: (vData.ProductBOM.Rows[p].ChildCount <= 0),
                                contentTheme: $.mobile.page.prototype.options.contentTheme
                            });

                            if (vParentDiv != undefined) {
                                vParentDiv.append(pDiv);
                            } else {
                                divProductBOM.after(pDiv);
                            }

                            if (level == undefined)
                                level = 0;

                            pDiv.find('h3 span:first').find('.ui-btn-text:first').text(vData.ProductBOM.Rows[p].ComponentName);

                            //Edit button
                            var pButtonEditComponent;
                            pButtonEditComponent = $('<a data-role="button" data-icon="plus" data-inline="true" style="float: right; margin-right: -5px; text-align: right;">')
                            pButtonEditComponent.attr('id', 'btnEditProductBOM_' + vData.ProductBOM.Rows[p].ID);
                            pButtonEditComponent.data('data', vData.ProductBOM.Rows[p]);
                            pButtonEditComponent.text('Edit');
                            pDiv.find('h3 span:first').find('.ui-btn-text:first').append(pButtonEditComponent);

                            $('#btnEditProductBOM_' + vData.ProductBOM.Rows[p].ID).click(function (e) {
                                e.preventDefault();

                                mParentID = $(this).data('data').ParentBOMProductID;
                                mParentComponentName = ($(this).data('data').ParentBOMComponentName || $("#spanProductShortName").text());

                                ShowProductBOMInfo($(this).data('data'));
                                return;
                            });

                            pDiv.find('h3:first').on('click', '#btnEditProductBOM_' + vData.ProductBOM.Rows[p].ID, function (e) {
                                e.stopPropagation()
                            });

                            //add button
                            var pButtonAddComponent;
                            pButtonAddComponent = $('<a data-role="button" data-icon="plus" data-inline="true" style="float: right; margin-right: 5px; text-align: right;">')
                            pButtonAddComponent.attr('id', 'btnAddProductBOM_' + vData.ProductBOM.Rows[p].ID);
                            pButtonAddComponent.data('data', vData.ProductBOM.Rows[p]);
                            pButtonAddComponent.text('Add Sub-Component');
                            pDiv.find('h3 span:first').find('.ui-btn-text:first').append(pButtonAddComponent);

                            $('#btnAddProductBOM_' + vData.ProductBOM.Rows[p].ID).click(function (e) {
                                e.preventDefault();

                                mParentID = $(this).data('data').ID;
                                mParentComponentName = $(this).data('data').BOMProductName

                                ShowProductBOMInfo(null);
                                return;
                            });

                            pDiv.find('h3:first').on('click', '#btnAddProductBOM_' + vData.ProductBOM.Rows[p].ID, function (e) {
                                e.stopPropagation()
                            });

                            if (vData.SubProductBOM != undefined) {
                                if (vData.SubProductBOM[0].ProductBOM != undefined) {

                                    pDivGroup = pDiv.find('[name=divProductBOMSub]');
                                    for (pIndex = 0; pIndex <= vData.SubProductBOM[0].ProductBOM.Rows.length - 1; pIndex++) {
                                        if (vData.ProductBOM.Rows[p].ID == vData.SubProductBOM[0].ProductBOM.Rows[pIndex].ParentBOMProductID) {
                                            level = level + 1; //increment by the number of subgroups
                                            LoadProductBOM(pDivGroup, vData.SubProductBOM[0], level);
                                            break;
                                        }
                                    }; // END for pIndex
                                };
                            }

                            pDiv;//.trigger("create");

                        }

                    }

                } // end if vdata undefined


            }; // END LoadBranch

            function RemoveProductBOMData() {

                //$.mobile.changePage('#pageOrgProductInfo');
                $('#pageOrgProduct').hide();
                $('#pageOrgProductInfo').show();
                $('#pageSKUMeasurement').hide();
                $('#pageSKUPrice').hide();
                $('#pageBOMSKUInfo').hide();
                $('#pageBrowseByOrgOwner').hide();
                $('#pageBarcodeSearch').hide();
                $('#pageCustomAdd').hide();
                $('#pageProductNamePreview').hide();

            }; // END LoadData

            function ShowProductBOMInfo(vData) {

                //$.mobile.changePage('#pageBOMSKUInfo');
                $('#pageOrgProduct').hide();
                $('#pageOrgProductInfo').hide();
                $('#pageSKUMeasurement').hide();
                $('#pageSKUPrice').hide();
                $('#pageBOMSKUInfo').show();
                $('#pageBrowseByOrgOwner').hide();
                $('#pageBarcodeSearch').hide();
                $('#pageCustomAdd').hide();
                $('#pageProductNamePreview').hide();

                ClearOrgProducBOMtFields();

                if (vData != undefined) {

                    $("#btnProductBOMRemove").show();

                    getComponentMeasurement(vData.BOMProductID, vData.UnitMeasureID);

                    $("#txtRecBOMProductID").val(vData.ID);

                    $("#txtBOMComponentName").val(vData.ComponentName);
                    $("#txtBOMProductID").val(vData.BOMProductID);
                    $("#btnBOMProduct").text(vData.BOMProductName);//.button('refresh');

                    $("#txtBOMProductQty").val(vData.Qty);

                    $('#selUnitMeasure').val(vData.UnitMeasureID);//.selectmenu('refresh');

                    $("#selComponentMethod").val(vData.ComponentMethod);//.selectmenu('refresh');

                }

                $("#legendComponentMeasurement").text("How much " + $("#txtBOMComponentName").val() + " do you need to make " + mParentComponentName);
                $("#legendComponentMethod").text("How do you make " + $("#txtBOMComponentName").val() + ".");

            }; // END LoadProducts

            function ClearOrgProducBOMtFields() {

                $("#txtRecBOMProductID").val('');

                $("#txtBOMComponentName").val('');
                $("#txtBOMProductID").val('');
                $("#btnBOMProduct").text('Please Select');//.button('refresh');

                $("#txtBOMProductQty").val('1');
                $('#selUnitMeasure').val('');//.selectmenu('refresh');

                $("#selComponentMethod").val('');//.selectmenu('refresh');

                $("#btnProductBOMRemove").hide();

            };

            function getComponentMeasurement(vProductID, vValue) {
                var pData = {};
                pData.ProductID = vProductID;

                $('#selUnitMeasure').empty();

                AjaxSend({
                    url: 'Organization/GetOrganizationProductMeasurement',
                    data: pData,
                    success: function (vData) {
                        var vRows = vData.OrgProductData.Rows;

                        $('#selUnitMeasure').append(new Option('Please Select', ''));
                        for (var p = 0; p <= vRows.length - 1; p++) {
                            $('#selUnitMeasure').append(new Option(vRows[p].UnitMeasure, vRows[p].UnitMeasureID, false, false));
                        }

                        if (vValue != undefined) {
                            $('#selUnitMeasure').val(vValue);//.selectmenu('refresh');
                        }

                    },
                    error: function (vError) {
                        var pDummy;
                    }
                });
            }

            function GetAllOrgOwners() {
                var pData = {};
                pData.KeyWord = $("#txtSearchOrgOwner").val();

                AjaxSend({
                    url: 'Browser/LibraryOrganizationProductPublisher',
                    data: pData,
                    success: function (vData) {
                        DisplayOrgOwner(vData);
                    }
                });
            }

            function numberWithCommas(x) {
                return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            }

            function DisplayOrgOwner(vData) {
                var pLV = $('#lstOrgOwner');
                pLV.empty();
                var pItem;

                $("#divOrgOwnerSearchResults").hide();
                $("#divOrgOwnerSearchNoResults").hide();

                var vRows = vData.Results.Rows;

                if (vRows.length > 0) {
                    $("#divOrgOwnerSearchResults").show();
                    $("#divOrgOwnerRecordCount").text("Number of record(s) found: " + vRows.length);

                    for (var i = 0; i < vRows.length; i++) {

                        pItem = $('<a href="" class="list-group-item list-group-item-action">')
                        .attr('ID', vRows[i].ID)
                        .data('CompanyName', vRows[i].CompanyName)
                        .attr('href', '#')
                        .append($('<h5>')
                            .text(vRows[i].CompanyName)
                        )
                        .append($('<p>')
                            .text("No. of Products : " + numberWithCommas(vRows[i].NoOfProducts))
                        )
                        .append($('<p>')
                            .text("Last Updated on " + $.format.date(new Date(vRows[i].LastUpdated)))
                        )
                        .click(function () {
                            selectedOrgPublisherID = '';
                            selectedOrgPublisherName = '';

                            selectedOrgPublisherID = $(this).attr('ID');
                            selectedOrgPublisherName = $(this).data('CompanyName');

                            //$("#divExpressCustomPopup").popup("open");
                            PopupButtonList('', 'Option', [
                                {
                                    Text: 'Express (All Products)', Click: function () {
                                        $('#btnExpress').click();
                                    }
                                }, {
                                    Text: 'Custom (Selected Products)', Click: function () {
                                        $('#btnCustom').click();
                                    }
                                }
                            ])

                        })
                        pLV.append(pItem);
                    }

                    pLV;//.listview('refresh');

                }
                else {
                    $("#divOrgOwnerSearchNoResults").show();

                }

            }

            function AddOrgOwnerProductList(id) {
                var pData = {};
                pData.ID = id;

                AjaxSend({
                    url: 'Organization/AddOrgOwnerProductList',
                    data: pData,
                    success: function (vData) {
                        if (CDec2(vData.OrgData) > 0) {
                            NewMessageBox('Successfully added ' + CDec2(vData.OrgData) + ' new product/s to your company-wide product list. To change the prices of the products, please click Continue to download your product list.', 'Add to Company-Wide Product List', [
                                {
                                    Text: 'Continue', Click: function () {
                                        AjaxSend({
                                            url: 'Login3/TestCookies',
                                            success: function (vData) {
                                                window.pageSecuritySourcePage = $.mobile.activePage;
                                                PromptUserAccess('Product Price', {
                                                    OnClickOKSuccess: function () {
                                                        //window.open(GetPathFormat("/PIMProductPrice/UploadDownloadExcelFile") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                                                        window.location.hash = "";
                                                        window.location.hash = "UniversalPage";
                                                        sessionStorage["AddViewLibBCName"] = 'Bulk Price Adjustment';
                                                        LibraryTable.BulkPriceAdjustmentPage();
                                                    }
                                                });
                                            },
                                            error: function (vError) {
                                                NewMessageBox('This feature requires internet connection.', 'Requires Internet Connection');
                                            }

                                        });
                                    }
                                }, { Text: 'Cancel' }
                            ]);
                        } else {
                            NewMessageBox('No new product/s found.', 'Products');
                        }
                    }
                });
            }

            function SaveOrgProductPriceBreak() {
                
                var pData = {
                    ProductID: $("#txtOrgProductID").val(),
                    UnitMeasureID: mPriceBreakUnitID,
                    Qty: $('#txtPriceBreakQty').val(),
                    Discount: $('#txtPriceBreakDiscount').val(),
                    DiscountType: $('[name=radioPriceBreakDiscountType]:checked').val()
                };

                AjaxSend({
                    url: 'Organization/SaveOrganizationProductPriceBreak',
                    data: pData,
                    success: function(vData) {
                        if (vData == null) {
                            NewMessageBox('Unspecified server response');
                            return;
                        }
                        if (vData.Error) 
                            NewMessageBox('Data failed to save. ' + vData.Error);
                        else if (vData.Result == 'ok') {
                            WebToast('Price break was successfully saved.', 'top');
                            GetProductPriceBreakList($("#txtOrgProductID").val());
                            $('#butPriceBreakCancel').click();
                        } else
                            NewMessageBox(vData);
                    }
                }); // END AjaxSend({
            } // END function SaveOrgProductPriceBreak() {

            var mInventoryLedgerLoaded;

            //$('#btnPIMMenuInventoryLedger').click(function () {

            //    mProductName = $('#txtOnlyOrgProductName').text();

            //    if (mInventoryLedgerLoaded)
            //        return;
            //    mInventoryLedgerLoaded = true;

            //    sessionStorage.IsFromInventoryBalance = true;
            //    sessionStorage.InventoryBalanceProductID = sessionStorage.ViewID;
            //    sessionStorage.InventoryBalanceProductName = mProductName;
                
            //    AjaxSend({
            //            ExactUrl: '/InventoryLedger?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=1',
            //            type: 'GET',
            //            dataType: 'text',
            //            success: function (vData) {
            //                sessionStorage["Inventory"] = "ledger";
            //                $('#piminventoryledger').empty();
            //                $('#piminventoryledger').append(vData);//.trigger('create');

            //                if (isFinishedLoadIL) {
            //                    InventoryLedger.CheckSource();
            //                }

            //                isFinishedLoadIL = true;
            //            }
            //        });
            //});
        }); // end $(function () {
    });