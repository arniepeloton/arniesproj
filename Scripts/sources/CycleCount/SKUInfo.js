$(function () {
    $("#navBranches").css({ "visibility": "visible" });
    $("#navWarehouse").css({ "visibility": "visible" });
    $("#navItems").css({ "visibility": "visible" });
    $("#navCountType").css({ "visibility": "visible" });
    
    $("#InputCountForActiveSellingArea").click(function () {        
        setCountType(1);
    });

    $("#InputCountForDeactivatedArea").click(function () {        
        setCountType(2);
    });

    $("#InputCountForBadStocks").click(function () {
        setCountType(3);
    });

})

function setCountType(CountType) {
    var options = {
        Type: CountType
    };

    $.ajax({
        url: "../setCountType/",
        data: JSON.stringify(options),
        dataType: "json",
        type: "POST",
        err: function (err) {
            alert("Error: " + err.toString());
        },
        success: function (data) {
        }
    });

    window.location = "../startcount/";
}