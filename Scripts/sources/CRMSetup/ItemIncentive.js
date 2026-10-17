require([
    'common'
    ], function () {
        $(function () {
            getItemIncentive();

            $("#btnNext").click(function () {
                window.open("../CollectionSetup/", "_self");
            });

            $("#btnAdd").click(function () {
                window.open("../ListOfItems?ItemName=" + $("#txtItem").val(), "_self");
            });

            $("#txtItem").keydown(function (e) {
                if (e.keyCode == 13) {
                    window.open("../ListOfItems?ItemName=" + $("#txtItem").val(), "_self");
                }
            });
        })

        function getItemIncentive() {
            $.ajax({
                url: "../getItemIncentive/",
                dataType: "json",
                type: "POST",
                err: function (err) {
                    alert("Error: " + err.toString());
                },
                beforeSend: function () {
                },
                success: function (data) {
                    var myData = data.d;
                    var strHTML = "";

                    $.each(myData, function (key, val) {
                        strHTML += "<li><a href='#' style='white-space:normal' class='btnItem' ID='" + val["ID"] + "'> " + val["Stock Description"] +
                "<span class='ui-li-count ui-btn-up-c ui-btn-corner-all' style='font-size:small'>";

                        strHTML += "Incentive: " + val["Incentive"] + "%"
                    });

                    $("#lstItems").html(strHTML);
                    $("#lstItems").listview('refresh');

                    $(".btnItem").click(function () {
                        window.open("../EditItem?Item=" + $(this).attr("ID"), "_self")
                    });
                }
            });
        }
    });