@Code
    ViewData("Title") = "Browser New Button"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code
<script src="/Scripts3/Browser/common.js"></script>
<script src="/Scripts3/Browser/LibraryCustomers.js"></script>
<script src="/Scripts3/Test/BrowserNewButton.js"></script>

<h1>
    Browser New Button</h1>
<button id="butBrowse">Browse</button>


@Section OtherPages
    <div id="pageCustomer" data-role="page">
        <div data-role="header">
            <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
            <h1>Add/Edit Customer</h1>
        </div>
        <div data-role="content">
            <button id="butBackToBrowse">Go back to browser</button>
            <button id="butBackToPage">Go back to page</button>
        </div>
    </div>
End Section
