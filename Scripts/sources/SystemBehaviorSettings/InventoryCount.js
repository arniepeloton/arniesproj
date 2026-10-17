require([
    'common',
    '/Scripts/jquery-ui-sortable.js'
], function () {

    var IsAutomaticEffect = 1;
    var SKURankNo = 0;

    $(function () {
        GetSystemBehavior();

        window.InventoryCountAdmin = {
            GetSystemBehavior: GetSystemBehavior
        }

        $('body')
            .on('click', '#rdInventoryCountAutomaticEffect', function () {
                IsAutomaticEffect = 1;
            })
            .on('click', '#rdInventoryCountNeedsApproval', function () {
                IsAutomaticEffect = 0;
            })
            .on('click', '[name="btnSaveInventoryCountSettings"]', function () {
                SaveInventoryCountSystemBehavior();
            })
            .on('click', '[name="btnDoneInvCnt"]', function () {
                location.hash = '';
            })
            .on('click', '#btnInventoryCountAddSKU', function () {
                BrowsePopup('LibraryPIMProduct', 'Select Product', [
                    {
                        DisplayText: [{ Label: 'Product No.', Column: 'ProductNo' }, { Label: 'Description', Column: 'ProductName' }],
                        SelectMultiple: true,
                        Click: function (e) {
                            for (var i in e) {
                                DisplayInventoryCountSKU(e[i]);
                            }
                        }
                    }
                ])
            })
            .on('keydown', '#txtSearchInventoryCountTrans', function (e) {
                var filter = $(this).val().toLowerCase();
                var lis = $('#listInventoryCountSKURanking > li');
                lis.each(function () {
                    var name = $(this).text();
                    if (name.toLowerCase().indexOf(filter) != -1)
                        $(this).css('display', 'block');
                    else
                        $(this).css('display', 'none');
                })
            })
    });

    function DisplayInventoryCountSKU(e) {
        SKURankNo += 1;
        e.OrderNo = SKURankNo;
        $('#listInventoryCountSKURanking').append($('<li class="list-group-item">')
            .data('data', e)
            .css('cursor', 'pointer')
            .append($('<div class="row align-items-center">')
                .append($('<div class="col-auto">').text(SKURankNo + '.'))
                .append($('<div class="col">').html(e.ProductNo + ' <br> ' + e.ProductName))
                .append($('<div class="col-auto">')
                    .append($('<button data-role="none" class="btn btn-danger">')
                        .append('<span class="bi-trash"></span>')
                        .click(function (e) {
                            $(this).parents().closest('li').remove();
                        })
                    )
                )
            )
        )
    }

    function SaveInventoryCountSystemBehavior() {
        var SKUList = [];
        SKURankNo = 0;

        $('#listInventoryCountSKURanking li').each(function () {
            SKURankNo += 1;
            $(this).data('data').OrderNo = SKURankNo;
            SKUList.push($(this).data('data'));
        })

        var data = {
            //IsAutomaticallyReceivedStocks: IsAutomaticReceived,
            IsAutomaticEffect: IsAutomaticEffect,
            IsAllowDecimalinInventoryCount: $.trim($("#selAllowDecimalinInventoryCount").is(':checked')),
            DefaultInventoryOption: $('#selInventoryCountDefault option:selected').val(),
            ShowDefaulUomInventoryCount: $.trim($("#btnInventoryCountDefaultUom").is(':checked')),
            AllowBasedUomInventoryCount: $.trim($("#btnInventoryCountAllowUom").is(':checked')),
            DontAllowViewZreadWhenNoInventory: $.trim($("#selDontAllowViewZreadWhenNoInventory").is(':checked')),
            SKUList: SKUList
        };        

        AjaxSend({
            url: 'SystemBehaviorSettings/SaveInventoryBehavior',
            data: data,
            success: function (vData) {
                if (vData.Result == "Success") {
                    WebToast('Successfully saved', 'top');
                    history.back();
                }
            }
        });

    }

    function GetSystemBehavior() {
        SKURankNo = 0;

        AjaxSend({
            url: 'SystemBehaviorSettings/GetInventoryCountBehavior',
            success: function (vData) {

                if (vData.IsAutomaticEffect == 1) {

                    $('#rdInventoryCountAutomaticEffect').prop('checked', true);//.checkboxradio("refresh");
                    $('#rdInventoryCountNeedsApproval').prop('checked', false);//.checkboxradio("refresh");
                    IsAutomaticEffect = 1;

                } else {

                    $('#rdInventoryCountNeedsApproval').prop('checked', true);//.checkboxradio("refresh");
                    $('#rdInventoryCountAutomaticEffect').prop('checked', false);//.checkboxradio("refresh");
                    IsAutomaticEffect = 0;

                }

                if (vData.IsAllowDecimalinInventoryCount == 1) {
                    $('#selAllowDecimalinInventoryCount').prop('checked', true);
                } else {
                    $('#selAllowDecimalinInventoryCount').removeAttr('checked');
                }

                $('#selInventoryCountDefault').val(vData.DefaultInventoryOption);
                $('#btnInventoryCountDefaultUom').prop('checked', vData.ShowDefaulUomInventoryCount);
                $('#btnInventoryCountAllowUom').prop('checked', vData.AllowBasedUomInventoryCount);
                $('#selDontAllowViewZreadWhenNoInventory').prop('checked', vData.DontAllowViewZreadWhenNoInventory);

                $('#listInventoryCountSKURanking').empty();

                for (var i in vData.SKUList.Rows) {
                    DisplayInventoryCountSKU(vData.SKUList.Rows[i]);
                }

                setTimeout(function () {
                    $('#listInventoryCountSKURanking').sortable({
                        list: '#listInventoryCountSKURanking',
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
                                $(this).find('.col-auto').text(newOrderNo + '.');
                                $(this).data('data').OrderNo = newOrderNo;
                                newOrderNo += 1;
                            })
                        }
                    });
                }, 100);

                //try {
                //    $('#selAllowDecimalinInventoryCount').slider('refresh');
                //}
                //catch (e) {
                //    $('#selAllowDecimalinInventoryCount').slider();
                //}

            }
        });
    }
});