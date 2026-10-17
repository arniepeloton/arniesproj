
$(function () {
    var mDocTitle = '';
    $(function () {
        GenerateBarCodeToPDF();
    });
    if (sessionStorage.getItem("paperSize") != "largerSize") {
        var templateXGrid = [-15.0, 70, 155];
        var templateYGrid = [0.0, 59.63, 119.16, 178.69, 238.22, 297.75, 357.28, 416.81, 476.34, 535.87, 595.4, 654.93];
        var yPosition = 0;
    } else {
        var templateYGrid = [10.0, 75.63, 135.16, 193.69, 253.22, 315.75, 375.28, 435.81, 496.34, 555.87, 615.4, 675.93, 735.46];
        var templateXGrid = [25.0, 130.717, 240.434, 350.151, 455.868];
        var yPosition = 10;
    }

    var rowCounter = 1;
    var columnCounter = 0;
    var xPosition = 0;

    function GenerateBarCodeToPDF() {
        var BarcodeTags = JSON.parse(sessionStorage.BarcodeTags);

        for (var i = 0; i <= BarcodeTags.length - 1; i++) {
            BarcodeTagsCountTotal = parseInt(BarcodeTagsCountTotal) + parseInt(BarcodeTags[i].NumLabels);
        }
        $('#txtTo').text(BarcodeTagsCountTotal);
        $('#progBar').attr('max', BarcodeTagsCountTotal)
        var SKUID;
        var BarcodeTag;
        var pLv = $('#BarCodeCanvass');
        var ctrGeneratedLabel = 0;

        for (var j = 0; j < BarcodeTags.length; j++) {

            var BarcodeDiv = 'Barcode' + BarcodeTags[j].ID;
            BarcodeTags[j].BarcodeDiv = BarcodeDiv;
            SKUID = BarcodeTags[j].ID;
            pLv.append('<br /> <img id="' + BarcodeDiv + '">');

            $('#' + BarcodeDiv).JsBarcode(BarcodeTags[j].Barcode, { format: "CODE128", displayValue: true, fontSize: 40 });
            for (var i = 0; i < parseInt(BarcodeTags[j].NumLabels) ; i++) {
                
                setTimeout(function (bc) {
                    ctrGeneratedLabel = ctrGeneratedLabel + 1;
                    $('#txtFrom').text(ctrGeneratedLabel);
                    $('#progBar').attr('value', ctrGeneratedLabel)
                    createPDF($('#' + bc.BarcodeDiv).attr('src'), bc.SkuDesc, bc.SkuDesc + ctrGeneratedLabel, bc.TradeName, bc.UnitMeasure, bc.Price, bc.UoM, bc.SKUID);
                }, 3000, BarcodeTags[j])

            }
        }

    }

    var BarcodeTagsCount = 0;
    var BarcodeTagsCountTotal = 0;
    var IsLastImage = false;

    var getImageFromUrl = function (url, callback, SkuDesc, TradeName, UnitMeasure, Price, UoM, SKUID) {
        var img = new Image();

        img.onError = function () {
            alert('Cannot load image: "' + url + '"');
        };
        img.onload = function () {
            callback(img.src, SkuDesc, TradeName, UnitMeasure, Price, UoM, SKUID);
        };
        img.setAttribute('crossOrigin', 'anonymous');
        img.src = url;
    }

    if (sessionStorage.getItem("paperSize") == "largerSize") {
        var doc = new jsPDF('p', 'pt', [841.8898, 595.276]);
    } else {
        if (sessionStorage.getItem("paperFormat") == "1x3") {
            var doc = new jsPDF('l', 'pt', [255.118, 56.6923]);
        }
        else {
            var doc = new jsPDF('l', 'pt', [72, 144]);
        }
    }

    // Since images are loaded asyncronously, we must wait to create
    // the pdf until we actually have the image.
    // If we already had the jpeg image binary data loaded into
    // a string, we create the pdf without delay.
    var createPDF = function (imgData, SkuDesc, BarcodeDesc, TradeName, UniteMeasure, Price, UoM, SKUID) {
        var fontsize, xpos, ypos, imgwidth, imgheight, pricexpos, priceypos, pricefont;

        if (sessionStorage.getItem("IncCo") === "false") {
            TradeName = "";
        }

        if (sessionStorage.getItem("IncPricing") === "false") {
            Price = "";
        }
        else {
            Price = Price.slice(0, (Price.length - 2));
        }

        if (sessionStorage.getItem("paperSize") == "largerSize") {
            if (columnCounter == 5) {
                //Go to next row if all columns are filled
                columnCounter = 0;
                xPosition = templateXGrid[columnCounter];
                columnCounter++;

                if (rowCounter == 13) {
                    //Create a new page if all the rows within the page are filled
                    rowCounter = 0;
                    yPosition = templateYGrid[rowCounter];
                    rowCounter++;
                    doc.addPage();
                } else {
                    //Fill the next row within the page
                    yPosition = templateYGrid[rowCounter];
                    rowCounter++;
                }

            } else {
                //Fill the next column with in the row
                xPosition = templateXGrid[columnCounter];
                columnCounter++;
            }
            fontsize = 7;
            xpos = xPosition + 20;
            ypos = yPosition + 35;
            imgwidth = 75;
            imgheight = 20;
            pricefont = 8;

            if (Price.length == 7) {
                pricexpos = xPosition + 69;
                priceypos = yPosition + 63;
            }
            else if (Price.length < 6) {
                pricexpos = xPosition + 75;
                priceypos = yPosition + 63;
            }
            else if (Price.length == 6) {
                pricexpos = xPosition + 70;
                priceypos = yPosition + 63;
            }
            else if (Price.length == 8) {
                pricexpos = xPosition + 65;
                priceypos = yPosition + 63;
            }
            else {
                pricexpos = xPosition + 61;
                priceypos = yPosition + 63;
            }
        } else {
            if (sessionStorage.getItem("paperFormat") == "1x3") {
                if (columnCounter == 3) {
                    //Go to next row if all columns are filled
                    columnCounter = 0;
                    xPosition = templateXGrid[columnCounter];
                    columnCounter++;
                    doc.addPage();
                } else {
                    //Fill the next column with in the row
                    xPosition = templateXGrid[columnCounter];
                    columnCounter++;
                }
                yPosition = -25;
                fontsize = 7;
                xpos = xPosition + 20;
                ypos = yPosition + 35;
                imgwidth = 75;
                imgheight = 20;
                pricefont = 8;

                if (Price.length == 7) {
                    pricexpos = xPosition + 69;
                    priceypos = yPosition + 63;
                }
                else if (Price.length < 6) {
                    pricexpos = xPosition + 75;
                    priceypos = yPosition + 63;
                }
                else if (Price.length == 6) {
                    pricexpos = xPosition + 70;
                    priceypos = yPosition + 63;
                }
                else if (Price.length == 8) {
                    pricexpos = xPosition + 65;
                    priceypos = yPosition + 63;
                }
                else {
                    pricexpos = xPosition + 61;
                    priceypos = yPosition + 63;
                }
            }
            else {
                yPosition = -20;
                if (columnCounter == 1) {
                    columnCounter = 0;
                    xPosition = templateXGrid[columnCounter];
                    columnCounter++;
                    doc.addPage();
                }
                else {
                    xPosition = templateXGrid[columnCounter];
                    columnCounter++;
                }
                fontsize = 10;
                xpos = xPosition + 20;
                ypos = yPosition + 37;
                imgwidth = 130;
                imgheight = 30;
                pricefont = 8;

                if (Price.length == 3) {
                    pricexpos = xPosition + 145;
                    priceypos = yPosition + 72;
                }
                else if (Price.length == 4) {
                    pricexpos = xPosition + 140;
                    priceypos = yPosition + 72;
                }
                else if (Price.length == 5) {
                    pricexpos = xPosition + 135;
                    priceypos = yPosition + 72;
                }
                else if (Price.length == 6) {
                    pricexpos = xPosition + 130;
                    priceypos = yPosition + 72;
                }
                else if (Price.length == 7) {
                    pricexpos = xPosition + 125;
                    priceypos = yPosition + 72;
                }
                else if (Price.length == 8) {
                    pricexpos = xPosition + 120;
                    priceypos = yPosition + 72;
                }
                else if (Price.length == 9) {
                    pricexpos = xPosition + 115;
                    priceypos = yPosition + 72;
                }
                else {
                    pricexpos = 0;
                    priceypos = 0;
                }
            }
        }

        //Instructions for Trade Name
        if (SkuDesc.length > 15) {
            doc.setFontSize(fontsize);
            doc.setFontStyle('normal');
            doc.setFont('times');
            doc.text(xPosition + 20, yPosition + 33, TradeName.substr(0, 20));
            //doc.text(xPosition + 20, yPosition + 33, SkuDesc.substr(14, 27));
        } else {
            doc.setFontSize(fontsize);
            doc.setFontStyle('normal');
            doc.setFont('times');
            doc.text(xPosition + 20, yPosition + 33, TradeName);
        }

        //Instructions for UPL Barcode

        doc.addImage(imgData, 'PNG', xpos, ypos, imgwidth, imgheight, BarcodeDesc);

        //Instructions for Price

        doc.setFontSize(pricefont);
        doc.setFontStyle('bold');
        doc.setFont('times');
        doc.text(pricexpos, priceypos, Price);

        //Instructions for Product
        if (sessionStorage.getItem("paperFormat") == "1x3") {
            if (SkuDesc.length > 14) {
                //SkuDesc = SkuDesc.toUpperCase();
                doc.setFontSize(6);
                doc.setFontStyle('normal');
                doc.setFont('times');
                doc.text(xPosition + 20, yPosition + 62, (SkuDesc + ' - ' + UoM).substr(0, 13));
                doc.text(xPosition + 20, yPosition + 68, (SkuDesc + ' - ' + UoM).substr(13, 20));
                doc.text(xPosition + 20, yPosition + 74, (SkuDesc + ' - ' + UoM).substr(33, 20));
            } else {
                doc.setFontSize(6);
                doc.setFontStyle('normal');
                doc.setFont('times');
                doc.text(xPosition + 20, yPosition + 63, SkuDesc);
            }
        }
        else {
            if (SkuDesc.length > 20) {
                //SkuDesc = '1234567890ABCDEFGHIJKLMNOPQRSTUVXYZ1234567890ABCDEFGHIJKLMNOPQRSTUVXYZ';
                doc.setFontSize(7);
                doc.setFontStyle('normal');
                doc.setFont('times');
                doc.text(xPosition + 20, yPosition + 72, SkuDesc.substr(0, 20));
                doc.text(xPosition + 20, yPosition + 80, SkuDesc.substr(20, 20));
                doc.text(xPosition + 20, yPosition + 88, SkuDesc.substr(40, 20));
            } else {
                doc.setFontSize(7);
                doc.setFontStyle('normal');
                doc.setFont('times');
                doc.text(xPosition + 20, yPosition + 72, SkuDesc);
            }
        }

        BarcodeTagsCount++;

        if (BarcodeTagsCount == BarcodeTagsCountTotal) {
            doc.save('BarcodeLabels.pdf');
            setTimeout(function () {
                window.close();
            }, 1000);
            //var iframe = document.createElement('iframe');
            //iframe.setAttribute('style', 'position:absolute;top:0;right:0;height:100%; width:50%; z-index:1000;');
            //document.body.appendChild(iframe);
            //iframe.src = doc.output('datauristring');
        }
    }

});