@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/ReceiptSettings/ReceiptSettingsInfo.js?crc=')]);
    </script>

End Section

<div id="pageReceiptSettings" data-role="page"> 

    <div id"divHeader" data-role="header">
        <h1>Receipt Settings Info</h1>
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
    </div>

    <div data-role="content" >

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtReceiptSetInfoID"/>                 
        </div>

        <div data-role="fieldcontain">
	        <label for="txtReceiptSetInfoOrganization">Organization: <span style="color: #FF0000;" >*</span></label>
            <input type="text" readonly id="txtReceiptSetInfoOrganization"/>
        </div>
                
        <div data-role="fieldcontain">            
	        <label for="cmbReceiptSetInfoOrgTradeName">Business or Trade Name: <span style="color: #FF0000;" >*</span></label>                
            <select id="cmbReceiptSetInfoOrgTradeName"> </select>
        </div>

        <div data-role="fieldcontain">            
	        <label for="cmbReceiptSetInfoOrgPaperSize">Paper Size: <span style="color: #FF0000;" >*</span></label>                
            <select id="cmbReceiptSetInfoOrgPaperSize"> 
	            <option value="">Please Select</option>
				<option value="76mm Paper Roll">76mm Paper Roll</option>
				<option value="Letter">Letter</option>
            </select>
        </div>

        <div data-role="fieldcontain">            
	            <label for="txtReceiptSetInfoTIN">Tax Identification No. (TIN): <span style="color: #FF0000;" >*</span></label>
	            <input type="text" readonly id="txtReceiptSetInfoTIN"/>        
        </div>

        <div>
            <ul data-role="listview" id="lstReceiptSetInfoMessageContainer" data-inset="true">   
				<li data-role="list-divider">Receipt Message List</li>
                <li>
                    <div data-role="fieldcontain"><a href="#" id="btnReceiptSetInfoAddMessage" data-role="button" data-icon="plus" data-inline="true">Add Message</a></div>
                    <div>
                        <ul data-role="listview" id="lstReceiptSetInfoMessages" data-inset="true">
                        </ul>
                    </div>
                </li>
            </ul>
        </div>

        <div>
            <ul data-role="listview" id="lstReceiptSetInfoPlacesContainer" data-inset="true">   
				<li data-role="list-divider">Apply the above receipt settings to the following Places:</li>
                <li>
                    <div id="divReceiptSettingsPlaces" data-role="fieldcontain">
                        <fieldset id="fldsetPlaces" data-role="controlgroup">
                        </fieldset>
                    </div>
                </li>
            </ul>
        </div>

        <div style="display:none;">
            <ul data-role="listview"data-inset="true">   
				<li data-role="list-divider">Receipt Logo</li>
                <li>
                    <form id="frmReceiptLogo" method="post" enctype="multipart/form-data" action="/ReceiptSettings/UploadReceiptLogo">
                        <input type='file' id="objFile" name="objFile"/>
			            <div id="divReceiptLogoList" data-role="fieldcontain">
                            <canvas id="cnvReceiptlogo" width="100px" height="100px"></canvas>
                        </div>
                        <input type="submit" value="Upload">
                    </form>
                </li>
            </ul>
        </div>
        
        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnReceiptSettingsInfoDone" data-role="button"><h3>Done</h3></a> </div>
        </div>

    </div>

</div>

<div id="pageReceiptSettingsInfoMessages" data-role="page">

    <div data-role="header">
        <h1>Receipt Messages</h1>
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
    </div>

    <div data-role="content">
        <div style="display:none;">
	        <input type="text" style="display:none;" id="txtReceiptSettingsInfoMessageID"/>                
        </div>

        <div data-role="fieldcontain">            
	        <label for="txtReceiptSettingsInfoMessageFooter">Message Footer: <span style="color: #FF0000;" >*</span></label>
	        <input type="text" id="txtReceiptSettingsInfoMessageFooter"/>
        </div>

        <div data-role="fieldcontain">
	            <label for="txtReceiptSettingsInfoSortOrder">Sort Order: <span style="color: #FF0000;" >*</span></label>
	            <input type="number" id="txtReceiptSettingsInfoSortOrder"/>
        </div>

        <div>  
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnReceiptSettingsInfoMessageOk" data-role="button"><h3>Ok</h3></a> </div>
	        <div style="text-align:center"> <a id="btnReceiptSettingsInfoMessageRemove" data-role="button"><h3>Remove</h3></a> </div>
	        <div style="text-align:center"> <a id="btnReceiptSettingsInfoMessageCancel" data-role="button"><h3>Cancel</h3></a> </div>
        </div>

    </div>

</div>