(function ($) {
    $.fn.extend({
        getBranchInfo: function () {
            return $(this).each(function () {
                getBranches();
            });
        },
        seveDepositRegistry: function (obj) {
            var defaults = obj;
            obj = $.extend(defaults, obj);
            return $(this).each(function () {
                saveDepositRegistry(obj);
            });
        },
        getDepositRegistry: function () {
            return $(this).each(function () {
                getDepositRegistry();
            });
        },
        getBranchInfoHistory: function () {
            return $(this).each(function () {
                getBranchesHistory();
            });
        },
        getDepositRegistryHistory: function (obj) {
            var defaults = obj;
            obj = $.extend(defaults, obj);
            return $(this).each(function () {
                getDepositRegistryHistory(obj);
            });
        }, getListOfBranches: function (obj) {
            var defaults = obj;
            obj = $.extend(defaults, obj);
            return $(this).each(function () {
                getListOfBranches();
            });
        }, getCheckDeposits: function (obj) {
            var defaults = obj;
            obj = $.extend(defaults, obj);
            return $(this).each(function () {
                displayCheckDetails(obj);
            });
        }
    });

    /* Revised OBDR Forms 
    ==================================================================
    */
    function getListOfBranches() {
        $.ajax({
            type: "POST",
            url: "getListOfBranchesByIn/",
            dataType: "json",
            beforeSend: function () {
                $("#optBranches").html("Loading...");
            }, success: function (data) {
                var mData = data.d.Branches.Rows;
                var indexCounter = 0;
                var id = 0;
                var html = "";
                $("#optBranches").html("");
                $.each(mData, function (vKey, vData) {
                    if (indexCounter == 0) {
                        id = vData["ID"];
                    }
                    html += "<option value='" + vData["ID"] + "'>" + vData["Name"] + "</option>";
                    indexCounter++;
                });
                $("#optBranches").html(html);
                //Machine Numbers
                $("#optBranches").ready(function (event) {
                    var obj = { "branchid": id };
                    getMachineNumber(obj);
                    displayAccountInfo(obj);
                });
                $("#optBranches").change(function (event) {
                    var obj = { "branchid": $(this).attr("value") };
                    getMachineNumber(obj);
                    displayAccountInfo(obj);
                });
            }, error: function () {
                //alert("Error Fetching Record. Please Try Again!");
            }
        });
    }

    function getMachineNumber(obj) {

        $.ajax({
            type: "POST",
            url: "getListOfMachineNumbers/",
            dataType: "json",
            data: JSON.stringify(obj),
            beforeSend: function () {
                $("#optMachineNumbers").html("Loading...");
            }, success: function (data) {
                var mData = data.d.MachineNumbers.Rows;
                var html = "";
                $("#optMachineNumbers").html("");
                $.each(mData, function (vKey, vData) {
                    html += "<option value='" + vData["ID"] + " " + vData["MachineNumber"] + "'>" + vData["Descriptions"] + "</option>";
                });
                $("#optMachineNumbers").html(html);
                if (obj["machinenumber"]) {
                    $("#optMachineNumbers option[value='" + obj["posdescid"] + " " + obj["machinenumber"] + "']").attr("selected", true);
                }
            }, error: function () {
                alert("Error Fetching Record. Please Try Again!");
            }
        });
    }

    function displayAccountInfo(obj) {
        $.ajax({
            type: "POST",
            url: "displayAccountInfo/",
            dataType: "json",
            data: JSON.stringify(obj),
            beforeSend: function () {
                $("#txtBankAccountNumber, #txtBankAccountName").attr("value", "Loading....");
            }, success: function (data) {
                var mData = data.d.AccountByBranch.Rows;
                $("#txtBankAccountNumber, #txtBankAccountName").attr("value", "");
                $.each(mData, function (vKey, vData) {
                    $("#txtBankAccountNumber").attr("value", vData["AccountNumber"]);
                    $("#txtBankAccountNumber").attr("name", vData["ID"]);
                    $("#txtBankAccountName").attr("value", vData["AccountName"]);
                });
            }, error: function () {
                alert("Error Fetching Record. Please Try Again!");
            }
        });
    }

    function displayCheckDetails(obj) {
        $.ajax({
            type: "POST",
            url: "displayCheckAmountToBeDeposits/",
            dataType: "json",
            data: JSON.stringify(obj),
            beforeSend: function () {
                $("#tPOSCheckDetails").html("<td cospan='7'>Loading...</td>");
            }, success: function (data) {
                var mData = data.d.POSCheckDetails.Rows;
                var html = "";
                $("#tPOSCheckDetails").html("<td>No Record Found!</td>");
                $.each(mData, function (vKey, vData) {
                    html += "<tr class='POSCheck'>";
                    html += "   <td id='" + vData["BankID"] + "'>" + vData["BankName"] + "</td>";
                    html += "   <td id='" + vData["BankBranchID"] + "'>" + vData["BranchName"] + "</td>";
                    html += "   <td>" + vData["CheckDate"] + "</td>";
                    html += "   <td>" + vData["CheckNo"] + "</td>";
                    html += "   <td>" + vData["CheckAmount"] + "</td>";
                    html += "   <td><input type='text' id='txtPayor' size='20' value='" + vData["Payor"] + "' /></td>";
                    if (vData["TransDepositCheckID"]) {
                        html += "   <td><input type='checkbox' value='" + vData["ID"] + "' checked /></td>";
                    } else {
                        html += "   <td><input type='checkbox' value='" + vData["ID"] + "' /></td>";
                    }
                    html += "</tr>";
                });
                $("#tPOSCheckDetails").html(html);
            }, error: function () {
            }
        });
    }
    /* =============================================================== */
    /* Functions */

    function getDepositRegistryByID(obj) {
        $.ajax({
            type: "POST",
            url: "getDepositRegistryByID/",
            dataType: "json",
            data: JSON.stringify(obj),
            beforeSend: function () {
                //Loading...
            }, success: function (data) {
                //Assign Value
                var mData = data.d.DepositRegistryByID.Rows;
                $.each(mData, function (key, data) {
                    $("#tpt-pos-account-number").attr("name", data["ID"]);
                    $("#tpt-pos-account-number").attr("value", data["AccountNumber"]);
                    var AccountName = data["AccountName"].split(" - ");
                    $("#tpt-pos-account-name").attr("value", AccountName[0]);
                    $("#tpt-pos-number").attr("value", data["POSDescriptionID"]);
                    $("#tpt-pos-deposit-amount").attr("value", data["DepositAmount"]);
                    $("#txtDateDeposit").attr("value", data["DateDeposits"]);
                    $("#tpt-pos-roving-teller").attr("value", data["RovingTeller"]);
                });

                $("#dialog-message").dialog({
                    title: "Online Bank Deposit Registry",
                    modal: true,
                    width: 375,
                    height: 300,
                    buttons: {
                        Add: function () {
                            $(this).seveDepositRegistry({
                                "id": $("#tpt-pos-account-number").attr("name"),
                                "bankinfoid": $("#tpt-pos-account-number").attr("name"),
                                "depositamount": $("#tpt-pos-deposit-amount").attr("value"),
                                "depositdate": $("#txtDateDeposit").attr("value"),
                                "transactiontype": $("#tpt-pos-transaction-type").attr("value"),
                                "posdescriptionid": $("#tpt-pos-number").attr("value"),
                                "rovingteller": $("#tpt-pos-roving-teller").attr("value"),
                                "isnew": false
                            });
                            $(this).dialog("close");
                            getDepositRegistryByID(obj);
                        }
                    }
                });
                $(this).getBranchInfo();
            }, error: function () {
                //Error Message
            }
        });
    }

    function getDepositRegistry() {
        $.ajax({
            type: "POST",
            url: "getDepositRegistry/",
            dataType: "json",
            beforeSend: function () {
                //Loading...
                var vHtml = "";
                $("#tpt-pos-deposit-display").html(vHtml);

                vHtml += "<tr>";
                vHtml += "  <td colspan='8' align='center'><img src='../../Content/images/ajax-loader.gif' /></td>";
                vHtml += "</tr>";
                $("#tpt-pos-deposit-display").html(vHtml);
            }, success: function (data) {
                $("#tpt-pos-deposit-display").html("<tr><td colspan='8' align='center'>No current deposit found for today.</td></tr>");
                var mData = data.d.DepositRegistry.Rows;
                var vHtml = "";
                var totalAmountDeposit = 0;
                var totalCheckDeposit = 0;

                $.each(mData, function (key, data) {
                    vHtml += "<tr>";
                    vHtml += "  <td><a href='#' class='pos-deposit-cancel' name='" + data["ID"] + "'>Cancel</a> | <a href='#' class='pos-deposit-edit' name='" + data["ID"] + "'>Edit</a></td>";
                    vHtml += "  <td align='left'>&nbsp;" + data["AccountNumber"] + "</td>";
                    vHtml += "  <td align='left'>&nbsp;" + data["AccountName"] + "</td>";
                    vHtml += "  <td align='left'>&nbsp;" + data["Name"] + "</td>";
                    vHtml += "  <td align='left'>&nbsp;" + data["Descriptions"] + "</td>";
                    vHtml += "  <td align='right'>" + formatCurrency(data["DepositAmount"]) + "</td>";
                    if (data["IsCheckPayment"] == "1") {
                        vHtml += "  <td align='right'>&nbsp;" + formatCurrency(data["CheckTotalAmount"]) + "</td>";
                    } else {
                        vHtml += "  <td align='left'> - </td>";
                    }
                    vHtml += "  <td align='center'>" + data["SalesDate"] + "</td>";
                    vHtml += "  <td align='center'>" + data["DateDeposits"] + "</td>";
                    vHtml += "  <td align='left'>&nbsp;" + data["RovingTeller"] + "</td>";
                    vHtml += "</tr>";
                    totalAmountDeposit += parseFloat(data["DepositAmount"]);
                    totalCheckDeposit += parseFloat(data["CheckTotalAmount"]);
                });
                vHtml += "<tr>";
                vHtml += "  <td colspan='5' align='right'><b>TOTAL AMOUNT DEPOSIT</b></td>";
                vHtml += "  <td align='right'>" + formatCurrency(totalAmountDeposit) + "</td>";
                vHtml += "  <td align='right'>" + formatCurrency(totalCheckDeposit) + "</td>";
                vHtml += "  <td colspan='3'></td>";
                vHtml += "</tr>";
                $("#tpt-pos-deposit-display").html(vHtml);

                $(".pos-deposit-cancel").click(function () {
                    var id = $(this).attr("name");
                    var remarks = $("#pos-deposit-registry-reason").attr("value");

                    $("#pos-deposit-registry-cancel").dialog({
                        title: "Cancel Bank Deposit Registry",
                        modal: true,
                        width: 375,
                        height: 300,
                        buttons: {
                            Ok: function () {
                                var input = { "id": id,
                                    "remarks": remarks
                                };
                                $.ajax({
                                    type: "POST",
                                    url: "deleteDepositRegistryByID/",
                                    dataType: "json",
                                    data: JSON.stringify(input),
                                    beforeSend: function () {
                                    }, success: function (data) {
                                        if (data.d.Text) {
                                            if (data.d.Text == "Ok") {
                                                getDepositRegistry();
                                            }
                                        } else if (data.d.Error) {
                                            alert(data.d.Error);
                                        }
                                    }, error: function () {
                                    }
                                });
                                $(this).dialog("close");
                            },
                            Cancel: function () {
                                $(this).dialog("close");
                            }
                        }
                    });

                });

                $(".pos-deposit-edit").click(function () {
                    var id = $(this).attr("name");
                    $("#checkdeposits").css({ "display": "block" });
                    //Get Information from the database
                    $.ajax({
                        type: "POST",
                        url: "getTransDepositForEdit/",
                        dataType: "json",
                        data: JSON.stringify({ "id": id }),
                        beforeSend: function () {
                        }, success: function (data) {
                            var mData = data.d.Deposits.Rows;

                            $.each(mData, function (vKey, vData) {
                                $("#optBranches option[value='" + vData["BranchID"] + "']").attr("selected", true);
                                var obj = { "branchid": vData["BranchID"], "machinenumber": vData["MachineNumber"], "posdescid": vData["POSDescID"] };
                                getMachineNumber(obj);
                                displayAccountInfo(obj);
                                $("#txtPOSSalesDate").attr("value", vData["SalesDate"]);
                                $("#txtBankDepositedDate").attr("value", vData["DateDeposits"]);
                                $("#txtTransactionType option[value='" + vData["TransactionType"] + "']").attr("selected", true);
                                $("#txtRovingTeller").attr("value", vData["RovingTeller"]);
                                $("#txtCashDepositedAmount").attr("value", vData["DepositAmount"]);

                                $(this).getCheckDeposits({ "branchid": vData["BranchID"], "salesdate": vData["SalesDate"], "machinenumber": vData["MachineNumber"] });
                                $("#txtCashDepositedAmount").attr("value", formatCurrency($("#txtCashDepositedAmount").attr("value")));
                            })

                            $("#dialog-message").dialog({
                                title: "Online Bank Deposit Registry",
                                modal: true,
                                width: 900,
                                height: 600,
                                buttons: {
                                    Add: function () {
                                        var checkInfo = [];
                                        $("#tPOSCheckDetails tr").each(function () {
                                            var checkId = $(this).find("td").eq(6).find("input").val();
                                            if ($(this).find("td").eq(6).find("input").attr("checked") == "checked") {
                                                checkInfo.push({ "payor": $(this).find("td").eq(5).find("#txtPayor").val(),
                                                    "checkID": checkId
                                                });
                                            }
                                        });
                                        //JSON Format String
                                        var machineNumber = $("#optMachineNumbers").val().split(" ");
                                        $(this).seveDepositRegistry({
                                            "id": id,
                                            "bankinfoid": $("#txtBankAccountNumber").attr("name"),
                                            "depositcashamount": $("#txtCashDepositedAmount").attr("value"),
                                            "salesdate": $("#txtPOSSalesDate").attr("value"),
                                            "depositdate": $("#txtBankDepositedDate").attr("value"),
                                            "transactiontype": $("#txtTransactionType").attr("value"),
                                            "posdescriptionid": machineNumber[0],
                                            "rovingteller": $("#txtRovingTeller").attr("value"),
                                            "checkInfo": checkInfo,
                                            "isnew": false
                                        });
                                        //$(this).dialog("close");
                                        $(this).getDepositRegistry();
                                    }
                                }
                            });
                            $(this).getBranchInfo();
                        }, error: function () {
                        }
                    });



                });
            }, error: function () {
            }
        });
    }

    function getDepositRegistryHistory(obj) {
        $.ajax({
            type: "POST",
            url: "getDepositRegistryByDateRange/",
            dataType: "json",
            data: JSON.stringify(obj),
            beforeSend: function () {
                //Loading...
                var vHtml = "";
                $("#tpt-pos-deposit-display").html(vHtml);
                vHtml += "<tr>";
                vHtml += "  <td colspan='8' align='center'><img src='../../Content/images/ajax-loader.gif' /></td>";
                vHtml += "</tr>";
                $("#tpt-pos-deposit-display").html(vHtml);
            }, success: function (data) {
                $("#tpt-pos-deposit-display").html("<tr><td colspan='8' align='center'>No current deposit found for today.</td></tr>");
                var mData = data.d.HistoricalDeposits.Rows;
                var vHtml = "";
                var totalAmountDeposit = 0;
                var totalCheckDeposit = 0;
                $("#pos-deposit-history-display").html(vHtml);

                $.each(mData, function (key, data) {
                    vHtml += "<tr>";
                    vHtml += "  <td><a href='#' class='pos-deposit-cancel' name='" + data["ID"] + "'>Cancel</a> | <a href='#' class='pos-deposit-edit' name='" + data["ID"] + "'>Edit</a></td>";
                    vHtml += "  <td align='left'>&nbsp;" + data["AccountNumber"] + "</td>";
                    vHtml += "  <td align='left'>&nbsp;" + data["AccountName"] + "</td>";
                    vHtml += "  <td align='left'>&nbsp;" + data["Name"] + "</td>";
                    vHtml += "  <td align='left'>&nbsp;" + data["Descriptions"] + "</td>";
                    vHtml += "  <td align='right'>" + formatCurrency(data["DepositAmount"]) + "</td>";
                    if (data["IsCheckPayment"] == "1") {
                        vHtml += "  <td align='right'>&nbsp;" + formatCurrency(data["CheckTotalAmount"]) + "</td>";
                    } else {
                        vHtml += "  <td align='left'> - </td>";
                    }
                    vHtml += "  <td align='center'>" + data["SalesDate"] + "</td>";
                    vHtml += "  <td align='center'>" + data["DateDeposits"] + "</td>";
                    vHtml += "  <td align='left'>&nbsp;" + data["RovingTeller"] + "</td>";
                    vHtml += "</tr>";
                    totalAmountDeposit += parseFloat(data["DepositAmount"]);
                    totalCheckDeposit += parseFloat(data["CheckTotalAmount"]);
                });
                vHtml += "<tr>";
                vHtml += "  <td colspan='5' align='right'><b>TOTAL AMOUNT DEPOSIT</b></td>";
                vHtml += "  <td align='right'>" + formatCurrency(totalAmountDeposit) + "</td>";
                vHtml += "  <td align='right'>" + formatCurrency(totalCheckDeposit) + "</td>";
                vHtml += "  <td colspan='3'></td>";
                vHtml += "</tr>";
                $("#pos-deposit-history-display").html(vHtml);

                $(".pos-deposit-cancel").click(function (e) {
                    e.preventDefault();
                    var id = $(this).attr("name");
                    var remarks = $("#pos-deposit-registry-reason").attr("value");

                    $("#pos-deposit-registry-cancel").dialog({
                        title: "Cancel Bank Deposit Registry",
                        modal: true,
                        width: 375,
                        height: 300,
                        buttons: {
                            Ok: function () {
                                var input = { "id": id,
                                    "remarks": remarks
                                };
                                $.ajax({
                                    type: "POST",
                                    url: "deleteDepositRegistryByID/",
                                    dataType: "json",
                                    data: JSON.stringify(input),
                                    beforeSend: function () {
                                    }, success: function (data) {
                                        if (data.d.Text) {
                                            if (data.d.Text == "Ok") {
                                                getDepositRegistry();
                                            }
                                        } else if (data.d.Error) {
                                            alert(data.d.Error);
                                        }
                                    }, error: function () {
                                    }
                                });
                                $(this).dialog("close");
                            },
                            Cancel: function () {
                                $(this).dialog("close");
                            }
                        }
                    });

                });

                $(".pos-deposit-edit").click(function () {
                    var id = $(this).attr("name");
                    $("#checkdeposits").css({ "display": "block" });
                    //Get Information from the database
                    $.ajax({
                        type: "POST",
                        url: "getTransDepositForEdit/",
                        dataType: "json",
                        data: JSON.stringify({ "id": id }),
                        beforeSend: function () {
                        }, success: function (data) {
                            var mData = data.d.Deposits.Rows;

                            $.each(mData, function (vKey, vData) {
                                $("#optBranches option[value='" + vData["BranchID"] + "']").attr("selected", true);
                                var obj = { "branchid": vData["BranchID"], "machinenumber": vData["MachineNumber"], "posdescid": vData["POSDescID"] };
                                getMachineNumber(obj);
                                displayAccountInfo(obj);
                                $("#txtPOSSalesDate").attr("value", vData["SalesDate"]);
                                $("#txtBankDepositedDate").attr("value", vData["DateDeposits"]);
                                $("#txtTransactionType option[value='" + vData["TransactionType"] + "']").attr("selected", true);
                                $("#txtRovingTeller").attr("value", vData["RovingTeller"]);
                                $("#txtCashDepositedAmount").attr("value", vData["DepositAmount"]);

                                $(this).getCheckDeposits({ "branchid": vData["BranchID"], "salesdate": vData["SalesDate"], "machinenumber": vData["MachineNumber"] });
                                $("#txtCashDepositedAmount").attr("value", formatCurrency($("#txtCashDepositedAmount").attr("value")));
                            })

                            $("#dialog-message").dialog({
                                title: "Online Bank Deposit Registry",
                                modal: true,
                                width: 900,
                                height: 600,
                                buttons: {
                                    Add: function () {
                                        var checkInfo = [];
                                        $("#tPOSCheckDetails tr").each(function () {
                                            var checkId = $(this).find("td").eq(6).find("input").val();
                                            if ($(this).find("td").eq(6).find("input").attr("checked") == "checked") {
                                                checkInfo.push({ "payor": $(this).find("td").eq(5).find("#txtPayor").val(),
                                                    "checkID": checkId
                                                });
                                            }
                                        });
                                        //JSON Format String
                                        var machineNumber = $("#optMachineNumbers").val().split(" ");
                                        $(this).seveDepositRegistry({
                                            "id": id,
                                            "bankinfoid": $("#txtBankAccountNumber").attr("name"),
                                            "depositcashamount": $("#txtCashDepositedAmount").attr("value"),
                                            "salesdate": $("#txtPOSSalesDate").attr("value"),
                                            "depositdate": $("#txtBankDepositedDate").attr("value"),
                                            "transactiontype": $("#txtTransactionType").attr("value"),
                                            "posdescriptionid": machineNumber[0],
                                            "rovingteller": $("#txtRovingTeller").attr("value"),
                                            "checkInfo": checkInfo,
                                            "isnew": false
                                        });
                                        //$(this).dialog("close");
                                        $(this).getDepositRegistry();
                                    }
                                }
                            });
                            $(this).getBranchInfo();
                        }, error: function () {
                        }
                    });

                });
            }, error: function () {
            }
        });
    }

    function saveDepositRegistry(obj) {
        $.ajax({
            type: "POST",
            url: "saveDepositRegistry/",
            dataType: "json",
            data: JSON.stringify(obj),
            beforeSend: function () {
                //Loading...
            }, success: function (data) {
                if (data.d.Text) {
                    $("#tpt-pos-deposit-amount").attr("value", 0);
                    alert("Record was successfully save.");
                } else if (data.d.Error) {
                    alert(data.d.Error);
                }
            }, error: function () {
            }
        });
    }

    function getAccountInfoByBranch(obj) {
        $.ajax({
            type: "POST",
            url: "getAccountInfo/",
            dataType: "json",
            data: JSON.stringify(obj),
            beforeSend: function () {
                //Loading...
            }, success: function (data) {
                var mData = data.d.AccountByBranch.Rows;
                $.each(mData, function (key, data) {
                    $("#tpt-pos-account-number").attr("name", data["ID"]);
                    $("#tpt-pos-account-number").attr("value", data["AccountNumber"]);
                    $("#tpt-pos-account-name").attr("value", data["AccountName"]);
                });
            }, error: function () {
            }
        });
    }

    function getBranches() {
        $.ajax({
            type: "POST",
            url: "GetLibraryBranches/",
            dataType: "json",
            beforeSend: function () {
                //Loading...
            }, success: function (data) {
                var mData = data.d.Branches.Rows;
                var vHtml = "";
                var selected = 0;

                $("#cboBranches").html(vHtml);
                $.each(mData, function (key, data) {
                    if (selected == 0) {
                        vHtml += "<option value='" + data["ID"] + "' default>" + data["Name"] + "</option>";
                        getPOSDescription({ "branchid": data["ID"] });
                    } else {
                        vHtml += "<option value='" + data["ID"] + "'>" + data["Name"] + "</option>";
                    }
                    selected++;
                });
                $("#cboBranches").html(vHtml);
                $("#cboBranches").select(function () {
                    getPOSDescription({ "branchid": $("#cboBranches").attr("value") });
                    // Load Account Information
                    getAccountInfoByBranch({ "branchid": $("#cboBranches").attr("value") });
                });
            }, error: function () {
            }
        });
    }

    function getBranchesHistory() {
        $.ajax({
            type: "POST",
            url: "GetLibraryBranches/",
            dataType: "json",
            beforeSend: function () {
                var vHtml = "";
                vHtml += "<option>loading...</option>";
                $("#pos-deposit-history-branches").html(vHtml);
            }, success: function (data) {
                var mData = data.d.Branches.Rows;
                var vHtml = "";

                vHtml += "<option>SELECT - VALUE</option>";
                $.each(mData, function (key, data) {
                    vHtml += "<option value='" + data["ID"] + "'>" + data["Name"] + "</option>";
                });
                $("#pos-deposit-history-branches").html(vHtml);
            }, error: function () {
            }
        });
    }

    function getPOSDescription(objValue) {
        $.ajax({
            type: "POST",
            url: "getPOSDescription/",
            dataType: "json",
            data: JSON.stringify(objValue),
            beforeSend: function () {
                //Loading...
            }, success: function (data) {
                var mData = data.d.POSDescriptions.Rows;
                var vHtml = "";

                $.each(mData, function (key, data) {
                    vHtml += "<option value='" + data["ID"] + "'>" + data["Descriptions"] + "</option>";
                });
                $("#tpt-pos-number").html(vHtml);
            }, error: function () {
            }
        });
    }

})(jQuery)