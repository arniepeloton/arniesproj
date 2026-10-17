@Code
    ViewData("Title") = "Device MDM Policy"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
@Section head
    <script>
        require([GetPathFormat('/Scripts/sources/DeviceMDMConfig/DeviceMDMConfig.js?crc=')]);
    </script>
    <style>
        .form-validation-error
        {
            border: 1px solid Red;
            box-shadow: 0 0 3px Red;
        }
        
        
        .form-validation-error:active
        {
            box-shadow: 0px 1px 3px rgba(0, 0, 0, 0.2) inset, 0px 0px 3px rgba(0, 0, 0, 0.2);
        }
        .smallfontRed
        {
            background-color: Red;
            color: White;
            margin-left: 8px;
            font-size: small;
        }
    </style>
End Section
<div id="pageMain" data-role="page">
    <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>
    <div data-role="content" data-dom-cache="true">
        <div>
            <input id="txtSearch" type="search" value="" placeholder="Search" /></div>
        <div>
            @*  <div data-role="fieldcontain">
            <label><input type="checkbox" id="chkRoleIsActive" /> Show InActive</label>
        </div>*@
            <ul id="lstPolicy" data-role="listview" data-inset="true" style="min-width: 210px;">
                <li data-role="list-divider"></li>
            </ul>
        </div>
        <a id="btnAddNewPolicy" data-role="button" data-theme="c">Add New Policy</a>
    </div>
</div>
<div id="pageConfigurationItems" data-role="page">
    <div data-role="header">
        <h1 id="headerPolicyName">
            Policy Settings</h1>
    </div>
    <div data-role="content" data-dom-cache="true">
        <div data-role="fieldcontain" style="display: none;">
            <input type="text" readonly id="txtSecurityRoleID" />
        </div>
        <div data-role="fieldcontain">
            <label for="txtRoleCode">
                Policy Name: <span style="color: #FF0000;">*</span></label>
            <fieldset data-role="controlgroup">
                <div>
                    <input type="text" id="txtPolicyName" maxlength="25" /></div>
                <span class="smallfontRed" id="spanPolicyName" style="display: none;">This field is
                    required.</span>
            </fieldset>
        </div>
        <div data-role="fieldcontain">
            <label for="txtRole">
                Policy Description: <span style="color: #FF0000;">*</span></label>
            <fieldset data-role="controlgroup">
                <div>
                    <input type="text" id="txtPolicyDescription" /></div>
                <span class="smallfontRed" id="spanPolicyDescription" style="display: none;">This field
                    is required.</span>
            </fieldset>
        </div>
        <div data-role="fieldcontain">
            <label style="display: none;">
                <input style="display: none;" type="checkbox" id="chkRoleIsActive" />
                Active</label>
        </div>
        <div data-role="collapsible" data-collapsed="false" data-theme="c" data-content-theme="d"
            data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">
            <h2>
                <label id="lbltitlePolicy">
                    Password Policy
                </label>
            </h2>
            <div data-role="fieldcontain" class="ui-radio">
                <fieldset data-role="controlgroup">
                    <legend>Auto Lock Screen? </legend>
                    <input data-role="button" type="radio" name="rdALS" id="rcAutoLockScreenYes" />                    
                    <label for="rcAutoLockScreenYes">
                        Yes</label>
                    <input data-role="button" type="radio" name="rdALS" id="rcAutoLockScreenNo" />
                    <label for="rcAutoLockScreenNo">
                        No</label>
                </fieldset>
            </div>
            <div data-role="fieldcontain">
                <fieldset data-role="controlgroup">
                    <legend>Auto-Lock after Inactivity of?</legend>
                    <select id="selAutoLockAfterInactivity ">
                        <option value="0">Select in Minute(s) </option>
                        <option value="1">1 minute </option>
                        <option value="5">5 minutes </option>
                        <option value="10">10 minutes </option>
                        <option value="15">15 minutes </option>
                        <option value="20">20 minutes </option>
                        <option value="30">30 minutes </option>
                        <option value="60">60 minutes (1 hour) </option>
                        <option value="180">180 minutes (3 hours)</option>
                    </select>
                </fieldset>
            </div>
            <div data-role="fieldcontain" class="ui-radio">
                <fieldset data-role="controlgroup">
                    <legend>Require Password? </legend>
                    <input type="radio" name="rdRP" id="rcRequirePasswordYes" />
                    <label for="rcRequirePasswordYes">
                        Yes</label>
                    <input type="radio" name="rdRP" id="rcRequirePasswordNo" />
                    <label for="rcRequirePasswordNo">
                        No</label>
                </fieldset>
            </div>
            <div data-role="fieldcontain">
                <fieldset data-role="controlgroup">
                    <legend>Password Strength?</legend>
                    <select id="selPasswordStrength">
                        <option value="0">Select Password Strength </option>
                        <option value="1">Weak </option>
                        <option value="2">Medium </option>
                        <option value="3">Strict </option>
                    </select>
                </fieldset>
            </div>
            <div data-role="fieldcontain">
                <fieldset data-role="controlgroup">
                    <legend>Password Reset Every?</legend>
                    <select id="selPasswordResetEvery">
                        <option value="0">Select in Day(s) </option>
                        <option value="1">1 day</option>
                        <option value="2">2 days</option>
                        <option value="7">7 days</option>
                        <option value="14">14 days</option>
                        <option value="30">30 days</option>
                        <option value="60">60 days</option>
                    </select>
                </fieldset>
            </div>
        </div>
        <div data-role="collapsible" data-collapsed="false" data-theme="c" data-content-theme="d"
            data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">
            <h2>
                <label id="lbltitlePolicy">
                    Encryption Policy
                </label>
            </h2>
            <div data-role="fieldcontain" class="ui-radio">
                <fieldset data-role="controlgroup">
                    <legend>Encrypt Data Storage?<br />
                        <label style="font-size: 0.7em;">
                            ( works for Honeycomb OS or later)
                        </label>
                    </legend>
                    <input type="radio" name="rdEIM" id="rcEncryptDataStorageYes" />
                    <label for="rcEncryptDataStorageYes">
                        Yes</label>
                    <input type="radio" name="rdEIM" id="rcEncryptDataStorageNo" />
                    <label for="rcEncryptDataStorageNo">
                        No</label>
                </fieldset>
            </div>
        </div>
        <div data-role="collapsible" data-collapsed="false" data-theme="c" data-content-theme="d"
            data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">
            <h2>
                <label id="lbltitlePolicy">
                    Data Storage WipeOut Policy
                </label>
            </h2>
            <div data-role="fieldcontain">
                <fieldset data-role="controlgroup">
                    <legend>Wipe Out Data After How Many Attempts?<br />
                        <label style="font-size: 0.7em;">
                            ( works for Honeycomb OS or later)
                        </label>
                    </legend>
                    <select id="selWipeOutData">
                        <option value="0">Select Number of Attempts </option>
                        <option value="3">3 tries </option>
                        <option value="5">5 tries</option>
                        <option value="8">8 tries</option>
                        <option value="12">12 tries</option>
                        <option value="20">20 tries</option>
                    </select>
                </fieldset>
            </div>
        </div>
        <div data-role="collapsible" data-collapsed="false" data-theme="c" data-content-theme="d"
            data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">
            <h2>
                <label id="lbltitlePolicy">
                    Certificate Expiration Policy
                </label>
            </h2>
            <div data-role="fieldcontain">
                <fieldset data-role="controlgroup">
                    <legend>No. of Days A Certificate Will Expire if not Updated<br />
                    </legend>
                    <select id="selNoDaysCertificateWillExpire">
                        <option value="0">Select Number of Days </option>
                        <option value="1">1 day </option>
                        <option value="2">2 days</option>
                        <option value="3">3 days</option>
                        <option value="7">7 days</option>
                    </select>
                </fieldset>
            </div>
        </div>
        <div data-role="collapsible" data-collapsed="false" data-theme="c" data-content-theme="d"
            data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">
            <h2>
                <label id="lbltitlePolicy">
                    @*White Listed Applications*@
                    Application Control
                </label>
            </h2>
            <div data-role="fieldcontain">
                <fieldset data-role="content">
                    <label for="rcNoRestriction">No Restriction </label>
                    <input type="radio" name="ApplicationControl" id="rcNoRestriction" class="custom" checked="checked">

                    <label for="rcWhiteList">White Listed Application</label>
                    <input type="radio" name="ApplicationControl" id="rcWhiteList" class="custom">

                    <label for="rcBlackList">Black Listed Application Control</label>
                    <input type="radio" name="ApplicationControl" id="rcBlackList" class="custom">

                    <br />
                    <div>
                    <ul id="lstWhiteListedApps" data-role="listview">
                    </ul>
                    </div>
                    <br /> <br />
                    <a data-role="button" data-icon="add" id="btnWhiteListApps" data-theme="c">Add</a>
                </fieldset>
            </div>
        </div>
        <a data-role="button" id="btnConfigUpdate" data-theme="c">Save</a> <a data-role="button"
            id="btnPolicyBack" data-rel="back" data-theme="c">Back</a> @*  <a data-role="button" id="btnView" data-theme="c">View Existing Policies</a>*@
    </div>
</div>
<div id="pageWhiteList" data-role="page">
    <div data-role="header">
        <h1 id="hWLA">
            Add White-Listed Applications</h1>
    </div>
    <div data-role="content">
        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
                <legend>Application Title <span style="color: #FF0000;">*</span></legend>
                <input type="text" id="txtApplicationTitle" data-role="field-contain" required>
                <span class="smallfontRed" id="spanApplicationTitle" style="display: none;">This field
                    is required.</span>
            </fieldset>
        </div>
        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
                <legend>APK Name <span style="color: #FF0000;">*</span> </legend>
                <input type="text" id="txtApplicationName" data-role="field-contain">
                <span class="smallfontRed" id="spanApplicationName" style="display: none;">This field
                    is required.</span>
            </fieldset>
        </div>
        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
                <legend>Directory </legend>
                <input type="text" id="txtKeywords" data-role="field-contain">
            </fieldset>
        </div>
        <a data-role="button" id="btnAppsWhiteListSave" data-theme="c">Save</a> <a data-role="button"
            id="btnAppsWhiteListBack" data-rel="back" data-theme="c">Back</a>
    </div>
</div>
