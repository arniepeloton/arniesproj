<div class="row mb-3 align-items-center">
    <div class="col-md-2 form-label">
        Date:
    </div>
    <div class="col-md">
        <input data-role="none" type="text" class="form-control" id="txtRAIDate" readonly />
    </div>
</div>

<div class="row mb-3 align-items-center">
    <div class="col-md-2 form-label">
        FOWST Location: <span class="required">*</span>
    </div>
    <div class="col-md">
        <select data-role="none" class="form-select" id="selRAIFowst"></select>
    </div>
</div>

<div class="row mb-3 align-items-center">
    <div class="col-md-2 form-label">
        Product:
    </div>
    <div class="col-md">
        <select data-role="none" class="form-select" id="selRAIMobizProduct">
            <option value="MCR">Mobiz Complete Retail</option>
            <option value="Mobiz EAS">Mobiz EAS</option>
            <option value="Mobiz DTR">Mobiz DTR</option>
            <option value="Others">Others</option>
        </select>
    </div>
</div>

<div class="row mb-3 align-items-center">
    <div class="col-md-2 form-label">
        Subject Line: <span class="required">*</span>
    </div>
    <div class="col-md">
        <select data-role="none" class="form-select" id="selRAISubject">
            <option value="">Please select</option>
            <option value="Bug Report">Bug Report</option>
            <option value="Request for information">Request for information</option>
            <option value="Feature request">Feature request</option>
            <option value="Others">Others</option>
        </select>
    </div>
</div>

<div class="row mb-3 align-items-center">
    <div class="col-md-2 form-label">
        Urgency: <span class="required">*</span>
    </div>
    <div class="col-md">
        <select data-role="none" class="form-select" id="selRAIUrgency">
            <option value="">Please select</option>
            <option value="Very Urgent">Very Urgent</option>
            <option value="Urgent">Urgent</option>
            <option value="N/A">Not Applicable</option>
            <option value="Nice to Have">Nice to Have</option>
        </select>
    </div>
</div>

<div class="row mb-3 align-items-center">
    <div class="col-md-2 form-label">
        Importance: <span class="required">*</span>
    </div>
    <div class="col-md">
        <select data-role="none" class="form-select" id="selRAIImportance">
            <option value="">Please select</option>
            <option value="Very Important">Very Important</option>
            <option value="Important">Important</option>
            <option value="N/A">Not Applicable</option>
            <option value="Nice to Have">Nice to Have</option>
        </select>
    </div>
</div>

<div class="row mb-3 align-items-center">
    <div class="col-md-2 form-label">
        Description: <span class="required">*</span>
    </div>
    <div class="col-md">
        <textarea data-role="none" class="form-control" id="txtRAIDescription" style="height:200px; resize:none"></textarea>
    </div>
</div>

<div class="mt-5 text-center">
    <button data-role="none" class="btn btn-success btn-lg" id="btnSendRAI">Send Report</button>
</div>