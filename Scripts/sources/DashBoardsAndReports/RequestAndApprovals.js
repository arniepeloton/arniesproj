require([
    'common', 'tablesort'
], function () {
    $(function () {
        var mContextData;
        var cbApprovalFilterFowst;
        var cbApprovalFilterStatus;
        var cbApprovalFilterType;
        var divSalesReturnList;
        var mIsSearched; 

        if (!window.InAndroid) {
            $('<applet name="jzebra" code="jzebra.PrintApplet.class" archive="/Scripts/jzebra.jar" width="10" height="10"></applet>').appendTo($('body'));
        }
        //            $("[data-role=header]").fixedtoolbar({ tapToggle: false });


        //            var pPage = $.mobile.activePage;
        //            var pHeader = pPage.find("[data-role=header]");
        //            var pHeight = pHeader.height();

        //            pPage.css('padding-top', pHeight + 'px');

        //            pHeader.fixedtoolbar({ tapToggle: false,
        //                updatePagePadding: false    // http://jquerymobile.com/test/docs/toolbars/bars-fixed-options.html
        //                // http://stackoverflow.com/questions/10545540/jquery-mobile-disable-tap-to-toggle-fixed-header-and-footer
        //            });


        breadcrumbs = $('<div class="ItemsBreadCrumbs"></div>'),
        arrowR = ' <span class="ui-icon ui-icon-arrow-r" style="display:inline-block; margin:0px 10px 0px 10px;"></span> ';
        //            $('div[data-role="dialog"]').live('pagebeforeshow', function (e, ui) {
        //                ui.prevPage.addClass("ui-dialog-background ");
        //            });

        //            $('div[data-role="dialog"]').live('pagehide', function (e, ui) {
        //                $(".ui-dialog-background ").removeClass("ui-dialog-background ");
        //            });

        // start tab
        // http://stackoverflow.com/a/6850059/334045
        $(document).delegate('.ui-navbar ul li > a', 'click', function () {

            //un-highlight and highlight only the buttons in the same navbar widget
            $(this).closest('.ui-navbar').find('a').removeClass('ui-navbar-btn-active');

            //this bit is the same, you could chain it off of the last call by using two `.end()`s
            $(this).addClass('ui-navbar-btn-active');

            //this starts the same but then only selects the sibling `.content_div` elements to hide rather than all in the DOM
            $('#' + $(this).attr('data-href')).show().siblings('.content_div').hide();
        });

        $('#butSearchSalesReturn').click(function(e) {
            Search($('#txtSearchSalesReturn').val());
        });

        $('#formRequestAndApprovalSearch').submit(function(e) {
            e.preventDefault();
            Search();
        }); // END $('#formRequestAndApprovalSearch').click(function(e) {

        divSalesReturnList = $('#divSalesReturnList');

        function Search() {
            
            var pData = {
                keywords: $('#txtSearchSalesReturn').val(),
                Fowst: cbApprovalFilterFowst.val(),
                Status: cbApprovalFilterStatus.val(),
                Type: cbApprovalFilterType.val()
            };

            divSalesReturnList.html('');
            AjaxSend({
                url: "/Browser/RequestAndApprovals",
                data: pData,
                success: function(vData) {
                    mIsSearched = true;
                    if (vData.CantViewRequests) {
                        divSalesReturnList.text('You have no access rights to view any requests');
                        return;
                    }

                    if (vData.NoResult) {
                        divSalesReturnList.text('Nothing was found');
                        return;
                    }
                    
                    var pTable = CreateTable(vData.Results, {
                        attr: {
                            'class': 'ui-body-d ui-shadow table-stripe ui-responsive table table-light table-hover table-striped',
                            'data-column-btn-theme': 'b'
                        },
                        theadAttr: {
                            'class': 'custom-theme-f'
                        },
                        AddAllColumns: false,
                        Columns: {
                            RequestType: {
                                Text: 'Request Type'
                            },
                            Reference: {
                                
                            },
                            FowstName: {    
                                Text: 'FOWST'
                            },
                            RequestedBy: {
                                Text: 'Requested By'
                            },
                            RequestedDateTime: {
                                Text: 'Requested Date/Time',
                                DataType: 'date',
                                Format: proj.DateFormat
                            },
                            ApprovedBy: {
                                Text: 'Approved By'
                            },
                            ApprovedDateTime: {
                                Text: 'Approved Date/Time',
                                DataType: 'date',
                                Format: proj.DateFormat
                            }

                            //MemoNumber: {
                            //    Text: 'SR No.',
                            //    DataType: 'number',
                            //    Format: '#,###'
                            //},
                            //LastUpdated: {
                            //    Text: 'Last Updated',
                            //    DataType: 'date'
                            //}
                        },
                        AfterRow: function(e) {
                            e.Row.css('cursor', 'pointer');
                            e.Row.click(function(e1) {
                                mContextData = $($(e1.target).closest('tr')).data('data');
                                if (proj.IsBootstrapDesign()) {
                                    PopupButtonList('', 'Approve Options', [
                                        {
                                            Text: 'Approve', Click: function () {
                                                $('[name=Approve]').click();
                                            }
                                        }, {
                                            Text: 'Disapprove', Click: function () {
                                                $('[name=Disapprove]').click();
                                            }
                                        }, {
                                            Text: 'View', Click: function () {
                                                $('[name=View]').click();
                                            }
                                        }
                                    ])
                                } else {
                                    $('#popApprove').popup('open');
                                }                                
                            });
                        }
                    });
                    divSalesReturnList.append(pTable);
                    pTable.table();
                    pTable.trigger('create');

                    var table = $("table").stupidtable({

                        "date": function (a, b) {
                            // Get these into date objects for comparison.

                            aDate = new Date(a);
                            bDate = new Date(b);

                            return aDate - bDate;
                        }
                    });
                }
            }); // END AjaxSend

        } // END Search

        (function InitPopup() {
            var pPop = $('#popApprove');

            pPop.find('[name=Approve]').click(function (e) {
                Approve({
                    MemoID:mContextData.ID, 
                    Approve: true
                });
                $('#popApprove').popup('close');
            });

            pPop.find('[name=Disapprove]').click(function(e) {
                Approve({
                    MemoID:mContextData.ID, 
                    Approve: false
                });
                $('#popApprove').popup('close');
            });

            // BEGIN if this function is modified, update change to all copies
            // js7DVC6QTAUU!
            function Approve(vParams) {
                /*
                vParams =  {
                    MemoID: 'string-guid',
                    Approve: boolean,
                    Username: 'optional',
                    Password: 'optional',
                    success: function() {} // optional callback
                }
                */
                var pParams = vParams;

                AjaxSend({
                    url: '/SalesReturns/Approve',
                    data: {
                        SalesReturnMemoID: vParams.MemoID,
                        Approve: CBool(vParams.Approve) ? 1 : 0,
                        Username: vParams.Username,
                        Password: vParams.Password
                    },
                    success: function(vData) {
                        if (vData.Error) {
                            MessageBox(vData.Error);
                            return;
                        } else if (vData.Result == 'approved') {
                            MessageBox('Request was successfully approved.');
                            if ($.isFunction(vParams.success))
                                vParams.success();
                        } else if (vData.Result == 'denied') {
                            MessageBox('Request was successfully disapproved.');
                            if ($.isFunction(vParams.success))
                                vParams.success();
                        } else if (vData.NoAccess) {
                            PromptUserAccess('[Approve Request]', {
                                OtherData: {
                                    FowstID: sessionStorage.FowstID
                                },
                                PromptOnly: true,
                                PromptCallback: function(vData) {
                                    Approve($.extend(vParams, vData));
                                }
                            });
                        }
                    }
                });
            } // END function Approve() {
            // END if this function is modified, update change to all copies

            pPop.find('[name=View]').click(function(e) {
                var pUrl;
                pUrl = MergeUrlParameters({
                    OrgID: sessionStorage.OrgID,
                    SalesReturnMemoID: mContextData.ID
                }, '/SalesReturns');
                window.open(pUrl, '_blank');
                $('#popApprove').popup('close');
            });

        })(); // END (function InitPopup() {

        function DisplayFowstFilter() {
            cbApprovalFilterFowst = $('#cbApprovalFilterFowst');
            var pOption;

            cbApprovalFilterFowst.html('');
            pOption = new Option('Loading FOWSTs');
            cbApprovalFilterFowst.append(pOption);
            //cbApprovalFilterFowst.selectmenu();
            //cbApprovalFilterFowst.selectmenu('refresh', true);        
            cbApprovalFilterFowst.change(function(e) {
                Search();
            });

            AjaxSend({
                url: '/Browser/RequestAndApproveFowsts',
                success: function(vData) {
                    var pDR;

                    cbApprovalFilterFowst.html('');
                    if (vData.CantViewRequests) {
                        pOption = new Option('No FOWST access');
                        cbApprovalFilterFowst.append(pOption);
                    } else {
                        pOption = new Option('All FOWST in my Group', 'all');
                        cbApprovalFilterFowst.append(pOption);
                        for (var p = 0; p <= vData.Results.Rows.length - 1; p++) {
                            pDR = vData.Results.Rows[p];
                            pOption = new Option(pDR.Name, pDR.ID);
                            cbApprovalFilterFowst.append(pOption);
                        }
                    }
                    //cbApprovalFilterFowst.selectmenu();
                    //cbApprovalFilterFowst.selectmenu('refresh', true);     
                    if (!mIsSearched)
                        Search();
                }
            });
        }

        function DisplayStatusFilter() {
            cbApprovalFilterStatus = $('#cbApprovalFilterStatus');

            cbApprovalFilterStatus.html('');
            cbApprovalFilterStatus.append(new Option('All pending requests', 'pending'));
            cbApprovalFilterStatus.append(new Option('All requests ever made', 'all'));
            //cbApprovalFilterStatus.selectmenu();
            //cbApprovalFilterStatus.selectmenu('refresh', true); 
            cbApprovalFilterStatus.change(function(e) {
                Search();
            });
        }

        function DisplayTypeFilter() {
            cbApprovalFilterType = $('#cbApprovalFilterType');

            cbApprovalFilterType.html('');
            cbApprovalFilterType.append(new Option('All request types', 'all'));
            cbApprovalFilterType.append(new Option('Only sales return requests', 'returns'));
            cbApprovalFilterType.append(new Option('Only sales discount requests', 'discounts'));
            cbApprovalFilterType.append(new Option('Only device authorization requests', 'device'));
            //cbApprovalFilterType.selectmenu();
            //cbApprovalFilterType.selectmenu('refresh', true); 
            cbApprovalFilterType.change(function(e) {
                Search();
            });

        }

        DisplayFowstFilter();
        DisplayStatusFilter();
        DisplayTypeFilter();

    }); // END $(function () {

});