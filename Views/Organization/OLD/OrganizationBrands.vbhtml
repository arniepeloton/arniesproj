@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/Organization/OrganizationBrands.js?crc=')
        ]);
    </script>
    
End Section

<div id="pageOrgBrand" data-role="page">

    <div id"divHeader" data-role="header">
        <h1>Brand List</h1>
    </div>

    <div data-role="content" >

        <div>
            <ul data-role="listview" id="lstOrgBrandContainer" data-inset="true">   
				<li data-role="list-divider">Brand List</li>
                <li>
                    <div data-role="fieldcontain"><a id="btnOrgBrandAdd" data-role="button" data-icon="plus" data-inline="true">Add Brand</a></div>
                    <div data-role="fieldcontain" id="divOrgBrandRecordCount"></div>
                    <div>
                        <ul data-role="listview" id="lstOrgBrand" data-inset="true">   
                        </ul>
                    </div>
                </li>
            </ul>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgBrandDone" data-role="button"><h3>Done</h3></a>
            </div>
        </div>

    </div>

</div>

<div id="pageOrgBrandInfo" data-role="page">

    <div data-role="header">
        <h1>Brand Info</h1>
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtOrgBrandRecID"/>                
        </div>

        <div data-role="fieldcontain">
	        <label for="txtOrgBrandName">Brand Name:</label>
            <input type="text" id="txtOrgBrandName"/>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgBrandInfoSave" data-role="button"><h3>Ok</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgBrandInfoCancel" data-role="button"><h3>Cancel</h3></a>
            </div>
        </div>

    </div>

</div>
