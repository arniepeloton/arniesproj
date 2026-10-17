require([
    'common'
    ], function () {

        $(function () {

            $('#frmPIMMeasureSignBrowser').submit(function (e) {
                e.preventDefault();

                ShowPIMMeasureSign();

            });

            $('#btnAddNewPIMMeasureSign').click(function () {

                CreateNewPIMMeasureSignEntry(1, '');

            });

            $('#btnPIMMeasureSignBackToHome').click(function () {

                window.open(GetPathFormat("/People/PeopleMenu"), "_self");

            }); 

            function ShowPIMMeasureSign() {

                var pData = {};
                pData.SearchString = $("#txtPIMMeasureSignSearch").val();

                AjaxSend({
                    url: 'PIMMeasureSign/getPIMMeasureSignList',
                    data: pData,
                    success: function (vData) {
                        LoadPIMMeasureSignList(vData.MeasureSignData.Rows);
                    }
                });

            }

            function LoadPIMMeasureSignList(vRows) {
                var pLV = $('#lstPIMMeasureSign');
                var pItem;

                $("#divPIMMeasureSignSearchResults").hide();
                $("#divPIMMeasureSignSearchNoResults").hide();
                $("#btnAddNewPIMMeasureSign").hide();

                if (vRows.length > 0) {
                    $("#divPIMMeasureSignSearchResults").show();
                    $("#divPIMMeasureSignRecordCount").text(" Number of record(s) found: " + vRows.length);

                    pLV.html('');
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        pItem = $('<li>')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();

                                CreateNewPIMMeasureSignEntry(0, $(this).attr('ID'));

                            })
                            .append($('<h3>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].Sign)
                            )
                        );
                        pLV.append(pItem);
                    };

                    pLV.listview('refresh');
                }
                else {
                    $("#divPIMMeasureSignSearchNoResults").show();
                    $("#btnAddNewPIMMeasureSign").show();
                    $("#lblPIMMeasureSignNotFound").text('The sign "' + $("#txtPIMMeasureSignSearch").val() + '" does not exist. Please consider to search thoroughly using an alternate spelling before creating a new sign to avoid duplication.');
                }
            };

            function CreateNewPIMMeasureSignEntry(IsNew, strID) {
                var pData = {};
                pData.IsNew = IsNew;
                pData.ID = strID;

                window.pageOrig = $.mobile.activePage;

                AjaxSend({
                    url: 'PIMMeasureSign/setCreateNew',
                    data: pData,
                    success: function (vData) {

                        window.IsPageLoaded = false;
                        ChangePage('/PIMMeasureSign/PIMMeasureSignInfo');
                    }
                });

            }





        });
    });