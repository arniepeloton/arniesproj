<script>
    require([GetPathFormat('/Scripts/sources/Organization/OrganizationCustomerFOWST.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryPIMProduct.js?crc=')
        ]);
</script>

<div id="pageOrgCustFOWSTList">
    <div data-role="header">
        <h2>Select Customer Places (FOWST)</h2>
    </div>

    <div data-role="content">
        <div data-role="fieldcontain" id="divOrgCustFOWSTProductRecordCount"></div>
        <div>
            <ul data-role="listview" id="lstOrgCustFOWSTProduct" data-inset="true">
            </ul>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgCustFOWSTProductDone" data-role="button"><h3>Done</h3></a>
            </div>
        </div>

    </div>

</div>










