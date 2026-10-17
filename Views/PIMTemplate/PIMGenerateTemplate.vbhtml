@Code
    ViewData("Title") = "Generate Excel Template for Product List"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/PIMTemplate/PIMGenerateTemplate.js?crc=')
            ]);
    </script>
    
End Section

<div id="pagePIMTemplate" data-role="page">
    <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>


    <div data-role="content">
        <div>
            <ul data-role="listview"data-inset="true">
                <li id="lstOrganizationBrandsHeader" data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        PIM Template
                    </div>
                </li>
                <li>
                    <div>
                        <ul data-role="listview" data-inset="true">
                            <li>
                                <a id="lnkDownloadGenTemplate">Download PIM Template</a>
                            </li>
                        </ul>
                    </div>
                </li>
             </ul>
        </div>

        <br />
        <a data-role="button" data-rel="back">Done</a>            
    </div>
</div>

