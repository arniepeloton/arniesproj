require([
    'common'
    ], function () {
        $(function () {
            getCustomerInfo();
            getTeamName();

            $("#btnAdd").click(function () {
                addCustomers();
            });

            $("#btnCancel").click(function () {
                window.open("../Customers/", "_self");
            });

            $("#btnOK").click(function () {
                window.open("../Customers/", "_self");
            });
        })


        function addCustomers(ID) {

            $.ajax({
                url: "../AddCustomers/",
                dataType: "Text",
                type: "POST",
                err: function (err) {
                    alert("Error: " + err.toString());
                },
                beforeSend: function () {
                },
                success: function (data) {
                    window.open("../Customers/", "_self");
                }
            });
        }

        function getCustomerInfo() {

            $.ajax({
                url: "../GetCustomerInfo/",
                dataType: "json",
                type: "POST",
                err: function (err) {
                    alert("Error: " + err.toString());
                },
                beforeSend: function () {
                },
                success: function (data) {
                    var myData = data.d;
                    var strHTML = "<br />"
                    var ctr = 1;

                    $.each(myData, function (key, val) {
                        $("#lblCustomer1").html('<b>' + val["CompanyName"] + '</b>');
                        $("#lblCustomer2").html('<b>' + val["CompanyName"] + '</b>');
                        $("#lblTeam1").html('<b>' + val["TeamName"] + '\'s</b>');
                        $("#lblTeam2").html('<b>' + val["TeamName"] + '\'s</b>');
                        $("#lblEmail").html('<b>' + val["EmailAddress"] + '</b>')

                        strHTML += ctr + ". " + val["Name"] + "<br />"
                        ctr += 1;
                    });

                    $("#ListOfMembers").html(strHTML);

                }
            });
        }

        function getTeamName(ID) {

            $.ajax({
                url: "../getTeamName/",
                dataType: "Text",
                type: "POST",
                err: function (err) {
                    alert("Error: " + err.toString());
                },
                beforeSend: function () {
                },
                success: function (data) {
                    $("#lblTeamName").html("<b>" + data + "</b>");
                }
            });
        }
    });