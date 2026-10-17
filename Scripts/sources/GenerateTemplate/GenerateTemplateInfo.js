require([
    (InAndroid ? '/Scripts/sources/People/PeopleLogger_offline.js' : '')
    ], function () {

        $(function () {
            $('html').removeClass('ui-mobile');

            $('#txtClass').focus();

            var classPosition = $("#txtClass").position();

            jQuery.curCSS = jQuery.css;
            //Autocomplete List of Class / Commodity
            $('#txtClass').autocomplete({
                minLength: 3,
                source: function (request, response) {
                    $.ajax({
                        url: 'GenerateTemplate/getUNSPSCClass',
                        dataType: 'json',
                        data: {
                            term: request.term
                        },
                        success: function (data) {
                            $(".ui-autocomplete").css('top', classPosition.top + 31 + 'px'); //set the top margin 
                            var results = [];
                            if (data != null) {
                                $.each(data, function (i, item) {
                                    if (item.UNSPSCClassData.Rows.length > 0) { //if has a result
                                        for (var p = 0; p <= item.UNSPSCClassData.Rows.length - 1; p++) {

                                            var itemToAdd = {

                                                value: item.UNSPSCClassData.Rows[p].Product,
                                                label: item.UNSPSCClassData.Rows[p].Product,
                                                id: item.UNSPSCClassData.Rows[p].ProductID
                                            };

                                            results.push(itemToAdd);

                                        }
                                        return response(results);
                                    } else { // if not results returned
                                        $('#spanClass').show();
                                        $('#spanClassRequired').hide();
                                        $('#txtClass').addClass('form-validation-error');
                                    }

                                });
                            }

                        }

                    });
                },
                select: function (event, ui) {
                    AutoCompleteSelectHandler(event, ui);

                }

            });

            var featurePosition = $("#txtFeature").position();

            //Autocomplete List of Features
            $('#txtFeature').autocomplete({
                minLength: 3,
                source: function (request, response) {
                    $.ajax({
                        url: 'GenerateTemplate/getPIMProductFeature',
                        dataType: 'json',
                        data: {
                            term: request.term
                        },
                        success: function (data) {
                            $(".ui-autocomplete").css('top', featurePosition.top + 31 + 'px'); //set the top margin 
                            var results = [];
                            if (data != null) {
                                $.each(data, function (i, item) {
                                    if (item.PIMProductFeatureData.Rows.length > 0) { //if has a result
                                        for (var p = 0; p <= item.PIMProductFeatureData.Rows.length - 1; p++) {

                                            var itemToAdd = {

                                                value: item.PIMProductFeatureData.Rows[p].ProductFeature,
                                                label: item.PIMProductFeatureData.Rows[p].ProductFeature,
                                                id: item.PIMProductFeatureData.Rows[p].ProductFeatureID
                                            };

                                            results.push(itemToAdd);

                                        }
                                        return response(results);
                                    } else { // if not results returned
                                        $('#spanAdd').show();
                                        $('#spanAddRequired').hide();
                                        $('#txtFeature').addClass('form-validation-error');
                                    }

                                });
                            }

                        }

                    });
                },
                select: function (event, ui) {
                    AutoCompleteSelectHandlerFeature(event, ui);

                }

            });

            var keyStop = {
                8: ":not(input:text, textarea, input:file, input:password)", // stop backspace = back
                13: "input:text, input:password", // stop enter = submit 

                end: null
            };


            $(document).bind("keydown", function (event) {
                var selector = keyStop[event.which];

                if (selector !== undefined && $(event.target).is(selector)) {
                    event.preventDefault(); //stop event
                }
                return true;
            });

            //Reset value on change to get the correct id on select
            $('#txtClass').change(function () {

                $('#hdnClassID').val('');
                $('#spanClass').hide();
                $('#spanClassRequired').hide();
                $('#spanClassButton').hide();
                $('#txtClass').removeClass('form-validation-error');


            }).bind("keydown", function (event) {
                if (event.keyCode === $.ui.keyCode.TAB &&
                $(this).data("autocomplete").menu.active) {
                    event.preventDefault();
                }

            });

            //Reset value on change to get the correct id on select
            $('#txtFeature').change(function () {
                $('#hdnFeatureID').val('');
                $('#spanAdd').hide();
                $('#spanAddRequired').hide();
                $('#spanAddButton').hide();
                $('#txtFeature').removeClass('form-validation-error');


            }).bind("keydown", function (event) {
                if (event.keyCode === $.ui.keyCode.TAB &&
                $(this).data("autocomplete").menu.active) {
                    event.preventDefault();
                }

            });

            $('#selClass').change(function () {
                $('#spanAddRequired').hide();
                $('#selClass').removeClass('form-validation-error');


            });

            $('#btnDownload').click(function (e) {
                e.preventDefault();

                if (array.length < 1) {
                    if ($('#hdnClassID').val() != '') {
                        $('#spanClassButton').show();

                    } else {
                        if ($.trim($('#txtClass').val()) != '' && $.trim($('#txtClass').val()) != 'Enter keyword/s') {
                            $('#spanClass').show();
                        }
                        else {
                            $('#spanClassRequired').show();

                        }

                        $('#txtClass').addClass('form-validation-error');
                    }

                    if ($('#hdnFeatureID').val() != '') {
                        $('#spanAddButton').show();

                    }
                    return;
                }

                if ($('#hdnFeatureID').val() != '') {
                    $('#spanAddButton').show();
                    return;
                }

                if (confirm('You are about to download PIM Excel Template. Do you want to continue?')) {
                    window.location = GetPathFormat("/GenerateTemplate/TestDownload");
                } else {
                    // Do nothing!
                }


            });


            var arrayAddFeature = [];

            function getCustomFeature() {
                var feature = $.trim($('#txtFeature').val());

                //Check if existing feature
                if (arrayAddFeature.indexOf($('#hdnFeatureID').val()) < 0) {
                    //Check existing
                    if ($('#selFeatures').find(":contains('" + feature + "')").length > 0) {
                        alert('Oops! The feature you are about to add already exists in the list.');

                    } else {
                        var pData = [];
                        pData = {
                            CustomFeature: feature,
                            FeatureID: $('#hdnFeatureID').val()
                        };

                        AjaxSend({
                            url: "GenerateTemplate/getCustomFeature",
                            data: pData,
                            success: function () {

                                $('#selCustomFeatures').append(new Option(feature, feature, false, false));
                                arrayAddFeature.push($('#hdnFeatureID').val());

                                alert('Custom feature successfully added to the list.');

                                //Clear textbox
                                $('#txtFeature').val('');
                                $('#selClass').val('');
                                $('#hdnFeatureID').val('');
                            }
                        });
                    }

                } else {
                    alert('Oops! The feature you are trying to add already exists in the list.');
                }

            }

            $('#btnAdd').click(function (e) {
                e.preventDefault();
                $('#spanAddButton').hide();

                if ($.trim($('#txtFeature').val()) == '' || $.trim($('#txtFeature').val()) == 'Enter keyword/s') {
                    $('#hdnFeatureID').val('');
                    $('#spanAddRequired').show();
                    $('#txtFeature').addClass('form-validation-error');

                }

                if ($.trim($('#selClass').val()) == '') {
                    $('#spanAddRequired').show();
                    $('#selClass').addClass('form-validation-error');
                    return;
                }

                if ($.trim($('#hdnFeatureID').val()) == '') {
                    if ($.trim($('#txtFeature').val()) != '' && $.trim($('#txtFeature').val()) != 'Enter keyword/s') {
                        $('#spanAdd').show();
                        $('#txtFeature').addClass('form-validation-error');

                    }
                    return;
                }

                getCustomFeature();

            });

            function removeA(arr) {
                var what, a = arguments, L = a.length, ax;
                while (L > 1 && arr.length) {
                    what = a[--L];
                    while ((ax = arr.indexOf(what)) !== -1) {
                        arr.splice(ax, 1);
                    }
                }
                return arr;
            }

            $('#btnRemove').click(function (e) {
                e.preventDefault();
                var customFeature = $.trim($('#selCustomFeatures option:selected').text());

                if (customFeature != '') {
                    if (confirm("Are you sure you want to remove " + customFeature + "?")) {
                        var pData = [];
                        pData = {
                            CustomFeature: customFeature
                        };

                        AjaxSend({
                            url: "GenerateTemplate/removeCustomFeature",
                            data: pData,
                            success: function (vData) {
                                $('#selCustomFeatures option:selected').remove();
                                removeA(arrayAddFeature, vData.FeatureIDData.Rows[0].ProductFeatureID);
                                alert('Feature has been removed from the list.');

                            }

                        });

                    } else {
                        // Do nothing!
                    }
                }


            });

            $('#btnAddClass').click(function (e) {
                e.preventDefault();
                $('#spanClassButton').hide();

                if ($.trim($('#txtClass').val()) == '' || $.trim($('#txtClass').val()) == 'Enter keyword/s') {
                    $('#hdnClassID').val('');
                    $('#spanClassRequired').show();
                    $('#txtClass').addClass('form-validation-error');

                }

                if ($.trim($('#hdnClassID').val()) == '') {
                    if ($.trim($('#txtClass').val()) != '' && $.trim($('#txtClass').val()) != 'Enter keyword/s') {
                        $('#spanClass').show();
                        $('#txtClass').addClass('form-validation-error');

                    }
                    return;
                }

                //Check if existing class/commodity 
                if (array.indexOf($('#hdnClassID').val()) < 0) {
                    getFeatures($('#hdnClassID').val());

                    alert('Product features successfully added to the list.');
                    //Clear textbox
                    $('#txtClass').val('');
                    $('#hdnClassID').val('');

                    $('#spanClass').hide();
                    $('#spanClassRequired').hide();
                    $('#txtClass').removeClass('form-validation-error');
                } else {
                    alert('Oops! You have already added the features of that product type. Please choose another.');
                }

            });


        }); //Inner function

        var productName;

        function AutoCompleteSelectHandler(event, ui) {
            var selectedObj = ui.item;
            $('#hdnClassID').val(selectedObj.id);

            productName = selectedObj.label;


        }

        function AutoCompleteSelectHandlerFeature(event, ui) {
            var selectedObj = ui.item;
            $('#hdnFeatureID').val(selectedObj.id);

        }

        var array = [];


        function getFeatures() {

            pData = { ProductID: $.trim($('#hdnClassID').val()), ProductType: productName };
            AjaxSend({
                url: 'GenerateTemplate/getUNSPSCFeature',
                data: pData,
                success: function (vData) {

                    var vRows = vData.UNSPSCFeatureData.Rows;
                    var selFeatures = $('#selFeatures');
                    //$('#selFeatures').empty();

                    $("#selFeatures").append("<optgroup label='" + productName + "'>");
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        $('#selFeatures').append(new Option(vRows[p].FeatureLabelValue, vRows[p].FeatureIDValue, false, false));
                    }
                    $('#selFeatures').append('</optgroup>');


                    $('#selClass').append(new Option(productName, $.trim($('#hdnClassID').val()), false, false));


                    array.push($.trim($('#hdnClassID').val()));

                },
                error: function (vError) {
                    var pDummy;
                }
            });

        }


    });
