$(function () {
    getOriginalSKUInformation();
    createDate();    

    $("#Smonth").change(function () {
        setDay();
        $("#optWaitingOptions").val(7);
        $('#optWaitingOptions').selectmenu('refresh');
    });

    $("#Sday").change(function () {
        setDay();
        $("#optWaitingOptions").val(7);
        $('#optWaitingOptions').selectmenu('refresh');
    });

    $("#Syear").change(function () {
        setDay();
        $("#optWaitingOptions").val(7);
        $('#optWaitingOptions').selectmenu('refresh');
    });

    $("#btnWait").click(function () {
        SaveBackOrderSKUAction();
    });

    $("#optWaitingOptions").change(function () {
        setDate();
    });

})

function getWaitingTime() {
    var option = {        
        m_BackorderID: TransSalesBackOrderItemsID        
    };

    $.ajax({
        url: "../getWaitingTime/",
        dataType: "json",
        data: JSON.stringify(option),
        type: "POST",
        err: function (err) {
            alert("Error: " + err.toString());
        },
        beforeSend: function () {
        },
        success: function (data) {
            var mdate = new Date();
            var myData = data.d;

            $.each(myData, function (key, val) {
                mdate.setDate(mdate.getDate() + val["CancellationPeriod"]);
            });
            
            $("#Sday").val(mdate.getDate());
            $("#Smonth").val(mdate.getMonth());
            $("#Syear").val(mdate.getFullYear());

            setDay();

            $('#Sday').selectmenu('refresh');
            $('#Smonth').selectmenu('refresh');
            $('#Syear').selectmenu('refresh');

        }
    });
}


function setDate() {
    
    var mdate = new Date();

    if ($("#optWaitingOptions").val() == 1) {
        mdate.setDate(mdate.getDate() + 1);
    }
    else if ($("#optWaitingOptions").val() == 2) {
        mdate.setDate(mdate.getDate() + 2);
    }
    else if ($("#optWaitingOptions").val() == 3) {
        mdate.setDate(mdate.getDate() + 3);
    }
    else if ($("#optWaitingOptions").val() == 4) {
        mdate.setDate(mdate.getDate() + 7);
    }
    else if ($("#optWaitingOptions").val() == 5) {
        mdate.setDate(mdate.getDate() + 14);
    }
    else if ($("#optWaitingOptions").val() == 6) {
        mdate.setDate(mdate.getDate() + 30);
    }

        $("#Sday").val(mdate.getDate());
        $("#Smonth").val(mdate.getMonth());
        $("#Syear").val(mdate.getFullYear());

        setDay();

        $('#Sday').selectmenu('refresh');
        $('#Smonth').selectmenu('refresh');
        $('#Syear').selectmenu('refresh');

}   

function setDay() {
    var myDays = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];

    var sMonth = parseInt($("#Smonth").val()) + 1;

    var mdate = new Date(sMonth + "/" + $("#Sday").val() + "/" + $("#Syear").val());

    if (myDays[mdate.getDay()] != undefined) {
        $("#Waitingday").html(myDays[mdate.getDay()]);
    }
        
}

function SaveBackOrderSKUAction() {

    var mMonth = parseInt($("#Smonth").val()) + 1;

    var option = {
        action: "wait",
        m_BackorderID: TransSalesBackOrderItemsID,
        value: mMonth + "/" + $("#Sday").val() + "/" + $("#Syear").val()
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
        },
        success: function (data) {
            window.open("../BackOrderList/", "_self");
        }
    });

}

function createDate(mDate) {    
    var mDate = new Date();
    var mYear = mDate.getFullYear();
    var ctr = 0;
    var strHTMLDay = "<option value=0 selected='selected'>Day</option> "; ;
    var strHTMLYear = "<option value=0 selected='selected'>Year</option> ";

    while (ctr < 31) {
        if (ctr < 3) {
            strHTMLYear += "<option value='" + (mYear + ctr) + "'> " + (mYear + ctr) + "</option> ";
        }
            strHTMLDay += "<option value='" + (ctr + 1) + "'>" + (ctr + 1) + "</option> ";
        ctr += 1;
    }

    strHTMLYear += "</select>";
    strHTMLDay += "</select>";

    $("#Sday").html(strHTMLDay);
    $("#Syear").html(strHTMLYear);


    $('#Smonth').selectmenu('refresh');
    $('#Sday').selectmenu('refresh');
    $('#Syear').selectmenu('refresh');
}


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
                TransSalesBackOrderItemsID = val["TransSalesBackOrderItemsID"];
                getWaitingTime();
            });
        }
    });
}