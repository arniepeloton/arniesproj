@Code
    ViewData("Title") = "Collections"
    Layout = "~/Views/Shared/plain.vbhtml"
End Code

@Section head
  
    <script>
        require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/DashBoardsAndReports.js?crc=')]);    
    </script>

End Section
    <div data-role="content">
        <div class="ui-grid-a">
                <div class="ui-block-a" style="width:79%;height: 50px; margin-bottom:5px;">
                    <div data=role="fieldcontain">
                        <input id="txtSearch" data-role="search" placeholder="Search customer"autofocus />            
                    </div>
                        
                </div>
                <div class="ui-block-b" style="width: 20%;margin-bottom:5px;">
                    <div data=role="fieldcontain">
                        <a data-role="button" data-theme="c" id="btnSearch">Go</a>    
                    </div>                   
                </div>
            </div>
    </div>
