<script>
    require([GetPathFormat('/Scripts/sources/PIMUpload/PIMUpload.js?v=' + assetversion),
                 GetPathFormat('/Scripts3/Browser/LibraryPIMFeature.js?crc=')
            ]);
</script>

<div id="pagePIMUpload" style="display:block;">
    <h1 class="display-6 mb-4">Import Products By Excel File</h1>

    <div data-role="content">
      
        <div id="divPIMUploadFailed" style="display:none;">
            <p>Something went wrong while trying to save your data. Upon validating, the system has detected either some required fields that were left blank.
             Please <a id="lnkPIMUploadTemplateFailedDL" style="cursor:pointer; font-size:16px;">download your file</a> to see the problem rows highlighted in red.</p>
            <p>Once you have corrected your file, please upload it back. Thank you.</p>
        </div>

        <div id="divFailedMessagesMain" style="display:none;">
        </div>
        <form @*action="PIMUpload/Index"*@ id="formupload" enctype="multipart/form-data" method="post">

            <div style="display:none;"><input type="text" data-role="none" class="form-control" id="OrgID" name="OrgID"></div>
            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="objFile">Upload Product Excel File:</label>
                <div class="col-sm-10">
                    <input type="file" data-role="none" id="objFile" class="form-control" accept=".xls,.xlsx" name="objFile">
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-4 mb-3" for="selParseProductName">Parse product name for packing size quantity:</label>
                <div class="col-sm-8">
                    <div class="form-check form-switch mb-3" style="font-size:20px;">
                        <input class="form-check-input" data-role="none" type="checkbox" name="selParseProductName" id="selParseProductName">
                    </div>
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-4 mb-3" for="selDoNotOverwriteBomLandedCost">Do not overwrite Factory Cost, Freight Cost, and Landed Cost for products with computed cost for BoM, Direct Labor and/or Direct Overhead:</label>
                <div class="col-sm-8">
                    <div class="form-check form-switch mb-3" style="font-size:20px;">
                        <input class="form-check-input" data-role="none" type="checkbox" name="selDoNotOverwriteBomLandedCost" id="selDoNotOverwriteBomLandedCost">
                    </div>
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="dtBegBalanceDateTime">Beginning Balance Date and Time:</label>
                <div class="col-sm-10">
                    <input type="datetime-local" data-role="none" id="dtBegBalanceDateTime" class="form-control">
                </div>
            </div>

            <div class="text-center mb-3">
                <button id="btnPIMUploadBackToHome" data-role="none" class="btn btn-secondary btn-lg">Cancel</button>
                <button type="submit" data-role="none" class="btn btn-success btn-lg">Upload File</button>
            </div>
        </form>

        <div class="row mb-3 mt-5">
            <div class="col-sm-4">
                Download the PIM template to see an example of the format required.
            </div>
            <div class="col-sm-4">
                <a data-role="none" class="btn btn-outline-secondary btn-block mb-3" id="lnkDownloadGenTemplate">Download PIM Template</a>                
            </div>
            <div class="col-sm-4">
                <a data-role="none" class="btn btn-outline-secondary btn-block" id="butDeleteColumnMapping">Delete Column Mappings</a>
            </div>
        </div>
    </div>
</div>

<div id="pagePIMUploadAnalyze" style="display:none;">
    <h1 class="display-6 mb-4">Analyze Uploaded File</h1>

    <div data-role="content">
        <div class="mb-3">
            We have detected a new data column(s) on the Excel file you are attempting to upload. Please set the equivalent Mobiz data field(s) for these columns.
            <input type="text" id="txtPIMUploadTemplateName" data-role="none" class="form-control" style="display:none;" />
        </div>

        <div id="divPIMUploadAnalyzeFailed" class="mb-3" style="display:none;">
            <p>Something went wrong while trying to save your data. Upon validating, the system has detected either some required fields that were left blank.
             Please <a id="lnkPIMUploadAnalyzeTemplateFailedDL" style="cursor:pointer; font-size:16px;">download your file</a> to see the problem rows highlighted in red.</p>
            <p>Once you have corrected your file, please upload it back. Thank you.</p>
        </div>

        <div id="divFailedMessages" class="mb-3" style="display:none;"></div>

        <div id="divPIMUploadAnalyze" class="mb-3"></div>
        
        <div class="text-center mb-3 mt-5">
            <button id="btnOrgPIMUploadCancel" data-role="none" class="btn btn-secondary btn-lg">Cancel</button>
            <button id="btnOrgPIMUpload" data-role="none" class="btn btn-success btn-lg">Upload</button>
        </div>
    </div>
</div>

<div id="pagePIMExcelColDataMatching" style="display:none;">
    <h1 class="display-6 mb-4">Data Matching</h1>

    <div data-role="content">

        <div id="divPIMUploadColDataMatch" class="mb-3"></div>

        <div class="text-center mb-3">
            <button id="btnOrgPIMUploadMatchDataCancel" data-role="none" class="btn btn-secondary btn-lg">Cancel</button>
            <button id="btnOrgPIMUploadMatchDataOk" data-role="none" class="btn btn-success btn-lg">Ok</button>
        </div>
    </div>
</div>

<div id="divPIMUploadPromptEmail" data-role="dialog" style="display:none;">
	<div data-role="header" data-theme="d">
		<h2>Email Address</h2>
	</div>

	<div data-role="content" data-theme="c">

        <fieldset data-role="controlgroup">
            <div><label for="txtPIMUploadEmailAddress">Please enter your email address:</label></div>
            <div><label for="txtPIMUploadEmailAddress" style="font-size:small;">Mobiz technical team will reply to this email address(es) once the column fields was resolved.</label></div>
            <div><label for="txtPIMUploadEmailAddress" style="font-size:small;">(Please separate multiple email addresses with a comma. Ex. sample1@email.com, sample2@email.com)</label></div>
            <div><input type="text" id="txtPIMUploadEmailAddress" /></div>
        </fieldset>

        <br /><br />
		<a id="btnPIMUploadEmailOK" data-role="button">Ok</a>       
		<a id="btnPIMUploadEmailCancel" data-role="button">Cancel</a>    

	</div>

</div>

<div id="divPIMUploadPromptPriceLevelDiscount" data-role="dialog" style="display:none;">
    <div data-role="header" data-theme="d">
        <h2>Volume Discount Scheme</h2>
    </div>

    <div data-role="content" data-theme="c">

        <fieldset data-role="controlgroup">
            <div>
                <legend>Oops, we found that you have too many volume discount schemes.  Mobiz can accomodate only one volume discount scheme per SKU.</legend>
                <legend>Which volume discount scheme would you like to apply to all?</legend>
                <select id="selPIMUploadPriceLevelDiscount">
                    <option value="">Please Select</option>
                </select>
            </div>
        </fieldset>

        <br /><br />
        <a id="btnPIMUploadVolumePriceSchemeOK" data-role="button">Ok</a>
        <a id="btnPIMUploadVolumePriceSchemeCancel" data-role="button">Cancel</a>

    </div>

</div>