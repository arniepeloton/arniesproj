@Code
    ViewData("Title") = "Online Directory"
    Layout = "~/Views/Shared/OnlineDirectoryLayout.vbhtml"
End Code
@Section head
    <script type="text/javascript">
        require(['/Scripts/sources/Directory/Index.js?crc=' + assetversion])
    </script>

    <style type="text/css">
        .suppliermaindiv {
            border: 1px solid #ccc;
            border-radius: 3px;
        }

        .suppliermaindiv:not(:first-child) {
            margin-top: 20px;
        }

        .suppliermaindiv:hover {
            cursor: pointer;
        }

        .suppliermaindiv > .suppliername {
            font-size: 20px;
            font-weight: normal;
            color: #0083c1;
            padding: 10px;
            border-bottom: 1px solid #ccc;
        }

        .suppliermaindiv > .suppliercatbrand {
            padding:5px 10px;
            font-size: 14px;
        }
    </style>
End Section
<div class="container">
    <div class="row">
        <div class="col-sm-offset-2 col-sm-8">
            <div id="divSearchPage" subpage="pagemaindirectory" style="display:none;">
                <div style="margin-top: 20px;">
                    <div style="text-align:center; font-size:30px;">
                        Mobiz Online Directory
                    </div>
                    <br />
                    <form id="formSearchSupplier">
                        <div class="input-group">
                            <div class="input-group-btn">
                                <button data-role="none" id="btnFilterDropdown" type="button" class="btn btn-primary dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><span id="lblFilterText">Search By All Suppliers</span> <span class="caret"></span></button>
                                <ul class="dropdown-menu">
                                    <li><a href="#" class="btnFilterSearch" value="all">All Suppliers</a></li>
                                    <li><a href="#" class="btnFilterSearch" value="categories">Products or Services</a></li>
                                    <li><a href="#" class="btnFilterSearch" value="brands">Brands</a></li>
                                </ul>
                            </div><!-- /btn-group -->
                            <input data-role="none" id="txtSearchSupplier" type="text" class="form-control" aria-label="...">
                        </div>
                    </form>
                    <br />
                    <div id="divSupplierResults">
                        @*<div class="suppliermaindiv">
                            <div class="suppliername">
                                Aqua Inc.
                            </div>
                            <div class="suppliercatbrand">
                                <i>Products or Services offer:</i> Computer Software, Xerox Machine, POS Software, Computer Hardware
                            </div>
                            <div class="suppliercatbrand">
                                <i>Brands offer:</i> Purefoods, Magnolia, Nestle
                            </div>
                        </div>*@
                    </div>
                </div>
            </div>

            <div id="divSupplierInfo" subpage="pagemaindirectory" style="display:none;">                
                <div style="background-color: #e6e6e6;">
                    <div style="background-color: #0083C1; color: #FFF; padding: 25px; box-shadow: 0 10px 20px rgb(255 255 255 / 19%), 0 6px 6px rgb(0 131 193);">
                        <button data-role="none" class="btn btn-primary btn-less-width btnSearchAgain">
                            <span class="glyphicon glyphicon-chevron-left"></span> Search Supplier
                        </button>
                        <h1 id="txtMACompanyName"></h1>
                        <span id="txtMACompanyAddress"></span>
                        <br /><br />
                        <span id="txtMABusInd"></span>
                    </div>

                    <div style="padding:25px;">
                        <div>
                            <br />
                            <span style="font-size:20px;">Company Background:</span>
                            <br /><br />
                            <p id="txtMACompanyBackground" style="white-space:pre-line"></p>
                        </div>

                        <div>
                            <br />
                            <span style="font-size:20px;">These are the Products or Services that we offer:</span>
                            <br /><br />
                            <ul data-role="none" id="lstProductsOrServicesOffer" style="list-style:decimal; margin-left:50px;"></ul>
                        </div>

                        <div>
                            <br />
                            <span style="font-size:20px;">These are the Brands that we offer:</span>
                            <br /><br />
                            <ul data-role="none" id="lstBrandsOffer" style="list-style:decimal; margin-left:50px;"></ul>
                        </div>

                        <div>
                            <br />
                            <span style="font-size:20px;">Please feel free to contact us if you have questions:</span>
                            <br /><br />
                            <div>
                                Email Address(es):
                                <ul data-role="none" id="lstCompanyEmail" style="list-style:decimal; margin-left:50px;"></ul>
                            </div>
                            <br />
                            <div>
                                Mobile Number(s):
                                <ul data-role="none" id="lstCompanyMobile" style="list-style:decimal; margin-left:50px;"></ul>
                            </div>
                            <br />
                            <div>
                                Telephone Number(s):
                                <ul data-role="none" id="lstCompanyTelephone" style="list-style:decimal; margin-left:50px;"></ul>
                            </div>
                        </div>

                        <div>
                            <br />
                            <span style="font-size:20px;">Company Pictures</span>
                            <br /><br />
                            <div id="divCompanyPicturesList" class="row">

                            </div>
                        </div>
                    </div>                    
                </div>
                <br />
                <button data-role="none" class="btn btn-default btn-less-width btnSearchAgain">
                    <span class="glyphicon glyphicon-chevron-left"></span> Search Supplier
                </button>
            </div>
        </div>
    </div>
</div>