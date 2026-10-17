@Code
    ViewData("Title") = "Fowst Bin"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/FowstBin/FowstBin.js?crc=')]);
    </script>
    
End Section

<div id="pageIndex" data-role="page">
    <div data-role="header">        
        <h1>@ViewData("Title")</h1>
    </div>

    <div data-role="content">        
        
        <div style="text-align:center"> <a id="btnNewFowstBin" href="#pageMain" data-role="button" data-icon="plus">Create New Bin</a> </div>

        <form id="frmReceivingSearch">
            <div><input id="txtSearchFowstBin" type="search" value="" /></div>
        </form>

        <div id="divFowstBinSearchResults">

            <div class="ListViewMarginBottom" id="divFowstBinRecordCount"></div>

            <ul id="lstFowstBinList" data-role="listview" data-filter="true">
            </ul>

        </div>
        
    </div>

</div>

<div id="pageMain" data-role="page">
    <div data-role="header">        
        <h1>@ViewData("Title")</h1>
    </div>

    <div data-role="content">        
        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
                <legend> Bin Name :</legend>                
                 <input type="text" id="txtFowstBinName"/>                 
            </fieldset>
        </div>      
        
        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
                <legend> Bin Description :</legend>                
                 <input type="text" id="txtFowstBinDescription"/>                 
            </fieldset>
        </div>      

        <div data-role="fieldcontain">            
                <a href="#" id="btnSaveFowstBin" data-role="button">Save</a> 
        </div>      
    </div>
</div>