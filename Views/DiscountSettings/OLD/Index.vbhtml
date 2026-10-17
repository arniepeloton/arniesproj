@Code
    ViewData("Title") = "Discount Settings"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
<script>
    require([GetPathFormat('/Scripts/sources/DiscountSettings/DiscountSettingsInfo.js?crc=')]);
</script>
<style id="4">
    .inline
    {
        display:inline;
        margin:5px 5px 5px 0px;
        vertical-align:top;
    }
    .ui-grid-b
    {
        margin-left: 15px;
    }
    .ui-grid-b .ui-block-a
    {
        width: 50%;
        margin-top: 5px;
        margin-bottom: 5px;
    }
    .ui-grid-b .ui-block-b
    {
        width: 40%;
        margin-top: 5px;
        margin-bottom: 5px;
    }
    .ui-grid-b .ui-block-c
    {
        width: 10%;
        margin-top: 5px;
        margin-bottom: 5px;
    }
</style>
<div id="pageMain" data-role="page">
    <div data-role="header">        
        <h1>@ViewData("Title")</h1>
    </div>
    <div data-role="content">

            <div class="ui-grid-b">
                <div class="ui-block-a">
                    <b>
                        Discount Type</b>   
                </div>
                <div class="ui-block-b">
                    <b>Maximum Percentage</b>
                </div>
                <div class="ui-block-c">
                
                </div>
            </div>


            <div class="ui-grid-b">
                <div class="ui-block-a">
                    <label for="txtSeniorCitizenDiscount" class="inline" >
                        Senior Citizen Discount:</label>      
                </div>
                <div class="ui-block-b">
                    <input id="txtSeniorCitizenDiscount" maxlength="5"/>
                </div>
                <div class="ui-block-c">
                    <label class="inline" style="font-size: large;">%</label>
                </div>
            </div>
   
            <div class="ui-grid-b">
                <div class="ui-block-a">
                    <label for="txtPersonWithDisabilityDiscount" class="inline" >
                        Person With Disability (PWD) Discount:</label>      
                </div>
                <div class="ui-block-b">
                    <input id="txtPersonWithDisabilityDiscount" maxlength="5"/>
                </div>
                <div class="ui-block-c">
                    <label class="inline" style="font-size: large;">%</label>
                </div>
            </div>


            <div class="ui-grid-b">
                <div class="ui-block-a">
                    <label for="txtEmployeeDiscount" class="inline" >
                        Employee Discount:</label>      
                </div>
                <div class="ui-block-b">
                    <input id="txtEmployeeDiscount" maxlength="5"/>
                </div>
                <div class="ui-block-c">
                    <label class="inline" style="font-size: large;">%</label>
                </div>
            </div>

            <div class="ui-grid-b">
                <div class="ui-block-a">
                    <label for="txtRegularDiscount1" class="inline" >
                        Regular Discount 1:</label>
                </div>
                <div class="ui-block-b">
                    <input id="txtRegularDiscount1" maxlength="5"/>
                </div>
                <div class="ui-block-c">
                    <label class="inline" style="font-size: large;">%</label>
                </div>
            </div>

            <div class="ui-grid-b">
                <div class="ui-block-a">
                    <label for="txtRegularDiscount2" class="inline" >
                        Regular Discount 2:</label>
                </div>
                <div class="ui-block-b">
                    <input id="txtRegularDiscount2" maxlength="5"/>
                </div>
                <div class="ui-block-c">
                    <label class="inline" style="font-size: large;">%</label>
                </div>
             </div>

            <div class="ui-grid-b">
                <div class="ui-block-a">
                    <label for="txtRegularDiscount3" class="inline" >
                        Regular Discount 3:</label>
                </div>
                <div class="ui-block-b">
                    <input id="txtRegularDiscount3" maxlength="5"/>
                </div>
                <div class="ui-block-c">
                    <label class="inline" style="font-size: large;">%</label>
                </div>
             </div>

            <div class="ui-grid-b">
                <div class="ui-block-a">
                    <label for="txtVIPDiscount" class="inline" >
                        VIP Discount:</label>
                        </div>
                <div class="ui-block-b">
                    <input id="txtVIPDiscount" maxlength="5"/>
                </div>
                <div class="ui-block-c">
                    <label class="inline" style="font-size: large;">%</label>
                </div>
             </div>

            <div class="ui-grid-b">
                <div class="ui-block-a">
                    <label for="txtGPCDiscount" class="inline" >
                        GPC Discount:</label>
                        </div>
                <div class="ui-block-b">
                    <input id="txtGPCDiscount" maxlength="5"/>
                </div>
                <div class="ui-block-c">
                    <label class="inline" style="font-size: large;">%</label>
                </div>
             </div>


        <br />
        <button id="butSave">Save</button>

        <a id="btnDiscountSettingsBack" data-role="button">Done</a> 

    </div>
</div>
