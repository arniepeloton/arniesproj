require([
    'common'
    ], function () {
        $(function () {

            $("#New").click(function () {
                IsNew(1);
            })

            $("#Edit").click(function () {
                IsNew(0);
            })
        });

        function IsNew(mNew) {
            var option = {
                IsNew: mNew
            };

            AjaxSend({
                url: 'crmsetup/AssignIsNew/',
                data: option,
                success: function (vData) {
                    if (mNew == 1) {
                        window.open("../Teams/", "_self");
                    }
                    else {
                        window.open("../ListOfTeams?TeamName=", "_self");
                    }
                },
                error: function (vError) {
                    var pDummy;
                }
            });
        }

    });