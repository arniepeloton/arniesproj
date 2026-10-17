$(function () {
    createDate();
    getBackOrder();
    getBackOrderDetails();

    $("#btnDone").click(function () {
        window.open("../Review/", "_self");
        //createNewOrder();
    });
})

function getBackOrder() {
    $.ajax({
        url: "../getBackOrder/",
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
                $("#customer").html(val["CompanyName"]);
                $("#telNo").html(val["ContactNumber"]);
                $("#SalesOrder").html(val["OrderNo"]);
                $("#SODate").html(val["LastUpdated"]);
            });
        }
    });
}

function getBackOrderDetails() {

    $.ajax({
        url: "../getBackOrderDetails/",
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
                
                if (val["IsHasReplacement"] == "" || val["IsHasReplacement"] == 0) {

                    if (val["ActionTaken"] == 'No Action Taken') {                        
                            strHTML += "<li> " +
                                    "<a href='#' class='btnWait' ID='" + val["StockID"] + "'> <p style='white-space:normal' class='ui-li-desc'> <h2> SKU # : " + val["StockNumber"] + " </h2> </p>" +
                                    "<p style='white-space:normal' class='ui-li-aside ui-li-desc'><strong> Pending Action </strong></p>" +
                                    "<p style='white-space:normal; font-size:large' class='ui-li-desc'> <b> " + val["StockDescription"] + " </b>" + "</p></a>" +
                                    "</li>";
                    }
                    else {

                            strHTML += "<li> " +
                                    "<a href='#' class='btnWait' ID='" + val["StockID"] + "'> <p style='white-space:normal' class='ui-li-desc'> <h2> SKU # : " + val["StockNumber"] + " </h2> </p>" +
                                    "<p style='white-space:normal' class='ui-li-aside ui-li-desc'><strong> OK </strong></p>" +
                                    "<p style='white-space:normal; font-size:large' class='ui-li-desc'> <b> " + val["StockDescription"] + " </b>" + "</p></a>" +
                                    "</li>";
                    }

                }

                else {

                    if (val["ActionTaken"] == 'No Action Taken') {
                            strHTML += "<li> " +
                                    "<a href='#' style='white-space:normal' class='btnReplace' ID='" + val["StockID"] + "'> <h2> SKU # : " + val["StockNumber"] + " </h2> " +
                                    "<p style='white-space:normal' class='ui-li-aside ui-li-desc'><strong> Pending Action </strong></p>" +
                                    "<p style='white-space:normal; font-size:large' class='ui-li-desc'> <b> " + val["StockDescription"] + " </b>" + "</p></a>" +
                                    "</li>";
                    }
                    else {
                            strHTML += "<li> " +
                                    "<a href='#' style='white-space:normal' class='btnReplace' ID='" + val["StockID"] + "'> <h2> SKU # : " + val["StockNumber"] + " </h2> " +
                                    "<p style='white-space:normal' class='ui-li-aside ui-li-desc'><strong> Ok </strong></p>" +
                                    "<p style='white-space:normal; font-size:large' class='ui-li-desc'> <b> " + val["StockDescription"] + " </b>" + "</p></a>" +
                                    "</li>";
                    }
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

function assignBackOrderSKUID(BackOrderItemsID, isWait) {
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
            if (isWait == 1) {
                window.open("../Wait/", "_self");
            }
            else {
                window.open("../Replace/", "_self");
            }

        }
    });
}

function SaveBackOrderSKUAction(action, val) {
    var option = {
        action: action,
        m_BackorderID: TransSalesBackOrderItemsID,
        Value: val
    };

    $.ajax({
        url: "../backorder/SaveBackOrderSKUAction/",
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
            window.open("../../", "_self");
        }
    });
}


function createDate(mDate) {
    var ctr = 0;
    var strHTMLDay = $('#select-choice-day');
    var strHTMLYear = $('#select-choice-year');
    

    while (ctr < 31) {

        if (mDate == undefined) {
            strHTMLDay.append(
                $('<option></option>').val(ctr + 1).html(ctr + 1)
            );            
        }
        else {
            if (mDate.getDay() = ctr) {
                strHTMLDay.append(
                    $('<option selected="selected"></option>').val(ctr + 1).html(ctr + 1)
                );
            }            
        }        
        ctr += 1;
    }
}
