<!DOCTYPE html>
<html>
<head>
    <title>@ViewData("Title") - Web ERP - Davao Unified Corporation - The Paper Tree</title>
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

    <script type="text/javascript" src="../../Scripts/sources/CommonAdmin/PageFlow.js"></script>

</head>
<body>
    <div id="bd-main">
        <!-- Main Body -->
        <header>
            <a href="/"><div id="logo">
            </div></a>
            <div style="display: inline">                
                <br />
                    <span><a class="aLink" title="Sales" href = "../CRM/"> Sales </a> </span>
                    <a class="aLink" title="Marketing" href = "../welcome/"> Marketing </a> 
                    <a class="aLink" title="Purchasing" href = "../DrpStocks/"> Purchasing </a>
                    <a class="aLink" title="Warehouse and Logistics" href = "../welcome/"> Warehouse &   Logistics </a>
                    <a class="aLink" title="Human Resource" href = "../welcome/"> Human Resource </a>
                    <a class="aLink" title="Management" href = "../reports/"> Management </a>
                    <a class="aLink" title="Retail Stores" href = "../BranchDashboard/"> Retail Stores </a>
               
@*
                Go to:
                <select id="Navigation">
                    <option> <a class="aLink" title="Sales" href = "../CRM/"> Sales </a> </option>
                    <option> <a class="aLink" title="Marketing" href = "../welcome/"> Marketing </a> </option>
                    <option> <a class="aLink" title="Purchasing" href = "../drp/"> Purchasing </a> </option>
                    <option> <a class="aLink" title="Warehouse and Logistics" href = "../welcome/"> Warehouse &   Logistics </a> </option>
                    <option> <a class="aLink" title="Human Resource" href = "../welcome/"> Human Resource </a> </option>
                    <option> <a class="aLink" title="Management" href = "../reports/"> Management </a> </option>
                    <option> <a class="aLink" title="Retail Stores" href = "../BranchDashboard/"> Retail Stores </a> </option>                                    
                </select>*@

                @*<nav>
                    <ul id="nav">
                        @Code
                            Dim mParent As New List(Of cls_Parent)
                            Dim mChild As New List(Of cls_Child)
            
                            WebErpSideBar.Init(mParent, mChild)
                            
                            For Each list_Parent As cls_Parent In mParent
                                If list_Parent._Has_Child = False Then
                                    @<li class="@list_Parent._Menu_Icon"><a href="@Url.Action("../" & list_Parent._Menu_Link & "/")"><span>@list_Parent._Menu_Name.ToString()</span></a></li>
                                        Else
                                    @<li class="@list_Parent._Menu_Icon"><a><span>@list_Parent._Menu_Name.ToString()</span></a>
                                        <ul>
                                            @For Each list_Child As cls_Child In mChild
                                                        If list_Parent._ID = list_Child._ID Then
                                                @<li><a href="@Url.Action("../" & list_Child._Menu_Link & "/")"><span>@list_Child._Menu_Name.ToString()</span></a></li>
                                                        End If
                                                    Next
                                        </ul>
                                    </li>
                                End If
                            Next
                        End Code
                    </ul>
                </nav>*@
            </div> 
                         
            <div id="header">
                <div>
                    <ul>                       
                        <li>
                            <div id="divUser">
                                @If ProWeb.Common.WebSession.UserID.Length > 0 Then
                                    @<div style="display: inline">
                                        <a id="lnkUser" class="i_triangle_down">Hi, @WebProj.WebSession.Instance.FirstName</a>
                                        <div id="divUserMenu" class="ui-widget-content ui-corner-all Shadow">
                                            <div>
                                                <a href="/profile" class="i_user">profile </a>
                                            </div>
                                            <div>
                                                <a href="/logout" class="i_walking_man">logout </a>
                                            </div>
                                        </div>
                                    </div>
                                Else
                                    @<span style="background:#ffffff"> <a href="/login" id="lnkLogin" >Login</a>
                                    </span>
                                End If
                            </div>
                        </li>
                    </ul>
                </div>
                <div id="searchbox">
                    <form id="searchform">
                    <input id="search" class="placeholder" type="search" placeholder="Search" name="query">
                    </form>
                </div>
            </div>
        </header>
