define([
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/PIM/relatedproducts/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('PIM.relatedproducts.controller', null, function () {

        var mOut = {};
        var mElement;

        mElement = $(mHtml);

        if ($('#relatedproducts *').length == 0) {
            $('#relatedproducts').append(mElement);
        }        

        mOut.element = mElement;

        mElement.find('[name="btnAdd"]').click(function () {
            var tag = $(this).attr('value');

            BrowsePopup('LibraryPIMProduct', 'Search ' + tag + 's', [
                {
                    DisplayText: [{ Lable: 'Product No.', Column: 'ProductNo' }, { Lable: 'Product Description', Column: 'ProductName' }],
                    SelectMultiple: true,
                    Message: 'You can select multiple products. Click OK when you\'re done.',
                    Click: function (e) {
                        Bs.Loading('show');

                        AjaxSend({
                            url: 'PIMProduct/SaveRelatedProducts',
                            data: {
                                SourceProductID: sessionStorage.ViewID,
                                Tag: tag,
                                RelatedProducts: e
                            }, success: function (e) {
                                Bs.Loading('hide');

                                if (e.Results == 'saved') {
                                    WebToast('Related Products has been added');
                                    mOut.Initialize();
                                } else {
                                    NewMessageBox(e.Results, 'Error')
                                }
                            }
                        })
                    }
                }
            ])
        })

        function DisplayRelatedProducts(table, data, isGrantAccessToModifyPIM) {
            if ($.fn.DataTable.isDataTable(table)) {
                //check if datatable is created 
                $(table).DataTable().clear().destroy();
            }

            var ctr = 0, tbl = $(table).DataTable({
                "bdestroy": true,
                "pageLength": 10,
                "order": [[0, "asc"]],
                data: data,
                "autoWidth": false,
                "columns": [
                    {
                        title: '#',
                        "data": function (data, type, full, meta) {
                            ctr += 1;
                            return ctr + '.';
                        },
                        "width": "30px"
                    },
                    {
                        title: 'Product No.',
                        "data": function (data, type, full, meta) {
                            return data.ProductNo;
                        }
                    },
                    {
                        title: 'Product Description',
                        "data": function (data, type, full, meta) {
                            return data.ProductName;
                        }
                    },
                    {
                        title: 'Brand',
                        "data": function (data, type, full, meta) {
                            return CStr2(data.Brand);
                        }
                    },
                    {
                        title: '',
                        "data": null,
                        visible: isGrantAccessToModifyPIM,
                        'createdCell': function (td, cellData, rowData, row, col) {
                            var btn = $('<button data-role="none" class="btn btn-danger">')
                                        .append('<span class="bi-trash"></span>')
                                        .data('data', rowData)
                                        .click(function () {
                                            var data = $(this).data('data');

                                            NewMessageBox('Are you sure you want deactivate <b>' + data.ProductName + '</b> as ' + data.Tag + '?', 'Confirmation Dialog', [
                                                {
                                                    Text: 'Yes', Click: function () {
                                                        Bs.Loading('show');

                                                        AjaxSend({
                                                            url: 'PIMProduct/DeactivateRelatedProduct',
                                                            data: { ID: data.ID }, success: function (e) {
                                                                Bs.Loading('hide');
                                                                if (e.Results == 'saved') {
                                                                    WebToast('Successfully deactivated', 'top');
                                                                    mOut.Initialize();
                                                                } else {
                                                                    NewMessageBox(e.Results, 'Error');
                                                                }
                                                            }
                                                        })
                                                    }
                                                }, { Text: 'No' }
                                            ])
                                        })
                            $(td).addClass('text-center').html(btn);
                        }, width: '100px'
                    }
                ],
                "aoColumnDefs": [{ "bSortable": false, "aTargets": [0] }],
                "responsive": true,
                "columnDefs": [{ "targets": 0, "orderable": false }],
                "sDom": '<"top">rt<"bottom"ip><"clear">',
                "initComplete": function () {
                    $(table).removeClass('dataTable');
                }
                //"stateSave": true
            })

            $(table + ' tbody').unbind('click').on('click', 'tr', function () {
                if (CStr2(tbl.row(this).data()) != '') {
                    //do nothing for now
                }
            });
        }

        mOut.Initialize = function (isGrantAccessToModifyPIM) { //params - naa sa PIM
            isLoad = false;

            if (!isGrantAccessToModifyPIM) $('[name="btnAdd"][value]').remove();

            AjaxSend({
                url: 'PIMProduct/GetRelatedProducts',
                data: { ProductID: sessionStorage.ViewID },
                success: function (e) {
                    DisplayRelatedProducts('#tblProductDirectCompetitors', e.DirectCompetitors.Rows, isGrantAccessToModifyPIM);
                    DisplayRelatedProducts('#tblProductVariants', e.Variants.Rows, isGrantAccessToModifyPIM);
                    DisplayRelatedProducts('#tblProductComplements', e.Complements.Rows, isGrantAccessToModifyPIM);
                }
            })
        }

        //mOut.Initialize();

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});