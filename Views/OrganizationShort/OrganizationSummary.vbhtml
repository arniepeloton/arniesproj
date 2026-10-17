<script>
    require([GetPathFormat('/Scripts/sources/OrganizationShort/OrganizationSummary.js?crc=')]);
</script>

<div id="pageOrganization">

    @*<div id"divHeader" data-role="header">
        <h1>Organization Information Summary</h1>
    </div>*@

    <h2>Organization Information Summary</h2>

    <div data-role="content" >

        <div>
            <ul data-role="listview" data-inset="true">

				<li data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Name
                        <a id='btnSumOrgInfo' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li>
                    <div data-role="fieldcontain" style="display:none;">
	                    <input type="text" readonly id="txtSumID"/>                
                    </div>

                    <div data-role="fieldcontain">
                        <fieldset data-role="controlgroup">
	                        <div><label for="txtSumCorporateName">Name: <span style="color: #FF0000;" >*</span></label></div>
	                        <div><input type="text" readonly id="txtSumCorporateName"/></div>
                        </fieldset>
                    </div>

                </li>

                <li data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Places (FOWST) List
                        <a id='btnSumOrgFOWST' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li>
                    <div>
                        <ul data-role="listview" id="lstSumAddress" data-inset="true">
                        </ul>
                    </div>
                </li>                

                <li data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Representative List
                        <a id='btnSumOrgRepresentative' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li>
                    <div>
                        <ul data-role="listview" id="lstSumRepresentative" data-inset="true">
                        </ul>
                    </div>
                </li>                

                <li data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Company-Wide Product List
                        <a id='btnSumOrgCustProductList' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li>
                    <div>
                        <ul data-role="listview" id="lstSumCompWideProductList" data-inset="true">
                        </ul>
                    </div>
                </li>   

                <li data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Places (FOWST) Product List
                        <a id='btnSumOrgCustFOWSTProductList' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li>
                    <div>
                        <ul data-role="listview" id="lstSumFOWSTProductList" data-inset="true">
                        </ul>
                    </div>
                </li>   

			</ul>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgSummaryDone" data-role="button"><h3>Done</h3></a>
            </div>
        </div>
    </div>

</div>

