@Code
    ViewData("Title") = "Download Database"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code

@Section head

    <script>
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
                require([GetPathFormat('/Scripts/sources/SignIn/DownloadDatabase.js?crc=' + assetversion)]);
            })
        })
    </script>

<style type="text/css">
    .ui-page-active, body {
        outline: none;
        background-color: #f8f9fa !important;
    }
</style>
    
End Section

<div data-role="page" class="container">
    <div data-role="content">
        <div class="mb-4 mt-4">
            <table align="center">
                <tbody>
                    @*<tr>
                        <td><h1 class="display-5 mb-4 mt-4">@ViewData("Title")</h1></td>
                    </tr>*@
                    <tr>
                        <td><h1 style="text-align:center; color:Red">Notice</h1></td>
                    </tr>
                    <tr>
                        <td>
                            <p>
                            The company information contained in this device is confidential unless they are deemed for public consumption or viewing.  <br /> <br />
                            Also note that your device may be tracked by your company via GPS as a way to monitor organizational performance.
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <br /> <br />
                            <div class="bg-white border rounded p-3 mb-3">
                                <div class="form-check">
                                    <input data-role="none" class="form-check-input" type="radio" name="radio-choice" id="chkAgree" value="radio-choice-1" checked="checked" />
                                    <label for="chkAgree" class="form-check-label" style="width:100%;">I Agree</label>
                                </div>
                            </div>

                            <div class="bg-white border rounded p-3 mb-3">
                                <div class="form-check">
                                    <input data-role="none" class="form-check-input" type="radio" name="radio-choice" id="chkCancel" value="radio-choice-2" />
                                    <label for="chkCancel" class="form-check-label" style="width:100%;">Cancel</label>
                                </div>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="text-center">
                            <button id="btnDoneNotice" data-role="none" class="btn btn-success btn-lg">Done</button>
                        </td>
                    </tr>                    
                </tbody>                
            </table>    
        </div>
    </div>        
</div>