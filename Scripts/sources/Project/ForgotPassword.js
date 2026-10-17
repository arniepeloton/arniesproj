$(function () {

    var formCaptcha = $('#formCaptcha');
    var txtCaptcha = $('[name=txtCaptcha]')
        .width(190);
    var butSubmit = $('#butSubmit')
        .click(butSubmit_Click);
    var txtEmail = $('[name=txtEmail]')
        .width(190);
    var divCaptcha = $('#divCaptcha');
    var divResult = $('#divResult');
    var imgCaptcha = formCaptcha.find('[name=imgCaptcha]')
        .hide()
        .load(imgCaptcha_Load);
    var spanLoadingText = $('<div>')
        .text('Loading image. Please wait.')
        .insertBefore(imgCaptcha);

    $('#divCaptchaInfo').MakeHighlight();

    function butSubmit_Click(e) {

        var pData;

        e.preventDefault();
        if (IsEmail(txtEmail.val()) == false) {
            divResult
                .text('Please enter a valid email')
                .MakeError();
            return;
        };
        divCaptcha.block();
        pData = {};
        pData.CaptchaKey = mCaptchaKey;
        pData.Email = txtEmail.val();
        pData.CaptchaText = txtCaptcha.val();
        AjaxSend({
            url: '/ForgotPassword/Verify',
            data: pData,
            success: function (vData) {
                if (vData.d) {
                    if (vData.d.Text) {
                        divResult
                            .text(vData.d.Text)
                            .MakeHighlight();
                    } else if (vData.d.Error) {
                        divResult
                            .text(vData.d.Error)
                            .MakeError();
                        GetAnotherImage();
                    };
                } else {
                    // error
                };
                divCaptcha.unblock();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                divCaptcha.unblock();
                divResult
                    .text('Error: ' + XMLHttpRequest.responseText)
                    .MakeError();
            }
        }); // END AjaxSend
    }; // END butSubmit_Click

    function GetAnotherImage() {

        imgCaptcha.fadeOut();
        AjaxSend({
            url: '/captcha/Create',
            success: function (vData) {
                spanLoadingText.show();
                mCaptchaKey = vData.d.Key;
                imgCaptcha.hide();
                imgCaptcha.attr('src', '/captcha/' + mCaptchaKey + 'jpg');
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                divCaptcha.unblock();
                divResult
                    .text('Error: ' + XMLHttpRequest.responseText)
                    .MakeError();
            }
        });
    }; // END GetAnotherImage

    function imgCaptcha_Load() {

        spanLoadingText.fadeOut(null, function () {
            imgCaptcha.fadeIn();
        });

    };
});