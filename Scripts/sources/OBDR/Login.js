$(function () {
    $("#btnLogin").click(function () {
        //alert("Click Button Test");
        $(this).UserLogin({
            "Username": $("#txtUsername").attr("value"),
            "Password": $("#txtPassword").attr("value")
        });
    });

    $("#txtUsername, #txtPassword").keypress(function (e) {
        if (e.keyCode == 13) {
            $(this).UserLogin({
                "Username":$("#txtUsername").attr("value"),
                "Password":$("#txtPassword").attr("value")
            });
        }
    });
});