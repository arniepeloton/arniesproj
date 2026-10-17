require(['common',
    '/Scripts3/Browser/LibraryOrganizationEmployeeFOWST.js?crc='
    ], function () {

        $(function () {

            var mPage;
            var txtSearch;
            var divList;
            var butApplyAll;
            var mData = [];
            var mLV;

            if (window.SalesMenuSalesRepresentatives == null)
                window.SalesMenuSalesRepresentatives = {
                    GetData: function () {
                        return mData;
                    }
                };

            // $('#butSalesRepresentatives, #butAddSalesRepresentatives').click(function() {
            $('body').on('click', '#butSalesRepresentatives, #butAddSalesRepresentatives, #butAssignRep', function () {
                if (!proj.IsBootstrapDesign()) {
                    $('#popupMoreOptions').popup('close');
                }                

                //Show();
                if (Business.SystemDefaults.Only1SalesRep()) {
                    if ($('#lstSalesRep li').length > 0) {
                        MessageBox('There can be only one representative.', 'Assign Sales Representatives');
                        return;
                    }
                    else {
                        BrowseRepresentative();
                    }
                }
                else {
                    BrowseRepresentative();
                }

                if (GetUrlParameters().TableNo != undefined) {
                    setTimeout(function () {
                        $('div[data-url="LibraryOrganizationEmployeeFOWST"]').css({ 'overflow': 'auto' }).children('div[data-role="content"]')
                        .css({ 'position': 'absolute', 'left': 0, 'right': 0 })
                    }, 500)
                    
                }
            }); // END $('#butSalesRepresentatives').click

            if (!proj.IsBootstrapDesign()) {
                AjaxSend({
                    ExactUrl: '/SalesMenu/SalesRepresentatives?cache=1',
                    dataType: 'text',
                    type: 'GET',
                    success: function (vData) {
                        // $('body').append(vData).trigger('create');
                        $('[data-role=page]:first').append(vData).trigger('create');
                        Loaded();
                        App.Deferred.fulfill('BPWQQX7KGV');
                    },
                    error: function () {
                        console.log('js6JWJMWZ4B4! ajax error on SalesRepresentatives');
                        App.Deferred.fulfill('BPWQQX7KGV');
                    }
                });
            }

            function BrowseRepresentative() {
                if (!proj.IsBootstrapDesign()) {
                    Browser.LibraryOrganizationEmployeeFOWST.Click = function (e) {
                        OnClick(e);
                    };
                    Browser.LibraryOrganizationEmployeeFOWST.Show('');
                } else {
                    BrowsePopup('LibraryOrganizationEmployeeFOWST', 'Select Sales Representative', [
                        {
                            Offline: true,
                            AutoSearch: true,
                            DisplayText: [{ Column: 'PeopleName' }],
                            Click: function (e) {
                                OnClick(e);
                            }
                        }
                    ])
                }
            }

            function OnClick(e) {
                if ($('#lstSalesRep li').length > 0) {
                    MessageBox('There can be only one representative.', 'Assign Sales Representatives');
                    return;
                }
                else {
                    mData.push({
                        ID: e.ID,
                        PeopleName: e.PeopleName
                    });
                    Business.TransInvoice.AssignSalesRepresentatives(sessionStorage.CartOrderID, mData); // CartOrderID = invoiceID
                    if ($.isFunction(proj.ShowSalesReps)) {
                        proj.ShowSalesReps();
                    }
                    mData = [];
                }
            }

            function Show() {

                var pElement = mPage;
                pElement.popup('open');
                pElement = pElement.closest('.ui-popup-container');
                if (pElement.length == 0)
                    console.trace('popup container for popupSalesRepresentatives not found');
                else
                // know issue: pop up dimension will revert to default after browser window resize
                    pElement.css({
                        left: 0,
                        width: '80%',
                        margin: 'auto 10%'
                    });

            } // END Show

            function Loaded() {

                mPage = $('#popupSalesRepresentatives');
                txtSearch = mPage.find('[name=txtSearch]:first');
                divList = mPage.find('[name=divList]:first');
                butApplyAll = mPage.find('[name=butApplyAll]:first');
                mLV = mPage.find('ul:first');
                mPage.popup();

                function FindByID(vID) {

                    var pItem;

                    for (var pIndex in mData) {
                        pItem = mData[pIndex];
                        if (ReadyGuid(pItem.ID) == ReadyGuid(vID))
                            return pIndex;
                    }

                    return -1;

                } // END FindByID

                function IsExist(vID) {

                    return (FindByID(vID) != -1);

                } // END IsExist

                function RemoveItem(vID) {

                    var pIndex = FindByID(vID);
                    if (pIndex != -1) {
                        mData.splice(pIndex, 1);
                        LoadList();
                    }
                }

                function LoadList() {

                    var pLnk;
                    var pLI;
                    var pItem;

                    mLV.html('');
                    for (var pIndex in mData) {
                        pItem = mData[pIndex];
                        pLI = $('<li>');

                        pLnk = $('<a>')
                        .attr('href', '#')
                        .data('data', pItem)
                        .append($('<span>')
                            .css('white-space', 'normal')
                            .text(pItem.PeopleName)
                        ).click(function (e) {
                            e.preventDefault();
                            proj.ProductID = $(this).data('data').ID;
                            startclock('#clockStock');
                            ChangePage('Stock');
                            return false;
                        });
                        pLI.append(pLnk);

                        pLnk = $('<a>')
                        .attr('href', "#")
                        .attr('data-icon', "minus")
                        .data('data', pItem)
                        .click(function (e) {
                            e.preventDefault();
                            RemoveItem($(this).data('data').ID);
                        });
                        pLI.append(pLnk);

                        mLV.append(pLI);
                    }

                    mLV.listview('refresh');

                } // END LoadList

                //Edited by Edmund R. Uba - 10/02/15
                $('#popupSalesRepresentatives form:first').submit(function (e) {
                    e.preventDefault();
                    //Check if Only1SalesRep is enabled
                    if (Business.SystemDefaults.Only1SalesRep()) {
                        //Check whether there is already a sales representative in the list
                        if (mData.length > 0) {
                            MessageBox('There should be only one representative.', 'Assign representatives', null, {
                                OnClose: function () {
                                    setTimeout(Show, 100);
                                }
                            });
                        } else {
                            Browser.LibraryOrganizationEmployeeFOWST.Click = function (e) {
                                if (!IsExist(e.ID)) {
                                    mData.push({
                                        ID: e.ID,
                                        PeopleName: e.PeopleName
                                    });
                                    LoadList();
                                }
                                setTimeout(Show, 100);
                            };
                            Browser.LibraryOrganizationEmployeeFOWST.Show(txtSearch.val());
                        }
                    } else {
                        Browser.LibraryOrganizationEmployeeFOWST.Click = function (e) {
                            if (!IsExist(e.ID)) {
                                mData.push({
                                    ID: e.ID,
                                    PeopleName: e.PeopleName
                                });
                                LoadList();
                            }
                            setTimeout(Show, 100);
                        };
                        Browser.LibraryOrganizationEmployeeFOWST.Show(txtSearch.val());
                    }
                    $("#popSalesRepresentatives").popup('open');
                }); // END $('#popupSalesRepresentatives form:first').submit
                //End of Edit

                butApplyAll.click(function () {

                    if (mData.length == 0) {
                        MessageBox('Please assign at least 1 sales representative.', null, null, {
                            OnClose: Show
                        });
                        return;
                    }

                    try {
                        Business.TransInvoice.AssignSalesRepresentatives(sessionStorage.CartOrderID, mData); // CartOrderID = invoiceID
                        mPage.popup('close');
                        if ($.isFunction(proj.ShowSalesReps))
                            proj.ShowSalesReps();
                    } catch (e) {
                        MessageBox('An error occured while assign sales representatives. ' + e.message, null, null, {
                            OnClose: Show
                        });
                    }
                }); // END butApplyAll.click

            } // END Loaded

        }); // $(function() {
    });     // END require
