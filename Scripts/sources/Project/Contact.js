$(function () {

    var divTweets = $('#divTweets');
    var divTweetsResult = $('#divTweetsResult');
    var txtComment = $('#txtComment')
        .keyup(txtComment_KeyUp)
        .select(txtComment_Select);
    var divCommentPreview = $('#divCommentPreview');
    var butSubmitComment = $('#butSubmitComment')
        .click(butSubmitComment_Click);

    LoadTweets();

    function LoadTweets() {

        divTweetsResult.empty().text('loading...').show();

        AjaxSend({
            url: 'http://api.twitter.com/1/statuses/user_timeline/arvinboggs.json?count=20&page=1&include_rts=true&callback=?',
            success: function (vData) {
                var pText = '';
                divTweetsResult.empty();
                if (vData) {
                    if (vData.length > 0) {
                        // the commented code is for custom rendering of news (tweets)
//                        for(var p = 0; p <= vData.length - 1; p++) {
//                            pText += HtmlEncode($.format.date(new Date(vData[p].created_at), 'MMM d, yyyy h:mm a')) + ': ' + HtmlEncode(vData[p].text) + '</br>';
//                        };
                        LoadGrid(vData);
                    } else {
                        divTweetsResult.text('No news');
                    };
                } else {
                };
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.responseText)
                    divTweetsResult.text(XMLHttpRequest.responseText).MakeError();
                else
                    divTweetsResult.text('Unknown error').MakeError();
            }
        });
    }; // END LoadTweets

    function LoadGrid(vData) {

        var pData = {};

        pData.Columns = {
            'created_at': {
                'Label': 'Date',
                'MajorDataType': 'Date',
                'Format': 'MMM d, yyyy h:mm a'
            },
            'text': {
                'Label': 'Message'
            }
        };
        pData.Rows = vData;

        divTweets.ProDataGrid(pData);

    }; // END LoadGrid

    function Linkify(inputText) {
        // http://stackoverflow.com/questions/37684/how-to-replace-plain-urls-with-links/3890175#3890175

        var replaceText, replacePattern1, replacePattern2, replacePattern3;

        //URLs starting with http://, https://, or ftp://
        replacePattern1 = /(\b(https?|ftp):\/\/[-A-Z0-9+&@#\/%?=~_|!:,.;]*[-A-Z0-9+&@#\/%=~_|])/gim;
        replacedText = inputText.replace(replacePattern1, '<a href="$1" target="_blank">$1</a>');

        //URLs starting with www. (without // before it, or it'd re-link the ones done above)
        replacePattern2 = /(^|[^\/])(www\.[\S]+(\b|$))/gim;
        replacedText = replacedText.replace(replacePattern2, '$1<a href="http://$2" target="_blank">$2</a>');

        //Change email addresses to mailto:: links
        replacePattern3 = /(\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,6})/gim;
        replacedText = replacedText.replace(replacePattern3, '<a href="mailto:$1">$1</a>');

        return replacedText
    };

    function txtComment_KeyUp() {
        
        divCommentPreview.html(Linkify(txtComment.val()));

    };

    function txtComment_Select(e) {

        var dummy;

    };

    function butSubmitComment_Click(e) {
        // http://stackoverflow.com/questions/2139616/window-getselection-gives-me-the-selected-text-but-i-want-the-html/2139792#2139792

        var pRange;
        var pContents;
        var pDiv;

        pRange = window.getSelection().getRangeAt(0);
        pContents = pRange.extractContents();
        pDiv = $('<span>');
        pDiv.css('background-color', 'red');
        pDiv.append(pContents);
        pRange.insertNode(pDiv[0]);
        alert('done');

    }; // END butSubmitComment_Click
});