require([
    'common'
    ], function () {
        $(function () {
            getCollectionInfo();

            $("#btnNext").click(function () {
                saveCollectionInfo();
            });

            $("#txtCollectionTo2").focusout(function () {
                $("#txtCollectionFrom3").val(parseInt($("#txtCollectionTo2").val()) + 1);
            });

            $("#txtCollectionTo3").focusout(function () {
                $("#txtCollectionFrom4").val(parseInt($("#txtCollectionTo3").val()));
            });
        })

        function saveCollectionInfo() {
            var option = {
                AgingOfAccountsFrom: $("#txtCollectionFrom2").val() + ',' + $("#txtCollectionFrom2").val() + ',' + $("#txtCollectionFrom3").val() + ',' + $("#txtCollectionFrom4").val(),
                AgingOfAccountsTo: $("#txtCollectionTo2").val() + ',' + $("#txtCollectionTo3").val(),
                DecreaseIncentive: $("#txtCollectionDecrease2").val() + ',' + $("#txtCollectionDecrease2").val() + ',' + $("#txtCollectionDecrease3").val() + ',' + $("#txtCollectionDecrease4").val()
            };

            $.ajax({
                url: "../saveCollectionInfo/",
                dataType: "json",
                data: JSON.stringify(option),
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

        function getCollectionInfo() {
            $.ajax({
                url: "../getCollectionInfo/",
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

                    $.each(myData, function (key, value) {
                        if (ctr == 0) {
                            $("#txtCollectionFrom1").val(value["From"]);
                            $("#txtCollectionDecrease1").val(value["Incentive"]);
                        }
                        else if (ctr == 1) {
                            $("#txtCollectionFrom2").val(value["From"]);
                            $("#txtCollectionTo2").val(value["To"]);
                            $("#txtCollectionDecrease2").val(value["Incentive"]);
                        }
                        else if (ctr == 2) {
                            $("#txtCollectionFrom3").val(value["From"]);
                            $("#txtCollectionTo3").val(value["To"]);
                            $("#txtCollectionDecrease3").val(value["Incentive"]);
                        }
                        else if (ctr == 3) {
                            $("#txtCollectionFrom4").val(value["From"]);
                            $("#txtCollectionDecrease4").val(value["Incentive"]);
                        }
                        ctr += 1;
                    });
                }
            });
        }
    });