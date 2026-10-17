define(['jquery'], function() {

    common = {
        AjaxCalls: []
    };

    var pSkippedAjaxSend = false;

    window.AjaxSend = function(vParams) {
        var pData;
        var pOrigSuccess;
        var pOrigError;
        var pUrl;
        var pOut;
        var pServer;
        var pUuid;
        var pMinimumDurationReached = true;
        var pSuccessReached = false;
        var pSuccessData;

        try {
            if (!CBool(vParams.NoSplash))
            	$.mobile.showPageLoadingMsg();
        } catch (e) {
            // Uncaught TypeError: Cannot read property 'loader' of undefined (Chrome)
            // TypeError: this.loaderWidget is undefined (Firefox)
        };

        if (window.InAndroid 
            && CBool(Android.IsAutoOfflineMode())
            && (vParams.ExactUrl == undefined)) {

            var pResponseText = 'Skipping AjaxSend because IsAutoOfflineMode is true. ref: jsEPF7ZMDJWP!';
            if (pSkippedAjaxSend == false) {
                Android.AddLog(pResponseText);
                pSkippedAjaxSend = true;
            }
            if ($.isFunction(vParams.error))
                vParams.error({
                    responseText: pResponseText
                });
            return;
        }
        vParams = $.extend({
            async: true, // async is already true by default.
            dataType: 'json',
            encodeURIComponent: false // needed to be TRUE by NanoHTTPD. coz percent sign have special meaning to NanoHTTPD 
            // dontHandleSessionExpired: false // false by default, if true, ajax success wont call proj.HandleSessionExpired
        }, vParams);
        if (!window.IsMobile && window.DebugMode)
            vParams.contentType = "application/json; charset=utf-8";

        if (vParams.dataType.toLowerCase() == 'json') {
            pData = {};
            if (localStorage['ASP.NET_SessionId'] != undefined)
                pData['ASP.NET_SessionId'] = localStorage['ASP.NET_SessionId'];
            if (sessionStorage.OrgID != undefined) pData.OrgID = sessionStorage.OrgID;
            if (sessionStorage.FowstID != undefined) pData.FowstID = sessionStorage.FowstID;
            pUuid = localStorage.getItem('UUID');
            if (pUuid != null)
                pData.UUID = pUuid;
            // BEGIN uncomment if you want to send username and password in every ajax
            //if (IsMobile) {
                // pData.IsMobile = 1;
                if ((CStr2(localStorage.Username) != '')
                    && (CStr2(localStorage.HashedPassword) != '')) {

                    pData.Username = localStorage.Username;
                    pData.HashedPassword = localStorage.HashedPassword;
                };
            //}; // END if (IsMobile) {
            // END uncomment if you want to send username and password in every ajax

            pData = $.extend(pData, vParams.data);
        } else
            pData = vParams.data;

        pOrigSuccess = vParams.success;
        pOrigError = vParams.error;

        if (vParams.ExactUrl == undefined) {
            pUrl = vParams.url;
            if (!IsAbsoluteUrl(pUrl)) {
                pServer = init.server || '';
                if (pServer.endsWith('/'))
                    pServer = pServer.substring(0, pServer.length - 1);
                if (pUrl.substring(0, 1) != '/')
                    pUrl = '/' + pUrl; // prepend "/" if not starting with "/"
                pUrl = pServer + pUrl; 
                if (InAndroid)
                    console.log('Ajax URL: ' + pUrl);
            };
        } else
            pUrl = vParams.ExactUrl;

        if (vParams.encodeURIComponent) {
            if (typeof pData != 'string')
                pData = JSON.stringify(pData);
            pData = encodeURIComponent(pData);
            pData = {d: pData};
            pData = JSON.stringify(pData);
        } else if (vParams.data instanceof FormData) {
            for (var pField in pData) {
                if (pData.hasOwnProperty(pField))
                    vParams.data.append(pField, pData[pField]);
            }
            pData = vParams.data;
            vParams.processData = false;
            vParams.contentType = false;
        } else
            pData = JSON.stringify(pData);


        vParams.minimumDuration = CInt2(vParams.minimumDuration);
        if (vParams.minimumDuration > 0) {
            pMinimumDurationReached = false;
            setTimeout(function() {
                if (pSuccessReached)
                    success(pSuccessData);
                else
                    pMinimumDurationReached = true;
            }, vParams.minimumDuration);
        }

        pOut = $.ajax({
            url: pUrl, 
            type: vParams.type || 'POST',
            dataType: vParams.dataType || 'json', // ajax expects to receive data in json format
            data: pData,
            async: vParams.async,
            timeout: vParams.timeout,
            processData: vParams.processData,
            contentType: vParams.contentType,
            success: function(vData) {
                if (pMinimumDurationReached)
                    success(vData);
                else {
                    pSuccessData = vData;
                    pSuccessReached = true;
                }
            },
            error: function(vParams) {
                if (window.InAndroid)
                    Android.SetAutoOfflineMode(true);
                if (pOrigError != null)
                    pOrigError(vParams);
                else if ((vParams != null) && (CStr2(vParams.responseText).length) > 0) {
                    if (!IsMobile)
                        NewMessageBox('A problem occurred while processing your request. Please try again later or contact a Mobiz representative for assistance.', 'Error');
                    else
                        Android.AddLog('ERROR: ' + pUrl)
                } else if ((vParams != null) 
                    && (CStr2(vParams.statusText).length > 0)
                    && (vParams.statusText.toLowerCase() != 'error')) {

                    if (!IsMobile)
                        NewMessageBox('A problem occurred while processing your request. Please try again later or contact a Mobiz representative for assistance.', 'Error');
                    else
                        Android.AddLog('ERROR: ' + pUrl)
                } else {
                    console.error("Can't establish a connection to the server");//Network or server error
                    alert("Can't establish a connection to the server: " + this.url);                    
                };
                try {
                    if (typeof Bs.Loading == 'function') Bs.Loading('hide');

                    if (!CBool(vParams.NoSplash))
                    	$.mobile.hidePageLoadingMsg();
                } catch (e) {
                    // Uncaught TypeError: Cannot read property 'loader' of undefined
                };
            },
            crossDomain : true,
            beforeSend: function(jqXHR, settings ) {
                if (IsSameDomain(settings.url))
                    jqXHR.setRequestHeader("AjaxRequest", "1");
                if (!CBool(vParams.NoSplash)) {
                    common.AjaxCalls.push(
                        $.extend(jqXHR, {
                            Start: new Date()
                        })
                    );
                    if (common.AjaxCallback != undefined)
                        common.AjaxCallback();
                }
            },
            complete: function (jqXHR, textStatus) {
                var p = common.AjaxCalls.indexOf(jqXHR);
                if (p >= 0)
                    common.AjaxCalls.splice(p, 1);
                if (common.AjaxCallback != undefined)
                    common.AjaxCallback();
                if ($.isFunction(vParams.complete)) 
                    vParams.complete();
            }
        });

        function success(vData) {
//                 // debugging tool only                
//                 if (InAndroid) {
//                    var pJson = {
//                        input: this,
//                        output: vData
//                    };
//                    pJson = 'ajax: ' + JSON.stringify(pJson);
//                    console.log(pJson);
//                 };

            if (vData != null) 
                if (vData.d)
                    vData = vData.d;
            if ((window.proj != undefined)
                && (!CBool(vParams.dontHandleSessionExpired)))
                if (proj.HandleSessionExpired(vData) == false) return;
            if (pOrigSuccess != null) {
                try {
                    pOrigSuccess(vData);
                } catch (e) {
                    console.error('Error in AjaxSend.Success: ' + e.message + '\n' + e.stack, e);
                    console.trace(e.stack);
                    if (typeof Bs == 'object') Bs.Loading('hide')
                };
            };
            try {
                if (!CBool(vParams.NoSplash))
                    $.mobile.hidePageLoadingMsg();
            } catch (e) {
                // Uncaught TypeError: Cannot read property 'loader' of undefined
            };
        } // END function success(vData) {

        return pOut;

    }; // AjaxSend

    var mOrigVal = $.fn.val;
    $.fn.val = function(vText) {
        if (CStr2(vText).length > 0) {
            if ((this.attr('type') == 'number')
                || (this.attr('type') == 'numeric')) {

                return mOrigVal.apply(this, [(CStr2(vText) || '').replace(/,/g, '')]);
            } else 
                return mOrigVal.apply(this, arguments);
        } else
            return mOrigVal.apply(this, arguments);
    };

    window.Sort = function (vArray, vParams) {
        
        // if (Array.isArray(vParams))
//        var pParam = {
//            Field: '',
//            CaseSensitive: false,
//            IsDate: false,
//            IsNumeric: false,
//            Reverse: false
//        };
        var pOut;

        if (typeof vParams == 'string')
            vParams = {
                Field: vParams
            };

        pOut = vArray.sort(function(v1, v2) {

            var p1;

            if (vParams.Reserve) {
                p1 = v1;
                v1 = v2;
                v2 = p1;
            };
            v1 = v1[vParams.Field];
            v2 = v2[vParams.Field];

            if (vParams.IsDate)
                return new Date(v1) > new Date(v2);
            else if (vParams.IsInt) 
                return CDec2(v1) > CDec2(v2);
            else if (vParams.CaseSensitive)
                return CStr2(v1).toUpperCase() > CStr2(v2).toUpperCase();
            else
                return CStr2(v1) > CStr2(v2);
                
        });

        return pOut;

    }; // END Sort

    window.CsvToDataTable = function (vCsv) {
        
        var pOut;
        var pSplits;
        var pColumns;

        pOut = {
            Rows: []
        };
        pSplits = vCsv.split('\r\n');
        pSplits.forEach(function(vSplit) {
            var pCells;
            var pRow;

            if (CStr2(vSplit) == '') return;
            if (pOut.Columns == undefined) {
                pColumns = vSplit.split(',');
                pOut.Columns = {};
                pColumns.forEach(function(vCell) {
                    pOut.Columns[vCell] = {
                        Name: vCell
                    };
                });

            } else {
                pCells = vSplit.split(',');
                pRow = {};
                for (var p = 0; p <= pCells.length - 1; p++) 
                    pRow[pColumns[p]] = DecodeCell(pCells[p]);
                pOut.Rows.push(pRow);
            };
        });

        function DecodeCell(vCell) {
            
            var pOut;

            if (vCell == '\\null')
                return null;
            pOut = vCell;
            pOut = pOut.replace(/\\crlf/g, '\n');
            pOut = pOut.replace(/\\comma/g, ',');
            pOut = pOut.replace(/\\\\/g, '\\');

            return pOut;
        };

        return pOut;

    }; // END CsvToDataTable

    window.FormatColumns = function (vArray, vFieldNames, vCallback) {
        
        var pItem;

        if (!$.isArray(vArray))
            vArray = [vArray];

        if (!$.isArray(vFieldNames))
            vFieldNames = [vFieldNames];

        for (var p  = 0; p <= vArray.length - 1; p++) {
            pItem = vArray[p];
            if (pItem == null)
                continue;
            for (var pFieldNameIndex = 0; pFieldNameIndex <= vFieldNames.length - 1; pFieldNameIndex++) {
                pItem[vFieldNames[pFieldNameIndex]] = vCallback(pItem[vFieldNames[pFieldNameIndex]]);
            }
        }

        return vArray;
    };

    window.AccountingFormatColumns = function (vArray, vFieldNames, vDecimals, vDontModifyNonNumbers) {
        
        return FormatColumns(vArray, vFieldNames, function(vValue) {
            if (!vDontModifyNonNumbers)
                vValue = CDec2(vValue);
            return AccountingFormat(vValue, vDecimals);
        });

    } // END AccountingFormatColumns

    if (window.DebugMode) {
        Storage.prototype._setItem = Storage.prototype.setItem;
        Storage.prototype.setItem = function(key, value) { 
         
            // http://stackoverflow.com/a/13613729/334045
            if (key == 'CartOrderID')
                console.log('js3QF3LG7NA7! ' + key + ': ' + value);

            if (this === window.localStorage) {
                 // do what you want if setItem is called on localStorage
            } else {
                 // fallback to default action
            }

            Storage.prototype._setItem.apply(this, arguments);
        }
    }
    // https://stackoverflow.com/a/18639999/334045
    var makeCRCTable = function(){
        var c;
        var crcTable = [];
        for(var n =0; n < 256; n++){
            c = n;
            for(var k =0; k < 8; k++){
                c = ((c&1) ? (0xEDB88320 ^ (c >>> 1)) : (c >>> 1));
            }
            crcTable[n] = c;
        }
        return crcTable;
    }

    window.crc32 = function(str) {
        var crcTable = window.crcTable || (window.crcTable = makeCRCTable());
        var crc = 0 ^ (-1);

        for (var i = 0; i < str.length; i++ ) {
            crc = (crc >>> 8) ^ crcTable[(crc ^ str.charCodeAt(i)) & 0xFF];
        }

        return (crc ^ (-1)) >>> 0;
    };
}); // define

String.prototype.trim = function () {
    // http://blog.stevenlevithan.com/archives/faster-trim-javascript
    return CStr2(this).replace(/^\s\s*/, '').replace(/\s\s*$/, '');

};

if (typeof String.prototype.startsWith != 'function') {
    // http://stackoverflow.com/a/646643/334045
  String.prototype.startsWith = function (str){
    return this.slice(0, str.length) == str;
  };
}

if (typeof String.prototype.endsWith != 'function') {
  String.prototype.endsWith = function (str){
    return this.slice(this.length - str.length)  == str;
  };
}

function IsEmail(vText) {

    var pPattern = /^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$/i;

    return pPattern.test(vText);

};

function IsNumeric(vText) {
    
    vText = CStr2(vText).replace(/,/g, ''); // remove commas

    // http://stackoverflow.com/questions/18082/validate-numbers-in-javascript-isnumeric/1830844#1830844
    // jQuery also has the ff code
    return !isNaN(parseFloat(vText)) && isFinite(vText);
};

function IsInteger(vText) {
    
    var pInt;

    vText = CStr2(vText).replace(/,/g, ''); // remove commas
    pInt = parseInt(vText);
    if (!isNaN(pInt) && isFinite(vText))
        return (pInt == vText);

    return false;

};

function CDec2(vText, vRemoveDecimals) {
// PARAMS:
//  vRemoveDecimals: optional

    vText = CStr2(vText).replace(/,/g, ''); // remove commas
    vText = vText.trim();

    if (IsNumeric(vText)) 
        if (CBool(vRemoveDecimals))
            return parseInt(vText);
        else
            return parseFloat(vText);
    else if (/^\([^()]+\)$/.test(vText)) {
        vText = vText.replace(/\(/, '');
        vText = vText.replace(/\)/, '');
        vText = vText.trim();
        return -CDec2(vText, vRemoveDecimals);
    } else if (vText.toLowerCase() == 'true')
        return 1;
    else
        return 0;

}; // END CDec2

function CInt2(vText) {

    return CDec2(vText, true);

}

function CBool(vText) {

    if (vText == undefined) return false;
    if (CDec2(vText) != 0) return true;
    if (CStr2(vText).trim().toLowerCase() == 'true') return true;

    return false;

};

String.prototype.repeat = function( num ) {
    // http://stackoverflow.com/questions/202605/repeat-string-javascript/202627#202627
    return new Array( num + 1 ).join( this );
}; // END String.prototype.repeat

function CStr2(vText) {

    if (vText == null)
        return '';
    else
        return vText.toString();

};

function ParseDate(vText) {
    var pStr;
    var pOut;

    if (vText == null)
        return null;

    if (vText instanceof Date)
        return vText;

    pStr = vText.trim().replace(' ', 'T').toUpperCase();
    if (!pStr.endsWith('Z'))
        pStr += 'Z';
    pOut = Date.parse(pStr);
    if (pOut.toString() != 'NaN') {
        pOut += new Date().getTimezoneOffset() * 60000;
        return new Date(pOut);
    }

    pOut = Date.parse(vText);
    if (pOut.toString() != 'NaN')
        return new Date(pOut);

};

function FormatDate(vText, vOptions) {
    // do not use for saving to Sqlite; use ReadyDate instead.
    vOptions = $.extend({
        DontDisplay12Am: false,
        FormatNoTime: null
    }, vOptions);
    if (vOptions.FormatNoTime == null) 
        vOptions.FormatNoTime = vOptions.Format;
    var pDate = ParseDate(vText);
    if (pDate == null)
        return null;
    if (vOptions.DontDisplay12Am)
        if (pDate.getTime() == RemoveTime(pDate).getTime())
            return $.format.date(pDate, (vOptions.FormatNoTime == undefined ? 'MM/dd/yyyy' : vOptions.FormatNoTime)); // added by arnie 82217
        else
            return $.format.date(pDate, (vOptions.Format == undefined ? 'MM/dd/yyyy hh:mm a' : vOptions.Format));
    return $.format.date(pDate, (vOptions.Format == undefined ? 'MM/dd/yyyy' : vOptions.Format)); // added by arnie 82217

}

function DateTimeStamp() {
    var pOut = FormatDate(new Date(), {Format: 'yyyy_MM_dd__HH_mm_ss'});
    return pOut;
}
function IsDate(vText) {
    var pStr;
    
    try {
        if (vText == undefined)
            return false;
        if (Date.parse(vText).toString() != 'NaN')
            return true; // vText is of Date type
        pStr = vText.trim().replace(' ', 'T').toUpperCase();
        if (Date.parse(pStr).toString() == 'NaN')
            return false;
        else
            return true;
    } catch (e) {
        return false;
    }
}

function AccountingFormat(vNumber, vDecimals, vZeroValue, vNegativePrefix, vNegativeSuffix, vAutoHideDecimals) {

    var pTail = '';
    
    if (vZeroValue == null)
        vZeroValue = '-';
    if (vNegativePrefix == null)
        vNegativePrefix = '(';
    if (vNegativeSuffix == null)
        vNegativeSuffix = ')';
    if (IsNumeric(vDecimals) == false) vDecimals = 2;
    vNumber = CDec2(vNumber);
    if (CDec2(vDecimals) > 0) 
        pTail = '.' + (vAutoHideDecimals ? '#' : '0').repeat(vDecimals);
    if (vNumber > 0) 
        return $.format.number(vNumber, '#,##0' + pTail);
    else if (vNumber < 0)
        return vNegativePrefix + $.format.number(Math.abs(vNumber), '#,##0' + pTail) + vNegativeSuffix;
    else
        return vZeroValue;

}; // END AccountingFormat

//Added By Francis Adan
//Date : 11/16/2012
//Function : Get the number of days of the given month and year
function getDaysOfMonth(intMonth, intYear) {
    return intMonth == 2 ? (intYear % 4 ? 28 : (intYear % 100 ? 29 : (intYear % 400 ? 28 : 29))) : ((intMonth - 1) % 7 % 2 ? 30 : 31);
};

//Added By Francis Adan
//Date : 11/16/2012
//Function : Set the collection of months on a select object
function setMonthCollection(objSelectMonthID) {

    $('#' + objSelectMonthID).empty();
    $('#' + objSelectMonthID).append(new Option("MMM", ""));
    $('#' + objSelectMonthID).append(new Option("Jan", "1"));
    $('#' + objSelectMonthID).append(new Option("Feb", "2"));
    $('#' + objSelectMonthID).append(new Option("Mar", "3"));
    $('#' + objSelectMonthID).append(new Option("Apr", "4"));
    $('#' + objSelectMonthID).append(new Option("May", "5"));
    $('#' + objSelectMonthID).append(new Option("Jun", "6"));
    $('#' + objSelectMonthID).append(new Option("Jul", "7"));
    $('#' + objSelectMonthID).append(new Option("Aug", "8"));
    $('#' + objSelectMonthID).append(new Option("Sep", "9"));
    $('#' + objSelectMonthID).append(new Option("Oct", "10"));
    $('#' + objSelectMonthID).append(new Option("Nov", "11"));
    $('#' + objSelectMonthID).append(new Option("Dec", "12"));
};

//Added By Francis Adan
//Date : 11/16/2012
//Function : Set the collection of days on a select object
function setDayCollection(objSelectMonthID, objSelectDayID, objSelectYearID, IsRefreshSelectMenu) {
    var intNumberOfDays;
    var intCurDay;
    var intYear;

    intYear = (new Date).getFullYear();
    if ($.trim(objSelectYearID) != ""){
        intYear = $('#' + objSelectYearID).val();
    }

    intCurDay = 0;
    if ($.trim($('#' + objSelectDayID).val()) != "") {
        intCurDay = $('#' + objSelectDayID).val();
    }
    intNumberOfDays = getDaysOfMonth($('#' + objSelectMonthID).val(), intYear);

    $('#' + objSelectDayID).empty();
    $('#' + objSelectDayID).append(new Option("DD", ""));
    for (var intDay = 1; intDay <= intNumberOfDays; intDay++) {
        $('#' + objSelectDayID).append(new Option(intDay, intDay));
    }

    if (intCurDay == 0 || intCurDay > intNumberOfDays) {
        $('#' + objSelectDayID).val('').attr('selected');
    }
    else {
        $('#' + objSelectDayID).val(intCurDay).attr('selected');
    }

    if (IsRefreshSelectMenu == true) {
        try {
            $('#' + objSelectDayID).selectmenu('refresh');
        } catch (e) {
            $('#' + objSelectDayID).selectmenu().selectmenu('refresh');
        }
    }

};

//Added By Francis Adan
//Date : 11/16/2012
//Function : Set the collection of years on a select object
function setYearCollection(objSelectYearID) {
    var intCurrentYear = (new Date).getFullYear();

    $('#' + objSelectYearID).empty();
    $('#' + objSelectYearID).append(new Option("YYYY", ""));
    for (var intYear = intCurrentYear ; intYear >= 1900; intYear--) { // (intCurrentYear + 10) why add 10? updated by arnie 01-28-2019
        $('#' + objSelectYearID).append(new Option(intYear, intYear));
    }

};

//Added By Francis Adan
//Date : 01/15/2013
//Function : Set the collection of Hours on a select object
function setHoursCollection(objSelectHourID) {

    $('#' + objSelectHourID).empty();
    $('#' + objSelectHourID).append(new Option("HR", ""));
    for (var intHour = 1; intHour <= 12; intHour++) {
        $('#' + objSelectHourID).append(new Option(intHour, intHour));
    }
};

//Added By Francis Adan
//Date : 01/15/2013
//Function : Set the collection of Minutes on a select object
function setMinutesCollection(objSelectMinID) {

    $('#' + objSelectMinID).empty();
    $('#' + objSelectMinID).append(new Option("MIN", ""));
    for (var intMin = 0; intMin <= 59; intMin++) {
        $('#' + objSelectMinID).append(new Option(intMin, intMin));
    }
};

//Added By Francis Adan
//Date : 11/24/2012
//Function : Accept only number
function NumberOnly(e) {

    if (e.keyCode == 46 || e.keyCode == 8 || e.keyCode == 9 || e.keyCode == 27 || e.keyCode == 13 || (e.keyCode == 65 && e.ctrlKey === true) || (e.keyCode >= 35 && e.keyCode <= 39)) {
        return false;
    }
    else {

        if (e.shiftKey || (e.keyCode < 48 || e.keyCode > 57) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }

        return true;
    }
}

function AlphaNumeric(vText) {
    // https://stackoverflow.com/a/5214668/334045
    var b = vText.replace(/[^a-z0-9]/gi,'');
    return b;
}

function printUsingGPrinter(strPrinterIPAddress) {
    try {
        window.pPrint.printUsingGPrinter(strPrinterIPAddress);
    } catch (e) {
        console.log('PrintInNewPrinter: ' + e);
    };
};

function NewGPrinter() {
    window.pPrint.newGPrinter();
}

function ReadyTextToGPrint(strText,
                        intFontType,
                        intFontSizeW,
                        intFontSizeH,
                        IsBold,
                        IsDoubleSize,
                        intTextAlign,
                        IsBarcodePrintQR,
                        IsBarcodePrint1D,
                        IsCutPaper) {

    try {
        window.pPrint.setTextToGPrinter(strText, intFontType, intFontSizeW, intFontSizeH, IsBold, IsDoubleSize, intTextAlign, IsBarcodePrintQR, IsBarcodePrint1D, IsCutPaper);
    } catch (e) {
        console.log('PrintInNewPrinter: ' + e);
    };
    
}

function PrintUsingSunmiPrinter(strText,
                        intFontType,
                        intFontSizeW,
                        intFontSizeH,
                        IsBold,
                        IsDoubleSize,
                        intTextAlign,
                        IsBarcodePrintQR,
                        IsBarcodePrint1D,
                        IsCutPaper) {

    try {
        if (strText.includes('_____')) {
            strText = strText.replace("_______\n", "");
            strText = strText.replace("_________________________________", "________________________________");            
        }
        window.pPrint.PrintUsingSunmiPrinter(strText, intFontType, intFontSizeW, intFontSizeH, IsBold, IsDoubleSize, intTextAlign, IsBarcodePrintQR, IsBarcodePrint1D, IsCutPaper);
    } catch (e) {
        console.log('PrintUsingSunmiPrinter: ' + e);
    };
    
}


//Added By Francis Hernandez
//Date : 12/15/2022
//Function : Use to add space in iMIN receipt

function AddPadding(OriginalString, InsertedString) {
    var printercharlength = 40;
    var newPosition = 0;
    var mpadding = '';

    //get starting point
    newPosition = printercharlength - InsertedString.toString().length;

    //get how many spaces should we add to the current string
    newPosition = newPosition - OriginalString.length

    //if price lenght is less, add more space
    if (InsertedString.length == 1) {
        newPosition += 8;
    }
    else if (InsertedString.length == 4) {
        newPosition += 3;
    }
    else if (InsertedString.length == 5) {
        newPosition += 2;
    } else if (InsertedString.length == 6) {
        newPosition += 1;
    }

    for (var i = 0; i <= newPosition; i++) {
        mpadding += ' ';
    }


    //add price to padding
    return mpadding += InsertedString;
}

function IminfeedPaper() {
    try {       
        window.pPrint.IminfeedPaper();
    } catch (e) {
        console.log('PrintUsingSunmiPrinter: ' + e);
    };
}

function IminCutPaper() {
    try {
        window.pPrint.IminCutPaper();
    } catch (e) {
        console.log('PrintUsingSunmiPrinter: ' + e);
    };
}

function IminPrintInColumns(firstString, secondString, thirdString,
                            firstWidthSize, secondWidthSize,  thirdWidthSize,
                            firstTextAlign, secondTextAlign, thirdTextAlign,
                            firstFontSize, secondFontSize, thirdFontSize) {

    //TextAlign 0-left, 1-center, 2,right
    //Ex. window.pPrint.PrintInColumns("Total Refund ", "", "140.00", 6, 1, 4, 0, 0, 2, 26, 26, 26);

    try {
        window.pPrint.PrintInColumns(firstString, secondString, thirdString,
                                     firstWidthSize, secondWidthSize, thirdWidthSize,
                                     firstTextAlign, secondTextAlign, thirdTextAlign,
                                     firstFontSize, secondFontSize, thirdFontSize);
    } catch (e) {
        console.log('PrintUsingSunmiPrinter: ' + e);
    };
}

function PrintUsingIMinPrinter(strText,
                        intFontType,
                        intFontSizeW,
                        intFontSizeH,
                        IsBold,
                        IsDoubleSize,
                        intTextAlign,
                        IsBarcodePrintQR,
                        IsBarcodePrint1D,
                        IsCutPaper) {

    intFontSizeW = 24;

    try {
        if (strText.includes('_____')) {
            strText = strText.replace("___________\n", ""); //remove second column lines
            strText = strText.replace("_____________________________", "_________________________________");
        }        
        strText = $.trim(strText);
        window.pPrint.PrintUsingIMinPrinter(strText, intFontType, intFontSizeW, intFontSizeH, IsBold, IsDoubleSize, intTextAlign, IsBarcodePrintQR, IsBarcodePrint1D, IsCutPaper);
    } catch (e) {
        console.log('PrintUsingIMinPrinter: ' + e);
    };

}

//telpo printing 07/24/2026

function TestTelpoPrinting() {
    try {
        window.pPrint.TestPrint();                
    } catch (e) {
        console.log('PrintUsingTelpoPrinter: ' + e);
    };
}

function InitializeTelpoPrinter() {
    try {
        window.pPrint.InitializeTelpoPrinter();
    } catch (e) {
        console.log('PrintUsingTelpoPrinter: ' + e);
    };
}

function PrintUsingTelpoPrinter(strText,
                        intFontType,
                        intFontSizeW,
                        intFontSizeH,
                        IsBold,
                        IsDoubleSize,
                        intTextAlign,
                        IsBarcodePrintQR,
                        IsBarcodePrint1D,
                        IsCutPaper) {
    try{
        //window.pPrint.TestPrint();        
        if (strText.includes('_____')) {
            strText = strText.replace("___________\n", ""); //remove second column lines
            strText = strText.replace("_____________________________", "________________________________");
        }
        strText = $.trim(strText);

        window.pPrint.PrintUsingTelpoPrinter(strText, intFontType, intFontSizeW, intFontSizeH, IsBold, IsDoubleSize, intTextAlign, IsBarcodePrintQR, IsBarcodePrint1D, IsCutPaper);
    } catch (e) {
        console.log('PrintUsingTelpoPrinter: ' + e);
    };    
}

function TelpofeedPaper() {
    try {
        window.pPrint.TelpofeedPaper();
    } catch (e) {
        console.log('PrintUsingTelpoPrinter: ' + e);
    };
}

function TelpoPrintInColumns(firstString, secondString, thirdString,
                            firstWidthSize, secondWidthSize, thirdWidthSize,
                            firstTextAlign, secondTextAlign, thirdTextAlign,
                            firstFontSize, secondFontSize, thirdFontSize) {

    //TextAlign 0-left, 1-center, 2,right
    //Ex. window.pPrint.PrintInColumns("Total Refund ", "", "140.00", 6, 1, 4, 0, 0, 2, 26, 26, 26);

    if (sessionStorage.PrinterPaperSize == 'Imin57MM') {
        if (secondString == '') {
            secondString = thirdString;
            thirdString = "";

            firstWidthSize = 5;
            secondWidthSize = 4;
            thirdWidthSize = 1;
        }
    }

    try {
        window.pPrint.TelpoPrintInColumns(firstString, secondString, thirdString,
                                     firstWidthSize, secondWidthSize, thirdWidthSize,
                                     firstTextAlign, secondTextAlign, thirdTextAlign,
                                     firstFontSize, secondFontSize, thirdFontSize);
    } catch (e) {
        console.log('PrintUsingTelpoPrinter: ' + e);
    };
}

//KwisPOS printing 07/28/2026

function TestKwikPOSPrinting() {
    try {
        window.pPrint.TestkwikPOSPrinting();
    } catch (e) {
        console.log('PrintUsingTelpoPrinter: ' + e);
    };
}

//Added By Francis H
//Date : 01/23/2015
//Function : Initialize new Camera Image

function NewCameraImage(vFileName) {
    try {
        window.pCapture.getImage(vFileName);
    } catch (e) {
        console.log('NewCameraImage: ' + e);
    };
};

function NewCameraImageManual(vFileName) {
    try {
        window.pCapture.getImageManual(vFileName);
    } catch (e) {
        console.log('getImageManual: ' + e);
    };
};

function NewProfileImage(vFileName) {
    try {
        window.pCapture.getProfileImage(vFileName);
    } catch (e) {
        console.log('NewProfileImage: ' + e);
    };
};

function stopCameraService() {
    try {
        window.pCapture.stopCameraService();
    } catch (e) {
        console.log('stopCameraService: ' + e);
    };
};

function startCameraService() {
    try {
        window.pCapture.startCameraService();
    } catch (e) {
        console.log('startCameraService: ' + e);
    };
};

//Added By Francis Adan
//Date : 03/06/2013
//Function : Use to format text for receipt printing
function FormatTextToPrint(intMaxChar, strText1, intLimit1, strText2, intLimit2, strText3, intLimit3, intTextAlign) {
    if (intLimit1 == null)
        intLimit1 = intMaxChar;
    if (strText2 == null)
        strText2 = '';
    if (intLimit2 == null)
        intLimit2 = 0;
    if (strText3 == null)
        strText3 = '';
    if (intLimit3 == null)
        intLimit3 = 0;
    if (intTextAlign == null)
        intTextAlign = 1;
    var strReturn = '';
    var intStart1 = 0;
    var intText1Index = 0;
    var intStart2 = intLimit1 + 1;
    var intText2Index = 0;
    var intStart3 = intLimit2 + 1;
    var intText3Index = 0;

    strText1 = CStr2(strText1);
    for (var p = (intTextAlign == 0 ? -2 : 0); p <= intMaxChar; p++) {

        //Text1
        if (0 <= p && p <= intLimit1) {
            if (intText1Index <= (strText1.length - 1)) {
                strReturn = strReturn + strText1.substring(intText1Index, intText1Index + 1);
                intText1Index = intText1Index + 1;
            } else {
                if (strText2.length > 0) {
                    strReturn = strReturn + " ";
                }
            }
        }
        else if ((intLimit1 + 1) < p && p <= intLimit2) {
            if (intText2Index <= (strText2.length - 1)) {
                strReturn = strReturn + strText2.substring(intText2Index, intText2Index + 1);
                intText2Index = intText2Index + 1;
            } else {
                if (strText3.length > 0) {
                    strReturn = strReturn + " ";
                }
            }
        }
        else if ((intLimit2 + 1) < p && p <= intLimit3) {
            if (intText3Index <= (strText3.length - 1)) {
                strReturn = strReturn + strText3.substring(intText3Index, intText3Index + 1);
                intText3Index = intText3Index + 1;
            } else {
                strReturn = strReturn + " ";
            }
        } else {
            //strReturn = strReturn + " ";
        }

        //Check If All text has been loaded, if not, do in new line
        if (p == (intMaxChar - 1)) {
            if (intText1Index < (strText1.length - 1) || intText2Index < (strText2.length - 1) || intText3Index < (strText3.length - 1)) {
                p = (intTextAlign == 0 ? -3 : 0);
            }

            strReturn = strReturn + "\n";

        }
    };

    return strReturn;
}

//Added By Francis Adan
//Date : 03/06/2013
//Function : Use to format text of Invoice Number and others for receipt printing
function SetPrefixPostfix(strText, intLenght, strChar, IsFromStart) {
    var strReturn = strText;

    while (strReturn.length < intLenght) {
        if (IsFromStart == 1) {
            strReturn = strReturn + strChar;
        } else {
            strReturn = strChar + strReturn;
        }
    }

    return strReturn;
}

//Added By Francis Adan
//Date : 03/06/2013
//Function : Initialize new set of data to print
function NewPrintBuilder() {
    
    if (sessionStorage.Printer == 'GPrinter') {
        NewGPrinter();
        return;
    }

    if (InWinRt) {
        var vSql = '';
        try {
            var pDT = Js.Execute({
                command: 'Print.NewPrintBuilder',
                Sql: vSql
            });
        } catch (e) {
            console.log('NewPrintBuilder: ' + e);
        };
    } else {
        try {
            window.pPrint.setNewPrintBuilder();

        } catch (e) {
            console.log('NewPrintBuilder: ' + e);
        };
    }
};

//Added By Francis Adan
//Date : 06/02/2014
//Function : clear command buffer
function ClearCommandBuffer() {
    //if (InWinRt) {
    //    var vSql = '';
    //    try {
    //        var pDT = Js.Execute({
    //            command: 'Print.ClearCommandBuffer',
    //            Sql: vSql
    //        });
    //    } catch (e) {
    //        console.log('ClearCommandBuffer: ' + e);
    //    };
    //} else {
    //    try {
    //        window.pPrint.setClearCommandBuffer();

    //    } catch (e) {
    //        console.log('ClearCommandBuffer: ' + e);
    //    };
    //}
};

//Added By Francis Adan
//Date : 03/06/2013
//Function : Use to send text for receipt printing
function ReadyTextToPrint(strText, intFontType, intFontSizeW, intFontSizeH, IsBold, IsDoubleSize, intTextAlign, IsBarcodePrintQR, IsBarcodePrint1D, IsCutPaper) {
    console.log('ReadyTextToPrint: ' + strText);

    if (sessionStorage.Printer == 'GPrinter') {
        if (CBool(IsCutPaper)) {
            ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 0, 0, 0, 0);
            ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 0, 0, 0, 0);
            ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 0, 0, 0, 0);
            ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 0, 0, 0, 0);
        }
        ReadyTextToGPrint(strText, intFontType, intFontSizeW, intFontSizeH, IsBold, IsDoubleSize, intTextAlign, IsBarcodePrintQR, IsBarcodePrint1D, IsCutPaper);
        return;
    } else if (sessionStorage.Printer == 'SunmiPrinter') {
        PrintUsingSunmiPrinter(strText, intFontType, intFontSizeW, intFontSizeH, IsBold, IsDoubleSize, intTextAlign, IsBarcodePrintQR, IsBarcodePrint1D, IsCutPaper);
        return;
    } else if (sessionStorage.Printer == 'IMin') {
        PrintUsingIMinPrinter(strText, intFontType, intFontSizeW, intFontSizeH, IsBold, IsDoubleSize, intTextAlign, IsBarcodePrintQR, IsBarcodePrint1D, IsCutPaper);
        return;
    } else if (sessionStorage.Printer == 'Telpo') {
        PrintUsingTelpoPrinter(strText, intFontType, intFontSizeW, intFontSizeH, IsBold, IsDoubleSize, intTextAlign, IsBarcodePrintQR, IsBarcodePrint1D, IsCutPaper);
        return;
    }

    if (InWinRt) {
        var vSql = '';
        
        try {
            var pDT = Js.Execute({
                command: 'Print.SetTextToPrint',
                Text: strText,
                FontType: intFontType,
                FontSizeW: intFontSizeW,
                FontSizeH: intFontSizeH,
                IsBold: IsBold,
                IsDoubleSize: IsDoubleSize,
                TextAlign: intTextAlign,
                IsBarcodePrintQR: IsBarcodePrintQR,
                IsBarcodePrint1D: IsBarcodePrint1D,
                IsCutPaper: IsCutPaper
            });
        } catch (e) {
            console.log('setTextToPrint: ' + e);
        };
                
    } else {
        try {
            if (GetStringObject('pPrint.setTextToPrint'))
                window.pPrint.setTextToPrint(strText, intFontType, intFontSizeW, intFontSizeH, IsBold, IsDoubleSize, intTextAlign, IsBarcodePrintQR, IsBarcodePrint1D, IsCutPaper);
        } catch (e) {
            console.log('setTextToPrint: ' + e);
        };
    }

    //try {
    //    if (GetStringObject('pPrint.setTextToPrint'))
    //        window.pPrint.setTextToPrint(strText, intFontType, intFontSizeW, intFontSizeH, IsBold, IsDoubleSize, intTextAlign, IsBarcodePrintQR, IsBarcodePrint1D, IsCutPaper);
    //} catch (e) {
    //    console.log('ReadyTextToPrint: ' + e);
    //};
};

//Added By Francis Adan
//Date : 03/06/2013
//Function : start printing 
function PrintText(strPrinterIPAddress) {
    
    if (sessionStorage.Printer == 'GPrinter') {        
        printUsingGPrinter(strPrinterIPAddress);
        return;
    }

    if (InWinRt) {
        var vSql = '';
        try {
            var pDT = Js.Execute({
                command: 'Print.PrintText',
                IP: strPrinterIPAddress
            });
        } catch (e) {
            console.log('PrintText: ' + e);
        };
    } else {
        try {
            window.pPrint.doPrintText(strPrinterIPAddress);

        } catch (e) {
            Android.AddLog('ClearCommandBuffer: ' + e);
        };
    }

    //try {
    //    window.pPrint.doPrintText(strPrinterIPAddress);
    //} catch (e) {
    //    console.log('PrintText: ' + e);
    //};
};

function CheckPrinterStatus(PrinterIP) {

    if (InWinRt) {
        var vSql = '';
        //try {
        //    var pDT = Js.Execute({
        //        command: 'Print.CheckPrinterStatus',
        //        Sql: vSql
        //    });
        //} catch (e) {
        //    console.log('CheckPrinterStatus: ' + e);
        //};
    } else {
        try {
            window.pPrint.checkPrinterStatus(PrinterIP);

        } catch (e) {
            console.log('CheckPrinterStatus: ' + e);
        };
    }
};
//Added By Francis Adan
//Date : 03/18/2013
//Function : Find Desktop Printer
function FindDesktopPrinter(strPrinterName) {

    try {
        var japplet = document.jzebra;

        if (japplet != null) {

            if(strPrinterName == ""){
                japplet.findPrinter();
            }else{
                japplet.findPrinter(strPrinterName);
            }

            if (japplet.getPrinter() != "" && japplet.getPrinter() != null && japplet.getPrinter() != undefined) {
                return true;
            } else {
                return false;
            }

        } else {
            return false;
        };
    } catch (e) {
        console.error(e);
        return true;
    };
};

//Added By Francis Adan
//Date : 03/18/2013
function SetTextAlign(strText, intTextAlign, intLength) {
    var strReturn = '';
    var strTemp = strText;
    var intTextIndex = 0;

    //intTextAlign 0 - Left 1 - center 2 - right

    if(intTextAlign==0){
        
        for (var p = 0; p < intLength; p++) {
            if(intTextIndex < strText.length){
                strReturn = strReturn + strText.substring(intTextIndex, intTextIndex + 1);
                intTextIndex = intTextIndex + 1;
            }

            //Check If All text has been loaded, if not, do in new line
            if (p == (intLength - 1)) {
                if (intTextIndex < (strText.length - 1)) {
                    p = 0;
                    strReturn = strReturn + "\n";
                }
            }
        }

    }else if(intTextAlign==1){
        
        for (var p = 0; p < intLength; p++) {
            if( p < ((intLength - strTemp.length)/2)){
                strReturn = strReturn + ' ';
            }else{
                strReturn = strReturn + strText.substring(intTextIndex, intTextIndex + 1);
                intTextIndex = intTextIndex + 1;
            }

            //Check If All text has been loaded, if not, do in new line
            if (p == (intLength - 1)) {
                if (intTextIndex < (strText.length - 1)) {
                    p = 0;
                    strReturn = strReturn + "\n";
                    strTemp = strText.substring(intTextIndex, (strText.length - 1));
                }

            }

        }

    }else if(intTextAlign==2){
        if(strText.length < intLength){
            for (var p = 0; p < (intLength - strText.length); p++) {
                strReturn = strReturn + ' ';
            }
        }

        strReturn = strReturn + strText;
    }else{
        strReturn = strText;
    }

    
    return strReturn;
}

//Added By Francis Adan
//Date : 03/18/2013
//Function : Use to format text for receipt printing
function FormatTextToDesktopPrint(intMaxChar, strText1, intLimit1, strText2, intLimit2, strText3, intLimit3) {
    var strReturn = '';
    var intStart1 = 0;
    var intText1Index = 0;
    var intStart2 = intLimit1 + 1;
    var intText2Index = 0;
    var intStart3 = intLimit2 + 1;
    var intText3Index = 0;

    for (var p = 0; p <= intMaxChar; p++) {

        //Text1
        if (0 <= p && p <= intLimit1) {
            if (intText1Index <= (strText1.length - 1)) {
                strReturn = strReturn + strText1.substring(intText1Index, intText1Index + 1);
                intText1Index = intText1Index + 1;
            } else {
                if (strText2.length > 0) {
                    strReturn = strReturn + " ";
                }
            }
        }
        else if ((intLimit1 + 1) < p && p <= intLimit2) {
            if (intText2Index <= (strText2.length - 1)) {
                strReturn = strReturn + strText2.substring(intText2Index, intText2Index + 1);
                intText2Index = intText2Index + 1;
            } else {
                if (strText3.length > 0) {
                    strReturn = strReturn + " ";
                }
            }
        }
        else if ((intLimit2 + 1) < p && p <= intLimit3) {
            if (intText3Index <= (strText3.length - 1)) {
                strReturn = strReturn + strText3.substring(intText3Index, intText3Index + 1);
                intText3Index = intText3Index + 1;
            } else {
                strReturn = strReturn + " ";
            }
        } else {
            strReturn = strReturn + " ";
        }

        //Check If All text has been loaded, if not, do in new line
        if (p == (intMaxChar - 1)) {
            if (intText1Index < (strText1.length - 1) || intText2Index < (strText2.length - 1) || intText3Index < (strText3.length - 1)) {
                p = 0;
                strReturn = strReturn + "\n";
            }

        }
    };

    return strReturn;
}

//Added By Francis Hernandez
//Date : 06/13/2016
//Function : Kick drawer that is connected to printer

function doDrawerKick(PrinterIP) {
    if (InWinRt) {
        var vSql = '';
        try {
            var pDT = Js.Execute({
                command: 'Print.doDrawerKick',
                Sql: vSql
            });
        } catch (e) {
            console.log('DrawerKick: ' + e);
        };
    } else {
        try {
            window.pPrint.doDrawerKick(PrinterIP);

        } catch (e) {
            console.log('DrawerKick: ' + e);
        };
    }
}

//Added By Francis Hernandez
//Date : 06/13/2016
//Function : Kick drawer that is connected to printer

function setPrinterConnectionType(ConnectionType, PrinterType, IsPrintByLineItem) {
    if (InWinRt) {
        var vSql = '';
        try {
            var pDT = Js.Execute({
                command: 'Print.setPrinterConnectionType',
                Sql: vSql
            });
        } catch (e) {
            console.log('DrawerKick: ' + e);
        };
    } else {
        try {
            window.pPrint.setPrinterConnectionType(ConnectionType, PrinterType, IsPrintByLineItem);

        } catch (e) {
            console.log('DrawerKick: ' + e);
        };
    }
}

//Added By Francis Adan
//Date : 02/21/2014
//Function : Connect To Bluetooth Printer
function ConnectToMiniThermalPrinter() {
    try {
        window.pMiniThermalPrint.ConnectBluetoothPrinter();
    } catch (e) {
        console.log('Connect To Bluetooth Printer : ' + e);
    };
};

//Added By Francis Adan
//Date : 02/21/2014
//Function : close To Bluetooth Printer
function CloseMiniThermalPrinter() {
    try {
        window.pMiniThermalPrint.CloseBluetoothPrinter();
    } catch (e) {
        console.log('Close Bluetooth Printer : ' + e);
    };
};

//Added By Francis Adan
//Date : 02/21/2014
//Function : Use to send text for receipt printing in Mini thermal Printer
function SendTextToPrintMiniThermalPrinter(strText) {
    console.log('SendTextToPrintMiniThermalPrinter: ' + strText);
    try {
        window.pMiniThermalPrint.setTextToPrint(strText);
    } catch (e) {
        console.log('SendTextToPrintMiniThermalPrinter: ' + e);
    };
};

function HtmlEncode(vText) {
    // http://stackoverflow.com/questions/24816/escaping-html-strings-with-jquery/24870#24870

    var escaped = vText + '';
    var findReplace = [[/&/g, "&amp;"], [/</g, "&lt;"], [/>/g, "&gt;"], [/"/g, "&quot;"]];
    for (x in findReplace)
        escaped = escaped.replace(findReplace[x][0], findReplace[x][1]);
    return escaped;

};

function HtmlDecode(str) {
    var htmlstr = $('<textarea />').html(str).text();
    return htmlstr;
}

function IsUrl(value) {
    
    // http://stackoverflow.com/a/9350644/334045
    return (/^(?:^((https?|ftp):\/\/)?(((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!$&'()*+,;=]|:)*@)?(((\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5]))|((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?)(:\d*)?)(\/((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!$&'()*+,;=]|:|@)+(\/(([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!$&'()*+,;=]|:|@)*)*)?)?(\?((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!$&'()*+,;=]|:|@)|[\uE000-\uF8FF]|\/|\?)*)?(#((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!$&'()*+,;=]|:|@)|\/|\?)*)?$)$/i.test(value));
    
};

// http://stackoverflow.com/a/901144/334045
function GetUrlParameters(vUrl) {
    // PARAMS:
    //  vUrl: [optional]

    vUrl = vUrl || window.location.search.substring(1);
    var urlParams = {};
    var match,
        pl     = /\+/g,  // Regex for replacing addition symbol with a space
        search = /([^&=]+)=?([^&]*)/g,
        decode = function (s) { return decodeURIComponent(s.replace(pl, " ")); },
        query  = vUrl;

    while (match = search.exec(query))
       urlParams[decode(match[1])] = decode(match[2]);

    return urlParams;

}; // GetUrlParameters

function GotoFirstPage() {

    $.mobile.changePage($($('[data-role=page]')[0]));

}; // GotoFirstPage

function ParseUrl(vUrl) {
    // http://stackoverflow.com/a/6644749/334045
    var a = document.createElement('a');
    
    a.href = vUrl;

    // make in work in IE11
    // http://stackoverflow.com/a/13405933/334045
    if (a.protocol == '')
        a.href = a.href;    

    // get the correct pathname in IE
    document.body.appendChild(a); // if you dont do this, IE11 will incorrectly omit the leading slash
    document.body.removeChild(a);

    return a;
}; // function parseUrl

function IsAbsoluteUrl(vUrl) {

    var pA;

    if (CStr2(vUrl).trim().length == 0) return false;

    pA = ParseUrl(vUrl);

    return (vUrl.toLowerCase().startsWith(pA.protocol.toLowerCase()));

};

function IsSameDomain(vUrl1, vUrl2) {

    var pUrl1 = ParseUrl(vUrl1);
    var pUrl2 = ParseUrl(vUrl2);

    pUrl1 = pUrl1.protocol + '//' + pUrl1.host;
    pUrl2 = pUrl2.protocol + '//' + pUrl2.host;

    return pUrl1 == pUrl2;

};

function RandomInteger(vFrom, vTo) {

    // http://stackoverflow.com/a/7228322/334045
    return Math.floor(Math.random()*(vTo - vFrom + 1) + vFrom);

};

// vFields = array of strings
function GetDistinct(vArray, vFields) {
            
    var pOut = [];
    var pItem;

    for (var p = 0; p <= vArray.length - 1; p++) {
        pItem = ExtractFields(vArray[p], vFields);
        if (IndexOf(pOut, pItem, vFields) == -1)
            pOut.push(pItem);
    };

    // return an object with specified fields (vFields) from an object (vObj).
    // vFields = array of strings
    function ExtractFields(vObj, vFields) {

        var pOut = {};
        var pField;

        for (var p = 0; p <= vFields.length - 1; p++) {
            pField = vFields[p];
            if (vObj.hasOwnProperty(pField)) 
                pOut[pField] = vObj[pField];
//             else
//                throw '"' + pField + '" is not a field of the specifed object.';
        }; // for

        return pOut;

    }; // ExtractFields

  

    return pOut; // GetDistinct

}; // GetDistinct

function MergeUrlParameters(vParams, vUrl, vHost) {
    // PARAMS:
    //  vParams: object
    //  vUrl (optional): if null, will use the current URL
    //  vHost (optional)

    var pUrl;
    var pParams, pParams2;
    var pPrefix;
    var pHash;

    if (CStr2(vUrl).trim() == '') {
        pPrefix = location;
        pUrl = window.location.search.substring(1);
        pHash = window.location.hash;
    } else {
        pPrefix = ParseUrl(vUrl);
        pUrl = vUrl;
        pHash = pPrefix.hash;
    }

    if (IsAbsoluteUrl(vUrl))
        pPrefix = pPrefix.protocol + '//' + pPrefix.host + pPrefix.pathname;
    else
        pPrefix = pPrefix.pathname;
    //pPrefix = pPrefix.replace(/\/\/\/\//g, '/'); // kitkat: replace 4-slash with 1-slash
    pPrefix = pPrefix.replace(/\/\/\/\/\/\//g, '/'); // kitkat: replace 6-slash with 1-slash
    pParams = GetUrlParameters(pUrl);
    pParams = $.extend(pParams, vParams);
    pParams2 = {};
    // remove blank entries
    for (var pParam in pParams) {
        if (pParams.hasOwnProperty(pParam)) 
            if (CStr2(pParams[pParam]) != '')
                pParams2[pParam] = pParams[pParam];
    }; // END remove blank entries
    pParams = $.param(pParams2);
    pUrl = pPrefix;
    if (pParams != '')
        pUrl += '?' + pParams;
    pUrl += pHash;

    if (CStr2(vHost).trim() != '') {
        if (vHost.indexOf('http') == -1)
            vHost = 'http://' + vHost;
        if ((vHost.lastIndexOf('/') != vHost.length - 1)
            && (pUrl.indexOf('/') != 0))
            vHost += '/';
        pUrl = vHost + pUrl;
    }

    return pUrl;

}; // END MergeUrlParameters

function ReadyBarcode(vText) {
    
    if (CStr2(vText).trim() == '') 
        return '';

    return /[\s0]*(\S+)/.exec(vText)[1];

}; // END ReadyBarcode

function ReadyGuid(vText) {

    vText = CStr2(vText).trim().toUpperCase();
    if ((vText.startsWith('{'))
        && (vText.endsWith('}')))

        return vText.substring(1,vText.length - 1);
    else
        return vText;

}

function GetStringObject(vString, vParent) {
//  PARAMS:
//      vString:
//      vParent: optional

    var pSplit;
    var pObj;

    if (CStr2(vString).trim() == '')
        return null;

    if (vParent == undefined)
        pObj = window;
    else
        pObj = vParent;

    pSplit = vString.split('.');
    for (var p = 0; p <= pSplit.length - 1; p++) {
        if (pObj == undefined)
            return undefined;
        if (pObj.hasOwnProperty(pSplit[p]))
            pObj = pObj[pSplit[p]];
        else
            return undefined;
    }

    return pObj;

} // END GetStringObject

function CreateStringObject(vString, vParent, vContent) {
//  PARAMS:
//      vString:
//      vParent: optional
//      vContent: optional

    var pSplit;
    var pObj;
    var pName = '';
    var pOldObj;
    var pOldIndex;

    if (CStr2(vString).trim() == '')
        return null;

    if ($.isFunction(vParent)) {
        vContent = vParent;
        vParent = undefined;
        pObj = window;
    } else if (vParent == undefined)
        pObj = window;
    else
        pObj = vParent;

    pSplit = vString.split('.');
    for (var p = 0; p <= pSplit.length - 1; p++) {
        if (!IsObject(pObj)) {
            if (pOldObj == null) {
                pObj = {};
            } else {
                pOldObj[pOldIndex] = {};
                pObj = pOldObj[pOldIndex];
            }
        }

        if (!pObj.hasOwnProperty(pSplit[p])) {
            if ((p == pSplit.length - 1)
                && (vContent != undefined)) {

                if (pOldObj != null)
                    pObj = pOldObj[pOldIndex];
                pObj[pSplit[p]] = vContent;
            } else
                pObj[pSplit[p]] = {};
            if (!pObj.hasOwnProperty(pSplit[p])) {
                console.warn('\'' + pSplit[p] + '\' property was not added to \'' + pName + '\'. CreateStringObject');
                return undefined;
            }
        }
        if (pName != '') 
            pName += '.';
        pName += pSplit[p];

        if ((p == pSplit.length - 1)
            && (vContent != undefined)) {

            pObj[pSplit[p]] = vContent;
        }
        pOldObj = pObj;
        pOldIndex = pSplit[p];
        pObj = pObj[pSplit[p]];
    }

    return pObj;

} // END CreateStringObject

function IsObject(vObj) {
    // https://stackoverflow.com/a/4320789/334045
    if (vObj == null)
        return false;
    var pType = Object.prototype.toString.call( vObj );
    return (pType === '[object Object]')
        || (pType === '[object Window]')
        || (pType === '[object window]')
        || (pType === '[object global]');
}

function GetFilenameWithoutExtension(vPath) {

    var pPattern = /^(?:.*?[\\/])*([^.]+)/;
    var pMatch = pPattern.exec(vPath);

    if (pMatch != undefined)
        return pMatch[1];
    else
        return '';

} // GetFilenameWithoutExtension


window.ToGuid = function(vStr) {

    var pRegEx = /^{?([A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12})}?$/i;
    var pMatch = pRegEx.exec(vStr);
    if (pMatch != null) {
	    return pMatch[1].toLowerCase();
    } else {
	    return '';
    };

}; // window.ToGuid

window.IsGuid = function(vStr) {

    return (ToGuid(vStr).length > 0);

}; // IsGuid

function RaiseError(vMessage) {

    var pErr = new Error(vMessage);
    console.trace(pErr);
    throw pErr;

}

function EndsWith(vFullText, vEndText) {
    // older devices dont have String.endsWith

    vFullText = CStr2(vFullText);
    vEndText = CStr2(vEndText);

    var pIndex = vFullText.lastIndexOf(vEndText);
    var pExpectedIndex = vFullText.length - vEndText.length;
    var pOut = pIndex == pExpectedIndex;

    return pOut;
}

//additional by janine july 3 clock

    var timerID = null;
    var timerRunning = false;
    var displayInput;
    function stopclock() {
        if (timerRunning)
            clearTimeout(timerID);
        timerRunning = false;
    }

    function startclock(input, vPrefix) {
        if (vPrefix != null)
            proj.ClockPrefix = vPrefix;
        displayInput = input;
        stopclock();
        showtime();
    }

    function showtime() {
        // var now = new Date(); // old code
        var now = proj.GetZReadDate();
        var m_names = new Array("Jan", "Feb", "Mar",
        "Apr", "May", "Jun", "Jul", "Aug", "Sep",
        "Oct", "Nov", "Dec");
        var d_names = new Array("Sunday", "Monday", "Tuesday", "Wednesday",
        "Thursday", "Friday", "Saturday");
        var curr_day = now.getDay();
        var curr_date = now.getDate();
        var curr_month = now.getMonth();
        var curr_year = now.getFullYear();

        var hours = now.getHours();
        var minutes = now.getMinutes();
        var seconds = now.getSeconds();
        var timeValue = "" + ((hours > 12) ? hours - 12 : hours);
        timeValue += ((minutes < 10) ? ":0" : ":") + minutes;
        //                    timeValue += ((seconds < 10) ? ":0" : ":") + seconds;
        timeValue += (hours >= 12) ? " PM" : " AM";
        //$(displayInput).html(d_names[curr_day] + ' ' + m_names[curr_month] + ' ' + curr_date + ', ' + curr_year + ' ' + timeValue);
        $(displayInput).html(proj.ClockPrefix + m_names[curr_month] + ' ' + curr_date + ', ' + curr_year + ' ' + timeValue);
        timerID = setTimeout(showtime, 1000);
        timerRunning = true;
    }


 //end of additional

 function PropertiesToArray(vObj, vExcludedProperties) {
    
    var pOut = [];

    if (vObj == null)
        return null;
    if (vExcludedProperties == null)
        vExcludedProperties = [];

    for (var pField in vObj) {
        if (vObj.hasOwnProperty(pField)) {
            if (vExcludedProperties.indexOf(pField) == -1)
                pOut.push(pField);
        }
    } // for

    return pOut;

 } // END PropertiesToArray

 function GroupAndConcat(vArray, vIncludeFields, vConcatFields) {
    var pRawRow, pOutRow;
    var pOut = GetDistinct(vArray, vIncludeFields);

    // create concat array
    for (var pRawIndex = 0; pRawIndex <= vArray.length - 1; pRawIndex++) {
        pRawRow = vArray[pRawIndex];
        for (var pOutIndex = 0; pOutIndex <= pOut.length - 1; pOutIndex++) {
            pOutRow = pOut[pOutIndex];
            if (AreObjectsEqual(pRawRow, pOutRow, vIncludeFields)) {
                AddToConcatArrayField(pOutRow, pRawRow);
            }
        }
    }
    
    // create concat string
    for (var pOutIndex = 0; pOutIndex <= pOut.length - 1; pOutIndex++) {
        pOutRow = pOut[pOutIndex];
        SortAndStringify(pOutRow);
    }

    return pOut;

    function AddToConcatArrayField(vOutRow, vRawRow) {
        var pValue;
        var pObj;

        if (!vOutRow.hasOwnProperty('ConcatArrayField')) {
            vOutRow.ConcatArrayField = [];
            vOutRow.ConcatObjectArrayField = [];
        }

        pObj = {};
        for (var p = 0; p <= vConcatFields.length - 1; p++) {
            pValue = vRawRow[vConcatFields[p]];
            if (vOutRow.ConcatArrayField.indexOf(pValue) == -1)
                vOutRow.ConcatArrayField.push(pValue);
            pObj[vConcatFields[p]] = pValue;
        }
        if (IndexOf(vOutRow.ConcatObjectArrayField, pObj, vConcatFields) == -1)
            vOutRow.ConcatObjectArrayField.push(pObj);

    } // END AddToConcatArrayField
 
    function SortAndStringify(vOutRow) {
        var pValue;

        vOutRow.ConcatArrayField.sort(function(v1, v2) {
            if (String(v1) < String(v2)) 
                return -1;
            if (String(v1) > String(v2))
                return 1;
            return 0;
        });

        for (var p = 0; p <= vOutRow.ConcatArrayField.length - 1; p++) {
            pValue = vOutRow.ConcatArrayField[p];
            if (pValue == null)
                continue;
            pValue = String(pValue);
            if (vOutRow.hasOwnProperty('ConcatField'))
                vOutRow.ConcatField += ';';
            else
                vOutRow.ConcatField = '';
            vOutRow.ConcatField += pValue;
        }
    } // END SortAndStringify
 } // END GroupAndConcat

 // returns true if 2 objects have the same fields
function AreObjectsEqual(vObj1, vObj2, vCompareFields) {
    var pField;
    var pObj;
    var pObjs = [vObj1, vObj2];

    if (vCompareFields == null) {
        vCompareFields = [];
        // for (var pObj of [vObj1, vObj2]) // this line doesnt work in Android webview
        for (var pObjIndex = 0; pObjIndex <= pObjs.length - 1; pObjIndex++) {
            pObj = pObjs[pObjIndex];
            for (pField in pObj)
                if (pObj.hasOwnProperty(pField))
                    if (vCompareFields.indexOf(pField) == -1)
                        vCompareFields.push(pField);
        }
    }

    for (var pFieldIndex = 0; pFieldIndex <= vCompareFields.length - 1; pFieldIndex++) {
        pField = vCompareFields[pFieldIndex];
        if (vCompareFields.indexOf(pField) == -1)
            continue;

        if (vObj1[pField] != vObj2[pField])
            return false;
    };

    return true;

} // AreObjectsEqual

function IndexOf(vArray, vObj, vCompareFields) {
    for (var p = 0; p <= vArray.length - 1; p++) {
        if (AreObjectsEqual(vArray[p], vObj, vCompareFields))
            return p;
    }
    return -1;
}

function SetFlag(vName) {
    if (window.Flags == undefined)
        window.Flags = [];
    if (window.Flags.indexOf(vName) == -1)
        window.Flags.push(vName);
}

function GetFlags(vNames) {
    if (window.Flags == undefined)
        return false;

    if (!$.isArray(vNames)) 
        vNames = [vNames];

    for (var p = 0; p <= vNames.length - 1; p++)
        if (window.Flags.indexOf(vNames[p]) == -1)
            return false;

    return true;
}

function RemoveTime(vDateTime) {
    var pOut;

    if (!IsDate(vDateTime))
        throw 'vDateTime must be a valid date. jsQG4S5MV6SM!';
    pOut = new Date(vDateTime);
    pOut = new Date(pOut.getFullYear(), pOut.getMonth(), pOut.getDate());
    return pOut;
}

function NumberDotOnly(elem, e, multipledot) {
    // Allow: backspace, delete, tab, escape, enter, .,-
    if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190, 109, 189]) !== -1 ||
        // Allow: Ctrl+A,Ctrl+C,Ctrl+V,Ctrl+Z, Command+A
      ((e.keyCode == 65 || e.keyCode == 86 || e.keyCode == 67 || e.keyCode == 90) && (e.ctrlKey === true || e.metaKey === true)) ||
        // Allow: home, end, left, right, down, up
      (e.keyCode >= 35 && e.keyCode <= 40)) {
        // let it happen, don't do anything

        if (elem.val().match(/\./g) != null && (e.keyCode == 190 || e.keyCode == 110) && (multipledot == undefined || multipledot == false)) {
            if (elem.val().match(/\./g).length != 2) {
                e.preventDefault()
            }
        }

        if (e.ctrlKey && e.keyCode == 86) {
            //Remove special chars when pasting
            elem.unbind('input').on('input', function () {
                var regex = ((multipledot == undefined || multipledot == false) ? /[^0-9]/gi : /[^0-9\.]/gi);
                $(this).val($(this).val().replace(regex, '')).unbind('input');
            })
        }

        return true;
    }
    // Ensure that it is a number and stop the keypress
    if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
        e.preventDefault();
    }

    return false;
}

function GetLatestGPSLocation(vLink) {
    return Transfers.ReadFile("LastKnownLocation.flg");
}
function SendTransactionAsSMS(vData, TransType, mGateWayMobileNo) {
    var strTemp = '';
    var strCount;
    var pSql;
    var charTo = 0;
    var DeviceNo;
    var messageCount = 0;

    pSql = "Select DeviceNo from private.LibraryDevice where UUID = " + ReadyString(Device.Serial());

    DeviceNo = Sql.ExecuteScalar(pSql);

    strTemp = JSON.stringify(vData);
    strCount = strTemp.length;

    var randomNumber = RandomInteger(0, 9).toString() + RandomInteger(0, 9).toString() + RandomInteger(0, 9).toString()

    var ID = DeviceNo + '|' + randomNumber + '|' + Math.ceil((strCount / 120) + 1).toString() + '|' + TransType;

    //SMS header
    var mMessage;

    mMessage = sessionStorage.OrgID + ';' + Web.DeviceID() + ';' + Web.DeviceSerial();

    pSql = 'Insert into TransSmSMessages(ID, SendTo, SendFrom, Message, SentToModemTries) Values (' +
                ReadyString(NewID()) + ',' +
                ReadyString(mGateWayMobileNo) + ',' +
                ReadyString(Web.PeopleUserID()) + ',' +
                ReadyString(ID + '|' + messageCount + '|' + mMessage) + ',' +
                0 + ')'

    Sql.Execute(pSql);

    messageCount++;
    //end of SMS 

    for (i = 0; i < strCount; i = charTo) {

        charTo += 120;

        if (charTo > strCount) {
            charTo = strCount;
        }

        pSql = 'Insert into TransSmSMessages(ID, SendTo, SendFrom, Message, SentToModemTries) Values (' +
                ReadyString(NewID()) + ',' +
                ReadyString(mGateWayMobileNo) + ',' +
                ReadyString(Web.PeopleUserID()) + ',' +
                ReadyString(ID + '|' + messageCount + '|' + strTemp.substring(i, charTo)) + ',' +
                0 + ')'

        Sql.Execute(pSql);

        messageCount++;

    }
}
window.setRegionCollection =  function(selector){
    var sel = $(selector)
        .append('<option value="">Please Select Region</option>')
        .append('<optgroup label="Region I - Ilocos Region">')
            .append('<option value="Region I- Ilocos Norte">Region I- Ilocos Norte</option>')
            .append('<option value="Region I- Ilocos Sur">Region I- Ilocos Sur</option>')
            .append('<option value="Region I- La Union">Region I- La Union</option>')
            .append('<option value="Region I- Pangasinan">Region I- Pangasinan</option>')
        .append('</optgroup>')
        .append('<optgroup label="Region II - Cagayan Valley">')
            .append('<option value="Region II- Batanes">Region II- Batanes</option>')
            .append('<option value="Region II- Cagayan">Region II- Cagayan</option>')
            .append('<option value="Region II- Isabela">Region II- Isabela</option>')
            .append('<option value="Region II- Nueva Viscaya">Region II- Nueva Viscaya</option>')
            .append('<option value="Region II- Quirino">Region II- Quirino</option>')
        .append('</optgroup>')
        .append('<optgroup label="Region III - Central Luzon">')
            .append('<option value="Region III- Aurora">Region III- Aurora</option>')
            .append('<option value="Region III- Bataan">Region III- Bataan</option>')
            .append('<option value="Region III- Bulacan">Region III- Bulacan</option>')
            .append('<option value="Region III- Nueva Ecija">Region III- Nueva Ecija</option>')
            .append('<option value="Region III- Pampanga">Region III- Pampanga</option>')
            .append('<option value="Region III- Tarlac">Region III- Tarlac</option>')
            .append('<option value="Region III- Zambales">Region III- Zambales</option>')
        .append('</optgroup>')
        .append('<optgroup label="Region IV(A) - CALABARZON">')
            .append('<option value="Region IV(A)- Batangas">Region IV(A)- Batangas</option>')
            .append('<option value="Region IV(A)- Cavite">Region IV(A)- Cavite</option>')
            .append('<option value="Region IV(A)- Laguna">Region IV(A)- Laguna</option>')
            .append('<option value="Region IV(A)- Quezon">Region IV(A)- Quezon</option>')
            .append('<option value="Region IV(A)- Rizal">Region IV(A)- Rizal</option>')
        .append('</optgroup>')
        .append('<optgroup label="Region IV(B) - MIMAROPA">')
            .append('<option value="Region IV(B)- Occidental Mindoro">Region IV(B)- Occidental Mindoro</option>')
            .append('<option value="Region IV(B)- Oriental Mindoro">Region IV(B)- Oriental Mindoro</option>')
            .append('<option value="Region IV(B)- Palawan">Region IV(B)- Palawan</option>')
            .append('<option value="Region IV(B)- Romblon">Region IV(B)- Romblon</option>')
        .append('</optgroup>')
        .append('<optgroup label="Region V - Bicol Region">')
            .append('<option value="Region V- Albay">Region V- Albay</option>')
            .append('<option value="Region V- Catanduanes">Region V- Catanduanes</option>')
            .append('<option value="Region V- Camarines Norte">Region V- Camarines Norte</option>')
            .append('<option value="Region V- Camarines Sur">Region V- Camarines Sur</option>')
            .append('<option value="Region V- Masbate">Region V- Masbate</option>')
            .append('<option value="Region V- Sorsogon">Region V- Sorsogon</option>')
        .append('</optgroup>')
        .append('<optgroup label="Region VI - Western Visayas">')
            .append('<option value="Region VI- Aklan">Region VI- Aklan</option>')
            .append('<option value="Region VI- Antique">Region VI- Antique</option>')
            .append('<option value="Region VI- Capiz">Region VI- Capiz</option>')
            .append('<option value="Region VI- Guimeras">Region VI- Guimeras</option>')
            .append('<option value="Region VI- Ilo-ilo">Region VI- Ilo-ilo</option>')
            .append('<option value="Region VI- Negros Occidental">Region VI- Negros Occidental</option>')
        .append('</optgroup>')
        .append('<optgroup label="Region VII - Central Visayas">')
            .append('<option value="Region VII- Bohol">Region VII- Bohol</option>')
            .append('<option value="Region VII- Cebu">Region VII- Cebu</option>')
            .append('<option value="Region VII- Negros Oriental">Region VII- Negros Oriental</option>')
            .append('<option value="Region VII- Siquijor">Region VII- Siquijor</option>')
        .append('</optgroup>')
        .append('<optgroup label="Region VIII - Eastern Visayas">')
            .append('<option value="Region VIII- Biliran">Region VIII- Biliran</option>')
            .append('<option value="Region VIII- Eastern Samar">Region VIII- Eastern Samar</option>')
            .append('<option value="Region VIII- Leyte">Region VIII- Leyte</option>')
            .append('<option value="Region VIII- Northern Samar">Region VIII- Northern Samar</option>')
            .append('<option value="Region VIII- Samar">Region VIII- Samar</option>')
            .append('<option value="Region VIII- Southern Leytre">Region VIII- Southern Leyte</option>')
        .append('</optgroup>')
        .append('<optgroup label="Region IX - Zamboanga Peninsula">')
            .append('<option value="Region IX- Zamboanga del Norte">Region IX- Zamboanga del Norte</option>')
            .append('<option value="Region IX- Zamboanga del Sur">Region IX- Zamboanga del Sur</option>')
            .append('<option value="Region IX- Zamboanga Sibugay">Region IX- Zamboanga Sibugay</option>')
        .append('</optgroup>')
        .append('<optgroup label="Region X - Northern Mindanao">')
            .append('<option value="Region X- Bukidnon">Region X- Bukidnon</option>')
            .append('<option value="Region X- Camiguin">Region X- Camiguin</option>')
            .append('<option value="Region X- Lanao del Norte">Region X- Lanao del Norte</option>')
            .append('<option value="Region X- Misamis Occidental">Region X- Misamis Occidental</option>')
            .append('<option value="Region X- Misamis Oriental">Region X- Misamis Oriental</option>')
        .append('</optgroup>')
        .append('<optgroup label="Region XI - Davao Region">')
            .append('<option value="Region XI- Compostella Valley">Region XI- Compostella Valley</option>')
            .append('<option value="Region XI- Davao del Norte">Region XI- Davao del Norte</option>')
            .append('<option value="Region XI- Davao del Sur">Region XI- Davao del Sur</option>')
            .append('<option value="Region XI- Davao Oriental">Region XI- Davao Oriental</option>')
        .append('</optgroup>')
        .append('<optgroup label="Region XII - SOCCSKSARGEN">')
            .append('<option value="Region XII- Cotabato">Region XII- Cotabato</option>')
            .append('<option value="Region XII- Sarangani">Region XII- Sarangani</option>')
            .append('<option value="Region XII- South Cotabato">Region XII- South Cotabato</option>')
            .append('<option value="Region XII- Sultan Kudarat">Region XII- Sultan Kudarat</option>')
        .append('</optgroup>')
        .append('<optgroup label="Region XIII - CARAGA">')
            .append('<option value="Region XIII- Agusan del Norte">Region XIII- Agusan del Norte</option>')
            .append('<option value="Region XIII- Agusan del Sur">Region XIII- Agusan del Sur</option>')
            .append('<option value="Region XIII- Surigao del Norte">Region XIII- Surigao del Norte</option>')
            .append('<option value="Region XIII- Surigao del Sur">Region XIII- Surigao del Sur</option>')
        .append('</optgroup>')
        .append('<optgroup label="ARMM">')
            .append('<option value="ARMM- Basilan">ARMM- Basilan</option>')
            .append('<option value="ARMM- Lanao del Sur">ARMM- Lanao del Sur</option>')
            .append('<option value="ARMM- Maguindanao">ARMM- Maguindanao</option>')
            .append('<option value="ARMM- Sulu">ARMM- Sulu</option>')
            .append('<option value="ARMM- Tawi-tawi">ARMM- Tawi-tawi</option>')
        .append('</optgroup>')
        .append('<optgroup label="CAR">')
            .append('<option value="CAR- Abra">CAR- Abra</option>')
            .append('<option value="CAR- Apayao">CAR- Apayao</option>')
            .append('<option value="CAR- Benguet">CAR- Benguet</option>')
            .append('<option value="CAR- Ifugao">CAR- Ifugao</option>')
            .append('<option value="CAR- Kalinga">CAR- Kalinga</option>')
            .append('<option value="CAR- Mountain Province">CAR- Mountain Province</option>')
        .append('</optgroup>')
        .append('<optgroup label="NCR">')
            .append('<option value="NCR- Caloocan">NCR- Caloocan</option>')
            .append('<option value="NCR- Las Piñas">NCR- Las Piñas</option>')
            .append('<option value="NCR- Makati">NCR- Makati</option>')
            .append('<option value="NCR- Malabon">NCR- Malabon</option>')
            .append('<option value="NCR- Mandaluyong">NCR- Mandaluyong</option>')
            .append('<option value="NCR- Manila">NCR- Manila</option>')
            .append('<option value="NCR- Marikina">NCR- Marikina"</option>')
            .append('<option value="NCR- Muntinlupa">NCR- Muntinlupa</option>')
            .append('<option value="NCR- Navotas">NCR- Navotas</option>')
            .append('<option value="NCR- Parañaque">NCR- Parañaque</option>')
            .append('<option value="NCR- Pasay">NCR- Pasay</option>')
            .append('<option value="NCR- Pasig">NCR- Pasig</option>')
            .append('<option value="NCR- Pateros">NCR- Pateros</option>')
            .append('<option value="NCR- Quezon City">NCR- Quezon City</option>')
            .append('<option value="NCR- San Juan">NCR- San Juan</option>')
            .append('<option value="NCR- Taguig">NCR- Taguig</option>')
            .append('<option value="NCR- Valenzuela">NCR- Valenzuela</option>')
        .append('</optgroup>')
    
    try {
        sel.selectmenu('refresh');
    } catch(e) {
        //do nothing
    }
};

window.GuidToBase64 = function (str) {

    str = str.replace(/-/g, "");

    return btoa(String.fromCharCode.apply(null,
          str.replace(/\r|\n/g, "").replace(/([\da-fA-F]{2}) ?/g, "0x$1 ").replace(/ +$/, "").split(" "))
        ).replace('==', '');
}

window.Base64ToGuid = function (str) {
    var bin;

    str += '==';

    for (var i = 0, bin = atob(str.replace(/[ \r\n]+$/, "")), hex = []; i < bin.length; ++i) {
        var tmp = bin.charCodeAt(i).toString(16);
        if (tmp.length === 1) tmp = "0" + tmp;
        hex[hex.length] = tmp;
    }

    var decoded = hex.join("");

    var chunks = [];
    chunks.push(decoded.substring(0, 8));
    chunks.push(decoded.substring(8, 12));
    chunks.push(decoded.substring(12, 16));
    chunks.push(decoded.substring(16, 20));
    chunks.push(decoded.substring(20));
    decoded = chunks.join("-");

    return decoded;
}

window.IsTodayOrEarlier = function(vText) {

    return CStr2(vText).trim().toLocaleLowerCase() == 'today or earlier';

};

window.FormatSeconds = function(vSeconds) {
    var pValue;

    pValue = vSeconds / 60 / 60 / 24;
    pValue = Math.floor(pValue);
    if (1 <= pValue)
        return pValue + ' day' + (pValue > 1 ? 's' : '');

    pValue = vSeconds / 60 / 60;
    pValue = Math.floor(pValue);
    if (1 <= pValue)
        return pValue + ' hour' + (pValue > 1 ? 's' : '');

    pValue = vSeconds / 60;
    pValue = Math.floor(pValue);
    if (1 <= pValue)
        return pValue + ' minute' + (pValue > 1 ? 's' : '');

    pValue = Math.floor(pValue);
    return pValue + ' second' + (pValue > 1 ? 's' : '');

};

window.IsInVendorPortal = function() {
    return (/^\/?VendorPortal(?:\/|\/.*)*$/im.test(location.pathname));
}

function CJsonDate(date) {
    if (isNaN(new Date(date))) {
        date = new Date(parseInt(date.substr(6)));
    } else {
        date = new Date(date);
    }

    return date;
}

function GetSecurityCurrentUserAcccess(vParams) {
    vParams = $.extend({
        ModuleName: 'string',
        Callback: function(vAllowed) {
        }
    }, vParams);
    var pData = {
        OrgID: sessionStorage["OrgID"],
        UserID: sessionStorage.PeopleID,
        ModuleName: vParams.ModuleName
    };
    AjaxSend({
        url: "SecurityRole/CheckSecurityCurrentUserAcccess",
        data: pData,
        success: function (vData) {
            if (vData.SecurityRoleData.Rows.length > 0) 
                vParams.Callback(true);
            else 
                vParams.Callback(false);
        }
    });
}

function RandomString(vLength, vCharacters) {
    var pOut = '';

    if (vCharacters == undefined)
        vCharacters = 'abcdefghijklmnopqrstuvwxyz1234567890';

    for (var p = 0; p <= vLength - 1; p++)
        pOut += vCharacters.substr(RandomInteger(0, vCharacters.length - 1), 1);

    return pOut;

}

function DateString(vDate) {
    if (IsDate(vDate))
        return $.format.date(new Date(vDate), 'yyyy-MM-dd HH:mm:ss.SSS');
}

function CallPage (container, htmlCarrier, url, exe) {
    //exe is optional
    if (exe != undefined) {
        if (!$.isFunction(exe)) {
            NewMessageBox('4th parameter must be a function', 'Message');
            return;
        }
    }

    if (CStr2(htmlCarrier) == '') {
        if (CStr2(container) != '') {
            $(container).html('<div class="text-center"><div class="spinner-border text-primary" role="status"></div></div>');

            if (!$(container).hasClass('tab-pane')) $(container).pageshow();
        } else {
            if (proj.IsBootstrapDesign()) {
                Bs.Loading('show');
            } else {
                NewMessageBox('Please wait ...', 'Message');
            }            
        }

        AjaxSend({
            ExactUrl: url + '?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&proj=eas&pid=' + sessionStorage.PeopleID + '&v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                if (proj.IsBootstrapDesign()) {
                    Bs.Loading('hide');
                }
                
                htmlCarrier = $('<div>').append(vData);
                if (CStr2(container) != '') {
                    var con = $(container).html('').append(htmlCarrier);
                    if (!proj.IsBootstrapDesign()) con.trigger('create');
                } else {
                    NewMessageBoxClose();
                }

                if (exe != undefined) {
                    exe(vData);
                }
            }
        });
    } else {
        if ($(container).find('> *').length == 0) {
            $(container).append(htmlCarrier);            
        }

        $(container).pageshow(); //.html('').append(htmlCarrier).trigger('create');

        if (exe != undefined) {
            exe(htmlCarrier);
        }
    }
}

function CopyTransToPlainText(transtype, vData, returnasstring) {
    returnasstring = returnasstring == undefined ? false : CBool(returnasstring);

    // FOR Quote, SO, SI, PO modules in EAS
    function FilterByProductID(vProductID, vData, vVendorID) {
        for (var p = 0; p <= vData.Stocks.Rows.length - 1; p++) {
            var pDR = vData.Stocks.Rows[p];
            if ((pDR.ProductID == vProductID)
                && (ReadyGuid(pDR.VendorID) == ReadyGuid(vVendorID)))
                return pDR;
        }
    }

    function NoteForDraft(isdraft) {
        var transtitle = '';

        if (transtype == 'PurchaseOrder') {
            transtitle = 'Purchase Order';
        } else if (transtype == 'SalesOrder') {
            transtitle = 'Sales Order';
        } else if (transtype == 'Invoice') {
            transtitle = 'Invoice';
        } else if (transtype == 'Quote') {
            transtitle = 'Quote';
        }

        if (!CBool(isdraft)) {
            return 'Note: This is a draft transaction for initial review. Prices and details are subject to change without prior notice. Prices and details will be firmed up after confirmation by the vendor and customer thru a finalized ' + transtitle + '. Thank you. \n\n';
        } else {
            return '';
        }        
    }

    var txtarea = $('<textarea id="txtCopyTransToPlainText" style="margin-left:-1000px;">');

    if ($('body').find('#txtCopyTransToPlainText').length == 0) {
        $('body').append(txtarea);
    } else {
        txtarea = $('#txtCopyTransToPlainText');
    }

    var headerdata = vData.Master, skus = vData.Units.Rows, msg = '', headerformat, backorder = vData.BackOrder;
    var invoiceID = headerdata.ID;

    if ((transtype == 'PurchaseOrder')
        || (transtype == 'AccountsPayable')) {

        if (!returnasstring) {
            if (transtype == 'AccountsPayable') {
                msg = 'ACCOUNTS PAYABLE ';
            } else if (CBool(headerdata.SavedAsFinal)) {
                msg = 'PURCHASE ORDER ';
            } else {
                msg = 'RFQ (Request for Quotation) ';
            }
            msg += 'from ' + sessionStorage.OrgName + '\n\n';
            msg += NoteForDraft(headerdata.SavedAsFinal);
            msg += '*** \n\n';
        }
        
        msg += headerdata.CustomerName + '\n';

        if (CBool(headerdata.SavedAsFinal)
            || (transtype == 'AccountsPayable')) {

            msg += 'PO No.: ';

            var pStr;
            if (transtype == 'AccountsPayable') {
                pStr = headerdata.AccountsPayableFullPoNum;
            } else {
                pStr = headerdata.MIN;
                if (CStr2(pStr).trim() != '')
                    pStr += '-';
                pStr += CInt2(headerdata.PurchaseOrderNo);
            }

            msg += pStr + '\n';

            if (CStr2((headerdata.PurchaseOrderDate)) != '')
                msg += 'PO Date: ' + $.format.date(new Date(headerdata.PurchaseOrderDate), 'MM-dd-yy') + '\n';
        }
    } else if (transtype == 'SalesOrder') {
        if (!returnasstring) {
            if (CBool(headerdata.SavedAsFinal)) {
                msg = 'SALES ORDER '
            } else {
                msg = 'PLEASE REVIEW '
            }
            msg += 'from ' + sessionStorage.OrgName + '\n\n';
            msg += NoteForDraft(headerdata.SavedAsFinal);
            msg += '*** \n\n';
        }
        
        msg += headerdata.CustomerName + '\n';

        if (CBool(headerdata.SavedAsFinal)) {
            msg += 'SO No.: ';

            var pStr = CStr2(headerdata.MIN).trim();
            if (pStr != '')
                pStr += '-';
            pStr += CInt2(headerdata.SalesOrderNo);

            msg += pStr + '\n';

            if (CStr2((headerdata.SalesOrderDate)) != '')
                msg += 'SO Date: ' + $.format.date(new Date(headerdata.SalesOrderDate), 'MM-dd-yy') + '\n';
        }        
    } else if (transtype == 'Invoice') {
        if (!returnasstring) {
            if (CBool(headerdata.SavedAsFinal)) {
                msg = 'INVOICE '
            } else {
                msg = 'PLEASE REVIEW '
            }
            msg += 'from ' + sessionStorage.OrgName + '\n\n';
            msg += NoteForDraft(headerdata.SavedAsFinal);
            msg += '*** \n\n';
        }
        
        msg += headerdata.CustomerName + '\n';

        if (CBool(headerdata.SavedAsFinal)) {
            msg += 'Invoice No.: ';

            var pStr = '';
            if (CStr2(headerdata.FowstWideInvoiceNumber).trim() != '')
                pStr = headerdata.FOWSTNo + '-' + headerdata.FowstWideInvoiceNumber;
            else if (CStr2(headerdata.MIN).trim() != '')
                pStr = headerdata.MIN + '-' + headerdata.InvoiceNumber;
            else
                pStr = headerdata.InvoiceNumber;

            var pDocName = headerdata.DocumentName;
            if (CStr2(pDocName).trim() == '')
                pDocName = 'SI';

            pStr = pDocName + '-' + pStr;

            msg += pStr + '\n';

            if (CStr2((headerdata.InvoiceDate)) != '')
                msg += 'Invoice Date: ' + $.format.date(new Date(headerdata.InvoiceDate), 'MM-dd-yy') + '\n';
        }
    } else if (transtype == 'Quote') {
        if (!returnasstring) {
            if (CBool(headerdata.SavedAsFinal)) {
                msg = 'QUOTE '
            } else {
                msg = 'PLEASE REVIEW '
            }
            msg += 'from ' + sessionStorage.OrgName + '\n\n';
            msg += NoteForDraft(headerdata.SavedAsFinal);
            msg += '*** \n\n';
        }
        
        msg += headerdata.CustomerName + '\n';

        if (CBool(headerdata.SavedAsFinal)) {
            var pStr = CStr2(headerdata.MIN).trim();
            if (pStr != '')
                pStr += '-';
            msg += 'Quote No.: ' + pStr + CInt2(headerdata.QuotationNo) + ' \n';

            if (CStr2((headerdata.QuotationDate)) != '')
                msg += 'Quote Date: ' + $.format.date(new Date(headerdata.QuotationDate), 'MM-dd-yy') + '\n';
        }
    }

    if (CStr2((headerdata.DateNeeded)) != '')
        msg += 'Date Needed: ' + $.format.date(new Date(headerdata.DateNeeded), 'MM-dd-yy') + '\n';

    msg += '\n*** \n\n'

    msg += 'ITEMS: \n\n';

    for (var i = 0; i < skus.length; i++) {
        var pDR = skus[i];
        var pDrProduct = FilterByProductID(pDR.ProductID, vData, pDR.VendorID);
        msg += (i + 1) + '.\n';
        msg += 'PN: ' + pDrProduct.ProductNo + '\n';
        msg += 'Brand: ' + pDrProduct.Brand + '\n';
        msg += 'Desc: ' + pDrProduct.ProductNameOnly + '\n';
        msg += '= ' + (transtype == 'Invoice' ? CDec2(pDR.Quantity) : CDec2(pDR.OrderedQuantity)) + ' ' + pDR.UnitMeasure + ' @ ' + AccountingFormat(pDR.UnitPrice, 4) + '\n';
        msg += '= Line Total: ' + AccountingFormat(transtype == 'Invoice' ? CDec2(pDR.ServedAmount) : CDec2(pDR.OrderedAmount), 4) + '\n\n';
    }

    msg += '= GRAND TOTAL: ' + AccountingFormat(transtype == 'Invoice' ? CDec2(headerdata.ServedAmount) : CDec2(headerdata.OrderedAmount), 4) + '\n\n';
    msg += '*** \n\n';    

    if (backorder != undefined) {
        var BOrows = backorder.Rows, BOGrandTotal = 0;

        if (BOrows.length > 0) {
            msg += 'BACK ORDER ITEMS: \n\n';

            for (var i = 0; i < BOrows.length; i++) {
                var pDR = BOrows[i], BOlinetotal = (transtype == 'Invoice' ? CDec2(pDR.ServedAmount) : CDec2(pDR.OrderedAmount));
                msg += (i + 1) + '.\n';
                msg += 'PN: ' + pDR.ProductNo + '\n';
                msg += 'Brand: ' + pDR.Brand + '\n';
                msg += 'Desc: ' + pDR.ProductNameOnly + '\n';
                msg += '= ' + (transtype == 'Invoice' ? CDec2(pDR.Quantity) : CDec2(pDR.OrderedQuantity)) + ' ' + pDR.UnitMeasure + ' @ ' + AccountingFormat(pDR.UnitPrice, 4) + '\n';
                msg += '= Line Total: ' + AccountingFormat(BOlinetotal, 4) + '\n\n';

                BOGrandTotal += BOlinetotal;
            }

            msg += '= GRAND TOTAL: ' + AccountingFormat(BOGrandTotal, 4) + '\n\n';
            msg += '*** \n\n';
        }        
    }

    if (CStr2(headerdata.Remarks) != '') {
        msg += 'REMARKS: \n';
        msg += headerdata.Remarks;
    }

    if (returnasstring) {
        return msg;
    } else {
        txtarea.val(msg).select();
        document.execCommand("copy");
        WebToast('Copied to clipboard');

        AjaxSend({
            url: '/ERP/TagTransAsFollowUp',
            data: { InvoiceID: invoiceID },
            success: function (e) {

            }
        })
    }
}

function ShowProductImage(productid) {
    lightbox.option({
        'resizeDuration': 200,
        'wrapAround': true,
        'imageFadeDuration': 100,
        'alwaysShowNavOnTouchDevices': true
    })

    AjaxSend({
        url: 'Organization/GetPIMPictures',
        data: { ProductID: productid },
        success: function (e) {
            var vRows = e.Results.Rows;
            var divPIMImages = $('#divPIMImages');

            if ($('body').find('#divPIMImages').length == 0) {
                $('body').append('<div id="divPIMImages" class="bxslider" style="display:none;"></div>');
                divPIMImages = $('#divPIMImages');
            }

            divPIMImages.empty();

            if (vRows.length > 0) {
                for (var i = 0; i < vRows.length; i++) {
                    vRows[i].Index = i;
                    var srcBig = '/Content/MobizImagePIM/' + sessionStorage.OrgID + '/Big/' + vRows[i].GroupImageID + '_big.jpg';

                    divPIMImages.append($('<a data-role="none" data-lightbox="pimimages">')
                        .attr('id', vRows[i].ID)
                        .attr('href', srcBig)
                        .append($('<img>')
                            .data('data', vRows[i]).attr('src', srcBig)
                        )
                    )
                }

                $('#' + vRows[0].ID).click();
            } else {
                NewMessageBox('No images found for this product.');
            }
        }
    })
}

function RoundOff(vNumber) {
    // https://stackoverflow.com/a/11832950/334045
    return Math.round((CDec2(vNumber) + Number.EPSILON) * 100) / 100;
}

function GetNewImageFiles(vParams) {
    vParams = $.extend({
        OrgID: null,
        Folder: null,
        Success: function() { },
        Error: function () { },
        Complete: function () { }
    }, vParams);

    var pFiles = Transfers.GetFiles('www/Content/' + vParams.Folder + '/' + vParams.OrgID);
    pFiles = pFiles.split('\n');
    AjaxSend({
        url: 'PIMProduct/GetPictureFilenames',
        data: {
            Folder: vParams.Folder,
            FowstID: vParams.FowstID
        },
        success: function (vData) {
            vData = vData.Result.split('\r\n');
            var pLackingFiles = CompareFilenames(pFiles, vData);
            DownloadFiles({
                OrgID: vParams.OrgID,
                Filenames: pLackingFiles,
                Success: vParams.Success,
                //    function () {
                //    pFiles = Transfers.GetFiles('www/Content/' + vParams.Folder + '/' + vParams.OrgID);
                //    pFiles = pFiles.split('\n');
                //    AjaxSend({
                //        url: 'PIMProduct/GetGuidesPictureFilenames',
                //        success: function (vData) {
                //            vData = vData.Result.split('\r\n');
                //            var pLackingFiles = CompareFilenames(pFiles, vData);
                //            DownloadFiles({
                //                OrgID: vParams.OrgID,
                //                Filenames: pLackingFiles,
                //                Success: vParams.Success,
                //                Error: vParams.Error,
                //                Path: 'Content/' + vParams.Folder + '/'
                //            });
                //        }
                //    });
                //},
                Complete: vParams.Complete,
                Error: vParams.Error,
                Path: 'Content/' + vParams.Folder + '/'
            });
        }, error: function () {
            vParams.Success()
        }
    });

    function CompareFilenames(vDeviceFiles, vRemoteFiles) {
        var pOut = [];
        for (var pRemoteIndex = 0; pRemoteIndex <= vRemoteFiles.length - 1; pRemoteIndex++) {
            var pRemoteFile = vRemoteFiles[pRemoteIndex];
            if (pRemoteFile == '')
                continue;
            var pExisting = false;
            for (var pDeviceIndex = 0; pDeviceIndex <= vDeviceFiles.length - 1; pDeviceIndex++) {
                var pDeviceFile = vDeviceFiles[pDeviceIndex];
                if (pDeviceFile == '')
                    continue;
                if (pDeviceFile.toLowerCase() == pRemoteFile.toLowerCase()) {
                    pExisting = true;
                    break;
                }
            }
            if (!pExisting)
                pOut.push(pRemoteFile);
        }

        return pOut;
    }

    function DownloadFiles(vParams) {
        vParams = $.extend({
            OrgID: null,
            Filenames: [],
            Index: 0,
            Success: function(){},
            Error: function(){}
        }, vParams);
        if (vParams.Index > vParams.Filenames.length - 1) {
            vParams.Complete();
            return;
        }

        vParams.Success((CInt2(vParams.Index) + 1) + ' of ' + vParams.Filenames.length)

        var pFilename = vParams.Filenames[vParams.Index];
        Transfers.Download({
            Source: vParams.Path + vParams.OrgID + pFilename,
            Destination: 'www/' + vParams.Path + vParams.OrgID + pFilename,
            Success: function(vData) {
                vParams.Index++;
                DownloadFiles(vParams);
            },
            Error: function(vData) {
                vParams.Error(vData);

                //download remaining files
                vParams.Index++;
                DownloadFiles(vParams);
            }
        });
    }
}

function RoundDecimal(vOriginalValue, vDenominator, vMethod) {
    let pDec, pFactor, pUpper, pLower;
    
    if (vDenominator > 1) {
        throw new Error("vDenominator must be less than 1");
    }

    vMethod = vMethod.toLowerCase();
    if (vDenominator === 0) {
        if (vMethod === 'up') {
            return Math.ceil(vOriginalValue);
        } else if (vMethod === 'down') {
            return Math.floor(vOriginalValue);
        } else {
            pUpper = Math.ceil(vOriginalValue);
            pLower = Math.floor(vOriginalValue);
            if (Math.abs(vOriginalValue - pUpper) < Math.abs(vOriginalValue - pLower)) {
                return pUpper;
            } else if (Math.abs(vOriginalValue - pLower) < Math.abs(vOriginalValue - pUpper)) {
                return pLower;
            } else {
                pDec = Math.floor(vOriginalValue) / 10;
                pDec = pDec - Math.floor(pDec);
                pDec *= 10;
                pDec = Math.floor(pDec);
                return (pDec % 2 === 0) ? pLower : pUpper;
            }
        }
    }
    
    pDec = vOriginalValue - Math.floor(vOriginalValue);
    pFactor = pDec / vDenominator;
    
    if (vMethod === 'up') {
        return Math.min(Math.ceil(pFactor) * vDenominator, 1) + Math.floor(vOriginalValue);
    } else if (vMethod === 'down') {
        return Math.min(Math.floor(pFactor) * vDenominator, 1) + Math.floor(vOriginalValue);
    } else {
        pUpper = Math.min(Math.ceil(pFactor) * vDenominator, 1) + Math.floor(vOriginalValue);
        pLower = Math.min(Math.floor(pFactor) * vDenominator, 1) + Math.floor(vOriginalValue);
        if (Math.abs(vOriginalValue - pUpper) < Math.abs(vOriginalValue - pLower)) {
            return pUpper;
        } else if (Math.abs(vOriginalValue - pLower) < Math.abs(vOriginalValue - pUpper)) {
            return pLower;
        } else {
            pDec = Math.floor(vOriginalValue) / 10;
            pDec = pDec - Math.floor(pDec);
            pDec *= 10;
            pDec = Math.floor(pDec);
            return (pDec % 2 === 0) ? pLower : pUpper;
        }
    }
}

function WrapTextByWords(text, limit) {
    var words = text.split(" ");
    var lines = [];
    var currentLine = "";

    for (var word of words) {
        if ((currentLine + word).length <= limit) {
            currentLine += (currentLine ? " " : "") + word;
        } else {
            lines.push(currentLine);
            currentLine = word;
        }
    }
    if (currentLine) {
        lines.push(currentLine);
    }

    return lines
}

function DateDiff(vDate1, vDate2) {

    vDate1 = RemoveTime(vDate1);
    vDate2 = RemoveTime(vDate2);

    const MS_PER_DAY = 1000 * 60 * 60 * 24;
    const diffDays = Math.floor((vDate2 - vDate1) / MS_PER_DAY);

    return diffDays;
}