require([
    'common'
    ], function () {

        $(document).ready(function () {
            //            var glow = $('.sample');
            //            setInterval(function () {
            //                glow.hasClass('glow') ? glow.removeClass('glow') : glow.addClass('glow');
            //            }, 1000);
            ConfirmEmployment();
        })

        function ConfirmEmployment() {
            var pData = {};
            pData.OrgID = GetUrlParameters().key;
            pData.PeopleID = GetUrlParameters().id;

            AjaxSend({
                url: 'ConfirmEmployment/CheckIfAlreadyConfirm',
                data: pData,
                success: function (vData) {
                    if (vData.CheckingResult.Rows[0].IsConfirmed == 0) {
                        pData.ConfirmKeyID = vData.CheckingResult.Rows[0].ID;
                        AjaxSend({
                            url: 'ConfirmEmployment/ConfirmEmployment',
                            data: pData,
                            success: function (msg) {
                                if (msg.Result == "Confirmed") {
                                    $('#msg').text('').html("<strong style='font-size:20px;'>Your confirmation of employment has been successfully confirmed</strong>. You will now redirecting to sign in page ...");
                                    setTimeout(function () {
                                        if (GetUrlParameters().mp == "mc") { //mobiz construct
                                            window.location.href = '/bomprojectexcel';
                                        }
                                        else {
                                            window.location.href = '/SignIn';
                                        }                                        
                                    }, 1000);
                                }
                                else {
                                    $('#msg').text('').html("<strong style='font-size:20px;'>There was an error occured.</strong>.");
                                    console.log(msg.Result);
                                }
                            }
                        });
                    }
                    else if (vData.CheckingResult.Rows.length >= 1) {
                        $('#msg').text('').html("<strong style='font-size:20px;'>You are already confirmed your employment</strong>. You will now redirecting to sign in page ...");
                        setTimeout(function () {
                            window.location.href = '/SignIn'
                        }, 1000);
                    }
                    else {
                        $('#msg').text('').html("<strong style='font-size:20px;'>There was an error occured.</strong>.");
                    }
                }
            })
        }

    })