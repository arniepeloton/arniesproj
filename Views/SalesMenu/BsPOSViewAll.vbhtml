@Code
    ViewData("Title") = "Welcome"
    
    If Request.IsAjaxRequest() Then
        Layout = ""
    Else
        Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
    End If
End Code
<style>
.modal, .modal-backdrop {
    bottom: 0 !important;
}

.no-hover.btn-outline-danger:hover{
    color: #dc3545 !important;
    background-color: initial !important;
}

.no-hover.btn-outline-success:hover{
    color: #28a745 !important;
    background-color: initial !important;
}

.required {
    color: #FF0000;
}
</style>
<link rel="stylesheet" href="/Content/metro-calendar.css" />
<link rel="stylesheet" href="/Content/Ecommerce/css/lightbox.css">
<script>
    require(['bootstrapjs'], function () {
        require(['/Content/bootstrap/js/bootstrap_modal.js?v='], function () {
            if (window.location.hash == '#TableView') {
                require([GetPathFormat('/Scripts3/TableView2.js?crc=')]);
            } else {
                require([GetPathFormat('/Scripts3/Bs_POSViewAll.js?crc=')]);
                require([GetPathFormat('/Scripts3/TableView2.js?crc=')]);
            }
        })
    })    
</script>
<div id="pageMain" custom-data-role="page" style="display:block;">
    <div class="bg-warning text-dark font-weight-bold pl-2 pr-2 w-100" name="hRsd" style="display:none;">
        <div class="row align-items-center">
            <div class="col">Reconstruct Sales Data</div>
            <div class="col-auto"><button data-role="none" class="btn" name="btnExitRSD">Exit</button></div>
        </div>
    </div>
    <div class="row" style="height:100%; bottom:0; width:100%;">
        <div id="divPanelLeft" class="col-5 p-0" style="height:100%;overflow:auto; position:fixed; left:0;">
            <div id="divCart" class="list-group">

            </div>
        </div>
        <div id="divPanelRight" class="col-7 p-0" style="height:100%; overflow:auto; position:fixed; right:0;">
            <div id="divPayment" style="display:none;overflow-y: scroll; height:100%; position:relative;"> </div>
            <div id="divPaymentCheck" style="display:none;overflow-y: scroll; height:100%; position:relative;"> </div>
            <div id="divPaymentDebitCard" style="display:none;overflow-y: scroll; height:100%; position:relative;"> </div>
            <div id="divPaymentCreditCard" style="display:none;overflow-y: scroll; height:100%; position:relative;"> </div>
            <div id="divPaymentSRV" style="display:none;overflow-y: scroll; height:100%; position:relative;"> </div>
            <div id="divPaymentGiftCert" style="display:none;overflow-y: scroll; height:100%; position:relative;"> </div>
            <div id="divPaymentOtherType" style="display:none;overflow-y: scroll; height:100%; position:relative;"> </div>
            <div id="divSeniorCitizenPWD" style="display:none;overflow-y: scroll; height:100%; position:relative;"> </div>
            <div id="divPreviewBill" style="display:none;overflow-y: scroll; height:100%; position:relative;"> </div>
            <div id="pageOrderTaker" style="display:none;overflow-y: scroll; height:100%; position:relative;"> </div>

            <div id="divCustomer" style=" display:none;overflow-y: scroll; height:100%; position:relative;"> </div>

            <div id="divHeader" style="position:absolute; top:0; width:100%;">

            </div>

            <div id="divHotButtons" class="bg-dark" style="overflow-y: scroll; width:100%; position:absolute; bottom:0;">

            </div>
        </div>
    </div>

    <div id="divFmcgFooter" data-role="footer" data-theme="a" style="position:fixed; bottom:0; width:calc(100% - 0%); max-height:100px; display:none;">
        <div class="ui-block-a" style="margin-top:5px">
            <div>
                PLU     PGDN
            </div>
            <div>
                QTY     END
            </div>
            <div>
                % DISC F11
            </div>
            <div>
                MISC F10
            </div>
        </div>
        <div class="ui-block-b" style="margin-top:5px">
            <div>
                PLU     PGDN
            </div>
            <div>
                QTY     END
            </div>
            <div>
                % DISC F11
            </div>
            <div>
                MISC F10
            </div>
        </div>
        <div class="ui-block-c" style="margin-top:5px">
            <div>
                PLU     PGDN
            </div>
            <div>
                QTY     END
            </div>
            <div>
                % DISC F11
            </div>
            <div>
                MISC F10
            </div>
        </div>
    </div>
</div>

<div id="pageCustomerOptions" custom-data-role="page" style="display:none;">
    
</div>

<div id="pageStockInfo" custom-data-role="page" style="display:none;">

</div>

<div id="pagePayment" custom-data-role="page" style="display:none;">
    <div id="divPaymentInfoLoading" class="mt-5" style="width:100%; text-align:center;">
        <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden"></span>
        </div>
    </div>
</div>

<div id="pageDuplicateReceipt" custom-data-role="page" style="display:none;">
    <div id="divDuplicateReceiptLoading" class="mt-5" style="width:100%; text-align:center;">
        <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden"></span>
        </div>
    </div>
</div>

<div id="pageHotButton" custom-data-role="page" style="display:none;">
    <div id="divHotButtonLoading" class="mt-5" style="width:100%; text-align:center;">
        <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden"></span>
        </div>
    </div>
</div>

<div id="popupChangePriceLevel" class="modal fade">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Change Prive Level</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <select id="selPriceLevel" data-role="none" class="form-select"></select>
            </div>
            <div class="modal-footer">
                <button data-role="none" data-dismiss="modal" class="btn btn-success btn-block">Ok</button>
            </div>
        </div>
    </div>
</div>

<div id="divSalesPaymentPromptEmail" data-role="dialog" style="display:none;">
	<div data-role="header" data-theme="d">
		<h1>Email Address</h1>
	</div>

	<div data-role="content" data-theme="c">

        <div><label>Please check the email address(es) that you want to send an electronic invoice to.</label></div>

        <div id="divSalesPaymentsEmail" data-role="fieldcontain">
            <div id="divSalesPaymentEmailNone" style="display:none; font-weight:bolder; color:Red;"><label>No Available Email</label></div>
            <fieldset id="fldsetSalesPayEmail" data-role="controlgroup">
            </fieldset>
        </div>

        <fieldset data-role="controlgroup">
            <div><label for="txtSalesPayEmailAddress">Other email address:</label></div>
            <div><label for="txtSalesPayEmailAddress" style="font-size:small;">(Please separate multiple email addresses with a comma. Ex. sample1@email.com, sample2@email.com)</label></div>
            <div><input type="text" id="txtSalesPayEmailAddress" /></div>
        </fieldset>

        <br /><br />
		<a id="btnSalesPaymentEmailOK" data-role="button">Ok</a>       
		<a id="btnSalesPaymentEmailCancel" data-role="button">Cancel</a>    

	</div>

</div>

<div data-role="page" id="TableView" style="display:none;">
    <div data-role="header" data-theme="d">
        <h1>Table View</h1>
    </div>

    <div data-role="content" data-theme="c">
        <div id="tblTables">
            <div class="ui-grid-c">
                <div class="ui-block-a"><a data-role="button" class="ui-bar ui-bar-c" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl1"></span></a></div>
                <div class="ui-block-b"><a data-role="button" class="ui-bar ui-bar-c" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl2"></span></a></div>
                <div class="ui-block-c"><a data-role="button" class="ui-bar ui-bar-c" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl3"></span></a></div>
                <div class="ui-block-d"><a data-role="button" class="ui-bar ui-bar-c" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl4"></span></a></div>
            </div>
            <div class="ui-grid-c">
                <div class="ui-block-a"><a data-role="button" class="ui-bar ui-bar-c" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl5"></span></a></div>
                <div class="ui-block-b"><a data-role="button" class="ui-bar ui-bar-c" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl6"></span></a></div>
                <div class="ui-block-c"><a data-role="button" class="ui-bar ui-bar-c" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl7"></span></a></div>
                <div class="ui-block-d"><a data-role="button" class="ui-bar ui-bar-c" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl8"></span></a></div>
            </div>
            <div class="ui-grid-c">
                <div class="ui-block-a"><a data-role="button" class="ui-bar ui-bar-c" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl9"></span></a></div>
                <div class="ui-block-b"><a data-role="button" class="ui-bar ui-bar-c" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl10"></span></a></div>
                <div class="ui-block-c"><a data-role="button" class="ui-bar ui-bar-c" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl11"></span></a></div>
                <div class="ui-block-d"><a data-role="button" class="ui-bar ui-bar-c" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl12"></span></a></div>
            </div>
            <div class="ui-grid-c">
                <div class="ui-block-a"><a data-role="button" class="ui-bar ui-bar-c" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl13"></span></a></div>
                <div class="ui-block-b"><a data-role="button" class="ui-bar ui-bar-c" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl14"></span></a></div>
                <div class="ui-block-c"><a data-role="button" class="ui-bar ui-bar-c" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl15"></span></a></div>
                <div class="ui-block-d"><a data-role="button" class="ui-bar ui-bar-c" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl16"></span></a></div>
            </div>
            <div class="ui-grid-c">
                <div class="ui-block-a"><a data-role="button" class="ui-bar ui-bar-c" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl17"></span></a></div>
                <div class="ui-block-b"><a data-role="button" class="ui-bar ui-bar-c" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl18"></span></a></div>
                <div class="ui-block-c"><a data-role="button" class="ui-bar ui-bar-c" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl19"></span></a></div>
                <div class="ui-block-d"><a data-role="button" class="ui-bar ui-bar-c" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl20"></span></a></div>
            </div>
        </div>
        <br /> <br />
        <div>
            <p style="text-align:center; font-weight:bold; font-size:larger">View Tables</p>
        </div>
        <div>
            <div class="ui-grid-d">
                <div class="ui-block-a"><a data-role="button" class="ui-bar ui-bar-c" style="height:60px; margin:5px" name="btnTableMultiplier" id="mtp0">1-20</a></div>
                <div class="ui-block-b"><a data-role="button" class="ui-bar ui-bar-c" style="height:60px; margin:5px" name="btnTableMultiplier" id="mtp1">21-40</a></div>
                <div class="ui-block-c"><a data-role="button" class="ui-bar ui-bar-c" style="height:60px; margin:5px" name="btnTableMultiplier" id="mtp2">41-60</a></div>
                <div class="ui-block-d"><a data-role="button" class="ui-bar ui-bar-c" style="height:60px; margin:5px" name="btnTableMultiplier" id="mtp3">61-80</a></div>
                <div class="ui-block-e"><a data-role="button" class="ui-bar ui-bar-c" style="height:60px; margin:5px" name="btnTableMultiplier" id="mtp4">81-100</a></div>
            </div>
        </div>
    </div>

    @* <div data-role="popup" id="popupPayNowMode" data-theme="b" class="ui-corner-all">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Billing Option</h3></center>
            <div data-role="content">

                <a id="lnkPrintBill" data-role="button" data-theme="e"> <h3>Print Billing</h3> </a>
                <a id="lnkBackToTableView" data-role="button" data-theme="e"> <h3>Go Back to Table View</h3> </a>
                <a id="lnkProceedToPayments" data-role="button" data-theme="e"> <h3>Pay Now</h3> </a>
            </div>
        </div>*@
</div>
<div data-role="page" id="SeniorCitizenPWD" style="overflow:auto !important; display:none;">


</div>
<div id="PreviewBill" style="display:none;">
    <div data-role="content" style="padding:0px 20px 0px 20px">
        <div class="ui-grid-a">
            <div class="ui-block-a" style="width: 20%">
                <button data-icon="back" id="btnBackPreviewBill">Back</button>
            </div>
            <div class="ui-block-b" style="text-align: right;width: 80%">
                <h2>Billing</h2>
            </div>
        </div>

        <div id="divDisplayBill">
            <table style="width: 100%">
                <tr>
                    <td colspan="2" style="text-align:center">
                        <label id="lblOrganizationBusinessName" style="font-size:xx-large; font-weight:bold"></label>
                    </td>
                </tr>
                <tr>

                    <td style="text-align:left">
                        <br />
                        <br />
                        Customer :
                    </td>
                    <td style="text-align:right">
                        <br />
                        <br />
                        <label id="lblCustomerNamePreviewBill"> - </label>
                    </td>
                </tr>
                <tr>
                    <td style="text-align:left">
                        Address :
                    </td>
                    <td style="text-align:right">
                        <label id="lblAddressPreviewBill"> -  </label>
                    </td>
                </tr>
                <tr>
                    <td style="text-align:left">
                        TIN :
                    </td>
                    <td style="text-align:right">
                        <label id="lblTINPreviewBill"> -  </label>
                    </td>
                </tr>
                <tr>
                    <td style="text-align:left">
                        Table # :
                    </td>
                    <td style="text-align:right">
                        <label id="lblTableNoPreviewBill"> -  </label>
                    </td>
                </tr>
            </table>
            <hr />
            <table style="width: 100%">
                <tr>
                    <td colspan="2" style="text-align:center">
                        <label id="lblBillNoPreviewBill" style="font-size:xx-large; font-weight:bold"></label> <br />
                        <label id="lblDateTimePreviewBill" style="font-size:xx-large; font-weight:bold"></label>
                    </td>
                </tr>
                <tr>

                    <td style="text-align:left">
                        <br />
                        Cashier :
                    </td>
                    <td style="text-align:right">
                        <br />
                        <label id="lblCashierNamePreviewBill"> -  </label>
                    </td>
                </tr>
            </table>
            <br />
            <div>
                <table style="width:100%">
                    <thead>
                        <tr style="background-color:ButtonFace;">
                            <td style="text-align:center; font-weight:bold; width: 35%; height:35px; border: 1px solid Gray;">Stock Description</td>
                            <td style="text-align:center; font-weight:bold; width: 10%; height:35px; border: 1px solid Gray;">Qty</td>
                            <td style="text-align:center; font-weight:bold; width: 15%; height:35px; border: 1px solid Gray;">Unit Measure</td>
                            <td style="text-align:center; font-weight:bold; width: 20%; height:35px; border: 1px solid Gray;">Unit Price</td>
                            <td style="text-align:center; font-weight:bold; width: 20%; height:35px; border: 1px solid Gray;">Sub Total</td>
                        </tr>
                    </thead>

                    <tbody id='tblBillBody'></tbody>

                </table>

            </div>
            <hr />
            <table style="width: 100%">
                <tr>
                    <td style="text-align:left; font-weight:bold">
                        <br />
                        SKU Count :
                    </td>
                    <td style="text-align:right; font-weight:bold">
                        <br />
                        <label id="lblSKUCountBilling"> - </label>
                    </td>
                </tr>
                <tr>
                    <td style="text-align:left; font-weight:bold">
                        Sub Total :
                    </td>
                    <td style="text-align:right; font-weight:bold">
                        <label id="lblSubTotalBilling"> - </label>
                    </td>
                </tr>
                <tr id="rowVATPercentBilling" style="display:none">
                    <td style="text-align:left; font-weight:bold">
                        Less : <label id="lblVATPercentValueBilling"></label>% VAT
                    </td>
                    <td style="text-align:right; font-weight:bold">
                        <label id="lblVATPercentBilling"> - </label>
                    </td>
                </tr>
                <tr id="rowAmountLessVATBilling" style="display:none">
                    <td style="text-align:left; font-weight:bold">
                        Total Amount :
                    </td>
                    <td style="text-align:right; font-weight:bold">
                        <label id="lblTotalAmountBilling"> - </label>
                    </td>
                </tr>

                <tr>
                    <td style="text-align:left; font-weight:bold">
                        Less : Discount Amount
                    </td>
                    <td style="text-align:right; font-weight:bold">
                        <label id="lblSalesPayTotalDiscountBilling"> - </label>
                    </td>
                </tr>
                <tr id="rowVATBackPercent" style="display:none">
                    <td style="text-align:left; font-weight:bold">
                        Add : VAT
                    </td>
                    <td style="text-align:right; font-weight:bold">
                        <label id="lblVATBackPercent"> - </label>
                    </td>
                </tr>
                <tr>
                    <td style="text-align:left; font-weight:bold">
                        Add : Charges
                    </td>
                    <td style="text-align:right; font-weight:bold">
                        <label id="lblSalesPayChargesBilling"> - </label>
                    </td>
                </tr>
                <tr>
                    <td style="text-align:left; font-weight:bold">
                        Add : Tip
                    </td>
                    <td style="text-align:right; font-weight:bold">
                        <label id="lblTipBilling"> - </label>
                    </td>
                </tr>
                <tr>
                    <td style="text-align:left; font-weight:bold">
                        Add : Local Tax
                    </td>
                    <td style="text-align:right; font-weight:bold">
                        <label id="lblSalesPayOtherTaxBilling"> - </label>
                    </td>
                </tr>
                <tr>
                    <td style="text-align:left; font-size:xx-large; font-weight:bold">
                        <br />
                        Amount Due :
                    </td>
                    <td style="text-align:right; font-size:xx-large; font-weight:bold">
                        <br />
                        <label id="lblOriginalAmountDueBilling" style="display:none"></label>
                        <label id="lblSalesPayAmountDueBilling"> - </label>
                    </td>
                </tr>

                <tr style="display:none">
                    <td style="text-align:left; font-weight:bold">
                        <br />
                        VAT-able Sales :
                    </td>
                    <td style="text-align:right; font-weight:bold">
                        <br />
                        <label id="lblVATableSales"> - </label>
                    </td>
                </tr>
                <tr style="display:none">
                    <td style="text-align:left; font-weight:bold">
                        VAT Amount :
                    </td>
                    <td style="text-align:right; font-weight:bold">
                        <label id="lblVATAmount"> - </label>
                    </td>
                </tr>
                <tr style="display:none">
                    <td style="text-align:left; font-weight:bold">
                        Non-VAT Sales :
                    </td>
                    <td style="text-align:right; font-weight:bold">
                        <label id="lblNonVATSales"> - </label>
                    </td>
                </tr>
                <tr style="display:none">
                    <td style="text-align:left; font-weight:bold">
                        VAT Exempt Sales :
                    </td>
                    <td style="text-align:right; font-weight:bold">
                        <label id="lblVATExemptSales"> - </label>
                    </td>
                </tr>
                <tr style="display:none">
                    <td style="text-align:left; font-weight:bold">
                        Zero-Rated VAT Sales :
                    </td>
                    <td style="text-align:right; font-weight:bold">
                        <label id="lblZeroRatedVATSales "> - </label>
                    </td>
                </tr>
            </table>



        </div>

        <br />
        <hr />
        <h2>Add Tip</h2>
        <div class="ui-grid-c">
            <div class="ui-block-a">
                <div style="text-align: center">
                    <a id="btnAddTip10" data-role="button">
                        <h3>
                            10
                        </h3>
                    </a>
                </div>
            </div>
            <div class="ui-block-b">
                <div style="text-align: center">
                    <a id="btnAddTip20" data-role="button">
                        <h3>
                            20
                        </h3>
                    </a>
                </div>
            </div>
            <div class="ui-block-c">
                <div style="text-align: center">
                    <a id="btnAddTip50" data-role="button">
                        <h3>
                            50
                        </h3>
                    </a>
                </div>
            </div>
            <div class="ui-block-d">
                <div style="text-align: center">
                    <a id="btnAddTip100" data-role="button">
                        <h3>
                            100
                        </h3>
                    </a>
                </div>
            </div>
        </div>
        <div class="ui-grid-a">
            <div class="ui-block-a">
                <label for="txtAddTip" style="font-size:x-large;margin-top:15px">Other Amount</label>
            </div>
            <div class="ui-block-b">
                <input type="number" id="txtAddTip" style="line-height: 2em;text-align: right;" />
            </div>
        </div>
        <br />
        <hr />
        <h2>Adjustments</h2>
        <div class="ui-grid-a">
            <div class="ui-block-a">
                <div style="text-align: center">
                    <a id="btnAddCharges" data-role="button" data-icon="add">
                        <h3>
                            Charges
                        </h3>
                    </a>
                </div>

            </div>
            <div class="ui-block-b">
                <div style="text-align: center">
                    <a id="btnAddDiscount" data-role="button" data-icon="minus">
                        <h3>
                            Discounts
                        </h3>
                    </a>
                </div>

            </div>
        </div>

        <br />
        <div style="text-align: center">
            <a id="btnPrintBilling" data-role="button" data-theme="e">
                <h3>
                    Print Billing
                </h3>
            </a>
        </div>
        <div style="text-align: center">
            <a id="btnPaymentBilling" data-role="button" data-theme="e">
                <h3>
                    Proceed to Payments
                </h3>
            </a>
        </div>
        <div style="text-align: center">
            <a id="btnBacktoTableViewBilling" data-role="button" data-theme="e">
                <h3>
                    Go Back to Table View
                </h3>
            </a>
        </div>
        <br />
        <br />
        <br />
        <br />
    </div>

    <div data-role="popup" id="popupCharges" class="ui-corner-all" style="padding: 20px;width: 400px;  display:none;">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
        <center><h3>Charges and Other Tax</h3></center>
        <div data-role="content">
            <label for="selChargeTypeBilling">Charge Type: </label>
            <select id="selChargeTypeBilling">
                <option value="Service Charge">Service Charge</option>
                <option value="Delivery Charge">Delivery Charge</option>
                <option value="Other Charges">Other Charges</option>
                <option value="Local Tax">Local Tax</option>
            </select>

            <label for="txtAddChargeBilling">Percentage (%): </label>
            <input type="number" id="txtAddChargePercentBilling" style="text-align: right;" />

            <label for="txtAddChargeBilling">Amount: </label>
            <input type="number" id="txtAddChargeBilling" style="text-align: right;" />

            <label for="txtChargeRemarksBilling">Remarks: </label>
            <textarea rows="3" id="txtChargeRemarksBilling"></textarea>
            <br />
            <button id="btnAddChargeBilling">Add</button>
            <br />
            <hr />
            <br />
            <table style="width:100%">
                <thead>
                    <tr style="background-color:ButtonFace;">
                        <th style="text-align: center">Type</th>
                        <th style="text-align: center">Amount</th>
                        <th style="text-align: center">Remarks</th>
                        <th></th>
                    </tr>

                </thead>

                <tbody id='tblChargesListBilling'></tbody>

            </table>

            <br />
            <hr />
            <div class="ui-grid-a">
                <div class="ui-block-a">
                    <div>
                        <h3>
                            Total Charges :
                        </h3>
                    </div>

                </div>
                <div class="ui-block-b">
                    <h3 id="lblBillTotalChargesBilling" style="text-align: right">
                        0.00
                    </h3>

                </div>
            </div>

        </div>

    </div>

    <div data-role="popup" id="popupDiscounts" class="ui-corner-all" style="padding: 20px; width: 400px; display:none;">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
        <center><h3>Discounts</h3></center>
        <div data-role="content">
            <div data-role="fieldcontain">
                <label for="selDiscountTypeBilling" class="select">Discount Type:</label>
                <select id="selDiscountTypeBilling">
                    <option value="">Please Select</option>
                </select>
            </div>

            <div style="display:none;"><input type="number" id="txtDiscountPercentCeilingBilling" readonly style="text-align: right;" /></div>

            <label for="txtDiscountPercentBilling">Percent Discount (%): </label>
            <input type="text" id="txtDiscountPercentBilling" style="text-align: right;" />

            <label for="txtDiscountAmountBilling">Discount Amount: </label>
            <input type="text" id="txtDiscountAmountBilling" style="text-align: right;" />

            <label for="txtDiscountRemarksBilling">Remarks: </label>
            <textarea rows="3" id="txtDiscountRemarksBilling"></textarea>


            <div id="divAddCustomerDetailsBilling" style="display:none;">
                <div id="divSCPWDDiscountBilling" style="display:none;">

                    <label for="txtSalesPayNoOfSCBilling">Number of Senior Citizens: <span style="color: #FF0000;">*</span></label>
                    <input type="number" id="txtSalesPayNoOfSCBilling" value="1" style="text-align: right;" />


                    <label for="txtSalesPayNoOfDinersBilling">Number of People (including the Senior Citizen): <span style="color: #FF0000;">*</span></label>
                    <input type="number" id="txtSalesPayNoOfDinersBilling" value="1" style="text-align: right;" />

                </div>
                <div>
                    <ul data-role="listview" id="lstCustomerContainerBilling" data-inset="true">
                        <li data-role="list-divider">List of Senior Citizens / PWD Availing the Discount</li>
                        <li>
                            <div data-role="fieldcontain"><a id="btnAddNewCustomerBilling" data-role="button" data-icon="plus" data-inline="true">Add New Senior Citizen / PWD Customer</a></div>

                            <form id="frmCustomerSearchBilling">
                                <div><input id="txtCustomerSearchBilling" type="search" value="" placeholder="Type Customer Name or Identification Number" /></div>
                            </form>

                            <div>
                                <ul data-role="listview" id="lstSalesPayCustomerNameBilling" data-inset="true"></ul>
                            </div>
                        </li>
                    </ul>
                </div>


            </div>
            @*         <br />

                <button id="btnApplyDiscountBilling">Apply Discount</button>*@
        </div>

    </div>

</div>
 <div data-role="dialog" id="dialogPrintConfirmationBill" class="ui-corner-all" data-dismissible="false" style="display:none;">
    <div data-role="content">
        <h2>Print Confirmation</h2>
        Your transaction has been saved. Were you able to successfully print out a bill?

        <br />
        <div style="text-align: center">
            <a id="btnPrintYesBill" data-role="button">
                <h3>
                    Yes
                </h3>
            </a>
        </div>
        <div style="text-align: center">
            <a id="btnPrintNoBill" data-role="button">
                <h3>
                    No, it didn't print
                </h3>
            </a>
        </div>
    </div>

</div>

<div data-role="dialog" id="dialogPrintConfirmationDispatchSlip" class="ui-corner-all" data-dismissible="false" style="display:none;">
    <div data-role="content">
        <h2>Print Confirmation</h2>
        Your transaction has been saved. Were you able to successfully print out a dispatch slip?

        <br />
        <div style="text-align: center">
            <a id="btnPrintYesDispatchSlip" data-role="button">
                <h3>
                    Yes
                </h3>
            </a>
        </div>
        <div style="text-align: center">
            <a id="btnPrintNoDispatchSlip" data-role="button">
                <h3>
                    No, it didn't print
                </h3>
            </a>
        </div>
    </div>

</div>

<div id="pageDiagnosePrinter" data-role="page" style="display:none;">
    <div data-role="header">
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
        <h1>
            Diagnose Printer
        </h1>
    </div>
    <div data-role="content" style="padding-top: 40px">
        <h3>You seem to have trouble printing</h3>
        Please make sure that:
        <ol>
            <li>Your printer is powered on.</li>
            <li>There is enough paper (and/or ink) on your printer.</li>
            <li>The printer cable is properly connected.</li>
            <li>If your printer is attached to a Wi-Fi router via LAN cable, please check the back of your printer for a lit LAN indicator light. If the indicator light is not lit, then your cable is not properly connected. Please re-attach LAN cable until the LAN indicator light at the back of your printer is on. Also, please make sure that your router’s LAN indicator light is properly lit to indicate a good connection.</li>
            <li>Reset your device if all else fails.</li>
        </ol>

        <br />
        <div style="text-align: center">
            <a id="btnReprint" data-role="button">
                <h3>Try to reprint again</h3>
            </a>
        </div>
        <div style="text-align: center">
            <a id="btnPrintOther" data-role="button">
                <h3>Print on another printer</h3>
            </a>
        </div>

        <br />

    </div>
</div>

<div id="pageOtherPrinter" data-role="page" style="display:none;">
    <div data-role="header">
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
        <h1>
            Print on Another Printer
        </h1>
    </div>
    <div data-role="content" style="padding-top: 40px">
        <h3>Please select a printer you want to use</h3>

        <select id="selOtherPrinter"></select>

        <h3>... or define a printer</h3>

        <div data-role="fieldcontain">
            <label for="txtNewPrinterName">Printer Name: <span style="color: #FF0000;">*</span></label>
            <input type="text" id="txtNewPrinterName" maxlength="50" />

        </div>
        <div data-role="fieldcontain">
            <label for="txtNewPrinterIP">Printer's IP Address: <span style="color: #FF0000;">*</span> <a href="#" id="lnkInstruction">?</a></label>
            <input type="text" id="txtNewPrinterIP" maxlength="15" />

        </div>
        <br />
        <div style="text-align: center">
            <a id="btnSavePrint" data-role="button">
                <h3>Save and Print</h3>
            </a>
        </div>

        <br />
    </div>

    <div data-role="popup" id="popupIPAddressInstruction" class="ui-corner-all" style=" display: none;">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>

        <div data-role="content">
            <h2>Instructions on how to know your printer's IP address</h2>
            <h3>For Epson Thermal Receipt Printers</h3>
            <ol>
                <li>Turn off printer.</li>
                <li>Press the “Feed” button while printer is off.</li>
                <li>Turn on the printer while still pressing on the “Feed” button.</li>
                <li>Do not release the “Feed” button until printer prints out IP details.</li>
                <li>Use the IP address details to define printer on the previous page.</li>
            </ol>

            <br />
        </div>

    </div>


</div>

 <div data-role="dialog" id="dialogPrintConfirmation" class="ui-corner-all" data-dismissible="false" style="display:none;">
    <div data-role="content">
        <h2>Print Confirmation</h2>
        Your transaction has been saved. Were you able to successfully print out a receipt?

        <br />
        <div style="text-align: center">
            <a id="btnPrintYes" data-role="button">
                <h3>
                    Yes
                </h3>
            </a>
        </div>
        <div style="text-align: center">
            <a id="btnPrintNo" data-role="button">
                <h3>
                    No, it didn't print
                </h3>
            </a>
        </div>
    </div>

</div>

<div id="pageViewTransactions" data-role="page" style="display:none;">
    <div data-role="header" id="divHeaderTransPage" data-position="fixed" style="background: none white; border: none">
        <div class="ui-grid-b">
            <div class="ui-block-a">
                <div id="btnHomeTransDetails" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                    <div><img src="/Content/images/home.png" style="width:27px;" /></div>
                    <span style="font-size:13px;">Home</span>
                </div>
            </div>
            <div class="ui-block-b">
                <div id="btnRecordLookup" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                    <div><img src="/Content/images/RecordLookup.png" style="width:20px;" /></div>
                    <span style="font-size:13px;">Search Record</span>
                </div>
            </div>
            <div class="ui-block-c">
                <div id="btnNewTransaction" style="margin:0px; height: 80px;" data-role="button" data-theme="green" data-corners="false">
                    <div><img src="/Content/images/Update_Cart.png" style="width:30px;" /></div>
                    <span style="font-size:13px;">New Transaction</span>
                </div>
            </div>
        </div>

        <div>
            <div style="margin: 0px;" data-role="button" data-theme="b" data-corners="false">
                <div class="lblPosBranch" style="float:left; text-overflow:ellipsis; overflow:hidden; width:40%; text-align:left; white-space:nowrap;">
                </div>
                <div style="float:right">
                    <span id="clockTransactionDetails"></span>
                </div>
                <div style="clear:both"></div>
            </div>
        </div>
    </div>
    <div data-role="content" id="divContentTransPage">
        @*<div class="custom-ui-label">
                <div>PO No.:</div>
                <div>1234567</div>
            </div>*@

        <div style="text-align:center;">
            <h3 id="txtROPONo">PO No.: -</h3>
        </div>

        <div class="custom-ui-label">
            <div>Date and Time:</div>
            <div id="txtRODatePurchase">-</div>
            <div style="clear:both;"></div>
        </div>

        <div class="custom-ui-label">
            <div>Customer:</div>
            <div id="txtROCustomer">-</div>
            <div style="clear:both;"></div>
        </div>

        @*Order Branch *@

        <div class="custom-ui-label">
            <div>Status:</div>
            <div id="txtROStatus">-</div>
            <div style="clear:both;"></div>
        </div>

        <div class="custom-ui-label">
            <div>Item Sub Total:</div>
            <div id="txtSubTotal" style="font-weight:bold;">-</div>
            <div style="clear:both;"></div>
        </div>

        <br />

        <ul id="lstROPOItems" data-role="listview" class="li-theme-c noBorderList custom-list custom-list-sm ui-listview">
            <li>
                <a id="">
                    <b>Cheese Burger</b>
                    <br />
                    <br />
                    1 &nbsp; &nbsp; Order &nbsp;&nbsp; 95.00 &nbsp;&nbsp; 95.00
                    <br />
                    Item Total: 95.00
                </a>
            </li>
        </ul>
    </div>
</div>


