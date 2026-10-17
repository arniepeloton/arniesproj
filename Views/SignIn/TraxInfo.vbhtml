@Code
    ViewData("Title") = "Mobiz Trax"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/SignIn/Trax.js?crc=')]);
    </script>
    
End Section

<div data-role="page">
    <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>

    <div data-role="content">
        <div>        
            <table align="center">
                <tbody>
                    <tr>
                        <td>
                            <p style="font-weight:bold; font-size: large; text-align:center">
                                This device is being tracked via GPS as part of the agreement with your organization.
                            </p>
                        </td>
                    </tr>

                    <tr>
                        <td>
                            <p style="font-weight:bold">
                                GPS data collected by this device will be sent to Mobiz Cloud via Internet and SMS (optional). <br /> <br /> <br /> 
                                <span style="color:Red"> SMS sending may cause additional charges to your account.  </span> <br /> <br /> <br /> 
                                To prevent unnceessary SMS charges, please make sure that: <br /> <br /> 
                                1.	You have inserted a SIM card with unlimited SMS service <br /> <br /> 
                                2.	You have selected the proper telco network of the SIM card inserted to this device:
                            </p>
                        </td>
                    </tr>

                    <tr>
                        <td>
                             <fieldset data-role="controlgroup">                                
                                <input type="radio" name="radio-choice-v-2" id="btnGLOBE" disabled="disabled">
                                <label for="btnGLOBE">GLOBE</label>
                                <input type="radio" name="radio-choice-v-2" id="btnTouchMobile" disabled="disabled">
                                <label for="btnTouchMobile">Touch Mobile</label>
                                <input type="radio" name="radio-choice-v-2" id="btnSMART" disabled="disabled">
                                <label for="btnSMART">SMART</label>
                                <input type="radio" name="radio-choice-v-2" id="btnTalkAndText" disabled="disabled">
                                <label for="btnTalkAndText">Talk and Text</label>
                                <input type="radio" name="radio-choice-v-2" id="btnSUN" >
                                <label for="btnSUN">SUN</label>
                                <input type="radio" name="radio-choice-v-2" id="btnNONE">
                                <label for="btnNONE">Do Not Send SMS Data</label>
                            </fieldset>
                            <p>
                                @*<label> <input type="radio" name="checkbox-0 " id="btnSUN">SUN</label>*@
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <p>
                                <a id="btnDoneTrax" data-role="button" href="#">OK</a>
                            </p>
                        </td>
                    </tr>
                </tbody>                
            </table>    
        </div>
    </div>
        
</div>
