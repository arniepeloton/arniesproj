require([
    'common'
    ], function () {

        $(function () {

            var mLoaded = false;

            $.mobile.activePage.bind('pageshow', PageShow);
            function PageShow() {
                mLoaded = true;

                if (!window.IsPageLoaded) {

                    AjaxSend({
                        url: 'PIMMeasureSign/IsNewEntry',
                        success: function (vData) {

                            ShowPIMMeasureSignInfo(vData.PIMMeasureSignID || '')

                        }
                    });

                    window.IsPageLoaded = true;
                }
            };
            if (!mLoaded) PageShow();

            $('#btnPIMMeasureSignInfoDone').click(function (e) {
                e.preventDefault();

                if ($.trim($("#txtPIMMeasureSign").val()) == "") {
                    MessageBox("Please enter a sign.",
                            "Sign",
                            [
                                { Text: 'Ok' }
                            ]
                    );
                    return;
                }

                SavePIMMeasureSignInfo();

            });

            function ShowPIMMeasureSignInfo(strID) {
                var pData = {};
                pData.PIMMeasureSignID = strID;

                ClearPIMMeasureSignFields();

                AjaxSend({
                    url: "PIMMeasureSign/getPIMMeasureSignInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.MeasureSignData.Rows.length > 0) {

                            $("#txtPIMMeasureSignID").val(vData.MeasureSignData.Rows[0].ID || '');

                            $("#txtPIMMeasureSign").val(vData.MeasureSignData.Rows[0].Sign || '');
                        }
                    }
                });

            };

            function ClearPIMMeasureSignFields() {

                $("#txtPIMMeasureSignID").val('');

                $("#txtPIMMeasureSign").val('');
            }

            function SavePIMMeasureSignInfo() {
                var pData = [];

                pData = {
                    PIMMeasureSignID: $.trim($("#txtPIMMeasureSignID").val()),
                    Sign: $.trim($("#txtPIMMeasureSign").val())
                };

                AjaxSend({
                    url: "PIMMeasureSign/CheckUniquePIMMeasureSign",
                    data: pData,
                    success: function (vData) {

                        if (vData.MeasureSignData.Rows.length > 0) {
                            MessageBox("Sign already exist.",
                            "Sign",
                            [
                                { Text: 'Ok' }
                            ]
                        );

                        }
                        else {
                            AjaxSend({
                                url: "PIMMeasureSign/SavePIMMeasureSignInfo",
                                data: pData,
                                success: function (vSaveData) {

                                    $("#txtPIMMeasureSignID").val(vSaveData.MeasureSignData || '');

                                    $.mobile.changePage("#" + $(window.pageOrig).attr('id'));

                                }
                            });

                        }

                    }
                });

            };


        });
    });