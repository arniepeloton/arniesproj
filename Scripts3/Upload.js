require([
    'common'
    ], function () {

    $(function () {
        GetAllDatabases();

        $('#pageDownloadError').bind('pageshow', PageShowError);

        $('#btnDone').click(function () {
            history.back();
        });

        $('#butUpload').click(function() {
            var pChecked = $('input:checked');
            var pFilenames = [];
            var pElement;

            if (pChecked.length == 0) {
                MessageBox('Please check at least 1 file to upload.');
                return;
            }

            for (var p = 0; p <= pChecked.length - 1; p++) {
                pElement = $(pChecked[p]);
                pFilenames.push(pElement.data('Filename'));
            }

            MessageBox('The selected database[s] are now being compressed. Please wait.');
            setTimeout(function() {
                Sql.UploadDatabase({
                    Filenames: pFilenames
                });
                MessageBox('The selected database[s] are now being uploaded. You may use the app while the upload is in progress but do not close or restart the app or device.');
            }, 500);
        });

    });

    function GetAllDatabases() {

        var pData = Sql.UploadableDatabaseFiles();
        pData = JSON.parse(pData);
        var pLV = $('#lstDatabaseFrame');
        var pItem;
        var pElementID;

        for (var p = 0; p <= pData.Items.length - 1; p++) {
            pItem = pData.Items[p];
            pElementID = 'chk' + NewID();
            pLV.append(
                $('<li>').append(
                    $('<input>')
                        .attr('id', pElementID)
                        .attr('type', 'checkbox')
                        .data('Filename', pItem.Filename)
                ).append(
                    $('<label>')
                        .attr('for', pElementID)
                        .text(pItem.Text)
                ).append(
                    $('<div>').css('font-weight', 'normal').text('Uncompressed file size: ' + AccountingFormat(pItem.Size, 0))
                ).append(
                    $('<div>').css('font-weight', 'normal').text('Last modified: ' + $.format.date(ParseDate(pItem.LastModified), proj.DateFormat))
                )
            );
        } // END for (var p = 0; p <= pData.Items.length - 1; p++) {
        $('input:checkbox').checkboxradio();

        try {
            pLV.listview('refresh');
        }
        catch (e) {
            pLV.listview();
        }

    }; //GetAllDatabases

    function PageShowError() {
        $('#lblPhoneName').text(Device.Name()); // device.name
        $('#lblPhoneModel').text(Device.Name()); // device.platform
        $('#lblUUID').text(Device.Serial()); // device.uuid
        $('#lblOS').text(Device.Platform()); // device.platform
        $('#lblIMEI').text(Device.Imei()); // window.pPrint.GetIMEI()

        var data = { Name: Device.Name(),
            Serial: Device.Serial(),
            Platform: Device.Platform(),
            Imei: Device.Imei()
        }

        Sql.DeleteIfExist(sessionStorage.OrgID);

        $('#dvDeviceQrCode').html('');

        var qrcode = new QRCode("dvDeviceQrCode", {
            text: JSON.stringify(data)
        });

    }

}); // END require([