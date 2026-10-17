require(['common'
    ], function() {

    $(function () {

        var mLink;

        (function Init() {

            var pParam = GetUrlParameters();

            if (CStr2(pParam.k).length == 30) {
                mLink = pParam.k;
                GetData(mLink);
            } else {
                $('#divInvalid').show();
            } ;

        })();

        function GetData(vKey) {

            AjaxSend({
                url: 'AddUser/GetData',
                data: {k: vKey},
                success: function (vData) {
                    if (vData.Result == 'ok') {
                        DisplayData(vData);
                    } else if (vData.Error) {
                        $('#divInvalid').show();
                        MessageBox(vData.Error);
                    };
                },
                error: function (vError) {
                    alert('Network error');                    
                }
            });

        };

        function DisplayData(vData) {
            
            $('#spanEmployeeName').text(vData.FullName);
            $('#spanSecurityLevel').text(vData.LevelType);
            $('#divValid').show();

        };

        $('#butOk').click(function() {

            var pUsername = $('#txtUsername').val();
            if (CStr2(pUsername).length == 0) {
                MessageBox('Please enter your username');
                return;
            };

            var pPassword = $('#txtPassword').val();
            if (CStr2(pPassword).length == 0) {
                MessageBox('Please enter your password');
                return;
            };

            AjaxSend({
                url: 'AddUser/Apply',
                data: {Link: mLink,
                    Username: pUsername,
                    Password: pPassword
                },
                success: function (vData) {
                    if (vData.Result == 'ok') {
                        MessageBox('Your account was successfully created', 'Success', [
                            {Text: 'Go to login'}
                            ], {
                            OnClose: function() {
                                $('#divValid').hide();
                                window.location = GetPathFormat('/Login3');
                            }
                        });
                    } else if (vData.Error) {
                        MessageBox(vData.Error);
                    };
                },
                error: function (vError) {
                    alert('Network error');                    
                }
            });

        }); // $('#butOk').click(function() {

//        var pComboDate = CreateDateCombo();
//        pComboDate.on('DateChanged', function(e, vDate) {
//            console.log(vDate);
//        });

//        $('#fieldDate').append(pComboDate);
//            //.parent().trigger('create');
//            
//        $('#divDiv').trigger('create');

        // $('#fieldDate').refresh();

    }); 
});