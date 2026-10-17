@Code
    ViewData("Title") = "Ecommerce"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
@Section head
    <script type="text/javascript">
        require(['/Scripts/sources/Ecommerce/PreSupplier.js?v=' + assetversion])
    </script>

    <link rel="stylesheet" href="/Content/Ecommerce/css/Ecommerce.css">
    <link rel="stylesheet" href="/Content/flatui-css-custom.css">
    <link rel="stylesheet" href="/Content/bootstrap-custom.css" />
    <link rel="stylesheet" href="/Content/Ecommerce/css/main.css">
End Section

<div class="container">
    <div id="divLinkSuccess" subpage="pageLink" style="display:none;">
        <h1>Welcome!</h1>  
        
        <div id="divMessageForCustomerPerson" subpage="divLinkSuccess" style="font-size:16px; display:none;">
            <p>
                <span class="lblCompanyName"></span> wants you to be one of their online customer but before that please provide us your basic personal information. Please fill the required fields with "<span class="required">*</span>".
            </p>
            <p>
                <b>Important note:</b>
                Please keep your login credentials because you will use to purchase online.
            </p>
        </div>

        <div id="divMessageForCustomerOrg" subpage="divLinkSuccess" style="font-size:16px; display:none;">
            <p>
                <span class="lblCompanyName"></span> wants you to be one of their online customer but before that please provide us your basic personal information. Please fill the required fields with "<span class="required">*</span>".
            </p>
            <p>
                <b>Important note:</b>
                As Administrator for this company, please provide also your personal information in the Administrator Info tab and keep your login credentials because you will to purchase online.
            </p>
        </div>

        <div id="divMessageForSupplier" subpage="divLinkSuccess" style="font-size:16px; display:none;">
            <p>
                <span class="lblCompanyName"></span> wants you to be a vendor to their online store but before that please provide us further details about your company. Please fill the required fields with "<span class="required">*</span>".
            </p>
            <p>
                <b>Important note:</b>
                As Administrator for this company, please provide also your personal information in the Administrator Info tab and keep your login credentials because you will use it in the Vendor Portal.
            </p>
        </div>

        <br />

        <div id="divForCompany" subpage="profiletype" style="display:none;">
            <div id="divTab">
                <ul class="nav nav-tabs">
                    <li class="active" custom-tab-content="#tabCompanyProfile"><a data-role="none">Company Profile <span class="required">*</span></a></li>
                    <li custom-tab-content="#tabCompanyContactPerson"><a data-role="none">Administrator Info <span class="required">*</span></a></li>
                    <li custom-tab-content="#tabCompanyContactInfo"><a data-role="none">Contact Info</a></li>
                    <li custom-tab-content="#tabCompanyOutlet"><a data-role="none">Outlet</a></li>
                    <li custom-tab-content="#tabCompanySize"><a data-role="none">Company Size</a></li>
                    @*<li custom-tab-content="#tabRevenue" class="hidetab"><a data-role="none">Revenue</a></li>*@
                </ul>
            </div>

            <div id="tabCompanyProfile" custom-tab-role="content" style="display:block;">
                <label style="font-size:1.5em; font-weight:bold;">Company Profile</label>
                <input data-role="none" type="text" id="txtCustomerOrgID" style="display:none;" />
                <input data-role="none" type="text" id="txtProspectOrgID" style="display:none;" />
                <input data-role="none" type="text" id="txtExistingCustomerOrgID" style="display:none;" />
                <br /><br />
                <div class="form-horizontal">
                    <div class="form-group" style="display:none;">
                        <label for="selOrgStatus" class="col-sm-2 control-label">Status: <span class="required">*</span></label>
                        <div class="col-sm-10">
                            <select data-role="none" id="selOrgStatus" class="form-control unicase-form-control text-input">
                                <option value="Active">Active</option>
                                <option value="Deactive">Inactive</option>
                            </select>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="txtProspectCompanyName" class="col-sm-2 control-label">Organization Name: <span class="required">*</span></label>
                        <div class="col-sm-10">
                            <input data-role="none" type="text" id="txtProspectCompanyName" class="form-control unicase-form-control text-input" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="txtProspectOrgTin" class="col-sm-2 control-label">TIN: <span class="required">*</span></label>
                        <div class="col-sm-10">
                            <input data-role="none" type="text" id="txtProspectOrgTin" class="form-control unicase-form-control text-input" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="txtProspectCompanyBackground" class="col-sm-2 control-label">Organization Background:</label>
                        <div class="col-sm-10">
                            <textarea data-role="none" id="txtProspectCompanyBackground" class="form-control unicase-form-control text-input" style="resize:none; height:100px; width:100%;" placeholder="Write here"></textarea>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="txtProspectCompanyMission" class="col-sm-2 control-label">Organization Mission:</label>
                        <div class="col-sm-10">
                            <textarea data-role="none" id="txtProspectCompanyMission" class="form-control unicase-form-control text-input" style="resize:none; height:100px; width:100%;" placeholder="Write here"></textarea>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="txtProspectCompanyVision" class="col-sm-2 control-label">Organization Vision:</label>
                        <div class="col-sm-10">
                            <textarea data-role="none" id="txtProspectCompanyVision" class="form-control unicase-form-control text-input" style="resize:none; height:100px; width:100%;" placeholder="Write here"></textarea>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="txtProspectCompanyFounding" class="col-sm-2 control-label">Founding Date/Year:</label>
                        <div class="col-sm-10">
                            <input data-role="none" type="text" id="txtProspectCompanyFounding" class="form-control unicase-form-control text-input" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="txtProspectCompanyFounding" class="col-sm-2 control-label">Business Industry:</label>
                        <div class="col-sm-10">
                            <input type="text" data-role="none" id="txtBusIndustryID" style="display:none;" />
                            <button data-role="none" class="btn btn-primary" id="btnProspectCompanyIndustry">Search here</button>
                        </div>
                    </div>
                    <div class="form-group" id="divCompany3rdPartyPlatform" style="display:none;">
                        <label for="txtProspectCompany3rdPartyPlatform" class="col-sm-2 control-label">3rd Party Platform:</label>
                        <div class="col-sm-10">
                            <input data-role="none" type="text" id="txtProspectCompany3rdPartyPlatform" class="form-control unicase-form-control text-input" />
                        </div>
                    </div>
                    <div class="form-group" id="divCompany3rdPartyID" style="display:none;">
                        <label for="txtProspectCompany3rdPartyID" class="col-sm-2 control-label">3rd Party ID:</label>
                        <div class="col-sm-10">
                            <input data-role="none" type="text" id="txtProspectCompany3rdPartyID" class="form-control unicase-form-control text-input" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="txtCodeNumber" class="col-sm-2 control-label">Code:</label>
                        <div class="col-sm-10">
                            <input data-role="none" type="text" id="txtCodeNumber" readonly placeholder="To be generated by the system" class="form-control unicase-form-control text-input" />
                        </div>
                    </div>
                </div>
                <br />

                <div class="panel panel-default">
                    <div class="panel-heading custom-theme-c">Company Main Address</div>
                    <div class="panel-body">

                        <div class="form-horizontal">
                            <div class="form-group">
                                <label for="txtProspectHouseBldgNo" class="col-sm-2 control-label">House or Building No: <span class="required">*</span>:</label>
                                <div class="col-sm-10">
                                    <input data-role="none" class="form-control unicase-form-control text-input" type="text" id="txtProspectHouseBldgNo" />
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="txtProspectStreetName" class="col-sm-2 control-label">Street: <span class="required">*</span>:</label>
                                <div class="col-sm-10">
                                    <input data-role="none" class="form-control unicase-form-control text-input" type="text" id="txtProspectStreetName" />
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="selProspectCities" class="col-sm-2 control-label">City Or Town: <span class="required">*</span>:</label>
                                <div class="col-sm-10">
                                    <select data-role="none" id="selProspectCities" class="form-control unicase-form-control text-input"></select>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="selProspectOrgRegion" class="col-sm-2 control-label">Region:</label>
                                <div class="col-sm-10">
                                    <select data-role="none" id="selProspectOrgRegion" class="form-control unicase-form-control text-input"></select>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="selProspectIsland" class="col-sm-2 control-label">Island Group:</label>
                                <div class="col-sm-10">
                                    <select data-role="none" id="selProspectIsland" class="form-control unicase-form-control text-input">
                                        <option value="">Please Select Island</option>
                                        <option value="Luzon">Luzon</option>
                                        <option value="Visayas">Visayas</option>
                                        <option value="Mindanao">Mindanao</option>
                                    </select>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="selProspectCountries" class="col-sm-2 control-label">Country: <span class="required">*</span>:</label>
                                <div class="col-sm-10">
                                    <select data-role="none" id="selProspectCountries" class="form-control unicase-form-control text-input"></select>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="panel panel-default">
                    <div class="panel-heading custom-theme-c">Organization Affiliation</div>
                    <div class="panel-body">
                        <div>
                            <div data-role="fieldcontain" style="text-align:right;">
                                <button id="btnAddOrgAffiliation" data-role="none" class="btn btn-primary btn-less-width">
                                    ADD
                                </button>
                            </div>
                            <div style="padding: 8px 15px 0px 15px;">
                                <ul data-role="listview" id="lstOrgAffiliation" class="li-theme-c noBorderList custom-list custom-list-sm"></ul>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="panel panel-default">
                    <div class="panel-heading custom-theme-c">Affiliated Person</div>
                    <div class="panel-body">
                        <div>
                            <div data-role="fieldcontain" style="text-align:right;">
                                <button id="btnAddAffiliatedPerson" data-role="none" class="btn btn-primary btn-less-width">
                                    ADD
                                </button>
                            </div>
                            <div style="padding: 8px 15px 0px 15px;">
                                <ul data-role="listview" id="lstAffiliatedPerson" class="li-theme-c noBorderList custom-list custom-list-sm"></ul>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div id="tabCompanyContactInfo" custom-tab-role="content" style="display:none;">
                <label style="font-size:1.5em; font-weight:bold;">Company Contact Info</label>
                <br /><br />
                <div class="panel panel-default">
                    <div class="panel-heading custom-theme-c">Telephone Number</div>
                    <div class="panel-body">
                        <div>
                            <div data-role="fieldcontain" style="text-align:right;">
                                <button id="btnAddOrgTelephone" data-role="none" class="btn btn-primary btn-less-width">
                                    ADD NEW TELEPHONE
                                </button>
                            </div>
                            <div style="padding: 8px 15px 0px 15px;">
                                <ul data-role="listview" id="lstOrgTelephone" class="li-theme-c noBorderList custom-list custom-list-sm"></ul>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="panel panel-default">
                    <div class="panel-heading custom-theme-c">Mobile Number</div>
                    <div class="panel-body">
                        <div>
                            <div data-role="fieldcontain" style="text-align:right;">
                                <button id="btnAddOrgMobile" data-role="none" class="btn btn-primary btn-less-width">
                                    ADD NEW MOBILE
                                </button>
                            </div>
                            <div style="padding: 8px 15px 0px 15px;">
                                <ul data-role="listview" id="lstOrgMobile" class="li-theme-c noBorderList custom-list custom-list-sm"></ul>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="panel panel-default">
                    <div class="panel-heading custom-theme-c">Email Address</div>
                    <div class="panel-body">
                        <div>
                            <div data-role="fieldcontain" style="text-align:right;">
                                <button id="btnAddOrgEmail" data-role="none" class="btn btn-primary btn-less-width">
                                    ADD NEW EMAIL
                                </button>
                            </div>
                            <div style="padding: 8px 15px 0px 15px;">
                                <ul data-role="listview" id="lstOrgEmail" class="li-theme-c noBorderList custom-list custom-list-sm"></ul>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="panel panel-default">
                    <div class="panel-heading custom-theme-c">Website</div>
                    <div class="panel-body">
                        <div>
                            <div data-role="fieldcontain" style="text-align:right;">
                                <button id="btnAddOrgWebsite" data-role="none" class="btn btn-primary btn-less-width">
                                    ADD NEW WEBSITE
                                </button>
                            </div>
                            <div style="padding: 8px 15px 0px 15px;">
                                <ul data-role="listview" id="lstOrgWebsite" class="li-theme-c noBorderList custom-list custom-list-sm"></ul>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div id="tabCompanyContactPerson" custom-tab-role="content" style="display:none;">
                <label style="font-size:1.5em; font-weight:bold;">Administrator Info</label>
                <br /><br />

                @*<div class="panel panel-default">
                <div class="panel-heading custom-theme-c">&nbsp;</div>
                <div class="panel-body">
                    <div>
                        <div data-role="fieldcontain" style="text-align:right;">
                            <button id="btnAddContactPerson" data-role="none" class="btn btn-primary btn-less-width">
                                ADD CONTACT PERSON
                            </button>
                        </div>
                        <div style="padding: 8px 15px 0px 15px;">
                            <ul data-role="listview" id="lstOrgContactPerson" class="li-theme-c noBorderList custom-list custom-list-sm"></ul>
                        </div>
                    </div>
                </div>
            </div>*@

                <form autocomplete="off" class="form-horizontal">
                    <div class="form-group">
                        <label for="txtPersonFirstName" class="col-sm-2 control-label">First Name: <span class="required">*</span></label>
                        <div class="col-sm-10">
                            <input data-role="none" type="text" class="form-control unicase-form-control text-input" id="txtPersonFirstName">
                        </div>
                    </div>

                    <div class="form-group">
                        <label for="txtPersonMiddleName" class="col-sm-2 control-label">Middle Name:</label>
                        <div class="col-sm-10">
                            <input data-role="none" type="text" class="form-control unicase-form-control text-input" id="txtPersonMiddleName">
                        </div>
                    </div>

                    <div class="form-group">
                        <label for="txtPersonLastName" class="col-sm-2 control-label">Last Name: <span class="required">*</span></label>
                        <div class="col-sm-10">
                            <input data-role="none" type="text" class="form-control unicase-form-control text-input" id="txtPersonLastName">
                        </div>
                    </div>

                    <div class="form-group">
                        <label for="selPersonGenSuffix" class="col-sm-2 control-label">Generational Suffix:</label>
                        <div class="col-sm-10">
                            <select data-role="none" id="selPersonGenSuffix" class="form-control unicase-form-control text-input">
                                <option value="">None</option>
                                <option value="Sr.">Sr.</option>
                                <option value="Jr.">Jr.</option>
                                <option value="I">I</option>
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
                    </div>

                    <div class="form-group">
                        <label for="selPersonBirthMonth" class="col-sm-2 control-label">Date of Birth: <span class="required">*</span></label>
                        <div class="col-sm-10">
                            <div style="display:flex;">
                                <select data-role="none" id="selPersonBirthMonth" class="form-control unicase-form-control text-input fixed">
                                    <option value="">Month</option>
                                    <option value="January">January</option>
                                    <option value="February">February</option>
                                    <option value="March">March</option>
                                    <option value="April">April</option>
                                    <option value="May">May</option>
                                    <option value="June">June</option>
                                    <option value="July">July</option>
                                    <option value="August">August</option>
                                    <option value="September">September</option>
                                    <option value="October">October</option>
                                    <option value="November">November</option>
                                    <option value="December">December</option>
                                </select>
                                <select data-role="none" id="selPersonBirthDay" class="form-control unicase-form-control text-input fixed"></select>
                                <input data-role="none" type="text" placeholder="Year" maxlength="4" class="form-control unicase-form-control text-input" id="txtPersonBirthYear" style="width:70px;">
                            </div>
                        </div>
                    </div>

                    <div class="form-group">
                        <label for="txtPersonEmailAdd" class="col-sm-2 control-label">Email Address: <span class="required">*</span></label>
                        <div class="col-sm-10">
                            <p class="text title-tag-line"><b>NOTE</b>: Please enter your valid email address because we will send a link to activate your account. And you must remember the email address you enter, this will be used to reset your account in case you forget your password.</p>
                            <input data-role="none" type="text" class="form-control unicase-form-control text-input" id="txtPersonEmailAdd">
                        </div>
                    </div>
                    <h4>Login Credentials</h4>
                    <div class="form-group">
                        <label for="txtAdminUsername" class="col-sm-2 control-label">Username: <span class="required">*</span> (minimum of 8 characters)</label>
                        <div class="col-sm-10">
                            <input data-role="none" autocomplete="off" type="text" class="form-control unicase-form-control text-input" id="txtAdminUsername">
                        </div>
                    </div>

                    <div class="form-group">
                        <label for="txtAdminPassword" class="col-sm-2 control-label">Password: <span class="required">*</span> (minimum of 8 characters)</label>
                        <div class="col-sm-10">
                            <input data-role="none" autocomplete="off" type="password" class="form-control unicase-form-control text-input" id="txtAdminPassword">
                        </div>
                    </div>

                    <div class="form-group">
                        <label for="txtAdminConfirmPassword" class="col-sm-2 control-label">Confirm Password: <span class="required">*</span></label>
                        <div class="col-sm-10">
                            <input data-role="none" autocomplete="off" type="password" class="form-control unicase-form-control text-input" id="txtAdminConfirmPassword">
                        </div>
                    </div>

                    <h4>Security Questions</h4>
                    <p class="text title-tag-line"><b>NOTE</b>: You must remember the answers to the security questions you selected. This will be used to reset your account in case you forget your password.</p>
                    <br />

                    <div class="form-group">
                        <label for="selSecQuestion1" class="col-sm-2 control-label">Question No. 1: <span class="required">*</span></label>
                        <div class="col-sm-10">
                            <select id="selSecQuestion1" data-role="none" class="form-control unicase-form-control text-input">
                                <option value="">Please Select</option>
                                <option value="Where is your mother's birthplace?">Where is your mother's birthplace?</option>
                                <option value="Who is your best childhood friend?">Who is your best childhood friend?</option>
                                <option value="What is the name of your first pet?">What is the name of your first pet?</option>
                                <option value="Who is your favorite teacher?">Who is your favorite teacher?</option>
                                <option value="Who is your favorite historical person?">Who is your favorite historical person?</option>
                                <option value="What is your grandfather's occupation?">What is your grandfather's occupation?</option>
                            </select>
                            <br />
                            <input data-role="none" type="text" placeholder="Answer of Question No. 1" class="form-control unicase-form-control text-input" id="txtSecQuestion1Answer">
                        </div>
                    </div>

                    <div class="form-group">
                        <label for="selSecQuestion2" class="col-sm-2 control-label">Question No. 2: <span class="required">*</span></label>
                        <div class="col-sm-10">
                            <select id="selSecQuestion2" data-role="none" class="form-control unicase-form-control text-input">
                                <option value="">Please Select</option>
                                <option value="Where is your mother's birthplace?">Where is your mother's birthplace?</option>
                                <option value="Who is your best childhood friend?">Who is your best childhood friend?</option>
                                <option value="What is the name of your first pet?">What is the name of your first pet?</option>
                                <option value="Who is your favorite teacher?">Who is your favorite teacher?</option>
                                <option value="Who is your favorite historical person?">Who is your favorite historical person?</option>
                                <option value="What is your grandfather's occupation?">What is your grandfather's occupation?</option>
                            </select>
                            <br />
                            <input data-role="none" type="text" placeholder="Answer of Question No. 2" class="form-control unicase-form-control text-input" id="txtSecQuestion2Answer">
                        </div>
                    </div>
                </form>
            </div>

            <div id="tabCompanyOutlet" custom-tab-role="content" style="display:none;">
                <label style="font-size:1.5em; font-weight:bold;">Company Outlet</label>
                <br /><br />

                <div class="panel panel-default">
                    <div class="panel-heading custom-theme-c">&nbsp;</div>
                    <div class="panel-body">
                        <div>
                            <div class="row">
                                <div class="col-sm-6">
                                    <label for="selCompanyOutletStatus" class="col-sm-2 control-label">Show status:</label>
                                    <div class="col-sm-10">
                                        <select id="selCompanyOutletStatus" data-role="none" class="form-control unicase-form-control text-input fixed">
                                            <option value="">All</option>
                                            <option value="1" selected>Active</option>
                                            <option value="0">Inactive</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="col-sm-6" style="text-align:right;">
                                    <button id="btnAddCompanyOutlet" data-role="none" class="btn btn-primary btn-less-width">
                                        ADD NEW OUTLET
                                    </button>
                                </div>
                            </div>
                            <div style="padding: 8px 15px 0px 15px;">
                                <ul data-role="listview" id="lstCompanyOutlet" class="li-theme-c noBorderList custom-list custom-list-sm"></ul>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div id="tabCompanySize" custom-tab-role="content" style="display:none;">
                <div class="form-horizontal">
                    <div class="form-group">
                        <label for="txtProspectNoOfEmployees" class="col-sm-2 control-label">Number of Employees:</label>
                        <div class="col-sm-10">
                            <input data-role="none" type="text" id="txtProspectNoOfEmployees" class="form-control unicase-form-control text-input" />
                        </div>
                    </div>

                    <div class="form-group">
                        <label for="txtProspectRevenuePerYear" class="col-sm-2 control-label">Revenue per Year:</label>
                        <div class="col-sm-10">
                            <input data-role="none" type="text" id="txtProspectRevenuePerYear" class="form-control unicase-form-control text-input" />
                        </div>
                    </div>

                    <div class="form-group">
                        <label for="selProspectScopeOperation" class="col-sm-2 control-label">Scope of Operations:</label>
                        <div class="col-sm-10">
                            <select data-role="none" id="selProspectScopeOperation" class="form-control unicase-form-control text-input">
                                <option value="">Please Select</option>
                                <option value="Regional">Regional</option>
                                <option value="National">National</option>
                                <option value="Trans-National">Trans-National</option>
                            </select>
                        </div>
                    </div>

                    <div class="form-group">
                        <label for="txtProspectNoFactories" class="col-sm-2 control-label">Number of Factories:</label>
                        <div class="col-sm-10">
                            <input data-role="none" type="text" id="txtProspectNoFactories" class="form-control unicase-form-control text-input" />
                        </div>
                    </div>

                    <div class="form-group">
                        <label for="txtProspectNoOffices" class="col-sm-2 control-label">Number of Offices:</label>
                        <div class="col-sm-10">
                            <input data-role="none" type="text" id="txtProspectNoOffices" class="form-control unicase-form-control text-input" />
                        </div>
                    </div>

                    <div class="form-group">
                        <label for="txtProspectNoWarehouses" class="col-sm-2 control-label">Number of Warehouses:</label>
                        <div class="col-sm-10">
                            <input data-role="none" type="text" id="txtProspectNoWarehouses" class="form-control unicase-form-control text-input" />
                        </div>
                    </div>

                    <div class="form-group">
                        <label for="txtProspectNoStores" class="col-sm-2 control-label">Number of Stores:</label>
                        <div class="col-sm-10">
                            <input data-role="none" type="text" id="txtProspectNoStores" class="form-control unicase-form-control text-input" />
                        </div>
                    </div>

                    <div class="form-group">
                        <label for="txtProspectNoTrucks" class="col-sm-2 control-label">Number of Trucks:</label>
                        <div class="col-sm-10">
                            <input data-role="none" type="text" id="txtProspectNoTrucks" class="form-control unicase-form-control text-input" />
                        </div>
                    </div>
                </div>
            </div>

            <div id="tabRevenue" custom-tab-role="content" style="display:none;">
                <div class="custom-ui-label">
                    <div>Sales Representative:</div>
                    <div><button id="btnRevSalesRepresentative">Search Representative</button></div>
                </div>

                <div class="custom-ui-label">
                    <div>Credit Terms:</div>
                    <div><button id="btnRevCreditTerms">Search Payment Terms</button></div>
                </div>

                <div data-role="fieldcontain">
                    <label for="txtRevCreditLimit">Credit Limit:</label>
                    <input type="text" id="txtRevCreditLimit" />
                </div>

                <div data-role="fieldcontain">
                    <label for="chkRevUnlimitedCredit">Unlimited</label>
                    <input type="checkbox" id="chkRevUnlimitedCredit" />
                </div>

                <div data-role="fieldcontain">
                    <label for="txtRevCreditBegBal">Credit Beg. Balance:</label>
                    <input type="text" id="txtRevCreditBegBal" />
                </div>

                <div class="custom-ui-label">
                    <div>Credit Beg. as of:</div>
                    <div>
                        <button id="btnRevCreditBegAsOf">Set Date</button>
                    </div>
                </div>



                <div data-role="fieldcontain">
                    <label for="txtRevThirdPartyBegBalance">Third Party Credit Beg. Balance:</label>
                    <input type="text" id="txtRevThirdPartyBegBalance" />
                </div>

                <div class="custom-ui-label">
                    <div>Third Party Credit Beg. as of:</div>
                    <div>
                        <button id="btnRevThirdPartyBegBalanceAsOf">Set Date</button>
                    </div>
                </div>


                <div class="panel">
                    <div class="panel-title custom-theme-h">
                        Price Level
                    </div>

                    <div class="panel-content txt-left">
                        <div data-role="fieldcontain">
                            <div data-role="fieldcontain" style="text-align:right;">
                                <button id="btnRevPriceLevel" data-role="none" class="btn custom-btn-theme-b btnCircle">
                                    <img alt="Add" src="/Content/images/plus-white.png" style="width:18px;" />
                                </button>
                            </div>
                            <div style="padding: 8px 15px 0px 15px;">
                                <ul data-role="listview" id="lstOrgPriceLevel" class="li-theme-c noBorderList custom-list custom-list-sm"></ul>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <br />
            <hr />
            <br />
            <div id="divErrMsg" style="color:#ff3737; text-align:center; font-size:14px; margin-bottom:25px; display:none;">Please complete the required field(s)</div>
            <br />
            <div style="font-size:16px;">If you are done, please click the Save button. Once you clicked the button, this link will no longer be accessible for security reasons but you can edit your information later on.</div>
            <br /><br />
            <button data-role="none" id="btnSaveAddingContact" class="btn btn-primary btn-lg" style="height:50px;">SAVE</button>
        </div>

        <div id="divForPersonal" subpage="profiletype" style="display:none;">
            <div id="divTabPerson">
                <ul class="nav nav-tabs">
                    <li class="active" custom-tab-content="#tabPersonProfile"><a data-role="none">Profile <span class="required">*</span></a></li>
                    <li custom-tab-content="#tabPersonContactInfo"><a data-role="none">Contact Info <span class="required">*</span></a></li>
                    <li custom-tab-content="#tabPersonEducation"><a data-role="none">Educational Background</a></li>
                    <li custom-tab-content="#tabPersonLoginCredentials"><a data-role="none">Login Credentials <span class="required">*</span></a></li>
                    @*<li custom-tab-content="#tabPersonRevenue"><a data-role="none">Revenue</a></li>*@
                    @*<li custom-tab-content="#tabPersonEmployment">Employment History</li>*@
                </ul>
            </div>

            <div id="tabPersonProfile" custom-tab-role="content" style="display:block;">
                <label style="font-size:1.5em; font-weight:bold;">Person Profile</label>
                <input data-role="none" type="text" id="txtCustomerPersonID" style="display:none;" />
                <input data-role="none" type="text" id="txtExistingCustomerPersonID" style="display:none;" />
                <input data-role="none" type="text" id="txtProspectPersonID" style="display:none;" />
                <br /><br />
                <div class="form-horizontal">
                    <div class="form-group" style="display:none;">
                        <label for="selPersonStatus" class="col-sm-2 control-label">Status: <span class="required">*</span></label>
                        <div class="col-sm-10">
                            <select data-role="none" id="selPersonStatus" class="form-control unicase-form-control text-input">
                                <option value="Active">Active</option>
                                <option value="Deactive">Inactive</option>
                            </select>
                        </div>
                    </div>

                    <div class="form-group">
                        <label for="txtProspectPerson3rdPartyID" class="col-sm-2 control-label">3rd Party System ID:</label>
                        <div class="col-sm-10">
                            <input data-role="none" class="form-control unicase-form-control text-input" type="text" id="txtProspectPerson3rdPartyID" />
                        </div>
                    </div>

                    <div class="form-group">
                        <label for="txtPersonCodeNumber" class="col-sm-2 control-label">Customer Code:</label>
                        <div class="col-sm-10">
                            <input data-role="none" class="form-control unicase-form-control text-input" readonly placeholder="To be generated by the system" type="text" id="txtPersonCodeNumber" />
                        </div>
                    </div>

                    <div class="form-group">
                        <label for="txtProspectPersonFirstName" class="col-sm-2 control-label">Full Name: <span style="color:#F00;">*</span></label>
                        <div class="col-sm-4">
                            <input data-role="none" class="form-control unicase-form-control text-input" type="text" id="txtProspectPersonFirstName" />
                        </div>
                        <div class="col-sm-3">
                            <input data-role="none" class="form-control unicase-form-control text-input" type="text" id="txtProspectPersonMiddleName" />
                        </div>
                        <div class="col-sm-3">
                            <input data-role="none" class="form-control unicase-form-control text-input" type="text" id="txtProspectPersonLastName" />
                        </div>
                    </div>

                    <div class="form-group">
                        <label for="txtProspectPersonTinNumber" class="col-sm-2 control-label">TIN:</label>
                        <div class="col-sm-10">
                            <input data-role="none" class="form-control unicase-form-control text-input" type="text" id="txtProspectPersonTinNumber" />
                        </div>
                    </div>

                    <div class="form-group">
                        <label for="selProspectPersonGenSuffix" class="col-sm-2 control-label">Generational Suffix:</label>
                        <div class="col-sm-10">
                            <select data-role="none" id="selProspectPersonGenSuffix" class="form-control unicase-form-control text-input">
                                <option value="">None</option>
                                <option value="Jr.">Jr.</option>
                                <option value="Sr.">Sr.</option>
                                <option value="I">I</option>
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
                    </div>

                    <div class="form-group">
                        <label for="txtProspectPersonTinNumber" class="col-sm-2 control-label">Date of Birth:</label>
                        <div class="col-sm-10">
                            <button data-role="none" class="btn btn-default" id="btnPersonBirthDate">Set Date</button>
                        </div>
                    </div>

                    <div class="form-group">
                        <label for="txtProspectPersonPosition" class="col-sm-2 control-label">Position/Title:</label>
                        <div class="col-sm-10">
                            <input data-role="none" class="form-control unicase-form-control text-input" type="text" id="txtProspectPersonPosition" />
                        </div>
                    </div>

                    <div class="form-group">
                        <label for="txtProspectPersonPositionDescription" class="col-sm-2 control-label">Position/Title Description:</label>
                        <div class="col-sm-10">
                            <input data-role="none" class="form-control unicase-form-control text-input" type="text" id="txtProspectPersonPositionDescription" />
                        </div>
                    </div>
                </div>
                <br />

                <div class="panel panel-default">
                    <div class="panel-heading custom-theme-c">Home Address</div>
                    <div class="panel-body">
                        <input data-role="none" type="text" id="txtPersonAddressID" style="display:none;" />

                        <div class="form-horizontal">
                            <div class="form-group">
                                <label for="txtPersonHouseBuildingNo" class="col-sm-2 control-label">House or Building No: <span class="required">*</span>:</label>
                                <div class="col-sm-10">
                                    <input data-role="none" class="form-control unicase-form-control text-input" type="text" id="txtPersonHouseBuildingNo" />
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="txtPersonStreet" class="col-sm-2 control-label">Street: <span class="required">*</span>:</label>
                                <div class="col-sm-10">
                                    <input data-role="none" class="form-control unicase-form-control text-input" type="text" id="txtPersonStreet" />
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="selPersonCity" class="col-sm-2 control-label">City Or Town: <span class="required">*</span>:</label>
                                <div class="col-sm-10">
                                    <select data-role="none" id="selPersonCity" class="form-control unicase-form-control text-input"></select>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="selPersonRegion" class="col-sm-2 control-label">Region:</label>
                                <div class="col-sm-10">
                                    <select data-role="none" id="selPersonRegion" class="form-control unicase-form-control text-input"></select>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="selPersonIsland" class="col-sm-2 control-label">Island Group:</label>
                                <div class="col-sm-10">
                                    <select data-role="none" id="selPersonIsland" class="form-control unicase-form-control text-input">
                                        <option value="">Please Select Island</option>
                                        <option value="Luzon">Luzon</option>
                                        <option value="Visayas">Visayas</option>
                                        <option value="Mindanao">Mindanao</option>
                                    </select>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="selPersonCountry" class="col-sm-2 control-label">Country: <span class="required">*</span>:</label>
                                <div class="col-sm-10">
                                    <select data-role="none" id="selPersonCountry" class="form-control unicase-form-control text-input"></select>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="selPersonTypeAddress" class="col-sm-2 control-label">Type of Address:</label>
                                <div class="col-sm-10">
                                    <select data-role="none" id="selPersonTypeAddress" class="form-control unicase-form-control text-input">
                                        <option value="Permanent Address">Permanent Address</option>
                                        <option value="Non-Permanent Address">Non-Permanent Address</option>
                                        <option value="Provincial Address">Provincial Address</option>
                                    </select>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="panel panel-default">
                    <div class="panel-heading custom-theme-c">Organization Affiliation</div>
                    <div class="panel-body">
                        <div>
                            <div data-role="fieldcontain" style="text-align:right;">
                                <button id="btnAddPersonOrgAffiliation" data-role="none" class="btn btn-primary btn-less-width">
                                    ADD
                                </button>
                            </div>
                            <div style="padding: 8px 15px 0px 15px;">
                                <ul data-role="listview" id="lstPersonOrgAffiliation" class="li-theme-c noBorderList custom-list custom-list-sm"></ul>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div id="tabPersonContactInfo" custom-tab-role="content" style="display:none;">
                <label style="font-size:1.5em; font-weight:bold;">Person Contact Info</label>
                <br /><br />

                <div class="panel panel-default">
                    <div class="panel-heading custom-theme-c">Telephone Number</div>
                    <div class="panel-body">
                        <div>
                            <div data-role="fieldcontain" style="text-align:right;">
                                <button id="btnAddPersonTelephone" data-role="none" class="btn btn-primary btn-less-width">
                                    ADD NEW TELEPHONE
                                </button>
                            </div>
                            <div style="padding: 8px 15px 0px 15px;">
                                <ul data-role="listview" id="lstPersonTelephone" class="li-theme-c noBorderList custom-list custom-list-sm"></ul>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="panel panel-default">
                    <div class="panel-heading custom-theme-c">Mobile Number</div>
                    <div class="panel-body">
                        <div>
                            <div data-role="fieldcontain" style="text-align:right;">
                                <button id="btnAddPersonMobile" data-role="none" class="btn btn-primary btn-less-width">
                                    ADD NEW MOBILE
                                </button>
                            </div>
                            <div style="padding: 8px 15px 0px 15px;">
                                <ul data-role="listview" id="lstPersonMobile" class="li-theme-c noBorderList custom-list custom-list-sm"></ul>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="panel panel-default">
                    <div class="panel-heading custom-theme-c">Email Address</div>
                    <div class="panel-body">
                        <div>
                            <b>Note:</b> Please enter your valid email address. To be able to use your account, we will send an activation link to activate your account.
                            <br /><br />
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <label for="txtProspectEmailAddress" class="col-sm-2 control-label">Email Address: <span class="required">*</span>:</label>
                                    <div class="col-sm-10">
                                        <input data-role="none" class="form-control unicase-form-control text-input" type="email" id="txtProspectEmailAddress" />
                                    </div>
                                </div>
                            </div>
                            @*<div data-role="fieldcontain" style="text-align:right;">
                                <button id="btnAddPersonEmail" data-role="none" class="btn btn-primary btn-less-width">
                                    ADD NEW EMAIL
                                </button>
                            </div>
                            <div style="padding: 8px 15px 0px 15px;">
                                <ul data-role="listview" id="lstPersonEmail" class="li-theme-c noBorderList custom-list custom-list-sm"></ul>
                            </div>*@
                        </div>
                    </div>
                </div>
            </div>

            <div id="tabPersonEducation" custom-tab-role="content" style="display:none;">
                <div class="panel panel-default">
                    <div class="panel-heading custom-theme-c">Elementary</div>
                    <div class="panel-body">
                        <div>
                            <div data-role="fieldcontain" style="text-align:right;">
                                <button id="btnAddElementary" data-role="none" class="btn btn-primary btn-less-width">
                                    ADD ELEMENTARY SCHOOL
                                </button>
                            </div>
                            <div style="padding: 8px 15px 0px 15px;">
                                <ul data-role="listview" id="lstElementary" class="li-theme-c noBorderList custom-list custom-list-sm"></ul>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="panel panel-default">
                    <div class="panel-heading custom-theme-c">High School</div>
                    <div class="panel-body">
                        <div>
                            <div data-role="fieldcontain" style="text-align:right;">
                                <button id="btnAddHs" data-role="none" class="btn btn-primary btn-less-width">
                                    ADD HIGH SCHOOL
                                </button>
                            </div>
                            <div style="padding: 8px 15px 0px 15px;">
                                <ul data-role="listview" id="lstHs" class="li-theme-c noBorderList custom-list custom-list-sm"></ul>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="panel panel-default">
                    <div class="panel-heading custom-theme-c">College</div>
                    <div class="panel-body">
                        <div>
                            <div data-role="fieldcontain" style="text-align:right;">
                                <button id="btnAddCollege" data-role="none" class="btn btn-primary btn-less-width">
                                    ADD COLLEGE
                                </button>
                            </div>
                            <div style="padding: 8px 15px 0px 15px;">
                                <ul data-role="listview" id="lstCollege" class="li-theme-c noBorderList custom-list custom-list-sm"></ul>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div id="tabPersonLoginCredentials" custom-tab-role="content" style="display:none;">
                <div class="form-horizontal">
                    <div class="form-group">
                        <label for="txtPersonUsername" class="col-sm-2 control-label">Username: <span class="required">*</span> (minimum of 8 characters)</label>
                        <div class="col-sm-10">
                            <input data-role="none" autocomplete="off" type="text" class="form-control unicase-form-control text-input" id="txtPersonUsername">
                        </div>
                    </div>

                    <div class="form-group">
                        <label for="txtPersonPassword" class="col-sm-2 control-label">Password: <span class="required">*</span> (minimum of 8 characters)</label>
                        <div class="col-sm-10">
                            <input data-role="none" autocomplete="off" type="password" class="form-control unicase-form-control text-input" id="txtPersonPassword">
                        </div>
                    </div>

                    <div class="form-group">
                        <label for="txtPersonConfirmPassword" class="col-sm-2 control-label">Confirm Password: <span class="required">*</span></label>
                        <div class="col-sm-10">
                            <input data-role="none" autocomplete="off" type="password" class="form-control unicase-form-control text-input" id="txtPersonConfirmPassword">
                        </div>
                    </div>

                    <h4>Security Questions</h4>
                    <p class="text title-tag-line"><b>NOTE</b>: You must remember the answers to the security questions you selected. This will be used to reset your account in case you forget your password.</p>
                    <br />

                    <div class="form-group">
                        <label for="selPersonSecQuestion1" class="col-sm-2 control-label">Question No. 1: <span class="required">*</span></label>
                        <div class="col-sm-10">
                            <select id="selPersonSecQuestion1" data-role="none" class="form-control unicase-form-control text-input">
                                <option value="">Please Select</option>
                                <option value="Where is your mother's birthplace?">Where is your mother's birthplace?</option>
                                <option value="Who is your best childhood friend?">Who is your best childhood friend?</option>
                                <option value="What is the name of your first pet?">What is the name of your first pet?</option>
                                <option value="Who is your favorite teacher?">Who is your favorite teacher?</option>
                                <option value="Who is your favorite historical person?">Who is your favorite historical person?</option>
                                <option value="What is your grandfather's occupation?">What is your grandfather's occupation?</option>
                            </select>
                            <br />
                            <input data-role="none" type="text" placeholder="Answer of Question No. 1" class="form-control unicase-form-control text-input" id="txtPersonSecQuestion1Answer">
                        </div>
                    </div>

                    <div class="form-group">
                        <label for="selPersonSecQuestion2" class="col-sm-2 control-label">Question No. 2: <span class="required">*</span></label>
                        <div class="col-sm-10">
                            <select id="selPersonSecQuestion2" data-role="none" class="form-control unicase-form-control text-input">
                                <option value="">Please Select</option>
                                <option value="Where is your mother's birthplace?">Where is your mother's birthplace?</option>
                                <option value="Who is your best childhood friend?">Who is your best childhood friend?</option>
                                <option value="What is the name of your first pet?">What is the name of your first pet?</option>
                                <option value="Who is your favorite teacher?">Who is your favorite teacher?</option>
                                <option value="Who is your favorite historical person?">Who is your favorite historical person?</option>
                                <option value="What is your grandfather's occupation?">What is your grandfather's occupation?</option>
                            </select>
                            <br />
                            <input data-role="none" type="text" placeholder="Answer of Question No. 2" class="form-control unicase-form-control text-input" id="txtPersonSecQuestion2Answer">
                        </div>
                    </div>
                </div>
            </div>

            <br />
            <hr />
            <br />
            <div id="divPersonErrMsg" style="color:#ff3737; text-align:center; font-size:14px; margin-bottom:25px; display:none;">Please complete the required field(s)</div>
            <br />
            <div style="font-size:16px;">If you are done, please click the Save button. Once you clicked the button, this link will no longer be accessible for security reasons but you can edit your information later on.</div>
            <br /><br />
            <button data-role="none" id="btnSavePersonAddingContact" class="btn btn-primary btn-lg" style="height:50px;">SAVE</button>
        </div>        

        <div>
            <div data-role="popup" id="popEducation" data-overlay-theme="a" data-theme="a" data-corners="false">
                <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
                <center><h3 id="txtAcademicLevel"></h3></center>
                <div data-role="content" class="ui-content ui-body-d jPopupContent">
                    <input type="text" data-role="none" style="display:none;" id="txtEdeHiID" />
                    <div class="form-horizontal">
                        <div class="form-group">
                            <label for="txtEdeHiSchoolName" class="col-sm-3 control-label">School Name:</label>
                            <div class="col-sm-9">
                                <input data-role="none" autocomplete="off" type="text" class="form-control unicase-form-control text-input" id="txtEdeHiSchoolName">
                            </div>
                        </div>

                        <div class="form-group">
                            <label for="txtEdeHiYearStarted" class="col-sm-3 control-label">Year Stared:</label>
                            <div class="col-sm-9">
                                <input data-role="none" autocomplete="off" type="text" class="form-control unicase-form-control text-input" id="txtEdeHiYearStarted">
                            </div>
                        </div>

                        <div class="form-group">
                            <label for="txtEdeHiYearGraduated" class="col-sm-3 control-label">Year Gradauted:</label>
                            <div class="col-sm-9">
                                <input data-role="none" autocomplete="off" type="text" class="form-control unicase-form-control text-input" id="txtEdeHiYearGraduated">
                            </div>
                        </div>
                    </div>

                    <br />
                    <button id="btnOkEduHi" data-role="none" class="btn custom-btn-theme-b">Ok</button>
                    <br /><br />
                    <a id="btnCancelEduHi" data-rel="back" data-role="none" class="btn custom-btn-theme-c">Cancel</a>
                </div>
            </div>

            <div id="popPersonTelInfo" custom-data-role="popup">
                <div class="popup-md">
                    <div class="popup-header">
                        Telephone Info
                        <a class="popup-close">x</a>
                    </div>

                    <div class="popup-content">
                        <div data-role="fieldcontain" style="display:none;">
                            <input type="text" readonly id="txtPersonTelCountryID" value="e783ed13-aa98-4cc1-918e-d49b05c0167e" />
                            <input type="text" readonly id="txtPersonTelID" />
                            <input type="text" readonly id="txtPersonTelAreaCodeID" />
                        </div>

                        <div class="form-horizontal">
                            <div class="form-group">
                                <label for="btnPersonTelCountryCodeDisable" class="col-sm-2 control-label">Country Code: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <button id="btnPersonTelCountryCodeDisable" data-role="none" class="btn custom-btn-theme-c">
                                        <span id="txtPersonCountryCode">PHL - 63</span>
                                    </button>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="btnPersonTelAreaCode" class="col-sm-2 control-label">Area Code: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <button id="btnPersonTelAreaCode" data-role="none" class="btn custom-btn-theme-c">
                                        <span id="txtPersonAreaCode">PHL - 63</span>
                                    </button>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="txtPersonTelephoneNum" class="col-sm-2 control-label">Telephone Number: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <input type="text" id="txtPersonTelephoneNum" data-role="none" class="form-control unicase-form-control text-input" />
                                    <br />
                                    <span id="spanerrorPersonTelephoneNum" style="color:#F00; display:none;">This field is required</span>
                                    <span id="spanerrorPersonTelephoneNumValidate" style="color:#F00; display:none;">Enter 7 digit number</span>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="txtPersonTelephoneExt" class="col-sm-2 control-label">Extension or Local No.:</label>
                                <div class="col-sm-10">
                                    <input type="text" id="txtPersonTelephoneExt" maxlength="10" data-role="none" class="form-control unicase-form-control text-input" />
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="selPersonTelType" class="col-sm-2 control-label">Contact Type: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <select data-role="none" id="selPersonTelType" class="form-control unicase-form-control text-input">
                                        <option value="">Please Select</option>
                                        <option value="Work Telephone">Work Telephone</option>
                                        <option value="Personal Telephone">Personal Telephone</option>
                                        <option value="Third Party Telephone">Third Party Telephone</option>
                                    </select>
                                    <br />
                                    <span id="spanerrorPersonTelType" style="color:#F00; display:none;">This field is required</span>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="popup-footer align-right">
                        <div style="text-align:right;">
                            <button id="btnCancelPersonTelephone" data-role="none" class="btn custom-btn-theme-c btnCircle">
                                <img alt="Options" src="/Content/images/close-black.png" />
                            </button>
                            <button id="btnSavePersonTelephone" data-role="none" class="btn custom-btn-theme-b btnCircle">
                                <img alt="Options" src="/Content/images/check.png" />
                            </button>
                        </div>
                    </div>
                </div>
            </div>

            <div id="popPersonMobileInfo" custom-data-role="popup">
                <div class="popup-md">
                    <div class="popup-header">
                        Mobile Info
                        <a class="popup-close">x</a>
                    </div>

                    <div class="popup-content">
                        <div data-role="fieldcontain" style="display:none;">
                            <input type="text" readonly id="txtPersonMobileCountryID" value="e783ed13-aa98-4cc1-918e-d49b05c0167e" />
                            <input type="text" readonly id="txtPersonMobileID" />
                            <input type="text" readonly id="txtPersonTelCoPrefixID" />
                        </div>

                        <div class="form-horizontal">
                            <div class="form-group">
                                <label for="btnPersonMobileCountryCodeDisable" class="col-sm-2 control-label">Country Code: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <button id="btnPersonMobileCountryCodeDisable" data-role="none" class="btn custom-btn-theme-c">
                                        <span id="txtPersonMobileCountryCode">PHL - 63</span>
                                    </button>
                                    <br /><span id="spanerrorPersonMobileCountryCode" style="color:#F00; display:none;">This field is required</span>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="txtPersonMobileNo" class="col-sm-2 control-label">Mobile Number: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <input data-role="none" type="text" id="txtPersonMobileNo" class="form-control unicase-form-control text-input" />
                                    <br />
                                    <span id="spanerrorPersonMobileNum" style="color:#F00; display:none;">This field is required</span>
                                    <span id="spanerrorPersonMobileNumValidate" style="color:#F00; display:none;">Enter 7 digit number</span>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="selPersonMobileType" class="col-sm-2 control-label">Contact Type: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <select data-role="none" id="selPersonMobileType" class="form-control unicase-form-control text-input">
                                        <option value="">Please Select</option>
                                        <option value="Work Mobile">Work Mobile</option>
                                        <option value="Personal Mobile">Personal Mobile</option>
                                        <option value="Third Party Mobile">Third Party Mobile</option>
                                    </select>
                                    <br />
                                    <span id="spanerrorPersonMobileType" style="color:#F00; display:none;">This field is required</span>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="popup-footer align-right">
                        <div style="text-align:right;">
                            <button id="btnCancelPersonMobile" data-role="none" class="btn custom-btn-theme-c btnCircle">
                                <img alt="Options" src="/Content/images/close-black.png" />
                            </button>
                            <button id="btnSavePersonMobile" data-role="none" class="btn custom-btn-theme-b btnCircle">
                                <img alt="Options" src="/Content/images/check.png" />
                            </button>
                        </div>
                    </div>
                </div>
            </div>

            <div id="popPersonEmailInfo" custom-data-role="popup">
                <div class="popup-md">
                    <div class="popup-header">
                        Email Info
                        <a class="popup-close">x</a>
                    </div>

                    <div class="popup-content">
                        <div data-role="fieldcontain" style="display:none;">
                            <input type="text" readonly id="txtPersonEmailID" />
                        </div>

                        <div class="form-horizontal">
                            <div class="form-group">
                                <label for="txtPersonEmailAdd" class="col-sm-2 control-label">Email Address: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <input data-role="none" type="text" id="txtPersonEmailAdd" class="form-control unicase-form-control text-input" />
                                    <br />
                                    <span id="spanerrorPersonEmailAddress" style="color:#F00; display:none;">This field is required</span>
                                    <span id="spanerrorPersonEmailAddressValidate" style="color:#F00; display:none;">Enter 7 digit number</span>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="selPersonEmailType" class="col-sm-2 control-label">Contact Type: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <select data-role="none" id="selPersonEmailType" class="form-control unicase-form-control text-input">
                                        <option value="">Please Select</option>
                                        <option value="Work Email">Work Email</option>
                                        <option value="Personal Email">Personal Email</option>
                                        <option value="Third Party Email">Third Party Email</option>
                                    </select>
                                    <br />
                                    <span id="spanerrorPersonEmailType" style="color:#F00; display:none;">This field is required</span>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="popup-footer align-right">
                        <div style="text-align:right;">
                            <button id="btnCancelPersonEmailAddress" data-role="none" class="btn custom-btn-theme-c btnCircle">
                                <img alt="Options" src="/Content/images/close-black.png" />
                            </button>
                            <button id="btnSavePersonEmailAddress" data-role="none" class="btn custom-btn-theme-b btnCircle">
                                <img alt="Options" src="/Content/images/check.png" />
                            </button>
                        </div>
                    </div>
                </div>
            </div>

            <div id="popContactPersonProfile" custom-data-role="popup">
                <div class="popup-md">
                    <div class="popup-header">
                        Person Profile
                        <a class="popup-close">x</a>
                    </div>

                    <div class="popup-content">
                        <input type="text" id="txtPeopleID" style="display:none;" data-role="none" />
                        <input type="text" id="txtContactPersonID" style="display:none;" data-role="none" />

                        <div class="custom-ui-label">
                            <div>Full Name:</div>
                            <div>
                                <input type="text" id="txtContactFirstName" placeholder="Firstname" />
                                <span id="spanerrorContactFname" style="color:#F00; display:none;">This field is required<br /><br /></span>
                                <input type="text" id="txtContactMiddleName" placeholder="Middlename" />
                                <input type="text" id="txtContactLastName" placeholder="Lastname" />
                            </div>
                        </div>

                        <div class="custom-ui-label">
                            <div>Generational Suffix:</div>
                            <div>
                                <select id="selContactGenSuffix">
                                    <option value="">None</option>
                                    <option value="Jr.">Jr.</option>
                                    <option value="Sr.">Sr.</option>
                                    <option value="I">I</option>
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
                        </div>

                        <div class="custom-ui-label">
                            <div>TIN: </div>
                            <div>
                                <input type="text" id="txtContactPersonTIN" />
                            </div>
                        </div>

                        <div class="custom-ui-label">
                            <div>Position/Title: </div>
                            <div>
                                <input type="text" id="txtContactPersonPosition" />
                            </div>
                        </div>

                        <div class="custom-ui-label">
                            <div>Position/Title Description: </div>
                            <div>
                                <input type="text" id="txtContactPersonPositionDescription" />
                            </div>
                        </div>
                        <br />

                        <div class="panel">
                            <div class="panel-title custom-theme-h">
                                Address
                            </div>

                            <div class="panel-content txt-left">
                                <input data-role="none" type="text" id="txtContactPersonAddressID" style="display:none;" />
                                <div class="custom-ui-label">
                                    <div>House or Building No:</div>
                                    <div><input type="text" id="txtContactPersonHouseBuildingNo" /></div>
                                </div>

                                <div class="custom-ui-label">
                                    <div>Street:</div>
                                    <div><input type="text" id="txtContactPersonStreet" /></div>
                                </div>

                                <div class="custom-ui-label">
                                    <div>City Or Town:</div>
                                    <div><select id="selContactPersonCity"></select></div>
                                </div>

                                <div class="custom-ui-label">
                                    <div>Country:</div>
                                    <div><select id="selContactPersonCountry"></select></div>
                                </div>

                                <div class="custom-ui-label">
                                    <div>Type of Address:</div>
                                    <div>
                                        <select id="selContactPersonTypeAddress">
                                            <option value="Permanent Address">Permanent Address</option>
                                            <option value="Non-Permanent Address">Non-Permanent Address</option>
                                            <option value="Provincial Address">Provincial Address</option>
                                        </select>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <br /><br />
                        <label style="font-size:1.5em; font-weight:bold;">Person Contact Info</label>
                        <br /><br />
                        <div class="panel">
                            <div class="panel-title custom-theme-h">
                                Telephone Number
                            </div>

                            <div class="panel-content txt-left">
                                <div data-role="fieldcontain">
                                    <div id="divContactPersonTel" subpage="contactpersontelpage" style="display:block;">
                                        <div data-role="fieldcontain" style="text-align:right;">
                                            <button id="btnAddContactPersonTelephone" data-role="none" class="btn custom-btn-theme-b btnCircle">
                                                <img alt="Add" src="/Content/images/plus-white.png" style="width:18px;" />
                                            </button>
                                        </div>
                                        <div style="padding: 8px 15px 0px 15px;">
                                            <ul data-role="listview" id="lstContactPersonTelephone" class="li-theme-c noBorderList custom-list custom-list-sm"></ul>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <br /><br /><br />
                        <div class="panel">
                            <div class="panel-title custom-theme-h">
                                Mobile Number
                            </div>

                            <div class="panel-content txt-left">
                                <div data-role="fieldcontain">
                                    <div id="divContactPersonMobileHome" subpage="contactpersonmobilepage" style="display:block;">
                                        <div data-role="fieldcontain" style="text-align:right;">
                                            <button id="btnAddContactPersonMobile" data-role="none" class="btn custom-btn-theme-b btnCircle">
                                                <img alt="Add" src="/Content/images/plus-white.png" style="width:18px;" />
                                            </button>
                                        </div>
                                        <div style="padding: 8px 15px 0px 15px;">
                                            <ul data-role="listview" id="lstContactPersonMobile" class="li-theme-c noBorderList custom-list custom-list-sm"></ul>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <br /><br /><br />
                        <div class="panel">
                            <div class="panel-title custom-theme-h">
                                Email Address
                            </div>

                            <div class="panel-content txt-left">
                                <div data-role="fieldcontain">
                                    <div id="divContactPersonEmailHome" subpage="contactpersonemailpage" style="display:block;">
                                        <div data-role="fieldcontain" style="text-align:right;">
                                            <button id="btnAddContactPersonEmail" data-role="none" class="btn custom-btn-theme-b btnCircle">
                                                <img alt="Add" src="/Content/images/plus-white.png" style="width:18px;" />
                                            </button>
                                        </div>
                                        <div style="padding: 8px 15px 0px 15px;">
                                            <ul data-role="listview" id="lstContactPersonEmail" class="li-theme-c noBorderList custom-list custom-list-sm"></ul>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="popup-footer align-right">
                        <div style="text-align:right;">
                            <button id="btnCancelContact" data-role="none" class="btn custom-btn-theme-c btnCircle">
                                <img alt="Options" src="/Content/images/close-black.png" />
                            </button>
                            <button id="btnOkContact" data-role="none" class="btn custom-btn-theme-b btnCircle">
                                <img alt="Options" src="/Content/images/check.png" />
                            </button>
                        </div>
                    </div>
                </div>
            </div>

            <div id="popOrgTelInfo" custom-data-role="popup">
                <div class="popup-md">
                    <div class="popup-header">
                        Telephone Info
                        <a class="popup-close">x</a>
                    </div>

                    <div class="popup-content">
                        <div data-role="fieldcontain" style="display:none;">
                            <input type="text" readonly id="txtOrgTelCountryID" value="e783ed13-aa98-4cc1-918e-d49b05c0167e" />
                            <input type="text" readonly id="txtOrgTelID" />
                            <input type="text" readonly id="txtOrgTelAreaCodeID" />
                        </div>

                        <div class="form-horizontal">
                            <div class="form-group">
                                <label for="btnOrgTelCountryCodeDisable" class="col-sm-2 control-label">Country Code: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <button id="btnOrgTelCountryCodeDisable" data-role="none" class="btn custom-btn-theme-c">
                                        <span id="txtCountryCode">PHL - 63</span>
                                    </button>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="btnOrgTelAreaCode" class="col-sm-2 control-label">Area Code: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <button id="btnOrgTelAreaCode" data-role="none" class="btn custom-btn-theme-c">
                                        <span id="txtAreaCode">PHL - 63</span>
                                    </button>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="txtTelephoneNum" class="col-sm-2 control-label">Telephone Number: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <input type="text" id="txtTelephoneNum" data-role="none" class="form-control unicase-form-control text-input" />
                                    <br />
                                    <span id="spanerrorTelephoneNum" style="color:#F00; display:none;">This field is required</span>
                                    <span id="spanerrorTelephoneNumValidate" style="color:#F00; display:none;">Enter 7 digit number</span>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="txtTelephoneExt" class="col-sm-2 control-label">Telephone Number: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <input type="text" id="txtTelephoneExt" maxlength="10" data-role="none" class="form-control unicase-form-control text-input" />
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="selOrgTelType" class="col-sm-2 control-label">Contact Type: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <select data-role="none" id="selOrgTelType" class="form-control unicase-form-control text-input">
                                        <option value="">Please Select</option>
                                        <option value="Work Telephone">Work Telephone</option>
                                        <option value="Personal Telephone">Personal Telephone</option>
                                        <option value="Third Party Telephone">Third Party Telephone</option>
                                    </select>
                                    <br />
                                    <span id="spanerrorTelType" style="color:#F00; display:none;">This field is required</span>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="popup-footer align-right">
                        <div style="text-align:right;">
                            <button id="btnCancelOrgTelephone" data-role="none" class="btn custom-btn-theme-c btnCircle">
                                <img alt="Options" src="/Content/images/close-black.png" />
                            </button>
                            <button id="btnSaveOrgTelephone" data-role="none" class="btn custom-btn-theme-b btnCircle">
                                <img alt="Options" src="/Content/images/check.png" />
                            </button>
                        </div>
                    </div>
                </div>
            </div>

            <div id="popOrgMobileInfo" custom-data-role="popup">
                <div class="popup-md">
                    <div class="popup-header">
                        Mobile Info
                        <a class="popup-close">x</a>
                    </div>

                    <div class="popup-content">
                        <div data-role="fieldcontain" style="display:none;">
                            <input type="text" readonly id="txtOrgMobileCountryID" value="e783ed13-aa98-4cc1-918e-d49b05c0167e" />
                            <input type="text" readonly id="txtOrgMobileID" />
                            <input type="text" readonly id="txtOrgTelCoPrefixID" />
                        </div>

                        <div class="form-horizontal">
                            <div class="form-group">
                                <label for="btnOrgMobileCountryCodeDisable" class="col-sm-2 control-label">Country Code: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <button id="btnOrgMobileCountryCodeDisable" data-role="none" class="btn custom-btn-theme-c">
                                        <span id="txtMobileCountryCode">PHL - 63</span>
                                    </button>
                                    <br /><span id="spanerrorMobileCountryCode" style="color:#F00; display:none;">This field is required</span>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="txtMobileNo" class="col-sm-2 control-label">Mobile Number: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <input data-role="none" type="text" id="txtMobileNo" class="form-control unicase-form-control text-input" />
                                    <br />
                                    <span id="spanerrorMobileNum" style="color:#F00; display:none;">This field is required</span>
                                    <span id="spanerrorMobileNumValidate" style="color:#F00; display:none;">Enter 7 digit number</span>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="selOrgMobileType" class="col-sm-2 control-label">Contact Type: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <select data-role="none" id="selOrgMobileType" class="form-control unicase-form-control text-input">
                                        <option value="">Please Select</option>
                                        <option value="Work Mobile">Work Mobile</option>
                                        <option value="Personal Mobile">Personal Mobile</option>
                                        <option value="Third Party Mobile">Third Party Mobile</option>
                                    </select>
                                    <br />
                                    <span id="spanerrorMobileType" style="color:#F00; display:none;">This field is required</span>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="popup-footer align-right">
                        <div style="text-align:right;">
                            <button id="btnCancelOrgMobile" data-role="none" class="btn custom-btn-theme-c btnCircle">
                                <img alt="Options" src="/Content/images/close-black.png" />
                            </button>
                            <button id="btnSaveOrgMobile" data-role="none" class="btn custom-btn-theme-b btnCircle">
                                <img alt="Options" src="/Content/images/check.png" />
                            </button>
                        </div>
                    </div>
                </div>
            </div>

            <div id="popOrgEmailInfo" custom-data-role="popup">
                <div class="popup-md">
                    <div class="popup-header">
                        Email Info
                        <a class="popup-close">x</a>
                    </div>

                    <div class="popup-content">
                        <div data-role="fieldcontain" style="display:none;">
                            <input type="text" readonly id="txtOrgEmailID" />
                        </div>

                        <div class="form-horizontal">
                            <div class="form-group">
                                <label for="txtEmailAdd" class="col-sm-2 control-label">Email Address: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <input data-role="none" type="text" id="txtEmailAdd" class="form-control unicase-form-control text-input" />
                                    <br />
                                    <span id="spanerrorEmailAddress" style="color:#F00; display:none;">This field is required</span>
                                    <span id="spanerrorEmailAddressValidate" style="color:#F00; display:none;">Enter 7 digit number</span>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="selOrgEmailType" class="col-sm-2 control-label">Contact Type: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <select data-role="none" id="selOrgEmailType" class="form-control unicase-form-control text-input">
                                        <option value="">Please Select</option>
                                        <option value="Work Email">Work Email</option>
                                        <option value="Personal Email">Personal Email</option>
                                        <option value="Third Party Email">Third Party Email</option>
                                    </select>
                                    <br />
                                    <span id="spanerrorEmailType" style="color:#F00; display:none;">This field is required</span>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="popup-footer align-right">
                        <div style="text-align:right;">
                            <button id="btnCancelOrgEmailAddress" data-role="none" class="btn custom-btn-theme-c btnCircle">
                                <img alt="Options" src="/Content/images/close-black.png" />
                            </button>
                            <button id="btnSaveOrgEmailAddress" data-role="none" class="btn custom-btn-theme-b btnCircle">
                                <img alt="Options" src="/Content/images/check.png" />
                            </button>
                        </div>
                    </div>
                </div>
            </div>

            <div id="popOrgWebsiteInfo" custom-data-role="popup">
                <div class="popup-md">
                    <div class="popup-header">
                        Website Info
                        <a class="popup-close">x</a>
                    </div>

                    <div class="popup-content">
                        <div data-role="fieldcontain" style="display:none;">
                            <input type="text" readonly id="txtOrgWebsiteID" />
                        </div>

                        <div class="form-horizontal">
                            <div class="form-group">
                                <label for="txtWebsiteUrl" class="col-sm-2 control-label">Website URL: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <input data-role="none" type="text" id="txtWebsiteUrl" class="form-control unicase-form-control text-input" />
                                    <br />
                                    <span id="spanerrorWebsiteUrl" style="color:#F00; display:none;">This field is required</span>
                                    <span id="spanerrorEmailAddressValidate" style="color:#F00; display:none;">Enter 7 digit number</span>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="popup-footer align-right">
                        <div style="text-align:right;">
                            <button id="btnCancelOrgWebsite" data-role="none" class="btn custom-btn-theme-c btnCircle">
                                <img alt="Options" src="/Content/images/close-black.png" />
                            </button>
                            <button id="btnSaveOrgWebsite" data-role="none" class="btn custom-btn-theme-b btnCircle">
                                <img alt="Options" src="/Content/images/check.png" />
                            </button>
                        </div>
                    </div>
                </div>
            </div>

            <div id="popContactPersonTelInfo" custom-data-role="popup">
                <div class="popup-md">
                    <div class="popup-header">
                        Telephone Info
                        <a class="popup-close">x</a>
                    </div>

                    <div class="popup-content">
                        <div data-role="fieldcontain" style="display:none;">
                            <input type="text" readonly id="txtContactPersonTelCountryID" value="e783ed13-aa98-4cc1-918e-d49b05c0167e" />
                            <input type="text" readonly id="txtContactPersonTelID" />
                            <input type="text" readonly id="txtContactPersonTelAreaCodeID" />
                        </div>

                        <div class="form-horizontal">
                            <div class="form-group">
                                <label for="btnContactPersonTelCountryCodeDisable" class="col-sm-2 control-label">Country Code: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <button id="txtContactPersonCountryCode" data-role="none" class="btn custom-btn-theme-c">
                                        <span id="txtCountryCode">PHL - 63</span>
                                    </button>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="btnContactPersonTelAreaCode" class="col-sm-2 control-label">Area Code: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <button id="btnContactPersonTelAreaCode" data-role="none" class="btn custom-btn-theme-c">
                                        <span id="txtContactPersonAreaCode">PHL - 63</span>
                                    </button>
                                    <br /> <span id="spanerrorContactPersonAreaCode" style="color:#F00; display:none;">This field is required</span>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="txtContactPersonTelephoneNum" class="col-sm-2 control-label">Telephone Number: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <input type="text" id="txtContactPersonTelephoneNum" data-role="none" class="form-control unicase-form-control text-input" />
                                    <br />
                                    <span id="spanerrorContactPersonTelephoneNum" style="color:#F00; display:none;">This field is required</span>
                                    <span id="spanerrorContactPersonTelephoneNumValidate" style="color:#F00; display:none;">Enter 7 digit number</span>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="txtContactPersonTelephoneExt" class="col-sm-2 control-label">Telephone Number: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <input type="text" id="txtContactPersonTelephoneExt" maxlength="10" data-role="none" class="form-control unicase-form-control text-input" />
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="selContactPersonTelType" class="col-sm-2 control-label">Contact Type: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <select data-role="none" id="selContactPersonTelType" class="form-control unicase-form-control text-input">
                                        <option value="">Please Select</option>
                                        <option value="Work Telephone">Work Telephone</option>
                                        <option value="Personal Telephone">Personal Telephone</option>
                                        <option value="Third Party Telephone">Third Party Telephone</option>
                                    </select>
                                    <br />
                                    <span id="spanerrorContactPersonTelType" style="color:#F00; display:none;">This field is required</span>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="popup-footer align-right">
                        <div style="text-align:right;">
                            <button id="btnCancelContactPersonTelephone" data-role="none" class="btn custom-btn-theme-c btnCircle">
                                <img alt="Options" src="/Content/images/close-black.png" />
                            </button>
                            <button id="btnSaveContactPersonTelephone" data-role="none" class="btn custom-btn-theme-b btnCircle">
                                <img alt="Options" src="/Content/images/check.png" />
                            </button>
                        </div>
                    </div>
                </div>
            </div>

            <div id="popContactPersonMobileInfo" custom-data-role="popup">
                <div class="popup-md">
                    <div class="popup-header">
                        Mobile Info
                        <a class="popup-close">x</a>
                    </div>

                    <div class="popup-content">
                        <div data-role="fieldcontain" style="display:none;">
                            <input type="text" readonly id="txtContactPersonMobileCountryID" value="e783ed13-aa98-4cc1-918e-d49b05c0167e" />
                            <input type="text" readonly id="txtContactPersonMobileID" />
                            <input type="text" readonly id="txtContactPersonTelCoPrefixID" />
                        </div>

                        <div class="form-horizontal">
                            <div class="form-group">
                                <label for="btnContactPersonMobileCountryCodeDisable" class="col-sm-2 control-label">Country Code: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <button id="btnContactPersonMobileCountryCodeDisable" data-role="none" class="btn custom-btn-theme-c">
                                        <span id="txtContactPersonMobileCountryCode">PHL - 63</span>
                                    </button>
                                    <br /><span id="spanerrorContactPersonMobileCountryCode" style="color:#F00; display:none;">This field is required</span>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="txtContactPersonMobileNo" class="col-sm-2 control-label">Mobile Number: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <input data-role="none" type="text" id="txtContactPersonMobileNo" class="form-control unicase-form-control text-input" />
                                    <br />
                                    <span id="spanerrorContactPersonMobileNum" style="color:#F00; display:none;">This field is required</span>
                                    <span id="spanerrorContactPersonMobileNumValidate" style="color:#F00; display:none;">Enter 7 digit number</span>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="selContactPersonMobileType" class="col-sm-2 control-label">Contact Type: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <select data-role="none" id="selContactPersonMobileType" class="form-control unicase-form-control text-input">
                                        <option value="">Please Select</option>
                                        <option value="Work Mobile">Work Mobile</option>
                                        <option value="Personal Mobile">Personal Mobile</option>
                                        <option value="Third Party Mobile">Third Party Mobile</option>
                                    </select>
                                    <br />
                                    <span id="spanerrorContactPersonMobileType" style="color:#F00; display:none;">This field is required</span>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="popup-footer align-right">
                        <div style="text-align:right;">
                            <button id="btnCancelContactPersonMobile" data-role="none" class="btn custom-btn-theme-c btnCircle">
                                <img alt="Options" src="/Content/images/close-black.png" />
                            </button>
                            <button id="btnSaveContactPersonMobile" data-role="none" class="btn custom-btn-theme-b btnCircle">
                                <img alt="Options" src="/Content/images/check.png" />
                            </button>
                        </div>
                    </div>
                </div>
            </div>

            <div id="popContactPersonEmailInfo" custom-data-role="popup">
                <div class="popup-md">
                    <div class="popup-header">
                        Email Info
                        <a class="popup-close">x</a>
                    </div>

                    <div class="popup-content">
                        <div data-role="fieldcontain" style="display:none;">
                            <input type="text" readonly id="txtContactPersonEmailID" />
                        </div>

                        <div class="form-horizontal">
                            <div class="form-group">
                                <label for="txtContactPersonEmailAdd" class="col-sm-2 control-label">Email Address: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <input data-role="none" type="text" id="txtContactPersonEmailAdd" class="form-control unicase-form-control text-input" />
                                    <br />
                                    <span id="spanerrorContactPersonEmailAddress" style="color:#F00; display:none;">This field is required</span>
                                    <span id="spanerrorContactPersonEmailAddressValidate" style="color:#F00; display:none;">Enter 7 digit number</span>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="selContactPersonEmailType" class="col-sm-2 control-label">Contact Type: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <select data-role="none" id="selContactPersonEmailType" class="form-control unicase-form-control text-input">
                                        <option value="">Please Select</option>
                                        <option value="Work Email">Work Email</option>
                                        <option value="Personal Email">Personal Email</option>
                                        <option value="Third Party Email">Third Party Email</option>
                                    </select>
                                    <br />
                                    <span id="spanerrorContactPersonEmailType" style="color:#F00; display:none;">This field is required</span>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="popup-footer align-right">
                        <div style="text-align:right;">
                            <button id="btnCancelContactPersonEmailAddress" data-role="none" class="btn custom-btn-theme-c btnCircle">
                                <img alt="Options" src="/Content/images/close-black.png" />
                            </button>
                            <button id="btnSaveContactPersonEmailAddress" data-role="none" class="btn custom-btn-theme-b btnCircle">
                                <img alt="Options" src="/Content/images/check.png" />
                            </button>
                        </div>
                    </div>
                </div>
            </div>

            <div id="prevOrgProfile" class="profile-preview" style="display:none;">
                <div class="grid grid-a">
                    <div id="btnPreviewClose" style="background: rgba(0,0,0,1); padding:15px; position: absolute; top: 0; width: 100%; right: 0; left: 0; color: #FFF; margin-bottom: 10px; cursor:pointer;">
                        Close
                    </div>
                    <br />
                    <div style="background-color:#FFF; padding:15px;">
                        <div id="roOrg" subpage="pagePreview" style="display:block;">
                            <h3>Company Profile</h3>
                            <div class="custom-ui-label">
                                <div>Name:</div>
                                <div id="txtpreviewCompanyName"></div>
                            </div>
                            <div class="custom-ui-label">
                                <div>Address:</div>
                                <div id="txtpreviewCompanyAddress"></div>
                            </div>
                            <div class="custom-ui-label">
                                <div>Background:</div>
                                <div id="txtpreviewCompanyBackground"></div>
                            </div>
                            <div class="custom-ui-label">
                                <div>Mission:</div>
                                <div id="txtpreviewCompanyMission"></div>
                            </div>
                            <div class="custom-ui-label">
                                <div>Vision:</div>
                                <div id="txtpreviewCompanyVision"></div>
                            </div>
                            <div class="custom-ui-label">
                                <div>Founding Date/Year:</div>
                                <div id="txtpreviewCompanyFounding"></div>
                            </div>
                            <div class="custom-ui-label">
                                <div>Business Industry:</div>
                                <div id="txtpreviewCompanyIndustry"></div>
                            </div>
                            <br />
                            <h3>Organization Affiliation</h3>
                            <ol id="listpreviewOrgAff">
                                <li>List here</li>
                            </ol>
                            <br />
                            <h3>Affiliated Person</h3>
                            <ol id="listpreviewAffPerson">
                                <li>List here</li>
                            </ol>
                            <br />
                            <h3>Contact Info</h3>
                            <ul class="list-padding">
                                <li>
                                    Telephone
                                    <ol class="list-padding" id="listpreviewOrgTel">
                                        <li>List of Telephone here</li>
                                    </ol>
                                </li>
                                <li>
                                    Mobile
                                    <ol class="list-padding" id="listpreviewOrgMobile">
                                        <li>List of Mobile here</li>
                                    </ol>
                                </li>
                                <li>
                                    Email
                                    <ol class="list-padding" id="listpreviewOrgEmail">
                                        <li>List of Email here</li>
                                    </ol>
                                </li>
                                <li>
                                    Website
                                    <ol class="list-padding" id="listpreviewOrgWebsite">
                                        <li>List of Website here</li>
                                    </ol>
                                </li>
                            </ul>
                            <br />
                            <h3>Contact Person</h3>
                            <ol class="list-padding" id="listpreviewOrgContactPerson">
                                <li>List here</li>
                            </ol>
                            <br />
                            <h3>Company Size</h3>
                            <div class="custom-ui-label">
                                <div>No. of Employees:</div>
                                <div id="txtprevNoOfEmp"></div>
                            </div>
                            <div class="custom-ui-label">
                                <div>Revenue per Year:</div>
                                <div id="txtprevRevenue"></div>
                            </div>
                            <div class="custom-ui-label">
                                <div>Scope of Operations:</div>
                                <div id="txtprevScope"></div>
                            </div>
                            <div class="custom-ui-label">
                                <div>No. of Factories:</div>
                                <div id="txtprevNoOfFactories"></div>
                            </div>
                            <div class="custom-ui-label">
                                <div>No. of Offices:</div>
                                <div id="txtprevNoOfOffices"></div>
                            </div>
                            <div class="custom-ui-label">
                                <div>No. of Warehouses:</div>
                                <div id="txtprevNoOfWarehouse"></div>
                            </div>
                            <div class="custom-ui-label">
                                <div>No. of Stores:</div>
                                <div id="txtprevNoOfStores"></div>
                            </div>
                            <div class="custom-ui-label">
                                <div>No. of Trucks:</div>
                                <div id="txtprevNoOfTrucks"></div>
                            </div>
                            @*<br />
                            <h3>Revenue</h3>
                            <div class="custom-ui-label">
                                <div>Sales Representative:</div>
                                <div id="txtprevSalesRep"></div>
                            </div>
                            <div class="custom-ui-label">
                                <div>Price Level:</div>
                                <div id="txtprevPriceLvl"></div>
                            </div>
                            <div class="custom-ui-label">
                                <div>Credit Terms:</div>
                                <div id="txtprevCreditTerms"></div>
                            </div>
                            <div class="custom-ui-label">
                                <div>Credit Limit:</div>
                                <div id="txtprevCreditLimit"></div>
                            </div>
                            <div class="custom-ui-label">
                                <div>Credit Beg. Balance:</div>
                                <div id="txtprevCreditBeg"></div>
                            </div>
                            <div class="custom-ui-label">
                                <div>Credit Beg. as of:</div>
                                <div id="txtprevCreditBegAsOf"></div>
                            </div>*@
                        </div>

                        <div id="roPerson" subpage="pagePreview" style="display:none;">
                            <h3>Person Profile</h3>
                            <div class="custom-ui-label">
                                <div>Full Name:</div>
                                <div id="txtpreviewPersonName"></div>
                            </div>
                            <div class="custom-ui-label">
                                <div>Date of Birth:</div>
                                <div id="txtpreviewBirthDate"></div>
                            </div>
                            <div class="custom-ui-label">
                                <div>TIN:</div>
                                <div id="txtpreviewPersonTin"></div>
                            </div>
                            <div class="custom-ui-label">
                                <div>Position:</div>
                                <div id="txtpreviewPosition"></div>
                            </div>
                            <div class="custom-ui-label">
                                <div>Position Description:</div>
                                <div id="txtpreviewPositionDesc"></div>
                            </div>
                            <div class="custom-ui-label">
                                <div>Address:</div>
                                <div id="txtprevAddress"></div>
                            </div>
                            <br />
                            <h3>Organization Affiliation</h3>
                            <ol id="listpreviewPersonOrgAff">
                                <li>List here</li>
                            </ol>
                            <br />
                            <h3>Contact Info</h3>
                            <ul class="list-padding">
                                <li>
                                    Telephone
                                    <ol class="list-padding" id="listpreviewPersonTel">
                                        <li>List of Telephone here</li>
                                    </ol>
                                </li>
                                <li>
                                    Mobile
                                    <ol class="list-padding" id="listpreviewPersonMobile">
                                        <li>List of Mobile here</li>
                                    </ol>
                                </li>
                                <li>
                                    Email
                                    <ol class="list-padding" id="listpreviewPersonEmail">
                                        <li>List of Email here</li>
                                    </ol>
                                </li>
                            </ul>
                            <br />
                            <h3>Educational Background</h3>
                            <ul class="list-padding">
                                <li>
                                    Elementary
                                    <ol class="list-padding" id="listpreviewPersonElem">
                                        <li>List of Telephone here</li>
                                    </ol>
                                </li>
                                <li>
                                    High School
                                    <ol class="list-padding" id="listpreviewPersonHS">
                                        <li>List of Mobile here</li>
                                    </ol>
                                </li>
                                <li>
                                    College
                                    <ol class="list-padding" id="listpreviewPersonCollege">
                                        <li>List of Email here</li>
                                    </ol>
                                </li>
                            </ul>
                        </div>
                    </div>
                </div>
                <div class="grid grid-b">
                    <div class="panel-menu">
                        <ul id="listPreviewMenu" class="custom-list-menu txt-left" panel-menu="true">
                            <li><a id="" data-gotopage="pageActions" menu-button="#btnPageProfile" class="btnPreview active-menu ui-link">View Profile</a></li>
                            @*<li><a id="" menu-button="#btnPageColdCall" class="btnPreview ui-link">Cold Call</a></li>
                            <li><a id="" data-gotopage="pageActions" menu-button="#btnPageToDo" class="btnPreview ui-link">To-Do</a></li>
                            <li><a id="" data-gotopage="pageActions" menu-button="#btnPageSMS" class="btnPreview ui-link">Send SMS</a></li>
                            <li><a id="" data-gotopage="pageActions" menu-button="#btnPageInteractionHistory" class="btnPreview ui-link">Interaction History</a></li>
                            <li><a id="" data-gotopage="pageActions" menu-button="#btnPageTakeANote" class="btnPreview ui-link">Take A Note</a></li>
                            <li><a id="" data-gotopage="pageActions" menu-button="#btnPageScheduleAnAction" class="btnPreview ui-link">Schedule An Action</a></li>*@
                        </ul>
                    </div>
                </div>
                <div style="clear:both;"></div>
            </div>

            <div data-role="popup" id="panAddCustomerLibrary" data-overlay-theme="a" data-theme="a" data-corners="false">
                <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right btnClosePopUp">Close</a>
                <center><h3 style="margin-left:10px; margin-right:10px;">Please type name of Customer</h3></center>
                <div data-role="content" class="ui-content ui-body-d" style="max-width:350px;">

                    <input type="text" id="txtCustomerSearch" />

                    <div id="divCustomerNoResultsFound" style="display:none;">
                        <br />
                        Oops! We can’t find what you’re looking for. Make sure <br /> your search is correctly spelled.
                    </div>
                    <div style="padding: 8px 15px 0px 15px; max-height:200px; overflow:auto;">
                        <ul id="CustomerlstSearchFound" data-role="listview" class="li-theme-c noBorderList custom-list custom-list-sm">
                            @*<li>
                                <a id="btnSettings">
                                    <h3>Samsung Mega</h3>
                                    <br />
                                    <p>IMEI: 357750050585396</p>
                                    <p>UUID: RF1D666JBZV</p>
                                </a>
                            </li>*@
                        </ul>
                        <br />
                    </div>

                    <div id="btnAddToCustomerLibrary" class="divLibAddOption" style="display:none;">
                        <br />
                        If you’ve correctly spelled your search but can’t find it, <br /> you might want to <a class="custom-link">add a new Customer</a>.
                        <br /><br />
                    </div>
                    <br />
                    <div class="ui-grid-solo">
                        <div class="ui-block-a">
                            <button id="btnSearchCustomerLibrary" data-role="none" class="btn custom-btn-theme-b">
                                Search
                            </button>
                            <br /><br />
                            <a href="#" data-rel="back" id="btnCancelCustomerLibrary" data-role="none" class="btn custom-btn-theme-c">
                                Cancel
                            </a>
                        </div>
                    </div>
                </div>
            </div>

            <div data-role="popup" id="popCompanyOutletDepartment" data-overlay-theme="a" data-theme="a" data-corners="false">
                <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
                <center><h3>Department</h3></center>
                <div data-role="content" class="ui-content ui-body-d jPopupContent">
                    <input type="text" data-role="none" style="display:none;" id="" />
                    Department Name: <br />
                    <input type="text" id="txtCompanyOutletDepartment" />
                    3rd Party ID: <br />
                    <input type="text" id="txtCompanyOutletDepartment3rdPartyID" />
                    <br />
                    <button id="btnOkCompanyOutletDepartment" data-role="none" class="btn custom-btn-theme-b">Ok</button>
                    <br /><br />
                    <a href="#" data-rel="back" data-role="none" class="btn custom-btn-theme-c">Cancel</a>
                </div>
            </div>

            <div data-role="popup" id="popCompanyOutletExSku" data-overlay-theme="a" data-theme="a" data-corners="false">
                <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
                <center><h3>Copy Existing List</h3></center>
                <div data-role="content" class="ui-content ui-body-d jPopupContent">
                    <input type="text" data-role="none" style="display:none;" id="" />
                    Organization Name: <br />
                    <select>
                        <option>Please Select</option>
                    </select>
                    Outlet: <br />
                    <select>
                        <option>Please Select</option>
                    </select>
                    <br /><br />
                    <button id="" data-role="none" class="btn custom-btn-theme-b">Ok</button>
                    <br /><br />
                    <a href="#" data-rel="back" data-role="none" class="btn custom-btn-theme-c">Cancel</a>
                </div>
            </div>

            <div id="popCompanyOutlet" custom-data-role="popup">
                <div class="popup-md">
                    <div class="popup-header">
                        Outlet
                        <a class="popup-close">x</a>
                    </div>

                    <div class="popup-content">
                        @*<input type="text" id="txtPeopleID" style="display:none;" data-role="none" />
                        <input type="text" id="txtContactPersonID" style="display:none;" data-role="none" />*@

                        <div id="divTabCustomerOutlet" class="custom-tab">
                            <ul>
                                <li class="active" custom-tab-content="#tabOutletInfo">Info</li>
                                <li custom-tab-content="#tabOutletDepartment">Department</li>
                                <li custom-tab-content="#tabOutletAuthSku" class="hidetab">Authorized SKU</li>
                            </ul>
                        </div>

                        <div id="tabOutletInfo" custom-tab-role="content" style="display:block;">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <label for="txtCompanyOutlet" class="col-sm-2 control-label">Outlet Name: <span class="required">*</span></label>
                                    <div class="col-sm-10">
                                        <input data-role="none" type="text" id="txtCompanyOutlet" class="form-control unicase-form-control text-input" />
                                    </div>
                                </div>

                                <div class="form-group">
                                    <label for="txtCompanyOutlet3rdPartyID" class="col-sm-2 control-label">3rd Party ID:</label>
                                    <div class="col-sm-10">
                                        <input data-role="none" type="text" id="txtCompanyOutlet3rdPartyID" class="form-control unicase-form-control text-input" />
                                    </div>
                                </div>
                            </div>

                            <br />

                            <div class="panel panel-default">
                                <div class="panel-heading custom-theme-c">Address</div>
                                <div class="panel-body">
                                    <input data-role="none" type="text" id="txtOutletAddressID" style="display:none;" />
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <label for="txtOutletHouseBuildingNo" class="col-sm-2 control-label">House or Building No:</label>
                                            <div class="col-sm-10">
                                                <input data-role="none" type="text" id="txtOutletHouseBuildingNo" class="form-control unicase-form-control text-input" />
                                            </div>
                                        </div>

                                        <div class="form-group">
                                            <label for="txtOutletStreet" class="col-sm-2 control-label">Street:</label>
                                            <div class="col-sm-10">
                                                <input data-role="none" type="text" id="txtOutletStreet" class="form-control unicase-form-control text-input" />
                                            </div>
                                        </div>

                                        <div class="form-group">
                                            <label for="txtOutletStreet" class="col-sm-2 control-label">City Or Town:</label>
                                            <div class="col-sm-10">
                                                <select data-role="none" id="selOutletCity" class="form-control unicase-form-control text-input"></select>
                                            </div>
                                        </div>

                                        <div class="form-group">
                                            <label for="txtOutletStreet" class="col-sm-2 control-label">Country:</label>
                                            <div class="col-sm-10">
                                                <select data-role="none" id="selOutletCountry" class="form-control unicase-form-control text-input"></select>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <br />
                            <label style="font-size:1.5em; font-weight:bold;">Outlet Contact Info</label>
                            <br /><br />
                            <div class="panel panel-default">
                                <div class="panel-heading custom-theme-c">Telephone Number</div>
                                <div class="panel-body">
                                    <div>
                                        <div data-role="fieldcontain" style="text-align:right;">
                                            <button id="btnAddOutletTelephone" data-role="none" class="btn btn-primary btn-less-width">
                                                ADD NEW TELEPHONE
                                            </button>
                                        </div>
                                        <div style="padding: 8px 15px 0px 15px;">
                                            <ul data-role="listview" id="lstOutletTelephone" class="li-theme-c noBorderList custom-list custom-list-sm"></ul>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <br />
                            <div class="panel panel-default">
                                <div class="panel-heading custom-theme-c">Mobile Number</div>
                                <div class="panel-body">
                                    <div>
                                        <div data-role="fieldcontain" style="text-align:right;">
                                            <button id="btnAddOutletMobile" data-role="none" class="btn btn-primary btn-less-width">
                                                ADD NEW MOBILE
                                            </button>
                                        </div>
                                        <div style="padding: 8px 15px 0px 15px;">
                                            <ul data-role="listview" id="lstOutletMobile" class="li-theme-c noBorderList custom-list custom-list-sm"></ul>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <br />
                            <div class="panel panel-default">
                                <div class="panel-heading custom-theme-c">Email Address</div>
                                <div class="panel-body">
                                    <div>
                                        <div data-role="fieldcontain" style="text-align:right;">
                                            <button id="btnAddOutletEmail" data-role="none" class="btn btn-primary btn-less-width">
                                                ADD NEW EMAIL
                                            </button>
                                        </div>
                                        <div style="padding: 8px 15px 0px 15px;">
                                            <ul data-role="listview" id="lstOutletEmail" class="li-theme-c noBorderList custom-list custom-list-sm"></ul>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div id="tabOutletDepartment" custom-tab-role="content" style="display:none;">
                            <div class="row">
                                <div class="col-sm-6">
                                    <label for="selCompanyOutletDeptStatus" class="col-sm-4 control-label">Show status:</label>
                                    <div class="col-sm-6">
                                        <select id="selCompanyOutletDeptStatus" data-role="none" class="form-control unicase-form-control text-input fixed">
                                            <option value="">All</option>
                                            <option value="1" selected>Active</option>
                                            <option value="0">Inactive</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="col-sm-6" style="text-align:right;">
                                    <button id="btnAddOutletDepartment" data-role="none" class="btn btn-primary btn-less-width">
                                        ADD NEW DEPARTMENT
                                    </button>
                                </div>
                            </div>
                            <div style="padding: 8px 15px 0px 15px;">
                                <ul data-role="listview" id="lstOutletDepartment" class="li-theme-c noBorderList custom-list custom-list-sm"></ul>
                            </div>
                        </div>

                        <div id="tabOutletAuthSku" custom-tab-role="content" style="display:none;">
                            <div class="divTable" style="font-size: inherit !important; width: auto !important;">
                                <div class="divTableRow">
                                    <div class="divTableCell txt-middle">Options:</div>
                                    <div class="divTableCell txt-middle">
                                        <div class="custom-radio">
                                            <label class="radio-only">
                                                <input type="radio" data-iconpos="notext" name="radAutoSku" value="all" />
                                            </label>
                                            <div>All SKUs</div>
                                        </div>
                                    </div>
                                    <div class="divTableCell txt-middle">
                                        <div class="custom-radio">
                                            <label class="radio-only">
                                                <input type="radio" data-iconpos="notext" name="radAutoSku" value="some" />
                                            </label>
                                            <div>Some SKUs</div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <br />
                            <div id="divForSomeSkus" style="display:none;">
                                <div style="text-align:right; width:100%;">
                                    <button data-role="none" id="btnOutletCopyExSku" class="btn custom-btn-theme-b btn-sm btn-less-width">Copy Existing List</button>
                                    <button data-role="none" id="btnOutletAddSku" class="btn custom-btn-theme-b btn-sm btn-less-width">Add SKU</button>
                                </div>
                                <br />
                                <table id="tblOutletAuthSku" data-role="table" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
                                    <thead>
                                        <tr class="custom-theme-f">
                                            <th style="width:20px !important;"> SKU # </th>
                                            <th style="width:20px !important;"> Description </th>
                                            <th style="cursor:pointer;"> Barcode # </th>
                                            <th style="cursor:pointer;"> Last Updated </th>
                                            <th style="cursor:pointer;"> Updated By </th>
                                            <th style="cursor:pointer;">  </th>
                                        </tr>
                                    </thead>
                                    <tbody id="tbodyOutletSkus"></tbody>
                                </table>
                            </div>
                        </div>
                    </div>

                    <div class="popup-footer align-right">
                        <button id="" data-role="none" class="btn custom-btn-theme-c btnCircle popup-close">
                            <img alt="Options" class="img-cancel" />
                        </button>
                        <button id="btnOkCompanyOutlet" data-role="none" class="btn custom-btn-theme-b btnCircle">
                            <img alt="Options" class="img-check-white" />
                        </button>
                    </div>
                </div>
            </div>

            <div id="popOutletTelInfo" custom-data-role="popup">
                <div class="popup-md">
                    <div class="popup-header">
                        Telephone Info
                        <a class="popup-close">x</a>
                    </div>

                    <div class="popup-content">
                        <div data-role="fieldcontain" style="display:none;">
                            <input type="text" readonly id="txtOutletTelCountryID" value="e783ed13-aa98-4cc1-918e-d49b05c0167e" />
                            <input type="text" readonly id="txtOutletTelID" />
                            <input type="text" readonly id="txtOutletTelAreaCodeID" />
                        </div>

                        <div class="form-horizontal">
                            <div class="form-group">
                                <label for="btnOutletTelCountryCodeDisable" class="col-sm-2 control-label">Country Code: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <button id="btnOutletTelCountryCodeDisable" data-role="none" class="btn custom-btn-theme-c">
                                        <span id="txtOutletCountryCode">PHL - 63</span>
                                    </button>
                                    <br />
                                    <span id="spanerrorTelCountryCode" style="color:#F00; display:none;">This field is required</span>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="btnOutletTelAreaCode" class="col-sm-2 control-label">Area Code: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <button id="btnOutletTelAreaCode" data-role="none" class="btn custom-btn-theme-c">
                                        <span id="txtOutletAreaCode">PHL - 63</span>
                                    </button>
                                    <br /> <span id="spanerrorOutletAreaCode" style="color:#F00; display:none;">This field is required</span>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="txtOutletTelephoneNum" class="col-sm-2 control-label">Telephone Number: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <input type="text" id="txtOutletTelephoneNum" data-role="none" class="form-control unicase-form-control text-input" />
                                    <br />
                                    <span id="spanerrorOutletTelephoneNum" style="color:#F00; display:none;">This field is required</span>
                                    <span id="spanerrorOutletTelephoneNumValidate" style="color:#F00; display:none;">Enter 7 digit number</span>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="txtOutletTelephoneExt" class="col-sm-2 control-label">Telephone Number: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <input type="text" id="txtOutletTelephoneExt" maxlength="10" data-role="none" class="form-control unicase-form-control text-input" />
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="selOutletTelType" class="col-sm-2 control-label">Contact Type: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <select data-role="none" id="selOutletTelType" class="form-control unicase-form-control text-input">
                                        <option value="">Please Select</option>
                                        <option value="Work Telephone">Work Telephone</option>
                                        <option value="Personal Telephone">Personal Telephone</option>
                                        <option value="Third Party Telephone">Third Party Telephone</option>
                                    </select>
                                    <br />
                                    <span id="spanerrorOutletTelType" style="color:#F00; display:none;">This field is required</span>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="popup-footer align-right">
                        <button data-role="none" class="btn custom-btn-theme-c btnCircle popup-close">
                            <img alt="Options" class="img-cancel" />
                        </button>
                        <button id="btnSaveOutletTelephone" data-role="none" class="btn custom-btn-theme-b btnCircle">
                            <img alt="Options" class="img-check-white" />
                        </button>
                    </div>
                </div>
            </div>

            <div id="popOutletMobileInfo" custom-data-role="popup">
                <div class="popup-md">
                    <div class="popup-header">
                        Mobile Info
                        <a class="popup-close">x</a>
                    </div>

                    <div class="popup-content">
                        <div data-role="fieldcontain" style="display:none;">
                            <input type="text" readonly id="txtOutletMobileCountryID" value="e783ed13-aa98-4cc1-918e-d49b05c0167e" />
                            <input type="text" readonly id="txtOutletMobileID" />
                            <input type="text" readonly id="txtOutletTelCoPrefixID" />
                        </div>

                        <div class="form-horizontal">
                            <div class="form-group">
                                <label for="btnOutletMobileCountryCodeDisable" class="col-sm-2 control-label">Country Code: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <button id="btnOutletMobileCountryCodeDisable" data-role="none" class="btn custom-btn-theme-c">
                                        <span id="txtOutletMobileCountryCode">PHL - 63</span>
                                    </button>
                                    <br /><span id="spanerrorMobileCountryCode" style="color:#F00; display:none;">This field is required</span>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="btnOutletMobileTelCode" class="col-sm-2 control-label">Tel Code: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <button id="btnOutletMobileTelCode" data-role="none" class="btn custom-btn-theme-c">
                                        <span id="txtOutletTelCode">Select</span>
                                    </button>
                                    <br /><span id="spanerrorOutletMobileTelCode" style="color:#F00; display:none;">This field is required</span>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="txtOutletMobileNo" class="col-sm-2 control-label">Mobile Number: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <input data-role="none" type="text" id="txtOutletMobileNo" class="form-control unicase-form-control text-input" />
                                    <br />
                                    <span id="spanerrorOutletMobileNum" style="color:#F00; display:none;">This field is required</span>
                                    <span id="spanerrorOutletMobileNumValidate" style="color:#F00; display:none;">Enter 7 digit number</span>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="selOutletMobileType" class="col-sm-2 control-label">Contact Type: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <select data-role="none" id="selOutletMobileType" class="form-control unicase-form-control text-input">
                                        <option value="">Please Select</option>
                                        <option value="Work Mobile">Work Mobile</option>
                                        <option value="Personal Mobile">Personal Mobile</option>
                                        <option value="Third Party Mobile">Third Party Mobile</option>
                                    </select>
                                    <br />
                                    <span id="spanerrorOutletMobileType" style="color:#F00; display:none;">This field is required</span>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="popup-footer align-right">
                        <div style="text-align:right;">
                            <button id="btnCancelOutletMobile" data-role="none" class="btn custom-btn-theme-c btnCircle">
                                <img alt="Options" class="img-cancel" />
                            </button>
                            <button id="btnSaveOutletMobile" data-role="none" class="btn custom-btn-theme-b btnCircle">
                                <img alt="Options" class="img-check-white" />
                            </button>
                        </div>
                    </div>
                </div>
            </div>

            <div id="popOutletEmailInfo" custom-data-role="popup">
                <div class="popup-md">
                    <div class="popup-header">
                        Email Info
                        <a class="popup-close">x</a>
                    </div>

                    <div class="popup-content">
                        <div data-role="fieldcontain" style="display:none;">
                            <input type="text" readonly id="txtOutletEmailID" />
                        </div>

                        <div class="form-horizontal">
                            <div class="form-group">
                                <label for="txtOutletEmailAdd" class="col-sm-2 control-label">Email Address: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <input data-role="none" type="text" id="txtOutletEmailAdd" class="form-control unicase-form-control text-input" />
                                    <br />
                                    <span id="spanerrorOutletEmailAddress" style="color:#F00; display:none;">This field is required</span>
                                    <span id="spanerrorOutletEmailAddressValidate" style="color:#F00; display:none;">Enter 7 digit number</span>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="selOutletEmailType" class="col-sm-2 control-label">Contact Type: <span class="required">*</span></label>
                                <div class="col-sm-10">
                                    <select data-role="none" id="selOutletEmailType" class="form-control unicase-form-control text-input">
                                        <option value="">Please Select</option>
                                        <option value="Work Email">Work Email</option>
                                        <option value="Personal Email">Personal Email</option>
                                        <option value="Third Party Email">Third Party Email</option>
                                    </select>
                                    <br />
                                    <span id="spanerrorOutletEmailType" style="color:#F00; display:none;">This field is required</span>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="popup-footer align-right">
                        <button data-role="none" class="btn custom-btn-theme-c btnCircle popup-close">
                            <img alt="Options" class="img-cancel" />
                        </button>
                        <button id="btnSaveOutletEmailAddress" data-role="none" class="btn custom-btn-theme-b btnCircle">
                            <img alt="Options" class="img-check-white" />
                        </button>
                    </div>
                </div>
            </div>

            <div data-role="popup" id="popConfirmCustomerDeactivation" data-overlay-theme="a" data-theme="a" data-corners="false">
                <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
                <center><h3>Confirm Dialog</h3></center>
                <div data-role="content" class="ui-content ui-body-d jPopupContent">
                    Please confirm by entering you password.
                    <br /><br />

                    <input type="password" data-role="none" class="form-control" id="txtCustomerDeactivatePassword" placeholder="Enter your password here" />

                    <br /><br />
                    <button id="btnDeactivateCustomerNow" data-role="none" class="btn custom-btn-theme-b">Confirm Now</button>
                    <br /><br />
                    <a href="#" data-rel="back" data-role="none" class="btn custom-btn-theme-c">Cancel</a>
                </div>
            </div>
        </div>
    </div>    

    <div id="divLinkInvalid" subpage="pageLink" style="display:none;">
        <br />
        <span class="glyphicon glyphicon-remove-circle" style="font-size:30px; color:#f44336;"></span>
        <span id="lblErrorMsg" style="font-size:30px; color:#b71509;">Sorry! This link is no longer available.</span>
    </div>

    <div id="divSavedSuccess" subpage="pageLink" style="display:none;">
        <h2 class="lblSuccessMsg">
            
        </h2>
        <h4>
            Your account needs to be verified <span class="foremail" style="display:none;">by you and</span> the online platform admin to make sure that your account really belongs to you (and not a bot or someone pretending to be you.)
            <br /><br />
            <span class="foremail" style="display:none;">
                On your side, please check your email and verify your account by clicking on the activation link we’ve emailed you. We’ve sent the verification link to your registered email at: <b id="lblSentTo"></b>
                <br /><br />
            </span>
            The platform administrator will also check your registration and approve it if everything looks fine.
        </h4>
    </div>
</div>