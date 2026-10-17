@Code
    ViewData("Title") = "Table View Settings"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
@Section head
    <style>

    /* for table view*/
    .img1
    {
        z-index: 2;
        position: absolute;
        cursor: move;


    }
    .img2
    {
        z-index: 100;


        cursor: pointer;

        /* Set up proportionate scaling */

        height: auto;

        /* Set up positioning */
        position: fixed;
        top: 0;
        left: 0;
    }
    .gridBackground
    {
        /* Set rules to fill background */
        min-height: 100%;
	
        /* Set up proportionate scaling */
        width: 100%;
        height: auto;
	
        /* Set up positioning */
        position: fixed;
        top: 0;
        left: 0;
    }
   .cnv
    {
        z-index: 1;
        background-image: url('/Content/images/canvasGrid.png');
        background-repeat: repeat;
        top:0;
        left:0;
        width:1280px;
        height:720px;
        position:relative;


    }
    .cnvRender
    {

        background-image: url('/Content/images/renderGrid.png');
        width:100%; /* this is a must */
        height:720px; /* this is a must */
        margin:0;
        padding:0;

    }
</style>
    <script>
        require([GetPathFormat('/Scripts/sources/TableView/TableViewInfo.js?crc=')]);
    </script>
End Section
<div id="pageTableViewTemplate" data-role="page">

    <div id="divHeader" data-role="header">
        <h1>Table View Settings</h1>
    </div>

    <div data-role="content" >

        <div>
            <ul data-role="listview" id="lstOrgEmployeeContainer" data-inset="true">   
				<li data-role="list-divider">Table Layout List</li>
                <li>
                    <div data-role="fieldcontain"><a id="btnAddTableViewTemplate" data-role="button" data-icon="plus" data-inline="true">Add New Table Layout</a></div>

                    <form id="frmTableViewSearch">
                        <div><input id="txtTableViewSearch" type="search" value="" /></div>
                    </form>

                    <div data-role="fieldcontain" id="divTableViewTemplate"></div>
                    <div>
                        <ul data-role="listview" id="lstTableViewTemplate" data-inset="true">   
                        </ul>
                    </div>
                </li>
            </ul>
        </div>

        <div>
            <br />
	        <div style="text-align:center"> <a id="btnTableViewDone" data-role="button">Done</a> </div>
        </div>

    </div>

</div>

<div id="pageLayout" data-role="page">
    <div data-role="header">
        <h1>Table View Settings</h1>
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
    </div>
    <div class="ui-grid-a" >
         <div class="ui-block-a" style="width:18%;">
                    <div data-role="content">
                     <label for="txtTemplateName">Section Name:</label>
                     <input type="text" id="txtTemplateName" maxlength="50"/>
                      <label for="selFowst" class="select">FOWST: </label>
                            <select id="selFowst">
                            </select>      
	                        <label for="selFloorArea" class="select">Floor Area in Squared Meters (m<sup>2</sup>): </label>
                            <select id="selFloorArea">
                                <option value="25">Up to 25</option>
                                <option value="50">Up to 50</option>
                                <option value="75">Up to 75</option>
                                <option value="100">Up to 100</option>
                                <option value="150">Up to 150</option>
                                <option value="200">Up to 200</option>
                                <option value="250">Up to 250</option>
                                <option value="300">Up to 300</option>
                                <option value="400">Up to 400</option>
                                <option value="500">Up to 500</option>
                                <option value="600">Up to 600</option>
                                <option value="700">Up to 700</option>
                                <option value="800">Up to 800</option>
                                <option value="900">Up to 900</option>
                                <option value="1000">Up to 1000</option>
                            </select>
                              Click an object to add it on the grid. Arrange the objects according to your establishment's floor plan. 
                        <div style="height:320px; overflow-y:scroll;">
                            <h4>Tables and Chairs</h4>
                            <button id="btnRoundTable2SeaterVertical"><img src="/Content/images/Table and Chairs for Table View Mode/100 pixel Round Table 2 Seater Vertical.png" height="140" width="90"/></button>
                            <button id="btnRoundTable2Seater"><img src="/Content/images/Table and Chairs for Table View Mode/100 pixel Round Table 2 Seater.png" height="90" width="140"/></button>
                            <button id="btnRoundTable4Seater"><img src="/Content/images/Table and Chairs for Table View Mode/100 pixel Round Table 4 Seater.png" height="140" width="140"/></button>
                            <button id="btnRoundTable4Seater150px"><img src="/Content/images/Table and Chairs for Table View Mode/150 pixel Round Table 4 Seater.png" height="145" width="145"/></button>
                            <button id="btnRoundTable8Seater"><img src="/Content/images/Table and Chairs for Table View Mode/200 pixel Round Table 8 Seater.png" height="140" width="140"/></button>
                            <button id="btnRoundTable10Seater"><img src="/Content/images/Table and Chairs for Table View Mode/300 pixel Round Table 10 Seater.png" height="140" width="140"/></button>
                            <button id="btnRoundTable12Seater"><img src="/Content/images/Table and Chairs for Table View Mode/350 pixel Round Table 12 Seater.png" height="140" width="140"/></button>

                            <button id="btnHorizontalRectangle100x100"><img src="/Content/images/Table and Chairs for Table View Mode/Horizontal Rectangle Table 100x100 pixel.png" height="120" width="80"/></button>
                            <button id="btnHorizontalRectangle100x200"><img src="/Content/images/Table and Chairs for Table View Mode/Horizontal Rectangle Table 100x200 pixel.png" height="120" width="180"/></button>
                            <button id="btnHorizontalRectangle100x300"><img src="/Content/images/Table and Chairs for Table View Mode/Horizontal Rectangle Table 100x300 pixel.png" height="100" width="180"/></button>
                            <button id="btnHorizontalRectangle100x400"><img src="/Content/images/Table and Chairs for Table View Mode/Horizontal Rectangle Table 100x400 pixel.png" height="80" width="180"/></button>
                            <button id="btnHorizontalRectangle100x500"><img src="/Content/images/Table and Chairs for Table View Mode/Horizontal Rectangle Table 100x500 pixel.png" height="80" width="180"/></button>
                        
                            <button id="btnRectangleTable100x100pixel4Seater"><img src="/Content/images/Table and Chairs for Table View Mode/Rectangle Table 100x100 pixel 4 Seater.png" height="140" width="140"/></button>
                            <button id="btnVerticalRectangle100x100"><img src="/Content/images/Table and Chairs for Table View Mode/Vertical Rectangle Table 100x100 pixel.png" height="80" width="140"/></button>
                            <button id="btnVerticalRectangle100x200"><img src="/Content/images/Table and Chairs for Table View Mode/Vertical Rectangle Table 100x200 pixel.png" height="180" width="140"/></button>
                            <button id="btnVerticalRectangle100x300"><img src="/Content/images/Table and Chairs for Table View Mode/Vertical Rectangle Table 100x300 pixel.png" height="180" width="80"/></button>
                            <button id="btnVerticalRectangle100x400"><img src="/Content/images/Table and Chairs for Table View Mode/Vertical Rectangle Table 100x400 pixel.png" height="180" width="80"/></button>
                            <button id="btnVerticalRectangle100x500"><img src="/Content/images/Table and Chairs for Table View Mode/Vertical Rectangle Table 100x500 pixel.png" height="180" width="80"/></button>
                        
                            <button id="btnBarStoolAvailable"><img src="/Content/images/Table and Chairs for Table View Mode/Bar Stool - Available.png"/></button>
                            
                            <h4>Doors</h4>
                            <button id="btnDoor1"><img src="/Content/images/Table and Chairs for Table View Mode/Door 1.png" /></button>
                            <button id="btnDoor2"><img src="/Content/images/Table and Chairs for Table View Mode/Door 2.png"/></button>
                            <button id="btnDoor3"><img src="/Content/images/Table and Chairs for Table View Mode/Door 3.png"/></button>
                            <button id="btnDoor4"><img src="/Content/images/Table and Chairs for Table View Mode/Door 4.png"/></button>
                            <button id="btnDoor5"><img src="/Content/images/Table and Chairs for Table View Mode/Door 5.png"/></button>
                            <button id="btnDoor6"><img src="/Content/images/Table and Chairs for Table View Mode/Door 6.png" /></button>
                            <button id="btnDoor7"><img src="/Content/images/Table and Chairs for Table View Mode/Door 7.png"/></button>
                            <button id="btnDoor8"><img src="/Content/images/Table and Chairs for Table View Mode/Door 8.png"/></button>

                            <h4>Windows</h4>
                            <button id="btnGlassWindowHorizontal"><img src="/Content/images/Table and Chairs for Table View Mode/Glass Window - Horizontal.png" width="180"/></button>
                            <button id="btnGlassWindowVertical"><img src="/Content/images/Table and Chairs for Table View Mode/Glass Window - Vertical.png"  height="180"/></button>
                            
                            <h4>POS Counters</h4>
                            <button id="btnPOSCounterLongHorizontalRectangle"><img src="/Content/images/Table and Chairs for Table View Mode/POS Counter Long Horizontal Rectangle.png" height="60" width="180"/></button>
                            <button id="btnPOSCounterLongVerticalRectangle"><img src="/Content/images/Table and Chairs for Table View Mode/POS Counter Long Vertical Rectangle.png" height="180" width="60"/></button>
                            <button id="btnPOSCounterMediumHorizontalRectangle"><img src="/Content/images/Table and Chairs for Table View Mode/POS Counter Medium Horizontal Rectangle.png" height="80" width="160" /></button>
                            <button id="btnPOSCounterMediumVerticalRectangle"><img src="/Content/images/Table and Chairs for Table View Mode/POS Counter Medium Vertical Rectangle.png" height="100" width="60"/></button>
                            <button id="btnPOSCounterSquare"><img src="/Content/images/Table and Chairs for Table View Mode/POS Counter Square.png" height="120" width="120"/></button>
                            
                            <h4>Panels / Dividers</h4>
                            <button id="btnWallPaneHorizontal"><img src="/Content/images/Table and Chairs for Table View Mode/Wall Panel - Horizontal.png" width="120" width="20"/></button>
                            <button id="btnWallPaneVertical"><img src="/Content/images/Table and Chairs for Table View Mode/Wall Panel - Vertical.png" height="180"/></button>
                            <button id="btnBarLongHorizontal"><img src="/Content/images/Table and Chairs for Table View Mode/Bar - Long Horizontal.png" width="180"/></button>
                            <button id="btnBarLongVertical"><img src="/Content/images/Table and Chairs for Table View Mode/Bar - Long Vertical.png" height="180"/></button>
                            <button id="btnBarShortHorizontal"><img src="/Content/images/Table and Chairs for Table View Mode/Bar - Short Horizontal.png" width="180" /></button>
                            <button id="btnBarShortVertical"><img src="/Content/images/Table and Chairs for Table View Mode/Bar - Short Vertical.png" height="180"/></button>

                        </div>
                      
                                <button id="btnPreviewTableView" data-theme="b">Preview</button>
                            <button id="btnSaveTableView" data-theme="e">Save Table Layout</button>
                    </div>
                </div>
                <div class="ui-block-b" style="width:82%;overflow-x:scroll;overflow-y:scroll;">
                    <div id="container" class="cnv">
     
                    </div>
              
                 </div>

            </div>
</div>
<div id="pageRender" data-role="page">
 
        <div id="divRenderGrid">

        </div>

</div>