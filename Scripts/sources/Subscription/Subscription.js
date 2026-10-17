require([
    'common'
    ], function () {

        $(function () {

            $(document).ready(function () {
                setTimeout(FollowUpStatusFromServer, 2000);
            });

            $('#lnkSignin').click(function (e) {
                e.preventDefault();

                window.location = GetPathFormat("/Signin?Ismobile=0");

            });

            function FollowUpStatusFromServer() {
                AjaxSend({
                    url: 'Subscription/GetStatus',
                    success: function (vData) {
                        if (vData.Status == "Success") {
                            $("#divSubscriber").hide();
                            $("#divSubscriberDone").show();
                        } else {
                            $("#divSubscriber").text((vData.Status || '') + "Please wait...");

                            setTimeout(FollowUpStatusFromServer, 2000);
                        }

                    }
                });
            }


        });

    });