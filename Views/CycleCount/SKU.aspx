<%@ Page Language="VB" Inherits="System.Web.Mvc.ViewPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <script type="text/javascript" src="../../Scripts/jquery-1.6.1.min.js"></script>
    <script type="text/javascript" src="../../Scripts/sources/CycleCount/jquery.mobile-1.0.1.min.js"></script>
    <script type="text/javascript" src="../../Scripts/sources/CycleCount/SKU.js"></script>
    <link rel="stylesheet"  href="../../Content/jquery.mobile-1.0.1.min.css" />
</head>
<body>
    <div>
        
        <div>Barcode/QrCode Scanner in this section</div> <br />
        <div class="ui-grid-a">
            <div data-role="footer" data-theme="a">
                <div class="ui-block-a"> <input type="text" style="font-weight:bold" id="txtSearch" placeholder="Search" data-theme="a"/> </div>
                <%--<div class="ui-block-b"> <a href="SKUInfo">Search</a>--%>
                    <div class="ui-block-b"> <button id="Search" data-theme="a" style="width:"10px"> Q </button> </div>
            </div>

            <br />

            <ul id="lstBranch" data-role="listview" class="ui-listview">
            
            </ul>

            </div>      
    </div>
    </div>
</body>
</html>
