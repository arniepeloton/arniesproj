require([
    'common'
    ], function () {
        $(function () {
            getItemIncentiveDetail();

            $("#btnDone").click(function () {
                saveItemIncentive();
            });
        })

        function saveItemIncentive() {
            option = {
                skuID: $("#lblItem").attr("class"),
                Qty: $("#txtQty").val(),
                UnitMeasure: $("#lblUnitMeasure").attr("class"),
                Incentive: $("#txtIncentive").val()
            };

            $.ajax({
                url: "saveItemIncentive/",
                dataType: "json",
                data: JSON.stringify(option),
                type: "POST",
                err: function (err) {
                    alert("Error: " + err.toString());
                },
                beforeSend: function () {
                },
                success: function (data) {
                    window.open("ItemIncentive/", "_self");
                }
            });
        }

        function getItemIncentiveDetail() {
            $.ajax({
                url: "getItemIncentiveDetail/",
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
                        $("#lblItem").attr("class", val["ProductID"]);
                        $("#lblItem").html("<b>" + val["Stock Description"] + "</b>");
                        $("#txtQty").val(val["Quota"]);
                        $("#lblUnitMeasure").attr("class", val["UnitMeasureID"]);
                        $("#lblUnitMeasure").html(val["UnitMeasure"] + '(S)');
                        $("#txtIncentive").val(val["Incentive"]);
                    });
                }
            });
        }
    });