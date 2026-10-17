<script>
    require([GetPathFormat('/Scripts/sources/Organization/OrganizationFOWSTProduct.js?v=' + assetversion),
                 GetPathFormat('/Scripts3/Browser/LibraryPIMProduct.js?crc=')
        ]);
</script>

<div id="pageOrgFOWSTList">
    <h1 class="display-6 mb-4">FOWST List</h1>

    <div data-role="content">
        <div data-role="none" class="mb-3" id="divOrgFOWSTProductRecordCount"></div>
        
        <div data-role="none" id="lstOrgFOWSTProduct" class="list-group"></div>

        <div class="mt-4 text-center">
            <a id="btnOrgFOWSTProductDone" data-role="none" class="btn btn-secondary btn-lg">Done</a>
        </div>
    </div>
</div>