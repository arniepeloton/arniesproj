$(function () {

    getCustomerInformation();
    getCustomerNotes();
    $("#level0").html("Employees");
    $("#level0").css({ 'visibility': 'visible' });
    $("#level1").html("Sales");
    $("#level1").css({ 'visibility': 'visible' });
    $("#level2").html("Customer Information");
    $("#level2").css({ 'visibility': 'visible' });
})

function getCustomerInformation() {

    var ctr = 0;

    $.ajax({
        url: "../getCustomerInformation/",
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
            $.each(myData, function (key, val) {
                $(".customerTitle").html(val["CompanyName"]);

                $("#companyName").html("<b>" + val["CompanyName"] + "</b>");

                var rating = "";

                switch (val["Rating"]) {
                    case "0": rating = "Cold"; break;
                    case "1": rating = "Warm"; break;
                    case "2": rating = "Hot"; break;
                }

                $("#companyInformation").html("<table style='font-size:large;'> " +
                                                "<tr>" + "<td>Contact Number: </td><td style = 'padding:30px'></td><td>" + val["ContactNumber"] + "</td></tr>" +
                                                "<tr>" + "<td>Business Type: </td><td style = 'padding:30px'></td><td>" + val["BusinessType"] + "</td>" +
                                                "<td rowspan=3 style='padding:30px; border:none'> </td>" +
                                                "<td rowspan=3 style='padding:30px; border:none'> </td>" +
                                                "<td rowspan=3>" +
                                                    "<table style='border-left: 1px solid; border-right: 1px solid; border-top: 1px solid; border-bottom: 1px solid;'>" +
                                                        "<tr>" +
                                                            "<td style = 'padding:10px'>" +
                                                                 "Est. Value/Month:" +
                                                            "</td>" +
                                                            "<td style = 'padding:30px'> </td>" +
                                                            "<td style = 'padding-right:10px'>" +
                                                                 val["EstimatedValue"] +
                                                            "</td>" +
                                                        "</tr>" +
                                                        "<tr>" +
                                                            "<td  style = 'padding:10px'>" +
                                                                 "Est. Close Date:" +
                                                            "</td>" +
                                                            "<td style = 'padding:30px'> </td>" +
                                                            "<td style = 'padding-right:10px'>" +
                                                                 val["EstimatedClosedDate"] +
                                                            "</td>" +
                                                        "</tr>" +
                                                        "<tr>" +
                                                            "<td  style = 'padding:10px'>" +
                                                                 "Rating:" +
                                                            "</td>" +
                                                            "<td style = 'padding:30px'> </td>" +
                                                            "<td style = 'padding-right:10px'>" +
                                                                 rating +
                                                            "</td>" +
                                                        "</tr>" +
                                                    "</table>" +
                                                "</td>" +
                                                "</tr>" +
                                                "<tr>" + "<td style='vertical-align:middle;font-weight:bold'><b>Contact Person: </b></td><td style = 'padding:30px'></td><td style='vertical-align:middle;font-weight:bold'>" + val["ContactPerson"] + "</td>" +
                                                "</tr>" +
                                                "<tr>" + "<td style='vertical-align:middle;font-weight:bold'><b>Email Address:  </b></td><td style = 'padding:30px'></td><td style='vertical-align:middle;font-weight:bold'>" + val["EmailAddress"] + "</td>" +
                                                "</tr>" +
                                                "<tr>" + "<td style='vertical-align:middle;font-weight:bold'><b>Tin:            </b></td><td style = 'padding:30px'></td><td style='vertical-align:middle;font-weight:bold'>" + val["TinNumber"] + "</td></tr>" +
                                                "<tr>" + "<td style='vertical-align:middle;font-weight:bold'><b>Address:        </b></td><td style = 'padding:30px'></td><td style='vertical-align:middle;font-weight:bold' colspan='5'> " +
                                                "<a href='http://maps.google.com/maps?f=q&amp;source=s_q&amp;hl=en&amp;geocode=&amp;q=" + val["Address"] + "' target='_blank'> " +
                                                val["Address"] + "</a></td></tr>" +
                                                "</table>"
                                                );
            });
        }
    });
}

function getCustomerNotes() {

    var ctr = 0;

    $.ajax({
        url: "../getCustomerNotes/",
        dataType: "Text",
        type: "POST",
        err: function (err) {
            alert("Error: " + err.toString());
        },
        beforeSend: function () {
            $("#bd-loading").html("<img src=\"/Content/WhiteLabel/light/images/loading.gif\" />");
        },
        success: function (data) {
            var myData = data.d;
            $("#notes").html(data);
        }
    });
}