@Code
    ViewData("Title") = "Zapatero"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code

<link rel="stylesheet" href="/Content/flatui-css-custom.css" />
<link rel="stylesheet" href="/Content/jquery.dataTables2.css" />

<script>
    require([GetPathFormat('/Scripts/sources/Zapatero/Main.js?crc=')]);
</script>

<div>
    <div class="container" style="margin-top:20px; margin-bottom: 20px">
       
        <div style="margin-top:20px; margin-bottom:20px">
            <div style="width:100%; overflow:auto;">
                <table id="tblSysAdminDataTable" data-role="table" data-mode="reflow" style="margin-right:5px; display:table;" class="ui-table ui-shadow ui-responsive table-stroke table-stripe">
                    <thead>
                        <tr id="tblHeaderDT" class="custom-theme-f txt-center">
                            <th>Device Name</th>
                            <th>FOWST Name</th>
                            <th>Permit to Use (PTU) Ref.</th>
                            <th>B.I.R Machine No</th>
                            <th>Registration Date</th>
                        </tr>
                    </thead>
                    <tbody id="tblBodyDT">                       
                    </tbody>
                </table>
            </div>
        </div>

    </div>


</div>
