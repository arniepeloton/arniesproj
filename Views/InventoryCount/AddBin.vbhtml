@Code
    ViewData("Title") = "Fowst Bin"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head
    
End Section

<div id="pageMain" data-role="page">
    <div data-role="header">        
        <h1>@ViewData("Title")</h1>
    </div>

    <div data-role="content">        
        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
                <legend> Bin/Shelf Name :</legend>                
                 <input type="text" id="txtFowstBinName"/>                 
            </fieldset>
        </div>      
        
        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
                <legend> Bin/Shelf Description :</legend>                
                 <input type="text" id="txtFowstBinDescription"/>                 
            </fieldset>
        </div>      

        <div data-role="fieldcontain">            
                <a id="btnSaveFowstBin" data-role="button">Save</a> 
        </div>      
    </div>
</div>