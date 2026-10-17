<html>
<head>    
    <title>
        @Code
            ViewData("Title") = "Wait"
            Layout = "~/Views/BackOrder/BackOrderMaster.vbhtml"
        End Code
    </title>

    <link href="../../Scripts2/dojo/dojox/mobile/themes/common/FixedSplitter.css" rel="stylesheet" /> </link>

    <script type="text/javascript" src="../../Scripts/sources/BackOrder/Replace.js"></script>

    <style> 
            .mblListItem{border-bottom:none}
    </style>

    <script type="text/javascript">
        require([
              "dojox/mobile/FixedSplitter",
              "dojox/mobile/Button"
            ]);
    </script>
</head>
<body>    
    <div>
            <div data-dojo-type="dojox.mobile.RoundRect" shadow="true" style="border:none" >  
                <ul id="lstBOAction">
                    <li data-dojo-type="dojox.mobile.ListItem">
                        <h3> ORIGINAL ORDER </h3>
                    </li>

                        <li data-dojo-type="dojox.mobile.ListItem">
                        
                        </li>

                        <li data-dojo-type="dojox.mobile.ListItem">
                            SKU# : <span id="OriginalSKUNo"> </span> <a href="#">(MORE INFO)</a>
                        </li>

                        <li data-dojo-type="dojox.mobile.ListItem">
                            SKU Description : <span id="OriginalSKUDescription"> </span>
                        </li>

                        <li data-dojo-type="dojox.mobile.ListItem">
                            QTY Ordered : <span id="OriginalQtyOrdered"> </span>
                        </li>

                        <li data-dojo-type="dojox.mobile.ListItem">
                            Unit Measure : <span id="OriginalUnitMeasure"> </span>
                        </li>

                        <li data-dojo-type="dojox.mobile.ListItem">
                            Price : <span id="OriginalPrice"> </span>
                        </li>

                        <li data-dojo-type="dojox.mobile.ListItem">
                            Sub Total : <span id="OriginalSubTotal"> </span>
                        </li>

                        <li data-dojo-type="dojox.mobile.ListItem">
                        
                        </li>

                        <li data-dojo-type="dojox.mobile.ListItem">
                            <button data-dojo-type="dojox.mobile.Button">CANCEL</button>
                            <button data-dojo-type="dojox.mobile.Button">WAIT</button>
                            <button data-dojo-type="dojox.mobile.Button">REPLACE</button>

                            @*<div data-dojo-type="dojox.mobile.FixedSplitter" orientation="H">
                                <div data-dojo-type="dojox.mobile.FixedSplitterPane" style="width:200px;border-right:1px solid black;background-color:yellow;">
                                    <div data-dojo-type="dojox.mobile.RoundRect" shadow="true">
                                      
                                      <div> unserved orders of this item </div>
                                    </div>
                                    unserved orders of this item 
                                </div>
                                <div data-dojo-type="dojox.mobile.FixedSplitterPane" style="background-color:pink;">
                                    <div data-dojo-type="dojox.mobile.RoundRect" shadow="true">
                                      
                                      <div> for original ordered to arrive </div>
                                    </div>
                                    for original ordered to arrive
                                </div>
                            </div>                  
                        </li>*@                        
                        
                </div>
                </ul>
            </div>
    </div>
</body>
</html>
