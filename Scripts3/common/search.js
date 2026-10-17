var mSearchResults;

$(document).bind('pageinit', function (e) {
    switch ($(e.target).attr('id')) {
        case 'pageKeywords':
            pageKeywords_Loaded();
            break;

        case 'pageResults':
            // pageResults_Loaded();
            break
    };
});

function pageKeywords_Loaded() {

    $('#butSearch').click(function () {
        AjaxSend({
            url: '/MobileTest/SearchCustomer',
            data: {keywords: $('#txtSearch').val()},
            success: function (vData) {
                mSearchResults = vData.Results;
                $.mobile.changePage('#pageResults');
                pageResults_Loaded();
            },
            error: function (vError) {
                var pDummy;
            }
        });
        
    });

};

function pageResults_Loaded() {

    var pLV = $('#ulResults');
    var pItem;
    var pLink;

    pLV.html('');
    for (var p = 0; p <= mSearchResults.Rows.length - 1; p++) {
        pItem = $('<li>')
            .append($('<a>')
                .attr('href', '/MobileTest/Customer')
                .append($('<h3>')
                    .text(mSearchResults.Rows[p].Name)
                )
                .append($('<p>')
                    .text(mSearchResults.Rows[p].HomeAddress)
                ).append($('<p>')
                    .addClass('ui-li-aside')
                    .text(mSearchResults.Rows[p].HomeNum)
                )
            );
        pLV.append(pItem);
    };
    
    pLV.listview('refresh');
    
};