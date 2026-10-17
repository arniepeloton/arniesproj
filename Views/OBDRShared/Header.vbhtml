<!DOCTYPE html>
<html>
<head>
    <title>@ViewData("Title")</title>
    @RenderPage("Head.vbhtml")

    <style type="text/css">
        .aLink
        {
            
            font-size:large;            
            padding:10px;
            font-style:italic;
        }
        
        #divUser
        {
            position: relative;
            padding: 10px;
            box-shadow: 0 0 5px #888888;
            border-radius: 4px 4px 4px 4px;
            margin: 10px;
        }
        #divUser:hover
        {
            background: white;
            z-index: 122;
        }
        #divUserMenu
        {
            display: none;
            position: absolute;
            top: 38px;
            right: 0px;
            background-color: White;
            padding: 10px;
            border-top: none;
            border-radius: 4px 4px 4px 4px;
        }
        #divUser:hover #divUserMenu
        {
            display: block;
        }
        #divUserMenu a
        {
            height: 16px;
            -moz-border-radius: 5px;
            border-radius: 5px;
            padding: 6px;
            display: block;
            background-position: left center;
            background-repeat: no-repeat;
            padding-left: 25px;
        }
        #divUserMenu a:hover
        {
            text-decoration: none;
            background-color: #A6C9E2 !important;
            color: White !important;
        }
        #divLoginButtons a
        {
            width: 75px;
            height: 50px;
            display: block;
            float: left;
        }
        #divLoginButtons div
        {
            clear: both;
        }
        #lnkUser
        {
            background-position: right center;
            background-repeat: no-repeat;
            padding-right: 25px;
        }
        header > div 
        {
            float:left;
            position:static
        }
        nav 
        {
            margin-top:9px
        }
    </style>

    <!-- Custom Scripts -->
    <script src="../../Scripts/sources/OBDR/plugins/plAccountManagements.js"></script>
    <script src="../../Scripts/sources/OBDR/BankRegistry.js"></script>
</head>
<body>
    <div id="bd-main">
    <!-- Main Body -->
        <header>
            <div id="BankLogo">
            </div>
            <div id="main-title">   
                <span class="main-title-style">Online Bank Deposit Registry</span>
                <br />
                <span class="sub-title-style">@ViewData("SubTitle")</span>
                <hr />
                <span class="user-title">Welcome @ViewData("BankUsersFirstName")</span>
                <div class="tpt-pos-logout"><a href="logout/" id="tpt-pos-obdr-logout">Logout</a></div>
                <hr />
            </div>
        </header>
        