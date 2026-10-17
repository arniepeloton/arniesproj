$(
    function () { // First Funtion
        var div_Gondola = $('#divGondola');

        var span_ItemDescription = div_Gondola.find('[name=spanItemDescription]');
        var btn_Browse_Item = div_Gondola.find('[name=btnBrowseItem]')
            .click(btn_Browse_Item_Click);

        var span_Branch = div_Gondola.find('[name=spanBranch]');
        var btn_Browse_Branch = div_Gondola.find('[name=btnBrowseBranch]')
            .click(btn_Browse_Branch_Click);

        var var_Item; //Find
        var var_Unit;
        var var_Branch;

        var var_ItemID;
        var var_UnitID;
        var var_BranchID;

        var var_Validation;

        function btn_Browse_Item_Click() {

            if (var_Item == null)
                var_Item = new Finder({
                    Title: 'Stock Information',
                    Url: '/StockInformation/Search',
                    Columns: {
                        'StockNumber': {
                            'Label': 'Stock Number'
                        },
                        'StockDescription': {
                            'Label': 'Item Description'
                        }
                    },
                    OnOk: function (var_Data) {
                        span_ItemDescription.text(var_Data.StockDescription);
                        var_ItemID = var_Data.ID;
                        get_Item();
                    }
                });
            var_Item.ShowDialog();
        }; // END btn_Browse_Click Function

        function btn_Browse_Branch_Click() {

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
                    OnOk: function (var_Data) {
                        span_Branch.text(var_Data.Name);
                        var_BranchID = var_Data.ID;
                    }
                });
            var_Branch.ShowDialog();
        }; // END btn_Browse_Click Function

        function get_Item() {

            AjaxSend({
                url: '/Planogram/get_Item',
                data: { ID: var_ItemID },
                success: function (var_Data) {
                    div_Gondola.unblock();
                    if (HandleSessionExpired(var_Data) == false) return;
                    if (var_Data.d.Error)
                        div_Result.text(var_Data.d.Error).MakeError();
                    else
                        load_Unit(var_Data.d);
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    div_Gondola.unblock();
                    div_Result.text(XMLHttpRequest.responseText).MakeError();
                }
            });

        }; // END GetData

        function load_Unit(var_Data) {

            var var_Text = "<select id='optValue'> " +
                           "    <option> -Select Value-</option> ";
            $.each(var_Data.UnitMeasure.Rows, function (key, val) {
                var_Text += "<option value='" + val.UnitMeasure + "'> " + val.UnitMeasure + " </option> "
            });
            var_Text += "</select>";

            $("#divUnitMeasure").html(var_Text);

        }; // END LoadData

    } // End First Function
);
        

