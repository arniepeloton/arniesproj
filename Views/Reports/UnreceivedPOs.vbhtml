@Code
    ViewData("Title") = "Unreceived PO's"
    Layout = "~/Views/Main/Layout.vbhtml"
End Code

    <script type="text/javascript">
        
    </script>
    @If ProWeb.Common.UseMinifiedJavascripts Then
        @<text>
        <script src="/Scripts/minified/UnreceivedPOs.js" type="text/javascript"></script>
        </text>
    Else
        @<text>
        <script src="/Scripts/sources/Project/UnreceivedPOs.js" type="text/javascript"></script>
        </text>
    End If
    <style type="text/css">
        
    </style>

<div id="divUnreceivedPOs">
    <div name="divResult">
    </div>
    <b>Instructions</b>
    <ul>
        <li>To view the content of a particular PO, please click "View" (first column) of the corresponding row. The content of that PO will then be displayed in a popup window.</li>
        <li>If you close the popup window, the contents will be displayed below. To display the popup again, click the "Popup this portion" button. </li>
    </ul>
    <b>Columns</b>
    <ul>
        <li><b>PO amount of unserved line items</b> - The PO amount of line items with zero received quantity. Line items with at least 1 received quantity will not add up to this amount</li>
    </ul>
    <input type="radio" name="optExpired" value="ActiveOnly" id="optExpiredActiveOnly" checked="checked" /><label for="optExpiredActiveOnly" class="CheckLabel">Show only PO's whose cancellation date has not yet passed</label>
    <input type="radio" name="optExpired" value="ExpiredOnly" id="optExpiredExpiredOnly" /><label for="optExpiredExpiredOnly" class="CheckLabel">Show only PO's whose cancellation date has already passed</label>
    <input type="radio" name="optExpired" value="Both" id="optExpiredBoth" /><label for="optExpiredBoth" class="CheckLabel">Show PO's regardless of cancellation date</label>
    <input type="checkbox" name="chkScheduled" id="chkScheduled" /><label for="chkScheduled" class="CheckLabel">Show only PO's that are scheduled for follow up</label>
    <div class="clear">
    </div>
    <button name="butRefresh">Refresh</button>
    <div name="divTable">
    </div>
    <div name="divPOsCount">
    </div>
    <br />
    <div name="divSingle">
        <h2 name="lblHeader">
        </h2>
        <div name="divMaster" class="TableForm">
            <table>
                <tr>
                    <td>
                        PO No.:
                    </td>
                    <td>
                        <span name="spanPoNum"></span>
                    </td>
                </tr>
                <tr>
                    <td>
                        PO Date.:
                    </td>
                    <td>
                        <span name="spanPoDate"></span>
                    </td>
                </tr>
                <tr>
                    <td>
                        Supplier:
                    </td>
                    <td>
                        <span name="spanSupplier"></span>
                    </td>
                </tr>
                <tr>
                    <td>
                        Supplier contact numbers:
                    </td>
                    <td>
                        <span name="spanSupplierContactNumber"></span>
                    </td>
                </tr>
                <tr>
                    <td>
                        Supplier email:
                    </td>
                    <td>
                        <span name="spanSupplierEmail"></span>
                    </td>
                </tr>
                <tr>
                    <td>
                        Line items:
                    </td>
                    <td>
                        <span name="spanLineItems"></span>
                    </td>
                </tr>
                <tr>
                    <td>
                        Total PO amount:
                    </td>
                    <td>
                        <span name="spanTotalAmount"></span>
                    </td>
                </tr>
            </table>
        </div>
        <div id="divSingleTabs">
            <ul>
                <li><a href="#divSingleTabs-1">Stocks</a></li>
                <li><a href="#divSingleTabs-2">Follow-up</a></li>
                <li><a href="#divSingleTabs-3">Follow-up history</a></li>
            </ul>
            <div name="divDetails" id="divSingleTabs-1">
                <div name="divDetailsTable">
                </div>
            </div>
            <div id="divSingleTabs-2">
                <div name="divSingleFollowUpResult"></div>
                Remarks:<br />
                <textarea name="txtFollowUpRemarks" style="width: 446px; height:314px; margin: 10px" maxlength="500"></textarea><br />
                <button name="butFollowUp">Tag as followed up</button>
            </div>
            <div id="divSingleTabs-3">
                <div name="divSingleFollowUpHistoryResult"></div>
                <button name="butFollowUpHistoryRefresh">Refresh</button>
                <div name="divFollowUpHistoryData" style="height:359px">
                </div>
            </div>
        </div>
        <button name="butPopup">Popup this portion</button>
    </div>
    <div name="divComposeWrapper" style="display: none">
        <div name="divCompose">
            <h2 name="lblHeader">
                Compose email</h2>
            <table>
                <tr>
                    <td>
                        To:
                    </td>
                    <td>
                        <input />
                    </td>
                </tr>
                <tr>
                    <td title="Carbon copy">
                        Cc:
                    </td>
                    <td>
                        <input />
                    </td>
                </tr>
                <tr>
                    <td title="Blind carbon copy">
                        Bcc:
                    </td>
                    <td>
                        <input />
                    </td>
                </tr>
                <tr>
                    <td>
                        Attachment:
                    </td>
                    <td>
                        Zipped Excel workbook containing the unserved PO items
                    </td>
                </tr>
                <tr>
                    <td>
                        Message
                    </td>
                    <td>
                        <textarea></textarea>
                    </td>
                </tr>
                <tr>
                    <td>
                        <button>Send</button>
                    </td>
                    <td>
                    </td>
                </tr>
            </table>
            <button name="butPopup">Popup this portion</button>
        </div>
    </div>
</div>
