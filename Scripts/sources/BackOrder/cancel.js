$(function () {
    getOriginalSKUInformation();
    createDate();

    $("#btnWait").click(function () {
        window.open("../WaitingTime/", "_self");        
    });

    $("#btnWait").click(function () {
        SaveBackOrderWaitAction();
    });

    $("#btnCancel").click(function () {
        SaveBackOrderSKUAction();        
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
                $("#SKUNo").html(val["StockNumber"]);
                $("#SKUDescription").html(val["StockDescription"]);
                $("#QtyOrdered").html(val["Quantity"]);
                $("#QtyUnitMeasure").html(val["UnitMeasure"]  + '(S)');
                $("#Price").html(formatCurrency(val["UnitPrice"]));
                $("#QtyUnitMeasure2").html('Per ' + val["UnitMeasure"] + '(S)');
                $("#SubTotal").html(formatCurrency(val["SubTotal"]));
                TransSalesBackOrderItemsID = val["TransSalesBackOrderItemsID"]
            });
        }
    });
}

function SaveBackOrderSKUAction() {
    var option = {
        action: "cancel",
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
            window.open("../BackOrderList/", "_self");
        }
    });
}

function SaveBackOrderWaitAction() {
    var option = {
        action: "wait",
        m_BackorderID: TransSalesBackOrderItemsID,
        value: $("#Smonth").val() + "/" + $("#Sday").val() + "/" + $("#Syear").val()
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
            window.open("../BackOrderList/", "_self");
        }
    });

}

function createDate(mDate) {
    var strHTMLDay = "<option val='0' selected='selected'>Day</option>";
    var strHTMLYear = "<option val='0' selected='selected'>Year</option>";
    var mDate = new Date();
    var mYear = mDate.getFullYear();
    var ctr = 0;

        while (ctr < 31) {
            if (ctr<3) {
                strHTMLYear += "<option value='" + (mYear + ctr) + "'> " + (mYear + ctr) + " </option> ";
            }

            strHTMLDay += "<option value='" + (ctr + 1) + "'>" + (ctr+1) + "</option> ";
            ctr += 1;
        }

        strHTMLYear += "</select>";
        strHTMLDay += "</select>";
        
        $("#Sday").html(strHTMLDay);
        $("#Syear").html(strHTMLYear);
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