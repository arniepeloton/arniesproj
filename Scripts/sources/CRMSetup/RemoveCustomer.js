require([
    'common'
    ], function () {
        $(function () {
            //    getEmployeeDetails();
            //    getTeamInfo();

            $("#btnRemove").click(function () {
                RemoveSelectedCustomer();
            });

            $("#btnCancel").click(function () {
                window.open("TeamMembers/", "_self");
            });
        })


        function RemoveSelectedCustomer() {
            $.ajax({
                url: "RemoveSelectedCustomer/",
                dataType: "json",
                type: "POST",
                err: function (err) {
                    alert("Error: " + err.toString());
                },
                beforeSend: function () {
                },
                success: function (data) {
                    window.open("Customers/", "_self");
                }
            });
        }

        function getEmployeeDetails() {
            var option = {
                Employee: $("#txtEmployee").val()
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
                        $("#lblName").html('<b>' + val["Name"] + '</b>');
                    });
                }
            });
        }


        function getTeamInfo() {
            $.ajax({
                url: "getTeamInfo/",
                dataType: "Text",
                type: "POST",
                err: function (err) {
                    alert("Error: " + err.toString());
                },
                beforeSend: function () {
                },
                success: function (data) {
                    $("#lblBranch").html('<b>' + data + '</b>');
                }
            });
        }
    });