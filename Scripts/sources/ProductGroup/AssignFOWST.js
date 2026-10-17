require([
    'common',
    GetPathFormat('/Scripts3/Browser/LibraryFowst.js')
    ], function () {

        var IsNew = 1;
        var IsConfirm = 0;
        var OrgName = '';
        var ID = '';
        var menuFowst = [];

        $(function () {

            var mLoaded = false;

            $.mobile.activePage.bind('pageshow', PageShow);

            function PageShow() {
                mLoaded = true;

                GetMenuFOWST();
            };
            if (!mLoaded) PageShow();
            $('#frmMenuFowstSearch').submit(function (e) {
                e.preventDefault();

                GetMenuFOWST(); ;

                return false;
            });

            $('#btnAssignToFOWST').click(function (e) {
                e.preventDefault();


                if ($.trim($("#selFOWST").val()) == "") {
                    MessageBox("Please select a FOWST",
                            "Assign to FOWST",
                            [
                                { Text: 'Ok' }
                            ]
                );
                    $("#selFOWST").focus();
                    return;
                }

                SaveFOWST();

            });

            $('#btnAssignFOWSTDone').click(function (e) {
                e.preventDefault();

                location = MergeUrlParameters({
                    OrgID: sessionStorage.OrgID,
                    MenuID: querySt('MenuID'), //add menu id to query string
                    IsMobile: CBool(IsMobile) ? 1 : 0
                }, '/ProductGroup/MenuTemplate');

            });

            function querySt(Key) {
                var url = window.location.href;
                KeysValues = url.split(/[\?&]+/);
                for (i = 0; i < KeysValues.length; i++) {
                    KeyValue = KeysValues[i].split("=");
                    if (KeyValue[0] == Key) {
                        return KeyValue[1];
                    }
                }
            }

            function getFOWSTList() {
                var pData = {};
                pData.IsNew = 0;
                pData.ID = sessionStorage["OrgID"];

                AjaxSend({
                    url: 'Organization/setCreateNew',
                    data: pData,
                    success: function (vData) {

                        AjaxSend({
                            url: 'Organization/getOrgFOWSTList',
                            success: function (vDataOrg) {

                                if (vDataOrg.Error == undefined) {

                                    var vRows = vDataOrg.OrganizationFOWSTData.Rows;

                                    $('#selFOWST').empty();

                                    $('#selFOWST').append(new Option('Select FOWST', ''));
                                    for (var p = 0; p <= vRows.length - 1; p++) {
                                        if (menuFowst[p] != vRows[p].ID)
                                            $('#selFOWST').append(new Option(vRows[p].Name, vRows[p].ID, false, false));
                                    }

                                    $('#selFOWST').val('');
                                    $('#selFOWST').selectmenu('refresh');
                                } else {
                                    MessageBox(vDataOrg.Error);

                                }
                            }
                        });
                    }
                });



            }

            function GetMenuFOWST() {
                if (querySt('MenuID')) {
                    var pData = {};
                    pData.SearchKeyword = $.trim($("#txtMenuFowstSearch").val());
                    pData.MenuID = querySt('MenuID');

                    AjaxSend({
                        url: '/ProductGroup/GetMenuFOWST',
                        data: pData,
                        success: function (vData) {
                            if (vData.Error == undefined) {

                                LoadPimMenu(vData.MenuFowstData.Rows);
                            }
                            else
                                MessageBox(vData.Error);
                        }
                    }); // END AjaxSend
                } else {

                    MessageBox('Invalid MenuID');

                }
            }; // END SaveProduct\

            function LoadPimMenu(vRows) {
                var pLV = $('#lstMenuFowstTemplate');
                var pItem;

                $("#divMenuFowstTemplate").text(" Number of record(s) found: " + vRows.length);

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li>')
                    .append($('<a>')
                        .attr('ID', vRows[p].FowstID)
                        .click(function (e) {
                            e.preventDefault();
                            //                            location = MergeUrlParameters({
                            //                                OrgID: sessionStorage.OrgID,
                            //                                MenuID: $(this).attr('ID'), //add menu id to query string
                            //                                IsMobile: CBool(IsMobile) ? 1 : 0
                            //                            }, '/ProductGroup/MenuTemplate');


                        }).append($('<h3>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].Name)
                        )
                    );

                    pLV.append(pItem);

                    menuFowst[p] = vRows[p].FowstID;
                 
                };

                pLV.listview('refresh');

                getFOWSTList();

            };


            function SaveFOWST() {
                if (querySt('MenuID')) {
                    AjaxSend({
                        url: '/ProductGroup/SaveFOWST',
                        data: {
                            FowstID: $('#selFOWST').val(),
                            MenuID: querySt('MenuID')
                        },
                        success: function (vData) {
                            if (vData.Result == 'ok') {
                                location = MergeUrlParameters({
                                    OrgID: sessionStorage.OrgID,
                                    MenuID: querySt('MenuID'), //add menu id to query string
                                    IsMobile: CBool(IsMobile) ? 1 : 0
                                }, '/ProductGroup/MenuTemplate');


                            }
                            else
                                MessageBox(vData.Error);
                        }
                    }); // END AjaxSend
                } else {

                    MessageBox('Invalid MenuID');

                }
            }; // END SaveProduct\


        });
    });