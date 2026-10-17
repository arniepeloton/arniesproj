define([(OfflineMode ? '/Scripts3/model/PeopleMenuUserDetails_Offline.js?crc=' : '')], function () {
    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {
        Bs.Loading('show');
        AjaxSend({
            ExactUrl: '/Ticket/ReportAnIssue?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                Bs.Loading('hide');
                mHtml = vData;
                ReportAnIssue.controller = new ReportAnIssue.controller();
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ReportAnIssue.controller', null, function () {

        var mOut = {};
        var mElement;

        mElement = $(mHtml);
        mOut.element = mElement;

        var txtRAIDate = mElement.find('#txtRAIDate');
        var selRAIFowst = mElement.find('#selRAIFowst');
        var selRAIMobizProduct = mElement.find('#selRAIMobizProduct');
        var selRAISubject = mElement.find('#selRAISubject');
        var selRAIUrgency = mElement.find('#selRAIUrgency');
        var selRAIImportance = mElement.find('#selRAIImportance');
        var txtRAIDescription = mElement.find('#txtRAIDescription');
        var btnSendRAI = mElement.find('#btnSendRAI');

        $('#pageReportAnIssue > .container-fluid').append(mElement);

        btnSendRAI.click(function () {
            var isValid = true;
            mElement.find('input, textarea, select').removeClass('is-invalid');

            if (selRAIFowst.val() == '') {
                selRAIFowst.addClass('is-invalid');
                isValid = false;
            }

            if (selRAIMobizProduct.val() == '') {
                selRAIMobizProduct.addClass('is-invalid');
                isValid = false;
            }

            if (selRAISubject.val() == '') {
                selRAISubject.addClass('is-invalid');
                isValid = false;
            }

            if (selRAIUrgency.val() == '') {
                selRAIUrgency.addClass('is-invalid');
                isValid = false;
            }

            if (selRAIImportance.val() == '') {
                selRAIImportance.addClass('is-invalid');
                isValid = false;
            }

            if (txtRAIDescription.val().trim() == '') {
                txtRAIDescription.addClass('is-invalid');
                isValid = false;
            }

            if (!isValid) {
                NewMessageBox('Please fill out all fields marked with an <span class="required">*</span>');
                return;
            }

            Bs.Loading('show', 'Sending report... please wait');
            AjaxSend({
                url: '/Ticket/SaveRAI',
                data: {
                    data: {
                        OrgID: sessionStorage.OrgID,
                        FowstID: selRAIFowst.val(),
                        Product: selRAIMobizProduct.val(),
                        Subject: selRAISubject.val(),
                        Urgency: selRAIUrgency.val(),
                        Importance: selRAIImportance.val(),
                        Description: txtRAIDescription.val().trim(),
                        ReportedBy: sessionStorage.PeopleID
                    }
                }, success: function (e) {
                    Bs.Loading('hide');

                    if (e.Results != undefined) {
                        mOut.ClearFields();
                        $('#pagePopUpMenuTroubleShootingTools').pageshow();
                        NewMessageBox('Your report has been submitted to Mobiz. Please keep a record of your reference number for updates and follow-ups. Thank you! <br><br> <div class="text-center" style="font-size: 40px;">' + e.Results + '</div>', 'Success', undefined, { AutoDismiss: false });
                    } else {
                        NewMessageBox(e.Error, 'Error');
                    }
                }
            })
        })

        mOut.ClearFields = function () {
            txtRAIDate.val($.format.date(new Date(), 'MMM dd, yyyy'));
            selRAISubject.val('');
            selRAIUrgency.val('N/A');
            selRAIImportance.val('N/A');
            txtRAIDescription.val('');
        }

        mOut.Initialize = function () {
            var vRows = new App.model.PeopleMenuUserDetails_offline().GetUserDetails(sessionStorage.OrgID)[0].Device.Rows;

            selRAIFowst.append(new Option('Please Select', ''));
            for (var i = 0; i < vRows.length; i++) {
                if (CStr2(vRows[i].Name).trim() != '') selRAIFowst.append(new Option(vRows[i].Name, vRows[i].FowstID));
            }

            if (vRows.length == 1) {
                selRAIFowst.val(vRows[0].FowstID);
            }
        }

        mOut.Initialize();

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();
})