define(['common'
], function () {

    var mDeferred = $.Deferred();

    $(function () {

        CreateStringObject('App.controller.Printer', null, function () {

            var mOut = {};
            var mCbPrinterBrand;
            var mCbPrinterModel;
            var mTxtPrinterAddress;
            var mTxtPrinterName;
            var mCbPrinterFowst;
            var mCbPrinterInterfaceType;
            var mButPrinterSave;
            var mCbPrinterRole;
            var mDivPrinterCategories;
            var mTxtPrinterSerialNumber;
            var btnPrinterAssignFowst;
            var arrAssignedFowst = [];

            mOut.Init = function() {
                mCbPrinterBrand = $('#cbPrinterBrand');
                mCbPrinterModel = $('#cbPrinterModel');
                mTxtPrinterAddress = $('#txtPrinterAddress');
                mTxtPrinterName = $('#txtPrinterName');
                mCbPrinterFowst = $('#cbPrinterFowst');
                mCbPrinterInterfaceType = $('#cbPrinterInterfaceType');
                mButPrinterSave = $('#butPrinterSave');
                mCbPrinterRole = $('#cbPrinterRole');
                mDivPrinterCategories = $('#divPrinterCategories');
                mTxtPrinterSerialNumber = $('#txtPrinterSerialNumber');
                btnPrinterAssignFowst = $('#btnPrinterAssignFowst');

                mButPrinterSave.unbind().click(Save);

                $('#butPrinterCancel').unbind().click(function() {
                    window.location.hash = 'LibraryPage';
                });

                btnPrinterAssignFowst.unbind().click(function () {
                    BrowsePopup('LibraryFowst', 'Select FOWST', [
                        {
                            StyleFormat: 'table',
                            DisplayText: [{ Label: 'Name', Column: 'FowstName' }],
                            AutoSearch: true,
                            AllowToSelectAll: true,
                            SelectMultiple: true,
                            Message: 'Note: You can select multiple records. Click OK when you\'re done.',
                            Click: function (fowst) {
                                Bs.Loading('show');

                                for (var i in fowst) {
                                    var data = fowst[i], isExist = false, index = null;

                                    $.grep(arrAssignedFowst, function (e, i) {
                                        if (e.ID == data.ID) {
                                            isExist = true;
                                            index = i;
                                        }
                                    })

                                    if (isExist) {
                                        if (arrAssignedFowst[index].Status == 'remove') arrAssignedFowst[i].Status = 'old';
                                    } else {
                                        arrAssignedFowst.push(data);
                                    }
                                }

                                var visibleFowst = [];
                                for (var i in arrAssignedFowst) {
                                    if (arrAssignedFowst[i].Status != 'remove') visibleFowst.push(arrAssignedFowst[i]);
                                }

                                PopAssignedFowst(visibleFowst);
                            }
                        }
                    ], { IsAll: 1 })
                })

                mCbPrinterRole.unbind().change(function () {
                    if ($(this).find('option:selected').text() == 'POS/ General Purpose Printer') {
                        $('#divPrinterCategories').hide();
                    } else $('#divPrinterCategories').show();
                })
            };

            mOut.LoadData = function (vParams) {
                if (CBool(vParams.IsNewSetup)) {
                    $('#divOldSetup').hide();
                    $('#divNewSetup').show();
                } else {
                    $('#divOldSetup').show();
                    $('#divNewSetup').hide();
                }

                GetData({
                    ID: vParams.ID,
                    success: function (vData) {
                        arrAssignedFowst = (vData.AssignedFowst == undefined ? [] : vData.AssignedFowst.Rows);
                        PopFowst(vData.Fowst);
                        
                        vData.Master = $.extend({}, vData.Master);
                        mTxtPrinterAddress.val(vData.Master.PrinterIPAddress);
                        mTxtPrinterName.val(vData.Master.PrinterName);

                        mCbPrinterBrand.find('option:contains(' + vData.Master.Brand + ')').prop('selected', true);

                        mCbPrinterModel.find('option:contains(' + vData.Master.Model + ')').prop('selected', true);
                        mCbPrinterModel;//.selectmenu('refresh');

                        mCbPrinterRole.find('>option:contains(' + vData.Master.PrinterRole + ')').prop('selected', true);
                        mCbPrinterRole;//.selectmenu('refresh');

                        mCbPrinterFowst.find('option[value=' + vData.Master.FowstID + ']').prop('selected', true);
                        mCbPrinterFowst;//.selectmenu('refresh');

                        mCbPrinterInterfaceType.find('option:contains(' + vData.Master.ConnectionType + ')').prop('selected', true);
                        mCbPrinterInterfaceType;//.selectmenu('refresh');

                        mTxtPrinterSerialNumber.val(vData.Master.SerialNum);

                        PopAssignedFowst(arrAssignedFowst);
                        PopCategories(vData.Categories);
                    }
                });
            };

            var tblPrinterAssignedFowst;
            function PopAssignedFowst(data) {
                tblPrinterAssignedFowst = $('#tblPrinterAssignedFowst');

                if ($.fn.DataTable.isDataTable(tblPrinterAssignedFowst)) {
                    //empty the table first before destroying datatabase para dili magdoble ang header
                    tblPrinterAssignedFowst.empty();
                    tblPrinterAssignedFowstDT.destroy();
                }

                tblPrinterAssignedFowstDT = tblPrinterAssignedFowst.DataTable({
                    pageLength: 10,
                    "ordering": false,
                    "data": data,
                    "initComplete": function () {
                        $('#tblPrinterAssignedFowst').removeClass('dataTable').css('width', '100%');
                        Bs.Loading('hide');

                        $('#txtPrinterFowstSearch').unbind('keyup').keyup(function () {
                            tblPrinterAssignedFowstDT.search($(this).val()).draw();
                        })
                    },
                    rowCallback: function (row, data, displayNum, displayIndex, dataIndex) {
                        data.Status = 'old';
                    },
                    "columns": [
                        {
                            "data": function (data) {
                                return data.FowstName;
                            }
                        },
                        {
                            "data": function (data) {
                                return "<button data-role='none' class='btn btn-danger'><span class='bi bi-trash'></span></button>";
                            },
                            createdCell: function (td, cellData, rowData, row, col) {
                                var btn = $(td).find('button');

                                rowData.RowIndex = row;

                                btn.data('data', rowData).unbind('click').click(function () {
                                    rowData.Status = 'remove';

                                    var visibleFowst = [];
                                    for (var i in arrAssignedFowst) {
                                        if (arrAssignedFowst[i].Status != 'remove') visibleFowst.push(arrAssignedFowst[i]);
                                    }

                                    PopAssignedFowst(visibleFowst);
                                })
                            }, className: 'text-right'
                        }
                    ],
                    "sDom": '<"top">rt<"bottom"ip><"clear">'
                });
            }

            function PopFowst(vData) {
                var pDR;
                mCbPrinterFowst.empty();
                for (var p = 0; p <= vData.Rows.length - 1; p++) {
                    pDR = vData.Rows[p];
                    var pOption = new Option(pDR.Name, pDR.ID);
                    mCbPrinterFowst.append(pOption);
                }
            }

            function PopCategories(vData) {
                var pCheckBox;
                var pDR;

                mDivPrinterCategories.empty();
                for (var p = 0; p <= vData.Rows.length - 1; p++) {
                    pDR = vData.Rows[p];
                    if (CStr2(pDR.Category).trim() == '')
                        continue;
                    pCheckBox = $('<div class="form-check mb-3" style="font-size:20px;">')
                                .append($('<input type="checkbox" class="form-check-input" data-role="none">')
                                    .attr('id', 'chk' + p)
                                    .prop('checked', CInt2(pDR.Included))
                                    .data('data', pDR)
                                ).append($('<label class="form-check-label">')
                                    .attr('for', 'chk' + p)
                                    .text(pDR.Category)
                                )

                    mDivPrinterCategories.append(pCheckBox);
                }
                $('input:checkbox').checkboxradio();
            }

            function GetData(vParams) {
                AjaxSend({
                    url: 'Organization/GetPrinterByDevicePrinterFowstID',
                    data: {
                        ID: vParams.ID,
                        IsNewSetup: sessionStorage.IsNewSetup
                    },
                    success: function(vData) {
                        vParams.success(vData);
                    }
                });
            }

            function Save() {
                var pData = {
                    ID: sessionStorage['ViewID'],
                    FowstID: mCbPrinterFowst.find('option:selected').prop('value'),
                    PrinterName: mTxtPrinterName.val(),
                    PrinterIPAddress: mTxtPrinterAddress.val(),
                    ConnectionType: mCbPrinterInterfaceType.find('option:selected').prop('text'),
                    PrinterRole: mCbPrinterRole.find('option:selected').prop('text'),
                    Brand: mCbPrinterBrand.find('option:selected').prop('text'),
                    Model: mCbPrinterModel.find('option:selected').prop('text'),
                    Categories: GetCheckedCategories(),
                    SerialNum: mTxtPrinterSerialNumber.val(),
                    IsNewSetup: sessionStorage.IsNewSetup,
                    AssignedFowst: arrAssignedFowst
                };

                AjaxSend({
                    url: '/Organization/SavePrinterByDevicePrinterFowst',
                    data: pData,
                    success: function(vData) {
                        vData = $.extend({}, vData);
                        if (vData.Error) 
                            NewMessageBox(vData.Error);
                        else if (IsGuid(vData.ID)) {
                            WebToast('Printer was successfully saved.');
                            window.location.hash = 'LibraryPage';
                            LibraryTable.Printers();
                        }
                    }
                });

                function GetCheckedCategories() {
                    var pOut = [];
                    var pChecked = mDivPrinterCategories.find('input:checkbox:checked');

                    for (var p = 0; p <= pChecked.length - 1; p++) {
                        pOut.push($(pChecked[p]).data('data').Category);
                    }

                    return pOut;
                } // END function GetCheckedCategories() {
            } // END function Save() {

            return mOut;

        }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    }); // END $(function () {

    return mDeferred.promise();
});