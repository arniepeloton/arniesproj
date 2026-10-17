@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head
    <script>
        require([GetPathFormat('/Scripts/sources/Organization/OrganizationFOWSTProduct.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryPIMProduct.js?crc=')
        ]);
    </script>

End Section

<div id="pageOrgFOWSTList" data-role="page">
    <div data-role="header">
        <h1>FOWST List</h1>
    </div>

    <div data-role="content">
        <div data-role="fieldcontain" id="divOrgFOWSTProductRecordCount"></div>
        <div>
            <ul data-role="listview" id="lstOrgFOWSTProduct" data-inset="true">
            </ul>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgFOWSTProductDone" data-role="button"><h3>Done</h3></a>
            </div>
        </div>

    </div>

</div>










