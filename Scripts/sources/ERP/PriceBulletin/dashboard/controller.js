define([
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/PriceBulletin/dashboard/view.html?v' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.PriceBulletin.dashboard.controller', null, function () {

        var mOut = {}, mElement, ctrProductList = 1, bulletinID = '';
        
        mElement = $(mHtml);
        mOut.element = mElement;        

        $('#tabPriceBulletinDashboard').empty().append(mElement);

        var popupBulletin = mElement.find('#popupBulletinDashboard');
        var tbody = popupBulletin.find('tbody');
        var popupPriceBulletinDashboard = mElement.find('#popupPriceBulletinDashboard');

        startclock(mElement.find('[name="lblDateToday"]'));

        function resize_to_fit() {
            return; //temp only
            var fontsize = $('.outer > div > div').css('font-size');

            if ($('.outer > div').height() >= $('.outer').height()) {
                $('.outer > div > div').css('fontSize', parseFloat(fontsize) - 10);
                resize_to_fit();
            } else {
                $('.outer > div > div').css('fontSize', parseFloat(fontsize) + 1);
            }
        }

        $(window).resize(function () {
            resize_to_fit();
        })

        mElement.find('[name="txtSearchPriceBulletin"]').keydown(function (e) {
            if (e.which == 13) {
                GetPriceBulletinList();
            }
        })

        mElement.find('[name="btnPBPlay"]').click(function (e) {
            GetDataPriceBulletin(function (e) {
                var ctrList = 0, arrProducts = e.Products, itemsperpage = ERP.PriceBulletin.main.controller.Settings.ItemsPerPage;
                var delay = CInt2(ERP.PriceBulletin.main.controller.Settings.PageChangeDelay) * 1000; // in seconds
                var clrTO;

                popupPriceBulletinDashboard.find('[name="lblBulletinName"]').text(e.Info[0].Name);

                function Show() {
                    var vTbody = popupPriceBulletinDashboard.find('[name="divProductList"]').empty();// popupPriceBulletinDashboard.find('tbody').empty();

                    for (var i = 0; i < itemsperpage; i++) {
                        var data = arrProducts[ctrList];
                        
                        if (data == undefined) {
                            ctrList = 0;
                            break;
                        }

                        vTbody.append($('<div style="border-bottom-width:3px !important; background: rgb(84 84 84 /100%);" class="row mb-3 g-0 rounded shadow text-light border border-bottom border-3 border-primary border-top-0 border-left-0 border-right-0">')
                                .append($('<div class="col text-truncate p-2">').text(data.ProductNameOnly))
                                .append($('<div class="col-auto text-center p-2 pr-3 colretail">').text(CDec2(data.RetailPrice) == 0 ? '' : AccountingFormat(data.RetailPrice)))
                                .append($('<div class="col-auto text-center p-2 pl-3 colwholesale">').text(CDec2(data.WholesalePrice) == 0 ? '' : AccountingFormat(data.WholesalePrice)))
                            )

                        //vTbody.append($('<tr>')
                        //        .append($('<td>').text(data.ProductNameOnly))
                        //        .append($('<td class="text-center align-middle">').text(CDec2(data.RetailPrice) == 0 ? '' : AccountingFormat(data.RetailPrice)))
                        //        .append($('<td class="text-center align-middle">').text(CDec2(data.WholesalePrice) == 0 ? '' : AccountingFormat(data.WholesalePrice)))
                        //    )

                        ctrList += 1;
                    }                    

                    //$('.outer > div > div').css('font-size', '200px');
                    //resize_to_fit()

                    $('.colretail').css('width', CStr2(Math.max.apply(Math, $('.colretail').map(function () { return $(this).outerWidth(); }).get())) + 'px');
                    $('.colwholesale').css('width', CStr2(Math.max.apply(Math, $('.colwholesale').map(function () { return $(this).outerWidth(); }).get())) + 'px');

                    if (arrProducts.length > itemsperpage) {
                        clrTO = setTimeout(function () {
                            Show();
                        }, delay)
                    }                    
                }
                document.documentElement.webkitRequestFullscreen();

                setTimeout(function () {
                    popupPriceBulletinDashboard.modal('show').unbind('hidden.bs.modal shown.bs.modal').on('hidden.bs.modal', function () {
                        if (typeof clrTO != undefined) {
                            clearTimeout(clrTO);
                        }                        
                    }).on('shown.bs.modal', function () {
                        resize_to_fit()
                    })
                    Show();
                }, 250)
            })
        })

        mOut.GetPriceBulletinList = function () {
            if ($.fn.DataTable.isDataTable("#tblPriceBulletinDashboard")) {
                //check if datatable is created 
                $('#tblPriceBulletinDashboard').DataTable().clear().destroy();
            }

            var tblPriceBulletinDashboard = $('#tblPriceBulletinDashboard').DataTable({
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
                            return CStr2(data.UpdatedByName) + ' on ' + $.format.date(CJsonDate(data.LastUpdated), 'MM-dd-yyyy hh:mm a');
                        }, 'createdCell': function (td, cellData, rowData, row, col) {
                            $(td).attr('style', 'text-align:center');
                        }
                    }
                ],
                "createdRow": function (row, data, index) {
                    $(row).data('data', data).click(function () {
                        var data = $(this).data('data');
                        bulletinID = data.ID;
                        GetDataPriceBulletin(function (e) {
                            mElement.find('[name="txtBulletinName"]').val(e.Info[0].Name);

                            for (var i in e.Products) {
                                DisplayProduct(e.Products[i]);
                            }

                            tbody = popupBulletin.find('tbody');
                            popupBulletin.modal('show');
                        })
                    })
                },
                "initComplete": function () {
                    $('#tblPriceBulletinDashboard').removeClass('dataTable');
                },
                "aoColumnDefs": [{ "bSortable": false, "aTargets": ['_all'] }],
                "responsive": true,
                //"columnDefs": [{ "className": "txt-right", "targets": [($('#selExpenseEmployee').val() == '' ? 7 : 6)] }],
                "sDom": '<"top">rt<"bottom"ip><"clear">'
                //"stateSave": true
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
                    .append($('<td>').text(data.DisplayName))
                    .append($('<td class="text-center">').text(CDec2(data.RetailPrice) == 0 ? '' : AccountingFormat(data.RetailPrice)))
                    .append($('<td class="text-center">').text(CDec2(data.WholesalePrice) == 0 ? '' : AccountingFormat(data.WholesalePrice)))
                )
            ctrProductList += 1;
        }

        function GetDataPriceBulletin(callback) {
            Bs.Loading('show');
            AjaxSend({
                url: '/Revenue/GetDataPriceBulletin',
                data: { BulletinID: bulletinID },
                success: function (e) {
                    Bs.Loading('hide');

                    callback(e);
                }
            })
        }
                
        mOut.Initialize = function () {
            mOut.GetPriceBulletinList();
        }

        mOut.Initialize();
        
        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});