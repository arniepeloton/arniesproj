require([
    'common'
    ], function () {
        $(function () {
            isNew();

            $("#txtTeam").keydown(function (e) {
                if (e.keyCode == 13) {
                    window.open("../ListOfTeams?TeamName=" + $("#txtTeam").val(), "_self");
                }
            });

            $("#btnTeams").click(function () {
                window.open("../ListOfTeams?TeamName=" + $("#txtTeam").val(), "_self");
            });

            $("#txtLocation").keydown(function (e) {
                if (e.keyCode == 13) {
                    window.open("../ListOfBranches?BranchName=" + $("#txtLocation").val(), "_self");
                }
            });

            $("#btnLocation").click(function () {
                window.open("../ListOfBranches?BranchName=" + $("#txtLocation").val(), "_self");
            });

            $("#btnNext").click(function () {
                saveTeam();
            })
        });

        function isNew() {

            AjaxSend({
                url: 'crmsetup/isNew/',
                success: function (vData) {
                    if (vData == 0) {
                        getTeamInfo();
                    }
                    else {
                        $("#btnTeams").css({ "visibility": "hidden" })
                        getBranchInfo();
                    }
                },
                error: function (vError) {
                    var pDummy;
                }
            });
        }

        function getTeamInfo() {

            AjaxSend({
                url: 'crmsetup/getTeamInfo/',
                success: function (vData) {
                    $("#txtTeam").val(vData.data.Rows[0].TeamName);
                    $("#txtDescription").val(vData.data.Rows[0].TeamDescription);
                    $("#txtObjective").val(vData.data.Rows[0].TeamObjective);
                    //$("#txtLocation").val(vData.data.Rows[0].Name);
                },
                error: function (vError) {
                    var pDummy;
                }
            });
        }

        function getBranchInfo() {

            AjaxSend({
                url: 'crmsetup/getBranchInfo/',
                dataType: "Text",
                success: function (vData) {
                    $("#txtLocation").val(vData)
                },
                error: function (vError) {
                    alert(vError)
                    var pDummy;
                }
            });
        }

        function saveTeam() {

            var option = {
                TeamName: $("#txtTeam").val(),
                Description: $("#txtDescription").val(),
                Objective: $("#txtObjective").val()
            };

            AjaxSend({
                url: 'crmsetup/saveTeam/',
                data: option,
                success: function (vData) {
                    window.open("TeamMembers/", "_self");
                },
                error: function (vError) {
                    var pDummy;
                }
            });
        }

        function PrintHeaderPOSReceipt() {
            var CompanyName = 'Davao Unified Corporation';
            var TradeName = '(The Paper Tree)';
            var ContactNumber = '02-259-90-36';
            var TIN = '005-161-114V';
            var Serial = '12345';

            return CompanyName + "\n" +
					TradeName + "\n" +
					'Tel No. ' + ContactNumber + "\n" +
					'TIN # ' + TIN + "\n" +
					'Serial # ' + Serial + "\n \n" +
					'Item Description\t' + 'Subtotal\n';
        }

        function PrintBodyPOSReceipt() {
            var CompanyName = 'Davao Unified Corporation';
            var TradeName = '(The Paper Tree)';
            var ContactNumber = '02-259-90-36';
            var TIN = '005-161-114V';
            var Serial = '12345';

            return CompanyName + "\n" +
					TradeName + "\n" +
					'Tel No. ' + ContactNumber + "\n" +
					'TIN # ' + TIN + "\n" +
					'Serial # ' + Serial + "\n \n" +
					'Item Description\t' + 'Subtotal\n';
        }

        

    });