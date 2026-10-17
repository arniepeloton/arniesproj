<script>
    require([GetPathFormat('/Scripts/sources/POSSettings/POSSettings.js?v=' + assetversion)]);
</script>

<div class="container-fluid">
    <div id="divTabPOSSettings" style="white-space:nowrap; overflow:auto;">
        <ul class="nav nav-tabs" id="divGatePassTab" role="tablist" style="flex-wrap:nowrap;">
            <li class="nav-item" role="presentation">
                <button data-role="none" class="nav-link active" data-toggle="tab" data-target="#tabPOSBehavior" type="button" role="tab" aria-controls="dashboard" aria-selected="true">POS Behavior</button>
            </li>
            <li class="nav-item" role="presentation">
                <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabPOSPaymentOptions" type="button" role="tab" aria-controls="dashboard" aria-selected="true">POS Payment Options</button>
            </li>
            <li class="nav-item" role="presentation">
                <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabListOfOPs" type="button" role="tab" aria-controls="work" aria-selected="false">List of Other Payments</button>
            </li>
        </ul>
    </div>

    <div class="tab-content p-3">
        <div id="tabPOSBehavior" class="tab-pane fade show active">
            <div>
                <div style="display:none;">
                    <div class="row mb-3">
                        <label class="col-sm-3 mb-3 col-form-label" for="selEnableBillingOption">Enable Billing Option:</label>
                        <div class="col-sm-9">
                            <div class="form-check form-switch mb-3" style="font-size:25px;">
                                <input class="form-check-input" data-role="none" type="checkbox" id="selEnableBillingOption">
                            </div>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <label class="col-sm-3 mb-3 col-form-label" for="selWhenPrintOrderSlip">When do you want to print Order Slip?</label>
                        <div class="col-sm-9">
                            <select id="selWhenPrintOrderSlip" data-role="none" class="form-select">
                                <option value="Not Applicable">Not Applicable</option>
                                <option value="After Payment">After Payment</option>
                                <option value="Before Payment">Before Payment</option>
                            </select>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <label class="col-sm-3 mb-3 col-form-label" for="selPrintOrderSlip">Print Order Slip:</label>
                        <div class="col-sm-9">
                            <div class="form-check form-switch mb-3" style="font-size:25px;">
                                <input class="form-check-input" data-role="none" type="checkbox" id="selPrintOrderSlip">
                            </div>
                        </div>
                    </div>
                </div>

                <div class="row mb-3 d-none">
                    <label class="col-sm-3 mb-3 col-form-label" for="selEnableDiningMode">Enable Dining Mode:</label>
                    <div class="col-sm-9">
                        <div class="form-check form-switch mb-3" style="font-size:25px;">
                            <input class="form-check-input" data-role="none" type="checkbox" id="selEnableDiningMode">
                        </div>
                    </div>
                </div>

                <div class="row mb-3 d-none">
                    <label class="col-sm-3 mb-3 col-form-label" for="selEnableHotButtonsContextualMenu">Enable hot buttons contextual menu:</label>
                    <div class="col-sm-9">
                        <div class="form-check form-switch mb-3" style="font-size:25px;">
                            <input class="form-check-input" data-role="none" type="checkbox" id="selEnableHotButtonsContextualMenu">
                        </div>
                    </div>
                </div>

                <div class="row mb-3 d-none">
                    <label class="col-sm-3 mb-3 col-form-label" for="selEnableVATExemptOption">Enable VAT Exempt Option:</label>
                    <div class="col-sm-9">
                        <div class="form-check form-switch mb-3" style="font-size:25px;">
                            <input class="form-check-input" data-role="none" type="checkbox" id="selEnableVATExemptOption">
                        </div>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-3 mb-3 col-form-label" for="selEnforceZRead">Enforce Z-Read date as the posting date:</label>
                    <div class="col-sm-9">
                        <div class="form-check form-switch mb-3" style="font-size:25px;">
                            <input class="form-check-input" data-role="none" type="checkbox" id="selEnforceZRead">
                        </div>
                    </div>
                </div>

                <div class="row mb-3 d-none">
                    <label class="col-sm-3 mb-3 col-form-label" for="selOnly1SalesRep">Only 1 sales representative per transaction:</label>
                    <div class="col-sm-9">
                        <div class="form-check form-switch mb-3" style="font-size:25px;">
                            <input class="form-check-input" data-role="none" type="checkbox" id="selOnly1SalesRep">
                        </div>
                    </div>
                </div>

                <div class="row mb-3 d-none">
                    <label class="col-sm-3 mb-3 col-form-label" for="selRequireSalesRep">Require a sales representative:</label>
                    <div class="col-sm-9">
                        <div class="form-check form-switch mb-3" style="font-size:25px;">
                            <input class="form-check-input" data-role="none" type="checkbox" id="selRequireSalesRep">
                        </div>
                    </div>
                </div>

                <div class="row mb-3 d-none">
                    <label class="col-sm-3 mb-3 col-form-label" for="selDontAllowGreaterFreeTypePrices">In Free Type A Price, don't allow amount greater than the regular price:</label>
                    <div class="col-sm-9">
                        <div class="form-check form-switch mb-3" style="font-size:25px;">
                            <input class="form-check-input" data-role="none" type="checkbox" id="selDontAllowGreaterFreeTypePrices">
                        </div>
                    </div>
                </div>

                <div class="row mb-3 d-none">
                    <label class="col-sm-3 mb-3 col-form-label" for="selAlwaysTestFreeTypeAPrice">Always test Free Type A Price:</label>
                    <div class="col-sm-9">
                        <div class="form-check form-switch mb-3" style="font-size:25px;">
                            <input class="form-check-input" data-role="none" type="checkbox" id="selAlwaysTestFreeTypeAPrice">
                        </div>
                    </div>
                </div>

                <div class="row mb-3 d-none">
                    <label class="col-sm-3 mb-3 col-form-label" for="selSearchOnlyForBarcode">Search only for barcode:</label>
                    <div class="col-sm-9">
                        <div class="form-check form-switch mb-3" style="font-size:25px;">
                            <input class="form-check-input" data-role="none" type="checkbox" id="selSearchOnlyForBarcode">
                        </div>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-3 mb-3 col-form-label" for="selShowCondensedProductInformationInSearchResult">Show condensed product information in search result:</label>
                    <div class="col-sm-9">
                        <div class="form-check form-switch mb-3" style="font-size:25px;">
                            <input class="form-check-input" data-role="none" type="checkbox" id="selShowCondensedProductInformationInSearchResult">
                        </div>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-3 mb-3 col-form-label" for="selShowCondensedProductInformationInCart">Show condensed product information in cart:</label>
                    <div class="col-sm-9">
                        <div class="form-check form-switch mb-3" style="font-size:25px;">
                            <input class="form-check-input" data-role="none" type="checkbox" id="selShowCondensedProductInformationInCart">
                        </div>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-3 mb-3 col-form-label" for="selShowCondensedProductInformationInItemDetailsPage">Show condensed product information in item details page:</label>
                    <div class="col-sm-9">
                        <div class="form-check form-switch mb-3" style="font-size:25px;">
                            <input class="form-check-input" data-role="none" type="checkbox" id="selShowCondensedProductInformationInItemDetailsPage">
                        </div>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-3 mb-3 col-form-label" for="selHideBarcodeScan">Hide barcode scan button:</label>
                    <div class="col-sm-9">
                        <div class="form-check form-switch mb-3" style="font-size:25px;">
                            <input class="form-check-input" data-role="none" type="checkbox" id="selHideBarcodeScan">
                        </div>
                    </div>
                </div>

                <div class="row mb-3 d-none">
                    <label class="col-sm-3 mb-3 col-form-label" for="selHideHotButtonsButton">Hide "hot buttons" button:</label>
                    <div class="col-sm-9">
                        <div class="form-check form-switch mb-3" style="font-size:25px;">
                            <input class="form-check-input" data-role="none" type="checkbox" id="selHideHotButtonsButton">
                        </div>
                    </div>
                </div>

                <div class="row mb-3 d-none">
                    <label class="col-sm-3 mb-3 col-form-label" for="selHideSearchWithinResults">Hide "search within results" button:</label>
                    <div class="col-sm-9">
                        <div class="form-check form-switch mb-3" style="font-size:25px;">
                            <input class="form-check-input" data-role="none" type="checkbox" id="selHideSearchWithinResults">
                        </div>
                    </div>
                </div>

                <div class="row mb-3 d-none">
                    <label class="col-sm-3 mb-3 col-form-label" for="selPrintDispatchSlip">Print dispatch slip right after printing order slip:</label>
                    <div class="col-sm-9">
                        <div class="form-check form-switch mb-3" style="font-size:25px;">
                            <input class="form-check-input" data-role="none" type="checkbox" id="selPrintDispatchSlip">
                        </div>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-3 mb-3 col-form-label" for="selSetAllItemsEligibleForSCPWD">
                        Make all SKUs eligible for Senior Citizens' (SC) and Persons with Disability (PWD) Discount?:
                        <a id="lnkSCPWDInfo" class="ui-link" style="cursor: pointer;">
                            <img alt="By clicking yes, all current and future SKUs will be tagged as eligible to SC and PWD discounts." src="/Content/images/info.png" style="vertical-align:middle;">
                        </a>
                    </label>
                    <div class="col-sm-9">
                        <div class="form-check form-switch mb-3" style="font-size:25px;">
                            <input class="form-check-input" data-role="none" type="checkbox" id="selSetAllItemsEligibleForSCPWD">
                        </div>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-3 mb-3 col-form-label" for="selVatable">VAT Percent:</label>
                    <div class="col-sm-9">
                        <input id="txtVatPercent" data-role="none" class="form-control" type="number" maxlength="5" />
                    </div>
                </div>

                <div class="row mb-3 d-none">
                    <label class="col-sm-3 mb-3 col-form-label" for="selVatable">VATable:</label>
                    <div class="col-sm-9">
                        <div class="form-check form-switch mb-3" style="font-size:25px;">
                            <input class="form-check-input" data-role="none" type="checkbox" id="selVatable">
                        </div>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-3 mb-3 col-form-label" for="selAutoServiceCharge">Automatically apply service charge to each sale transaction?</label>
                    <div class="col-sm-9">
                        <div class="form-check form-switch mb-3" style="font-size:25px;">
                            <input class="form-check-input" data-role="none" type="checkbox" id="selAutoServiceCharge">
                        </div>
                        <div style="display:none;" id="divServiceChargeField">
                            <input type="text" data-role="none" class="form-control" id="txtServiceCharge" placeholder="Enter service charge" />
                        </div>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-3 mb-3 col-form-label">Service Charge</label>
                    <div class="col-sm-9">
                        <div class="form-check mb-3">
                            <input class="form-check-input" data-role="none" name="radServiceChargeBasis" value="vatex" type="radio" id="radServiceChargeBasis1">
                            <label class="form-check-label" for="radServiceChargeBasis1">
                                Service charge is based on VAT-exclusive price.
                            </label>
                        </div>

                        <div class="form-check mb-3">
                            <input class="form-check-input" data-role="none" name="radServiceChargeBasis" value="vatin" type="radio" id="radServiceChargeBasis2">
                            <label class="form-check-label" for="radServiceChargeBasis2">
                                Service charge is based on VAT-inclusive price.
                            </label>
                        </div>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-3 mb-3 col-form-label">Allow users to transact on the POS even if there is unflushed data from previous days?</label>
                    <div class="col-sm-9">
                        <div class="form-check mb-3">
                            <input class="form-check-input" data-role="none" name="CanTransactWithUnflushedData" value="1" type="radio" id="CanTransactWithUnflushedDataYes" checked>
                            <label class="form-check-label" for="CanTransactWithUnflushedDataYes">
                                Yes, allow them to transact.
                            </label>
                        </div>

                        <div class="form-check mb-3">
                            <input class="form-check-input" data-role="none" name="CanTransactWithUnflushedData" value="0" type="radio" id="CanTransactWithUnflushedDataNo">
                            <label class="form-check-label" for="CanTransactWithUnflushedDataNo">
                                No, they must flush all data from previous days before they can transact.
                            </label>
                        </div>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-3 mb-3 col-form-label" for="selPictureOfIDDurationSCPWDDiscount">Require Serior Citizen/PWD ID Picture when SCD/PWD discount is used in transaction</label>
                    <div class="col-sm-9">
                        <div class="form-check form-switch mb-3" style="font-size:25px;">
                            <input class="form-check-input" data-role="none" type="checkbox" id="selPictureOfIDDurationSCPWDDiscount">
                        </div>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-3 mb-3 col-form-label" for="setAllowOfflineSrvPayments">Allow offline SRV payments</label>
                    <div class="col-sm-9">
                        <div class="form-check form-switch mb-3" style="font-size:25px;">
                            <input class="form-check-input" data-role="none" type="checkbox" id="setAllowOfflineSrvPayments">
                        </div>
                        <div>
                            <p style="white-space: normal; word-break: break-word;">This setting allows cashiers to accept SRV payments even when a connection to the Mobiz server is not available.</p>
                            <p style="white-space: normal; word-break: break-word;">However, turning this feature on restricts SRV payments to be accepted only in the POS machine where the SRV was originally issued.</p>
                            <p style="white-space: normal; word-break: break-word;">Also, the original sales and SRV transaction must be available on the local storage of said POS machine to successfully transact the SRV payment.</p>
                            <p style="white-space: normal; word-break: break-word;">Data syncing of POS machine may cause it to not have access to these transaction records in its local storage.</p>
                        </div>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-3 mb-3 col-form-label" for="txtMaxMobizAppDatabaseAge">Maximum age (in minutes) of Mobiz app database:</label>
                    <div class="col-sm-9">
                        <input id="txtMaxMobizAppDatabaseAge" data-role="none" class="form-control" type="number" maxlength="5" />
                        <div>
                            <p style="white-space: normal; word-break: break-word;">During Synchronize Database, the server will provide you the lastest database (instead of creating a new one) if said database was created within the specified number of minutes.</p>
                        </div>
                    </div>
                </div>

                <div class="row mb-3 d-none">
                    <label class="col-sm-3 mb-3 col-form-label" for="selVatable">Decimal rules in selling prices</label>
                    <div class="col-sm-9">
                        <div class="form-check mb-3 d-none">
                            <input class="form-check-input" data-role="none" name="radRoundUpPrices" value="once" type="radio" id="radRoundUpPrices1">
                            <label class="form-check-label" for="radRoundUpPrices1">
                                Round up product selling prices per unit to the nearest <span class="spanRoundUpSellingPricesCentavo once"></span> centavos. Execute once.
                            </label>
                        </div>

                        <div class="form-check mb-3">
                            <input class="form-check-input" data-role="none" name="radRoundUpPrices" value="always" type="radio" id="radRoundUpPrices2">
                            <label class="form-check-label" for="radRoundUpPrices2">
                                Round up product selling prices per unit to the nearest <span class="spanRoundUpSellingPricesCentavo always"></span> centavos. Execute for every modification of product pricing information.
                            </label>
                        </div>

                        <div class="form-check mb-3">
                            <input class="form-check-input" data-role="none" name="radRoundUpPrices" value="" type="radio" id="radRoundUpPrices3" checked>
                            <label class="form-check-label" for="radRoundUpPrices3">
                                Don't round up product selling prices. Previously modified prices will stay at their current values.
                            </label>
                        </div>

                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-3 mb-3 col-form-label" for="txtUnflushedReminder">Show unflushed reminder popup every X minutes</label>
                    <div class="col-sm-9">
                        <input id="txtUnflushedReminderMinutes" data-role="none" class="form-control" type="number" maxlength="5" />
                        <div>
                            <p style="white-space: normal; word-break: break-word;" class="mb-0 mt-3">Message:</p>
                        </div>
                        <textarea id="txtUnflushedReminderMessage" class="form-control" maxlength="1000">

                        </textarea>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-3 mb-3 col-form-label" for="txtUnflushedCountSendingInterval">Unflushed count sending interval in minutes</label>
                    <div class="col-sm-9">
                        <input id="txtUnflushedCountSendingInterval" data-role="none" class="form-control" type="number" maxlength="5" />
                    </div>
                </div>

                <h5 class="mb-4 d-none">FMCG Mode</h5>

                <div class="row mb-3 d-none">
                    <label class="col-sm-3 mb-3 col-form-label" for="selVatable">How would you like to display the item(s) in the cart?</label>
                    <div class="col-sm-9">
                        <div class="form-check mb-3">
                            <input class="form-check-input" data-role="none" name="radFMCGOption" value="1" type="radio" id="radLineDisplay" checked>
                            <label class="form-check-label" for="radLineDisplay">
                                Line Item Display
                                <br /><br />
                                <img src="/Content/images/LineItemDisplay.png" />
                            </label>
                        </div>

                        <div class="form-check mb-3">
                            <input class="form-check-input" data-role="none" name="radFMCGOption" value="0" type="radio" id="radJournalDisplay" checked>
                            <label class="form-check-label" for="radJournalDisplay">
                                Journal Entry Display
                                <br /><br />
                                <img src="/Content/images/JournalEntryDisplay.png" />
                            </label>
                        </div>
                    </div>
                </div>
            </div>

            <div class="d-none">
                <h1 class="display-6 mb-4">Customer Input Fields</h1>

                <div class="row mb-3">
                    <label class="col-sm-3 mb-3 col-form-label" for="selCustomerTableNo">Show Customer or Table Number:</label>
                    <div class="col-sm-9">
                        <div class="form-check form-switch mb-3" style="font-size:25px;">
                            <input class="form-check-input" data-role="none" type="checkbox" id="selCustomerTableNo">
                        </div>
                    </div>
                </div>

                <div class="row mb-3" id="divCustomerTableNumCount" style="display:none;">
                    <label class="col-sm-3 mb-3 col-form-label" for="txtTableNoLimit">No. of Tables:</label>
                    <div class="col-sm-9">
                        <input id="txtTableNoLimit" data-role="none" class="form-control" type="number" maxlength="5" />
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-3 mb-3 col-form-label" for="selGasPumpNo">Show Gas Pump Number:</label>
                    <div class="col-sm-9">
                        <div class="form-check form-switch mb-3" style="font-size:25px;">
                            <input class="form-check-input" data-role="none" type="checkbox" id="selGasPumpNo">
                        </div>
                    </div>
                </div>

                <div class="row mb-3" id="divGasPumpCount" style="display:none;">
                    <label class="col-sm-3 mb-3 col-form-label" for="txtGasPumpNoLimit">No. of Gas Pumps:</label>
                    <div class="col-sm-9">
                        <input id="txtGasPumpNoLimit" data-role="none" class="form-control" type="number" maxlength="5" />
                    </div>
                </div>
            </div>

            <div class="d-none">
                <h1 class="display-6 mb-4">Order Slip</h1>

                <div class="row mb-3">
                    <label class="col-sm-3 mb-3 col-form-label" for="txtGasPumpNoLimit">When printing Order Slips, don't insert cuts in between different line item categories (e.g. don't insert a cut between the grouped drink items and the grouped food items)?</label>
                    <div class="col-sm-9" id="divOrderSlipFowsts" style="height:300px; border:1px solid #CCC; overflow:auto;">
                        <div class="form-check mb-3" style="font-size:20px;">
                            <input class="form-check-input" data-role="none" type="checkbox" id="chkREKH75X4YB">
                            <label class="form-check-label" for="chkREKH75X4YB">Main Branch</label>
                        </div>
                    </div>
                </div>
            </div>

            <div class="mt-4" style="text-align: center">
                <a name="btnCustomerInputFieldsCancel" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
                <a name="btnCustomerInputFieldsSave" data-role="none" class="btn btn-success btn-lg">Save</a>
            </div>
        </div>

        <div id="tabPOSPaymentOptions" class="tab-pane fade">
            <div class="row mb-3">
                <label class="col-sm-3 mb-3 col-form-label" for="selEnablePaymentCheck">Show Check/Gift Certificate:</label>
                <div class="col-sm-9">
                    <div class="form-check form-switch mb-3" style="font-size:25px;">
                        <input class="form-check-input" data-role="none" type="checkbox" id="selEnablePaymentCheck">
                    </div>
                </div>
            </div>
            <div class="row mb-3">
                <label class="col-sm-3 mb-3 col-form-label" for="selEnablePaymentDebitCard">Show Debit Card:</label>
                <div class="col-sm-9">
                    <div class="form-check form-switch mb-3" style="font-size:25px;">
                        <input class="form-check-input" data-role="none" type="checkbox" id="selEnablePaymentDebitCard">
                    </div>
                </div>
            </div>
            <div class="row mb-3">
                <label class="col-sm-3 mb-3 col-form-label" for="selEnablePaymentCreditCard">Show Credit Card:</label>
                <div class="col-sm-9">
                    <div class="form-check form-switch mb-3" style="font-size:25px;">
                        <input class="form-check-input" data-role="none" type="checkbox" id="selEnablePaymentCreditCard">
                    </div>
                </div>
            </div>
            <div class="row mb-3">
                <label class="col-sm-3 mb-3 col-form-label" for="selEnablePaymentSRV">Show SRV:</label>
                <div class="col-sm-9">
                    <div class="form-check form-switch mb-3" style="font-size:25px;">
                        <input class="form-check-input" data-role="none" type="checkbox" id="selEnablePaymentSRV">
                    </div>
                </div>
            </div>
            <div class="row mb-3">
                <label class="col-sm-3 mb-3 col-form-label" for="selEnableOtherPayment">Show Other Payment:</label>
                <div class="col-sm-9">
                    <div class="form-check form-switch mb-3" style="font-size:25px;">
                        <input class="form-check-input" data-role="none" type="checkbox" id="selEnableOtherPayment">
                    </div>
                </div>
            </div>
            <div class="row mb-3">
                <label class="col-sm-3 mb-3 col-form-label" for="selEnablePaymentOnCredit">Show On Credit:</label>
                <div class="col-sm-9">
                    <div class="form-check form-switch mb-3" style="font-size:25px;">
                        <input class="form-check-input" data-role="none" type="checkbox" id="selEnablePaymentOnCredit">
                    </div>
                </div>
            </div>
            <div class="row mb-3">
                <label class="col-sm-3 mb-3 col-form-label" for="selEnablePaymentBillingInfo">Show Billing Info:</label>
                <div class="col-sm-9">
                    <div class="form-check form-switch mb-3" style="font-size:25px;">
                        <input class="form-check-input" data-role="none" type="checkbox" id="selEnablePaymentBillingInfo">
                    </div>
                </div>
            </div>
            <div class="row mb-3">
                <label class="col-sm-3 mb-3 col-form-label" for="selEnablePaymentDeliveryInfo">Show Delivery Info:</label>
                <div class="col-sm-9">
                    <div class="form-check form-switch mb-3" style="font-size:25px;">
                        <input class="form-check-input" data-role="none" type="checkbox" id="selEnablePaymentDeliveryInfo">
                    </div>
                </div>
            </div>
            <div class="row mb-3">
                <label class="col-sm-3 mb-3 col-form-label" for="selEnablePaymentDeliveryPayment">Show Delivery Payment:</label>
                <div class="col-sm-9">
                    <div class="form-check form-switch mb-3" style="font-size:25px;">
                        <input class="form-check-input" data-role="none" type="checkbox" id="selEnablePaymentDeliveryPayment">
                    </div>
                </div>
            </div>
            <div class="row mb-3">
                <label class="col-sm-3 mb-3 col-form-label" for="selEnablePaymentDiscounts">Show Discounts:</label>
                <div class="col-sm-9">
                    <div class="form-check form-switch mb-3" style="font-size:25px;">
                        <input class="form-check-input" data-role="none" type="checkbox" id="selEnablePaymentDiscounts">
                    </div>
                </div>
            </div>
            <div class="row mb-3">
                <label class="col-sm-3 mb-3 col-form-label" for="selEnablePaymentCharges">Show Charges and Other Tax:</label>
                <div class="col-sm-9">
                    <div class="form-check form-switch mb-3" style="font-size:25px;">
                        <input class="form-check-input" data-role="none" type="checkbox" id="selEnablePaymentCharges">
                    </div>
                </div>
            </div>
            <div class="row mb-3">
                <label class="col-sm-3 mb-3 col-form-label" for="selEnablePaymentRewards">Show Rewards Program:</label>
                <div class="col-sm-9">
                    <div class="form-check form-switch mb-3" style="font-size:25px;">
                        <input class="form-check-input" data-role="none" type="checkbox" id="selEnablePaymentRewards">
                    </div>
                </div>
            </div>
            <div class="row mb-3">
                <label class="col-sm-3 mb-3 col-form-label" for="selEnablePaymentSalesRepresentative">Show Assign Sales Representative:</label>
                <div class="col-sm-9">
                    <div class="form-check form-switch mb-3" style="font-size:25px;">
                        <input class="form-check-input" data-role="none" type="checkbox" id="selEnablePaymentSalesRepresentative">
                    </div>
                </div>
            </div>

            <div class="mt-4" style="text-align: center">
                <a name="btnCustomerInputFieldsCancel" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
                <a name="btnCustomerInputFieldsSave" data-role="none" class="btn btn-success btn-lg">Save</a>
            </div>
        </div>

        <div id="tabListOfOPs" class="tab-pane fade">
            <div class="mb-3 row">
                <div class="col-md-6">
                    <input data-role="none" class="form-control" id="txtSearchOtherPayment" placeholder="Search here" />
                </div>
                <div class="col-md-6 text-right">
                    <button data-role="none" class="btn btn-success" id="btnNewOtherPayment">Add Other Payment</button>
                </div>
            </div>

            <div class="row">
                <div class="col-6">
                    <ul id="listOTherPayments" class="list-group"></ul>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="popupNewOtherPayment" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">New Other Payment</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div class="mb-3">
                    <label class="form-label">Enter Other Payment Name: <span class="required">*</span></label>
                    <input data-role="none" class="form-control" id="txtNewOtherPayment" />
                </div>

                <div class="row mb-3 align-items-center">
                    <div class="col-auto">Ensure that the reference number's character length is strictly followed:</div>
                    <div class="col">
                        <div class="form-check form-switch mb-3" style="font-size:25px;">
                            <input class="form-check-input" data-role="none" type="checkbox" id="selRequiredOPRefNumber">
                        </div>
                    </div>
                </div>

                <div id="divOPRefNumCharLength" style="display:none">
                    <div class="mb-3">
                        <label class="form-label">Enter Reference Number Character Length: <span class="required">*</span> (Include special characters in the count)</label>
                        <input data-role="none" type="number" class="form-control" id="txtOPRefNumCharLength" />
                    </div>                    

                    <div class="mb-3">
                        <label class="form-label">Data Type:</label>
                        <select data-role="none" id="selOPRefNumDataType" class="form-select">
                            <option value="alphanumeric">Alphanumeric</option>
                            <option value="alphabetic">Alphabetic</option>
                            <option value="numeric">Numeric</option>
                        </select>
                    </div>                    
                </div>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col">
                        <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Close</button>
                    </div>
                    <div class="col">
                        <button data-role="none" type="button" id="btnSaveNewOtherPayment" class="btn btn-success btn-block btn-lg">Save</button>
                    </div>
                </div>                
            </div>
        </div>
    </div>
</div>