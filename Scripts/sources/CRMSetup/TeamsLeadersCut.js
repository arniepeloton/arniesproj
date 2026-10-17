require([
    'common'
    ], function () {
        $(function () {
            getTeamLeadersCut();

            $("#btnNext").click(function () {
                saveTeamLeadersCut();
            })
        })

        function isNew() {
            $.ajax({
                url: "../isNew/",
                dataType: "Text",
                type: "POST",
                err: function (err) {
                    alert("Error: " + err.toString());
                },
                beforeSend: function () {
                },
                success: function (data) {
                    if (data == 0) {
                        getTeamLeadersCut();
                    }
                }
            });
        }

        function getTeamLeadersCut() {
            $.ajax({
                url: "../getTeamLeadersCut/",
                dataType: "json",
                type: "POST",
                err: function (err) {
                    alert("Error: " + err.toString());
                },
                beforeSend: function () {
                },
                success: function (data) {
                    var myData = data.d;

                    $.each(myData, function (key, val) {
                        $("#txtTeamLeadersCut").val(val["OutrightShare"])
                    });
                }
            });
        }

        function saveTeamLeadersCut() {

            var option = {
                OutrightShare: $("#txtTeamLeadersCut").val()
            };

            $.ajax({
                url: "../saveTeamLeadersCut/",
                dataType: "json",
                data: JSON.stringify(option),
                type: "POST",
                err: function (err) {
                    alert("Error: " + err.toString());
                },
                beforeSend: function () {
                },
                success: function (data) {
                    window.open("../Done/", "_self");
                }
            });
        }
    });