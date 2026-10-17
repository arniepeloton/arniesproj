<script>
    require([GetPathFormat('/Scripts/sources/Decimals/Decimals.js?v=' + assetversion)]);
</script>

<div class="container-fluid">
    <div class="row mb-3">
        <label class="col-sm-2 mb-3 col-form-label" for="txtRegularDiscount3">Allow Decimals in Qty:</label>
        <div class="col-sm-10">
            <div class="form-check form-switch mb-3" style="font-size:25px;">
                <input class="form-check-input" data-role="none" type="checkbox" id="selSlider">
                <label class="form-check-label" for="selSlider"></label>
            </div>

            <div id="dvIsAllowDecimal" style="visibility:hidden; display:none">
                <div class="row mb-3">
                    <div class="col-offset-2 col-sm-10">
                        <div class="form-check mb-3">
                            <input class="form-check-input" data-role="none" name="rcDecimals" value="choice-1" type="radio" id="rcAllowAllDecimals" checked>
                            <label class="form-check-label" for="rcAllowAllDecimals">Allow decimal inputs to all products</label>
                        </div>

                        <div class="form-check mb-3">
                            <input class="form-check-input" data-role="none" name="rcDecimals" value="choice-2" type="radio" id="rcAllowOnSelected">
                            <label class="form-check-label" for="rcAllowOnSelected">Allow decimal inputs on selected categories</label>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>    

    <div id="dvIsAllowOnSelectedCategories" style="visibility:hidden; display:none; max-height:300px; overflow:auto;" >
        <table data-role="none" class="table table-striped table-hover">
            <thead>
                <tr>
                    <th style="width:300px;">
                        Category
                    </th>

                    <th>
                        Allow Decimal in Qty Inputs?
                    </th>
                </tr>
            </thead>
            <tbody id="tbCategories"></tbody>
        </table>
    </div>
    
    <div class="mt-4" style="text-align: center">
        <a id="btnCancelDecimalSettings" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
        <a id="btnSaveDecimalSettings" data-role="none" class="btn btn-success btn-lg">Save</a>
    </div>             
</div>