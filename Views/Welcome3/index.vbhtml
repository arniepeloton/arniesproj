@Code
    ViewData("Title") = "Welcome"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
<script>
    $(function () {

    });
</script>
<div id="pageIndex" data-role="page">
    <div data-role="header">
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
        <h1>
            @ViewData("Title")</h1>
    </div>
    <div data-role="content">
        <a href="#popupMenu" data-rel="popup" data-role="button" data-inline="true">Popup menu</a>
        <a href="#pageMenu" data-role="button" data-inline="true">Page menu</a>
        <a href="#pageDialog" data-rel="dialog" data-role="button" data-inline="true">Dialog menu</a>
        <div data-role="popup" id="popupMenu">
            <ul data-role="listview" data-inset="true" style="min-width: 210px;">
                <li data-role="divider">Menu</li>
                <li><a href="#">Enter Customer Name</a></li>
                <li data-role="divider">View Purchase History</li>
                <li><a href="#">Last 2 Weeks of Purchases</a></li>
                <li><a href="#">Last 4 Weeks Purchases</a></li>
                <li><a href="#">Last 3 months of Purchases</a></li>
                <li><a href="#">Last 6 months of purchases</a></li>
                <li><a href="#">Last 1 year of Purchases</a></li>
                <li><a href="#">View Quoted Items</a></li>
                <li><a href="#">View Recently Searched Items</a></li>
                <li><a href="#">View Promos and Featured Items</a></li>
            </ul>
        </div>
    </div>
</div>
<div id="pageMenu" data-role="page">
    <div data-role="header">
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
        <h1>
            Menu</h1>
    </div>
    <div data-role="content">
        <ul data-role="listview">
            <li data-role="divider">Menu</li>
            <li><a href="#">Enter Customer Name</a></li>
            <li data-role="divider">View Purchase History</li>
            <li><a href="#">Last 2 Weeks of Purchases</a></li>
            <li><a href="#">Last 4 Weeks Purchases</a></li>
            <li><a href="#">Last 3 months of Purchases</a></li>
            <li><a href="#">Last 6 months of purchases</a></li>
            <li><a href="#">Last 1 year of Purchases</a></li>
            <li><a href="#">View Quoted Items</a></li>
            <li><a href="#">View Recently Searched Items</a></li>
            <li><a href="#">View Promos and Featured Items</a></li>
        </ul>
    </div>
</div>

<div id="pageDialog" data-role="page">
    <div data-role="header">
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
        <h1>
            Menu</h1>
    </div>
    <div data-role="content">
        <ul data-role="listview">
            <li data-role="divider">Menu</li>
            <li><a href="#">Enter Customer Name</a></li>
            <li data-role="divider">View Purchase History</li>
            <li><a href="#">Last 2 Weeks of Purchases</a></li>
            <li><a href="#">Last 4 Weeks Purchases</a></li>
            <li><a href="#">Last 3 months of Purchases</a></li>
            <li><a href="#">Last 6 months of purchases</a></li>
            <li><a href="#">Last 1 year of Purchases</a></li>
            <li><a href="#">View Quoted Items</a></li>
            <li><a href="#">View Recently Searched Items</a></li>
            <li><a href="#">View Promos and Featured Items</a></li>
        </ul>
    </div>
</div>