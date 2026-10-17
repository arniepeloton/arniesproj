@Code
    ViewData("Title") = "Vending Machine"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code
@Section head
    <link rel="stylesheet" href="/Content/jquery.buttons.dataTables.min.css" />
    <link rel="stylesheet" href="/Content/jquery.dataTables2.css" />
    <link rel="stylesheet" href="/Content/vdm.css" />

    <script>
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
                require(['/Scripts/sources/VendingMachine/main.js?crc=' + assetversion])
            })
        })

        function scrollNav(scroll) {
            document.getElementById('listMenu').scrollBy({
                left: scroll,
                behavior: 'smooth' // Ensures a smooth gliding animation
            });
        }
    </script>
End Section

<div class="w-100 h-100 position-absolute bgvm-primary-base">
    <div id="btnMoreOptions" style="width:70px; height:70px; position:absolute; z-index:100;">

    </div>

    <div id="page_inappsettings" subpage="main" class="vh-100 w-100" style="display:none;">

    </div>

    <div id="page_passcode" subpage="main" class="vh-100 w-100" style="display:none;">
        <div class="d-flex flex-column justify-content-center align-items-center h-100 textvm-primary text-center overflow-auto">
            <div class="passcode-card p-4 text-center shadow-lg">
                <h5 class="textvm-primary mb-4 d-flex align-items-center justify-content-center gap-2">
                    <i class="bi-lock-fill text-secondary"></i> System Access Passcode
                </h5>

                <div class="d-flex justify-content-center align-items-center gap-3 mb-2" id="dotsContainer">
                    <div class="dot"></div>
                    <div class="dot"></div>
                    <div class="dot"></div>
                    <div class="dot"></div>
                    <div class="dot"></div>
                </div>

                <div class="small text-secondary mb-4" id="statusText" style="height: 20px;">0/5 Digits</div>

                <div class="row gx-3 gy-3 row-cols-3" id="keypadGrid">
                </div>
            </div>
            <button data-role="none" id="btnCancelPasscode" class="btn btnvm-secondary textvm-primary m-5 d-inline-flex align-items-center"><span class="bi-x-circle-fill mr-3"></span>Cancel</button>
        </div>
    </div>

    <div id="page_VMScreen" subpage="main" class="vh-100 w-100" style="display:block;">
        <div id="page_start" subpage="page_VMScreen" class="vh-100 w-100" style="display:block;">
            <div class="d-flex flex-column justify-content-center align-items-center h-100 text-center bgvm-primary text-white">
                <div class="d-flex justify-content-center mb-5">
                    <div class="spinner-border" style="width: 8rem; height: 8rem; border-width:10px;" role="status"></div>
                </div>
                @*<img src="/Content/MobizImagePIM/cfe39df6-8809-4973-9515-810df75aed12/Big/7434bae9-cc35-4cf2-b091-aaa05bf1d474_big.jpg" style="width:100px; height:100px;" class="rounded">
                <img src="/Content/VendingMachine/cfe39df6-8809-4973-9515-810df75aed12/ads_01312025_015214pm_c0d07aa5-5ddb-4d77-8268-6af3f79319c7.jpg" style="width:100px; height:100px;" class="rounded">*@
                @*<video style="margin: auto;width: 200px; height:100px;" controls muted autoplay>
                    <source src="/Content/VendingMachine/cfe39df6-8809-4973-9515-810df75aed12/ads_01312025_015216pm_f43747ce-1f94-4cb9-98a1-dc46023fa3fc.mp4" type="video/mp4">
                </video>
                <video style="margin: auto;width: 200px; height:100px;" src="/Content/VendingMachine/cfe39df6-8809-4973-9515-810df75aed12/ads_01312025_015216pm_f43747ce-1f94-4cb9-98a1-dc46023fa3fc.mp4" muted autoplay>
                </video>*@
                <h3>Checking media library...</h3>
                <button data-role="none" class="btn btn-secondary d-none btnInitDone">Done</button>
            </div>
        </div>

        <div id="page_idle" subpage="page_VMScreen" class="vh-100 w-100 idle" style="display:none;">
            @*<video muted style="margin: auto; max-width: 300px" controls>
                <source src="/Content/VendingMachine/cfe39df6-8809-4973-9515-810df75aed12/ads_03262025_080536am_f43747ce-1f94-4cb9-98a1-dc46023fa3fc.webm" />
            </video>*@
            <div class="slideshow-container">
                <ul class="slides slidesLandscape" id="listIdleAds"></ul>
                <div class="scroll-indicator">
                    <div class="row g-0 justify-content-center">
                        <div class="col-auto">
                            <button data-role="none" id="btnPressToStart" class="btn rounded-pill m-2 bgvm-primary bordervm-primary text-white light-beam navbtn-light font-weight-bold mb-4 pl-5 pr-5"><h3 class="m-0">Touch the screen to start shopping</h3></button>
                        </div>
                    </div>
                </div>
            </div>
            @*<div class="d-flex flex-column justify-content-center align-items-center h-100 text-center bgvm-primary">
                <div class="pt-5 pb-5 text-white" style="font-size:80px;">Hi! <span class="bi bi-emoji-smile"></span></div>
                <div class="pt-5 pb-5 text-white" style="font-size:60px;">Press any key to start</div>
            </div>*@
        </div>

        <div id="page_trans" subpage="page_VMScreen" class="h-100 w-100" style="display:none;">
            <div class="row g-0 h-100">
                <div class="col-3 bg-secondary text-white">
                    @*<button data-role="none" id="btnUnmute" style="left:-1000px;">Unmute</button>*@
                    @*<div style="width:100%;">
                        <video class="w-100" controls muted loop style="object-fit:cover; height:100vh;">
                            <source src="../Content/VM_ads.mp4" type="video/mp4">
                            Your browser does not support the video tag.
                        </video>
                    </div>*@
                    <div class="slideshow-container productselection">
                        <div class="random_ads" subpage="productselection" style="display:none;">
                            <ul class="slides slidesPortrait" id="listAds"></ul>
                        </div>
                    </div>
                    @*<img src="../Content/images/chicken_adobo.jpg" style="width: 100%; height:100%; object-fit:cover;" />*@
                </div>
                <div class="col-9">
                    <div id="" subpage="about_item" class="h-100 w-100 page_menu" style="display:none;">
                        <div class="vh-100 d-flex flex-column overflow-hidden">
                            <div class="flex-shrink-1 bg-white border">
                                <div class="row g-0 text-white m-2 rounded-2">
                                    <div class="col overflow-auto align-self-center" style="scrollbar-width: none;">
                                        <div class="nav-wrapper">
                                            <button data-role="none" class="btn btnvm-secondary textvm-primary left rounded-circle" onclick="scrollNav(-200)"><span class="bi-caret-left-fill"></span></button>

                                            <ul id="listMenu" class="nav-container list-group list-group-horizontal list-unstyled ml-3 mr-3">
                                            </ul>

                                            <button data-role="none" class="btn btnvm-secondary textvm-primary right rounded-circle" onclick="scrollNav(200)"><span class="bi-caret-right-fill"></span></button>
                                        </div>

                                        <div class="d-flex align-items-center position-relative w-100 d-none">
                                            <div class="position-absolute" style="left:0; right:0;">
                                                <div class="position-relative">
                                                    @*<button data-role="none" class="btn btn-secondary btn-xl position-absolute" style="left:0;" id="scroll-left"><span class="bi-chevron-left"></span></button>*@

                                                    @*<button data-role="none" class="btn btn-secondary btn-xl position-absolute" style="right:0;" id="scroll-right"><span class="bi-chevron-right"></span></button>*@
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row g-0 align-items-center h-100 d-none">
                                            <div class="col-5 h-100 p-2">
                                                <div class="d-flex flex-column justify-content-center h-100">
                                                    <form autocomplete="off" class="h-100">
                                                        <input data-role="none" type="text" id="txtSelectedItemNo" class="form-control-lg text-center shadow-none form-control-plaintext bg-white rounded-2" autocomplete="off" placeholder="Please enter item code to select" readonly style="font-size:35px;" />
                                                    </form>
                                                </div>
                                            </div>
                                            <div class="col p-1 text-center flashing-text">
                                                <ul id="listEmptyField" subpage="header" class="list-unstyled pl-0 mb-0" style="display:block;">
                                                    <li>Press item code to select item.</li>
                                                    <li>Press <b>Enter</b> to view your cart.</li>
                                                    @*<li>Press <b>PgUp</b> or <b>PgDn</b> to view items.</li>*@
                                                </ul>
                                                <ul id="listNotEmptyField" subpage="header" class="list-unstyled pl-0 mb-0" style="display:none;">
                                                    <li>Press <b>Enter</b> to select item.</li>
                                                    <li>Press <b>Esc</b> to clear item code.</li>
                                                    @*<li>&nbsp;</li>*@
                                                </ul>
                                            </div>
                                        </div>
                                    </div>
                                    @*<div class="col-1">
                                        <div class="d-flex flex-column align-items-center justify-content-center h-100">
                                            <div style="border: 3px solid; height:50%; width: 0px; background-color: #fff;"></div>
                                        </div>
                                    </div>*@
                                    <div class="col-3 p-1 d-flexx flex-column justify-content-center text-center btnViewOrder d-none" style="font-size:25px; font-weight:bold;">
                                        <div class="d-flex flex-column justify-content-center h-100">
                                            <div class="w-100">
                                                <div class="row pr-2 g-0 align-items-center">
                                                    <div class="col-4">
                                                        Items:
                                                    </div>
                                                    <div class="col text-right txtLineItem">
                                                        0
                                                    </div>
                                                </div>
                                                <div class="row pr-2 g-0 align-items-center rounded">
                                                    <div class="col-4">
                                                        Total:
                                                    </div>
                                                    <div class="col text-right txtCartTotal">
                                                        P 0.00
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        @*<div >
                                            TOTAL:
                                        </div>
                                        <div >P 0.00</div>*@
                                    </div>
                                </div>
                            </div>
                            <div id="containerSKUs" class="d-flex flex-fill align-items-start flex-column overflow-auto w-100 position-relative">
                                <div id="listofskus" class="p-4 w-100"></div>
                            </div>
                            <div class="flex-shrink-1 border">
                                <div class="scroll-indicatorx position-sticky bottom-0 bg-white w-100" style="margin-top:auto;">
                                    <div class="row g-0 justify-content-center align-items-center text-center">
                                        <div class="col-auto p-3">
                                            Total:
                                        </div>
                                        <div class="col-auto p-3">
                                            <h3 class="txtCartTotal font-weight-bold m-0">P 0.00</h3>
                                            @*<button data-role="none" class="btn rounded-pill m-2 bg-white bordervm-primary textvm-primary light-beam navbtn-dark font-weight-bold"><span><span class="arrow-bounce up"></span> PgUp</span></button>*@
                                        </div>
                                        <div class="col-auto p-3 text-right">
                                            <button id="btnMenuViewOrder" data-role="none" class="btn btnvm-primary rounded-2 btnViewOrder" style="min-width:200px;"><span class="bi-cart4 mr-3"></span> View Cart <span class="txtLineItem badge bgvm-secondary textvm-primary ml-3">0</span></button>
                                        </div>
                                        @*<div class="col-auto p-3 text-right">
                                            <button id="btnOpenDoor" data-role="none" class="btn btn-info rounded-2">Open Door</button>
                                        </div>
                                        <div class="col-auto p-3 text-right">
                                            <button id="btnCloseDoor" data-role="none" class="btn btn-info rounded-2">Close Door</button>
                                        </div>
                                        <div class="col-auto p-3 text-right">
                                            <button id="btnStatusBottle" data-role="none" class="btn btn-info rounded-2">Status Bottle</button>
                                        </div>*@
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="" class="h-100 w-100 page_productinfo" subpage="about_item" style="display:none;">
                        <div class="d-flex flex-column justify-content-center align-items-center h-100">
                            <div style="width:90%">
                                @*<div class="mb-3 text-info"><span class="bi bi-info-circle"></span>Please note that you may select only one variety of rice per transaction. If you wish to select multiple varieties, you may do so in separate transactions.</div>*@
                                <div id="lblProductName" class="font-weight-bold mb-3 text-center" style="font-size:40px; line-height:1;">Chicken Adobo</div>

                                <div class="row align-items-center">
                                    <div class="col-4">
                                        <img id="imgProductImage" class="img-thumbnail rounded-2" style="object-fit:cover;" />
                                    </div>
                                    <div class="col">
                                        <div id="divDefaultSelection" subpage="pageSelection" style="display:block;">
                                            <div class="row align-items-center mb-3">
                                                <div class="col">
                                                    <h3 class="text-secondary mb-0">Price:</h3>
                                                </div>
                                                <div class="col-4 text-center">
                                                    <h3 id="lblProductPrice" class="text-secondary mb-0">0.00</h3>
                                                </div>
                                            </div>

                                            <div class="row align-items-center mb-3">
                                                <div class="col">
                                                    <h3 class="text-secondary mb-0">Available:</h3>
                                                </div>
                                                <div class="col-4 text-center">
                                                    <h3 class="text-secondary mb-0 lblProductAvailableQty">0.00</h3>
                                                </div>
                                            </div>

                                            <div class="row align-items-center mb-3">
                                                <div class="col">
                                                    <h3 class="text-secondary mb-0">Order Quantity:</h3>
                                                    @*<div class="text-secondary pl-4 text-left" style="font-size:20px;">
                                                        <ul>
                                                            <li>Use keypad to type order quantity.</li>
                                                        </ul>
                                                    </div>*@
                                                </div>
                                                <div class="col-4">
                                                    <form autocomplete="off">
                                                        <div class="text-center rounded-2 border bg-white">
                                                            <div class="gradient-wrapperx input-group">
                                                                <button data-role="none" id="btnOrderReduce" class="btn textvm-primary m-2 rounded-2 bg-light" style="font-size:20px;"><span class="bi-dash-lg"></span></button>
                                                                <input data-role="none" type="text" id="txtOrderQty" class="form-control form-control-plaintext form-control-lg text-center shadow-none bg-white gradient-borderx" readonly autocomplete="off" style="font-size:20px;" />
                                                                <button data-role="none" id="btnOrderIncrease" class="btn textvm-primary m-2 rounded-2 bgvm-secondary" style="font-size:20px;"><span class="bi-plus-lg"></span></button>
                                                            </div>
                                                            <span class="text-danger lblToRemoveItem">Press <b>0</b> to remove this item</span>
                                                        </div>
                                                    </form>
                                                </div>
                                            </div>
                                        </div>

                                        <div id="divUomSelection" subpage="pageSelection" style="display:none; font-size:25px; text-align:center;">
                                            <div class="row align-items-center mb-3">
                                                <div class="col text-left">
                                                    <h3 class="text-secondary mb-0">Available:</h3>
                                                </div>
                                                <div class="col-4 text-center">
                                                    <h3 class="text-secondary mb-0 lblProductAvailableQty">0.00</h3>
                                                </div>
                                            </div>
                                            <h3 class="w-100 mb-3 text-left text-secondary">Choose quantity:</h3>
                                            <div id="listUom">
                                                <div class="border rounded p-4 text-center text-secondary">
                                                    1/4 kg = 23.00
                                                </div>
                                            </div>
                                            <span class="text-danger lblToRemoveItem">Press <b>0</b> to remove this item</span>
                                        </div>
                                    </div>
                                </div>

                                <div id="divErrMsgProductInfo" class="mt-5 alert alert-danger text-center" style="display:none;">
                                    <h5 class="mb-0">The quantity entered exceeds what is available.</h5>
                                </div>

                                <div class="row justify-content-end mt-5 text-center text-secondary">
                                    <div class="col-4">
                                        <button data-role="none" class="btn btn-light border rounded-2 btn-block mb-3 d-flex align-items-center justify-content-center btnBackToMenu" style="font-size:20px;"><span class="bi-chevron-left mr-3"></span> Back</button>
                                    </div>
                                    <div class="col-4">
                                        <button data-role="none" id="btnAddToCart" class="btn bgvm-primary text-white border rounded-2 btn-block mb-3 d-flex align-items-center justify-content-center" style="font-size:20px;"><span class="bi-cart4 mr-3"></span> Add to Cart</button>
                                    </div>
                                    @*<div class="col">
                                        <h3>Press <button data-role="none" class="light-beam navbtn-dark"><span>Esc</span></button> to go back</h3>
                                    </div>
                                    <div class="col">
                                        <h3>Press <button data-role="none" class="light-beam navbtn-dark"><span>Enter</span></button> to confirm</h3>
                                    </div>*@
                                </div>
                            </div>
                        </div>
                    </div>

                    @*<div id="page_itemadded" subpage="about_item" style="display:none;">
                        <div class="row g-0 justify-content-center bgvm-primary-gradient text-white">
                            <div class="col-8">
                                <div class="vh-100 d-flex align-items-center text-center overflow-hidden" style="font-size:60px;">
                                    <div class="w-100 product_itemadded">
                                        <img src="/Content/images/icons/basket.gif" style="width:300px;" class="mb-5" />

                                        <div class="mb-5">Item added to the cart</div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>*@

                    <div subpage="about_item" class="h-100 w-100 page_Cart" style="display:none; overflow:hidden;">
                        <div class="row g-0">
                            <div class="col-8 p-4 pb-0 pt-0">
                                <div class="vh-100 d-flex flex-column overflow-hidden">
                                    <div class="flex-shrink-1">
                                        <h2 class="pt-3 pb-3 mb-0 font-weight-bold">Order Details</h2>
                                        @*<div class="text-info mb-3" style="font-size:25px;">
                                            <ul class="listInstructions">
                                                <li class="m-0 mb-2">Press the number beside the item you want to edit.</li>
                                            </ul>
                                        </div>*@
                                    </div>
                                    <div class="d-flex flex-fill align-items-start flex-column overflow-auto w-100 position-relative">
                                        <div id="divCartItems" class="w-100"></div>
                                        <div class="scroll-indicator d-none">
                                            <div class="row g-0 justify-content-center">
                                                <div class="col-auto">
                                                    <button data-role="none" class="btn rounded-pill m-2 bg-white bordervm-primary textvm-primary light-beam navbtn-dark font-weight-bold"><span><span class="arrow-bounce down"></span> PgDn</span></button>
                                                </div>
                                                <div class="col-auto">
                                                    <button data-role="none" class="btn rounded-pill m-2 bg-white bordervm-primary textvm-primary light-beam navbtn-dark font-weight-bold"><span><span class="arrow-bounce up"></span> PgUp</span></button>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="flex-shrink-1">

                                    </div>
                                </div>
                            </div>
                            <div class="col pl-4 pr-4 bg-white">
                                <div class="d-flex align-items-start flex-column bd-highlight h-100">
                                    <div class="mb-auto p-2 w-100 text-center bd-highlight">
                                        <h1 class="mb-3 pt-3 text-secondary">ITEMS:</h1>
                                        <h3 class="m-0 font-weight-bold textvm-primary txtLineItem" style="font-size:40px;">0</h3>
                                        <h1 class="mb-3 pt-3 text-secondary">TOTAL:</h1>
                                        <h3 class="m-0 font-weight-bold textvm-primary txtCartTotal" style="font-size:40px;">P 0.00</h3>
                                        @*<button data-role="none" class="btn btnBackToMenu btn-light border btn-xl btn-block shadow-sm" style="font-size:35px; min-height:120px;">Add More Items</button>*@
                                    </div>
                                    <div class="mt-auto mb-5 p-2 w-100 text-center bd-highlight">
                                        <button data-role="none" id="btnPayNow" class="btn btnvm-primary btn-xl btn-block shadow-sm navbtn-light rounded-2 font-weight-bold d-flex align-items-center justify-content-center" style="font-size:25px; min-height:120px;"><span class="bi-cash mr-3"></span> Pay Now</button>
                                        <button data-role="none" class="btn btn-xl btn-light textvm-primary btn-block shadow-sm navbtn-dark rounded-2 d-flex align-items-center justify-content-center btnBackToMenu" style="font-size:25px; min-height:120px;"><span class="bi-plus-circle-fill mr-3"></span> Add more items</button>
                                        @*<button data-role="none" id="btnPayNow" class="btn btnvm-primary btn-xl btn-block shadow-sm light-beam navbtn-light" style="font-size:25px; min-height:120px;"><span>Press<br /><b style="font-size:35px;">Enter</b><br /> to pay</span></button>
                                        <button data-role="none" class="btn btn-xl btn-block shadow-sm border light-beam navbtn-dark" style="font-size:20px; min-height:120px;"><span>Press<br /><b style="font-size:20px;">Esc</b><br /> to shop for more items</span></button>*@
                                    </div>
                                    @*<div class="p-2 w-100 bd-highlight listInstructions text-center">

                                    </div>*@
                                </div>
                            </div>
                        </div>
                    </div>

                    <div subpage="about_item" class="h-100 w-100 page_paynow" style="display:none;">
                        <div class="d-flex flex-column justify-content-center align-items-center bgvm-primary h-100 text-center text-white">
                            <div class="pb-5 font-weight-bold" style="font-size:60px;">Insert your Cash</div>

                            <div>
                                <div class="row g-5">
                                    <div class="col-auto">
                                        <div class="rounded-2 text-center" style="width:300px; padding:25px;">
                                            <div style="font-size: 20px; margin-bottom:25px;">Amount to Pay</div>
                                            <hr />
                                            <div id="lblAmountToPay" style="margin-top:25px; font-size:40px; font-weight:bold;">500.00</div>
                                        </div>
                                    </div>
                                    <div class="col-auto">
                                        <div class="rounded-2 text-center textvm-primary shadow-lg" style="width:300px; padding:25px; border: 1px solid #FFF; background-color: #FFF;">
                                            <div style="font-size: 20px; margin-bottom:25px;">Amount Inserted</div>
                                            <hr />
                                            <div id="lblAmountInserted" style="margin-top:25px; font-size:40px; font-weight:bold;">500.00</div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="page_paid" subpage="about_item" class="h-100 w-100" style="display:none;">
                        <div class="d-flex flex-column justify-content-center align-items-center h-100 text-center textvm-primary">
                            <div class="pb-5 font-weight-bold" style="font-size:60px;">Paid Successfully</div>
                            <div class="d-flex justify-content-center">
                                <div class="spinner-border" style="width: 8rem; height: 8rem; border-width:10px;" role="status"></div>
                            </div>
                            <div class="pt-5 pb-5" style="font-size:40px;">Please wait a moment, we're <br /> preparing your order...</div>
                            <div class="pt-5 pb-5 d-none" style="font-size:20px;">It's taking longer than usual, attempting to dispense the item again...</div>
                        </div>
                    </div>

                    <div subpage="about_item" class="h-100 w-100 page_dispense" style="display:none;">
                        <div class="d-flex flex-column align-items-center h-100">
                            <div style="width:70%">
                                <h1 style="font-size:40px;" class="text-dark text-center mb-5 mt-5">Dispensing Now</h1>
                                <table data-role="none" class="table table-light">
                                    <thead>
                                        <tr class="text-center text-secondary">
                                            <th class="border-bottom-0"></th>
                                            <th class="border-bottom-0">Item Description</th>
                                            <th class="border-bottom-0" style="width:170px;">Ordered</th>
                                            <th class="border-bottom-0" style="width:170px;">Dispensed</th>
                                            <th class="border-bottom-0" style="width:40px;"></th>
                                        </tr>
                                    </thead>
                                    <tbody id="tbodyDispenseItems">
                                        <tr class="textvm-primary font-weight-bold" productid="0d43d1c8-e2b9-4f0f-b4ae-7276d33516d9">
                                            <td class="text-center"><img src="/Content/MobizImagePIM/cfe39df6-8809-4973-9515-810df75aed12/Big/ffbadb57-7548-4c7f-ae9a-11f3730ad24c_big.jpg" id="skuimg_edbf85fb-7228-4499-91bc-4591dd9a64ae" style="width:70px;" class="rounded-2 "></td>
                                            <td class="text-center">Candy Pizza</td>
                                            <td class="text-center">1</td>
                                            <td class="text-center tdDispensedQty">1</td>
                                            <td class="text-center"><span class="bi-check-circle-fill"></span></td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>

                    <div subpage="about_item" class="h-100 w-100 page_confirmation" style="display:none;">
                        <div class="d-flex flex-column justify-content-center align-items-center h-100 text-center">
                            <div>
                                <h1 style="font-size:55px;" class="text-dark mb-5">Are you sure you want to remove <br /><b id="lblRemoveProductName"></b>?</h1>
                                <div class="row justify-content-center mt-5 text-center text-secondary">
                                    <div class="col-auto">
                                        <button data-role="none" id="btnCancelRemoveItem" class="btn btn-light border rounded-2 btn-block mb-3 btnBackToMenu d-inline-flex align-items-center" style="font-size:20px;"><span class="bi-arrow-left mr-3"></span> Cancel</button>
                                    </div>
                                    <div class="col-auto">
                                        <button data-role="none" id="btnRemoveItem" class="btn bg-danger text-white rounded-2 btn-block mb-3 d-inline-flex align-items-center" style="font-size:20px;"><span class="bi-trash mr-3"></span> Delete item</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div subpage="about_item" class="h-100 w-100 page_confirmpay" style="display:none;">
                        <div class="d-flex flex-column justify-content-center align-items-center h-100 text-center">
                            <div>
                                <h1 style="font-size:55px;" class="text-dark mb-5">Are you sure you want to pay now?</h1>
                                <div class="row align-items-center text-center text-secondary mt-5 listInstructions">
                                    <div class="col">
                                        <h3>Press <button data-role="none" class="light-beam navbtn-dark"><span>Esc</span></button> to cancel</h3>
                                    </div>
                                    <div class="col">
                                        <h3>Press <button data-role="none" class="light-beam navbtn-dark"><span>Enter</span></button> to confirm</h3>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div subpage="about_item" class="h-100 w-100 page_cartaction" style="display:none;">
                        <div class="d-flex flex-column justify-content-center align-items-center h-100 text-center">
                            <div>
                                <h1 style="font-size:55px;" class="text-dark mb-5"></h1>
                                <div class="row row-cols-1 gy-5 align-items-center text-center text-secondary mt-5 listInstructions">
                                    <div class="col">
                                        <h3>Press <span>PgUp</span> to edit</h3>
                                    </div>
                                    <div class="col">
                                        <h3>Press <span>PgDn</span> to remove</h3>
                                    </div>
                                    <div class="col">
                                        <h3>Press <span>Esc</span> to cancel</h3>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div subpage="about_item" class="h-100 w-100 page_updatechosenuom" style="display:none;">
                        <div class="d-flex flex-column justify-content-center align-items-center h-100 text-center pl-5 pr-5">
                            <div>
                                @*<h1 style="font-size:40px;" class="text-dark mb-5"></h1>*@
                                <div class="alert alert-primary d-flex align-items-center mb-5" style="font-size:25px;">
                                    <span class="bi-info-circle-fill"></span>
                                    <div>We only allow one variety of this item per transaction. If you wish to select multiple varieties, please make separate transactions.</div>
                                </div>
                                <div class="row align-items-center">
                                    <div class="col-3">
                                        <img id="imgOriginalProduct" class="img-thumbnail rounded-2" style="object-fit:cover;" />
                                    </div>
                                    <div class="col-1">
                                        <span class="bi-arrow-right"></span>
                                    </div>
                                    <div class="col-4 text-center">
                                        <h1 class="mb-5">Switch Item?</h1>
                                        <h3>Would you like to replace Hard Soap to Liquid Soap?</h3>
                                    </div>
                                    <div class="col-1">
                                        <span class="bi-arrow-right"></span>
                                    </div>
                                    <div class="col-3">
                                        <img id="imgReplacementProduct" class="img-thumbnail rounded-2" style="object-fit:cover;" />
                                    </div>
                                </div>
                                <div class="row justify-content-center text-center text-secondary mt-5">
                                    <div class="col-auto">
                                        <button data-role="none" id="btnKeepOriginal" class="btn btn-light border rounded-2 btn-block mb-3 d-inline-flex align-items-center" style="font-size:20px;"><span class="bi-x-circle mr-3"></span> No, keep original</button>
                                    </div>
                                    <div class="col-auto">
                                        <button data-role="none" id="btnReplaceOriginal" class="btn bgvm-primary text-white rounded-2 btn-block mb-3 d-inline-flex align-items-center" style="font-size:20px;"><span class="bi-check-circle-fill mr-3"></span> Yes, replace</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="page_additonalitem" subpage="about_item" class="h-100 w-100" style="display:none;">
                        <div class="d-flex flex-column justify-content-center align-items-center h-100 text-center">
                            <div style="width:70%">
                                <div class="row align-content-center justify-content-center mb-5">
                                    <div class="col-4">
                                        <img id="imgMainSKU" class="img-thumbnail rounded-2" style="object-fit:cover;" />
                                    </div>
                                    <div class="col-1 align-self-center">
                                        <span class="bi-plus-lg" style="font-size:40px;"></span>
                                    </div>
                                    <div class="col-4">
                                        <img id="imgAddOnSKU" class="img-thumbnail rounded-2" style="object-fit:cover;" />
                                    </div>
                                </div>
                                <h1 style="font-size:40px;" class="text-dark mb-5">Add a plastic bottle for P10.00?</h1>
                                <div class="row align-items-center text-center text-secondary mt-5">
                                    <div class="col">
                                        <button data-role="none" id="btnUseOwn" class="btn btnvm-secondary textvm-primary rounded-2 btn-block mb-3" style="font-size:20px;">No, I'll use my own</button>
                                    </div>
                                    <div class="col">
                                        <button data-role="none" id="btnAddRecomSKU" class="btn bgvm-primary text-white rounded-2 btn-block mb-3" style="font-size:20px;">Yes, I'll buy a bottle</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="page_guides" subpage="page_VMScreen" class="h-100 w-100" style="display:none;">
            <div id="divStepTemplate" class="row g-0 h-100" subpage="page_guides" style="display:none;">
                <div class="d-flex justify-content-center text-center h-100">
                    <div class="imgguides w-100">
                        <img class="h-100 d-none" style="margin: auto; width: 100%; object-fit:cover;" />
                        <video controls class="rounded h-100 mw-100 d-none" style="object-fit:cover;" playsinline></video>
                        <div class="row g-0 h-100 w-100 top-0 vid_info">
                            <div class="col-8 p-4 pb-0 pt-0">
                                <div class="header textContainer bgvm-primary shadow p-3 rounded">Sample Text</div>
                            </div>
                            <div class="col pl-4">
                                <div class="d-flex justify-content-center align-items-center flex-column bd-highlight h-100">
                                    <div class="rounded text-center bgvm-primary mb-3" style="width:300px; padding:25px;">
                                        <div style="font-size: 20px; margin-bottom:25px;">Ordered</div>
                                        <hr />
                                        <div class="lblOrderedWeight" style="margin-top:25px; font-size:20px; font-weight:bold;"></div>
                                    </div>

                                    <div class="rounded text-center textvm-primary shadow-lg" style="width:300px; padding:25px; border: 1px solid #FFF; background-color: #FFF;">
                                        <div style="font-size: 20px; margin-bottom:25px;">Dispensing Weight</div>
                                        <hr />
                                        <div class="lblDispensingWeight" style="margin-top:25px; font-size:20px; font-weight:bold;"></div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="fixed-bottom d-none justify-content-center align-items-center gap-3">
                            <button data-role="none" class="btnNextStep btn bgvm-primary text-white shadow rounded-2 mb-3 d-inline-flex align-items-center" style="font-size:20px;">Next <span class="bi-arrow-right-circle-fill ml-3"></span></button>
                        </div>
                        <div class="fixed-bottom d-none justify-content-center align-items-center gap-3">
                            <button data-role="none" class="btnStop btn btn-danger shadow rounded-2 mb-3 d-inline-flex align-items-center"><span class="bi-stop-fill mr-3"></span> Stop</button>
                            <button data-role="none" disabled class="btnContinue btn bgvm-primary shadow text-white rounded-2 mb-3 d-inline-flex align-items-center"><span class="bi-play-fill mr-3"></span> Continue</button>
                            <button data-role="none" class="btn btn-light border shadow textvm-primary rounded-2 mb-3 d-inline-flex align-items-center text-nowrap">Dispensing: <span class="ml-3 font-weight-bold lblDispensingVolume">0 ml</span></button>
                        </div>
                        @*<div class="header textContainer">Sample Text</div>
                            <div class="footer">
                                <div class="row align-items-center">
                                    <div class="col dispenseinfox"></div>
                                    <div class="col-3">Page Up</div>
                        <div class="col-3">Page Down</div>
                                </div>
                            </div>*@
                    </div>
                </div>

                @*<div class="col-6 bgvm-primary text-white">
                    <div class="vh-100 d-flex flex-column overflow-hidden" style="font-size:20px;">
                        <div class="flex-shrink-1 text-center">
                            <h1 class="pt-3 pb-3 font-weight-bold">Instructions</h1>
                        </div>
                        <div class="d-flex flex-fill align-items-start flex-column overflow-auto w-100">
                            <div class="fit-text textContainer">
                                Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer tincidunt porttitor augue, vel mollis leo dapibus pellentesque. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer tincidunt porttitor augue, vel mollis leo dapibus pellentesque. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer tincidunt porttitor augue, vel mollis leo dapibus pellentesque. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer tincidunt porttitor augue, vel mollis leo dapibus pellentesque. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer tincidunt porttitor augue, vel mollis leo dapibus pellentesque. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer tincidunt porttitor augue, vel mollis leo dapibus pellentesque. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer tincidunt porttitor augue, vel mollis leo dapibus pellentesque. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer tincidunt porttitor augue, vel mollis leo dapibus pellentesque. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer tincidunt porttitor augue, vel mollis leo dapibus pellentesque. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer tincidunt porttitor augue, vel mollis leo dapibus pellentesque.
                            </div>
                        </div>
                        <div class="flex-shrink-1 textvm-primary bg-white">
                            <div class="row text-center mt-4 mb-4 listInstructions">
                                <div class="col">
                                    Press <span>PAGE DOWN</span> to go back
                                </div>
                                <div class="col">
                                    Press <span>PAGE UP</span> to go next
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-6">
                    <div class="d-flex flex-column justify-content-center text-center h-100">
                        <img class="w-100" />
                    </div>
                </div>*@
            </div>
            @*<div id="divGuidesIntro" class="divStep h-100 bgvm-primary text-white" subpage="page_guides" style="display:none;">
                <div class="d-flex flex-column justify-content-center text-center h-100">
                    <h1>Introduction here</h1>
                </div>
            </div>
            <div id="divGuidesLast" class="divStep h-100 bgvm-primary text-white" subpage="page_guides" style="display:none;">
                <div class="d-flex flex-column justify-content-center text-center h-100">
                    <h1>Are your ready?</h1>
                    <h3 class="listInstructions" style="margin-top:100px;">Press <span>Enter</span> confirm</h3>
                </div>
            </div>*@
        </div>

        <div id="page_screentimeout" subpage="page_VMScreen" class="h-100 w-100" style="display:none;">
            <div class="d-flex flex-column justify-content-center align-items-center h-100 textvm-primary text-center">
                <div style="width:60%">
                    <div class="row align-items-center">
                        <div class="col mb-4">
                            <span class="bi-hourglass-split" style="font-size:150px;"></span>
                            <h2 class="mt-3"><b>Are you still there?</b><br />Your session is about to end due to inactivity.</h2>
                        </div>
                        <div class="col-4 mb-4">
                            <div class="timer-section">
                                <div class="timer-container">
                                    <svg class="timer-svg">
                                        <circle class="timer-bg" cx="100" cy="100" r="85"></circle>
                                        <circle class="timer-progress" id="progressCircle" cx="100" cy="100" r="85"></circle>
                                    </svg>
                                    <div class="timer-text-container">
                                        <div class="timer-label">Session ends in</div>
                                        <div class="time-display" id="timeText">00:30</div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row align-items-center justify-content-center mt-4">
                        <div class="col-auto">
                            <button data-role="none" class="btn btnvm-primary extendBtn" style="font-size:20px;"><span class="bi-check-circle mr-3"></span> Yes, I need more time</button>
                        </div>
                        <div class="col-auto">
                            <button data-role="none" class="btn bgvm-secondary textvm-primary doneBtn" style="font-size:20px;"><span class="bi-x-circle-fill mr-3"></span> No, end session</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="page_sessionends" subpage="page_VMScreen" class="h-100 w-100" style="display:none;">
            <div class="d-flex flex-column justify-content-center align-items-center h-100 textvm-primary text-center">
                <div style="width:60%">
                    <div class="bi-person-raised-hand mb-5" style="font-size: 8rem;"></div>
                    <h3>Have a Great Day!</h3>
                    <h4>Thank you for stopping by!</h4>
                </div>
            </div>
        </div>

        <div id="page_settings_pin" subpage="page_VMScreen" class="h-100 w-100" style="display:none;">
            <div class="d-flex flex-column justify-content-center align-items-center h-100 text-center">
                <div style="width:60%">
                    <h1 style="font-size:40px;" class="textvm-primary mb-5">Enter your PIN</h1>
                    <h3 style="font-size:25px;" class="text-dark text-center mb-3"></h3>
                    <div class="p-3 text-center text-danger" id="lblPinMsg" style="display:none;"></div>
                    <div class="row align-items-center text-center text-secondary mt-3 justify-content-center">
                        <div class="col-auto pin">
                            <input data-role="none" type="password" class="form-control form-control-lg text-center" style="width:70px; font-size:40px" readonly />
                        </div>
                        <div class="col-auto pin">
                            <input data-role="none" type="password" class="form-control form-control-lg text-center" style="width: 70px; font-size: 40px" readonly />
                        </div>
                        <div class="col-auto pin">
                            <input data-role="none" type="password" class="form-control form-control-lg text-center" style="width: 70px; font-size: 40px" readonly />
                        </div>
                        <div class="col-auto pin">
                            <input data-role="none" type="password" class="form-control form-control-lg text-center" style="width: 70px; font-size: 40px" readonly />
                        </div>
                        <div class="col-auto pin">
                            <input data-role="none" type="password" class="form-control form-control-lg text-center" style="width: 70px; font-size: 40px" readonly />
                        </div>
                    </div>
                    <div class="row align-items-center text-center text-secondary mt-3 listInstructions">
                        <div class="col">
                            <h3>Press <button data-role="none" class="light-beam navbtn-dark"><span>Esc</span></button> to cancel</h3>
                        </div>
                        <div class="col">
                            <h3>Press <button data-role="none" class="light-beam navbtn-dark"><span>Enter</span></button> to confirm</h3>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="page_settings_options" subpage="page_VMScreen" class="h-100 w-100" style="display:none;">
            <div class="vh-100 d-flex flex-column textvm-primary align-items-center overflow-hidden">
                <div class="flex-shrink-1 text-center">
                    <h1 class="pt-3 pb-3 mb-0 font-weight-bold">Press the number beside the option to select</h1>
                </div>
                <div class="d-flex flex-fill overflow-auto w-100 align-items-start justify-content-center">
                    <ul id="listOptions" class="list-unstyled pl-0"></ul>
                </div>
                <div class="flex-shrink-1 text-center">
                    <div class="row align-items-center text-center text-secondary mt-3 mb-3 listInstructions">
                        <div class="col">
                            <h3>Press <button data-role="none" class="light-beam navbtn-dark"><span>Esc</span></button> to go back</h3>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="page_paymentcancelled" subpage="page_VMScreen" class="h-100 w-100" style="display:none;">
            <div class="d-flex flex-column justify-content-center align-items-center h-100 textvm-primary text-center">
                <div style="width:60%">
                    <div class="bi-info-circle-fill mb-5 text-warning" style="font-size: 8rem;"></div>
                    <h3>You’ve cancelled your transaction.</h3>
                </div>
            </div>
        </div>

        <div id="page_cashchange" subpage="page_VMScreen" class="h-100 w-100" style="display:none;">
            <div class="d-flex flex-column justify-content-center align-items-center h-100 textvm-primary text-center">
                <div style="width:60%">
                    <div class="d-flex justify-content-center mb-5">
                        <div class="spinner-border" style="width: 8rem; height: 8rem; border-width:10px;" role="status"></div>
                    </div>
                    <h3>Please wait while we dispense your change...</h3>
                </div>
            </div>
        </div>

        <div id="page_cashchangeerror" subpage="page_VMScreen" class="h-100 w-100" style="display:none;">
            <div class="d-flex flex-column justify-content-center align-items-center h-100 textvm-primary text-center">
                <div style="width:60%">
                    <div class="bi-exclamation-circle-fill mb-5 text-danger" style="font-size: 8rem;"></div>
                    <h3></h3>
                </div>
            </div>
        </div>

        <div id="page_cashchangecompleted" subpage="page_VMScreen" class="h-100 w-100" style="display:none;">
            <div class="d-flex flex-column justify-content-center align-items-center h-100 textvm-primary text-center">
                <div style="width:60%">
                    <div class="bi-check-circle-fill mb-5" style="font-size: 8rem;"></div>
                    <h3>Your change is ready! Please take it!</h3>
                </div>
            </div>
        </div>

        <div id="page_pickupitems" subpage="page_VMScreen" class="h-100 w-100" style="display:none;">
            <div class="d-flex flex-column justify-content-center align-items-center h-100 textvm-primary text-center">
                <div style="width:60%">
                    <div class="row align-items-center">
                        <div class="col mb-4">
                            <div class="bi-basket mb-5" style="font-size:8rem;"></div>
                            <h2>Your goods are ready for pick-up from the basket below.</h2>
                        </div>
                        <div class="col-4 mb-4">
                            <div class="timer-section">
                                <div class="timer-container">
                                    <svg class="timer-svg">
                                        <circle class="timer-bg" cx="100" cy="100" r="85"></circle>
                                        <circle class="timer-progress" id="progressCircle" cx="100" cy="100" r="85"></circle>
                                    </svg>
                                    <div class="timer-text-container">
                                        <div class="timer-label">Door closes in</div>
                                        <div class="time-display" id="timeText">00:30</div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row align-items-center justify-content-center mt-4">
                        <div class="col-auto">
                            <button data-role="none" class="btn btnvm-primary extendBtn" style="font-size:20px;"><span class="bi-stopwatch mr-3"></span> TAP HERE TO EXTEND TIME</button>
                        </div>
                        <div class="col-auto">
                            <button data-role="none" class="btn bgvm-secondary textvm-primary doneBtn" style="font-size:20px;"><span class="bi-check-circle-fill mr-3"></span> I'M DONE</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="page_ordercomplete" subpage="page_VMScreen" class="h-100 w-100" style="display:none;">
            <div class="d-flex flex-column justify-content-center align-items-center h-100 textvm-primary text-center">
                <div style="width:60%">
                    <div class="bi-emoji-smile mb-5" style="font-size:8rem;"></div>
                    <h3>Your order is now complete.<br />Thank you for your purchase!</h3>
                </div>
            </div>
        </div>

        <div id="page_insertbottle" subpage="page_VMScreen" class="h-100 w-100" style="display:none;">
            <div class="d-flex flex-column justify-content-center align-items-center h-100 textvm-primary text-center">
                <div style="width:60%">
                    <img src="/Content/images/InsertingBottle.png" width="300" />
                    <h3 class="mb-5">Insert your bottle in the dispensing bay.<br />Once ready, tap the button below.</h3>
                    <button data-role="none" id="btnStartDispenseLiquid" class="btn btnvm-primary" style="font-size:20px;"><span class="bi-check-circle mr-3"></span> Start Now</button>
                </div>
            </div>
        </div>

        <div id="page_removebottle" subpage="page_VMScreen" class="h-100 w-100" style="display:none;">
            <div class="d-flex flex-column justify-content-center align-items-center h-100 textvm-primary text-center">
                <div style="width:60%">
                    <div class="row align-items-center">
                        <div class="col mb-4">
                            <img src="/Content/images/RemovingBottle.png" width="300" />
                            <h2><b>Dispensing complete!</b><br /><br />Please remove your bottle from the bay and tap the <b>DONE</b> button below to finish.</h2>
                        </div>
                        <div class="col-4 mb-4">
                            <div class="timer-section">
                                <div class="timer-container">
                                    <svg class="timer-svg">
                                        <circle class="timer-bg" cx="100" cy="100" r="85"></circle>
                                        <circle class="timer-progress" id="progressCircle" cx="100" cy="100" r="85"></circle>
                                    </svg>
                                    <div class="timer-text-container">
                                        <div class="timer-label">Door closes in</div>
                                        <div class="time-display" id="timeText">00:30</div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row align-items-center justify-content-center mt-4">
                        <div class="col-auto">
                            <button data-role="none" class="btn btnvm-primary extendBtn" style="font-size:20px;"><span class="bi-stopwatch mr-3"></span> TAP HERE TO EXTEND TIME</button>
                        </div>
                        <div class="col-auto">
                            <button data-role="none" class="btn bgvm-secondary textvm-primary doneBtn" style="font-size:20px;"><span class="bi-check-circle-fill mr-3"></span> I'M DONE</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="page_cancelledtrans" subpage="page_VMScreen" class="h-100 w-100" style="display:none;">
            <div class="d-flex flex-column justify-content-center align-items-center h-100 text-center">
                <div class="d-flex justify-content-center mb-5 textvm-primary">
                    <div class="spinner-border" style="width: 8rem; height: 8rem; border-width:10px;" role="status"></div>
                </div>
                <h3 class="text-secondary">We're cancelling your transaction.<br />Please wait a moment...</h3>
            </div>
        </div>

        <div id="page_flushedcoins" subpage="page_VMScreen" class="h-100 w-100" style="display:none;">
            <div class="vh-100 d-flex flex-column align-items-center overflow-hidden">
                <div class="d-flex flex-fill align-items-center justify-content-center overflow-auto w-100">
                    <div class="desc text-center textvm-primary"><h1>TESTING</h1></div>
                </div>
                <div class="flex-shrink-1 text-center">
                    <div class="row align-items-center text-center text-secondary mt-3 mb-3 listInstructions">
                        <div class="col">
                            <h3>Press <button data-role="none" class="light-beam navbtn-dark"><span>Esc</span></button> to cancel</h3>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="page_failedorder" subpage="page_VMScreen" class="h-100 w-100" style="display:none;">
            <div class="d-flex flex-column justify-content-center align-items-center h-100 textvm-primary text-center">
                <div style="width:80%">
                    <div class="row justify-content-center">
                        <div class="col-5">
                            <h3 style="font-size:30px;" class="text-dark text-center mb-5 mt-5">Due to some unforeseen issues, some of your items didn’t dispense properly. <b class="textvm-primary">We'll be refunding you!</b></h3>
                            <div class="mt-5 text-secondary"><div class="spinner-border mr-3" role="status"></div>Please wait a moment</div>
                        </div>
                        <div class="col-7 border-left">
                            <table data-role="none" class="table table-light mb-5">
                                <thead>
                                    <tr class="text-center text-secondary">
                                        <th class="border-bottom-0 text-nowrap">Item Description</th>
                                        <th class="border-bottom-0" style="width:170px;">Qty</th>
                                        <th class="border-bottom-0" style="width:170px;">Price</th>
                                        <th class="border-bottom-0" style="width:170px;">Line Total</th>
                                    </tr>
                                </thead>
                                <tbody id="tbodyFailedItems">
                                </tbody>
                            </table>
                            <h2>Amount to be refunded: 10.00</h2>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="page_testallmotors" subpage="page_VMScreen" class="h-100 w-100" style="display:none;">
            <div class="d-flex flex-column align-items-center h-100 textvm-primary text-center">
                <div style="width:70%">
                    <div class="vh-100 d-flex flex-column align-items-center overflow-hidden">
                        <div class="flex-shrink-1 text-center">
                            <h1 class="pt-3 pb-3 mb-0 font-weight-bold">List of Motors</h1>
                        </div>
                        <div class="d-flex flex-fill overflow-auto w-100">
                            <div class="table-fixed-container w-100 h-100">
                                <div class="table-fixed d-flex align-items-start justify-content-center scrollable_area" id="scrollable_area">
                                    <table data-role="none" class="table table-light w-auto">
                                        <thead>
                                            <tr class="text-center text-secondary">
                                                <th class="fixed-col border-bottom-0">Motor No.</th>
                                                <th class="fixed-col border-bottom-0">Status</th>
                                            </tr>
                                        </thead>
                                        <tbody id="tbodyTestAllMotors">
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                        <div class="flex-shrink-1 text-center">
                            <div class="row align-items-center text-center text-secondary mt-3 mb-3 listInstructions">
                                <div class="col">
                                    <h3>Press <button data-role="none" class="light-beam navbtn-dark"><span>Esc</span></button> to cancel</h3>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="page_inventory" subpage="page_VMScreen" class="h-100 w-100" style="display:none;">
            <div class="d-flex flex-column align-items-center h-100 textvm-primary text-center">
                <div style="width:70%">
                    <div class="vh-100 d-flex flex-column align-items-center overflow-hidden">
                        <div class="flex-shrink-1 text-center">
                            <h1 class="pt-3 pb-3 mb-0 font-weight-bold">Inventory Balance</h1>
                        </div>
                        <div class="d-flex flex-fill overflow-auto w-100">
                            <div class="table-fixed-container w-100 h-100">
                                <div class="table-fixed d-flex align-items-start justify-content-center scrollable_area" id="scrollable_area">
                                    <table data-role="none" class="table table-light w-auto">
                                        <thead>
                                            <tr class="text-center text-secondary">
                                                <th class="fixed-col border-bottom-0">#</th>
                                                <th class="fixed-col border-bottom-0">Product No.</th>
                                                <th class="fixed-col border-bottom-0">Product Description</th>
                                                <th class="fixed-col border-bottom-0">Motor No.</th>
                                                <th class="fixed-col border-bottom-0">Total Balance</th>
                                            </tr>
                                        </thead>
                                        <tbody id="tbodyInventory">
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                        <div class="flex-shrink-1 text-center">
                            <div class="row align-items-center text-center text-secondary mt-3 mb-3 listInstructions">
                                <div class="col">
                                    <h3>Press <button data-role="none" class="light-beam navbtn-dark"><span>Esc</span></button> to cancel</h3>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="popupVDMDialog" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content rounded-2">
            <div class="modal-body">
                <div class="text-center mt-4 mb-4">
                    <i class="bi bi-box-seam-fill bg-warning-light p-3 text-warning rounded-pill" style="font-size:50px;"></i>
                </div>

                <div class="text-center">
                    <h4 class="textvm-primary mb-5">Limited Stock Available</h4>

                    <h6>There are only 7 items left for Classic Bears Candy.</h6>
                </div>
            </div>
            <div class="modal-footer border-0 p-3">
                <div class="row">
                    <div class="col-12 text-center">
                        <button data-role="none" type="button" class="btn btnvm-primary btn-lg rounded-pill pl-4 pr-4" data-dismiss="modal">Got it</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>