require(['require'],
function () {
    $(document).ready(function () {
        $('#btnRSRemarksMessages').click(function () {
            Bs.Loading('show');
            require(['promise!/Scripts/sources/ERP/RemarksMessages/controller.js?v=' + assetversion], function () {
                Bs.Loading('hide');
                ERP.RemarksMessages.main.controller.show({ module: 'revenue' });
            })
        })

        function LoadRequiredData() {
            $('#btnRSRemarksMessages').click();
        }

        LoadRequiredData();
    })
})