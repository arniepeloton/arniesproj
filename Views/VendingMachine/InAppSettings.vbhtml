<script>
    require(['/Scripts/sources/VendingMachine/InAppSettings.js?v=' + assetversion])
</script>

<style>
    .sidebar-panel {
        width: 20%;
        height: 100vh;
        overflow-y: auto; /* Scrollbar appears here if reports overflow */
        border-right: 1px solid #dee2e6;
    }

    .main-panel {
        width: 80%;
        height: 100vh;
        overflow-y: auto;
    }

    .sidebar-scroll-content {
        flex-grow: 1;
        overflow-y: auto;
    }
</style>

<div class="d-flex">

    <aside class="sidebar-panel bg-white p-3 d-flex flex-column">
        <div class="mb-4">
            <h5 class="text-secondary uppercase fw-bold tracking-wider">VM Settings</h5>
        </div>

        <div class="sidebar-scroll-content">
            <div class="nav nav-pills flex-column gap-2">
                <button class="btn btnSettingMenu text-start d-flex align-items-center gap-2" type="button" page="salessummary">
                    <i class="bi bi-bar-chart-line-fill bgvm-secondary p-2 textvm-primary rounded-pill"></i>
                    <span>Sales Summary</span>
                </button><button accessright="CONTROL_DOOR" class="btn btnSettingMenu text-start d-flex align-items-center gap-2" type="button" page="controldoor">
                    <i class="bi bi-door-closed-fill bgvm-secondary p-2 textvm-primary rounded-pill"></i>
                    <span>Control Door</span>
                </button>
                <button accessright="CONTROL_LED" class="btn btnSettingMenu text-start d-flex align-items-center gap-2" type="button" page="controlled">
                    <i class="bi bi-lightbulb-fill bgvm-secondary p-2 textvm-primary rounded-pill"></i>
                    <span>Control LED</span>
                </button>
                <button accessright="CONTROL_LOCK" class="btn btnSettingMenu text-start d-flex align-items-center gap-2" type="button" page="controllock">
                    <i class="bi bi-lock-fill bgvm-secondary p-2 textvm-primary rounded-pill"></i>
                    <span>Control Lock</span>
                </button>
                <button accessright="CONTROL_POWER" class="btn btnSettingMenu text-start d-flex align-items-center gap-2" type="button" page="controlpower">
                    <i class="bi bi-lightning-fill bgvm-secondary p-2 textvm-primary rounded-pill"></i>
                    <span>Control Power</span>
                </button>
                <button accessright="CONTROL_VALVE" class="btn btnSettingMenu text-start d-flex align-items-center gap-2" type="button" page="controlvalve">
                    <i class="bi bi-droplet-half bgvm-secondary p-2 textvm-primary rounded-pill"></i>
                    <span>Control Valve</span>
                </button>
                <button accessright="DISPENSE_ITEM" class="btn btnSettingMenu text-start d-flex align-items-center gap-2" type="button" page="dispenseitem">
                    <i class="bi bi-box-seam-fill bgvm-secondary p-2 textvm-primary rounded-pill"></i>
                    <span>Dispense Items</span>
                </button>
                <button accessright="DISPENSE_VOLUME" class="btn btnSettingMenu text-start d-flex align-items-center gap-2" type="button" page="dispensevolume">
                    <i class="bi bi-droplet-fill bgvm-secondary p-2 textvm-primary rounded-pill"></i>
                    <span>Dispense Volume</span>
                </button>
                <button accessright="CASH_DEPOSIT" class="btn btnSettingMenu text-start d-flex align-items-center gap-2" type="button" page="cashacceptor">
                    <i class="bi bi-cash-coin bgvm-secondary p-2 textvm-primary rounded-pill"></i>
                    <span>Cash Acceptor</span>
                </button>
                <button accessright="MACHINE_INVENTORY" class="btn btnSettingMenu text-start d-flex align-items-center gap-2" type="button" page="inventory">
                    <i class="bi bi-basket2-fill bgvm-secondary p-2 textvm-primary rounded-pill"></i>
                    <span>Inventory</span>
                </button>
                <button class="btn btnSettingMenu text-start d-flex align-items-center gap-2" type="button" page="moresettings">
                    <i class="bi bi-gear-fill bgvm-secondary p-2 textvm-primary rounded-pill"></i>
                    <span>More Settings</span>
                </button>
            </div>
        </div>

        <div class="mt-auto pt-3 border-top">
            <button id="btnBacktoMainScreen" class="btn btn-warning btn-block d-flex align-items-center justify-content-center">
                <span class="bi bi-chevron-left"></span> Back
            </button>
        </div>
    </aside>

    <main class="main-panel p-4">
        @*<header class="d-flex justify-content-between align-items-center mb-4 pb-3 border-bottom">
            <div>
                <h2 class="h3 mb-0 fw-semibold">Sales Overview Report</h2>
                <p class="text-muted small mb-0">Data updated 10 minutes ago</p>
            </div>
        </header>*@

        <div class="container-fluid px-0">
            <div id="page_vdmsettings" subpage="setting" style="display:none">
                <div class="row">
                    <div class="col-8">
                        <div class="row align-content-center mb-5">
                            <div class="col-auto">Available Boards:</div>
                            <div class="col-auto"><select data-role="none" id="selAvailableBoards" class="form-select"></select></div>
                        </div>

                        <div id="controldoor" subpage="inappsetting" style="display:none;">
                            <div class="row align-items-center mb-3">
                                <div class="col-2">
                                    Door:
                                </div>
                                <div class="col-auto">
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" name="control_door" id="door0" value="0">
                                        <label class="form-check-label" for="door0">Sliding</label>
                                    </div>
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" name="control_door" id="door1" value="1">
                                        <label class="form-check-label" for="door1">Product</label>
                                    </div>
                                </div>
                            </div>
                            <div class="row align-items-center mb-3">
                                <div class="offset-2 col-auto">
                                    <button type="button" class="btn btnvm-primary btnDoorState" state="1">Open</button>
                                    <button type="button" class="btn btnvm-secondary btnDoorState" state="0">Close</button>
                                </div>
                            </div>
                        </div>

                        <div id="dispenseitem" subpage="inappsetting" style="display:none;">
                            <div class="row align-items-center mb-3">
                                <div class="col-3">
                                    From motor no.
                                </div>
                                <div class="col-4">
                                    <div class="text-center rounded-2 border bg-white">
                                        <div class="input-group">
                                            <button data-role="none" action="-" class="btn btnFromMotorAction textvm-primary m-2 rounded-2 bg-light"><span class="bi-dash-lg"></span></button>
                                            <input data-role="none" type="text" id="FromMotorNo" class="form-control form-control-plaintext text-center shadow-none bg-white gradient-borderx" readonly autocomplete="off" />
                                            <button data-role="none" action="+" class="btn btnFromMotorAction textvm-primary m-2 rounded-2 bgvm-secondary"><span class="bi-plus-lg"></span></button>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-auto">
                                    to
                                </div>
                                <div class="col-4">
                                    <div class="text-center rounded-2 border bg-white">
                                        <div class="input-group">
                                            <button data-role="none" action="-" class="btn btnToMotorAction textvm-primary m-2 rounded-2 bg-light"><span class="bi-dash-lg"></span></button>
                                            <input data-role="none" type="text" id="ToMotorNo" class="form-control form-control-plaintext text-center shadow-none bg-white gradient-borderx" readonly autocomplete="off" />
                                            <button data-role="none" action="+" class="btn btnToMotorAction textvm-primary m-2 rounded-2 bgvm-secondary"><span class="bi-plus-lg"></span></button>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="row align-items-center mb-3">
                                <div class="col-3">
                                    Rotate each motor:
                                </div>
                                <div class="col-4">
                                    <div class="text-center rounded-2 border bg-white">
                                        <div class="input-group">
                                            <button data-role="none" action="-" class="btn btnTurnAction textvm-primary m-2 rounded-2 bg-light"><span class="bi-dash-lg"></span></button>
                                            <input data-role="none" type="text" id="TurnNo" class="form-control form-control-plaintext text-center shadow-none bg-white gradient-borderx" readonly autocomplete="off" />
                                            <button data-role="none" action="+" class="btn btnTurnAction textvm-primary m-2 rounded-2 bgvm-secondary"><span class="bi-plus-lg"></span></button>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-auto">
                                    time(s)
                                </div>
                            </div>
                            <div class="row align-items-center mb-3">
                                <div class="offset-3 col-auto">
                                    <button type="button" id="btnDispenseItem" class="btn btnvm-primary">Dispense</button>
                                </div>
                            </div>
                        </div>

                        <div id="dispensevolume" subpage="inappsetting" style="display:none;">
                            <div class="row align-items-center mb-3">
                                <div class="col-3">
                                    Motor No.:
                                </div>
                                <div class="col">
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" name="dispense_volume_motor" id="volume_motor1" value="1">
                                        <label class="form-check-label" for="volume_motor1">1</label>
                                    </div>
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" name="dispense_volume_motor" id="volume_motor2" value="2">
                                        <label class="form-check-label" for="volume_motor2">2</label>
                                    </div>
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" name="dispense_volume_motor" id="volume_motor3" value="3">
                                        <label class="form-check-label" for="volume_motor3">3</label>
                                    </div>
                                </div>
                            </div>

                            <div class="row align-items-center mb-3">
                                <div class="col-3">
                                    Volume:
                                </div>
                                <div class="col">
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" name="dispense_volume" id="volume_5ml" value="5">
                                        <label class="form-check-label" for="volume_5ml">5 ml</label>
                                    </div>
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" name="dispense_volume" id="volume_10ml" value="10">
                                        <label class="form-check-label" for="volume_10ml">10 ml</label>
                                    </div>
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" name="dispense_volume" id="volume_100ml" value="100">
                                        <label class="form-check-label" for="volume_100ml">100 ml</label>
                                    </div>
                                </div>
                            </div>
                            <div class="row align-items-center mb-3">
                                <div class="offset-3 col-auto">
                                    <button type="button" id="btnDispenseVolume" class="btn btnvm-primary">Dispense</button>
                                </div>
                            </div>
                        </div>

                        <div id="cashacceptor" subpage="inappsetting" style="display:none;">
                            <div class="row align-items-center mb-3">
                                <div class="col-3">
                                    Action:
                                </div>
                                <div class="col-auto">
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" name="cash_acceptor" id="accept" value="accept" checked>
                                        <label class="form-check-label" for="accept">Accept</label>
                                    </div>
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" name="cash_acceptor" id="dispense" value="dispense">
                                        <label class="form-check-label" for="dispense">Dispense</label>
                                    </div>
                                </div>
                            </div>
                            <div id="accept_option" style="display:block;">
                                <div class="row align-items-center mb-3">
                                    <div class="col-3">
                                        Amount to be accepted:
                                    </div>
                                    <div class="col-auto">
                                        <div class="text-center rounded-2 border bg-white">
                                            <div class="input-group">
                                                <button data-role="none" action="-" class="btn btnCashAcceptor textvm-primary m-2 rounded-2 bg-light"><span class="bi-dash-lg"></span></button>
                                                <input data-role="none" type="text" id="CashAcceptAmount" class="form-control form-control-plaintext text-center shadow-none bg-white gradient-borderx" readonly autocomplete="off" />
                                                <button data-role="none" action="+" class="btn btnCashAcceptor textvm-primary m-2 rounded-2 bgvm-secondary"><span class="bi-plus-lg"></span></button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div id="dispense_option" style="display:none;">
                                <div class="row align-items-center mb-3">
                                    <div class="col-3">
                                        Dispense Coin:
                                    </div>
                                    <div class="col-auto">
                                        <div class="form-check form-check-inline">
                                            <input class="form-check-input" type="checkbox" name="cash_acceptor" id="coin1" value="coin1">
                                            <label class="form-check-label" for="coin1">&#8369 0.25</label>
                                        </div>
                                        <div class="form-check form-check-inline">
                                            <input class="form-check-input" type="checkbox" name="cash_acceptor" id="coin2" value="coin2">
                                            <label class="form-check-label" for="coin2">&#8369 1</label>
                                        </div>
                                        <div class="form-check form-check-inline">
                                            <input class="form-check-input" type="checkbox" name="cash_acceptor" id="coin3" value="coin3">
                                            <label class="form-check-label" for="coin3">&#8369 5</label>
                                        </div>
                                        <div class="form-check form-check-inline">
                                            <input class="form-check-input" type="checkbox" name="cash_acceptor" id="coin4" value="coin4">
                                            <label class="form-check-label" for="coin4">&#8369 20</label>
                                        </div>
                                    </div>
                                </div>
                                <div class="row align-items-center mb-3">
                                    <div class="col-3">
                                        Quantity per coin:
                                    </div>
                                    <div class="col-auto">
                                        <div class="text-center rounded-2 border bg-white">
                                            <div class="input-group">
                                                <button data-role="none" action="-" class="btn btnQtyCoin textvm-primary m-2 rounded-2 bg-light"><span class="bi-dash-lg"></span></button>
                                                <input data-role="none" type="text" id="QtyCoin" class="form-control form-control-plaintext text-center shadow-none bg-white gradient-borderx" readonly autocomplete="off" />
                                                <button data-role="none" action="+" class="btn btnQtyCoin textvm-primary m-2 rounded-2 bgvm-secondary"><span class="bi-plus-lg"></span></button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="row align-items-center mb-3">
                                <div class="offset-3 col-auto">
                                    <button type="button" id="btnCoinAcceptorRun" class="btn btnvm-primary">Run</button>
                                    <button type="button" id="btnDisableCoinAcceptor" class="btn bg-danger-light text-danger">Turn Off Cash Acceptor</button>
                                </div>
                            </div>
                        </div>

                        <div id="controllock" subpage="inappsetting" style="display:none;">
                            <div class="row align-items-center mb-3">
                                <div class="offset-3 col-auto">
                                    <button type="button" id="btnOpenVM" class="btn btnvm-primary">Open Vending Machine</button>
                                </div>
                            </div>
                        </div>

                        <div id="controlvalve" subpage="inappsetting" style="display:none;">
                            <div class="row align-items-center mb-3">
                                <div class="col-3">
                                    Motor No.:
                                </div>
                                <div class="col">
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" name="valve_motor" id="valve_motor1" value="1">
                                        <label class="form-check-label" for="valve_motor1">1</label>
                                    </div>
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" name="valve_motor" id="valve_motor2" value="2">
                                        <label class="form-check-label" for="valve_motor2">2</label>
                                    </div>
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" name="valve_motor" id="valve_motor3" value="3">
                                        <label class="form-check-label" for="valve_motor3">3</label>
                                    </div>
                                </div>
                            </div>
                            <div class="row align-items-center mb-3">
                                <div class="offset-3 col-auto">
                                    <button type="button" state="1" class="btn btnvm-primary btnValveStatus">Open</button>
                                    <button type="button" state="0" class="btn btnvm-secondary btnValveStatus">Close</button>
                                </div>
                            </div>
                        </div>

                        <div id="controlled" subpage="inappsetting" style="display:none;">
                            <div class="row align-items-center mb-3">
                                <div class="col-3">
                                    LED Color:
                                </div>
                                <div class="col">
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" name="control_led" id="led_off" value="0">
                                        <label class="form-check-label" for="led_off">Off</label>
                                    </div>
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" name="control_led" id="led_red" value="1">
                                        <label class="form-check-label" for="led_red">Red</label>
                                    </div>
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" name="control_led" id="led_green" value="2">
                                        <label class="form-check-label" for="led_green">Green</label>
                                    </div>
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" name="control_led" id="led_blue" value="3">
                                        <label class="form-check-label" for="led_blue">Blue</label>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div id="controlpower" subpage="inappsetting" style="display:none;">
                            <div class="row align-items-center mb-3">
                                <div class="col-3">
                                    Power Channel:
                                </div>
                                <div class="col-auto">
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" name="control_power" id="PC24v" value="1">
                                        <label class="form-check-label" for="PC24v">24V Rail</label>
                                    </div>
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" name="control_power" id="PC12v" value="2">
                                        <label class="form-check-label" for="PC12v">12V Rail</label>
                                    </div>
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" name="control_power" id="PCAll" value="10" checked>
                                        <label class="form-check-label" for="PCAll">All</label>
                                    </div>
                                </div>
                            </div>
                            <div class="row align-items-center mb-3">
                                <div class="col-3">
                                    Switch:
                                </div>
                                <div class="col-auto">
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" name="control_power_switch" id="PCSwitchOn" value="1" checked>
                                        <label class="form-check-label" for="PCSwitchOn">On</label>
                                    </div>
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" name="control_power_switch" id="PCSwitchOff" value="0">
                                        <label class="form-check-label" for="PCSwitchOff">Off</label>
                                    </div>
                                </div>
                            </div>
                            <div class="row align-items-center mb-3">
                                <div class="offset-3 col-auto">
                                    <button type="button" id="btnRunControlPower" class="btn btnvm-primary">Run</button>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-4 rounded-2 bg-white">
                        <h3>Results</h3>
                        <pre style="white-space: break-spaces"></pre>
                    </div>
                </div>
            </div>

            <div id="salessummary" subpage="setting" style="display:none;">
                <div class="flex-shrink-1 bg-white shadow-sm rounded-1 mb-3 p-3 border bordervm-secondary">
                    <div class="row m-0 gx-3 align-items-center">
                        <div class="col-auto">
                            Period:
                        </div>
                        <div class="col">
                            <div class="btn-group">
                                <input data-role="none" type="radio" class="btn-check" name="chbSalesPeriod" id="chbSalesPeriodToday" value="Today" autocomplete="off" checked>
                                <label class="btn btnvm-primary-outline border bordervm-secondary shadow-sm rounded-pill mr-2 ml-2" for="chbSalesPeriodToday">Today</label>

                                <input data-role="none" type="radio" class="btn-check" name="chbSalesPeriod" id="chbSalesPeriodYesterday" value="Yesterday" autocomplete="off">
                                <label class="btn btnvm-primary-outline border bordervm-secondary shadow-sm rounded-pill mr-2 ml-2" for="chbSalesPeriodYesterday">Yesterday</label>

                                <input data-role="none" type="radio" class="btn-check" name="chbSalesPeriod" id="chbSalesPeriodThisWeek" value="This Week" autocomplete="off">
                                <label class="btn btnvm-primary-outline border bordervm-secondary shadow-sm rounded-pill mr-2 ml-2" for="chbSalesPeriodThisWeek">This Week</label>

                                <input data-role="none" type="radio" class="btn-check" name="chbSalesPeriod" id="chbSalesPeriodThisMonth" value="This Month" autocomplete="off">
                                <label class="btn btnvm-primary-outline border bordervm-secondary shadow-sm rounded-pill mr-2 ml-2" for="chbSalesPeriodThisMonth">This Month</label>

                                <input data-role="none" type="radio" class="btn-check" name="chbSalesPeriod" id="chbSalesPeriodThisYear" value="This Year" autocomplete="off">
                                <label class="btn btnvm-primary-outline border bordervm-secondary shadow-sm rounded-pill mr-2 ml-2" for="chbSalesPeriodThisYear">This Year</label>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col">
                        <div class="flex-shrink-1 bg-white shadow-sm rounded-1 p-3 border bordervm-secondary" style="border-left: 5px solid #188750 !important;">
                            <div class="row align-items-center">
                                <div class="col">
                                    <h6 class="textvm-primary">Total Net Sales</h6>
                                    <h2 id="lblNetSales"></h2>
                                    <span class="badge textvm-primary bgvm-secondary p-2 pr-3 pl-3 rounded-pill"><span class="bi-graph-up-arrow mr-2"></span> <label id="lblNetSalesPercentage"></label></span> <span class="text-secondary">vs. previous <label class="lblPreviosPeriod">day</label></span>
                                </div>
                                <div class="col-auto">
                                    <div class="rounded-circle bgvm-secondary p-4 d-flex align-items-center justify-content-center" style="height: 80px; width: 80px;">
                                        <span class="bi bi-graph-up-arrow textvm-primary" style="font-size: 30px;"></span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col">
                        <div class="flex-shrink-1 bg-white shadow-sm rounded-1 p-3 border bordervm-secondary" style="border-left: 5px solid #188750 !important;">
                            <div class="row align-items-center">
                                <div class="col">
                                    <h6 class="textvm-primary">Items Sold</h6>
                                    <h2 id="lblItemsSold">18</h2>
                                    <span class="badge textvm-primary bgvm-secondary p-2 pr-3 pl-3 rounded-pill"><span class="bi-graph-up-arrow mr-2"></span> <label id="lblItemsSoldPercentage"></label></span> <span class="text-secondary">vs. previous <label class="lblPreviosPeriod">day</label></span>
                                </div>
                                <div class="col-auto">
                                    <div class="rounded-circle bgvm-secondary p-4 d-flex align-items-center justify-content-center" style="height: 80px; width: 80px;">
                                        <span class="bi bi-cart4 textvm-primary" style="font-size: 30px;"></span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col">
                        <div class="flex-shrink-1 bg-white shadow-sm rounded-1 p-3 border bordervm-secondary" style="border-left: 5px solid #188750 !important;">
                            <div class="row align-items-center">
                                <div class="col">
                                    <h6 class="textvm-primary">Top Item Sold</h6>
                                    <h2 id="lblTopItemSoldName"></h2>
                                    <span class="badge textvm-primary bgvm-secondary p-2 pr-3 pl-3 rounded-pill"><span class="bi-lightning-charge mr-2"></span> <label id="lblTopItemSoldCount"></label> units</span>
                                </div>
                                <div class="col-auto">
                                    <div class="rounded-circle bgvm-secondary p-4 d-flex align-items-center justify-content-center" style="height: 80px; width: 80px;">
                                        <span class="bi bi-star textvm-primary" style="font-size: 30px;"></span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-7">
                        <div class="bg-white shadow-sm rounded-1 p-3 border bordervm-secondary h-100">
                            <div class="row align-items-center mb-3">
                                <div class="col">
                                    <h5><b>Sales over time</b></h5>
                                </div>
                                <div class="col text-right">
                                    <h6 class="text-secondary" id="lblSalesOvertime">Hourly Revenue <span class="bi-dot"></span> Today</h6>
                                </div>
                            </div>

                            <!-- Added flex-grow-1, position-relative, and min-height -->
                            <div class="flex-grow-1 position-relative" style="min-height: 0;">
                                <canvas id="chartADS" style="position: absolute; top: 0; left: 0; bottom: 0; width: 100%; height: 100%;"></canvas>
                            </div>
                        </div>
                    </div>
                    <div class="col-5">
                        <div class="flex-shrink-1 bg-white shadow-sm rounded-1 p-3 border bordervm-secondary h-100">
                            <div class="row align-items-center mb-3">
                                <div class="col">
                                    <h5><b>Most items sold</b></h5>
                                </div>
                                <div class="col text-right">
                                    <h6 class="text-secondary">Top 5 Products</h6>
                                </div>
                            </div>

                            <ul id="listMostSoldItems" class="list-group list-group-flush"></ul>
                        </div>
                    </div>
                </div>

                <div class="bg-white shadow-sm rounded-1 p-3 border bordervm-secondary h-100 d-none">
                    <div class="row align-items-center mb-3">
                        <div class="col-7">
                            <h5><b>Sales by Category</b></h5>

                            <table data-role="none" class="table table-sm">
                                <tbody id="tbodySalesByCategory"></tbody>
                            </table>
                        </div>
                        <div class="col-5">
                            <div class="row align-items-center mb-3">
                                <div class="col">
                                    <h5><b>Most items sold</b></h5>
                                </div>
                                <div class="col text-right">
                                    <h6 class="text-secondary">Top 5 Products</h6>
                                </div>
                            </div>

                            <ul id="listMostSoldItems" class="list-group list-group-flush"></ul>
                        </div>
                    </div>
                </div>
            </div>

            <div id="inventory" subpage="setting" style="display:none;">

            </div>

            <div id="moresettings" subpage="setting" style="display:none;">
                <div class="row align-items-center mb-3 rounded-2 border bg-white p-2 gx-0">
                    <div class="col-2 d-flex justify-content-center">
                        <div class="rounded-circle bgvm-secondary p-4 d-flex align-items-center justify-content-center" style="height: 80px; width: 80px;">
                            <span class="bi bi-pc-display-horizontal textvm-primary" style="font-size: 30px;"></span>
                        </div>
                    </div>
                    <div class="col">
                        <h3>About Machine</h3>
                        <div id="lblMachineVersion" class="text-secondary">Version:</div>
                        <div id="lblMachineDatabaseCreation" class="text-secondary">Database Created:</div>
                        <div id="lblMachineLastSync" class="text-secondary">Last Sync:</div>
                    </div>
                    <div class="col-3 d-flex justify-content-center">
                        <button data-role="none" id="btnVMSyncDB" class="btn btnvm-primary">
                            <span class="bi bi-arrow-clockwise" style="font-size:30px;"></span>
                            <div>Synchronize Database</div>
                        </button>
                    </div>
                </div>
                <br />
                <h5 class="text-secondary">Other Settings</h5>
                <div class="row align-items-center mb-3">
                    <div class="col-2">
                        Display Mode:
                    </div>
                    <div class="col-auto">
                        <div class="form-check form-check-inline">
                            <input class="form-check-input" type="radio" name="display_screen" id="screen_pin" value="pin">
                            <label class="form-check-label" for="screen_pin">Full Screen</label>
                        </div>
                        <div class="form-check form-check-inline">
                            <input class="form-check-input" type="radio" name="display_screen" id="screen_unpin" value="unpin">
                            <label class="form-check-label" for="screen_unpin">Normal</label>
                        </div>
                    </div>
                </div>

                @*<video controls="" width="200" id="myVideo" playsinline="" preload="metadata"><source type="video/mp4" src="/Content/VendingMachine/cfe39df6-8809-4973-9515-810df75aed12/selectionguide_menu_e20e4cb6-421f-4311-bccf-c925b7315641.mp4"></video>*@
            </div>
        </div>
    </main>

</div>