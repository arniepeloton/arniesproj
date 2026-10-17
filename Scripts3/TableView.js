require(['common'
    ], function () {
        require([(OfflineMode ? '/Scripts3/SalesMenu_offline.js?crc=' : '')], function () {
            $(function () {

                var mLoaded = false;

                $.mobile.activePage.bind('pageshow', PageShow);

                function PageShow() {
                    mLoaded = true;
                    getPlanogramLayout();
                };
                if (!mLoaded) PageShow();


                function getPlanogramLayout() {

                    AjaxSend({
                        url: 'TableView/getPlanogramLayout',
                        success: function (vData) {

                            loadPlanogram(vData.TableLayoutData.Rows);

                        }
                    });
                }



                function loadPlanogram(vRows) {

                    var i = 0;
                    var arrObject = [];
                    var topComputed;
                    var leftComputed;
                    var arrExist = [];

                    //          
                    $('#divRenderGrid').empty();
                    $('.gridBackground').remove();
                    
                    for (var i = 0; i <= vRows.length - 1; i++) {

                        //                        alert(arrExist.indexOf($(this).attr('id')) < 0);
                        //                        if (arrExist.indexOf($(this).attr('id')) < 0) {
                        //arrObject = $('#container > img').attr('id').split('_');
                        topComputed = (parseFloat(vRows[i].PositionTop) / parseFloat(vRows[i].ImageHeight)) * 100;
                        leftComputed = (parseFloat(vRows[i].PositionLeft) / parseFloat(vRows[i].ImageWidth)) * 100;

                        //save the params to db kay mao to ang i-load on ajaxsend success
                        var id = vRows[i].ObjectType + '_' + vRows[i].DefaultSeatNo + '_' + i;
                        loadObjectByType(vRows[i].ObjectType, vRows[i].DefaultSeatNo, id, topComputed, leftComputed, vRows[i].ImageWidth, vRows[i].ImageHeight);

                        //                            arrExist[i] = $(this).attr('id');
                        //                        }
                    }


                    // clear contents
                    //                    $('#container').empty();

//                    $.mobile.changePage('#pageRender');

                    $('#divRenderGrid').append($('<img class="gridBackground" src="/Content/images/renderGrid.png"/>'));
                    //make it fullscreen in any screen resolution
                    $('#divRenderGrid').css('width', $(window).width());
                    $('#divRenderGrid').css('height', $(window).height());

                    //                    //todo
                    //                    $('#divRenderGrid').append($('<button id="btnBackPreview" style="z-index: 200;position:absolute; float: left;" data-rel="back" data-theme="b" >')
                    //                                                        .text("Back")
                    //                                                        .click(function (e) {
                    //                                                            e.preventDefault();
                    //                                                            history.back();
                    //                                                        })
                    //                                                    );


                }



                function loadObjectByType(type, seat, id, posTop, posLeft, imgWidth, imgHeight) {

                    switch (type) {
                        case 'RoundTableVertical':
                            $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/100 pixel Round Table 2 Seater Vertical.png"/>'));

                            break;
                        case 'RoundTable':
                            if (seat == '2')
                                $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/100 pixel Round Table 2 Seater.png"/>'));
                            else if (seat == '4')
                                $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/100 pixel Round Table 4 Seater.png"/>'));
                            else if (seat == '8')
                                $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/200 pixel Round Table 8 Seater.png"/>'));
                            else if (seat == '10')
                                $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/300 pixel Round Table 10 Seater.png"/>'));
                            else
                                $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/350 pixel Round Table 12 Seater.png"/>'));
                            break;
                        case 'RoundTable150px':
                            $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/150 pixel Round Table 4 Seater.png"/>'));
                            break;
                        case 'HorizontalRectangle':
                            if (seat == '2')
                                $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/Horizontal Rectangle Table 100x100 pixel.png"/>'));
                            else if (seat == '6')
                                $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/Horizontal Rectangle Table 100x200 pixel.png"/>'));
                            else if (seat == '8')
                                $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/Horizontal Rectangle Table 100x300 pixel.png"/>'));
                            else if (seat == '10')
                                $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/Horizontal Rectangle Table 100x400 pixel.png"/>'));
                            else
                                $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/Horizontal Rectangle Table 100x500 pixel.png"/>'));

                            break;
                        case 'RectangleTable':
                            $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/Rectangle Table 100x100 pixel 4 Seater.png"/>'));

                            break;
                        case 'VerticalRectangle':
                            if (seat == '2')
                                $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/Vertical Rectangle Table 100x100 pixel.png"/>'));
                            else if (seat == '6')
                                $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/Vertical Rectangle Table 100x200 pixel.png"/>'));
                            else if (seat == '8')
                                $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/Vertical Rectangle Table 100x300 pixel.png"/>'));
                            else if (seat == '10')
                                $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/Vertical Rectangle Table 100x400 pixel.png"/>'));
                            else
                                $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/Vertical Rectangle Table 100x500 pixel.png"/>'));

                            break;
                        case 'BarStoolAvailable':
                            $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/Bar Stool - Available.png"/>'));

                            break;
                        case 'Door1':
                            $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/Door 1.png"/>'));

                            break;
                        case 'Door2':
                            $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/Door 2.png"/>'));

                            break;
                        case 'Door3':
                            $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/Door 3.png"/>'));

                            break;
                        case 'Door4':
                            $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/Door 4.png"/>'));

                            break;
                        case 'Door5':
                            $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/Door 5.png"/>'));

                            break;
                        case 'Door6':
                            $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/Door 6.png"/>'));

                            break;
                        case 'Door7':
                            $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/Door 7.png"/>'));

                            break;
                        case 'Door8':
                            $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/Door 8.png"/>'));

                            break;
                        case 'GlassWindowHorizontal':
                            $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/Glass Window - Horizontal.png"/>'));

                            break;
                        case 'GlassWindowVertical':
                            $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/Glass Window - Vertical.png"/>'));

                            break;
                        case 'POSCounterLongHorizontalRectangle':
                            $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/POS Counter Long Horizontal Rectangle.png"/>'));

                            break;
                        case 'POSCounterLongVerticalRectangle':
                            $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/POS Counter Long Vertical Rectangle.png"/>'));

                            break;
                        case 'POSCounterMediumHorizontalRectangle':
                            $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/POS Counter Medium Horizontal Rectangle.png"/>'));

                            break;
                        case 'POSCounterMediumVerticalRectangle':
                            $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/POS Counter Medium Vertical Rectangle.png"/>'));

                            break;
                        case 'POSCounterSquare':
                            $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/POS Counter Square.png"/>'));

                            break;
                        case 'WallPaneHorizontal':
                            $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/Wall Panel - Horizontal.png"/>'));

                            break;
                        case 'WallPaneVertical':
                            $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/Wall Panel - Vertical.png"/>'));

                            break;
                        case 'BarLongHorizontal':
                            $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/Bar - Long Horizontal.png"/>'));

                            break;
                        case 'BarLongVertical':
                            $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/Bar - Long Vertical.png"/>'));

                            break;
                        case 'BarShortHorizontal':
                            $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/Bar - Short Horizontal.png"/>'));

                            break;
                        case 'BarShortVertical':
                            $('#divRenderGrid').append($('<img id="' + id + 'render" class="img2" src="/Content/images/Table and Chairs for Table View Mode/Bar - Short Vertical.png"/>'));

                            break;

                        default:


                    }

                    $('#' + id + 'render').css('top', posTop + '%');
                    $('#' + id + 'render').css('left', posLeft + '%');
                    $('#' + id + 'render').css('width', (parseFloat(getImageRealSize($('#selFloorArea').val(), id, imgWidth, "width")) / parseFloat($(window).width())) * 100 + '%');
                    $('#' + id + 'render').css('height', (parseFloat(getImageRealSize($('#selFloorArea').val(), id, imgHeight, "height")) / parseFloat($(window).height())) * 100 + '%');

                }





                function getImageRealSize(flrArea, imgID, propVal, property) {

                    switch (flrArea) {
                        case '25':
                            return (propVal * 0.8);
                            break;
                        case '50':
                            return (propVal * 0.4);

                            break;
                        case '75':

                            return (propVal * 0.2666666667);

                            break;
                        case '100':

                            return (propVal * 0.2);
                            break;
                        case '150':
                            return (propVal * 0.1333333333);

                            break;
                        case '200':

                            return (propVal * 0.1);


                            break;
                        case '250':

                            return (propVal * 0.08);

                            break;
                        case '300':
                            return (propVal * 0.0666666667);

                            break;
                        case '400':
                            return (propVal * 0.05);
                            break;
                        case '500':
                            return (propVal * 0.04);

                            break;
                        case '600':
                            return (propVal * 0.0333333333);


                            break;
                        case '700':
                            return (propVal * 0.0285714286);

                            break;
                        case '800':
                            return (propVal * 0.0250000000);

                            break;
                        case '900':
                            return (propVal * 0.0222222222);
                            break;
                        case '1000':
                            return (propVal * 0.0200000000);
                            break;
                        default:
                            return (propVal * 0.8);

                    }
                }

                function setImageSize(flrArea) {

                    switch (flrArea) {
                        case '25':
                            $('.img1').css('-webkit-transform', 'scale(0.8)');
                            $('.img1').css('-moz-transform', 'scale(0.8)');
                            $('.img1').css('-o-transform', 'scale(0.8)');
                            $('.img1').css('-ms-transform', 'scale(0.8)');
                            $('.img1').css('transform', 'scale(0.8)');

                            break;
                        case '50':
                            $('.img1').css('-webkit-transform', 'scale(0.4)');
                            $('.img1').css('-moz-transform', 'scale(0.4)');
                            $('.img1').css('-o-transform', 'scale(0.4)');
                            $('.img1').css('-ms-transform', 'scale(0.4)');
                            $('.img1').css('transform', 'scale(0.4)');

                            break;
                        case '75':
                            $('.img1').css('-webkit-transform', 'scale(0.2666666667)');
                            $('.img1').css('-moz-transform', 'scale(0.2666666667)');
                            $('.img1').css('-o-transform', 'scale(0.2666666667)');
                            $('.img1').css('-ms-transform', 'scale(0.2666666667)');
                            $('.img1').css('transform', 'scale(0.2666666667)');

                            break;
                        case '100':
                            $('.img1').css('-webkit-transform', 'scale(0.2)');
                            $('.img1').css('-moz-transform', 'scale(0.2)');
                            $('.img1').css('-o-transform', 'scale(0.2)');
                            $('.img1').css('-ms-transform', 'scale(0.2)');
                            $('.img1').css('transform', 'scale(0.2)');

                            break;
                        case '150':
                            $('.img1').css('-webkit-transform', 'scale(0.1333333333)');
                            $('.img1').css('-moz-transform', 'scale(0.1333333333)');
                            $('.img1').css('-o-transform', 'scale(0.1333333333)');
                            $('.img1').css('-ms-transform', 'scale(0.1333333333)');
                            $('.img1').css('transform', 'scale(0.1333333333)');

                            break;
                        case '200':
                            $('.img1').css('-webkit-transform', 'scale(0.1)');
                            $('.img1').css('-moz-transform', 'scale(0.1)');
                            $('.img1').css('-o-transform', 'scale(0.1)');
                            $('.img1').css('-ms-transform', 'scale(0.1)');
                            $('.img1').css('transform', 'scale(0.1)');

                            break;
                        case '250':
                            $('.img1').css('-webkit-transform', 'scale(0.08)');
                            $('.img1').css('-moz-transform', 'scale(0.08)');
                            $('.img1').css('-o-transform', 'scale(0.08)');
                            $('.img1').css('-ms-transform', 'scale(0.08)');
                            $('.img1').css('transform', 'scale(0.08)');

                            break;
                        case '300':
                            $('.img1').css('-webkit-transform', 'scale(0.0666666667)');
                            $('.img1').css('-moz-transform', 'scale(0.0666666667)');
                            $('.img1').css('-o-transform', 'scale(0.0666666667)');
                            $('.img1').css('-ms-transform', 'scale(0.0666666667)');
                            $('.img1').css('transform', 'scale(0.0666666667)');
                            break;
                        case '400':
                            $('.img1').css('-webkit-transform', 'scale(0.05)');
                            $('.img1').css('-moz-transform', 'scale(0.05)');
                            $('.img1').css('-o-transform', 'scale(0.05)');
                            $('.img1').css('-ms-transform', 'scale(0.05)');
                            $('.img1').css('transform', 'scale(0.05)');

                            break;
                        case '500':
                            $('.img1').css('-webkit-transform', 'scale(0.04)');
                            $('.img1').css('-moz-transform', 'scale(0.04)');
                            $('.img1').css('-o-transform', 'scale(0.04)');
                            $('.img1').css('-ms-transform', 'scale(0.04)');
                            $('.img1').css('transform', 'scale(0.04)');

                            break;
                        case '600':
                            $('.img1').css('-webkit-transform', 'scale(0.0333333333)');
                            $('.img1').css('-moz-transform', 'scale(0.0333333333)');
                            $('.img1').css('-o-transform', 'scale(0.0333333333)');
                            $('.img1').css('-ms-transform', 'scale(0.0333333333)');
                            $('.img1').css('transform', 'scale(0.0333333333)');

                            break;
                        case '700':
                            $('.img1').css('-webkit-transform', 'scale(0.0285714286)');
                            $('.img1').css('-moz-transform', 'scale(0.0285714286)');
                            $('.img1').css('-o-transform', 'scale(0.0285714286)');
                            $('.img1').css('-ms-transform', 'scale(0.0285714286)');
                            $('.img1').css('transform', 'scale(0.0285714286)');

                            break;
                        case '800':
                            $('.img1').css('-webkit-transform', 'scale(0.0250000000)');
                            $('.img1').css('-moz-transform', 'scale(0.0250000000)');
                            $('.img1').css('-o-transform', 'scale(0.0250000000)');
                            $('.img1').css('-ms-transform', 'scale(0.0250000000)');
                            $('.img1').css('transform', 'scale(0.0250000000)');

                            break;
                        case '900':
                            $('.img1').css('-webkit-transform', 'scale(0.0222222222)');
                            $('.img1').css('-moz-transform', 'scale(0.0222222222)');
                            $('.img1').css('-o-transform', 'scale(0.0222222222)');
                            $('.img1').css('-ms-transform', 'scale(0.0222222222)');
                            $('.img1').css('transform', 'scale(0.0222222222)');

                            break;
                        case '1000':
                            $('.img1').css('-webkit-transform', 'scale(0.0200000000)');
                            $('.img1').css('-moz-transform', 'scale(0.0200000000)');
                            $('.img1').css('-o-transform', 'scale(0.0200000000)');
                            $('.img1').css('-ms-transform', 'scale(0.0200000000)');
                            $('.img1').css('transform', 'scale(0.0200000000)');

                            break;
                        default:
                            $('.img1').css('-webkit-transform', 'scale(0.8)');
                            $('.img1').css('-moz-transform', 'scale(0.8)');
                            $('.img1').css('-o-transform', 'scale(0.8)');
                            $('.img1').css('-ms-transform', 'scale(0.8)');
                            $('.img1').css('transform', 'scale(0.8)');

                    }
                }



                $.fn.drags = function (opt) {

                    opt = $.extend({ handle: "", cursor: "move" }, opt);

                    if (opt.handle === "") {
                        var $el = this;
                    } else {
                        var $el = this.find(opt.handle);
                    }

                    return $el.css('cursor', opt.cursor).on("mousedown", function (e) {
                        if (opt.handle === "") {
                            var $drag = $(this).addClass('draggable');
                        } else {
                            var $drag = $(this).addClass('active-handle').parent().addClass('draggable');
                        }
                        var z_idx = $drag.css('z-index'),
                                drg_h = $drag.outerHeight(),
                                drg_w = $drag.outerWidth(),
                                pos_y = $drag.offset().top + drg_h - e.pageY,
                                pos_x = $drag.offset().left + drg_w - e.pageX;
                        $drag.css('z-index', 1000).parents().on("mousemove", function (e) {
                            $('.draggable').offset({
                                top: e.pageY + pos_y - drg_h,
                                left: e.pageX + pos_x - drg_w
                            }).on("mouseup", function () {
                                $(this).removeClass('draggable').css('z-index', z_idx);
                            });
                        });
                        e.preventDefault(); // disable selection
                    }).on("mouseup", function (e) {
                        //                        alert($(this).attr('id'));
                        if (opt.handle === "") {
                            $(this).removeClass('draggable');
                        } else {
                            $(this).removeClass('active-handle').parent().removeClass('draggable');
                        }
                    });

                }

                ////////////////

            }); // $(function() {

        }); // require offline 
    });                                                                                                                                                                                                                                                                                                                                                                                                  // require require


