@Code
    ViewData("Title") = "Welcome"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
<style>
.ui-page-header-fixed {
    padding-top: 0px;
}

.ui-page .ui-body-c .ui-page-footer-fixed .ui-page-header-fixed .ui-page-active
{
    padding-bottom: 0px;
}
.ui-btn-up-green {
    border: 1px solid #3A7214;
    background: #5BAB25;
    color: #fff;
    text-shadow: 0 -1px 1px #000;
    background-image: -moz-linear-gradient(top, #7EC54E, #41AB25);
    background-image: -webkit-gradient(linear,left top,left bottom,
        color-stop(0, #7EC54E),
        color-stop(1, #41AB25));
    -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#7EC54E', EndColorStr='#41AB25')";
}
.ui-btn-up-green a.ui-link-inherit { 
    color: #fff;
}
.ui-btn-hover-green {
    border: 1px solid #176E00;
    background: #6EB64B;
    color: #fff;
    text-shadow: 0 -1px 1px #000;
    background-image: -moz-linear-gradient(top, 
        #99D472, 
        #6EB64B);
    background-image: -webkit-gradient(linear,left top,left bottom,
        color-stop(0, #99D472),
        color-stop(1, #6EB64B));
    -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#99D472', EndColorStr='#6EB64B')";
}
.ui-btn-hover-green a.ui-link-inherit {
    color: #fff;
}
.ui-btn-down-green {
    border: 1px solid #347722;
    background: #7EC54E;
    color: #fff;
    text-shadow: 0 -1px 1px #000;
    background-image: -moz-linear-gradient(top, #569E39, #7EC54E);
    background-image: -webkit-gradient(linear,left top,left bottom,
        color-stop(0, #569E39),
        color-stop(1, #7EC54E));
    -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#569E39', EndColorStr='#7EC54E')";
}
.ui-btn-down-green a.ui-link-inherit {
    color: #fff;
}
.ui-btn-up-green,
.ui-btn-hover-green,
.ui-btn-down-green {
    font-family: Helvetica, Arial, sans-serif;
    text-decoration: none;
}

.tableAvailable {
    height: 60px;
    margin: 5px;
    border-radius: 50px;
    text-align: center;
    line-height: 60px;
    background-color: #16a085;
    color: #fff;
    cursor: pointer;
    font-size: 20px;
    width: 95%;
    border: none;
    outline:none;
    -moz-user-select: none;
    -khtml-user-select: none;
    -webkit-user-select: none;
    user-select: none;
}

.tableOccupied {
    background-color: #c1392b;
}

.tableDisable {
    display:none;
    position: absolute;
    left: 3px;
    top: 0;
    width: 100%;
    height: 100%;
    background-color: rgba(255,255,255,0.7);
}

.tableChecklist{
    display:none;
    position: absolute;
    left: 0;
    top: 0;
    width: 100%;
    height: 100%;
}

.tableBadge{
    position: absolute;
    top: 0;
    right: 0;
    background-color: #CCC;
    color: #FFF;
    padding: 4px;
    text-shadow: none;
    max-width: 50%;
    overflow: hidden;
    white-space: nowrap;
    text-overflow: ellipsis;
    border-radius: 50px;
    display:none;
    box-shadow: 0px 1px 1px #404040;
}

.tableBadge-a {
    background-color: #2196f3;
    color: #FFF;
}

.tableBadge-b {
    background-color: #ff9800;
}

.tableBadge-c {
    background-color: #3f43b5;
}

.tableBadge-d {
    background-color: #ffeb3b;
    color: #3c3c3c;
}

.tableBadge-e {
    background-color: #795548;
}

.tableBadge-f {
    background-color: #673ab7;
}

.tableBadge-g {
    background-color: #00bcd4;
}

.tableMerge{
    border: 3px solid #3c3532;
}

</style>
@Section head
<link type="text/css" href="/Content/flatui-css-custom.css" rel="Stylesheet" />
<link type="text/css" href="/Content/bootstrap-custom.css" rel="Stylesheet" />
<script>
    require([GetPathFormat('/Scripts/sources/TableView/NewTable.js?crc=')]);
</script>

End Section


<div data-role="page" id="TableView">
    <div class="ui-grid-c" subpage="header" id="divMainHeader" style="display:block;">
        <div class="ui-block-a">
            <div id="btnBack" style="margin: 0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                <div style="height: 25px;"><img alt="home" src="/Content/images/Home.png" width="30px" align="center" /><br /></div>
                <div style="font-size: 13px; margin-top: 10px;">Home</div>
            </div>
        </div>
        <div class="ui-block-b">
            <div id="btnTableFowst" style="margin: 0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                <div style="font-size: 13px; margin-top: 10px;" id="txtFowstName"></div>
            </div>
        </div>
        <div class="ui-block-c">
            <div id="butTableViewOptions" style="margin: 0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                <div style="font-size: 13px; margin-top: 10px;">Options</div>
            </div>
        </div>
        <div class="ui-block-d">
            <div id="butViewMode" style="margin: 0px; height: 80px;" data-role="button" data-theme="green" data-corners="false">
                <div style="height: 25px;">
                    <img alt="More Options" src="/Content/images/View_Mode.png" width="30px" align="center" /><br />
                </div>
                <div style="font-size: 13px; margin-top: 10px;">Table View</div>
            </div>
        </div>
    </div>

    <div subpage="header" id="divJoinHeader" style="display:none;">
        <div class="ui-grid-a">
            <div class="ui-block-a">
                <div id="btnJoinCancel" style="margin: 0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                    Cancel
                </div>
            </div>
            <div class="ui-block-b">
                <div id="btnJoinOk" style="margin: 0px; height: 80px;" data-role="button" data-theme="green" data-corners="false">
                    Join
                </div>
            </div>
        </div>
        <div style="padding:15px;">
            <b>Note: <span class="required">*</span></b>
            Please select tables that you want to join. Then click "Join" button.
        </div>
    </div>    

    <div class="ui-grid-a" subpage="header" id="divTransferTableHeader" style="display:none;">
        <div class="ui-block-a">
            <div id="butTransferTableCancel" style="margin: 0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                Cancel
            </div>
        </div>
        <div class="ui-block-b">
            <div id="butTransferTableOk" style="margin: 0px; height: 80px;" data-role="button" data-theme="green" data-corners="false">
                Transfer
            </div>
        </div>
    </div>

    <div class="ui-grid-solo" subpage="header" id="divSplitHeader" style="display:none;">
        <div class="ui-block-a">
            <div id="btnSplitCancel" style="margin: 0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                Done
            </div>
        </div>
    </div>

    <div subpage="header" id="divSplitBillHeader" style="display:none;">
        <div class="ui-grid-solo">
            <div class="ui-block-a">
                <div id="btnSplitBillCancel" style="margin: 0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                    Cancel
                </div>
            </div>            
        </div>
        <div style="padding:15px;">
            <b>Note: <span class="required">*</span></b>
            <span id="spanSplitBillInstruction"></span>
        </div>
    </div>    

    <div>
        <div data-role="popup" id="popSplitJoinedTables" data-overlay-theme="a" data-theme="a" data-corners="false">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Split Table</h3></center>
            <div data-role="content" class="ui-content ui-body-d jPopupContent">
                <b>Noted <span class="required">*</span>: </b>
                To remove a table one by one, just click the X button at the right of the table name. Or to split all tables, just click the 'Split All' button.

                <div style="padding:15px 15px 0px 15px;">
                    <ul id="listOfSelectedJoinedTables" data-role="listview" class="li-theme-c noBorderList custom-list custom-list-sm"></ul>
                </div>
                
                <br />
                <br />
                <a data-role="none" id="btnSplitAllTable" class="btn custom-btn-theme-a">Split All</a>
                <br /><br />
                <a data-role="none" id="btnOkTable" class="btn custom-btn-theme-b">Ok</a>
                <br /><br />
                <a data-rel="back" data-role="none" class="btn custom-btn-theme-c">Cancel</a>
            </div>
        </div>

        <div data-role="popup" id="popupTotalNumberOfCustomer" class="ui-corner-all" style="padding:20px;">

            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Guest Count</h3></center>
            
            <div data-role="content" style="text-align: center; ">
                <label id="lblTableNo"></label>
                <br /><br />
                <label for="txtTotalNumberOfCustomer">No. of Guest (Diners)<span style="color: #FF0000;">*</span></label>
                @*<input type="number" id="txtTotalNumberOfCustomer" maxlength="3" style="text-align: center" value="1" />*@
                <select id="txtTotalNumberOfCustomer"></select>
            </div>

            <div id="divProductSalesPersonContainer">
                <div>                    
                    <button id="btnAddRepresentative">Add Representative</button>
                </div>
                <div id="divProductSalesPerson">
                    <ul data-role="listview" id="lstSalesRepContainer" data-inset="true">
                        <li data-role="list-divider">Sales Representative List</li>
                        <li>
                            <div>
                                <ul data-role="listview" id="lstSalesRep" data-inset="true"></ul>
                            </div>
                        </li>
                    </ul>
                </div>
                <br />
            </div>

            <button id="btnSaveTotalNumberOfCustomer" type="submit">Save</button>
            <a data-role="button" data-rel="back">Cancel</a>
        </div>

        <div data-role="popup" id="popupTableOptions" class="ui-corner-all" style="padding:20px;">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Options</h3></center>
            <div data-role="content" style="text-align: center; ">
                <button id="butTableOptionsSplitTable">Split Table</button>
                <button id="btnTableOptionJoin">Join Table</button>
                <button id="butTableOptionsTransferTable">Transfer Table</button>
                <button id="butTableOptionsSplitMergeBill">Split / Merge Bill</button>
                <a data-role="button" data-rel="back">Cancel</a>
            </div>
        </div>
    </div>

    <div data-role="content" data-theme="c">
        <div>
            <div style="display:none;" id="instructionSplit">
                <b>Note: <span class="required">*</span></b>
                Click a table to split the joined tables.
                <br /><br />
            </div>
            <div style="display:none;" id="instructionTransferTable">
                <b>Note: <span class="required">*</span></b>
                <span id="spanTransferTableInstruction"></span>
                <br /><br />
            </div>
            <div id="divDiningArea" class="custom-tab">
                <ul id="lstDiningArea">
                    @*<li class="active" custom-tab-content="#tabCompanyProfile">Profile</li>*@
                </ul>
            </div>
            <div id="divTabContent">

            </div>
        </div>
    </div>

    <div data-role="popup" id="divPopNumericPassword" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
        <center><h3 style="margin-left:10px; margin-right:10px;">Sales Rep</center>
        <div data-role="content">
            Please enter your PIN.
            <div name="message"></div>
            @* http://stackoverflow.com/a/25484576/334045 *@
            <input type="number" pattern="[0-9]*" style="-webkit-text-security:disc" /> 
            <button name="ok">Ok</button>
            <button name="close">Close</button>
        </div>

    </div>
</div>