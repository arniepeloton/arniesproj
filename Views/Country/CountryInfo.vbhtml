@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/Country/CountryInfo.js?crc=')
        ]);
    </script>
    
End Section

<div id="pageCountryInfo" data-role="page">

    <div data-role="header">
        <h1>Country Info</h1>
    </div>

    <div data-role="content" >

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtCountryID"/>                
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	            <div><label for="txtCountryName">Country Name: <span style="color: #FF0000;" >*</span></label></div>
	            <div><input type="text" id="txtCountryName"/></div>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	            <div><label for="txtCountryCode">Aplha Three (3) Code: <span style="color: #FF0000;" >*</span></label></div>
	            <div><input type="text" id="txtCountryCode"/></div>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	            <div><label for="txtCountryAlpha2Code">Alpha Two (2) Code:</label></div>
	            <div><input type="text" id="txtCountryAlpha2Code"/></div>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	            <div><label for="txtCountryNumericCode">Numeric Code:</label></div>
	            <div><input type="number" id="txtCountryNumericCode"/></div>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	            <div><label for="txtCountryISOCode">ISO Code:</label></div>
	            <div><input type="text" id="txtCountryISOCode"/></div>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	            <div><label for="txtCountryCallingCode">Calling Code: <span style="color: #FF0000;" >*</span></label></div>
	            <div><input type="text" id="txtCountryCallingCode"/></div>
            </fieldset>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnCountryInfoDone" data-role="button">Done</a> </div>
        </div>

    </div>

</div>

