
$(function () {
    var mDocTitle = '';

    $(function () {
        GenerateQRCodeToPDF();
    });
	var labelCount = 0;
	var yPosition = 0.0;
	var templateGrid = [0.0, 2.53, 5.06, 7.59, 10.12, 12.65, 15.18, 17.71, 20.24, 22.77, 25.3, 27.83];
    function GenerateQRCodeToPDF() {

        var BinIDs = JSON.parse(sessionStorage.BinIDs);
        var BinAreas = JSON.parse(sessionStorage.AreaLabel);

        BinCounTotal = BinIDs.length;

        var BinID;
        var BinName;
        var BinArea;

        var pLv = $('#QrCodeCanvass');

        //save
        for (var i = 0; i <= BinIDs.length - 1; i++) {
            
            var QRDiv = 'QR' + i + '';

            pLv.append('<br /> <div id="' + QRDiv + '">');

            if (sessionStorage.getItem("paperSize") == "smallerSize") {
                var qrcode = new QRCode(document.getElementById(QRDiv), {
                    width: 100,
                    height: 100
                });
            } else {
                var qrcode = new QRCode(document.getElementById(QRDiv), {
                    width: 100,
                    height: 100
                });
            }
            

            BinID = BinIDs[i][0];
            BinName = BinIDs[i][1];
            BinArea = BinAreas[i];

            qrcode.makeCode(GuidToBase64(BinID).replace('==', ''));

            //SaveBin(data);
            

            (function (vBinID, vBinName, BinArea, vQRDiv) {
                window.setTimeout(function () {
                    getImageFromUrl($('#' + vQRDiv + ' img').attr('src'), createPDF, vBinName, BinArea);
                }, 1000);
            })(BinID, BinName, BinArea, QRDiv);
        }
    }

    var BinCount = 0;
    var BinCounTotal = 0;
    var IsLastImage = false;
    
    var getImageFromUrl = function (url, callback, BinName, BinArea) {
        var img = new Image();

        img.onError = function () {
            alert('Cannot load image: "' + url + '"');
        };
        img.onload = function () {
            callback(img, BinName, BinArea);
        };
        img.setAttribute('crossOrigin', 'anonymous');
        img.src = url;
    }
    
    //Create the document and configure the size
    if (sessionStorage.getItem("paperSize") == "largerSize") {
        var doc = new jsPDF('l', 'cm', [3.81, 7.62]);
    }    
    else if (sessionStorage.getItem("paperSize") == "smallerSize") {
        var doc = new jsPDF('l', 'cm', [2.54, 5.08]);
    }
    else if (sessionStorage.getItem("paperSize") == "mediumSize") {
        var doc = new jsPDF('l', 'cm', [2.5, 9]);
    }
    else {
        var doc = new jsPDF('p', 'cm', [31.57, 11.49]);
        //var doc = new jsPDF('l', 'cm', [2.33, 8.89]);
    }
    //var doc = new jsPDF();
    
    var createPDF = function (imgData, BinName, BinArea) {
    	
        var BinOrder = [];
        //var yPosition = 0;
        var xLocation = 0;
        if (sessionStorage.getItem("paperSize") == "mediumSize") {
            ypos = 1.2;
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
                doc.addImage(imgData, 'JPEG', 0.5, 0.5, 2.5, 2.5, BinName);
            }
            else if (sessionStorage.getItem("paperSize") == "mediumSize") {
                doc.addImage(imgData, 'JPEG', 0.25, 0.25, 1, 1, BinName);
            }
            else if (sessionStorage.getItem("paperSize") == "smallerSize") {
                doc.addImage(imgData, 'JPEG', 0.25, 0.25, 2, 2, BinName);
            }
            else {
                yPosition = yPosition + 0.75;
                doc.addImage(imgData, 'JPEG', 1.6, yPosition, 1.5, 1.5, BinName);
            }

            if (sessionStorage.getItem("paperSize") == "largerSize") {
                var yLocation = 1;
            }
            else if (sessionStorage.getItem("paperSize") == "smallerSize") {
                yPosition = .2;
                xLocation = 2.3;
            }
            else {
                xLocation = 3.4;
            }
        }

        var BinNameArr = BinName.split('.')
        var binAlign = 0;
                
        //bin order should be dynamic depends on what BIN the user selected in Shelf and Bin Labelling
        if (BinNameArr.length == 3) {
            BinOrder[0] = [];
            BinOrder[0][0] = 'Rack';
            BinOrder[1] = [];
            BinOrder[1][0] = 'Section';
            BinOrder[2] = [];
            BinOrder[2][0] = 'Shelf';
        } else {
            BinOrder[0] = [];
            BinOrder[0][0] = 'Aisle';
            BinOrder[1] = [];
            BinOrder[1][0] = 'Side';
            BinOrder[2] = [];
            BinOrder[2][0] = 'Section';
            BinOrder[3] = [];
            BinOrder[3][0] = 'Shelf';
        }

        if (sessionStorage.getItem("paperSize") == "mediumSize") {
            for (var barpos = 0; barpos < 3; barpos++) {
                xpos = 0;
                ypos = 0.2;

                if (barpos == 0) {
                    doc.setFontSize(6);

                    doc.text(xpos + .3, ypos + .2, 'Bin Address Tag (BAT)');
                    doc.setFontSize(3);
                    doc.text(xpos + .31, ypos + .33, 'FOWST Location');
                    doc.setFontSize(6);
                    doc.text(xpos + .3, ypos + .51, sessionStorage.FowstName);
                    doc.setFontSize(3);
                    doc.text(xpos + .31, ypos + .65, 'Area');
                    doc.setFontSize(6);
                    doc.text(xpos + .3, ypos + .85, BinArea || '');

                } else if (barpos == 1) {
                    xpos += 3;
                    doc.setFontSize(6);

                    doc.text(xpos + .3, ypos + .2, 'Bin Address Tag (BAT)');
                    doc.setFontSize(3);
                    doc.text(xpos + .31, ypos + .33, 'FOWST Location');
                    doc.setFontSize(6);
                    doc.text(xpos + .3, ypos + .51, sessionStorage.FowstName);
                    doc.setFontSize(3);
                    doc.text(xpos + .31, ypos + .65, 'Area');
                    doc.setFontSize(6);
                    doc.text(xpos + .3, ypos + .85, BinArea || '');
                } else {
                    xpos += 6;
                    doc.setFontSize(6);

                    doc.text(xpos + .3, ypos + .2, 'Bin Address Tag (BAT)');
                    doc.setFontSize(3);
                    doc.text(xpos + .31, ypos + .33, 'FOWST Location');
                    doc.setFontSize(6);
                    doc.text(xpos + .3, ypos + .51, sessionStorage.FowstName);
                    doc.setFontSize(3);
                    doc.text(xpos + .31, ypos + .65, 'Area');
                    doc.setFontSize(6);
                    doc.text(xpos + .3, ypos + .85, BinArea || '');
                }
            }
        }
        else if (!(sessionStorage.getItem("paperSize") == "smallerSize")) {
            yPosition = yPosition + 1.25;            
        } else {
            doc.setFontSize(6.2);
            doc.text(xLocation + .3, yPosition + .25, 'Bin Address Tag (BAT)');

            doc.setFontSize(3);
            doc.text(xLocation + .31, yPosition + .38, 'FOWST Location');

            doc.setFontSize(6.2);
            doc.text(xLocation + .3, yPosition + .60, sessionStorage.FowstName);

            doc.setFontSize(3);
            doc.text(xLocation + .31, yPosition + .75, 'Area');

            doc.setFontSize(6.2);
            doc.text(xLocation + .3, yPosition + .97, BinArea || '');
            
            yPosition = yPosition + 2;
        }
		
        //yPosition = yPosition + 1.25;
        if (sessionStorage.getItem("paperSize") == "mediumSize") {
            xLocation = 1.2;
            yPosition = 2;

            for (var barpos = 0; barpos < 3; barpos++) {
                xpos = 0;
                ypos = 0.2;

                for (var i = 0; i < BinOrder.length; i++) {
                    doc.setFontSize(3);

                    //For adjusting the section and rack label to fir into the paper
                    //gigamit ang xLocation sa baba na code so dili naku sya machange
                    if (i == 1) {
                        xLocation = xLocation + 0.1;
                        doc.text(xLocation - .1, yPosition, BinOrder[i][0]);
                    } else if (i == 2) {
                        xLocation = xLocation - 0.1;
                        doc.text(xLocation - 0.05, yPosition, BinOrder[i][0]);
                    } else if (i == 3) {
                        //xLocation = xLocation + 0.1;
                        doc.text(xLocation + .03, yPosition, BinOrder[i][0]);
                    } else {
                        xLocation = xLocation + 0.3;
                        doc.text(xLocation, yPosition, BinOrder[i][0]);
                    }

                    //For center alignment of the numbers
                    if (BinNameArr[i].length == 2) {
                        binAlign = -0.05;
                    } else if (BinNameArr[i].length == 1) {
                        if (i == 2) {
                            binAlign = 0.07;
                        } else if (i == 1) {
                            binAlign = -0.05;
                        } else {
                            binAlign = 0.05;
                        }
                    } else {
                        if (i == 2) {
                            binAlign = -0.01;
                        } else {
                            binAlign = -0.05;
                        }
                    }

                    doc.setFontSize(7);
                    doc.text(xLocation + binAlign, yPosition - 0.25, BinNameArr[i]);
                    xLocation = xLocation + .30;
                }

                xLocation += 1.5;
            }
           
        } else {
            for (var i = 0; i < BinOrder.length; i++) {
                if (sessionStorage.getItem("paperSize") == "smallerSize") {
                    doc.setFontSize(3);

                    //For adjusting the section and rack label to fir into the paper
                    if (i == 1) {
                        xLocation = xLocation + 0.1;
                    } else if (i == 2) {
                        xLocation = xLocation - 0.1;
                    } else if (i == 3) {
                        //xLocation = xLocation + 0.1;
                    } else {
                        xLocation = xLocation + 0.3;
                    }

                    doc.text(xLocation, yPosition, BinOrder[i][0]);

                    //For center alignment of the numbers
                    if (BinNameArr[i].length == 2) {
                        binAlign = -0.05;
                    } else if (BinNameArr[i].length == 1) {
                        if (i == 2) {
                            binAlign = 0.07;
                        } else if (i == 1) {
                            binAlign = -0.05;
                        } else {
                            binAlign = 0.05;
                        }
                    } else {
                        if (i == 2) {
                            binAlign = -0.01;
                        } else {
                            binAlign = -0.05;
                        }
                    }

                    doc.setFontSize(10);
                    doc.text(xLocation + binAlign, yPosition - 0.25, BinNameArr[i]);
                    xLocation = xLocation + .65;
                }
                else if (!(sessionStorage.getItem("paperSize") == "largerSize")) {
                    doc.setFontSize(14);

                    //For adjusting the section and rack label to fir into the paper
                    if (i == 1 || i == 2) {
                        xLocation = xLocation - 0.3;
                    }
                    doc.text(xLocation + 0.17, yPosition, BinOrder[i][0]);

                    //For center alignment of the numbers
                    if (BinNameArr[i].length == 2) {
                        if (i == 2) {
                            binAlign = 0.6;
                        } else {
                            binAlign = 0.3;
                        }
                    } else if (BinNameArr[i].length == 1) {
                        if (i == 2) {
                            binAlign = 0.8;
                        } else {
                            binAlign = 0.5;
                        }
                    } else {
                        if (i == 2) {
                            binAlign = 0.4;
                        } else {
                            binAlign = 0.1;
                        }
                    }

                    doc.text(xLocation + binAlign, yPosition - 0.65, BinNameArr[i]);
                    xLocation = xLocation + 2.0;
                }
                else {

                    //doc.text(3.5, yLocation, BinNameArr[i]);
                    //doc.text(3.5, yLocation, BinOrder[i][0]);
                    //doc.text(6.5, yLocation, BinNameArr[i]);
                    //yLocation = yLocation + 0.6;

                    //Bin Letters/Numbers
                    doc.setFontSize(9);
                    doc.text(3.35, 0.75, "BIN LOCATION TAG (BLT)");

                    doc.setFontSize(6);
                    doc.text(3.35, 1.25, sessionStorage.FowstName);

                    doc.setFontSize(6);
                    doc.text(3.35, 1.70, BinArea);

                    doc.setFontSize(8);
                    doc.text(3.35, 1.85, "_________________________");
                    

                    doc.setFontSize(14);
                    doc.text(xLocation + 3.5, yLocation + 1.50, parseInt(BinNameArr[i]).toString());

                    //Bin Letters/numbers Assignment
                    doc.setFontSize(8);

                    if (BinOrder[i][0] == 'Section') {
                        doc.text(xLocation + 3.35, yLocation + 2.00, 'Sctn');
                    } else
                        doc.text(xLocation + 3.35, yLocation + 2.00, BinOrder[i][0]);

                    xLocation = xLocation + 1;
                }
            }
        }
        // As you can guess, using the cached image reduces the generated PDF size by 50%!
        
        BinCount++;
        if (BinCount == BinCounTotal) {
            doc.save('ShelfLabels.pdf');
            setTimeout(function () {
                window.close();
            }, 1000);            
            
        } else {
            if (sessionStorage.getItem("paperSize") == "largerSize" || sessionStorage.getItem("paperSize") == "smallerSize" || sessionStorage.getItem("paperSize") == "mediumSize") {
	        	doc.addPage();
          	} else {
          		if (labelCount < 11) {
	        		labelCount = labelCount + 1;
	        		//yPosition = ((2.33*labelCount) + (0.1*labelCount)) + (0.25*labelCount);
	        		yPosition = templateGrid[labelCount];
	        	} else {
	            	doc.addPage();
	            	labelCount=0;
	            	yPosition=0.0;
	           }
          	}
        }
    }
        
    function GuidToBase64(str) {

        str = str.replace(/-/g, "");

        return "BLT--" + btoa(String.fromCharCode.apply(null,
              str.replace(/\r|\n/g, "").replace(/([\da-fA-F]{2}) ?/g, "0x$1 ").replace(/ +$/, "").split(" "))
            );
    } 
    //function GetUrlParameters(vUrl) {
    //    // PARAMS:
    //    //  vUrl: [optional]

    //    vUrl = vUrl || window.location.search.substring(1);
    //    var urlParams = {};
    //    var match,
    //        pl = /\+/g,  // Regex for replacing addition symbol with a space
    //        search = /([^&=]+)=?([^&]*)/g,
    //        decode = function (s) { return decodeURIComponent(s.replace(pl, " ")); },
    //        query = vUrl;

    //    while (match = search.exec(query))
    //        urlParams[decode(match[1])] = decode(match[2]);

    //    return urlParams;

    //}

});