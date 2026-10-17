require([
    'common'
    ], function () {
        $(function () {
            getEmployees();

            //    $("#btnGo").click(function () {
            //        getEmployees();
            //    });

            $("#btnBack").click(function () {
                window.open("TeamMembers/", "_self");
            });
        })

        function getEmployees() {
            var option = {
                Employee: 1
            };

            $.ajax({
                url: "getEmployees/",
                dataType: "json",
                data: JSON.stringify(option),
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
                        strHTML += "<li> " +
                            "<a href='#' style='white-space:normal' class='btnEmployee' ID='" + val["ID"] + "'>" + val["Name"] +
                            "</a></li>";
                    });

                    $("#lstEmployees").html(strHTML);
                    $("#lstEmployees").listview('refresh');

                    $(".btnEmployee").click(function () {
                        addEmployee($(this).attr("ID"))
                    });
                }
            });
        }

        function addEmployee(ID) {
            var option = {
                EmployeeID: ID
            };

            $.ajax({
                url: "AddEmployee/",
                dataType: "Text",
                data: JSON.stringify(option),
                type: "POST",
                err: function (err) {
                    alert("Error: " + err.toString());
                },
                beforeSend: function () {
                },
                success: function (data) {
                    window.open("EditTeamMembers?EmployeeID=" + data, "_self");
                }
            });

        }
    });