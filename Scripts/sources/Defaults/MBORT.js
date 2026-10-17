$(function () {
    getMBORT();

    $("#btnSave").click(function () {
        
        var options = {
            MBORTResponseTime: $("#txtMBORT").val(),
            MBORTUnAnsweredBOA: $("#optMBORT").val()
        };

        saveMBORTDefaults(options);
    });
})

function getMBORT() {


    $.ajax({
        url: "../getMBORT/",
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
            $.each(myData, function (key, val) {
                $("#bd-loading").html("");

                if (val["Description"] == "MBORTResponseTime") {
                    $("#txtMBORT").val(val["DefaultValue"]);
                }
                else {
                    if (val["DefaultValue"] == "cancel") {                        
                        $("#optMBORT")[0].selectedIndex = 0;
                    }
                    else {
                        $("#optMBORT")[0].selectedIndex = 1;
                    }
                }
            });            
        }
    });
}


function saveMBORTDefaults(options) {
    $.ajax({
        url: "../saveMBORTDefaults/",
        data: JSON.stringify(options),
        dataType: "json",
        type: "POST",
        err: function (err) {
            alert("Error: " + err.toString());
        },
        success: function (data) {
            getMBORT();
        }
    });
}
