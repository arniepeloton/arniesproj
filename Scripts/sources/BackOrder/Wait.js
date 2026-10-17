$(function () {
    getOriginalSKUInformation();
//    createDate();

    $("#btnWait").click(function () {
        window.open("../WaitingTime/", "_self");
        //SaveBackOrderSKUAction();
    });

    $("#btnCancel").click(function () {
        window.open("../Cancel/", "_self");
    });

    $("#optWaitingOptions").click(function () {
        if ($("#optWaitingOptions").val() == 1) {

        }
        else if ($("#optWaitingOptions").val() == 2) {

        }
        else if ($("#optWaitingOptions").val() == 3) {

        }
        else if ($("#optWaitingOptions").val() == 4) {

        }
        else if ($("#optWaitingOptions").val() == 5) {

        }
        else if ($("#optWaitingOptions").val() == 6) {

        }
    });
})

function getOriginalSKUInformation() {

    $.ajax({
        url: "../getOriginalSKUInformation/",
        dataType: "json",
        type: "POST",
        err: function (err) {
            alert("Error: " + err.toString());
        },
        beforeSend: function () {
            //            $("#bd-loading").html("<img src=\"/Content/WhiteLabel/light/images/loading.gif\" />");
        },
        success: function (data) {
            var myData = data.d;
            var strHTML = "";

            //            $("#bd-loading").html("");

            $.each(myData, function (key, val) {                
                $("#WaitSKUNo").html(val["StockNumber"]);
                $("#WaitSKUDescription").html(val["StockDescription"]);
                $("#WaitQtyOrdered").html(val["Quantity"]);
                $("#WaitQtyUnitMeasure").html(val["UnitMeasure"] + '(S)');
                $("#WaitPrice").html(formatCurrency(val["UnitPrice"]));
                $("#WaitQtyUnitMeasure2").html('Per ' + val["UnitMeasure"] + '(S)');
                $("#WaitSubTotal").html(formatCurrency(val["SubTotal"]));
                TransSalesBackOrderItemsID = val["TransSalesBackOrderItemsID"]
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
    return (((sign) ? '' : '-') + num + '.' + cents);
}
//  End -->