
$(function () {

    var BinOrder = [];
    var BinOrderCount = 0;

    $(function () {
        var IsHasArea = false;

        $('#chkArea').click(function () {
            if ($('#chkArea').is(':checked')) {
                $('#txtArea').removeAttr('readonly', '');
                AddToBinOrder('Area');
                PreviewSampleFormat('Area');
            } else {
                $('#txtArea').attr('readonly', '');                
                RemoveFromBinOrder('Area');
                PreviewSampleFormat('Area');
            }
        });

        $('#chkRack').click(function () {
            if ($('#chkRack').is(':checked')) {
                $('#txtRack').removeAttr('readonly', '');
                AddToBinOrder('Rack');
                PreviewSampleFormat('Rack');
            } else {
                $('#txtRack').attr('readonly', '');
                RemoveFromBinOrder('Rack');
                PreviewSampleFormat('Rack');
            }
        });
        
        $('#chkSection').click(function () {
            if ($('#chkSection').is(':checked')) {
                $('#txtSection').removeAttr('readonly', '');
                AddToBinOrder('Section');
                PreviewSampleFormat('Section');
            } else {
                $('#txtSection').attr('readonly', '');
                RemoveFromBinOrder('Section');
                PreviewSampleFormat('Section');
            }
        });

        $('#chkShelf').click(function () {
            if ($('#chkShelf').is(':checked')) {
                $('#txtShelf').removeAttr('readonly', '');
                AddToBinOrder('Shelf');
                PreviewSampleFormat('Shelf');
            } else {
                $('#txtShelf').attr('readonly', '');
                RemoveFromBinOrder('Shelf');
                PreviewSampleFormat('Shelf');
            }
        });

        $('#chkAisle').click(function () {
            if ($('#chkAisle').is(':checked')) {
                $('#txtAisle').removeAttr('readonly', '');
                AddToBinOrder('Aisle');
                PreviewSampleFormat('Aisle');
            } else {
                $('#txtAisle').attr('readonly', '');
                RemoveFromBinOrder('Aisle');
                PreviewSampleFormat('Aisle');
            }
        });

        $('#chkSide').click(function () {
            if ($('#chkSide').is(':checked')) {
                $('#txtSide').removeAttr('readonly', '');
                AddToBinOrder('Side');
                PreviewSampleFormat('Side');
            } else {
                $('#txtSide').attr('readonly', '');
                RemoveFromBinOrder('Side');
                PreviewSampleFormat('Side');
            }
        });

        $('#GenerateQRCodeToPDF').click(function () {
            GenerateQRCodeToPDF();
        });
    });

    function GenerateQRCodeToPDF() {

        var qrcode = new QRCode(document.getElementById("QrCodeCanvass"), {
            width: 100,
            height: 100
        });

        qrcode.makeCode('test1234');

        getImageFromUrl($('#QrCodeCanvass img').attr('src'), createPDF);

    }

    function RemoveFromBinOrder(mFormat) {
        for (i = 0; i < BinOrder.length; i++) {
            if (mFormat == BinOrder[i][0]) {
                BinOrder[i][0] = undefined;
            }
        }
    }

    function AddToBinOrder(mFormat) {
        BinOrder[BinOrderCount] = [];

        BinOrder[BinOrderCount][0] = mFormat;

        if (mFormat == 'Shelf') {
            //else if mFormat = alpha
            BinOrder[BinOrderCount][1] = 'A';
        }
        else if (mFormat == 'Side') {
            //else if mFormat = side (L/R Left or Right)
            BinOrder[BinOrderCount][1] = 'L';
        }
        else{
            //if mFormat = numeric
            BinOrder[BinOrderCount][1] = 1;
        }
        

        
        BinOrderCount++;

    }

    function PreviewSampleFormat(mFormat) {
        var mFormat = '';
        var mFormatLabel = '';
        var hasNo1 = false;
                
        for (i = 0; i < BinOrder.length; i++) {
            if (BinOrder[i][0] != undefined) {
                if (!hasNo1) {
                    mFormatLabel = '(' + BinOrder[i][0] + '';
                    mFormat = BinOrder[i][1];
                    hasNo1 = true;
                } else {
                    mFormatLabel += '.' + '' + BinOrder[i][0] + '';
                    mFormat += '.' + BinOrder[i][1];
                }                
            }
        }

        if (hasNo1) {
            mFormatLabel += ')';
        }
        
        $('#lblSampleFormat').text(mFormat);
        $('#lblSampleFormatLabel').text(mFormatLabel);
    }

    // Because of security restrictions, getImageFromUrl will
    // not load images from other domains.  Chrome has added
    // security restrictions that prevent it from loading images
    // when running local files.  Run with: chromium --allow-file-access-from-files --allow-file-access
    // to temporarily get around this issue.
    var getImageFromUrl = function (url, callback) {
        var img = new Image();

        img.onError = function () {
            alert('Cannot load image: "' + url + '"');
        };
        img.onload = function () {
            callback(img);
        };
        img.setAttribute('crossOrigin', 'anonymous');
        img.src = url;
    }

    // Since images are loaded asyncronously, we must wait to create
    // the pdf until we actually have the image.
    // If we already had the jpeg image binary data loaded into
    // a string, we create the pdf without delay.
    var createPDF = function (imgData) {
        var doc = new jsPDF();

        // This is a modified addImage example which requires jsPDF 1.0+
        // You can check the former one at examples/js/basic.js

        doc.addImage(imgData, 'JPEG', 20, 40, 50, 50, 'monkey'); // Cache the image using the alias 'monkey'
        //doc.addImage('monkey', 70, 10, 100, 120); // use the cached 'monkey' image, JPEG is optional regardless
        doc.text(20, 20, 'Hello world!');
        // As you can guess, using the cached image reduces the generated PDF size by 50%!

        // Rotate Image - new feature as of 2014-09-20
        //doc.addImage({
        //    imageData: imgData,
        //    angle: -20,
        //    x: 10,
        //    y: 78,
        //    w: 45,
        //    h: 58
        //});

        // Output as Data URI
        //doc.output('datauri');

        doc.save('test.pdf');
    }
});