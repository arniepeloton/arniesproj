<script>
    require([GetPathFormat('/Scripts/sources/Organization/OrganizationProduct.js?crc=' + assetversion),
                 GetPathFormat('/Scripts3/Browser/LibraryPIMProduct.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryPIMProductPublic.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryPIMProductMeasure.js?crc=' + assetversion),
                 GetPathFormat('/Scripts3/Browser/LibraryPIMMeasure.js?crc=' + assetversion),
                 GetPathFormat('/Scripts3/Browser/LibraryPIMPriceLevel.js?crc=' + assetversion),
                 GetPathFormat('/Scripts3/Browser/LibraryPIMMeasureLength.js?crc=' + assetversion),
                 GetPathFormat('/Scripts3/Browser/LibraryPIMMeasureWeight.js?crc=' + assetversion),
                 GetPathFormat('/Scripts/jquery.bxslider.min.js')
    ]);
</script>

<link rel="stylesheet" type="text/css" href="/Content/css/imgareaselect-default.css" />
<link rel="Stylesheet" type="text/css" href="/Content/css/jquery.bxslider.css" />
<script type="text/javascript" src="/Scripts/jquery.imgareaselect.min.js"></script>
<link rel="stylesheet" href="/Content/Ecommerce/css/lightbox.css">

<script>
    /* A function to compress images and convert it to a string */
    function imageCompressor(source_img_obj, quality, output_format) {
        var mime_type = "image/jpeg";
        if (typeof output_format !== "undefined" && output_format == "png") {
            mime_type = "image/png";
        }

        var cvs = document.createElement('canvas');
        cvs.width = source_img_obj.naturalWidth;
        cvs.height = source_img_obj.naturalHeight;
        var ctx = cvs.getContext("2d").drawImage(source_img_obj, 0, 0);
        var newImageData = cvs.toDataURL(output_format, quality / 100);
        var result_image_obj = new Image();
        result_image_obj.src = newImageData;
        return result_image_obj;

    }

    var readerResult

    /* Load the default image of the product */
    var readerResult = $.ajax({
        url: "/Organization/LoadImagePIM",
        data: JSON.stringify({ ViewID: sessionStorage.ViewID, conditionalOperator: "profileImg" }),
        type: 'POST',
        async: false,
        cache: false,
        contentType: false
    });

    /* Load all the images */
    readerResult.done(function (dataResult) {
        if (dataResult != "NotExist") {
            $('#finalImage').attr('src', dataResult);
        }
    });

    window.canvasArea = null;

    $('body')

        /* Checks which image is selected for processing */
        //.on('click', "#imgSelection img", function(){
        //    $('.selected').removeClass('selected');
        //    $(this).addClass('selected');
        //})

        /* Function for changing the default photo */
        .on('click', "#btnDefaultPhoto", function () {
            var imageName = $('.selected').attr('src');
            var readerResult = $.ajax({
                url: "/Organization/LoadImagePIM",
                data: JSON.stringify({ ViewID: sessionStorage.ViewID, conditionalOperator: "changing", imageData: imageName }),
                type: 'POST',
                async: false,
                cache: false,
                contentType: false
            }).done(function (dataResult) {
                if (dataResult == "DoneChange") {
                    $('#panUploadProductImagePopup').modal('hide');
                } else if (dataResult == "NotAllowed") {
                    alert("Error! File is already in default.");
                    $('#panUploadProductImagePopup').modal('hide');
                } else {
                    alert("Error! File you are trying to change does not exist.");
                    $('#panUploadProductImagePopup').modal('hide');
                }
            });
            $("#imgSelection").empty();
            location.reload();
        })

        .on('click', "#btnDeletePhoto", function () {
            var imageName = $('.selected').attr('src');
            var readerResult = $.ajax({
                url: "/Organization/LoadImagePIM",
                data: JSON.stringify({ ViewID: sessionStorage.ViewID, conditionalOperator: "deletion", imageData: imageName }),
                type: 'POST',
                async: false,
                cache: false,
                contentType: false
            });
            /* Process all the images and check for errors*/
            readerResult.done(function (dataResult) {
                if (dataResult == "DoneDeletion") {
                    //location.reload();
                } else {
                    alert("Error! File you are trying to delete does not exist.");
                    $('#panUploadProductImagePopup').modal('hide');
                }
            });
            $("#imgSelection").empty();
            //location.reload();
        });


    var output_format = "jpeg";

    /* Initialize coordinates to zero. */
    var x1 = 0, x2 = 0, y1 = 0, y2 = 0;

    /* Load the image and compress it */
    var openFile = function (event) {
        var input = event.target;
        var reader = new FileReader();
        var img = document.getElementById('canvasImage');
        var clippedImageH, clippedImageW;

        $(img).css({
            width: $('#contentImageCrop').innerWidth(),
        })

        reader.onload = function () {
            var dataURL = reader.result;
            var compressOutput = document.getElementById('compressImage');
            compressOutput.src = dataURL;
        };
        reader.readAsDataURL(input.files[0]);
        $('#panUploadProductImagePopup').modal('hide');
        var source_image = document.getElementById('compressImage');
        var result_image = document.getElementById('canvasImage');
        var thumbnail_image = document.getElementById('thumbnail');
        var final_image = document.getElementById('finalImage');
        var quality = parseInt(99);
        var canvasorig, cSmall, cMedium, cBig, resizedImage;
        output_format = input.files[0].type;
        source_image.onload = function () {
            result_image.src = imageCompressor(source_image, quality, output_format).src;
        }

        /* Selection and cropping begins here. */
        $(document).ready(function () {
            result_image.onload = function () {
                result_image.style.display = "block";
                $('#divCropPic').pageshow();
                $('.imgareaselect-outer').show();
                $('.imgareaselect-selection').show();
                $('.imgareaselect-border1').show();
                $('.imgareaselect-border2').show();
                $('.imgareaselect-border3').show();
                $('.imgareaselect-border4').show();
                $('#fileUploadProductImage').val('');

                if (canvasArea != undefined) {
                    $('#canvasImage').imgAreaSelect({ remove: true })
                    canvasArea.setOptions({ hide: false })
                    canvasArea.update();
                }

                canvasArea = $('#canvasImage').imgAreaSelect({
                    instance: true,
                    minWidth: 100,
                    minHeight: 100,
                    resizable: true,
                    handles: true,
                    persistent: true,
                    show: true,
                    aspectRatio: "1:1",
                    x1: 0, y1: 0, x2: 100, y2: 100,
                    onSelectEnd: function (img, selection) {
                        x1 = selection.x1;
                        y1 = selection.y1;
                        x2 = selection.x2// - selection.x1;
                        y2 = selection.y2// - selection.y1;
                        clippedImageH = selection.height;
                        clippedImageW = selection.width;
                    },
                });
            }
        });

        $("#btnCancelCrop, .btnPIMMenu").unbind('click').click(function () {

            /* Refresh current page to refresh the Cropping libraries */
            //location.reload();
            //$('#popPIMPictureGallery').hide();
            $('#canvasImage').imgAreaSelect({ remove: true })
            canvasArea.cancelSelection();
            canvasArea.setOptions({ hide: false })
            canvasArea.update();
        });

        $("#btnDoneCrop").unbind('click').click(function () {

            /* Saving the images */
            canvasorig = document.createElement('canvas') //document.getElementById('canvasorig');
            cSmall = document.createElement('canvas');
            cMedium = document.createElement('canvas');
            cBig = document.createElement('canvas');

            cSmall.width = "50";
            cSmall.height = "50";
            cMedium.width = "100";
            cMedium.height = "100";
            cBig.width = "400";
            cBig.height = "400";

            canvasorig.width = CDec2($(img)[0].width);
            canvasorig.height = CDec2($(img)[0].height);

            var wRatio = canvasorig.width / CDec2($(img)[0].naturalWidth);
            var hRatio = canvasorig.height / CDec2($(img)[0].naturalHeight);
            var ratio = Math.min(wRatio, hRatio);

            if (x1 == 0 && y1 == 0 && x2 == 0 && y2 == 0) {
                x1 = 120;
                y1 = 90;
                x2 = 220;
                y2 = 190;
                clippedImageH = 100;
                clippedImageW = 100;
            }

            canvasorig.getContext("2d").drawImage(img, 0, 0, CDec2($(img)[0].naturalWidth), CDec2($(img)[0].naturalHeight), 0, 0, Math.round(CDec2($(img)[0].naturalWidth) * ratio), Math.round(CDec2($(img)[0].naturalHeight) * ratio));

            resizedImage = document.getElementById("resizedImage");
            resizedImage.src = canvasorig.toDataURL(output_format, 1);
            //drawRotated();
            setTimeout(function () {
                //drawRotated();
                cSmall.getContext("2d").drawImage(resizedImage, x1, y1, clippedImageW, clippedImageH, 0, 0, 50, 50);
                cMedium.getContext("2d").drawImage(resizedImage, x1, y1, clippedImageW, clippedImageH, 0, 0, 100, 100);
                cBig.getContext("2d").drawImage(resizedImage, x1, y1, clippedImageW, clippedImageH, 0, 0, 400, 400);
                thumbnail_image.src = cSmall.toDataURL(output_format, 1);

                if ($('#imgSelection img').length == 0) { // display picture if there's no default
                    final_image.src = cBig.toDataURL(output_format, 1);
                }

                /* Send image src to save */
                var imageString = final_image.src.replace('data:' + output_format + ';base64,', '');
                var prefixID = NewID();
                //return
                SaveImagePIM(JSON.stringify({ State: 'new', PrefixID: prefixID, OrgID: sessionStorage.OrgID, ViewID: sessionStorage.ViewID, imageData: cBig.toDataURL(output_format, 1).replace('data:' + output_format + ';base64,', ''), Size: 'big' }), function () {
                    SaveImagePIM(JSON.stringify({ State: 'new', PrefixID: prefixID, OrgID: sessionStorage.OrgID, ViewID: sessionStorage.ViewID, imageData: cMedium.toDataURL(output_format, 1).replace('data:' + output_format + ';base64,', ''), Size: 'medium' }), function () {
                        SaveImagePIM(JSON.stringify({ State: 'new', PrefixID: prefixID, OrgID: sessionStorage.OrgID, ViewID: sessionStorage.ViewID, imageData: cSmall.toDataURL(output_format, 1).replace('data:' + output_format + ';base64,', ''), Size: 'small' }), function () {
                            SaveImagePIM(JSON.stringify({ State: 'new', PrefixID: prefixID, OrgID: sessionStorage.OrgID, ViewID: sessionStorage.ViewID, imageData: canvasorig.toDataURL(output_format, 1).replace('data:' + output_format + ';base64,', ''), Size: 'original' }), function () {
                                $("#btnCancelCrop").click();
                                WebToast('Successfully uploaded', 'top');
                                OrgProd.GetPIMPictures(sessionStorage.ViewID);
                            })
                        })
                    })
                })

                $('#fileUploadProductImage').val('');
            }, 500)

            /* Refresh current page to refresh the Cropping libraries */
            //location.reload();
        });
    }

    function SaveImagePIM(data, callback) {
        $.ajax({
            url: "/Organization/SaveImagePIM",
            data: data,
            type: 'POST',
            async: false,
            cache: false,
            processData: false,
            contentType: false,
            success: function (e) {
                var d = JSON.parse(e);
                if (d.d.Results == 'saved') {
                    callback()
                } else {
                    NewMessageBox(d.d.Results);
                }
            }
        });
    }

    window.SaveImagePIM = SaveImagePIM

</script>

<style type="text/css">
    #preview {
        position: absolute;
        border: 1px solid #ccc;
        background: #333;
        padding: 5px;
        display: none;
        color: #fff;
    }

    .staticeditmodeinputtext, .staticeditmodetextarea {
        visibility: hidden;
        display: none;
    }
</style>

<div id="divPIMLoading" subpage="divSettingsPIM" style="display:block;">
    <div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div><div>Please wait ...</div></div>
</div>

<div id="popBarcode" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Barcode</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <input data-role="none" class="form-control" type="text" id="txtBarcodeID" style="display:none;" />
                <div class="row mb-3">
                    <label class="col-sm-4 col-form-label" for="">Barcode: <span class="required">*</span></label>
                    <div class="col-sm-8">
                        <input type="text" id="txtBarcodeNumber" data-role="none" class="form-control" />
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-4 col-form-label" for="">Barcode Type:</label>
                    <div class="col-sm-8">
                        <select id="selBarcodeType" class="form-select" data-role="none"></select>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <button data-role="none" type="button" id="btnBarcodeSave" class="btn btn-success btn-block">Save</button>
                <button data-role="none" type="button" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>                
            </div>
        </div>
    </div>
</div>

<div id="pageOrgProduct" style="display:none;">
    <div data-role="header" data-position="fixed1" style="background: none white; border: none">
        <div class="ui-grid-b">
            <div class="ui-block-a">
                @*10%*@
                <div id="btnHome" style="margin: 0px; height: 55px;" data-role="button" data-theme="b" data-corners="false">
                    <span>Home</span>
                </div>
            </div>
            @*<div class="ui-block-b" style="width: 20%;">
                    <div id="btnSystemAdmin" style="margin: 0px; height: 55px;" data-role="button" data-theme="b" data-corners="false">
                        <span>System Administration</span>
                    </div>
                </div>
                <div class="ui-block-c" style="width: 20%;">
                    <div id="btnDataLibraries" style="margin: 0px; height: 55px;" data-role="button" data-theme="b" data-corners="false">
                        <span>Data Libraries</span>
                    </div>
                </div>*@
            <div class="ui-block-b">
                @*20%*@
                <div id="btnPIM" style="margin: 0px; height: 55px;" data-role="button" data-theme="b" data-corners="false">
                    <span>PIM</span>
                </div>
            </div>
            <div class="ui-block-c">
                @*30%*@
                <div id="btnPIMCompanyWide" style="margin: 0px; height: 55px;" data-role="button" data-theme="b" data-corners="false">
                    <span>Company Wide Product List</span>
                </div>
            </div>
        </div>

    </div>

    <div data-role="content">

        <div>
            <ul data-role="listview" id="lstOrgProductContainer" data-inset="true">
                <li data-role="list-divider">Your company's current product list</li>
                <li>
                    <form id="frmOrgProductSearch">
                        <div data-role="fieldcontain">
                            <label for="txtOrgProductSearch">Search Company Products: </label>
                            <input id="txtOrgProductSearch" type="search" value="" />
                        </div>
                    </form>
                    @*<div data-role="fieldcontain">This list is limited to show 50 SKU records.</div>*@
                    <div data-role="fieldcontain" id="divOrgProductRecordCount">
                        <label id="lblProductRecordcount"></label>
                        <a id="lnkProductList" style="cursor:pointer; font-size:16px;"> Click here to download Excel file to view full company product list.</a>
                    </div>
                    <div>
                        <ul data-role="listview" id="lstOrgProduct" data-inset="true"></ul>
                    </div>
                </li>
            </ul>
        </div>

        <div>
            <br />
            <div style="text-align: center">
                <a id="btnAddNewProducts" data-role="button"><h3>Add a Product</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgProductDone" data-role="button"><h3>Cancel</h3></a>
            </div>
            @*<div style="text-align: center">
                    <a id="btnOrgProductGoToSetupChecklist" data-role="button"><h3>Go to Setup Checklist</h3></a>
                </div>*@
        </div>

    </div>
    <div id="divAddProductPopup" data-role="popup">
        <ul data-role="listview" data-inset="true" style="min-width: 210px;">
            <li data-role="divider">Options</li>
            <li><a id="btnBarcodeSearch">Scan Barcode / Search Product</a></li>
            <li><a id="btnExcelFile">Import By Excel File</a></li>
            <li><a id="btnSubscribe">Subscribe to Company Product List</a></li>
        </ul>
    </div>
</div>

<div id="pageOrgProductInfoNew" subpage="divSettingsPIM" style="display:none;">
    <div id="popEditVendorPrice" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Edit Vendor Price</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        <label class="form-label">Vendor Name:</label>
                        <input data-role="none" type="text" name="txtVendorPriceName" class="form-control" readonly placeholder="Select Vendor" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label">New Price:</label>
                        <input data-role="none" type="number" name="txtVendorPrice" class="form-control" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label">Unit Measure:</label>
                        <select data-role="none" class="form-select" name="selVendorPriceUnitMeasure"></select>
                    </div>

                    <div class="mb-3">
                        <label class="form-label">URL:</label>
                        <input data-role="none" type="text" name="selVendorPriceURL" class="form-control" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label">Note:</label>
                        <textarea data-role="none" class="form-control" name="txtVendorPriceNote" style="max-height:300px; height:300px; resize:none;"></textarea>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col-6">
                            <button data-role="none" type="button" class="btn btn-secondary btn-lg btn-block" data-dismiss="modal">Cancel</button>
                        </div>
                        <div class="col-6">
                            <button data-role="none" type="button" name="butSaveVendorPrice" class="btn btn-success btn-lg btn-block">Save</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="panUploadProductImagePopup" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="divAddProductTitle">SKU's Pictures</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div id="divPIMPicPreview" class="bxslider">

                    </div>
                    <div id="divPIMPicPreviewThumbnails" style="overflow:auto;">

                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popupUpcharge" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Upcharge</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3"><b>Note:</b> If you don't want to apply upcharge to this SKU, just leave the field blank then click Save.</div>
                    <div class="row mb-3">
                        <label class="col-sm-4 col-form-label" for="">Enter Upcharge:</label>
                        <div class="col-sm-7">
                            <input type="number" id="txtUpcharge" data-role="none" class="form-control" />
                        </div>
                        <div class="col-sm-1 col-form-label">
                            %
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button data-role="none" type="button" id="btnSaveUpcharge" class="btn btn-success btn-block">Save</button>
                    <button data-role="none" type="button" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    <div class="row wrapper min-vh-100 flex-column flex-md-row">
        <div class="col-12 col-md-4 col-xl-2 p-0 bg-light flex-shrink-1 mb-3">
            <img id="compressImage" style="visibility:hidden; display:none;" />
            <img id="thumbnail" style="visibility:hidden; display:none;" />
            <input type="file" id="fileUploadProductImage" accept="image/*" style="visibility:hidden; display:none;" data-role="none" onchange='openFile(event)' />
            <a data-role="none" id="btnUploadProductImage" class="preview" style="text-align:center; height:100px; font-size:20px; cursor:pointer;">
                <img id="finalImage" class="img-thumbnail mx-auto d-block" width="200" src="/images/placeholder.png" />
            </a>

            <nav class="navbar navbar-expand-md navbar-light bg-light align-items-start flex-md-column flex-row p-2">
                <a data-role="none" class="navbar-brand" href="#">Menu</a>
                <a data-role="none" href="#" class="navbar-toggler" data-toggle="collapse" data-target=".sidebarPIM">
                    <span class="navbar-toggler-icon"></span>
                </a>
                <div class="collapse navbar-collapse sidebarPIM mt-2">
                    <ul class="flex-column w-100 justify-content-between nav nav-pills">
                        <li><a class="nav-link active" role="button" id="btnPIMMenuProductInformation" data-role="none" data-toggle="tab" href="#productinfopage">Product Information</a></li>
                        <li><a class="nav-link" role="button" id="btnPIMMenuUnitOfMeasures" data-role="none" data-toggle="tab" href="#unitofmeasurespage">Unit of Measures</a></li>
                        <li><a class="nav-link" role="button" id="btnPIMMenuProductPrices" data-role="none" data-toggle="tab" href="#productpricespage">Product Prices</a></li>
                        <li><a class="nav-link" role="button" id="btnPIMMenuBillOfMaterials" data-role="none" data-toggle="tab" href="#billofmaterialspage">Bill of Materials / Direct Labor Cost / Direct Overhead Cost</a></li>
                        <li><a class="nav-link" role="button" id="btnPIMMenuPictureGallery" data-role="none" data-toggle="tab" href="#pimpicturegallery">Picture Gallery</a></li>
                        <li><a class="nav-link" role="button" id="btnPIMMenuFileAttachments" data-role="none" data-toggle="tab" href="#pimfileattachments">File Attachments</a></li>
                        <li><a class="nav-link" role="button" id="btnPIMMenuInventoryLedger" data-role="none" data-toggle="tab" href="#piminventoryledger">Inventory Ledger</a></li>
                        <li><a class="nav-link" role="button" id="btnPIMMenuRelatedProducts" data-role="none" data-toggle="tab" href="#relatedproducts">Related Products/Services</a></li>
                        <li><a class="nav-link disabled" role="button" id="btnPIMMenuFowstLocation" data-role="none" data-toggle="tab" href="#notyetimplemented">FOWST Location Availability</a></li>
                        <li><a class="nav-link disabled" role="button" id="btnPIMMenuCustomerSKU" data-role="none" data-toggle="tab" href="#notyetimplemented">Customers of this SKU</a></li>
                    </ul>
                </div>
            </nav>
        </div>

        <div class="col bg-faded" style="font-size:18px;">
            <div id="divContainerMenuPage" style="display:block;">
                <div class="tab-content p-3">
                    <h1 id="txtOnlyOrgProductName" class="display-6 mb-4 statictext staticmodeinputtext"></h1>
                    <div class="tab-pane fade active show" id="productinfopage">
                        <div data-role="none" style="display:none;">
                            <input type="text" data-role="none" class="form-control" readonly id="txtOrgPIMProductID" />
                        </div>
                        <div class=" inputtext staticeditmodeinputtext">
                            <div class="row mb-3">
                                <label class="col-md-3 col-form-label" for="">Product Name:</label>
                                <div class="col-md">
                                    <input type="text" data-role="none" class="form-control" id="txtOrgProductName" maxlength="250" placeholder="Product Name" />
                                </div>
                            </div>
                        </div>
                        <div class="mb-3" id="btnProductBrowser">
                        </div>
                        <div style="display:none;"><input type="text" data-role="none" class="form-control" readonly id="txtOrgProductID" /></div>
                        <div class="mb-3 text-right">
                            <button id="btnOrgProductEdit" data-role="none" actionmode="edit" class="btn btn-success">
                                Edit Information
                            </button>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3" for="">Brand:</label>
                            <div class="col-md">
                                <span id="txtOnlyOrgProductBrand" class="statictext staticmodeinputtext"></span>
                                <div class="inputtext staticeditmodeinputtext">
                                    <div class="input-group">
                                        <input type="text" data-role="none" class="form-control" id="txtOrgProductBrand" />
                                        <button data-role="none" id="btnSearchProductBrand" class="btn btn-secondary">Search Brand</button>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3" for="">Product Number:</label>
                            <div class="col-md">
                                <span id="txtOnlyOrgProductNo" class="statictext staticmodeinputtext"></span>
                                <div class="inputtext staticeditmodeinputtext">
                                    <input type="text" data-role="none" class="form-control" id="txtOrgProductNo" maxlength="50" />
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3" for="">Product Group No.:</label>
                            <div class="col-md">
                                <span id="txtOnlyOrgProductGroupNo" class="statictext staticmodeinputtext"></span>
                                <div class="inputtext staticeditmodeinputtext">
                                    <div class="input-group">
                                        <input type="text" data-role="none" class="form-control" id="txtOrgProductGroupNo" maxlength="50" />
                                        <button data-role="none" id="btnSearchProductGroupNo" class="btn btn-secondary">Search Group No.</button>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3" for="">Division:</label>
                            <div class="col-md">
                                <span id="txtOnlyOrgProductDivision" class="statictext staticmodeinputtext"></span>
                                <div class="inputtext staticeditmodeinputtext">
                                    <input type="text" data-role="none" class="form-control" id="txtOrgProductDivision" maxlength="50" />
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3" for="">Category:</label>
                            <div class="col-md">
                                <span id="txtOnlyOrgProductCategory" class="statictext staticmodeinputtext"></span>
                                <div class="inputtext staticeditmodeinputtext">
                                    <div class="input-group">
                                        <input type="text" data-role="none" class="form-control" id="txtOrgProductCategory" />
                                        <button data-role="none" id="btnSearchProductCategory" class="btn btn-secondary">Search Category</button>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3" for="">Marketing Text:</label>
                            <div class="col-md">
                                <span id="txtOnlyOrgProductMaketingText" class="statictext staticmodeinputtext"></span>
                                <div class="inputtext staticeditmodeinputtext">
                                    <input type="text" data-role="none" class="form-control" id="txtOrgProductMaketingText" maxlength="50" />
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3" for="">Warranty:</label>
                            <div class="col-md">
                                <span id="txtOnlyOrgProductWarranty" class="statictext staticmodeinputtext"></span>
                                <div class="inputtext staticeditmodeinputtext">
                                    <input type="text" data-role="none" class="form-control" id="txtOrgProductWarranty" maxlength="50" />
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3" for="">Line Item Payment Term:</label>
                            <div class="col-md">
                                <span id="txtOnlyOrgProductLineItemPaymentTerm" class="statictext staticmodeinputtext"></span>
                                <div class="inputtext staticeditmodeinputtext">
                                    <div class="input-group">
                                        <input type="text" data-role="none" class="form-control" id="txtOrgProductLineItemPaymentTerm" />
                                        <button data-role="none" type="button" id="butProductLineItemPaymentTerm" class="btn btn-secondary">Search Term</button>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3" for="">Published By:</label>
                            <div class="col-md">
                                <span id="txtOnlyOrgProductPublishedBy" class="statictext staticmodeinputtext"></span>
                                <div class="inputtext staticeditmodeinputtext">
                                    <input type="text" data-role="none" class="form-control" id="txtOrgProductPublishedBy" readonly maxlength="50" />
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3" for="">Uploaded By:</label>
                            <div class="col-md">
                                <span id="txtOnlyOrgProductUploadedBy" class="statictext staticmodeinputtext"></span>
                                <div class="inputtext staticeditmodeinputtext">
                                    <input type="text" data-role="none" class="form-control" id="txtOrgProductUploadedBy" readonly maxlength="50" />
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3" for="">Do not share transaction data with the brand owner?</label>
                            <div class="col-md">
                                <span id="txtOrgProductDoNotShare" class="statictext staticmodeinputtext"></span>
                                <div class="inputtext staticeditmodeinputtext">
                                    <div class="form-check form-switch mb-3" style="font-size:20px;">
                                        <input class="form-check-input" data-role="none" type="checkbox" id="selOrgProductDoNotShareData">
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3" for="">Eligible for Senior Citizens' (SC) and Persons with Disability (PWD) Discount?</label>
                            <div class="col-md">
                                <span id="txtOrgVatExempt" class="statictext staticmodeinputtext"></span>
                                <div class="inputtext staticeditmodeinputtext">
                                    <div class="form-check form-switch mb-3" style="font-size:20px;">
                                        <input class="form-check-input" data-role="none" type="checkbox" id="selOrgVatExempt">
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3" for="">Max Allowable Discountable Amount:</label>
                            <div class="col-md">
                                <span id="txtOnlyMaxAllowableDiscountableAmount" class="statictext staticmodeinputtext"></span>
                                <div class="inputtext staticeditmodeinputtext">
                                    <div class="row align-items-center">
                                        <div class="col-md-auto">
                                            <div class="form-check form-check-inline">
                                                <input class="form-check-input" type="radio" name="chbMaxOption" id="chbMaxCustomAmount" checked value="custom">
                                                <label class="form-check-label" for="inlineCheckbox1">Enter custom amount</label>
                                            </div>
                                            <div class="form-check form-check-inline">
                                                <input class="form-check-input" type="radio" name="chbMaxOption" id="chbMaxReferenceSKUAmount" value="reference">
                                                <label class="form-check-label" for="inlineCheckbox2">SKU Reference</label>
                                            </div>
                                        </div>
                                        <div class="col-md">
                                            <input type="number" data-role="none" class="form-control" placeholder="Enter custom amount here" id="txtOrgMaxAllowableDiscountableAmount" />
                                            <div class="input-group" id="divMaxReferenceSKU" style="display:none;">
                                                <input data-role="none" type="text" class="form-control" readonly id="txtMaxReferenceSKUName" />
                                                <button data-role="none" class="btn btn-secondary" id="btnMaxReferenceSKUSearch">Search Product</button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3" for="">When using SC/PWD discount, how do you want to display and compute the final discounted price of the SKU?</label>
                            <div class="col-md">
                                <span id="txtOnlySCPWDDiscountedPriceAction" class="statictext staticmodeinputtext"></span>
                                <div class="inputtext staticeditmodeinputtext">
                                    <div class="mb-3">Eg: SKU's discounted price: 23.50</div>

                                    <div class="form-check">
                                        <input data-role="none" class="form-check-input" type="radio" name="radRoundOptions" id="radExactPrice" checked value="Exact Price">
                                        <label class="form-check-label" for="radExactPrice">Do not round up/down, apply the exact price (23.50)</label>
                                    </div>

                                    <div class="form-check">
                                        <input data-role="none" class="form-check-input" type="radio" name="radRoundOptions" id="radRoundDown" value="Round Down">
                                        <label class="form-check-label" for="radRoundDown">Round Down (23)</label>
                                    </div>

                                    <div class="form-check">
                                        <input data-role="none" class="form-check-input" type="radio" name="radRoundOptions" id="radRoundUp" value="Round Up">
                                        <label class="form-check-label" for="radRoundUp">Round Up (24)</label>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3" for="">Activation Date:</label>
                            <div class="col-md">
                                <span id="txtOrgProductActivationDate" class="statictext staticmodeinputtext"></span>
                                <div class="inputtext staticeditmodeinputtext">
                                    <div class="input-group">
                                        <select data-role="none" class="form-select" id="selOrgProductActivationDateMonth">
                                            <option value="">MMM</option>
                                        </select>
                                        <select data-role="none" class="form-select" id="selOrgProductActivationDateDay">
                                            <option value="">DD</option>
                                        </select>
                                        <select data-role="none" class="form-select" id="selOrgProductActivationDateYear">
                                            <option value="">YYYY</option>
                                        </select>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3" for="">Deactivate Record:</label>
                            <div class="col-md">
                                <span id="txtOrgProductDeactivateRecord" class="statictext staticmodeinputtext"></span>
                                <div class="inputtext staticeditmodeinputtext">
                                    <div class="form-check form-switch mb-3" style="font-size:20px;">
                                        <input class="form-check-input" data-role="none" type="checkbox" id="selOrgProductDeactivate">
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3 divOrgProductDeactivateDetails" style="display:none;">
                            <label class="col-md-3" for="">Deactivation Date:</label>
                            <div class="col-md">
                                <span id="txtOrgProductDeactivateDate" class="statictext staticmodeinputtext"></span>
                                <div class="inputtext staticeditmodeinputtext">
                                    <div class="row">
                                        <div class="col-md-3">
                                            <select data-role="none" class="form-select" id="selOrgProductDateInActiveMonth">
                                                <option value="">MMM</option>
                                            </select>
                                        </div>
                                        <div class="col-md-3">
                                            <select data-role="none" class="form-select" id="selOrgProductDateInActiveDay">
                                                <option value="">DD</option>
                                            </select>
                                        </div>
                                        <div class="col-md-3">
                                            <select data-role="none" class="form-select" id="selOrgProductDateInActiveYear">
                                                <option value="">YYYY</option>
                                            </select>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3 divOrgProductDeactivateDetails" style="display:none;">
                            <label class="col-md-3" for="">Reason for Deactivating:</label>
                            <div class="col-md">
                                <span id="txtOnlyOrgProductInActiveReason" class="statictext staticmodeinputtext"></span>
                                <div class="inputtext staticeditmodeinputtext">
                                    <textarea id="txtOrgProductInActiveReason" data-role="none" class="form-control" style="height:100px; resize:none;"></textarea>
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3 divOrgProductDeactivateDetails" style="display:none;">
                            <label class="col-md-3" for="">Comments:</label>
                            <div class="col-md">
                                <span id="txtOnlyOrgProductComments" class="statictext staticmodeinputtext"></span>
                                <div class="inputtext staticeditmodeinputtext">
                                    <textarea id="txtOrgProductComments" data-role="none" class="form-control" style="height:100px; resize:none;"></textarea>
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3 divOrgProductDeactivateDetails" style="display:none;">
                            <label class="col-md-3" for="">Ban this record for future transactions?</label>
                            <div class="col-md">
                                <span id="txtOrgProductBan" class="statictext staticmodeinputtext"></span>
                                <div class="inputtext staticeditmodeinputtext">
                                    <div class="form-check form-switch mb-3" style="font-size:20px;">
                                        <input class="form-check-input" data-role="none" type="checkbox" id="selOrgProductBan">
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3 divProductAvailableToMobile">
                            <label class="col-md-3" for="">Make Available to Mobile DB:</label>
                            <div class="col-md">
                                <span id="txtOrgProductAvailableToMobile" class="statictext staticmodeinputtext"></span>
                                <div class="inputtext staticeditmodeinputtext">
                                    <div class="form-check form-switch mb-3" style="font-size:20px;">
                                        <input class="form-check-input" data-role="none" type="checkbox" id="selOrgProductAvailableToMobile">
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3 divProductAvailableToMobile">
                            <label class="col-md-3" for="">Tag as rentable asset:</label>
                            <div class="col-md">
                                <span id="txtOnlyOrgProductIsAsset" class="statictext staticmodeinputtext"></span>
                                <div class="inputtext staticeditmodeinputtext">
                                    <div class="form-check form-switch mb-3" style="font-size:20px;">
                                        <input class="form-check-input" data-role="none" type="checkbox" id="selProdIsAsset">
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3 divProductAvailableToMobile">
                            <label class="col-md-3" for="">Tag as promo item:</label>
                            <div class="col-md">
                                <span id="txtOnlyOrgProductIsPromoItem" class="statictext staticmodeinputtext"></span>
                                <div class="inputtext staticeditmodeinputtext">
                                    <div class="form-check form-switch mb-3" style="font-size:20px;">
                                        <input class="form-check-input" data-role="none" type="checkbox" id="selProdIsPromoItem">
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3 divProductAvailableToMobile">
                            <label class="col-md-3" for="">This is an inventory-count item?</label>
                            <div class="col-md">
                                <span id="txtOnlyOrgProductIsInventoryCountItem" class="statictext staticmodeinputtext"></span>
                                <div class="inputtext staticeditmodeinputtext">
                                    <div class="form-check form-switch mb-3" style="font-size:20px;">
                                        <input class="form-check-input" data-role="none" type="checkbox" id="selProdIsInventoryCountItem" checked>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3" for="">Allow vendor to sell this SKU? If No, only E-Commerce Platform owner can sell</label>
                            <div class="col-md">
                                <span id="txtOnlyOrgProductSellInEcommerce" class="statictext staticmodeinputtext"></span>
                                <div class="inputtext staticeditmodeinputtext">
                                    <div class="form-check form-switch mb-3" style="font-size:20px;">
                                        <input class="form-check-input" data-role="none" type="checkbox" id="selProdSellInEcommerce">
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="mt-4 text-center">
                            <a id="btnOrgProductInfoCancel" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
                            <a id="btnOrgProductInfoSave" data-role="none" class="btn btn-success btn-lg">Save</a>
                            <a id="btnOrgProductInfoSaveAs" data-role="none" class="btn btn-success btn-lg">Save As</a>
                        </div>
                    </div>

                    <div class="tab-pane fade" id="unitofmeasurespage">
                        <div class="mb-3 text-right">
                            <button id="btnOrgProductMeasurementAdd" data-role="none" class="btn btn-success">
                                Add Measurement
                            </button>
                        </div>

                        <div style="width:100%; overflow:auto">
                            <table data-role="none" class="table table-striped table-hover table-bordered">
                                <thead>
                                    <tr>
                                        <th rowspan="2" style="text-align:center; vertical-align: middle;">#</th>
                                        <th rowspan="2" style="text-align:center; vertical-align: middle;">Packing Size Unit</th>
                                        <th colspan="2" style="text-align:center; vertical-align: middle;">
                                            Packing Size Content<br />
                                            (Stated In Base Unit Of Measure)
                                        </th>
                                        <th rowspan="2" style="text-align:center; vertical-align: middle;">Packing Size's Volume Discount</th>
                                        <th colspan="6" style="text-align:center; vertical-align: middle;">
                                            Packing Size Content<br />
                                            (Stated In Different Unit Of Measure)
                                        </th>
                                    </tr>
                                    <tr>
                                        <th style="text-align:center; vertical-align: middle;">Quantity</th>
                                        <th style="text-align:center; vertical-align: middle;">Unit Measure</th>
                                        <th style="text-align:center; vertical-align: middle;">Quantity</th>
                                        <th style="text-align:center; vertical-align: middle;">Unit Measure</th>
                                        <th style="text-align:center; vertical-align: middle;">Length</th>
                                        <th style="text-align:center; vertical-align: middle;">Width</th>
                                        <th style="text-align:center; vertical-align: middle;">Height</th>
                                        <th style="text-align:center; vertical-align: middle;">Weight</th>
                                    </tr>
                                    @*<tr class="custom-theme-f">
                            <th>#</th>
                            <th>Quantity</th>
                            <th>Unit Measure</th>
                            <th>Volume Discount</th>
                            <td>Content</td>
                        </tr>*@

                                </thead>
                                <tbody id="tblBodyMeaurement"></tbody>
                            </table>
                        </div>
                        <br /><br />

                        <table>
                            <tr>
                                <td>Add upcharge for purchase quantities below the base unit qty:</td>
                                <td><div style="display:flex; align-items:center;"><input data-role="none" type="text" readonly class="form-control" id="txtEncodedUpcharge" style="width:100px;" /> %</div></td>
                                <td><a data-role="none" id="btnEditUpcharge" class="custom-link">Edit</a></td>
                            </tr>
                        </table>

                        <br /><br />

                        <div class="row mb-3 align-items-center">
                            <div class="col-sm-6">
                                <h3>Bulk Discounting</h3>
                            </div>
                            <div class="col-sm-6 text-right">
                                <button id="btnOrgPriceBreakAdd" data-role="none" class="btn btn-success">
                                    Add Price Break
                                </button>
                            </div>
                        </div>

                        <div style="width:100%; overflow:auto;">
                            <table data-role="none" class="table table-striped table-hover table-bordered">
                                <thead>
                                    <tr>
                                        <th>#</th>
                                        <th>Quantity</th>
                                        <th>Unit Measure</th>
                                        <th>Volume Discount</th>
                                        <td>Content</td>
                                    </tr>
                                </thead>
                                <tbody id="tblBodyPriceBreak"></tbody>
                            </table>
                        </div>
                        @*<div data-role="fieldcontain"><a id="btnOrgProductMeasurementAdd" data-role="button" data-icon="plus" data-inline="true">Add Measurement</a></div>
            <div data-role="fieldcontain" id="divOrgProductMeasurementRecordCount"></div>
            <div>
                <ul data-role="listview" id="lstOrgProductMeasurement" data-inset="true">
                </ul>
            </div>*@
                    </div>

                    <div class="tab-pane fade" id="productpricespage">
                        <div class="mb-3 text-right">
                            <button id="btnOrgProductPriceLevelAdd" data-role="none" class="btn btn-success">
                                Add Product Price
                            </button>
                        </div>

                        <div style="width:100%;" class="mb-5">
                            <h3>Book Value Cost and Selling Prices</h3>
                            <div class="table-responsive">
                                <table data-role="none" class="table table-light table-striped table-hover">
                                    <thead>
                                        <tr class="text-center align-top">
                                            <th>#</th>
                                            <th>Price Level</th>
                                            <th>Price</th>
                                            <th>Gross Profit</th>
                                            <th>Gross Profit Margin</th>
                                            <th>Upcoming Price</th>
                                            <th>Last Updated By</th>
                                            <th>Last Update Date</th>
                                            <th></th>
                                        </tr>
                                    </thead>
                                    <tbody id="tblBodyProductPrice"></tbody>
                                </table>
                            </div>
                        </div>

                        <div style="width:100%;">
                            <h3>Vendor Prices</h3>
                            <div class="row align-items-center mb-3">
                                <div class="col-auto col-md-auto">
                                    Show:
                                </div>
                                <div class="col-auto col-md-auto">
                                    <select id="selVendorPriceStatus" data-role="none" class="form-select fixed">
                                        <option value="1">Active</option>
                                        <option value="0">Inactive</option>
                                    </select>
                                </div>
                                <div class="col col-md text-right">
                                    <button data-role="none" id="btnAddVendorPrice" class="btn btn-success">Add Vendor Price</button>
                                </div>
                            </div>

                            <div class="table-responsive">
                                <table data-role="none" class="table table-light table-striped table-hover">
                                    <thead>
                                        <tr class="text-center align-top">
                                            <th>#</th>
                                            <th>Vendor</th>
                                            <th>Price</th>
                                            <th>UoM</th>
                                            <th>Last Update Date</th>
                                            <th>Modified By</th>
                                            <th></th>
                                        </tr>
                                    </thead>
                                    <tbody id="tblBodyProductVendorPrice"></tbody>
                                </table>
                            </div>

                            <div id="divVendorPriceHistory" class="table-responsive" style="display:none;margin-top:40px">
                                <h4 id="lblVendorPriceHistory">Vendor Price History for </h4>
                                <table data-role="none" class="table table-light table-striped table-hover">
                                    <thead>
                                        <tr class="text-center align-top">
                                            <th>#</th>
                                            <th>Transaction Date</th>
                                            <th><b>As of Date Price</b></th>
                                            <th>Previous Price</th>
                                            <th>Nominal Change</th>
                                            <th>Percent Change</th>
                                            <th>Source of Price Change</th>
                                        </tr>
                                    </thead>
                                    <tbody id="tblBodyVendorPriceHistory"></tbody>
                                </table>
                            </div>
                        </div>

                        @*<div data-role="fieldcontain"><a id="btnOrgProductPriceLevelAdd" data-role="button" data-icon="plus" data-inline="true">Add Product Price</a></div>
            <div data-role="fieldcontain" id="divOrgProductPriceRecordCount"></div>
            <div>
                <ul data-role="listview" id="lstOrgProductPrice" data-inset="true">
                </ul>
            </div>*@
                    </div>

                    <div class="tab-pane fade" id="billofmaterialspage">
                        <div class="row mb-4 align-items-center">
                            <div class="col-6">
                                <h5>Bill of Materials (BoM) / Direct Labor Cost / Direct Overhead Cost</h5>
                            </div>
                            <div class="col-6 text-right">
                                <button id="btnOrgProductBOMAdd" data-role="none" class="btn btn-success">Add Raw Materials</button>
                            </div>
                        </div>

                        <div class="mb-3" style="width:100%; overflow:auto;">
                            <table data-role="none" class="table table-striped table-hover">
                                <thead>
                                    <tr>
                                        <th>#</th>
                                        <th>Product No.</th>
                                        <th>Product Name</th>
                                        <th>Quantity</th>
                                        <th>Unit Measure</th>
                                        <th>Unit Factory Price</th>
                                        <th>Total Factory Price</th>
                                        <th>Unit Freight Cost</th>
                                        <th>Total Freight Cost</th>
                                        <th>Unit Landed Cost</th>
                                        <th>Total Landed Cost</th>
                                        <th></th>
                                    </tr>
                                </thead>
                                <tbody id="tblBodyBOM"></tbody>
                                <tfooter>
                                    <tr class="custom-theme-f">
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th id="thBomTotalFactoryPrice"></th>
                                        <th></th>
                                        <th id="thBomTotalFreightCost"></th>
                                        <th></th>
                                        <th id="thBomTotalLandedCost"></th>
                                        <th></th>
                                    </tr>
                                </tfooter>
                            </table>
                        </div>

                        <div>
                            <h5 class="mb-3">Production Type</h5>
                            <div class="form-check mb-3">
                                <input data-role="none" class="form-check-input" type="radio" name="optJit" id="jit" value="1">
                                <label class="form-check-label" for="jit">
                                    <b>Just-In-Time (JIT) Production</b>
                                    <span style="font-size: smaller; font-weight: normal">
                                        <br />
                                        This product is assembled or produced only immediately after a Point-of-Sale (POS) entry has been made.
                                        <br />
                                        Examples of products done in <b>JIT</b> fashion are short order food items such as spaghetti, noodle soup, soft drinks dispensed via soda fountain, and other products made at the last possible minute.
                                    </span>
                                </label>
                            </div>

                            <div class="form-check mb-3">
                                <input data-role="none" class="form-check-input" type="radio" name="optJit" id="bp" value="0">
                                <label class="form-check-label" for="bp">
                                    <b>Batch Production (BP)</b>
                                    <span style="font-size: smaller; font-weight: normal">
                                        <br />
                                        This product is produced even before being recorded in the Point-of-Sale (POS) due to lengthy preparation times.  Production of this product is done in advance of a sale. Also, production activity of this product is recorded through the Mobiz Product Maker module.
                                        <br />
                                        Examples of products done in <b>BP</b> fashion are cakes, pastries, breads, sauces, and other products that need to be prepared in anticipation of being sold at a later time.
                                    </span>
                                </label>
                            </div>

                            <div id="divActionButtonForJit" class="mt-4 mb-4 text-center" style="display:none">
                                <a id="btnOrgProdCancelChangesBom" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
                                <a id="btnOrgProdSaveChangesBom" data-role="none" class="btn btn-success btn-lg">Save Changes</a>
                            </div>

                            <h5 class="mb-3">Production Instructions: (<a id="btnOrgProductBOMInstructionEdit" actionmode="edit" class="custom-link"> Edit </a>)</h5>
                            <pre id="txtBomInstruction" style="font-family:Helvetica, Arial, sans-serif; border:1px solid #D3D800; background-color:#FFFFD0; border-radius:10px; width:98%; min-height:80px; padding:5px;" class="statictextarea staticmodetextarea">
                            
                        </pre>
                            <div class="inputtextarea staticeditmodetextarea">
                                <textarea id="txtareaBomInstruction" data-role="none" class="form-control" style="min-height:90px;"></textarea>
                                <div class="mt-4 mb-4 text-center">
                                    <a id="btnOrgProdCancelBomInstruction" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
                                    <a id="btnOrgProdSaveBomInstruction" data-role="none" class="btn btn-success btn-lg">Save Instructions</a>
                                </div>
                            </div>
                        </div>

                        <div>
                            <h5 class="mb-3">Sales Returns Behavior</h5>

                            <div class="row mb-3">
                                <label class="col-md" for="selSeperateIntoComponentDuringSalesReturns">Break down this Finished Good (FG) into its Bill of Material (BOM) components upon sales return.</label>
                                <div class="col-md-3">
                                    <div class="form-check form-switch mb-3" style="font-size:20px;">
                                        <input class="form-check-input" data-role="none" type="checkbox" id="selSeperateIntoComponentDuringSalesReturns">
                                    </div>
                                </div>
                            </div>

                            <div id="divActionButtonForComponentsDuringSalesReturns" class="mt-4 mb-4 text-center" style="display:none; text-align:right;">
                                <a id="btnOrgProdCancelSeperateIntoComponentDuringSalesReturns" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
                                <a id="btnOrgProdSaveSeperateIntoComponentDuringSalesReturns" data-role="none" class="btn btn-success btn-lg">Save Sales Return Behavior</a>
                            </div>
                        </div>
                    </div>

                    <div class="tab-pane fade" id="pimpicturegallery">
                        <div id="divMainPicGallery" subpage="pimpicturegallery" style="display:block;">
                            <div class="row mb-3">
                                <div class="col-sm-6">
                                    <h5>Picture Gallery</h5>
                                </div>
                                <div class="col-sm-6 text-right">
                                    <button data-role="none" id="btnChoosePicture" class="btn btn-success">Add Picture</button>
                                </div>
                            </div>

                            <div class="mb-3"><i><b>Note:</b> Maximum of 5 pictures only.</i></div>

                            <div id="imgSelection">

                            </div>
                        </div>

                        <div id="divCropPic" subpage="pimpicturegallery" style="display:none;">
                            <span style="font-size: 30px; font-weight: bold;">Please crop the picture</span>
                            <br /><br />
                            <img id="resizedImage" style="display:none;" />
                            <img id="canvasImage" style="max-width:50%; margin-left: auto; margin-right: auto;" />
                            <div class="mt-4 mb-4 text-center">
                                <a id="btnCancelCrop" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
                                <a id="btnDoneCrop" data-role="none" class="btn btn-success btn-lg">Save</a>
                            </div>
                        </div>
                    </div>

                    <div class="tab-pane fade" id="piminventoryledger">

                    </div>

                    <div class="tab-pane fade" id="relatedproducts">

                    </div>

                    <div class="tab-pane fade" id="notyetimplemented">
                        <h2>Not yet implemented.</h2>
                    </div>

                    <div class="tab-pane fade" id="pimfileattachments">
                        <input type="file" id="filePimAttachments" style="visibility:hidden; display:none;" data-role="none" multiple />
                        <div class="row mb-3">
                            <div class="col">
                                <h5>File Attachments</h5>
                            </div>
                            <div class="col text-right">
                                <button data-role="none" id="btnPIMAddFiles" onclick="document.getElementById('filePimAttachments').click()" class="btn btn-success">Add Files</button>
                            </div>
                        </div>

                        <ul class="list-group" id="listPimFileAttachments"></ul>
                    </div>
                </div>
            </div>

            <div id="pageSKUMeasurement" class="p-3" style="display:none;">
                @*/SubPages/OrganizationProduct/UnitOfMeasure.html*@
            </div>

            <div id="pageSKUPrice" class="p-3" style="display:none;">

            </div>
        </div>
    </div>

    <table style="width:100%; display:none;">
        <tr>
            <td style="width:300px" class="align-top pr-3">
                <div>
                    

                    <ul id="" class="nav nav-pills flex-column custom-list-menu mt-4">
                        <li><a href="" data-role="none" id="" data-gotopage="" class="active active-menu nav-link btnPIMMenu">Product Information</a></li>
                        <li><a href="" data-role="none" id="" data-gotopage="" class="nav-link btnPIMMenu"></a></li>
                        <li><a href="" data-role="none" id="" data-gotopage="" class="nav-link btnPIMMenu"></a></li>
                        <li><a href="" data-role="none" id="" data-gotopage="" class="nav-link btnPIMMenu"></a></li>
                        <li><a href="" data-role="none" id="" data-gotopage="" class="nav-link btnPIMMenu"></a></li>
                        <li style="display:none">
                            <a href="" data-role="none" id="btnPIMMenuModifiers" data-gotopage="modifierspage" class="nav-link btnPIMMenu">Modifiers</a>
                            <ol>
                                <li id="btnKitOptions">Kit Options</li>
                                <li id="" class="disabled-menu">Add-Ons</li>
                                <li id="" class="disabled-menu">Return for Rebates</li>
                                <li id="" class="disabled-menu">Replacements</li>
                                <li id="" class="disabled-menu">Preferences</li>
                            </ol>
                        </li>
                        <li><a href="" data-role="none" id="" data-gotopage="" class="nav-link btnPIMMenu"></a></li>
                        <li><a href="" data-role="none" id="" data-gotopage="" class="nav-link btnPIMMenu"></a></li>
                        <li><a href="" data-role="none" id="" data-gotopage="" class="nav-link disabled btnPIMMenu"></a></li>
                        <li><a href="" data-role="none" id="" data-gotopage="notyetimplemented" class="nav-link disabled btnPIMMenu"></a></li>
                        @*<li><a href="" data-role="none" id="btnPIMMenuSupplier" data-gotopage="notyetimplemented" class="nav-link disabled btnPIMMenu">Suppliers</a></li>*@
                    </ul>
                </div>
            </td>
            <td class="align-top pl-3">
                <div>
                    

                    <div id="divContainerMenuPage" style="display:block;">
                        <div id="productinfopage" style="display:block; font-size:18px;">
                            
                        </div>

                        <div id="unitofmeasurespage" style="display:none;">
                            
                        </div>

                        <div id="productpricespage" style="display:none;">
                            
                        </div>

                        <div id="divProductBOMContainer" style="display:none; margin: 0px 7px 0 7px;">
                            @*OLD BOM UI*@
                            <div>
                                <h3><span id="spanProductShortName"></span></h3>
                                <p>Use this tool to create Bill Of Materials/Kit for this product. Tap on "Add Component" to add specific components.</p>
                            </div>

                            <div data-role="fieldcontain"><a id="btnAddProductBOMMain" data-role="button" data-icon="plus" data-inline="true">Add Component</a></div>

                            <div id="divProductBOM" style="display:none;">

                                <div name="divMain_ProductBOMSKU" class="col" data-role="collapsible_clone" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d">
                                    <h3><span></span></h3>
                                    <div name="divProductBOMSub">
                                    </div>
                                </div>

                            </div>
                        </div>

                        <div id="billofmaterialspage" style="display:none;">
                            
                        </div>

                        <div id="modifierspage" style="display:none;">
                            <div id="pageKitOptions" subpage="modifierpage" style="display:block;">
                                <div id="pagemainKitOption" subpage="pageKitOptions" style="display:block;">
                                    <h3>Kit Options</h3>
                                    <p>This modifier allows customers to choose what exact items to include in their set (i.e. set meal, product kit, etc.)</p>
                                    <div style="float:right">
                                        <span style="display:block; float:right">
                                            <button id="btnAddGroupKitOptions" data-role="none" class="btn custom-btn-theme-b btn-less-width">
                                                Add Group
                                            </button>
                                        </span>
                                    </div>
                                    <br /><br /><br /><br />
                                    <table data-role="table" data-mode="reflow" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe">
                                        <thead>
                                            <tr class="custom-theme-f">
                                                <th>Group Name</th>
                                                <th>Options for Group</th>
                                                <th>Min Allowed Group Retail Value</th>
                                                <th>Max Allowed Group Retail Value</th>
                                            </tr>
                                        </thead>
                                        <tbody id=""></tbody>
                                    </table>
                                </div>

                                <div id="pageAddGroup" subpage="pageKitOptions" style="display:none;">
                                    <h3>New Group</h3>

                                    <div data-role="fieldcontain">
                                        <label for="txtKOGroupName">Group Name:</label>
                                        <input type="text" id="txtKOGroupName" />
                                    </div>

                                    <div style="float:right">
                                        <button id="btnKOGroupItem" data-role="none" class="btn custom-btn-theme-b btn-less-width">
                                            Add Item
                                        </button>
                                    </div>
                                    <br /><br /><br /><br />
                                    <ul id="listKOGroupItems" data-role="listview" data-inset="true"></ul>
                                </div>
                            </div>
                        </div>

                        <div id="piminventoryledger" style="display:none">
                        </div>

                        <div id="relatedproducts" style="display:none">
                        </div>

                        <div id="pimpicturegallery" style="display:none">
                            
                        </div>

                        <div id="notyetimplemented" style="display:none;">
                            
                        </div>
                    </div>

                    
                </div>
            </td>
        </tr>
    </table>
    <div class="row">


    </div>
    <div style="width:100%;">

    </div>
    <div style="clear:both;"></div>
</div>

<div id="pageSKUMeasurementOLD" data-role="page" style="display:none;">
    <div data-role="header">
        <h1>Measurement Details</h1>
    </div>

    <div data-role="content">
        <div data-role="fieldcontain" style="display:none;">
            <input type="text" readonly id="txtRecProductMeasurementID" />
            <input type="text" readonly id="txtRecProductMeasurementPackingSizeUnitID" />
        </div>

        <div data-role="fieldcontain">
            <label for="btnPackingSizeUnit">Packing Size Unit <span style="color: #FF0000;">*</span></label>
            <fieldset data-role="controlgroup">
                <button id="btnPackingSizeUnit" data-role="button">Please Select</button>
            </fieldset>
        </div>

        <div>
            <div data-role="fieldcontain">
                <label for="txtPackingSizeQty">Packing Size Qty <span style="color: #FF0000;">*</span></label>
                <fieldset data-role="controlgroup">
                    <input type="number" id="txtPackingSizeQty" style="text-align: right;" />
                </fieldset>
            </div>
        </div>

        <div>
            <div data-role="fieldcontain">
                <label for="txtPackingSizeVolDiscount">Volume Discount (%)</label>
                <fieldset data-role="controlgroup">
                    <input type="number" id="txtPackingSizeVolDiscount" style="text-align: right;" />
                </fieldset>
            </div>
        </div>

        <div>
            <div data-role="fieldcontain">
                <label for="txtPackingSizeContentQty">Content Qty</label>
                <fieldset data-role="controlgroup">
                    <input type="number" id="txtPackingSizeContentQty" style="text-align: right;" />
                </fieldset>
            </div>
        </div>

        <div data-role="fieldcontain">
            <label for="btnPackingSizeContentUnit">Content Unit</label>
            <fieldset data-role="controlgroup">
                <button id="btnPackingSizeContentUnit" data-role="button">Please Select</button>
            </fieldset>
        </div>

        <div>
            <div data-role="fieldcontain">
                <label for="txtPackingSizeWeight">Packing Size Weight</label>
                <fieldset data-role="controlgroup">
                    <input type="number" id="txtPackingSizeWeight" style="text-align: right;" />
                </fieldset>
            </div>
        </div>

        <div data-role="fieldcontain">
            <label for="btnPackingSizeWeightUnit">Packing Size Weight Unit</label>
            <fieldset data-role="controlgroup">
                <button id="btnPackingSizeWeightUnit" data-role="button">Please Select</button>
            </fieldset>
        </div>

        <div>
            <ul data-role="listview" data-inset="true">
                <li data-role="list-divider">Dimensions</li>
                <li>
                    <div>
                        <div data-role="fieldcontain">
                            <label for="txtPackingSizeDimensionLength">Dimension Length</label>
                            <fieldset data-role="controlgroup">
                                <input type="number" id="txtPackingSizeDimensionLength" style="text-align: right;" />
                            </fieldset>
                        </div>
                    </div>

                    <div data-role="fieldcontain">
                        <label for="btnPackingSizeDimLengthUnit">Dimension Length Unit</label>
                        <fieldset data-role="controlgroup">
                            <button id="btnPackingSizeDimLengthUnit" data-role="button">Please Select</button>
                        </fieldset>
                    </div>

                    <div>
                        <div data-role="fieldcontain">
                            <label for="txtPackingSizeDimensionWidth">Dimension Width</label>
                            <fieldset data-role="controlgroup">
                                <input type="number" id="txtPackingSizeDimensionWidth" style="text-align: right;" />
                            </fieldset>
                        </div>
                    </div>

                    <div data-role="fieldcontain">
                        <label for="btnPackingSizeDimWidthUnit">Dimension Width Unit</label>
                        <fieldset data-role="controlgroup">
                            <button id="btnPackingSizeDimWidthUnit" data-role="button">Please Select</button>
                        </fieldset>
                    </div>

                    <div>
                        <div data-role="fieldcontain">
                            <label for="txtPackingSizeDimensionHeight">Dimension Height</label>
                            <fieldset data-role="controlgroup">
                                <input type="number" id="txtPackingSizeDimensionHeight" style="text-align: right;" />
                            </fieldset>
                        </div>
                    </div>

                    <div data-role="fieldcontain">
                        <label for="btnPackingSizeDimensionHeightUnit">Dimension Height Unit</label>
                        <fieldset data-role="controlgroup">
                            <button id="btnPackingSizeDimensionHeightUnit" data-role="button">Please Select</button>
                        </fieldset>
                    </div>


                </li>
            </ul>
        </div>

        <div>
            <ul data-role="listview" id="lstProductMeasurementBarcodeContainer" data-inset="true">
                <li data-role="list-divider">Product Measurement Barcode</li>
                <li>

                    <div style="padding:5px;">
                        <div class="ui-grid-a" style="margin:0px; padding: 2px;">
                            <div class="ui-block-a" style="width:78%;height: 50px;padding: 0px;margin-bottom:5px;">
                                <input id="txtProductMeasurementBarcode" type="text" placeholder="Enter Barcode Number Here" style="height: 40px; padding:0px;" />
                            </div>
                            <div class="ui-block-b" style="width: 22%;height: 50px;padding: 0px;margin:0px;">
                                <center>
                                    <div>
                                        <img src="/Content/images/barcode96x96.png" class="detailimage2" id="butMeasurementBarCode" data-role="button" data-corners="false" />
                                    </div>
                                </center>
                            </div>
                        </div>

                    </div>
                    <br />
                    <div>
                        <ul data-role="listview" id="lstProductMeasurementBarcode" data-inset="true" data-split-icon="delete"></ul>
                    </div>
                </li>
            </ul>
        </div>

        <div style="text-align: center">
            <a id="btnProductMeasurementSave" data-role="button">
                <h3>Save</h3>
            </a>
        </div>

        <div style="text-align: center;">
            <a id="btnProductMeasurementRemove" data-role="button" style="display:none;">
                <h3>Remove</h3>
            </a>
        </div>

        <div style="text-align: center">
            <a id="btnProductMeasurementCancel" data-role="button">
                <h3>Cancel</h3>
            </a>
        </div>

    </div>

</div>

<div id="pageSKUPriceOLD" data-role="page" style="display:none;">
    <div data-role="header">
        <h1>Price Details</h1>
    </div>

    <div data-role="content">
        <div data-role="fieldcontain" style="display:none;">
            <input type="text" readonly id="txtRecProductPriceID" />
            <input type="text" readonly id="txtRecProductPriceLevelID" />
        </div>

        <div data-role="fieldcontain">
            <label for="btnProductPriceLevel">Price Level <span style="color: #FF0000;">*</span></label>
            <fieldset data-role="controlgroup">
                <button id="btnProductPriceLevel" data-role="button">Please Select</button>
            </fieldset>
        </div>

        <div>
            <div data-role="fieldcontain">
                <label for="txtProductPrice">Price <span style="color: #FF0000;">*</span></label>
                <fieldset data-role="controlgroup">
                    <input type="number" id="txtProductPrice" style="text-align: right;" />
                </fieldset>
            </div>
        </div>

        <div style="text-align: center">
            <a id="btnProductPriceSave" data-role="button">
                <h3>Save</h3>
            </a>
        </div>

        <div style="text-align: center;">
            <a id="btnProductPriceRemove" data-role="button" style="display:none;">
                <h3>Remove</h3>
            </a>
        </div>

        <div style="text-align: center">
            <a id="btnProductPriceCancel" data-role="button">
                <h3>Cancel</h3>
            </a>
        </div>

    </div>

</div>

<div id="pageBOMSKUInfo" style="display:none;">
    <h1 class="display-6 mb-4">Component Details</h1>

    <div data-role="content">
        <div data-role="fieldcontain" style="display:none;">
            <input type="text" data-role="none" class="form-control" readonly id="txtRecBOMProductID" />
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtBOMComponentName">Component Short Name:<span style="color: #FF0000;">*</span></label>
            <div class="col-sm-10">
                <input type="text" data-role="none" class="form-control" id="txtBOMComponentName" />
            </div>
        </div>

        <div style="display:none;"><input type="text" readonly id="txtBOMProductID" /></div>
        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="btnBOMProduct">SKU Record:<span style="color: #FF0000;">*</span></label>
            <div class="col-sm-10">
                <button id="btnBOMProduct" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
            </div>
        </div>

        <h5 class="mb-3" id="legendComponentMeasurement">How much this component do you need</h5>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtBOMProductQty">Quantity:<span style="color: #FF0000;">*</span></label>
            <div class="col-sm-10">
                <input type="number" data-role="none" class="form-control" id="txtBOMProductQty" />
            </div>
        </div>

        <h5 class="mb-3" id="legendUnitMeasure">Unit Of Measure</h5>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="selUnitMeasure">Quantity:<span style="color: #FF0000;">*</span></label>
            <div class="col-sm-10">
                <select id="selUnitMeasure" data-role="none" class="form-select">
                    <option value="">Please Select</option>
                </select>
            </div>
        </div>

        <h5 class="mb-3" id="legendComponentMethod">How do you make this. <span style="color: #FF0000;">*</span></h5>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="selComponentMethod">Quantity:<span style="color: #FF0000;">*</span></label>
            <div class="col-sm-10">
                <select id="selComponentMethod" data-role="none" class="form-select">
                    <option value="">Please Select</option>
                    <option value="Made-to-Order">Made-to-Order</option>
                    <option value="Batch Processing">Batch Processing</option>
                    <option value="Store Bought">Store Bought</option>
                </select>
            </div>
        </div>

        <div class="mt-4 text-center">
            <a id="btnProductBOMCancel" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
            <a id="btnProductBOMRemove" data-role="none" class="btn btn-danger btn-lg" style="display:none;">Remove</a>
            <a id="btnProductBOMSave" data-role="none" class="btn btn-success btn-lg">Save</a>
        </div>
    </div>
</div>

<div id="pageBrowseByOrgOwner" style="display:none;">
    <h1 class="display-6 mb-4">List of Organizations with Publicly Available Products</h1>

    <div data-role="content">
        <div class="row mb-3">
            <label class="col-sm-4 mb-3 col-form-label" for="txtSearchOrgOwner">Select an organization to subscribe to their product list:</label>
            <div class="col-sm-8">
                <form autocomplete="off" id="frmOrgOwnerSearch">
                    <div><input id="txtSearchOrgOwner" data-role="none" class="form-control" type="search" value="" placeholder="Search Organization" /></div>
                </form>
            </div>
        </div>

        <div id="divOrgOwnerSearchResults" style="display:none;">

            <div class="ListViewMarginBottom mb-3" id="divOrgOwnerRecordCount"></div>

            <div id="lstOrgOwner" data-role="none" class="list-group mb-3"></div>

        </div>

        <div id="divOrgOwnerSearchNoResults" style="display:none;">
            <div>
                <div data-role="fieldcontain">
                    <label id="lblOrgOwnerNotFound">No organizations found</label>
                </div>
            </div>
        </div>
    </div>
    <div id="divExpressCustomPopup" data-role="popup" style="display:none">
        <ul data-role="listview" data-inset="true" style="min-width: 210px;">
            <li data-role="divider">Option</li>
            <li><a id="btnExpress">Express (All Products)</a></li>
            <li><a id="btnCustom">Custom (Selected Products)</a></li>
        </ul>
    </div>
</div>

<div id="pageBarcodeSearch" style="display:none;">
    <h1 class="display-6 mb-4">Search All Publicly Available Products</h1>

    <div data-role="content">
        <div class="row">
            <div class="col-md">
                <form autocomplete="off" id="frmSearchPublicProduct">
                    <input id="txtPublicSearch" type="text" placeholder="Search products" class="form-control form-control-lg" autofocus />
                </form>
            </div>
            <div class="col-md-3">
                <button data-role="none" id="butBarCode" class="btn btn-success btn-block btn-lg">
                    Scan Barcode
                </button>
            </div>
        </div>
    </div>
</div>

<div id="pageCustomAdd" style="display:none;">
    <h1 class="display-6 mb-4">Custom Add</h1>

    <div data-role="content">
        <div class="row mb-3">
            <label class="col-sm-1 mb-3 col-form-label" for="selFilter">Filter:</label>
            <div class="col-sm-11">
                <select id="selFilter" data-role="none" class="form-select">
                    <option value="">Please select</option>
                    <option value="Brand">By Brand</option>
                    <option value="Category">By Category</option>
                </select>
            </div>
        </div>

        <div id="divResult" style="display:none;">
            <table id="tblResult" style="width:100%"></table>

            <div class="mt-4 text-center">
                <a id="btnAddProductsCustomCancel" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
                <a id="btnAddProductsCustom" data-role="none" class="btn btn-success btn-lg">Add</a>
            </div>
        </div>
        <div id="divNoResult" style="display:none;">No record(s) found</div>
    </div>
</div>

<div id="pageProductNamePreview" style="display:none;">
    <div data-role="header">
        @*<a href="#" data-icon="arrow-l" data-rel="back">Back</a>*@
        <h2>Preview Products</h2>
    </div>

    <div data-role="content">

        <div id="divResultName">
            You are about to add the following products to your company-wide product list:
            <ul data-role="listview" id="lstResultContainerName" data-inset="true">
                <li>
                    <table id="tblResultName" style="width:100%"></table>
                </li>
            </ul>
            <div>
                <br />
                <div style="text-align: center">
                    <a id="btnAddProductsContinue" data-role="button"><h3>Continue</h3></a>
                </div>
                <div style="text-align: center">
                    <a id="btnAddProductsCancel" data-role="button"><h3>Cancel</h3></a>
                </div>
            </div>

        </div>
        <div id="divNoResultName" style="display:none;">No record(s) found</div>


    </div>

</div>