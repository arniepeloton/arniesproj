<div data-role="content" style="position: inherit; width: 100%;">
    <div style="padding:15px">
        <div class="ui-grid-a">
            <div class="ui-block-a" style="width: 20%">
                <button data-icon="back" id="btnBackSenior">Back</button>
            </div>
            <div class="ui-block-b" style="text-align: right;width: 80%">
                <h2>Customer Profile</h2>
            </div>
        </div>


        <div data-role="fieldcontain" style="display:none;">
            <input type="text" readonly id="txtID" />
            <input type="text" readonly id="txtSCPWDCustomerID" />
            <input type="text" readonly id="txtCustomerIndex" />
        </div>

        @*        <div data-role="fieldcontain">
            <label for="btnTitle">Title:</label>
            <div style="display:none;"><input type="text" readonly id="txtTitleID"/></div>
            <button id="btnTitle" data-role="button">Please Select</button>
        </div>*@

        You can enter customer name in fullname or in detailed format.
        <div data-role="fieldcontain">
            <label for="txtSurname">Full Name: <span style="color: #FF0000;">*</span></label>
            <input type="text" id="txtFullname" maxlength="255" />
        </div>

        <div data-role="fieldcontain">
            <label for="txtSurname">Surname (Last Name): <span style="color: #FF0000;">*</span></label>
            <input type="text" id="txtSurname" maxlength="255" />
        </div>

        <div data-role="fieldcontain">
            <label for="txtFirstName">First Name: <span style="color: #FF0000;">*</span></label>
            <input type="text" id="txtFirstName" maxlength="255" />
        </div>

        <div data-role="fieldcontain">
            <label for="txtMiddleName">Full Middle Name: <span style="color: #FF0000;">*</span></label>
            <input type="text" id="txtMiddleName" maxlength="255" />
        </div>

        <div data-role="fieldcontain">
            <label for="selPersonalSuffixName">Generational Suffix (Name):</label>
            <select id="selPersonalSuffixName">
                <option value="">Please Select</option>
                <option value="Sr.">Senior (Sr.)</option>
                <option value="Jr.">Junior (Jr.)</option>
                <option value="II">II</option>
                <option value="III">III</option>
                <option value="IV">IV</option>
                <option value="V">V</option>
                <option value="VI">VI</option>
                <option value="VII">VII</option>
                <option value="VIII">VIII</option>
                <option value="IX">IX</option>
                <option value="X">X</option>
            </select>
        </div>
        <div class="custom-ui-label">
            <div>Date Of Birth: <span style="color: #FF0000;">*</span></div>
            <div>
                <fieldset data-role="controlgroup" data-type="horizontal">
                    <select id="selDOBMonth">
                        <option value="">MMM</option>
                    </select>
                    <select id="selDOBDay">
                        <option value="">DD</option>
                    </select>
                    <select id="selDOBYear">
                        <option value="">YYYY</option>
                    </select>
                </fieldset>
            </div>
        </div>

        <div data-role="fieldcontain">
            <label for="txtCustomerTIN">TIN: <span style="color: #FF0000;">*</span></label>
            <input type="text" id="txtCustomerTIN" minlength="12" maxlength="12" required />
        </div>

        <div>
            <ul data-role="listview" id="lstCustomerIdentificationContainer" data-inset="true">
                <li data-role="list-divider">Identification Number</li>
                <li>
                    <div data-role="fieldcontain"><a id="btnSalesPayCustomerIdentificationAdd" data-role="button" data-icon="plus" data-inline="true">Add New ID</a></div>

                    <div data-role="fieldcontain" id="divRecordCountCustomerIdentification"></div>
                    <div>
                        <ul data-role="listview" id="lstSalesPayCustomerIdentification" data-inset="true"></ul>
                    </div>
                </li>
            </ul>
        </div>

        <br />
        <br />
        <button id="btnSalesPayCustomerNameSave" type="submit">Ok</button>
        <a id="btnSalesPayCustomerNameRemove" data-role="button" style="display:none;">Remove</a>
        <button id="btnSalesPayCustomerNameCancel" type="submit">Cancel</button>
    </div>    
 </div>



 <div data-role="popup" id="popupAddNewID" class="ui-corner-all"  style="padding: 20px; width: 400px">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
				<center><h3>Valid Customer Identification Number</h3></center>

                <div data-role="content">
                    <select id="selIDType">
                        <option value="">Please Select</option>
                        <option value="Senior Citizens' ID">Senior Citizens' ID</option>
                        <option value="Persons With Disabilities (PWD) ID">Persons With Disabilities (PWD) ID</option>
                        <option value="Passport">Passport</option>
                        <option value="Driver's License">Driver's License</option>
                        <option value="SSS ID">SSS ID</option>
                        <option value="GSIS ID">GSIS ID</option>
                        <option value="PRC ID">PRC ID</option>
                        <option value="Postal ID">Postal ID</option>
                        <option value="Voter's ID">Voter's ID</option>
                        <option value="Other">Other</option>

                    </select>
                    <div id="divOtherIDType" style="display:none;">
                        <input type="text" id="txtOtherIDType" maxlength="50" style="text-align: center" placeholder="Enter ID Type"/>
                    </div>
                    <input type="text" id="txtIDText" maxlength="20" style="text-align: center" placeholder="Enter a valid identification number"/>
                    <br />
                    <button id="btnSalesPayCustomerIdentificationSave" type="submit">Ok</button>

                    <a id="btnSalesPayCustomerIdentificationRemove" data-role="button" style="display:none;">Remove</a>
                    <a id="btnSalesPayCustomerIdentificationCancel" data-role="button" data-rel="back">Cancel</a>
                    
                </div>

     

        </div>


</div>

