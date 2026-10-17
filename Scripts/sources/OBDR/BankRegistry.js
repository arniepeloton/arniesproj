$(function () {
    var currentDate = new Date();
    var month = currentDate.getMonth() + 1;
    var day = currentDate.getDate();
    var year = currentDate.getFullYear();

    $("#dtFrom").datepicker();
    $("#dtTo").datepicker();
    $("#dtDateFrom").datepicker();
    $("#dtDateTo").datepicker();
    $("#txtDateDeposit").datepicker();
    $("#tpt-pos-sales, #txtPOSSalesDate, #txtBankDepositedDate").datepicker();
    $(this).ready(function(){
        $(this).getDepositRegistry();
        $("#tpt-pos-sales, #txtPOSSalesDate, #txtBankDepositedDate").attr("value",month + "/" + day + "/" + year);
    });

    $("#dialog:ui-dialog").dialog("destroy");
    $("#btnRegisterDeposits").click(function () {
        $("#checkdeposits").css({"display":"none"});
        $("#dialog-message").dialog({
            title: "Online Bank Deposit Registry",
            modal: true,
            width: 900,
            height: 600,
            buttons: {
                Add: function () {
                    var checkInfo = [];
                    $("#tPOSCheckDetails tr").each(function(){
                        var checkId = $(this).find("td").eq(6).find("input").val();
                        if ($(this).find("td").eq(6).find("input").attr("checked") == "checked"){
                            checkInfo.push({"payor":$(this).find("td").eq(5).find("#txtPayor").val(),
                                            "checkID":checkId});
                        }
                    });
                    //JSON Format String
                    var machineNumber = $("#optMachineNumbers").val().split(" ");
                    $(this).seveDepositRegistry({
                        "bankinfoid":$("#txtBankAccountNumber").attr("name"),
                        "depositcashamount":$("#txtCashDepositedAmount").attr("value"),
                        "salesdate":$("#txtPOSSalesDate").attr("value"),
                        "depositdate":$("#txtBankDepositedDate").attr("value"),
                        "transactiontype":$("#txtTransactionType").attr("value"),
                        "posdescriptionid":machineNumber[0],
                        "rovingteller":$("#txtRovingTeller").attr("value"),
                        "checkInfo":checkInfo,
                        "isnew":true
                    });
                    //$(this).dialog("close");
                    $(this).getDepositRegistry();
                }
            }
        });
        $(this).getBranchInfo();
    });

    $("#link-check-refresh").click(function(e){
        e.preventDefault();
        var machineNumber = $("#optMachineNumbers").val().split(" ");
        $(this).getCheckDeposits({"branchid":$("#optBranches").val(),"salesdate":$("#txtPOSSalesDate").val(),"machinenumber":machineNumber[1]});
    });

    $(this).getListOfBranches({"salesdate":$("#txtPOSSalesDate").attr("value")});

    $("#tpt-pos-deposit-amount, #txtCashDepositedAmount").focusout(function(){
        $(this).attr("value", formatCurrency($(this).attr("value")));
    });
    //Transaction Type
    /* ======================================================================== */
    $("#btnAddTransactionType").click(function(){
        $("#transaction-type").dialog({
            title: "Add Transaction Type",
            modal: true,
            width: 420,
            height: 125,
            buttons: {
                Add: function () {
                    $(this).dialog("close");
                }
            }
         });
    });
    
    /* ======================================================================== */
    //Assigning POS Machine Number
    /* ======================================================================== */
    $("#tab-4-event").click(function(){
        $(this).getLibraryBranches();
        $(this).getPOSDescriptions();
    });
    $("#pos-machine-number-description-save").click(function(){
        var posnumber = $("#pos-machine-number-entry-form-pos-machine-select").attr("value").split("-");
        $(this).savePOSMachineDescription({
            "branchid":$("#pos-machine-number-entry-form-branches-select").attr("value"),
            "cashierid":posnumber[1],
            "descriptions":$("#pos-machine-number-description").attr("value"),
            "machinenumber":posnumber[0]
        });
    });
    /* ======================================================================== */

    /* ======================================================================== */
    //User Account Registration
    /* ======================================================================== */
    $("#btnAddNewUser").click(function(){
        $("#user-account-form").dialog({
            title: "Register New User Account",
            modal: true,
            width: 500,
            height: 375,
            buttons: {
                Add: function () {
                    $(this).RegisterAccount({
                        "lastname":$("#txtLastname").attr("value"),
                        "firstname":$("#txtFirstname").attr("value"),
                        "middlename":$("#txtMiddlename").attr("value"),
                        "emailaddress":$("#txtEmailAddress").attr("value"),
                        "username":$("#txtUsername").attr("value"),
                        "password":$("#txtPassword").attr("value"),
                        "positionid":$("#library-bank-positions").attr("value"),
                        "bankid":$("#library-banks").attr("value")
                    });
                },
            }
        });
        $(this).getLibraryBankPosition();
        $(this).getLibraryBanks();
    });

/* ======================================================================== */
    $("#event-tabs-1").click(function(){
        $(this).getDepositRegistry();
    });

    $("#event-tabs-2").click(function(){
        $(this).getBranchInfoHistory();
    });

    $("#pos-deposit-registry-search").click(function(e){
        e.preventDefault();
        $(this).getDepositRegistryHistory({
            "datestart":$("#dtDateFrom").attr("value"),
            "dateend":$("#dtDateTo").attr("value"),
            "branchid":$("#pos-deposit-history-branches").attr("value")
        });
    });

    $("#tab-3-event").click(function(){
        $(this).getRegisteredAccounts();
    });
    
});

//<!-- Original:  Cyanide_7 (leo7278@hotmail.com) -->
    //<!-- Web Site:  http://www7.ewebcity.com/cyanide7 -->
    //<!-- This script and many more are available free online at -->
    //<!-- The JavaScript Source!! http://javascript.internet.com -->
    //<!-- Begin
    function formatCurrency(num) {
        num = num.toString().replace(/\$|\,/g, '');
        if (isNaN(num))
            num = "0";
        sign = (num == (num = Math.abs(num)));
        num = Math.floor(num * 100 + 0.50000000001);
        cents = num % 100;
        num = Math.floor(num / 100).toString();
        if (cents < 10)
            cents = "0" + cents;
        for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3); i++)
            num = num.substring(0, num.length - (4 * i + 3)) + ',' +
    num.substring(num.length - (4 * i + 3));
        return (((sign) ? '' : '-') + num + '.' + cents);
    }
    //  End -->