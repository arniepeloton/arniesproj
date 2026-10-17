require(['common'
    ], function () {
        require([(OfflineMode ? '/Scripts3/SalesMenu_offline.js?crc=' : '')], function () {
            $(function () {

                var mLoaded = false;

                $.mobile.activePage.bind('pageshow', PageShow);

                function PageShow() {
                    mLoaded = true;
                    getTableLayout();
                };
                if (!mLoaded) PageShow();

                $('#btnAddTableViewTemplate').click(function (e) {
                    e.preventDefault();

                    $.mobile.changePage('#pageLayout');

                });

                $('#btnTableViewDone').click(function (e) {
                    e.preventDefault();

                    window.location = '/People/PeopleMenu';

                });

                $('#frmTableViewSearch').submit(function (e) {
                    e.preventDefault();

                    getTableLayout();

                    return false;
                });

                function getTableLayout() {
                    var pData = {};
                    pData.SearchKeyword = $.trim($("#txtTableViewSearch").val());

                    AjaxSend({
                        url: 'TableView/GetTableLayout',
                        data: pData,
                        success: function (vData) {

                            LoadTableLayout(vData.TableLayoutData.Rows);
                        }
                    });
                }

                function LoadTableLayout(vRows) {
                    var pLV = $('#lstTableViewTemplate');
                    var pItem;

                    $("#divTableViewTemplate").text(" Number of record(s) found: " + vRows.length);

                    pLV.html('');
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        pItem = $('<li>')
                    .append($('<a>')
                        .attr('ID', vRows[p].ID)
                        .click(function (e) {
                            e.preventDefault();
//                            location = MergeUrlParameters({
//                                OrgID: sessionStorage.OrgID,
//                                MenuID: $(this).attr('ID'), //add menu id to query string
//                                IsMobile: CBool(IsMobile) ? 1 : 0
//                            }, '/TableView/MenuTemplate');
                            $.mobile.changePage('#pageLayout');


                        })
                        .append($('<h3>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].SectionName)
                        )
                        .append($('<p>')
                            .attr("style", "white-space:normal")
                            .text("Created By: " + vRows[p].Surname + ', ' + vRows[p].FirstName + ' ' + vRows[p].MiddleName)
                        )
                        .append($('<p>')
                            .attr("style", "white-space:normal")
                            .text("Last Updated: " + $.format.date(new Date(vRows[p].LastUpdated), 'MMM dd, yyyy'))
                        )
                    );

                        pLV.append(pItem);
                    };

                    pLV.listview('refresh');
                };



                GetFowst();

                $('#btnPreviewTableView').click(function (event) {
                    event.preventDefault();

                    var i = 0;
                    var arrObject = [];
                    var topComputed;
                    var leftComputed;
                    var arrExist = [];

                    //          
                    $('#divRenderGrid').empty();
                    $('.gridBackground').remove();
                    $('#container > img').each(function () {

                        //                        alert(arrExist.indexOf($(this).attr('id')) < 0);
                        //                        if (arrExist.indexOf($(this).attr('id')) < 0) {
                        arrObject = $(this).attr('id').split('_');
                        topComputed = (parseFloat($(this).position().top) / parseFloat($('#container').height())) * 100;
                        leftComputed = (parseFloat($(this).position().left) / parseFloat($('#container').width())) * 100;
                        //save the params to db kay mao to ang i-load on ajaxsend success
                        loadObjectByType(arrObject[0], arrObject[1], $(this).attr('id'), topComputed, leftComputed, $(this).width(), $(this).height());

                        i = i + 1;

                        //                            arrExist[i] = $(this).attr('id');
                        //                        }
                    });


                    // clear contents
                    //                    $('#container').empty();

                    $.mobile.changePage('#pageRender');

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


                });



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

                $('#btnSaveTableView').click(function (event) {
                    event.preventDefault();

                    if ($.trim($("#txtTemplateName").val()) == "") {
                        MessageBox("Please enter section name.",
                           "Section Name",
                            [
                                { Text: 'Ok',
                                    Click: function () {
                                        history.back();
                                    }
                                }
                            ]
                        );
                        return;
                    }

                    if ($.trim($("#selFowst").val()) == "") {
                        MessageBox("Please select a FOWST.",
                           "FOWST",
                            [
                                { Text: 'Ok',
                                    Click: function () {
                                        history.back();
                                    }
                                }
                            ]
                        );
                        return;
                    }

                    if ($('#container').children().length <= 0) {
                        MessageBox("Please put objects into the grid.",
                           "Table View Settings",
                            [

                                { Text: 'Ok',
                                    Click: function () {
                                        history.back();
                                    }
                                }
                            ]
                        );
                        return;
                    }

                    // put the images into an array
                    var i = 0;

                    $('#container > img').each(function () {
                      
                        objectArray[i] = {
                            ObjectHtmlID: $(this).attr('id'),
                            PositionTop: (parseFloat($(this).position().top) / parseFloat($('#container').height())) * 100,
                            PositionLeft: (parseFloat($(this).position().left) / parseFloat($('#container').width())) * 100,
                            ImageWidth: $(this).width(),
                            ImageHeight: $(this).height()
                        };

                        i = i + 1;
                    });

                    SaveTableViewSettings();


                });

                var objectArray = [];

                function SaveTableViewSettings() {

                    var pData = { ID: '', SectionName: $.trim($('#txtTemplateName').val()), FowstID: $('#selFowst').val(), FloorArea: $('#selFloorArea').val(), ObjectDataList: objectArray, IsActive: 1
                    }

                    AjaxSend({
                        url: 'TableView/CheckUniqueTableViewTemplateName',
                        data: pData,
                        success: function (vDataCheck) {

                            if (vDataCheck.TemplateData.Rows.length > 0) {

                                MessageBox("There is already an existing template with the same section name.", "Table View Settings");
                                return;
                            } else {
                                AjaxSend({
                                    url: 'TableView/SaveTableViewSettings',
                                    data: pData,
                                    success: function (vData) {
                                        //                                        loadObjectByType(arrObject[0], arrObject[1], $(this).attr('id'), topComputed, leftComputed, $(this).width(), $(this).height());
                                        alert('saved');
                                        if (vData.Error != undefined) {
                                            MessageBox(vData.Error);
                                        };
                                    },
                                    error: function (vError) {
                                        var pDummy;
                                    }
                                });
                            } //end if
                        },
                        error: function (vError) {
                            var pDummy;
                        }
                    });




                }

                // create counter for each kind
                var roundTable2SeaterVerticalIdCtr = 1;
                var roundTable2SeaterIdCtr = 1;
                var roundTable4SeaterIdCtr = 1;
                var roundTable4Seater150pxIdCtr = 1;
                var roundTable8SeaterIdCtr = 1;
                var roundTable10SeaterIdCtr = 1;
                var roundTable12SeaterIdCtr = 1;
                var horizontalRectangle100x100IdCtr = 1;
                var horizontalRectangle100x200IdCtr = 1;
                var horizontalRectangle100x300IdCtr = 1;
                var horizontalRectangle100x400IdCtr = 1;
                var horizontalRectangle100x500IdCtr = 1;
                var rectangleTable100x100pixel4SeaterIdCtr = 1;
                var verticalRectangle100x100IdCtr = 1;
                var verticalRectangle100x200IdCtr = 1;
                var verticalRectangle100x300IdCtr = 1;
                var verticalRectangle100x400IdCtr = 1;
                var verticalRectangle100x500IdCtr = 1;
                var barStoolAvailableIdCtr = 1;
                var door1IdCtr = 1;
                var door2IdCtr = 1;
                var door3IdCtr = 1;
                var door4IdCtr = 1;
                var door5IdCtr = 1;
                var door6IdCtr = 1;
                var door7IdCtr = 1;
                var door8IdCtr = 1;
                var glassWindowHorizontalIdCtr = 1;
                var glassWindowVerticalIdCtr = 1;
                var pOSCounterLongHorizontalRectangleIdCtr = 1;
                var pOSCounterLongVerticalRectangleIdCtr = 1;
                var pOSCounterMediumHorizontalRectangleIdCtr = 1;
                var pOSCounterMediumVerticalRectangleIdCtr = 1;
                var pOSCounterSquareIdCtr = 1;
                var wallPaneHorizontalIdCtr = 1;
                var wallPaneVerticalIdCtr = 1;
                var barLongHorizontalIdCtr = 1;
                var barLongVerticalIdCtr = 1;
                var barShortHorizontalIdCtr = 1;
                var barShortVerticalIdCtr = 1;

                $('#selFloorArea').val('25').attr('selected');
                $('#selFloorArea').selectmenu();
                $('#selFloorArea').selectmenu('refresh');

                $('#btnRoundTable2SeaterVertical').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="RoundTableVertical_2_' + roundTable2SeaterVerticalIdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/100 pixel Round Table 2 Seater Vertical.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());

                    roundTable2SeaterVerticalIdCtr = roundTable2SeaterVerticalIdCtr + 1;


                });

                $('#btnRoundTable2Seater').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="RoundTable_2_' + roundTable2SeaterIdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/100 pixel Round Table 2 Seater.png"/>'));
                    $('.img1').drags();

                    setImageSize($('#selFloorArea').val());

                    roundTable2SeaterIdCtr = roundTable2SeaterIdCtr + 1;
                });


                $('#btnRoundTable4Seater').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="RoundTable_4_' + roundTable4SeaterIdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/100 pixel Round Table 4 Seater.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());
                    roundTable4SeaterIdCtr = roundTable4SeaterIdCtr + 1;
                });


                $('#btnRoundTable4Seater150px').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="RoundTable150px_4_' + roundTable4Seater150pxIdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/150 pixel Round Table 4 Seater.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());
                    roundTable4Seater150pxIdCtr = roundTable4Seater150pxIdCtr + 1;
                });


                $('#btnRoundTable8Seater').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="RoundTable_8_' + roundTable8SeaterIdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/200 pixel Round Table 8 Seater.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());
                    roundTable8SeaterIdCtr = roundTable8SeaterIdCtr + 1;
                });

                $('#btnRoundTable10Seater').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="RoundTable_10_' + roundTable10SeaterIdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/300 pixel Round Table 10 Seater.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());
                    roundTable10SeaterIdCtr = roundTable10SeaterIdCtr + 1;
                });

                $('#btnRoundTable12Seater').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="RoundTable_12_' + roundTable12SeaterIdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/350 pixel Round Table 12 Seater.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());
                    roundTable12SeaterIdCtr = roundTable12SeaterIdCtr + 1;
                });

                $('#btnHorizontalRectangle100x100').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="HorizontalRectangle_2_' + horizontalRectangle100x100IdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/Horizontal Rectangle Table 100x100 pixel.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());
                    horizontalRectangle100x100IdCtr = horizontalRectangle100x100IdCtr + 1;
                });

                $('#btnHorizontalRectangle100x200').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="HorizontalRectangle_6_' + horizontalRectangle100x200IdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/Horizontal Rectangle Table 100x200 pixel.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());
                    horizontalRectangle100x200IdCtr = horizontalRectangle100x200IdCtr + 1;
                });

                $('#btnHorizontalRectangle100x300').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="HorizontalRectangle_8_' + horizontalRectangle100x300IdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/Horizontal Rectangle Table 100x300 pixel.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());
                    horizontalRectangle100x300IdCtr = horizontalRectangle100x300IdCtr + 1;
                });

                $('#btnHorizontalRectangle100x400').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="HorizontalRectangle_10_' + horizontalRectangle100x400IdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/Horizontal Rectangle Table 100x400 pixel.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());
                    horizontalRectangle100x400IdCtr = horizontalRectangle100x400IdCtr + 1;
                });

                $('#btnHorizontalRectangle100x500').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="HorizontalRectangle_12_' + horizontalRectangle100x500IdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/Horizontal Rectangle Table 100x500 pixel.png"/>'));
                    $('.img1').drags();
                    setImageSize(25);
                    horizontalRectangle100x500IdCtr = horizontalRectangle100x500IdCtr + 1;
                });

                $('#btnRectangleTable100x100pixel4Seater').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="RectangleTable_4_' + rectangleTable100x100pixel4SeaterIdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/Rectangle Table 100x100 pixel 4 Seater.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());
                    rectangleTable100x100pixel4SeaterIdCtr = rectangleTable100x100pixel4SeaterIdCtr + 1;
                });

                $('#btnVerticalRectangle100x100').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="VerticalRectangle_2_' + verticalRectangle100x100IdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/Vertical Rectangle Table 100x100 pixel.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());
                    verticalRectangle100x100IdCtr = verticalRectangle100x100IdCtr + 1;
                });

                $('#btnVerticalRectangle100x200').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="VerticalRectangle_6_' + verticalRectangle100x200IdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/Vertical Rectangle Table 100x200 pixel.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());
                    verticalRectangle100x200IdCtr = verticalRectangle100x200IdCtr + 1;
                });

                $('#btnVerticalRectangle100x300').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="VerticalRectangle_8_' + verticalRectangle100x300IdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/Vertical Rectangle Table 100x300 pixel.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());
                    verticalRectangle100x300IdCtr = verticalRectangle100x300IdCtr + 1;
                });

                $('#btnVerticalRectangle100x400').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="VerticalRectangle_10_' + verticalRectangle100x400IdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/Vertical Rectangle Table 100x400 pixel.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());
                    verticalRectangle100x400IdCtr = verticalRectangle100x400IdCtr + 1;
                });


                $('#btnVerticalRectangle100x500').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="VerticalRectangle_12_' + verticalRectangle100x500IdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/Vertical Rectangle Table 100x500 pixel.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());
                    verticalRectangle100x500IdCtr = verticalRectangle100x500IdCtr + 1;
                });


                $('#btnBarStoolAvailable').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="BarStoolAvailable_1_' + barStoolAvailableIdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/Bar Stool - Available.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());
                    barStoolAvailableIdCtr = barStoolAvailableIdCtr + 1;
                });

                $('#btnDoor1').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="Door1_0_' + door1IdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/Door 1.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());
                    door1IdCtr = door1IdCtr + 1;
                });

                $('#btnDoor2').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="Door2_0_' + door2IdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/Door 2.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());
                    door2IdCtr = door2IdCtr + 1;
                });

                $('#btnDoor3').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="Door3_0_' + door3IdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/Door 3.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());
                    door3IdCtr = door3IdCtr + 1;
                });

                $('#btnDoor4').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="Door4_0_' + door4IdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/Door 4.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());
                    door4IdCtr = door4IdCtr + 1;
                });


                $('#btnDoor5').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="Door5_0_' + door5IdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/Door 5.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());
                    door5IdCtr = door5IdCtr + 1;
                });


                $('#btnDoor6').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="Door6_0_' + door6IdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/Door 6.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());
                    door6IdCtr = door6IdCtr + 1;
                });



                $('#btnDoor7').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="Door7_0_' + door7IdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/Door 7.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());
                    door7IdCtr = door7IdCtr + 1;
                });



                $('#btnDoor8').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="Door8_0_' + door8IdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/Door 8.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());
                    door8IdCtr = door8IdCtr + 1;
                });



                $('#btnGlassWindowHorizontal').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="GlassWindowHorizontal_0_' + glassWindowHorizontalIdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/Glass Window - Horizontal.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());
                    glassWindowHorizontalIdCtr = glassWindowHorizontalIdCtr + 1;
                });



                $('#btnGlassWindowVertical').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="GlassWindowVertical_0_' + glassWindowVerticalIdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/Glass Window - Vertical.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());
                    glassWindowVerticalIdCtr = glassWindowVerticalIdCtr + 1;
                });

                $('#btnPOSCounterLongHorizontalRectangle').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="POSCounterLongHorizontalRectangle_0_' + pOSCounterLongHorizontalRectangleIdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/POS Counter Long Horizontal Rectangle.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());
                    pOSCounterLongHorizontalRectangleIdCtr = pOSCounterLongHorizontalRectangleIdCtr + 1;
                });



                $('#btnPOSCounterLongVerticalRectangle').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="POSCounterLongVerticalRectangle_0_' + pOSCounterLongVerticalRectangleIdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/POS Counter Long Vertical Rectangle.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());
                    pOSCounterLongVerticalRectangleIdCtr = pOSCounterLongVerticalRectangleIdCtr + 1;
                });



                $('#btnPOSCounterMediumHorizontalRectangle').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="POSCounterMediumHorizontalRectangle_0_' + pOSCounterMediumHorizontalRectangleIdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/POS Counter Medium Horizontal Rectangle.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());
                    pOSCounterMediumHorizontalRectangleIdCtr = pOSCounterMediumHorizontalRectangleIdCtr + 1;
                });




                $('#btnPOSCounterMediumVerticalRectangle').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="POSCounterMediumVerticalRectangle_0_' + pOSCounterMediumVerticalRectangleIdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/POS Counter Medium Vertical Rectangle.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());
                    pOSCounterMediumVerticalRectangleIdCtr = pOSCounterMediumVerticalRectangleIdCtr + 1;
                });



                $('#btnPOSCounterSquare').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="POSCounterSquare_0_' + pOSCounterSquareIdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/POS Counter Square.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());
                    pOSCounterSquareIdCtr = pOSCounterSquareIdCtr + 1;
                });



                $('#btnWallPaneHorizontal').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="WallPaneHorizontal_0_' + wallPaneHorizontalIdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/Wall Panel - Horizontal.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());
                    wallPaneHorizontalIdCtr = wallPaneHorizontalIdCtr + 1;
                });



                $('#btnWallPaneVertical').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="WallPaneVertical_0_' + wallPaneVerticalIdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/Wall Panel - Vertical.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());
                    wallPaneVerticalIdCtr = wallPaneVerticalIdCtr + 1;
                });

                $('#btnBarLongHorizontal').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="BarLongHorizontal_0_' + barLongHorizontalIdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/Bar - Long Horizontal.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());
                    barLongHorizontalIdCtr = barLongHorizontalIdCtr + 1;
                });


                $('#btnBarLongVertical').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="BarLongVertical_0_' + barLongVerticalIdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/Bar - Long Vertical.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());
                    barLongVerticalIdCtr = barLongVerticalIdCtr + 1;
                });


                $('#btnBarShortHorizontal').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="BarShortHorizontal_0_' + barShortHorizontalIdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/Bar - Short Horizontal.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());
                    barShortHorizontalIdCtr = barShortHorizontalIdCtr + 1;
                });


                $('#btnBarShortVertical').click(function (event) {
                    event.preventDefault();

                    $('#container').append($('<img id="BarShortVertical_0_' + barShortVerticalIdCtr + '" class="img1" src="/Content/images/Table and Chairs for Table View Mode/Bar - Short Vertical.png"/>'));
                    $('.img1').drags();
                    setImageSize($('#selFloorArea').val());
                    barShortVerticalIdCtr = barShortVerticalIdCtr + 1;
                });


                $('#selFloorArea').change(function (event) {

                    setImageSize($('#selFloorArea').val());
                });


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

                function GetFowst() {
                    var data = { FilterFowst: 0
                    }

                    AjaxSend({
                        url: 'Browser/LibraryFowst',
                        data: data,
                        success: function (vData) {
                            var vRows = vData.Results.Rows;

                            $('#selFowst').empty();

                            $('#selFowst').append(new Option('Please Select', ''));
                            for (var p = 0; p <= vRows.length - 1; p++) {
                                $('#selFowst').append(new Option(vRows[p].FowstName, vRows[p].ID, false, false));
                            }


                            $('#selFowst').selectmenu();

                            $('#selFowst').selectmenu('refresh', true);

                        }
                    });
                }

                ////////////////

            }); // $(function() {

        }); // require offline 
    });                                                                                                                                                                                                                                                                                                                                                                                             // require require


