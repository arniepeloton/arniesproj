@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/PIMMeasure/PIMMeasureInfo.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryPIMMeasureSign.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryCountryLanguageVocabularyLabel.js?crc=')
        ]);
    </script>
    
End Section

<div id="pagePIMMeasureInfo" data-role="page">

    <div data-role="header">
        <h1>Measurement Info</h1>
    </div>

    <div data-role="content" >

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtPIMMeasureID"/>                
        </div>

        <div data-role="fieldcontain">
            <div style="display:none;"><input type="text" readonly id="txtPIMMeasureNameID"/></div>
	        <div><label for="btnPIMMeasureName">Measurement: <span style="color: #FF0000;" >*</span></label></div>
            <fieldset data-role="controlgroup">
                <button id="btnPIMMeasureName" data-role="button">Please Select</button>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <div style="display:none;"><input type="text" readonly id="txtPIMMeasureSignID"/></div>
	        <div><label for="btnPIMMeasureSign">Measurement Sign: <span style="color: #FF0000;" >*</span></label></div>
            <fieldset data-role="controlgroup">
                <button id="btnPIMMeasureSign" data-role="button">Please Select</button>
            </fieldset>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnPIMMeasureInfoDone" data-role="button">Done</a> </div>
        </div>

    </div>

</div>

