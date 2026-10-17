@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require(['/Scripts/sources/ReceiptSettings/ReceiptSettingsMessage.js']);
    </script>
        
End Section

<div id="pageReceiptSettingsMessage" data-role="page">

    <div id"divHeader" data-role="header">
        <a id="btnGoTo" href="#popupMenu" data-rel="popup" data-inline="true" class="ui-btn-right">Go To</a>
        <h1>Receipt Message</h1>
    </div>

    <div data-role="content" >

        <div>
            <ul data-role="listview" id="lstMessageContainer" data-inset="true">   
				<li data-role="list-divider">Receipt Message List</li>
                <li>
                    <div data-role="fieldcontain"><a href="#" id="btnAddMessage" data-role="button" data-icon="plus" data-inline="true">Add Message</a></div>
                    <div>
                        <ul data-role="listview" id="lstMessages" data-inset="true">
                        </ul>
                    </div>
                </li>
            </ul>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnReceiptMsgSettingsBack" data-role="button">Back</a> </div>
	        <div style="text-align:center"> <a id="btnReceiptMsgSettingsDone" data-role="button">Done</a> </div>
        </div>

        <div id="popupMenu" data-role="popup">
            <ul data-role="listview" data-inset="true" style="min-width: 210px;">
                <li data-role="divider">Menu</li>
                <li><a href="ReceiptSettingsInfo">Receipt Settings</a></li>
            </ul>
        </div>

    </div>

</div>

<div id="pageMessages" data-role="page">

    <div data-role="header">
        <h1>Receipt Messages</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain">
	        <input type="text" readonly style="display:none;" id="txtMessageID"/>                
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	            <div><label for="txtMessageFooter">Message Footer: <span style="color: #FF0000;" >*</span></label></div>
	            <div><input type="text" id="txtMessageFooter"/></div>              
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	            <div><label for="txtSortOrder">Sort Order: <span style="color: #FF0000;" >*</span></label></div>
	            <div><input type="number" id="txtSortOrder"/></div>              
            </fieldset>
        </div>

        <div>  
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnMessageSave" data-role="button">Ok</a> </div>
	        <div style="text-align:center"> <a id="btnMessageRemove" data-role="button">Remove</a> </div>
	        <div style="text-align:center"> <a id="btnMessageCancel" data-role="button">Cancel</a> </div>
        </div>

    </div>

</div>
