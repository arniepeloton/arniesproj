require(['/Scripts/PrintPDF/config.js', '/Scripts/PrintPDF/zlib.js', '/Scripts/PrintPDF/png.js'], function () {
    require(['/Scripts/PrintPDF/html2pdf.js'], function () {
        require(['/Scripts/PrintPDF/jspdf.js'], function () {
            require(['/Scripts/PrintPDF/html2canvas.js'], function () {
                require(['/Scripts/PrintPDF/addhtml.js'], function () {
                    require(['/Scripts/PrintPDF/addimage.js'], function () {
                        require(['/Scripts/PrintPDF/png_support.js'], function () {
                            require(['/Scripts/PrintPDF/FileSaver.js'], function () {
                                require(['/Scripts/PrintPDF/split_text_to_size.js'], function () {
                                    require(['/Scripts/PrintPDF/standard_fonts_metrics.js'], function () {
                                        require(['/Scripts/sources/crm/qrcode.js'], function () {
                                            $(document).ready(function () {
                                                $('#btnPrint').click(function () {
                                                    //                                    var pdf = new jsPDF('p', 'px', 'a4');
                                                    //                                    var canvas = pdf.canvas;
                                                    //                                    //canvas.width=8.5*72;

                                                    //                                    pdf.addHTML($('#fromHTMLtestdiv'), function () {
                                                    //                                        pdf.output('dataurlnewwindow');
                                                    //                                    });


                                                    //pdf.output('dataurlnewwindow');
                                                })

                                                window.PrintPDF = function (headerDetails, bodyDetails) {
                                                    var pdf = new jsPDF('p', 'pt', 'a4');
                                                    var canvas = pdf.canvas;
                                                    var CurNoOfNewLine = 0;
                                                    var PrevNoOfNewLine = 0;

                                                    $('body').append('<div id="divQRCode"></div>');

                                                    //Create qrcode first
                                                    for (var h = 0; h < headerDetails.operation.length; h++) {
                                                        GenerateQRCode(h);
                                                    }

                                                    //setTimeout because it needs the qrcode to be loaded first so we can get the dataUri
                                                    setTimeout(function () {
                                                        for (var h = 0; h < headerDetails.operation.length; h++) {

                                                            var rowYPosition = 410;
                                                            var bodyDetailsData = bodyDetails.data[1].rows;
                                                            var total = 0;
                                                            var maxRow = 0;
                                                            var pageNo = 1;

                                                            DisplayHeader(pdf, h, pageNo, $('#qr_' + h).find('img').attr('src'));

                                                            for (var i = 0; i < bodyDetailsData[h].length; i++) {
                                                                var rowDetails = bodyDetailsData[h][i];
                                                                pdf.setFontSize(11);
                                                                //pdf.setFontType('bold');
                                                                pdf.text(AddNewlines(rowDetails.ItemCode.toString(), 13), 30, rowYPosition);
                                                                PrevNoOfNewLine = CurNoOfNewLine;

                                                                pdf.setFontSize(11);
                                                                //pdf.setFontType('bold');
                                                                pdf.text(AddNewlines(rowDetails.ItemDescription.toString(), 30), 133, rowYPosition);

                                                                if (CurNoOfNewLine > PrevNoOfNewLine) {
                                                                    PrevNoOfNewLine = CurNoOfNewLine;
                                                                }

                                                                pdf.setFontSize(11);
                                                                //pdf.setFontType('bold');
                                                                pdf.text(AddNewlines(rowDetails.Qty.toString(), 6), 330, rowYPosition);

                                                                if (CurNoOfNewLine > PrevNoOfNewLine) {
                                                                    PrevNoOfNewLine = CurNoOfNewLine;
                                                                }

                                                                pdf.setFontSize(11);
                                                                //pdf.setFontType('bold');
                                                                pdf.text(AddNewlines(rowDetails.UoM.toString(), 8), 376, rowYPosition);

                                                                if (CurNoOfNewLine > PrevNoOfNewLine) {
                                                                    PrevNoOfNewLine = CurNoOfNewLine;
                                                                }

                                                                pdf.setFontSize(11);
                                                                //pdf.setFontType('bold');
                                                                pdf.text(AddNewlines(AccountingFormat(rowDetails.Price).toString(), 10), 438, rowYPosition);

                                                                if (CurNoOfNewLine > PrevNoOfNewLine) {
                                                                    PrevNoOfNewLine = CurNoOfNewLine;
                                                                }

                                                                pdf.setFontSize(11);
                                                                //pdf.setFontType('bold');
                                                                pdf.text(AddNewlines(AccountingFormat(CDec2(rowDetails.Qty) * CDec2(rowDetails.Price)).toString(), 10), 508, rowYPosition);

                                                                if (CurNoOfNewLine > PrevNoOfNewLine) {
                                                                    PrevNoOfNewLine = CurNoOfNewLine;
                                                                }

                                                                rowYPosition = rowYPosition + 15;

                                                                total = total + (CDec2(rowDetails.Qty) * CDec2(rowDetails.Price));

                                                                if (PrevNoOfNewLine >= CurNoOfNewLine) {
                                                                    if (PrevNoOfNewLine == 1) {
                                                                        rowYPosition = rowYPosition;
                                                                    }
                                                                    else if (PrevNoOfNewLine == 2) {
                                                                        rowYPosition = rowYPosition + 15;
                                                                        maxRow = maxRow + 1;
                                                                    }
                                                                    else if (PrevNoOfNewLine == 3) {
                                                                        rowYPosition = rowYPosition + 30;
                                                                        maxRow = maxRow + 2;
                                                                    }
                                                                    else if (PrevNoOfNewLine == 4) {
                                                                        rowYPosition = rowYPosition + 45;
                                                                        maxRow = maxRow + 3;
                                                                    }
                                                                    else if (PrevNoOfNewLine == 5) {
                                                                        rowYPosition = rowYPosition + 60;
                                                                        maxRow = maxRow + 4;
                                                                    }
                                                                    else if (PrevNoOfNewLine == 6) {
                                                                        rowYPosition = rowYPosition + 75;
                                                                        maxRow = maxRow + 5;
                                                                    }
                                                                    else if (PrevNoOfNewLine == 7) {
                                                                        rowYPosition = rowYPosition + 90;
                                                                        maxRow = maxRow + 6;
                                                                    }
                                                                    else if (PrevNoOfNewLine == 8) {
                                                                        rowYPosition = rowYPosition + 105;
                                                                        maxRow = maxRow + 7;
                                                                    }
                                                                    else if (PrevNoOfNewLine == 9) {
                                                                        rowYPosition = rowYPosition + 120;
                                                                        maxRow = maxRow + 8;
                                                                    }
                                                                    else if (PrevNoOfNewLine == 10) {
                                                                        rowYPosition = rowYPosition + 135;
                                                                        maxRow = maxRow + 9;
                                                                    }
                                                                }
                                                                else {
                                                                    rowYPosition = rowYPosition;
                                                                }

                                                                if (maxRow > 23) {
                                                                    pageNo = pageNo + 1;
                                                                    pdf.addPage();
                                                                    DisplayHeader(pdf, h, pageNo, imgSrcData);
                                                                    rowYPosition = 410;
                                                                    maxRow = 0;
                                                                }

                                                                if (i == bodyDetailsData[h].length - 1) {

                                                                    //                                                        pdf.setLineWidth(1.5);
                                                                    //                                                        pdf.line(560, rowYPosition, 430, 425);

                                                                    pdf.setFontSize(14);
                                                                    //pdf.setFontType('normal');
                                                                    pdf.text('TOTAL: ' + AccountingFormat(total), 430, (rowYPosition + 15));

                                                                    pdf.setFontSize(12);
                                                                    //pdf.setFontType('normal');
                                                                    pdf.text('PO Created By: __________________  PO Approved By: __________________', 30, 810);
                                                                }

                                                                maxRow = maxRow + 1;
                                                                //PrevNoOfNewLine = CurNoOfNewLine;
                                                            }

                                                            //////////////////// END TABLE

                                                            pdf.addPage();
                                                        }

                                                        //                                                        html2canvas(document.body, {
                                                        //                                                            canvas: canvas,
                                                        //                                                            onrendered: function (canvas) {
                                                        //                                                                var iframe = document.createElement('iframe');
                                                        //                                                                iframe.setAttribute('style', 'position:absolute;top:0;right:0;height:100%; width:800px; z-index:1000;');
                                                        //                                                                document.body.appendChild(iframe);
                                                        //                                                                iframe.src = pdf.output('datauristring');
                                                        //                                                            }
                                                        //                                                        });

                                                        pdf.save('PO_' + $.format.date(new Date(), 'MMM_dd_yyyy') + '.pdf');
                                                    }, 100)

                                                    function GenerateQRCode(h) {
                                                        var divQRCode = $('#divQRCode');
                                                        var divQRid = 'qr_' + h;
                                                        var qrcode;
                                                        var imgData;

                                                        if (divQRCode.find('div[id="' + divQRid + '"]').length > 0) {
                                                            imgData = $('#' + divQRid + ' img').attr('src');
                                                        }
                                                        else {
                                                            divQRCode.append($('<div>').attr('id', divQRid));
                                                            qrcode = new QRCode(document.getElementById(divQRid), {
                                                                width: 100,
                                                                height: 100
                                                            });

                                                            qrcode.makeCode(headerDetails.operation[h][20].qrcode);
                                                            imgData = $('#' + divQRid + ' img').attr('src');
                                                            return qrcode;
                                                        }
                                                    }

                                                    function DisplayHeader(pdf, h, pageNo, imgSrc) {
                                                        pdf.setFontSize(22);
                                                        //pdf.setFontType('bold');
                                                        pdf.text(headerDetails.operation[h][9].orgNameRaw, 30, 50);

                                                        pdf.setFontSize(22);
                                                        //pdf.setFontType('bold');
                                                        pdf.text('Purchase Order', 365, 50);

                                                        pdf.setFontSize(12);
                                                        //pdf.setFontType('normal');
                                                        pdf.text(headerDetails.operation[h][10].orgAddressRaw, 30, 70);

                                                        pdf.setFontSize(12);
                                                        //pdf.setFontType('normal');headerDetails.operation[h][2].docNum
                                                        pdf.text('PO Number: ' + headerDetails.operation[h][2].docNum + '\rPO Date: ' + headerDetails.operation[h][3].docTypeDate + '\rDelivery Date: ' + headerDetails.operation[h][6].deliveryDate, 550, 70, 'right');

                                                        pdf.setFontSize(12);
                                                        //pdf.setFontType('normal');
                                                        pdf.text(AddNewlines('Supplier: ' + headerDetails.operation[h][11].custNameRaw, 40) + AddNewlines(headerDetails.operation[h][12].custAddressRaw, 50), 30, 120);

                                                        pdf.addImage(imgSrc, 'JPEG', 365, 115, 55, 55, 'PO No.: ' + headerDetails.operation[h][2].docNum);

                                                        pdf.rect(30, 180, 170, 170);
                                                        pdf.rect(212, 180, 170, 170);
                                                        pdf.rect(392, 180, 170, 170);

                                                        pdf.setFontSize(12);
                                                        //pdf.setFontType('bold');
                                                        pdf.text('BILLING DETAILS', 35, 195);

                                                        pdf.setFontSize(12);
                                                        //pdf.setFontType('bold');
                                                        pdf.text('DELIVERY DETAILS', 215, 195);

                                                        pdf.setFontSize(12);
                                                        //pdf.setFontType('bold');
                                                        pdf.text('DELIVERY INSTRUCTIONS', 395, 195);

                                                        pdf.setFontSize(12);
                                                        //pdf.setFontType('bold');
                                                        pdf.text('Billing Address', 35, 215);

                                                        pdf.setFontSize(12);
                                                        //pdf.setFontType('bold');
                                                        pdf.text('Delivery Address', 215, 215);

                                                        pdf.setFontSize(12);
                                                        //pdf.setFontType('normal');
                                                        pdf.text(AddNewlines((headerDetails.operation[h][19].deliveryRemarksRaw == "" ? " " : headerDetails.operation[h][19].deliveryRemarksRaw), 26), 395, 215);

                                                        pdf.setFontSize(11);
                                                        //pdf.setFontType('normal');
                                                        pdf.text(AddNewlines(headerDetails.operation[h][13].billingAddressDetailsRaw, 31), 35, 235);

                                                        pdf.setFontSize(11);
                                                        //pdf.setFontType('normal');
                                                        pdf.text(AddNewlines(headerDetails.operation[h][16].deliveryAddressDetailsRaw, 31), 215, 235);

                                                        pdf.setFontSize(12);
                                                        //pdf.setFontType('bold');
                                                        pdf.text('Contact No:', 35, 310);

                                                        pdf.setFontSize(12);
                                                        //pdf.setFontType('bold');
                                                        pdf.text('Contact No:', 215, 310);

                                                        pdf.setFontSize(11);
                                                        //pdf.setFontType('normal');
                                                        pdf.text(AddNewlines(headerDetails.operation[h][14].billingContactDetailsRaw, 31), 35, 325);

                                                        pdf.setFontSize(11);
                                                        //pdf.setFontType('normal');
                                                        pdf.text(AddNewlines(headerDetails.operation[h][17].deliveryContactDetailsRaw, 31), 215, 325);

                                                        pdf.setFontSize(11);
                                                        //pdf.setFontType('bold');
                                                        pdf.text('Item Code                 Item Description                                       Qty         UoM         Unit Price       Sub Total', 30, 385);

                                                        pdf.setLineWidth(1);
                                                        pdf.line(30, 390, 560, 390);

                                                        pdf.setFontSize(12);
                                                        //pdf.setFontType('normal');
                                                        pdf.text('| Page ' + pageNo, 510, 810);

                                                        ////////////////////// TABLE ROWS
                                                    }
                                                }

                                                window.WordWrapPDF = function (str, maxChar) {
                                                    return AddNewlines(str, maxChar);
                                                }

                                                function AddNewlines(str, maxChar) {
                                                    if (str != undefined) {
                                                        CurNoOfNewLine = 0;
                                                        var result = '';
                                                        while (str.length > 0) {
                                                            result += str.substring(0, maxChar) + '\n';
                                                            str = str.substring(maxChar);
                                                        }
                                                        CurNoOfNewLine = result.split("\n").length - 1;
                                                        return result;
                                                    }
                                                    else {
                                                        return "";
                                                    }
                                                }
                                            })
                                        })
                                    })
                                })
                            });
                        });
                    })
                });
            });
        });
    });
});