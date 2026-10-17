$(function () {

    var chkNormal = $('#chkNormal');
    var chkNewSeal = $('#chkNewSeal');
    var chkSerialMismatch = $('#chkSerialMismatch');
    var chkTampered = $('#chkTampered');
    var butRefresh = $('#butRefresh')
        .click(butRefresh_Click);
    var divInput = $('#divInput');
    var divResult = $('#divResult');
    var spanCount = $('#spanCount');

    mData = mData.d;
    LoadGrid(mData);

    function LoadGrid(vData) {

        vData.Columns = {
            Reason: {
                Label: 'Reason'
            },
            LowerFuelCurrent: {
                Label: 'Fuel is lower than dipstick benchmark'
            },
            LastUpdatedCurrent: {
                Label: 'Last Updated Current',
                //Format: 'MM/dd/yyyy',
                MajorDataType: 'Date'
            },
            LastUpdatedPrevious: {
                Label: 'Last Updated Previous',
                //Format: 'MM/dd/yyyy',
                MajorDataType: 'Date'
            },
            SerialCurrent: {
                Label: 'Serial Current'
            },
            SerialPrevious: {
                Label: 'Serial Previous'
            },
            TamperedCurrent: {
                Label: 'Tampered Current'
            },
            TamperedPrevious: {
                Label: 'TamperedPrevious'
            },
            UserCurrent: {
                Label: 'Encoder Current'
            },
            UserPrevious: {
                Label: 'Encoder Previous'
            },
            Model: {
                Label: 'Equipment Model'
            },
            VehicleType: {
                Label: 'Vehicle Type'
            },
            PlateNumber: {
                Label: 'Plate Number'
            },
            DriverCurrent: {
                Label: 'Driver Current'
            },
            DriverPrevious: {
                Label: 'Driver Previous'
            },
            Crew1Current: {
                Label: 'Crew 1 Current'
            },
            Crew2Current: {
                Label: 'Crew 2 Current'
            },
            Crew3Current: {
                Label: 'Crew 3 Current'
            },
            Crew4Current: {
                Label: 'Crew 4 Current'
            },
            Crew5Current: {
                Label: 'Crew 5 Current'
            },
            Crew6Current: {
                Label: 'Crew 6 Current'
            },
            Crew1Previous: {
                Label: 'Crew 1 Previous'
            },
            Crew2Previous: {
                Label: 'Crew 2 Previous'
            },
            Crew3Previous: {
                Label: 'Crew 3 Previous'
            },
            Crew4Previous: {
                Label: 'Crew 4 Previous'
            },
            Crew5Previous: {
                Label: 'Crew 5 Previous'
            },
            Crew6Previous: {
                Label: 'Crew 6 Previous'
            },

            Checker1Current: {
                Label: 'Checker 1 Current'
            },
            Checker2Current: {
                Label: 'Checker 2 Current'
            },
            Checker3Current: {
                Label: 'Checker 3 Current'
            },

            Checker1Previous: {
                Label: 'Checker 1 Previous'
            },
            Checker2Previous: {
                Label: 'Checker 2 Previous'
            },
            Checker3Previous: {
                Label: 'Checker 3 Previous'
            }
        };
        $('#divData').ProDataGrid(vData);
        spanCount.text('Log count: ' + vData.Rows.length);

    }; // END LoadGrid

    function butRefresh_Click() {

        if (chkNormal.prop('checked') == false 
            && chkNewSeal.prop('checked') == false
            && chkSerialMismatch.prop('checked') == false
            && chkTampered.prop('checked') == false) {

            divResult
                .text('Please choose at least 1 reason to show')
                .MakeError();
            return;
        };

        divInput.block();
        var pData = {};
        if (chkNormal.prop('checked')) pData.chkNormal = 1;
        if (chkNewSeal.prop('checked')) pData.chkNewSeal = 1;
        if (chkSerialMismatch.prop('checked')) pData.chkSerialMismatch = 1;
        if (chkTampered.prop('checked')) pData.chkTampered = 1;

        AjaxSend({
            url: '/DeliveryLogger/AlertsData',
            data: pData,
            success: function(vData) {
                if (vData.d.Error) 
                    divResult
                    .text(vData.d.Error)
                    .MakeError();
                else
                    LoadGrid(vData.d);
                divInput.unblock();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                divInput.unblock();
                divResult
                    .text('An error occured while retrieving data. ' + errorThrown + ' Error: ' + XMLHttpRequest.responseText)
                    .MakeError();
            }
        }); // END AjaxSend
    }; // END butRefresh_Click
});