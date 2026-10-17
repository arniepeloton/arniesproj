$(function () {
    $("#navBranches").css({ "visibility": "visible" });
    $("#navWarehouse").css({ "visibility": "visible" });
    
    getWarehouse();
})

function getWarehouse() {
    $.ajax({
        url: "../getAllWarehouses/",
        dataType: "json",
        type: "POST",
        err: function (err) {
            alert("Error: " + err.toString());
        },
        beforeSend: function () {
            $("#bd-loading").html("<img src=\"/Content/WhiteLabel/light/images/loading.gif\" />");
        },
        success: function (data) {
            var myData = data.d;
            var strHTML = "";

            $("#bd-loading").html("");

            $.each(myData, function (key, val) {
                strHTML += "<li><a href='#' class='warehouse' ID='" + val["ID"] + "'> " + val["WarehouseName"] + " </a></li>";
                strHTML += "<span class='ui-icon ui-icon-arrow-r ui-icon-shadow'></span>"
            });

            $("#lstWarehouse").html(strHTML);
            $('#lstWarehouse').listview('refresh');

            $(".warehouse").click(function () {
                var options = {
                    ID: $(this).attr("ID")
                };

                $.ajax({
                    url: "../setWarehouseID/",
                    data: JSON.stringify(options),
                    dataType: "Text",
                    type: "POST",
                    err: function (err) {
                        alert("Error: " + err.toString());
                    },
                    success: function (data) {
                        window.location = "../sku/";
                    }
                });

            });
        }
    });
}