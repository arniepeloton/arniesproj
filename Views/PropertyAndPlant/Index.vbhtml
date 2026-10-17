@Code
    ViewData("Title") = "Property And Plant"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require(['/Scripts/sources/PropertyAndPlant/index.js']);
    </script>
        
End Section

<div id="PropertyAndPlant" data-role="page">
   <div data-role="content">
        <div> <a href="#" id="btnNew" data-role="button">Add New</a> </div>
   </div>
</div>