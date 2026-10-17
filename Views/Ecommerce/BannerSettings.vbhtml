<script>
    require([GetPathFormat('/Scripts/sources/Ecommerce/BannerSettings.js?v=' + assetversion)]);
</script>

<div data-role="content">
    <img id="imgTempOrgLogo" style="display:none;">
    <input type="file" id="fileUploadOrgLogo" accept="image/*" multiple style="visibility:hidden; display:none;" data-role="none">
    <div class="row">
        <div class="col-sm-8">
            <b>NOTE:</b> Please make that the picture file you are uploading conform to these specifications:
            <br />
            <ul>
                <li>File Type: JPG or PNG</li>
                <li>Dimensions: 850px X 360px</li>
            </ul>
        </div>
        <div class="col-sm-4 text-right">
            <button data-role="none" id="btnAddBanner" class="btn btn-success">Add Banner</button>
        </div>
    </div>
    <br />
    <div id="divListBanners">
    </div>
    
    <div class="mt-4 text-center">
        <a id="btnCancelEcommerceBannerSettings" data-role="none" class="btn btn-secondary btn-lg">Back</a>
    </div>    
</div>