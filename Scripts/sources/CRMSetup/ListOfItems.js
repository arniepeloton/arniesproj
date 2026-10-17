require([
    'common'
    ], function () {
        $(function () {
            getItems();

            $("#btnBack").click(function () {
                window.open("ItemIncentive/", "_self");
            })
        })

        function getItems() {

            $.ajax({
                url: "getItems/",
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
                        strHTML += "<li> " +
                            "<a href='#' style='white-space:normal' class='btnItem' ID='" + val["ID"] + "' name='" + val["Stock Description"] + "'>" + val["StockNumber"] + ' - ' + val["Stock Description"] +
                            "</a></li>";
                    });

                    $("#lstItems").html(strHTML);
                    $("#lstItems").listview('refresh');

                    $(".btnItem").click(function () {
                        assignSKUID($(this).attr("ID"), $(this).attr("name"));
                    });
                }
            });
        }


        function assignSKUID(ID) {
            var option = {
                skuID: ID
            };

            $.ajax({
                url: "assignSKUID/",
                dataType: "json",
                data: JSON.stringify(option),
                type: "POST",
                err: function (err) {
                    alert("Error: " + err.toString());
                },
                beforeSend: function () {
                },
                success: function (data) {
                    window.open("EditItem?Item=", "_self")
                }
            });

        }
    });