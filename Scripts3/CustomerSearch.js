$(function() {

    var mSearchResults;
    var mPageResultsShown; // boolean

    (function Init() {

        $('#formSearch').submit(function (e) {
            e.preventDefault();
            Search();
            return false;
        });

        $('#pageKeywords').bind('pageshow', function() {
            $('#txtSearch').focus();
            $('#txtSearch').val(getParameterByName('id')); // debug
        });
        $('#txtSearch').focus();
        $('#txtSearch').val(getParameterByName('id')); // debug
        $('#pageResults').bind('pageshow', function(e) {
            mPageResultsShown = true;
            pageResults_Show();
        }).bind('pagebeforeshow', function(e) {
            if (mPageResultsShown) {
                $('#ulResults').html('');
                $('#popNoRecord').popup('close');
            };            
        }).bind('pageinit', function() {
            pageResults_Init();
        });

    })();

// http://stackoverflow.com/a/901144/334045
function getParameterByName(name)
{
  name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
  var regexS = "[\\?&]" + name + "=([^&#]*)";
  var regex = new RegExp(regexS);
  var results = regex.exec(window.location.search);
  if(results == null)
    return "";
  else
    return decodeURIComponent(results[1].replace(/\+/g, " "));
};


    function Search() {

        AjaxSend({
            url: 'CustomerSearch/Search',
            data: { keywords: $('#txtSearch').val() },
            success: function (vData) {
                mSearchResults = vData.Results;
                $.mobile.changePage('#pageResults');
            },
            error: function (vError) {
                var pDummy;
            }
        });

    };

    function pageResults_Init() {
      
        var pLV = $('#ulResults');
        var pMessage = 'We can\'t seem to find [keyword] within this current results page. Let\'s try looking for [keyword] in our records.';

        $('#butNoRecordCancel').click(function() {
            $('#popNoRecord').popup('close');
        });
        $('#butNoRecordOk').click(function() {
            $.mobile.changePage('#pageKeywords');
            $('#txtSearch').val($('#pageResults input[data-type="search"]').val());
            Search();
        });
        pCount = pLV.children(':visible').not('#no-results').length;
        $('[name=divRecordCount]').text('No records');

        $('#pageResults input[data-type="search"]').on('keyup', function() {
            var pMessage2;
            var pCount;

            pCount = pLV.children(':visible').not('#no-results').length;
            if (pCount === 0) {
                pMessage2 = pMessage.replace(/\[keyword\]/ig, $('#pageResults input[data-type="search"]').val());
                $('#popNoRecord p').text(pMessage2);
                $('#popNoRecord').popup('open');
                $('[name=divRecordCount]').text('No records');
            } else {
                $('[name=divRecordCount]').text('No. of records: ' + pCount);
                //$('#no-results').fadeOut(250);
            };
        });

        $(this).delegate('.ui-input-clear', 'click', function() {
            //$('#no-results').fadeOut(100);
        });

    };

    function pageResults_Show() {

        var pLV = $('#ulResults');
        var pItem;
        var pLink;
        var pCount;

        pLV.html('');
        for (var p = 0; p <= mSearchResults.Rows.length - 1; p++) {
            pItem = $('<li>')
                .append($('<a>')
                    .attr('href', 'Customer?id=' + mSearchResults.Rows[p].ID)
                    .append($('<h3>')
                        .text(mSearchResults.Rows[p].CompanyName)
                    )
                    .append($('<p>')
                        .text(mSearchResults.Rows[p].ContactNumber)
                    ).append($('<p>')
                        .addClass('ui-li-aside')
                        .text(mSearchResults.Rows[p].TinNumber)
                    )
                );
            pLV.append(pItem);
        };
    
        pLV.listview('refresh');
        $('#pageResults input[data-type="search"]').focus();
        pCount = pLV.children(':visible').not('#no-results').length;
        if (pCount === 0) {
            $('[name=divRecordCount]').text('No records');
        } else {
            $('[name=divRecordCount]').text('No. of records: ' + pCount);
        };

    };

});