require(['common'], function () {
    $('document').ready(function () {
        $('#formMobizPortal').submit(function (e) {
            e.preventDefault();

            var txtUname = $('#txtUname');
            var txtPword = $('#txtPassword');
            var isValid = true;

            if ($.trim(txtUname.val()) == '') {
                txtUname.addClass('is-invalid');
                isValid = false;
            } else {
                txtUname.removeClass('is-invalid');
            }

            if ($.trim(txtPword.val()) == '') {
                txtPword.addClass('is-invalid');
                isValid = false;
            } else {
                txtPword.removeClass('is-invalid');
            }

            if (isValid) {
                $('#btnLogin').text('Logging in ...').attr('disabled', 'disabled');

                AjaxSend({
                    url: 'MobizPortal/Login',
                    data: {
                        Username: $.trim(txtUname.val()),
                        Password: $.trim(txtPword.val())
                    },
                    success: function (e) {
                        var res = e.Results;

                        $('#btnLogin').text('Login').removeAttr('disabled');

                        if(res == 'invalid'){
                            $('#divAlertMsg').show();
                        } else if (res == 'success') {                            
                            $('#divAlertMsg').hide();
                            location.href = '/MobizPortal/Dashboard';
                        } else {
                            alert('There\'s an error logging in')
                        }
                    }
                })
            }
        })
    })
})