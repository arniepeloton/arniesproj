define([
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/PriceBulletin/settings/view.html?v' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.PriceBulletin.settings.controller', null, function () {

        var mOut = {}, mElement, ctrProductList = 1, arrProductList = [], bulletinID = '';

        mElement = $(mHtml);
        mOut.element = mElement;
        mOut.ItemsPerPage = 5;
        mOut.PageChangeDelay = 5;

        $('#tabPriceBulletinSettings').empty().append(mElement);

        var popupBulletin = mElement.find('#popupBulletin');
        var tbody = popupBulletin.find('tbody');

        mElement.find('[name="btnPBSaveGeneralSettings"]').click(function () {
            AjaxSend({
                url: '/Revenue/SavePriceBulletinGeneralSettings',
                data: {
                    ItemsPerPage: mElement.find('[name="selPBItemsPerPage"] option:selected').val(),
                    PageChangeDelay: mElement.find('[name="selPBItemsPageChangeDelay"] option:selected').val(),
                }, success: function (e) {
                    GetGeneralSettings();
                    WebToast('Successfully saved')
                }
            })
        })

        mElement.find('[name="btnPBAddPriceBulletin"]').click(function () {
            ClearPriceBulletinFields();
        })

        mElement.find('[name="btnPBAddProduct"]').click(function () {
            BrowsePopup('LibraryPimProduct', 'Search Products', [
                {
                    Message: 'You can select multiple products. Click "Ok" button when you\'re done.',
                    SelectMultiple: true,
                    DisplayText: [{ Label: 'Product No.', Column: 'ProductNo', Style: { width: '150px' } }, { Label: 'Brand', Column: 'Brand', Style: { width: '150px' } }, { Label: 'Description', Column: 'ProductNameOnly' }],
                    StyleFormat: 'table',
                    Click: function (e) {
                        for (var i in e) {
                            e[i].State = 'new';
                            e[i].DisplayName = e[i].ProductNameOnly;
                            DisplayProduct(e[i]);
                        }
                        tbody = popupBulletin.find('tbody');
                    }
                }
            ])
        })

        mElement.find('[name="btnPBSave"]').click(function () {
            var txtBulletinName = mElement.find('[name="txtBulletinName"]').removeClass('is-invalid');

            if ($.trim(txtBulletinName.val()) == '') {
                txtBulletinName.addClass('is-invalid');
                WebToast('Please enter Price Bulletin Name');
                return;
            }

            if (arrProductList.length == 0) {
                WebToast('Please add at least one product');
                return;
            }

            AjaxSend({
                url: '/Revenue/SavePriceBulletin',
                data: {
                    BulletinID: bulletinID,
                    BulletinName: txtBulletinName.val(),
                    Products: arrProductList
                }, success: function (e) {
                    if (e.Result == 'saved') {
                        popupBulletin.modal('hide');
                        WebToast('Successfully saved');
                        GetPriceBulletinList();
                    } else {
                        NewMessageBox(e.Error, 'Error');
                    }
                }
            })
        })

        mElement.find('[name="txtSearchPriceBulletin"]').keydown(function (e) {
            if (e.which == 13) {
                GetPriceBulletinList();
            }
        })

        function GetPriceBulletinList() {
            if ($.fn.DataTable.isDataTable("#tblPriceBulletin")) {
                //check if datatable is created 
                $('#tblPriceBulletin').DataTable().clear().destroy();
            }

            var tblPriceBulletin = $('#tblPriceBulletin').DataTable({
                retrieve: true,
                "bdestroy": true,
                "pageLength": 25,
                //"order": [[1, "asc"]],
                "bProcessing": true,
                "bServerSide": true,
                "ajax": {
                    url: MergeUrlParameters({
                        OrgID: sessionStorage.OrgID,
                        keywords: mElement.find('[name="txtSearchPriceBulletin"]').val()
                    }, '/Revenue/GetPriceBulletinList'),
                    type: 'POST'
                },
                "autoWidth": false,
                "columns": [
                    {
                        "data": function (data, type, full, meta) {
                            return data.RowNum + '.';
                        },
                        "width": "50px"
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return CStr2(data.Name);
                        }
                    },
                    {
                        title: 'Last Updated',
                        "data": function (data, type, full, meta) {
                            return CStr2(data.UpdatedByName) + ' on '+ $.format.date(CJsonDate(data.LastUpdated), 'MM-dd-yyyy hh:mm a');
                        }, 'createdCell': function (td, cellData, rowData, row, col) {
                            $(td).attr('style', 'text-align:center');
                        }
                    }
                ],
                "createdRow": function (row, data, index) {
                    $(row).data('data', data).click(function () {
                        var data = $(this).data('data');
                        ClearPriceBulletinFields();
                        bulletinID = data.ID;
                        Bs.Loading('show');
                        AjaxSend({
                            url: '/Revenue/GetDataPriceBulletin',
                            data: { BulletinID: bulletinID },
                            success: function (e) {
                                Bs.Loading('hide');

                                mElement.find('[name="txtBulletinName"]').val(e.Info[0].Name);

                                for (var i in e.Products) {
                                    DisplayProduct(e.Products[i]);
                                }

                                tbody = popupBulletin.find('tbody');
                                popupBulletin.modal('show');
                            }
                        })
                    })
                },
                "initComplete": function () {
                    $('#tblPriceBulletin').removeClass('dataTable');
                },
                "aoColumnDefs": [{ "bSortable": false, "aTargets": ['_all'] }],
                "responsive": true,
                //"columnDefs": [{ "className": "txt-right", "targets": [($('#selExpenseEmployee').val() == '' ? 7 : 6)] }],
                "sDom": '<"top">rt<"bottom"ip><"clear">'
                //"stateSave": true
            })
        }

        function GetGeneralSettings() {
            AjaxSend({
                url: '/Revenue/GetPriceBulletinGeneralSettings',
                success: function (e) {
                    mOut.ItemsPerPage = e.ItemsPerPage;
                    mOut.PageChangeDelay = e.PageChangeDelay;
                    mElement.find('[name="selPBItemsPerPage"]').val(e.ItemsPerPage);
                    mElement.find('[name="selPBItemsPageChangeDelay"]').val(e.PageChangeDelay);
                }
            })
        }

        function DisplayProduct(data) {
            if (tbody.find('tr[productid="' + data.ID + '"]').length > 0) return;

            tbody.append($('<tr>')
                    .data('data', data)
                    .attr('productid', data.ID)
                    .append($('<td>').text(ctrProductList + '.'))
                    .append($('<td>').text(data.ProductNo))
                    .append($('<td>').text(data.Brand))
                    .append($('<td>').text(data.ProductNameOnly))
                    .append($('<td>').append($('<input data-role="none" class="form-control" value="' + data.DisplayName + '"/>')
                            .data('data', data)
                            .blur(function () {
                                var dis = $(this), data = dis.data('data');
                                data.DisplayName = dis.val()
                            })
                        )
                    )
                    .append($('<td class="text-center">').text(CDec2(data.RetailPrice) == 0 ? '' : AccountingFormat(data.RetailPrice)))
                    .append($('<td class="text-center">').text(CDec2(data.WholesalePrice) == 0 ? '' : AccountingFormat(data.WholesalePrice)))
                    .append($('<td>')
                        .append($('<button data-role="none" class="btn btn-danger">')
                            .data('data', data)
                            .append('<span class="bi-trash"></span>')
                            .click(function () {
                                var dis = $(this), data = dis.data('data');

                                NewMessageBox('Are you sure you want to remove this product?', data.ProductNameOnly, [
                                    {
                                        Text: 'Yes', Click: function (e) {
                                            dis.parents().closest('tr').remove();
                                            data.State = 'remove';
                                            RestartNumbering();
                                        }
                                    }, { Text: 'No' }
                                ])
                            })
                        )
                    )
                )
            ctrProductList += 1;
            arrProductList.push(data);
        }

        function RestartNumbering() {
            ctrProductList = 1;
            tbody.find('tr:not(.hide)').each(function () {
                $(this).find('td:eq(0)').text(ctrProductList + '.');
                ctrProductList += 1;
            })
        }
        
        function ClearPriceBulletinFields() {
            bulletinID = '';
            ctrProductList = 1;
            mElement.find('input').val();
            tbody.empty();
        }

        mOut.Initialize = function () {
            GetGeneralSettings();
            GetPriceBulletinList();
        }

        mOut.Initialize();
        
        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});