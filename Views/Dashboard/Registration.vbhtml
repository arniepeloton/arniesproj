@Code
    ViewData("Title") = "Mobiz Dashboard"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head
    <script>
        require([GetPathFormat('/Scripts/sources/Dashboard/DashboardList.js?crc=')]);
    </script>
End Section

<div id="pageMain" data-role="page">
    <div data-role="header">        
        <h1>@ViewData("Title")</h1>
    </div>

    <div data-role="content" data-dom-cache="true">
        <div><input id="txtSearch" type="search" value="" placeholder="Search" /></div>

        <br />
        <div id="divRecordCount"></div>
        <table data-role="table" id="tblDashboard" data-mode="reflow" class="ui-responsive">
            <thead>
                <tr>
                    <th>Name of Business</th>
                    <th>Business Industry</th>
                    <th>Contact Person</th>
                    <th>Town or City of Operations</th>
                    <th>Telephone No.</th>
                    <th>Email Address</th>
                    <th>Date of Registration</th>
                    <th></th>
                </tr>                    
            </thead>                
            <tbody id='tblDashboardBody'>
            </tbody>
        </table>

        <br />

    </div>

</div>
<div data-role="dialog" data-close-btn="none" id="dialog">
	
		<div data-role="header" data-theme="d">
			<h1>Details</h1>
		</div>

		<div data-role="content" data-theme="c">
            <p><strong>Business Name: </strong><label id="lblBusinessName"></label></p>
            <p><strong>Business Industry: </strong><label id="lblBusinessIndustry"></label></p>
            <p><strong>Country: </strong><label id="lblCountry"></label></p>
            <p><strong>Town or City of Business Operations: </strong><label id="lblCity"></label></p>
            <p><strong>Contact Person: </strong><label id="lblContactPerson"></label></p>
            <p><strong>Telephone Number: </strong><label id="lblTel"></label></p>
            <p><strong>Mobile Number: </strong><label id="lblMobile"></label></p>
            <p><strong>Email Address: </strong><label id="lblEmail"></label></p>
            <p><strong>Product/s of Interest: </strong><label id="lblProduct"></label></p>
            <p><strong>Date of Registration: </strong><label id="lblDate"></label></p>
            <br />
			<a href="dashboard" data-role="button" data-rel="back" data-theme="c">OK</a>    
		</div>
	</div>