require(['common'], function () {
    $(document).ready(function () {
        var pageCtr = 1;
        var templateID = '';
        var arrAssignedFowst = [], arrSelectedValidFowst = [], arrSelectedInvalidFowst = [], arrNewlyAddedFowst = [];

        if (typeof VDM == 'undefined') {
            window.VDM = {};
        }

        VDM.PhysicalButtonInit = function () {
            AjaxSend({
                url: 'VendingMachine/GetModelB',
                success: function (e) {
                    var vRows = e.Results.Rows;
                    var selVDMModelBTemplate = $('#selVDMModelBTemplate').empty();

                    selVDMModelBTemplate.append(new Option('Please select', ''));

                    for (var i in vRows) {
                        selVDMModelBTemplate.append($('<option>').data('data', vRows[i]).attr('value', vRows[i].ID).text(vRows[i].Name));
                    }

                    if (CStr2(sessionStorage.ViewID) != '') {
                        GetPhysicalButtonsTemplateInfo();
                    }
                }
            })
        }        

        $('body')
            .on('click', '#btnVDMAddPage', function () {
                if ($('#selVDMModelBTemplate option:selected').val() == '') {
                    NewMessageBox('Please select Model-B Template first before you can add new page.')
                } else {
                    AddPage();
                }
            })
            .on('click', '#btnVDMSavePage', function () {
                var txtVDMPBTemplateName = $.trim($('#txtVDMPBTemplateName').val());
                var txtVDMPBTemplateDescription = $.trim($('#txtVDMPBTemplateDescription').val());
                var selVDMModelBTemplate = $.trim($('#selVDMModelBTemplate').val());
                var arrPages = [], templateerror = false;

                if (txtVDMPBTemplateName == '') {
                    NewMessageBox('Please enter template name')
                    return;
                }

                if (selVDMModelBTemplate == '') {
                    NewMessageBox('Please select Model-B type for this template');
                    return;
                }

                $('.vdmpages').each(function () {                    var dis = $(this);
                    
                    var pageid = dis.attr('pageid');
                    var txtPageName = $.trim(dis.find('.txtPageName').val());
                    var selPagePurpose = $.trim(dis.find('.selPagePurpose option:selected').val());
                    var txtPageContent = $.trim(dis.find('.txtPageContent').val());
                    var arrButtons = [];

                    if (txtPageName == '') {
                        NewMessageBox('Please enter Page name');
                        return false;
                    }

                    if (selPagePurpose == '') {
                        NewMessageBox('Please select purpose for page ' + txtPageName);
                        return false;
                    }

                    if (txtPageContent == '') {
                        NewMessageBox('Please enter content for page ' + txtPageName);
                        return false;
                    }

                    var page = {
                        PageID: pageid,
                        PageName: txtPageName,
                        PagePurpose: selPagePurpose,
                        PageContent: txtPageContent
                    }

                    if (selPagePurpose == 'Product Selection') {
                        var tr = dis.find('tbody tr');
                        var trerror = false;
                        tr.each(function () {
                            var trdis = $(this);
                            var trdata = trdis.data('data');
                            var trbtnno = trdis.attr('buttonno');
                            var trbtnlabel = $.trim(trdis.find('.txtButtonLabel').val());

                            if (trdata != undefined) {
                                if (trbtnlabel == '') {
                                    NewMessageBox('Please provide a label for Button No. ' + trbtnno + ' on the ' + txtPageName + ' page.');
                                    trerror = true;
                                    return false;
                                }

                                trdata.ButtonNo = trbtnno;
                                trdata.ButtonLabel = trbtnlabel;
                                arrButtons.push(trdata);
                            }
                        })

                        if (trerror) {
                            templateerror = true;
                            return false;
                        }

                        if (arrButtons.length == 0) {
                            NewMessageBox('Please assign at least one SKU to your selected button for page ' + txtPageName);
                            templateerror = true;
                            return false;
                        }

                        page.SKUButtons = arrButtons;
                    } else {
                        var tr = dis.find('tbody tr.navbutton');
                        var trerror = false;

                        tr.each(function () {
                            var trdis = $(this);
                            var nav = trdis.find('.selNav option:selected').val();
                            var redirectpage = trdis.find('.selPageList option:selected').val();
                            var trbtnno = trdis.attr('buttonno');
                            var trbtnlabel = $.trim(trdis.find('.txtButtonLabel').val());

                            if (redirectpage == '') {
                                NewMessageBox('Please select a page to redirect to after clicking Navigation Button No. ' + trbtnno + ' on page ' + txtPageName);
                                trerror = true;
                                return false;
                            }

                            if (trbtnlabel == '') {
                                NewMessageBox('Please provide a label for Button No. ' + trbtnno + ' on the ' + txtPageName + ' page.');
                                trerror = true;
                                return false;
                            }

                            arrButtons.push({
                                NavButtonNo: trbtnno,
                                Action: nav,
                                RedirectToPage: redirectpage,
                                ButtonLabel: trbtnlabel,
                            })
                        })

                        if (trerror) {
                            templateerror = true;
                            return false;
                        }

                        page.NavButtons = arrButtons;
                    }

                    arrPages.push(page)
                })

                if (templateerror) return false;

                var arrTemplate = {
                    TemplateID: templateID,
                    Name: txtVDMPBTemplateName,
                    Description: txtVDMPBTemplateDescription,
                    ModelID: selVDMModelBTemplate,
                    Pages: arrPages,
                    AssignedFOWST: arrAssignedFowst
                }
                console.log(arrTemplate)
                AjaxSend({
                    url: 'VendingMachine/SavePhysicalButtons',
                    data: arrTemplate,
                    success: function (e) {

                    }
                })
            })
            .on('click', '#btnVDMSCancelPage', function () {
                window.location.hash = 'LibraryPage';
            })
            .on('click', '#btnVDMAddFOWST', function () {
                arrSelectedValidFowst = [];
                arrSelectedInvalidFowst = [];
                function validate(e) {
                    for (var i = 0; i < e.length; i++) {
                        e[i].State = 'new';

                        if (CStr2(e[i].HBTemplateID) == '') {
                            arrSelectedValidFowst.push(e[i]);
                        } else {
                            arrSelectedInvalidFowst.push(e[i]);
                        }
                    }

                    if (arrSelectedInvalidFowst.length > 0) {
                        NewMessageBox('Some of selected FOWST locations are already assigned to another Hot Button Template.<br><br> What would you like to do?', 'Message', [
                            {
                                Text: 'Add all selected FOWST locations to this template',
                                Click: function () {
                                    ValidateSelectedFowst($.merge(arrSelectedValidFowst, arrSelectedInvalidFowst));
                                }
                            },
                            {
                                Text: 'Do not add selected FOWST locations that already have another assigned Hot Button template',
                                Click: function () {
                                    ValidateSelectedFowst(arrSelectedValidFowst);
                                }
                            },
                            {
                                Text: 'Cancel'
                            }
                        ])
                    } else {
                        ValidateSelectedFowst(arrSelectedValidFowst);
                    }
                }
                PopupButtonList('', 'Options', [
                    {
                        Text: 'Select from Company\'s FOWST list', Click: function () {
                            BrowsePopup('LibraryFOWST', 'Select FOWST Locations', [
                                {
                                    DisplayText: [{ Column: 'FowstName' }],
                                    AutoSearch: true,
                                    SelectMultiple: true,
                                    Message: 'You can select multiple FOWST locations. Click OK when you\'re done.',
                                    Click: function (vData) {
                                        validate(vData);
                                    }
                                }
                            ], { ShowHBTemplate: true, IsAll: true })
                        }
                    }, {
                        Text: 'Select from Group\'s FOWST list', Click: function () {
                            BrowsePopup('LibraryFranchiseGroup', 'Select Group', [
                                {
                                    DisplayText: [{ Column: 'GroupName' }],
                                    AutoSearch: true,
                                    Message: '<b>Note:</b> FOWST location(s) of the selected group will be added on the list.',
                                    Click: function (e) {
                                        Bs.Loading('show');

                                        AjaxSend({
                                            url: 'ItemModifiers/GetFowstOfSelectedGroup',
                                            data: {
                                                GroupID: e.ID,
                                                ShowHBTemplate: true,
                                            }, success: function (e) {
                                                Bs.Loading('hide');
                                                validate(e.Results.Rows);
                                            }
                                        })
                                    }
                                }
                            ])
                        }
                    }
                ])
            })

        function ValidateSelectedFowst(data) {
            if (arrAssignedFowst.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    var isExist = false;

                    $.grep(arrAssignedFowst, function (a) {
                        if (a.ID == data[i].ID) {
                            isExist = true;
                            return false;
                        }
                    })

                    if (!isExist) {
                        arrNewlyAddedFowst.push(data[i]);
                    }
                }
            } else {
                $('#listVDMPBFowst').empty();
                arrNewlyAddedFowst = data;
            }

            arrAssignedFowst = $.merge(arrAssignedFowst, arrNewlyAddedFowst);

            AssignedFOWST(arrAssignedFowst);
        }

        function AssignedFOWST(data) {
            var listVDMPBFowst = $('#listVDMPBFowst');

            if (data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    data[i].Index = i;
                    if (listVDMPBFowst.find('a[fid="' + data[i].ID + '"]').length == 0) {
                        listVDMPBFowst.append($('<a href="#" class="list-group-item list-group-item-action">')
                            .attr('fid', data[i].ID)
                            .append($('<table style="width:100%;">')
                                .append($('<tr>')
                                    .append($('<td style="word-break:break-all;">')
                                        .data('data', data[i])
                                        .append($('<h5>')
                                            .attr("style", "white-space:normal")
                                            .text(data[i].FowstName)
                                        )
                                    )
                                    .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                        .append($('<span class="bi-trash-fill" aria-hidden="true">'))
                                        .data('data', data[i])
                                        .click(function (e) {
                                            e.preventDefault();
                                            var dis = $(this);
                                            var data = dis.data('data');

                                            NewMessageBox('Are you sure you want to remove this FOWST location to this template?', 'Message', [
                                                {
                                                    Text: 'Yes', Click: function () {
                                                        dis.parents().closest('a').remove();
                                                        if (data.State == 'new') {
                                                            arrAssignedFowst.splice(data.Index, 1);
                                                        } else {
                                                            Bs.Loading('show');
                                                            AjaxSend({
                                                                url: '/ProductGroup/RemoveFOWSTHB',
                                                                data: { FOWSTID: data.ID },
                                                                success: function (e) {
                                                                    Bs.Loading('hide');
                                                                    if (e.Result == 'removed') {
                                                                        dis.remove();
                                                                        WebToast('Successfully removed')
                                                                    } else {
                                                                        NewMessageBox(e.Error, 'Error')
                                                                    }
                                                                }
                                                            })
                                                        }
                                                    }
                                                }, { Text: 'No' }
                                            ])
                                        })
                                    )
                                    )
                                )
                            )
                        )
                    }
                }
            } else {
                listVDMPBFowst.append($('<a href="#" class="list-group-item list-group-item-action">').text('No result(s) found'));
            }

            listVDMPBFowst;//.listview('refresh');
        }

        function AddPage(pageData) {
            var newPage = $('#dvPageTemplate').clone();
            var model = $('#selVDMModelBTemplate option:selected').data('data');

            if (typeof pageData == 'undefined') {
                newPage.attr('pageid', NewID())
                newPage.find('.lblPageName').text('Page ' + pageCtr);
            } else {
                newPage.attr('pageid', pageData.ID);
                newPage.find('.lblPageName').text('Page ' + pageCtr + ' - ' + pageData.PageName);
                newPage.find('.txtPageName').val(pageData.PageName);
                newPage.find('.selPagePurpose').val(pageData.PagePurpose);
                newPage.find('.txtPageContent').val(pageData.PageContent);
            }            

            newPage.find('button[data-toggle="collapse"]').attr('data-target', '#collapsepage' + pageCtr);
            newPage.find('.collapse').prop('id', 'collapsepage' + pageCtr);

            newPage.find('.txtPageName').keyup(function () {
                newPage.find('.lblPageName').text('Page ' + newPage.attr('pagectr') + ' - ' + $(this).val());
            })

            newPage.find('.selPagePurpose').change(function () {
                PopulateSelectionButton($(this).val(), newPage, model);
            })
            
            newPage.attr('pagectr', pageCtr).addClass('vdmpages').removeAttr('id').removeClass('d-none');
            $('#divVDMPages').append(newPage);
            //PopulateSelectionButton(newPage.find('.selPagePurpose').val(), newPage, model);
            pageCtr += 1;
            return newPage
        }

        function PopulateSelectionButton(purpose, page, model) {
            var tbl = page.find('.tblPageSelectionButtons tbody');
            var ctrSelBut = CInt2(model.NoOfPhysicalSelectionButtons);
            var ctrNavBut = CInt2(model.NoOfPhysicalNavigationButtons);

            tbl.empty();

            if (purpose == 'Product Selection') {
                for (var i = 0; i < ctrSelBut; i++) {
                    tbl.append($('<tr>')
                        .attr('buttonno', (i + 1))
                        .append($('<td>').text('Button ' + (i + 1)))
                        .append($('<td>').append($('<button data-role="none" class="btn btn-secondary border btn-block">')
                            .text('Choose SKU')
                            .click(function () {
                                var tr = $(this).parents().closest('tr');

                                BrowsePopup('LibraryPIMProduct', 'Select SKU', [
                                    {
                                        Message: 'You can select multiple SKUs.',
                                        DisplayText: [
                                            { Column: 'ProductNo', Label: 'Product No' },
                                            { Column: 'ProductName', Label: 'Name' },
                                            {
                                                Column: function (e) {
                                                    return AccountingFormat(e.RetailPrice);
                                                }, Label: 'Retail Price'
                                            }
                                        ],
                                        Click: function (e) {
                                            tr.data('data', e);
                                            tr.find('button').text(e.ProductName);
                                            tr.find('input').val(e.ProductName);
                                        }
                                    }
                                ])
                            })
                            )
                        )
                        .append($('<td>').append('<input data-role="none" type="text" class="form-control txtButtonLabel" />'))
                    )
                }
            } else {
                tbl.append($('<tr>')
                    .append($('<td>').text('Button 1 to ' + ctrSelBut))
                    .append($('<td>').text('These buttons are dynamically assigned based on the product selected'))
                    .append($('<td>').append(''))
                )

                for (var i = 0; i < ctrNavBut; i++) {
                    tbl.append($('<tr class="navbutton">')
                        .attr('buttonno', (i + 1))
                        .append($('<td>').text('Navigation Button ' + (i + 1)))
                        .append($('<td>')
                            .append($('<div class="input-group">')
                                .append($('<select data-role="none" class="form-select selNav">')
                                    .append('<option value="Back">Back</option>')
                                    .append('<option value="Go">Go</option>')
                                )
                                .append('<span class="input-group-text">to</span>')
                                .append($('<select data-role="none" class="form-select selPageList">'))
                            )
                        )
                        .append($('<td>').append('<input data-role="none" type="text" class="form-control txtButtonLabel" />'))
                    )
                }

                PopulatePages();
            }
        }

        function PopulatePages() {
            var selPageList = $('.selPageList');

            selPageList.each(function () {
                var disSelect = $(this);

                if (disSelect.find('option[value=""]').length == 0) {
                    disSelect.append('<option value="">Please Select</option>');
                }

                $('.vdmpages').each(function () {
                    var dis = $(this);
                    var pageid = dis.attr('pageid');
                    var pagename = dis.find('.lblPageName').text();

                    if (disSelect.find('option[value="' + pageid + '"]').length == 0) {
                        disSelect.append('<option value="' + pageid + '">' + pagename + '</option>')
                    } else {
                        disSelect.find('option[value="' + pageid + '"]').text(pagename);
                    }
                })

                if (disSelect.find('option[value="Payments Page"]').length == 0) {
                    disSelect.append('<option value="Payments Page">Payments Page</option>')
                }
            })
        }

        function GetPhysicalButtonsTemplateInfo() {
            AjaxSend({
                url: 'VendingMachine/GetPhysicalButtonsTemplateInfo',
                data: { ID: sessionStorage.ViewID },
                success: function (e) {
                    var info = e.Info[0];
                    var pages = e.Pages;

                    $('#txtVDMPBTemplateName').val(info.Name);
                    $('#txtVDMPBTemplateDescription').val(info.Description);
                    $('#selVDMModelBTemplate').val(info.ModelID);

                    for (var i in pages) {
                        var html = AddPage(pages[i]);
                        PopulateSelectionButton(pages[i].PagePurpose, html, $('#selVDMModelBTemplate option:selected').data('data'));
                    }
                }
            })
        }

        VDM.PhysicalButtonInit();
    })
})