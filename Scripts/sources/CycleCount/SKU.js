$(function () {

    $("#navBranches").css({ "visibility": "visible" });
    $("#navWarehouse").css({ "visibility": "visible" });
    $("#navItems").css({ "visibility": "visible" });

    $("#bd-loading").html("");

    searchItem();

    $("#Search").click(function () {
        searchItem();
    });

    $("#txtSearch").keydown(function (e) {
        if (e.keyCode == 13) {
            searchItem();
        }
    });
})


function BranchID() {
    var m_BranchID;

    this.getID = function(){
        alert(m_BranchID);
        return (m_BranchID);        
    };

    this.setID = function (val) {
        m_BranchID = val;
        $("#bID").html(val);
    };
}


function searchItem() {
    var options = {
        stock: $("#txtSearch").val()
    };

    $.ajax({
        url: "../searchItem/",
        data: JSON.stringify(options),
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
                strHTML += "<li>"
                strHTML += "<a href='#' class='stockID' ID='" + val["StockID"] + "'>"                
                strHTML += "<h3 class='ui-li-heading'> Stock No : " + val["StockNumber"] + "</h3>"
                strHTML += "<p class='ui-li-desc'> Description :" + val["StockDescription"] + "</p>"
                strHTML += "<p class='ui-li-desc'> Count :" + val["Counted"] + "</p>"
                strHTML += "<span class='ui-icon ui-icon-arrow-r ui-icon-shadow'></span>"
                strHTML += "</a></li>"
            });

            $("#lstItems").html(strHTML);
            $('#lstItems').listview('refresh');

            $(".stockID").click(function () {
                var options = {
                    ID: $(this).attr("ID")
                };

                $.ajax({
                    url: "../setItemID/",
                    data: JSON.stringify(options),
                    dataType: "Text",
                    type: "POST",
                    err: function (err) {
                        alert("Error: " + err.toString());
                    },
                    success: function (data) {
                        window.location = "../SKUInfo/";
                    }
                });
            });
        }
    });
}