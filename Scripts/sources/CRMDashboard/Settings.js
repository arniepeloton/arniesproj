require([
    'common'
], function () {

    var arrScripts = [];
    var intScriptsCtr = 0;
    var ScriptsState = "";
    var ScriptsRankNo = "";

    $(document).ready(function () {

        $('body')
        .on('click', '#btnSettingForms', function () {
            $('#divSettingForms').pageshow();
        })
        .on('click', '#btnSettingUserScripts', function () {
            $('#divSettingUserScripts').pageshow();
        })
        .on('click', '#btnAddScript', function () {
            ClearScriptsFields();
            $('#popNewScript').show();
        })
        .on('click', '#btnCancelNewScripts', function () {
            $('#popNewScript').hide();
        })
        .on('click', '#btnAddForm', function () {
            ClearFormVariablesFields();
            $('#pageSettingNewForm').pageshow();
        })
        .on('click', '#btnOkNewScripts', function () {
            intScriptsCtr = intScriptsCtr + 1;

            if (ScriptsState == "") {
                arrScripts.push({
                    ScriptID: $('#txtScriptID').val(),
                    Name: $('#txtScriptName').val(),
                    Language: $('#selScriptLanguage').val(),
                    Purpose: $('#txtScriptPurpose').val(),
                    Scripts: $('#txtScript').val(),
                    ScriptState: 'Add',
                    IsNew: 1,
                    ScriptsRankNo: intScriptsCtr
                })
            }
            else {
                arrScripts[sessionStorage.FormScritpindex].ScriptsRankNo = ScriptsRankNo;
                arrScripts[sessionStorage.FormScritpindex].ScriptID = $('#txtScriptID').val();
                arrScripts[sessionStorage.FormScritpindex].Name = $('#txtEmailAdd').val();
                arrScripts[sessionStorage.FormScritpindex].Language = $('#selScriptLanguage').val();
                arrScripts[sessionStorage.FormScritpindex].Purpose = $('#txtScriptPurpose').val();
                arrScripts[sessionStorage.FormScritpindex].Scripts = $('#txtScript').val();
                arrScripts[sessionStorage.FormScritpindex].ScriptState = sessionStorage.FormScriptState;
                arrScripts[sessionStorage.FormScritpindex].IsNew = sessionStorage.FormScriptsIsNew;
            }

            AddScriptsInfo();
            ScriptsState = "";
            $('#popNewScript').hide();
        })

        function AddScriptsInfo() {
            var pLV = $('#lstFormScript');

            pLV.empty();

            for (var i = 0; i < arrScripts.length; i++) {
                if (arrScripts[i].ScriptState != 'Remove') {
                    var arrindex = i;
                    pLV.append($('<li data-icon="delete">')
                        .append($('<a>')
                            .data(arrScripts[i])
                            .attr('arrindex', i)
                            .html('<b>Name:</b> ' + arrScripts[i].Name + '<br>' +
                                    '<span style="font-weight:normal">Purpose: ' + (arrScripts[i].Purpose || '-') + '</span><br>' +
                                    '<span style="font-weight:normal">Language: ' + (arrScripts[i].Language || '-') + '</span>'
                            )
                            .click(function () {
                                EditScriptInfo($(this).attr('arrindex'));
                            })
                        )
                        .append($('<a>')
                                .data(arrScripts[i])
                                .click(function (e) {
                                    var StateRankNo = $(this).data().StateRankNo;

                                    if ($(this).data().IsNew == 1) {
                                        //remove selected script
                                        arrScripts = $.grep(arrScripts, function (val) {
                                            return val.StateRankNo != StateRankNo;
                                        })
                                    }
                                    else {
                                        arrScripts[arrindex].ScriptState = "Remove";
                                    }

                                    AddScriptsInfo();
                                })
                            )
                    )
                }
            }

            pLV.listview('refresh');
        }

        function EditScriptInfo(arrindex) {
            ScriptsRankNo = arrScripts[arrindex].EmailRankNo;
            ScriptsState = "Edit";
            sessionStorage["FormScritpindex"] = arrindex;
            sessionStorage["FormScriptIsNew"] = arrScripts[arrindex].IsNew;
            sessionStorage["FormScriptState"] = arrScripts[arrindex].ScriptState;
            $('#txtScriptID').val(arrScripts[arrindex].ScriptID == undefined ? "" : arrScripts[arrindex].ScriptID);
            $('#txtScriptName').val(arrScripts[arrindex].Name);
            $('#selScriptLanguage').val(arrScripts[arrindex].Language).selectmenu('refresh');
            $('#txtScriptPurpose').val(arrScripts[arrindex].Purpose);
            $('#txtScript').val(arrScripts[arrindex].Scripts);
            $('#popNewScript').show();
        }

        function ClearScriptsFields() {
            $('#txtScriptID').val('')
            $('#txtScriptName').val(''),
            $('#selScriptLanguage').val(''),
            $('#txtScriptPurpose').val(''),
            $('#txtScript').val('')
        }

        function ClearFormVariablesFields() {
            arrScripts = [];
            intScriptsCtr = 0;
            ScriptsState = "";
            ScriptsRankNo = "";
            $('#txtScriptName').val('');
            $('#selScriptLanguage').val('').selectmenu().selectmenu('refresh');
            ClearScriptsFields();
        }
    })// end document
})// end require