@Code
    ViewData("Title") = "Assign Work Schedule"
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
            <label for="txtEmployeeName">Employee Name : </label>            
            <input type="text" id="txtEmployeeName" disabled="disabled"/>
        </div>

        <div data-role="fieldcontain"> 
            <label for="txtFOWST">FOWST : </label>            
            <input type="text" id="txtFOWST" disabled="disabled"/>
            @*<label for="selFowst">FOWST : </label>            
            <select id="selFowst"></select>*@
        </div>

        <div data-role="fieldcontain">
            <label for="selSchedule">Schedule : </label>            
            <select id="selSchedule"></select>
        </div>

        <div data-role="fieldcontain">
            <fieldset id="txtEffectivityDateFrom" data-role="DateCombo" data-usecurrent="1"> 
                <legend> Effectivity Date From : </legend>
            </fieldset>            
        </div>            

        <div data-role="fieldcontain">
            <fieldset id="txtEffectivityDateto" data-role="DateCombo" data-usecurrent="1"> 
                <legend> Effectivity Date To : </legend>
            </fieldset>            
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
                <legend> &nbsp; </legend>
                <label><input type="checkbox" name="checkbox-0" id="chkInfiniteEffectivityDate"/> Infinite Effectivity Date </label>
            </fieldset>            
        </div>

        <br />

        <div> <a id="btnAddScheduleToEmployee" data-role="button" data-icon="plus">Add Schedule to Employee</a> </div>

        <div>
            <ul data-role="listview" id="lstOrgEmployeeScheduleList" data-split-icon="delete" data-inset="true">
                @*<li data-role="list-divider">Employee Schedule List</li>*@
            </ul>
        </div>

        @*<table>
            <tr>
                <td>
                    <label for="txtEffectivityDate">Effectivity Date : </label>            
                </td>
                <td>
                    
                </td>
            </tr>
        </table>*@
        
        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnDone" data-role="button"><h3>Done</h3></a>
            </div>
        </div>

    </div>

</div>

<div id="pageNewSchedule" data-role="page">
    <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>

    <div data-role="content">       
        <div data-role="fieldcontain">
            <label for="txtName"> Work Schedule Name: </label>
            <input type="text" id="txtName"/>
        </div>

        <div data-role="fieldcontain">
            <label for="txtDescription"> Description: </label>
            <input type="text" id="txtDescription"/>
        </div>
               
        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnSave" data-role="button"><h3>Save Schedule</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnAssignEmployees" data-role="button"><h3>Assign Employees to this schedule</h3></a>
            </div>
        </div>

    </div>

</div>




