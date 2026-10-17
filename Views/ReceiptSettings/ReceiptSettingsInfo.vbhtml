<script>
    require([GetPathFormat('/Scripts/sources/ReceiptSettings/ReceiptSettingsInfo.js?v=' + assetversion)]);
</script>

<div id="pageReceiptSettings" class="container-fluid" style="display:block;"> 

    @*<div id"divHeader" data-role="header">
        <h1>Receipt Settings Info</h1>
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
    </div>*@

    <div data-role="content" >

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" data-role="none" class="form-control" readonly id="txtReceiptSetInfoID"/>                 
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtReceiptSetInfoOrganization">Organization: <span style="color: #FF0000;">*</span></label>
            <div class="col-sm-10">
                <input type="text" data-role="none" class="form-control" id="txtReceiptSetInfoOrganization" readonly />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="cmbReceiptSetInfoOrgTradeName">Business or Trade Name: <span style="color: #FF0000;">*</span></label>
            <div class="col-sm-10">
                <select id="cmbReceiptSetInfoOrgTradeName" data-role="none" class="form-select"> <option value="">Please Select</option> </select>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="cmbReceiptSetInfoOrgPaperSize">Paper Size: <span style="color: #FF0000;">*</span></label>
            <div class="col-sm-10">
                <select id="cmbReceiptSetInfoOrgPaperSize" data-role="none" class="form-select">
                    <option value="">Please Select</option>
                    <option value="76mm Paper Roll">76mm Paper Roll</option>
                    <option value="Letter">Letter</option>
                </select>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtReceiptSetInfoTIN">Tax Identification No. (TIN): <span style="color: #FF0000;">*</span></label>
            <div class="col-sm-10">
                <input type="text" data-role="none" class="form-control" id="txtReceiptSetInfoTIN" readonly />
            </div>
        </div>

        <div>
            <div class="row mb-4">
                <div class="col-6">
                    <h1 class="display-6">Receipt Message List</h1>
                </div>
                <div class="col-6 text-right">
                    <button data-role="none" id="btnReceiptSetInfoAddMessage" class="btn btn-success">Add Message</button>
                </div>
            </div>

            <ul id="lstReceiptSetInfoMessages" class="list-group mb-3">
                <li class="list-group-item">No result(s) found</li>
            </ul>
        </div>

        <div>
            <h1 class="display-6 mb-4">Apply the above receipt settings to the following Places</h1>

            <fieldset id="fldsetPlaces" data-role="none" class="mb-3" style="max-height:300px; overflow:auto;"></fieldset>
        </div>

        <div style="display:none;">
            <h1 class="display-6 mb-4">Receipt Logo</h1>
            <form id="frmReceiptLogo" method="post" enctype="multipart/form-data" action="/ReceiptSettings/UploadReceiptLogo">
                <input type='file' id="objFile" name="objFile" />
                <div id="divReceiptLogoList" data-role="fieldcontain">
                    <canvas id="cnvReceiptlogo" width="100" height="100"></canvas>
                </div>
                <input type="submit" value="Upload">
            </form>
        </div>

        <div class="mt-4" style="text-align: center">
            <a id="btnReceiptSettingsInfoCancel" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
            <a id="btnReceiptSettingsInfoDone" data-role="none" class="btn btn-success btn-lg">Save</a>
        </div>
    </div>
</div>

<div id="pageReceiptSettingsInfoMessages" class="container-fluid" style="display:none;">

    @*<div data-role="header">
        <h1>Receipt Messages</h1>
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
    </div>*@

    <h1 class="display-6 mb-4">Receipt Message</h1>

    <div data-role="content">
        <div style="display:none;">
	        <input type="text" data-role="none" class="form-control" style="display:none;" id="txtReceiptSettingsInfoMessageID"/>                
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtReceiptSettingsInfoMessageFooter">Message Footer: <span style="color: #FF0000;">*</span></label>
            <div class="col-sm-10">
                <input type="text" data-role="none" class="form-control" id="txtReceiptSettingsInfoMessageFooter" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtReceiptSettingsInfoSortOrder">Sort Order: <span style="color: #FF0000;">*</span></label>
            <div class="col-sm-10">
                <input type="number" data-role="none" class="form-control" id="txtReceiptSettingsInfoSortOrder" />
            </div>
        </div>

        <div class="mt-4" style="text-align: center">
            <a id="btnReceiptSettingsInfoMessageCancel" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
            <a id="btnReceiptSettingsInfoMessageRemove" data-role="none" class="btn btn-danger btn-lg">Remove</a>
            <a id="btnReceiptSettingsInfoMessageOk" data-role="none" class="btn btn-success btn-lg">Ok</a>
        </div>
    </div>
</div>