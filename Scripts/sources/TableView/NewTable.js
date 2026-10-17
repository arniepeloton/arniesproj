require(['common',
    '/Scripts3/controller/PrePos.js',
    '/Scripts3/model/OrderTakerSetup.js'
], function () {
    require(['/Scripts3/model/TableView_Offline.js',
        '/Scripts3/Offline/Browser/LibraryFowst.js',
        '/Scripts3/Offline/Browser/LibraryPeople.js',
        '/Scripts3/Browser/LibraryOrganizationEmployeeFOWST.js',
        'promise!/Scripts3/controller/OrderReceiver.js',
        '/Scripts3/controller/OrderTransmitter.js?crc='
], function () {
        var arrColorBadge = [{ color: 'tableBadge-a', selected: false }, { color: 'tableBadge-b', selected: false }, { color: 'tableBadge-c', selected: false }, { color: 'tableBadge-d', selected: false }, { color: 'tableBadge-e', selected: false }];
        var arrFowst = [], tableSetID, isSplitTable = false;
        var arrToJoinTables = [], ctrJoinedTable = 0, ctrRemoveJoinedTable = 0;
        var arrJoinedTable = [], arrRemoveTable = [], arrlistofsplittable = [];

        var IsNewSalesRep = true;
        var arSalesRep = new Array();
        var intSalesRepCntr = 0;
        var intSalesRepIndex = 0;

        var intSalesRepRecID = 0;
        var intSalesRepID = 1;
        var intSalesRepName = 2;
        var intSalesRepState = 3;
        var mOrderTakerSetup = new App.model.OrderTakerSetup();
        var mTransferTableMode = false;
        var mTransferTableFrom;
        var mSplitBill;
        var mSplitMergeBillMode; // 0 / null = not in split/merge mode
                                 // 1 = waiting for a table to split
                                 // 2 = waiting for Table1 to merge
                                 // 3 = waiting for Table2 to merge
        var mIsSplitBill;
        var mSplitBillFromID, mSplitBillToID;
        
        TableView = new App.model.TableView();
        var mPrePos;
        var defaultNoOfCustomer;

        $(function () {
            Business.TransInvoice.FixNoCustomers();
            Business.TransInvoice.FixNoItemCount();
            if ($.isFunction(GetStringObject('GetIsOrderTaker', mOrderTakerSetup))) 
                if (mOrderTakerSetup.GetIsOrderTaker())
                    sessionStorage.setItem('IsOrderTaker', 1);
                else
                    sessionStorage.setItem('IsOrderTaker', 0);
            else
                sessionStorage.setItem('IsOrderTaker', 0);
            sessionStorage.removeItem('CartOrderID');
            sessionStorage.removeItem('CustomerOrganizationID');
            sessionStorage.NoOfSeniorCitizens = 1;
            mPrePos = new App.controller.PrePos();
            mPrePos.callback = LoadRequiredData;
            Android.AddLog('before mPrePos.GetUser() js6JFA75CAWF!');
            mPrePos.GetUser(); 
            Android.AddLog('after mPrePos.GetUser() js2H497F3C77!');

            $('#txtTotalNumberOfCustomer').empty();
            
            for (var i = 0; i <= 100; i++) {
                $('#txtTotalNumberOfCustomer').append(new Option(CStr2(i), CStr2(i), false));
            }
            
            defaultNoOfCustomer = Transfers.ReadFile('defaultNoOfCustomer.dat');

            if (CInt2(defaultNoOfCustomer) >= 0) {
                $('#txtTotalNumberOfCustomer').val(CInt2(defaultNoOfCustomer));
            } else {
                $('#txtTotalNumberOfCustomer').val('0');
            }
            

            try {
                $('#txtTotalNumberOfCustomer').selectmenu('refresh');
            }
            catch (e) {
                $('#txtTotalNumberOfCustomer').selectmenu();
            }

            $('#lnkChangeDefaultValue').click(function () {
                $('#popupDefaultNumberOfCustomer').popup('open');
            })

            $('#btnSaveDefaultNumberOfCustomer').click(function () {
                if (CInt2($('#txtDefaultNumberOfCustomer').val())>=0){}
                Transfer.WriteFile('defaultNoOfCustomer.dat', null, $('#txtDefaultNumberOfCustomer').val());
                $('#popupDefaultNumberOfCustomer').popup('close');
            })

            //BUTTON Table Options
            $('#btnBack').click(function () {
                window.location.href = '/People/PeopleMenu';
            })

            $('#btnTableFowst').click(function () {
                BrowsePopup('LibraryFowst', 'Select F.O.W.S.T', [
                    {
                        DisplayText: [{ Column: 'FowstName' }],
                        Click: function (e) {
                            sessionStorage.FowstID = e.ID;
                            sessionStorage.FowstName = e.FowstName;
                            ClearVariables();
                            $('#txtFowstName').text('FOWST: ' + sessionStorage.FowstName);
                            DisplayDiningArea(TableView.GetTableTemplate(e.ID));
                            GetJoinedTable();
                        },
                        AutoSearch: true,
                        Offline: true,
                        WithSearchbox: false
                    }
                ], {FilterFowst : true})
            })

            $('#btnTableOptionJoin').click(function () {
                ResetCheckList('show')
                $('#divJoinHeader').pageshow();
                arrToJoinTables = [];
                tableSetID = NewID();
                $('#popupTableOptions').popup('close');
            })

            $('#butTableOptionsSplitMergeBill').click(function () {
                //$('#popupTableOptions').popup('close');
                NewMessageBox('', 'Split / Merge Bill', [
                    {
                        Text: 'Split Bill',
                        Click: function() {
                            $('#divSplitBillHeader').pageshow();
                            $('#spanSplitBillInstruction').text('Select the table to split bill.');
                            mSplitMergeBillMode = 1;
                        },
                        Theme: 'c',
                        Size: 2
                    },
                    {
                        Text: 'Split / Merge Tables',
                        Click: function() {
                            // commented on 2018_05_29__15_34_57_9027
                            //NewMessageBox('This function is not yet implemented. ref: jsD9Y4U8B4W6!');
                            //return;

                            $('#divSplitBillHeader').pageshow();
                            $('#spanSplitBillInstruction').text('Select the table to split/merge bill from.');
                            mSplitMergeBillMode = 2;
                            $('.tableChecklist').prev().each(function () {
                                if (!CBool($(this).hasClass('tableOccupied'))) {
                                    $(this).next().next().next().show()
                                }
                            })
                        },
                        Size: 2
                    }
                ]);
            });

            //END Table Options

            //BUTTON Join
            $('#btnJoinCancel').click(function () {
                ResetCheckList('hide');
                $('#divMainHeader').pageshow();
            })

            $('#btnJoinOk').click(function () {
                ctrJoinedTable += 1;
                var joinedTable = JoinedTable();
                joinedTable.forEach(function (currentValue, index) {
                    TableView.SaveJoinedTables(currentValue);
                })
            })
            //END Join

            $('#butTransferTableCancel').click(function () {
                mTransferTableMode = false;
                ResetCheckList('hide');
                $('#instructionTransferTable').hide();
                $('#divMainHeader').pageshow();
                ShowOccupiedTable('hide');
            });

            //BUTTON Split
            $('#butTableOptionsSplitTable').click(function () { // was btnTableOptionSplit
                $('#popupTableOptions').popup('close');
                if (arrJoinedTable.length == 0) {
                    NewMessageBox('There\'s nothing to split tables.', 'Message');
                } else {
                    isSplitTable = true;
                    $('#instructionSplit').show();
                    $('#divSplitHeader').pageshow();
                    ShowJoinedTable('show');
                }                
            });

            $('#butTableViewOptions').click(function () {
                $('#popupTableOptions').popup('open');
            });

            $('#butTableOptionsTransferTable').click(function () {
                mTransferTableMode = true;
                $('#popupTableOptions').popup('close');
                $('#divTransferTableHeader').pageshow();
                $('#instructionTransferTable').show();
                $('#spanTransferTableInstruction').text('Click a table to transfer from.');
                ShowOccupiedTable('show');
            });

            $('#btnSplitCancel').click(function () {
                isSplitTable = false;
                $('#instructionSplit').hide();
                $('#divMainHeader').pageshow();
                ShowJoinedTable('hide');
            })
          
            $('#btnSplitBillCancel').click(function () {
                mIsSplitBill = 0;
                mSplitMergeBillMode = 0;
                $('#divMainHeader').pageshow();
                ShowJoinedTable('hide');
            });

            $('#btnOkTable').click(function () {
                var remainingTable = arrlistofsplittable.length - ctrRemoveJoinedTable;

                if (remainingTable == 1) {
                    //if remaining table in joined-set-table is one just do the split all method
                    NewMessageBox('The remaining one table will also be removed from joined table set. Are you sure you want to continue?', 'Warning',
                        [
                            {
                                Text: 'Yes',
                                Click: function () {
                                    $('#listOfSelectedJoinedTables li').each(function () {
                                        $(this).data('tableinfo').State = 'removed';
                                        arrRemoveTable.push($(this).data('tableinfo'));
                                    })
                                    ctrRemoveJoinedTable += 1;
                                    RemoveTable();
                                }
                            }, {
                                Text: 'No', Click: function () {
                                    NewMessageBoxClose();
                                    $('#popSplitJoinedTables').popup('open');
                                }
                            }
                        ]
                    )
                } else {
                    $('#listOfSelectedJoinedTables li').each(function () {
                        arrRemoveTable.push($(this).data('tableinfo'));
                    })
                    RemoveTable();
                }  
            })

            $('#btnSplitAllTable').click(function () {
                var isOccupied = false;
                var parentName, msg;

                for (var i = 0; i < arrlistofsplittable.length; i++){
                    if (CBool(arrlistofsplittable[i].IsOccupied) && CBool(arrlistofsplittable[i].IsParent)) {
                        isOccupied = true;
                        parentName = arrlistofsplittable[i].TableName;
                    }
                }
                
                if (isOccupied) {
                    msg = 'You\'re about to split all tables within joined table set. We cannot remove this ' + parentName +
                                    ' from this table joining because the billing ' +
                                    ' payment for this group is based on table ' +
                                    parentName + ', but the members may be split from the joined table set. <br><br>' +
                                    'Are you sure you want continue?';                    
                } else {
                    msg = 'Are you sure you want to split all tables within joined table set?'
                }

                NewMessageBox(msg, 'Message',
                                [
                                    {
                                        Text: 'Yes',
                                        Click: function () {
                                            ctrRemoveJoinedTable = 0;
                                            $('#listOfSelectedJoinedTables li').each(function () {
                                                $(this).data('tableinfo').State = 'removed';
                                                ctrRemoveJoinedTable += 1;
                                                arrRemoveTable.push($(this).data('tableinfo'));
                                            })
                                            RemoveTable();
                                        }
                                    },
                                    {
                                        Text: 'No',
                                        Click: function () {
                                            NewMessageBoxClose();
                                            $('#popSplitJoinedTables').popup('open');
                                        }
                                    }
                                ]
                            )
            })
            //END Split

            $('#butViewMode').click(function () {
                NewMessageBox('', 'POS Mode', [
                        {
                            Text: '<b>RETAIL MODE</b> <br>' +
                                '<span style="font-size:14px;">Makes it easy for retailers to search for SKUs using keyboard entry or bar code / QR code scanning (best for retailers with hundreds or thousands of SKUs).</span>',
                            Size: 4,
                            Click: function () {
                                var pParams = GetUrlParameters();

                                location = MergeUrlParameters({
                                    mode: pParams.mode,
                                    OrgID: sessionStorage.OrgID,
                                    rsd: pParams.rsd,
                                    IsMobile: CBool(IsMobile) ? 1 : 0
                                }, '/SalesMenu');
                            }
                        },
                        {
                            Text: '<b>HOT BUTTON MODE</b> <br>' +
                                '<span style="font-size:14px;">Shows predefined buttons sorted by categories (best for fast food outlets and retailers whose items do not have UPC or bar code labels). Administrators can customize these buttons online.</span>',
                            Size: 4,
                            Click: function () {
                                sessionStorage.PosMode = 'Hot Button Mode';

                                if ($(window).width() > proj.WideWidth) {
                                    location = MergeUrlParameters({
                                        OrgID: sessionStorage["OrgID"],
                                        mode: 'pos',
                                        IsMobile: IsMobile ? 1 : 0
                                    }, '/SalesMenu/POSViewAll');
                                } else {
                                    location = MergeUrlParameters({
                                        HotButtons: 1,
                                        OrgID: sessionStorage["OrgID"],
                                        mode: 'pos',
                                        IsMobile: IsMobile ? 1 : 0
                                    }, '/SalesMenu');
                                }
                            }
                        },
                        {
                            Text: '<b>TABLE VIEW MODE</b> <br>' +
                                '<span style="font-size:14px;">Lets you view dining patrons as they are seated in tables (best for fine dining restaurants that require a high-touch service).</span>',
                            Size: 4,
                            Click: function () {
                                if (OfflineMode) {
                                    Android.Toast('You\'re already in Table View Mode');
                                } else {
                                    NewMessageBox("Table View Mode is only available on the mobile app",
                                                "Table View Mode",
                                                [
                                                    { Text: 'Ok' }
                                                ]
                                        );
                                }
                            }
                        },
                        {
                            Text: '<b>FMCG MODE (Modern)</b> <br>' +
                                '<span style="font-size:14px;">Fast Moving Consumer Goods</span>',
                            Size: 4,
                            Click: function () {
                                if(window.location.pathname == '/SalesMenu/FMCG'){
                                    if (OfflineMode) {
                                        Android.Toast('You are already in FMCG (Modern) Mode');
                                    }
                                    else {
                                        WebToast('You are already in FMCG (Modern) Mode');
                                    }
                                } else {
                                    location = MergeUrlParameters({
                                        OrgID: sessionStorage["OrgID"],
                                        mode: 'pos',
                                        IsMobile: IsMobile ? 1 : 0
                                    }, '/SalesMenu/FMCG');
                                }
                            }
                        },
                        {
                            Text: '<b>FMCG MODE (DOS)</b> <br>' +
                                '<span style="font-size:14px;">Fast Moving Consumer Goods</span>',
                            Size: 4,
                            Click: function () {
                                if (window.location.pathname == '/SalesMenu/FMCGBasic') {
                                    if (OfflineMode) {
                                        Android.Toast('You are already in FMCG (DOS) Mode');
                                    }
                                    else {
                                        WebToast('You are already in FMCG (DOS) Mode');
                                    }
                                } else {
                                    location = MergeUrlParameters({
                                        OrgID: sessionStorage["OrgID"],
                                        mode: 'pos',
                                        IsMobile: IsMobile ? 1 : 0
                                    }, '/SalesMenu/FMCGBasic');
                                }
                            }
                        }
                    ], {AutoDismiss : true, CloseButton: 'enable'})
            })

            var mSelectedSalesRep;

            $('#btnAddRepresentative').click(function (e) {
                                    
                    BrowsePopup('LibraryPeople', 'Employee', [
                        {
                            DisplayText: [{ Column: 'PeopleName' }],
                            Click: function (e) {
                                
                                if (!IsSalesRepExist(e.ID)) {
                                    PromptUserAccess('Table view: add sales rep w/o PIN', {
                                        OnClickOKSuccess: function() {
                                            AddSalesRep(e);    
                                        },
                                        OnFailed: function() {
                                            $('#popupTotalNumberOfCustomer').popup('close');
                                            setTimeout(function() {
                                                $('#divPopNumericPassword input[type=password]').val('');
                                                $('#divPopNumericPassword').popup({
                                                    afteropen: function( event, ui ) {
                                                        $('#divPopNumericPassword input').focus();
                                                    } 
                                                });
                                                $('#divPopNumericPassword').popup('open');
                                            }, 200);
                                            mSelectedSalesRep = e;
                                        }
                                    });
                                }
                            },
                            Close: function () {
                                $('#popupTotalNumberOfCustomer').popup('open');
                            },
                            AutoSearch: true,
                            Offline: true
                        }
                    ]);
                 
            }); // pTxtSalesRep.keydown(function(e) {

            function AddSalesRep(e) {
                IsNewSalesRep = true;
                intSalesRepCntr = intSalesRepCntr + 1;
                intSalesRepIndex = intSalesRepCntr;

                arSalesRep = new Array();
                arSalesRep[intSalesRepRecID] = "";
                arSalesRep[intSalesRepID] = e.ID;
                arSalesRep[intSalesRepName] = e.PeopleName;
                arSalesRep[intSalesRepState] = (IsNewSalesRep ? "New" : "Edit");

                sessionStorage["POSSalesRep-" + intSalesRepIndex] = JSON.stringify(arSalesRep);
                sessionStorage["POSSalesRepIndex"] = intSalesRepIndex;

                LoadPOSSalesRepList();
            }

            $('#divPopNumericPassword button[name=ok]').click(function() {
                $('#divPopNumericPassword div[name=message]').text('');
                var pDR = Business.LibraryPeople.ByID(mSelectedSalesRep.ID);
                if (pDR == null) {
                    $('#divPopNumericPassword div[name=message]').text('Employee does not exist. ref: js92HP5PMQTJ!');
                    return;
                }
                var pPin = $('#divPopNumericPassword input[type=password]').val();
                if (pDR.DTRPIN != pPin) {
                    $('#divPopNumericPassword div[name=message]').text('Invalid PIN');
                    return;
                }
                AddSalesRep(mSelectedSalesRep);
                $('#divPopNumericPassword').popup('close');
                $('#popupTotalNumberOfCustomer').popup('open');
            });

            $('#divPopNumericPassword button[name=close]').click(function() {
                $('#divPopNumericPassword').popup('close');
                $('#popupTotalNumberOfCustomer').popup('open');
            });

            Android.AddLog('begin proj.OrderReceiver = new App.controller.OrderReceiver(); js6QD685KAMU!');
            proj.OrderReceiver = new App.controller.OrderReceiver();
            Android.AddLog('1 jsLNGXA7TG2E!');
            proj.ReloadTableView = function() {
                if (CInt2(defaultNoOfCustomer) >= 0) {
                    $('#txtTotalNumberOfCustomer').val(CInt2(defaultNoOfCustomer));
                } else {
                    $('#txtTotalNumberOfCustomer').val('0');
                }
                $('#txtTotalNumberOfCustomer').selectmenu('refresh');
                DisplayDiningArea(TableView.GetTableTemplate(sessionStorage.FowstID));
                GetJoinedTable();
            };
            proj.OrderReceiver.AddSaveListener(function() {
                proj.ReloadTableView();
            });
            
            Android.AddLog('2 jsMJUJY8PRFZ!');
            proj.OrderTransmitter = new App.controller.OrderTransmitter();
            Android.AddLog('3 js795AVVGMQN!');
            proj.OrderTransmitter.Start();
            Android.AddLog('end proj.OrderReceiver = new App.controller.OrderReceiver(); jsXT7XFMYDLC!');
        }); // $(function() {

        function IsSalesRepExist(vSalesRepID) {

            for (var i = 0; i <= intSalesRepCntr; i++) {

                if (sessionStorage["POSSalesRep-" + i] != undefined) {

                    arSalesRep = new Array();
                    arSalesRep = JSON.parse(sessionStorage["POSSalesRep-" + i]);

                    if (arSalesRep[intSalesRepState] != "Remove") {

                        if (arSalesRep[intSalesRepID] == vSalesRepID) {
                            return true;
                        }

                    }

                }
            }

            return false;
        }


        function LoadPOSSalesRepList() {
            var pLV = $('#lstSalesRep');
            var pItem;

            pLV.html('');
            for (var i = 0; i <= intSalesRepCntr; i++) {

                if (sessionStorage["POSSalesRep-" + i] != undefined) {

                    arSalesRep = new Array();
                    arSalesRep = JSON.parse(sessionStorage["POSSalesRep-" + i]);

                    if (arSalesRep[intSalesRepState] != "Remove") {

                        pItem = $('<li id="lst' + i + '">')
                        .append($('<a>')
                            .attr('ID', i)
                            .data('Data', arSalesRep)
                            .append($('<h3>')
                                    .attr("style", "white-space:normal")
                                    .text(arSalesRep[intSalesRepName])
                            )
                        )
                        .append($('<a>')
                            .attr('ID', i)
                            .attr('href', "#")
                            .attr('data-icon', "minus")
                            .data('Data', arSalesRep)
                            .click(function (e) {
                                e.preventDefault();

                                sessionStorage["POSSalesRepToRemoveIndex"] = $(this).attr('ID');
                                sessionStorage["POSSalesRepToRemove"] = JSON.stringify($(this).data('Data'));

                                MessageBox("Remove this record?",
                                    'Remove',
                                    [
                                        {
                                            Text: 'Yes',
                                            Click: function () {

                                                var arSalesRepToRemove = new Array();
                                                arSalesRepToRemove = JSON.parse(sessionStorage["POSSalesRepToRemove"]);
                                                intSalesRepIndex = sessionStorage["POSSalesRepToRemoveIndex"];

                                                arSalesRepToRemove[intSalesRepState] = "Remove";
                                                sessionStorage["POSSalesRep-" + intSalesRepIndex] = JSON.stringify(arSalesRepToRemove);

                                                LoadPOSSalesRepList();

                                                sessionStorage.removeItem("POSSalesRepToRemove");
                                                sessionStorage.removeItem("POSSalesRepToRemoveIndex");
                                                history.back();
                                            }
                                        }
                                        ,
                                        {
                                            Text: 'No'
                                        }
                                    ]
                                );

                            })
                    );

                        pLV.append(pItem);

                    }

                }
            }

            pLV.listview('refresh');
        };

        function RemoveTable() {
            var newJoinedTable = [];
            var parentname, isSetNewParent;
            for (var i = 0; i < arrRemoveTable.length; i++) {
                var table = $('[tableid="' + arrRemoveTable[i].ID + '"]')
                var isOccupiedAndParent = (table.hasClass('tableOccupied') && CBool(table.data('tableinfo').IsParent));

                if (CBool(arrRemoveTable[i].IsParent) && arrRemoveTable[i].State == "removed" && ctrRemoveJoinedTable != arrRemoveTable.length) {
                    arrRemoveTable[i + 1].IsParent = true
                }

                if (arrRemoveTable[i].State != "removed") {
                    newJoinedTable.push(arrRemoveTable[i])
                } else {
                    $('[badgeid="' + arrRemoveTable[i].ID + '"]').attr('class', 'tableBadge').hide();
                    table.removeData('joinedData').removeClass('joined').removeClass((isOccupiedAndParent ? '' : 'tableOccupied'))
                        .next().next().next().show();
                }

                if (CBool(arrRemoveTable[i].IsParent)) {
                    parentname = arrRemoveTable[i].TableName;
                }

                table.removeData('joinedData');
                TableView.SaveJoinedTables(arrRemoveTable[i]);
            }

            //update the joinedData object in available tables
            for (var m = 0; m < newJoinedTable.length; m++) {
                $('[badgeID="' + newJoinedTable[m].ID + '"]').text(parentname)
                $('[tableID="' + newJoinedTable[m].ID + '"]')
                    .removeData('joinedData').data('joinedData', newJoinedTable)
                    .removeData('tableinfo').data('tableinfo', newJoinedTable[m]);
            }

            $('#popSplitJoinedTables').popup('close');
        }

        function ShowOccupiedTable(display) {
            if (display == 'show') {
                $('.tableChecklist').prev().each(function () {
                    if ($(this).hasClass('tableOccupied') == false) {
                        $(this).next().next().next().show();
                    }
                });
            } else {
                $('.tableDisable').hide();
            }            
        }

        function ShowVacantTable(display) {
            if (display == 'show') {
                $('.tableChecklist').prev().each(function () {
                    if ($(this).hasClass('tableOccupied')) {
                        $(this).next().next().next().show();
                    }
                });
            } else {
                $('.tableDisable').hide();
            }            
        }

        function ShowJoinedTable(display) {
            if (display == 'show') {
                $('.tableChecklist').prev().each(function () {
                    if ($(this).hasClass('joined') == false) {
                        $(this).next().next().next().show()
                    }
                })
            } else {
                $('.tableDisable').hide()
            }            
        }

        function JoinedTable() {
            if ((arrToJoinTables.length == 1 && CBool(arrToJoinTables[0].IsOccupied) == false) || arrToJoinTables.length == 0) {
                NewMessageBox('Please select at least two (2) or more tables to join.', 'Message');
            } else {                
                arrToJoinTables[0].IsParent = true;
                var color = ColorBadge(), tempJoinedTable = [];

                SortByTableNo();

                for (var i = 0; i < arrToJoinTables.length; i++) {
                    //just show badge if there's multiple joined or occupied table
                    if (arrToJoinTables.length != 1) {
                        $('[badgeID="' + arrToJoinTables[i].ID + '"]')
                        .addClass(color)                        
                        .text(arrToJoinTables[0].TableName)
                        .show()
                    }                    

                    $('[tableID="' + arrToJoinTables[i].ID + '"]')
                        .addClass((CBool(arrToJoinTables[i].IsOccupied) ? 'tableOccupied' : ''))
                        .removeData('joinedData')
                        .data('joinedData', arrToJoinTables)
                        .addClass('joined') // just to know what buttons are joined

                }
                tempJoinedTable = arrToJoinTables;
                arrJoinedTable.push(arrToJoinTables);                
                arrToJoinTables = [];
                
                ResetCheckList('hide');
                $('#divMainHeader').pageshow();
                return tempJoinedTable
            }
        }

        function LoadRequiredData() {
            //arrFowst = OfflineBrowser.LibraryFowst.GetData({FilterFowst : 1, keywords: ''}).Rows;

            //if(sessionStorage.FowstID == undefined){
            //    sessionStorage.FowstID = arrFowst[0].FowstID;
            //    sessionStorage.FowstName = arrFowst[0].FowstName;
            //}
            var pData;
            var pUseOnlineData;

            $('#txtFowstName').text('FOWST: ' + sessionStorage.FowstName);
            pUseOnlineData = mOrderTakerSetup.GetSettings().dataTransmissionMethod == 'clientServer';
            if (pUseOnlineData) {
                var pParams = {
                    UseOnlineData: true,
                    Callback: function(vData) {
                        Proceed(vData);
                    }
                };
                TableView.GetTableTemplate(sessionStorage.FowstID, pParams);
            } else {
                pData = TableView.GetTableTemplate(sessionStorage.FowstID);
                Proceed(pData);
            }

            function Proceed(vData) {
                DisplayDiningArea(vData);
                GetJoinedTable();
            }
        }

        function GetJoinedTable() {
            var existJoinedTable = TableView.JoinedTables();
            
            for (var i = 0; i < existJoinedTable.length; i++) {
                arrToJoinTables = existJoinedTable[i];
                ctrJoinedTable += 1;
                JoinedTable();
            }
        }

        function DisplayDiningArea(vDine) {
            var lstDiningArea = $('#lstDiningArea');
            var ctrDiningAreaTab = 1;

            $('#lstDiningArea').empty();
            $('#divTabContent').empty();

            if (vDine == null || vDine == undefined) {
                $('#divTabContent').append($('<h3>')
                        .html('No Table Setup found. <br>')
                    )
                    .append($('<span>')
                        .html('To create table setup for this location (FOWST), go to System Administration > Table View Settings or just copy the link below and paste it to your browser and follow the instructions. <br><br>' +
                            'Link Description: "How do I set up my dining area and tables in Mobiz POS > Table View mode?"<br><br>' +
                            'kb.mobiz-apps.com/KnowledgeBase?t=d2KROViIse&a=nEgGZOzbBH')
                    )
                return;
            }

            if (vDine.DiningArea.length > 0) {
                for (var i = 0; i < vDine.DiningArea.length; i++) {                    
                    var tabName = 'tab' + (vDine.DiningArea[i].DiningAreaName).replace(/\s/g, "") + vDine.DiningArea[i].OrderNo;
                    var parentTabContent = $('<div id="' + tabName + '" custom-tab-role="content" style="display: ' + (ctrDiningAreaTab == 1 ? 'block' : 'none') + '">')
                    var arrTables = vDine.DiningArea[i].Tables;
                    var ctrTables = 1;
                    var gridTable = $('<div class="ui-grid-c">');

                    lstDiningArea.append($('<li>')
                            .attr('custom-tab-content', '#' + tabName)
                            .addClass((ctrDiningAreaTab == 1 ? 'active' : ''))
                            .text(vDine.DiningArea[i].DiningAreaName)
                        )

                    if (arrTables.length > 0) {
                        for (var t = 0; t < arrTables.length; t++) {
                            var blockTable = $('<div>');

                            if (ctrTables == 5) {
                                parentTabContent.append(gridTable);
                                gridTable = $('<div class="ui-grid-c">')
                                ctrTables = 1;
                            }

                            if (ctrTables == 1) {
                                blockTable.addClass('ui-block-a');
                            } else if (ctrTables == 2) {
                                blockTable.addClass('ui-block-b');
                            } else if (ctrTables == 3) {
                                blockTable.addClass('ui-block-c');
                            } else if (ctrTables == 4) {
                                blockTable.addClass('ui-block-d');
                            }                            

                            var pOccupiedClass = '';
                            if (CBool(arrTables[t].IsOccupied))
                                pOccupiedClass = ' tableOccupied';
                            blockTable.css('position', 'relative')
                                .append($('<button data-role="none" class="tableAvailable' + pOccupiedClass +'">')
                                    .attr('tableID', arrTables[t].ID)
                                    .attr('tablename', arrTables[t].InternalTableName)
                                    .removeData('tableinfo')
                                    .data('tableinfo', arrTables[t])
                                    .text(arrTables[t].TableName)
                                    .click(function (e) {
                                        e.preventDefault();
                                        e.stopPropagation();
                                        var data = $(this).data();
                                        var badgeno = $('[badgeid=' + data.tableinfo.ID + ']').text();

                                        if ($(this).hasClass('tableMerge')) {
                                            WebToast('Please select first table to merge', 'top');
                                            $(this).removeClass('tableMerge');
                                            mSplitMergeBillMode = 2;
                                            return;
                                        }

                                        if ($(this).hasClass('tableOccupied')) {
                                            if (mTransferTableMode) {
                                                if (CStr2(data.tableinfo.InvoiceID) == '') {
                                                    NewMessageBox('You may not transfer a non-parent table of joined tables. ref: jsHQKXKDQNVP!');
                                                    return;
                                                }
                                                mTransferTableFrom = data;
                                                $('#spanTransferTableInstruction').text('Click a table to transfer to.');
                                                ShowOccupiedTable('hide');
                                                ShowVacantTable('show');
                                            } else if (mSplitMergeBillMode == 1) {
                                                if (CStr2(data.tableinfo.InvoiceID) == '') {
                                                    NewMessageBox('You may not split bill from an empty table.');
                                                    return;
                                                }
                                                mSplitMergeBillMode = 0;
                                                mSplitBillToID = data.tableinfo.InvoiceID;
                                                $('#divMainHeader').pageshow();
                                                require(['promise!/Scripts/sources/ERP/SplitBill/controller.js'], function() {
                                                    if (mSplitBill == null)
                                                        mSplitBill = ERP.SplitBill.controller();
                                                    mSplitBill.popup({
                                                        ParentTableID: data.tableinfo.ParentID || data.tableinfo.ID,
                                                        InvoiceID1: data.tableinfo.InvoiceID,
                                                        AfterSave: function() {
                                                            DisplayDiningArea(TableView.GetTableTemplate(sessionStorage.FowstID));
                                                            GetJoinedTable();
                                                        }
                                                    });
                                                });
                                            } else if (mSplitMergeBillMode == 2) {
                                                mSplitBillFromID = data.tableinfo.InvoiceID;
                                                $('#spanSplitBillInstruction').text('Select the table to split/merge bill to.');
                                                mSplitMergeBillMode = 3;
                                                $(this).addClass('tableMerge');
                                                WebToast('Please select second table to merge', 'top');
                                            } else if (mSplitMergeBillMode == 3) {
                                                mSplitMergeBillMode = 0;
                                                mSplitBillToID = data.tableinfo.InvoiceID;
                                                $('#divMainHeader').pageshow();
                                                $('.tableMerge').removeClass('tableMerge');
                                                ShowJoinedTable('hide');
                                                require(['promise!/Scripts/sources/ERP/SplitBill/controller.js'], function() {
                                                    if (mSplitBill == null)
                                                        mSplitBill = ERP.SplitBill.controller();
                                                    mSplitBill.popup({
                                                        InvoiceID1: mSplitBillFromID,
                                                        InvoiceID2: mSplitBillToID
                                                    });                                                    
                                                });
                                            } else
                                                GoToCart(data);
                                        } else {
                                            if (isSplitTable == true) {
                                                SplitJoinedTables(data.joinedData);
                                            } else if (mTransferTableMode) {
                                                SalesMenu_offline().TransferTable(mTransferTableFrom.tableinfo.InvoiceID, data.tableinfo.InternalTableName);
                                                mTransferTableMode = false;
                                                ShowVacantTable('hide');
                                                $('#instructionTransferTable').hide();
                                                $('#divMainHeader').pageshow();
                                                ShowOccupiedTable('hide');
                                                DisplayDiningArea(TableView.GetTableTemplate(sessionStorage.FowstID));
                                                GetJoinedTable();
                                            } else {
                                                $('#popupTotalNumberOfCustomer').popup('open');
                                                $('#txtTotalNumberOfCustomer').focus();
                                                
                                                $('#btnSaveTotalNumberOfCustomer')
                                                    .unbind('click')
                                                    .click(function () {
                                                        if (CInt2($('#txtTotalNumberOfCustomer').val()) > 0) {                                                            
                                                            sessionStorage.TotalNumberOfCustomer = $('#txtTotalNumberOfCustomer').val();
                                                            $('#popupTotalNumberOfCustomer').popup('close');
                                                            GoToCart(data);
                                                        } else {
                                                            NewMessageBox('Please enter number of guest(diner)', 'Table View',
                                                                [
                                                                    {
                                                                        Text: 'Ok',
                                                                        Click: function () {
                                                                            NewMessageBoxClose();
                                                                            $('#popupTotalNumberOfCustomer').popup('open');
                                                                        }
                                                                    }
                                                                ]
                                                                );
                                                            
                                                            return;
                                                        }                                                        
                                                    });

                                                if ($.trim(badgeno) == '') {
                                                    $('#lblTableNo').html('Table No.: <b>' + data.tableinfo.TableName + '</b>')
                                                } else {
                                                    $('#lblTableNo').html('Group Table No.: <b>' + badgeno + '</b><br>' +
                                                            'Table No: ' + data.tableinfo.TableName
                                                        )
                                                }
                                            }                                            
                                        }                                        
                                    })
                                ).append($('<div class="tableChecklist">')
                                    .data('tableinfo', arrTables[t])
                                    .append($('<div class="custom-checkbox">')
                                        .append($('<label for="chb-' + arrTables[t].ID + '" class="checkbox-only">')
                                            .append($('<input type="checkbox" id="chb-' + arrTables[t].ID + '" data-iconpos="notext">')
                                                .removeData('tableinfo')
                                                .data('tableinfo', arrTables[t])
                                                .unbind('click')
                                                .click(function (e) {
                                                    e.stopPropagation();
                                                    e.preventDefault();                                                    
                                                    AddJoinTables($(this).data('tableinfo'));
                                                })
                                            )
                                        )
                                    )
                                    .click(function (e) {
                                        e.stopPropagation();
                                        e.preventDefault();
                                        var chb = $(this).find('input[type=checkbox]');

                                        if(chb.is(':checked')){
                                            chb.attr('checked', false).checkboxradio('refresh');
                                        } else {
                                            chb.attr('checked', true).checkboxradio('refresh');
                                        }
                                        
                                        AddJoinTables($(this).data('tableinfo'));
                                    })
                                )
                                .append($('<div class="tableBadge">').attr('badgeID', arrTables[t].ID))
                                .append($('<div class="tableDisable">'))
                                .append($('<div style="text-align:center; white-space:nowrap; overflow:hidden; text-overflow:ellipsis; width:100%;">').text((CBool(arrTables[t].IsOccupied) ? arrTables[t].CustomerName : '')))

                            if (t == (arrTables.length - 1)) {
                                //used for the new row
                                //append the new block(s) even though not equal to 4 blocks
                                parentTabContent.append(gridTable);
                            }

                            gridTable.append(blockTable)
                            ctrTables += 1;
                        }
                        
                        $('input[type="checkbox"]').checkboxradio().checkboxradio('refresh')
                    }

                    $('#divTabContent').append(parentTabContent);

                    ctrDiningAreaTab += 1;
                }

                $('div[custom-tab-role="content"]:first').show();
                setTimeout(function () {
                    $('#divTabContent').resize();
                }, 500)
            } else {
                $('#divTabContent').append($('<h3>')
                        .html('No Table Setup found. <br>')
                    )
                    .append($('<span>')
                        .html('To create table setup for this location (FOWST), go to System Administration > Table View Settings or just copy the link below and paste it to your browser and follow the instructions. <br><br>' +
                            'Link Description: "How do I set up my dining area and tables in Mobiz POS > Table View mode?"<br><br>' +
                            'kb.mobiz-apps.com/KnowledgeBase?t=d2KROViIse&a=nEgGZOzbBH')
                    )
            }
        }

        function GoToCart(data) {
            var pLocation;

            if (isSplitTable == true) {
                SplitJoinedTables(data.joinedData)
            } else {
                var tableParentNo = data.tableinfo.InternalTableName;
                var tableSet = data.joinedData;

                if (tableSet != undefined) {
                    for (var i = 0; i < tableSet.length; i++) {
                        if (CBool(tableSet[i].IsParent)) {
                            tableParentNo = tableSet[i].InternalTableName;
                        }
                    }
                }

                if ($(window).width() > proj.WideWidth) { // 932 old . force non-widescreen
                    pLocation = MergeUrlParameters({
                        OrgID: sessionStorage["OrgID"],
                        mode: 'pos',
                        IsMobile: IsMobile ? 1 : 0,
                        TableNo: tableParentNo
                    }, '/SalesMenu/POSViewAll');
                } else {
                    //redirect to retail mode first
                    pLocation = MergeUrlParameters({
                        OrgID: sessionStorage["OrgID"],
                        mode: 'pos',
                        IsMobile: IsMobile ? 1 : 0,
                        TableView: 'true',
                        TableNo: tableParentNo
                    }, '/SalesMenu');
                }

                if (window.InAndroid) {
                    Android.SetSessionStorage({
                        WebViewIndex: 2
                    });
                    var pJson = {
                        TableNo: tableParentNo
                    };
                    pJson = JSON.stringify(pJson);
                    pJson = 'proj.SalesMenuInit(' + pJson + ')';
                    Android.ScriptToWebView({
                        Script: pJson,
                        WebViewIndex: 2
                    });
                    Android.SetCurrentTab(1);
                } else
                    location = pLocation;
            }
        }

        function SplitJoinedTables(data) {
            var listOfSelectedJoinedTables = $('#listOfSelectedJoinedTables');
            arrRemoveTable = [];
            ctrRemoveJoinedTable = 0;

            arrlistofsplittable = data;

            listOfSelectedJoinedTables.empty();

            for (var i = 0; i < data.length; i++){
                listOfSelectedJoinedTables.append($('<li data-icon="delete">')
                        .data('tableinfo', data[i])
                        .append($('<a>')
                            .text(data[i].TableName)
                        )
                        .append($('<a>')
                            .data('tableinfo', data[i])
                            .text(data[i].TableName)
                            .click(function () {
                                if (CBool($(this).data('tableinfo').IsOccupied) && CBool($(this).data('tableinfo').IsParent)) {
                                    NewMessageBox('We cannot remove this ' + $(this).data('tableinfo').TableName +
                                                ' from this table joining because the billing ' +
                                                ' payment for this group is based on table ' +
                                                $(this).data('tableinfo').TableName + '.<br><br>' +
                                                'To remove ' + $(this).data('tableinfo').TableName +
                                                ' from this grouping, please settle the bill for this group first.', 'Message',
                                                [
                                                    {
                                                        Text: 'Ok',
                                                        Click: function () {
                                                            NewMessageBoxClose();
                                                            $('#popSplitJoinedTables').popup('open');
                                                        }
                                                    }
                                                ])
                                }
                                else {
                                    if ($('#listOfSelectedJoinedTables li').length == 2) {
                                        NewMessageBox('If you remove this table, the remaining one table will also be removed from joined table set. Are you sure you want to continue?', 'Message',
                                            [
                                                {
                                                    Text: 'Yes',
                                                    Click: function () {
                                                        $('#listOfSelectedJoinedTables li').each(function () {
                                                            $(this).data('tableinfo').State = 'removed';
                                                            ctrRemoveJoinedTable += 1;
                                                            arrRemoveTable.push($(this).data('tableinfo'));
                                                        })
                                                        ctrJoinedTable -= 1;
                                                        RemoveTable();
                                                    }
                                                }, {
                                                    Text: 'No', Click: function () {
                                                        NewMessageBoxClose();
                                                        $('#popSplitJoinedTables').popup('open');
                                                    }
                                                }
                                            ]
                                        )
                                    } else {
                                        ctrRemoveJoinedTable += 1;
                                        $(this).parent().data('tableinfo').State = 'removed';
                                        //arrRemoveTable.push($(this).data())
                                        $(this).parent().removeAttr('class').text('Removed');
                                    }
                                }
                            })
                        )
                    )
            }
            listOfSelectedJoinedTables.listview('refresh')
            $('#popSplitJoinedTables').popup('open');
        }

        function AddJoinTables(data) {
            var isExist = false;

            data.JoinedID = NewID(); // row id in LibraryTableJoined
            data.TableSetID = tableSetID;
            data.State = 'add';
            data.IsOccupied = false;
            data.IsParent = false;

            if(arrToJoinTables.length > 0){
                $.grep(arrToJoinTables, function (e) {
                    if (e.ID == data.ID) {
                        isExist = true;
                    }
                })
                if (isExist == false) {
                    arrToJoinTables.push(data);
                } else {
                    arrToJoinTables = $.grep(arrToJoinTables, function (e) {
                        return e.ID != data.ID;
                    })
                }
            } else {
                arrToJoinTables.push(data);
            }
        }

        function ResetCheckList(display) {
            if (display == 'show') {
                $('.tableChecklist').prev().each(function () {
                    if ($(this).hasClass('joined') == false && $(this).hasClass('tableOccupied') == false) {
                        $(this).next().show().find('input[type=checkbox]').attr('checked', false).checkboxradio().checkboxradio('refresh');
                    }
                    else {
                        $(this).next().next().next().show()
                    }
                })                
            } else {
                $('.tableChecklist').hide().removeClass('tableDisable').find('input[type=checkbox]').attr('checked', false).checkboxradio().checkboxradio('refresh');
                $('.tableDisable').hide()
            }            
        }
        
        function ColorBadge() {
            var isSelectedAll = false;
            var selectedColorIndex;
            var noOfColor = arrColorBadge.length;

            var selectedColorIndex = ((ctrJoinedTable / noOfColor) - (CInt2(ctrJoinedTable / noOfColor))) * noOfColor;

            //for (var i = 0; i < arrColorBadge.length; i++){
            //    if (arrColorBadge[i].selected == true) {
            //        isSelectedAll = true;
            //        selectedColorIndex = 0;                    
            //    } else {
            //        isSelectedAll = false;
            //        selectedColorIndex = i;
            //        arrColorBadge[selectedColorIndex].selected = true;
            //        break;
            //    }
            //}

            //if (isSelectedAll) {
            //    for (var m = 0; m < arrColorBadge.length; m++) {
            //        arrColorBadge[m].selected = false;
            //    }
            //    arrColorBadge[selectedColorIndex].selected = true;
            //}            

            return arrColorBadge[Math.round(selectedColorIndex)].color;
        }

        function ClearVariables() {
            arrFowst = [];
            tableSetID = "";
            isSplitTable = false;
            arrToJoinTables = [];
            ctrJoinedTable = 0;
            arrJoinedTable = [];
            arrRemoveTable = [];
            arrlistofsplittable = [];
        }

        function SortByTableNo() {
            arrToJoinTables.sort(function (a, b) {
                var a1 = CInt2(a.OrderNo), b1 = CInt2(b.OrderNo);
                if (a1 == b1) return 0;
                return a1 > b1 ? 1 : -1;
            });
        }
    }) 
});