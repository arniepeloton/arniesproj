@Code
    ViewData("Title") = "Upload Database"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code

<script>
    require([GetPathFormat('/Scripts3/Upload.js?crc=')]);
</script>


@Section head

    
End Section


        <div data-role="fieldcontain">    
            <div>Choose database to upload: </div>
            <ul data-role="listview" id="lstDatabaseFrame" data-inset="true">                
            </ul>
        </div>
        <a data-role="button" id="butUpload"><h3>Upload</h3></a>
        <a href="#" data-role="button" data-rel="back"><h3>Back</h3></a>
        
        <div data-role="popup" id="popupMenu" >
            <div data-role="popup" id="dvpopup" data-theme="a" class="ui-corner-all" data-dismissible="false" data-overlay-theme="a">
                <p id="popMessage" style="padding:10px 20px;text-align:center"></p>
            </div>
        </div>
