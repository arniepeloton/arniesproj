<!DOCTYPE html>
<html>
<head>
    <title>@ViewData("Title") - Web ERP - Davao Unified Corporation - The Paper Tree
    </title>
    @RenderPage("../Main/Header.vbhtml")
</head>
<body>
    <div id="divWrap">
        <table id="tableMain">
            <tr>
                <td id="tdMainTop" colspan="3">
                    <div id="divMainHeadTop">
                    </div>
                    <div id="divMainHeader">
                        <a href="/">
                            <img src="/images/header.jpg" /></a></div>
                </td>
            </tr>
            <tr>
                <td id="tdMainLeft">
                </td>
                <td>
                    <div class="divBorderLeft ui-widget-content ui-corner-all">
                        @RenderBody()
                    </div>
                </td>
                <td id="tdMainRight">
                    <div class="divBorderRight ui-widget-content ui-corner-all">
                        <div id="divResult">
                        </div>
                        <h2>
                            Menu</h2>
                        <ul>
                            <li><a href="/">Home</a></li>
                            <li><a href="/DateTime">Date Time</a></li>
                            <li><a href="/DBConnect">DB Connect</a></li>
                        </ul>
                    </div>
                </td>
            </tr>
            <tr>
                <td id="tdMainBottom" colspan="3">
                    <div class="divBorderBoth  ui-widget-content ui-corner-all">
                        <div id="divMainFooterLinks">
                            <a href="#">Home</a> | <a href="#">About</a> | <a href="#">Contact</a>
                        </div>
                        &copy;2011 Davao Unified Corporation / The Paper Tree
                    </div>
                </td>
            </tr>
        </table>
    </div>
</body>
</html>
