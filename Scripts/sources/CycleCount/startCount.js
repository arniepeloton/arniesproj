$(function () {

    $("#navBranches").css({ "visibility": "visible" });
    $("#navWarehouse").css({ "visibility": "visible" });
    $("#navItems").css({ "visibility": "visible" });
    $("#navCountType").css({ "visibility": "visible" });
    $("#navCount").css({ "visibility": "visible" });

    $("#lnkCounting").click();

    $("#txtQuantity").keydown(function () {
        if ($("#txtQuantity").val() == "") {
            $("#txtQuantity").val(formatCurrency($("#txtQuantity").val() + "0"));
        }
        else { 
            formatCurrency($("#txtQuantity").val();
        }
    });

    $("#btn0").click(function (e) {
        if ($("#txtQuantity").val() != "0") {
            $("#txtQuantity").val(formatCurrency($("#txtQuantity").val() + "0"));
        }
    });

    $("#btn1").click(function (e) {
        if ($("#txtQuantity").val() != "0") {
            $("#txtQuantity").val(formatCurrency($("#txtQuantity").val() + "1"));
        }
        else $("#txtQuantity").val("1");
    });

    $("#btn2").click(function (e) {
        if ($("#txtQuantity").val() != "0") {
            $("#txtQuantity").val(formatCurrency($("#txtQuantity").val() + "2"));
        }
        else $("#txtQuantity").val("2");
    });

    $("#btn3").click(function (e) {
        if ($("#txtQuantity").val() != "0") {
            $("#txtQuantity").val(formatCurrency($("#txtQuantity").val() + "3"));
        }
        else $("#txtQuantity").val("3");
    });

    $("#btn4").click(function (e) {
        if ($("#txtQuantity").val() != "0") {
            $("#txtQuantity").val(formatCurrency($("#txtQuantity").val() + "4"));
        }
        else $("#txtQuantity").val("4");
    });

    $("#btn5").click(function (e) {
        if ($("#txtQuantity").val() != "0") {
            $("#txtQuantity").val(formatCurrency($("#txtQuantity").val() + "5"));
        }
        else $("#txtQuantity").val("5");
    });

    $("#btn6").click(function (e) {
        if ($("#txtQuantity").val() != "0") {
            $("#txtQuantity").val(formatCurrency($("#txtQuantity").val() + "6"));
        }
        else $("#txtQuantity").val("6");
    });

    $("#btn7").click(function (e) {
        if ($("#txtQuantity").val() != "0") {
            $("#txtQuantity").val(formatCurrency($("#txtQuantity").val() + "7"));
        }
        else $("#txtQuantity").val("7");
    });

    $("#btn8").click(function (e) {
        if ($("#txtQuantity").val() != "0") {
            $("#txtQuantity").val(formatCurrency($("#txtQuantity").val() + "8"));
        }
        else $("#txtQuantity").val("8");
    });

    $("#btn9").click(function (e) {
        if ($("#txtQuantity").val() != "0") {
            $("#txtQuantity").val(formatCurrency($("#txtQuantity").val() + "9"));
        }
        else $("#txtQuantity").val("9");

    });
    $("#btnBackSpace").click(function (e) {
        if ($("#txtQuantity").val().length == 1) {
            $("#txtQuantity").val("0");
        }
        else if ($("#txtQuantity").val() != "0") {
            $("#txtQuantity").val(formatCurrency($("#txtQuantity").val().substring(0, $("#txtQuantity").val().length - 1)));
        }

    });
    //    $("#btnUnitMeasure3").click(function () {
    //        $("#unitMeasure").css({"display":"block"})
    //    });

    $("#btnUnitMeasure1").click(function () {
        if (this.attr("ID") != "") {
            alert("ID on!");
        }
    });

    //    $("#datepicker").datepicker();
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

                    $("#btnUnitMeasure1").html(val["UnitMeasure1"]);
                    $("#btnUnitMeasure1").attr("ID", val["UnitMeasureID1"]);

                    strHTML += "<li><a href='#main' class='unitMeasures'> " + val["UnitMeasure1"] + "</a></li>";

                    if (val["UnitMeasure2"] != "") {
                        $("#btnUnitMeasure2").html(val["UnitMeasure2"]);
                        strHTML += "<li><a href='#main' class='unitMeasures' id='" + val["UnitMeasureID2"] + "'> " +
                                    val["UnitMeasure2"] + "<br> (1 " + val["UnitMeasure2"] + " = " + val["UnitMeasure2Qty"] + " " + val["UnitMeasure1"] + "'s)</a></li>";
                    }
                    else { $("#btnUnitMeasure2").append("N/A"); $("#btnUnitMeasure3").append("N/A"); return false; }

                    if (val["UnitMeasure3"] != "") {
                        $("#btnUnitMeasure3").append("More Unit Measures");
                        strHTML += "<li><a href='#main' class='unitMeasures' id='" + val["UnitMeasureID3"] + "'> " +
                        val["UnitMeasure3"] + "<br> (1 " + val["UnitMeasure3"] + " = " + val["UnitMeasure3Qty"] + " " + val["UnitMeasure1"] + "'s)</a></li>";
                    }

                    if (val["UnitMeasure4"] != "") {
                        strHTML += "<li><a href='#main' class='unitMeasures' id='" + val["UnitMeasureID4"] + "'> " +
                        val["UnitMeasure4"] + "<br> (1 " + val["UnitMeasure4"] + " = " + val["UnitMeasure4Qty"] + " " + val["UnitMeasure1"] + "'s)</a></li>";
                    }

                    $("#lstUnitMeasures").html(strHTML);


                    return false;
                });
            }
        });
    
    }


    //<!-- Original:  Cyanide_7 (leo7278@hotmail.com) -->
    //<!-- Web Site:  http://www7.ewebcity.com/cyanide7 -->
    //<!-- This script and many more are available free online at -->
    //<!-- The JavaScript Source!! http://javascript.internet.com -->
    //<!-- Begin
    function formatCurrency(num) {
        num = num.toString().replace(/\$|\,/g, '');
        if (isNaN(num))
            num = "0";
        sign = (num == (num = Math.abs(num)));
        num = Math.floor(num * 100 + 0.50000000001);
        cents = num % 100;
        num = Math.floor(num / 100).toString();
        if (cents < 10)
            cents = "0" + cents;
        for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3); i++)
            num = num.substring(0, num.length - (4 * i + 3)) + ',' +
    num.substring(num.length - (4 * i + 3));
        return (((sign) ? '' : '-') + num);
    }
    //  End -->