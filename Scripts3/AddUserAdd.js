require(['common',
    GetPathFormat('/Scripts3/Browser/EmployeeProfiles.js?crc='),
    GetPathFormat('/Scripts3/Browser/MainSecurityLevel.js?crc=')
    ], function() {

    $(function () {

        var mEmployeeID;
        var mSecurityLevelID;

        $('#butEmployee').click(function() {
            Browser.EmployeeProfiles.Click = function(e) {
                mEmployeeID = e.ID;
                $('#spanEmployeeName').text(e.FullName);
            };
            Browser.EmployeeProfiles.Show();
        }); // butEmployee

        $('#butSecurityLevel').click(function() {
            Browser.MainSecurityLevel.Click = function(e) {
                mSecurityLevelID = e.ID;
                $('#spanSecurityLevel').text(e.LevelType);
            };
            Browser.MainSecurityLevel.Show();
        }); // butSecurityLevel

        $('#butOk').click(function() {

            if (mEmployeeID == undefined) {
                MessageBox('Please select an employee first.');
                return;
            };

            if (mSecurityLevelID == undefined) {
                MessageBox('Please select a security level first.');
                return;
            };

            // if (!IsEmail($('#txtEmail').val())) {
            if (CStr2($('#txtEmail').val()).trim().length == 0) {
                MessageBox('Please enter a valid email address');
                return;
            };

            AjaxSend({
                url: 'AddUser/SetupUser',
                data: {
                    EmployeeID: mEmployeeID,
                    SecurityLevelID: mSecurityLevelID,
                    Email: $('#txtEmail').val()
                },
                success: function (vData) {
                    if (vData.Result == 'ok') {
                        MessageBox(vData.Message);
                    } else if (vData.Error) {
                        MessageBox(vData.Error);
                    };
                },
                error: function (vError) {
                    alert(vError);                    
                }
            });
        }); // butOk

    });
});