$(function () {
    getReplacementSKU();
    getBackOrderDetails(str);

    $("#btnDone").click(function () {
        createNewOrder();
        window.open("../Finish/", "_self");
    });
})

var str;

function getBackOrderDetails(m_ReplacementItem) {

    $.ajax({
        url: "../getReviewOrderDetails/",
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
            var strHTMLItems = "";
            var strHTMLAction = "";
            var strHTML = "";
           
            $("#bd-loading").html("");

            $.each(myData, function (key, val) {
                var isReplace = "";

                if (val["ActionTaken"] == 'Replaced') {

                    assignBackOrderSKUID(val["StockID"]);
                    
                    strHTML += "<li> " +
                                "<a href='#'> <p class='ui-li-desc' style='white-space:normal; font-size:large'> <b>" + str + " </b> </p> <br/>" +
                                "<p style='white-space:normal; font-size:large'> <b> in " + val["ActionTaken"] + " with " + "</b> </p> <br/>" +
                                "<p style='white-space:normal; font-size:large'> <b> SKU # " + val["StockNumber"] + " <br/> " + val["StockDescription"] + "</b> </p>" +
                                "</a>" +
                                "</li>";
                }

                else if (val["ActionTaken"] == 'wait') {
                    var mDate = new Date();

                    mDate.setDate(mDate.getDate() + parseInt(val["CancellationPeriod"]));

                    var newdate = (mDate.getMonth() + 1) + "/" + mDate.getDate() + "/" + mDate.getFullYear()

                    strHTML += "<li> " +
                                "<a href='#' class='btnWait' ID='" + val["StockID"] + "'> <p class='ui-li-desc' style='white-space:normal; font-size:large'> <b> SKU # : " + val["StockNumber"] + "</b> </p>" +
                                "<p style='white-space:normal; font-size:large'><b>" + val["StockDescription"] + " </b></p> <br/>" +
                                "<p style='white-space:normal; font-size:large'> <b> Action taken: " + val["ActionTaken"] + " until " + newdate +
                                " </b> </p> </a>" +
                                "</li>";
                }

                else {
                    strHTML += "<li> " +
                                "<a href='#' class='btnWait' ID='" + val["StockID"] + "'> <p class='ui-li-desc' style='white-space:normal; font-size:large'> <b> SKU # : " + val["StockNumber"] + "</b> </p>" +
                                "<p style='white-space:normal; font-size:large'><b>" + val["StockDescription"] + " </b></p> <br/>" +
                                "<p style='white-space:normal; font-size:large'><b> Action taken: " + val["ActionTaken"] +
                                "</b> </p> </a>" +
                                "</li>";
                }

            });

            $("#lstItems").html(strHTML);
            $('#lstItems').listview('refresh');

            $(".btnReplace").click(function () {
                assignBackOrderSKUID($(this).attr("ID"), 0);
            });

            $(".btnWait").click(function () {
                assignBackOrderSKUID($(this).attr("ID"), 1);
            });
        }
    });
}

function getReplacementSKU() {
    var m_Replacement = "";

    $.ajax({
        url: "../getSuggestedSKUInformation/",
        dataType: "json",
        type: "POST",
        err: function (err) {
            alert("Error: " + err.toString());
        },
        beforeSend: function () {

        },
        success: function (data) {
            var myData = data.d;

            $.each(myData, function (key, val) {
                str = "SKU # : " + val["StockNumber"] + " <br/> " + val["StockDescription"];                
            });
        }
    });
}

function assignBackOrderSKUID(BackOrderItemsID) {
    var option = {
        m_skuID: BackOrderItemsID
    };

    $.ajax({
        url: "../AssignBackOrderSKUID/",
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

function createNewOrder() {
    $.ajax({
        url: "../CreateNewOrder/",
        dataType: "json",
        type: "POST",
        err: function (err) {
            alert("Error: " + err.toString());
        },
        beforeSend: function () {
            //            $("#bd-loading").html("<img src=\"/Content/WhiteLabel/light/images/loading.gif\" />");
        },
        success: function (data) {
            window.open("../Finish/", "_self");
        }
    });
}