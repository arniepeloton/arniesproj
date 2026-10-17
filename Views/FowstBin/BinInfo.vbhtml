@Code
    ViewData("Title") = "Fowst Bin"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/FowstBin/FowstBin.js?crc=')]);
    </script>
    
End Section

<div data-role="page">
    <div data-role="header">        
        <h1>@ViewData("Title")</h1>
         <a href="#" data-icon="arrow-l" data-rel="back" >Back</a>
    </div>

    <div data-role="content">        
        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
                <legend> Bin/Shelf Name :</legend>                
                 <input type="text" id="txtFowstBinName2"/>                 
            </fieldset>
        </div>      
        
        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
                <legend> Bin/Shelf Description :</legend>                
                 <input type="text" id="txtFowstBinDescription2"/>                 
            </fieldset>
        </div>      

        <div data-role="fieldcontain">            
                <a href="#" id="btnSaveFowstBin2" data-role="button">Save</a> 
        </div>      
    </div>
</div>