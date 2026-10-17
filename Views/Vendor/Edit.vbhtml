@Code
    ViewData("Title") = "Vendor Information Sheet"
    Layout = "~/Views/Main/Layout.vbhtml"
End Code

    @If ProWeb.Common.UseMinifiedJavascripts Then
        @<text>
        <script src="/Scripts/minified/Vendor.js" type="text/javascript"></script>
        </text>
    Else
        @<text>
        <script src="/Scripts/sources/Project/Vendor.js" type="text/javascript"></script>
        </text>
    End If
    <style type="text/css">
        
    </style>

<h2>@ViewData("Title")</h2>
<div id="divResult" class="ui-state-error ui-corner-all">
    <div style="font-weight: bold; margin-bottom: 7px">
        Error</div>
    <table>
        <tr>
            <td>
                <span class="ui-icon ui-icon-alert" style="margin-right: 7px; float: left"></span>
            </td>
            <td>
                dsffgdfgsdfsd
                <br />
                dfgdfgdhgfh
                <br />
                dfgdfgdhgfh
                <br />
                dfgdfgdhgfh
                <br />
                dfgdfgdhgfh
                <div>
                    asd sdf sdf sdf<br />
                    assdfsdfsdf
                </div>
            </td>
        </tr>
    </table>
</div>
<form id="formVendor">
<table class="TableForm">
    <tr>
        <td class="label tdField">
            <span>Vendor Name</span><span class="Required">*</span>:
        </td>
        <td class="tdValue">
            <input name="txtName" class="full" />
        </td>
    </tr>
    <tr>
        <td class="label tdField">
            Regional Address:
        </td>
        <td>
            <input name="txtAddress" class="full" />
        </td>
    </tr>
    <tr>
        <td>
        </td>
        <td>
            <input name="txtAddress2" class="full" />
        </td>
    </tr>
    <tr>
        <td class="label tdField">
            Terms of Payment:
        </td>
        <td>
            <input name="txtPaymentTerms" class="full" />
        </td>
    </tr>
    <tr>
        <td class="label tdField">
            Contact Person:
        </td>
        <td>
            <input name="txtContactPerson" class="full" />
        </td>
    </tr>
    <tr>
        <td class="label tdField">
            Contact Number:
        </td>
        <td>
            <input name="txtContactNumber" class="full" />
        </td>
    </tr>
    <tr>
        <td class="label tdField">
            Email:
        </td>
        <td>
            <input name="txtEmail" class="full" />
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <div class="ui-widget-content" style="border-left: none; border-bottom: none; border-right: none;
                padding-top: 3px;">
            </div>
        </td>
    </tr>
    <tr>
        <td class="tdField">
            Delivery Mode:
        </td>
        <td>
            <input id="chkDeliverToDistriCenter" type="checkbox" class="checkbox" />
            <label for="chkDeliverToDistriCenter" class="label">
                Deliver to Distribution Center</label>
            <input id="chkDirectToStore" type="checkbox" class="checkbox" /><label for="chkDirectToStore"
                class="label">Direct to Store/Branch</label>
        </td>
    </tr>
    <tr>
        <td class="tdField">
            Mode of Payment:
        </td>
        <td>
            <input id="chkModeOfPaymentCwo" type="checkbox" class="checkbox" />
            <label for="chkModeOfPaymentCwo" class="label">
                CWO</label>
            <input id="chkModeOfPaymentCod" type="checkbox" class="checkbox" />
            <label for="chkModeOfPaymentCod" class="label">
                COD</label>
            <input id="chkModeOfPaymentCbd" type="checkbox" class="checkbox" />
            <label for="chkModeOfPaymentCbd" class="label">
                CBD</label>
            <input id="chkModeOfPaymentTerms" type="checkbox" class="checkbox" />
            <label for="chkModeOfPaymentTerms" class="label" style="float: left; margin-left: 2px">
                Terms</label>
            <input name="txtTermDays" style="margin-left: 5px; margin-top: -10px; width: 50px"
                maxlength="3" />
            Days
        </td>
    </tr>
    <tr>
        <td class="label tdField">
            COD Discount:
        </td>
        <td>
            <input name="txtCodDiscount" style="width: 50px; margin-right: 6px;" maxlength="6" />%
        </td>
    </tr>
    <tr>
        <td class="tdField">
            Budget for promotion available?
        </td>
        <td>
            <input id="optBudgetYes" name="optBudget" type="radio" class="checkbox" />
            <label for="optBudgetYes" class="label" style="float: left; margin-left: 2px">
                Yes, Amount: Php</label>
            <input style="margin-left: 5px; margin-top: -5px; width: 200px; clear: right" maxlength="300" />
            <input id="optBudgetNo" name="optBudget" type="radio" class="checkbox" style="clear: both" />
            <label for="optBudgetNo" class="label">
                No</label>
        </td>
    </tr>
    <tr>
        <td class="tdField">
            Fixtures and Equipment:
        </td>
        <td>
            <input id="chkFixturesRefrigerator" type="checkbox" class="checkbox" />
            <label for="chkFixturesRefrigerator" class="label">
                Refrigerator</label>
            <input id="chkFixturesFreezer" type="checkbox" class="checkbox" />
            <label for="chkFixturesFreezer" class="label">
                Freezer</label>
            <input id="chkFixturesChiller" type="checkbox" class="checkbox" />
            <label for="chkFixturesChiller" class="label">
                Chiller</label>
            <input id="chkFixturesMicrowave" type="checkbox" class="checkbox" />
            <label for="chkFixturesMicrowave" class="label">
                Microwave</label>
            <input id="chkFixturesDispensers" type="checkbox" class="checkbox" />
            <label for="chkFixturesDispensers" class="label">
                Dispensers</label>
            <input id="chkFixturesDisplay" type="checkbox" class="checkbox" />
            <label for="chkFixturesDisplay" class="label">
                Display Collateral</label>
            <input id="chkFixturesOthers" type="checkbox" class="checkbox" />
            <label for="chkFixturesOthers" class="label" style="float: left; margin-left: 2px">
                Others</label>
            <input name="txtFixturesOthers" style="margin-left: 5px; margin-top: -5px; width: 200px"
                maxlength="300" />
        </td>
    </tr>
    <tr>
        <td class="label tdField">
            Discounts:
        </td>
        <td style="padding: 0">
            <table class="inner">
                <tr>
                    <td>
                        <input id="chkDiscountCash" type="checkbox" class="checkbox" />
                    </td>
                    <td>
                        <label for="chkDiscountCash" class="label">
                            Cash</label>
                    </td>
                    <td>
                        <input name="txtDiscountCash" style="width: 50px" maxlength="5" />
                        %
                    </td>
                </tr>
                <tr>
                    <td>
                        <input id="chkDiscountVolume" type="checkbox" class="checkbox" />
                    </td>
                    <td>
                        <label for="chkDiscountVolume" class="label">
                            Volume</label>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                        Target:
                    </td>
                    <td>
                        <input name="txtDiscountTarget" style="width: 100px" maxlength="10" />
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                        Vol. Disc.:
                    </td>
                    <td>
                        <input name="txtDiscountVolume" style="width: 100px" maxlength="10" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td class="tdField">
            Budget Allocated for Special Events / Advertising
        </td>
        <td style="padding: 0">
            <table class="inner">
                <tr>
                    <td>
                        <input id="optBudgetAdvertisingYes" name="optBudgetAdvertising" type="radio" class="checkbox"
                            value="yes" />
                    </td>
                    <td>
                        <label for="optBudgetAdvertisingYes" class="label">
                            Yes</label>
                    </td>
                    <td style="padding-left: 10px">
                        Php
                        <input name="txtBudgetCash" style="width: 50px" maxlength="12" />
                        <input name="txtBudgetPercent" style="width: 50px" maxlength="12" />
                        %
                    </td>
                </tr>
                <tr>
                    <td>
                        <input id="optBudgetAdvertisingNo" name="optBudgetAdvertising" type="radio" class="checkbox"
                            value="no" />
                    </td>
                    <td colspan="2">
                        <label for="optBudgetAdvertisingNo" class="label">
                            No</label>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td class="label tdField">
            Lead time:
        </td>
        <td>
            <input name="txtLeadTime" style="width: 50px; margin-right: 3px" maxlength="3" />Days
            upon sending of PO
        </td>
    </tr>
    <tr>
        <td class="tdField">
            Capable of receiving P.O.s via Email:
        </td>
        <td style="padding: 0">
            <table class="inner">
                <tr>
                    <td>
                        <input id="optCanEmailYes" name="optCanEmail" type="radio" class="checkbox" value="yes" />
                    </td>
                    <td>
                        <label for="optCanEmailYes" class="label">
                            Yes</label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <input id="optCanEmailNo" name="optCanEmail" type="radio" class="checkbox" value="no" />
                    </td>
                    <td>
                        <label for="optCanEmailNo" class="label">
                            No</label>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td class="tdField">
            Merchandiser Available?
        </td>
        <td style="padding: 0">
            <table class="inner">
                <tr>
                    <td>
                        <input id="optMerchandiserYes" name="optMerchandiser" type="radio" class="checkbox"
                            value="yes" />
                    </td>
                    <td>
                        <label for="optMerchandiserYes" class="label">
                            Yes</label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <input id="optMerchandiserlNo" name="optMerchandiser" type="radio" class="checkbox"
                            value="no" />
                    </td>
                    <td>
                        <label for="optMerchandiserlNo" class="label">
                            No</label>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td class="label tdField">
            Allowance for Bad Orders:
        </td>
        <td>
            <input name="txtLeadTime" style="width: 50px; margin-right: 3px" maxlength="5" />%
        </td>
    </tr>
    <tr>
        <td class="label tdField">
            BO Policy:
        </td>
        <td>
            <textarea class="full" maxlength="300"></textarea>
        </td>
    </tr>
    <tr>
        <td class="label tdField">
            Lot Size Policy:
        </td>
        <td style="padding: 0">
            <table class="inner">
                <tr>
                    <td>
                        <input id="optLotSizeBreakCase" name="optLotSize" type="radio" class="checkbox" value="yes" />
                    </td>
                    <td>
                        <label for="optLotSizeBreakCase" class="label">
                            Break Case Allowed?</label>
                    </td>
                    <td style="padding-left: 10px">
                        <input name="txtLotSizeBreakCase" style="width: 100px" maxlength="50" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <input id="optLotSizeFullCase" name="optLotSize" type="radio" class="checkbox" value="no" />
                    </td>
                    <td colspan="2">
                        <label for="optLotSizeFullCase" class="label">
                            Full Case Only</label>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td class="label tdField">
            Other Remarks:
        </td>
        <td>
            <textarea class="full" maxlength="300"></textarea>
            <div id="sh">
            </div>
        </td>
    </tr>
    <tr>
        <td class="label tdField">
            Prepared By:
        </td>
        <td>
            <input name="txtContactNumber" maxlength="50" class="full" />
        </td>
    </tr>
    <tr>
        <td class="label tdField">
            Conforme:
        </td>
        <td>
            <input name="txtContactNumber" maxlength="50" class="full" />
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <div class="ui-widget-content" style="border-left: none; border-bottom: none; border-right: none;
                padding-top: 3px;">
            </div>
            <button id="butSave" type="submit" class="button ui-icon-disk">
                Save</button>
        </td>
    </tr>
</table>
</form>
