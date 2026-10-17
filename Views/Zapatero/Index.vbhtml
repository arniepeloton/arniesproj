@Code
    ViewData("Title") = "Zapatero"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code

<link rel="stylesheet" href="/Content/flatui-css-custom.css" />
<link rel="stylesheet" href="/Content/jquery.dataTables2.css" />

<script>
    require([GetPathFormat('/Scripts/sources/Zapatero/Zapatero.js?crc=')]);
</script>

<div>
    <div class="container" style="margin-top:20px; margin-bottom: 20px">
        <div class="ui-grid-d">
            <div class="ui-block-a" style="width:25%">@*<button data-role="button" data-mini="true" id="btnDeleteData">Delete Zapatero Data</button>*@</div>
            <div class="ui-block-b" style="width:20%">@*<button data-role="button" data-mini="true">View Z-Read Edit</button>*@</div>
            <div class="ui-block-c" style="width:15%"></div>
            <div class="ui-block-d" style="width:20%">@*<button data-role="button" data-mini="true" id="btnGetZReadDataFroServer">Get Z-Read Data from Source</button>*@</div>
            <div class="ui-block-e" style="width:20%; background-color:green"><button data-role="button" data-mini="true" style="background-color:green" id="btnSendZReadDataToServer">Save</button>
            </div>
        </div><!-- /grid-b -->

        <br />

        <div style="margin-top:20px; margin-bottom:20px">
            <div style="width:100%; overflow:auto;">
                <table id="tblSysAdminDataTable" data-role="table" data-mode="reflow" style="margin-right:5px; display:table;" class="ui-table ui-shadow ui-responsive table-stroke table-stripe">
                    <thead>
                        <tr id="tblHeaderDT" class="custom-theme-f txt-center">
                            <th>POS MIN</th>
                            <th>Location</th>
                            <th>Permit to Use (PTU) Ref.</th>
                            <th>Registration Date</th>
                            <th>Latest Accumulated Gross Sales Reading (A)</th>
                            <th>Latest Accumulated Sales Returns and Refunds (B)</th>
                            <th>Latest Accumulated Net Sales Reading (A-B)</th>
                            <th>Reading Date</th>
                            <th>Zap: Latest Accumulated Gross Sales Reading (D)</th>
                            <th>Zap: Latest Accumulated Sales Returns and Refunds (E)</th>
                            <th>Zap: Latest Accumulated Net Sales Reading (D-E)</th>
                            <th>Zap: Reading Date</th>
                        </tr>
                    </thead>
                    <tbody id="tblBodyDT">                       
                    </tbody>
                </table>
            </div>
        </div>

    </div>

    <div id="divDeleteData" data-role="popup">
        <div style="margin:30px">
            <h1> <span color="red"> Warning: </span> </h1>
            <p>
                This function will wipe out ALL your Zapatero data. However,
                deleting Zapatero data will not affect your source data (i.e. POS system data).
                You can still use Zapatero in the future by retrieving data from your source POS system.
            </p>

            <p>Are you sure you want to proceed with deleting all of your Zapatero data?</p>

            <br />

            <div>
                <button>Yes, delete all Zapatero Data</button>
                <button id="btnCancelDelete">Cancel Delete</button>
            </div>
        </div>
    </div>
    
    <div id="divEnterAccumulatedAmount" data-role="popup">
        <div style="margin:30px">
            <h2> <span color="red"> Enter New Gross Accumulated Amount: </span> </h2>
            
            <br />
            <input type="number" placeholder="Enter Amount" id="txtNewAccumulatedAmount"/>
            <br />

            <div>
                <button id="btnAccumDone">Done</button>
            </div>
        </div>
    </div>

    <div id="divSendDataToServer" data-role="popup">
        <div style="margin:30px">
            <h1> <span color="red"> Note </span> </h1>
            <p>
                The data that you have saved in Zapatero will be used as the beginning balance data for your POS system where applicable.
            </p>

            <br />

            <div>
                <button id="btnSendData">Okay, Send Data</button>
            </div>
        </div>
    </div>

    <div id="divDataReceiveInServer" data-role="popup">
        <div style="margin:30px">
            <h1> <span color="red"> Note </span> </h1>
            <p>
                The latest accumulated gross of your Z-Read has been saved. Please please check your Z-Read page to check for updates. 
            </p>

            <br />

            <div>
                <button id="btnReceiveData">Okay</button>
            </div>
        </div>
    </div>

    <div id="divDataReceiveFailed" data-role="popup">
        <div style="margin:30px">
            <h1> <span color="red"> Note </span> </h1>
            <p>
                We cannot update your Z-Read data. Please contact your system administrator.
            </p>

            <br />

            <div>
                <button id="btnReceiveFailed">Okay</button>
            </div>
        </div>
    </div>
</div>
