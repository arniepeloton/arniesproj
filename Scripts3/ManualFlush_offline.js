require(['common',
    '/Scripts3/Offline/Web.js?crc=',
    '/Scripts3/Offline/Browser/common.js?crc=',
    '/Scripts/sources/SalesPayments/SalesPaymentsExport_offline.js?crc='
    ], function () {

    proj.EnsureMobile(); // END EnsureMobile
    RefreshDatabases();

    function RefreshDatabases() {

        var pRawList;
        var pGuidList = [];
        var pLV;
        var divDatabases = $('#divDatabases');

        pRawList = Sql.GetDatabaseFiles();
        for (var p = 0; p <= pRawList.length - 1; p++) {
            pGuidList.push(pRawList[p]);
        }

        divDatabases.html('');
        pLV = CreateListView();
        pLV.Element.appendTo(divDatabases);
        pLV.OnItem = function(vDataRow, vItem, e) {
        
            var pLink = $('<a>')
                .attr('href', '#')
                .data('data', vDataRow)
                .append($('<h3>')
                    .css('white-space', 'normal')
                    .text(vDataRow)
                ).click(function(e) {
                    e.preventDefault();
                    BeforeStart($(this).data('data'));
                });

            vItem.append(pLink);

        };
        pLV.SetData(pGuidList);
    };

    function BeforeStart(vDbName) {
        
        vDbName = GetFilenameWithoutExtension(vDbName);
        if (IsGuid(vDbName) == false) {
            MessageBox('Filename must be a valid GUID. You chose ' + vDbName);
            return;
        }

        MessageBox('Are you sure you want to manually flush ' + vDbName + '?', '', 
            [
                {
                    Text: 'Yes',
                    Click: function() {
                        setTimeout(function() {
                            Start(vDbName);
                        }, 1000);
                    }
                },
                {
                    Text: 'No'
                }
            ]
        );

    }; // END BeforeStart

    function Start(vDbName) {
        
        Sql.OpenDatabase('Mobiz');
        Web.SetOrgID(vDbName);
        if (Web.HandleOrganization(true)) {
            window.SalesPaymentsExport_offline().Export({
                OnHasExportable: function(vData) {
                    // dont display anything to the user
                    // pDialog = MessageBox('Exporting offline data. Please wait. This box will automatically close after export has finished.', null, []);
                },
                OnDone: function(vData) {
                    MessageBox('Done');
                    // dont display anything to the user
    //                            if (pDialog != undefined)
    //                                pDialog.dialog('close');
    //                            if (vData.Error) 
    //                                setTimeout(function() {
    //                                    MessageBox(vData.Error + ' (' + vData.ErrorLocation + ')');
    //                                }, 3000);
                },
                OnUploaded: function() {
                    window.SalesPaymentsExport_offline().FollowUp();
                }
            }); // END window.SalesPaymentsExport_offline().Export
        }
    };

}); // define