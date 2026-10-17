<html>
    <head>
        <title>
            @Code
                ViewData("Title") = "Planogram"
                Layout = "~/Views/Main/Layout.vbhtml"
            End Code
        </title>

        <style type="text/css">
            .Indent
            {
                padding-left: 30px;
            }
        </style>
        
        <script type="text/javascript">
            $(function () {
                $("#tabs").tabs();
            });

            function load_Select(var_Name, var_Num) {
                document.write("<select name=" + var_Name + " style=width:230px>");
                for (int_X = 1; int_X <= var_Num; int_X++)
                    document.write("<option value=" + int_X + ">" + int_X + "</option>");

                document.write("<select/>");
            }

            function On_Click() {
                $("#divDialog").dialog({
                    minWidth: 500,
                    minHeight: 329,
                    buttons: [
                    {
                        text: "Save"
                    },
                    {
                        text: "Close",
                        click: function () {
                            $("#divDialog").dialog("close");
                        }
                    }]
                });
            }
        </script>

        <script src="../../Scripts/sources/Project/PlanGeneral.js" type="text/javascript"></script>
        <script src="../../Scripts/sources/Project/PlanGondola.js" type="text/javascript"></script>
    </head>
    
    <body>
        <div class="demo">
            <div id="tabs">
	            <ul>
		            <li><a href="#tabs-1">General</a></li>
		            <li><a href="#tabs-2">Section/Gondola</a></li>
	            </ul>
	            <div id="tabs-1">
                    <div id="divGeneral">
                        <div name="divResult">
                        </div>
                        <table class="TableForm">
                            <tr>
                                <td class="Indent">Branch Name:</td>
                                <td>
                                    <input type="text" name="txtBranch" style="width:225px" />
                                    <input type="button" name="btnBrowse" value="Browse" />
                                </td>
                            </tr>
                            <tr>
                                <td class="Indent">No. of Aisles from Left to Right of Store:</td>
                                <td>
                                    <script type="text/javascript">
                                        load_Select("cmbAisles", 100);
                                    </script>
                                </td>
                            </tr>
                            <tr>
                                <td class="Indent">Total no. of Store Aisles:</td>
                                <td>
                                    <script type="text/javascript">
                                        load_Select("cmbStore", 100);
                                    </script>
                                </td>
                            </tr>
                            <tr>
                                <td class="Indent">Default no. of Sections per Aisle:</td>
                                <td>
                                    <script type="text/javascript">
                                        load_Select("cmbSection", 20);
                                    </script>
                                </td>
                            </tr>
                            <tr>
                                <td class="Indent">Default no. of Shelves per Section:</td>
                                <td>
                                    <script type="text/javascript">
                                        load_Select("cmbShelf", 10);
                                    </script>
                                </td>
                            </tr>
                            <tr>
                                <td class="Indent">Number of End Caps:</td>
                                <td>
                                    <script type="text/javascript">
                                        load_Select("cmbEndCap", 100);
                                    </script>
                                </td>
                            </tr>
                            <tr>
                                <td class="Indent">Safety Stock in Days:</td>
                                <td>
                                    <script type="text/javascript">
                                        load_Select("cmbSafety", 100);
                                    </script>
                                </td>
                            </tr>
                            <tr>
                                <td class="Indent">Min Inventory in Days:</td>
                                <td>
                                    <script type="text/javascript">
                                        load_Select("cmbMinimum", 7);
                                    </script>
                                </td>
                            </tr>
                            <tr>
                                <td class="Indent">Max Inventory in Days:</td>
                                <td>
                                    <script type="text/javascript">
                                        load_Select("cmbMaximum", 7);
                                    </script>
                                </td>
                            </tr>
                            <tr>
                                <td class="Indent">Nice Cap of Shelf(% of Min/Max in Units:)</td>
                                <td>
                                    <script type="text/javascript">
                                        load_Select("cmbNiceCap", 100);
                                    </script>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <button name="btnSave" style="width:100px">Save</button>
                                </td>
                            </tr>
                        </table>         
                    </div>
	            </div>
	            <div id="tabs-2">
                    <div id="divGondola">
                        <div name="divResult">
                        </div>

                        <table class="TableForm">
                            <tr>  
                                <td>Branch Name: </td>
                                <td>
                                    <span name="spanBranch"></span>
                                    <input type="button" name="btnBrowseBranch" value="Browse" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <table style="font-size:10px; text-align:center">
                                        <tr>
                                            <td>Aisles</td>
                                            <td>Position</td>
                                            <td>Section</td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <div name="divAisles">
                                                </div>
                                            </td>
                                            <td>
                                                <div name="divPosition">
                                                </div>
                                            </td>
                                            <td>
                                                <div name="divSection">
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <br />
                        <div id="divDialog">
                            <table class="TableForm">
                                <tr>
                                    <th colspan="2">SAMPLE OUTPUT</th>
                                </tr>
                                <tr>
                                    <td>Item Description: </td>
                                    <td>
                                        <span name="spanItemDescription"></span>
                                        <input type="button" name="btnBrowseItem" value="Browse"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Unit Measure: </td>
                                    <td>
                                        <div id="divUnitMeasure">
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <br />
                        <!--###########################-->
@*                        <button name="btnNew" onclick="On_Click()">New</button>
                        <button name="btnEdit">Edit</button>*@
                        <!--###########################-->
                      
                        <br />
		                <table class="TableForm" style="font-size:8px; text-align:center">
                            <tr>
                                <th>Item Description</th>
                                <th>Avg. Sales Value / Day</th>
                                <th>Avg. Sales Volume / Day</th>
                                <th>Avg. Sales Unit / Day</th>
                                <th>Unit Measure</th>
                                <th>Lead Time in Days</th>
                                <th>Safety Stock in Days</th>
                                <th>Min Inv. in Days</th>
                                <th>Min Inv. in Units</th>
                                <th>Unit Measure</th>
                                <th>Max Inv. in Days</th>
                                <th>Max Inv. in Units</th>
                                <th>Unit Measure</th>
                                <th>Facing in Units</th>
                                <th>Layer in Units</th>
                                <th>Depth in Units</th>
                                <th>Total Units in Display</th>
                                <th>Nice Cap of Shelf(% of Min)</th>
                                <th>Nice Cap</th>
                                <th>Recommendation</th>
                            </tr>
                        </table>
                        <br />
                        @*<button name="btnSave" style="width:100px">Save</button>*@
                    </div>
	            </div>
            </div>
        </div><!-- End demo -->
    </body>
</html>

