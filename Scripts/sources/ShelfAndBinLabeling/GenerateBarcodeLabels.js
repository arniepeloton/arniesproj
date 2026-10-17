
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

        BarcodeTagsCountTotal = BarcodeTags.length;

        //for (var i = 0; i <= BarcodeTags.length - 1; i++) {
        //    BarcodeTagsCountTotal = parseInt(BarcodeTagsCountTotal) + parseInt(BarcodeTags[i].NumLabels);
        //}
        $('#txtTo').text(BarcodeTagsCountTotal);

        $('#progBar').attr('max', BarcodeTags.length)
        var SKUID;
        var BarcodeTag;
        var pLv = $('#BarCodeCanvass');
        var ctrGeneratedLabel = 0;
        var BarcodePosition;

        for (var j = 0; j < BarcodeTags.length; j++) {

            var BarcodeDiv = 'Barcode' + BarcodeTags[j].ID;
            BarcodeTags[j].BarcodeDiv = BarcodeDiv;
            SKUID = BarcodeTags[j].ID;
            pLv.append('<br /> <img id="' + BarcodeDiv + '">');

            if((j+1)%3==0){
                BarcodePosition=3;
            }else if((j+1)%2==0){
                BarcodePosition=2;
            }else{
                BarcodePosition=1;
            }

            $('#' + BarcodeDiv).JsBarcode(BarcodeTags[j].Barcode, { format: "CODE128", displayValue: true, fontSize: 20 });
            //$('#' + BarcodeDiv).JsBarcode(BarcodeTags[j].Barcode, { format: "EAN13", displayValue: true, fontSize: 20 });
            //$('#' + BarcodeDiv).barcode(BarcodeTags[j].Barcode, "CODE128", "fontSize: 40" );

            (function (bc, BarcodePosition) {
                setTimeout(function () {
                    ctrGeneratedLabel = ctrGeneratedLabel + 1;
                    $('#txtFrom').text(ctrGeneratedLabel);
                    $('#progBar').attr('value', ctrGeneratedLabel)
                    createPDF($('#' + bc.BarcodeDiv).attr('src'), bc.ProductName, bc.UnitMeasure, bc.Price, bc.UnitMeasure, bc.ID, BarcodePosition);
                }, 1000)
            })(BarcodeTags[j], BarcodePosition);

            //}
        }

    }

    var BarcodeTagsCount = 0;
    var BarcodeTagsCountTotal = 0;
    var IsLastImage = false;

    var getImageFromUrl = function (url, callback, SkuDesc, UnitMeasure, Price, UoM, SKUID, BarcodePosition) {
        var img = new Image();

        img.onError = function () {
            alert('Cannot load image: "' + url + '"');
        };
        img.onload = function () {
            callback(img.src, SkuDesc, UnitMeasure, Price, UoM, SKUID, BarcodePosition);
        };
        img.setAttribute('crossOrigin', 'anonymous');
        img.src = url;
    }

    if (sessionStorage.getItem("paperSize") == "largerSize") {
        var doc = new jsPDF('p', 'pt', [841.8898, 595.276]);
    } else {
        if (sessionStorage.getItem("paperSize") == "smallerSize") {
            var doc = new jsPDF('l', 'cm', [2, 9]);
        } else if (sessionStorage.getItem("paperSize") == "mediumSize") {
            var doc = new jsPDF('l', 'cm', [2.5, 9]);
        }
        else {
            var doc = new jsPDF('l', 'pt', [72, 144]);
        }
    }

    // Since images are loaded asyncronously, we must wait to create
    // the pdf until we actually have the image.
    // If we already had the jpeg image binary data loaded into
    // a string, we create the pdf without delay.

    //BarcodeDesc, TradeName, 
    var createPDF = function (imgData, SkuDesc, UniteMeasure, Price, UoM, SKUID, BarcodePosition) {

        var fontsize, xpos, ypos, imgwidth, imgheight, pricexpos, priceypos, pricefont;        
        
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
            imgwidth = 100;
            imgheight = 100;
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
            
            //Instructions for UPL Barcode

            doc.addImage(imgData, 'PNG', xpos, ypos, imgwidth, imgheight);

        } else {
            if (sessionStorage.getItem("paperSize") == "smallerSize") {

                ypos = 0.75;
                imgwidth = 2.6
                imgheight = 1 ;

                for (var barpos = 0; barpos < 3; barpos++) {

                    xpos = 0.25;
                    
                    if (barpos == 0) {
                        doc.addImage(imgData, 'JPEG', xpos, ypos, imgwidth, imgheight);
                    }
                    if (barpos == 1) {
                        xpos += 3;
                        doc.addImage(imgData, 'JPEG', xpos, ypos, imgwidth, imgheight);
                    } else {
                        xpos += 6;
                        doc.addImage(imgData, 'JPEG', xpos, ypos, imgwidth, imgheight);
                    }

                }
            } else if (sessionStorage.getItem("paperSize") == "mediumSize") {

                ypos = 1.5;
                imgwidth = 2.5;
                imgheight = 1;

                for (var barpos = 0; barpos < 3; barpos++) {

                    xpos = 0.25;

                    if (barpos == 0) {
                        doc.addImage(imgData, 'JPEG', xpos, ypos, imgwidth, imgheight);
                    }
                    if (barpos == 1) {
                        xpos += 3;
                        doc.addImage(imgData, 'JPEG', xpos, ypos, imgwidth, imgheight);
                    } else {
                        xpos += 6;
                        doc.addImage(imgData, 'JPEG', xpos, ypos, imgwidth, imgheight);
                    }

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

        //Instructions for Price
        
        doc.setFontSize(pricefont);
        doc.setFontStyle('bold');
        doc.setFont('times');

        //Instructions for Product
        if (sessionStorage.getItem("paperSize") == "smallerSize") {

            //if (SkuDesc.length > 14) {
                SkuDesc = SkuDesc.toUpperCase();
                doc.setFontSize(6);

                for (var barpos = 0; barpos < 3; barpos++) {

                    xpos = 0.3;
                    ypos = 0.25;

                    if (barpos == 0) {
                        doc.text(xpos, ypos, (SkuDesc).substr(0, 16).trim());
                        doc.text(xpos, ypos + .2, (SkuDesc).substr(16, 16).trim());
                        doc.text(xpos, ypos + .4, (SkuDesc).substr(32, 10).trim());
                        doc.text(xpos + 1.5, ypos + .4, 'P' + Price);
                    } else if (barpos == 1) {
                        xpos += 3;
                        doc.text(xpos, ypos, (SkuDesc).substr(0, 16).trim());
                        doc.text(xpos, ypos + .2, (SkuDesc).substr(16, 16).trim());
                        doc.text(xpos, ypos + .4, (SkuDesc).substr(32, 10).trim());
                        doc.text(xpos + 1.5, ypos + .4, 'P' + Price);
                    } else {
                        xpos += 6;
                        doc.text(xpos, ypos, (SkuDesc).substr(0, 16).trim());
                        doc.text(xpos, ypos + .2, (SkuDesc).substr(16, 16).trim());
                        doc.text(xpos, ypos + .4, (SkuDesc).substr(32, 10).trim());
                        doc.text(xpos + 1.5, ypos + .4, 'P' + Price);
                    }
                }
                
        } else if (sessionStorage.getItem("paperSize") == "mediumSize") {

            //if (SkuDesc.length > 14) {
            SkuDesc = SkuDesc.toUpperCase();
            
            for (var barpos = 0; barpos < 3; barpos++) {

                xpos = 0.25;
                ypos = 0.55;

                if (barpos == 0) {
                    doc.setFontSize(6);
                    doc.text(xpos, ypos, (SkuDesc).substr(0, 16).trim());
                    doc.text(xpos, ypos + .2, (SkuDesc).substr(16, 16).trim());
                    doc.text(xpos, ypos + .4, (SkuDesc).substr(32, 10).trim());
                    doc.setFontSize(10);
                    doc.text(xpos + 1.25, ypos + .75, 'P' + Price);
                } else if (barpos == 1) {
                    xpos += 3;
                    doc.setFontSize(6);
                    doc.text(xpos, ypos, (SkuDesc).substr(0, 16).trim());
                    doc.text(xpos, ypos + .2, (SkuDesc).substr(16, 16).trim());
                    doc.text(xpos, ypos + .4, (SkuDesc).substr(32, 10).trim());
                    doc.setFontSize(10);
                    doc.text(xpos + 1.25, ypos + .75, 'P' + Price);
                } else {
                    xpos += 6;
                    doc.setFontSize(6);
                    doc.text(xpos, ypos, (SkuDesc).substr(0, 16).trim());
                    doc.text(xpos, ypos + .2, (SkuDesc).substr(16, 16).trim());
                    doc.text(xpos, ypos + .4, (SkuDesc).substr(32, 10).trim());
                    doc.setFontSize(10);
                    doc.text(xpos + 1.25, ypos + .75, 'P' + Price);
                }
            }
                
        }
        else {
            if (SkuDesc.length > 20) {
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
            doc.save('ItemLabels.pdf');
            setTimeout(function () {
                window.close();
            }, 1000);
        } else {
            doc.addPage();
        }

    }

});