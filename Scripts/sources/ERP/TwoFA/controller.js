define([]
    , function () {
        var mDeferred = $.Deferred();
        var mHtml;

        (function staticInit() {

            AjaxSend({
                ExactUrl: '/Scripts/sources/ERP/TwoFA/view.html?v=' + assetversion,
                type: 'GET',
                dataType: 'text',
                success: function (vData) {
                    mHtml = vData;
                    mDeferred.resolve();
                }
            });
        })();

        CreateStringObject('ERP.TwoFA.main.controller', null, function () {

            var mOut = {}, data;
            var mElement, isAddOnly = false;
            var invalidMsg = '<div class="alert alert-danger d-flex align-items-center mb-3" role="alert"><span class="bi-exclamation-circle-fill mr-3" style="font-size:20px;"></span><div>Code entered is expired or no longer valid.</div></div>';

            mElement = $(mHtml);
            mOut.element = mElement;            

            $('body').append(mElement);

            var pop2FAEmailAdd = $('#pop2FAEmailAdd');

            mOut.vParams = null;

            mOut.Initialize = function (vParams) {
                mOut.vParams = vParams;

                Bs.Loading('show', 'Sending a verification code to your email address. Please wait a moment ...');
                AjaxSend({
                    url: '/Signin/SendTwoFA',
                    data: { OrgID: sessionStorage.OrgID, PeopleID: sessionStorage.PeopleID },
                    success: function (e) {
                        Bs.Loading('hide');
                        if (CStr2(e.Error) != '') {
                            NewMessageBox('There\'s was an error while sending a verification code. Please contact Mobiz representative for assistance.');
                            return;
                        }

                        if (CBool(e.NoEmailAdd)) {
                            NewMessageBox('You don\'t have an email address encoded in the system, therefore we can\'t send you a verification code. Ask your system administrator for help. Thank you!');
                            return;
                        }

                        $('#lblUserEmail').text((CStr2(e.EmailAddress) == '' ? '' : ' (' + e.EmailAddress + ')'));
                        pop2FAEmailAdd.find('input').val('');
                        pop2FAEmailAdd.find('.alert').remove();
                        pop2FAEmailAdd.modal('show');

                        CountdownResend();

                        $('#form2FA').unbind('submit').submit(function (e) {
                            e.preventDefault();
                            e.stopPropagation();

                            pop2FAEmailAdd.find('.alert').remove();

                            var txt2FACode = $('#txt2FACode').removeClass('is-invalid');
                            var txt2FACodeval = $.trim($('#txt2FACode').val());

                            if (txt2FACodeval == '') {
                                txt2FACode.addClass('is-invalid');
                                return;
                            }

                            Bs.Loading('show', 'Verifying code. Please wait ...');

                            AjaxSend({
                                url: '/SignIn/VerifyTwoFACode',
                                data: {
                                    PeopleID: sessionStorage.PeopleID,
                                    Code: txt2FACodeval
                                }, success: function (e) {
                                    Bs.Loading('hide');

                                    if (CBool(e.Valid)) {
                                        vParams.Callback()
                                    } else if (CBool(e.Invalid)) {
                                        $(invalidMsg).insertBefore('#pop2FAEmailAdd form');
                                    } else {
                                        NewMessageBox('There\'s was an error while verifying the code. Ask Mobiz\'s representative for assistance.')
                                    }
                                }
                            })
                        })
                    }
                })                
            }

            $('#txt2FACode').keydown(function (e) {
                if (!NumberOnly(e)) {
                    return;
                }
            })

            function CountdownResend() {
                var lblResending = $('#lblResending'), timeleft = 300;

                $('#linkResend2FACode').addClass('text-secondary').removeClass('text-primary').unbind('click');
                lblResending.text('Resend code in ' + timeleft + ' seconds');

                var clearInt = setInterval(function () {
                    if (timeleft <= 0) {
                        clearInterval(clearInt);
                        lblResending.text('');

                        $('#linkResend2FACode').removeClass('text-secondary').addClass('text-primary').click(function () {
                            Bs.Loading('show', 'Sending a verification code to your email address. Please wait a moment ...')
                            AjaxSend({
                                url: '/Signin/CheckTwoFA',
                                data: { OrgID: sessionStorage.OrgID, PeopleID: sessionStorage.PeopleID },
                                success: function (e) {
                                    if (CBool(e.TwoFA)) {
                                        mOut.Initialize(mOut.vParams);
                                    }

                                    if (CStr2(e.Error) != '') {
                                        NewMessageBox('There\'s was an error while sending a verification code. Please contact Mobiz representative for assistance.');
                                        return;
                                    }

                                    if (CBool(e.NoEmailAdd)) {
                                        NewMessageBox('You don\'t have an email address encoded in the system, therefore we can\'t send you a verification code. Ask your system administrator for help. Thank you!');
                                        return;
                                    }
                                }
                            })
                        })
                    } else {
                        timeleft -= 1;
                        lblResending.text('Resend code in ' + timeleft + ' seconds');
                    }
                }, 1000)
            }

            return mOut;

        }); // END CreateStringObject

        return mDeferred.promise();
    })