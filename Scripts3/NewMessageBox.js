define([
    'common'//, 'datatables.net'
    
    ], function () {

    var mDeferred = $.Deferred();

    require([
        '/Scripts/jquery.widget.min.js'
    ], function () {
        require([
            '/Scripts/metro.min.js'
        ], function () {
            require([
                '/Scripts/metro-calendar.js',
                '/Scripts/metro-datepicker.js'
                , //'/Scripts/jquery.dataTables.min.js?crc=',   
                '/Scripts/dataTables.fixedHeader.min.js'
            ], function () {
                require(['/Scripts/jquery.dataTables.buttons.min.js',
                    '/Scripts/dataTables.rowReorder.min',
                    '/Scripts/dataTables.responsive.min'
                ], function () {
                    require([//'/Scripts/jquery.dataTables.buttons.flash.min.js',
                        '/Scripts/jszip.min.js',
                        //'/Scripts/vfs_fonts.js',
                        //'/Scripts/pdfmake.min.js',                        
                        '/Scripts/buttons.html5.min.js',
                        //'/Scripts/buttons.print.min.js'
                    ], function () {
                        var closeFunction = '';
                        var mLastModifiedFrom;
                        var mLastModifiedTo;
                        var isCreatePopUp = false;
                        var cssOverLay = {
                            background: 'rgba(0, 0, 0, 0.5)',
                            bottom: '0',
                            left: '0',
                            position: 'fixed',
                            right: '0',
                            top: '0',
                            zIndex: '1099',
                            overflow: 'auto'
                        }
                        var page = $('body');
                        var popup = $('<div id="popupCustomMsgBox" data-role="popup" data-theme="a" data-dismissible="false" data-corners="false" style="visibility:hidden; display:none;max-width: 400px;">' +
                            '<a href="#" style="display:none;" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right btnCustomPopupClose">Close</a>' +
                            '<center><h3 style="margin-left:10px; margin-right:10px;" id="txtCustomMsgBoxTitle"></h3></center>' +
                            '</div>')
                            , content = $('<div data-role="content" class="ui-content ui-body-d" style="max-width:400px; word-wrap:break-word;"> ' +
                                '<div id="CustomMsgBoxtxtMessage" style="line-height:1.3">' +

                                '</div>' +

                                '<a data-role="none" id="btnCustomMsgBoxOK" style="white-space:normal; display:none;"></a>' +
                                '<div id="divNegativeButton" style="display:none;"><br><a data-role="none" id="btnCustomMsgBoxCancel" class="btn custom-btn-theme-c" style="white-space:normal;"></a></div>' +
                                '<div id="divForButtonNewMsgBox"></div>' +
                            '</div>').appendTo(popup);

                        var browsepopup = $('<div id="popupCustomBrowseLibrary" class="fittoscreen" data-role="popup" data-theme="a" data-dismissible="true" data-corners="false" style="visibility:hidden; display:none;">' +
                           '<a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right btnCustomPopupClose">Close</a>' +
                           '<center><h3 style="margin-left:10px; margin-right:10px;" id="txtCustomBrowseLibraryTitle"></h3></center>' +
                           '</div>')
                           , browsecontent = $('<div class="ui-body-d" style="word-wrap: break-word; max-width:500px;"> ' +
                                   '<div id="divClearBtn" style="display:none; text-align:right; padding:15px 15px 0px 15px;"><a data-role="none" class="custom-link" id="btnCustomClear">Clear</a></div>' +
                                   '<div style="display:none; padding:15px 15px 0px 15px;" id="divMsgErrorBrowseLibrary"></div>' +
                                   '<table style="width:100%;"><tr>' +
                                       '<td><div id="divBrowserTopText" style="margin:15px 15px 0 15px"></div><div style="display:block; padding:15px 15px 0px 15px;" id="divSearchBoxBrowseLibrary"><input type="search" id="txtBrowseLibrary" placeholder="Search here!"></div></td>' +
                                       '<td style="width:50px; display:none;" id="divBarcode"><div style="padding:25px 0px 11px 0px;">' +
                                           '<button data-role="none" title="Scan Barcode" id="btnBrowseScanBarcode" class="btn custom-btn-theme-a btn-less-width btn-xs">' +
                                               '<img alt="Barcode" class="img-barcode"/>' +
                                           '</button>' +
                                       '</div></td>' +
                                       '<td style="width:100px; display:none;" id="divGlobalAddSKU"><div style="padding:25px 0px 11px 0px;" >' +
                                           '<button data-role="none" title="Add SKU using Web Form" id="btnGlobalAddSKU" class="btn custom-btn-theme-a btn-less-width btn-sm">' +
                                               'Add SKU' +
                                           '</button>' +
                                       '</div></td></tr>' +
                                   '</table>' +
                                   '<table id="tblOption" style="display:block; padding:0px 15px 0px 15px;"></table>' +
                                   '<div id="divBrowseListFormat" style="padding: 0px 15px 15px 15px; overflow:auto; display:block">' +
                                       '<ul id="lstOfCustomBrowseLibrary" data-role="listview" class="li-theme-c noBorderList custom-list custom-list-sm">' +
                                           '<li><a>Loading ...</a></li>' +
                                       '</ul>' +
                                       '' +
                                   '</div>' +
                                   '<div id="divBrowseTableFormat" style="padding: 0px 15px 15px 15px; overflow:auto; display:block">' +
                                       '<br><table id="tblBrowsePopup" data-role="table" data-role="reflow" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">' +
                                           '<thead id="tblBrowsePopupHeader"></thead>' +
                                           '<tbody id="tblBrowsePopupBody"></tbody>' +
                                       '</table>' +
                                   '</div>' +
                           '</div><div class="divBrowseFooter" style="background-color:#FFF; overflow:hidden; padding:10px; display:none;"><button data-role="none" id="btnOkBrowseSelectMultiple" class="btn custom-btn-theme-b">Ok</button></div>').appendTo(browsepopup);

                        var listpopup = $('<div id="popupCustomList" class="fittoscreen" data-role="popup" data-theme="a" data-dismissible="true" data-corners="false" style="visibility:hidden; display:none;">' +
                            '<a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right btnCustomPopupClose">Close</a>' +
                            '<center><h3 style="margin-left:10px; margin-right:10px;" id="txtCustomListTitle"></h3></center>' +
                            '</div>')
                            , listcontent = $('<div class="ui-body-d" style="max-width:500px; word-wrap: break-word;"> ' +
                                    '<div class="ui-content" id="txtCustomListMsg" style="display:none;"></div>' +
                                    '<div id="divOfCustomList" style="padding: 0px 15px 15px 15px; overflow:auto;">' +
                                        '<ul id="lstOfCustomList" data-role="listview" class="li-theme-c noBorderList custom-list custom-list-sm">' +
                                        '</ul>' +
                                        '' +
                                    '</div>' +
                            '</div>').appendTo(listpopup);

                        var periodpopup = $('<div id="popupCustomPeriod" data-role="popup" data-theme="a" data-dismissible="true" data-corners="false" style="visibility:hidden; display:none;">' +
                            '<a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right btnCustomPopupClose">Close</a>' +
                            '<center><h3 style="margin-left:10px; margin-right:10px;" id="txtCustomPeriodTitle">Period</h3></center>' +
                            '</div>').appendTo(page).trigger('create')
                            , periodcontent = $('<div class="ui-body-d" style="max-width:350px; word-wrap: break-word;"> ' +
                                    '<div class="ui-content" id="txtCustomPeriodMsg" style="display:none;"></div>' +
                                    '<div style="padding: 0px 15px 15px 15px; max-height:400px; overflow:auto;">' +
                                        '<ul id="lstOfCustomPeriod" data-role="listview" class="li-theme-c noBorderList custom-list custom-list-sm">' +
                                            '<li id="btnCustomPeriod2DaysAgo" data-corners="false" data-shadow="false" data-iconshadow="true" data-wrapperels="div" data-icon="arrow-r" data-iconpos="right" data-theme="d" class="ui-btn ui-btn-icon-right ui-li-has-arrow ui-li  ui-first-child ui-last-child ui-btn-up-d"><div class="ui-btn-inner ui-li"><div class="ui-btn-text"><a arr="0" class="ui-link-inherit">2 Days Ago</a></div><span class="ui-icon ui-icon-arrow-r ui-icon-shadow">&nbsp;</span></div></li>' +
                                            '<li id="btnCustomPeriodYesterday" data-corners="false" data-shadow="false" data-iconshadow="true" data-wrapperels="div" data-icon="arrow-r" data-iconpos="right" data-theme="d" class="ui-btn ui-btn-icon-right ui-li-has-arrow ui-li  ui-first-child ui-last-child ui-btn-up-d"><div class="ui-btn-inner ui-li"><div class="ui-btn-text"><a arr="0" class="ui-link-inherit">Yesterday</a></div><span class="ui-icon ui-icon-arrow-r ui-icon-shadow">&nbsp;</span></div></li>' +
                                            '<li id="btnCustomPeriodToday" data-corners="false" data-shadow="false" data-iconshadow="true" data-wrapperels="div" data-icon="arrow-r" data-iconpos="right" data-theme="d" class="ui-btn ui-btn-icon-right ui-li-has-arrow ui-li  ui-first-child ui-last-child ui-btn-up-d"><div class="ui-btn-inner ui-li"><div class="ui-btn-text"><a arr="0" class="ui-link-inherit">Today</a></div><span class="ui-icon ui-icon-arrow-r ui-icon-shadow">&nbsp;</span></div></li>' +
                                            '<li id="btnCustomPeriodWeekToDate" data-corners="false" data-shadow="false" data-iconshadow="true" data-wrapperels="div" data-icon="arrow-r" data-iconpos="right" data-theme="d" class="ui-btn ui-btn-icon-right ui-li-has-arrow ui-li  ui-first-child ui-last-child ui-btn-up-d"><div class="ui-btn-inner ui-li"><div class="ui-btn-text"><a arr="0" class="ui-link-inherit">Week To Date</a></div><span class="ui-icon ui-icon-arrow-r ui-icon-shadow">&nbsp;</span></div></li>' +
                                            '<li id="btnCustomPeriodMonthToDate" data-corners="false" data-shadow="false" data-iconshadow="true" data-wrapperels="div" data-icon="arrow-r" data-iconpos="right" data-theme="d" class="ui-btn ui-btn-icon-right ui-li-has-arrow ui-li  ui-first-child ui-last-child ui-btn-up-d"><div class="ui-btn-inner ui-li"><div class="ui-btn-text"><a arr="0" class="ui-link-inherit">Month To Date</a></div><span class="ui-icon ui-icon-arrow-r ui-icon-shadow">&nbsp;</span></div></li>' +
                                            '<li id="btnCustomPeriodCustom" data-corners="false" data-shadow="false" data-iconshadow="true" data-wrapperels="div" data-icon="arrow-r" data-iconpos="right" data-theme="d" class="ui-btn ui-btn-icon-right ui-li-has-arrow ui-li  ui-first-child ui-last-child ui-btn-up-d"><div class="ui-btn-inner ui-li"><div class="ui-btn-text"><a arr="0" class="ui-link-inherit">Custom Period</a></div><span class="ui-icon ui-icon-arrow-r ui-icon-shadow">&nbsp;</span></div></li>' +
                                            '<li id="btnCustomPeriodTodayOrEarlier" style="display:none" data-corners="false" data-shadow="false" data-iconshadow="true" data-wrapperels="div" data-icon="arrow-r" data-iconpos="right" data-theme="d" class="ui-btn ui-btn-icon-right ui-li-has-arrow ui-li  ui-first-child ui-last-child ui-btn-up-d"><div class="ui-btn-inner ui-li"><div class="ui-btn-text"><a arr="0" class="ui-link-inherit">Today or Earlier</a></div><span class="ui-icon ui-icon-arrow-r ui-icon-shadow">&nbsp;</span></div></li>' +
                                        '</ul>' +
                                    '</div>' +
                            '</div>').appendTo(periodpopup);

                        var dualcalendarpopup = $('<div id="popupCustomDualCalendar" data-role="popup" data-theme="a" data-dismissible="true" data-corners="false" style="visibility:hidden; display:none;">' +
                            '<a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right btnCustomPopupClose">Close</a>' +
                            '<center><h3 style="margin-left:10px; margin-right:10px;" id="txtCustomDualCalendarTitle">Date Range</h3></center>' +
                            '</div>')
                            , dualcalendarcontent = $('<div class="ui-body-d" style="padding:5px; word-wrap: break-word;"> ' +
                                                        '<span id="txtErrorMsgDualCalendar" style="display:none; color:#F00"></span>' +
                                                        '<div class="ui-grid-a ui-responsive">' +
                                                            '<div class="ui-block-a" style="padding:7px;">' +
                                                                '<div class="metro" style="max-width:252px; overflow:auto;">' +
                                                                    'From: <span id="txtDateFromDualCalendar"></span> <br><br>' +
                                                                    '<div class="fromCustomDualCalendar calendar" data-other-days="1"></div>' +
                                                                '</div>' +
                                                                '<div id="divDualCalendarTimeFrom" style="display:none;">' +
                                                                    '<br>From: <select data-role="none" class="form-control fixed" id="selDCTimeFromHour"></select>' +
                                                                    '<select data-role="none" class="form-control fixed" id="selDCTimeFromMinute"></select>' +
                                                                    '<select data-role="none" class="form-control fixed" id="selDCTimeFromMeredian">' +
                                                                        '<option value="AM">AM</option>' +
                                                                        '<option value="PM">PM</option>' +
                                                                    '</select>' +
                                                                '</div>' +
                                                            '</div>' +
                                                            '<div class="ui-block-b" style="padding:7px;">' +
                                                                '<div class="metro" style="max-width:252px; overflow:auto;">' +
                                                                    'To: <span id="txtDateToDualCalendar"></span><br><br>' +
                                                                    '<div class="toCustomDualCalendar calendar" data-other-days="1"></div>' +
                                                                '</div>' +
                                                                '<div id="divDualCalendarTimeTo" style="display:none;">' +
                                                                    '<br>To: <select data-role="none" class="form-control fixed" id="selDCTimeToHour"></select>' +
                                                                    '<select data-role="none" class="form-control fixed" id="selDCTimeToMinute"></select>' +
                                                                    '<select data-role="none" class="form-control fixed" id="selDCTimeToMeredian">' +
                                                                        '<option value="AM">AM</option>' +
                                                                        '<option value="PM">PM</option>' +
                                                                    '</select>' +
                                                                '</div>' +
                                                            '</div>' +
                                                        '</div>' +
                                                        '<br />' +
                                                        '<div class="ui-grid-a ui-responsive">' +
                                                            '<div class="ui-block-a" style="padding:7px;">' +
                                                                '<a id="btnCancelDualCalendar" class="btn custom-btn-theme-c">Cancel</a>' +
                                                            '</div>' +
                                                            '<div class="ui-block-b" style="padding:7px;">' +
                                                                '<a id="btnDoneDualCalendar" class="btn custom-btn-theme-b">Done</a>' +
                                                            '</div>' +
                                                        '</div>' +
                                                    '</div>').appendTo(dualcalendarpopup);

                        var singlecalendarpopup = $('<div id="popupCustomSingleCalendar" data-role="popup" data-theme="a" data-dismissible="true" data-corners="false" style="visibility:hidden; display:none;">' +
                            '<a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right btnCustomPopupClose">Close</a>' +
                            '<center><h3 style="margin-left:10px; margin-right:10px;" id="txtCustomSingleCalendarTitle">Date</h3></center>' +
                            '</div>')
                            , singlecalendarcontent = $('<div class="ui-body-d jPopupContent" style="padding:5px; word-wrap: break-word;"> ' +
                                                        '<div id="divClearBtnSingleCalendar" style="display:block; text-align:right;"><a data-role="none" class="custom-link" id="btnClearSingleCalendar">Clear</a></div>' +
                                                        '<div class="ui-grid-solo ui-responsive">' +
                                                            '<div class="ui-block-a" style="padding:7px;">' +
                                                                '<div class="metro" style="width:100%; overflow:hidden;">' +
                                                                    'Date: <span id="txtDateSingleCalendar"></span> <br><br>' +
                                                                    '<div class="fromCustomSingleCalendar calendar" style="width:100%;" data-other-days="1"></div>' +
                                                                '</div>' +
                                                                '<div id="divSingleCalendarTime" style="display:none;">' +
                                                                    '<br>Time: <select data-role="none" class="form-control fixed" id="selSingleTimeHour"></select>' +
                                                                    '<select data-role="none" class="form-control fixed" id="selSingleTimeMinute"></select>' +
                                                                    '<select data-role="none" class="form-control fixed" id="selSingleTimeMeredian">' +
                                                                        '<option value="AM">AM</option>' +
                                                                        '<option value="PM">PM</option>' +
                                                                    '</select>' +
                                                                '</div>' +
                                                            '</div>' +
                                                        '</div>' +
                                                        '<br />' +
                                                        '<div class="ui-grid-a ui-responsive">' +
                                                            '<div class="ui-block-a" style="padding:7px;">' +
                                                                '<a id="btnCancelSingleCalendar" class="btn custom-btn-theme-c">Cancel</a>' +
                                                            '</div>' +
                                                            '<div class="ui-block-b" style="padding:7px;">' +
                                                                '<a id="btnDoneSingleCalendar" class="btn custom-btn-theme-b">Done</a>' +
                                                            '</div>' +
                                                        '</div>' +
                                                    '</div>').appendTo(singlecalendarpopup);

                        var settimepopup = $('<div id="popupCustomSetTime" data-role="popup" data-theme="a" data-dismissible="true" data-corners="false" style="visibility:hidden; display:none;">' +
                            '<a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right btnCustomPopupClose">Close</a>' +
                            '<center><h3 style="margin-left:10px; margin-right:10px;">Set Time</h3></center>' +
                            '</div>')
                            , settimecontent = $('<div class="ui-body-d" style="padding:5px; word-wrap: break-word;"> ' +
                                                        '<table cellpadding="10" style="text-align:center;">' +
                                                            '<tbody>' +
                                                                '<tr>' +
                                                                    '<td><button data-role="none" id="btnSetTimeAsc" class="btn custom-btn-theme-c btnCircle-xs">&#9650</button></td>' +
                                                                    '<td rowspan="3"><button data-role="none" class="btn custom-btn-theme-d btnCircle-sm" id="btnSetTimeMeridian">AM</button></td>' +
                                                                '</tr>' +
                                                                '<tr>' +
                                                                    '<td><span id="SetTimeHour" style="width:40px !important;" class="btn btnCircle-sm timeunactive timeactive"><input type="text" data-role="none" value="1" /></span>:<span id="SetTimeMinute" style="width:40px !important;" class="btn btnCircle-sm timeunactive"><input type="text" data-role="none" value="00" /></span></td>' +
                                                                '</tr>' +
                                                                '<tr>' +
                                                                    '<td><button data-role="none" id="btnSetTimeDesc" class="btn custom-btn-theme-c btnCircle-xs">&#9660</button></td>' +
                                                                '</tr>' +
                                                            '</tbody>' +
                                                        '</table>' +
                                                        '<br />' +
                                                        '<div class="ui-grid-a ui-responsive">' +
                                                            '<div class="ui-block-a" style="padding:7px;">' +
                                                                '<a id="btnCancelSetTime" class="btn custom-btn-theme-c">Cancel</a>' +
                                                            '</div>' +
                                                            '<div class="ui-block-b" style="padding:7px;">' +
                                                                '<a id="btnOkSetTime" class="btn custom-btn-theme-b">Ok</a>' +
                                                            '</div>' +
                                                        '</div>' +
                                                    '</div>').appendTo(settimepopup);

                        var continuousbarcode = $('<div id="popupContinuousBarcode" data-role="popup" data-theme="a" data-dismissible="false" data-corners="false" style="visibility:hidden; display:none;max-width:500px">' +
                            '<a href="#" style="display:none;" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right btnCustomPopupClose">Close</a>' +
                            '<center><h3 style="margin-left:10px; margin-right:10px;" id="txtContinuousBarcodeTitle">Loop Scan</h3></center>' +
                            '</div>')
                            , content = $('<div data-role="content" class="ui-content ui-body-d" style="max-width:500px; width; 500px word-wrap:break-word;"> ' +
                                '<div id="ContinuousBarcodetxtMessage" style="line-height:1.3; text-align:justify; font-size:medium">' +
                                    'The Loop Scan feature allows you to continuously ' +
                                    'scan barcodes and/or QR (Quick Response) codes ' +
                                    'without being interrupted by non-barcode input ' +
                                    'data. The purpose of Loop Scan is to quickly collect ' +
                                    'barcode/QR code data from your physical ' +
                                    'merchandise and then process these data for use ' +
                                    'with your intended application modules such as for ' +
                                    'inventory counting, sales ordering, stock receiving, ' +
                                    'stock transferring and other modules. ' +
                                    '    <br /> <br />  ' +
                                    'How would you like to scan barcodes/ QR codes? ' +
                                    '    <br />' +
                                    '<a href="#" id="lnkMobileScanner" style="color: black; text-decoration: none;"> <div class="row" style="margin:1px; border: 1px solid gray">' +
                                        '<div class="col-sm-2" style="margin-top:20px"><img align="center" height="92" width="64" src="../../Content/images/loopscancameraicon.png" /></div>' +
                                        '<div class="col-sm-10" style="text-align:justify;font-size:small">' +
                                        '<br /> <b> Use the scanner app installed on my mobile device </b> <br />' +
                                        'This is a very convenient way to scan barcodes/ QR codes. ' +
                                        'However, since this method uses your device’s camera and is ' +
                                        'dependent on reflected environmental lighting, ' +
                                        'this method’s scanning performance is adversely affected by poor lightning conditions.' +
                                        '<br /><br />' +
                                        '</div>' +                                        
                                    '</div> </a>' + //End of div row
                                    '    <br />' +
                                    '<a href="#" id="lnkBarcodeScanner" style="color: black; text-decoration: none;"> <div title="" class="row" style="margin:1px; border: 1px solid gray">' +
                                        '<div class="col-sm-2" style="margin-top:30px"><img align="center" img height="92" width="64" src="../../Content/images/loopscanbarcodeicon.png" /></div>' +
                                        '<div class="col-sm-10" style="text-align:justify;font-size:small">' +
                                        '<br /> <b> Use external USB (with OTG dongle if necessary) or Bluetooth bar code scanner </b> <br />' +
                                        'This method requires an external bar code scanner connected via USB or Bluetooth interface. ' +
                                        'Depending on the scan performance of your barcode scanner, ' +
                                        'this can be the most reliable way to capture barcode and/or ' +
                                        'QR code data as your external barcode scanner emits it own light ' +
                                        'and is not negatively affected by poor environmental lighting conditions.' +
                                        '<br /><br />' +
                                        '</div>' +                                        
                                    '</div> </a>' + //End of div row
                                '</div>' +

                                '<div id="ContinuousBarcodeItemList" style="line-height:1.3; text-align:center; font-size:medium; visibility:hidden; display:none">' +
                                    '<label>Enter Product Barcode</label> <br />' +
                                    '<input type="text" id="txtContinuousBarcode">' +
                                '</div>' +

                                '<br />' +

                                '<div id="dvBarcodeCount" style="visibility:hidden; display:none">' +
                                    '<table id="tblBarcodeCount" data-role="table" data-role="reflow" style="margin-right:5px; display:table;" class="display ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">' +
                                            '<thead>' +
                                                '<tr class="custom-theme-f fixed-cell-width">' +
                                                    '<th class="txt-center"> Product Barcode </th>' +
                                                    '<th class="txt-center"> Barcode Count </th>' +
                                                '</tr>' +
                                        '</thead>' +
                                    '</table>' +

                                    '<br />' +

                                    '<a id="btnContinuousBarcodeBoxOK" class="btn custom-btn-theme-b">Done</a>' +
                                '</div>' +

                                '<div id="divNegativeButton" style="display:none;"><br><a data-role="none" id="btnContinuousBarcodeCancel" class="btn custom-btn-theme-c" style="white-space:normal;"></a></div>' +
                                '<div id="divForButtonContinuousBarcode"></div>' +
                            '</div>').appendTo(continuousbarcode);

                        var tooltip = $('<div id="webtooltip" style="display:none; font-family:Helvetica, Arial, sans-serif; text-shadow:none; word-break:break-word; position:fixed; background-color:#fceda7; z-index:2000; left:50%; transform: translate(-50%, 0%); padding:10px; border-radius:3px; box-shadow:0px 5px 10px #3E3D3D; border:1px solid #fadb4e;">')
                        var popupPIMWebForm = $('<div id="globalpopupPIMWebForm" custom-data-role="popup">')
                                                    .append($('<div class="popup-md ui-content ui-body-d" id="divGlobalPIMWebForm">'))
                        var htmlPIMWebForm = '';
                        var isUniversalPopup = false;

                        if (typeof Bs == 'undefined') {
                            //\Content\bootstrap\js\bootstrap_modal.js
                            popup.appendTo(page);
                            browsepopup.appendTo(page);
                            listpopup.appendTo(page);
                            singlecalendarpopup.appendTo(page);
                            dualcalendarpopup.appendTo(page);
                            settimepopup.appendTo(page);
                            continuousbarcode.appendTo(page);

                            try {
                                popup.trigger('create');
                                browsepopup.trigger('create');
                                listpopup.trigger('create');
                                singlecalendarpopup.trigger('create');
                                dualcalendarpopup.trigger('create');
                                settimepopup.trigger('create');
                                continuousbarcode.trigger('create');
                            } catch (e) {
                                // do nothing
                            }
                        }

                        //custom-tab                    
                        $('.custom-tab').each(function () {
                            $(this).nextAll('div[custom-tab-role="content"]:not(":first")').hide()
                        })

                        ResponsiveCustomTab();

                        page
                        .append(popupPIMWebForm)
                        .on('click', '.btnCustomPopupClose', function () {
                            $('.popupCustomOverlayTheme').remove();
                        })
                        .on('click', 'li[custom-tab-content]', function () {
                            var dis = $(this);
                            var ul = $(this).parent();
                            var li = ul.find(' > li');

                            if (dis[0].hasAttribute('disabled')) {
                                //put attribute 'disabled' to specific tab to override the current click function
                                return; // do not open
                            }

                            li.each(function () {
                                $($(this).attr('custom-tab-content')).hide();
                            })

                            li.removeClass('active');
                            dis.addClass('active');

                            $($(this).attr('custom-tab-content')).show();

                            //commented by arnie 10-15-2018 forgot the purpose :x
                            //ul.parent().nextAll(':not("div[custom-tab-role="content"]")').show(); // (if any) show all tags that are not tab-content
                        })
                        //custom popup for mobiz project
                        .on('click', '.popup-close', function () {
                            $(this).parents().closest('[custom-data-role="popup"]').css('display', 'none');
                        })
                        .on('click', '.custom-list-menu li', function () {
                            if (!$(this).find('a').hasClass('disabled-menu')) {
                                $('.custom-list-menu').find('li').find('a.active-menu').removeClass('active-menu');
                                $(this).find('a').addClass('active-menu')
                            }
                        })
                        .on('click', '.fixed-ActionBtn > button', function () {
                            var dis = $(this).parent();
                            var isToggle = dis.hasClass('toggle-on');

                            $.each(dis.find('ul > li'), function (i, el) {
                                var el = $(el).find('button');

                                if (isToggle == false) {
                                    setTimeout(function () {
                                        $(el).css({
                                            display: 'inline-block',
                                            transition: 'transform 1000ms ease-in-out'
                                        })//.fadeIn();
                                    }, 100 + (i * 100));
                                    dis.removeClass('toggle-off').addClass('toggle-on');
                                }
                                else {
                                    setTimeout(function () {
                                        $(el).css({
                                            display: 'none'
                                        })//.fadeOut();
                                    }, 100 + (i * 100));
                                    dis.removeClass('toggle-on').addClass('toggle-off');
                                }
                            });
                        })
                        .on('click', '.fixed-ActionBtn > ul', function () {
                            var dis = $(this).find('li');

                            $.each(dis, function (i, el) {
                                var el = $(el).find('button');

                                setTimeout(function () {
                                    $(el).fadeOut();
                                }, 100 + (i * 100));
                                dis.parent().parent().removeClass('toggle-on').addClass('toggle-off');
                            });
                        })
                        .on('click', '.list-menu > li', function (e) {
                            var selectedPage = $(this).find('a').attr('data-gotopage');

                            //                        $('.custom-list-menu > li').each(function () {
                            //                            $('#' + $(this).find('a').attr('data-gotopage')).hide();
                            //                        })
                            if (!$(this).find('a').hasClass('disabled-menu')) {
                                $('#' + selectedPage).pageshow();
                                $('.custom-list-menu-toggle').text($(this).find('a').text()).removeClass('toggle-on');

                                if ($('.custom-list-menu-toggle').css('display') == "block") {
                                    $('#' + $(this).parent().attr('id')).fadeOut();
                                }
                            } else {
                                e.preventDefault();
                                e.stopPropagation();
                            }
                        })
                        .on('click', '.custom-list-menu-toggle', function () {
                            var dis = $(this)
                            var toggleID = dis.attr('data-toggle');

                            if (dis.hasClass('toggle-on') == false) {
                                dis.addClass('toggle-on');
                                $('#' + toggleID).fadeIn('slow');
                            }
                            else {
                                dis.removeClass('toggle-on');
                                $('#' + toggleID).fadeOut();
                            }
                        })
                        .on('click', '.custom-checkbox', function (e) {
                            if (proj.IsBootstrapDesign()) {
                                return
                            }

                            e.stopPropagation();
                            e.preventDefault();
                            var customCHB = $(this).find('div:last').parent().find('input[type="checkbox"]');
                            if (customCHB.is(':disabled')) {
                                return;
                            }
                            try{
                                customCHB.prop('checked', !customCHB.is(':checked')).checkboxradio('refresh');
                            } catch(e){
                                customCHB.prop('checked', !customCHB.is(':checked')).checkboxradio().checkboxradio('refresh');
                            }
                            customCHB.trigger('change');
                        })
                        .on('click', '.custom-radio', function (e) {
                            if (proj.IsBootstrapDesign()) {
                                return
                            }

                            e.stopPropagation();
                            e.preventDefault();

                            var customRad = $(this).find('input[type="radio"]:first');
                            if (customRad.is(':disabled')) {
                                return;
                            }
                            var customRadName = customRad.attr('name');
                            $('input[type="radio"][name="' + customRadName + '"]').prop('checked', false).checkboxradio('refresh');

                            if (customRad.is(':checked')) {
                                customRad.prop('checked', false).checkboxradio('refresh');
                            }
                            else {
                                customRad.prop('checked', true).checkboxradio('refresh');
                            }
                            customRad.trigger('change');
                        })
                        .on('click', '.tab-responsive-header', function () {
                            var dis = $(this);

                            if (dis.hasClass('active') == false) {
                                dis.removeClass('active')
                                dis.next().hide();

                                dis.addClass('active');
                                dis.next().show();
                            } else {
                                dis.removeClass('active');
                                dis.next().hide();
                            }
                        })
                        .on('click', '[panel-menu="true"]  > li', function () {
                            if ($(this).find('a').attr('data-gotopage') != undefined) {
                                var menu = $('#' + $(this).find('a').attr('data-gotopage'));
                                menu.pageshow()
                                $('#' + $(this).parent().parent().attr('id')).toggle('fast').css('overflow', 'auto')
                            }
                        })
                        .on('click', '[custom-panel]', function () {
                            $('#' + $(this).attr('custom-panel')).toggle('fast').css('overflow', 'auto')
                        })
                        .on('click', '.accordion-header', function () {
                            var dis = $(this);
                            var type = $.trim(dis.parent().attr('custom-data-role'));

                            if (type == 'accordion') {
                                if (dis.hasClass('active')) {
                                    dis.removeClass('active');
                                    dis.next().hide();
                                } else {
                                    dis.addClass('active');
                                    dis.next().show();
                                }
                            }
                        })
                        .on('click', '.btnCloseToolTip', function () {
                            $('.tooltip-window:visible').hide();
                        })
                        //used for back button in android
                        $(window).on("navigate", function (event, data) {
                            var direction = data.state.direction;
                            if (direction == 'back') {
                                ClosePopUp();
                            }
                        }).resize(function (e) {
                            ResponsiveCustomTab();
                            PopupFitToScreen();

                            var content = $('.ui-popup-active');
                            content.css({
                                maxWidth: ($(window).width() - 30) + 'px'
                            })

                            page.find('.tooltip-window > .tooltip-content').css('max-height', ($(window).height() - 140) + 'px')
                                .css('max-width', ($(window).width() - 50) + 'px')
                        })

                        page.append(tooltip)

                        var tempToast = [], webtoasttimer;
                        window.WebToast = function (msg, position, delay) {
                            position = position || 'top';

                            if (typeof Bs != 'undefined') {
                                //\Content\bootstrap\js\bootstrap_modal.js
                                Bs.WebToast(msg, position, delay)
                                return;
                            }

                            if (position == 'top') {
                                tooltip.css('top', '30px')
                                tooltip.css('bottom', '')
                            }
                            else {
                                tooltip.css('top', '')
                                tooltip.css('bottom', '30px')
                            }

                            if ($('#webtooltip').css('display') == 'block') {
                                setTimeout(function () {
                                    $('#webtooltip').html(msg);
                                }, 1000)
                            } else {
                                $('#webtooltip').html(msg);
                            }

                            $('#webtooltip').fadeIn("slow");
                            clearTimeout(webtoasttimer);
                            
                            webtoasttimer = setTimeout(function () {
                                $('#webtooltip').fadeOut("slow")
                            }, 4000);

                            //$('#webtooltip').fadeIn("slow").delay(3000).fadeOut("slow", function (e) {
                            //    if (tempToast.length > 0) {
                            //        webtoasttimer = setTimeout(function () {
                            //            WebToast(tempToast[0].Msg, tempToast[0].Pos);
                            //            tempToast.splice(0, 1);
                            //        }, 50);
                            //    }
                            //});
                            //if ($('#webtooltip').css('display') != 'block') {                                
                                
                            //} else {
                            //    tempToast.push({
                            //        Msg: msg,
                            //        Pos: position
                            //    })
                            //}
                        }

                        window.BreadCrumbs = function (bcContainer, vParams) {
                            var arrBC = vParams;
                            var bc = $(bcContainer);

                            if (window.location.pathname == '/erp') {
                                AddBreadCrumb(arrBC[arrBC.length - 1], bcContainer);
                            } else {
                                bc.text('');
                                for (var i = 0; i < arrBC.length; i++) {
                                    var arrowR = ' <span bcno="' + i + '" class="ui-icon ui-icon-arrow-r" style="display:inline-block; margin:0px 10px 0px 10px;"> ';

                                    if (arrBC.length == (i + 1)) {
                                        bc.append($('<span bcno="' + i + '" class="bcname">')
                                                .data('bcdata', arrBC[i])
                                                .text(arrBC[i].Text)
                                            )
                                        //bc.append('<span bcno="' + i + '" class="bcname">' + arrBC[i].Text + '</span>')
                                    }
                                    else {
                                        var link = $('<a data-role="none">').data('bcdata', arrBC[i]).html(arrBC[i].Text).data(arrBC[i])
                                        .attr('bcno', i)
                                        .click(function () {
                                            var bcnoLength = CDec2($('*[bcno]:last').attr('bcno'));
                                            var bcno = CDec2($(this).attr('bcno'));
                                            $('.tooltip-window').hide();

                                            for (var x = 0; x < bcnoLength; x++) {
                                                bcno = bcno + 1;
                                                $('*[bcno="' + bcno + '"]').remove();
                                                $('span[bcno="' + (bcno - 1) + '"]').remove();
                                            }

                                            bcno = CDec2($(this).attr('bcno'));
                                            $(this).data().Click();
                                            $('a[bcno="' + bcno + '"]').contents().unwrap();
                                        })
                                        bc.append(link).append(arrowR);
                                    }
                                }
                            }
                        }

                        window.CustomSetTime = function (vParams) {
                            if (typeof Bs != 'undefined') {
                                //\Content\bootstrap\js\bootstrap_modal.js
                                Bs.SetTime(vParams);
                                return;
                            }

                            isUniversalPopup = true;
                            var Format = { 13: 1, 14: 2, 15: 3, 16: 4, 17: 5, 18: 6, 19: 7, 20: 8, 21: 9, 22: 10, 23: 11, 24: 12 };
                            var h = "1", m = "01", md = "AM"
                            settimepopup.remove();
                            settimepopup.popup();

                            ClosePopUp();
                            page.append($('<div>').addClass('popupCustomOverlayTheme').css(cssOverLay));

                            $('.popupCustomOverlayTheme').unbind('click').click(function () {
                                ClosePopUp();
                            })

                            if (vParams.Cancel != undefined || vParams.Close != undefined) {
                                closeFunction = (vParams.Cancel || vParams.Close);
                            }

                            $('#btnSetTimeAsc').unbind().click(function () {
                                var timeType = $('.timeactive').attr('id');
                                var currentTime = CInt2($('#' + timeType).find('input[type="text"]').val());
                                var limitasc, setTime = 1;
                                if (timeType == "SetTimeHour") {
                                    limitasc = 12;
                                }
                                else {
                                    limitasc = 59;
                                }

                                if (currentTime < limitasc) {
                                    currentTime = currentTime + setTime;
                                }
                                else if (currentTime == 59) {
                                    currentTime = 0;
                                }
                                else {
                                    currentTime = 1;
                                }

                                if (timeType == "SetTimeMinute") {
                                    currentTime = (currentTime < 10 ? "0" + currentTime : currentTime);
                                }
                                $('#' + timeType).find('input[type="text"]').val(currentTime)
                            })

                            $('#btnSetTimeDesc').unbind().click(function () {
                                var timeType = $('.timeactive').attr('id');
                                var currentTime = CInt2($('#' + timeType).find('input[type="text"]').val());
                                var limitasc, setTime = 1;
                                if (timeType == "SetTimeHour") {
                                    limitasc = 12;
                                }
                                else {
                                    limitasc = 59;
                                }

                                if (currentTime == 0) {
                                    currentTime = 59;
                                }
                                else {
                                    currentTime = currentTime - setTime;

                                    //for hour
                                    if (currentTime == 0 && timeType == "SetTimeHour") {
                                        currentTime = 12
                                    }
                                }

                                if (timeType == "SetTimeMinute") {
                                    currentTime = (currentTime < 10 ? "0" + currentTime : currentTime);
                                }
                                $('#' + timeType).find('input[type="text"]').val(currentTime)
                            })

                            $('#btnSetTimeMeridian').unbind().click(function () {
                                var val = $(this).text().toLowerCase();

                                if (val == "am") {
                                    $(this).text('PM');
                                }
                                else {
                                    $(this).text('AM');
                                }
                            })

                            $('#SetTimeHour').unbind().click(function () {
                                $('#SetTimeMinute').removeClass('timeactive');
                                $(this).addClass('timeactive');
                            })

                            $('#SetTimeMinute').unbind().click(function () {
                                $('#SetTimeHour').removeClass('timeactive');
                                $(this).addClass('timeactive');
                            })

                            $('#SetTimeMinute').find('input[type="text"]').unbind().blur(function () {
                                var val = CInt2($(this).val());
                                if (val > 59) {
                                    $(this).val('01')
                                }
                                else {
                                    $(this).val((val < 10 ? "0" + val : val))
                                }
                            }).keydown(function (e) {
                                if (!NumberOnly(e)) {
                                    return;
                                }
                            })

                            $('#SetTimeHour').find('input[type="text"]').unbind().blur(function () {
                                var val = CInt2($(this).val());
                                if (val > 12) {
                                    $(this).val('1')
                                }
                            }).keydown(function (e) {
                                if (!NumberOnly(e)) {
                                    return;
                                }
                            })

                            $('#btnOkSetTime').unbind().click(function () {
                                var selectedTime = {
                                    h: $('#SetTimeHour').find('input[type="text"]').val(),
                                    m: $('#SetTimeMinute').find('input[type="text"]').val(),
                                    md: $('#btnSetTimeMeridian').text()
                                }

                                selectedTime.time = selectedTime.h + ":" + selectedTime.m + " " + selectedTime.md;

                                vParams.Click(selectedTime)
                                ClosePopUp();
                            })

                            $('#btnCancelSetTime').unbind().click(function () {
                                ClosePopUp();
                            })

                            $('#popupCustomSetTime').css({ visibility: 'visible', display: 'block' }).popup('open');
                            RepositionPopup();

                            $('#SetTimeHour input').focus();
                        }
                            
                        var BalancesTable = $('#tblBarcodeCount');
                        var balancesDatatable;

                        window.ContinuousBarcode = function (vParams) {
                            if (typeof Bs != 'undefined') {
                                //\Content\bootstrap\js\bootstrap_modal.js
                                Bs.ContinuousBarcode(vParams);
                                return;
                            }

                            var pProductBarcodeCount = [];
                            
                            isUniversalPopup = true;
                            
                            $('#ContinuousBarcodetxtMessage').css('visibility', 'visible');
                            $('#ContinuousBarcodetxtMessage').css('display', 'block');
                            $('#ContinuousBarcodeItemList').css('visibility', 'hidden');
                            $('#dvBarcodeCount').css('visibility', 'hidden');
                            $('#dvBarcodeCount').css('display', 'none');
                            $('#ContinuousBarcodeItemList').css('display', 'none');

                            if ($.fn.DataTable.isDataTable(BalancesTable)) {
                                //empty the table first before destroying datatabase para dili magdoble ang header
                                BalancesTable.empty();

                                if (balancesDatatable != undefined) {
                                    balancesDatatable.destroy();
                                }
                            }

                            balancesDatatable = BalancesTable.DataTable({
                                "data": pProductBarcodeCount,
                                "columns": [
                                    { "data": "Barcode" },
                                    { "data": "Count" }
                                ]
                            })
                            
                            continuousbarcode.remove();
                            continuousbarcode.popup();

                            ClosePopUp();
                            page.append($('<div>').addClass('popupCustomOverlayTheme').css(cssOverLay));

                            $('.popupCustomOverlayTheme').unbind('click').click(function () {
                                ClosePopUp();
                            })

                            if (vParams.Cancel != undefined || vParams.Close != undefined) {
                                closeFunction = (vParams.Cancel || vParams.Close);
                            }

                            RepositionPopup();

                            $('#lnkBarcodeScanner').click(function () {
                                $('#ContinuousBarcodetxtMessage').css('visibility', 'hidden');
                                $('#ContinuousBarcodetxtMessage').css('display', 'none');
                                $('#ContinuousBarcodeItemList').css('visibility', 'visible');

                                $('#dvBarcodeCount').css('visibility', 'visible');
                                $('#dvBarcodeCount').css('display', 'block');
                                

                                $('#ContinuousBarcodeItemList').css('display', 'block');
                                $('#txtContinuousBarcode').focus();
                                RepositionPopup();
                            })

                            $('#txtContinuousBarcode').keydown(function (e) {
                                if (e.keyCode == 13) {
                                    var results;
                                    var isExist = false;
                                    var barcodeValue = '';

                                    if ($('#txtContinuousBarcode').val() == '')
                                        return

                                    barcodeValue = $('#txtContinuousBarcode').val();

                                    pProductBarcodeCount.forEach(function (item) {
                                        if (barcodeValue === item.Barcode) {
                                            item.Count++;
                                            isExist = true;
                                            return;
                                        }
                                    });

                                    if (!isExist) {
                                        pProductBarcodeCount.push({
                                            Barcode: barcodeValue,
                                            Count: 1
                                        })
                                    }

                                    if ($.fn.DataTable.isDataTable(BalancesTable)) {
                                        //empty the table first before destroying datatabase para dili magdoble ang header
                                        BalancesTable.empty();

                                        if (balancesDatatable != undefined) {
                                            balancesDatatable.destroy();
                                        }                                        
                                    }

                                    balancesDatatable = BalancesTable.DataTable({
                                        "data": pProductBarcodeCount,
                                        "columns": [
                                            { "data": "Barcode" },
                                            { "data": "Count" }
                                        ]
                                    })

                                    $('#txtContinuousBarcode').val('');
                                    $('#txtContinuousBarcode').focus();
                                    RepositionPopup();
                                }
                                
                            });

                            $('#lnkMobileScanner').click(function () {
                                $('#ContinuousBarcodetxtMessage').css('visible', 'hidden')
                                $('#ContinuousBarcodetxtMessage').css('display', 'none')
                                $('#ContinuousBarcodeItemList').css('display', 'none')
                                $('#dvBarcodeCount').css('visibility', 'visible');
                                $('#dvBarcodeCount').css('display', 'block');
                                LoopBarcode();

                                function LoopBarcode() {
                                    mobile.Scan({
                                        Success: function (e) {
                                            var results;
                                            var isExist = false;

                                            pProductBarcodeCount.forEach(function (item) {
                                                if (e.Value === item.Barcode) {
                                                    item.Count++;
                                                    isExist = true;
                                                    return;
                                                }
                                            });

                                            if (!isExist) {
                                                pProductBarcodeCount.push({
                                                    Barcode: e.Value,
                                                    Count: 1
                                                })
                                            }

                                            LoopBarcode();
                                        },
                                        Error: function () {
                                            var BalancesTable = $('#tblBarcodeCount'); // diri ni ibutang kay kung sa taas kay dili na magload ang datatable
                                            if ($.fn.DataTable.isDataTable(BalancesTable)) {
                                                //empty the table first before destroying datatabase para dili magdoble ang header
                                                BalancesTable.empty();
                                                balancesDatatable.destroy();
                                            }

                                            balancesDatatable = BalancesTable.DataTable({
                                                "data": pProductBarcodeCount,
                                                "columns": [
                                                    { "data": "Barcode" },
                                                    { "data": "Count" }
                                                ]
                                            })

                                            RepositionPopup();
                                        }
                                    });
                                }
                            });

                            $('#btnContinuousBarcodeBoxOK').unbind('click').click(function () {

                                ClosePopUp();
                                closeFunction = '';
                                vParams.Click(pProductBarcodeCount);

                            })

                            $('#popupContinuousBarcode').css({ visibility: 'visible', display: 'block' }).popup('open');
                        }

                        window.SingleCalendar = function (vParams) {
                            if (typeof Bs != 'undefined') {
                                //\Content\bootstrap\js\bootstrap_modal.js
                                Bs.Calendar(vParams)
                                return;
                            }

                            var m_DateNow = new Date();
                            var arrDate = {}, isShowTime = false;

                            isUniversalPopup = true;

                            periodpopup.remove();
                            periodpopup.popup();

                            ClosePopUp();
                            page.append($('<div>').addClass('popupCustomOverlayTheme').css(cssOverLay));

                            $('.popupCustomOverlayTheme').unbind('click').click(function () {
                                ClosePopUp();
                            })
                            $('#divSingleCalendarTime').hide();

                            if (vParams.ClearButton != undefined || vParams.Clear != undefined) {
                                $('#btnClearSingleCalendar').show().text(vParams.ClearButton || 'Unset Date');
                            }
                            else {
                                $('#btnClearSingleCalendar').hide();
                            }

                            if (vParams.Close != undefined || vParams.Cancel != undefined) {
                                closeFunction = (vParams.Close || vParams.Cancel);
                            }

                            if(vParams.ShowTime != undefined){ // value must be boolean
                                if (CBool(vParams.ShowTime)) {
                                    isShowTime = true;
                                    $('#divSingleCalendarTime').show();
                                    var selSingleTimeHour = $('#selSingleTimeHour');
                                    var selSingleTimeMinute = $('#selSingleTimeMinute');
                                    var selSingleTimeMeredian = $('#selSingleTimeMeredian');
                                    var hour = 1, minute = 0;

                                    selSingleTimeHour.empty();
                                    selSingleTimeMinute.empty();

                                    for (var i = 0; i < 12; i++) {
                                        selSingleTimeHour.append(new Option(hour, hour));
                                        hour += 1;
                                    }

                                    for (var i = 0; i <= 59; i++) {
                                        var formatnumber = $.format.number(minute, '00');
                                        selSingleTimeMinute.append(new Option(formatnumber, formatnumber));
                                        minute += 1;
                                    }

                                    if (vParams.SetTime != undefined) { // must be 12 hr format
                                        var time = (vParams.SetTime).split(':');
                                        selSingleTimeHour.val(CInt2(time[0]));
                                        selSingleTimeMinute.val(parseInt(time[1]));
                                        selSingleTimeMeredian.val($.trim((vParams.SetTime).split(' ')[1]));
                                    }
                                }
                            }

                            if(vParams.Title != undefined){
                                $('#txtCustomSingleCalendarTitle').text(vParams.Title);
                            } else {
                                $('#txtCustomSingleCalendarTitle').text('Date');
                            }

                            $(".fromCustomSingleCalendar").calendar({
                                format: 'mmmm dd, yyyy',
                                multiSelect: false,
                                otherDays: true,
                                minDate: (vParams.MinDate == undefined ? false : vParams.MinDate),
                                maxDate: (vParams.MaxDate == undefined ? false : vParams.MaxDate),
                                getDates: function (data) {
                                    $("#txtDateSingleCalendar").text($.format.date(new Date(data), (vParams.Format == undefined ? "MMMM dd, yyyy" : vParams.Format)));
                                }
                            });

                            $('.fromCustomSingleCalendar').calendar('unsetDate');

                            if (vParams.SetDate != undefined) {
                                $('.fromCustomSingleCalendar').calendar('setDate', $.format.date(new Date(vParams.SetDate), (vParams.Format == undefined ? "MMMM dd, yyyy" : vParams.Format)));
                            }
                            else {
                                $('.fromCustomSingleCalendar').calendar('setDate', $.format.date(m_DateNow, (vParams.Format == undefined ? "MMMM dd, yyyy" : vParams.Format)));
                            }

                            $('#btnDoneSingleCalendar').unbind('click').click(function () {
                                var setDate = $.format.date(new Date($('#txtDateSingleCalendar').text()), (vParams.Format == undefined ? "MMM dd, yyyy" : vParams.Format)) + (isShowTime ? ' ' + selSingleTimeHour.val() + ':' + selSingleTimeMinute.val() + ' ' + selSingleTimeMeredian.val() : '');
                                if (vParams.SetText != undefined) {
                                    if ($.trim($('#txtDateSingleCalendar').text()) != "") {
                                        ClosePopUp();
                                        $(vParams.SetText).text(setDate).button('refresh');
                                        vParams.Click($('#txtDateSingleCalendar').text());
                                    }
                                    else {
                                        ClosePopUp();
                                        $(vParams.SetText).text(vParams.Placeholder).button('refresh');
                                        vParams.Click('');
                                    }
                                }
                                else {
                                    ClosePopUp();
                                    vParams.Click(setDate);
                                }
                            })

                            $('#btnCancelSingleCalendar').unbind('click').click(function () {
                                ClosePopUp();
                                if (vParams.Cancel != undefined) {
                                    vParams.Cancel();
                                }
                            })

                            $('#btnClearSingleCalendar').unbind('click').click(function () {
                                ClosePopUp();
                                vParams.Clear('');
                            })

                            $('#popupCustomSingleCalendar').css({ visibility: 'visible', display: 'block' }).popup('open');
                            RepositionPopup();
                        }

                        window.DualCalendar = function (vParams) {
                            var m_DateFrom = (IsDate(vParams.SetFromDate) ? new Date(vParams.SetFromDate) : new Date());
                            var m_DateTo = (IsDate(vParams.SetToDate) ? new Date(vParams.SetToDate) : new Date());

                            if (typeof Bs != 'undefined') {
                                //\Content\bootstrap\js\bootstrap_modal.js
                                Bs.DateRange(vParams, m_DateFrom, m_DateTo);
                                return;
                            }

                            isUniversalPopup = true;

                            DualCalendar(vParams, m_DateFrom, m_DateTo);

                            if (vParams.Cancel != undefined || vParams.Close != undefined) {
                                closeFunction = vParams.Cancel || vParams.Close;
                            }
                        }

                        window.PopupPeriod = function (vParams) {
                            vParams = $.extend({
                                Display2DaysAgo: true,
                                DisplayYesterday: true,
                                DisplayToday: true,
                                DisplayWeekToDate: true,
                                DisplayMonthToDate: true,
                                DisplayCustomDate: true,
                                DisplayCustomPeriod: true,
                                DisplayTodayOrEarlier: false // id: btnCustomPeriodTodayOrEarlier
                            }, vParams);

                            if (typeof Bs != 'undefined') {
                                //\Content\bootstrap\js\bootstrap_modal.js
                                Bs.PopupPeriod(vParams);
                                return;
                            }

                            var m_DateFrom = new Date();
                            var m_DateTo = new Date();
                            var arrDate = {};
                            isUniversalPopup = true;

                            $('#lstOfCustomPeriod > li').show();

                            if (vParams.DisplayTodayOrEarlier)
                                $('#btnCustomPeriodTodayOrEarlier').show();
                            else
                                $('#btnCustomPeriodTodayOrEarlier').hide();
                                
                            periodpopup.remove();
                            periodpopup.popup();

                            ClosePopUp();
                            page.append($('<div>').addClass('popupCustomOverlayTheme').css(cssOverLay));

                            if (!vParams.Display2DaysAgo) {
                                $('#btnCustomPeriod2DaysAgo').hide();
                            }

                            if (!vParams.DisplayYesterday) {
                                $('#btnCustomPeriodYesterday').hide();
                            }

                            if (!vParams.DisplayToday) {
                                $('#btnCustomPeriodToday').hide();
                            }

                            if (!vParams.DisplayWeekToDate) {
                                $('#btnCustomPeriodWeekToDate').hide();
                            }

                            if (!vParams.DisplayMonthToDate) {
                                $('#btnCustomPeriodMonthToDate').hide();
                            }

                            if (!vParams.DisplayCustomPeriod) {
                                $('#btnCustomPeriodCustom').hide();
                            }

                            $('.popupCustomOverlayTheme').unbind('click').click(function () {
                                ClosePopUp();
                            })

                            $('#btnCustomPeriod2DaysAgo').unbind('click').click(function () {
                                m_DateFrom.setDate(m_DateFrom.getDate() - 2);
                                m_DateTo.setDate(m_DateTo.getDate() - 2);

                                arrDate.Period = "2 days ago";
                                arrDate.From = $.format.date(m_DateFrom, 'MMM dd, yyyy');
                                arrDate.To = $.format.date(m_DateTo, 'MMM dd, yyyy');

                                ClosePopUp();
                                if (vParams.SetText != undefined) {
                                    $(vParams.SetText).html('2 Days Ago');
                                }
                                vParams.Click(arrDate);
                            })

                            $('#btnCustomPeriodYesterday').unbind('click').click(function () {
                                m_DateFrom.setDate(m_DateFrom.getDate() - 1);
                                m_DateTo.setDate(m_DateTo.getDate() - 1);

                                arrDate.Period = "Yesterday";
                                arrDate.From = $.format.date(m_DateFrom, 'MMM dd, yyyy');
                                arrDate.To = $.format.date(m_DateTo, 'MMM dd, yyyy');

                                ClosePopUp();
                                if (vParams.SetText != undefined) {
                                    $(vParams.SetText).html('Yesterday');
                                }
                                vParams.Click(arrDate);
                            })

                            $('#btnCustomPeriodToday').unbind('click').click(function () {
                                arrDate.Period = "Today";
                                arrDate.From = $.format.date(m_DateFrom, 'MMM dd, yyyy');
                                arrDate.To = $.format.date(m_DateTo, 'MMM dd, yyyy');

                                ClosePopUp();
                                if (vParams.SetText != undefined) {
                                    $(vParams.SetText).html('Today');
                                }
                                vParams.Click(arrDate);
                            })

                            $('#btnCustomPeriodWeekToDate').unbind('click').click(function () {
                                m_DateFrom.setDate(m_DateFrom.getDate() - m_DateFrom.getDay());

                                arrDate.Period = "Week to Date";
                                arrDate.From = $.format.date(m_DateFrom, 'MMM dd, yyyy');
                                arrDate.To = $.format.date(m_DateTo, 'MMM dd, yyyy');

                                ClosePopUp();
                                if (vParams.SetText != undefined) {
                                    $(vParams.SetText).html('Week To Date');
                                }
                                vParams.Click(arrDate);
                            })

                            $('#btnCustomPeriodMonthToDate').unbind('click').click(function () {
                                m_DateFrom.setDate(m_DateFrom.getDate() - (m_DateFrom.getDate() - 1));

                                arrDate.Period = "Month to Date";
                                arrDate.From = $.format.date(m_DateFrom, 'MMM dd, yyyy');
                                arrDate.To = $.format.date(m_DateTo, 'MMM dd, yyyy');

                                ClosePopUp();
                                if (vParams.SetText != undefined) {
                                    $(vParams.SetText).html('Month To Date');
                                }
                                vParams.Click(arrDate);
                            })

                            $('#btnCustomPeriodCustom').unbind('click').click(function () {
                                DualCalendar(vParams, m_DateFrom, m_DateTo);
                            })

                            $('#popupCustomPeriod').css({ visibility: 'visible', display: 'block' }).popup('open');

                            $('#btnCustomPeriodTodayOrEarlier').unbind('click').click(function () {
                                ClosePopUp();
                                vParams.Click('Today or Earlier');
                            });

                            RepositionPopup();
                        }

                        var dataTableBrowsePopup, isButtonRefresh = false;
                        
                        window.BrowsePopup = function (LibraryName, title, vParams, data) {
                            if (typeof Bs != 'undefined') {
                                //\Content\bootstrap\js\bootstrap_modal.js
                                Bs.Browse(LibraryName, title, vParams, data)
                                return;
                            }

                            data = $.extend({
                                DontCloseOnOverlayClick: false
                            }, data);
                            isUniversalPopup = true;
                            var list = $('#lstOfCustomBrowseLibrary'), returnData;
                            var isFromListData = false, styleformat = 'list', arrSelectedData = [], isResultFromBarcodeScanner = false;
                            list.empty();
                            //$('#tblBrowsePopupHeader, #tblBrowsePopupBody').empty();
                            $('.divBrowseFooter').hide();
                            $('#tblBrowsePopup_wrapper > .bottom').hide(); // hide pagination
                            if (CStr2(vParams[0].TopText).trim() == '')
                                $('#divBrowserTopText').hide();
                            else
                                $('#divBrowserTopText').show().text(CStr2(vParams[0].TopText));
                            browsepopup.remove();
                            browsepopup.popup();

                            ClosePopUp();
                            page.append($('<div>').addClass('popupCustomOverlayTheme').css(cssOverLay));
                            $('#popupCustomBrowseLibrary').css({ visibility: 'visible', display: 'block' }).popup('open');
                            $('#popupCustomBrowseLibrary').find('> .ui-body-d').css({ height: 'auto', maxWidth: '500px' });

                            $('#divBrowseListFormat, #divBrowseTableFormat, #divGlobalAddSKU').hide();
                            
                            $('#tblOption').empty();
                            if (LibraryName.toLowerCase() == "librarypimproduct") {
                                var tblOption = $('#tblOption');
                                var tblOptionTR = $('<tr>');

                                tblOptionTR.append('<td>Show:</td>' +
                                   '        <td>' +
                                   '            <div class="custom-radio"> <label class="radio-only"> <input type="radio" value="0" name="rbFilter" data-iconpos="notext" checked /> </label> <div>Active</div> </div>' +
                                   '        </td>' +
                                   '        <td>' +
                                   '            <div class="custom-radio"> <label class="radio-only"> <input type="radio" value="1" name="rbFilter" data-iconpos="notext" /> </label> <div>Inactive</div> </div>' +
                                   '        </td>')

                                if (CBool(vParams[0].AddUsingWebForm)) { // option to add SKU
                                    $('#divGlobalAddSKU').show();
                                } else {
                                    $('#divGlobalAddSKU').hide();
                                }

                                tblOption.append(tblOptionTR);

                                $('#tblOption').append('<tr>' +
                                   
                                   '    </tr>');
                                $('#tblOption').append('<tr name=trDefineNewProduct>' + // style="dislay:none" has no effect
                                    '<td colspan=4>' +
                                    'If you’ve correctly spelled your search but can’t find it, <br /> you might want to <a class="custom-link">define a new product</a>.' + 
                                    '<td>' + 
                                    '    </tr>');
                                browsepopup.find('[name="rbFilter"]').checkboxradio().checkboxradio('refresh');
                                $('#tblOption').find('[name=trDefineNewProduct]').hide();
                                $('#tblOption').find('[name=trDefineNewProduct] .custom-link').click(function() {
                                    var pQuickPim = GetStringObject('ERP.QuickPim.controller');
                                    if (pQuickPim) {
                                        ClosePopUp();
                                        pQuickPim.popup();
                                        //setTimeout(function() {
                                        //    pQuickPim.popup();
                                        //}, 5000);
                                    }
                                });
                            } else if (LibraryName.toLowerCase() == "transinvoices") {
                                var pStr = '<tr>' +
                                   '        <td>Show:</td>' +
                                   '        <td>' +
                                   '            <div class="custom-radio"> <label class="radio-only"> <input type="radio" value="0" name="rbFilter" data-iconpos="notext" checked /> </label> <div>Active</div> </div>' +
                                   '        </td>' +
                                   '        <td>' +
                                   '            <div class="custom-radio"> <label class="radio-only"> <input type="radio" value="1" name="rbFilter" data-iconpos="notext" /> </label> <div>Cancelled</div> </div>' +
                                   '        </td>';

                                if (data.TransType == 'Invoice')
                                    pStr += '<td></td>' +
                                        '        <td>' +
                                        '            <div class="custom-radio"> <label class="radio-only"> <input type="radio" value="eas" name="rbInvoiceSource" data-iconpos="notext" checked /> </label> <div>EAS Invoices</div> </div>' +
                                        '        </td>' +
                                        '        <td>' +
                                        '            <div class="custom-radio"> <label class="radio-only"> <input type="radio" value="pos" name="rbInvoiceSource" data-iconpos="notext" /> </label> <div>POS Invoices</div> </div>' +
                                        '        </td>';

                                if (vParams[0].ShowHideChildSos) {
                                    pStr += '<td style="padding-left:60px">Show child SO\'s:</td>' +
                                        '        <td>' +
                                        '            <div class="custom-radio"> <label class="radio-only"> <input type="radio" value="1" name="rbFilterShowChildSos" data-iconpos="notext"  /> </label> <div>Yes</div> </div>' +
                                        '        </td>' +
                                        '        <td>' +
                                        '            <div class="custom-radio"> <label class="radio-only"> <input type="radio" value="0" name="rbFilterShowChildSos" data-iconpos="notext" checked /> </label> <div>No</div> </div>' +
                                        '        </td>';
                                }

                                if (vParams[0].ShowLastModifiedFilter) {
                                    pStr += ' <td>' + '\n' + 
                                        '     <button id="butMessageBoxLastModifiedFilter"  style="margin-right:10px;" data-role="none" class="btn btn-less-width btn-transparent btnRequired">' + '\n' + 
                                        '         <span class="lblSelectedInventoryType-x"> Last Modified </span>' + '\n' + 
                                        '         <span class="ui-icon ui-icon-arrow-d" style="display:inline-block; margin-left:10px;"></span>' + '\n' + 
                                        '     </button>' + '\n' + 
                                        ' </td>';
                                }

                                if (vParams[0].ShowApprovedFilter) {
                                    pStr += '<td style="padding-left: 30px;">' + '\n' + 
                                            '    <select data-role="none" class="form-control fixed" id="selApproveStatus">' + '\n' + 
                                            '        <option value="unapproved">Unapproved</option>' + '\n' + 
                                            '        <option value="approved">Approved</option>' + '\n' + 
                                            '        <option value="all">All</option>' + '\n' + 
                                            '    </select>' + '\n' + 
                                            '</td>';
                                    browsepopup.find('#selApproveStatus').change(function() {
                                        BrowseNow();
                                    });
                                }

                                pStr += '    </tr>';
                                $('#tblOption').append(pStr);
                                browsepopup.find('[name="rbFilter"]').checkboxradio().checkboxradio('refresh');
                                browsepopup.find('[name="rbInvoiceSource"]').checkboxradio().checkboxradio('refresh');
                                browsepopup.find('#selApproveStatus').change(function() {
                                    BrowseNow();
                                });
                                if (vParams[0].ShowHideChildSos) 
                                    browsepopup.find('[name="rbFilterShowChildSos"]').checkboxradio().checkboxradio('refresh');
                                if (vParams[0].ShowPoAndInvoiceRadios) {
                                    $('#tblOption').append('<tr>' +
                                       '        <td>Show:</td>' +
                                       '        <td>' +
                                       '            <div class="custom-radio"> <label class="radio-only"> <input type="radio" value="po" name="rbFilterPoAndInvoice" data-iconpos="notext" checked /> </label> <div>Purchase Orders</div> </div>' +
                                       '        </td>' +
                                       '        <td>' +
                                       '            <div class="custom-radio"> <label class="radio-only"> <input type="radio" value="invoice" name="rbFilterPoAndInvoice" data-iconpos="notext" /> </label> <div>Invoices</div> </div>' +
                                       '        </td>' +
                                       '    </tr>');
                                    browsepopup.find('[name="rbFilterPoAndInvoice"]').checkboxradio().checkboxradio('refresh');
                                }
                                if (vParams[0].ShowLastModifiedFilter) {
                                    $('#butMessageBoxLastModifiedFilter')
                                        .text('Last Modified: ' + FormatDate(vParams[0].LastModifiedFrom, {Format: 'MMM d, yyyy'}) + ' to ' + FormatDate(vParams[0].LastModifiedTo, {Format: 'MMM d, yyyy'}))
                                        .click(function() {
                                            DualCalendar({
                                                Click: function (e) {
                                                    vParams[0].LastModifiedFilterChange(e);
                                                    mLastModifiedFrom = e.From;
                                                    mLastModifiedTo = e.To;
                                                }
                                            }, new Date(), new Date())
                                        });
                                }

                                
                            } else if (LibraryName.toLowerCase() == "TransProfitability".toLowerCase()) {

                                pStr += ' <td style="padding-left: 30px;">' + '\n' + 
                                        '    <button id="butMessageBoxLastModifiedFilter"  style="margin-right:10px;" data-role="none" class="btn btn-less-width btn-transparent btnRequired">' + '\n' + 
                                        '        <span class=""> Original Transaction Last Modified </span>' + '\n' + 
                                        '        <span class="ui-icon ui-icon-arrow-d" style="display:inline-block; margin-left:10px;"></span>' + '\n' + 
                                        '    </button>' + '\n' + 
                                        '</td>' + '\n' + 
                                        '<td>' + '\n' + 
                                        '    <select data-role="none" class="form-control fixed" id="selProfitabilityApproveStatus">' + '\n' + 
                                        '        <option value="unapproved">Unapproved</option>' + '\n' + 
                                        '        <option value="approved">Approved</option>' + '\n' + 
                                        '        <option value="all">All</option>' + '\n' + 
                                        '    </select>' + '\n' + 
                                        '</td>';
                                $('#tblOption').append(pStr);
                                mLastModifiedFrom = vParams[0].LastModifiedFrom;
                                mLastModifiedTo = vParams[0].LastModifiedTo;
                                browsepopup.find('#butMessageBoxLastModifiedFilter').text('Original Transaction Last Modified: ' + FormatDate(mLastModifiedFrom, {Format: 'MMM dd, yyyy'}) + ' to ' + FormatDate(mLastModifiedTo, {Format: 'MMM dd, yyyy'}));
                                browsepopup.find('#selProfitabilityApproveStatus').change(function() {
                                    BrowseNow();
                                });
                                browsepopup.find('#butMessageBoxLastModifiedFilter').click(function() {
                                    DualCalendar({
                                        Click: function (e) {
                                            vParams[0].LastModifiedFilterChange(e);
                                            mLastModifiedFrom = e.From;
                                            mLastModifiedTo = e.To;
                                        }
                                    }, new Date(), new Date());    
                                });

                                
                            }

                            if (vParams[0].Options != undefined) {
                                //vParams[0].Options = { Fields: [], OnChange: fx }
                                //Fields must be an Array, the value must be a string or object HTML table rows
                                //OnChange must return array object
                                for (var i = 0; i < vParams[0].Options.Fields.length; i++) {
                                    $('#tblOption').html(vParams[0].Options.Fields[i]);
                                }

                                $('#tblOption').trigger('create');
                            }

                            if (title == '' || title == undefined) {
                                title = '&nbsp;';
                            }
                            else {
                                title = title;
                            }

                            if (data == undefined) {
                                data = {}
                            }

                            if (vParams[0].AddButton != undefined) {
                                $('#divClearBtn').show();
                                if (vParams[0].AddButton[0].Text != undefined) {
                                    $('#btnCustomClear').html(vParams[0].AddButton[0].Text);
                                }
                                else {
                                    $('#btnCustomClear').text('Clear');
                                }

                                $('#btnCustomClear').click(function () {
                                    ClosePopUp();
                                    vParams[0].AddButton[0].Click();
                                })
                            }
                            else {
                                $('#divClearBtn').hide();
                            }

                            if (CBool(vParams[0].WithSearchbox) == true || vParams[0].WithSearchbox == undefined) {
                                $('#divSearchBoxBrowseLibrary').show().empty().append($('<input>')
                                    .attr({
                                        text: 'text',
                                        id: 'txtBrowseLibrary',
                                        placeholder: 'Search here'
                                    })
                                );
                                $('#divSearchBoxBrowseLibrary').trigger('create')
                                $('#txtBrowseLibrary').focus().val(data.keywords || '');
                            }
                            else {
                                $('#divSearchBoxBrowseLibrary').hide();
                            }                            

                            if (vParams[0].Search != undefined) {
                                $('#txtBrowseLibrary').val(vParams[0].Search);
                                if (vParams[0].listData == undefined) {
                                    isResultFromBarcodeScanner = false;
                                    data.keywords = vParams[0].Search;
                                    BrowseNow();
                                    RepositionPopup();
                                }
                            }

                            if (vParams[0].Close != undefined) {
                                closeFunction = vParams[0].Close;
                            }

                            if (vParams[0].StyleFormat != undefined) {
                                if (vParams[0].StyleFormat == 'table') {                                   
                                    $('#popupCustomBrowseLibrary').find('> .ui-body-d').css('max-width', 'inherit');
                                    styleformat = 'table';
                                    if ($.fn.DataTable.isDataTable($('#tblBrowsePopup'))) {
                                        dataTableBrowsePopup.destroy();
                                        $('#tblBrowsePopupBody').empty();
                                    }
                                } else {
                                    styleformat = 'list';
                                }
                            } else {
                                styleformat = 'list';
                            }

                            if (CBool(vParams[0].AutoSearch) == true) {
                                BrowseNow();
                            }

                            if (vParams[0].listData != undefined) {
                                (CStr2(vParams[0].StyleFormat) == 'table' ? CreateHeader() : '');
                                DisplayBrowseResult(vParams[0].listData);
                                isFromListData = true;
                            }

                            $('#txtCustomBrowseLibraryTitle').html(title);
                            $('#divMsgErrorBrowseLibrary').hide();

                            if (vParams[0].Message != undefined) {
                                $('#divMsgErrorBrowseLibrary').html(vParams[0].Message).show();
                            }

                            if (CBool(vParams[0].Barcode)) {
                                if (OfflineMode) {
                                    $('#divBarcode').show();
                                } else {
                                    $('#divBarcode').hide();
                                }
                            } else {
                                $('#divBarcode').hide();
                            }

                            $('#btnGlobalAddSKU').click(function () {
                                PopupPIMWebForm();
                            })

                            $('#txtBrowseLibrary').keypress(function (e) {
                                if (e.which == 13 && isFromListData == false) {
                                    if (vParams[0].AllowEmptySearch != undefined && !CBool(vParams[0].AllowEmptySearch) && $.trim($(this).val()) == '') {
                                        return;
                                    }
                                    if (styleformat == 'table') {
                                        if ($.fn.DataTable.isDataTable($('#tblBrowsePopup'))) {
                                            dataTableBrowsePopup.destroy()
                                        }
                                    }
                                    isResultFromBarcodeScanner = false;
                                    data.keywords = $.trim($(this).val());
                                    BrowseNow();
                                    RepositionPopup();
                                }

                                isFromListData = false;
                            })

                            $('#btnBrowseScanBarcode').unbind('click').click(function () {
                                mobile.Scan({
                                    Success: function (vData) {
                                        isResultFromBarcodeScanner = true;
                                        data.keywords = $.trim(vData.Value);
                                        $('#txtBrowseLibrary').val(data.keywords);
                                        BrowseNow();
                                        RepositionPopup();
                                    }
                                });
                            })

                            if (!data.DontCloseOnOverlayClick)
                                $('.popupCustomOverlayTheme').unbind('click').click(function () {
                                    ClosePopUp();
                                });

                            $('#btnOkBrowseSelectMultiple').unbind('click').click(function () {
                                ClosePopUp();
                                closeFunction = '';
                                vParams[0].Click(arrSelectedData, isResultFromBarcodeScanner);
                            });

                            $('[name="rbFilter"],[name=rbInvoiceSource]').unbind('change').change(function () {
                                data.keywords = $.trim($('#txtBrowseLibrary').val());
                                BrowseNow();
                                RepositionPopup();
                            });

                            $('[name="rbFilterShowChildSos"]').unbind('change').change(function () {
                                data.keywords = $.trim($('#txtBrowseLibrary').val());
                                BrowseNow();
                                RepositionPopup();
                            });

                            $('[name="rbFilterPoAndInvoice"]').unbind('change').change(function () {
                                data.keywords = $.trim($('#txtBrowseLibrary').val());
                                BrowseNow();
                                RepositionPopup();
                            });

                            function DisplayBrowseResult(vData) {
                                var vRows;
                                list.empty();
                                $('#tblBrowsePopupBody').empty();

                                if (CStr2(vData.Results) == '') {
                                    vRows = vData[vParams[0].ReturnObjectName];
                                } else {
                                    vRows = vData.Results.Rows;
                                }

                                if (CBool(vParams[0].SelectMultiple) 
                                    || CBool(vParams[0].ShowOkButton)) {

                                    $('#popupCustomBrowseLibrary').find('> .divBrowseFooter').show();
                                } else {
                                    $('#popupCustomBrowseLibrary').find('> .divBrowseFooter').hide();
                                }

                                if (vParams[0].StyleFormat != undefined) {
                                    if (vParams[0].StyleFormat == 'table') {
                                        $('#divBrowseListFormat').hide();
                                        $('#divBrowseTableFormat').show();
                                    } else {
                                        $('#divBrowseListFormat').show();
                                        $('#divBrowseTableFormat').hide();
                                    }
                                } else {
                                    $('#divBrowseListFormat').show();
                                    $('#divBrowseTableFormat').hide();
                                }

                                if (vRows.length > 0) {
                                    if (styleformat == 'table') {
                                        var dtColumns = [];
                                        for (var m = 0; m < vParams[0].DisplayText.length; m++) {
                                            var params = vParams[0].DisplayText[m];
                                            var pRender = function (data) {
                                                return CStr2(data);
                                            };
                                            if ($.isFunction(params.Render))
                                                pRender = params.Render;
                                            dtColumns.push({
                                                "data": params.Column,
                                                "width": (params.Style == undefined ? 'auto' : params.Style.width),
                                                "render": pRender,

                                                /*
                                                    https://datatables.net/manual/styling/classes#Cell-classes
                                                    possible values:
                                                    dt[-head|-body]-left
                                                    dt[-head|-body]-center
                                                    dt[-head|-body]-right
                                                    dt[-head|-body]-justify
                                                    dt[-head|-body]-nowrap
                                                */
                                                className: params.className
                                            });
                                        }

                                        var pDataTableArgs = {
                                            order: [(vParams[0].OrderBy == undefined ? [0, 'asc'] : vParams[0].OrderBy)], //vParams[0].OrderBy is array . use only in display table
                                            data: vRows,
                                            fixedHeader: vParams[0].FixedHeader,
                                            //"scrollX": true,
                                            "scrollY": vParams[0].FixedHeader ? '300px' : '',
                                            //"scrollCollapse": true,
                                            columns: dtColumns,
                                            initComplete: function () {
                                                PopupReposition();
                                                if ($.isFunction(vParams[0].InitComplete))
                                                    vParams[0].InitComplete(this); // "this" is a table jquery object
                                            },
                                            //buttons: [{extend: 'excel', text: 'Download Excel File'}],
                                            //dom: 'Bfrtip'
                                            sDom: '<"top">rt<"bottom"ip><"clear">', //to show button add B to <"top"> (<"top"B>)
                                            rowCallback: function(row, data, dataIndex) {
                                                $(row).data('data', data);
                                                if (vParams[0].rowCallback)
                                                    vParams[0].rowCallback.apply(this, arguments);
                                            },
                                            bSort: (vParams[0].Sorting == undefined ? true : CBool(vParams[0].Sorting))
                                        };
                                        if (CStr2(vParams[0].DownloadCsvTitle).trim() != '') {
                                            pDataTableArgs.buttons = [
                                                {
                                                    extend: 'csvHtml5', 
                                                    text: 'Download CSV', 
                                                    title: vParams[0].DownloadCsvTitle
                                                }
                                            ];
                                            pDataTableArgs.sDom = '<"top"B>rt<"bottom"ip><"clear">';
                                        }
                                        if (vParams[0].NoInitialSorting)
                                            pDataTableArgs.order = [];
                                        dataTableBrowsePopup = $('#tblBrowsePopup').DataTable(pDataTableArgs);

                                        $('#tblBrowsePopup tbody')
                                            .unbind('click')
                                            .on('click', 'td', function () {
                                                var pData = dataTableBrowsePopup.row(this).data();                                                
                                                if (pData != undefined) {
                                                    var dt = $('#tblBrowsePopup').DataTable();
                                                    pData.DataTableRowColumnIndex = dt.cell(this).index().columnVisible;
                                                    TriggerClick(pData, $(this));
                                                }
                                            })

                                        $('#tblBrowsePopup').css({ width: '100%' });
                                        PopupReposition();
                                    } else {
                                        for (var i = 0; i < vRows.length; i++) {
                                            var listName = $('<a>').addClass('ui-link-inherit');
                                            var isWideSpace = false; // <br><br>
                                            var tr = $('<tr>'); // for table format
                                            for (var m = 0; m < vParams[0].DisplayText.length; m++) {
                                                var txtName = '';
                                                var params = vParams[0].DisplayText[m];

                                                if ($.isFunction(params.Column)) {
                                                    txtName = params.Column(vRows[i]);
                                                } else {
                                                    txtName = params.Column.replace(/\"/g, "");
                                                    txtName = $('<textarea />').html(vRows[i][txtName]).text();
                                                }

                                                if (styleformat == 'table') {
                                                    tr.append($('<td>')
                                                        .css((params.Style == undefined ? {} : params.Style))
                                                        .attr((params.Attr == undefined ? {} : params.Attr))
                                                        .html('<b class="ui-table-cell-label">' + params.Label + '</b>' + txtName))
                                                } else {
                                                    if (vParams[0].DisplayText.length >= 2) {
                                                        if (m < 1) {
                                                            listName.append((params.Label == undefined ? txtName : params.Label + ' <span style="font-weight:normal">' + txtName + '</span>'));
                                                        }
                                                        else {
                                                            if (isWideSpace == false) {
                                                                listName.append('<br><br>');
                                                                isWideSpace = true;
                                                            }
                                                            else {
                                                                listName.append('<br>');
                                                            }

                                                            listName.append((params.Label == undefined ? '<span style="font-weight:normal">' + txtName + '</span>' : params.Label + ' <span style="font-weight:normal">' + txtName + '</span>'))
                                                        }
                                                    }
                                                    else {
                                                        listName.append((params.Label == undefined ? txtName : params.Label + ' <span style="font-weight:normal">' + txtName + '</span>'));
                                                    }
                                                }
                                            }

                                            var isExist = '';
                                            for (var w = 0; w < arrSelectedData.length; w++) {
                                                if (JSON.stringify(vRows[i]) == JSON.stringify(arrSelectedData[w])) {
                                                    isExist = 'browseSelected';
                                                }
                                            }

                                            if (styleformat == "table") {
                                                $('#tblBrowsePopupBody').append(tr
                                                        .css('cursor', 'pointer')
                                                        .addClass(isExist)
                                                        .data('data', vRows[i])
                                                        .click(function (e) {
                                                            e.preventDefault();
                                                            TriggerClick($(this).data('data'), $(this));
                                                        })
                                                    );
                                                $('#tblBrowsePopupBody').parent().table();
                                                $('body').resize();
                                            } else {
                                                list.append($('<li data-corners="false" data-shadow="false" data-iconshadow="true" data-wrapperels="div" data-icon="arrow-r" data-iconpos="right" data-theme="d" class="ui-btn ui-btn-icon-right ui-li-has-arrow ui-li ui-first-child ui-last-child ui-btn-up-d">')
                                                        .css('cursor', 'pointer')                                                        
                                                        .append($('<div>')
                                                            .addClass('ui-btn-inner ui-li')
                                                            .append($('<div>')
                                                                .addClass('ui-btn-text')
                                                                .append(listName
                                                                    .addClass(isExist)
                                                                    .data('data', vRows[i])
                                                                    .unbind('click')
                                                                    .click(function (e) {
                                                                        e.preventDefault();
                                                                        TriggerClick($(this).data('data'), $(this));
                                                                    })
                                                                )
                                                            )
                                                            .append($('<span>')
                                                                .addClass('ui-icon ui-icon-arrow-r ui-icon-shadow')
                                                                .html('&nbsp;')
                                                            )
                                                        )
                                                    )
                                            }
                                        }
                                        PopupFitToScreen();
                                    }
                                }
                                else {
                                    if (styleformat == 'table') {
                                        $('#tblBrowsePopupBody').append('<tr><td style="text-align:center" colspan="' + vParams[0].DisplayText.length + '">No Results(s) Found</td></tr>')
                                    } else {
                                        list.append('<li style="text-align:center;">No Result(s) Found</li>');
                                    }
                                    $('#tblOption').find('[name=trDefineNewProduct]').show();
                                    (arrSelectedData.length > 0 ? $('.divBrowseFooter').show() : $('.divBrowseFooter').hide());
                                }
                                RepositionPopup();
                                isFromListData = false;
                            }

                            function TriggerClick(data, element) {
                                if (vParams[0].Validation != undefined) {
                                    var result = vParams[0].Validation(data, isResultFromBarcodeScanner);
                                    if (result != true) {
                                        if (result != false) {
                                            $('#divMsgErrorBrowseLibrary').show();
                                            $('#divMsgErrorBrowseLibrary').html(result);
                                        }
                                        else {
                                            $('#divMsgErrorBrowseLibrary').hide();
                                        }
                                        RepositionPopup();
                                        return false;
                                    }
                                }

                                if (!CBool(vParams[0].SelectMultiple)) {
                                    if (!CBool(vParams[0].DontCloseWhenClick)) {
                                        ClosePopUp();
                                        closeFunction = '';
                                        vParams[0].Click(data, isResultFromBarcodeScanner);
                                    }                                    
                                } else {
                                    if(styleformat.toLowerCase() == 'table'){
                                        element = element.parent();
                                    }

                                    if (!element.hasClass('browseSelected')) {
                                        element.addClass('browseSelected');
                                        arrSelectedData.push(data);
                                    } else {
                                        element.removeClass('browseSelected');

                                        for (var i = 0; i < arrSelectedData.length; i++) {
                                            if (data.ID == arrSelectedData[i].ID) {
                                                (arrSelectedData).splice(i, 1);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }

                            function BrowseNow() {
                                if (vParams[0].Options != undefined) {
                                    if (vParams[0].Options.OnChange != undefined) {
                                        data = $.extend(data, vParams[0].Options.OnChange())
                                    }
                                }

                                if (styleformat == 'table') {
                                    $('#tblBrowsePopupBody').empty().append('<tr><td style="text-align:center" colspan="' + vParams[0].DisplayText.length + '"><img alt="loading" src="/Content/images/ajax-loader.gif" align="absmiddle"> Loading ...</td></tr>')
                                } else {
                                    list.empty();
                                    list.append('<li style="text-align:center;"><img alt="loading" src="/Content/images/ajax-loader.gif" align="absmiddle"> Loading ...</li>');
                                }
                                $('#popupCustomBrowseLibrary').find('> .ui-body-d').css('height', 'auto');
                                $('.divBrowseFooter,[name="trDefineNewProduct"]').hide();
                                if (OfflineMode) {
                                    if (vParams[0].Offline != undefined) {
                                        setTimeout(function () {
                                            var OfflineBrowser,OBFunction;

                                            if (vParams[0].OfflineBrowser == undefined || vParams[0].OfflineBrowser == true){
                                                //mao ni ang default
                                                //pag kwaon sa global variable na OfflineBrowser (/Scripts3/Offline/Browser/...)
                                                OfflineBrowser = 'return OfflineBrowser.' + LibraryName + '.GetData(' + JSON.stringify(data) + ')';
                                                OBFunction = new Function(OfflineBrowser.replace(/\'/g, ''));
                                            } else {
                                                //mao ni ang bago as of 1-27-2021 11:17 AM by arnie
                                                //pag buhat2 na function. Eg: LibraryName value = Expense(true) or Org.GetData() or any. See specific example sa Expenses.js kung giunsa
                                                //dapat global ang function
                                                OfflineBrowser = 'return ' + LibraryName;
                                                OBFunction = new Function(OfflineBrowser);
                                            }

                                            if (styleformat == "table") {
                                                CreateHeader();
                                                DisplayBrowseResult({ Results: OBFunction() });
                                            } else {
                                                DisplayBrowseResult({ Results: OBFunction() });
                                            }
                                        }, 20);
                                        return;
                                    }
                                }

                                var url = ((LibraryName).indexOf('/') == -1 ? 'Browser/' + LibraryName : LibraryName);

                                if (LibraryName.toLowerCase() == "librarypimproduct") {
                                    data.IsDeactivated = $('[name="rbFilter"]:checked').val();
                                } else if (LibraryName.toLowerCase() == "transinvoices") {                            
                                    data.IsCancelled = $('[name="rbFilter"]:checked').val();
                                    data.FilterPoAndInvoice = $('[name="rbFilterPoAndInvoice"]:checked').val();
                                    if (CBool(vParams[0].ShowHideChildSos))
                                        data.ShowChildSos = $('[name="rbFilterShowChildSos"]:checked').val();
                                    else
                                        data.ShowChildSos = true;
                                    data.InvoiceSource = $('[name="rbInvoiceSource"]:checked').val();
                                }
                                if (mLastModifiedFrom != null)
                                    data.LastModifiedFrom = mLastModifiedFrom;
                                if (mLastModifiedTo != null)
                                    data.LastModifiedTo = mLastModifiedTo;

                                var pProfitabilityApproveStatus = $('#selProfitabilityApproveStatus').val();
                                if (CStr2(pProfitabilityApproveStatus) != '')
                                    data.ApprovedStatus = pProfitabilityApproveStatus;

                                var pApproveStatus = $('#selApproveStatus').val();
                                if (CStr2(pApproveStatus) != '')
                                    data.ApprovedStatus = pApproveStatus;

                                if (vParams[0].OfflineBrowser != null) {
                                    var pSearchResult = vParams[0].OfflineBrowser.GetData(data);
                                    pSearchResult = {
                                        Results: pSearchResult
                                    };
                                    DisplayResults(pSearchResult);
                                } else {
                                    if (CBool(vParams[0].ServerSideProcessing)) {
                                        $('#divBrowseListFormat').hide();
                                        $('#divBrowseTableFormat').show();

                                        CreateHeader();

                                        var dtColumns = [];
                                        for (var m = 0; m < vParams[0].DisplayText.length; m++) {
                                            var params = vParams[0].DisplayText[m];
                                            var pRender = function (data) {
                                                return CStr2(data);
                                            };
                                            if ($.isFunction(params.Render))
                                                pRender = params.Render;
                                            dtColumns.push({
                                                "data": params.Column,
                                                "width": (params.Style == undefined ? 'auto' : params.Style.width),
                                                "render": pRender
                                            });
                                        }

                                        dataTableBrowsePopup = $('#tblBrowsePopup').DataTable({
                                            order: [(vParams[0].OrderBy == undefined ? [0, 'asc'] : vParams[0].OrderBy)], //vParams[0].OrderBy is array . use only in display table
                                            ajax: {
                                                url: MergeUrlParameters($.extend(data, { OrgID: sessionStorage.OrgID }), url),
                                                type: 'POST'
                                            },
                                            "pageLength": (vParams[0].PageLength || 25),
                                            columns: dtColumns,
                                            "bProcessing": true,
                                            "bServerSide": true,
                                            "drawCallback": function( settings ) {
                                                PopupReposition();
                                            },
                                            initComplete: function () {                                                
                                                if ($.isFunction(vParams[0].InitComplete))
                                                    vParams[0].InitComplete(this); // "this" is a table jquery object
                                            },
                                            //buttons: [{extend: 'excel', text: 'Download Excel File'}],
                                            //dom: 'Bfrtip'
                                            sDom: '<"top">rt<"bottom"ip><"clear">', //to show button add B to <"top"> (<"top"B>)
                                            rowCallback: function (row, data, dataIndex) {
                                                $(row).data('data', data);
                                                if (vParams[0].rowCallback)
                                                    vParams[0].rowCallback.apply(this, arguments);
                                            },
                                            bSort: (vParams[0].Sorting == undefined ? true : CBool(vParams[0].Sorting))
                                        });

                                        $('#tblBrowsePopup tbody')
                                            .unbind('click')
                                            .on('click', 'td', function () {
                                                var pData = dataTableBrowsePopup.row(this).data();
                                                if (pData != undefined) {
                                                    var dt = $('#tblBrowsePopup').DataTable();
                                                    pData.DataTableRowColumnIndex = dt.cell(this).index().columnVisible;
                                                    TriggerClick(pData, $(this));
                                                }
                                            })

                                        $('#tblBrowsePopup').css({ width: '100%' });
                                    } else {
                                        AjaxSend({
                                            url: url,
                                            data: data,
                                            success: function (vData) {
                                                DisplayResults(vData);
                                            }
                                        });
                                    }                                    
                                }                                    

                                function DisplayResults(vData) {
                                    if (vData.Error) {
                                        NewMessageBox(vData.Error);
                                        return;
                                    }
                                    if (vParams[0].ReturnVariable != undefined) {
                                        var arrVariable = (vParams[0].ReturnVariable).split('.');
                                        for (var i = 0; i < arrVariable.length; i++) {
                                            vData = vData[arrVariable[i]];
                                        }
                                        //magbase ta kung unsa ang usually return json format sa BrowseController
                                        vData = {
                                            Results: {
                                                Rows: vData
                                            }
                                        };
                                    }
                                    if (styleformat == "table") {
                                        CreateHeader();
                                        DisplayBrowseResult(vData);
                                    } else {
                                        DisplayBrowseResult(vData);
                                    }
                                }
                            }

                            function CreateHeader() {
                                if ($.fn.DataTable.isDataTable($('#tblBrowsePopup'))) 
                                    dataTableBrowsePopup.destroy();
                                var tr = $('<tr class="custom-theme-f">');
                                var tblBrowsePopupHeader = $('#tblBrowsePopupHeader');

                                tblBrowsePopupHeader.empty();

                                for (var i = 0; i < vParams[0].DisplayText.length; i++) {
                                    var data = vParams[0].DisplayText[i];
                                    tr.append($('<td>')
                                            .css('text-align', 'center')
                                            .text((data.Label == undefined ? '' : data.Label))
                                        )
                                }

                                tblBrowsePopupHeader.append(tr);
                            }

                            window.BrowsePopupNow = BrowseNow;

                            RepositionPopup();
                        }

                        window.PopupButtonList = function (msg, title, vParams, vOption) {
                            if (typeof Bs != 'undefined') {
                                //\Content\bootstrap\js\bootstrap_modal.js
                                Bs.PopupButtonList(msg, title, vParams, vOption)
                                return;
                            }

                            isUniversalPopup = true;                            

                            listpopup.remove();
                            listpopup.popup();

                            ClosePopUp();
                            page.append($('<div>').addClass('popupCustomOverlayTheme').css(cssOverLay));

                            var listButton = $('#lstOfCustomList');
                            listButton.empty();

                            if (title == '') {
                                title = '&nbsp';
                            }

                            if (msg != '') {
                                $('#txtCustomListMsg').show();
                                $('#txtCustomListMsg').html(msg);
                            }
                            else {
                                $('#txtCustomListMsg').hide();
                            }

                            if (vOption != undefined) {
                                if (vOption.CloseButton != undefined) {
                                    if (vOption.CloseButton == "enable") {
                                        $('.btnCustomPopupClose').show();
                                    }
                                    else {
                                        $('.btnCustomPopupClose').hide();
                                    }
                                }

                                if (vOption.AutoDismiss != undefined) {
                                    if (vOption.AutoDismiss == true) {
                                        $('.popupCustomOverlayTheme').unbind('click').click(function () {
                                            ClosePopUp();
                                        })
                                    }
                                }
                                else {
                                    $('.popupCustomOverlayTheme').unbind('click').click(function () {
                                        ClosePopUp();
                                    })
                                }

                                if (vOption.OverflowScroll != undefined) {
                                    $('#divOfCustomList').css('max-height', '100%');
                                }
                            }

                            $('#txtCustomListTitle').html(title);

                            try {
                                $('#popupCustomList').css({ visibility: 'visible', display: 'block' }).popup('open').trigger('create');
                            } catch (e){

                            }                            

                            if (vParams.length > 0) {
                                for (var i = 0; i < vParams.length; i++) {
                                    var setClassForImage = '';
                                    var Attr = { x: 'y' };
                                    var data = {};

                                    if (vParams[i].HasImage != undefined) {
                                        if (vParams[i].HasImage == true) {
                                            setClassForImage = 'ui-li-has-thumb';
                                        }
                                    }

                                    if (vParams[i].attr != undefined) {
                                        Attr = vParams[i].attr;
                                    }

                                    if (vParams[i].data != undefined) {
                                        data = vParams[i].data;
                                    }

                                    listButton.append($('<li data-corners="false" data-shadow="false" data-iconshadow="true" data-wrapperels="div" data-icon="arrow-r" data-iconpos="right" data-theme="d" class="ui-btn ui-btn-icon-right ui-li-has-arrow ui-li ' + setClassForImage + ' ui-first-child ui-last-child ui-btn-up-d">')
                                        .append($('<div>')
                                            .addClass('ui-btn-inner ui-li')
                                            .append($('<div>')
                                                .addClass('ui-btn-text')
                                                .append($('<a>')
                                                    .attr('arr', i)
                                                    .attr(Attr)
                                                    .addClass('ui-link-inherit')
                                                    .data('data', data)
                                                    .html(vParams[i].Text)
                                                    .unbind('click')
                                                    .click(function (e) {
                                                        e.preventDefault();
                                                        if (vParams[CInt2($(this).attr('arr'))].Validation != undefined) {
                                                            if (vParams[CInt2($(this).attr('arr'))].Validation($(this).data('data')) != true) {
                                                                return false;
                                                            }
                                                        }

                                                        if (vParams[CInt2($(this).attr('arr'))].Click != undefined) {
                                                            ClosePopUp();
                                                            vParams[CInt2($(this).attr('arr'))].Click($(this).data('data'));
                                                        }
                                                    })
                                                )
                                            )
                                            .append($('<span>')
                                                .addClass('ui-icon ui-icon-arrow-r ui-icon-shadow')
                                                .html('&nbsp;')
                                            )
                                        )
                                    )
                                }
                                PopupFitToScreen();
                            }
                            else {
                                $('#popupCustomList').find('> .ui-body-d').css('height', 'auto');
                                listButton.append($('<li>')
                                    .text('No Result(s) Found')
                                )
                            }

                            RepositionPopup();

                            $('#popupCustomList').bind('pageinit', function () {
                                $('#popupCustomList').trigger('create');
                                listpopup.popup();
                                RepositionPopup();
                            })
                        }

                        window.NewMessageBox = function (msg, title, vParams, vOption) {
                            //                vParams = {
                            //                    Text: 'Option',
                            //                    Click: function () { }, // optional, - use for btnPositive and btnNegative buttons
                            //                    Validation: function () { }, // optional - use for generated html tags (input, textarea, select, etc.)
                            //                    CustomClick: function () { } // optional - use for generated button or link (<a>)
                            //                }

                            if (typeof Bs != 'undefined') {
                                //\Content\bootstrap\js\bootstrap_modal.js
                                return Bs.MessageBox(msg, title, vParams, vOption);
                            }

                            isUniversalPopup = true;
                            var btnPositive, btnNegative;
                            var btnPositiveAttr = {
                                'class': 'btn custom-btn-theme-b'
                            };
                            var btnNegativeAttr = {
                                'class': 'btn custom-btn-theme-c'
                            };
                            vOption = $.extend({
                                MaxWidth: null
                            }, vOption);
                            if (vOption.MaxWidth == null) {
                                content.css('maxWidth', '400px');
                                popup.css('maxWidth', '400px');
                            } else {
                                content.css('maxWidth', vOption.MaxWidth);
                                popup.css('maxWidth', vOption.MaxWidth);
                            }
                            popup.remove();
                            popup.popup();

                            ClosePopUp();
                            if (location.pathname == "/SalesMenu/POSViewAll") {
                                page.append($('<div>').addClass('popupCustomOverlayTheme').css(cssOverLay).append(popup));
                                $('.popupCustomOverlayTheme').find('#popupCustomMsgBox')
                                    .css({
                                        'position': 'absolute', 'margin-left': '50%', 'margin-top': '40px', 'margin-bottom': '40px', 'transform': 'translate(-50%, 40px)'
                                    })
                            }
                            else {
                                page.append($('<div>').addClass('popupCustomOverlayTheme').css(cssOverLay));
                            }

                            if (title == '' || title == undefined) {
                                title = '&nbsp;';
                            }
                            else {
                                title = title
                            }

                            if (vParams != undefined) {
                                if (vParams.length > 0) {
                                    if (vParams[0].Text != undefined) {
                                        btnPositive = vParams[0].Text;
                                        btnPositiveAttr = {};

                                        //$('#btnCustomMsgBoxOK').show();
                                        if (btnPositive.toLowerCase() == "hide" || btnPositive.toLowerCase() == "hidebutton") {
                                            $('#btnCustomMsgBoxOK').hide();
                                        }

                                        if (vParams[0].CustomClick != undefined) {
                                            vParams[0].CustomClick();
                                        }

                                        if (vParams[0].Attr != undefined) {
                                            btnPositiveAttr = vParams[0].Attr;
                                        }
                                        else {
                                            btnPositiveAttr = {
                                                'class': 'btn custom-btn-theme-b'
                                            }
                                        }
                                    }
                                    else {
                                        btnPositive = 'Ok';
                                    }

                                    if (vParams[1] != undefined) {
                                        //$('#divNegativeButton').show();
                                        btnNegativeAttr = {};

                                        if (vParams[1].Text != undefined) {
                                            btnNegative = vParams[1].Text;
                                        }
                                        else {
                                            btnNegative = 'Cancel';
                                        }

                                        if (vParams[1].Attr != undefined) {
                                            btnNegativeAttr = vParams[0].Attr;
                                        }
                                        else {
                                            btnNegativeAttr = {
                                                'class': 'btn custom-btn-theme-c'
                                            }
                                        }
                                    }
                                    else {
                                        $('#divNegativeButton').hide();
                                    }
                                }
                            }
                            else {
                                btnPositive = 'Ok';
                                $('#divNegativeButton').hide();
                            }

                            if (vOption != undefined) {
                                if (vOption.CloseButton != undefined) {
                                    if (vOption.CloseButton == "enable") {
                                        $('.btnCustomPopupClose').show();
                                    }
                                    else {
                                        $('.btnCustomPopupClose').hide();
                                    }
                                }

                                if (vOption.AutoDismiss != undefined) {
                                    if (vOption.AutoDismiss == true) {
                                        $('#popupCustomMsgBox').attr('data-dismissible', true)
                                        $('.popupCustomOverlayTheme').unbind('click').click(function () {
                                            ClosePopUp();
                                        })
                                    } else {
                                        $('#popupCustomMsgBox').attr('data-dismissible', false)
                                        $('.popupCustomOverlayTheme').unbind('click')
                                    }
                                } else { // auto close messagebox
                                    $('#popupCustomMsgBox').attr('data-dismissible', true)
                                    $('.popupCustomOverlayTheme').unbind('click').click(function () {
                                        ClosePopUp();
                                    })
                                }

                                if (vOption.LiveValidation != undefined) {
                                    vOption.LiveValidation();
                                }

                                if (vOption.Close != undefined || vOption.OnClose != undefined) {
                                    closeFunction = (vOption.Close || vOption.OnClose);
                                }
                            } else { // auto close messagebox
                                $('#popupCustomMsgBox').attr('data-dismissible', true)
                                $('.popupCustomOverlayTheme').unbind('click').click(function () {
                                    ClosePopUp();
                                })
                            }

                            if (msg != "") {
                                msg = msg + '<br><br>';
                            }

                            $('#divForButtonNewMsgBox').empty();

                            if (vParams != undefined) {
                                if (vParams.length > 0) {
                                    var ctrBtn = 0;
                                    for (var i = 0; i < vParams.length; i++) {
                                        var btnTheme;
                                        var css = { 'white-space': 'normal', 'margin-bottom': '10px' }
                                        if (vParams.length >= 3) {
                                            if (vParams[i].Theme != undefined) {
                                                btnTheme = vParams[i].Theme;
                                            }
                                            else {
                                                btnTheme = 'c';
                                            }
                                        }
                                        else {
                                            if (ctrBtn == 0) {
                                                if (vOption.SameColorButtons)
                                                    btnTheme = 'c';
                                                else
                                                    btnTheme = 'b';
                                            }
                                            else {
                                                btnTheme = 'c';
                                            }
                                        }

                                        if(ctrBtn == 0 && !IsMobile){
                                            btnTheme += ' msgbox-selected-button';
                                        }

                                        if (vParams[i].Size != undefined) {
                                            if (vParams[i].Size == 2) {
                                                css.minHeight = '50px';
                                            }
                                            else if (vParams[i].Size == 3) {
                                                css.minHeight = '80px';
                                            }
                                            else if (vParams[i].Size == 4) {
                                                css.minHeight = '100px';
                                            }
                                        }

                                        $('#divForButtonNewMsgBox').append($('<button>')
                                                                                .addClass('btn custom-btn-theme-' + btnTheme)
                                                                                .attr({ 'no': i, 'data-role': 'none' })
                                                                                .css(css)
                                                                                .html(vParams[i].Text)
                                                                                .data('data', vParams[i])
                                                                                .click(function () {
                                                                                    var no = $(this).attr('no');

                                                                                    if (vParams[no].Validation != undefined) {
                                                                                        if (vParams[no].Validation() == false) {
                                                                                            return false;
                                                                                        }
                                                                                    }

                                                                                    if (vParams[no].Click == undefined) {
                                                                                        ClosePopUp();
                                                                                    }
                                                                                    else {
                                                                                        ClosePopUp();
                                                                                        vParams[no].Click();
                                                                                    }
                                                                                })
                                                                                .hover(function () {
                                                                                    if(!IsMobile){                                                                                        
                                                                                        $('#divForButtonNewMsgBox > button').removeClass('msgbox-selected-button');
                                                                                        $(this).addClass('msgbox-selected-button');
                                                                                    }
                                                                                })
                                        )
                                        ctrBtn = ctrBtn + 1;
                                    }
                                } else {
                                    $('#divForButtonNewMsgBox').append($('<button>')
                                                                    .addClass('btn custom-btn-theme-b ' + (!IsMobile ? 'msgbox-selected-button' : '')) //
                                                                    .attr({ 'data-role': 'none' })
                                                                    .css({ 'white-space': 'normal', 'margin-bottom': '10px' })
                                                                    .text('Ok')
                                                                    .click(function () {
                                                                        ClosePopUp();
                                                                    })
                                        )
                                }
                            }
                            else {
                                $('#divForButtonNewMsgBox').append($('<button>')
                                                                    .addClass('btn custom-btn-theme-b ' + (!IsMobile ? 'msgbox-selected-button' : '')) //
                                                                    .attr({ 'data-role': 'none' })
                                                                    .css({ 'white-space': 'normal', 'margin-bottom': '10px' })
                                                                    .text('Ok')
                                                                    .click(function () {
                                                                        ClosePopUp();
                                                                    })
                                        )
                            }

                            $('#txtCustomMsgBoxTitle').html(title);
                            $('#CustomMsgBoxtxtMessage').html(msg);
                            $('#btnCustomMsgBoxOK').attr(btnPositiveAttr).html(btnPositive);
                            $('#btnCustomMsgBoxCancel').attr(btnNegativeAttr).html(btnNegative);

                            $('#btnCustomMsgBoxOK').click(function () {
                                if (vParams != undefined) {
                                    if (vParams[0].Validation != undefined) {
                                        if (vParams[0].Validation() == false) {
                                            return false;
                                        }
                                    }

                                    if (vParams[0].Click != undefined) {
                                        ClosePopUp();
                                        vParams[0].Click();
                                    }
                                    else {
                                        ClosePopUp();
                                    }
                                }
                                else {
                                    ClosePopUp();
                                }
                            })

                            $('#btnCustomMsgBoxCancel').click(function () {
                                if (vParams != undefined) {
                                    if (vParams[1].Click != undefined) {
                                        ClosePopUp();
                                        vParams[1].Click();
                                    }
                                    else {
                                        ClosePopUp();
                                    }
                                }
                                else {
                                    ClosePopUp();
                                }
                            })

                            $('#CustomMsgBoxtxtMessage').trigger('create');
                            $('#popupCustomMsgBox').css({ visibility: 'visible', display: 'block' }).popup('open');
                            RepositionPopup();
                            $('#CustomMsgBoxtxtMessage').find('input:first').focus();
                            $('#CustomMsgBoxtxtMessage').find('textarea:first').focus();
                            setTimeout(function () {
                                if (CBool($('#popupCustomMsgBox').parent().hasClass('ui-popup-hidden'))) {
                                    $('.popupCustomOverlayTheme').remove();
                                }
                            }, 100)
                            RepositionPopup();
                        }

                        window.NewMessageBoxClose = function () {
                            if (typeof Bs != 'undefined') {
                                //\Content\bootstrap\js\bootstrap_modal.js
                                Bs.MessageBoxClose()
                                return;
                            }

                            ClosePopUp();
                        }

                        window.PopupReposition = function () {
                            ResizePopupBackground();
                            PopupFitToScreen();
                            RepositionPopup();
                        }

                        window.ResizePopupBackground = function () {
                            if ($('.ui-popup-active').length == 0) {
                                return;
                            }
                            // if popup content overflow within the window screen, the popup background height should update based on the popup content
                            var curPopupOpen = '#' + $('.ui-popup-active').attr('id').replace('-popup', '');
                            var curPopupHeight = $(curPopupOpen).outerHeight();
                            var windowHeight = $(window).outerHeight();

                            if (curPopupHeight > windowHeight) {
                                $(curPopupOpen + '-screen').css('height', curPopupHeight);
                            } else {
                                $(curPopupOpen + '-screen').css('height', windowHeight);
                            }
                        }

                        window.PopupFitToScreen = function () {
                            if ($('.ui-popup-active').length == 0) {
                                return;
                            }

                            var currentPopupActive = $('#' + $('.ui-popup-active').attr('id').replace('-popup', ''));
                            if (currentPopupActive.hasClass('fittoscreen')) {
                                var windowheight = $(window).outerHeight();
                                var currentPopupHeight = currentPopupActive.outerHeight();
                                var currentPopupheader = currentPopupActive.find('center').children().outerHeight();
                                var currentPopupfooter = (currentPopupActive.find('.divBrowseFooter').css('display') == 'none' ? 0 : CInt2(currentPopupActive.find('.divBrowseFooter').outerHeight()))
                                var height = windowheight - (currentPopupheader + currentPopupfooter) - 60 + 'px';

                                if (currentPopupHeight > windowheight) {
                                    currentPopupActive.find('> .ui-body-d')
                                    .css({
                                        height: height,
                                        overflow: 'auto'
                                    })
                                } else {
                                    if (currentPopupActive.find('> .ui-body-d').hasScrollBar()) {
                                        var currentPopupContentHeight = currentPopupActive.find('> .ui-body-d')[0].scrollHeight;
                                        height = ((currentPopupContentHeight + (currentPopupheader + currentPopupfooter)) - (((currentPopupContentHeight + (currentPopupheader + currentPopupfooter)) - windowheight) + (currentPopupheader + currentPopupfooter)) - 60);
                                        currentPopupActive.find('> .ui-body-d')
                                        .css({
                                            height: height,
                                            overflow: 'auto'
                                        })
                                    } else {
                                        currentPopupActive.find('> .ui-body-d')
                                        .css({
                                            height: 'auto',
                                            overflow: 'auto'
                                        })
                                    }
                                }

                                ResizePopupBackground();
                            }
                        }

                        //this is the updated function on how to add breadcrumbs - simple and better than before
                        window.AddBreadCrumb = function (data, selector) {
                            //selector means the container of breadcrumbs
                            var selector = $(selector);
                            var isNameExist = false;

                            if (CBool(data.ClearNav)){
                                selector.empty();
                            }

                            if (data.UpdateLastchild != undefined) {
                                if (CBool(data.UpdateLastchild) == true) {
                                    var lastChild = selector.find('.bcname.last-child');
                                    if (lastChild.length > 0) {
                                        lastChild
                                            .removeData('bcdata')
                                            .data('bcdata', data)
                                            .text(data.text)
                                        return;
                                    }
                                }
                            }

                            if (selector.find('span.bcname:contains("' + data.text + '")').length > 0) {
                                selector.find('span.bcname:contains("' + data.text + '")').each(function () {
                                    var dis = $(this);

                                    if ($.trim(dis.text()) == data.text) {
                                        (dis.parent().is('a') ? dis.unwrap('a') : '');
                                        dis.nextAll().remove();
                                        isNameExist = true;
                                        return;
                                    }
                                })
                            }

                            if ($.trim(selector.text()) != "" && isNameExist == false) {
                                selector.find('.bcname').each(function () {
                                    if ($(this).parent().is('a') == false) {
                                        $(this).wrap($('<a class="custom-link link-light">').css('cursor', 'pointer'));

                                        selector.append('<span>&nbsp;</span> <span class="glyphicon glyphicon-chevron-right bi-chevron-right" style="display:inline-block; margin:-2px 5px; font-size:16px;"></span> <span>&nbsp;</span>');
                                    }
                                })
                            }

                            if (isNameExist == false) {
                                selector.append($('<span>')
                                        .addClass('bcname').addClass((CBool(data.UpdateLastchild) ? 'last-child' : ''))
                                        .data('bcdata', data)
                                        .text((data.text || data.Text))
                                        .click(function () {
                                            if ($(this).data('bcdata').Click != undefined) {
                                                $(this).data('bcdata').Click();
                                            }

                                            if ($(this).parent().is('a') == true) {
                                                $(this).parent().nextAll().remove();
                                                $(this).unwrap();
                                            }

                                            $('.BreadCrumbs').animate({scrollLeft: selector.scrollLeft() + 2000}, 800)
                                        })
                                    )
                            }

                            $('.BreadCrumbs').animate({scrollLeft: selector.scrollLeft() + 2000}, 800)
                        }

                        window.RemoveBreadCrumb = function (bcs, container) {
                            //to remove multiple breadcrumb, separate 'selector' with commas
                            var bc = (bcs).split(',');

                            for (var i = 0; i < bc.length; i++) {
                                //var bcname = $(container).find('.bcname:contains(' + $.trim(bc[i]) + ')');
                                var bcname = $(container).find('.bcname').filter(function () {
                                    return $(this).text() === $.trim(bc[i]);
                                });

                                if(bcname.parent().is('a')){
                                    bcname.parent().nextAll().remove();
                                    bcname.parent().remove();
                                } else {
                                    bcname.nextAll().remove();
                                    bcname.remove();
                                }
                                
                                $(container).find('.bcname:last').parent().nextAll().remove();
                                $(container).find('.bcname:last').unwrap();
                            }
                        }

                        var curWindow;
                        window.WebPrint = function (data) {
                            //if(curWindow && !curWindow.closed){
                            //    curWindow.close();
                            //}
                            //localStorage.removeItem('DataToPrint');
                            //localStorage.setItem('DataToPrint', data);
                            //curWindow = window.open(window.location.origin + "/WebPrint", "Print", "width=800,height=500,toolbar=no,scrollbars=no,top=0,left=0");
                            WebToast('Close the print window first', 'top');
                            setTimeout(function () {
                                var PrintWindow = window.open('', 'Print', 'height=' + screen.availHeight + ',width=' + screen.availWidth);
                                PrintWindow.document.write('<html><head><title>Print</title>');
                                PrintWindow.document.write('<style>@page { size: auto; margin: 0mm; padding: 50px; }</style>');
                                PrintWindow.document.write('<link rel="stylesheet" href="/Content/flatui-css-custom.css" type="text/css" />');
                                PrintWindow.document.write('</head><body style="-webkit-print-color-adjust:exact;">');
                                PrintWindow.document.write(data);
                                PrintWindow.document.write('</body></html>');

                                PrintWindow.document.close(); // necessary for IE >= 10
                                PrintWindow.focus(); // necessary for IE >= 10

                                PrintWindow.onload = function () {
                                    setTimeout(function () {
                                        PrintWindow.print();
                                        setTimeout(function () {
                                            PrintWindow.close();
                                        }, 200)
                                    }, 100)
                                }                                
                            }, 500)                            
                        }

                        window.AfterPopupClose = function (v) {
                            closeFunction = v;
                        }

                        window.CopyToClipboard = function (txtBox) {
                            $(txtBox).select();
                            document.execCommand("copy");
                            WebToast('Copied to clipboard');
                        }

                        window.PopupPIMWebForm = function (e) {
                            sessionStorage.IsGlobalPIMWebForm = true;
                            ClosePopUp();
                            if (htmlPIMWebForm == '') {
                                NewMessageBox('Please wait ...', 'Message');
                                AjaxSend({
                                    ExactUrl: '/PIMUpload/PIMWebForm?cache=1&OrgID=' + sessionStorage.OrgID,
                                    type: 'GET',
                                    dataType: 'text',
                                    success: function (vData) {
                                        NewMessageBoxClose();
                                        htmlPIMWebForm = vData;
                                        $('#divGlobalPIMWebForm').append(htmlPIMWebForm).trigger('create').scrollTop(0);
                                        $('#globalpopupPIMWebForm').show();
                                    }
                                });
                            } else {
                                $('#divGlobalPIMWebForm').append(htmlPIMWebForm).trigger('create').scrollTop(0);
                                $('#globalpopupPIMWebForm').show();
                                $('#divGlobalPIMWebForm').scrollTop(0)
                                PimWebForm.GetPackingSizeUnit();
                            }
                        }

                        window.IsPopupOpen = function () {
                            if ($('div[custom-data-role="popup"]:visible, .ui-popup-active').length > 0 ||
                                $('.modal.show').length > 0) {
                                return true;
                            } else {
                                return false;
                            }
                        }

                        function DualCalendar(vParams, m_DateFrom, m_DateTo) {
                            var arrDate = {}, isShowTimeFrom = false, isShowTimeTo = false;

                            ClosePopUp();
                            dualcalendarpopup.remove();
                            dualcalendarpopup.popup();
                            page.append($('<div>').addClass('popupCustomOverlayTheme').css(cssOverLay));

                            $('#divDualCalendarTimeFrom, #divDualCalendarTimeTo').hide();

                            $(".fromCustomDualCalendar").calendar({
                                format: "mmmm dd, yyyy",
                                multiSelect: false,
                                otherDays: true,
                                minDate: (vParams.MinFromDate == undefined ? false : vParams.MinFromDate),
                                maxDate: (vParams.MaxFromDate == undefined ? false : vParams.MaxFromDate),
                                getDates: function (data) {
                                    $("#txtDateFromDualCalendar").text($.format.date(new Date(data), 'MMM dd, yyyy'));
                                    $('#txtErrorMsgDualCalendar').hide();
                                }
                            });

                            $(".toCustomDualCalendar").calendar({
                                format: "mmmm dd, yyyy",
                                multiSelect: false,
                                otherDays: true,
                                minDate: (vParams.MinToDate == undefined ? false : vParams.MinToDate),
                                maxDate: (vParams.MaxToDate == undefined ? false : vParams.MaxToDate),
                                getDates: function (data) {
                                    $("#txtDateToDualCalendar").text($.format.date(new Date(data), 'MMM dd, yyyy'));
                                    $('#txtErrorMsgDualCalendar').hide();
                                }
                            });

                            if(vParams.Title != undefined){
                                $('#txtCustomDualCalendarTitle').text(vParams.Title);
                            } else {
                                $('#txtCustomDualCalendarTitle').text('Date Range');
                            }

                            if (vParams.ShowTime != undefined) {
                                //vParams.ShowTime - accepted value 'ft', 'f', 't'
                                var showtime = vParams.ShowTime;
                                var selDCTimeFromHour = $('#selDCTimeFromHour');
                                var selDCTimeToHour = $('#selDCTimeToHour');
                                var selDCTimeFromMinute = $('#selDCTimeFromMinute');
                                var selDCTimeToMinute = $('#selDCTimeToMinute');
                                var selDCTimeFromMeredian = $('#selDCTimeFromMeredian');
                                var selDCTimeToMeredian = $('#selDCTimeToMeredian');
                                var hour = 1; minute = 0;

                                $('#selDCTimeFromHour, #selDCTimeToHour, #selDCTimeFromMinute, #selDCTimeToMinute').empty();

                                if(showtime.indexOf('f') >= 0){
                                    isShowTimeFrom = true;
                                    $('#divDualCalendarTimeFrom').show();
                                }

                                if (showtime.indexOf('t') >= 0) {
                                    isShowTimeTo = true;
                                    $('#divDualCalendarTimeTo').show();
                                }

                                for (var i = 0; i < 12; i++){
                                    selDCTimeFromHour.append(new Option(hour, hour));
                                    selDCTimeToHour.append(new Option(hour, hour));
                                    hour += 1;
                                }

                                for (var i = 0; i < 4; i++) {
                                    var formatnumber = $.format.number(minute, '00');
                                    selDCTimeFromMinute.append(new Option(formatnumber, formatnumber));
                                    selDCTimeToMinute.append(new Option(formatnumber, formatnumber));
                                    minute += 15;
                                }

                                if (vParams.SetFromTime != undefined) { // must be 12 hr format
                                    var time = (vParams.SetFromTime).split(':');
                                    selDCTimeFromHour.val(time[0]);
                                    selDCTimeFromMinute.val(time[1]);
                                    selDCTimeFromMeredian.val($.trim((time).split(' ')[1]));
                                }

                                if (vParams.SetToTime != undefined) { // must be 12 hr format
                                    var time = (vParams.SetToTime).split(':');
                                    selDCTimeToHour.val(CInt2(time[0]));
                                    selDCTimeToMinute.val(CInt2(time[1]));
                                    selDCTimeToMeredian.val($.trim((vParams.SetToTime).split(' ')[1]));
                                }
                            }

                            $('.fromCustomDualCalendar').calendar('unsetDate');
                            $('.toCustomDualCalendar').calendar('unsetDate');

                            $('.fromCustomDualCalendar').calendar('setDate', $.format.date(m_DateFrom, 'MMM dd, yyyy'));
                            $('.toCustomDualCalendar').calendar('setDate', $.format.date(m_DateTo, 'MMM dd, yyyy'));

                            $('#btnCancelDualCalendar').unbind('click').click(function () {
                                ClosePopUp();
                            })

                            $('#btnDoneDualCalendar').unbind('click').click(function () {
                                var from = $.trim($('#txtDateFromDualCalendar').text());
                                var to = $.trim($('#txtDateToDualCalendar').text());
                                var msg = $('#txtErrorMsgDualCalendar');

                                if (IsDate(from) == false || IsDate(to) == false) {
                                    if (vParams.Required == undefined || vParams.Required == false) {
                                        ClosePopUp();
                                        vParams.Click('');
                                    }
                                    else {
                                        msg.text('Please provide a valid date range').show();
                                    }
                                }
                                else if (IsDate(from) && IsDate(to)) {
                                    var datefrom = new Date(from);
                                    var dateto = new Date(to);

                                    if (datefrom > dateto || dateto < datefrom) {
                                        msg.text('Please provide a valid date range').show();
                                    }
                                    else {
                                        arrDate.Period = "Custom";
                                        arrDate.From = $.format.date(datefrom, (vParams.Format || 'MMM dd, yyyy')) + (isShowTimeFrom ? ' ' + selDCTimeFromHour.val() + ':' + selDCTimeFromMinute.val() + ' ' + selDCTimeFromMeredian.val() : '');
                                        arrDate.To = $.format.date(dateto, (vParams.Format || 'MMM dd, yyyy')) + (isShowTimeTo ? ' ' + selDCTimeToHour.val() + ':' + selDCTimeToMinute.val() + ' ' + selDCTimeToMeredian.val() : '');
                                        ClosePopUp();

                                        if (vParams.SetText != undefined) {
                                            $(vParams.SetText).html(arrDate.From + ' to ' + arrDate.To);
                                        }

                                        vParams.Click(arrDate);
                                    }
                                }
                            })

                            $('#popupCustomDualCalendar').css({ visibility: 'visible', display: 'block' }).popup('open');
                            RepositionPopup();
                        }

                        function CustomChangePage(pageid) {
                            $('div[custom-data-role="page"]').not(pageid).hide();
                            $(pageid).show();
                            $(window).scrollTop(0);
                        }

                        function ClosePopUp() {
                            try {
                                $('div[data-role="popup"]').popup().popup('close');
                                $('.popupCustomOverlayTheme').remove();
                            } catch (e) {
                                // just ignore the jquery error here
                            }
                        }

                        function RepositionPopup() {
                            $('div[data-role="popup"]').popup().popup("reposition", { positionTo: 'window' });
                        }

                        function ResponsiveCustomTab() {
                            if ($(window).width() <= 640) {
                                $('div[custom-tab-role="content"]').wrapInner(function () {
                                    var content = $(this).find('.tab-responsive-content');
                                    if (content.length == 0) {
                                        return $('<div class="tab-responsive-content">')
                                    } else {
                                        content.removeClass('inactive');
                                    }
                                })

                                $('li[custom-tab-content]').each(function () {
                                    var divID = $($(this).attr('custom-tab-content'));
                                    var tabContainer = $(this).parent().parent().attr('id');

                                    if (divID.find('.tab-responsive-header').length == 0) {
                                        divID.prepend($('<div class="tab-responsive-header">')
                                                .attr('tab-container', tabContainer)
                                                .text($(this).text())
                                            )
                                    } else {
                                        divID.find('.tab-responsive-header').show();
                                    }

                                    if ($(this).hasClass('active')) {
                                        divID.find('.tab-responsive-header').addClass('active');
                                        divID.find('.tab-responsive-content').show();
                                    } else {
                                        divID.find('.tab-responsive-header').removeClass('active');
                                        divID.find('.tab-responsive-content').hide();
                                    }

                                    divID.addClass('tab-responsive').show()

                                    if ($(this).hasClass('hidetab')) {
                                        $(divID).hide();
                                        divID.find('.tab-responsive-header').hide();
                                        divID.find('.tab-responsive-content').hide();
                                    }
                                })

                                $('.custom-tab').hide();
                                $('div[custom-tab-role="content"]').show();
                            } else {
                                $('li[custom-tab-content]').each(function () {
                                    var divID = $($(this).attr('custom-tab-content'));
                                    var tabContainer = $(this).parent().parent().attr('id');

                                    if ($(this).hasClass('active') == false) {
                                        divID.hide()
                                    }

                                    if ($(this).hasClass('hidetab')) {
                                        $(this).hide();
                                    }

                                    divID.removeClass('tab-responsive');
                                    divID.attr('tab-container', tabContainer)
                                })

                                $('.custom-tab').show();
                                $('.tab-responsive-header').hide();
                                $('.tab-responsive-content').addClass('inactive').show();
                            }

                            $('.custom-tab').each(function () {
                                if($(this).hasClass('hidetab')){
                                    $(this).hide();
                                }
                            })
                        }

                        mDeferred.resolve();

                        $(document)
                        .keydown(function (e) {
                            if (e.which == 27) {
                                if ($('#popupCustomBrowseLibrary').css('display') == 'block' || $('#popupCustomBrowseLibrary').css('visibility') == 'visible') {
                                    $('.popupCustomOverlayTheme').remove();
                                }

                                if ($('.tooltip-window').is(':visible')) {
                                    $('.tooltip-window').hide();
                                    return;
                                }

                                var arrPopupOpen = $('div[custom-data-role="popup"]:visible, .ui-popup-active');
                                var lastOpenPopupNo = arrPopupOpen.length - 1; // select the last popup opened
                                var lastPopup = $(arrPopupOpen[lastOpenPopupNo]);

                                if (lastPopup.hasClass('ui-popup-container') && closeFunction == '') { // if its a jquery popup
                                    if (CBool($('#' + lastPopup.attr('id').replace('-popup', '')).attr('data-dismissible'))) { // do not close popup if this attr is set to false
                                        $('#' + lastPopup.attr('id').replace('-popup', '')).popup().popup('close');
                                        $('.popupCustomOverlayTheme').remove();
                                    }
                                } else { // then its a custom popup
                                    lastPopup.hide();

                                    if (closeFunction != '') {
                                        closeFunction()
                                        closeFunction = '';
                                    }
                                }

                                //$('div[data-role="popup"]').popup().popup('close');                        
                                //$('div[custom-data-role="popup"]').hide();
                                $('.tooltip-window').hide();
                                sessionStorage.IsGlobalPIMWebForm = false;
                                $('#divGlobalPIMWebForm').empty(); // for adding SKU globally
                            }
                        })
                        .mouseup(function (e) {
                            //close custom dropdown on select tag
                            var container = $(".custom_sel_option");

                            if (container.css('display') == 'block') {
                                if (!container.is(e.target) // if the target of the click isn't the container...
                                && container.has(e.target).length === 0) // ... nor a descendant of the container
                                {
                                    container.hide();
                                    $('[name="' + container.attr('custom-select') + '"]').focus();
                                }
                            }
                        })
                        .on('keydown', function (e) {
                            var custom_sel_option = $('.custom_sel_option');
                            if (custom_sel_option.css('display') == 'block') {
                                var li = custom_sel_option.find('ul > li');
                                var optSelected = li.find('.selected');
                                var curIndex = optSelected.index();
                                li.removeClass('selected');
                                if (e.which == 38) {
                                    // arrow up
                                    li.eq(curIndex - 1).addClass('selected')
                                }
                                else if (e.which == 40) {
                                    if (optSelected.is(':last-child')) {
                                        li.eq(0).addClass('selected')
                                    }
                                    else {
                                        li.eq(curIndex + 1).addClass('selected');
                                    }
                                }
                            }
                                                        
                            if ($('#popupCustomMsgBox').parent().hasClass('ui-popup-active') && $('.msgbox-selected-button').length > 0) {
                                e.stopPropagation();
                                e.preventDefault();
                                var selectedButton = $('.msgbox-selected-button');
                                var countButton = 0;

                                if (e.which == 38) { // up
                                    countButton = selectedButton.prev('button').addClass('msgbox-selected-button').length;
                                } else if (e.which == 40) { // down
                                    countButton = selectedButton.next('button').addClass('msgbox-selected-button').length;
                                }

                                if (countButton > 0) {
                                    selectedButton.removeClass('msgbox-selected-button');
                                }

                                // commented by Arvin
                                // bug: pag ENTER sa search textbox = ma "click" ang previous message box
                                // fixed by Arnie 09-20-2018 4:27PM
                                if (e.which == 13) {
                                    $('.msgbox-selected-button').click();
                                }
                            }
                        })
                        .on("popupafteropen", 'div[data-role="popup"]', function (event, ui) {
                            //if(isUniversalPopup){
                            //    $('.ui-page-active').css('position', 'fixed'); // disable parent's scroll
                            //}
                            
                            // 2018_10_11__10_26_45_2120
                            // this line causes lag in Deposits
                            // fixed by arnie 11-12-2018
                            $('.ui-popup-active').find('div[data-role="popup"]').resize();

                            //var pElements = $('div[data-role="popup"]');
                            //for (var pElementIndex = 0; pElementIndex <= pElements.length - 1; pElementIndex++) {
                            //    var pStartTime = new Date();
                            //    var pElement = $(pElements[pElementIndex]);
                            //    //pElement.resize(); // commented to reduce lag in Deposits
                            //    var pEndTime = new Date();
                            //    var pTimeDiff = pEndTime.getTime() - pStartTime.getTime(); // in milliseconds
                            //    console.log ('pElementIndex: ' + pElementIndex + '; ' +  'ref: jsBHYWTFTK67!');
                            //}

                            if ($('.ui-popup-active').children('div').attr('data-dismissible') == undefined) {
                                $('.ui-popup-active').children('div').attr('data-dismissible', true); // set popup dismissible to true
                            }

                            $('.ui-popup-active').find('div[data-role="content"]').addClass('ui-content ui-body-d');
                            $('.ui-popup-active').css({
                                maxWidth: ($(window).width() - 30) + 'px'
                            })
                            //PopupReposition();
                        })
                        .on("popupafterclose", 'div[data-role="popup"]', function (event, ui) {
                            if (window.location.pathname == '/SalesMenu/FMCGBasic' || window.location.pathname == '/SalesMenu/FMCG') {
                                Popup.popupname = '';
                                FmcgBasicCart.txtQtyPlu.focus();

                                if (Popup.params != undefined) {
                                    Popup.params.success(Popup.returnVal);
                                }
                            }

                            if (closeFunction != '' && typeof closeFunction == 'function') {
                                ClosePopUp();
                                closeFunction()
                                closeFunction = '';
                            }

                            //if (isUniversalPopup) {
                            //    $('.ui-page-active').css('position', 'unset'); // enable parent's scroll
                            //}
                            $('#divForButtonNewMsgBox > button').removeClass('msgbox-selected-button'); //remove this
                            //isUniversalPopup = false;
                        })
                    });
                })
            })
        })
    })
        var _oldShow = $.fn.show;

        $.fn.show = function (speed, oldCallback) {
            return $(this).each(function () {
                var obj = $(this);
                if (obj.hasClass('tooltip-window')) {
                    obj.bind('aftershow', function () {
                        $(this).find('.tooltip-content').css('max-height', ($(window).height() - 140) + 'px')
                            .css('max-width', ($(window).width() - 50) + 'px')
                    })
                    obj.trigger('beforeshow');
                    obj.trigger('aftershow');
                }
                // now use the old function to show the element passing the new callback
                _oldShow.apply(obj);
            });
        }

        $.fn.attachDragger = function (option) {
            var attachment = false, lastPosition, position, difference;
            $($(this).selector).on("mousedown mouseup mousemove", function (e) {
                if (option != undefined) {
                    if (option.onDrag != undefined) {
                        if (option.onDrag() == true) {
                            return;
                        }                        
                    }
                }
                if (e.type == "mousedown") attachment = true, lastPosition = [e.clientX, e.clientY];
                if (e.type == "mouseup") attachment = false;
                if (e.type == "mousemove" && attachment == true) {
                    position = [e.clientX, e.clientY];
                    difference = [(position[0] - lastPosition[0]), (position[1] - lastPosition[1])];
                    $(this).scrollLeft($(this).scrollLeft() - difference[0]);
                    $(this).scrollTop($(this).scrollTop() - difference[1]);
                    lastPosition = [e.clientX, e.clientY];
                }
                $('#' + $(this).attr('id')).addClass('NoHighLight');
            });
            $(window).on("mouseup", function () {
                attachment = false;
            });
        }

        $.fn.pageshow = function () {
            var pages = ($(this).selector).split(',');

            for (var i = 0; i < pages.length; i++) {
                var pageid = $.trim(pages[i]);
                var subpage = $(pageid).attr('subpage');
                var customListMenuID = $(pageid).attr('data-gotopage');

                if (customListMenuID == undefined) {
                    if (subpage == undefined) {
                        $('[custom-data-role="page"]').not(pageid).hide();
                        $(window).scrollTop(0);
                    }
                    else {
                        $('[subpage="' + subpage + '"]').not(pageid).hide();
                    }
                }
                else if (customListMenuID != undefined) {
                    subpage = $('#' + customListMenuID).attr('subpage');
                    $('[subpage="' + subpage + '"]').not(pageid).hide();
                }

                //            if ($(pageid).find('div[subpage]').length > 0) {
                //                $(pageid).find('div[subpage]:eq(0)').show();
                //                $(pageid).find('div[subpage]:not(:eq(0))').fadeOut();
                //            }

                $(pageid).fadeIn();
            }
            
            $('#mainContainer').css('margin-top', $('#divHeaderForProjects').height());
        }

        $.fn.pagehide = function () {
            var pages = $(this).selector;
            var pageArr = pages.split(',');
            
            try {
                for (var i = 0; i < pageArr.length; i++) {
                    var page = $.trim(pageArr[i]);
                    $(page).hide();
                }
            } catch(e){
                console.log(e.message)
            }            
        }

        var curTime;
        $.fn.blink = function (options) {
            var defaults = {
                delay: 500
            };
            var options = $.extend(defaults, options);

            clearInterval(curTime)

            return this.each(function () {
                var obj = $(this);
                
                curTime = setInterval(function () {
                    if ($(obj).css("visibility") == "visible") {
                        $(obj).css('visibility', 'hidden');
                    }
                    else {
                        $(obj).css('visibility', 'visible');
                    }

                    if ($.trim(obj.text()) == '') {
                        clearInterval(curTime);
                        $(obj).css('visibility', 'visible');
                    }
                }, options.delay);
            });
        }

        $.fn.customselect = function () {
            var nAgt = window.navigator.userAgent;
            var OSName = "Unknown";
            if (nAgt.indexOf("Windows") != -1) OSName = "Windows";
            if (nAgt.indexOf("Mac") != -1) OSName = "Mac/iOS";
            if (nAgt.indexOf("X11") != -1) OSName = "UNIX";
            if (nAgt.indexOf("Linux") != -1) OSName = "Linux";
            if (nAgt.indexOf("Android") != -1) OSName = "Android";

            if(OSName != "Unknown"){
                var sel = $($(this).selector);
                var id = (sel.attr('id') || sel.attr('name'));

                /*get the coordinates of the button element using jquery offset*/
                var offset = sel.position();

                /*set the position of the info element*/
                var newOption = $("<div>").addClass('custom_sel_option')
                    .attr('custom-select', id)
                    .css({
                        left: (offset.left) + "px",
                        width: sel.outerWidth() + "px",
                        display: 'none'
                    });

                var arrOption = [];
                sel.find('option').each(function () {
                    arrOption.push({
                        text: $(this).text(),
                        value: $(this).val(),
                        isselect: CBool($(this).is(':selected'))
                    })
                })

                var ul = $('<ul>');
                for (var i = 0; i < arrOption.length; i++) {
                    ul.append($('<li>')
                            .data(arrOption[i])
                            .attr('value', arrOption[i].value)
                            .addClass((arrOption[i].isselect ? 'selected' : ''))
                            .text(arrOption[i].text)
                            .click(function () {
                                sel.find('option[value="' + $(this).data().value + '"]').prop('selected', true);
                                sel.change();
                                $('.custom_sel_option[custom-select="' + (sel.attr('id') || sel.attr('name')) + '"]').hide();
                                $('[name="' + (sel.attr('id') || sel.attr('name')) + '"]').focus();
                            })
                        )
                        .hover(function () {
                            $(this).find('li').removeClass('selected') // remove class selected on every LI
                        })
                }
                
                sel.attr('readonly', true) // hide dropdown list

                if (!sel.hasClass('done') && offset.top != 0 && offset.left != 0) {
                    // append new option after the element
                    sel.after(newOption.append(ul))

                    sel.after($('<div>')
                            .attr('id', 'select-' + id)
                            .css({
                                // position the overlay in the same real estate as the original parent element 
                                position: "absolute"
                                  , top: sel.position().top
                                  , left: sel.position().left
                                  , width: sel.outerWidth()
                                  , height: sel.outerHeight()
                                  , zIndex: 999
                                // IE needs a color in order for the layer to respond to mouse events
                                  , backgroundColor: "#fff"
                                // set the opacity to 0, so the element is transparent
                                  , opacity: 0
                            })
                            .click(function () {
                                $('.custom_sel_option[custom-select="' + $(this).attr('id').split('-')[1] + '"]')
                                    .toggle()
                                    .blur()
                                    .find('ul > li')
                                        .removeClass('selected') // remove class selected on list which is previous selected value
                                        .parent()
                                        .find('li[value="' + sel.val() + '"]')
                                            .addClass('selected')

                            })
                        )
                    sel.addClass('done'); // set .done once custom-option created
                    return true; // true if successfully created custom option
                }                

                return false; // false if not successfully created custom option
            }            
        }

        $.fn.horizontalbar = function (percent) {
            var sel = $(this).selector;

            percent = (percent.toString().match(/%/g) == null ? percent + '%' : percent);

            $(sel).addClass('horizontal-bar')
                    .attr('style', 'background: linear-gradient(90deg, #FFEB3B ' + percent + ', #f3f3f3 0%) !important')
        }

        $.fn.ellipse = function (style) {
            var p = $($(this).selector).find('p');
            var divh = $($(this).selector).height();

            if(style == undefined){
                style = '...'
            }
            while ($(p).outerHeight() > divh) {
                $(p).text(function (index, text) {
                    return text.replace(/\W*\s(\S)*$/, style);
                });
            }

            $($(this).selector).css('height', 'auto');
        }

        $.fn.opentabmenu = function (callback) {
            var tabs = ($(this).selector).split(',');

            for (var i = 0; i < tabs.length; i++) {
                var tabid = $.trim(tabs[i]);

                if (proj.IsBootstrapDesign()){
                    $('[data-target="' + tabid + '"], [href="' + tabid + '"]').tab('show');
                    continue;
                }

                var tab = $('[custom-tab-content="' + tabid + '"]');
                
                if(tab.length > 0){
                    $('#' + tab.parents().parents().attr('id')).find('> ul > li').each(function () {
                        $(this).removeClass('active');
                        $($(this).attr('custom-tab-content')).hide();
                    })

                    tab.addClass('active');
                    if (tab.length > 0) {
                        if (!tab[0].hasAttribute('disabled')) {
                            tab.click()
                        }
                    }
                    $(tabid).show();
                } else {
                    //bootstrap
                    tab = $('a[href="'+ tabid +'"]');
                    tab.parents().closest('ul').find('a').removeClass('active');
                    tab.addClass('active');

                    $(tabid).parent().find('> div').removeClass('active show');
                    $(tabid).addClass('active show');
                }
                
                if($.isFunction(callback)){
                    callback();
                }
            }
        }

        $.fn.customaccordion = function () {
            var tabs = ($(this).selector).split(',');

            for (var i = 0; i < tabs.length; i++) {
                var tabid = $.trim(tabs[i]);
                var tab = $(tabid);

                if (proj.IsBootstrapDesign()) {
                    tab.find('.collapse').removeClass('show');
                } else {
                    tab.find(' > .accordion-header').click();
                }                
            }
        }

        $.fn.hasScrollBar = function () {
            return this.get(0).scrollHeight > this.height();
        }

        $.fn.hidetabmenu = function () {
            var tabs = ($(this).selector).split(',');

            for (var i = 0; i < tabs.length; i++) {
                if (proj.IsBootstrapDesign()) {
                    $('[data-target="' + $.trim(tabs[i]) + '"]').parent().hide();
                } else {
                    $('[custom-tab-content="' + $.trim(tabs[i]) + '"]').addClass('hidetab');
                }                
            }
        }

        $.fn.showtabmenu = function () {
            var tabs = ($(this).selector).split(',');

            for (var i = 0; i < tabs.length; i++) {
                if (proj.IsBootstrapDesign()) {
                    $('[data-target="' + $.trim(tabs[i]) + '"]').parent().show();
                } else {
                    $('[custom-tab-content="' + $.trim(tabs[i]) + '"]').removeClass('hidetab');
                }                
            }
        }

    return mDeferred.promise();

})