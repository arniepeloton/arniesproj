@Code
    ViewData("Title") = "Employment Confirmation"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head
    <link rel="stylesheet" href="/Content/flatui-css-custom.css" />
    <script>
        require([GetPathFormat('/Scripts/sources/CRMDashboard/EmploymentConfirmation.js?crc=')]);
    </script>
End Section

<div data-role="page" style="display:block;">
    <div class="custom-theme-a position-fixed">

        <div style="padding: 20px; text-align: center;">
            MESSAGE DETAILS
        </div>       
        
    </div>

    <br /><br /><br />
    <div data-role="content">
        <input type="text" data-role="none" style="display:none;" id="txtPeopleMsgT1ID" />
        <h3 id="lblPeopleMsgT1Subject"></h3>

        <br />
        <br />

        <div class="ui-grid-a">
            <div class="ui-block-a" style="width: 15%">
                FROM:
            </div>
            <div class="ui-block-b" id="lblPeopleMsgT1From" style="width: 85%; font-size: 20px;">
                    
            </div>
        </div>

        <br />

        <div class="ui-grid-a">
            <div class="ui-block-a" style="width: 15%">
                DATE and TIME:
            </div>
            <div class="ui-block-b" id="lblPeopleMsgT1DateTime" style="width: 85%; font-size: 20px;">
                    
            </div>
        </div>

        <br /> <hr /> <br />

        <div style="font-size: 18px;" id="lblPeopleMsgT1"></div>

        <br />
        <br />
        <br />
        <br />

        <button data-role="none" id="btnPeopleMsgT1Confirm" class="btn custom-btn-theme-b btn-large">
            Confirm Employment
            <br />
            <span style="font-size:14px; font-weight:normal;">
                Click on this button to confirm that you are an employee of this company.
            </span>
        </button>

        <div style="display:none;">
            <br /><br />

            <button data-role="none" id="btnPeopleMsgT1Deny" class="btn custom-btn-theme-d btn-large">
                Decline Employment Invitation
                <br />
                <span style="font-size:14px; font-weight:normal;">
                    Click on this button if you are not an employee of this company.
                </span>
            </button>
        </div>        

        <br /><br />

        <button data-role="none" id="btnPeopleMsgT1Cancel" class="btn custom-btn-theme-c btn-large">
            Cancel
        </button>
    </div>
</div>
