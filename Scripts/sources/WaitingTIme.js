$(function () {
    getOriginalSKUInformation();
    createDate();

    $("#btnWait").click(function () {
        SaveBackOrderSKUAction();
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

function SaveBackOrderSKUAction() {
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
        if (ctr < 3) {
            strHTMLYear += "<option value='" + (mYear + ctr) + "'> " + (mYear + ctr) + " </option> ";
        }

        strHTMLDay += "<option value='" + (ctr + 1) + "'>" + (ctr + 1) + "</option> ";
        ctr += 1;
    }

    strHTMLYear += "</select>";
    strHTMLDay += "</select>";

    $("#Sday").html(strHTMLDay);
    $("#Syear").html(strHTMLYear);
}