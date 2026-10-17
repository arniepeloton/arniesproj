@Code
    ViewData("Title") = "Fast Moving Consumer Goods"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
@Section head
    <link rel="stylesheet" href="/Content/flatui-css-custom.css" />
    <link rel="stylesheet" href="/Content/bootstrap-custom.css" />
    <link rel="stylesheet" href="/Content/FMCG.css" />
    <link rel="stylesheet" href="/Content/jquery.dataTables2.css" />

    <script>
        require([GetPathFormat('/Scripts/sources/SalesMenu/FMCG.js?crc=')]);
    </script>

    <style type="text/css">
        .lineitemdisplay > li {
            border-bottom: 2px solid #e8e8e8 !important;
        }

        .fmcg-container{
            background-color:#FFF !important;
            color: #000 !important;
        }

        .fmcgbasic-list > li{
            border:2px solid #FFF;
        }

        .lineitemdisplay > li.void,
        .void{
            border: 2px solid #2196F3 !important;
        }
    </style>
End Section

<div class="fmcg-container-modern" style="text-shadow:none;">
    <div class="fmcg-left">
        <div class="fmcg-header custom-theme-a" id="">
            <div class="divTable">
                <div class="divTableRow border-a">
                    <div class="divTableCell txt-center txt-middle custom-btn-theme-a gradient hover" id="btnFMCGHome">
                        <span class="font-d bold">HOME</span>
                        <br />
                        <span class="font-d">(Ctrl + H)</span>
                    </div>
                    <div class="divTableCell txt-center txt-middle custom-btn-theme-a gradient hover" id="btnFMCGFowst">
                        FOWST: <span id="txtFowstName">&nbsp;</span>
                        <br />
                        <span class="font-d">(Ctrl + F)</span>
                    </div>
                    <div class="divTableCell txt-center txt-middle custom-btn-theme-a gradient hover" id="btnFMCGNewCart">
                        <span class="font-d bold">New Cart</span>
                        <br />
                        <span class="font-d">(Shift + N)</span>
                    </div>
                    <div class="divTableCell txt-center txt-middle custom-btn-theme-a gradient hover" id="btnFMCGMode">
                        <span class="font-d bold">FMCG Mode</span>
                        <br />
                        <span class="font-d">(Ctrl + M)</span>
                    </div>
                    <div class="divTableCell txt-center txt-middle custom-btn-theme-a gradient">                        
                        <span id="txtOrgName">&nbsp;</span>
                        <br />
                        <span id="txtFMCGCurDate"></span>
                        <br />
                        <span id="txtFMCGCurTime"></span>
                    </div>
                </div>
            </div>
        </div>
        <div class="fmcg-content font-d" id="divCart" style="padding:10px;">
            <div id="pageCart" custom-data-role="page" style="display:block; font-size:20px; position:relative; height:100%;">
                <div id="divListOfPlu" style="position: relative; height: calc(100% - 50px); overflow: auto;">
                    <div style="padding:10px;">
                        <table id="tblHeaderForLineDisplay" style="width:100%; border-bottom: 2px solid #828282; padding-bottom: 10px; display:none; table-layout:fixed;">
                            <tbody>
                                <tr class="txt-center" style="border-bottom:2px solid #FFF;">
                                    <td style="width:130px">Product #</td>
                                    <td>Description</td>
                                    <td style="width:50px">Qty</td>
                                    <td style="width:100px;">Price</td>
                                    <td style="width:100px;">Item Total</td>
                                </tr>
                            </tbody>
                        </table>

                        <ul id="ulSelectedProduct" class="custom-list fmcgbasic-list">
                        </ul>
                    </div>
                </div>
                <div style="width:100%; box-sizing: border-box; position:absolute; bottom:0;">
                    <table width="100%">
                        <tr>
                            <td style="width:100px; font-size:25px;">
                                <div subpage="divCartIndication" id="divPLU" style="display:block"><span class="lblCurrentProdQty">1</span> X</div>
                                <div subpage="divCartIndication" id="divQty" style="display:none;">Qty:</div>
                                <div subpage="divCartIndication" id="divVoid" style="display:none;">Void:</div>
                            </td>
                            <td><input type="text" data-role="none" class="form-control" name="txtQtyPlu" placeholder="Enter PLU" style="font-size: 20px;" autofocus /></td>
                        </tr>
                    </table>
                </div>
            </div>
            <div id="pageCustomerOptions" custom-data-role="page" style="display:none;">
            </div>
        </div>
        <div class="fmcg-fleft custom-theme-j gradient">
            <div class="divTable">
                <div class="divTableRow">
                    <div class="divTableCell border-j-right txt-middle">
                        <div id="btnFMCGPLU" class="divTextArea divShortcutKeys">
                            <span class="alignleft font-d">Plu</span>
                            <span class="alignright font-d bold">PGDN</span>
                        </div>
                        <div id="btnFMCGQty" class="divTextArea divShortcutKeys">
                            <span class="alignleft font-d">Qty</span>
                            <span class="alignright font-d bold">END</span>
                        </div>
                        <div id="btnFMCGDisc" class="divTextArea divShortcutKeys">
                            <span class="alignleft font-d">% Disc</span>
                            <span class="alignright font-d bold">F11</span>
                        </div>                        
                    </div>
                    <div class="divTableCell border-j-right txt-middle">
                        <div id="btnFMCGMisc" class="divTextArea divShortcutKeys">
                            <span class="alignleft font-d">Misc</span>
                            <span class="alignright font-d bold">F10</span>
                        </div>
                        <div id="btnFMCGVoid" class="divTextArea divShortcutKeys">
                            <span class="alignleft font-d">Void</span>
                            <span class="alignright font-d bold">INS</span>
                        </div>
                        <div id="btnFMCGSub" class="divTextArea divShortcutKeys">
                            <span class="alignleft font-d">Sub</span>
                            <span class="alignright font-d bold">DEL</span>
                        </div>
                    </div>
                    <div class="divTableCell border-j-right txt-middle">
                        <div id="btnFMCGCashKey" class="divTextArea divShortcutKeys">
                            <span class="alignleft font-d">Cash</span>
                            <span class="alignright font-d bold">PGUP</span>
                        </div>
                        <div id="btnFMCGSoldTo" class="divTextArea divShortcutKeys">
                            <span class="alignleft font-d">SoldTo</span>
                            <span class="alignright font-d bold">HOME</span>
                        </div>
                        <div id="btnFMCGCard" class="divTextArea divShortcutKeys">
                            <span class="alignleft font-d">Card</span>
                            <span class="alignright font-d bold">F6</span>
                        </div>
                        @*<div id="btnFMCGClear" class="divTextArea divShortcutKeys">
                <span class="alignleft font-d">Clear</span>
                <span class="alignright font-d bold">F12</span>
            </div>
            <div class="divTextArea divShortcutKeys">
                <span class="alignleft font-d">Validate</span>
                <span class="alignright font-d bold">F9</span>
            </div>
            <div class="divTextArea divShortcutKeys">
                <span class="alignleft font-d">Reports</span>
                <span class="alignright font-d bold">F8</span>
            </div>*@
                    </div>
                    <div class="divTableCell border-j-right txt-middle">
                        <div class="divTextArea divShortcutKeys">
                            <span class="alignleft font-d">Suspend</span>
                            <span class="alignright font-d bold">F5</span>
                        </div>
                        <div id="btnFMCGRecall" class="divTextArea divShortcutKeys">
                            <span class="alignleft font-d">Recall</span>
                            <span class="alignright font-d bold">F4</span>
                        </div>
                        <div id="btnFMCGGiftCheck" class="divTextArea divShortcutKeys">
                            <span class="alignleft font-d">Gift C</span>
                            <span class="alignright font-d bold">F3</span>
                        </div>
                        @*<div class="divTextArea divShortcutKeys">
                <span class="alignleft font-d">R/A</span>
                <span class="alignright font-d bold">F7</span>
            </div>*@
                    </div>
                    <div class="divTableCell border-j-right txt-middle" style="vertical-align:top;">
                        <div id="btnFMCGPrint" class="divTextArea divShortcutKeys">
                            <span class="alignleft font-d">Print</span>
                            <span class="alignright font-d bold">F2</span>
                        </div>   
                        <div id="btnFMCGPriceVerifier" class="divTextArea divShortcutKeys">
                            <span class="alignleft font-d">Price Ver.</span>
                            <span class="alignright font-d bold">F1</span>
                        </div>                     
                        <div id="btnFMCGSave" class="divTextArea divShortcutKeys">
                            <span class="alignleft font-d">Save</span>
                            <span class="alignright font-d bold">CTRL+S</span>
                        </div>                        
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="fmcg-right">
        <div class="fmcg-header custom-theme-k gradient">
            <div class="divTable">
                <div class="divTableRow">
                    <div class="divTableCell txt-center txt-middle">

                    </div>
                </div>
            </div>
        </div>
        <div class="fmcg-content" id="divPayment" style="background-color: #F7F7F7;">
            <!-- <div class="txt-center"><span>123456789042</span></div>
            <div style="width:100%;">
                <div><span>1 ITEM(S)</span></div>
                <div style="float:right;"><span>100.00</span></div>
                <div style="clear:both;"></div>
            </div>
            <div class="divTextArea">
                <span class="alignleft">STATUS</span>
                <span class="alignright">SUBTOTAL</span>
            </div> -->
            <table cellspacing="3" style="width:100%;height:100%;">
                <tr>
                    <td class="txt-center"><span name="divTransNum" class="font-c">&nbsp;</span></td>
                </tr>
                <tr>
                    <td style="height:40px;">
                        <div class="divTextArea">
                            <span class="alignleft font-c"><span class="lblCurrentProdQty2">1</span> Unit(s)</span>
                            <span class="alignright font-a bold lblCurrentProdTotal">0.00</span>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div class="divTextArea">
                            <span class="alignleft font-c">STATUS</span>
                            <span class="alignright font-c">Subtotal</span>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div class="divTextArea" style="padding:3px 0px 3px 0px; font-size:16px;">
                            <span class="alignleft">Sub Total</span>
                            <span class="alignright bold" id="txtSubTotal">0.00</span>
                        </div>
                        <div class="divTextArea" id="rowVATPercent" style="display:none; padding:3px 0px 3px 0px; font-size:16px;">
                            <span class="alignleft">Less: <span id="lblVATPercentValue"></span> % VAT</span>
                            <span class="alignright bold" id="lblVATPercent">0.00</span>
                        </div>
                        <div class="divTextArea" id="rowAmountLessVAT" style="display:none; padding:3px 0px 3px 0px; font-size:16px;">
                            <span class="alignleft">Total Amount:</span>
                            <span class="alignright bold" id="lblTotalAmount">0.00</span>
                        </div>
                        <div class="divTextArea" style="padding:3px 0px 3px 0px; font-size:16px;">
                            <span class="alignleft">Less Discount</span>
                            <span class="alignright bold" id="txtLessDiscount">0.00</span>
                        </div>
                        <div class="divTextArea">
                            <span class="alignleft font-c">G.Total</span>
                            <span class="alignright font-a bold" id="txtTotal">0.00</span>
                        </div>
                        <div class="divTextArea">
                            <span class="alignleft font-c">Cash</span>
                            <span class="alignright font-a bold" id="txtCash">0.00</span>
                        </div>
                        <div class="divTextArea">
                            <span class="alignleft font-c">Card</span>
                            <span class="alignright font-a bold txtCard">0.00</span>
                        </div>
                        <div class="divTextArea">
                            <span class="alignleft font-c">G.Cert.</span>
                            <span class="alignright font-a bold txtCGC">0.00</span>
                        </div>
                        <div class="divTextArea">
                            <span class="alignleft font-c">Misc Pay</span>
                            <span class="alignright font-a bold txtMisc">0.00</span>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div class="divTextArea">
                            <span class="alignleft font-c" id="lblBalChange">UNPAID</span>
                            <span class="alignright font-a bold" id="txtBalChange">0.00</span>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div class="divTextArea">
                            <span class="alignleft font-c">QUANTITY</span>
                            <span class="alignright font-a bold" id="txtTotalQty">0</span>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td style="text-align:center;">
                        <span id="txtMessage">&nbsp;</span>
                    </td>
                </tr>
            </table>
        </div>
        <div class="fmcg-fright">
            <div class="divTable custom-theme-j gradient">
                <div class="divTableRow">
                    <div class="divTableCell font-d txt-middle border-j-right">
                        Cashier: <span id="txtFMCGCashierName"></span>
                        @*<br /><br />
            Checker: <span id="txtFMCGCheckerName">asdasd <br /> asd</span>*@
                    </div>
                    <div class="divTableCell font-d txt-center txt-middle border-j-right">
                        @*<span class="font-c">P48</span>
            <br />*@
                        <span>SN:</span>
                        <br />
                        <span id="txtFMCGSerialNum"></span>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="divFMCGPopup">

    </div>
</div>