$(
    function () { // First Funtion
        var div_General = $('#divGeneral');

        var div_Result = div_General.find('[name=divResult]');
        var txt_Branch = div_General.find('[name=txtBranch]');

        var btn_Browse = div_General.find('[name=btnBrowse]')
            .click(btn_Browse_Click);

        var cmb_Aisles = div_General.find('[name=cmbAisles]');
        var cmb_Store = div_General.find('[name=cmbStore]');
        var cmb_Section = div_General.find('[name=cmbSection]');
        var cmb_Shelf = div_General.find('[name=cmbShelf]');
        var cmb_EndCap = div_General.find('[name=cmbEndCap]');
        var cmb_Safety = div_General.find('[name=cmbSafety]');
        var cmb_Minimum = div_General.find('[name=cmbMinimum]');
        var cmb_Maximum = div_General.find('[name=cmbMaximum]');
        var cmb_NiceCap = div_General.find('[name=cmbNiceCap]');

        var btn_Save = div_General.find('[name=btnSave]')
            .click(btn_Save_Click);

        var var_Branch; //Find
        var var_BranchID;
        var var_Validation;

        (function () { // First Function within the First Function
            var pParsedUrl;

//            pParsedUrl = parseUri(location);
//            var_BranchID = pParsedUrl.queryKey.branchid;

            //GetData();

            function InitValidation() {
                var_Validation = new Validator;
                var_Validation.SummaryDiv = div_Result;

                var_Validation.AddItem({
                    Label: 'Branch Name',
                    Control: txt_Branch,
                    ErrorControl: null
                }).AddRequiredValidation();
            }; // End InitValidation Function
        })(); // End First Function within the First Function

        function btn_Browse_Click() {

            if (var_Branch == null)
                var_Branch = new Finder({
                    Title: 'Branch Library',
                    Url: '/Planograms/Search',
                    Columns: {
                        'Name': {
                            'Label': 'Branch Name'
                        },
                        'Address': {
                            'Label': 'Address'
                        }
                    },
                    OnOk: function (vData) {
                        txt_Branch.val(vData.Name);
                        var_BranchID = vData.ID;
                        //GetData();
                    }
                });
            var_Branch.ShowDialog();
        }; // END btn_Browse_Click Function

//        function GetData() {

//            AjaxSend({
//                url: '/Planogram/GetData',
//                data: {ID : var_BranchID },
//                success: function (vData) {
//                    div_General.unblock();
//                    if (HandleSessionExpired(vData) == false) return;
//                    if (vData.d.Error)
//                        div_Result.text(vData.d.Error).MakeError();
//                    else
//                        LoadData(vData.d);
//                },
//                error: function (XMLHttpRequest, textStatus, errorThrown) {
//                    div_General.unblock();
//                    div_Result.text(XMLHttpRequest.responseText).MakeError();
//                }
//            });

//        }; // END GetData

//        function LoadData(vData) {

//            txt_Branch.text(vData.Name);
//            var_BranchID = vData.ID;
//                    txtLeadTime.val(vData.LeadTime);
//                    txtLotSize.val(vData.LotSize);
//                    cbLotSize.val(vData.LotSizeUnit);
//                    txtMoq.val(vData.MinimumOrderQty);
//                    cbMoq.val(vData.MinimumOrderUnit);

//            $.uniform.update();

//        }; // END LoadData

        function btn_Save_Click() {

            if (var_Validation.Validate() == false) return;

            div_General.block();
            div_Result.hide();
            AjaxSend({
                url: '/Planogram/Save',
                data: {
                    ID : var_BranchID,
                    txt_Branch : txt_Branch.val(),
                    cmb_Aisles : cmb_Aisles.val(),
                    cmb_Store : cmb_Store.val(),
                    cmb_Section : cmb_Section.val(),
                    cmb_Shelf : cmb_Shelf.val(),
                    cmb_EndCap : cmb_EndCap.val(),
                    cmb_Safety : cmb_Safety.val(),
                    cmb_Minimum : cmb_Minimum.val(),
                    cmb_Maximum: cmb_Maximum.val(),
                    cmb_NiceCap : cmb_NiceCap.val()
                },
                success: function (vData) {
                    div_General.unblock();
                    if (HandleSessionExpired(vData) == false) return;
                    if (vData.d.Error)
                        div_Result.text(vData.d.Error).MakeError();
                    else
                        div_Result.text('Success').MakeHighlight();
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    div_General.unblock();
                    div_Result.text(XMLHttpRequest.responseText).MakeError();
                }
            });
        }; // END butSave_Click
    } // End First Function
);
        

