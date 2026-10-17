@Code
    ViewData("Title") = "Mobiz Portal Dashboard"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code
<link href="https://maxcdn.bootstrapcdn.com/font-awesome/4.7.0/css/font-awesome.min.css" rel="stylesheet">
<link rel="Stylesheet" type="text/css" href="/Content/bootstrap/css/v4.3.1/simple-sidebar.css" />
@*<link rel="Stylesheet" type="text/css" href="/Content/bootstrap/css/v4.3.1/bootstrap-table.min.css" />*@

<script type="text/javascript">
    require(['bootstrapjs'], function () {
        require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
            require([GetPathFormat('/Scripts/sources/MobizPortal/Dashboard.js?crc=' + assetversion)]);
        })
    })
</script>

<style type="text/css">
    .ui-loader {
        display:none !important;
    }
</style>

<div>
    <div class="d-flex" id="wrapper">
        <!-- Sidebar -->
        <div class="bg-light border-right" id="sidebar-wrapper">
            <div class="sidebar-heading">Mobiz Portal</div>
            <div class="list-group list-group-flush" id="list-tab" role="tablist">
                <a class="list-group-item list-group-item-action active" id="btnDashboard" data-toggle="list" href="#list-dashboard" role="tab" aria-controls="dashboard">Dashboard</a>
                <a class="list-group-item list-group-item-action bg-light" id="btnLogout" href="#">Logout</a>
            </div>
        </div>

        <div id="page-content-wrapper">
            <nav class="navbar navbar-expand-lg navbar-light bg-primary border-bottom">
                <div class="row" style="width:100%;">
                    <div class="col-6">
                        <button data-role="none" class="btn btn-outline-light" id="menu-toggle">Menu</button>
                    </div>
                    <div class="col-6">
                        <input data-role="none" id="txtSearch" class="form-control" type="search" placeholder="Search here" aria-label="Search">
                    </div>
                </div>
            </nav>
            <div class="container-fluid">
                <div class="tab-content" id="nav-tabContent">
                    <div class="tab-pane fade show active" id="list-dashboard" role="tabpanel" aria-labelledby="list-dashboard-list">
                        <br />
                        <div class="table-responsive">
                            <table id="tblNewCompany" class="table table-hover table-striped">
                                <thead>
                                    <tr>
                                        <th scope="col"></th>
                                        <th scope="col">#</th>
                                        <th scope="col">Company Name</th>
                                        <th scope="col">Administrator</th>
                                        <th scope="col">Contacts</th>
                                        <th scope="col">Subscription Date</th>                                        
                                    </tr>
                                </thead>
                                <tbody id="tbodyNewCompany"></tbody>
                            </table>
                        </div>                        
                    </div>
                </div>                    
            </div>
        </div>
    </div>

    <div class="modal fade" id="modalMessage" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="exampleModalLabel">Message</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    Are you sure you want to activate <b id="lblCompanyName"></b>?
                    <br /><br />
                    If yes, activating organization will create private database to the Mobiz Server.
                </div>
                <div class="modal-footer">
                    <button data-role="none" type="button" class="btn btn-secondary" data-dismiss="modal">No</button>
                    <button data-role="none" type="button" id="btnCreateDatabaseNow" class="btn btn-success">Yes</button>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="divToast" class="toast" role="alert" aria-live="assertive" aria-atomic="true" data-delay="5000" style="display:none; position: fixed; top: 1rem; left: 50%; transform: translate(-50%, 0%);">
    <div class="toast-header" style="background-color:#dc3545; color:#FFF;">
        Message
    </div>
    <div id="divToastMsg" class="toast-body" style="background-color:#f5f5f5">
        Sample
    </div>
</div>