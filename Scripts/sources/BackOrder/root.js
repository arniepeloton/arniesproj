$(function () {
    getBackOrderCancellationDate();
    getBackOrderCancellationAction();

    $("#btnShowBackOrderList").click(function () {
        window.open("../backorder/BackOrderList/", "_self");
    })
})

function getBackOrderCancellationDate() {
    $.ajax({
        url: "../backorder/getBackOrderCancellationDate/",
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
                if (val["DefaultValue"] == 0) {
                    $("#mbortRespond").html("<b> Cancel all items on your order that have no stock on hand </b>");
                }
                else {
                    $("#mbortRespond").html("<b> Replace all items on your order list that have available substitutes.  However, we shall still cancel all items on your order list that have no available substitutes. </b>");
                }                
                //('<b>' + mDate.getMonth() + '/' + mDate.getDate() + '/' + mDate.getFullYear() + '</b>');
            });
        }
    });
}

function getBackOrderCancellationAction() {
    $.ajax({
        url: "../backorder/getBackOrderCancellationAction/",
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
            var mDate = new Date();

            $.each(myData, function (key, val) {
                $("#mbortAction").html('<b>' + val["DefaultValue"] + '</b>');                
            });
        }
    });
}

function formatDate(mDate) {
    var myDays = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
    var myMonths = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];

    return myDays[mDate.getDay()] + ", " + myMonths[mDate.getMonth()] + " " + mDate.getDate() + " " + mDate.getFullYear() + " " + mDate.getHours() + ":" + mDate.getMinutes();

}

var skuID = '';
var orderID = 'c95b250f-cbb9-4dcf-91b3-42cc289c9345';
var backLocation = '';
//var BackOrderItemsID = '';
var TransSalesBackOrderItemsID = '';


