@Code
    ViewData("Title") = "Pay Grade"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/Organization/OrganizationEmployeeAssignSchedule.js?crc=')]);
    </script>

End Section

<div id="pageOrganizationEmployeeSchedule" data-role="page">
    <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>

    <div data-role="content">       
        
        <div data-role="fieldcontain">
            <label for="txtPayCode">Pay Grade Code : </label>            
            <input type="text" id="txtPayCode"/>
        </div>

        <div data-role="fieldcontain"> 
            <label for="txtPayName">Pay Grade Name : </label>            
            <input type="text" id="txtPayName"/>
        </div>

        <div data-role="fieldcontain">
            <label for="txtPayDescription">Pay Grade Description : </label>
            <input type="text" id="txtPayDescription"/>
        </div>

        <br />

        <div data-role="fieldcontain">            
            <fieldset data-role="controlgroup" data-type="horizontal">
                <legend>Pay Rate</legend>
                <input type="radio" name="rdSort" id="rdHourly" checked="checked" />
                <label for="rdHourly">Hourly</label>
                <input type="radio" name="rdSort" id="rdDaily"/>
                <label for="rdDaily">Daily</label>
                <input type="radio" name="rdSort" id="rdMonthly"/>
                <label for="rdMonthly">Monthly</label>
            </fieldset>
        </div>
        
        <br />

        @*Hour*@

        <div data-role="fieldcontain" id="dvHourlyRate">
            <table style="width:100%"> 
                <tr>
                    <td>
                        Hourly Rate : 
                    </td>
                    <td>
                        <input type="number" id="txtHourlyRateNumeric"/>
                    </td>
                    <td>
                        <select id='selHourlyRateCurrency'>
                            <option value=""> Php </option>
                        </select>
                    </td>
                </tr>
                <tr>
                    <td>
                        Pay Denominator : 
                    </td>
                    <td>
                        <input type="text" id="txtHourlyRatePayDenominatorMinutes"/>
                    </td>
                    <td>
                        minutes per hour
                    </td>
                </tr>
                <tr>
                    <td>
                        No Work No Pay : 
                    </td>
                    <td colspan="2">
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <input type="radio" name="rdSort" id="rdHourlyRateNoWorkNoPayYes" checked="checked" />
                            <label for="rdHourlyRateNoWorkNoPayYes">Yes</label>
                            <input type="radio" name="rdSort" id="rdHourlyRateNoWorkNoPayNo"/>
                            <label for="rdHourlyRateNoWorkNoPayNo">No</label>                
                        </fieldset>
                    </td>                    
                </tr>
                <tr>
                    <td></td>
                    <td colspan="2">
                        <br />
                            When "Yes" is selected, pay deductions are made due to absences and tardiness
                    </td>
                </tr>
            </table>
        </div>

        @*daily*@
        <div data-role="fieldcontain" id="dvDailyRate">
            <table style="width:100%"> 
                <tr>
                    <td colspan="3">
                        Daily Rate : 
                    </td>
                </tr>

                <tr>
                    <td>
                       &nbsp; &nbsp;  Basic Rate:
                    </td>
                    <td>
                        <input type="number" id="txtDailyBasicRateNumeric"/>
                    </td>
                    <td>
                        <select id='selDailyBasicRateCurrency'>
                            <option value=""> Php </option>
                        </select>
                    </td>
                </tr>
                <tr>
                    <td>
                       &nbsp; &nbsp;  COLA Rate:
                    </td>
                    <td>
                        <input type="number" id="txtDailyColaRateNumeric"/>
                    </td>
                    <td>
                        <select id='selDailyColaRateCurrency'>
                            <option value=""> Php </option>
                        </select>
                    </td>
                </tr>

                <tr>
                    <td>
                        Pay Denominator : 
                    </td>
                    <td>
                        <input type="text" id="txtDailyRatePayDenominatorHours"/>
                    </td>
                    <td>
                        hours per day
                    </td>
                </tr>

                <tr>
                    <td>
                        
                    </td>
                    <td>
                        <input type="text" id="txtDailyRatePayDenominatorminutes"/>
                    </td>
                    <td>
                        minutes per hour
                    </td>
                </tr>

                <tr>
                    <td>
                        No Work No Pay : 
                    </td>
                    <td colspan="2">
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <input type="radio" name="rdSort" id="rdDailyRateNoWorkNoPayYes" checked="checked" />
                            <label for="rdDailyRateNoWorkNoPayYes">Yes</label>
                            <input type="radio" name="rdSort" id="rdDailyRateNoWorkNoPayNo"/>
                            <label for="rdDailyRateNoWorkNoPayNo">No</label>                
                        </fieldset>
                    </td>                    
                </tr>
                <tr>
                    <td></td>
                    <td colspan="2">
                        <br />
                            When "Yes" is selected, pay deductions are made due to absences and tardiness
                    </td>
                </tr>
            </table>
        </div>
        
        @*monthly*@

        <div data-role="fieldcontain" id="dvDailyRate">
            <table style="width:100%"> 
                <tr>
                    <td colspan="3">
                        Monthly Rate : 
                    </td>
                </tr>

                <tr>
                    <td>
                       &nbsp; &nbsp;  Basic Rate:
                    </td>
                    <td>
                        <input type="number" id="txtMonthlyBasicRateNumeric"/>
                    </td>
                    <td>
                        <select id='selMonthlyBasicRateCurrency'>
                            <option value=""> Php </option>
                        </select>
                    </td>
                </tr>
                <tr>
                    <td>
                       &nbsp; &nbsp;  Allowances:
                    </td>
                    <td>
                        <input type="number" id="txtMonthlyAllowanceRateNumeric"/>
                    </td>
                    <td>
                        <select id='selMonthlyAllowanceRateCurrency'>
                            <option value=""> Php </option>
                        </select>
                    </td>
                </tr>

                <tr>
                    <td>
                        Pay Denominator : 
                    </td>
                    <td>
                        <input type="text" id="txtMonthlyRatePayDenominatorMonth"/>
                    </td>
                    <td>
                        days per month
                    </td>
                </tr>

                <tr>
                    <td>
                        
                    </td>
                    <td>
                        <input type="text" id="txtMonthlyRatePayDenominatorDay"/>
                    </td>
                    <td>
                        hours per day
                    </td>
                </tr>

                <tr>
                    <td>
                        
                    </td>
                    <td>
                        <input type="text" id="txtMonthlyRatePayDenominatorHour"/>
                    </td>
                    <td>
                        minutes per hour
                    </td>
                </tr>

                <tr>
                    <td>
                        No Work No Pay : 
                    </td>
                    <td colspan="2">
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <input type="radio" name="rdSort" id="rdMonthlyRateNoWorkNoPayYes" checked="checked" />
                            <label for="rdMonthlyRateNoWorkNoPayYes">Yes</label>
                            <input type="radio" name="rdSort" id="rdMonthlyRateNoWorkNoPayNo"/>
                            <label for="rdMonthlyRateNoWorkNoPayNo">No</label>                
                        </fieldset>
                    </td>                    
                </tr>
                <tr>
                    <td></td>
                    <td colspan="2">
                        <br />
                            When "Yes" is selected, pay deductions are made due to absences and tardiness
                    </td>
                </tr>
            </table>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnSaveEmployeeSchedule" data-role="button"><h3>Save pay grade</h3></a>
            </div>
        </div>

    </div>

</div>





