@Code
    ViewData("Title") = "POS Setup"
    Layout = "~/Views/Shared/Mobiz.vbhtml"
End Code
<script>
    require([GetPathFormat('/Scripts3/PosSetup.js?crc=')]);
</script>
<h4>
    Branches</h4>
<div id="divBranches">
</div>
@Section OtherPages
    <div id="pageDetails" data-role="page">
        <div data-role="header">
            <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
            <h1>
                Branch Detail</h1>
        </div>
        <div data-role="content">
            <div data-role="fieldcontain">
                <fieldset data-role="controlgroup">
                    <legend>Address:</legend><span id="spanAddress">spanAddress</span>
                </fieldset>
            </div>
            <div data-role="fieldcontain">
                <fieldset data-role="controlgroup" data-type="horizontal">
                    <legend>Branch:</legend>
                    <div data-role="controlgroup">
                        <div id="spanBranch" style="margin-bottom: 10px;">
                            spanBranch
                        </div>
                    </div>
                </fieldset>
            </div>

            <div data-role="fieldcontain">
                <fieldset data-role="controlgroup" data-type="horizontal">
                    <legend>FOWST Name:</legend>
                    <div data-role="controlgroup">
                        <div id="spanFowstName" style="margin-bottom: 10px;">
                            spanFowstName
                        </div>
                        <button id="butFowst">Browse...</button>
                    </div>
                </fieldset>
            </div>
           @* <div data-role="fieldcontain">
                <fieldset data-role="controlgroup" data-type="horizontal">
                    <legend>POS OR Counter: </legend>
                    <input id="txtPosOrCounter" />
                    <p>
                        The next transaction's official receipt number will be this value + 1
                    </p>
                </fieldset>
            </div>*@
            <button id="butSave">Save</button>
        </div>
    </div>
End Section
@*OtherPages*@ 