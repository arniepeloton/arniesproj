require([
    'common'
    ], function () {
        $(function () {
            getEmployeeDetails();
            getTeamInfo();

            createDate();
            setDate();

            $("#btnConfirm").click(function () {
                saveDetails();
            });

            $("#btnCancel").click(function () {
                getSearchField();
            })
        })

        function getSearchField() {
            $.ajax({
                url: "getSearchKeyName/",
                dataType: "Text",
                type: "POST",
                err: function (err) {
                    alert("Error: " + err.toString());
                },
                beforeSend: function () {
                },
                success: function (data) {
                    window.open("ListOfEmployees?EmployeeName=" + data, "_self");
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

            $("#SdayStart").html(strHTMLDay);
            $("#SyearStart").html(strHTMLYear);

            $('#SmonthStart').selectmenu('refresh');
            $('#SdayStart').selectmenu('refresh');
            $('#SyearStart').selectmenu('refresh');
        }

        function setDate() {

            var mdate = new Date();

            $("#SdayStart").val(mdate.getDate());
            $("#SmonthStart").val(mdate.getMonth());
            $("#SyearStart").val(mdate.getFullYear());

            $('#SdayStart').selectmenu('refresh');
            $('#SmonthStart').selectmenu('refresh');
            $('#SyearStart').selectmenu('refresh');

        }

        function saveDetails() {
            var mMonthStart = parseInt($("#SmonthStart").val()) + 1;
            var mMonthEnd = parseInt($("#SmonthEnd").val()) + 1;

            var option = {
                DateStart: mMonthStart + "/" + $("#SdayStart").val() + "/" + $("#SyearStart").val(),
                DateEnd: mMonthEnd + "/" + $("#SdayEnd").val() + "/" + $("#SyearEnd").val()
            };

            $.ajax({
                url: "UpdateEmployee/",
                dataType: "Text",
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
                        $("#lblBranch").html('<b>' + val["TeamName"] + '</b>');
                    });
                }
            });
        }
    });