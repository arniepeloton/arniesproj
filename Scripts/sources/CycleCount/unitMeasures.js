$(function () {

    getUnitMeasures();
})

function getUnitMeasures() {

    $.ajax({
        url: "../getUnitMeasure/",
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
            var ctr = 0;

            $("#bd-loading").html("");

            $.each(myData, function (key, val) {

                if (val["UnitMeasure1"] != "" || val["UnitMeasure1"] != "undefined") {
                    strHTML += "<li><a href='#' class='unitMeasures' id='" + val["UnitMeasureID1"] + "'> " + val["UnitMeasure1"] + "</a></li>";
                    strHTML += "<span class='ui-icon ui-icon-arrow-r ui-icon-shadow'></span>"
                }

                if (val["UnitMeasure2"] != "") {
                    strHTML += "<li><a href='#' class='unitMeasures' id='" + val["UnitMeasureID2"] + "'> " + val["UnitMeasure2"] + "</a></li>";
                    strHTML += "<span class='ui-icon ui-icon-arrow-r ui-icon-shadow'></span>"
                }

                if (val["UnitMeasure3"] != "") {
                    strHTML += "<li><a href='#' class='unitMeasures' id='" + val["UnitMeasureID3"] + "'> " + val["UnitMeasure3"] + "</a></li>";
                    strHTML += "<span class='ui-icon ui-icon-arrow-r ui-icon-shadow'></span>"
                }

                if (val["UnitMeasure4"] != "") {
                    strHTML += "<li><a href='#' class='unitMeasures' id='" + val["UnitMeasureID4"] + "'> " + val["UnitMeasure4"] + "</a></li>";
                    strHTML += "<span class='ui-icon ui-icon-arrow-r ui-icon-shadow'></span>"
                }

                return false;

            });

            $("#lstUnitMeasures").html(strHTML);


            $('#lstUnitMeasures').listview('refresh');

//            $(".unitMeasures").click(function () {
//                window.location = "../sku/";
//            });
        }
    });
}