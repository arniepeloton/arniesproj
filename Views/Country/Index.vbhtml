@Code
    ViewData("Title") = "Country Information"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/Country/CountryMenu.js?crc=')]);
    </script>
    
End Section

<div id="pageCountryList" data-role="page">

    <div data-role="header">
        <h1>Country List</h1>
    </div>

    <div data-role="content">

        <form id="frmCountryBrowser" style="margin: 0 -7px">
            <div>
                <input id="txtCountrySearch" type="search" placeholder="Search a Country" value="" />
            </div>
        </form>

        <div id="divCountrySearchResults" style="display:none;">

            <div class="ListViewMarginBottom" id="divCountryRecordCount"></div>

            <ul id="lstCountry" data-role="listview" data-filter="true">
            </ul>

        </div>

        <div id="divCountrySearchNoResults" style="display:none;">
	        <div>
                <div data-role="fieldcontain">
                    <label id="lblCountryNotFound"></label>
                </div>
            </div>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center;"> <a id="btnAddNewCountry" data-role="button" style="display:none;">Add New Country</a> </div>
	        <div style="text-align:center"> <a id="btnCountryBackToHome" data-role="button">Back To Home</a> </div>
        </div>

    </div>

</div>







