@Code
    ViewData("Title") = "Decimal Settings"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/Decimals/Decimals.js?crc=')]);
    </script>
    
End Section

<div id="pageDeviceMenu" data-role="page">
    <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>

    <div data-role="content">
        <div data-role="fieldcontain">           
            <label for="selSlider">Allow Decimals in Qty :</label>                
            <select data-role="slider" id="selSlider">
                <option value="optAllowNo">No</option>
                <option value="optAllowYes">Yes</option>                
            </select>
        </div>
        <br />

        <div id="dvIsAllowDecimal" style="visibility:hidden; display:none">
            <div data-role="fieldcontain">
             <fieldset data-role="controlgroup">
                <legend>&nbsp;</legend>
                    <input type="radio" name="rcDecimals" id="rcAllowAllDecimals" value="choice-1" checked="checked">
                    <label for="rcAllowAllDecimals">Allow decimal inputs to all products</label>
                    <input type="radio" name="rcDecimals" id="rcAllowOnSelected" value="choice-2">
                    <label for="rcAllowOnSelected">Allow decimal inputs on selected categories</label>
                    @*<input type="radio" name="rcDecimals" id="rcDoNotAllow" value="choice-3">
                    <label for="rcDoNotAllow">Disallow decimal inputs</label>*@
            </fieldset>
            </div>
        </div>

        <br /> <br />
        <div id="dvIsAllowOnSelectedCategories" style="visibility:hidden; display:none" >
            <div data-role="fieldcontain">
                <ul id="lstCategories" data-role="listview">
                    <li data-role="list-divider"> </li>
                    <li>
                        <table data-role="table" data-mode="reflow" class="ui-responsive" style="width:100%">
                            <thead>
                                <tr>
                                    <th style="text-align:center">
                                        Category
                                    </th>
                                
                                    <th style="text-align:center">
                                        Allow Decimal in Qty Inputs?
                                    </th>
                                </tr>
                            </thead>
                            <tbody id="tbCategories">
                                
                            </tbody>
                        </table>
                    </li>
                </ul>
            </div>
        </div>
        <br />
        <a id="btnSaveDecimalSettings" data-role="button" href="#">Save Decimal Settings</a>                    
    </div>
</div>