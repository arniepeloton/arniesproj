@*@Code
    ViewData("Title") = "Z-Report"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head
*@
    <script>
        require([GetPathFormat('/Scripts/sources/ReportsPOS/EJournal.js?crc=' + assetversion)
            ]);
    </script>
    @*
End Section
*@
<div id="pageAyalaReport">
   @* <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>*@

    <div>
@*
        @If Common.WebRequest.QueryString("IsMobile") <> "1" Then
            @<applet name="jzebra" code="jzebra.PrintApplet.class" archive="/Scripts/jzebra.jar" width="10" height="10">
            </applet>
        End If
*@
        <div style="width:100%">
            <div class="row mb-3 align-items-center">
                <div class="col-md-2">
                    Select Store:
                </div>
                <div class="col-md-auto">
                    <button style="margin-right:10px;" data-role="none" class="btn btn-secondary btnEJournaleSelectFowst">
                        Select Store
                    </button>
                </div>
            </div>

            <div class="row mb-3 align-items-center">
                <div class="col-md-2">
                    Select Store Machine:
                </div>
                <div class="col-md-auto">
                    <select data-role="none" class="form-select" id="selEJournalFOWSTDevices"></select>
                </div>
            </div>

            <div class="row mb-3 align-items-center">
                <div class="col-md-2">
                    Date Period:
                </div>
                <div class="col-md-auto">
                    <input data-role="none" type="date" id="txtEJournalDateFrom" class="form-control" />
                </div>
                <div class="col-md-auto">
                    to
                </div>
                <div class="col-md-auto">
                    <input data-role="none" type="date" id="txtEJournalDateTo" class="form-control" />
                </div>
            </div>

            <div class="row mb-3 align-items-center">
                <div class="offset-md-2 col-md-auto">
                    <button style="margin-right:10px;" data-role="none" class="btn btn-success" id="btnPOSSalesEJournal">
                        Download E-Journal
                    </button>
                </div>
            </div>
        </div>
    </div>
</div>