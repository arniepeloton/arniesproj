require([
    'common',
    '/Scripts3/Browser/LibraryOrganization.js'
    ], function() {

    $(function () {

        $('#btnNewOrganizationInfo').click(function (e) {
            e.preventDefault();

            CreateNewEntry(1, '');

        });

        $('#frmOrganizationSearch').submit(function (e) {
            e.preventDefault();

            Browser.LibraryOrganization.Click = function (e) {

                CreateNewEntry(0, e.ID);

            };
            Browser.LibraryOrganization.Show($('#txtSearchOrganization').val()); 

            return false;
        });

        $('#btnSearchOrganization').click(function () {

            Browser.LibraryOrganization.Click = function (e) {

                CreateNewEntry(0, e.ID);

            };
            Browser.LibraryOrganization.Show($('#txtSearchOrganization').val());

        });

        function CreateNewEntry(IsNew, strID) {
            var pData = {};
            pData.IsNew = IsNew;
            pData.ID = strID;

            AjaxSend({
                url: 'Organization/setCreateNew',
                data: pData,
                success: function (vData) {

                    if ((vData.IsNew || '') == "TRUE") {
                        window.open("Organization/OrganizationInfo", "_self");
                    }
                    else {
                        window.open("Organization/OrganizationInfoSummary", "_self");
                    }
                }
            });

        }

    });

});
