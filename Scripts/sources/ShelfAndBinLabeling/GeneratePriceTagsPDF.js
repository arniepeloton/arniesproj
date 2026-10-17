
$(function () {
    var mDocTitle = '';
    $(function () {
        GenerateQRCodeToPDF();
    });
    var labelCount = 0;
    var yPosition = 0.0;
    var templateGrid = [0.0, 71.72, 143.43, 215.15, 286.87, 358.58, 430.3, 502.02, 573.73, 645.45, 717.17, 788.88];

    function GenerateQRCodeToPDF() {

        var ShelfPrice = JSON.parse(sessionStorage.PriceTags);
        
        ShelfPriceCountTotal = ShelfPrice.length;

        var ProductID;
        var ProductNo;
        var BinName;
        var BinArea;

        var pLv = $('#QrCodeCanvass');

        //save
        for (var i = 0; i <= ShelfPrice.length - 1; i++) {

            var QRDiv = 'QR' + i + '';

            pLv.append('<br /> <div id="' + QRDiv + '">');

            var qrcode = new QRCode(document.getElementById(QRDiv), {
                width: 100,
                height: 100
            });

            ProductID = ShelfPrice[i].ID;
            ProductNo = ShelfPrice[i].ProductNo;
            BinName = ShelfPrice[i].BinName;
            BinArea = ShelfPrice[i].BinArea;

            //BinName = BinIDs[i][1];

            qrcode.makeCode(GuidToBase64(ProductID).replace('==',''));

            //SaveBin(data);


            (function (vProductID, ProductName, vUnitMeasure, Price, LastUpdated, vQRDiv, MaxPerProduct, MinPerProduct, ProductNo, BinName, BinArea) {
                window.setTimeout(function () {
                    getImageFromUrl($('#' + vQRDiv + ' img').attr('src'), createPDF, ProductName, vUnitMeasure, Price, LastUpdated, MaxPerProduct, MinPerProduct, ProductNo, BinName, BinArea);
                }, 1000);
            })(ProductID, ShelfPrice[i].ProductName, ShelfPrice[i].UnitMeasure, '-', '-', QRDiv, ShelfPrice[i].MaxPerProduct, ShelfPrice[i].MinPerProduct, ProductNo, BinName, BinArea);
            //ShelfPrice[i].Price, $.format.date(new Date(ShelfPrice[i].LastUpdated 'MM/dd/yyyy')),
        }
    }

    var ShelfPriceCount = 0;
    var ShelfPriceCountTotal = 0;
    var IsLastImage = false;

    var getImageFromUrl = function (url, callback, ProductName, UnitMeasure, Price, LastUpdated, MaxPerProduct, MinPerProduct, ProductNo, BinName, BinArea) {
        var img = new Image();

        img.onError = function () {
            alert('Cannot load image: "' + url + '"');
        };
        img.onload = function () {
            callback(img, ProductName, UnitMeasure, Price, LastUpdated, MaxPerProduct, MinPerProduct, ProductNo, BinName, BinArea);
        };
        img.setAttribute('crossOrigin', 'anonymous');
        img.src = url;
    }



    //var doc = new jsPDF('l', 'cm', [3.81, 7.62]);
    if (sessionStorage.getItem("paperSize") == "largerSize") {
        var doc = new jsPDF('l', 'pt', [108, 216]);
    } else if (sessionStorage.getItem("paperSize") == "smallerSize") {
        var doc = new jsPDF('l', 'cm', [2.54, 5.08]);
    } else if (sessionStorage.getItem("paperSize") == "mediumSize") {
        var doc = new jsPDF('l', 'cm', [2.5, 9]);
    } else {
        var doc = new jsPDF('p', 'pt', [894.9, 325.7]);
        //var doc = new jsPDF('l', 'pt', [66.04, 252]);
    }

    var createPDF = function (imgData, ProductName, UnitMeasure, Price, LastUpdated, MaxPerProduct, MinPerProduct, ProductNo, BinName, BinArea) {

        var xLocation;
        var yLocation;

        //ProductName = ProductName.replace(/[aeiou]/ig,'');
        if (sessionStorage.getItem("paperSize") == "mediumSize") {
            ypos = 1.1;
            imgwidth = 1.1;
            imgheight = 1.1;

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
            //doc.addImage(imgData, 'JPEG', 0.25, 1.15, 1.2, 1.2, ProductName);
        } else {
            if (sessionStorage.getItem("paperSize") == "largerSize") {
                if (sessionStorage.getItem("paperSize") == "largerSize" && Price == '-') {
                    doc.addImage(imgData, 'JPEG', 135, 12, 65, 65, ProductName);
                } else {
                    doc.addImage(imgData, 'JPEG', 15, 45, 55, 55, ProductName);
                }                
            } else if (sessionStorage.getItem("paperSize") == "smallerSize") {
                doc.addImage(imgData, 'JPEG', 0.25, 1.15, 1.2, 1.2, ProductName);
            } else {
                doc.addImage(imgData, 'JPEG', 30, yPosition + 23, 40, 40, ProductName);
            }

            //Instructions for ProductName
            //Get the paper size and set the location
            if (sessionStorage.getItem("paperSize") == "largerSize") {
                yLocation = 20;
                yPosition = 0;
                xLocation = 15;
            } else if (sessionStorage.getItem("paperSize") == "smallerSize") {
                yLocation = 0.50;
                yPosition = 0;
                xLocation = 0.25;

            } else if (sessionStorage.getItem("paperSize") == "mediumSize") {
                //yLocation = 0.50;
                //yPosition = 0;
                //xLocation = 0.25;

            }
            else {
                yLocation = 15;
                xLocation = 30;
            }

            ProductName = ProductName.toUpperCase();

            if (sessionStorage.getItem("paperSize") == "largerSize" && Price == '-') {
                yLocation += 20;
            }

            if (ProductName.length < 18) {
                doc.setFontSize(12);
                doc.text(xLocation, yLocation + yPosition + .1, ProductName);
            } else if (ProductName.length < 22) {
                doc.setFontSize(10);
                doc.text(xLocation, yLocation + yPosition + .1, ProductName);
            } else if (ProductName.length < 27) {
                doc.setFontSize(8);
                doc.text(xLocation, yLocation + yPosition + .1, ProductName);
            }
                //if (ProductName.length < 39)
            else {

                doc.setFontSize(6);
                if (ProductName.length > 30) {
                    doc.setFontSize(10);
                    doc.text(xLocation, 20, 'PN: ' + ProductNo);

                    doc.setFontSize(11.5);
                    doc.text(xLocation, yLocation + yPosition - 6, ProductName.substr(0, 16));
                    doc.text(xLocation, yLocation + yPosition + 5, ProductName.substr(16, 16));
                    doc.text(xLocation, yLocation + yPosition + 16, ProductName.substr(32, 16));
                    doc.text(xLocation, yLocation + yPosition + 27, ProductName.substr(48, 16));
                    doc.text(xLocation, yLocation + yPosition + 38, ProductName.substr(64, 16));
                    //doc.text(xLocation, yLocation + yPosition + 50, ProductName.substr(80, 16));
                    
                    doc.setFontSize(6);
                    doc.text(xLocation, 90, BinArea.substr(0, 38));
                    doc.text(xLocation, 97, sessionStorage.FowstName.substr(0, 38));

                    //doc.text(120, 95, 'Area: ' + AreaName);
                    doc.setFontSize(11.5);
                    doc.text(150, 95, 'BLT: ' + BinName.replaceAll('.', ' '));
                    
                } else {
                    doc.setFontSize(7);
                    doc.text(xLocation, yLocation + yPosition + .1, ProductName)
                }
            }
        }

        //Instructions for price
        //The location of the price is determined by the length of the string
        
        if (Price.length > 6) {
            doc.setFontSize(24);
            //doc.setFontStyle('bold');
            //doc.setFont('monospace');
            if (sessionStorage.getItem("paperSize") == "largerSize" && Price == '-') {
                //don't put price
            }
            else if (sessionStorage.getItem("paperSize") == "largerSize") {
                Price = Price + '00';
                doc.text(85, 60, Price.slice(0, (Price.length - 2)));
            } else if (sessionStorage.getItem("paperSize") == "smallerSize") {
                doc.setFontSize(18);
                doc.text(2.8, 1.65, Price);
            } else if (sessionStorage.getItem("paperSize") == "mediumSize") {
                //doc.setFontSize(16);
                //doc.text(2.6, 1.45, Price);
            }            
            else {
                Price = Price + '00';
                yPosition = yPosition + 40;
                doc.text(200, yPosition, Price.slice(0, (Price.length - 2)));
            }
        } else if (7 > Price.length && Price.length > 5) {
            doc.setFontSize(24);
            //doc.setFontStyle('bold');
            //doc.setFont('monospace');
            if (sessionStorage.getItem("paperSize") == "largerSize" && Price == '-') {
                //don't put price
            }
            else if (sessionStorage.getItem("paperSize") == "largerSize") {
                Price = Price + '00';
                doc.text(105, 60, Price.slice(0, (Price.length - 2)));
            } else if (sessionStorage.getItem("paperSize") == "smallerSize") {
                doc.setFontSize(18);
                doc.text(2.8, 1.65, Price);
            } else if (sessionStorage.getItem("paperSize") == "mediumSize") {
                //doc.setFontSize(18);
                //doc.text(2.6, 1.45, Price);
            }            
            else {
                Price = Price + '00';
                yPosition = yPosition + 40;
                doc.text(225, yPosition, Price.slice(0, (Price.length - 2)));
            }
        } else {
            doc.setFontSize(24);
            //doc.setFontStyle('bold');
            //doc.setFont('monospace');
            if (sessionStorage.getItem("paperSize") == "largerSize" && Price == '-') {
                //don't put price
            }
            else if (sessionStorage.getItem("paperSize") == "largerSize") {
                Price = Price + '00';
                doc.text(130, 60, Price.slice(0, (Price.length - 2)));
            } else if (sessionStorage.getItem("paperSize") == "smallerSize") {
                doc.setFontSize(18);
                doc.text(3.2, 1.65, Price);
            } else if (sessionStorage.getItem("paperSize") == "mediumSize") {
                //doc.setFontSize(20);
                //doc.text(3.2, 1.65, Price);
            } else {
                Price = Price + '00';
                yPosition = yPosition + 40;
                doc.text(180, yPosition, Price.slice(0, (Price.length - 2)));
            }
        }

        if (sessionStorage.getItem("paperSize") == "mediumSize") {

            //yPosition = yPosition + 1.8;
            //yPosition = yPosition + 2.1;

            //if (UnitMeasure.length <= 5) {
            //    doc.setFontSize(10);
            //    doc.text(3.7, yPosition, UnitMeasure);
            //} else {
            //    doc.setFontSize(10);
            //    var tempx = 3.7 - (.1 * (UnitMeasure.length - 3))
            //    doc.text(tempx, yPosition, UnitMeasure);
            //}

            var SkuDesc = ProductName;

            SkuDesc = SkuDesc.toUpperCase();

            for (var barpos = 0; barpos < 3; barpos++) {

                xpos = 0.25;
                ypos = 0.55;

                var dateStr = LastUpdated.replace('20', '').replace(/\//g, '.') + '.' + (MaxPerProduct || 0) + '.' + (MinPerProduct || 0);
             
               

                if (barpos == 0) {
                    doc.setFontSize(6);
                    doc.text(xpos, ypos, (SkuDesc).substr(0, 16).trim());
                    doc.text(xpos, ypos + .2, (SkuDesc).substr(16, 16).trim());
                    doc.text(xpos, ypos + .4, (SkuDesc).substr(32, 10).trim());
                    doc.setFontSize(8);
                    doc.text(xpos + 1.4, ypos + .75, 'P' + Price);

                    doc.setFontSize(9);
                    var tempx = 2 - (.1 * (UnitMeasure.length - 3))
                    doc.text(tempx, ypos + 1.25, UnitMeasure);

                    doc.setFontSize(5.5);
                    tempx = 1.7;
                    doc.text(tempx, ypos + 1.65, dateStr);

                } else if (barpos == 1) {
                    xpos += 3;
                    doc.setFontSize(6);
                    doc.text(xpos, ypos, (SkuDesc).substr(0, 16).trim());
                    doc.text(xpos, ypos + .2, (SkuDesc).substr(16, 16).trim());
                    doc.text(xpos, ypos + .4, (SkuDesc).substr(32, 10).trim());
                    doc.setFontSize(8);
                    doc.text(xpos + 1.4, ypos + .75, 'P' + Price);

                    doc.setFontSize(9);
                    var tempx = 5 - (.1 * (UnitMeasure.length - 3))
                    doc.text(tempx, ypos + 1.25, UnitMeasure);

                    doc.setFontSize(5.5);
                    tempx = 4.7;
                    doc.text(tempx, ypos + 1.65, dateStr);
                } else {
                    xpos += 6;
                    doc.setFontSize(6);
                    doc.text(xpos, ypos, (SkuDesc).substr(0, 16).trim());
                    doc.text(xpos, ypos + .2, (SkuDesc).substr(16, 16).trim());
                    doc.text(xpos, ypos + .4, (SkuDesc).substr(32, 10).trim());
                    doc.setFontSize(8);
                    doc.text(xpos + 1.4, ypos + .75, 'P' + Price);

                    doc.setFontSize(9);
                    var tempx = 8 - (.1 * (UnitMeasure.length - 3))
                    doc.text(tempx, ypos + 1.25, UnitMeasure);

                    doc.setFontSize(5.5);
                    tempx = 7.7;
                    doc.text(tempx, ypos + 1.65, dateStr);
                }
            }

        } else {
            //Instructions for unit size
            doc.setFontSize(12);
            //doc.setFontStyle('normal');
            //doc.setFont('times');
            if (sessionStorage.getItem("paperSize") == "largerSize" && Price == '-') {
                //don't put unit measure
            }
            else if (sessionStorage.getItem("paperSize") == "largerSize") {
                doc.text(15, 35, UnitMeasure);
            } else if (sessionStorage.getItem("paperSize") == "smallerSize") {

                //yPosition = yPosition + 1.8;
                yPosition = yPosition + 2.2;

                if (UnitMeasure.length <= 5) {
                    doc.setFontSize(11);
                    doc.text(3.7, yPosition, UnitMeasure);
                } else {
                    doc.setFontSize(11);
                    var tempx = 3.7 - (.1 * (UnitMeasure.length - 3))
                    doc.text(tempx, yPosition, UnitMeasure);
                }
            } else {
                yPosition = yPosition + 15;
                doc.text(235, yPosition, UnitMeasure);
            }

            if (sessionStorage.getItem("paperSize") == "largerSize" && Price == '-') {

            }else {
            //Instructions for last updated date
            //doc.setFontSize(7.5);

            //remove 20 in 2017
            var dateStr = LastUpdated.replace('20', '').replace(/\//g, '.') + '.' + (MaxPerProduct || 0) + '.' + (MinPerProduct || 0);
            //if (sessionStorage.getItem("paperSize") == "largerSize") {
            //    doc.text(80, 95, dateStr);
            //} else if (sessionStorage.getItem("paperSize") == "smallerSize") {
            doc.setFontSize(7.5);
            yPosition = yPosition;
            doc.text(1.6, yPosition, dateStr);
            //} else {
            //    yPosition = yPosition + 10;
            //    doc.text(145, yPosition, dateStr);
            //}
            }
        }

        ShelfPriceCount++;

        if (ShelfPriceCount == ShelfPriceCountTotal) {
            doc.save('ShelfLabels.pdf');
            setTimeout(function () {
                window.close();
            }, 1000);

        } else {
            if (sessionStorage.getItem("paperSize") == "smallerSize") {
                doc.addPage();
            } else if (sessionStorage.getItem("paperSize") == "mediumSize") {
                doc.addPage();
            } else if (sessionStorage.getItem("paperSize") != "largerSize") {
                if (labelCount < 11) {
                    labelCount = labelCount + 1;
                    //yPosition = ((2.33*labelCount) + (0.1*labelCount)) + (0.25*labelCount);
                    yPosition = templateGrid[labelCount];
                } else {
                    doc.addPage();
                    labelCount = 0;
                    yPosition = 0.0;
                }
            } else {
                doc.addPage();
            }
        }
    }

    function GuidToBase64(str) {

        str = str.replace(/-/g, "");

        return "PST--" + btoa(String.fromCharCode.apply(null,
              str.replace(/\r|\n/g, "").replace(/([\da-fA-F]{2}) ?/g, "0x$1 ").replace(/ +$/, "").split(" "))
            );
    }
});