require(['common',
    GetPathFormat('/Scripts3/Browser/LibraryFowst.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryOrganization.js?crc=')
    ], function () {

    $(function () {

        var mOrganizationID;
        var mFowstID;

        (function() {

            var pParam = GetUrlParameters();
            if (CStr2(pParam.LoginHash).length != 50)
                MessageBox('Invalid login hash. Please use Mobiz Desktop to access this page', '', null, {
                    OnClose: function() {
                        location = GetPathFormat('/People/PeopleMenu');
                    }
                });
            
        })();

        $('#butOrganization').click(function () {
            Browser.LibraryOrganization.AutoSearch = true;
            Browser.LibraryOrganization.InputData.FilterPeople = 1;
            Browser.LibraryOrganization.Click = function (e) {
                mOrganizationID = e.ID;
                $('#divOrganization').text(e.CompanyName);
                if (sessionStorage.OrgID != mOrganizationID) {
                    sessionStorage.OrgID = mOrganizationID;
                    mFowstID = null;
                    sessionStorage.removeItem('FowstID');
                    $('#divFowst').text('');
                };

            };
            Browser.LibraryOrganization.Show();
        }); // $('#butOrganization').click(function () {

        $('#butFowst').click(function () {
            if (mOrganizationID == undefined) {
                MessageBox('Please select an organization first.', 'Organization');
                return;
            };

            Browser.LibraryFowst.InputData.FilterFowst = 1;
            Browser.LibraryFowst.InputData.OrganizationID = mOrganizationID;
            Browser.LibraryFowst.Click = function (e) {
                mFowstID = e.ID;
                sessionStorage.FowstID = mFowstID;
                $('#divFowst').text(e.FowstName);
            };
            Browser.LibraryFowst.Show();
        }); // $('#butOrganization').click(function () {

        $('#butOk').click(function() {

            if (mOrganizationID == undefined) {
                MessageBox('Please select an organization first.', 'Organization');
                return;
            };

            if (mFowstID == undefined) {
                MessageBox('Please select a FOWST first.', 'FOWST');
                return;
            };

            var pParam = GetUrlParameters();
            AjaxSend({
                url: 'MallLessor/SetMall',
                data: {
                    LoginHash: pParam.LoginHash,
                    Mall: $('#cbMall > option:selected').val()
                },
                success: function(vData) {
                    if (vData.Error) {
                        MessageBox(vData.Error);
                        return;
                    } else if (vData.Result) {
                        MessageBox('Setup was successful.', 'Success', [{
                            Text: 'Continue to  People Menu'
                        }], {
                            OnClose: function() {
                                window.location = GetPathFormat('/People/PeopleMenu');
                            }
                        });
                    };
                }
            }); // AjaxSend({

        }); // $('#butOk').click(function() {

    }); // $(function() {
});
