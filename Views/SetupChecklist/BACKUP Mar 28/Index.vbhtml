@Code
    ViewData("Title") = "Setup Checklist"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/SetupChecklist/SetupChecklist.js?crc=')]);
    </script>
    
End Section

<div id="pageSetUpCheckList" data-role="page">

    <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>

    <div data-role="content">
    
    <div class="ui-grid-a" >
        <div class="ui-block-a" style="width: 60%;">
            <div id="btnSetupOrganization" style="margin:0px;" data-role="button" data-theme="c" data-corners="false">
                <div style="text-align:left; margin-top:28px;"><span>Organization</span></div>
                <p style="text-align:left; white-space: normal; font-size:smaller; font-weight:lighter;">Click here to add more information about your organization. This information may be published to an online business directory, used for system-generating General Information Sheet (GIS) for the SEC or DTI, and/or other value-added services in the future. Your consent must be given before Mobiz publishes any of these information to the public.</p>
            </div>
        </div>
        <div class="ui-block-b" style="width: 40%;">
            <div id="btnSetupOrganizationVal" style="margin:0px;" data-role="button" data-theme="c" data-corners="false">
                <div style="text-align:center;">
                    <div><img id="imgOrg" src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                    <div><label id="lblOrgLastUpdated">(08/20/2014)</label></div>
                </div>
            </div>
        </div>
    </div>
    <div class="ui-grid-a" >
        <div class="ui-block-a" style="width: 60%;">
            <div id="btnSetupOrganizationFOWST" style="margin:0px;" data-role="button" data-theme="c" data-corners="false">
                <div style="text-align:left; margin-top:28px;"><span>FOWST</span></div>
                <p style="text-align:left; white-space: normal; font-size:smaller; font-weight:lighter;">At least one (1) record of a FOWST (Factory, Office, Warehouse, Store or Truck) is needed.</p>
            </div>
        </div>
        <div class="ui-block-b" style="width: 40%;">
            <div id="btnSetupOrganizationFOWSTVal" style="margin:0px;" data-role="button" data-theme="c" data-corners="false">
                <div style="text-align:center;">
                    <div><img id="imgFOWST" src="/Content/images/cross.png" style="width: 30px; height: 30px;"></div>
                    <div><label id="lblFOWSTCount">0</label></div>
                    <div><label id="lblFOWSTLastUpdated">(08/20/2014)</label></div>
                </div>
            </div>
        </div>
    </div>
    <div class="ui-grid-a" >
        <div class="ui-block-a" style="width: 60%;">
            <div id="btnSetupDevice" style="margin:0px;" data-role="button" data-theme="c" data-corners="false">
                <div style="text-align:left; margin-top:28px;"><span>Authorized Device</span></div>
                <p style="text-align:left; white-space: normal; font-size:smaller; font-weight:lighter;">At least one (1) device is needed to be registered for use in your company.</p>
            </div>
        </div>
        <div class="ui-block-b" style="width: 40%;">
            <div id="btnSetupDeviceVal" style="margin:0px;" data-role="button" data-theme="c" data-corners="false">
                <div style="text-align:center;">
                    <div><img id="imgDevice" src="/Content/images/cross.png" style="width: 30px; height: 30px;"></div>
                    <div><label id="lblDeviceCount">0</label></div>
                    <div><label id="lblDeviceLastUpdated">(08/20/2014)</label></div>
                </div>
            </div>
        </div>
    </div>
    <div class="ui-grid-a" >
        <div class="ui-block-a" style="width: 60%;">
            <div id="btnSetupReceiptSettings" style="margin:0px;" data-role="button" data-theme="c" data-corners="false">
                <div style="text-align:left; margin-top:28px;"><span>Receipt Settings</span></div>
                <p style="text-align:left; white-space: normal; font-size:smaller; font-weight:lighter;">Fill up the Receipt Settings Info for each FOWST.</p>
            </div>
        </div>
        <div class="ui-block-b" style="width: 40%;">
            <div id="btnSetupReceiptSettingsVal" style="margin:0px;" data-role="button" data-theme="c" data-corners="false">
                <div style="text-align:center;">
                    <div><img id="imgReceiptSettings" src="/Content/images/cross.png" style="width: 30px; height: 30px;"></div>
                    <div><label id="lblReceiptSettingsCount">0</label></div>
                    <div><label id="lblReceiptSettingsLastUpdated">(08/20/2014)</label></div>
                </div>
            </div>
        </div>
    </div>
    <div class="ui-grid-a" >
        <div class="ui-block-a" style="width: 60%;">
            <div id="btnSetupSecurity" style="margin:0px;" data-role="button" data-theme="c" data-corners="false">
                <div style="text-align:left; margin-top:28px;"><span>Security Roles</span></div>
                <p style="text-align:left; white-space: normal; font-size:smaller; font-weight:lighter;">Add  or edit existing roles in your organization for fine-tuning staff's access rights to Mobiz apps.</p>
            </div>
        </div>
        <div class="ui-block-b" style="width: 40%;">
            <div id="btnSetupSecurityVal" style="margin:0px;" data-role="button" data-theme="c" data-corners="false">
                <div style="text-align:center;">
                    <div><img id="imgSecurityAccess" src="/Content/images/cross.png" style="width: 30px; height: 30px;"></div>
                    <div><label id="lblSecurityAccessCount">0</label></div>
                    <div><label id="lblSecurityAccessLastUpdated">(08/20/2014)</label></div>
                </div>
            </div>
        </div>
    </div>
    <div class="ui-grid-a" >
        <div class="ui-block-a" style="width: 60%;">
            <div id="btnSetupEmployee" style="margin:0px;" data-role="button" data-theme="c" data-corners="false">
                <div style="text-align:left; margin-top:28px;"><span>Employee</span></div>
                <p style="text-align:left; white-space: normal; font-size:smaller; font-weight:lighter;">Each employee must be assigned his or her own user name/password and Security Role (cashier, manager, etc.) before he/she can use Mobiz apps.</p>
            </div>
        </div>
        <div class="ui-block-b" style="width: 40%;">
            <div id="btnSetupEmployeeVal" style="margin:0px;" data-role="button" data-theme="c" data-corners="false">
                <div style="text-align:center;">
                    <div><img id="imgEmployeeAccess" src="/Content/images/cross.png" style="width: 30px; height: 30px;"></div>
                    <div><label id="lblEmployeeCount">0</label></div>
                    <div><label id="lblEmployeeLastUpdated">(08/20/2014)</label></div>
                </div>
            </div>
        </div>
    </div>
    <div class="ui-grid-a" >
        <div class="ui-block-a" style="width: 60%;">
            <div id="btnSetupProducts" style="margin:0px;" data-role="button" data-theme="c" data-corners="false">
                <div style="text-align:left; margin-top:28px;"><span>Products</span></div>
                <p style="text-align:left; white-space: normal; font-size:smaller; font-weight:lighter;">At least one (1) product is needed. You may add your products via Excel upload or you may choose from the publicly available product database of Mobiz.</p>
            </div>
        </div>
        <div class="ui-block-b" style="width: 40%;">
            <div id="btnSetupProductsVal" style="margin:0px;" data-role="button" data-theme="c" data-corners="false">
                <div style="text-align:center;">
                    <div><img id="imgProduct" src="/Content/images/cross.png" style="width: 30px; height: 30px;"></div>
                    <div><label id="lblProductCount">0</label></div>
                    <div><label id="lblProductLastUpdated">(08/20/2014)</label></div>
                </div>
            </div>
        </div>
    </div>
    <div class="ui-grid-a" >
        <div class="ui-block-a" style="width: 60%;">
            <div id="btnSetupCustomer" style="margin:0px;" data-role="button" data-theme="c" data-corners="false">
                <div style="text-align:left; margin-top:28px;"><span>Customer/Client</span></div>
                <p style="text-align:left; white-space: normal; font-size:smaller; font-weight:lighter;">Add known customer/client information here so that they can be identified during sales transactions.</p>
            </div>
        </div>
        <div class="ui-block-b" style="width: 40%;">
            <div id="btnSetupCustomerVal" style="margin:0px;" data-role="button" data-theme="c" data-corners="false">
                <div style="text-align:center;">
                    <div><img id="imgCustomer" src="/Content/images/cross.png" style="width: 30px; height: 30px;"></div>
                    <div><label id="lblCustomerCount">0</label></div>
                    <div><label id="lblCustomerLastUpdated">(08/20/2014)</label></div>
                </div>
            </div>
        </div>
    </div>
    <div class="ui-grid-a" >
        <div class="ui-block-a" style="width: 60%;">
            <div id="btnSetupSupplier" style="margin:0px;" data-role="button" data-theme="c" data-corners="false">
                <div style="text-align:left; margin-top:28px;"><span>Supplier/Vendor</span></div>
                <p style="text-align:left; white-space: normal; font-size:smaller; font-weight:lighter;">Add known supplier/vendor information here so that they can be identified during inventory transactions.</p>
            </div>
        </div>
        <div class="ui-block-b" style="width: 40%;">
            <div id="btnSetupSupplierVal" style="margin:0px;" data-role="button" data-theme="c" data-corners="false">
                <div style="text-align:center;">
                    <div><img id="imgSupplier" src="/Content/images/cross.png" style="width: 30px; height: 30px;"></div>
                    <div><label id="lblSupplierCount">0</label></div>
                    <div><label id="lblSupplierLastUpdated">(08/20/2014)</label></div>
                </div>
            </div>
        </div>
    </div>

    <div>
        <br />
        <br />
        <div style="text-align: center">
            <a id="btnSetupChecklistDone" data-role="button"><h3>Done</h3></a>
        </div>
    </div>

</div>

