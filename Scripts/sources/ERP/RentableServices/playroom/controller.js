define([
    //'promise!/Scripts/sources/ERP/AssetsManagement/dashboard/model.js'
    'moment'
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/RentableServices/playroom/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.RentableServices.playroom.controller', null, function () {

        var mOut = {}, mElement, ctrChildren = 1, cntChildren = 0;

        mElement = $(mHtml);
        mOut.element = mElement;        

        $('#tabRentableServicesPlayRoom').empty().append(mElement);        

        var imgRepresentativePic = mElement.find('#imgRepresentativePic');
        var txtPRRepresentativeName = mElement.find('#txtPRRepresentativeName');
        var txtPRReprensentativeContactNo = mElement.find('#txtPRReprensentativeContactNo');
        var txtPRNoOfChildren = mElement.find('#txtPRNoOfChildren');
        var txtPRNoOfAdults = mElement.find('#txtPRNoOfAdults');
        var txtPREnterTime = mElement.find('#txtPREnterTime');
        var txtPRExitTime = mElement.find('#txtPRExitTime');
        var txtPRDurationTime = mElement.find('#txtPRDurationTime');
        var txtPRBagLocker = mElement.find('#txtPRBagLocker');
        var txtPRShoeLocker = mElement.find('#txtPRShoeLocker');
        var txtPRReceptionist = mElement.find('#txtPRReceptionist');
        var txtPRTotalAmount = mElement.find('#txtPRTotalAmount');
        var txtPRORNo = mElement.find('#txtPRORNo');
        var PersonID = '', TransID = '', MobileID = '';
        var arrPic = [], timeIntervalEnterTime, timeIntervalExitTime, curTrans = null;
        var divListOfPlayers = $('#divListOfPlayers');

        mElement.find('#btnPRRegister').click(function () {
            //PopupButtonList('', 'Registration Type', [
            //    {
            //        Text: 'New Customer', Click: function () {
                        NewCustomer();
            //        }
            //    }, {
            //        Text: 'Existing Customer', Click: function () {                        
            //            BrowsePopup('LibraryOrganizationCustomer', 'Search Customer', [
            //                {
            //                    DisplayText: [{ Column: 'CustomerName' }],
            //                    Click: function (e) {
            //                        Bs.Loading('show');
            //                        NewCustomer();
            //                        AjaxSend({
            //                            url: '/PlayRoom/GetCustomerInfo',
            //                            data: { PeopleID: e.ID },
            //                            success: function (v) {
            //                                Bs.Loading('hide');
            //                                var vRep = v.Representative, vChild = v.Children.Rows;
            //                                PersonID = e.ID;
            //                                MobileID = vRep.MobileID;
            //                                txtPRRepresentativeName.val(vRep.RepresentativeName);
            //                                txtPRReprensentativeContactNo.val(vRep.RepresentiveContactNo);

            //                                if (CStr2(vRep.ProfilePicture) != '') {
            //                                    imgRepresentativePic.attr('src', '/Content/MobizPeople/' + vRep.ProfilePicture);
            //                                }

            //                                if (vChild.length > 0) {
            //                                    $('#tbodyChildren').empty();
            //                                    ctrChildren = 1;
            //                                    for (var i = 0 ; i < vChild.length; i++) {
            //                                        var tr = AddChildren();
            //                                        vChild[i].Action = 'new';
            //                                        tr.data('data', vChild[i]);
            //                                        tr.find('[name="txtChildFullname"]').val(vChild[i].FullName);
            //                                        tr.find('[name="txtChildAge"]').val(vChild[i].Age);
            //                                        tr.find('[name="selChildGender"]').val(vChild[i].Gender);

            //                                        if (CStr2(vChild[i].ProfilePicture) != '') {
            //                                            tr.find('[name="imgChildPic"]').attr('src', '/Content/MobizPeople/' + vChild[i].ProfilePicture);
            //                                        }
            //                                    }
            //                                }
            //                            }
            //                        })
            //                    }
            //                }
            //            ], { FilterBy: 'Person' })
            //        }
            //    }
            //])
        })

        mElement.find('#btnPRAddChildren').click(function () {
            AddChildren();
        })

        mElement.find('#imgRepresentativePic').click(function () {
            TakePic($(this))
        })

        txtPRReprensentativeContactNo.keydown(function (e) {
            if (!NumberOnly(e)) {
                return;
            }
        })

        mElement.find('#btnPRSave').click(function () {
            Save();
        })

        mElement.find('#txtSearchPlayers').keydown(function (e) {
            if (e.which == 13) {
                divListOfPlayers.find('.players').each(function () {
                    $(this).data('data').IsDone = true;
                })
                GetPlayers();
            }
        })

        txtPREnterTime.click(function () {
            CustomSetTime({
                Click: function () {

                }
            })
        })

        function NewCustomer() {
            ClearRegisterFields();
            AddChildren();
            $('#popupPRRegister').modal('show');

            timeIntervalEnterTime = setInterval(function () {
                var curDate = new Date();
                txtPREnterTime.val($.format.date(new Date(curDate.setSeconds(curDate.getSeconds())), 'HH:mm'));
            }, 1000);

            timeIntervalExitTime = setInterval(function () {
                var curDate = new Date(), exitDate = new Date('1/1/1 ' + txtPRExitTime.val());
                curDate.setHours(exitDate.getHours())
                curDate.setMinutes(exitDate.getMinutes());
                curDate.setSeconds(curDate.getSeconds() + 1);
                txtPRExitTime.val($.format.date(curDate, 'HH:mm'));
            }, 1000)

            mElement.find('#btnPRSave').parent().show();
        }

        function Save() {
            var tbodyChildren = mElement.find('#tbodyChildren tr');
            var isValid = true, arrChildren = [], piccode, invalidmsg = '';
            var curSeconds = $.format.date(new Date(), 'ss');

            mElement.find('input').removeClass('is-invalid');

            if (txtPRRepresentativeName.val().trim() == '') {
                isValid = false;
                txtPRRepresentativeName.addClass('is-invalid');
            }

            if (txtPRReprensentativeContactNo.val().trim() == '') {
                isValid = false;
                txtPRReprensentativeContactNo.addClass('is-invalid');
            } else {
                if (txtPRReprensentativeContactNo.val() < 11) {
                    isValid = false;
                    invalidmsg += '- Enter 11 digit mobile number. <br>';
                }

                if (txtPRReprensentativeContactNo.val().substr(0, 2) != '09') {
                    isValid = false;
                    invalidmsg += '- Contact number must begin with 09. <br>';
                }
            }

            if (txtPRNoOfChildren.val().trim() == '') {
                isValid = false;
                txtPRNoOfChildren.addClass('is-invalid');
            }

            if (txtPRNoOfAdults.val().trim() == '') {
                isValid = false;
                txtPRNoOfAdults.addClass('is-invalid');
            }

            if (txtPREnterTime.val().trim() == '') {
                isValid = false;
                txtPREnterTime.addClass('is-invalid');
            }

            if (txtPRExitTime.val().trim() == '') {
                isValid = false;
                txtPRExitTime.addClass('is-invalid');
            } else {
                if (TransID == '' && !(new Date('1/1/2022 ' + txtPRExitTime.val()) > new Date('1/1/2022 ' + txtPREnterTime.val()))) {
                    isValid = false;
                    txtPRExitTime.addClass('is-invalid');
                    invalidmsg += '- Invalid time duration. <br>';
                }
            }

            if (txtPRBagLocker.val().trim() == '') {
                isValid = false;
                txtPRBagLocker.addClass('is-invalid');
            }

            if (txtPRShoeLocker.val().trim() == '') {
                isValid = false;
                txtPRShoeLocker.addClass('is-invalid');
            }

            //if (txtPRTotalAmount.val().trim() == '') {
            //    isValid = false;
            //    txtPRTotalAmount.addClass('is-invalid');
            //}

            //if (txtPRORNo.val().trim() == '') {
            //    isValid = false;
            //    txtPRORNo.addClass('is-invalid');
            //}

            if (tbodyChildren.length > 0) {
                tbodyChildren.each(function () {
                    var dis = $(this);
                    var data = dis.data('data');
                    var imgChildPic = dis.find('[name="imgChildPic"]');
                    var txtChildFullname = dis.find('[name="txtChildFullname"]');
                    var txtChildAge = dis.find('[name="txtChildAge"]');
                    var selChildGender = dis.find('[name="selChildGender"]');

                    if (txtChildFullname.val().trim() == '') {
                        isValid = false;
                        txtChildFullname.addClass('is-invalid');
                    }

                    if (txtChildAge.val().trim() == '') {
                        isValid = false;
                        txtChildAge.addClass('is-invalid');
                    }

                    if (selChildGender.val().trim() == '') {
                        isValid = false;
                        selChildGender.addClass('is-invalid');
                    }

                    piccode = NewID();
                    arrChildren.push({
                        //Picture: CBool(imgChildPic.attr('takepic')) ? imgChildPic.attr('src').replace('data:image/jpeg;base64,', '') : '',
                        PicCode: piccode,
                        ID: (data == null ? '' : data.ID),
                        PeopleID: (data == null ? '' : data.PeopleID),
                        FullName: txtChildFullname.val(),
                        Age: txtChildAge.val(),
                        Gender: selChildGender.val(),
                        Action: (data == null ? 'new' : data.Action)
                    })

                    if (CBool(imgChildPic.attr('takepic'))) {
                        arrPic.push({ PicCode: piccode, imgData: imgChildPic.attr('src').replace('data:image/jpeg;base64,', '') })
                    }
                })
            } else {
                isValid = false;
            }

            if (arrChildren.length == 0) {
                isValid = false;
                invalidmsg += '- Add at least one child. <br>';
            }

            if (invalidmsg != '') invalidmsg = 'See the following errors: <br><br>' + invalidmsg;

            if (!isValid) {
                NewMessageBox('Please fill out all fields marked with an <span class="required">*</span>. ' + invalidmsg);
                return;
            }

            piccode = NewID();
            var data = {
                //Picture: CBool(imgRepresentativePic.attr('takepic')) ?  : '',
                PicCode: piccode,
                TransID: TransID,
                PersonID: PersonID,
                MobileID: MobileID,
                RepresentiveName: txtPRRepresentativeName.val(),
                RepresentiveContactNo: txtPRReprensentativeContactNo.val(),
                TotalNoOfChildren: txtPRNoOfChildren.val(),
                TotalNoOfAdult: txtPRNoOfAdults.val(),
                EnterTime: txtPREnterTime.val() + (CStr2(TransID) == '' ? ':' + curSeconds : ''), //if new reg, add seconds para tama ang countdown
                ExitTime: txtPRExitTime.val() + (CStr2(TransID) == '' ? ':' + curSeconds : ''),
                TotalDuration: txtPRDurationTime.val(),
                BagLocker: txtPRBagLocker.val(),
                ShoeLocker: txtPRShoeLocker.val(),
                TotalAmount: txtPRTotalAmount.val(),
                ORNo: txtPRORNo.val(),
                Receptionist: sessionStorage.PeopleID,
                Children: arrChildren,
            }

            if (CBool(imgRepresentativePic.attr('takepic'))) {
                arrPic.push({ PicCode: piccode, imgData: imgRepresentativePic.attr('src').replace('data:image/jpeg;base64,', '') })
            }

            Bs.Loading('show', 'Saving ...');
            AjaxSend({
                url: '/PlayRoom/Save',
                data: { data: data },
                success: function (e) {
                    Bs.Loading('hide');
                    if (e.Results == 'saved') {
                        if (arrPic.length > 0) {
                            for (var i = 0; i < arrPic.length; i++) {
                                for (var p = 0; p < e.PeopleID.length; p++) {
                                    var s = e.PeopleID[p].split('|');
                                    var code = s[0], pid = s[1];

                                    if (code == arrPic[i].PicCode) arrPic[i].PeopleID = pid;
                                }
                            }
                            SavePic();
                        } else {
                            GetPlayers();
                            $('#popupPRRegister').modal('hide');
                        }
                    } else {
                        NewMessageBox(e.Results, 'Error');
                    }
                }
            })
        }

        var ctrSavePic = 0;
        function SavePic() {
            function Go(index) {
                Bs.Loading('show', 'Uploading image(s)');
                AjaxSend({
                    url: '/PlayRoom/SavePic',
                    data: {
                        PeopleID: arrPic[index].PeopleID,
                        imgData: arrPic[index].imgData
                    }, success: function (e) {
                        Bs.Loading('hide');
                        if (e.Results != 'success') {
                            NewMessageBox(e.Results, 'Error when saving picture');
                            return;
                        }

                        if (arrPic.length == (ctrSavePic + 1)) {
                            ctrSavePic = 0;
                            GetPlayers();
                            $('#popupPRRegister').modal('hide');
                        } else {
                            ctrSavePic += 1;
                            Go(ctrSavePic)
                        }
                    }
                })
            }

            Go(ctrSavePic);
        }
        
        mOut.GetPlayers = GetPlayers;
        function GetPlayers() {            
            divListOfPlayers.empty();
            AjaxSend({
                url: '/PlayRoom/GetPlayers',
                data: { searchkey: $.trim(mElement.find('#txtSearchPlayers').val()) },
                success: function (e) {
                    var vRows = e.Results.Rows;

                    if (vRows.length > 0) {
                        for (var i = 0; i < vRows.length; i++) {
                            var pData = vRows[i];
                            var divPlayer = $('<div class="w-100 h-100 p-2 players">')
                                            .data('data', pData)
                                            .append($('<div class="row">')
                                                .append($('<div class="col-sm-12 col-md-12 col-lg-auto text-center align-self-center">')
                                                    .append('<img class="img-thumbnail" name="displayPic" src="/Content/images/NoPhoto.jpg" style="width:100px; height:100px; object-fit:cover;" />')
                                                )
                                                .append($('<div class="col-sm-12 col-md-12 col-lg text-dark">')
                                                    .append('<div class="text-center text-lg-left mt-2 mt-lg-0 mb-2 font-weight-bold">' + pData.FullName + '</div>')
                                                    .append('En. Time: ' + $.format.date(new Date('1/1/2000 ' + pData.EnterTime), 'hh:mm a') + '<br>')
                                                    .append('Ex. Time: <span name="spanExitTime">' + $.format.date(new Date('1/1/2000 ' + pData.ExitTime), 'hh:mm a') + '</span><br>')
                                                    .append('Remaining: <span name="spanRemainingTime"></span>')
                                                )
                                            )
                                            .click(function () {
                                                var dis = $(this), data = dis.data('data'), msg = '<div class="row mb-3">';

                                                if (CStr2(data.RepresentativePicture) != '') {
                                                    msg += '<div class="col-auto">' +
                                                                '<img class="img-thumbnail" src="/Content/MobizPeople/' + data.RepresentativePicture + '" style="width:100px; height:100px; object-fit:cover;" />' +
                                                            '</div>'
                                                } else {
                                                    msg += '<div class="col-auto">' +
                                                                '<img class="img-thumbnail" src="/Content/images/NoPhoto.jpg" style="width:100px; height:100px; object-fit:cover;" />' +
                                                            '</div>'
                                                }

                                                msg += '<div class="col-auto">' +
                                                            'Representative Name: <br> <b>' + data.RepresentativeName + '</b>' +
                                                        '</div>'
                                                msg += '</div>'

                                                msg += '<div class="row">';

                                                if (CStr2(data.ProfilePicture) != '') {
                                                    msg += '<div class="col-auto">' +
                                                                '<img class="img-thumbnail" src="/Content/MobizPeople/' + data.ProfilePicture + '" style="width:100px; height:100px; object-fit:cover;" />' +
                                                            '</div>'
                                                } else {
                                                    msg += '<div class="col-auto">' +
                                                                '<img class="img-thumbnail" src="/Content/images/NoPhoto.jpg" style="width:100px; height:100px; object-fit:cover;" />' +
                                                            '</div>'
                                                }

                                                msg += '<div class="col-auto">' +
                                                            'Child Name: <br> <b>' + data.FullName + '</b>' +
                                                        '</div>'
                                                msg += '</div>'

                                                PopupButtonList(msg, 'Options', [
                                                    {
                                                        Text: 'Edit Details', Click: function () {
                                                            mOut.GetDetails(data);
                                                            mElement.find('#btnPRSave').parent().show();
                                                        }
                                                    }, {
                                                        Text: 'Extend Time', Click: function () {
                                                            mElement.find('[name="lblPlayerName"]').text(data.FullName);
                                                            //mElement.find('#txtExtendExitTime').val($.format.date(new Date('1/1/2000 ' + data.ExitTime), 'HH:mm'));
                                                            $('#popupPRExtendTime').modal('show');

                                                            mElement.find('#btnPRExtendSave').unbind('click').click(function () {
                                                                Bs.Loading('show', 'Extending ...');
                                                                var exitTime = new Date('1/1/2022 ' + data.ExitTime);
                                                                var newExitTime = new Date(exitTime.setMinutes(exitTime.getMinutes() + CInt2(mElement.find('#txtExtendExitTime').val())));
                                                                newExitTime = $.format.date(newExitTime, 'HH:mm:ss');
                                                                AjaxSend({
                                                                    url: '/PlayRoom/ExtendTime',
                                                                    data: {
                                                                        ChildTransID: data.ID,
                                                                        ExtendedTime: newExitTime
                                                                    }, success: function (e) {
                                                                        Bs.Loading('hide');
                                                                        $('#popupPRExtendTime').modal('hide');
                                                                        if (e.Results == 'extended') {
                                                                            WebToast('Exit time for ' + data.FullName + ' has been extended', 'top');
                                                                            data.ExitTime = newExitTime;
                                                                            dis.find('[name="spanExitTime"]').text($.format.date(new Date('1/1/2000 ' + newExitTime), 'hh:mm a'));

                                                                            if (data.IsDone) {
                                                                                data.PlayNotif = false;
                                                                                data.IsDone = false;
                                                                                StartTimer(dis);
                                                                            }
                                                                        } else {
                                                                            NewMessageBox(e.Results, 'Error');
                                                                        }
                                                                    }
                                                                })
                                                            })
                                                        }
                                                    }, {
                                                        Text: 'End Now', Click: function () {
                                                            var exitDate = new Date($.format.date(new Date(), 'yyyy-MM-dd ' + data.ExitTime + ':ss'));
                                                            var enterDate = new Date($.format.date(new Date(), 'yyyy-MM-dd ' + data.EnterTime + ':00'));
                                                            var setDuration = moment(exitDate, "YYYY-MM-DD HH:mm:ss").diff(moment(enterDate, "YYYY-MM-DD HH:mm:ss"));
                                                            var intendedDuration = moment.utc(setDuration).format("HH:mm:ss");
                                                            var actualDuration = moment.utc(moment(new Date(), "YYYY-MM-DD HH:mm:ss").diff(moment(enterDate, "YYYY-MM-DD HH:mm:ss"))).format("HH:mm:ss")

                                                            NewMessageBox('Are you sure you want to continue?', 'Confirmation', [
                                                                {
                                                                    Text: 'Yes', Click: function () {
                                                                        Bs.Loading('show');

                                                                        AjaxSend({
                                                                            url: '/PlayRoom/EndPlay',
                                                                            data: {
                                                                                PlayRoomID: data.PlayRoomID,
                                                                                ChildTransID: data.ID,
                                                                                TotalDuration: intendedDuration,
                                                                                ActualTotalDuration: actualDuration
                                                                            }, success: function (e) {
                                                                                Bs.Loading('hide');
                                                                                if (e.Results == 'ended') {
                                                                                    dis.parent().remove();
                                                                                } else {
                                                                                    NewMessageBox(e.Results, 'Error');
                                                                                }
                                                                            }
                                                                        })
                                                                    }
                                                                }, { Text: 'No' }
                                                            ])
                                                        }
                                                    }
                                                ])
                                            })
                            divListOfPlayers.append($('<div class="col-6 col-sm-4 col-md-4 col-xl-3 mb-3">')
                                    .append(divPlayer)
                                )

                            if (CStr2(pData.ProfilePicture) != '') {
                                divPlayer.find('[name="displayPic"]').attr('src', '/Content/MobizPeople/' + pData.ProfilePicture);
                            }

                            StartTimer(divPlayer);
                        }
                    } else {
                        divListOfPlayers.append('<div class="mt-5 text-center text-secondary"><div class="bi bi-joystick" style="font-size:70px;"></div><div class="mt-2" style="font-size:20px;">No result(s) found</div></div>');
                    }
                }
            })
        }

        mOut.GetDetails = function (data) {
            Bs.Loading('show');
            AjaxSend({
                url: '/PlayRoom/GetPlayersInfo',
                data: { TransID: data.PlayRoomID },
                success: function (e) {
                    Bs.Loading('hide');
                    ClearRegisterFields();
                    curTrans = e.Trans
                    var child = e.Children.Rows;
                    TransID = curTrans.ID
                    PersonID = curTrans.PeopleID;
                    MobileID = CStr2(curTrans.MobileID)
                    txtPRRepresentativeName.val(curTrans.RepresentativeName);
                    txtPRReprensentativeContactNo.val(CStr2(curTrans.RepresentiveContactNo));
                    txtPRNoOfChildren.val(curTrans.TotalNoOfChildren);
                    txtPRNoOfAdults.val(curTrans.TotalNoOfAdult);
                    txtPREnterTime.val(curTrans.EnterTime);
                    txtPRExitTime.attr('readonly', 1).val(curTrans.ExitTime);
                    //txtPRDurationTime.val();
                    txtPRBagLocker.val(curTrans.BagLocker);
                    txtPRShoeLocker.val(curTrans.ShoeLocker);
                    txtPRReceptionist.val(curTrans.ReceptionistName);

                    if (CStr2(curTrans.ProfilePicture) != '') {
                        $('#imgRepresentativePic').attr('src', '/Content/MobizPeople/' + curTrans.ProfilePicture);
                    }

                    for (var i = 0 ; i < child.length; i++) {
                        var tr = AddChildren();
                        child[i].Action = 'update';
                        tr.data('data', child[i]);
                        tr.find('[name="txtChildFullname"]').val(child[i].FullName);
                        tr.find('[name="txtChildAge"]').val(child[i].Age);
                        tr.find('[name="selChildGender"]').val(child[i].Gender);

                        if (CStr2(child[i].ProfilePicture) != '') {
                            tr.find('[name="imgChildPic"]').attr('src', '/Content/MobizPeople/' + child[i].ProfilePicture);
                        }
                    }

                    $('#popupPRRegister').modal('show');
                }
            })
        }

        function StartTimer(dis) {
            var timer = setInterval(function (dis) {
                var data = dis.data('data');

                if (data == undefined) {
                    clearInterval(timer);
                    return;
                }

                var container = dis.find('[name="spanRemainingTime"]');
                var exitTime = data.ExitTime;
                var curDate = new Date()
                var cYear = curDate.getFullYear()
                var cMonth = Number(curDate.getMonth() + 1)
                var cDate = curDate.getDate()
                var sT = new Date(cYear + "-" + cMonth + "-" + cDate + " " + data.EnterTime);
                var eT = new Date(cYear + "-" + cMonth + "-" + cDate + " " + exitTime);
                var timeSpan;

                timeSpan = eT.getTime() - sT.getTime()
                timeSpan = timeSpan - (curDate.getTime() - sT.getTime())

                var hours = Math.floor(timeSpan / (1000 * 60 * 60))
                var minutes = Math.floor((timeSpan % (1000 * 60 * 60)) / (1000 * 60))
                var seconds = Math.floor(((timeSpan % (1000 * 60 * 60)) % (1000 * 60)) / 1000)
                var countDownOutput = ('0' + hours).slice(-2) + ":" + ('0' + minutes).slice(-2) + ":" + ('0' + seconds).slice(-2);

                if (timeSpan < 1000 ||
                    $.format.date(new Date(data.PlayDate), 'MM-dd-yyyy') != $.format.date(curDate, 'MM-dd-yyyy')) {
                    container.text('-')
                    data.IsDone = true;
                    clearInterval(timer)
                    dis.removeClass('players-warning').addClass('players-done');
                    new Audio('/Content/sounds/notif.mp3').play();
                } else {
                    container.text(countDownOutput);
                    dis.removeClass('players-warning players-done')
                    if (hours == 0 && minutes <= 14) {
                        if (!CBool(data.PlayNotif)) {
                            data.PlayNotif = true;
                            new Audio('/Content/sounds/notif.mp3').play();
                        }                        
                        dis.addClass('players-warning');
                    }
                }
            }, 1000, dis)           
        }

        function TakePic(imgholder) {
            var fileTakePic = $('#fileTakePic');
            var reader = new FileReader();
            fileTakePic.val('').click();

            fileTakePic.unbind('change').change(function () {
                reader.onload = function () {
                    var dataURL = reader.result;
                    var image = new Image();
                    image.src = dataURL;
                    //preview.appendChild(image); // preview commented out, I am using the canvas instead
                    image.onload = function () {
                        // have to wait till it's loaded
                        var canvas = document.createElement('canvas');

                        var width = image.width;
                        var height = image.height;

                        // resize the canvas and draw the image data into it
                        canvas.width = width;
                        canvas.height = height;
                        var ctx = canvas.getContext("2d");
                        ctx.drawImage(image, 0, 0, width, height);

                        imgholder.attr({
                            src: canvas.toDataURL("image/jpeg", 0.5),
                            takepic: 1
                        });
                    }
                };
                reader.readAsDataURL($(this)[0].files[0]);
            })
        }

        function AddChildren() {
            var tr = $('<tr>')
                        .data('data', null)
                        .append($('<td>').append($('<img class="img-thumbnail" name="imgChildPic" takepic="0" updatepic="0" src="/Content/images/NoPhoto.jpg" style="width:100px; height:100px; object-fit:cover;" />')
                                .click(function () {
                                    TakePic($(this));
                                })
                            ))
                        .append($('<td>').append($('<input data-role="none" type="text" name="txtChildFullname" class="form-control" />')
                                .blur(function () {
                                    var val = $(this).val();

                                    if (val == '') {
                                        cntChildren += 1;
                                    } else {
                                        cntChildren -= 1;
                                    }

                                    //txtPRNoOfChildren.val(cntChildren);
                                })
                            ))
                        .append($('<td>').append($('<input data-role="none" type="number" name="txtChildAge" class="form-control text-center" />')))
                        .append($('<td class="text-center">').append($('<select name="selChildGender" class="form-select">')
                                .append('<option value="">Pls. Select</option>')
                                .append('<option value="Male">Boy</option>')
                                .append('<option value="Female">Girl</option>')
                            ))
                        .append($('<td>').append($('<button data-role="none" class="btn btn-danger">')
                                .append('<span class="bi bi-trash"></span>')
                                .click(function () {
                                    var dis = $(this), tr = dis.parents().closest('tr'), data = tr.data('data');

                                    NewMessageBox('Are you sure you want to remove this record?', 'Confirmation', [
                                        {
                                            Text: 'Yes', Click: function () {
                                                txtPRNoOfChildren.val(CInt2(txtPRNoOfChildren.val()) - 1);
                                                if (data == null) {
                                                    tr.remove();
                                                } else {
                                                    data.Action = 'remove';
                                                    tr.hide();
                                                }
                                            }
                                        }, { Text: 'No' }
                                    ])
                                })
                            ))

            $('#tbodyChildren').append(tr);
            txtPRNoOfChildren.val(ctrChildren);
            ctrChildren += 1;
            return tr;
        }

        function ClearRegisterFields() {
            TransID = '';
            PersonID = '';
            MobileID = '';
            arrPic = [];
            curTrans = null;
            ctrChildren = 1;
            clearInterval(timeIntervalEnterTime);
            clearInterval(timeIntervalExitTime);
            $('#popupPRRegister input').val('');
            $('#popupPRRegister tbody').empty();
            $('#imgRepresentativePic').attr({
                src: '/Content/images/NoPhoto.jpg',
                takepic: 0,
                updatepic: 0
            });
            txtPREnterTime.val($.format.date(new Date(), 'HH:mm'));
            txtPRExitTime.removeAttr('readonly').val($.format.date(new Date(new Date().setHours(new Date().getHours() + 1)), 'HH:mm'));
            txtPRReceptionist.val(sessionStorage.CurrentUserFullName);            
        }

        mOut.Initialize = function () {
            $('#popupPRRegister').appendTo('#divRentableServicesMain');
        }

        mOut.Initialize();

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});