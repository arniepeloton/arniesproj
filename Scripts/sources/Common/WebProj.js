$(function(){

    $.blockUI.defaults.message = '<div class="divInnerBlocker"><div>Please wait...</div></div>';
    $.blockUI.defaults.css.width = null;
    AddScrollLink();
    //InitCss();
    $.TextAreaMaxLength();
    $.datepicker.setDefaults({
        changeMonth: true,
        changeYear: true
    });

}); // $(function(){

function Enum() {};

function InitCss() {
};

function InitCss1() {
    
    $('h1, h2, h3')
        .addClass('ui-widget-header ui-corner-all')
        .css('margin-bottom', '3px');

    var pButtons;

    pButtons = $('button, .Button')
    pButtons.each(function(vIndex, vItem){
        var pList;
        var pRegEx;
        var pPrimary, pSecondary;

        pRegEx = new RegExp(/(ui-icon-\w+)/g);
        pList = pRegEx.exec(vItem.className);

        while (pList != null) {
            if (pPrimary == null)
                pPrimary = pList[0];    
            else {
                pSecondary = pList[0];
                break;
            };
            pList = pRegEx.exec(vItem.className);
        };

        $(vItem).button({
            icons: {
                primary: pPrimary,
                secondary: pSecondary
            }
        });

    }); // pButtons.each

    InitTextboxes();
    InitDropDowns();
    $.ToolTip();
    $('.Error').MakeError();
    $('.Highlight').MakeHighlight();

    function InitTextboxes() {
     
        var pElements;
        pElements = $(':text, :password, textarea');
        pElements.not('.simple')
            .css('padding', '.4em')
            .addClass('ui-widget ui-widget-content ui-corner-all');
        pElements.filter('.simple')
            .addClass('ui-widget ui-widget-content');
        $('textarea').autogrow();

    }; // END InitTextboxes

    function InitDropDowns() {
        // comboboxes
        $('select')
            .css('padding', '.4em')
            .addClass('ui-widget ui-widget-content ui-corner-all');
        $('option')
            .css('padding-right', '10px');
    };

    // i didn't include "select"
    $("input:checkbox, input:radio, input:file").uniform();

}; // InitCss

// vController = (string) name of the asp.net MVC controller
// vParentElement = (div)
// vCallBack = (function(ID)) = called during a successful search
//function InitSearch(vController, vParentElement, vCallBack) {
//    $('.divSearch .divProgress').hide();
//    vParentElement.find('.divSearch :button').click(function (e) {
//        e.preventDefault();
//        vParentElement.find('[name=divSearch]').block();
//        AjaxSend({
//            url: '/' + vController + '/Search',
//            data: vParentElement.find('[name=KeyWords]').serializeObject(),
//            success: function (data) {
//                try {
//                    vParentElement.find("[name=tbData] tr.DataRow").remove();
//                    if (data.length > 0)
//                        vParentElement.find('[name=SearchResultTemplate]').tmpl(data).appendTo(vParentElement.find('[name=tbData]'));
//                    vParentElement.find('[name=tbData] img').load(function () {
//                        $(this).vAlign()
//                            .fadeIn()
//                            .parent().css('background-image', 'url("")');
//                    }).error(function (e) {
//                        $(this).attr('src', '/content/images/NoPhoto.jpg')
//                            .vAlign()
//                            .parent().css('background-image', '');
//                    });
//                    vParentElement.find('[name=tbData] .lnkEdit').click(function (e) {
//                        e.preventDefault();
//                        vCallBack(e.target.name);
//                    });
//                    vParentElement.find("[name=tbData]").show();
//                } catch (err) {
//                    alert(err.message);
//                }
//                vParentElement.find('.divSearch .divProgress').hide(1000);
//                vParentElement.find('[name=divSearch]').unblock();
//            }, // function (data)
//            error: function (XMLHttpRequest, textStatus, errorThrown) {
//                vParentElement.find('[name=divSearch]').unblock();
//                alert('Error: ' + XMLHttpRequest.responseText);
//            }
//        }); // AjaxSend
//    });         // $('.divSearch :button').click(function (e) 
//};

// VERTICALLY ALIGN FUNCTION
// http://www.seodenver.com/simple-vertical-align-plugin-for-jquery/
$.fn.vAlign = function() {
	return this.each(function(i){
	var ah = $(this).height();
	var ph = $(this).parent().height();
	var mh = (ph - ah) / 2;
	$(this).css('margin-top', mh);
	});
};

function AddScrollLink() {
    // http://www.learningjquery.com/2007/08/animated-scrolling-for-same-page-links
    // http://beski.wordpress.com/2009/04/21/scroll-effect-with-local-anchors-jquery/
    
    $('a[href*=#]').click(function (e) {
        if (location.pathname.replace(/^\//, '') == this.pathname.replace(/^\//, '')
            && location.hostname == this.hostname) {

            var pTarget = $(this.hash);

            pTarget = pTarget.length && pTarget || $('[name=' + this.hash.slice(1) + ']');
            pTarget = pTarget.filter('a');
            if (pTarget.length > 0) {
                e.preventDefault();
                pTarget.ScrollTo();
                return false;
            }
        };
    });
};

$.fn.ScrollTo = function () {
    
    $('html, body').animate({ scrollTop: this.offset().top }, 500);
    return this;

};

$.fn.serializeObject = function () {
    var o = {};
    var a = this.serializeArray();
    $.each(a, function () {
        if (o[this.name]) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        } else {
            o[this.name] = this.value || '';
        }
    });
    return o;
};

function HtmlEncode(vText) {
    // http://stackoverflow.com/questions/24816/escaping-html-strings-with-jquery/24870#24870

    var escaped = vText + '';
    var findReplace = [[/&/g, "&amp;"], [/</g, "&lt;"], [/>/g, "&gt;"], [/"/g, "&quot;"]];
    for (x in findReplace)
        escaped = escaped.replace(findReplace[x][0], findReplace[x][1]);
    return escaped;

};

function AjaxSend(vParams) {
    var pData;
    var pShouldEncode;

    pData = vParams.data;
    if (pData) {
        pData = JSON.stringify(pData);  
        if (pData && pData.search(/[<>]/) >= 0) {
            pData = HtmlEncode(pData);
            pShouldEncode = true;
        };
    };
    $.ajax({
        url: vParams.url,
        type: 'POST',
        dataType: 'json', // ajax expects to receive data in json format
        data: pData,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('AjaxRequest', '1');
            if (pShouldEncode)
                xhr.setRequestHeader('ShouldEncode', '1');
        },
        success: vParams.success,
        error: vParams.error
    });
};

// UL[s] is the expected object
// vParent = (j) optional. Tabify will search for tab_content only inside this jElement
//           if undefined, will search the entire document 
$.fn.Tabify = function (vParent) {

    this.each(function (vIndex, vValue) {
        Tabs.CreateTabGroup($(vValue), vParent);
    });

    return this;

}; // Tabify

// class Tabs
var Tabs = new function () {

    this.CreateTabGroup = function (vJElement, vParent) {
        
        var pLinks;
        var pTabGroup;
        var pTabContent;
        var pLink;
          
        pTabGroup = new TabGroup();

        pLinks = vJElement.find('a[href*=#]');
        pLinks.each(function (vIndex, vValue) {
            pLink = $(vValue);
            if (vParent)
                pTabContent = vParent.find(pLink.attr('href'));
            else
                pTabContent = $(pLink.attr('href'));
            if (pTabContent.length == 0)
                if (vParent)
                    pTabContent = vParent.find('[name=' + pLink.attr('href').substring(1) + ']');
                else
                    pTabContent = $('[name=' + pLink.attr('href').substring(1) + ']');
            pTabContent = pTabContent.filter('.tab_content');
            if (pTabContent.length > 0) {
                // this is a valid tab
                pTabGroup.Add(pLink, pTabContent);
            };
        });

        if (pTabGroup.Length() > 0) {
            pTabGroup.SelectTab();
        };

    }; // CreateTabGroup

    function TabGroup () {

        var mItems;
        var MyClass = this;

        mItems = [];
        this.Add = function (vLink, vContent) {
            vLink.click(this.LinkClick);
            mItems[mItems.length] =  {
                Link: vLink,
                Content: vContent
            };
        }; // Add

        this.LinkClick = function (e) {
                
            e.preventDefault();
            MyClass.SelectTab($(e.target));

        }; // LinkClick

        // vLink = jQuery object of <a>
        this.SelectTab = function (vLink) {
                
            if (vLink == undefined) {
                // no parameter given, so find a list item or link with "active" class
                $(mItems).each(function (vIndex, vValue) {
                    if (vValue.Link.hasClass('active')
                        || vValue.Link.parent().hasClass('active')) {

                        vLink = vValue.Link;
                    };
                });

                if (vLink == undefined) {
                    // no list item nor link with "active" class, so find the first item
                    vLink = mItems[0].Link;
                };
            };

            $(mItems).each(function (vIndex, vValue) {
                if (vValue.Link.attr('href') == vLink.attr('href')) {
                    vValue.Link.addClass('active');
                    vValue.Link.parent().addClass('active');
                    vValue.Content.fadeIn();
                } else {
                    vValue.Link.removeClass('active');
                    vValue.Link.parent().removeClass('active');
                    vValue.Content.hide();
                };
            });

        };

        this.Length = function () {

            return mItems.length;

        }; // Length
    }; // TabGroup

}; // class Tabs

String.prototype.trim = function () {
    // http://blog.stevenlevithan.com/archives/faster-trim-javascript
    return this.replace(/^\s\s*/, '').replace(/\s\s*$/, '');

};

function IsEmail(vText) {

    var pPattern = /^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$/i;

    return pPattern.test(vText);

};

function IsNumeric(vText) {
    // http://stackoverflow.com/questions/18082/validate-numbers-in-javascript-isnumeric/1830844#1830844
    return !isNaN(parseFloat(vText)) && isFinite(vText);
};

function CDec2(vText) {

    if (vText == null)
        return 0;
    else if (typeof vText == 'boolean') 
        return vText ? 1 : 0;
    else if (IsNumeric(vText))
        return parseFloat(vText);
    else if (/^\s*\d+,?\d+\s*$/.test(vText)) {
        vText = vText.replace(/,/, '');
        return parseFloat(vText);
    } else if (/^\s*\(\d+,?\d+\)\s*$/.test(vText)) {
        vText = vText.replace(/\(/, '');
        vText = vText.replace(/\)/, '');
        vText = vText.replace(/,/, '');
        return -parseFloat(vText);
    } else
        return 0;

}; // END CDec2

function CStr2(vText) {

    if (vText == null)
        return '';
    else
        return vText.toString();

};

function IsDate(vText) {
    if (Date.parse(vText).toString() == 'NaN')
        return false;
    else
        return true;
};

Date.prototype.RemoveTime = function () {
    
    return new Date(this.getFullYear(), this.getMonth(), this.getDate());

}; // END Date.prototype.RemoveTime

function DateDiff(vInterval, vDateFrom, vDateTo) {

    var pDivisor;   // milliseconds
    var pDiff;      // milliseconds

    pDiff = new Date(vDateTo) - new Date(vDateFrom);

    switch (vInterval) {
        case DateDiff.Day:
            pDivisor = 1000 * 60 * 60 * 24;
            break;

        default:
            throw 'vInterval: invalid argument';

    }; // END switch (vInterval)

    return parseInt(pDiff / pDivisor);

}; // END DateDiff

DateDiff.Day = 4;

$.TextAreaMaxLength = function() {
    
    $("textarea[maxlength]").TextAreaMaxLength();

}; // $.TextAreaMaxLength

$.fn.TextAreaMaxLength = function() {
    //http://www.techley.net/jquery/textarea-maxlength-using-jquery.html

    $(this).keyup(function(){
        var pText = $(this).val();
        var pLimit = $(this).attr('maxlength');
        var pNewText;

        if (pText.length > pLimit)
        {
            pNewText = pText.substr(0, pLimit);
            $(this).val(pNewText);
            return false;
        };
     });

}; // $.fn.TextAreaMaxLength

$.fn.HasScrollBar = function() {
    // http://stackoverflow.com/questions/2059743/detect-elements-overflow-using-jquery/2060003#2060003
    //note: clientHeight= height of holder
    //scrollHeight= we have content till this height
    var _elm = this[0];
    var _hasScrollBar = false; 
    if ((_elm.clientHeight < _elm.scrollHeight) || (_elm.clientWidth < _elm.scrollWidth)) {
        _hasScrollBar = true;
    }
    return _hasScrollBar;

};

function scrollbarWidth() {
    // http://jdsharp.us/jQuery/minute/calculate-scrollbar-width.php
    var div = $('<div style="width:50px;height:50px;overflow:hidden;position:absolute;top:-2000px;left:-2000px;"><div style="height:100px;"></div>');
    // Append our div, do our calculation and then remove it
    $('body').append(div);
    var w1 = $('div', div).innerWidth(); 
    div.css('overflow-y', 'scroll');
    var w2 = $('div', div).innerWidth();
    $(div).remove();
    return (w1 - w2);
};

function ShowMessage(vText, vTitle, vButtons, vUseHtml) {
    // vButtons (optional) 
    var pDiv;

    if (!vButtons) {
        vButtons = {
            'Ok': function () {
                $(this).dialog('close');
            }
        };
    };

    pDiv = $('<div>');
    if (vUseHtml)
        pDiv.html(vText);
    else
        pDiv.text(vText);
    pDiv.dialog({
        modal: true,
        buttons: vButtons,
        title: vTitle    
    });

}; // END ShowMessage

$.fn.MakeHighlight = function () {

    return this
        .removeClass('ui-state-error')
        .removeClass('alert warning')    // from WhiteLabel
        .addClass('alert info') // "alert warning" from WhiteLabel
        //.addClass('ui-widget ui-state-highlight ui-corner-all HighlightDiv') // old format
        .show()
        .delay(1000)
        .show('pulsate', null, null);

};

$.fn.MakeError = function () {

    return this
        .removeClass('ui-state-highlight')
        .removeClass('alert info')    // from WhiteLabel
        .addClass('alert warning') // "alert warning" from WhiteLabel
        //.addClass('ui-widget ui-state-error ui-corner-all HighlightDiv') // old format
        .show()
        .delay(1000)
        .show('pulsate', null, null);

};

function PropertyCount(vObj) {
// RETURNS 
//       (integer): the number of properties in an object
// http://stackoverflow.com/questions/126100/how-to-efficiently-count-the-number-of-keys-properties-of-an-object-in-javascript

    var pCount = 0;
    var p;

    for (p in vObj)
        if (vObj.hasOwnProperty(p))
            pCount++;

    return pCount;

}; // END  PropertyCount

function GetProperties(vObj) {
    
    var pOut = []
    var p;

    for (p in vObj)
        if (vObj.hasOwnProperty(p))
            pOut.push(p);

    return pOut;
};

function Log(vText) {

    if (console == undefined) return;
    console.log('[' + $.format.date(new Date(), 'yyyy MM dd | hh:mm:ss a') + ']: ' + vText)

};

//http://www.codigomanso.com/en/2009/01/truco-manso-saber-si-un-elemento-es-visible-con-jquery/#comment-4058
jQuery.extend(
    jQuery.expr[":"], {
        reallyvisible: function(a) {
            return (jQuery(a).is(':visible') && jQuery(a).parents(':hidden').length == 0) 
        }
    }
);

function JsonToCsv(vData) {

    var pArray = typeof vData != 'object' ? JSON.parse(vData) : vData;
    var p, p2;
    var pOut;
    var pLine;
    var pCell;

    pOut = '';

    for (p = 0; p <= pArray.length - 1; p++) {
        pLine = '';

        for (p2 in pArray[p]) {
            if (pLine != '') pLine += ',';
            pCell = pArray[p][p2];
            if (typeof pCell == 'string') {
                pCell = '"' + pCell.replace(/"/g, '""') + '"';
                pLine += pCell;
            } else {
                if (typeof pCell != 'object') {
                    Log('JsonToCsv typeof pCell = ' + typeof pCell);
                };
            };
        };
        pOut += pLine + '\r\n';
    };

    return pOut;

}; // END JsonToCsv;

function HandleSessionExpired(vData) {
// PARAMS:
//  vData: from Ajax. Detect if vData.d.Error == 'Access denied'
// RETURNS:
//  false if vData.d.Error == 'Access denied'. Otherwise, true

    if (vData && vData.d && vData.d.Error == 'Access denied') {
        $('<div>').text('Your web browsing session has expired due to extended period of inactivity or the webapp was upgraded. Please log in again.')
            .dialog({
                title: 'You\'ve been logged out',
                modal: true,
                buttons: {
                    'Go to Log In page': function() {
                        window.location = '/login';
                    }
                },
                beforeClose: function() {
                    window.location = '/login';
                    return false;
                },
                width: 300,
                height: 200
            });
        return false;
    } else
        return true;

}; // HandleSessionExpired

String.prototype.repeat = function( num ) {
    // http://stackoverflow.com/questions/202605/repeat-string-javascript/202627#202627
    return new Array( num + 1 ).join( this );
}; // END String.prototype.repeat

function AccountingFormat(vNumber, vDecimals) {

    var pTail = '';

    if (IsNumeric(vDecimals) == false) vDecimals = 2;
    vNumber = CDec2(vNumber);
    if (CDec2(vDecimals) > 0) pTail = '.' + '0'.repeat(vDecimals);
    if (vNumber > 0) 
        return $.format.number(vNumber, '#,##0' + pTail);
    else if (vNumber < 0)
        return '(' + $.format.number(Math.abs(vNumber), '#,##0' + pTail) + ')';
    else
        return '-';

}; // END AccountingFormat
