@Code
    ViewData("Title") = "Upload/Download Product List"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/PIMUpload/PIMUpload.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryPIMFeature.js?crc=')
            ]);
    </script>
    
End Section

<div id="pagePIMUpload" data-role="page">
    <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>

    <div data-role="content">
            <h2>PIM Template</h2>
                <a data-role="button" id="lnkDownloadGenTemplate" >Download PIM Template</a>   
             <br />
             <hr /><br />
              <h2>Upload Product Excel File</h2>
        <div id="divPIMUploadFailed" style="display:none;">
            <p>Something went wrong while trying to save your data. Upon validating, the system has detected either some required fields that were left blank.
             Please <a id="lnkPIMUploadTemplateFailedDL" style="cursor:pointer; font-size:16px;">download your file</a> to see the problem rows highlighted in red.</p>
            <p>Once you have corrected your file, please upload it back. Thank you.</p>
        </div>

        <div id="divFailedMessagesMain" style="display:none;">
        </div>
        <form action="PIMUpload/Index" enctype="multipart/form-data" method="post">

          @*  <label>Upload Product Excel File :</label>*@
            <div style="display:none;"><input type="text" id="OrgID" name="OrgID" style="display:none;"></div>
            <input type="file" id="objFile" name="objFile">
            <input type="submit" value="Upload">
        </form>

        <div>
            <br />
            <br />
	        <div style="text-align:center;"> <a id="btnPIMUploadBackToHome" data-role="button" data-rel="back"><h3>Go Back to Main</h3></a></div>
        </div>

    </div>

</div>

<div id="pagePIMUploadAnalyze" data-role="page">

    <div data-role="header">
        <h1>Analyze Uploaded File</h1>
    </div>

    <div data-role="content">
        
        <br />
        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
	            <div><legend>We have detected a new data column(s) on the Excel file you are attempting to upload. Please set the equivalent Mobiz data field(s) for these columns.</legend></div>
	            <div style="display:none;"><input type="text" id="txtPIMUploadTemplateName"/></div>
            </fieldset>
        </div>
        <br />
        <div id="divPIMUploadAnalyzeFailed" style="display:none;">
            <p>Something went wrong while trying to save your data. Upon validating, the system has detected either some required fields that were left blank.
             Please <a id="lnkPIMUploadAnalyzeTemplateFailedDL" style="cursor:pointer; font-size:16px;">download your file</a> to see the problem rows highlighted in red.</p>
            <p>Once you have corrected your file, please upload it back. Thank you.</p>
        </div>
        <br />
        <div id="divFailedMessages" style="display:none;">
        </div>
        <br />
        <br />

        <div id="divPIMUploadAnalyze">
        </div>

        <div>  
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnOrgPIMUpload" data-role="button">Upload</a> </div>
	        <div style="text-align:center"> <a id="btnOrgPIMUploadCancel" data-role="button">Cancel</a> </div>
        </div>

    </div>

</div>

<div id="pagePIMExcelColDataMatching" data-role="page">

    <div data-role="header">
        <h1>Data Matching</h1>
    </div>

    <div data-role="content">

        <div id="divPIMUploadColDataMatch">
        </div>

        <div>  
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnOrgPIMUploadMatchDataOk" data-role="button">Ok</a> </div>
	        <div style="text-align:center"> <a id="btnOrgPIMUploadMatchDataCancel" data-role="button">Cancel</a> </div>
        </div>

    </div>

</div>

<div id="divPIMUploadPromptEmail" data-role="dialog">
	<div data-role="header" data-theme="d">
		<h1>Email Address</h1>
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

<div id="divPIMUploadPromptPriceLevelDiscount" data-role="dialog">
	<div data-role="header" data-theme="d">
		<h1>Volume Discount Scheme</h1>
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
