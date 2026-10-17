$(function() {

    var txtSearch = $('#txtSearch');

    $('#butSearch').click(function(e) {
        if (txtSearch.val().length == 0) 
            MessageBox('Please enter a keyword');
        else
            MessageBox('Not yet implemented');

    });

});