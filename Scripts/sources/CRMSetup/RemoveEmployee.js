require([
    'common'
    ], function () {
        $(function () {
            getEmployeeDetails();
            getTeamInfo();
            createDate();
            setDate();

            $("#btnConfirm").click(function () {
                RemoveSelectedEmployee();
            });

            $("#btnCancel").click(function () {
                window.open("TeamMembers/", "_self");
            });
        })


        function RemoveSelectedEmployee() {
            var mMonthEnd = parseInt($("#SmonthEnd").val()) + 1;

            var option = {
                DateEnd: mMonthEnd + "/" + $("#SdayEnd").val() + "/" + $("#SyearEnd").val()
            }

            $.ajax({
                url: "RemoveSelectedEmployee/",
                dataType: "json",
                data: JSON.stringify(option),
                type: "POST",
                err: function (err) {
                    alert("Error: " + err.toString());
                },
                beforeSend: function () {
                },
                success: function (data) {
                    window.open("TeamMembers/", "_self");
                }
            });
        }

        function getEmployeeDetails() {
            var option = {
                Employee: $("#txtEmployee").val()
            };

            $.ajax({
                url: "getEmployees/",
                dataType: "json",
                data: JSON.stringify(option),
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
                        $("#lblName").html('<b>' + val["Name"] + '</b>');
                        $("#lblName2").html('<b>' + val["Name"] + '</b>');
                    });
                }
            });
        }


        function getTeamInfo() {
            $.ajax({
                url: "getTeamInfo/",
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
                        $("#lblTeamName").html('<b>' + val["TeamName"] + '</b>');
                        $("#lblTeamName2").html('<b>' + val["TeamName"] + '</b>');
                    });
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

            $("#SdayEnd").html(strHTMLDay);
            $("#SyearEnd").html(strHTMLYear);

            $('#SmonthEnd').selectmenu('refresh');
            $('#SdayEnd').selectmenu('refresh');
            $('#SyearEnd').selectmenu('refresh');
        }

        function setDate() {

            var mdate = new Date();

            $("#SdayEnd").val(mdate.getDate());
            $("#SmonthEnd").val(mdate.getMonth());
            $("#SyearEnd").val(mdate.getFullYear());

            $('#SdayEnd').selectmenu('refresh');
            $('#SmonthEnd').selectmenu('refresh');
            $('#SyearEnd').selectmenu('refresh');

        }
    });