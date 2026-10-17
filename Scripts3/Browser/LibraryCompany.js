$(function () {

    Browser.Init('LibraryCompany', {
        // Url: optional; if omitted, follows vKey
        //  this is the name of the action. Controller URL is prepended internally

        OnItem: function (vDataRow, vItem) {
            // PARAMS:
            //  vDataRow: object containing data. 
            //  vItem: $('<li>')

            vItem.append($('<a>')
                .attr('href', 'LibraryCompany?id=' + vDataRow.ID)
                .data('data', vDataRow)
                .append($('<h3>')
                    .text(vDataRow.Code + ' - ' + vDataRow.CompanyName)
                )
                .click(function (e) {
                    e.preventDefault();
                    if ($.isFunction(Browser.LibraryCompany.Click))
                        Browser.LibraryCompany.Click($(this).data('data'));
                    history.back();

                    return false;
                })
            );
        } // OnItem: function(vDataRow, vItem) {
    }); // Browser.Init('Customer', {

});