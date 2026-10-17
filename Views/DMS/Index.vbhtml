@Code
    ViewData("Title") = "Document Management System"
    Layout = "~/Views/Shared/DMSLayout.vbhtml"
End Code
@Section head
    <script>
        require([GetPathFormat('/Scripts/sources/DMS/Index.js?crc=')]);
    </script>

@*<link rel="stylesheet" href="/Content/flatui-css-custom.css">
<link rel="stylesheet" href="/Content/bootstrap-custom.css">
<link rel="stylesheet" href="/Content/Ecommerce/css/bootstrap.min.css">

<link rel="stylesheet" href="/Content/Ecommerce/css/main.css">
<link rel="stylesheet" href="/Content/Ecommerce/css/blue.css">*@

    <style type="text/css">
        body {
            background-color: white !important;
        }
        .buttonExpense > img{
            height: 35px;
            text-overflow: clip;
            margin-bottom: 10px;
            margin-top: 10px;
        }

        .form-control{
            height:40px;
        }

        .header{
            position: fixed;
            top: 0;
            width: 100%;
            z-index: 1000;
        }

        .header~.container{
            margin-top:50px;
        }

        .header > .container{
            display:flex;
        }

        .header > .container > div{
            float: left;
            margin-bottom: 0px;
            overflow: hidden;
            font-size: 25px;
            white-space: nowrap;
            text-overflow: ellipsis;
        }

        .alert-neutral {
            background-color: #f2f2f2;
            border-color: #d6d6d6;
            color: #000;
        }
    </style>
End Section

<div class="container">
    
</div>