$(function () {

    getAllDefaults();

})

function getAllDefaults() {

    $.ajax({

        url: "../getSystemDefaults/",
        datatype: "json",
        type: "post",
        err: function (err) {
            alert(err.toString);
        },

        beforeSend: function () {
            $("#bd-loading").html("<img src=\"/Content/WhiteLabel/light/images/loading.gif\" />");
        },

        success: function (data) {
            $("#bd-loading").html("");

            var myData = data.d;

            var strHTML = "<table> <tr><td> Description </td> <td> Value </td> <td> Unit </td> <td> Tip </td> <tr>";

            $.each(myData, function (key, val) {
                strHTML += "<tr> <td>" + val["Description"] + "</td> <td>" + val["Value"] + "</td> <td>" + val["Unit"] + "</td> <td>" + val["Tip"] + "</td></tr>";
            });

            strHTML += "</table>";

            $("#tblSystemDefaults").html(strHTML);
        }

    })
}
