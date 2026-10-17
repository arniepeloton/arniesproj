$(function () {
    $("#navBranches").css({ "visibility" : "visible" });
    getBranches();
})

function getBranches() {
    $.ajax({
        url: "../getAllBranches/",
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
                strHTML += "<li><a href='#' class='branch' ID='" + val["ID"] + "'> " + val["BranchName"] + " </a></li>";
                strHTML += "<span class='ui-icon ui-icon-arrow-r ui-icon-shadow'></span>"
            });

            $("#lstBranch").html(strHTML);
            $('#lstBranch').listview('refresh');

            $(".branch").click(function () {
                var options = {
                    ID: $(this).attr("ID")
                };

                $.ajax({
                    url: "../setBranchID/",
                    data: JSON.stringify(options),
                    dataType: "Text",
                    type: "POST",
                    err: function (err) {
                        alert("Error: " + err.toString());
                    },
                    success: function (data) {
                        window.location = "../Warehouse/";
                    }
                });

            });
        }
    });
}