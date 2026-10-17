<html>
    <head>
    <script type="text/javascript" src="../../Scripts/jquery-1.6.1.min.js"></script>    

    <meta name="viewport" content="width=device-width,initial-scale=1,maximum-scale=1,minimum-scale=1,user-scalable=no"/>
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <title>Planograming Module</title>
    <!-- stylesheet will go here -->
   <link href="../../Scripts2/dojo/dojox/mobile/themes/iphone/iphone.css" rel="stylesheet" />

    <!-- dojo/javascript will go here -->
    <script type="text/javascript" src="../../Scripts2/dojo/dojo/dojo.js"
    djConfig="isDebug:true, parseOnLoad:true"></script>


    <script type="text/javascript">
        dojo.require("dojox.mobile.parser");
        dojo.require("dojox.mobile");
        dojo.requireIf(!dojo.isWebKit, "dojox.mobile.compat");
    </script>
    </head>
    <body>
        <section>@RenderBody()</section>
    </body>
    </html>