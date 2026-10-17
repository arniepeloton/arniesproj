@Code
    ViewData("Title") = "Duplicate Receipt"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
@Section head
    <script>
        require([GetPathFormat('/Scripts/sources/SalesPayments/DuplicateReceipt.js?crc=')
            ]);
    </script>
End Section

<div id="pageSalesPaymentDuplicateReceipt" data-role="page"> 
    <div data-role="header"> 
        <h1>@ViewData("Title")</h1>
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
    </div>
    <div data-role="content">

        @*@If Common.WebRequest.QueryString("IsMobile") <> "1" Then
            @<applet name="jzebra" code="jzebra.PrintApplet.class" archive="/Scripts/jzebra.jar" width="10" height="10">
            </applet>
        End If*@

        <div id="divTransInvoice">
            <button id="btnTransInvoice">Select Invoice</button>
        </div>

        <div data-role="fieldcontain">
            <label for="txtDuplicateReceiptReceiptNo">Receipt No.: <span style="color: #FF0000;">*</span></label>
            <input type="text" id="txtDuplicateReceiptReceiptNo" maxlength="15" />
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnDuplicateReceiptPrint" data-role="button">
                <h3>
                    Print Invoice Receipt</h3>
                </a>
            </div>
            <div style="text-align: center">
                <a id="btnDuplicateDeliveryPrint" data-role="button">
                    <h3>
                        Print Delivery Receipt
                    </h3>
                </a>
            </div>
            <div style="text-align: center">
                <a id="btnDuplicateReceiptEmail" data-role="button">
                <h3>
                    Email Invoice Receipt</h3>
                </a>
            </div>
            <div style="text-align: center">
                <a id="btnDuplicateReceiptPrintEmail" data-role="button">
                <h3>
                    Print and Email Receipt</h3>
                </a>
            </div>
	     @*   <div style="text-align:center"> <a id="btnDuplicateReceiptDone" data-role="button" data-rel="back">Done</a></div>*@
        </div>
    </div>
</div>

<div id="divDuplicateReceiptPromptEmail" data-role="dialog">
	<div data-role="header" data-theme="d">
		<h1>Email Address</h1>
	</div>

	<div data-role="content" data-theme="c">

        <div><label>Please check the email address(es) that you want to send an electronic invoice to.</label></div>

        <div id="divDuplicateReceiptEmail" data-role="fieldcontain">
            <div id="divDuplicateReceiptEmailNone" style="display:none; font-weight:bolder; color:Red;"><label>No Available Email</label></div>
            <fieldset id="fldsetDuplicateEmailEmail" data-role="controlgroup">
            </fieldset>
        </div>

        <fieldset data-role="controlgroup">
            <div><label for="txtDuplicateReceiptEmailAddress">Other email address:</label></div>
            <div><label for="txtDuplicateReceiptEmailAddress" style="font-size:small;">(Please separate multiple email addresses with a comma. Ex. sample1@email.com, sample2@email.com)</label></div>
            <div><input type="text" id="txtDuplicateReceiptEmailAddress" /></div>
        </fieldset>

        <br /><br />
		<a id="btnDuplicateReceiptEmailOK" data-role="button">Ok</a>       
		<a id="btnDuplicateReceiptEmailCancel" data-role="button">Cancel</a>    

	</div>

</div>