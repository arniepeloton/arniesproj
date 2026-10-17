@Code
    ViewData("Title") = "Payment Details"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
@Section head
<link rel="stylesheet" href="/Content/flatui-css-custom.css" />
    <script>
        require([GetPathFormat('promise!/Scripts/sources/SalesPayments/SalesPayments.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryOrganizationBanks.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryCountries.js?crc=')
        ]);
    </script>
    <style>
        .ui-grid-a .ui-block-a {
            width: 75%;
        }

        .ui-grid-a .ui-block-b {
            width: 25%;
        }
    </style>
End Section
<div id="pageSalesPayment" data-role="page" style="display:none;">
    <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>
    <div data-role="content">

        @*@If Common.WebRequest.QueryString("IsMobile") <> "1" Then
                @<applet name="jzebra" code="jzebra.PrintApplet.class" archive="/Scripts/jzebra.jar" width="10" height="10">
                </applet>
            End If*@

        <div style="display: none;"><input type="text" readonly id="txtSalesPayCustomerID" /></div>

        <div class="ui-body">
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Sub Total:
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPaySubTotal" style="float: right; text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
            <div data-role="fieldcontain" id="rowVATPercent" style="display:none">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        Less : <label id="lblVATPercentValue"></label>% VAT
                    </div>
                    <div class="ui-block-b">
                        <label id="lblVATPercent" style="float: right; text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
            <div data-role="fieldcontain" id="rowAmountLessVAT" style="display:none">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Total Amount:
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblTotalAmount" style="float: right; text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Less : Discount Amount
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPayTotalDiscount" style="float: right; text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Add : Charges
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPayCharges" style="float: right; text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
            <div data-role="fieldcontain" id="divPaymentsTip" style="display:none">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Add : Tip
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPayTip" style="float: right; text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
            <div data-role="fieldcontain" id="divPaymentsOtherTax" style="display:none">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Add : Other Tax
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPayOtherTax" style="float: right; text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
            <div data-role="fieldcontain">
                @If Common.WebRequest.QueryString("IsMobile") <> "1" Then
                    @<div class="ui-grid-a">
                        <div class="ui-block-a">
                            <label>Amount Due:</label>
                        </div>

                        <div class="ui-block-b">
                            <br />
                            <label id="lblSalesPayAmountDue" style="float: right; text-align: right; font-size: xx-large;">0.00</label>
                        </div>
                    </div>
                Else
                    @<div>
                        <label>
                            Amount Due:
                        </label>
                    </div>
                    @<div>
                        <br />
                        <label id="lblSalesPayAmountDue" style="float: right; text-align: right; font-size: xx-large;">
                            0.00
                        </label>
                    </div>
                End If
            </div>
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Amount Tendered:
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPayTotalAmountTendered" style="float: right; text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Amount Lacking:
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPayAmountLacking" style="float: right;text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Change Amount:
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPayTotalChangeAmount" style="float: right;text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>

            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            SKU Count:
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="pSkuCount" style="float: right; text-align: right;">0</label>
                    </div>
                </div>
            </div>

            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Total Quantity:
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="pTotalQuantity" style="float: right; text-align: right;">0</label>
                    </div>
                </div>
            </div>

            @*
                <div data-role="fieldcontain">
                    <fieldset data-role="controlgroup" data-type="horizontal">
                        <legend>SKU Count:</legend>
                        <div data-role="controlgroup">
                            <span id="pSkuCount" />
                        </div>
                    </fieldset>
                </div>
                <div data-role="fieldcontain">
                    <fieldset data-role="controlgroup" data-type="horizontal">
                        <legend>Total Quantity:</legend>
                        <div data-role="controlgroup">
                            <span id="pTotalQuantity" />
                        </div>
                    </fieldset>
                </div>

            *@

        </div>
        
        <div data-role="collapsible" data-collapsed="false" data-theme="c" data-content-theme="d" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">
            <h2>
                <label>
                    Cash
                </label>
                <label id="lblSalesPayTotalCash" style="float: right; margin-right: -10px; text-align: right;">
                    0.00
                </label>
            </h2>

            <input type="number" step="any" id="txtSalesPaymentCash" placeholder="Cash Amount" style="text-align: right; font-size: xx-large;" />

        </div>
        <div data-role="collapsible" data-theme="c" data-content-theme="d" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">
            <h2>
                <label>
                    Check/ Gift Certificate
                </label>
                <label id="lblSalesPayTotalCheck" style="float: right; margin-right: -10px; text-align: right;">
                    0.00
                </label>
            </h2>
            <div data-role="fieldcontain">
                <a id="btnSalesPayAddCheck" data-role="button" data-icon="plus" data-inline="true">Add Check/ Gift Certificate</a>
            </div>
            <div data-role="fieldcontain" id="divSalesPayRecordCountCheck">
            </div>
            <div>
                <ul data-role="listview" id="lstSalesPayCheck" data-inset="true"></ul>
            </div>
        </div>
        <div data-role="collapsible" data-theme="c" data-content-theme="d" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">
            <h2>
                <label>
                    Debit Card
                </label>
                <label id="lblSalesPayTotalDebitCard" style="float: right; margin-right: -10px; text-align: right;">
                    0.00
                </label>
            </h2>
            <div data-role="fieldcontain">
                <a id="btnSalesPayAddDebitCard" data-role="button" data-icon="plus" data-inline="true">Add Debit Card</a>
            </div>
            <div data-role="fieldcontain" id="divSalesPayRecordCountDebitCard">
            </div>
            <div>
                <ul data-role="listview" id="lstSalesPayDebitCard" data-inset="true"></ul>
            </div>
        </div>
        <div data-role="collapsible" data-theme="c" data-content-theme="d" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">
            <h2>
                <label>
                    Credit Card
                </label>
                <label id="lblSalesPayTotalCreditCard" style="float: right; margin-right: -10px; text-align: right;">
                    0.00
                </label>
            </h2>
            <div data-role="fieldcontain">
                <a id="btnSalesPayAddCreditCard" data-role="button" data-icon="plus" data-inline="true">Add Credit Card</a>
            </div>
            <div data-role="fieldcontain" id="divSalesPayRecordCountCreditCard">
            </div>
            <div>
                <ul data-role="listview" id="lstSalesPayCreditCard" data-inset="true"></ul>
            </div>
        </div>
        <div data-role="collapsible" data-theme="c" data-content-theme="d" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">
            <h2>
                <label>
                    SRV
                </label>
                <label id="lblSalesPayTotalSRV" style="float: right; margin-right: -10px; text-align: right;">
                    0.00
                </label>
            </h2>
            <div data-role="fieldcontain">
                <a id="btnSalesPayAddSRV" data-role="button" data-icon="plus" data-inline="true">Add SRV</a>
            </div>
            <div data-role="fieldcontain" id="divSalesPayRecordCountCreditCard">
            </div>
            <div>
                <ul data-role="listview" id="lstSalesPaySRV" data-inset="true"></ul>
            </div>
        </div>
        <div data-role="collapsible" data-theme="c" data-content-theme="d" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true" style="display: none;">
            <h2>
                <label>
                    Gift Certificate
                </label>
                <label id="lblSalesPayTotalGiftCert" style="float: right; margin-right: -10px; text-align: right;">
                    0.00
                </label>
            </h2>
            <div data-role="fieldcontain">
                <a id="btnSalesPayAddGC" data-role="button" data-icon="plus" data-inline="true">Add Gift Certificate</a>
            </div>
            <div data-role="fieldcontain" id="divSalesPayRecordCountGC">
            </div>
            <div>
                <ul data-role="listview" id="lstSalesPayGiftCertificate" data-inset="true"></ul>
            </div>
        </div>
        <div id="divOnCredit" style="display:none" data-role="collapsible" data-theme="c" data-content-theme="d" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">
            <h2>
                <label>
                    On Credit
                </label>
                <!--<label id="lblSalesPayTotalOtherType" style="float: right; margin-right: -10px; text-align: right;">
                0.00
            </label>-->
            </h2>
            <div data-role="fieldcontain">
                <label for="txtOnCredit" style="font-weight:normal">
                    On Credit:
                </label>
                <input id="txtOnCredit" type="number" step="any">
            </div>
            <div data-role="fieldcontain">
                <label for="txtOnCreditRemarks" style="font-weight:normal">
                    Remarks:
                </label>
                <textarea id="txtOnCreditRemarks"> </textarea>
            </div>
        </div>

        <div data-role="collapsible" data-theme="c" data-content-theme="d" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">
            <h2>
                <label>
                    Other Payment Type
                </label>
                <label id="lblSalesPayTotalOtherType" style="float: right; margin-right: -10px; text-align: right;">
                    0.00
                </label>
            </h2>
            <div data-role="fieldcontain">
                <a id="btnSalesPayAddOtherType" data-role="button" data-icon="plus" data-inline="true">Add Other Payment Type</a>
            </div>
            <div data-role="fieldcontain" id="divSalesPayRecordCountOther">
            </div>
            <div>
                <ul data-role="listview" id="lstSalesPayOtherType" data-inset="true"></ul>
            </div>
        </div>
        <div data-role="collapsible" data-theme="c" data-content-theme="d" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true" style="display: none;">
            <h2>
                <label>
                    Credit Terms
                </label>
                <label id="lblSalesPayTotalCreditTerms" style="float: right; margin-right: -10px; text-align: right;">0.00</label>
            </h2>
            <div data-role="fieldcontain">
                <input type="text" readonly style="display: none;" id="txtSalesPayCreditID" />
            </div>

            <div style="display: none;"><input type="text" readonly id="txtSalesPayCreditTermCustomerID" /></div>
            <div data-role="fieldcontain">
                <label for="btnSalesPayCreditTermCustomer">Customer: <span style="color: #FF0000;">*</span></label>
                <button id="btnSalesPayCreditTermCustomer" data-role="button">Please Select</button>

            </div>
            <div data-role="fieldcontain">
                <label for="txtSalesPayCreditTermRemainingCredit">Remaining Credit:</label>
                <input type="number" readonly id="txtSalesPayCreditTermRemainingCredit" />

            </div>
            <div data-role="fieldcontain">
                <label for="txtSalesPayCreditTermTenderedAmount">Tendered Amount: <span style="color: #FF0000;">*</span></label>
                <input type="number" id="txtSalesPayCreditTermTenderedAmount" />

            </div>
        </div>
        <div data-role="collapsible" data-collapsed="false" data-theme="c" data-content-theme="d" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">
            <h2>
                <label>Billing Info</label>
            </h2>
            <div data-role="fieldcontain">
                <input type="checkbox" id="chkForGovReport">
                <label for="chkForGovReport" style="font-weight:normal">
                    <h3>Print a Named Receipt</h3>
                    <p style="font-weight: normal; margin-top: -15px;">
                        Include customer's a.) Name, b.) TIN c.) Billing Address in sales invoice for government reporting.
                    </p>
                </label>
            </div>
        </div>
        <div data-role="collapsible" data-theme="c" data-content-theme="d" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">
            <h2>
                <label>Delivery Info</label>
            </h2>

            <div data-role="fieldcontain">
                <input type="checkbox" id="chkPrintDeliveryReceipt">
                <label for="chkPrintDeliveryReceipt" style="font-weight:normal">
                    <h3>Print Delivery Receipt</h3>
                    @*<p style="font-weight: normal; margin-top: -15px;">
                Include customer's a.) Name, b.) TIN c.) Billing Address in sales invoice for government reporting.
            </p>*@
                </label>
            </div>

            <div data-role="fieldcontain">
                <label for="txtOrgCustomer">Customer Account:</label>
                <input type="text" readonly id="txtOrgCustomer" />
            </div>

            <div style="display:none;"><input type="text" readonly id="txtOrderedByID" /></div>
            <div class="custom-ui-label">
                <div>Ordered By: <span style="color: #FF0000;">*</span></div>
                <div><button id="btnOrderBy" data-role="button">Please Select</button></div>
            </div>

            <div style="display:none;"><input type="text" readonly id="txtBillToID" /></div>
            <div class="custom-ui-label">
                <div>Bill To: <span style="color: #FF0000;">*</span></div>
                <div><button id="btnBillTo" data-role="button">Please Select</button></div>
            </div>

            <div style="display:none;"><input type="text" readonly id="txtShipToID" /></div>
            <div class="custom-ui-label">
                <div>Ship To: <span style="color: #FF0000;">*</span></div>
                <div><button id="btnShipTo" data-role="button">Please Select</button></div>
            </div>

            <div data-role="fieldcontain">
                <label for="txtDeliveryInfoTelNo">Tel. No.: </label>
                <input type="text" readonly id="txtDeliveryInfoTelNo" />
            </div>

            <div data-role="fieldcontain">
                <label for="txtDeliveryInfoFaxNo">Fax No.: </label>
                <input type="text" readonly id="txtDeliveryInfoFaxNo" />

            </div>

            <div data-role="fieldcontain">
                <label for="txtDeliveryInfoMobileNo">Mobile No.: </label>
                <input type="text" readonly id="txtDeliveryInfoMobileNo" />

            </div>

            <div data-role="fieldcontain">
                <label for="txtDeliveryInfoEmail">Email : </label>
                <input type="text" readonly id="txtDeliveryInfoEmail" />

            </div>

            <div id="divDeliveryInfoDelMode" data-role="fieldcontain">
                <label for="selDeliveryMode" >Delivery Mode:</label>
                <select id="selDeliveryMode">
                    <option value="">Please Select</option>
                    <option value="Deliver (Downtown)">Deliver (Downtown)</option>
                    <option value="Deliver (Out Of Town)">Deliver (Out Of Town)</option>
                    <option value="PickUp">Pick Up</option>
                    <option value="Dine In">Dine In</option>
                    <option value="Take Out">Take Out</option>
                </select>
            </div>

            <div id="divDeliverDownTown" style="display:none;">

                <fieldset data-role="controlgroup" data-type="horizontal">
                    <input type="radio" id="rdoRush" name="DownTownOption" value="1" />
                    <label for="rdoRush">Rush</label>
                    <input type="radio" id="rdoNotRush" name="DownTownOption" value="0" />
                    <label for="rdoNotRush">Not Rush</label>
                </fieldset>

                <div id="divRush" data-role="fieldcontain" style="display:none;">
                    <div data-role="fieldcontain">
                        <label for="txtDeliverWithIn">Deliver within </label>
                        <input type="number" id="txtDeliverWithIn" />

                    </div>

                    <div data-role="fieldcontain">
                        <fieldset data-role="controlgroup">
                            <div>
                                <select id="selHourMinute">
                                    <option value="">Hour or Minute</option>
                                    <option value="Working Hours">Working Hours</option>
                                    <option value="Working Minutes">Working Minutes</option>
                                </select>
                            </div>
                        </fieldset>
                    </div>
                </div>

                <div id="divNotRush" data-role="fieldcontain" style="display:none;">
                    <fieldset data-role="controlgroup">
                        <div data-role="fieldcontain">
                            <label>Maximum service time : 8 working hour(s)</label>
                        </div>
                    </fieldset>
                </div>

            </div>

            <div id="divDeliverOutOfTown" style="display:none;">

                <fieldset data-role="controlgroup" data-type="horizontal">
                    <input type="radio" id="rdoNormalRoute" name="OutOfTownOption" value="1" />
                    <label for="rdoNormalRoute">Normal Route</label>
                    <input type="radio" id="rdoBus" name="OutOfTownOption" value="0" />
                    <label for="rdoBus">Bus</label>
                </fieldset>

                <div id="divBus" data-role="fieldcontain" style="display:none;">
                    <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
                        <div data-role="fieldcontain">
                            <label id="lblOutOfTownBus">Maximum service time : 2 hour(s)</label>
                        </div>
                    </fieldset>
                </div>

            </div>

            <div id="divPickup" style="display:none;">

                <div id="divPickUpType">
                    <fieldset data-role="controlgroup" data-type="horizontal">
                        <input type="radio" id="rdoCustomerIsWaiting" name="PickUpOption" value="1" />
                        <label for="rdoCustomerIsWaiting">Customer is waiting</label>
                        <input type="radio" id="rdoDateAndTime" name="PickUpOption" value="0" />
                        <label for="rdoDateAndTime">Pick Up</label>
                    </fieldset>
                </div>

                <div style="display:none;"><input type="text" readonly id="txtPickUpLocationID" /></div>
                <div class="custom-ui-label">
                    <div>Pick Up Location: <span style="color: #FF0000;">*</span></div>
                    <div><button id="btnPickLocation" data-role="button">Please Select</button></div>
                </div>

                <div id="divCustomerIsWaiting" data-role="fieldcontain" style="display:none;">
                    <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
                        <div data-role="fieldcontain">
                            <label id="lblPickUpCustomerIsWaiting">Maximum service time : 30 Minute(s)</label>
                        </div>
                    </fieldset>
                </div>

                <div id="divDateAndTime" data-role="fieldcontain" style="display:none;">
                    <div class="custom-ui-label">
                        <div>Pick Up Date and Time: <span style="color: #FF0000;">*</span></div>
                        <div>
                            <fieldset data-role="controlgroup" data-type="horizontal">
                                <select id="selPickUpMonth">
                                    <option value="">MMM</option>
                                </select>
                                <select id="selPickUpDay">
                                    <option value="">DD</option>
                                </select>
                                <select id="selPickUpYear">
                                    <option value="">YYYY</option>
                                </select>
                            </fieldset>
                            <br />
                            <fieldset data-role="controlgroup" data-type="horizontal">
                                <select id="selPickUpHour">
                                    <option value="">HR </option>
                                </select>
                                <select id="selPickUpMin">
                                    <option value="">MN</option>
                                </select>
                                <select id="selPickUpAMPM">
                                    <option value="">AM/PM</option>
                                    <option value="AM">AM</option>
                                    <option value="PM">PM</option>
                                </select>
                            </fieldset>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div data-role="collapsible" data-theme="c" data-content-theme="d" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">
            <h2>
                <label>Discounts</label>
            </h2>
            <div data-role="fieldcontain">
                <label for="selDiscountType">Discount Type:</label>
                <select id="selDiscountType">
                    <option value="">Please Select</option>
                </select>
            </div>

            <div style="display:none;"><input type="number" id="txtDiscountPercentCeiling" readonly style="text-align: right;" /></div>
            <div data-role="fieldcontain">
                <label for="txtDiscountPercent">Effective Discount (%): </label>
                <input type="text" id="txtDiscountPercent" style="text-align: right;" />
            </div>
            <div data-role="fieldcontain">
                <label for="txtDiscountAmount">Discount Amount: </label>
                <input type="text" id="txtDiscountAmount" style="text-align: right;" />
            </div>
            <div data-role="fieldcontain">
                <label for="txtDiscountRemarks">Remarks: </label>
                <input type="text" id="txtDiscountRemarks" />
            </div>


            <div id="divAddCustomerDetails" style="display:none;">
                <div id="divSCPWDDiscount" style="display:none;">
                    <div data-role="fieldcontain">
                        <label for="txtSalesPayNoOfSC">Number of Senior Citizens / PWD: <span style="color: #FF0000;">*</span></label>
                        @*<input type="number" id="txtSalesPayNoOfSC" value="1" style="text-align: right;" />*@
                        <select id="txtSalesPayNoOfSC">1</select>
                    </div>
                    <div data-role="fieldcontain">
                        <label for="txtSalesPayNoOfDiners">Number of People (including the Senior Citizen/ PWD): <span style="color: #FF0000;">*</span></label>
                        @*<input type="number" id="txtSalesPayNoOfDiners" value="1" style="text-align: right;" />*@
                        <select id="txtSalesPayNoOfDiners">1</select>
                    </div>
                </div>
                <div>
                    <ul data-role="listview" id="lstCustomerContainer" data-inset="true">
                        <li data-role="list-divider">List of Senior Citizens / PWD Availing the Discount</li>
                        <li>
                            <div data-role="fieldcontain"><a id="btnAddNewCustomer" data-role="button" data-icon="plus" data-inline="true">Add New Senior Citizen / PWD Customer</a></div>

                            <form id="frmCustomerSearch">
                                <div><input id="txtCustomerSearch" type="search" value="" placeholder="Type Customer Name or Identification Number" /></div>
                            </form>

                            <div>
                                <ul data-role="listview" id="lstSalesPayCustomerName" data-inset="true"></ul>
                            </div>
                        </li>
                    </ul>
                </div>

            </div>
        </div>

        <div data-role="collapsible" data-theme="c" data-content-theme="d" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">
            <h2>
                <label>Charges</label>
            </h2>
            <div data-role="content">
                <div data-role="fieldcontain">
                    <label for="selChargeType">Charge Type: </label>
                    <select id="selChargeType">
                        <option value="Service Charge">Service Charge</option>
                        <option value="Delivery Charge">Delivery Charge</option>
                        <option value="Other Charges">Other Charges</option>
                        <option value="Local Tax">Local Tax</option>
                    </select>
                </div>
                
                <div data-role="fieldcontain">
                    <label for="txtAddChargePercent">Percentage (%): </label>
                    <input type="number" id="txtAddChargePercent" style="text-align: right;" />
                </div>                    

                <div data-role="fieldcontain">
                    <label for="txtAddCharge">Amount: </label>
                    <input type="number" id="txtAddCharge" style="text-align: right;" />
                </div>
                    
                <div class="custom-ui-label">
                    <div>Remarks:</div>
                    <div>
                        <textarea rows="3" id="txtChargeRemarks"></textarea>
                        <button id="btnAddCharge">Add</button>
                    </div>
                </div>
                    
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

                    <tbody id='tblChargesList'></tbody>

                </table>

                <br />
                <hr />
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <div>
                            <h3>
                                Total Charges
                            </h3>
                        </div>

                    </div>
                    <div class="ui-block-b">
                        <h3 id="lblBillTotalCharges" style="text-align: right">
                            0.00
                        </h3>

                    </div>
                </div>

            </div>
        </div>
        <div id="divSalesPaymentSalesRep">
            <div style="text-align: center">
                <a id="butAddSalesRepresentatives" data-role="button">
                    <h3>Assign Sales Representatives</h3>
                    <p style="font-weight: normal; margin-top: -15px;">
                        Assign sales representatives for this transaction
                    </p>
                </a>
            </div>
            <div id="divSalesPaymentSalesRepList">

            </div>
            <ul id="lstSalesRep" data-role="listview" data-inset="true">
                
            </ul>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnSalesPaymentDone" data-role="button" data-theme="e">
                    <h3>
                        Print Receipt
                    </h3>
                </a>
            </div>
            <div id="divPrintReceiptOrderSlip" style="text-align: center; display:none">
                <a id="butPrintReceiptOrderSlip" data-role="button" data-theme="e">
                    <h3>
                        Print Receipt / Order Slip
                    </h3>
                </a>
            </div>
            <div style="text-align: center">
                <a id="btnSalesPaymentEmail" data-role="button" data-theme="e">
                    <h3>
                        Email Receipt
                    </h3>
                </a>
            </div>
            <div style="text-align: center">
                <a id="btnSalesPaymentPrintEmail" data-role="button" data-theme="e">
                    <h3>
                        Print and Email Receipt
                    </h3>
                </a>
            </div>
            <div id="divSaveReceipt" style="text-align: center; display:none">
                <a id="butSaveReceipt" data-role="button" data-theme="e">
                    <h3>
                        Save Receipt
                    </h3>
                </a>
            </div>
            <div id="divPrintBilling" style="text-align: center; display:none">
                <a id="butPrintBilling" data-role="button" data-theme="e">
                    <h3>
                        Print Billing
                    </h3>
                </a>
            </div>
            <div id="divPrintOrderSlip" style="text-align: center; display:none">
                <a id="butPrintOrderSlip" data-role="button" data-theme="e">
                    <h3>
                        Print Order Slip
                    </h3>
                </a>
            </div>
            <div id="divPrintOrderSlipAndBilling" style="text-align: center; display:none">
                <a id="butPrintOrderSlipAndBilling" data-role="button" data-theme="e">
                    <h3>
                        Print Order Slip and Billing
                    </h3>
                </a>
            </div>
            <div style="text-align:center;">
                <a id="btnDeliveryInfoPriceQoute" data-role="button" data-theme="e">
                    <h3>Quote Prices</h3>
                    <p style="font-weight:normal; margin-top:-15px;">Save Items for Checkout Later</p>
                </a>
            </div>
            <div style="text-align: center">
                <a id="btnNewCart" data-role="button">
                    <h3>
                        New Cart
                    </h3>
                </a>
            </div>
            <div style="text-align: center">
                <a id="btnSalesPaymentBack" data-role="button">
                    <h3>
                        Back
                    </h3>
                </a>
            </div>
        </div>



    </div>




</div>

<div id="pageSalesPayCheck" data-role="page">
    <div data-role="header">
        <h1>
            Check/ Gift Certificate Payment
        </h1>
    </div>
    <div data-role="content">
        <div data-role="fieldcontain" style="display: none;">
            <input type="text" readonly id="txtSalesPayCheckID" />
        </div>

        <div class="ui-body ui-body-b">
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Sub Total:
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPayCheckSubTotal" style="float: right; text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Less : Discount Amount
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPayCheckTotalDiscount" style="float: right; text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Add : Charges
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPayCheckCharges" style="float: right; text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
            <div data-role="fieldcontain">
                @If Common.WebRequest.QueryString("IsMobile") <> "1" Then
                    @<div class="ui-grid-a">
                        <div class="ui-block-a">
                            <label>Amount Due:</label>
                        </div>
                        <div class="ui-block-b">
                            <br />
                            <label id="lblSalesPayCheckAmountDue" style="float: right; text-align: right; font-size: xx-large;">0.00</label>
                        </div>
                    </div>
                Else
                    @<div>
                        <label>
                            Amount Due:
                        </label>
                    </div>
                    @<div>
                        <label id="lblSalesPayCheckAmountDue" style="float: right; text-align: right; font-size: xx-large;">
                            0.00
                        </label>
                    </div>
                End If
            </div>
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Amount Tendered:
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPayCheckTotalAmountTendered" style="float: right; text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Amount Lacking:
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPayCheckAmountLacking" style="float: right; text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Change Amount:
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPayCheckTotalChangeAmount" style="float: right; text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
        </div>

        <div style="display: none;"><input type="text" readonly id="txtSalesPayCheckBankID" /></div>

        <div class="divTable" style="width:auto; font-size:inherit;">
            <div class="divTableRow">
                <div class="divTableCell txt-middle">Options:</div>
                <div class="divTableCell">
                    <div class="custom-radio">
                        <label class="radio-only">
                            <input type="radio" data-iconpos="notext" name="radOpPayCheck" id="radOpCheck" value="check" />
                        </label>
                        <div>Bank Check</div>
                    </div>
                </div>
                <div class="divTableCell">
                    <div class="custom-radio">
                        <label class="radio-only">
                            <input type="radio" data-iconpos="notext" name="radOpPayCheck" id="radOpGC" value="gc" />
                        </label>
                        <div>Gift Check/ Certificate</div>
                    </div>
                </div>
            </div>
        </div>

        <div id="divForCheck" style="display:none;">
            <div class="custom-ui-label">
                <div>Bank/ Issuing Organization: <span style="color: #FF0000;">*</span></div>
                <div>
                    <div id="divCheckBankButton">
                        <button id="btnSalesPayCheckBank" data-role="button">Please Select</button>
                    </div>

                    <div id="divCheckBankTextbox" style="display:none;">
                        <input type="text" id="txtSalesPayCheckBankName" />
                    </div>
                </div>
            </div>

            <div style="display: none;"><input type="text" readonly id="txtSalesPayCheckBankBranchID" /></div>
            <div data-role="fieldcontain">
                <label for="btnSalesPayCheckBankBranch">Bank/ Issuing Organization Branch: <span style="color: #FF0000;">*</span></label>
                <input type="text" id="txtSalesPayCheckBankBranch" style="display:none;" />

                <div id="divCheckBankBranchButton">
                    <button id="btnSalesPayCheckBankBranch" data-role="button">Please Select</button>

                </div>

            </div>

            <div data-role="fieldcontain">
                <label for="txtSalesPayCheckAccountName">Account Name (Issued To): <span style="color: #FF0000;">*</span></label>
                <input type="text" id="txtSalesPayCheckAccountName" />

            </div>

            <div data-role="fieldcontain">
                <label for="txtSalesPayCheckAccountNo">Account No.: <span style="color: #FF0000;">*</span></label>
                <input type="text" id="txtSalesPayCheckAccountNo" />

            </div>
        </div>            

            <div data-role="fieldcontain">
                <label for="txtSalesPayCheckNo"><span id="lblPayCheckNo"></span></label>
                <input type="text" id="txtSalesPayCheckNo" />

            </div>

        <div class="custom-ui-label">
            <div><span id="lblPayCheckDate"></span></div>
            <div>
                <fieldset data-role="controlgroup" data-type="horizontal">
                    <select id="selSalesPayCheckDateMonth">
                        <option value="">MMM</option>
                    </select>
                    <select id="selSalesPayCheckDateDay">
                        <option value="">DD</option>
                    </select>
                    <select id="selSalesPayCheckDateYear">
                        <option value="">YYYY</option>
                    </select>
                </fieldset>
            </div>
        </div>

            <div data-role="fieldcontain">
                <label for="txtSalesPayCheckAmount"><span id="lblPayCheckAmount"></span></label>
                <input type="number" id="txtSalesPayCheckAmount" />

            </div>
            <div>
                <br />
                <br />
                <div style="text-align: center">
                    <a id="btnSalesPayCheckSave" data-role="button">Ok</a>
                </div>
                <div style="text-align: center">
                    <a id="btnSalesPayCheckRemove" data-role="button">Remove</a>
                </div>
                <div style="text-align: center">
                    <a id="btnSalesPayCheckCancel" data-role="button">Cancel</a>
                </div>
            </div>
        </div>
</div>
<div id="pageSalesPayCreditCard" data-role="page">
    <div data-role="header">
        <h1>
            Credit Card Payment
        </h1>
    </div>
    <div data-role="content">
        <div data-role="fieldcontain" style="display: none;">
            <input type="text" readonly id="txtSalesPayCreditCardID" />
        </div>

        <div class="ui-body ui-body-b">
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Sub Total:
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPayCreditCardSubTotal" style="float: right; text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Less : Discount Amount
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPayCreditCardTotalDiscount" style="float: right; text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Add : Charges
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPayCreditCardCharges" style="float: right;text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
            <div data-role="fieldcontain">
                @If Common.WebRequest.QueryString("IsMobile") <> "1" Then
                    @<div class="ui-grid-a">
                        <div class="ui-block-a">
                            <label>Amount Due:</label>
                        </div>
                        <div class="ui-block-b">
                            <br />
                            <label id="lblSalesPayCreditCardAmountDue" style="float: right; text-align: right; font-size: xx-large;">0.00</label>
                        </div>
                    </div>
                Else
                    @<div>
                        <label>
                            Amount Due:
                        </label>
                    </div>
                    @<div>
                        <label id="lblSalesPayCreditCardAmountDue" style="float: right; text-align: right; font-size: xx-large;">
                            0.00
                        </label>
                    </div>
                End If
            </div>
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Amount Tendered:
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPayCreditCardTotalAmountTendered" style="float: right; text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Amount Lacking:
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPayCreditCardAmountLacking" style="float: right; text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Change Amount:
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPayCreditCardTotalChangeAmount" style="float: right; text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
        </div>

        <div class="custom-ui-label">
            <div>Credit Card: <span style="color: #FF0000;">*</span></div>
            <div>
                <select id="selSalesPayCreditCardType">
                    <option value="">Please Select</option>
                    <option value="Visa">Visa</option>
                    <option value="Master Card">Master Card</option>
                    <option value="UnionPay">UnionPay</option>
                    <option value="JCB">JCB</option>
                    <option value="Diners">Diners</option>
                    <option value="American Express">American Express</option>
                    <option value="Other Card">Other Card</option>
                </select>
            </div>
        </div>

        <div id="divCreditCardTypeOther" data-role="fieldcontain" style="display:none;">
            <label for="txtSalesPayCreditCardTypeOther">Other Card Type : <span style="color: #FF0000;">*</span></label>
            <input type="text" id="txtSalesPayCreditCardTypeOther" />

        </div>

        <div style="display: none;"><input type="text" readonly id="txtSalesPayCreditCardBankID" /></div>
        <div class="custom-ui-label">
            <div>Issuing Bank: <span style="color: #FF0000;">*</span></div>
            <div>
                <div id="divSalesPayCreditCardBankButton">
                    <button id="btnSalesPayCreditCardBank" data-role="button">Please Select</button>

                </div>
                <div id="divSalesPayCreditCardBankTextbox" style="display:none;">
                    <input type="text" id="txtSalesPayCreditCardBank" />

                </div>
            </div>
        </div>

        <div style="display: none;"><input type="text" readonly id="txtSalesPayCreditCardBankLocationID" /></div>
        <div class="custom-ui-label">
            <div>Bank Location (Country):</div>
            <div><button id="btnSalesPayCreditCardBankLocation" data-role="button">Please Select</button></div>
        </div>

        <div>
            <ul data-role="listview" id="lstSalesPayCardHolder" data-inset="true">
                <li data-role="list-divider">Card Holder</li>
                <li>
                    <div data-role="fieldcontain">
                        <label for="txtSalesPayCreditCardLastName">Last Name: <span style="color: #FF0000;">*</span></label>
                        <input type="text" id="txtSalesPayCreditCardLastName" />

                    </div>

                    <div data-role="fieldcontain">
                        <label for="txtSalesPayCreditCardFirstName">First Name: <span style="color: #FF0000;">*</span></label>
                        <input type="text" id="txtSalesPayCreditCardFirstName" />

                    </div>

                    <div data-role="fieldcontain">
                        <label for="txtSalesPayCreditCardMiddleName">Middle Name:</label>
                        <input type="text" id="txtSalesPayCreditCardMiddleName" />

                    </div>

                    <div data-role="fieldcontain">
                        <label for="selSalesPayPersonalSuffixName" >
                            Generational Suffix (Name):
                        </label>
                        <select id="selSalesPayPersonalSuffixName">
                            <option value="">Please Select</option>
                            <option value="Sr.">Senior (Sr.)</option>
                            <option value="Jr.">Junior (Jr.)</option>
                            <option value="II">II</option>
                            <option value="III">III</option>
                            <option value="IV">IV</option>
                            <option value="V">V</option>
                            <option value="VI">VI</option>
                            <option value="VII">VII</option>
                            <option value="VIII">VIII</option>
                            <option value="IX">IX</option>
                            <option value="X">X</option>
                        </select>
                    </div>
                </li>
            </ul>
        </div>

        <div data-role="fieldcontain">
            <label for="txtSalesPayCreditCardNo">Credit Card No.: <span style="color: #FF0000;">*</span>      <p>Please enter the last four(4) digit of the card only.</p></label>

            <input type="text" id="txtSalesPayCreditCardNo" maxlength="4" />

        </div>

        <div class="custom-ui-label">
            <div>Issuance Date:</div>
            <div>
                <fieldset data-role="controlgroup" data-type="horizontal">
                    <select id="selSalesPayCreditCardIssuanceDateMonth">
                        <option value="">MMM</option>
                    </select>
                    <select id="selSalesPayCreditCardIssuanceDateDay">
                        <option value="">DD</option>
                    </select>
                    <select id="selSalesPayCreditCardIssuanceDateYear">
                        <option value="">YYYY</option>
                    </select>
                </fieldset>
            </div>
        </div>

        <div class="custom-ui-label">
            <div>Expiration Date:</div>
            <div>
                <fieldset data-role="controlgroup" data-type="horizontal">
                    <select id="selSalesPayCreditCardExpirationDateMonth">
                        <option value="">MMM</option>
                    </select>
                    <select id="selSalesPayCreditCardExpirationDateDay">
                        <option value="">DD</option>
                    </select>
                    <select id="selSalesPayCreditCardExpirationDateYear">
                        <option value="">YYYY</option>
                    </select>
                </fieldset>
            </div>
        </div>

        <div data-role="fieldcontain">
            <label for="txtSalesPayCreditCardApprovalNo">Approval No.:</label>
            <input type="text" id="txtSalesPayCreditCardApprovalNo" />

        </div>

        <div data-role="fieldcontain">
            <label for="txtSalesPayCreditCardNoOfPayments">Number of Installment Payments:</label>
            <input type="number" id="txtSalesPayCreditCardNoOfPayments" />

        </div>

        <div data-role="fieldcontain">
            <label for="txtSalesPayCreditCardTenderedAmount">Amount: <span style="color: #FF0000;">*</span></label>
            <input type="number" id="txtSalesPayCreditCardTenderedAmount" maxlength="10" />

        </div>
        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnSalesPayCreditCardSave" data-role="button">Ok</a>
            </div>
            <div style="text-align: center">
                <a id="btnSalesPayCreditCardRemove" data-role="button">Remove</a>
            </div>
            <div style="text-align: center">
                <a id="btnSalesPayCreditCardCancel" data-role="button">Cancel</a>
            </div>
        </div>
    </div>
</div>
<div id="pageSalesPayDebitCard" data-role="page">
    <div data-role="header">
        <h1>
            Debit Card Payment
        </h1>
    </div>
    <div data-role="content">
        <div data-role="fieldcontain" style="display: none;">
            <input type="text" readonly id="txtSalesPayDebitCardID" />
        </div>

        <div class="ui-body ui-body-b">
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Sub Total:
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPayDebitCardSubTotal" style="float: right; text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Less : Discount Amount
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPayDebitCardTotalDiscount" style="float: right; text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Add : Charges
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPayDebitCardCharges" style="float: right; text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
            <div data-role="fieldcontain">
                @If Common.WebRequest.QueryString("IsMobile") <> "1" Then
                    @<div class="ui-grid-a">
                        <div class="ui-block-a">
                            <label>Amount Due:</label>
                        </div>
                        <div class="ui-block-b">
                            <br />
                            <label id="lblSalesPayDebitCardAmountDue" style="float: right; text-align: right; font-size: xx-large;">0.00</label>
                        </div>
                    </div>
                Else
                    @<div>
                        <label>
                            Amount Due:
                        </label>
                    </div>
                    @<div>
                        <label id="lblSalesPayDebitCardAmountDue" style="float: right; text-align: right; font-size: xx-large;">
                            0.00
                        </label>
                    </div>
                End If
            </div>
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Amount Tendered:
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPayDebitCardTotalAmountTendered" style="float: right; text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Amount Lacking:
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPayDebitCardAmountLacking" style="float: right; text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Change Amount:
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPayDebitCardTotalChangeAmount" style="float: right; text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
        </div>

        <div data-role="fieldcontain" style="display:none;">
            <label for="selSalesPayDebitCardType" >
                Debit Card: <span style="color: #FF0000;">*</span>
            </label>
            <select id="selSalesPayDebitCardType">
                <option value="">Please Select</option>
                <option value="Visa">Visa</option>
                <option value="Master Card">Master Card</option>
                <option value="JCB">JCB</option>
                <option value="Diners">Diners</option>
                <option value="American Express">American Express</option>
                <option value="Other Card">Other Card</option>
            </select>
        </div>

        <div style="display: none;"><input type="text" readonly id="txtSalesPayDebitCardBankID" /></div>
        <div class="custom-ui-label">
            <div>Issuing Bank: <span style="color: #FF0000;">*</span></div>
            <div>
                <div id="divSalesPayDebitCardBankButton">
                    <button id="btnSalesPayDebitCardBank" data-role="button">Please Select</button>

                </div>
                <div id="divSalesPayDebitCardBankTextbox" style="display:none;">
                    <input type="text" id="txtSalesPayDebitCardBank" />

                </div>
            </div>
        </div>

        <div style="display: none;"><input type="text" readonly id="txtSalesPayDebitCardBankLocationID" /></div>
        <div class="custom-ui-label">
            <div>Bank Location (Country):</div>
            <div><button id="btnSalesPayDebitCardBankLocation" data-role="button">Please Select</button></div>
        </div>

        <div>
            <ul data-role="listview" id="lstSalesPayCardHolder" data-inset="true">
                <li data-role="list-divider">Card Holder</li>
                <li>
                    <div data-role="fieldcontain">
                        <label for="txtSalesPayDebitCardLastName">Last Name: <span style="color: #FF0000;">*</span></label>
                        <input type="text" id="txtSalesPayDebitCardLastName" />

                    </div>

                    <div data-role="fieldcontain">
                        <label for="txtSalesPayDebitCardFirstName">First Name: <span style="color: #FF0000;">*</span></label>
                        <input type="text" id="txtSalesPayDebitCardFirstName" />

                    </div>

                    <div data-role="fieldcontain">
                        <label for="txtSalesPayDebitCardMiddleName">Middle Name:</label>
                        <input type="text" id="txtSalesPayDebitCardMiddleName" />

                    </div>

                    <div data-role="fieldcontain">
                        <label for="selSalesPayDebitPersonalSuffixName" >
                            Generational Suffix (Name):
                        </label>
                        <select id="selSalesPayDebitPersonalSuffixName">
                            <option value="">Please Select</option>
                            <option value="Sr.">Senior (Sr.)</option>
                            <option value="Jr.">Junior (Jr.)</option>
                            <option value="II">II</option>
                            <option value="III">III</option>
                            <option value="IV">IV</option>
                            <option value="V">V</option>
                            <option value="VI">VI</option>
                            <option value="VII">VII</option>
                            <option value="VIII">VIII</option>
                            <option value="IX">IX</option>
                            <option value="X">X</option>
                        </select>
                    </div>
                </li>
            </ul>
        </div>

        <div data-role="fieldcontain">
            <label for="txtSalesPayDebitCardNo">Debit Card No.: <span style="color: #FF0000;">*</span> <p>Please enter the last four(4) digit of the card only.</p></label>

            <input type="text" id="txtSalesPayDebitCardNo" maxlength="4" />

        </div>

        <div class="custom-ui-label">
            <div>Issuance Date:</div>
            <div>
                <fieldset data-role="controlgroup" data-type="horizontal">
                    <select id="selSalesPayDebitCardIssuanceDateMonth">
                        <option value="">MMM</option>
                    </select>
                    <select id="selSalesPayDebitCardIssuanceDateDay">
                        <option value="">DD</option>
                    </select>
                    <select id="selSalesPayDebitCardIssuanceDateYear">
                        <option value="">YYYY</option>
                    </select>
                </fieldset>
            </div>
        </div>

        <div class="custom-ui-label">
            <div>Expiration Date:</div>
            <div>
                <fieldset data-role="controlgroup" data-type="horizontal">
                    <select id="selSalesPayDebitCardExpirationDateMonth">
                        <option value="">MMM</option>
                    </select>
                    <select id="selSalesPayDebitCardExpirationDateDay">
                        <option value="">DD</option>
                    </select>
                    <select id="selSalesPayDebitCardExpirationDateYear">
                        <option value="">YYYY</option>
                    </select>
                </fieldset>
            </div>
        </div>

        <div data-role="fieldcontain">
            <label for="txtSalesPayDebitCardApprovalNo">Approval No.:</label>
            <input type="text" id="txtSalesPayDebitCardApprovalNo" />

        </div>

        <div data-role="fieldcontain">
            <label for="txtSalesPayDebitCardNoOfPayments">Number of Installment Payments:</label>
            <input type="number" id="txtSalesPayDebitCardNoOfPayments" />

        </div>

        <div data-role="fieldcontain">
            <label for="txtSalesPayDebitCardTenderedAmount">Amount: <span style="color: #FF0000;">*</span></label>
            <input type="number" id="txtSalesPayDebitCardTenderedAmount" maxlength="10" />

        </div>
        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnSalesPayDebitCardSave" data-role="button">Ok</a>
            </div>
            <div style="text-align: center">
                <a id="btnSalesPayDebitCardRemove" data-role="button">Remove</a>
            </div>
            <div style="text-align: center">
                <a id="btnSalesPayDebitCardCancel" data-role="button">Cancel</a>
            </div>
        </div>
    </div>
</div>
<div id="pageSalesPaySRV" data-role="page">
    <div data-role="header">
        <h1>
            Sales Return Voucher (SRV)
        </h1>
    </div>
    <div data-role="content">
        <div data-role="fieldcontain" style="display: none;">
            <input type="text" readonly id="txtSalesPayTransSRVID" />
        </div>
        <div class="ui-body ui-body-b">
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Sub Total:
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPaySRVSubTotal" style="float: right; text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Less : Discount Amount
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPaySRVTotalDiscount" style="float: right; text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Add : Charges
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPaySRVCharges" style="float: right; text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
            <div data-role="fieldcontain">
                @If Common.WebRequest.QueryString("IsMobile") <> "1" Then
                    @<div class="ui-grid-a">
                        <div class="ui-block-a">
                            <label>Amount Due:</label>
                        </div>
                        <div class="ui-block-b">
                            <br />
                            <label id="lblSalesPaySRVAmountDue" style="float: right; text-align: right; font-size: xx-large;">0.00</label>
                        </div>
                    </div>
                Else
                    @<div>
                        <label>
                            Amount Due:
                        </label>
                    </div>
                    @<div>
                        <label id="lblSalesPaySRVAmountDue" style="float: right; text-align: right; font-size: xx-large;">
                            0.00
                        </label>
                    </div>
                End If
            </div>
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Amount Tendered:
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPaySRVTotalAmountTendered" style="float: right; text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Amount Lacking:
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPaySRVAmountLacking" style="float: right; text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Change Amount:
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPaySRVTotalChangeAmount" style="float: right; text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
        </div>

        <div style="display: none;"><input type="text" readonly id="txtSalesPaySRVTransMemoID" /></div>
        <div class="custom-ui-label">
            <div>SRV No.: <span style="color: #FF0000;">*</span></div>
            <div>
                <input type="number" id="txtSalesPaySRVNo" maxlength="8" />
                <a id="btnSalesPayScanSRVNumber" data-role="button">Scan SRV</a>
            </div>
        </div>

        <div class="custom-ui-label">
            <div>PIN Code: <span style="color: #FF0000;">*</span></div>
            <div>
                <input type="password" id="txtSalesPaySRVPinCode" />
                <a id="btnSalesPayScanSRVPin" data-role="button">Scan Pin Code</a>
                <br />
                <a id="btnSalesPaySRVVerify" data-role="button">Verify SRV</a>
            </div>
        </div>

        <div data-role="fieldcontain">
            <label for="txtSalesPaySRVAmount">SRV Amount: <span style="color: #FF0000;">*</span></label>
            <input type="number" readonly id="txtSalesPaySRVAmount" />
        </div>

        <div data-role="fieldcontain">
            <label for="txtSalesPaySRVConsumedAmount">Amount: <span style="color: #FF0000;">*</span></label>
            <input type="number" readonly id="txtSalesPaySRVConsumedAmount" />
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnSalesPaySRVSave" data-role="button">Ok</a>
            </div>
            <div style="text-align: center">
                <a id="btnSalesPaySRVRemove" data-role="button">Remove</a>
            </div>
            <div style="text-align: center">
                <a id="btnSalesPaySRVCancel" data-role="button">Cancel</a>
            </div>
        </div>
    </div>
</div>

<div id="pageSalesPayGiftCert" data-role="page">
    <div data-role="header">
        <h1>
            Gift Certificate
        </h1>
    </div>
    <div data-role="content">

        <div data-role="fieldcontain" style="display: none;">
            <input type="text" readonly id="txtSalesPayGiftCertID" />
        </div>

        <div data-role="fieldcontain">
            <label for="txtSalesPayGiftCertIssuingOrg">Issuing Organization: <span style="color: #FF0000;">*</span></label>
            <input type="text" id="txtSalesPayGiftCertIssuingOrg" />
        </div>

        <div data-role="fieldcontain">
            <label for="txtSalesPayGiftCertIssuedTo">Issued To:</label>
            <input type="text" id="txtSalesPayGiftCertIssuedTo" />
        </div>

        <div data-role="fieldcontain">
            <label for="txtSalesPayGiftCertNo">Gift Certificate No.: <span style="color: #FF0000;">*</span></label>
            <input type="text" id="txtSalesPayGiftCertNo" />
        </div>

        <div data-role="fieldcontain">
            <label>Date of Issue:</label>
            <fieldset data-role="controlgroup" data-type="horizontal">
                <select id="selSalesPayGiftCertDateOfIssueMonth">
                    <option value="">MMM</option>
                </select>
                <select id="selSalesPayGiftCertDateOfIssueDay">
                    <option value="">DD</option>
                </select>
                <select id="selSalesPayGiftCertDateOfIssueYear">
                    <option value="">YYYY</option>
                </select>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <label>Expiration Date: <span style="color: #FF0000;">*</span></label>
            <fieldset data-role="controlgroup" data-type="horizontal">
                <select id="selSalesPayGiftCertExpirationDateMonth">
                    <option value="">MMM</option>
                </select>
                <select id="selSalesPayGiftCertExpirationDateDay">
                    <option value="">DD</option>
                </select>
                <select id="selSalesPayGiftCertExpirationDateYear">
                    <option value="">YYYY</option>
                </select>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <label for="txtSalesPayGiftCertAmount">Amount: <span style="color: #FF0000;">*</span></label>
            <fieldset data-role="controlgroup">
                <input type="number" id="txtSalesPayGiftCertAmount" />
            </fieldset>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnSalesPayGiftCertSave" data-role="button">Ok</a>
            </div>
            <div style="text-align: center">
                <a id="btnSalesPayGiftCertRemove" data-role="button">Remove</a>
            </div>
            <div style="text-align: center">
                <a id="btnSalesPayGiftCertCancel" data-role="button">Cancel</a>
            </div>
        </div>
    </div>
</div>

<div id="pageSalesPayOtherPaymentType" data-role="page">
    <div data-role="header">
        <h1>
            Other Payment Type
        </h1>
    </div>
    <div data-role="content">
        <div data-role="fieldcontain" style="display: none;">
            <input type="text" readonly id="txtSalesPayOtherPaymentTypeID" />
        </div>

        <div class="ui-body ui-body-b">
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Sub Total:
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPayOtherTypeSubTotal" style="float: right; text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Less : Discount Amount
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPayOtherTypeTotalDiscount" style="float: right; text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Add : Charges
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPayOtherTypeCharges" style="float: right; text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
            <div data-role="fieldcontain">
                @If Common.WebRequest.QueryString("IsMobile") <> "1" Then
                    @<div class="ui-grid-a">
                        <div class="ui-block-a">
                            <label>Amount Due:</label>
                        </div>
                        <div class="ui-block-b">
                            <br />
                            <label id="lblSalesPayOtherTypeAmountDue" style="float: right; text-align: right; font-size: xx-large;">0.00</label>
                        </div>
                    </div>
                Else
                    @<div>
                        <label>
                            Amount Due:
                        </label>
                    </div>
                    @<div>
                        <label id="lblSalesPayOtherTypeAmountDue" style="float: right; text-align: right; font-size: xx-large;">
                            0.00
                        </label>
                    </div>
                End If
            </div>
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Amount Tendered:
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPayOtherTypeTotalAmountTendered" style="float: right; text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Amount Lacking:
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPayOtherTypeAmountLacking" style="float: right; text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Change Amount:
                        </label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPayOtherTypeTotalChangeAmount" style="float: right; text-align: right;">
                            0.00
                        </label>
                    </div>
                </div>
            </div>
        </div>



        <div data-role="fieldcontain">
            <label for="selOtherPaymentType">Payment Type: <span style="color: #FF0000;">*</span></label>
            <select id="selOtherPaymentType">
                <option value="">Please select</option>
                <option value="AliPay">AliPay</option>
                <option value="Complimentary">Complimentary</option>
                <option value="Discount Voucher / On Line Deals">Discount Voucher / On Line Deals</option>
                <option value="E-Plus">E-Plus</option>
                <option value="FoodPanda">Food Panda</option>
                <option value="GCash">GCash</option>
                <option value="GrabFood">Grab Food</option>
                <option value="PayMaya">PayMaya</option>
                <option value="SMAC">SMAC</option>
                <option value="V-Plus">V Plus</option>

                @*<option value="">Please select</option> 
                <option value="Complimentary">Complimentary</option>
                <option value="Discount Voucher / On Line Deals">Discount Voucher / On Line Deals</option>
                <option value="E-Plus">E-Plus</option>
                <option value="GCash">GCash</option>
                <option value="SMAC">SMAC</option>
                <option value="V-Plus">V Plus</option>*@
                @*<option value="Room In">Room In</option>
                <option value="Send Bill">Send Bill</option>*@
            </select>
        </div>

        <div id="divForVSED" style="display:block;">
            <div data-role="fieldcontain">
                <label for="txtSalesPayOtherPaymentTypeCardNo">Card/ Reference No.: <span style="color: #FF0000;">*</span></label>
                <input type="text" id="txtSalesPayOtherPaymentTypeCardNo" />
            </div>

            <div data-role="fieldcontain">
                <label for="txtSalesPayOtherPaymentTypeCardHolder">Card/ Voucher Holder Name:</label>
                <input type="text" id="txtSalesPayOtherPaymentTypeCardHolder" />

            </div>

            <div data-role="fieldcontain">
                <label for="txtSalesPayOtherPaymentTypeApprovalNo">Payment Approval No.:</label>
                <input type="text" id="txtSalesPayOtherPaymentTypeApprovalNo" />
            </div>

            <div data-role="fieldcontain">
                <label for="txtSalesPayOtherPaymentTypeAmount">Payment Amount: <span style="color: #FF0000;">*</span></label>
                <input type="number" id="txtSalesPayOtherPaymentTypeAmount" />
            </div>

            <div data-role="fieldcontain">
                <label for="txtSalesPayOtherPaymentTypeIssuingOrg">Issuing Organization:</label>
                <input type="text" id="txtSalesPayOtherPaymentTypeIssuingOrg" />

            </div>
            
            <div data-role="fieldcontain">
                <label for="txtSalesPayOtherPaymentTypeAvailablePoints">Available Points:</label>
                <input type="text" id="txtSalesPayOtherPaymentTypeAvailablePoints" />
            </div>

            <div class="custom-ui-label">
                <div>Issuance Date:</div>
                <div>
                    <fieldset data-role="controlgroup" data-type="horizontal">
                        <select id="selSalesPayOtherPaymentTypeCardIssuanceDateMonth">
                            <option value="">MMM</option>
                        </select>
                        <select id="selSalesPayOtherPaymentTypeCardIssuanceDateDay">
                            <option value="">DD</option>
                        </select>
                        <select id="selSalesPayOtherPaymentTypeCardIssuanceDateYear">
                            <option value="">YYYY</option>
                        </select>
                    </fieldset>
                </div>
            </div>

            <div class="custom-ui-label">
                <div>Expiration Date:</div>
                <div>
                    <fieldset data-role="controlgroup" data-type="horizontal">
                        <select id="selSalesPayOtherPaymentTypeCardExpirationDateMonth">
                            <option value="">MMM</option>
                        </select>
                        <select id="selSalesPayOtherPaymentTypeCardExpirationDateDay">
                            <option value="">DD</option>
                        </select>
                        <select id="selSalesPayOtherPaymentTypeCardExpirationDateYear">
                            <option value="">YYYY</option>
                        </select>
                    </fieldset>
                </div>
            </div>
        </div>

        <div id="divForRoomIn" data-role="fieldcontain" style="display:none;">
            <label for="txtSalesPayOtherPaymentTypeRoomNo">Room No: <span style="color: #FF0000;">*</span></label>
            <input type="text" id="txtSalesPayOtherPaymentTypeRoomNo" />
        </div>

        <div id="divForRoomInSendBill" data-role="fieldcontain" style="display:none;">
            <label for="txtSalesPayOtherPaymentTypeRemarks">Remarks:</label>
            <input type="text" id="txtSalesPayOtherPaymentTypeRemarks" />
        </div>

        <div id="divForGCash" data-role="fieldcontain" style="display:none">
            <div class="custom-ui-label">
                <div></div>
                <div>
                    <button data-role="button" id="btnGCashPayBal">Auto Fill Payment Balance</button>
                </div>
            </div>
            <div data-role="fieldcontain" id="dvGcashReferenceNo">
                <label for="txtSalesPayOtherPaymentTypeGCashRefNo">GCash Reference Number: <span style="color: #FF0000;">*</span></label>
                <input type="number" id="txtSalesPayOtherPaymentTypeGCashRefNo" />
            </div>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnSalesPayOtherPaymentTypeSave" data-role="button">Ok</a>
            </div>
            <div style="text-align: center">
                <a id="btnSalesPayOtherPaymentTypeRemove" data-role="button">Remove</a>
            </div>
            <div style="text-align: center">
                <a id="btnSalesPayOtherPaymentTypeCancel" data-role="button">Cancel</a>
            </div>
        </div>
    </div>
</div>

<div id="divSalesPaymentPromptEmail" data-role="dialog">
    <div data-role="header" data-theme="d">
        <h1>Email Address</h1>
    </div>

    <div data-role="content" data-theme="c">

        @*<div><label>Please check the email address(es) that you want to send an electronic invoice to.</label></div>

        <div id="divSalesPaymentsEmail" data-role="fieldcontain">
            <div id="divSalesPaymentEmailNone" style="display:none; font-weight:bolder; color:Red;"><label>No Available Email</label></div>
            <fieldset id="fldsetSalesPayEmail" data-role="controlgroup"></fieldset>
        </div>*@

        <fieldset data-role="controlgroup">
            @*<div><label for="txtSalesPayEmailAddress">Other email address:</label></div>*@
            <div><label for="txtSalesPayEmailAddress">Enter email address:</label></div>
            <div><label for="txtSalesPayEmailAddress" style="font-size:small;">(Please separate multiple email addresses with a comma. Ex. sample1@email.com, sample2@email.com)</label></div>
            <div><input type="text" id="txtSalesPayEmailAddress" /></div>
        </fieldset>

        <br /><br />
        <a id="btnSalesPaymentEmailOK" data-role="button">Ok</a>
        <a id="btnSalesPaymentEmailCancel" data-role="button">Cancel</a>

    </div>

</div>

<div data-role="page" id="pageAddNewCustomer">
    <div data-role="header">
        <h1>Customer Profile</h1>
    </div>
    <div data-role="content">
        <div data-role="fieldcontain" style="display:none;">
            <input type="text" readonly id="txtID" />
            <input type="text" readonly id="txtCustomerIndex" />
            <input type="text" readonly id="txtSCPWDCustomerID" />
        </div>
        @*
            <div data-role="fieldcontain">
                <label for="btnTitle">Title:</label>
                <div style="display:none;"><input type="text" readonly id="txtTitleID"/></div>
                <button id="btnTitle" data-role="button">Please Select</button>
            </div>*@
        You can enter customer name in fullname or in detailed format.
        <div data-role="fieldcontain">
            <label for="txtSurname">Full Name: <span style="color: #FF0000;">*</span></label>
            <input type="text" id="txtFullname" maxlength="255" />
        </div>

        <div data-role="fieldcontain">
            <label for="txtSurname">Surname (Last Name): <span style="color: #FF0000;">*</span></label>
            <input type="text" id="txtSurname" maxlength="255" />
        </div>

        <div data-role="fieldcontain">
            <label for="txtFirstName">First Name: <span style="color: #FF0000;">*</span></label>
            <input type="text" id="txtFirstName" maxlength="255" />
        </div>

        <div data-role="fieldcontain">
            <label for="txtMiddleName">Full Middle Name: <span style="color: #FF0000;">*</span></label>
            <input type="text" id="txtMiddleName" maxlength="255" />
        </div>

        <div data-role="fieldcontain">
            <label for="selPersonalSuffixName" >Generational Suffix (Name):</label>
            <select id="selPersonalSuffixName">
                <option value="">Please Select</option>
                <option value="Sr.">Senior (Sr.)</option>
                <option value="Jr.">Junior (Jr.)</option>
                <option value="II">II</option>
                <option value="III">III</option>
                <option value="IV">IV</option>
                <option value="V">V</option>
                <option value="VI">VI</option>
                <option value="VII">VII</option>
                <option value="VIII">VIII</option>
                <option value="IX">IX</option>
                <option value="X">X</option>
            </select>
        </div>
        <div class="custom-ui-label">
            <div>Date Of Birth:</div>
            <div>
                <fieldset data-role="controlgroup" data-type="horizontal">
                    <select id="selDOBMonth">
                        <option value="">MMM</option>
                    </select>
                    <select id="selDOBDay">
                        <option value="">DD</option>
                    </select>
                    <select id="selDOBYear">
                        <option value="">YYYY</option>
                    </select>
                </fieldset>
            </div>
        </div>

        <div>
            <ul data-role="listview" id="lstCustomerIdentificationContainer" data-inset="true">
                <li data-role="list-divider">Identification Number</li>
                <li>
                    <div data-role="fieldcontain"><a id="btnSalesPayCustomerIdentificationAdd" data-role="button" data-icon="plus" data-inline="true">Add New ID</a></div>

                    <div data-role="fieldcontain" id="divRecordCountCustomerIdentification"></div>
                    <div>
                        <ul data-role="listview" id="lstSalesPayCustomerIdentification" data-inset="true"></ul>
                    </div>
                </li>
            </ul>
        </div>

        <br />
        <br />
        <button id="btnSalesPayCustomerNameSave" type="submit">Ok</button>
        <a id="btnSalesPayCustomerNameRemove" data-role="button" style="display:none;">Remove</a>
        <button id="btnSalesPayCustomerNameCancel" type="submit">Cancel</button>
    </div>
    <div data-role="popup" id="popupAddNewID" data-theme="b" class="ui-corner-all">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
        <center><h3>&nbsp;&nbsp; Valid Customer Identification Number &nbsp;&nbsp;</h3></center>

        <div data-role="content" style="text-align: center; ">
            <select id="selIDType">
                <option value="">Please Select</option>
                <option value="Senior Citizens' ID">Senior Citizens' ID</option>
                <option value="Persons With Disabilities (PWD) ID">Persons With Disabilities (PWD) ID</option>
                <option value="Passport">Passport</option>
                <option value="Driver's License">Driver's License</option>
                <option value="SSS ID">SSS ID</option>
                <option value="GSIS ID">GSIS ID</option>
                <option value="PRC ID">PRC ID</option>
                <option value="Postal ID">Postal ID</option>
                <option value="Voter's ID">Voter's ID</option>
                <option value="Other">Other</option>

            </select>
            <div id="divOtherIDType" style="display:none;">
                <input type="text" id="txtOtherIDType" maxlength="50" style="text-align: center" placeholder="Enter ID Type" />
            </div>
            <input type="text" id="txtIDText" maxlength="20" style="text-align: center" placeholder="Enter a valid identification number" />
            <br />
            <button id="btnSalesPayCustomerIdentificationSave" type="submit">Ok</button>

            <a id="btnSalesPayCustomerIdentificationRemove" data-role="button" style="display:none;">Remove</a>
            <a id="btnSalesPayCustomerIdentificationCancel" data-role="button" data-rel="back">Cancel</a>

        </div>

    </div>

</div>


<div id="pageDiagnosePrinter" data-role="page">
    <div data-role="header">
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
        <h1>
            Diagnose Printer
        </h1>
    </div>
    <div data-role="content">
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

<div id="pageOtherPrinter" data-role="page">
    <div data-role="header">
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
        <h1>
            Print on Another Printer
        </h1>
    </div>
    <div data-role="content">
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

    <div data-role="popup" id="popupIPAddressInstruction" class="ui-corner-all">
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

<div data-role="dialog" id="dialogPrintConfirmation" class="ui-corner-all" data-dismissible="false">
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

<div data-role="dialog" id="dialogPrintConfirmationBill" class="ui-corner-all" data-dismissible="false">
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
