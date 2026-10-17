require([
    'common'
    ], function () {
        $(function () {
            getCustomers();

            $("#txtCustomer").keydown(function (e) {
                if (e.keyCode == 13) {
                    window.open("../ListOfCustomers?CustomerName=" + $("#txtCustomer").val(), "_self");
                }
            });

            $("#btnGo").click(function () {
                window.open("../ListOfCustomers?CustomerName=" + $("#txtCustomer").val(), "_self");
            });

            $("#btnNext").click(function () {
                window.open("../TeamLeadersCut/", "_self");
                //        window.open("../Done/", "_self");
            });
        })

        function getCustomers() {
            $.ajax({
                url: "../getTeamCustomers/",
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
                    var ctr = 0;

                    $.each(myData, function (key, val) {
                        strHTML += "<li> " +
                            "<a href='#' style='white-space:normal' class='btnCustomers' ID='" + val["ID"] + "'>" + val["CompanyName"] +
                            "</a></li>";
                    });

                    $("#lstCustomers").html(strHTML);
                    $("#lstCustomers").listview('refresh');

                    $(".btnCustomers").click(function () {
                        window.open("../RemoveCustomer?CustomerID=" + $(this).attr("ID"), "_self");
                    });
                }
            });
        }
    });