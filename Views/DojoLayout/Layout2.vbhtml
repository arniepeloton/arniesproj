<!DOCTYPE html>
<html>
<head>
    <title>@ViewData("Title")</title>
    @RenderPage("~/Views/DojoLayout/Head.vbhtml")
</head>
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
        .headerLine{margin:5px;}
    </style>

<script type="text/javascript" src="/Scripts/sources/BranchDashboard/BranchDashboard.js"></script>
<link href="/Content/BranchDashboard.css" rel="Stylesheet" type="text/css" media="all" />

<body class="tundra">
    <div id="bd-main">
        <!-- Main Body -->
        <header>
            <a href="/"><div id="logo"></div></a>
        </header>
        <div id="duc-nav-menu">
            <ul>
                <li><a class="aLink" title="Sales" href = "../CRM/"> Sales </a> </span></li>
                <li><a class="aLink" title="Marketing" href = "../welcome/"> Marketing </a></li>
                <li><a class="aLink" title="Purchasing" href = "../DrpStocks/"> Purchasing </a></li>
                <li><a class="aLink" title="Warehouse and Logistics" href = "../welcome/"> Warehouse &   Logistics </a></li>
                <li><a class="aLink" title="Human Resource" href = "../welcome/"> Human Resource </a></li>
                <li><a class="aLink" title="Management" href = "../reports/"> Management </a></li>
                <li><a class="aLink" title="Retail Stores" href = "../BranchDashboard/"> Retail Stores </a></li>
            </ul>
        </div>
    </div>
    <hr class="headerLine" />
    <div>
        @RenderBody()
    </div>
</body>
</html>
@If WebProj.WebSession.Instance.UserID.Length > 0 Then
    
End If