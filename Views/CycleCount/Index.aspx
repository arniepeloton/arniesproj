<%@ Page Language="VB" Inherits="System.Web.Mvc.ViewPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">    
<script type="text/javascript" src="../../Scripts/jquery-1.6.1.min.js"></script>
    <script type="text/javascript" src="../../Scripts/sources/CycleCount/index.js"></script>
    <script type="text/javascript" src="../../Scripts/sources/CycleCount/jquery.mobile-1.0.1.min.js"></script>

    <link rel="stylesheet"  href="../../Content/jquery.mobile-1.0.1.min.css" />
    
</head>
<body>
    <div data-role="page">

	    <div data-role="header">
		    <h1>Cycle Count</h1>
	    </div><!-- /header -->

        <br />
        <div style="text-align:center">
	        <div style="font-weight:bold">Username <input type="text" style="font-weight:bold" id="txtUsername" placeholder="Username" data-theme="a"/></div>
	        <div style="font-weight:bold">Password <input type="password" style="font-weight:bold" id="txtPassword" placeholder="Password" data-theme="a"/></div>	   
            <div><button id="Login" data-theme="a">Login</button></div>

            <span id="notification"></span>
        </div>
        <!--</div>-->
    </div>
</body>
</html>
