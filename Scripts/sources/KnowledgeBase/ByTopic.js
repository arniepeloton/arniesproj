require([
    'common',
    '/Scripts/sources/crm/qrcode.js',    
], function () {
    require(['/Content/bootstrap/js/bootstrap.js?crc=', '/Scripts/jquery-sortable.js'])

    var arrFilter = [], filterVal = '', productname = '';

    $(document).ready(function () {

        //        if (sessionStorage.IsMainSearchOk != undefined) {
        //            $('#txtMainSearch').val(sessionStorage.txtMainSearch);
        //            MainSearch()
        //        }

        var $cache = $('.divSearchBox');       

        //store the initial position of the element
        var vTop = $cache.offset().top - parseFloat($cache.css('margin-top').replace(/auto/, 0));
        $(window).scroll(function (event) {
            // what the y position of the scroll is
            var y = $(this).scrollTop();

            // whether that's below the form
            if (y >= vTop) {
                // if so, ad the fixed class
                $cache.addClass('stuck');
            } else {
                // otherwise remove it
                $cache.removeClass('stuck');
            }
        });

        LoadRequiredData();

        $('#btnHome').click(function () {
            $('#pageMain').pageshow();
        })

        $('#txtMainSearch').keypress(function (e) {
            if (e.which == 13) {
                if ($.trim($(this).val()) != "") {
                    MainSearch()
                }
                else {
                    $(this).focus()
                }
            }
        })

        $('#btnMainSearch').click(function () {
            if ($.trim($('#txtMainSearch').val()) != "") {
                MainSearch()
            }
            else {
                $('#txtMainSearch').focus()
            }
        })

        $('.txtMainSearchResult').keypress(function (e) {
            if (e.which == 13) {
                if ($.trim($(this).val()) != "") {
                    MainSearch()
                }
                else {
                    $(this).focus()
                }
            }
        })

        $('#selFilterTopic').change(function (e) {
            if ($(this).val() != '') {
                GetAllFilterSubTopics($(this).val());
            } else {
                $('#selFilterSubTopic').empty().append('<option value="">Please Select<option>').attr('disabled', 'disabled');;
            }
        });

        $('#btnOkFilter').click(function () {
            var isSelectEmpty = true
            $('#formFilter select').each(function () {
                if ($(this).val() != "") {
                    isSelectEmpty = false;
                }
            })

            if (isSelectEmpty == true) {
                WebToast('Please fill at least one dropdown', 'top');
                return;
            } else {
                AddFilter();
                $('#modFilter').modal('hide')
            }
        })

        $('#btnAnswerDetailsBack').click(function () {
            $('#divMultiAnswers').pageshow();
        })
    }) // end $(document)

    function AddFilter() {
        var topic = $('#selFilterTopic');
        var subtopic = $('#selFilterSubTopic');
        var product = $('#selFilterProduct');
        var language = $('#selFilterLanguage');

        var tempArr = [topic, subtopic, product, language];
        arrFilter = [];

        for (var i = 0; i < tempArr.length; i++) {
            var isExist = false;
            var type = '';

            if (arrFilter.length > 0) {
                $.grep(arrFilter, function (e) {
                    if (CStr2(e.ID) == tempArr[i].val()) {
                        isExist = true;
                    }
                })
            }

            if (isExist == false && (tempArr[i].val() || '') != "") {
                if (i == 0) {
                    type = 'topic';
                } else if (i == 1) {
                    type = 'subtopic';
                } else if (i == 2) {
                    type = 'product';
                } else if (i == 3) {
                    type = 'language';
                }
                arrFilter.push({
                    id: tempArr[i].val(),
                    description: tempArr[i].find('option:selected').text(),
                    type: type
                })
            }
        }

        DisplayFilter();
        FilteredText();
    }

    function FilteredText() {
        filterVal = '';
        for (var i = 0; i < arrFilter.length; i++) {
            filterVal = filterVal + (i > 0 ? '||' : ''); // for splitting purposes only
            filterVal = filterVal + arrFilter[i].type + '\/' + arrFilter[i].description + '\/' + arrFilter[i].id
        }
        
        filterVal = (arrFilter.length == 0 ? "" : filterVal);
        MainSearch();
    }

    function DisplayFilter() {
        var listFilteredBy = $('#listFilteredBy');

        listFilteredBy.empty();

        for (var i = 0; i < arrFilter.length; i++) {
            listFilteredBy.append($('<div class="chip-plain">')
                            .append($('<div>')
                                .attr('title', arrFilter[i].description)
                                .text(arrFilter[i].description)
                            )
                            .append($('<span class="closebtn">')
                                .attr('i', i)
                                .text('×')
                                .click(function () {
                                    arrFilter.splice($(this).attr('i'), 1);
                                    DisplayFilter();
                                    FilteredText();
                                })
                            )
                        )
        }
    }

    function MainSearch() {
        setHeightContainer()
        var valSearch = $.trim($('#txtMainSearch').val());

        $('.txtMainSearchResult').val(valSearch);
        //$('#divMainSearchResults').pageshow();

        $('#pageSearchList').pageshow();
        $('#pageInfo').text('Searching keywords ...').pageshow();

        AjaxSend({
            url: 'KnowledgeBase/GetSearchResult',
            data: { SearchKey: valSearch, Filter: filterVal },
            success: function (vData) {                
                DisplaySearchResults(vData.Results.Rows);
            }
        })
    }

    function DisplaySearchResults(vRows) {
        var lstSearchResults = $('#listKbSearchResult');

        lstSearchResults.empty();

        if (vRows.length > 0) {
            $('#pageSuccess').pageshow();
            for (var i = 0; i < vRows.length; i++) {
                var ctrSolution = 0;
                var strSolution = '';

                if (vRows[i].Solution != undefined) {
                    strSolution = $.trim(vRows[i].Solution);
                }
                //'<span>' + vRows[i].Product + ' - </span>' + 
                lstSearchResults.append($('<li>')
                    .data(vRows[i])
                    .append($('<div id="' + i + '" class="searchResult">')
                        .append($('<div>')
                            .html($.trim(vRows[i].Problem))
                        )
                        .append($('<p>')
                            .text(strSolution)
                        )
                    )
                    .click(function () {
                        ViewTopic($(this).data())
                    })
                )

                $('#' + i).ellipse();
            }
        } else {
            $('#pageInfo').text('Sorry! No matching keywords found.').pageshow();
        }
    }

    function ViewTopic(data) {
        $('#myTopicLabel').text((data.Problem).toUpperCase())
        AjaxSend({
            url: 'KnowledgeBase/GetProblemSolution',
            data: { ProblemID: data.ProblemID },
            success: function (vData) {
                var listAnswers = $('#listAnswers');

                sessionStorage.OrgID = vData[0].OrgID;
                
                listAnswers.empty();                

                if (vData.length == 1) {
                    vData[0].Problem = data.Problem;
                    vData[0].ProblemID = data.ProblemID;
                    $('#btnAnswerDetailsBack').hide();
                    AnswerDetails(vData)
                }
                else {
                    $('#divMultiAnswers').pageshow();
                    for (var i = 0; i < vData.length; i++) {
                        vData[i].Problem = data.Problem;
                        vData[i].ProblemID = data.ProblemID;
                        listAnswers.append($('<li>')
                            .data(vData[i])
                            .append($('<div id="ans' + i + '" class="searchResult">')
                                //.append($('<div>')
                                //    .html(i + '.')
                                //)
                                .append($('<p>')
                                    .text($.trim(vData[i].Solution))
                                )
                            )
                            .click(function () {
                                $('#btnAnswerDetailsBack').show();
                                AnswerDetails([$(this).data()])
                            })
                        )

                        setTimeout(function (i) {
                            $('#ans' + i).ellipse();
                        }, 200, i)
                    }
                }

                $('#modViewTopic').modal('show')
            }
        })
    }

    function HTMLDecode(str) {
        var htmlstr = $('<textarea />').html(str).text();
        return htmlstr;
    }

    function AnswerDetails(vData) {
        var listSolution = $('<ol type="A" style="line-height:1.9">');
        var listAnswerDetails = $('#listAnswerDetails');
        var dRow = $('<div class="row">')
        var listSteps = $('<ol>')
        var ansLink = 'kb.mobiz-apps.com' + MergeUrlParameters({
            t: vData[0].ProblemID,
            a: vData[0].SolutionID
        }, 'KnowledgeBase')

        $('#divAnswerDetails').pageshow();

        listAnswerDetails.empty();

        for (m = 0; m < vData[0].Step.length; m++) {
            listSteps.append($('<li>')
                    .html(HTMLDecode(vData[0].Step[m].Step))
                )
        }

        dRow.append($('<div class="col-md-8">')
                    .append($('<li>')
                    .append(vData[0].Solution.replace(/\r\n|\r|\n/g, "<br />"))
                    .append(listSteps)
                )
            )
            .append($('<div class="col-md-4">')
                .append($('<div id="bc' + 0 + '" style="margin-top:10px;">'))
                .append('<br>')
                .append($('<div class="input-group">')
                    .append($('<input id="link-' + 0 + '" data-role="none" type="text" class="form-control" placeholder="Answer Link" onclick="this.select()" readonly>')
                        .val(ansLink)
                    )
                    .append($('<span class="input-group-btn">')
                        .append($('<button id="btn-' + 0 + '" data-role="none" class="btn btn-primary" type="button" data-toggle="tooltip" data-placement="bottom" title="Click to copy link">')
                            .append($('<span class="glyphicon glyphicon-link" aria-hidden="true">'))
                            .click(function () {
                                copyToClipboard($(this).attr('id').split('-')[1])
                            })
                        )
                    )
                )
            )

        listSolution.append(dRow)

        listAnswerDetails.append(listSolution)

        for (m = 0; m < vData[0].Step.length; m++) {
            for (var n = 0; n < vData[0].Step[m].Images.length; n++){
                $('[imgid="' + vData[0].Step[m].Images[n].ImgID + '"]').attr('src', '../../../KBPictures/' + vData[0].Step[m].Images[n].Location)
            }
        }

        MakeQRCode('bc' + 0, ansLink)
    }

    function copyToClipboard(element) {
        $('#link-' + element).select();
        document.execCommand("copy");
        WebToast('Copied to clipboard');
    }

    function MakeQRCode(div, msg) {
        var qrcode;
        $('#' + div).empty();

        qrcode = new QRCode(document.getElementById(div), {
            width: 200,
            height: 200
        });

        qrcode.makeCode(msg);
    }

    function ViewSearchResults(results) {
        $('#divViewSearchResult').pageshow();

        for (var m = 0; m < solution; m++) {
            ol.append($('<li style="margin:10px;">').text(vRows[i].Solution.split(',')[m]));
        }
    }

    function setHeightContainer() {
        $('#mainContainer').css('margin-top', $('#divHeaderForProjects').height());
    }

    function LoadRequiredData() {
        $('#txtMainSearch').focus();
        //GetStartupData();
        //GetAllTopics();
        //GetAllProducts();
        //GetAllLanguages();

        var param = GetUrlParameters()
        if (param.tid != undefined && param.p != undefined) {
            productname = param.p;

            GetTopicCases(param)
        }

        //$('[data-toggle="tooltip"]').tooltip()
    }

    function GetTopicCases(param) {
        var pageByTopic = $('#pageByTopic');
        
        pageByTopic.empty().append('<h3>Please wait ...</h3>');

        AjaxSend({
            url: 'KnowledgeBase/GetTopicCases',
            data: { ID: param.tid },
            success: function (vData) {
                pageByTopic.empty()

                var ul = $('<ul class="custom-list">');

                if (vData.SubTopic.length > 0 || vData.NoSubTopic.length > 0) {
                    if (vData.NoSubTopic.length > 0) {
                        var li = ul.append($('<li>')
                                    .append($('<h3>')
                                        .text(param.p + ' > ' + vData.Topic)
                                    )                                    
                                )
                        var ultopic = $('<ol>')
                        for (var i = 0; i < vData.NoSubTopic.length; i++) {
                            ultopic.append($('<li>')
                                    .append($('<a>')
                                        .append($('<h4>')
                                            .text(vData.NoSubTopic[i].Problem)
                                        )
                                    )
                                )
                        }

                        li.append(ultopic)
                    }

                    if (vData.SubTopic.length > 0) {
                        for (var i = 0; i < vData.SubTopic.length; i++) {
                            var li = ul.append($('<li>')
                                    .append($('<h3>')
                                        .text(param.p + ' > ' + vData.Topic + ' > ' + vData.SubTopic[i].SubTopic)
                                    )
                                )

                            var ultopic = $('<ol>')
                            for (var m = 0; m < vData.SubTopic[i].Problem.length; m++) {
                                ultopic.append($('<li>')
                                    .append($('<a>')
                                        .append($('<h4>')
                                            .text(vData.SubTopic[i].Problem[m].Problem)
                                        )
                                    )
                                )
                            }

                            li.append(ultopic)
                        }
                    }

                    pageByTopic.append(ul)
                }
            }
        })
    }

    function GetStartupData() {
        var divStartup = $('#divStartup');

        divStartup.empty();

        AjaxSend({
            url: 'KnowledgeBase/GetStartupData',
            success: function (vData) {
                if (vData.length > 0){
                    for (var i = 0; i < vData.length; i++){
                        var parentDiv = $('<div>')

                        if(vData[i].Topic.length > 0){
                            parentDiv.append($('<h3>')
                                            .text(vData[i].Name)
                                        )

                            //parentDiv.append('<br/>')

                            var ctrGrid = 1;
                            var divRow = $('<div class="row">')
                            for (var m = 0; m < vData[i].Topic.length; m++) {
                                if (ctrGrid == 5) {                                    
                                    parentDiv.append(divRow)
                                    //parentDiv.append('<br/>')
                                    divRow = $('<div class="row">') // New Row
                                    ctrGrid = 1;
                                    //divRow.append('<br/><br/><br><br><br><br><br>')
                                }

                                if (m == (vData[i].Topic.length - 1)) {
                                    parentDiv.append(divRow)
                                }

                                divRow.append($('<div class="col-sm-3">')
                                        .data(vData[i].Topic[m])
                                        .append('<br>')
                                        .append($('<div class="kbTopicBtn custom-theme-l">')
                                            .text(vData[i].Topic[m].Topic)
                                        )
                                        .click(function () {
                                            window.open('http://localhost:8080/KnowledgeBase/ByTopic?tid=' + $(this).data('ID') , '_blank');
                                        })
                                    )

                                ctrGrid += 1;
                            }

                            parentDiv.append(divRow)
                        }

                        divStartup.append('<br/>')
                        divStartup.append(parentDiv)
                    }
                }                
            }
        })
    }

    function GetLinkedDetails(param) {
        AjaxSend({
            url: 'KnowledgeBase/GetLinkedDetails',
            data: param,
            success: function (vData) {
                var linkDetails = $('#linkDetails');
                var lstSolution = $('<ol type="A" class="list-padding">')

                $('#pageViaLink').pageshow();

                if(vData[0].Error == undefined){
                    linkDetails.empty()

                    sessionStorage.OrgID = vData[0].OrgID

                    for (var i = 0; i < vData[0].Solution.length; i++) {
                        var lstSteps = $('<ol type="1">');
                        for (var m = 0; m < vData[0].Solution[i].Step.length; m++) {
                            lstSteps.append($('<li>')
                                    .html(HTMLDecode(vData[0].Solution[i].Step[m].Step))
                                )
                        }

                        lstSolution.append($('<li>')
                                .html(vData[0].Solution[i].Solution.replace(/\r\n|\r|\n/g, "<br />"))
                                .append(lstSteps)
                            )
                    }

                    $('#txtProblem').text(vData[0].Problem)

                    linkDetails.append(lstSolution)

                    for (var i = 0; i < vData[0].Solution.length; i++) {
                        for (var m = 0; m < vData[0].Solution[i].Step.length; m++) {
                            for (var n = 0; n < vData[0].Solution[i].Step[m].Images.length; n++) {
                                $('[imgid="' + vData[0].Solution[i].Step[m].Images[n].ImgID + '"]').attr('src', '../../../KBPictures/' + vData[0].Solution[i].Step[m].Images[n].Location)
                            }
                        }
                    }
                } else {
                    $('#txtProblem').text(vData[0].Error)
                }                
            }
        })
    }

    function GetAllTopics() {
        AjaxSend({
            url: 'KnowledgeBase/GetAllTopics',
            success: function (vData) {
                var vRows = vData.Results.Rows;

                $('#selFilterTopic').empty();

                $('#selFilterTopic').append(new Option('Please Select', '', true));

                if (vRows.length > 0) {
                    for (var i = 0; i <= vRows.length - 1; i++) {
                        $('#selFilterTopic').append(new Option(vRows[i].Topic, vRows[i].ID, false));
                    }
                }
            }
        });
    }

    function GetAllFilterSubTopics(id) {
        var data = { TopicID: id }
        AjaxSend({
            url: 'KnowledgeBase/GetAllSubTopics',
            data: data,
            success: function (vData) {
                var vRows = vData.Results.Rows;

                $('#selFilterSubTopic').empty();

                $('#selFilterSubTopic').append(new Option('Please Select', '', true));

                if (vRows.length > 0) {
                    for (var i = 0; i <= vRows.length - 1; i++) {
                        $('#selFilterSubTopic').append(new Option(vRows[i].SubTopic, vRows[i].ID, false));
                    }
                    $('#selFilterSubTopic').removeAttr('disabled');
                } else {
                    $('#selFilterSubTopic').attr('disabled', 'disabled');
                }
            }
        });
    }

    function GetAllProducts() {
        AjaxSend({
            url: 'KnowledgeBase/GetAllProducts',
            success: function (vData) {
                var vRows = vData.Results.Rows;
                $('#selFilterProduct').empty();

                $('#selFilterProduct').append(new Option('Please Select', '', false));
                if (vRows.length > 0) {
                    for (var i = 0; i <= vRows.length - 1; i++) {
                        $('#selFilterProduct').append(new Option(vRows[i].Product, vRows[i].ID, false));
                    }
                }
            }
        });
    }

    function GetAllLanguages() {
        AjaxSend({
            url: 'KnowledgeBase/GetAllLanguages',
            success: function (vData) {
                var vRows = vData.Results.Rows;

                $('#selFilterLanguage').empty();

                $('#selFilterLanguage').append(new Option('Please Select', '', false));

                if (vRows.length > 0) {
                    for (var i = 0; i <= vRows.length - 1; i++) {
                        $('#selFilterLanguage').append(new Option(vRows[i].Language, vRows[i].ID, false));
                    }
                }
            }
        });
    }

}) // end require