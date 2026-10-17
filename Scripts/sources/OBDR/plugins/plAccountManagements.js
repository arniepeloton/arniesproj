/* 
Account Management
#################################################################################
This plugin is responsible for managing all Bank User Accounts and Encrypt every
password with RSA Encryption 128 Bits
#################################################################################
*/

(function ($) {
    $.fn.extend({
        RegisterAccount: function (objValue) {
            var defaults = objValue;

            objValue = $.extend(defaults, objValue);
            return $(this).each(function () {
                /*############################################## 
                Validate all input form the user
                ##############################################*/
                //var results = ValidateUserInputs(objValue);
                RegisterAccountInput(objValue);
            });
        },
        getRegisteredAccounts: function () {
            return $(this).each(function () {
                getRegisteredAccounts();
            });
        },
        getLibraryBankPosition: function () {
            return $(this).each(function () {
                getLibraryPositions();
            });
        },
        getLibraryBanks: function () {
            return $(this).each(function () {
                getLibraryBanks();
            });
        },
        getLibraryBranches: function () {
            return $(this).each(function () {
                getLibraryBranches();
            });
        },
        savePOSMachineDescription: function (objValue) {
            var defaults = objValue;
            objValue = $.extend(defaults, objValue);
            return $(this).each(function () {
                savePOSMachineDescription(objValue);
                getPOSDescriptions();
            });
        },
        getPOSDescriptions: function () {
            return $(this).each(function () {
                getPOSDescriptions();
            });
        }
    });


    /* 
    ##############################################################################
    Main Module Function in JQuery
    ##############################################################################
    Libraries
    ##############################################################################
    */
    function getPOSMachineNumberWithCashier(objValue) {
        $.ajax({
            type: "POST",
            url: "getPOSCashiers/",
            dataType: "json",
            data: JSON.stringify(objValue),
            beforeSend: function () {
                //Loading...
            }, success: function (data) {
                var mData = data.d.POSMachine.Rows;
                var vHtml = "";
                $.each(mData, function (key, data) {
                    vHtml += "<option value='" + data["MachineNumber"] + "-" + data["UserID"] + "'>" + data["MachineNumber"] + "</option>";
                });
                $("#pos-machine-number-entry-form-pos-machine-select").html(vHtml);
            }, error: function () {
            }
        });
    }

    function getPOSDescriptions() {
        $.ajax({
            type: "POST",
            url: "getPOSDescription/",
            dataType: "json",
            beforeSend: function () {
                //Loading...
            }, success: function (data) {
                var mData = data.d.POSDescription.Rows;
                var vHtml = "";
                $.each(mData, function (key, data) {
                    vHtml += "<tr>";
                    vHtml += "  <td align='center'>" + data["MachineNumber"] + "</td>";
                    vHtml += "  <td align='center'>" + data["Descriptions"] + "</td>";
                    vHtml += "  <td align='center'>" + data["Name"] + "</td>";
                    vHtml += "<tr>";
                });
                $("#pos-machine-number-descriptions").html(vHtml);
            }, error: function () {
            }
        });
    }

    function getLibraryBranches() {
        $.ajax({
            type: "POST",
            url: "getLibraryBranches/",
            dataType: "json",
            beforeSend: function () {
                //Loading...
            }, success: function (data) {
                var mData = data.d.Branches.Rows;
                var vHtml = "";

                vHtml += "<option value='" + data["ID"] + "'>SELECT - VALUE</option>";
                $.each(mData, function (key, data) {
                    vHtml += "<option value='" + data["ID"] + "'>" + data["Name"] + "</option>";
                });
                $("#pos-machine-number-entry-form-branches-select").html(vHtml);
                $("#pos-machine-number-entry-form-branches-select").change(function () {
                    getPOSMachineNumberWithCashier({ "branchid": $(this).attr("value") });
                });
            }, error: function () {
            }
        });
    }
    function getLibraryBanks() {
        $.ajax({
            type: "POST",
            url: "getLibraryBanks/",
            dataType: "json",
            beforeSend: function () {
                //Loading...
            }, success: function (data) {
                var mData = data.d.LibraryBanks.Rows;
                var vHtml = "";
                $.each(mData, function (key, data) {
                    vHtml += "<option value='" + data["ID"] + "'>" + data["BankName"] + "</option>";
                });
                $("#library-banks").html(vHtml);
            }, error: function () {
            }
        });
    }

    function getLibraryPositions() {
        $.ajax({
            type: "POST",
            url: "GetLibraryBankPositions/",
            dataType: "json",
            beforeSend: function () {
                //Loading...
            }, success: function (data) {
                var mData = data.d.LibraryBankPositions.Rows;
                var vHtml = "";
                $.each(mData, function (key, data) {
                    vHtml += "<option value='" + data["ID"] + "'>" + data["Position"] + "</option>";
                });
                $("#library-bank-positions").html(vHtml);
            }, error: function () {
            }
        });
    }

    /*##############################################################################*/

    function savePOSMachineDescription(objValue) {
        $.ajax({
            type: "POST",
            url: "savePOSDescription/",
            dataType: "json",
            data: JSON.stringify(objValue),
            beforeSend: function () {
                //$(".ui-dialog-title").html("Register New User Account <img src='../../Content/images/ajax-loader.gif' />");
            }, success: function (data) {
                //$(".ui-dialog-title").html("Register New User Account");
                //var mData = data.InputResults;
                //alert(mData);
                alert("Save");
            }, error: function () {
                alert("Error had occured - Javascript Level");
            }
        });
    }

    function RegisterAccountInput(objValue) {
        //Execute Ajax
        $.ajax({
            type: "POST",
            url: "RegisterUsers/",
            dataType: "json",
            data: JSON.stringify(objValue),
            beforeSend: function () {
                $(".ui-dialog-title").html("Register New User Account <img src='../../Content/images/ajax-loader.gif' />");
            }, success: function (data) {
                $(".ui-dialog-title").html("Register New User Account");
                var mData = data.InputResults;
                alert(mData);
            }, error: function () {
                alert("Error had occured - Javascript Level");
            }
        });
    }

    function getRegisteredAccounts() {
        $.ajax({
            type: "POST",
            url: "GetRegisteredUsers/",
            dataType: "json",
            beforeSend: function () {
                $("#emptyRecords").html("<img src='../../Content/images/ajax-loader.gif' />");
            }, success: function (data) {
                var mData = data.d.Accounts.Rows;
                var vHtml = null;

                $.each(mData, function (key, data) {
                    vHtml += "<tr>";
                    vHtml += "  <td align='center'>";
                    vHtml += "      <a href='#' class='records' id='" + data["ID"] + "'>Delete</a> | ";
                    vHtml += "      <a href='#' class='records' id='" + data["ID"] + "'>Edit</a>";
                    vHtml += "  </td>";
                    vHtml += "  <td align='center'>" + data["LastName"] + ", " + data["FirstName"] + " " + data["MiddleName"] + "</td>";
                    vHtml += "  <td align='center'>" + data["EmailAddress"] + "</td>";
                    vHtml += "  <td align='center'>" + data["BankName"] + "</td>";
                    vHtml += "  <td align='center'>" + data["Position"] + "</td>";
                    vHtml += "</tr>";
                });
                $("#tblUserAccountManagement tbody").html(vHtml);
                /* functions on editing and deleting of records */

            }, error: function () {
                alert("Error had occured - Javascript Level");
            }
        });
    }
})(jQuery);