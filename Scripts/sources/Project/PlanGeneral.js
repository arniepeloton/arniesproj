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

        (function () {

            InitValidation();

            function InitValidation() {
                var_Validation = new Validator;
                var_Validation.SummaryDiv = div_Result;

                var_Validation.AddItem({
                    Label: 'Branch Name',
                    Control: txt_Branch,
                    ErrorControl: null
                }).AddRequiredValidation();
            };
        })();

        function btn_Browse_Click() {

            if (var_Branch == null)
                var_Branch = new Finder({
                    Title: 'Branch Library',
                    Url: '/Branch/Search',
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
                    }
                });
            var_Branch.ShowDialog();
        };

        function get_Branch() {

            AjaxSend({
                url: '/Supplier/get_Branch',
                data: { ID: var_BranchID },
                success: function (var_Data) {
                    div_General.unblock();
                    if (HandleSessionExpired(var_Data) == false) return;
                    if (var_Data.d.Error)
                        div_Result.text(var_Data.d.Error).MakeError();
                    else
                        LoadData(vData.d);
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    div_General.unblock();
                    div_Result.text(XMLHttpRequest.responseText).MakeError();
                }
            });

        }; // END GetData

        function load_Branch(var_Data) {
            alert('NoOfAisles: ' + var_Data.NoOfAisles);
            cmb_Aisles.val(var_Data.NoOfAisles);
            cmb_Store.val(var_Data.TotalOfAisles);
            cmb_Section.val(var_Data.Sections);
            cmb_Shelf.val(var_Data.Shelves);
            cmb_EndCap.val(var_Data.EndCaps);
            cmb_Safety.val(var_Data.SafetyStocks);
            cmb_Minimum.val(var_Data.Minimums);
            cmb_Maximum.val(var_Data.Maximums);
            cmb_NiceCap.val(var_Data.NiceCaps);

            $.uniform.update();

        }; // END LoadData

        function btn_Save_Click() {

            if (var_Validation.Validate() == false) return;

            div_General.block();
            div_Result.hide();
            AjaxSend({
                url: '/Planogram/Save',
                data: {
                    ID: var_BranchID,
                    txt_Branch: txt_Branch.val(),
                    cmb_Aisles: cmb_Aisles.val(),
                    cmb_Store: cmb_Store.val(),
                    cmb_Section: cmb_Section.val(),
                    cmb_Shelf: cmb_Shelf.val(),
                    cmb_EndCap: cmb_EndCap.val(),
                    cmb_Safety: cmb_Safety.val(),
                    cmb_Minimum: cmb_Minimum.val(),
                    cmb_Maximum: cmb_Maximum.val(),
                    cmb_NiceCap: cmb_NiceCap.val()
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
        };
    }
);
        

