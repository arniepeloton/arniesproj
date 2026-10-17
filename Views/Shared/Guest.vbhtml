<!DOCTYPE html>
<html>
<head>
    <title>@ViewData("Title") - Web ERP - Davao Unified Corporation - The Paper Tree </title>
    @RenderPage("~/Views/Main/Head.vbhtml")
    <style type="text/css">
        #divUser
        {
            display: inline;
            position: relative;
        }
        #divUserMenu
        {
            display: none;
            position: absolute;
            top: 15px;
            right: 0px;
        }
        #divUser:hover #divUserMenu
        {
            display: block;
        }
        #divUserMenu
        {
            background-color: White;
            padding: 10px;
        }
        #divUserMenu div div
        {
            width: 100px;
            height: 16px;
            -moz-border-radius: 5px;
            border-radius: 5px;
            padding: 6px;
        }
        #divUserMenu div a:hover
        {
            text-decoration: none;
        }
        #divUserMenu div a:hover div
        {
            background-color: #A6C9E2;
            color: White;
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
    </style>
</head>
<body>
    <div id="divWrap">
        <table id="tableMain">
            <tr>
                <td id="tdMainTop" colspan="3">
                    <div id="divMainHeadTop">
                    </div>
                    <div id="divMainHeader" class="ui-corner-all ui-widget-content">
                        <a href="/">@*<img src="/images/header.jpg" />*@</a>
                        <table style="width: 100%">
                            <tr>
                                <td>
                                    <div id="home-logo">
                                        <a href="/"><img src="../../Content/images/logo.png" /></a>
                                    </div>
                                    <div id="home-menu">
                                        <span class="first"><a href="/Home/AboutUs/")>About Us</a></span>
                                        <span><a href="/Home/ContactDetails/">Contact Details</a></span>
                                        <span><a href="http://thepapertree.com.ph/CustomerSurvey/CustomerSurvey/">Give Us Feedback</a></span>
                                    </div>
                                    <div style="clear: both"></div>
                                </td>
                                @*<td style="width: 100%">@ViewData("Title")
                                </td>*@
                                <td>
                                    <div style="width: 100px;">
                                        <table>
                                            <tr>
                                                <td colspan="2" style="vertical-align: middle">
                                                   @* <a id="lnkRightMenu">Menu</a>*@
                                                    @*|*@
                                                    <div id="divUser">
                                                        @If Common.WebSession.UserID.Length > 0 Then
                                                            @<div style="display: inline;padding-top:10px;">
                                                                <a id="lnkUser">Hi, @Common.WebSession.FirstName</a>
                                                                <div id="divUserMenu" class="ui-widget-content ui-corner-all Shadow">
                                                                    <div>
                                                                        <a href="/profile">
                                                                        <div>
                                                                            profile</div>
                                                                        </a>
                                                                    </div>
                                                                    <div>
                                                                        <a href="/logout">
                                                                        <div>
                                                                            logout</div>
                                                                        </a>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        Else
                                                            @*@<span> <a href="/login" id="lnkLogin">Login</a>
                                                            </span>*@
                                                        End If
                                                    </div>
                                                </td>
                                            </tr>
                                           @* <tr>
                                                <td style="vertical-align: middle">
                                                    Davao Unified Corporation
                                                </td>
                                                <td>
                                                    <img style="width: 32px; height: 32px" src="/images/DUCLogo.jpg" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    The Paper Tree
                                                </td>
                                            </tr>*@
                                        </table>
                                        <br />
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
            <tr>
                @*<td id="tdMainLeft">
                </td>*@
                <td>
                    <div class="divBorderLeft ui-widget-content ui-corner-all">
                        @RenderBody()
                    </div>
                </td>
                @*                <td id="tdMainRight">
                    <div class="divBorderRight ui-widget-content ui-corner-all">
                        <div id="divResult">
                        </div>
                        <h2>
                            Login</h2>
                        <div id="divLogin" class="">
                            <form id="formLogin" action="/login" method="post">
                            <label>
                                Username:</label>
                            <input id="txtUsername" name="txtUsername" class="textbox" />
                            <label>
                                Password:</label>
                            <input id="txtPassword" name="txtPassword" type="password" class="textbox" />
                            <button id="butLogin" type="submit" class="button ui-icon-key" style="display: block">
                                Login</button>
                            <a href="/forgotpassword">Forgot my password</a>
                            </form>
                        </div>
                    </div>
                </td>
*@
            </tr>
            <tr>
                <td id="tdMainBottom" @*colspan="3"*@>
                    <div class="divBorderBoth  ui-widget-content ui-corner-all">
                        <div id="divMainFooterLinks">
                            <a href="/">Home</a>
                            | <a href="/about">About</a>
                            | <a href="/contact">Contact</a>
                        </div>
                        &copy;2011 Davao Unified Corporation / The Paper Tree
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <style type="text/css">
        #menuRight
        {
            padding: 10px;
        }
        #menuRight table td
        {
            padding: 20px;
        }
        #menuRight .MenuHeader
        {
            font-weight: bold;
        }
        #menuRight li
        {
            padding: 3px;
        }
        #menuRight .MenuHeader
        {
            padding-bottom: 3px;
            display: block;
        }
    </style>
    <div id="menuRight" class="ui-widget-content ui-corner-all Shadow" style="display: none">
        <div style="margin: 0px">
            <form action="/login" method="post">
            Username:
            <input id="txtUsername" name="txtUsername" />
            Password:
            <input id="txtPassword" name="txtPassword" type="password" />
            <button id="butLogInMenu">Login</button>
            </form>
        </div>
        <div>
            <a href="/forgotpassword">Forgot your password</a></div>
        <table>
            <tr>
                <td>
                    <a href="/" class="MenuHeader">Home</a>
                    <ul>
                        <li><a href="/about">About Us</a></li>
                        <li><a href="/contact">Contact Us </a>
                        </li>
                    </ul>
                </td>
                <td>
                    <a href="#1" class="MenuHeader">Employees</a>
                    <ul>
                        <li><a href="/PhilHr">PhilHR</a></li>
                        <li><a href="/CRM">CRM</a></li>
                        <li><a href="/OrderManagement">Order Management</a></li>
                    </ul>
                </td>
                <td>
                    <a href="#1" class="MenuHeader">Customers</a>
                    <ul>
                        <li><a href="#1">Online Shopping</a></li>
                        <li><a href="#1">Track Orders</a></li>
                        <li><a href="#1">Billing Statement</a></li>
                        <li><a href="#1">Pay Bills</a></li>
                        <li><a href="#1">Customer Rewards</a></li>
                        <li><a href="#1">Requet Sales Return</a></li>
                        <li><a href="#1">Shopping Basket</a></li>
                        <li><a href="#1">Withholding Taxable</a></li>
                        <li><a href="#1">Policies</a></li>
                    </ul>
                </td>
                <td>
                    <a href="#1" class="MenuHeader">Suppliers</a>
                    <ul>
                        <li><a href="#1">Track PO</a></li>
                        <li><a href="#1">Track Payments</a></li>
                        <li><a href="#1">Policies</a></li>
                    </ul>
                </td>
            </tr>
        </table>
    </div>
    <!-- login window -->
    <div id="divLogin" style="display: none; text-align: left; padding: 50px">
        <div style="font-size: 24pt; margin-bottom: 20px">
            Log In to The Paper Tree</div>
        <div name="divResult" style="margin-bottom: 20px">
        </div>
        <span name="spanClose" style="position: absolute; right: 10px; top: 10px; cursor: pointer" class="ui-icon ui-icon-closethick" title="close"></span>
        <form name="formLogIn">
        <div name="divBlock">
            <div style="font-size: 12pt; margin-bottom: 10px">
                Username</div>
            <input name="txtUsername" style="font-size: 20pt; width: 412px; margin-bottom: 20px" />
            <div style="font-size: 12pt; margin-bottom: 10px">
                Password</div>
            <input name="txtPassword" type="password" style="font-size: 20pt; width: 412px; margin-bottom: 20px" />
            <div id="divLoginButtons">
                <a href="" class="btn">Don't have an account yet?</a><a href="" class="btn">Get an Account</a><a href="/ForgotPassword" class="btn">Forgot My Password</a>
                <a name="butLogIn" class="btn">Log In</a>
                <button name="butLogIn" class="btn" style="display: none">test</button>
                <div>
                </div>
            </div>
        </div>
        </form>
    </div>
    <!-- END login window -->
</body>
</html>
