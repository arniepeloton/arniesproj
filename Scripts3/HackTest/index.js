$(function() {

    var formSearch = $('#formSearch');
    var txtSearch = $('#txtSearch');

    $('#formSearch').submit(function(e) {
        e.preventDefault();
        if (txtSearch.val().length == 0) 
            MessageBox('Please enter a comment');
        else
            Search();

    });

    $('#butMessage').click(function() {
        MessageBox('mao ni ang message',
            'mao ni ang title',
            [
                {Text: 'mao ni ang button text',
                Click: function() {
                    alert('button1 clicked');
                    }
                }
            ,
                {Text: '2nd button, no close',
                CloseOnClick: false,
                Click: function() {
                    alert('button2 clicked');
                    }
                }
            ]
        );

    });

    function Search() {
        
        var pData = {};

        pData.keywords = txtSearch.val();
        pData.anotherKeyword = 'qeqeqe';
        pData['mobileNum' + 1] = '13123123123';
        pData['mobileNum' + 2] = '13123123123';
        pData.ID = 'mao ni ang id';

        var pIndex = 0
        $('[name=txtMobileNum]').each(function(){
            pIndex ++;
            pData['txtMobileNum' + pIndex] = $(this).val();
        });

        AjaxSend({
            url: 'HackTest/Search',
//            type: 'TEXT',
            data: pData,
            success: function (vData) {
                LoadResult(vData.data.Rows);
            },
            error: function (vError) {
                var pDummy;
            }
        });

    };

    function LoadResult(vRows) {
        
        var pLV = $('#ulResults');
        var pItem;

        pLV.html('');
        for (var p = 0; p <= vRows.length - 1; p++) {
            pItem = $('<li>')
                .append($('<a>')
                    .attr('href', 'Bank?id=' + vRows[p].ID)
                    .attr('data', vRows[p].BankName)
                    .click(function(e) {
                        e.preventDefault();
                        MessageBox('you clicked ' + $(this).attr('data'));
                    })
                    .append($('<h3>')
                        .text(vRows[p].BankName)
                    )
                    .append($('<p>')
                        .text(vRows[p].UserID)
                    ).append($('<a>')
                        //.addClass('ui-li-aside')
                        .attr('href', 'asdasd')
                        .addClass('ui-li-count')
                        .text(vRows[p].ID)
                        .click(function(e) {
                            e.preventDefault();
                            MessageBox('you clicked bubble');
                            return false;
                        })
                    )
                );
            pLV.append(pItem);
        };
    
        pLV.listview('refresh');
//        $('#pageResults input[data-type="search"]').focus();
//        pCount = pLV.children(':visible').not('#no-results').length;
//        if (pCount === 0) {
//            $('[name=divRecordCount]').text('No records');
//        } else {
//            $('[name=divRecordCount]').text('No. of records: ' + pCount);
//        };        
    };

    $('#butAjaxDictionaryArray').click(function() {

        var pData = [];

        pData[0] = {ID: 'asdasdasd',
            Date: new Date(),
            Int: 123
        };

        pData[03] = {ID: '3 three',
            Date: new Date(),
            Int: 3
        };

        pData = {
            data: pData,
            name: 'name nako'
        };

        sessionStorage.data = JSON.stringify(pData);

        AjaxSend({
            url: 'HackTest/AjaxDictionaryArray',
            data: pData,
            success: function (vData) {
                var pDummy;
            },
            error: function (vError) {
                var pDummy;
            }
        });

    });
});