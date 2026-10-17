require([
    'common'
    ], function () {
        $(function () {

            getCustomer();

            //    $("#btnGo").click(function () {
            //        getCustomer();
            //    });

            $("#btnBack").click(function () {
                window.open("Customers/", "_self");
            });
        })

        function getCustomer() {
            var option = {
                Customer: 1
            };

            $.ajax({
                url: "getCustomers/",
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
                            "<a href='#' style='white-space:normal' class='btnCustomers' ID='" + val["ID"] + "'>" + val["CompanyName"] + " - Team Owner: " + val["TeamName"] +
                            "</a></li>";
                    });

                    $("#lstCustomers").html(strHTML);
                    $("#lstCustomers").listview('refresh');

                    $(".btnCustomers").click(function () {
                        addCustomers($(this).attr("ID"))
                    });
                }
            });
        }

        function addCustomers(ID) {
            var option = {
                CustomerID: ID
            };

            $.ajax({
                url: "CheckIfCustomerExist/",
                dataType: "Text",
                data: JSON.stringify(option),
                type: "POST",
                err: function (err) {
                    alert("Error: " + err.toString());
                },
                beforeSend: function () {
                },
                success: function (data) {
                    if (data == "0") {
                        window.open("Customers/", "_self");
                    }
                    else {
                        window.open("CustomerAlreadyExist/", "_self");
                    }

                }
            });

        }
    });