require([
    'common',
    GetPathFormat('/Scripts3/Browser/LibraryFowst.js?crc=')
    ], function () {

        $("#OrgID").val(sessionStorage.OrgID);

        if (sessionStorage.FowstID == undefined || sessionStorage.FowstID == "" || sessionStorage.FowstID == null) {
            GetFowst();
        }

        function GetFowst() {
            var pData = { FilterFowst: 1 };
            AjaxSend({
                url: 'Browser/LibraryFowst',
                data: pData,
                success: function (vFowstData) {

                    if (vFowstData.Results.Rows.length == 1) {
                        sessionStorage.FowstID = vFowstData.Results.Rows[0].ID;
                    } else {
                        BrowseAddresses();
                    }
                }
            });
        }; // function GetFowst() {

        function BrowseAddresses() {

            Browser.LibraryFowst.InputData.OrganizationID = sessionStorage.OrgID;
            Browser.LibraryFowst.InputData.FilterFowst = 1;
            Browser.LibraryFowst.Click = function (e) {
                sessionStorage.FowstID = e.ID;
            };
            Browser.LibraryFowst.Show();

        }; // function BrowseAddresses() {

        $(function () {

            $(document).ready(function () {
                getPIMTemplateUploaded();

            });

            $('#btnPIMTemplateBackToHome').click(function (e) {

                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu"), "_self");
            });

            $('#lnkUploadTemplate').click(function (e) {

                e.preventDefault();

                //                window.open(GetPathFormat("/PIMTemplate"), "_self");
                window.open(GetPathFormat("/PIMTemplate") + "?OrgID=" + sessionStorage["OrgID"], "_self");
            });

            $('#lnkDownloadReturnTemplate').click(function (e) {
                e.preventDefault();

//                window.location = GetPathFormat("/PIMTemplate/DownloadReturnTemplate");
                window.open(GetPathFormat("/PIMTemplate/DownloadReturnTemplate") + "?OrgID=" + sessionStorage["OrgID"], "_self");
            });
        });

        function getPIMTemplateUploaded() {
            

            AjaxSend({
                url: 'PIMTemplate/IsPIMTemplateUploaded',
                success: function (vData) {
                    if (vData.PIMTemplateUpload == 'TRUE') {
                        MessageBox('PIM template successfully uploaded.', 'PIM Template',
                        [{ Text: 'Ok',
                            Click: function () {
                                window.open(GetPathFormat("/PIMTemplate") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                            }
                        }]); // MessageBox
                        
                    }
                }
            });
        }

    });