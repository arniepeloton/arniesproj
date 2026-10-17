require([
    'common'
    ], function () {

        var IsNew = 1;
        var IsConfirm = 0;
        var OrgName = '';
        var ID = '';

        $(function () {

            var mLoaded = false;

            $.mobile.activePage.bind('pageshow', PageShow);

            function PageShow() {
                mLoaded = true;
            };
            if (!mLoaded) PageShow();

            $('body').on('click', '#btnCreateStoreCategoryTemplate', function (e) {
                e.preventDefault();

                if ($.trim($("#txtStoreCategoryTemplateName").val()) == "") {
                    NewMessageBox("Please enter a template name.", "Template Name");
                    return;
                }

                SaveTemplateName();
            })
            .on('click', '#btnCancelStoreCategoryTemplate', function (e) {
                window.location.hash = 'LibraryPage';
            })

            function SaveTemplateName() {
                var data = {
                    Name: $.trim($("#txtStoreCategoryTemplateName").val())
                };
                AjaxSend({
                    url: 'OnlineStoreCategories/CreateTemplate',
                    data: data,
                    success: function (vData) {
                        if (vData.Result == 'ok') {

                            //location = MergeUrlParameters({
                            //    OrgID: sessionStorage.OrgID,
                            //    IsMobile: CBool(IsMobile) ? 1 : 0
                            //}, '/ProductGroup');
                            $('#tblBody').html('');
                            window.location.hash = '';
                            window.location.hash = 'LibraryPage';
                            LibraryTable.OnlineStoreCategories();

                        }
                        else {
                            NewMessageBox(vData.Error, 'Template Name');
                        }
                    }
                });
            }

        });
    });