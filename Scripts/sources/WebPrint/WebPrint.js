require(['common'], 
    function () {
        $(document).ready(function () {
            $('body').css('user-select', 'none'); // disable select text in web print page

            $('#divPrintHtml').html(localStorage.getItem('DataToPrint'));

            $('#btnPrintNow').click(function () {
                var datatoprint = $('#divPrintHtml').html();
                var PrintWindow = window.open('about:blank', 'Print');
                PrintWindow.document.write('<html><head><title>Print</title>');
                PrintWindow.document.write('<link rel="stylesheet" href="/Content/flatui-css-custom.css" type="text/css" />');
                PrintWindow.document.write('</head><body style="-webkit-print-color-adjust:exact;">');
                PrintWindow.document.write(datatoprint);
                PrintWindow.document.write('</body></html>');

                PrintWindow.document.close(); // necessary for IE >= 10
                PrintWindow.focus(); // necessary for IE >= 10

                setTimeout(function () {
                    PrintWindow.print();
                    PrintWindow.close();
                }, 100)

                return true;
            })
        })
        .on('keyup keydown', function (e) {
            // 44 Print Screen
            // 83 S , 67 C , 65 A
            // 12 F12
            if (e.which == 44 || e.which == 123 ||
                ((e.which == '83') && (e.ctrlKey || e.metaKey)) ||
                ((e.which == '67') && (e.ctrlKey || e.metaKey)) ||
                ((e.which == '65') && (e.ctrlKey || e.metaKey))) {
                //disable CTRL+A, CTRL+C, CTRL+S, Print Screen Key, F12
                return false;
            }
        })
        .contextmenu(function () {
            //disable right click
            return false;
        });
    }
)