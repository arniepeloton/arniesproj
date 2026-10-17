@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/Country/CountrySummary.js?crc=')]);
    </script>
    
End Section

<div id="pageCountrySummary" data-role="page">

    <div id"divHeader" data-role="header">
        <h1>Country Information Summary</h1>
    </div>

    <div data-role="content" >

        <div>
            <ul data-role="listview" data-inset="true">

				<li data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Organization Name
                        <a id='btnEditSumCountryInfo' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li>
                    <div data-role="fieldcontain" style="display:none;">
	                    <input type="text" readonly id="txtSumCountryID"/>                
                    </div>

                    <div data-role="fieldcontain">
                        <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                        <div><label for="txtSumCountryName">Country Name:</label></div>
	                        <div><input type="text" readonly id="txtSumCountryName"/></div>
                        </fieldset>
                    </div>

                    <div data-role="fieldcontain">
                        <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                        <div><label for="txtSumCountryCode">Aplha Three (3) Code:</label></div>
	                        <div><input type="text" readonly id="txtSumCountryCode"/></div>
                        </fieldset>
                    </div>

                    <div data-role="fieldcontain">
                        <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                        <div><label for="txtSumCountryAlpha2Code">Alpha Two (2) Code:</label></div>
	                        <div><input type="text" readonly id="txtSumCountryAlpha2Code"/></div>
                        </fieldset>
                    </div>

                    <div data-role="fieldcontain">
                        <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                        <div><label for="txtSumCountryNumericCode">Numeric Code:</label></div>
	                        <div><input type="number" readonly id="txtSumCountryNumericCode"/></div>
                        </fieldset>
                    </div>

                    <div data-role="fieldcontain">
                        <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                        <div><label for="txtSumCountryISOCode">ISO Code:</label></div>
	                        <div><input type="text" readonly id="txtSumCountryISOCode"/></div>
                        </fieldset>
                    </div>

                    <div data-role="fieldcontain">
                        <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                        <div><label for="txtSumCountryCallingCode">Calling Code:</label></div>
	                        <div><input type="text" readonly id="txtSumCountryCallingCode"/></div>
                        </fieldset>
                    </div>

                </li>

                <li data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Language List
                        <a id='btnEditSumCountryLanguage' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li>
                    <div>
                        <ul data-role="listview" id="lstSumCountryLanguage" data-inset="true">
                        </ul>
                    </div>
                </li>
			</ul>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align:center"> <a id="btnSummaryCountryDone" data-role="button">Done</a> </div>
        </div>
    </div>

</div>

