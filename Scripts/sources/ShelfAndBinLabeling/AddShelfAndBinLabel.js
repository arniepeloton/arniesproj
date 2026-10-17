require([
    'common',
    GetPathFormat('/Scripts3/Browser/LibraryFowst.js?crc=')
], function () {

    var BinOrder = [];
    var BinOrderCount = 0;

    require([(OfflineMode ? '/Scripts3/Offline/Browser/LibraryFowst.js?crc=' : '')], function () {

        $(function () {
            var IsHasArea = false;

            if (OfflineMode) {
                GetFowst();
            }
            else {
                GetFowst();
            }

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

                    $('#txtRack').val('');
                    $('#txtRack').removeAttr('readonly', '');


                    $('#chkAisle').prop('checked', false).checkboxradio('refresh');

                    AddToBinOrder('Rack');
                    PreviewSampleFormat('Rack');
                } //else {
                //    $('#txtRack').attr('readonly', '');
                //    RemoveFromBinOrder('Rack');
                //    PreviewSampleFormat('Rack');
                //}
            });
            
            $('#chkAisle').click(function () {
                if ($('#chkAisle').is(':checked')) {

                    $('#txtRack').val('L/R (Left Or Right)');
                    $('#txtRack').attr('readonly', '');

                    $('#chkRack').prop('checked', false).checkboxradio('refresh');

                    AddToBinOrder('Aisle');
                    PreviewSampleFormat('Aisle');
                } //else {
                //    $('#txtAisle').attr('readonly', '');
                //    RemoveFromBinOrder('Aisle');
                //    PreviewSampleFormat('Aisle');
                //}
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
                if ($('#selFOWST').val() == "") {
                    MessageBox('Please select FOWST');
                    return false;
                } else {
                	PopupButtonList('', 'Paper Options', [
		                {
		                    HasImage: true,
		                    Text: '<img src="/Content/images/print.png" align="absmiddle" class="ui-li-thumb" style="width:50%;"/> <b>Desktop Printer</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width: 100%;"><ul><li><strong>Description: </strong>Multiple labels will be printed <br /> in one sheet using a desktop printer.</li><li><strong>Label Size: </strong>23.30mm x 88.90mm</li><li><strong>Paper Size: </strong>4.53in x 12.43in</li><li><strong>Number of labels per sheet: </strong>12</li></ul></div>',
		                    Click: function () {
		                        sessionStorage.setItem('paperSize',"smallerSize");
		                        GenerateBins();
		                        //GenerateQRCodeToPDF();
		                    }
		                },
		                {
		                    HasImage: true,
		                    Text: '<img src="/Content/images/excel.png" align="absmiddle" class="ui-li-thumb" style="width:50%;"/> <b>Barcode Printer</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width:100%;"><ul><li><strong>Description: </strong>Each label will be printed <br /> using a barcode printer or a <br />special purpose printer.</li><li><strong>Label Size: </strong>38.1mm x 76.2mm</li><li><strong>Paper Size: </strong>N/A</li><li><strong>Number of labels per sheet: </strong>N/A</li></ul></div>',
		                    Click: function () {
		                        sessionStorage.setItem('paperSize',"largerSize");
		                        GenerateBins();
		                        //GenerateQRCodeToPDF();
		                    }
		                }
		            ], { AutoDismiss: true });
                }
            });

            $('#btnCancel').click(function () {
                location = MergeUrlParameters({
                    OrgID: sessionStorage.OrgID,
                    IsMobile: CBool(IsMobile) ? 1 : 0
                }, '/ShelfAndBinLabeling');
            })
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
            BinOrder[BinOrderCount][1] = '1';
        }
        else if (mFormat == 'Side') {
            //else if mFormat = side (L/R Left or Right)
            BinOrder[BinOrderCount][1] = 'L';
        }
        else {
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

    function GenerateBins() {
        var NewBinOrder = []; //useless sa sa karonn
        var NewBinOrderCounter = 0;
        var TotalBinLabel = '';

        var BinName = '';
        var BinNames = [];
        var BinNamesCounter = 0;

        var mMax;
        var mValue;

        if ($('#chkArea').is(':checked')) {
            for (var a = 1; a <= parseInt($('#txtArea').val()); a++) {
                if ($('#chkRack').is(':checked') || $('#chkAisle').is(':checked')) {

                    if ($('#chkAisle').is(':checked')) {
                        mMax = 2;
                    } else {
                        mMax = parseInt($('#txtRack').val());
                    }

                    for (var b = 1; b <= mMax; b++) {

                        if ($('#chkAisle').is(':checked')) {
                            if (b == 1) {
                                mValue = 'L';
                            } else {
                                mValue = 'R';
                            }                            
                        } else {
                            mValue = b;
                        }

                        if ($('#chkSection').is(':checked')) {
                            for (var c = 1; c <= parseInt($('#txtSection').val()); c++) {
                                if ($('#chkShelf').is(':checked')) {
                                    for (var d = 1; d <= parseInt($('#txtShelf').val()); d++) {
                                        BinNames[BinNamesCounter] = a + '.' + mValue + '.' + c + '.' + d;
                                        BinNamesCounter++;
                                    }
                                } //end Rack Shelf
                            }
                        } //end Rack Section
                    }
                } //end Rack 
            }
        } //end Area
        else if ($('#chkRack').is(':checked') || $('#chkAisle').is(':checked')) {

            if ($('#chkAisle').is(':checked')) {
                mMax = 2;
            } else {
                mMax = parseInt($('#txtRack').val());
            }

            for (var b = 1; b <= mMax; b++) {

                if ($('#chkAisle').is(':checked')) {
                    if (b == 1) {
                        mValue = 'L';
                    } else {
                        mValue = 'R';
                    }                            
                } else {
                    mValue = b;
                }            
                if ($('#chkSection').is(':checked')) {
                    for (var c = 1; c <= parseInt($('#txtSection').val()); c++) {
                        if ($('#chkShelf').is(':checked')) {
                            for (var d = 1; d <= parseInt($('#txtShelf').val()); d++) {
                                BinNames[BinNamesCounter] = mValue + '.' + c + '.' + d;
                                BinNamesCounter++;
                            }
                        } //end Rack Shelf
                    }
                } //end Rack Section
            }
        }
        else if ($('#chkSection').is(':checked')) {
            for (var c = 1; c <= parseInt($('#txtSection').val()); c++) {
                if ($('#chkShelf').is(':checked')) {
                    for (var d = 1; d <= parseInt($('#txtShelf').val()); d++) {
                        BinNames[BinNamesCounter] = c + '.' + d;
                        BinNamesCounter++;
                    }
                } //end Rack Shelf
            }
        }
        else if ($('#chkShelf').is(':checked')) {
            for (var d = 1; d <= parseInt($('#txtShelf').val()); d++) {
                BinNames[BinNamesCounter] = c + '.' + d;
                BinNamesCounter++;
            }
        }


        var BinID = [];
        var FowstID = $('#selFOWST').val();
        var binCount = 0;
        
        var binIds = [];
        for (var i = 0; i <= BinNames.length; i++) {
	        ID = NewID();
	        binIds.push(ID);
        }
        
        var data = {
            BinID: binIds,
            FowstID: FowstID,
            BinNames: BinNames,
        };
		
		AjaxSend({
            url: 'FowstBin/SaveBinBatch',
            data: data,
            success: function (vData) {
            	
            	for (var i = 0; i <= vData.BinName.length; i++) {
			        if (vData.BinName[i] != null) {
				        BinID[binCount] = [];
				        BinID[binCount][0] = vData.BinID[i];
				        BinID[binCount][1] = vData.BinName[i];
				        binCount++;
			        }
			    }
		            
		        sessionStorage.BinIDs = JSON.stringify(BinID);
			    sessionStorage.BinOrder = JSON.stringify(BinOrder);
			    var f = window.open('GeneratePDF', '_blank');
	
	        	MessageBox('Your Bins has been saved. Were you able to download the PDF file?',
	                        "Save",
	                        [
	                            {
	                                Text: 'Yes',
	                                Click: function () {
	                                    var f = window.open('../ShelfAndBinLabeling', '_self');
	                                }
	                            },
	                            {
	                                Text: 'No',
	                                Click: function () {
	                                    var f = window.open('GeneratePDF', '_blank');
	                                }
	                            }
	            ]); // END MessageBox
                   
            }
        });

        

        //CreateBins(BinNames);
    }

    function SaveBin(data) {

        if (OfflineMode) {

            var mResult = InventoryCount.SaveBin(data);
            if (mResult) {
                $.mobile.changePage(window.pageSourcePage);
            }
        }
        else {
            AjaxSend({
                url: 'FowstBin/SaveBin',
                data: data,
                success: function (vData) {

                }
            });
        }
    }

    function GetFowst() {
        var data = { FilterFowst: 1 }
        AjaxSend({
            url: 'Browser/LibraryFowst',
            data: data,
            success: function (vData) {
                DisplayFowst(vData.Results);
            }
        });
    }

    function DisplayFowst(vData) {
        $('#selFOWST').empty();
        $('#selFOWST').append(new Option('Please Select', '', false));

        for (var i = 0; i <= vData.Rows.length - 1; i++) {
            $('#selFOWST').append(new Option(vData.Rows[i].FowstName, vData.Rows[i].ID, false));
        }

        try {
            $('#selFOWST').selectmenu('refresh');
        }
        catch (e) {
            $('#selFOWST').selectmenu();
        }
    }

});