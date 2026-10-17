require([
    'common'
    ], function () {

        $(function () {

            /* Initialize coordinates to zero. */
            var x1 = 0, x2 = 0, y1 = 0, y2 = 0;

            $(document).ready(function () {

                require(['/Scripts/jquery.imgareaselect.js'], function () {
                    //Declaring the image area select sript
                });

                /* Load the default image of the product */
                var readerResult = $.ajax({
                    url: "/People/LoadImagePerson",
                    data: JSON.stringify({ conditionalOperator: "profileImg", OrgID: sessionStorage.OrgID }),
                    type: 'POST',
                    async: false,
                    cache: false,
                    contentType: false
                });

                /* Load all the images */
                readerResult.done(function (dataResult) {
                    if (dataResult != "NotExist") {
                        $('#profilePic').append(dataResult);
                    }
                });

                $("#imgSelection").empty();
                $.ajax({
                    url: "/People/LoadImagePerson",
                    data: JSON.stringify({ conditionalOperator: "selectioning", OrgID: sessionStorage.OrgID }),
                    type: 'POST',
                    async: false,
                    cache: false,
                    contentType: false
                }).done(function (dataResult) {
                    if (dataResult != "NotExist") {
                        $("#imgSelection").append(dataResult);
                    } else {
                        MessageBox('There is a problem in loading your profile pictures',
			            "Selection",
			            [
			                {
			                    Text: 'Ok',
			                    Click: function () {
			                        window.open(GetPathFormat("PeopleInfoSummary"), "_self");
			                    }
			                }
			        ]); // END MessageBox
                    }
                });

            });

            function imageCompressor(source_img_obj, quality, output_format) {
                var mime_type = "image/jpeg";
                if (typeof output_format !== "undefined" && output_format == "png") {
                    mime_type = "image/png";
                }

                var cvs = document.createElement('canvas');
                cvs.width = source_img_obj.naturalWidth;
                cvs.height = source_img_obj.naturalHeight;
                var ctx = cvs.getContext("2d").drawImage(source_img_obj, 0, 0);
                var newImageData = cvs.toDataURL(mime_type, quality / 100);
                var result_image_obj = new Image();
                result_image_obj.src = newImageData;
                return result_image_obj;

            }

            $("#btnCancelCrop").click(function (e) {
                e.preventDefault();
                $('#cropImagePopup').popup('close');
                $('.imgareaselect-outer').hide();
                $('.imgareaselect-selection').hide();
                $('.imgareaselect-border1').hide();
                $('.imgareaselect-border2').hide();
                $('.imgareaselect-border3').hide();
                $('.imgareaselect-border4').hide();
                /* Refresh current page to refresh the Cropping libraries */
                //window.open(GetPathFormat("PeopleInfoSummary"), "_self");
            });

            /* Function for changing the default photo */
            $("#btnDefaultPhoto").click(function (e) {
                e.preventDefault();
                if ($("#imgSelection").find(".selected").attr('src') != undefined) {
                    var imageName = $('.selected').attr('src').replace('data:image/jpeg;base64,', '');
                    var readerResult = $.ajax({
                        url: "/People/LoadImagePerson",
                        data: JSON.stringify({ conditionalOperator: "changing", imageData: imageName, OrgID: sessionStorage.OrgID }),
                        type: 'POST',
                        async: false,
                        cache: false,
                        contentType: false
                    }).done(function (dataResult) {
                        if (dataResult == "DoneChange") {
                            MessageBox('Selected photo is now defaulted.',
			                "Default Photo",
			                [
			                    {
			                        Text: 'Ok',
			                        Click: function () {
			                            window.open(GetPathFormat("PeopleInfoSummary"), "_self");
			                        }
			                    }
			            ]); // END MessageBox
                        } else if (dataResult == "NotAllowed") {
                            MessageBox('This is already your default picture.',
			                "Default Photo",
			                [
			                    {
			                        Text: 'Ok',
			                        Click: function () {
			                            window.open(GetPathFormat("PeopleInfoSummary"), "_self");
			                        }
			                    }
			            ]); // END MessageBox
                        } else {
                            MessageBox('Error! File you are trying to change does not exist.',
			                "Default Photo",
			                [
			                    {
			                        Text: 'Ok',
			                        Click: function () {
			                            window.open(GetPathFormat("PeopleInfoSummary"), "_self");
			                        }
			                    }
			            ]); // END MessageBox
                        }
                    });
                    $("#imgSelection").empty();
                } else {
                    MessageBox('Please select a photo to be defaulted.',
			            "Default Photo",
			            [
			                {
			                    Text: 'Ok'
			                }
			        ]); // END MessageBox
                }
            });

            /* Function for deleting a photo */
            $("#btnDeletePhoto").click(function (e) {
                e.preventDefault();

                if ($("#imgSelection").find(".selected").attr('src') != undefined) {
                    var imageName = $('.selected').attr('src').replace('data:image/jpeg;base64,', '');
                    var readerResult = $.ajax({
                        url: "/People/LoadImagePerson",
                        data: JSON.stringify({ conditionalOperator: "deletion", OrgID: sessionStorage.OrgID, imageData: imageName }),
                        type: 'POST',
                        async: false,
                        cache: false,
                        contentType: false
                    });
                    /* Process all the images and check for errors*/
                    readerResult.done(function (dataResult) {
                        if (dataResult == "DoneDeletion") {
                            location.reload();
                        } else {
                            MessageBox('Error! File you are trying to delete does not exist.',
			            "Default Photo",
			            [
			                {
			                    Text: 'Ok'
			                }
			        ]); // END MessageBox
                        }
                    });
                    $("#imgSelection").empty();
                } else {
                    MessageBox('Please select a photo to delete.',
			            "Delete Photo",
			            [
			                {
			                    Text: 'Ok'
			                }
			        ]); // END MessageBox
                }

            });

            /* Checks which image is selected for processing */
            $(document).on('click', '#imgSelection img', function (e) {
                if ($(this).attr('src') != '/Content/images/placeholder.png') {
                    $('.selected').removeClass('selected');
                    $(this).addClass('selected');
                }
            });

            $("#btnDoneCrop").click(function (e) {
                e.preventDefault();

                /* Saving the images */
                var thumbnail_image = document.getElementById('thumbnail');
                var final_image = document.getElementById('finalImage');
                var img = document.getElementById('canvasImage');
                var cSmall = document.createElement('canvas');
                var cMedium = document.createElement('canvas');
                cSmall.width = "50";
                cSmall.height = "50";
                cMedium.width = "100";
                cMedium.height = "100";
                if (x1 == 0 && y1 == 0 && x2 == 0 && y2 == 0) {
                    x1 = 120;
                    y1 = 90;
                    x2 = 220;
                    y2 = 190;
                }
                cSmall.getContext("2d").drawImage(img, x1, y1, x2, y2, 0, 0, 50, 50);
                cMedium.getContext("2d").drawImage(img, x1, y1, x2, y2, 0, 0, 100, 100);
                thumbnail_image.src = cSmall.toDataURL("image/jpeg");
                final_image.src = cMedium.toDataURL("image/jpeg");

                /* Send image src to save */
                var imageString = final_image.src.replace('data:image/jpeg;base64,', '');
                $.ajax({
                    url: "/People/UploadImagePerson",
                    data: JSON.stringify({ imageData: imageString, OrgID: sessionStorage.OrgID }),
                    type: 'POST',
                    async: false,
                    cache: false,
                    processData: false,
                    contentType: false
                });

                /* Refresh current page to refresh the Cropping libraries */
                location.reload();
            });

            $('#btnUploadPhoto').click(function (e) {
                e.preventDefault();

                var IsVacant = false;

                $('#imgSelection img').each(function () {

                    if ($(this).attr('src') == '/Content/images/placeholder.png') {
                        IsVacant = true;
                    }

                });

                if (IsVacant) {
                    $('#fileUploadPersonImage').click();
                } else {
                    MessageBox('You already have 5 pictures. Please delete a photo to upload more.',
			            "Upload Photo",
			            [
			                {
			                    Text: 'Ok',
			                    Click: function () {
			                        window.open(GetPathFormat("PeopleInfoSummary"), "_self");
			                    }
			                }
			            ]); // END MessageBox
                }
            });

            $('#fileUploadPersonImage').change(function (e) {
                e.preventDefault();

                var output_format = "jpeg";

                /* Load the image and compress it */
                var input = e.target;
                var reader = new FileReader();

                reader.onload = function () {
                    var dataURL = reader.result;
                    var compressOutput = document.getElementById('compressImage');
                    compressOutput.src = dataURL;
                };
                reader.readAsDataURL(input.files[0]);
                var source_image = document.getElementById('compressImage');
                var result_image = document.getElementById('canvasImage');

                var quality = parseInt(99);
                source_image.onload = function () {
                    result_image.src = imageCompressor(source_image, quality, output_format).src;
                };

                /* Selection and cropping begins here. */
                $(document).ready(function () {
                    result_image.onload = function () {
                        result_image.style.display = "block";
                        $('#panUploadPersonImagePopup').popup('close');
                        $('#cropImagePopup').popup('open');
                        $('.imgareaselect-outer').show();
                        $('.imgareaselect-selection').show();
                        $('.imgareaselect-border1').show();
                        $('.imgareaselect-border2').show();
                        $('.imgareaselect-border3').show();
                        $('.imgareaselect-border4').show();
                        $('#canvasImage').imgAreaSelect({
                            minWidth: 100,
                            minHeight: 100,
                            resizable: true,
                            show: true,
                            aspectRatio: "1:1",
                            x1: 120, y1: 90, x2: 220, y2: 190,
                            onSelectEnd: function (img, selection) {
                                x1 = selection.x1;
                                y1 = selection.y1;
                                x2 = selection.x2 - selection.x1;
                                y2 = selection.y2 - selection.y1;
                            }
                        });
                    };
                });

            });

        });
    });