<!-- CSS -->
<link href="/Content/themes/redmond/jquery-ui-1.8.9.custom.css" rel="stylesheet" type="text/css" />
<link rel="stylesheet" href="/Content/WhiteLabel/style.css" />
<link rel="stylesheet" href="/Content/WhiteLabel/light/theme.css" />
<link href="/Content/Site.css" rel="stylesheet" type="text/css" />
<link href="/Content/WebErp.css" rel="stylesheet" type="text/css" />
<link href="/Content/css/jquery.jqplot.min.css" rel="Stylesheet" type="text/css" media="all" />

<link href="/Content/css/common.css" rel="Stylesheet" />
<link href="/Content/css/WebErp2.css" rel="Stylesheet" />
<link href="/scripts2/dojo/resources/dojo.css" rel="Stylesheet" />
<link href="/scripts2/dijit/themes/tundra/tundra.css" rel="Stylesheet" />
<link href="/scripts2/dojox/grid/resources/Grid.css" rel="Stylesheet" />
<link href="/scripts2/dojox/grid/resources/tundraGrid.css" rel="Stylesheet" />
<link href="/Content/css/dojo_overrides.css" rel="Stylesheet" />

<!-- JQuery -->
<script type="text/javascript" src="../../Scripts/jquery-1.6.1.min.js"></script>
<script type="text/javascript" src="../../Scripts/jquery-ui-1.8.13.custom.min.js"></script>
<script type="text/javascript" src="../../Scripts/sources/CommonAdmin/PageFlow.js"></script>
<script src="/Scripts2/dojo/dojo.js" data-dojo-config="parseOnLoad:false, async: true"></script>
<script src="/Scripts/sources/common/ToolTip.js" type="text/javascript"></script>
<script src="/Scripts/sources/WhiteLabel/plugins.js"></script>

<!-- The script for JQPlot -->
<script src="/Scripts/jqplot/jquery.jqplot.min.js"></script>
<script src="/Scripts/jqplot/jqplot.barRenderer.min.js"></script>
<script src="/Scripts/jqplot/jqplot.pieRenderer.min.js"></script>
<script src="/Scripts/jqplot/jqplot.categoryAxisRenderer.min.js"></script>
<script src="/Scripts/jqplot/jqplot.highlighter.min.js"></script>
<script src="/Scripts/jqplot/jqplot.pointLabels.min.js"></script>

<!-- Other Scripts -->
@If ProWeb.Common.UseMinifiedJavascripts Then
    @<text>
    <script src="/Scripts/minified/common.js" type="text/javascript"></script>
    <script src="/Scripts/minified/Site.js" type="text/javascript"></script>
    <script src="/Scripts/minified/WhiteLabel.js" type="text/javascript"></script>
    <script src="/Scripts/minified/others.js" type="text/javascript"></script>
    </text>
Else
    @<text>
     <script src="/Scripts/sources/others/jquery.format.js" type="text/javascript"></script>
    <script src="/Scripts/sources/others/json2.js" type="text/javascript"></script>
    <script src="/Scripts/sources/others/jquery.blockUI.js" type="text/javascript"></script>
    <script src="/Scripts/sources/others/jquery.autogrow-textarea.js" type="text/javascript"></script>
    <script src="/Scripts/sources/others/jquery.ba-resize.js" type="text/javascript"></script>
    <script src="/Scripts/sources/others/jquery.ba-throttle-debounce.js" type="text/javascript"></script>
    <script src="/Scripts/sources/others/swfobject.js" type="text/javascript"></script>
    <script src="/Scripts/sources/others/downloadify.js" type="text/javascript"></script>
    <script src="/Scripts/sources/others/parseuri.js" type="text/javascript"></script>
    <script src="/Scripts/sources/others/jquery.pure-uploader.js" type="text/javascript"></script>
    <script src="/Scripts/sources/others/jquery.uniform.js" type="text/javascript"></script>
    <script src="/Scripts/sources/others/jquery.textchange.js" type="text/javascript"></script>
    <script src="/Scripts/sources/common/WebProj.js" type="text/javascript"></script>
    <script src="/Scripts/sources/common/Validation.js" type="text/javascript"></script>
    <script src="/Scripts/sources/common/ProWebGrid.js" type="text/javascript"></script>
    <script src="/Scripts/sources/common/ProDataGrid.js" type="text/javascript"></script>
    <script src="/Scripts/sources/common/DataTable.js" type="text/javascript"></script>
    <script src="/Scripts/sources/common/Finder.js" type="text/javascript"></script>
    <script src="/Scripts/sources/common/ToolTip.js" type="text/javascript"></script>
    <script src="/Scripts/sources/common/IeWarning.js" type="text/javascript"></script>
    <script src="/Scripts/sources/common/Slider.js" type="text/javascript"></script>
    <script src="/Scripts/sources/common/Poper.js" type="text/javascript"></script>
    </text>
End If

<!-- ################################################# -->


<style>
   @* html, body
    {
        height: 100%;
        width: 100%;
    }*@
    h2
    {
        display: inline;
        margin-top: 0;
    }
</style>
