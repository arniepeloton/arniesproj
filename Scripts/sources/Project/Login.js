$(function () {

    var formLogin = $('#formLogin');
    var txtUsername = formLogin.find('[name=txtUsername]')
        .focus();
    var txtPassword = formLogin.find('[name=txtPassword]');
    var butLogIn = formLogin.find('[name=butLogIn]')
        .click(butLogIn_Click);
    var divResult = formLogin.find('[name=divResult]');
    var divBlock = formLogin.find('[name=divBlock]');

    //    (function () { // Init 

    //    })();

    function butLogIn_Click(e) {
        e.preventDefault();
        if (txtUsername.val().trim().length == 0) {
            txtUsername.focus();
            divResult
                    .text('Please enter your username')
                    .MakeError();
            return;
        };
        if (txtPassword.val().trim().length == 0) {
            divResult
                    .text('Please enter your password')
                    .MakeError();
            txtPassword.focus();
            return;
        };
        divBlock.block();
        AjaxSend({
            url: '/login/verify',
            data: formLogin.serializeObject(),
            success: function (vData) {
                divBlock.unblock();
                if (vData.d.Text) {
                    if (vData.d.Text == 'Ok') {
                        txtUsername.hide();
                        txtPassword.hide();
                        butLogIn.hide();
                        divResult.html('Welcome.').MakeHighlight();
                        if (window.location.pathname == '/'
                            || window.location.pathname == '/login'
                            || window.location.pathname == '/logout')

                            window.location = '/welcome';

                        else
                            window.location.reload();
                    } else
                        divResult.text(vData.d.Text).MakeHighlight();
                } else if (vData.d.Error) {
                    divResult.text(vData.d.Error).MakeError();
                } else if (vData.d.BankUserAccess) {
                    window.location = '/OBDR/';
                };
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                divBlock.unblock();
                divResult.text(XMLHttpRequest.responseText).MakeError();
            }
        });
    };

});