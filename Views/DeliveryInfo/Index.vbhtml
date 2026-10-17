@Code
    ViewData("Title") = "Delivery and Billing Information"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/DeliveryInfo/DeliveryInfo.js?crc=')]);
    </script>
    
    <style>
        .ui-panel-inner .ui-content myElement {
            overflow: scroll;
            -webkit-overflow-scrolling: touch;
        }
    </style>
End Section

<div id="pageDeliveryInfo" data-role="page">
    <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>

    <div data-role="content">

        <div style="display:none;"><input type="text" readonly id="txtOrgCustomerID"/></div>
        <div data-role="fieldcontain">
	        <label for="txtOrgCustomer">Customer Account:</label>
            <fieldset data-role="controlgroup">
                <input type="text" readonly id="txtOrgCustomer"/>
            </fieldset>
        </div>

        <div id="divDeliveryInfoPONo" data-role="fieldcontain">
	        <label for="txtDeliveryInfoPONo">PO No.: </label>
            <fieldset data-role="controlgroup">
                <input type="text" id="txtDeliveryInfoPONo"/>
            </fieldset>
        </div>

        <div id = "divDeliveryInfoPODate" data-role="fieldcontain">
	        <label>PO Date: </label>
            <fieldset data-role="controlgroup" data-type="horizontal">
                <select id="selDeliveryInfoPODateMonth">
	                <option value="">MMM</option>
                </select>
                <select id="selDeliveryInfoPODateDay">
	                <option value="">DD</option>
                </select>                
                <select id="selDeliveryInfoPODateYear">
	                <option value="">YYYY</option>
                </select>                
            </fieldset>
        </div>

        <div style="display:none;" ><input type="text" readonly id="txtOrderedByID"/></div>
        <div id="divDeliveryInfoOrderedBy" data-role="fieldcontain">
	        <label for="btnOrderBy">Ordered By: <span style="color: #FF0000;" >*</span></label>
            <fieldset data-role="controlgroup">
                <button id="btnOrderBy" data-role="button">Please Select</button>
            </fieldset>
        </div>

        <div style="display:none;"><input type="text" readonly id="txtBillToID"/></div>
        <div data-role="fieldcontain">
	        <label for="btnBillTo">Bill To: <span style="color: #FF0000;" >*</span></label>
            <fieldset data-role="controlgroup">
                <button id="btnBillTo" data-role="button">Please Select</button>
            </fieldset>
        </div>

        <div style="display:none;"><input type="text" readonly id="txtShipToID"/></div>
        <div data-role="fieldcontain">
	        <label for="btnShipTo">Ship To: <span style="color: #FF0000;" >*</span></label>
            <fieldset data-role="controlgroup">
                <button id="btnShipTo" data-role="button">Please Select</button>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
	        <label for="txtDeliveryInfoTelNo">Tel. No.: </label>
            <fieldset data-role="controlgroup">
                <input type="text" readonly id="txtDeliveryInfoTelNo"/>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
	        <label for="txtDeliveryInfoFaxNo">Fax No.: </label>
            <fieldset data-role="controlgroup">
                <input type="text" readonly id="txtDeliveryInfoFaxNo"/>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
	        <label for="txtDeliveryInfoMobileNo">Mobile No.: </label>
            <fieldset data-role="controlgroup">
                <input type="text" readonly id="txtDeliveryInfoMobileNo"/>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
	        <label for="txtDeliveryInfoEmail">Email : </label>
            <fieldset data-role="controlgroup">
                <input type="text" readonly id="txtDeliveryInfoEmail"/>
            </fieldset>
        </div>

        <div id="divDeliveryInfoDelMode" data-role="fieldcontain">
	        <label for="selDeliveryMode" class="select">Delivery Mode:</label>
            <select id="selDeliveryMode">
	            <option value="">Please Select</option>
				<option value="Deliver (Downtown)">Deliver (Downtown)</option>
				<option value="Deliver (Out Of Town)">Deliver (Out Of Town)</option>
				<option value="Pick Up">Pick Up</option>
            </select>
        </div>

        <div id="divDeliverDownTown" style="display:none;">

            <fieldset data-role="controlgroup" data-type="horizontal">
     	        <input type="radio" id="rdoRush" name="DownTownOption" value="1"/>
     	        <label for="rdoRush">Rush</label>
     	        <input type="radio" id="rdoNotRush" name="DownTownOption" value="0"/>
     	        <label for="rdoNotRush">Not Rush</label>
            </fieldset>

            <div id="divRush" data-role="fieldcontain" style="display:none;">
                <div data-role="fieldcontain">
	                <label for="txtDeliverWithIn">Deliver within </label>
                    <fieldset data-role="controlgroup">
                        <input type="number" id="txtDeliverWithIn"/>
                    </fieldset>
                </div>

                <div data-role="fieldcontain">
                    <fieldset data-role="controlgroup">
                        <div>
                            <select id="selHourMinute">
	                            <option value="">Hour or Minute</option>
				                <option value="Working Hours">Working Hours</option>
				                <option value="Working Minutes">Working Minutes</option>
                            </select>
                        </div>
                    </fieldset>
                </div>
            </div>

            <div id="divNotRush" data-role="fieldcontain" style="display:none;">
                <fieldset data-role="controlgroup">
                    <div data-role="fieldcontain">
	                    <label>Maximum service time : 8 working hour(s)</label>
                    </div>
                </fieldset>
            </div>

        </div>

        <div id="divDeliverOutOfTown" style="display:none;">

            <fieldset data-role="controlgroup" data-type="horizontal">
     	        <input type="radio" id="rdoNormalRoute" name="OutOfTownOption" value="1"/>
     	        <label for="rdoNormalRoute">Normal Route</label>
     	        <input type="radio" id="rdoBus" name="OutOfTownOption" value="0"/>
     	        <label for="rdoBus">Bus</label>
            </fieldset>

            <div id="divBus" data-role="fieldcontain" style="display:none;">
                <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
                    <div data-role="fieldcontain">
	                    <label id="lblOutOfTownBus">Maximum service time : 2 hour(s)</label>
                    </div>
                </fieldset>
            </div>

        </div>

        <div id="divPickup" style="display:none;">

            <div id="divPickUpType">
                <fieldset data-role="controlgroup" data-type="horizontal">
     	            <input type="radio" id="rdoCustomerIsWaiting" name="PickUpOption" value="1"/>
     	            <label for="rdoCustomerIsWaiting">Customer is waiting</label>
     	            <input type="radio" id="rdoDateAndTime" name="PickUpOption" value="0"/>
     	            <label for="rdoDateAndTime">Pick Up</label>
                </fieldset>
            </div>

            <div style="display:none;"><input type="text" readonly id="txtPickUpLocationID"/></div>
            <div id="divPickupLocation" data-role="fieldcontain" style="display:none;">
	            <label for="btnPickLocation">Pick Up Location: <span style="color: #FF0000;" >*</span></label>
                <fieldset data-role="controlgroup">
                    <button id="btnPickLocation" data-role="button">Please Select</button>
                </fieldset>
            </div>

            <div id="divCustomerIsWaiting" data-role="fieldcontain" style="display:none;">
                <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
                    <div data-role="fieldcontain">
	                    <label id="lblPickUpCustomerIsWaiting">Maximum service time : 30 Minute(s)</label>
                    </div>
                </fieldset>
            </div>

            <div id="divDateAndTime" data-role="fieldcontain" style="display:none;">
                <div data-role="fieldcontain">
	                <label>Pick Up Date and Time: <span style="color: #FF0000;" >*</span></label>
                    <fieldset data-role="controlgroup" data-type="horizontal">
                        <select id="selPickUpMonth">
	                        <option value="">MMM</option>
                        </select>
                        <select id="selPickUpDay">
	                        <option value="">DD</option>
                        </select>                
                        <select id="selPickUpYear">
	                        <option value="">YYYY</option>
                        </select>                
                    </fieldset>
                </div>
                <div data-role="fieldcontain">
                    <fieldset data-role="controlgroup" data-type="horizontal">
                        <select id="selPickUpHour">
	                        <option value="">HR </option>
                        </select>   
                        <select id="selPickUpMin">
	                        <option value="">MN</option>
                        </select>                
                        <select id="selPickUpAMPM">
	                        <option value="">AM/PM</option>
	                        <option value="AM">AM</option>
	                        <option value="PM">PM</option>
                        </select>                
                    </fieldset>
                </div>
            </div>

        </div>

        <div id="divDeliveryInfoHolder" data-role="fieldcontain">
            <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	            <div>Hold order?</div>
                <div>
                    <select id="selHoldOrder" data-role="slider">
				        <option value="0">No</option>
	                    <option value="1">Yes</option>
                    </select>
                </div>
            </fieldset>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center;"> <a id="btnDeliveryInfoProceedPayments" data-role="button"><h3>Proceed To Payments</h3></a> </div>
	        <div style="text-align:center;"> <a id="btnDeliveryInfoPriceQoute" data-role="button"><h3>Quote Prices</h3><p style="font-weight:normal; margin-top:-15px;">Save Items for Checkout Later</p></a> </div>
	        <div style="text-align:center;"> <a id="btnDeliveryInfoDone" data-role="button"><h3>Done</h3></a> </div>
	        <div style="text-align:center;"> <a id="btnDeliveryInfoBack" data-role="button"><h3>Back</h3></a></div>
        </div>

    </div>

</div>

<div id="divDeliveryInfoPromptEmail" data-role="dialog">
	<div data-role="header" data-theme="d">
		<h1>Email Address</h1>
	</div>

	<div data-role="content" data-theme="c">

        <div><label>Please check the email address(es) that you want to send an electronic price qoute to.</label></div>

        <div id="divDeliveryInfoEmail" data-role="fieldcontain">
            <div id="divDeliveryInfoEmailNone" style="display:none; font-weight:bolder; color:Red;"><label>No Available Email</label></div>
            <fieldset id="fldsetDeliveryInfoEmail" data-role="controlgroup">
            </fieldset>
        </div>

        <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
            <div><label for="txtDeliveryInfoEmailAddress">Other email address:</label></div>
            <div><label for="txtDeliveryInfoEmailAddress" style="font-size:small;">(Please separate multiple email addresses with a comma. Ex. sample1@email.com, sample2@email.com)</label></div>
            <div><input type="text" id="txtDeliveryInfoEmailAddress" /></div>
        </fieldset>
        <br /><br />
		<a id="btnDeliveryInfoEmailOK" data-role="button">Ok</a>       
		<a id="btnDeliveryInfoEmailCancel" data-role="button">Cancel</a>    

	</div>

</div>





