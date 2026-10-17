@Code
    ViewData("Title") = "Customer Service Level"
    Layout = "~/Views/Main/Layout.vbhtml"
End Code

    <script src="/Scripts/jqplot/jquery.jqplot.min.js" type="text/javascript"></script>
    <script src="/Scripts/jqplot/jqplot.categoryAxisRenderer.min.js" type="text/javascript"></script>
    <script src="/Scripts/jqplot/jqplot.canvasAxisLabelRenderer.min.js" type="text/javascript"></script>
    <script src="/Scripts/jqplot/jqplot.canvasTextRenderer.min.js" type="text/javascript"></script>
    <script src="/Scripts/jqplot/jqplot.canvasAxisTickRenderer.min.js" type="text/javascript"></script>
    <script src="/Scripts/jqplot/jqplot.barRenderer.min.js" type="text/javascript"></script>
    <script src="/Scripts/jqplot/jqplot.highlighter.min.js" type="text/javascript"></script>
    <script src="/Scripts/jqplot/jqplot.cursor.min.js" type="text/javascript"></script>
    @If ProWeb.Common.UseMinifiedJavascripts Then
        @<text>
        <script src="/Scripts/minified/CustomerServiceLevel.js" type="text/javascript"></script>
        </text>
    Else
        @<text>
        <script src="/Scripts/sources/Project/CustomerServiceLevel.js" type="text/javascript"></script>
        </text>
    End If
    <link href="/Content/css/jquery.jqplot.min.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        
    </style>

<div id="divCustomerServiceLevel">
    <div name="divResult">
    </div>
    <div class="widgets">
        <div class="widget">
            <h3 class="handle">
                Parameters</h3>
            <div>
                <table class="TableForm">
                    <tr>
                        <td class="label tdField">
                            Invoice date from:
                        </td>
                        <td>
                            <input id="txtInvoiceDateFrom" name="txtInvoiceDateFrom" value="@Format(Now.Date.AddMonths(-1), cGlobal.FormatDateGrid)" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label tdField">
                            Invoice date to:
                        </td>
                        <td>
                            <input id="txtInvoiceDateTo" name="txtInvoiceDateTo" value="@Format(Now.Date, cGlobal.FormatDateGrid)" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label tdField">
                            Group by:
                        </td>
                        <td>
                            <select name="cbDurationType">
                                <option value="weekly">Weekly</option>
                                <option value="monthly">Monthly</option>
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td>
                            <button name="butDisplay">Display</button>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div class="widget">
            <h3 class="handle">
                Data</h3>
            <div>
                <div name="divData">
                </div>
            </div>
        </div>
        <div class="widget">
            <h3 class="handle">
                Data Chart</h3>
            <div>
                <div name="divChart" id="divChart" style="height: 400px; width: 750px;">
                </div>
            </div>
        </div>
        <div class="widget">
            <h3 class="handle">
                Percentage Chart</h3>
            <div>
                <div name="divChart" id="divChart2" style="height: 400px; width: 700px;">
                </div>
            </div>
        </div>
    </div>
    <div>
        Note: To zoom in, drag the portion of the chart that you want to zoom</div>
    <div>
        To zoom out, double-click the chart</div>
</div>
