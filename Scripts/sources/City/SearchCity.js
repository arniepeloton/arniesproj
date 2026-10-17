
function SearchCity() {

    $.ajax({
        url: "getCityList/",
        dataType: "json",
        type: "POST",
        data: { 'Keywords': $("#txtSearch").val() },
        success: function (data) {

            var strHTML = "";
            for (var i = 0; i < data.length; i++) {
                strHTML += "<li><a ID='" + data[i]["ID"] + "'>" + data[i]["City"] + "</a></li>";
            }

            $("#lstResultList").html(strHTML);
            $("#lstResultList").listview('refresh');

            $("#divRecordCount").text("No. of records: " + data.length);

        }
    });

}

