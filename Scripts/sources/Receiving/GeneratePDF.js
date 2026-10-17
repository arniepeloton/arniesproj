
$(function () {
    var mDocTitle = '';

    $(function () {
        GenerateQRCodeToPDF();
    });
     
    function GenerateQRCodeToPDF() {

        var qrcode = new QRCode(document.getElementById("QrCodeCanvass"), {
            width: 100,
            height: 100
        });

        var vRetInfo = JSON.parse(sessionStorage.mReceivingInfo);

        qrcode.makeCode(vRetInfo.Rows[0].ID);

        window.setTimeout(function () {
            getImageFromUrl($('#QrCodeCanvass img').attr('src'), createPDF);
        }, 500);
    }

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

        var vRetDataHeader = JSON.parse(sessionStorage.mHeader);
        var vRetDataReceivingInfo = JSON.parse(sessionStorage.mReceivingInfo);
        var vRetDataReceivingDetails = JSON.parse(sessionStorage.mReceivingDetails);
        
        var xPos = 20;
        var yPos = 20;

        doc.setFontSize(16);
        doc.setFontType("bold");
        doc.text(xPos, yPos, $.trim((vRetDataHeader.Rows[0].CorporateName || '')));

        doc.setFontSize(8);
        doc.text(120, yPos, $.trim((vRetDataHeader.Rows[0].CompleteAddress || '')));

        //yPos = 25
        yPos += 5;

        doc.setFontSize(8);
        doc.text(xPos, yPos, $.trim((vRetDataHeader.Rows[0].BusinessTradeName || '')));

        doc.setFontSize(8);
        doc.text(120, yPos, 'Tel. No. ' + $.trim((vRetDataHeader.Rows[0].OrgTelNo || '')));
        
        //yPos = 45
        yPos += 20;

        if (vRetDataReceivingInfo.Rows[0].IsReceiving == '1') {
            doc.setFontSize(16);
            doc.setFontType("bold");
            doc.text(xPos, yPos, 'Receiving Report');
        } else {
            doc.setFontSize(16);
            doc.setFontType("bold");
            doc.text(xPos, yPos, 'Transfer-Out Report');
        }

        doc.addImage(imgData, 'JPEG', 120, yPos - 10, 20, 20, 'ReceivingAndTransfers'); // Cache the image using the alias 'ReceivingAndTransfers'

        //yPos = 55
        yPos += 15;

        doc.setFontSize(8);
        doc.setFontType("bold");
        doc.text(xPos, yPos, '_____________________________________________________________________________________________________');

        //yPos = 65
        yPos += 15;

        doc.setFontSize(8);
        doc.text(xPos, yPos, 'Reference No. ' + (vRetDataReceivingInfo.Rows[0].IsReceiving == '1' ? 'RCV #' : 'T-OUT #') + $.trim((vRetDataReceivingInfo.Rows[0].ReceivingNo || '')));

        mDocTitle = (vRetDataReceivingInfo.Rows[0].IsReceiving == '1' ? 'RCV #' : 'T-OUT #') + $.trim((vRetDataReceivingInfo.Rows[0].ReceivingNo || ''));

        doc.setFontSize(8);
        doc.text(120, yPos, 'Document Date: ' + $.trim((vRetDataReceivingInfo.Rows[0].DocumentDate || '')).substring(0, 9));

        //yPos = 70
        yPos += 5;

        if (vRetDataReceivingInfo.Rows[0].IsReceiving == '1') {
            doc.setFontSize(8);
            doc.text(xPos, yPos, 'Receive In: ' + $.trim((vRetDataReceivingInfo.Rows[0].ReceivingPlace || '')));

            doc.setFontSize(8);
            doc.text(120, yPos, 'Document Type: ' + $.trim((vRetDataReceivingInfo.Rows[0].DocumentType || '')));

            //yPos = 75
            yPos += 5;

            doc.setFontSize(8);
            doc.text(xPos, yPos, 'Receive From: ' + $.trim((vRetDataReceivingInfo.Rows[0].SourceOfSKU1 || '')));

        } else {
            doc.setFontSize(8);
            doc.text(xPos, yPos, 'Transfer From: ' + $.trim((vRetDataReceivingInfo.Rows[0].ReceivingPlace || '')));

            doc.setFontSize(8);
            doc.text(120, yPos, 'Document Type: ' + $.trim((vRetDataReceivingInfo.Rows[0].DocumentType || '')));

            //yPos = 75
            yPos += 5;

            doc.setFontSize(8);
            doc.text(xPos, yPos, 'Transfer To: ' + $.trim((vRetDataReceivingInfo.Rows[0].SourceOfSKU1 || '')));
        }
        
        doc.setFontSize(8);
        doc.text(120, yPos, 'Document No. ' + $.trim((vRetDataReceivingInfo.Rows[0].DocumentNo || '')));

        //yPos = 80
        yPos += 5;

        doc.setFontSize(8);
        doc.text(xPos, yPos, 'Transported By: ' + $.trim((vRetDataReceivingInfo.Rows[0].TransportedBy || '')));
        
        
        //yPos = 95
        yPos += 15;

        doc.setFontSize(16);
        doc.setFontType("bold");
        doc.text(85, yPos, 'Details');

        //yPos = 95
        yPos += 5;

        doc.rect(xPos, yPos, 160, 8); // empty square

        //yPos = 100
        yPos += 5;

        doc.setFontSize(8);
        doc.text(xPos, yPos, ' Product Description');

        doc.setFontSize(8);
        doc.text(120, yPos, ' Unit Measure');

        doc.setFontSize(8);
        doc.text(150, yPos, ' Quantity');

        //yPos = 110

        yPos += 10;
        
        for (var i = 0; i <= vRetDataReceivingDetails.Rows.length - 1; i++) {
            doc.setFontSize(8);
            doc.text(23, yPos, ' ' + $.trim((vRetDataReceivingDetails.Rows[i].ProductName || '')));

            doc.setFontSize(8);
            doc.text(123, yPos, '  ' + $.trim((vRetDataReceivingDetails.Rows[i].Label || '')));

            doc.setFontSize(8);
            doc.text(153, yPos, '  ' + $.trim((vRetDataReceivingDetails.Rows[i].QuantityGood || '')));

            doc.setFontSize(8);
            doc.text(20, yPos + 1, '_____________________________________________________________________________________________________');

            yPos += 5;
        }

        yPos += 20;

        doc.setFontSize(8);
        doc.text(20, yPos, '  ' + $.trim((vRetDataReceivingInfo.Rows[0].TransportedBy || '')));

        yPos += 1;

        doc.setFontSize(8);
        doc.text(20, yPos, '__________________');

        yPos += 5;

        doc.setFontSize(8);
        doc.text(20, yPos, ' Transported By: ');


        doc.save(mDocTitle + '.pdf');

        setTimeout(function () {
            window.close();
        }, 1000);
        
    }

    function GetUrlParameters(vUrl) {
        // PARAMS:
        //  vUrl: [optional]

        vUrl = vUrl || window.location.search.substring(1);
        var urlParams = {};
        var match,
            pl = /\+/g,  // Regex for replacing addition symbol with a space
            search = /([^&=]+)=?([^&]*)/g,
            decode = function (s) { return decodeURIComponent(s.replace(pl, " ")); },
            query = vUrl;

        while (match = search.exec(query))
            urlParams[decode(match[1])] = decode(match[2]);

        return urlParams;

    }

});