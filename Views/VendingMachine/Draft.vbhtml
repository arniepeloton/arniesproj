@Code
    ViewData("Title") = "Vending Machine"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code
@Section head
<link rel="stylesheet" href="/Content/jquery.buttons.dataTables.min.css" />
<link rel="stylesheet" href="/Content/jquery.dataTables2.css" />

<script>
    require(['bootstrapjs'], function () {
        require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
            require(['/Scripts/sources/VendingMachine/main.js?crc=' + assetversion])
        })
    })
</script>

    <style>
        body {
            overflow: hidden;
            font-family: 'Poppins', sans-serif;
        }

        div[data-role="page"]:first-child {
            background-color: #eaeaea;
        }

        .productcontainer {
            height: 100vh;
            display: grid;
            grid-template-rows: 80px 1fr 100px;
        }

        .productcontainer-body {
            flex: 1;
            overflow: auto;
        }

        .divAds {
            width: 500px;
            height: 100%;
        }

        body > div[data-role="page"]:first-child {
            background-color: #e5e5e5;
        }
    </style>
End Section

<div>
    <table border="0" cellspacing="0" cellpadding="0" style="width: 100%; height: 100%; top: 0px; bottom: 0px; position: absolute;">
        <tbody>
            <tr>
                <td class="divAds">
                    @*<video width="320" height="240" autoplay>
                        <source src="../Content/VM_ads.mp4" type="video/mp4">
                        Your browser does not support the video tag.
                    </video>*@
                    <img src="../Content/images/chicken_adobo.jpg" style="width: 100%; height:100%; object-fit:cover;" />
                </td>
                <td>
                    <div id="page_menu" subpage="about_item" style="display:none;">
                        <div class="vh-100 d-flex flex-column overflow-hidden">
                            <div class="flex-shrink-1 mb-4">
                                <div class="row bg-white g-0 mb-4">
                                    <div class="col p-3"></div>
                                    <div class="col p-3 text-right">
                                        <button data-role="none" class="btn btnvm-primary-outline btn-lg rounded-pill btnViewOrder">
                                            <span class="bi-cart4"></span>
                                            View Order
                                            <span class="font-weight-bolder">5</span>
                                        </button>
                                    </div>
                                </div>

                                <div>
                                    <ul class="list-group list-group-horizontal list-unstyled pl-3">
                                        <li class="mr-4"><button data-role="none" class="btn btnvm-primary shadow-sm btn-xl" style="width:200px;">Meals</button></li>
                                        <li class="mr-4"><button data-role="none" class="btn btnvm-primary-outline border-0 shadow-sm btn-xl" style="width:200px;">Soft Drinks</button></li>
                                        <li class="mr-4"><button data-role="none" class="btn btnvm-primary-outline border-0 shadow-sm btn-xl" style="width:200px;">Juices</button></li>
                                        <li class="mr-4"><button data-role="none" class="btn btnvm-primary-outline border-0 shadow-sm btn-xl" style="width:200px;">Add-Ons</button></li>
                                    </ul>
                                </div>
                            </div>
                            <div class="flex-fill d-flex flex-row overflow-auto w-100 h-100">
                                <div class="row row-cols-3 g-0">
                                    <div class="col p-3">
                                        <div class="border rounded bg-white text-dark shadow-sm btnItem">
                                            <img src="../Content/images/chicken_adobo.jpg" style="width:100%;" />
                                            <div class="font-weight-bold p-3" style="font-size:20px;">
                                                Chicken Adobo
                                            </div>
                                            <div class="row g-0 align-items-center">
                                                <div class="col pl-3" style="font-size:25px;">
                                                    50.00
                                                </div>
                                                <div class="col-auto pr-3">
                                                    <span class="badge bg-light border text-secondary">10 items left</span>
                                                </div>
                                            </div>

                                            <div class="p-3">
                                                <button data-role="none" class="btn btnvm-primary-outline btn-lg btn-block">
                                                    <span class="bi-plus-lg"></span>
                                                    Add to Cart
                                                </button>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col p-3">
                                        <div class="border rounded bg-white text-dark shadow-sm btnItem">
                                            <img src="../Content/images/chicken_adobo.jpg" style="width:100%;" />
                                            <div class="font-weight-bold p-3" style="font-size:20px;">
                                                Chicken Adobo
                                            </div>
                                            <div class="row g-0 align-items-center">
                                                <div class="col pl-3" style="font-size:25px;">
                                                    50.00
                                                </div>
                                                <div class="col-auto pr-3">
                                                    <span class="badge bg-light border text-secondary">10 items left</span>
                                                </div>
                                            </div>

                                            <div class="p-3">
                                                <button data-role="none" class="btn btnvm-primary-outline btn-lg btn-block">
                                                    <span class="bi-plus-lg"></span>
                                                    Add to Cart
                                                </button>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col p-3">
                                        <div class="border rounded bg-white text-dark shadow-sm btnItem">
                                            <img src="../Content/images/chicken_adobo.jpg" style="width:100%;" />
                                            <div class="font-weight-bold p-3" style="font-size:20px;">
                                                Chicken Adobo
                                            </div>
                                            <div class="row g-0 align-items-center">
                                                <div class="col pl-3" style="font-size:25px;">
                                                    50.00
                                                </div>
                                                <div class="col-auto pr-3">
                                                    <span class="badge bg-light border text-secondary">10 items left</span>
                                                </div>
                                            </div>

                                            <div class="p-3">
                                                <button data-role="none" class="btn btnvm-primary-outline btn-lg btn-block">
                                                    <span class="bi-plus-lg"></span>
                                                    Add to Cart
                                                </button>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col p-3">
                                        <div class="border rounded bg-white text-dark shadow-sm btnItem">
                                            <img src="../Content/images/chicken_adobo.jpg" style="width:100%;" />
                                            <div class="font-weight-bold p-3" style="font-size:20px;">
                                                Chicken Adobo
                                            </div>
                                            <div class="row g-0 align-items-center">
                                                <div class="col pl-3" style="font-size:25px;">
                                                    50.00
                                                </div>
                                                <div class="col-auto pr-3">
                                                    <span class="badge bg-light border text-secondary">10 items left</span>
                                                </div>
                                            </div>

                                            <div class="p-3">
                                                <button data-role="none" class="btn btnvm-primary-outline btn-lg btn-block">
                                                    <span class="bi-plus-lg"></span>
                                                    Add to Cart
                                                </button>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col p-3">
                                        <div class="border rounded bg-white text-dark shadow-sm btnItem">
                                            <img src="../Content/images/chicken_adobo.jpg" style="width:100%;" />
                                            <div class="font-weight-bold p-3" style="font-size:20px;">
                                                Chicken Adobo
                                            </div>
                                            <div class="row g-0 align-items-center">
                                                <div class="col pl-3" style="font-size:25px;">
                                                    50.00
                                                </div>
                                                <div class="col-auto pr-3">
                                                    <span class="badge bg-light border text-secondary">10 items left</span>
                                                </div>
                                            </div>

                                            <div class="p-3">
                                                <button data-role="none" class="btn btnvm-primary-outline btn-lg btn-block">
                                                    <span class="bi-plus-lg"></span>
                                                    Add to Cart
                                                </button>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col p-3">
                                        <div class="border rounded bg-white text-dark shadow-sm btnItem">
                                            <img src="../Content/images/chicken_adobo.jpg" style="width:100%;" />
                                            <div class="font-weight-bold p-3" style="font-size:20px;">
                                                Chicken Adobo
                                            </div>
                                            <div class="row g-0 align-items-center">
                                                <div class="col pl-3" style="font-size:25px;">
                                                    50.00
                                                </div>
                                                <div class="col-auto pr-3">
                                                    <span class="badge bg-light border text-secondary">10 items left</span>
                                                </div>
                                            </div>

                                            <div class="p-3">
                                                <button data-role="none" class="btn btnvm-primary-outline btn-lg btn-block">
                                                    <span class="bi-plus-lg"></span>
                                                    Add to Cart
                                                </button>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col p-3">
                                        <div class="border rounded bg-white text-dark shadow-sm btnItem">
                                            <img src="../Content/images/chicken_adobo.jpg" style="width:100%;" />
                                            <div class="font-weight-bold p-3" style="font-size:20px;">
                                                Chicken Adobo
                                            </div>
                                            <div class="row g-0 align-items-center">
                                                <div class="col pl-3" style="font-size:25px;">
                                                    50.00
                                                </div>
                                                <div class="col-auto pr-3">
                                                    <span class="badge bg-light border text-secondary">10 items left</span>
                                                </div>
                                            </div>

                                            <div class="p-3">
                                                <button data-role="none" class="btn btnvm-primary-outline btn-lg btn-block">
                                                    <span class="bi-plus-lg"></span>
                                                    Add to Cart
                                                </button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="page_productinfo" subpage="about_item" style="display:none;">
                        <div class="row justify-content-center">
                            <div class="col-10">
                                <div class="vh-100 d-flex flex-column overflow-hidden" style="font-size:30px;">
                                    <div class="flex-shrink-1 mb-4">
                                        <div class="row mt-3 g-0 align-items-center">
                                            <div class="col">
                                                <div class="font-weight-bold mb-3" style="font-size:70px; line-height:1;">Chicken Adobo</div>
                                                <h1 class="text-secondary mb-0">Price: 50.00</h1>
                                            </div>
                                            <div class="col-auto">
                                                <img src="../Content/images/chicken_adobo.jpg" class="img-thumbnail" style="width: 250px; height:250px; object-fit:cover;" />
                                            </div>                                            
                                        </div>
                                    </div>
                                    <span class="mb-3 mt-0">Description:</span>
                                    <div class="flex-fill bg-white d-flex flex-row overflow-auto w-100 h-100 border rounded bordervm-primary p-3">
                                        <div class="">
                                            Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam semper eu diam a vehicula. Etiam euismod semper efficitur. Nunc pellentesque volutpat enim ac aliquam. In dignissim nulla nibh, vel ultrices purus lacinia non. Ut fringilla, enim rhoncus vestibulum tristique, eros neque maximus nisl, vitae fermentum nulla metus sit amet nulla. Aenean eget semper purus. Fusce id erat felis. Nunc facilisis finibus neque, sed venenatis mauris. Morbi pretium pellentesque aliquam. Aliquam porttitor, arcu non ornare egestas, tellus dui iaculis libero, ac ornare mauris leo ut metus.
                                        </div>
                                    </div>
                                    <div class="flex-shrink-1">
                                        <div class="row mt-3 g-5">
                                            <div class="col text-center">
                                                <div class="shadow-sm mb-3" style="border-radius: 50rem;">
                                                    <div class="input-group">
                                                        <button data-role="none" class="btn btnvm-primary-outline btn-xl border-0 p-0" style="border-top-left-radius: 50rem;border-bottom-left-radius: 50rem;width: 130px; font-size:70px;"><span class="bi-dash-lg text-danger"></span></button>
                                                        <input data-role="none" type="text" class="form-control text-center bg-white border-0 textvm-primary" value="10" style="font-size:70px;" readonly>
                                                        <button data-role="none" class="btn btnvm-primary-outline btn-xl border-0 p-0" style="border-top-right-radius: 50rem;border-bottom-right-radius: 50rem;width: 130px; font-size:70px;"><span class="bi-plus-lg text-success"></span></button>
                                                    </div>
                                                </div>
                                                <span class="text-center text-secondary mb-3" style="font-size:16px;">(<span class="font-weight-bold">10</span> items left)</span>
                                                <h1 class="textvm-primary mb-5">Total: 500.00</h1>
                                            </div>
                                            <div class="col">
                                                <button data-role="none" id="btnAddToCart" class="btn btnvm-primary rounded-pill btn-xl btn-block shadow-sm" style="font-size:40px; height:120px;">Add to Cart</button>
                                                <button data-role="none" class="btn btnBackToMenu btn-light border rounded-pill btn-xl btn-block shadow-sm" style="font-size: 20px;">Back</button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>                        
                    </div>
                    
                    <div id="page_Cart" subpage="about_item" style="display:none;">
                        <div class="row justify-content-center">
                            <div class="col-8">
                                <div class="vh-100 d-flex flex-column overflow-hidden" style="font-size:40px;">
                                    <div class="flex-shrink-1">
                                        <h1 class="pt-3 pb-3 font-weight-bold">Your Order</h1>
                                    </div>
                                    <div class="d-flex flex-fill align-items-start flex-column overflow-auto w-100">
                                        <div class="p-3 rounded shadow-sm bg-white w-100 mb-3">
                                            <div class="row">
                                                <div class="col-2">
                                                    <img src="../Content/images/chicken_adobo.jpg" style="width: 100px; height:100px; object-fit:cover;" />
                                                </div>
                                                <div class="col">
                                                    <h3>Chicken Adobo</h3>
                                                    <div class="row g-0 align-items-center">
                                                        <div class="col">
                                                            <h4 class="textvm-primary">50.00</h4>
                                                        </div>
                                                        <div class="col-5">
                                                            <div class="shadow-sm mb-1" style="border-radius: 50rem;">
                                                                <div class="input-group">
                                                                    <button data-role="none" class="btn btnvm-primary-outline btn-xl border-0 p-0" style="border-top-left-radius: 50rem;border-bottom-left-radius: 50rem;width: 80px; font-size:30px;"><span class="bi-dash-lg text-danger"></span></button>
                                                                    <input data-role="none" type="text" class="form-control text-center bg-white border-0 textvm-primary" value="10" style="font-size:30px;" readonly>
                                                                    <button data-role="none" class="btn btnvm-primary-outline btn-xl border-0 p-0" style="border-top-right-radius: 50rem;border-bottom-right-radius: 50rem;width: 80px; font-size:30px;"><span class="bi-plus-lg text-success"></span></button>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="col-auto">
                                                            <button data-role="none" class="btn btnvm-primary-outline btn-xl border-0 p-0" style="border-top-right-radius: 50rem;border-bottom-right-radius: 50rem;width: 80px; font-size:30px;"><span class="bi-trash text-danger"></span></button>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="flex-shrink-1 mt-3">
                                        <div class="bgvm-primary-gradient text-white p-3 rounded-lg shadow">
                                            <div class="row align-items-center">
                                                <div class="col">
                                                    <h1 class="m-0">Total Amount:</h1>
                                                </div>
                                                <div class="col-auto">
                                                    <div class="bg-white p-3 textvm-primary rounded-pill text-center" style="min-width:200px;">
                                                        <h1 class="m-0">500.00</h1>
                                                    </div>                                                    
                                                </div>
                                            </div>
                                        </div>                                        
                                    </div>
                                    <div class="flex-shrink-1">
                                        <div class="row pt-4 pb-4 g-5 justify-content-center align-items-center">
                                            <div class="col">
                                                <button data-role="none" class="btn btnBackToMenu btn-light border rounded-pill btn-xl btn-block shadow-sm" style="font-size:40px; height:120px;">Back</button>
                                            </div>
                                            <div class="col">
                                                <button data-role="none" id="btnPayNow" class="btn btnvm-primary rounded-pill btn-xl btn-block shadow-sm" style="font-size:40px; height:120px;">Pay Now</button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="page_paynow" subpage="about_item" style="display:block;">
                        <div class="row g-0 justify-content-center bgvm-primary-gradient text-white">
                            <div class="col-8">
                                <div class="vh-100 d-flex flex-column overflow-hidden" style="font-size:80px;">
                                    <div class="flex-shrink-1 text-center">
                                        <div class="pt-5 pb-5 font-weight-bold">Insert your Cash</div>
                                    </div>
                                    <div class="d-flex flex-fill flex-column overflow-auto w-100 text-center">
                                        <div style="font-size: 40px;" class="mb-5">Amount to Pay: <b>500.00</b></div>
                                        <div style="font-size: 40px;" class="mb-3 mt-5">Amount Inserted:</div>
                                        <div class="mb-3 p-3 bg-white shadow rounded text-center textvm-primary font-weight-bold">0.00</div>
                                    </div>
                                    <div class="d-flex flex-shrink-1 justify-content-center">
                                        <div class="border bordervm-primary p-3 rounded-pill text-center m-5 btnBackToMenu" style="width:200px; font-size:30px;">
                                            CANCEL
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>                        
                    </div>
                </td>
            </tr>
        </tbody>
    </table>
</div>