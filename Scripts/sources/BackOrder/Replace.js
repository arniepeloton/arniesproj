$(function () {
    getOriginalSKUInformation();
    getSuggestedSKUInformation();

    $("#btnReplace").click(function () {
        SaveBackOrderSKUAction();
        window.open("../BackOrderList/", "_self");
    });

    $("#btnCancel").click(function () {
        window.open("../Cancel/", "_self");
    });

    $("#btnWait").click(function () {
        window.open("../WaitingTime/", "_self");
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
        },
        success: function (data) {
            var myData = data.d;
            var strHTML = "";
            
            $.each(myData, function (key, val) {
                $("#OriginalSKUNo").html(val["StockNumber"]);
                $("#OriginalSKUDescription").html(val["StockDescription"]);
                $("#OriginalQtyOrdered").html(val["Quantity"]);
                $("#OriginalQtyUnitMeasure").html(val["UnitMeasure"] + '(S)');
                $("#OriginalPrice").html(formatCurrency(val["UnitPrice"]));
                $("#OriginalQtyUnitMeasure2").html('Per ' + val["UnitMeasure"] + '(S)');
                $("#OriginalSubTotal").html(formatCurrency(val["SubTotal"]));
                TransSalesBackOrderItemsID = val["TransSalesBackOrderItemsID"];
            });
        }
    });
}

function getSuggestedSKUInformation() {
   
    $.ajax({
        url: "../getSuggestedSKUInformation/",
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
                $("#ReplaceSKUNo").html(val["StockNumber"]);
                $("#ReplaceSKUDescription").html(val["StockDescription"]);
                $("#ReplaceQtyOrdered").html(val["Quantity"]);
                $("#ReplaceQtyUnitMeasure").html(val["UnitMeasure"] + '(s)');
                $("#ReplacePrice").html(formatCurrency(val["UnitPrice"]));
                $("#ReplaceQtyUnitMeasure2").html(val["UnitMeasure"] + '(s)');
                $("#ReplaceSubTotal").html(formatCurrency(val["SubTotal"]));                
            });

        }
    });

}


function SaveBackOrderSKUAction() {
    var option = {
        action: "replace",
        m_BackorderID: TransSalesBackOrderItemsID,
    };

    $.ajax({
        url: "../SaveBackOrderSKUAction/",
        dataType: "json",
        data: JSON.stringify(option),
        type: "POST",
        err: function (err) {
            alert("Error: " + err.toString());
        },
        beforeSend: function () {
            //            $("#bd-loading").html("<img src=\"/Content/WhiteLabel/light/images/loading.gif\" />");
        },
        success: function (data) {

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