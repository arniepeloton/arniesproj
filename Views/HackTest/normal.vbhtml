<html>
    <head>
        <style>
            #parent 
            {
                height:200px;
                background-color:Red;
            }
            #photo 
            {
                width:100px;
                background-color:Black;
                height:180px;
                float:left;
            }
            #text 
            {
                float:left;
                height:150px;
                background-color:Blue;
                display:block;
                width:100%;
            }
        </style>
        <script src="/Scripts3/others/require.js"></script>
        <script src="/Scripts/jQuery.js"></script>

        <script>
            require(['/Scripts3/others/jquery.mobile.js'], function() {
                $(function() {
                    //$('#txtFocus').focus();
                    $('#butFocus').click(function() {
                        $('#txtFocus').focus();
                    });
                    //$('#butFocus').click();

                    setTimeout( 
                        function() {
                    $('#butFocus').click()
                        }, 2000);
                });
            });
        </script>
    </head>
    <body>
        <input />
        <input id="txtFocus"/>
        <button id="butFocus">Focus</button>

        <div id="parent">
            <div id="photo"></div>
            <div id="text">
            </div>
        </div>
    </body>
</html>