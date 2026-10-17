require(['common', '/Content/Ecommerce/js/lightbox-2.11.1.js'], function () {
    $(document).ready(function () {
        var mParams;
        var dataTableBrowsePopup, BalancesTable = $('#tblBarcodeCount'), balancesDatatable;
        var mRemoteServerMode = false;
        var mLnkRemoteServerToggle;
        var modal_msgbox = $('<div class="modal fade" id="modal_MessageBox" tabindex="-1" role="dialog" style="overflow-y:auto;">')
                            .append($('<div class="modal-dialog modal-dialog-centered" role="document">')
                                .append($('<div class="modal-content">')
                                    .append($('<div class="modal-header">')
                                        .append('<span data-role="none" id="msgbox_title" class="modal-title" style="font-size:18px;"></span>')
                                        //.append('<button data-role="none" id="btn_msgbox_close" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>')
                                    )
                                    .append($('<div class="modal-body" id="msgbox_body">')

                                    )
                                    .append($('<div class="modal-footer" id="msgbox_footer">'))
                                )
                            )

        var modal_browse = $('<div class="modal fade" id="modal_Browse" tabindex="-1" role="dialog">')
                            .append($('<div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">')
                                .append($('<div class="modal-content">')
                                    .append($('<div class="modal-header">')
                                        .append('<span data-role="none" id="browse_title" class="modal-title" style="font-size:18px;"></span>')
                                        .append('<button data-role="none" id="btn_browse_close" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>')
                                    )
                                    .append($('<div class="modal-body" id="browse_body" style="padding:0;">')                                        
                                        .append('<textarea id="txtBrowseCopyTransToPlainText" style="margin-left:-1000px; position:absolute;"></textarea>')
                                        .append($('<div>')
                                            .append('<div id="divClearBtn" style="display:none; text-align:right; padding:15px 15px 0px 15px;"><a data-role="none" class="custom-link" id="btnCustomClear">Clear</a></div>')
                                            .append($('<div id="BrowseMessage" class="mt-3 mb-3" style="display:none; padding:0px 10px;">'))
                                            .append($('<div id="divSearchBoxBrowseLibrary" class="mt-3 mb-3" style="padding:0px 10px; display:block;">').append($('<table style="width:100%;">')
                                                    .append($('<tr>')
                                                        .append($('<td>')
                                                            .append($('<input data-role="none" type="search" class="form-control form-control-lg unicase-form-control text-input" id="txtBrowseLibrary" placeholder="Search here">'))
                                                        )
                                                        .append($('<td style="width:50px; padding-left: 3px; display:none;" id="divBarcode">')
                                                            .append($('<button data-role="none" title="Scan Barcode" id="btnBrowseScanBarcode" class="btn btn-secondary btn-lg">')
                                                                .append($('<span class="bi-upc-scan">'))
                                                            )
                                                        )
                                                        .append($('<td style="width:120px; padding-left: 3px;" id="divGlobalAddSKU">')
                                                            .append($('<button data-role="none" title="Scan Barcode" id="btnGlobalAddSKU" class="btn btn-secondary btn-lg">')
                                                                .text('Add SKU')
                                                            )
                                                        )
                                                    )
                                                )
                                            )
                                            .append($('<div id="browseOption" class="mb-3" style="display:block; padding:0px 10px; overflow:auto;">')
                                                .append($('<table id="tblOption">'))
                                            )
                                            .append($('<div name="trDefineNewProduct" class="mb-3" style="display:none; padding:0px 10px;">')
                                                .append($('<div>').html('If you’ve correctly spelled your search but can’t find it, you might want to <a class="custom-link">define a new product</a>.'))
                                            )
                                            .append('<div id="divBrowseListFormat" class=" mb-3" style="display:block">' +
                                                       '<div id="browselistresult" class="list-group">' +
                                                       '</div>' +
                                                   '</div>' +
                                                   '<div id="divBrowseTableFormat" class="table-responsive mb-3" style="padding: 0px 15px 15px 15px; overflow:auto; display:none">' +
                                                       '<br-x><table id="tblBrowsePopup" style="display:table; width:100%;" class="table table-striped table-hover table-borderless">' +
                                                           '<thead class="table-secondary align-top text-center" style="font-size:14px;" id="tblBrowsePopupHeader"></thead>' +
                                                           '<tbody id="tblBrowsePopupBody"></tbody>' +
                                                       '</table>' +
                                                   '</div>'
                                            )                                            
                                        )                                        
                                    )
                                    .append($('<div class="modal-footer m-0 p-0" id="divBrowseFooter" style="display:none;">')
                                        .append($('<div id="divNoOfSelectedData" class="border-bottom text-primary font-weight-bold pt-1 pb-1 text-center overflow w-100" style="cursor:pointer;">').text(''))
                                        .append($('<div name="divReturnablesFooter" class="row" style="display:none;">')
                                            .append($('<div>')
                                                .append($('<a name="lnkReturnableFooter" href="">'))
                                            )
                                        )
                                        .append($('<div class="row gx-2 m-0">')
                                            .append($('<div class="col-sm order-2 p-2 order-sm-0">')
                                                .append($('<button data-role="none" data-dismiss="modal" class="btn btn-secondary btn-block btn-lg col-6 h-100">').text('Cancel'))
                                            )
                                            .append($('<div class="col-sm order-0 p-2 order-sm-1">')
                                                .append($('<button data-role="none" id="btnOkBrowseSelectMultiple" class="btn btn-success btn-block btn-lg col-6 h-100">').text('Ok'))
                                            )
                                            .append($('<div class="col-sm order-1 p-2 order-sm-2" style="display:none;">')
                                                .append($('<button data-role="none" id="btnCopyTextBrowseData" class="btn btn-success btn-block btn-lg col-6 h-100">').text('Copy to plain text'))
                                            )
                                        )
                                    )
                                )
                            )

        var modal_contextualmenu = $('<div class="modal fade" id="modal_ContextualMenu" tabindex="-1" role="dialog">')
                            .append($('<div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">')
                                .append($('<div class="modal-content">')
                                    .append($('<div class="modal-header">')
                                        .append('<span data-role="none" id="txtCustomListTitle" class="modal-title" style="font-size:18px;"></span>')
                                        .append('<button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>')
                                    )
                                    .append($('<div class="modal-body" style="padding:0;">')
                                        .append($('<div id="txtCustomListMsg" style="display:none; padding:1rem;">'))
                                        .append($('<div class="p-3" style="display:none;">').append($('<input data-role="none" type="text" id="txtCustomListSearch" class="form-control form-control-lg" placeholder="Search here">')))
                                        .append($('<div id="lstOfCustomList" class="list-group">'))
                                    )
                                    .append($('<div class="modal-footer m-0 p-0" id="divContextualMenuFooter" style="display:none;">')
                                        .append($('<div class="row gx-2 m-0">')
                                            .append($('<div class="col-sm order-2 p-2 order-sm-0">')
                                                .append($('<button data-role="none" data-dismiss="modal" class="btn btn-secondary btn-block btn-lg col-6 h-100">').text('Cancel'))
                                            )
                                            .append($('<div class="col-sm order-0 p-2 order-sm-1">')
                                                .append($('<button data-role="none" id="btnOkContextualMenuSelectMultiple" class="btn btn-success btn-block btn-lg col-6 h-100">').text('Ok'))
                                            )
                                        )
                                    )
                                )
                            )

        var modal_PromptUser = $('<div class="modal fade" id="modal_PromptUser" data-backdrop="static" data-keyboard="false" tabindex="-1" role="dialog">')
                            .append($('<div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">')
                                .append($('<div class="modal-content">')
                                    .append($('<div class="modal-header">')
                                        .append('<span data-role="none" class="modal-title" style="font-size:18px;">Security Access</span>')
                                        //.append('<button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>')
                                    )
                                    .append($('<div class="modal-body">')
                                        .append($('<div class="mb-3">')
                                            .append('<span id="lblPromptUserMsg"></span>' +
                                                '<br> <br> Please enter a valid username and password to gain access to this module or feature.')
                                        )
                                        .append($('<div class="mb-3">')
                                            .append($('<label for="txtPromptUserUsername" class="form-label">').text('Username:'))
                                            .append($('<input data-role="none" type="text" class="form-control" id="txtPromptUserUsername">'))
                                        )
                                        .append($('<div class="mb-3">')
                                            .append($('<label for="txtPromptUserPassword" class="form-label">').text('Password:'))
                                            .append($('<input data-role="none" type="password" class="form-control" id="txtPromptUserPassword">'))
                                        )
                                    )
                                    .append($('<div class="modal-footer">')
                                        .append($('<div class="row">')
                                            .append($('<div class="col-6">')
                                                .append($('<button data-role="none" id="btnPromptUserCancel" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">').text('Cancel'))
                                            )
                                            .append($('<div class="col-6">')
                                                .append($('<button data-role="none" id="btnPromptUserOk" class="btn btn-success btn-block btn-lg">').text('Okay'))
                                            )
                                        )
                                    )
                                )
                            )

        var tooltip = $('<div id="toast" class="toast" role="alert" aria-live="assertive" aria-atomic="true" style="position: fixed; display:none; top: 29px; left: 50%; transform: translate(-50%, 0%); z-index:2000;">')
            .append($('<div class="toast-body" style="background-color:#ffdc73;">')
                .append($('<table>')
                    .append($('<tr>')
                        .append($('<td id="toast_msg" style="font-size:medium;">'))
                        .append($('<td>')
                            .append($('<button type="button" class="ml-2 mb-1 close" data-dismiss="toast" aria-label="Close" style="line-height:0;">')
                                .append($('<span aria-hidden="true" style="line-height:0;">').html('&times;'))
                            )
                        )
                    )
                )
            )

        var loading = $('<div id="bsloading" style="display:none;">').append($('<div style="display:table; width:100%; position:fixed; z-index:2000; top:0; bottom:0; height:100%; background-color: rgb(0 0 0 / 50%);">')
                            .append($('<div style="display:table-cell; vertical-align:middle; text-align:center; color: #FFF; text-shadow: 0px 1px 2px #000; padding:3rem;">')
                                .append($('<div class="spinner-border text-light" role="status">').append($('<span class="visually-hidden">')))
                                .append('<br>')
                                .append($('<span id="bsloadinglbl" style="font-size: 20px;">'))
                            )
                        )

        var modal_calendar = $('<div class="modal fade" id="modal_Calendar" tabindex="-1" role="dialog">')
                            .append($('<div class="modal-dialog modal-dialog-centered" role="document">')
                                .append($('<div class="modal-content">')
                                    .append($('<div class="modal-header">')
                                        .append('<span data-role="none" id="txtCustomSingleCalendarTitle" class="modal-title" style="font-size:18px;">Select Date</span>')
                                        .append('<button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>')
                                    )
                                    .append($('<div class="modal-body">')
                                        .append($('<div id="divClearBtnSingleCalendar" class="mb-3" style="display:block; text-align:right;">')
                                            .append($('<a data-role="none" class="custom-link" id="btnClearSingleCalendar">').text('Clear'))
                                        )
                                        .append($('<div class="metro mb-3" style="width:100%; overflow:hidden;">')
                                            .append($('<div>')
                                                .append('Date: <span id="txtDateSingleCalendar"></span>')
                                                .append($('<div class="fromCustomSingleCalendar calendar mt-3" style="width:100%;" data-other-days="1">'))
                                            )
                                        )
                                        .append($('<div id="divSingleCalendarTime" style="display:none;">')
                                            .append($('<div class="row">')
                                                .append($('<div class="col">').text('Time:'))
                                                .append($('<div class="col">')
                                                    .append($('<select data-role="none" id="selSingleTimeHour" class="form-select">'))
                                                )
                                                .append($('<div class="col">')
                                                    .append($('<select data-role="none" id="selSingleTimeMinute" class="form-select">'))
                                                )
                                                .append($('<div class="col">')
                                                    .append($('<select data-role="none" id="selSingleTimeMeredian" class="form-select">')
                                                        .append('<option value="AM">AM</option>')
                                                        .append('<option value="PM">PM</option>')
                                                    )
                                                )
                                            )
                                        )
                                    )
                                    .append($('<div class="modal-footer">')
                                        .append($('<div class="row">')
                                            .append($('<div class="col-6">')
                                                .append($('<button data-role="none" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">').text('Cancel'))
                                            )
                                            .append($('<div class="col-6">')
                                                .append($('<button data-role="none" id="btnDoneSingleCalendar" class="btn btn-success btn-block btn-lg">').text('Okay'))
                                            )
                                        )
                                    )
                                )
                            )

        var modal_daterange = $('<div class="modal fade" id="modal_DateRange" tabindex="-1" role="dialog">')
                            .append($('<div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">')
                                .append($('<div class="modal-content">')
                                    .append($('<div class="modal-header">')
                                        .append('<span data-role="none" id="txtCustomDualCalendarTitle" class="modal-title" style="font-size:18px;">Select Date</span>')
                                        .append('<button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>')
                                    )
                                    .append($('<div class="modal-body">')
                                        .append($('<div class="row">')
                                            .append($('<div class="col-sm-6">')
                                                .append($('<div class="metro mb-3" style="width:100%; overflow:hidden;">')
                                                    .append($('<div>')
                                                        .append('<b>From</b>: <span id="txtDateFromDualCalendar"></span>')
                                                        .append($('<div class="fromCustomDualCalendar calendar mt-3" style="width:100%;" data-other-days="1">'))
                                                    )
                                                )
                                                .append($('<div id="divDualCalendarTimeFrom" style="display:none;">')
                                                    .append($('<div class="row mb-4">')
                                                        .append($('<div class="col col-form-label">').text('Time:'))
                                                        .append($('<div class="col">')
                                                            .append($('<select data-role="none" id="selDCTimeFromHour" class="form-select">'))
                                                        )
                                                        .append($('<div class="col">')
                                                            .append($('<select data-role="none" id="selDCTimeFromMinute" class="form-select">'))
                                                        )
                                                        .append($('<div class="col">')
                                                            .append($('<select data-role="none" id="selDCTimeFromMeredian" class="form-select">')
                                                                .append('<option value="AM">AM</option>')
                                                                .append('<option value="PM">PM</option>')
                                                            )
                                                        )
                                                    )
                                                )
                                            )
                                            .append($('<div class="col-sm-6">')
                                                .append($('<div class="metro mb-3" style="width:100%; overflow:hidden;">')
                                                    .append($('<div>')
                                                        .append('<b>To</b>: <span id="txtDateToDualCalendar"></span>')
                                                        .append($('<div class="toCustomDualCalendar calendar mt-3" style="width:100%;" data-other-days="1">'))
                                                    )
                                                )
                                                .append($('<div id="divDualCalendarTimeTo" style="display:none;">')
                                                    .append($('<div class="row mb-4">')
                                                        .append($('<div class="col col-form-label">').text('Time:'))
                                                        .append($('<div class="col">')
                                                            .append($('<select data-role="none" id="selDCTimeToHour" class="form-select">'))
                                                        )
                                                        .append($('<div class="col">')
                                                            .append($('<select data-role="none" id="selDCTimeToMinute" class="form-select">'))
                                                        )
                                                        .append($('<div class="col">')
                                                            .append($('<select data-role="none" id="selDCTimeToMeredian" class="form-select">')
                                                                .append('<option value="AM">AM</option>')
                                                                .append('<option value="PM">PM</option>')
                                                            )
                                                        )
                                                    )
                                                )
                                            )
                                        )                                        
                                    )
                                    .append($('<div class="modal-footer">')
                                        .append($('<div class="row">')
                                            .append($('<div class="col-6">')
                                                .append($('<button data-role="none" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">').text('Cancel'))
                                            )
                                            .append($('<div class="col-6">')
                                                .append($('<button data-role="none" id="btnDoneDualCalendar" class="btn btn-success btn-block btn-lg">').text('Okay'))
                                            )
                                        )
                                    )
                                )
                            )

        var modal_settime = $('<div class="modal fade" id="modal_SetTime" tabindex="-1" role="dialog">')
                            .append($('<div class="modal-dialog modal-sm modal-dialog-centered" role="document">')
                                .append($('<div class="modal-content">')
                                    .append($('<div class="modal-header">')
                                        .append('<span data-role="none" class="modal-title" style="font-size:18px;">Set Time</span>')
                                        .append('<button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>')
                                    )
                                    .append($('<div class="modal-body">')
                                        .append($('<div class="row mb-3">')
                                            .append($('<div class="col-12">')
                                                .append($('<button data-role="none" id="btnSetTimeAsc" class="btn btn-light btn-block btn-lg">')
                                                    .append($('<span class="bi-caret-up-fill">'))
                                                )
                                            )
                                        )
                                        .append($('<div class="row mb-3 align-items-center">')
                                            .append($('<div class="col-4">')
                                                .append($('<span id="SetTimeHour" class="timeunactive timeactive">')
                                                    .append($('<input data-role="none" id="txtSetTimeHour" type="text" class="form-control form-control-lg" value="1">'))
                                                )
                                            )
                                            .append($('<div class="col-1 text-center" style="font-size:25px;">')
                                                .append(':')
                                            )
                                            .append($('<div class="col-4">')
                                                .append($('<span id="SetTimeMinute" class="timeunactive">')
                                                    .append($('<input data-role="none" id="txtSetTimeMinute" type="text" class="form-control form-control-lg" value="00">'))
                                                )
                                            )
                                            .append($('<div class="col-3">')
                                                .append($('<button data-role="none" id="btnSetTimeMeridian" class="btn btn-info btn-block btn-lg pl-0 pr-0">')
                                                    .text('AM')
                                                )
                                            )
                                        )
                                        .append($('<div class="row">')
                                            .append($('<div class="col-12">')
                                                .append($('<button data-role="none" id="btnSetTimeDesc" class="btn btn-light btn-block btn-lg">')
                                                    .append($('<span class="bi-caret-down-fill">'))
                                                )
                                            )
                                        )
                                    )
                                    .append($('<div class="modal-footer">')
                                        .append($('<div class="row">')
                                            .append($('<div class="col-6">')
                                                .append($('<button data-role="none" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">').text('Cancel'))
                                            )
                                            .append($('<div class="col-6">')
                                                .append($('<button data-role="none" id="btnOkSetTime" class="btn btn-success btn-block btn-lg">').text('Okay'))
                                            )
                                        )
                                    )
                                )
                            )

        var modal_continuousbarcode = $('<div class="modal fade" id="modal_ContinuousBarcode" tabindex="-1" role="dialog">')
                            .append($('<div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">')
                                .append($('<div class="modal-content">')
                                    .append($('<div class="modal-header">')
                                        .append('<span data-role="none" class="modal-title" style="font-size:18px;">Loop Scan</span>')
                                        .append('<button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>')
                                    )
                                    .append($('<div class="modal-body">')
                                        .append($('<div id="ContinuousBarcodeItemList" class="mb-3" style="display:none;">')
                                            .append($('<input data-role="none" class="form-control form-control-lg" placeholder="Enter or Scan Product Barcode" type="text" id="txtContinuousBarcode">'))
                                        )
                                        .append($('<div class="table-responsive mb-3">')
                                            .append('<table id="tblBarcodeCount" class="table table-striped">' +
                                                    '<thead>' +
                                                            '<tr>' +
                                                                '<th class="txt-center"> Product Barcode </th>' +
                                                                '<th class="txt-center"> Barcode Count </th>' +
                                                            '</tr>' +
                                                    '</thead>' +
                                                '</table>'
                                            )
                                        )
                                    )
                                    .append($('<div class="modal-footer">')
                                        .append($('<div class="row">')
                                            .append($('<div class="col-6">')
                                                .append($('<button data-role="none" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">').text('Cancel'))
                                            )
                                            .append($('<div class="col-6">')
                                                .append($('<button data-role="none" id="btnContinuousBarcodeBoxOK" class="btn btn-success btn-block btn-lg">').text('Done'))
                                            )
                                        )
                                    )
                                )
                            )

        $('body')
            .append(modal_msgbox)
            .append(modal_browse)
            .append(modal_contextualmenu)
            .append(modal_PromptUser)
            .append(tooltip)
            .append(loading)
            .append(modal_calendar)
            .append(modal_daterange)
            .append(modal_settime)
            .append(modal_continuousbarcode)
            .on('click', '.nav-link', function(){
                var href = $(this).attr('href');
                if (href != undefined && $(this).attr('data-toggle') != undefined){
                    if (href.charAt(0) == '#'){
                        $('[href='+ href +']').tab('show');
                    }
                }
            })
            .on('click', '.navbar-collapse li', function() {
                var navbartoggler = $(this).closest('div'), sidebar = navbartoggler.prev().attr('data-target');
                if (navbartoggler.css('display') == 'block'){
                    $(sidebar).collapse('toggle');
                }                
            })

        $('body').find('.modal').on('shown.bs.modal', function () {
            $('body').addClass('modal-open');
        })

        //Bs (Boostrap)
        window.Bs = {
            MessageBox: function (msg, title, vParams, vOption) {
                //if (!$('#modal_MessageBox').hasClass('show')) {
                //    $('body').removeClass('modal-open');
                //    $('.modal_MessageBox').modal('hide');
                //}                                

                Bs.Loading('hide');

                if (CStr2(title) == '') {
                    title = 'Message';
                }                

                if (vOption != undefined) {
                    if (!IsMobile){
                        vOption.HidePrevPopup = false;
                    }

                    if (vOption.HidePrevPopup == undefined || vOption.HidePrevPopup == true) {
                        if ($('.modal.show').length > 0) {
                            //close any modal first
                            $('.modal.show').modal('hide');
                            setTimeout(function () {
                                Bs.MessageBox(msg, title, vParams, vOption);
                            }, 400)
                            return;
                        }
                    }

                    $('#modal_MessageBox > .modal-dialog').removeClass('modal-sm modal-lg modal-xl modal-xxl');
                    if (CStr2(vOption.ModalSize) != '') {
                        //ModalSize values (sm, lg, xl, xxl)
                        $('#modal_MessageBox > .modal-dialog').addClass('modal-' + vOption.ModalSize);
                    }

                    if (vOption.CloseButton != undefined) {
                        if (vOption.CloseButton == "enable") {
                            $('#btn_msgbox_close').show();
                        }
                        else {
                            $('#btn_msgbox_close').hide();
                        }
                    }

                    if (vOption.AutoDismiss != undefined) {
                        if (vOption.AutoDismiss == false) {
                            $('#modal_MessageBox').attr({
                                'data-backdrop': 'static',
                                'data-keyboard': 'false'
                            })
                        } else {
                            $('#modal_MessageBox').removeAttr('data-backdrop data-keyboard');
                        }
                    }
                    else {
                        $('#modal_MessageBox').removeAttr('data-backdrop data-keyboard');
                    }

                    if (vOption.LiveValidation != undefined) {
                        vOption.LiveValidation();
                    }
                } else { // auto close messagebox
                    $('#modal_MessageBox').prop('onclick', null).on('click');
                    $('#modal_MessageBox').removeAttr('data-backdrop data-keyboard');
                }

                if (msg != "") {
                    msg = msg + '<br><br>';
                }

                $('#msgbox_title').html(title);
                $('#msgbox_body').html(msg);

                if (vParams == undefined || vParams == null) {
                    vParams = [{ Text: 'Ok' }];
                }

                $('#msgbox_footer').empty();

                var ctrBtn = 0;
                for (var i = 0; i < vParams.length; i++) {
                    var btnTheme, btnClass = '';

                    if (vParams[i].Theme != undefined) {
                        btnClass += ' ' + vParams[i].Theme;
                    }
                    else {
                        btnClass += ctrBtn == 0 ? ' btn-success ' : ' btn-secondary ';
                    }

                    if (IsMobile) {
                        btnClass += ' btn-lg ';
                    }

                    $('#msgbox_footer')
                        .append($('<button data-role="none" class="btn btn-block">')
                            .addClass($.trim(btnClass))
                            .html(vParams[i].Text)
                            .data('data', vParams[i])
                            .click(function () {
                                var data = $(this).data('data');

                                if (data.Validation != undefined) {
                                    if (data.Validation() == false) {
                                        return false;
                                    }
                                }

                                $('#modal_MessageBox').modal('toggle');
                                //butangan ug delay para naa pay time ma close jud ang modal before ishow ang another modal if tawagon na pud
                                setTimeout(function () {
                                    if (data.Click == undefined) {
                                        //ClosePopUp(vOption);
                                    }
                                    else {
                                        //ClosePopUp(vOption);
                                        data.Click();
                                    }
                                }, 400)                                
                            })
                        )

                    ctrBtn += 1
                }

                $('#modal_MessageBox').unbind('hidden.bs.modal').on('hidden.bs.modal', function () {
                    //ClosePopUp(vOption);
                    //$('#modal_MessageBox').modal('hide');
                    if ($('.modal.show').length == 0)
                        $('.modal-backdrop').remove(); // remove when MessageBox calls multiple times
                    ClosePopUp(vOption);
                })

                if ($('#modal_MessageBox').hasClass('show') && $('.modal-backdrop').length > 0) {
                    $('.modal-backdrop').remove(); // remove when MessageBox calls multiple times
                }

                try {
                    $('#modal_MessageBox').modal('dispose').modal('show');
                } catch(e){
                    $('#modal_MessageBox').modal('show');
                }

                return $('#modal_MessageBox')
            },
            Browse: function (LibraryName, title, vParams, data) {
                var list = $('#browselistresult'), returnData;
                var isFromListData = false, styleformat = 'list', arrSelectedData = [], isResultFromBarcodeScanner = false, alreadyadatable = false;
                var mLastModifiedFrom;
                var mLastModifiedTo, fxLibraryName, arrSearchResults = [], pStr = '';

                list.empty();

                if (!IsMobile){
                    vParams[0].HidePrevPopup = false;
                }

                if (vParams[0].HidePrevPopup == undefined || vParams[0].HidePrevPopup == true) {
                    $('.modal.show').modal('hide');
                }                                

                if (CStr2(title) == '') {
                    title = '&nbsp;';
                }

                if (data == undefined) {
                    data = {}
                }

                $('#browse_title').html(title);
                $('#txtBrowseLibrary').val('').focus();

                $('#divBrowseFooter').find('.col-sm:not(:eq(2))').show();
                $('#divBrowseListFormat, #divBrowseTableFormat, #divGlobalAddSKU, #divBrowseFooter').hide();
                $('#tblOption, #tblBrowsePopupHeader').empty();
                $('#divNoOfSelectedData').unbind('click').text('').hide();

                if (CBool(vParams[0].Barcode)) {
                    if (OfflineMode) {
                        $('#divBarcode').show();
                    } else {
                        $('#divBarcode').css('display', '');
                    }
                } else {
                    $('#divBarcode').css('display', 'none');
                }

                $('#BrowseMessage').hide();
                $('#btn_browse_close').show();

                if (vParams[0].Message != undefined) {
                    $('#BrowseMessage').html(vParams[0].Message).show();
                }

                var modalBrowse = $('#modal_Browse > .modal-dialog');
                modalBrowse.addClass('modal-dialog-centered modal-dialog-scrollable').removeClass('modal-sm modal-lg modal-xl modal-fullscreen modal-fullwidth');
                if (vParams[0].ModalSize != undefined) {
                    //ModalSize values (sm, lg, xl, fullscreen)
                    if (vParams[0].ModalSize == 'fullscreen'){
                        modalBrowse.removeClass('modal-dialog-centered modal-dialog-scrollable')
                    }

                    modalBrowse.addClass('modal-' + vParams[0].ModalSize);
                } else {
                    modalBrowse.addClass('modal-lg');
                }

                if (vParams[0].AutoDismiss != undefined || CBool(data.DontCloseOnOverlayClick)) {
                    if (vParams[0].AutoDismiss == false || CBool(data.DontCloseOnOverlayClick)) {
                        $('#btn_browse_close').hide();
                        $('#modal_Browse').attr({
                            'data-backdrop': 'static',
                            'data-keyboard': 'false'
                        })
                    } else {
                        $('#modal_Browse').removeAttr('data-backdrop data-keyboard');
                    }
                }
                else {
                    $('#modal_Browse').removeAttr('data-backdrop data-keyboard');
                }

                $('#browseOption').show();

                if (!$.isFunction(LibraryName)) {
                    if (LibraryName.toLowerCase() == "librarypimproduct" && !IsMobile) {
                        $('#tblOption')
                            .append($('<tr>')
                                .append($('<td style="width:50px;">').text('Show:'))
                                .append($('<td style="width:90px;">')
                                    .append($('<div class="form-check">')
                                        .append($('<input class="form-check-input" type="radio" data-role="none" id="SkuStatusActive" name="rbFilter" checked value="0">'))
                                        .append($('<label class="form-check-label" for="SkuStatusActive">').text('Active'))
                                    )
                                )
                                .append($('<td style="width:50px;">')
                                    .append($('<div class="form-check">')
                                        .append($('<input class="form-check-input" type="radio" data-role="none" id="SkuStatusInactive" name="rbFilter" value="1">'))
                                        .append($('<label class="form-check-label" for="SkuStatusInactive">').text('Inactive'))
                                    )
                                )
                                .append($('<td style="padding-left:30px; display:flex;">')
                                    .append($('<span>Show inventory for this FOWST:</span>'))
                                    .append($('<select id=cbPimFowst class="form-select" style="margin-left:10px">'))
                                )
                            )                            
                            //.append($('<tr name="trDisplayShowInactive" style="display:none;">')
                            //    .append($('<td colspan="4">').html('<a name=lnkSearchInactive class="custom-link">Click here to search among inactive records.</a>'))
                            //);

                        $('#tblOption [name=lnkSearchInactive]').click(function() {
                            $('#tblOption [name=rbFilter][value=1]').prop('checked', true);
                            BrowseNow();
                        });

                        if (CBool(vParams[0].AddUsingWebForm)) { // option to add SKU
                            $('#divGlobalAddSKU').show();
                        } else {
                            $('#divGlobalAddSKU').hide();
                        }

                        var pCbPimFowst = $('#cbPimFowst');
                        if (sessionStorage.system == 'eas') {
                            AjaxSend({
                                ExactUrl: '/Browser/LibraryFowst',
                                success: function (vData) {
                                    for (var pDR of vData.Results.Rows) {
                                        var pOption = new Option(pDR.FowstName, pDR.ID);
                                        pCbPimFowst.append(pOption);
                                    }
                                    pCbPimFowst.val(sessionStorage.FowstID);
                                    pCbPimFowst.change(BrowseNow);
                                }
                            });
                        } else {
                            pCbPimFowst.parent().hide();
                        }                        

                        $('[name=trDefineNewProduct]').hide();
                        $('#tblOption [name=trDisplayShowInactive]').hide();
                        $('[name=trDefineNewProduct] .custom-link').click(function () {
                            var pQuickPim = GetStringObject('ERP.QuickPim.controller');
                            if (pQuickPim) {
                                $('#modal_Browse').modal('hide');
                                pQuickPim.popup();
                                //setTimeout(function() {
                                //    pQuickPim.popup();
                                //}, 5000);
                            }
                        });
                    } else if (LibraryName.toLowerCase() == "transinvoices" && !IsMobile) {
                        if (CBool(vParams[0].ShowRecordStatusAsDropDown)) {
                            $('#tblOption')
                                .append($('<tr>')
                                    .append($('<td style="width:90px;">')
                                        .append($('<select data-role="none" id="selRecordStatus" class="form-select" style="width:220px">')
                                            .append($('<optgroup label="Record Status">')
                                                .append('<option value="active">Record Status: Active</option>')
                                                .append('<option value="cancelled">Record Status: Cancelled</option>')
                                            ) // END optgroup
                                        )
                                    )
                                ) // END #tblOption

                            $('#selRecordStatus').change(function () {
                                BrowseNow();
                            });
                        } else
                            $('#tblOption')
                                .append($('<tr>')
                                    .append($('<td style="width:50px;">').text('Show:'))
                                    .append($('<td style="width:90px;">')
                                        .append($('<div class="form-check">')
                                            .append($('<input type="radio" class="form-check-input" data-role="none" id="SkuStatusActive" name="rbFilter" checked value="0">'))
                                            .append($('<label class="form-check-label" for="SkuStatusActive">').text('Active'))
                                        )
                                    )
                                    .append($('<td style="width:50px;">')
                                        .append($('<div class="form-check">')
                                            .append($('<input type="radio" class="form-check-input" data-role="none" id="SkuStatusInactive" name="rbFilter" value="1">'))
                                            .append($('<label class="form-check-label" for="SkuStatusInactive">').text('Cancelled'))
                                        )
                                    )
                                )

                        if (data.TransType == 'Invoice') {
                            $('#tblOption tr')
                                .append($('<td style="width:50px;">').text(''))
                                .append($('<td style="width:130px;">')
                                    .append($('<div class="form-check">')
                                        .append($('<input type="radio" class="form-check-input" data-role="none" id="rbInvoiceSourceEAS" name="rbInvoiceSource" checked value="eas">'))
                                        .append($('<label class="form-check-label" for="rbInvoiceSourceEAS">').text('EAS Invoices'))
                                    )
                                )
                            .append($('<td style="width:130px;">')
                                    .append($('<div class="form-check">')
                                        .append($('<input type="radio" class="form-check-input" data-role="none" id="rbInvoiceSourcePOS" name="rbInvoiceSource" value="pos">'))
                                        .append($('<label class="form-check-label" for="rbInvoiceSourcePOS">').text('POS Invoices'))
                                    )
                                );

                            if (data.InvoiceSource == 'pos') 
                                $('#rbInvoiceSourcePOS').prop('checked', true);

                            $('#tblOption tr')
                                .append($('<td style="width:50px;">').text(''))
                                .append($('<td style="width:130px;">')
                                    .append($('<div class="form-check">')
                                        .append($('<input type="radio" class="form-check-input" data-role="none" id="rbInvoiceActiveRecords" name="rbInvoiceArchive" checked value="active">'))
                                        .append($('<label class="form-check-label" for="rbInvoiceActiveRecords">').text('Active Records'))
                                    )
                                )
                            .append($('<td style="width:130px;">')
                                    .append($('<div class="form-check">')
                                        .append($('<input type="radio" class="form-check-input" data-role="none" id="rbInvoiceArchivedRecords" name="rbInvoiceArchive" value="archive">'))
                                        .append($('<label class="form-check-label" for="rbInvoiceArchivedRecords">').text('Archived Records'))
                                    )
                                )
                        }

                        if (vParams[0].ShowHideChildSos) {
                            $('#tblOption tr')
                                .append($('<td style="width:140px; text-align:center;">').text('Show child SO\'s:'))
                                .append($('<td style="width:60px;">')
                                    .append($('<div class="form-check">')
                                        .append($('<input type="radio" class="form-check-input" data-role="none" id="rbFilterShowChildSosYes" name="rbFilterShowChildSos" value="1">'))
                                        .append($('<label class="form-check-label" for="rbFilterShowChildSosYes">').text('Yes'))
                                    )
                                )
                            .append($('<td style="width:50px;">')
                                    .append($('<div class="form-check">')
                                        .append($('<input type="radio" class="form-check-input" data-role="none" id="rbFilterShowChildSosNo" name="rbFilterShowChildSos" checked value="0">'))
                                        .append($('<label class="form-check-label" for="rbFilterShowChildSosNo">').text('No'))
                                    )
                            )
                    }

                    if (vParams[0].ShowDraftFinalizedFilter) {
                        $('#tblOption tr')
                            .append($('<td style="width:20px;">').text(''))
                            .append($('<td style="width:auto;">')
                                .append('<select data-role="none" class="form-select fixed" id="selDraftStatus">' + '\n' + 
                                    '        <optgroup label="Finalized?">' + '\n' + 
                                    '           <option value="finalized">Finalized: Yes</option>' + '\n' + 
                                    '           <option value="draft">Finalized: No</option>' + '\n' + 
                                    '           <!-- <option value="">Finalized: All</option> -->' + '\n' + 
                                    '        </optgroup>' + '\n' + 
                                    '    </select>'
                                )
                            )

                        $('#selDraftStatus').change(function() {
                            BrowseNow();
                        });
                    }

                    if (vParams[0].ShowApprovedFilter) {
                        $('#tblOption tr')
                            .append($('<td style="width:20px;">').text(''))
                            .append($('<td style="width:auto;">')
                                .append('<select data-role="none" class="form-select fixed" id="selApproveStatus">' + '\n' +
                                    '        <optgroup label="Approved?">' + '\n' + 
                                    '           <option value="unapproved">Approved: No</option>' + '\n' +
                                    '           <option value="approved" >Approved: Yes</option>' + '\n' +
                                    '           <!-- <option value="all">All</option> -->' + '\n' +
                                '           </optgroup>' + '\n' + 
                                    '    </select>'
                                )
                            )

                        $('#selApproveStatus').change(function () {
                            BrowseNow();
                        });
                    }

                    if (vParams[0].ShowLastActionTakenFilter) {
                        $('#tblOption tr')
                            .append($('<td style="width:20px;">').text(''))
                            .append($('<td style="width:auto;">')
                                .append('<select data-role="none" class="form-select fixed" id="selLastActionTaken">' + '\n' +
                                    '        <optgroup label="Last Action taken">' + '\n' + 
                                    '           <option value="approved">Step 1: Saved as Draft PO</option>' + '\n' +
                                    '           <option value="approved">Step 2: Saved as Draft PO and Outputted</option>' + '\n' +
                                    '           <option value="approved">Step 3: Saved as Final PO</option>' + '\n' +
                                    '           <option value="approved">Step 4: Saved as Final PO and Outputted</option>' + '\n' +
                                    '           <option value="approved">Step 5: Saved as Final PO and Received from Vendor</option>' + '\n' +
                                    '           <option value="approved">Step 6: Saved as Final PO, Received and Paid</option>' + '\n' +
                                    '           <!-- <option value="all">All</option> -->' + '\n' +
                                '           </optgroup>' + '\n' + 
                                    '    </select>'
                                )
                            )

                        $('#selApproveStatus').change(function () {
                            BrowseNow();
                        });
                    }

                        if (vParams[0].ShowLastModifiedFilter) {
                            $('#tblOption tr')
                                .append($('<td>')
                                    .append('<button id="butMessageBoxLastModifiedFilter"  style="margin-right:10px;" data-role="none" class="btn btn-light border">' +
                                        '<span class="lblSelectedInventoryType-x"> Last Modified </span>' +
                                    '</button>'
                                    )
                                )
                        }

                        if (vParams[0].ShowPoAndInvoiceRadios) {
                            $('#tblOption')
                            .append($('<tr>')
                                .append($('<td style="width:50px;">').text('Show:'))
                                .append($('<td style="width:90px;">')
                                    .append($('<div class="form-check">')
                                        .append($('<input type="radio" class="form-check-input" data-role="none" id="rbFilterPoAndInvoicePOs" name="rbFilterPoAndInvoice" checked value="po">'))
                                        .append($('<label class="form-check-label" for="rbFilterPoAndInvoicePOs">').text('Purchase Orders'))
                                    )
                                )
                                .append($('<td style="width:50px;">')
                                    .append($('<div class="form-check">')
                                        .append($('<input type="radio" class="form-check-input" data-role="none" id="rbFilterPoAndInvoiceInvs" name="rbFilterPoAndInvoice" value="invoice">'))
                                        .append($('<label class="form-check-label" for="rbFilterPoAndInvoiceInvs">').text('Invoices'))
                                    )
                                )
                            )
                        }

                        if (vParams[0].ShowLastModifiedFilter) {
                            $('#butMessageBoxLastModifiedFilter')
                                .text('Last Modified: ' + FormatDate(vParams[0].LastModifiedFrom, { Format: 'MMM d, yyyy' }) + ' to ' + FormatDate(vParams[0].LastModifiedTo, { Format: 'MMM d, yyyy' }))
                                .click(function () {
                                    DualCalendar({
                                        Click: function (e) {
                                            vParams[0].LastModifiedFilterChange(e);
                                            mLastModifiedFrom = e.From;
                                            mLastModifiedTo = e.To;
                                        }
                                    }, new Date(), new Date())
                                });
                        }
                    } else if (LibraryName.toLowerCase() == "TransProfitability".toLowerCase()) {
                        pStr += ' <td style="padding-left: 30px;">' + '\n' +
                                '    <button id="butMessageBoxLastModifiedFilter"  style="margin-right:10px;" data-role="none" class="btn btn-less-width btn-transparent btnRequired">' + '\n' +
                                '        <span class=""> Original Transaction Last Modified </span>' + '\n' +
                                '        <span class="ui-icon ui-icon-arrow-d" style="display:inline-block; margin-left:10px;"></span>' + '\n' +
                                '    </button>' + '\n' +
                                '</td>' + '\n' +
                                '<td>' + '\n' +
                                '    <select data-role="none" class="form-control fixed" id="selProfitabilityApproveStatus">' + '\n' +
                                '        <option value="unapproved">Unapproved</option>' + '\n' +
                                '        <option value="approved">Approved</option>' + '\n' +
                                '        <option value="all">All</option>' + '\n' +
                                '    </select>' + '\n' +
                                '</td>';
                        $('#tblOption').append($('<tr>').append(pStr));
                        mLastModifiedFrom = vParams[0].LastModifiedFrom;
                        mLastModifiedTo = vParams[0].LastModifiedTo;
                        modal_browse.find('#butMessageBoxLastModifiedFilter').text('Original Transaction Last Modified: ' + FormatDate(mLastModifiedFrom, { Format: 'MMM dd, yyyy' }) + ' to ' + FormatDate(mLastModifiedTo, { Format: 'MMM dd, yyyy' }));
                        modal_browse.find('#selProfitabilityApproveStatus').change(function () {
                            BrowseNow();
                        });
                        modal_browse.find('#butMessageBoxLastModifiedFilter').click(function () {
                            DualCalendar({
                                Click: function (e) {
                                    vParams[0].LastModifiedFilterChange(e);
                                    mLastModifiedFrom = e.From;
                                    mLastModifiedTo = e.To;
                                }
                            }, new Date(), new Date());
                        });

                    //}  else if (LibraryName.toLowerCase() == "TransInvoicesReturnables".toLowerCase()) {
                    //    $('#tblOption').append($('<tr>')
                    //        .append($('<td style="width:50px;">').text('Show'))
                    //        .append($('<td style="width:130px;">')
                    //            .append($('<div class="form-check">')
                    //                .append($('<input type="radio" class="form-check-input" data-role="none" id="rbReturnableInvoiceOffline" name="rbReturnableSource" checked value="offline">'))
                    //                .append($('<label class="form-check-label" style="white-space:nowrap" for="rbReturnableInvoiceOffline">').text('Offline Invoices'))
                    //            )
                    //        )
                    //        .append($('<td style="width:130px;">')
                    //            .append($('<div class="form-check" style="padding-left:50px">')
                    //                .append($('<input type="radio" class="form-check-input" data-role="none" id="rbReturnableInvoiceOnline" name="rbReturnableSource" value="online">'))
                    //                .append($('<label class="form-check-label" style="white-space:nowrap" for="rbReturnableInvoiceOnline">').text('Online Invoices'))
                    //            )
                    //        )
                    //    );
                        
                    }  else if (LibraryName.toLowerCase() == "LibraryOrganization".toLowerCase()) {
                        if (CBool(vParams[0].AllowToAddNewCustomer)){
                            $('#tblOption').append($('<tr>')
                                .append($('<td name="tdAddNewCustomer" style="display:none;">')
                                    .append('If you\'ve correctly spelled your search but can\'t find it, you might want to <a data-role="none" href="#">add new Customer</a>')
                                    .click(function(){
                                        function Go(){
                                            Bs.Loading('show');
                                            require(['promise!/Scripts3/Bs_CustomerDetails.js?v=' + assetversion], function(){
                                                Bs.Loading('hide');
                                                $('#popupAddNewCustomer').modal('show');
                                                Bs.DoneAddingNewCustomer = function (e) {
                                                    WebToast('New customer has been added successfully', 'top');
                                                    $('#modal_Browse').modal('hide');
                                                    vParams[0].Click(e);
                                                }
                                            })
                                        }         
                                        
                                        if (CBool(sessionStorage.IsWebPos)){
                                            PromptUserAccess('Add New Customer', {
                                                Message: 'You\'re not allowed to add new customer.',
                                                OnClickOKSuccess: function () {
                                                    Go();
                                                }
                                            });
                                        } else {
                                            Go();
                                        }
                                    })
                                )
                            )
                        }
                    } else {
                        $('#browseOption').hide();
                    }
                } else {
                    fxLibraryName = LibraryName;
                    LibraryName = LibraryName();
                }

                if (vParams[0].Options != undefined) {
                    //vParams[0].Options = { Fields: [], OnChange: fx }
                    //Fields must be an Array, the value must be a string or object HTML table rows
                    //OnChange must return array object
                    for (var i = 0; i < vParams[0].Options.Fields.length; i++) {
                        $('#tblOption').html(vParams[0].Options.Fields[i]);
                    }

                    if (vParams[0].Options.OnLoad != undefined){
                        vParams[0].Options.OnLoad();
                    }

                    $('#browseOption').show();
                }                

                if (vParams[0].AddButton != undefined) {
                    $('#divClearBtn').show();
                    if (vParams[0].AddButton[0].Text != undefined) {
                        $('#btnCustomClear').html(vParams[0].AddButton[0].Text);
                    }
                    else {
                        $('#btnCustomClear').text('Clear');
                    }

                    $('#btnCustomClear').click(function () {
                        $('#modal_Browse').modal('hide');
                        setTimeout(function () {
                            ClosePopUp();
                            vParams[0].AddButton[0].Click();
                        }, 400)                        
                    })
                }
                else {
                    $('#divClearBtn').hide();
                }

                if (CBool(vParams[0].WithSearchbox) == true || vParams[0].WithSearchbox == undefined) {
                    $('#divSearchBoxBrowseLibrary').css('display', '');
                }
                else {
                    $('#divSearchBoxBrowseLibrary').hide();
                }

                if (CBool(vParams[0].AutoSearch) == true) {
                    BrowseNow();
                }

                if (vParams[0].Search != undefined) {
                    if (CBool(vParams[0].DisplaySearhKey) || vParams[0].DisplaySearhKey == undefined){
                        $('#txtBrowseLibrary').val(vParams[0].Search);
                    }
                    
                    if (vParams[0].listData == undefined) {
                        isResultFromBarcodeScanner = false;
                        data.keywords = vParams[0].Search;
                        BrowseNow();
                    }
                }

                if (vParams[0].Close != undefined) {
                    closeFunction = vParams[0].Close;
                }

                if (vParams[0].StyleFormat != undefined) {
                    if (vParams[0].StyleFormat == 'table') {
                        styleformat = 'table';
                        if ($.fn.DataTable.isDataTable($('#tblBrowsePopup'))) {
                            $('#tblBrowsePopup').DataTable().clear().destroy();
                            $('#tblBrowsePopupBody').empty();
                        }
                    } else {
                        styleformat = 'list';
                    }
                } else {
                    styleformat = 'list';
                }

                if (vParams[0].listData != undefined) {
                    (CStr2(vParams[0].StyleFormat) == 'table' ? CreateHeader() : '');
                    DisplayBrowseResult(vParams[0].listData);
                    isFromListData = true;
                }

                $('#btnCopyTextBrowseData').unbind('click').click(function () {
                    var colToCopy = [], len = arrSearchResults.length, i = 0, msg = '';

                    for (i; i < vParams[0].DisplayText.length; i++) {
                        var col = vParams[0].DisplayText[i];
                        if (!CBool(col.DontCopyToPlainText))
                            colToCopy.push(col);
                    }

                    i = 0;
                    for (i; i < len; i++) {
                        var rowdata = arrSearchResults[i];
                        msg += (i + 1) + '.\n';

                        for (var c = 0; c < colToCopy.length; c++) {
                            msg += (colToCopy[c].Label == undefined ? '' : colToCopy[c].Label + ': ') + ($.isFunction(colToCopy[c].Column) ? colToCopy[c].Column(rowdata) : CStr2(rowdata[colToCopy[c].Column])) + ' \n';
                        }

                        msg += '\n';
                    }

                    $('#txtBrowseCopyTransToPlainText').val(msg).select();
                    document.execCommand("copy");
                    WebToast('Copied to clipboard');
                })

                $('#btnOkBrowseSelectMultiple').unbind('click').click(function () {
                    if (vParams[0].Validation != undefined) {
                        var result = vParams[0].Validation(arrSelectedData, isResultFromBarcodeScanner);
                        if (result != true) {
                            if (result != false) {
                                $('#divMsgErrorBrowseLibrary').show();
                                $('#divMsgErrorBrowseLibrary').html(result);
                            }
                            else {
                                $('#divMsgErrorBrowseLibrary').hide();
                            }
                            return false;
                        }
                    }

                    $('#modal_Browse').modal('hide');
                    setTimeout(function () {
                        vParams[0].Click(arrSelectedData, isResultFromBarcodeScanner);
                    }, 400)                    
                })

                $('#txtBrowseLibrary').unbind('keyup').keyup(function (e) {
                    if (e.keyCode == 13) {
                        if (e.which == 13 && isFromListData == false) {
                            if (vParams[0].AllowEmptySearch != undefined && !CBool(vParams[0].AllowEmptySearch) && $.trim($(this).val()) == '') {
                                return;
                            }
                            if (styleformat == 'table') {
                                if ($.fn.DataTable.isDataTable($('#tblBrowsePopup'))) {
                                    $('#tblBrowsePopup').DataTable().clear().destroy()
                                }
                            }
                            isResultFromBarcodeScanner = false;
                            data.keywords = $.trim($(this).val());
                            $(this).blur();
                            BrowseNow();
                        }

                        isFromListData = false;
                    }
                })

                $('[name="rbFilter"], [name=rbInvoiceSource], [name=rbInvoiceArchive], [name=rbReturnableSource]').unbind('change').change(function () {
                    data.keywords = $.trim($('#txtBrowseLibrary').val());
                    BrowseNow();
                })

                $('#modal_Browse').unbind('hidden.bs.modal').on('hidden.bs.modal', function () {
                    ClosePopUp(vParams[0]);
                    $('#modal_Browse').modal('hide');
                    if ($('.modal.show').length == 0)
                        $('.modal-backdrop').remove(); // remove when MessageBox calls multiple times
                })

                $('#modal_Browse').unbind('shown.bs.modal').on('shown.bs.modal', function () {
                    if(!IsMobile){
                        $('#txtBrowseLibrary').focus();
                    }                    
                })

                if (!$('#modal_Browse').hasClass('show')){
                    $('#modal_Browse').modal('dispose').modal('show');
                }                

                function BrowseNow() {
                    styleformat = (vParams[0].StyleFormat || 'list');

                    if (vParams[0].Options != undefined) {
                        if (vParams[0].Options.OnChange != undefined) {
                            data = $.extend(data, vParams[0].Options.OnChange())
                        }
                    }

                    if (styleformat == 'table') {
                        $('#tblBrowsePopupBody').empty().append('<tr><td style="text-align:center" colspan="' + (vParams[0].DisplayText.length + 1) + '"><div id="div" class="spinner-border text-primary" role="status"><span class="visually-hidden"></span></div></td></tr>');
                        $('#divBrowseTableFormat').show();
                    } else {
                        list.empty();
                        list.append('<a data-role="none" class="list-group-item list-group-item-action text-center"><div id="div" class="spinner-border text-primary" role="status"><span class="visually-hidden"></span></div></a>');
                        $('#divBrowseListFormat').show();
                    }
                    $('#divBrowseFooter,[name="trDefineNewProduct"]').hide();
                    $('#tblOption [name=trDisplayShowInactive]').hide();
                    $('#tblOption [name="tdAddNewCustomer"]').hide()

                    if ($.isFunction(LibraryName)) {
                        fxLibraryName = LibraryName;
                        LibraryName = LibraryName();
                    }

                    if (OfflineMode) {
                        if (CBool(vParams[0].Offline)) {
                            setTimeout(function () {
                                var OfflineBrowser, OBFunction;

                                if (vParams[0].OfflineBrowser == undefined || vParams[0].OfflineBrowser == true) {
                                    //mao ni ang default
                                    //pag kwaon sa global variable na OfflineBrowser (/Scripts3/Offline/Browser/...)
                                    OfflineBrowser = 'return OfflineBrowser.' + LibraryName + '.GetData(' + JSON.stringify(data) + ')';
                                    OBFunction = new Function(OfflineBrowser.replace(/\'/g, ''));
                                } else {
                                    //mao ni ang bago as of 1-27-2021 11:17 AM by arnie
                                    //pag buhat2 na function. Eg: LibraryName value = Expense(true) or Org.GetData() or any. See specific example sa Expenses.js kung giunsa
                                    //dapat global ang function
                                    OfflineBrowser = 'return ' + LibraryName;
                                    OBFunction = new Function(OfflineBrowser);
                                }

                                DisplayResults({ Results: OBFunction() })
                                //if (styleformat == "table") {
                                //    CreateHeader();
                                //    DisplayBrowseResult();
                                //} else {
                                //    DisplayBrowseResult({ Results: OBFunction() });
                                //}
                            }, 500);
                            return;
                        }
                    }

                    var url = ((LibraryName).indexOf('/') == -1 ? 'Browser/' + LibraryName : LibraryName);

                    if (LibraryName.toLowerCase() == "librarypimproduct") {
                        data.IsDeactivated = $('[name="rbFilter"]:checked').val();
                        var pInventoryFowstID = $('#cbPimFowst').val();
                        if (CStr2(pInventoryFowstID) != '')
                            data.InventoryFowstID = pInventoryFowstID;

                    } else if (LibraryName.toLowerCase() == "transinvoices") {
                        if (vParams[0].ShowRecordStatusAsDropDown)
                            data.IsCancelled = $('#selRecordStatus option:checked').val() == 'cancelled';
                        else
                            data.IsCancelled = $('[name="rbFilter"]:checked').val();
                        data.FilterPoAndInvoice = $('[name="rbFilterPoAndInvoice"]:checked').val();
                        if (CBool(vParams[0].ShowHideChildSos))
                            data.ShowChildSos = $('[name="rbFilterShowChildSos"]:checked').val();
                        else
                            data.ShowChildSos = true;
                        if ($('[name="rbInvoiceSource"]:checked').length > 0)
                            data.InvoiceSource = $('[name="rbInvoiceSource"]:checked').val();
                    } else if (LibraryName.toLowerCase() == "TransInvoicesReturnables".toLowerCase()) {
                        if ($('[name="rbReturnableSource"]:checked').length > 0)
                        data.OfflineOnline = $('[name="rbReturnableSource"]:checked').val();
                    }
                    if ($('[name="rbInvoiceArchive"]:checked').length > 0)
                        data.Archive = $('[name="rbInvoiceArchive"]:checked').val();

                    if (mLastModifiedFrom != null)
                        data.LastModifiedFrom = mLastModifiedFrom;
                    if (mLastModifiedTo != null)
                        data.LastModifiedTo = mLastModifiedTo;

                    var pProfitabilityApproveStatus = $('#selProfitabilityApproveStatus').val();
                    if (CStr2(pProfitabilityApproveStatus) != '')
                        data.ApprovedStatus = pProfitabilityApproveStatus;
                    else {
                        var pApproveStatus = $('#selApproveStatus').val();
                        var pDraftStatus = $('#selDraftStatus').val();
                        delete data.DraftStatus;
                        delete data.ApprovedStatus;
                        if (CStr2(pDraftStatus) != '')
                            data.DraftStatus = pDraftStatus;

                        // commented on 2022_02_28__16_32_56_1824
                        //if (pDraftStatus == 'finalized') {
                        //    $('#selApproveStatus').val('approved'); // this will not trigger 'change'. so, no need to return
                        //    pApproveStatus = 'approved';
                        //}

                        if ((pDraftStatus == 'draft')
                            && (pApproveStatus == 'approved')) {

                            $('#selApproveStatus').val('unapproved'); // this will not trigger 'change'. so, no need to return
                            pApproveStatus = 'unapproved';
                            //return;
                        }

                        if (CStr2(pApproveStatus) != '')
                            data.ApprovedStatus = pApproveStatus;
                    }

                    if (vParams[0].OfflineBrowser != null) {
                        if (vParams[0].StyleFormat == 'table') {
                            $('#tblBrowsePopupHeader').empty();
                            $('#divBrowseListFormat').hide();
                            $('#divBrowseTableFormat').show();
                        }
                        setTimeout(function () {
                            var pSearchResult = vParams[0].OfflineBrowser.GetData(data);
                            pSearchResult = {
                                Results: pSearchResult
                            };
                            DisplayResults(pSearchResult);
                        }, 500)                        
                    } else {
                        if (CBool(vParams[0].ServerSideProcessing)) {
                            $('#divBrowseListFormat').hide();
                            $('#divBrowseTableFormat').show();

                            CreateHeader();

                            var dtColumns = [];

                            if (LibraryName.toLowerCase() == 'librarypimproduct' && CBool(vParams[0].ShowPic)) {
                                dtColumns.push({
                                    "data": function (data, type, full, meta){
                                        return '<img width="100" src="../Content/images/box.png" />'
                                    },
                                    createdCell: function (td, cellData, rowData, row, col){
                                        if (CStr2(rowData.Image) != '') {
                                            $(td).css('width', '100px')
                                                .data('data', rowData)
                                                .click(function(e){
                                                    e.preventDefault();
                                                    e.stopPropagation();
                                                    ShowProductImage($(this).data('data').ID);
                                                })
                                                .find('img').attr('src', '/Content/MobizImagePIM/' + sessionStorage.OrgID + '/Medium/' + rowData.Image)
                                        }
                                    }
                                });
                            }

                            for (var m = 0; m < vParams[0].DisplayText.length; m++) {
                                var params = vParams[0].DisplayText[m];
                                var pRender = function (data) {
                                    return CStr2(data);
                                };
                                if ($.isFunction(params.Render))
                                    pRender = params.Render;
                                dtColumns.push({
                                    "data": params.Column,
                                    "width": (params.Style == undefined ? 'auto' : params.Style.width),
                                    "render": pRender,
                                    "className": CStr2(params.className)
                                });
                            }

                            dataTableBrowsePopup = $('#tblBrowsePopup').DataTable({
                                order: [(vParams[0].OrderBy == undefined ? [0, 'asc'] : vParams[0].OrderBy)], //vParams[0].OrderBy is array . use only in display table
                                ajax: {
                                    url: MergeUrlParameters($.extend(data, { OrgID: sessionStorage.OrgID }), url),
                                    type: 'POST'
                                },
                                "pageLength": (vParams[0].PageLength || 25),
                                columns: dtColumns,
                                "bProcessing": true,
                                "bServerSide": true,
                                "drawCallback": function (settings) {
                                    PopupReposition();
                                },
                                initComplete: function (e, data) {
                                    arrSearchResults = data.data;
                                    if ($.isFunction(vParams[0].InitComplete))
                                        vParams[0].InitComplete(this, arrSearchResults); // "this" is a table jquery object

                                    $('#tblBrowsePopup').find('table').remove(); //idunno why naay table sa sulod, remove nalang

                                    if (CBool(vParams[0].SelectMultiple) || CBool(vParams[0].ShowOkButton)) {
                                        $('#divBrowseFooter').show();
                                    } else {
                                        $('#divBrowseFooter').hide();
                                    }

                                    if (CBool(vParams[0].CopyText)) {
                                        $('#divBrowseFooter').show();

                                        if (CBool(vParams[0].SelectMultiple)
                                        || CBool(vParams[0].ShowOkButton)) {
                                            $('#divBrowseFooter').find('.col-sm:eq(1)').show();
                                        } else {
                                            $('#divBrowseFooter').find('.col-sm:eq(1)').hide();
                                        }
                                        $('#divBrowseFooter').find('.col-sm:eq(2)').show();
                                    } else {
                                        $('#divBrowseFooter').find('.col-sm:eq(2)').hide();
                                    }
                                },
                                //buttons: [{extend: 'excel', text: 'Download Excel File'}],
                                //dom: 'Bfrtip'
                                sDom: '<"top">rt<"bottom"ip><"clear">', //to show button add B to <"top"> (<"top"B>)
                                rowCallback: function (row, data, dataIndex) {
                                    $(row).data('data', data);
                                    if (vParams[0].rowCallback)
                                        vParams[0].rowCallback.apply(this, arguments);
                                },
                                bSort: (vParams[0].Sorting == undefined ? true : CBool(vParams[0].Sorting))
                            });

                            $('#tblBrowsePopup tbody')
                                .unbind('click')
                                .on('click', 'td', function () {
                                    var pData = dataTableBrowsePopup.row(this).data();
                                    if (pData != undefined) {
                                        var dt = $('#tblBrowsePopup').DataTable();
                                        pData.DataTableRowColumnIndex = dt.cell(this).index().columnVisible;
                                        TriggerClick(pData, $(this));
                                    }
                                })

                            $('#tblBrowsePopup').css({ width: '100%' });
                        } else {
                            setTimeout(function () {
                                var pUrl = url;

                                if ($.isFunction(vParams[0].BeforeAjaxSend)) 
                                    vParams[0].BeforeAjaxSend(vParams[0], data);

                                if (CStr2(vParams[0].Server).trim() != '') {
                                    pUrl = MergeUrlParameters({}, url, vParams[0].Server);
                                } else if (CStr2(vParams[0].RemoteServer).trim() != '') {
                                    if (mRemoteServerMode)
                                        pUrl = MergeUrlParameters({}, url, vParams[0].RemoteServer);
                                }

                                AjaxSend({
                                    url: pUrl,
                                    data: data,
                                    success: function (vData) {
                                        DisplayResults(vData);
                                    }, error: function (e) {
                                        Bs.MessageBox('There\'s something wrong while getting data from the server or maybe this feature required an internet connection.');
                                    }
                                });
                            }, 500)                            
                        }
                    }
                }

                function DisplayResults(vData) {
                    if (vData.Error) {
                        NewMessageBox(vData.Error);
                        return;
                    }
                    if (vParams[0].ReturnVariable != undefined) {
                        var arrVariable = (vParams[0].ReturnVariable).split('.');
                        for (var i = 0; i < arrVariable.length; i++) {
                            vData = vData[arrVariable[i]];
                        }
                        //magbase ta kung unsa ang usually return json format sa BrowseController
                        vData = {
                            Results: {
                                Rows: vData
                            }
                        };
                    }
                    if (styleformat == "table") {
                        CreateHeader();
                        DisplayBrowseResult(vData);
                    } else {
                        DisplayBrowseResult(vData);
                    }
                }

                function CreateHeader() {
                    if ($.fn.DataTable.isDataTable($('#tblBrowsePopup')))
                        $('#tblBrowsePopup').DataTable().clear().destroy();
                    var tr = $('<tr class="custom-theme-f" style="vertical-align:top; text-align:center;">');
                    var tblBrowsePopupHeader = $('#tblBrowsePopupHeader');

                    tblBrowsePopupHeader.empty();

                    if (LibraryName.toLowerCase() == 'librarypimproduct' && CBool(vParams[0].ShowPic)) {
                        tr.append($('<th>')
                                .css('text-align', 'center')
                                .text('Image')
                            )
                    }

                    for (var i = 0; i < vParams[0].DisplayText.length; i++) {
                        var data = vParams[0].DisplayText[i];
                        tr.append($('<th>')
                                .css('text-align', 'center')
                                .text((data.Label == undefined ? '' : data.Label))
                            )
                    }

                    tblBrowsePopupHeader.append(tr);
                }

                function DisplayBrowseResult(vData) {
                    var vRows;
                    var list = $('#browselistresult')
                    list.empty();
                    $('#tblBrowsePopupBody').empty();

                    if (CStr2(vData.Results) == '') {
                        vRows = vData[vParams[0].ReturnObjectName];
                    } else {
                        vRows = vData.Results.Rows;
                    }

                    arrSearchResults = vRows;

                    if (CBool(vParams[0].SelectMultiple)
                        || CBool(vParams[0].ShowOkButton)) {

                        $('#divBrowseFooter').show();
                    } else {
                        $('#divBrowseFooter').hide();
                    }

                    if (CBool(vParams[0].CopyText)) {
                        $('#divBrowseFooter').show();

                        if (CBool(vParams[0].SelectMultiple)
                        || CBool(vParams[0].ShowOkButton)) {
                            $('#divBrowseFooter').find('.col-sm:eq(1)').show();
                        } else {
                            $('#divBrowseFooter').find('.col-sm:eq(1)').hide();
                        }
                        $('#divBrowseFooter').find('.col-sm:eq(2)').show();
                    } else {
                        $('#divBrowseFooter').find('.col-sm:eq(2)').hide();
                    }

                    if (vParams[0].StyleFormat != undefined) {
                        if (vParams[0].StyleFormat == 'table') {
                            $('#divBrowseListFormat').hide();
                            $('#divBrowseTableFormat').show();
                        } else {
                            $('#divBrowseListFormat').show();
                            $('#divBrowseTableFormat').hide();
                        }
                    } else {
                        $('#divBrowseListFormat').show();
                        $('#divBrowseTableFormat').hide();
                    }

                    if ($.fn.DataTable.isDataTable($('#tblBrowsePopup'))) {
                        $('#tblBrowsePopup').DataTable().clear().destroy()
                    }

                    if (vRows.length > 0) {
                        if (vParams[0].AutoClick) {
                            $('#modal_Browse').modal('hide');
                            vParams[0].Click(vRows[0], isResultFromBarcodeScanner, mRemoteServerMode);
                            return;
                        }

                        if (styleformat == 'table') {
                            var dtColumns = [];

                            if (LibraryName.toLowerCase() == 'librarypimproduct' && CBool(vParams[0].ShowPic)) {
                                dtColumns.push({
                                    "data": function (data, type, full, meta){
                                        return '<img width="100" src="../Content/images/box.png" />'
                                    },
                                    createdCell: function (td, cellData, rowData, row, col){
                                        if (CStr2(rowData.Image) != '') {
                                            $(td).css('width', '100px')
                                                .data('data', rowData)
                                                .click(function(e){
                                                    e.preventDefault();
                                                    e.stopPropagation();
                                                    ShowProductImage($(this).data('data').ID);
                                                })
                                                .find('img').attr('src', '/Content/MobizImagePIM/' + sessionStorage.OrgID + '/Medium/' + rowData.Image)
                                        }
                                    }
                                });
                            }

                            for (var m = 0; m < vParams[0].DisplayText.length; m++) {
                                var params = vParams[0].DisplayText[m];
                                var pRender = function (data) {
                                    return CStr2(data);
                                };
                                var createdcell = null;
                                if ($.isFunction(params.Render))
                                    pRender = params.Render;
                                if ($.isFunction(params.CreatedCell))
                                    createdcell = params.CreatedCell // params: td, cellData, rowData, row, col
                                dtColumns.push({
                                    "data": params.Column,
                                    "width": (params.Style == undefined ? 'auto' : params.Style.width),
                                    "render": pRender,
                                    className: params.className,
                                    createdCell: createdcell
                                });
                            }

                            var pDataTableArgs = {
                                order: [(vParams[0].OrderBy == undefined ? [0, 'asc'] : vParams[0].OrderBy)], //vParams[0].OrderBy is array . use only in display table
                                data: vRows,
                                fixedHeader: vParams[0].FixedHeader,
                                //"scrollX": true,
                                "scrollY": vParams[0].FixedHeader ? '300px' : '',
                                //"scrollCollapse": true,
                                columns: dtColumns,
                                "lengthMenu": [[-1, 10, 25, 50, 75, 100], ['All', 10, 25, 50, 75, 100]],
                                "aLengthMenu": [[-1, 10, 25, 50, 75, 100], ['All', 10, 25, 50, 75, 100]],
                                initComplete: function () {
                                    PopupReposition();
                                    if ($.isFunction(vParams[0].InitComplete))
                                        vParams[0].InitComplete(this, vRows); // "this" is a table jquery object
                                },
                                //buttons: [{extend: 'excel', text: 'Download Excel File'}],
                                //dom: 'Bfrtip'
                                //sDom: '<"top">rt<"bottom"ip><"clear">', //to show button add B to <"top"> (<"top"B>)
                                searching: false,
                                rowCallback: function (row, data, dataIndex) {
                                    $(row).data('data', data);
                                    if (vParams[0].rowCallback)
                                        vParams[0].rowCallback.apply(this, arguments);
                                },
                                bSort: (vParams[0].Sorting == undefined ? true : CBool(vParams[0].Sorting)),
                                drawCallback: function(settings){
                                    if (LibraryName.toLowerCase() == "LibraryOrganization".toLowerCase()) {
                                        $('#tblOption [name="tdAddNewCustomer"]').show();
                                    }
                                }
                            };
                            if (CStr2(vParams[0].DownloadCsvTitle).trim() != '') {
                                pDataTableArgs.buttons = [
                                    {
                                        extend: 'csvHtml5',
                                        text: 'Download CSV',
                                        title: vParams[0].DownloadCsvTitle
                                    }
                                ];
                                pDataTableArgs.sDom = '<"top"B>rt<"bottom"ip><"clear">';
                            }
                            if (vParams[0].NoInitialSorting)
                                pDataTableArgs.order = [];

                            dataTableBrowsePopup = $('#tblBrowsePopup').DataTable(pDataTableArgs);

                            $('#tblBrowsePopup tbody')
                                .unbind('click')
                                .on('click', 'td', function () {
                                    var pData = dataTableBrowsePopup.row(this).data();
                                    if (pData != undefined) {
                                        var dt = $('#tblBrowsePopup').DataTable();
                                        pData.DataTableRowColumnIndex = dt.cell(this).index().columnVisible;
                                        TriggerClick(pData, $(this));
                                    }
                                })

                            $('#tblBrowsePopup').css({ width: '100%' });
                        } else {
                            for (var i = 0; i < vRows.length; i++) {
                                var listName = $('<a href="" data-role="none" class="list-group-item list-group-item-action d-flex justify-content-between align-items-center">');
                                var isWideSpace = false, text = $('<div style="width:100%;">');; // <br><br>
                                var tr = $('<tr>'); // for table format

                                if (LibraryName.toLowerCase() == 'librarypimproduct' && CBool(vParams[0].ShowPic)) {
                                    listName.append($('<div style="margin-right:10px;">')
                                            .append($('<img style="width:100px;">')
                                                .attr('id', 'cartimg_' + vRows[i]['ID'])
                                                .attr('src', '../Content/images/box.png')
                                            )
                                        )
                                }

                                for (var m = 0; m < vParams[0].DisplayText.length; m++) {
                                    var params = vParams[0].DisplayText[m];
                                    var txtName = '', style = (params.Style == undefined ? {} : params.Style), label = '';

                                    if (params.Label != undefined) {
                                        if ($.isFunction(params.Label)) {
                                            label = params.Label(vRows[i]);
                                        } else {
                                            label = params.Label.replace(/\"/g, "");
                                        }
                                    }                                    

                                    if ($.isFunction(params.Column)) {
                                        txtName = params.Column(vRows[i]);
                                    } else {
                                        txtName = params.Column.replace(/\"/g, "");
                                        txtName = $('<textarea />').html(vRows[i][txtName]).text();
                                    }

                                    if (styleformat == 'table') {
                                        tr.append($('<td>')
                                            .css(style)
                                            .attr((params.Attr == undefined ? {} : params.Attr))
                                            .html('<b class="ui-table-cell-label">' + label + '</b>' + txtName))
                                    } else {
                                        style["padding"] = "1em";

                                        listName.css(style);

                                        var lbl = $('<div>').append((params.Label == undefined ? $('<div style="font-weight: ' + (m == 0 ? (vParams[0].DisplayText.length == 2 ? 'bold' : 'normal') : 'normal') + '">').html(txtName) : ' <span style="font-weight:bold">' + label + ':</span> ' + txtName))
                                        
                                        if (m == 1) {
                                            //text.append('<br>');
                                        }                                        

                                        text.append(lbl);
                                        listName.append(text);
                                    }
                                }

                                var isExist = '';
                                for (var w = 0; w < arrSelectedData.length; w++) {
                                    if (JSON.stringify(vRows[i]) == JSON.stringify(arrSelectedData[w])) {
                                        isExist = 'browseSelected';
                                    }
                                }

                                if (styleformat == "table") {
                                    $('#tblBrowsePopupBody').append(tr
                                            .css('cursor', 'pointer')
                                            .addClass(isExist)
                                            .data('data', vRows[i])
                                            .click(function (e) {
                                                e.preventDefault();
                                                TriggerClick($(this).data('data'), $(this));
                                            })
                                        );
                                } else {
                                    list.append(listName
                                            .append($('<span class="bi-chevron-right">'))
                                            .addClass(isExist)
                                            .data('data', vRows[i])
                                            .unbind('click')
                                            .click(function (e) {
                                                e.preventDefault();
                                                TriggerClick($(this).data('data'), $(this));
                                            })
                                        )
                                }

                                if (vRows[i].Image != undefined){
                                    $('#cartimg_' + vRows[i]['ID']).attr('src', '/Content/MobizImagePIM/' + sessionStorage.OrgID + '/Medium/' + vRows[i].Image)
                                        .data('data', vRows[i])
                                        .click(function(e){
                                            e.preventDefault();
                                            e.stopPropagation();
                                            ShowProductImage($(this).data('data').ID);
                                        })
                                }
                            }

                            if ($.isFunction(vParams[0].InitComplete))
                                vParams[0].InitComplete(this, vRows); // "this" is a table jquery object

                            if (LibraryName.toLowerCase() == "LibraryOrganization".toLowerCase()) {
                                $('#tblOption [name="tdAddNewCustomer"]').show();
                            }
                        }

                        if (CBool(vParams[0].AllowToSelectAll)){
                            if (styleformat == "table") {
                                $('#tblBrowsePopupBody').prepend($('<tr>')
                                        .append($('<td colspan="20">').html('<b>*** Click here to select all ***</b>'))
                                        .click(function (e) {
                                            e.preventDefault();
                                            TriggerClick(vRows, $(this), true);
                                        })
                                    )
                            } else {
                                list.prepend($('<a href="" data-role="none" class="list-group-item list-group-item-action d-flex justify-content-between align-items-center" style="padding:1em;">')
                                    .append('<b>*** Click here to select all ***</b>')
                                    .append($('<span class="bi-chevron-right">'))
                                    .click(function (e) {
                                        e.preventDefault();
                                        TriggerClick(vRows, $(this), true);
                                    })
                                )
                            }                            
                        }
                    }
                    else {
                        if (styleformat == 'table') {
                            $('#tblBrowsePopupBody').append('<tr><td style="text-align:center" colspan="' + vParams[0].DisplayText.length + '">No Results(s) Found</td></tr>')
                        } else {
                            list.append('<a data-role="none" class="list-group-item list-group-item-action d-flex justify-content-between align-items-center">No Result(s) Found</a>');
                        }
                        //$('[name=trDefineNewProduct]').show();
                        (arrSelectedData.length > 0 ? $('#divBrowseFooter').show() : $('#divBrowseFooter').hide());

                        if (CBool(vParams[0].CopyText)) {
                            $('#divBrowseFooter').show();
                            $('#divBrowseFooter').find('.col:eq(2)').show();
                        }
                        if (CInt2($('[name="rbFilter"]:checked').val()) == 0) {
                            $('#tblOption [name=trDisplayShowInactive]').show();
                        }

                        if (LibraryName.toLowerCase() == "LibraryOrganization".toLowerCase()) {
                            $('#tblOption [name="tdAddNewCustomer"]').show();
                        }

                        if ($.isFunction(vParams[0].NoRecordCallback))
                            vParams[0].NoRecordCallback();

                        
                    }
                    isFromListData = false;
                    if (CStr2(vParams[0].RemoteServer) != '') {
                        $('#divBrowseFooter').show();
                        $('#divBrowseFooter [name=divReturnablesFooter]').show();

                        if (mLnkRemoteServerToggle == null) {
                            mLnkRemoteServerToggle = $('#divBrowseFooter [name=lnkReturnableFooter]');
                            (function(vMyData) {
                                data = vMyData; // this is needed 2022_07_07__15_11_30_1178
                                mLnkRemoteServerToggle.click(function() {
                                    if (mRemoteServerMode)
                                        mLnkRemoteServerToggle.text('Can\'t find what you\'re looking for? Click here to browse for online invoices.');
                                    else
                                        mLnkRemoteServerToggle.text('Can\'t find what you\'re looking for? Click here to browse for offline invoices.');
                                    mRemoteServerMode = !mRemoteServerMode;
                                    BrowseNow();
                                }); // END mLnkRemoteServerToggle.click(function() {
                            })(data);
                        } // END if (mLnkRemoteServerToggle == null) {

                        if (mRemoteServerMode)
                            mLnkRemoteServerToggle.text('Can\'t find what you\'re looking for? Click here to browse for offline invoices.');
                        else
                            mLnkRemoteServerToggle.text('Can\'t find what you\'re looking for? Click here to browse for online invoices.');
                    }

                    if (typeof vParams[0].ShowAddButton == 'function') {
                        $('#browseOption').show();
                        $('#trBrowseAddButton').remove();
                        $('#tblOption')
                            .append($('<tr id="trBrowseAddButton">')
                                .append($('<td style="width:100%;">')
                                    .append($('<a href="#">')
                                        .text('Can\'t find what you\'re looking for? Click here to add new.')
                                        .click(function (e) {
                                            e.preventDefault();
                                            e.stopPropagation();
                                            vParams[0].ShowAddButton();
                                            $('#modal_Browse').modal('hide');
                                        })
                                    )
                                )
                            )
                    }
                }

                function TriggerClick(data, element, isSelectAllList) {
                    if (vParams[0].Validation != undefined) {
                        var result = vParams[0].Validation(data, isResultFromBarcodeScanner);
                        if (result != true) {
                            if (result != false) {
                                $('#divMsgErrorBrowseLibrary').show();
                                $('#divMsgErrorBrowseLibrary').html(result);
                            }
                            else {
                                $('#divMsgErrorBrowseLibrary').hide();
                            }
                            return false;
                        }
                    }

                    if (!CBool(vParams[0].SelectMultiple) || CBool(isSelectAllList)) {
                        if (!CBool(vParams[0].DontCloseWhenClick)) {
                            $('#modal_Browse').modal('hide');
                        }

                        if ($.isArray(data)){
                            for (var i in arrSelectedData){
                                var isExist = false;
                                for (var s in data){
                                    if (JSON.stringify(data[s]) == JSON.stringify(arrSelectedData[i])){
                                        isExist = true;
                                        break
                                    }
                                }

                                if (isExist) continue;
                                else data.push(arrSelectedData[i]);
                            }
                        }

                        closeFunction = '';
                        setTimeout(function () {
                            vParams[0].Click(data, isResultFromBarcodeScanner, mRemoteServerMode);
                        }, 400)
                    } else {
                        if (styleformat.toLowerCase() == 'table') {
                            element = element.parent();
                        }

                        if (!element.hasClass('browseSelected')) {
                            element.addClass('browseSelected');
                            arrSelectedData.push(data);
                        } else {
                            element.removeClass('browseSelected');

                            for (var i = 0; i < arrSelectedData.length; i++) {
                                if (data.ID == arrSelectedData[i].ID) {
                                    (arrSelectedData).splice(i, 1);
                                    break;
                                }
                            }
                        }

                        $('#divNoOfSelectedData').text('').hide();
                        if (arrSelectedData.length > 0) {
                            $('#divNoOfSelectedData')
                                .show()
                                .text(arrSelectedData.length + ' selected')
                                .unbind('click').click(function () {  
                                    var selectedList = [];
                                    for (var i in arrSelectedData) {
                                        var text = '';
                                        for (var m = 0; m < vParams[0].DisplayText.length; m++) {
                                            var params = vParams[0].DisplayText[m];
                                            var txtName = '', style = (params.Style == undefined ? {} : params.Style), label = '';

                                            if (params.Label != undefined) {
                                                if ($.isFunction(params.Label)) {
                                                    label = params.Label(vRows[i]);
                                                } else {
                                                    label = params.Label.replace(/\"/g, "");
                                                }
                                            }

                                            if ($.isFunction(params.Column)) {
                                                txtName = params.Column(vRows[i]);
                                            } else {
                                                txtName = params.Column.replace(/\"/g, "");
                                                txtName = $('<textarea />').html(arrSelectedData[i][txtName]).text();
                                            }

                                            if (text != '') text += '<br>';

                                            text += '<b>' + label + '</b>' + txtName;
                                        }
                                        selectedList.push({ Text: text });
                                    }

                                    Bs.PopupButtonList('', 'Selected data', selectedList);
                                })
                        }
                    }
                }

                window.BrowsePopupNow = function () {
                    if ($.isFunction(fxLibraryName))
                        LibraryName = fxLibraryName;
                    BrowseNow();
                }
            },
            MessageBoxClose: function () {
                $('.modal.show').modal('hide');
            },
            PopupButtonList: function (msg, title, vParams, vOption) {
                $('#modal_ContextualMenu').removeAttr('data-backdrop data-keyboard');
                var listButton = $('#lstOfCustomList'), arrSelectedData = [];
                listButton.empty();

                if (title == '') {
                    title = '&nbsp';
                }

                if (msg != '') {
                    $('#txtCustomListMsg').show();
                    $('#txtCustomListMsg').html(msg);
                }
                else {
                    $('#txtCustomListMsg').hide();
                }

                $('#divContextualMenuFooter').hide();

                if (vOption != undefined) {
                    if (vOption.CloseButton != undefined) {
                        if (vOption.CloseButton == "enable") {
                            $('#btnCustomPopupClose').show();
                        }
                        else {
                            $('#btnCustomPopupClose').hide();
                        }
                    }

                    if (vOption.AutoDismiss != undefined) {
                        if (vOption.AutoDismiss == false) {
                            $('#modal_ContextualMenu').attr({
                                'data-backdrop': 'static',
                                'data-keyboard': 'false'
                            })
                        } else {
                            $('#modal_ContextualMenu').removeAttr('data-backdrop data-keyboard');
                        }
                    }
                    else {
                        $('#modal_ContextualMenu').removeAttr('data-backdrop data-keyboard');
                    }

                    if (CBool(vOption.WithSearchbox)){
                        $('#txtCustomListSearch').parent().show();
                    } else {
                        $('#txtCustomListSearch').parent().hide();
                    }

                    if (CBool(vOption.SelectMultiple)) {
                        $('#divContextualMenuFooter').show();
                    }
                } else {
                    vOption = {};
                }

                $('#txtCustomListTitle').html(title);

                $('#txtCustomListSearch').unbind('keydown').keydown(function(e) {
                    if (e.which == 13){
                        var keyword = $.trim($(this).val()).toLowerCase();

                        if (keyword != '') {
                            var list = listButton.find('a');

                            list.each(function(){
                                var dis = $(this);
                                if (dis.text().toLowerCase().indexOf(keyword) == -1) {
                                    dis.removeClass('d-flex');
                                    dis.addClass('d-none');
                                } else {
                                    dis.addClass('d-flex');
                                    dis.removeClass('d-none');
                                }
                            })
                        } else {
                            listButton.find('a').addClass('d-flex').removeClass('d-none');
                        }
                    }
                }).val('');

                if (vParams.length > 0) {
                    for (var i = 0; i < vParams.length; i++) {
                        var setClassForImage = '';
                        var Attr = { x: 'y' };
                        var data = vParams[i], style = {};

                        if (vParams[i].HasImage != undefined) {
                            if (vParams[i].HasImage == true) {
                                setClassForImage = 'ui-li-has-thumb';
                            }
                        }

                        if (vParams[i].attr != undefined) {
                            Attr = vParams[i].attr;
                        }

                        if (vParams[i].data != undefined) {
                            data = $.extend(data, vParams[i].data);
                        }

                        if (vParams[i].Visible == undefined) vParams[i].Visible = true;

                        if (!CBool(vParams[i].Visible)) continue;

                        style["padding"] = "1em";

                        if (CBool(vOption.SelectMultiple)) {
                            if (CStr2(vParams[i].ClassName).split(' ').includes('browseSelected')) {
                                arrSelectedData.push(data);
                            }
                        }

                        listButton.append($('<a href="#" class="list-group-item list-group-item-action d-flex justify-content-between align-items-center ' + (vParams[i].ClassName == undefined ? '' : vParams[i].ClassName) + '">')
                                        .css(style)
                                        .attr('arr', i)
                                        .attr(Attr)
                                        .data('data', data)
                                        .append($('<div style="width:100%; text-align:' + (vOption.TextAlign || 'left') + ';">')
                                            .append($('<div style="font-weight: '+ (vParams[i].Description != undefined ? 'bold' : 'normal') +'">').html(vParams[i].Text))
                                            .append(vParams[i].Description != undefined ? 
                                            $('<div>').html(vParams[i].Description)
                                            : '')
                                        )                                        
                                        .append($('<span class="bi-chevron-right">'))
                                        .unbind('click')
                                        .click(function (e) {
                                            e.preventDefault();
                                            var dis = $(this), vData = dis.data('data');

                                            if (CBool(vOption.SelectMultiple)) {
                                                if (!dis.hasClass('browseSelected')) {
                                                    dis.addClass('browseSelected');
                                                    arrSelectedData.push(vData);
                                                } else {
                                                    dis.removeClass('browseSelected');

                                                    for (var i = 0; i < arrSelectedData.length; i++) {
                                                        if (JSON.stringify(vData) == JSON.stringify(arrSelectedData[i])) {
                                                            (arrSelectedData).splice(i, 1);
                                                            break;
                                                        }
                                                    }
                                                }
                                                return
                                            }

                                            if (vParams[CInt2(dis.attr('arr'))].Validation != undefined) {
                                                if (vParams[CInt2(dis.attr('arr'))].Validation(vData) != true) {
                                                    return false;
                                                }
                                            }

                                            if (vParams[CInt2(dis.attr('arr'))].Click != undefined) {
                                                $('#modal_ContextualMenu').modal('hide');
                                                //$('.modal-backdrop').remove();
                                                //$('body').removeClass('modal-open');
                                                setTimeout(function () {
                                                    vParams[CInt2(dis.attr('arr'))].Click(vData);
                                                    ClosePopUp(vOption)
                                                }, 400)                                                
                                            }
                                        })
                                    )
                    }
                }
                else {
                    listButton.append($('<a href="#" class="list-group-item list-group-item-action">')
                        .text('No Result(s) Found')
                    )
                }

                $('#btnOkContextualMenuSelectMultiple').unbind('click').click(function () {
                    vParams[0].Click(arrSelectedData);
                    $('#modal_ContextualMenu').modal('hide');
                })

                $('#modal_ContextualMenu').unbind('hidden.bs.modal').on('hidden.bs.modal', function () {
                    ClosePopUp(vOption);
                })

                $('#modal_ContextualMenu').modal('dispose').modal('show');
            },
            PromptUser: function (strModuleName, vParams) {
                //('.modal').modal('hide');

                $("#txtPromptUserUsername").val('');
                $("#txtPromptUserPassword").val('');

                vParams = $.extend(vParams, vParams);
                mParams = vParams;
                var vDataAccess;
                var pData = {};
                pData.ModuleName = strModuleName;

                if (sessionStorage["OrgID"] != undefined) {
                    pData.OrgID = sessionStorage["OrgID"];
                }

                if (CStr2(vParams.Message) != '') {
                    $('#lblPromptUserMsg').html(vParams.Message);
                } else {
                    $('#lblPromptUserMsg').html('You don\'t have access to <b>' + strModuleName + '</b>');
                }

                if (OfflineMode) {
                    var IsExists;

                    IsExists = Transfers.ReadFile(sessionStorage.OrgID + '.flg')
                    if (IsExists == undefined) {
                        window.location = '/SignIn/DownloadDatabase?IsMobile=1';
                        return;
                    }

                    pData.UserID = Web.PeopleUserID();

                    Web.SetOrgID(sessionStorage["OrgID"]);
                    Web.HandleOrganization();
                    vDataAccess = security_offline().CheckSecurityCurrentUserAcccess(pData);

                    if (vDataAccess.Rows.length > 0) {
                        if ($.isFunction(vParams.OnClickOKSuccess)) {
                            vParams.OnClickOKSuccess(vDataAccess);
                        }
                    } else {
                        $('#modal_PromptUser').modal('show');
                        $('#SecurityAccessTitle').text(strModuleName);

                        $('#btnPromptUserOk').unbind('click').click(function () {
                            //Check if has access
                            pData = {};
                            pData.ModuleName = strModuleName;
                            pData.Username = $("#txtPromptUserUsername").val();
                            pData.UserPassword = $("#txtPromptUserPassword").val();
                            pData.UserID = security_offline().CheckSecurityUserAcccess(pData);

                            if (sessionStorage["OrgID"] != undefined) {
                                pData.OrgID = sessionStorage["OrgID"];
                            }

                            vDataAccess = security_offline().CheckSecurityCurrentUserAcccess(pData);

                            if (vDataAccess.Rows.length > 0) {
                                if ($.isFunction(vParams.OnClickOKSuccess)) {
                                    $('#modal_PromptUser').modal('hide');                                    
                                    $("#txtPromptUserUsername, #txtPromptUserPassword").val('');
                                    setTimeout(function () {
                                        vParams.OnClickOKSuccess(vDataAccess);
                                    }, 400);
                                }
                            } else {
                                WebToast('Invalid username or password');
                                $("#txtPromptUserUsername, #txtPromptUserPassword").val('');
                            }
                        })
                    }
                }
                else {
                    Bs.Loading('show');
                    AjaxSend({
                        url: "SecurityRole/CheckSecurityCurrentUserAcccess",
                        data: pData,
                        success: function (vData) {
                            Bs.Loading('hide');
                            if (vData.SecurityRoleData.Rows.length > 0) {
                                if ($.isFunction(vParams.OnClickOKSuccess)) {
                                    vParams.OnClickOKSuccess(vData);
                                }
                            } else {
                                $('#modal_PromptUser').modal('show');
                                var pLabel = vData.Label;
                                if (CStr2(pLabel).trim() == '') {
                                    pLabel = strModuleName;
                                    $('#lblPromptUserMsg').html('You don\'t have access <b>' + pLabel + '</b>');
                                    $('#SecurityAccessTitle').text(pLabel);
                                }

                                $('#btnPromptUserOk').unbind('click').click(function () {
                                    var pData = {};
                                    pData.ModuleName = strModuleName;
                                    pData.Username = $("#txtPromptUserUsername").val();
                                    pData.UserPassword = $("#txtPromptUserPassword").val();

                                    if (sessionStorage["OrgID"] != undefined) {
                                        pData.OrgID = sessionStorage["OrgID"];
                                    }

                                    AjaxSend({
                                        url: "SecurityRole/CheckSecurityUserAcccess",
                                        data: pData,
                                        success: function (vDataUserAcess) {

                                            if (vDataUserAcess.SecurityRoleData.Rows.length > 0) {
                                                if ($.isFunction(vParams.OnClickOKSuccess)) {
                                                    $('#modal_PromptUser').modal('hide');
                                                    $("#txtPromptUserUsername, #txtPromptUserPassword").val('');
                                                    setTimeout(function () {
                                                        vParams.OnClickOKSuccess(vDataUserAcess);
                                                    }, 400);
                                                }
                                            } else {
                                                WebToast('Invalid username or password');
                                                $("#txtPromptUserUsername, #txtPromptUserPassword").val('');
                                            }
                                        }
                                    });
                                })                                
                            }
                        }
                    });
                }

                $('#btnPromptUserCancel').unbind('click').click(function(){
                    if ($.isFunction(mParams.OnFailed))
                        setTimeout(function () {
                            vParams.OnFailed();
                        }, 400);
                    if ($.isFunction(vParams.OnCancel)) {
                        $('#modal_PromptUser').modal('hide');
                        setTimeout(function () {
                            vParams.OnCancel();
                        }, 400);
                    }
                })
            },
            WebToast: function (msg, position, delay) {
                $('#toast_msg').html(msg);
                $('#toast').toast('dispose').toast({ delay: delay || 5000 }).toast('show').show();
            },
            Loading: function (display, text) {
                $('#bsloadinglbl').html((text == undefined ? 'Please wait' : text));

                if (display == 'show') {
                    $('#bsloading').show();
                } else {
                    $('#bsloading').hide();
                }
            },
            Calendar: function (vParams) {
                var m_DateNow = new Date();
                var arrDate = {}, isShowTime = false;

                isUniversalPopup = true;

                if (!IsMobile){
                    vParams.HidePrevPopup = false;
                }

                if (vParams.HidePrevPopup == undefined || vParams.HidePrevPopup == true){
                    $('.modal.show').modal('hide');
                }
                
                $('#modal_Calendar').modal('show');

                $('#divSingleCalendarTime').hide();

                if (vParams.ClearButton != undefined || vParams.Clear != undefined) {
                    $('#divClearBtnSingleCalendar').show();
                    $('#btnClearSingleCalendar').show().text(vParams.ClearButton || 'Unset Date');
                }
                else {
                    $('#divClearBtnSingleCalendar').hide();
                }

                if (vParams.Close != undefined || vParams.Cancel != undefined) {
                    closeFunction = (vParams.Close || vParams.Cancel);
                }

                if (vParams.ShowTime != undefined) { // value must be boolean
                    if (CBool(vParams.ShowTime)) {
                        isShowTime = true;
                        $('#divSingleCalendarTime').show();
                        var selSingleTimeHour = $('#selSingleTimeHour');
                        var selSingleTimeMinute = $('#selSingleTimeMinute');
                        var selSingleTimeMeredian = $('#selSingleTimeMeredian');
                        var hour = 1, minute = 0;

                        selSingleTimeHour.empty();
                        selSingleTimeMinute.empty();

                        for (var i = 0; i < 12; i++) {
                            selSingleTimeHour.append(new Option(hour, hour));
                            hour += 1;
                        }

                        for (var i = 0; i <= 59; i++) {
                            var formatnumber = $.format.number(minute, '00');
                            selSingleTimeMinute.append(new Option(formatnumber, formatnumber));
                            minute += 1;
                        }

                        if (vParams.SetTime != undefined) { // must be 12 hr format
                            var time = (vParams.SetTime).split(':');
                            selSingleTimeHour.val(CInt2(time[0]));
                            selSingleTimeMinute.val(parseInt(time[1]));
                            selSingleTimeMeredian.val($.trim((vParams.SetTime).split(' ')[1]));
                        }
                    }
                }

                if (vParams.Title != undefined) {
                    $('#txtCustomSingleCalendarTitle').text(vParams.Title);
                } else {
                    $('#txtCustomSingleCalendarTitle').text('Date');
                }

                $(".fromCustomSingleCalendar").calendar({
                    format: 'mmmm dd, yyyy',
                    multiSelect: false,
                    otherDays: true,
                    minDate: (vParams.MinDate == undefined ? false : vParams.MinDate),
                    maxDate: (vParams.MaxDate == undefined ? false : vParams.MaxDate),
                    getDates: function (data) {
                        $("#txtDateSingleCalendar").text($.format.date(new Date(data), (vParams.Format == undefined ? "MMMM dd, yyyy" : vParams.Format)));
                    }
                });

                $('.fromCustomSingleCalendar').calendar('unsetDate');

                if (vParams.SetDate != undefined) {
                    $('.fromCustomSingleCalendar').calendar('setDate', $.format.date(new Date(vParams.SetDate), (vParams.Format == undefined ? "MMMM dd, yyyy" : vParams.Format)));
                }
                else {
                    $('.fromCustomSingleCalendar').calendar('setDate', $.format.date(m_DateNow, (vParams.Format == undefined ? "MMMM dd, yyyy" : vParams.Format)));
                }

                $('#btnDoneSingleCalendar').unbind('click').click(function () {
                    var setDate = $.format.date(new Date($('#txtDateSingleCalendar').text()), (vParams.Format == undefined ? "MMM dd, yyyy" : vParams.Format)) + (isShowTime ? ' ' + selSingleTimeHour.val() + ':' + selSingleTimeMinute.val() + ' ' + selSingleTimeMeredian.val() : '');
                    if (vParams.SetText != undefined) {
                        if ($.trim($('#txtDateSingleCalendar').text()) != "") {
                            $('#modal_Calendar').modal('hide');
                            ClosePopUp(vParams);
                            $(vParams.SetText).text(setDate);
                            setTimeout(function () {
                                vParams.Click($('#txtDateSingleCalendar').text());
                            }, 400);
                        }
                        else {
                            $('#modal_Calendar').modal('hide');
                            ClosePopUp(vParams);
                            $(vParams.SetText).text(vParams.Placeholder);
                            setTimeout(function () {
                                vParams.Click('');
                            }, 400);
                        }
                    }
                    else {
                        $('#modal_Calendar').modal('hide');
                        ClosePopUp(vParams);
                        setTimeout(function () {
                            vParams.Click(setDate);
                        }, 400);
                    }
                })

                $('#btnCancelSingleCalendar').unbind('click').click(function () {
                    $('#modal_Calendar').modal('hide');
                    ClosePopUp(vParams);
                    if (vParams.Cancel != undefined) {
                        vParams.Cancel();
                    }
                })

                $('#btnClearSingleCalendar').unbind('click').click(function () {
                    $('#modal_Calendar').modal('hide');
                    ClosePopUp(vParams);
                    setTimeout(function () {
                        vParams.Click('');
                    }, 400);
                })

                $('#modal_Calendar').unbind('hidden.bs.modal').on('hidden.bs.modal', function () {
                    ClosePopUp(vParams);
                })
            },
            DateRange: function (vParams, m_DateFrom, m_DateTo) {
                var arrDate = {}, isShowTimeFrom = false, isShowTimeTo = false;

                //$('.modal.show').modal('hide');
                $('#modal_DateRange').modal('show');

                $('#divDualCalendarTimeFrom, #divDualCalendarTimeTo').hide();

                $(".fromCustomDualCalendar").calendar({
                    format: "mmmm dd, yyyy",
                    multiSelect: false,
                    otherDays: true,
                    minDate: (vParams.MinFromDate == undefined ? false : vParams.MinFromDate),
                    maxDate: (vParams.MaxFromDate == undefined ? false : vParams.MaxFromDate),
                    getDates: function (data) {
                        $("#txtDateFromDualCalendar").text($.format.date(new Date(data), 'MMM dd, yyyy'));
                        $('#txtErrorMsgDualCalendar').hide();
                    }
                });

                $(".toCustomDualCalendar").calendar({
                    format: "mmmm dd, yyyy",
                    multiSelect: false,
                    otherDays: true,
                    minDate: (vParams.MinToDate == undefined ? false : vParams.MinToDate),
                    maxDate: (vParams.MaxToDate == undefined ? false : vParams.MaxToDate),
                    getDates: function (data) {
                        $("#txtDateToDualCalendar").text($.format.date(new Date(data), 'MMM dd, yyyy'));
                        $('#txtErrorMsgDualCalendar').hide();
                    }
                });

                if (vParams.Title != undefined) {
                    $('#txtCustomDualCalendarTitle').text(vParams.Title);
                } else {
                    $('#txtCustomDualCalendarTitle').text('Date Range');
                }

                if (vParams.ShowTime != undefined) {
                    //vParams.ShowTime - accepted value 'ft', 'f', 't' (f = from, t = to)
                    var showtime = vParams.ShowTime;
                    var selDCTimeFromHour = $('#selDCTimeFromHour');
                    var selDCTimeToHour = $('#selDCTimeToHour');
                    var selDCTimeFromMinute = $('#selDCTimeFromMinute');
                    var selDCTimeToMinute = $('#selDCTimeToMinute');
                    var selDCTimeFromMeredian = $('#selDCTimeFromMeredian');
                    var selDCTimeToMeredian = $('#selDCTimeToMeredian');
                    var hour = 1; minute = 0;

                    $('#selDCTimeFromHour, #selDCTimeToHour, #selDCTimeFromMinute, #selDCTimeToMinute').empty();

                    if (showtime.indexOf('f') >= 0) {
                        isShowTimeFrom = true;
                        $('#divDualCalendarTimeFrom').show();
                    }

                    if (showtime.indexOf('t') >= 0) {
                        isShowTimeTo = true;
                        $('#divDualCalendarTimeTo').show();
                    }

                    for (var i = 0; i < 12; i++) {
                        selDCTimeFromHour.append(new Option(hour, hour));
                        selDCTimeToHour.append(new Option(hour, hour));
                        hour += 1;
                    }

                    for (var i = 0; i < 4; i++) {
                        var formatnumber = $.format.number(minute, '00');
                        selDCTimeFromMinute.append(new Option(formatnumber, formatnumber));
                        selDCTimeToMinute.append(new Option(formatnumber, formatnumber));
                        minute += 15;
                    }

                    if (vParams.SetFromTime != undefined) { // must be 12 hr format
                        var time = (vParams.SetFromTime).split(':');
                        selDCTimeFromHour.val(time[0]);
                        selDCTimeFromMinute.val(time[1]);
                        selDCTimeFromMeredian.val($.trim((time).split(' ')[1]));
                    }

                    if (vParams.SetToTime != undefined) { // must be 12 hr format
                        var time = (vParams.SetToTime).split(':');
                        selDCTimeToHour.val(CInt2(time[0]));
                        selDCTimeToMinute.val(CInt2(time[1]));
                        selDCTimeToMeredian.val($.trim((vParams.SetToTime).split(' ')[1]));
                    }
                }

                $('.fromCustomDualCalendar').calendar('unsetDate');
                $('.toCustomDualCalendar').calendar('unsetDate');

                $('.fromCustomDualCalendar').calendar('setDate', $.format.date(m_DateFrom, 'MMM dd, yyyy'));
                $('.toCustomDualCalendar').calendar('setDate', $.format.date(m_DateTo, 'MMM dd, yyyy'));

                $('#btnCancelDualCalendar').unbind('click').click(function () {
                    ClosePopUp();
                })

                $('#btnDoneDualCalendar').unbind('click').click(function () {
                    var from = $.trim($('#txtDateFromDualCalendar').text());
                    var to = $.trim($('#txtDateToDualCalendar').text());
                    var msg = $('#txtErrorMsgDualCalendar');

                    if (IsDate(from) == false || IsDate(to) == false) {
                        if (vParams.Required == undefined || vParams.Required == false) {
                            ClosePopUp();
                            setTimeout(function () {
                                vParams.Click('');
                            }, 400)                            
                        }
                        else {
                            WebToast('Please provide a valid date range');
                        }
                    }
                    else if (IsDate(from) && IsDate(to)) {
                        var datefrom = new Date(from);
                        var dateto = new Date(to);

                        if (datefrom > dateto || dateto < datefrom) {
                            WebToast('Please provide a valid date range');
                        }
                        else {
                            arrDate.Period = "Custom";
                            arrDate.From = $.format.date(datefrom, (vParams.Format || 'MMM dd, yyyy')) + (isShowTimeFrom ? ' ' + selDCTimeFromHour.val() + ':' + selDCTimeFromMinute.val() + ' ' + selDCTimeFromMeredian.val() : '');
                            arrDate.To = $.format.date(dateto, (vParams.Format || 'MMM dd, yyyy')) + (isShowTimeTo ? ' ' + selDCTimeToHour.val() + ':' + selDCTimeToMinute.val() + ' ' + selDCTimeToMeredian.val() : '');
                            ClosePopUp();

                            if (vParams.SetText != undefined) {
                                $(vParams.SetText).html(arrDate.From + ' to ' + arrDate.To);
                            }
                            $('#modal_DateRange').modal('hide');
                            setTimeout(function () {
                                vParams.Click(arrDate);
                            }, 400)                           
                        }
                    }
                })

                $('#modal_DateRange').unbind('hidden.bs.modal').on('hidden.bs.modal', function () {
                    ClosePopUp(vParams);
                })
            },
            SetTime: function (vParams) {
                var Format = { 13: 1, 14: 2, 15: 3, 16: 4, 17: 5, 18: 6, 19: 7, 20: 8, 21: 9, 22: 10, 23: 11, 24: 12 };
                var h = "1", m = "01", md = "AM"
                
                $('#modal_SetTime').modal('show').on('shown.bs.modal', function(){
                    $('#SetTimeHour input').focus();
                });

                if (vParams.Cancel != undefined || vParams.Close != undefined) {
                    closeFunction = (vParams.Cancel || vParams.Close);
                }

                if (CStr2(vParams.SetTime) != '') {
                    var setTime = new Date('1/1/2022 ' + vParams.SetTime), pHour = (setTime.getHours() >= 13 ? Format[setTime.getHours()] : setTime.getHours());
                    $('#txtSetTimeHour').val(pHour);
                    $('#txtSetTimeMinute').val($.format.number(setTime.getMinutes(), '00'));

                    if (setTime.getHours() > 11) {                        
                        $('#btnSetTimeMeridian').text('PM');
                    } else {                        
                        $('#btnSetTimeMeridian').text('AM');
                    }
                } else {
                    $('#txtSetTimeHour').val(8);
                    $('#txtSetTimeMinute').val('00');
                    $('#btnSetTimeMeridian').text('AM');
                }

                $('#btnSetTimeAsc').unbind().click(function () {
                    var timeType = $('.timeactive').attr('id');
                    var currentTime = CInt2($('#' + timeType).find('input[type="text"]').val());
                    var limitasc, setTime = 1;
                    if (timeType == "SetTimeHour") {
                        limitasc = 12;
                    }
                    else {
                        limitasc = 59;
                    }

                    if (currentTime < limitasc) {
                        currentTime = currentTime + setTime;
                    }
                    else if (currentTime == 59) {
                        currentTime = 0;
                    }
                    else {
                        currentTime = 1;
                    }

                    if (timeType == "SetTimeMinute") {
                        currentTime = (currentTime < 10 ? "0" + currentTime : currentTime);
                    }
                    $('#' + timeType).find('input[type="text"]').val(currentTime).focus();
                })

                $('#btnSetTimeDesc').unbind().click(function () {
                    var timeType = $('.timeactive').attr('id');
                    var currentTime = CInt2($('#' + timeType).find('input[type="text"]').val());
                    var limitasc, setTime = 1;
                    if (timeType == "SetTimeHour") {
                        limitasc = 12;
                    }
                    else {
                        limitasc = 59;
                    }

                    if (currentTime == 0) {
                        currentTime = 59;
                    }
                    else {
                        currentTime = currentTime - setTime;

                        //for hour
                        if (currentTime == 0 && timeType == "SetTimeHour") {
                            currentTime = 12
                        }
                    }

                    if (timeType == "SetTimeMinute") {
                        currentTime = (currentTime < 10 ? "0" + currentTime : currentTime);
                    }
                    $('#' + timeType).find('input[type="text"]').val(currentTime).focus();
                })

                $('#btnSetTimeMeridian').unbind().click(function () {
                    var val = $(this).text().toLowerCase();

                    if (val == "am") {
                        $(this).text('PM');
                    }
                    else {
                        $(this).text('AM');
                    }
                })

                $('#txtSetTimeHour').unbind().click(function () {
                    $('#SetTimeMinute').removeClass('timeactive');
                    $('#SetTimeHour').addClass('timeactive');
                    $(this).focus();
                })

                $('#txtSetTimeMinute').unbind().click(function () {
                    $('#SetTimeHour').removeClass('timeactive');
                    $('#SetTimeMinute').addClass('timeactive');
                    $(this).focus();
                })

                $('#SetTimeMinute').unbind().blur(function () {
                    var val = CInt2($(this).val());
                    if (val > 59) {
                        $(this).val('01')
                    }
                    else {
                        $(this).val((val < 10 ? "0" + val : val))
                    }
                }).keydown(function (e) {
                    if (!NumberOnly(e)) {
                        return;
                    }
                })

                $('#SetTimeHour').unbind().blur(function () {
                    var val = CInt2($(this).val());
                    if (val > 12) {
                        $(this).val('1')
                    }
                }).keydown(function (e) {
                    if (!NumberOnly(e)) {
                        return;
                    }
                })

                $('#txtSetTimeHour').click();

                $('#btnOkSetTime').unbind().click(function () {
                    var selectedTime = {
                        h: $('#SetTimeHour').find('input[type="text"]').val(),
                        m: $('#SetTimeMinute').find('input[type="text"]').val(),
                        md: $('#btnSetTimeMeridian').text()
                    }

                    selectedTime.time = selectedTime.h + ":" + selectedTime.m + " " + selectedTime.md;

                    $('#modal_SetTime').modal('hide');

                    setTimeout(function () {
                        vParams.Click(selectedTime)
                        ClosePopUp();
                    }, 400)                    
                })                
            },
            ContinuousBarcode: function (vParams) {
                var pProductBarcodeCount = [];

                Bs.PopupButtonList('The Loop Scan feature allows you to continuously scan barcodes and/or QR (Quick Response) codes without being interrupted by non-barcode input data. The purpose of Loop Scan is to quickly collect barcode/QR code data from your physical merchandise and then process these data for use with your intended application modules such as for inventory counting, sales ordering, stock receiving, stock transferring and other modules. <br><br> How would you like to scan barcodes/ QR codes?'
                    , 'Loop Scan', [
                        {
                            Text: 'Use the scanner app installed on my mobile device',
                            Description: 'This is a very convenient way to scan barcodes/ QR codes. ' +
                                        'However, since this method uses your device’s camera and is ' +
                                        'dependent on reflected environmental lighting, ' +
                                        'this method’s scanning performance is adversely affected by poor lightning conditions.',
                            Click: function () {
                                $('#ContinuousBarcodeItemList').hide();
                                $('#modal_ContinuousBarcode').modal('show').unbind('shown.bs.modal');

                                LoopBarcode();

                                function LoopBarcode() {
                                    mobile.Scan({
                                        Success: function (e) {
                                            var results;
                                            var isExist = false;

                                            pProductBarcodeCount.forEach(function (item) {
                                                if (e.Value === item.Barcode) {
                                                    item.Count++;
                                                    isExist = true;
                                                    return;
                                                }
                                            });

                                            if (!isExist) {
                                                pProductBarcodeCount.push({
                                                    Barcode: e.Value,
                                                    Count: 1
                                                })
                                            }

                                            LoopBarcode();
                                        },
                                        Error: function () {
                                            if ($.fn.DataTable.isDataTable("#tblBarcodeCount")) {
                                                //check if datatable is created 
                                                $('#tblBarcodeCount').DataTable().clear().destroy();
                                            }

                                            balancesDatatable = $('#tblBarcodeCount').DataTable({
                                                "data": pProductBarcodeCount,
                                                "columns": [
                                                    { "data": "Barcode" },
                                                    { "data": "Count" }
                                                ],
                                                "initComplete": function () {
                                                    $('#tblBarcodeCount').removeClass('dataTable').css('width', '100%');
                                                },
                                                "sDom": '<"top">rt<"bottom"ip><"clear">'
                                            })
                                        }
                                    });
                                }
                            }
                        }, {
                            Text: 'Use external USB (with OTG dongle if necessary) or Bluetooth bar code scanner',
                            Description: 'This method requires an external bar code scanner connected via USB or Bluetooth interface. ' +
                                        'Depending on the scan performance of your barcode scanner, ' +
                                        'this can be the most reliable way to capture barcode and/or ' +
                                        'QR code data as your external barcode scanner emits it own light ' +
                                        'and is not negatively affected by poor environmental lighting conditions.',
                            Click: function () {
                                $('#ContinuousBarcodeItemList').show();
                                $('#modal_ContinuousBarcode').modal('show').unbind('shown.bs.modal').on('shown.bs.modal', function () {
                                    $('#txtContinuousBarcode').focus();
                                });
                            }
                        }
                    ], {
                    TextAlign: 'center'
                    })

                if ($.fn.DataTable.isDataTable("#tblBarcodeCount")) {
                    //check if datatable is created 
                    $('#tblBarcodeCount').DataTable().clear().destroy();
                }

                balancesDatatable = $('#tblBarcodeCount').DataTable({
                    "data": pProductBarcodeCount,
                    "columns": [
                        { "data": "Barcode" },
                        { "data": "Count" }
                    ],
                    "initComplete": function () {
                        $('#tblBarcodeCount').removeClass('dataTable').css('width', '100%');
                    },
                    "sDom": '<"top">rt<"bottom"ip><"clear">'
                })

                $('#txtContinuousBarcode').unbind('keydown').keydown(function (e) {
                    if (e.keyCode == 13) {
                        var results;
                        var isExist = false;
                        var barcodeValue = '';

                        if ($('#txtContinuousBarcode').val() == '')
                            return

                        barcodeValue = $('#txtContinuousBarcode').val();

                        pProductBarcodeCount.forEach(function (item) {
                            if (barcodeValue === item.Barcode) {
                                item.Count++;
                                isExist = true;
                                return;
                            }
                        });

                        if (!isExist) {
                            pProductBarcodeCount.push({
                                Barcode: barcodeValue,
                                Count: 1
                            })
                        }

                        if ($.fn.DataTable.isDataTable("#tblBarcodeCount")) {
                            //check if datatable is created 
                            $('#tblBarcodeCount').DataTable().clear().destroy();
                        }

                        balancesDatatable = $('#tblBarcodeCount').DataTable({
                            "data": pProductBarcodeCount,
                            "columns": [
                                { "data": "Barcode" },
                                { "data": "Count" }
                            ],
                            "initComplete": function () {
                                $('#tblBarcodeCount').removeClass('dataTable').css('width', '100%');
                            },
                            "sDom": '<"top">rt<"bottom"ip><"clear">'
                        })

                        $('#txtContinuousBarcode').val('');
                        $('#txtContinuousBarcode').focus();
                    }

                });

                $('#btnContinuousBarcodeBoxOK').unbind('click').click(function () {
                    $('#modal_ContinuousBarcode').modal('hide');
                    setTimeout(function () {
                        vParams.Click(pProductBarcodeCount);
                    }, 400);
                })
            },
            PopupPeriod: function (vParams) {
                var m_DateFrom = new Date();
                var m_DateTo = new Date();
                var arrDate = {};
                var options = [];

                if (vParams.Display2DaysAgo) {
                    options.push({
                        Text: '2 Days Ago', Click: function () {
                            m_DateFrom.setDate(m_DateFrom.getDate() - 2);
                            m_DateTo.setDate(m_DateTo.getDate() - 2);

                            arrDate.Period = "2 days ago";
                            arrDate.From = $.format.date(m_DateFrom, 'MMM dd, yyyy');
                            arrDate.To = $.format.date(m_DateTo, 'MMM dd, yyyy');

                            ClosePopUp();
                            if (vParams.SetText != undefined) {
                                $(vParams.SetText).html('2 Days Ago');
                            }
                            vParams.Click(arrDate);
                        }
                    })
                }

                if (vParams.DisplayYesterday) {
                    options.push({
                        Text: 'Yesterday', Click: function () {
                            m_DateFrom.setDate(m_DateFrom.getDate() - 1);
                            m_DateTo.setDate(m_DateTo.getDate() - 1);

                            arrDate.Period = "Yesterday";
                            arrDate.From = $.format.date(m_DateFrom, 'MMM dd, yyyy');
                            arrDate.To = $.format.date(m_DateTo, 'MMM dd, yyyy');

                            ClosePopUp();
                            if (vParams.SetText != undefined) {
                                $(vParams.SetText).html('Yesterday');
                            }
                            vParams.Click(arrDate);
                        }
                    })
                }

                if (vParams.DisplayToday) {
                    options.push({
                        Text: 'Today', Click: function () {
                            arrDate.Period = "Today";
                            arrDate.From = $.format.date(m_DateFrom, 'MMM dd, yyyy');
                            arrDate.To = $.format.date(m_DateTo, 'MMM dd, yyyy');

                            ClosePopUp();
                            if (vParams.SetText != undefined) {
                                $(vParams.SetText).html('Today');
                            }
                            vParams.Click(arrDate);
                        }
                    })
                }

                if (vParams.DisplayWeekToDate) {
                    options.push({
                        Text: 'Week To Date', Click: function () {
                            m_DateFrom.setDate(m_DateFrom.getDate() - m_DateFrom.getDay());

                            arrDate.Period = "Week to Date";
                            arrDate.From = $.format.date(m_DateFrom, 'MMM dd, yyyy');
                            arrDate.To = $.format.date(m_DateTo, 'MMM dd, yyyy');

                            ClosePopUp();
                            if (vParams.SetText != undefined) {
                                $(vParams.SetText).html('Week To Date');
                            }
                            vParams.Click(arrDate);
                        }
                    })
                }

                if (vParams.DisplayMonthToDate) {
                    options.push({
                        Text: 'Month To Date', Click: function () {
                            m_DateFrom.setDate(m_DateFrom.getDate() - (m_DateFrom.getDate() - 1));

                            arrDate.Period = "Month to Date";
                            arrDate.From = $.format.date(m_DateFrom, 'MMM dd, yyyy');
                            arrDate.To = $.format.date(m_DateTo, 'MMM dd, yyyy');

                            ClosePopUp();
                            if (vParams.SetText != undefined) {
                                $(vParams.SetText).html('Month To Date');
                            }
                            vParams.Click(arrDate);
                        }
                    })
                }

                if (vParams.DisplayCustomDate) {
                    options.push({
                        Text: 'Custom Date', Click: function () {
                            Bs.Calendar({
                                //MaxDate: $.format.date(new Date(), 'MM-dd-yyyy'), // commented on 2023-11-13
                                Click: function (e) {
                                    vParams.Click({
                                        Period: 'Custom Date',
                                        From: e,
                                        To: e
                                    })
                                }
                            })
                        }
                    })
                }

                if (vParams.DisplayCustomPeriod) {
                    options.push({
                        Text: 'Custom Period', Click: function () {
                            Bs.DateRange(vParams, m_DateFrom, m_DateTo);
                        }
                    })
                }

                if (vParams.DisplayTodayOrEarlier) {
                    options.push({
                        Text: 'Today or Earlier', Click: function () {
                            ClosePopUp();
                            vParams.Click('Today or Earlier');
                        }
                    })
                }

                Bs.PopupButtonList('', 'Date Period', options)
            },
            NestedModal: function (currentmodal, nestedmodal, vOption) {
                //Once the nestedmodal is hidden, the currentmodal will be shown again

                $(currentmodal).unbind('hidden.bs.modal').modal('hide');
                $(nestedmodal).modal('show').unbind('hidden.bs.modal').on('hidden.bs.modal', function () {
                    $(nestedmodal).unbind('hidden.bs.modal');
                    $(currentmodal).modal('show');

                    if (typeof vOption != 'undefined'){
                        if (vOption.Hidden != undefined){
                            vOption.Hidden();
                        }
                    }
                }).unbind('shown.bs.modal').on('shown.bs.modal', function () {
                    if (typeof vOption != 'undefined'){
                        if (vOption.Shown != undefined){
                            vOption.Shown();
                        }
                    }
                });
            }
        }

        function ClosePopUp(vOption) {
            if (vOption != undefined) {
                if (vOption.Close != undefined || vOption.OnClose != undefined) {
                    var closeFunction = (vOption.Close || vOption.OnClose);
                    closeFunction();
                }
            }
        }
    })
    .on('show.bs.modal', '.modal', function (event) {
        var zIndex = 1040 + (10 * $('.modal.show').length);
        $(this).css('z-index', zIndex);
        setTimeout(function() {
            $('.modal-backdrop').not('.modal-stack').css('z-index', zIndex - 1).addClass('modal-stack');
        }, 0);
    })
    .on('hidden.bs.modal', '.modal', function (event) {
        if ($('.modal.show').length > 0){
            $('body').addClass('modal-open');
        }
    });
})