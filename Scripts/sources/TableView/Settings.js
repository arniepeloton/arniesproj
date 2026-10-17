require(['/Scripts/jquery-ui-sortable.js'],
function () {
    $(document).ready(function () {
        var arrlocation = [], arrDiningArea = [], arrAllFowst = [];
        var isNewDiningArea = true, indexDiningArea = 0, ctrDiningArea = 0, curIndex = 0, isNewTemplate = true,
            isEditDine = true, ctrTableNo = 1;

        ClearVariables();        

        window.TableView = {
            LoadData: function(){
                ClearVariables();
            }
        }
        
        $('body')
        .on('click', '#btnAddTableFowst', function () {
            BrowsePopup('/TableView/GetAllFowst', 'Select FOWST Location', [
                {
                    Message: 'You can add the assigned location to this template but the selected assigned location will remove to its assigned template.',
                    AutoSearch: true,
                    DisplayText: [{
                        Column: function (e) {
                            var text = (CBool(e.IsAssigned) ? 'Assigned' : 'Not Assign');
                            var color = (CBool(e.IsAssigned) ? 'c32222' : '28a745');
                            return e.FowstName + ' <span style="color:#' + color + '; font-weight: normal; text-shadow: none;">(' + text + ')</span>'
                        }
                    }],
                    Validation: function(e){
                        var isexist = false;

                        //Check first if there's exist fowst on a lists
                        $('#lstTableFowst a').each(function () {
                            if ($(this).data().ID == e.ID) {
                                isexist = true;
                                return;
                            }
                        });

                        if (isexist) {
                            WebToast(e.FowstName + ' was already exist.', 'top')
                        } else {
                            return true
                        }
                    },
                    Click: function (e) {
                        if (CBool(e.IsAssigned)) {
                            NewMessageBox('Are you sure you want to add this assigned location to this template? If yes, it will remove to its assigned template.', 'Warning',
                                [
                                    {
                                        Text: 'Yes',
                                        Click: function () {
                                            e.IsNew = true;
                                            e.State = 'update'; // remove means will change the default template to current template
                                            arrlocation.push(e);
                                            DisplayLocation();
                                        }
                                    }, {
                                        Text: 'No', Click: function () {
                                            PopupButtonList(msg, 'Location', fowst)
                                        }
                                    }
                                ]
                            )
                        } else {
                            e.IsNew = true;
                            e.State = 'new';
                            arrlocation.push(e);
                            DisplayLocation();
                        }
                    }
                }
            ])
        })
        .on('click', '#btnAddDiningArea', function () {
            $('#popDiningArea').modal('show');
            ClearDiningFields();
        })
        .on('keydown', '#txtDiningNo', function (e) {
            if(!NumberOnly(e)){
                return;
            }
        })
        .on('click', '#btnOkDiningArea', function () {
            var txtDiningAreaName = $.trim($('#txtDiningAreaName').val());
            var txtDiningNo = CDec2($.trim($('#txtDiningNo').val()));
            var txtDiningPrefix = $.trim($('#txtDiningPrefix').val());
            var arrTableNo = [];
            var isExistAreaName = false, isExistPrefix = false;

            if(txtDiningAreaName == ""){
                WebToast('Please enter dining area name', 'top');
                return;
            }

            if (txtDiningNo == "" || txtDiningNo == 0) {
                WebToast('Please enter number of tables in dining area', 'top');
                return;
            }            

            arrTableNo = GenerateTableNoArray(txtDiningNo, txtDiningPrefix, true);

            if (isNewDiningArea) {
                for (var i = 0; i < arrDiningArea.length; i++) {
                    if ((arrDiningArea[i].AreaName).toLowerCase() == txtDiningAreaName.toLowerCase()) {
                        isExistAreaName = true;
                    }

                    if(txtDiningPrefix != ""){
                        if ((txtDiningPrefix).toLowerCase() == (arrDiningArea[i].Prefix).toLowerCase()) {
                            isExistPrefix = true
                        }
                    }
                }

                if (isExistAreaName) {
                    WebToast('Dining area name already exist.', 'top');
                    return;
                }

                if (isExistPrefix) {
                    WebToast('Table Prefix label already exist.', 'top');
                    return;
                }

                ctrDiningArea += 1;

                arrDiningArea.push({
                    No: ctrDiningArea,
                    AreaName: txtDiningAreaName,
                    NoOfTable: txtDiningNo,
                    Prefix: txtDiningPrefix,
                    Table: arrTableNo,
                    State: 'new'
                })                
            } else {
                arrDiningArea[indexDiningArea].AreaName = txtDiningAreaName;
                arrDiningArea[indexDiningArea].NoOfTable = txtDiningNo;
                arrDiningArea[indexDiningArea].Prefix = txtDiningPrefix;
                arrDiningArea[indexDiningArea].Table = arrTableNo;
            }

            DisplayDiningArea();
            $('#popDiningArea').modal('hide');
        })
        .on('click', '#btnOkTable', function () {            
            var newTblName = $.trim($('#txtTableName').val());

            if (newTblName == "") {
                WebToast('Please enter table name');
                return;
            }

            if(isEditDine){
                var liDine = $('#d' + curIndex);
                liDine.data().AreaName = newTblName;
                liDine.find('> span').text(newTblName + ' ');
            } else {
                var liTbl = $('#tbl' + curIndex);
                liTbl.data().Name = newTblName;
                liTbl.find('> span').text(newTblName + ' ');
            }            
            $('#popTable').modal('hide');
        })
        .on('click', '#btnDiningCancel', function () {
            window.location.hash = 'LibraryPage';
        })
        .on('click', '#btnDiningSave', function () {
            SaveTemplate();
        })
        .on('click', '#btnDiningRemove', function () {
            NewMessageBox('Are you sure you want to remove the entire template? If yes, the template would be deleted permanently.', 'Warning',
                    [
                        {
                            Text: 'Yes',
                            Click: function () {
                                $('#btnDiningRemove').attr('disabled', true);
                                AjaxSend({
                                    url: 'TableView/RemoveTemplate',
                                    data: { ID: $('#txtTemplateID').val() },
                                    success: function (vData) {
                                        $('#btnDiningRemove').removeAttr('disabled');
                                        if (vData.Results == "removed") {
                                            window.location.hash = 'LibraryPage';
                                            LibraryTable.TableView();
                                        } else {
                                            NewMessageBox(vData.Results)
                                        }
                                    }
                                })
                            }
                        }, { Text: 'No' }
                    ]
                )            
        })
        .on('click', '#btnOkNewTable', function () {
            var newTblName = $.trim($('#txtNewTableName').val());

            if (newTblName == "") {
                WebToast('Please enter table name');
                return;
            }

            var liDine = $('#d' + curIndex);
            ctrTableNo = liDine.data().Table.length + 1;
            var newTableArr = {
                No: ctrTableNo,
                Name: newTblName,
                State: 'new',
                DineNo: liDine.data().No
            }
            liDine.data().Table.push(newTableArr);

            $('#' + curIndex).append(MakeListTableNo(newTableArr))
            $('#popNewTable').modal('hide');
        })

        function SaveTemplate() {
            var txtTableViewTemplate = $.trim($('#txtTableViewTemplate').val());
            var txtTemplateID = $.trim($('#txtTemplateID').val());
            var pData = [], newArrDine = [], newOrderNo = 0;
            var hasTable = false, dineName = '';

            if (txtTableViewTemplate == "") {
                WebToast('Please enter template name', 'top');
                return;
            }

            if(arrlocation.length == 0){
                NewMessageBox('Please add at least one location.', 'Message');
                return;
            }            

            $('#lstDiningArea > li').each(function () {
                var tempArrDine = $(this).data();                
                tempArrDine.Table = [];
                tempArrDine.sortableItem = "";
                dineName = tempArrDine.AreaName;
                hasTable = false;

                $(this).find('ol > li').each(function () {
                    var tempArrTable = $(this).data();
                    tempArrTable.sortableItem = "";
                    tempArrDine.Table.push(tempArrTable);
                    hasTable = true;
                })

                if (tempArrDine.State == 'remove') {
                    hasTable = true; // if the dining area is to be removed set to True to bypass the validation below.
                }

                if (tempArrDine.State != 'remove') {
                    newOrderNo += 1; // update the NO of unremoved table
                }
                
                tempArrDine.No = newOrderNo;

                newArrDine.push(tempArrDine)
            })

            if (hasTable == false) {
                NewMessageBox('Opps! We found out that the dining area named \'<b>' + dineName + '</b>\' has no tables. Please add at least one table or just delete the entire \'' + dineName + '\' dining area.', 'Warning');
                return;
            }

            if (arrDiningArea.length == 0 || $('#lstDiningArea > li:visible').length == 0) {
                NewMessageBox('Please add at least one dining area.', 'Message');
                return;
            }

            pData.ID = txtTemplateID;
            pData.TemplateName = txtTableViewTemplate;
            pData.Location = arrlocation;
            pData.Dine = newArrDine;
            pData.State = isNewTemplate;

            $('#btnDiningSave').attr('disabled', true);
            AjaxSend({
                url: 'TableView/SaveTemplate',
                data: pData,
                success: function (vData) {
                    $('#btnDiningSave').removeAttr('disabled');
                    if(vData.Results == "saved"){
                        window.location.hash = 'LibraryPage';
                        LibraryTable.TableView();
                    } else {
                        NewMessageBox(vData.Results, 'Message')
                    }
                }
            })
        }

        function DisplayDiningArea() {
            var lstDiningArea = $('#lstDiningArea');

            //lstDiningArea.empty();

            SortList('lstDiningArea');

            for (var i = 0; i < arrDiningArea.length; i++) {
                arrDiningArea[i].Index = i;
                //$('#d' + arrDiningArea[i].No).length -> there must be no existing dining area in a list
                if (arrDiningArea[i].State != 'remove' && $('#d' + arrDiningArea[i].No).length == 0) {
                    var olTable = $('<ol id="' + arrDiningArea[i].No + '" class="draggableList mt-3" style="display:none;">');
                    var liDining = $('<li class="list-group-item">')
                                        .data(arrDiningArea[i])
                                        .attr('id', 'd' + arrDiningArea[i].No)
                                        .append($('<button data-role="none" class="btn" style="font-weight:bold;">')
                                            .text(arrDiningArea[i].AreaName + ' ')
                                        )
                                        .append($('<a ol="' + arrDiningArea[i].No + '" class="btn btn-light">').text('Show Table')
                                            .click(function () {
                                                if ($(this).text() == 'Show Table') {
                                                    $('#' + $(this).attr('ol')).show()
                                                    $(this).text('Hide Table')
                                                } else {
                                                    $('#' + $(this).attr('ol')).hide()
                                                    $(this).text('Show Table')
                                                }
                                            })
                                        )
                                        .append(' | ')
                                        .append($('<a class="btn btn-light">')
                                            .data(arrDiningArea[i])
                                            .text('Add' + ' ')
                                            .click(function () {
                                                curIndex = $(this).data().No;
                                                $('#popNewTable').modal('show');
                                                $('#txtNewTableName').val('').focus();
                                            })
                                        )
                                        .append(' | ')
                                        .append($('<a class="btn btn-light">')
                                            .data(arrDiningArea[i])
                                            .text('Edit' + ' ')
                                            .click(function () {
                                                isEditDine = true
                                                curIndex = $(this).data().No;
                                                $('#popTable').modal('show');
                                                $('#titleDinePopup').text('Edit Dining Area');
                                                $('#lblDinePopup').text('New Dining Area Name');
                                                $('#txtTableName').val($(this).data().AreaName).focus();
                                            })
                                        )
                                        .append(' | ')
                                        .append($('<a class="btn btn-light">')
                                            .data(arrDiningArea[i])
                                            .text('Delete' + ' ')
                                            .click(function () {
                                                var dine = $('#d' + $(this).data().No);
                                                var ind = dine.data().No;

                                                NewMessageBox('Are you sure you want to delete this dining area?', 'Warning',
                                                    [
                                                        {
                                                            Text: 'Yes',
                                                            Click: function () {
                                                                if (dine.data().State == 'new') {
                                                                    dine.remove();
                                                                    arrDiningArea = $.grep(arrDiningArea, function (e) {
                                                                        return e.No != ind
                                                                    })
                                                                } else {
                                                                    dine.hide();
                                                                    dine.data().State = 'remove';
                                                                    arrDiningArea[CInt2(dine.data().Index)].State = 'remove';
                                                                }
                                                            }
                                                        }, { Text: 'No' }
                                                    ]
                                                )
                                            })
                                        )
                    var arrTables = arrDiningArea[i].Table;

                    SortList(arrDiningArea[i].No);

                    for (var t = 0; t < arrTables.length; t++) {
                        arrTables[t].DineNo = i;
                        olTable.append(MakeListTableNo(arrTables[t])
                            )
                    }

                    liDining.append(olTable);
                    lstDiningArea.append(liDining);
                }                
            }
        }

        function MakeListTableNo(data) {
            return $('<li class="list-group-item">')
                .data(data)
                .attr('id', 'tbl' + data.DineNo + '' + data.No)
                .html('<button class="btn" style="font-weight:bold;">' + data.Name + ' </button> ')
                .append($('<a class="btn btn-light">')
                    .data(data)
                    .text('Edit' + ' ')
                    .click(function () {
                        isEditDine = false;
                        curIndex = $(this).parent().data().DineNo + '' + $(this).parent().data().No;
                        $('#popTable').modal('show');
                        $('#titleDinePopup').text('Edit Table');
                        $('#lblDinePopup').text('New Table Name');
                        $('#txtTableName').val($(this).parent().data().Name).focus();
                    })
                )
                .append(' | ')
                .append($('<a class="btn btn-light">')
                    .data(data)
                    .text('Delete' + ' ')
                    .click(function () {
                        var data = $(this).parent().data();

                        NewMessageBox('Are you sure you want to delete this table?', 'Warning',
                            [
                                {
                                    Text: 'Yes',
                                    Click: function () {
                                        if (data.State == 'new') {
                                            $('#tbl' + data.DineNo + '' + data.No).remove();
                                        } else {
                                            $('#tbl' + data.DineNo + '' + data.No).hide();
                                            data.State = 'remove';
                                        }
                                    }
                                }, { Text: 'No' }
                            ]
                        )
                    })
                )
        }

        function SortList(id) {
            setTimeout(function (e) {
                $('#' + e).sortable({
                    delay: 150,
                    start: function (event, ui) {
                        ui.item.parent().css('border', '1px solid #CCC');
                    },
                    stop: function (event, ui) {
                        ui.item.parent().css('border', '1px solid #FFF');
                    },
                    update: function (event, ui) {
                        var id = ui.item.parent().attr('id');
                        var newOrderNo = 1;
                        $('#' + id).find('> li').each(function () {
                            $(this).data().No = newOrderNo;
                            newOrderNo += 1;
                        })
                    }
                });
                //$('#' + e).disableSelection();
            }, 100, id)
        }

        function GenerateTableNoArray(no, prefix, isNew) {
            var arrTableNo = [];
            ctrTableNo = 1;

            for (var i = 0; i < no; i++){
                arrTableNo.push({
                    No: ctrTableNo,
                    Name: (prefix == "" ? ctrTableNo : prefix + '-' + ctrTableNo),
                    State: 'new'
                })
                ctrTableNo += 1;
            }

            return arrTableNo;
        }

        function ClearDiningFields() {            
            $('#txtDiningAreaName').val('').focus();
            $('#txtDiningNo').val('')
            $('#txtDiningPrefix').val('')
        }

        function DisplayLocation() {
            var lstTableFowst = $('#lstTableFowst');

            lstTableFowst.empty();

            for (var i = 0; i < arrlocation.length; i++) {
                arrlocation[i].Index = i;
                if(arrlocation[i].State != 'remove'){
                    lstTableFowst.append($('<a href="#" class="list-group-item list-group-item-action">')
                        .data(arrlocation[i])
                        .append($('<table style="width:100%;">')
                            .append($('<tr>')
                                .append($('<td style="word-break:break-all;">')
                                    .data(arrlocation[i])
                                    .text(arrlocation[i].FowstName)
                                )
                                .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                        .append($('<span class="bi-trash-fill" aria-hidden="true">'))
                                        .data(arrlocation[i])
                                        .click(function (e) {
                                            var ID = $(this).data().ID;

                                            if (CBool($(this).data().IsNew) == true) {
                                                arrlocation = $.grep(arrlocation, function (val) {
                                                    return val.ID != ID;
                                                })
                                            }
                                            else {
                                                arrlocation[$(this).data().Index].State = "remove";
                                            }

                                            DisplayLocation();
                                        })
                                    )
                                )
                            )
                        )
                    )
                }                
            }

            //lstTableFowst.listview('refresh');
        }

        function ClearVariables() {
            arrlocation = [];
            arrDiningArea = [];
            isNewDiningArea = true;
            indexDiningArea = 0;
            ctrDiningArea = 0;
            curIndex = 0;
            isNewTemplate = true;

            $('#txtTemplateID').val('');
            $('#txtTableViewTemplate').val('').focus();
            $('#lstTableFowst').empty();
            $('#lstDiningArea').empty();
            $('#btnDiningRemove').hide();
            
            GetAllFowst();

            if (sessionStorage.ViewID != undefined) {
                $('#btnDiningRemove').show();
                isNewTemplate = false;
                GetTemplateData();
            } else {
                GetUnassignedFowst();
            }
        }

        function GetTemplateData() {
            WebToast('Please wait while getting the data ...', 'top');
            AjaxSend({
                url: 'TableView/GetTemplateData',
                data: { ID: sessionStorage.ViewID },
                success: function (vData) {
                    isNewTemplate = false;
                    $('#txtTemplateID').val(vData[0].ID);
                    $('#txtTableViewTemplate').val(vData[0].TemplateName).focus();

                    arrlocation = vData[0].Location;
                    DisplayLocation();

                    ctrDiningArea = vData[0].Dine.length;
                    arrDiningArea = vData[0].Dine;
                    DisplayDiningArea();                    
                }
            })
        }

        function GetUnassignedFowst() {
            AjaxSend({
                url: 'TableView/GetUnassignedFowst',
                success: function (vData) {
                    if (sessionStorage.ViewID == undefined) {
                        arrlocation = vData.Results.Rows;
                        DisplayLocation();
                    }
                }
            })
        }

        function GetAllFowst() {
            AjaxSend({
                url: 'TableView/GetAllFowst',
                success: function (vData) {
                    arrAllFowst = vData.Results.Rows;
                }
            })
        }
    })
})