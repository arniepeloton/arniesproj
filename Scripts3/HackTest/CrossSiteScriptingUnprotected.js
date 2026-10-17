$(function() {

    var txtComment = $('#txtComment');

    $('#butPost').click(function(e) {
        if (txtSearch.val().length == 0) 
            MessageBox('Please enter a comment');
        else
            PostComment(txtComment.text());

    });

    function PostComment(vText) {
        
        AjaxSend({
            url: 'HackTest/Search',
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

});