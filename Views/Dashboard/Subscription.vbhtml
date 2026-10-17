@Code
    ViewData("Title") = "Mobiz Subscription Dashboard"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head
    <script>
        require([GetPathFormat('/Scripts/sources/Dashboard/SubscriptionDashboard.js?crc=')]);
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
                    <th>Subscription No</th>
                    <th>Super Administrator</th>
                    <th>Email Address</th>
                    <th>Mobile Number</th>
                    <th>Company Name</th>
                    <th>Location</th>
                    <th>Business Industry</th>
                    <th>Source</th>
                    <th>Email Verification</th>
                    <th>Last Updated</th>
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
			<h1>Subscription Details</h1>
		</div>

		<div data-role="content" data-theme="c">

        <h3 id="hdrCompanyName"></h3>
        <div><strong>Office Address:</strong> <label id="lblOfficeAddress"></label></div>
        <div><strong>Business Address:</strong> <label id="lblBusinessAddress"></label></div>
        <div><strong>Promo Code:</strong> <label id="lblPromoCode"></label></div>
        <div><strong>Agent Name:</strong> <label id="lblAgentName"></label></div>
        <div><strong>Subscription Date:</strong> <label id="lblSubscriptionDate"></label></div>
        <br />
        <table data-role="table" id="tblDashboardDetails" data-mode="reflow" class="ui-responsive" border="1">
            <thead>
                <tr>
                    <th>Product</th>
                    <th>Concurrent Users</th>
                    <th>Status</th>
                    <th>Activation Date</th>
                </tr>                    
            </thead>                
            <tbody id='tblDashboardBodyDetails'>
            </tbody>
        </table>
            <br />
			<a href="dashboard" data-role="button" data-rel="back" data-theme="c">OK</a>    
		</div>
	</div>