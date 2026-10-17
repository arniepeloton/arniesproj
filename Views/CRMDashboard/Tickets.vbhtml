<div id="popTicketDetails" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-fullscreen">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Ticket Details</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <textarea data-role="none" id="txtTicketPlainText" style="margin-left:-1000px; position:absolute"></textarea>

                <div class="mb-3">
                    <div class="row border" id="divMyTickerReportSummary">
                        <div class="col-10 col-md-5 p-3 border font-weight-bold">
                            No. of new tickets in last 7 days:
                        </div>
                        <div class="col-2 col-md-1 text-center nosum p-3 border" id="lblNoOfNewTicketsInLast7Days">
                            0
                        </div>
                        <div class="col-10 col-md-5 p-3 border font-weight-bold">
                            Total number of tickets:
                        </div>
                        <div class="col-2 col-md-1 text-center nosum p-3 border" id="lblNoOfTickets">
                            0
                        </div>

                        <div class="col-10 col-md-5 p-3 border font-weight-bold">
                            No. of resolved tickets in last 7 days:
                        </div>
                        <div class="col-2 col-md-1 text-center nosum p-3 border" id="lblNoOfResolvedTicketsInLast7Days">
                            0
                        </div>
                        <div class="col-10 col-md-5 p-3 border font-weight-bold">
                            Total number of resolved tickets:
                        </div>
                        <div class="col-2 col-md-1 text-center nosum p-3 border" id="lblNoOfResolvedTickets">
                            0
                        </div>

                        <div class="col-10 col-md-5 p-3 border font-weight-bold">
                            No. of unresolved tickets in last 7 days:
                        </div>
                        <div class="col-2 col-md-1 text-center nosum p-3 border" id="lblNoOfUnresolvedTicketsInLast7Days">
                            0
                        </div>
                        <div class="col-10 col-md-5 p-3 border font-weight-bold">
                            Total number of unresolved tickets:
                        </div>
                        <div class="col-2 col-md-1 text-center nosum p-3 border" id="lblNoOfUnrsolvedTickets">
                            0
                        </div>
                    </div>
                </div>

                <div class="row mb-3 align-items-center">
                    <div class="col-md-auto mb-2 mb-md-0">Status:</div>
                    <div class="col-md-auto mb-2 mb-md-0">
                        <select data-role="none" class="form-select" id="selFilterClientTicketDetails">
                            <option value="All">All</option>
                            <option value="Unresolved">Unresolved</option>
                            <option value="Resolved">Resolved</option>
                            <option value="Cancelled">Cancelled</option>
                        </select>
                    </div>
                    <div class="col-md-auto mb-2 mb-md-0 divfg" style="display:none;">
                        FOWST's Group:
                    </div>
                    <div class="col-md-auto mb-2 mb-md-0 divfg" style="display:none;">
                        <select data-role="none" class="form-select" id="selFilterFOWSTGroup"></select>
                    </div>
                    <div class="col-md mb-2 mb-md-0 text-right">
                        <button data-role="none" class="btn btn-success" id="btnDownloadClientIssueTickets">Export to Excel File</button>
                    </div>
                </div>

                <div class="table-responsive">
                    <table id="tblDataTableTicketsByClientPopup" data-role="none" style="display:table;" class="table table-light table-striped table-hover">
                        <thead>
                            <tr class="align-top text-center" style="font-size:14px;">
                                <th style="cursor:pointer;"> Ticket Number </th>
                                <th style="cursor:pointer;"> Subject </th>
                                <th style="cursor:pointer;"> Location </th>
                                <th style="cursor:pointer;"> Remarks </th>
                                <th style="cursor:pointer;"> Status </th>
                                <th style="cursor:pointer;"> Due Date </th>
                                <th style="cursor:pointer;"> Date Opened </th>
                                <th style="cursor:pointer;"> Date Closed </th>
                                <th style="cursor:pointer;"> Comment </th>
                                <th style="cursor:pointer;"> Assigned To </th>
                                <th style="cursor:pointer;"> Encoded By </th>
                                <th style="cursor:pointer;"> Action </th>
                            </tr>
                        </thead>
                        <tbody id="tbodyTicketsDetails"></tbody>
                    </table>
                </div>                
            </div>
            <div class="modal-footer">
                <button data-role="none" type="button" class="btn btn-secondary btn-lg" data-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>

<div id="popTicketNewComment" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Add Comment</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <form method="post" enctype="multipart/form-data" id="formTicketPic">
                    <input data-role="none" type="file" accept="image/*" name="fileTicketPic" id="fileTicketPic" style="margin-left:-1500px; position:absolute;">
                </form>
                <div class="mb-3 text-right">
                    <button data-role="none" class="btn btn-success btnTicketAttachImage">Attach Image</button>
                </div>
                <div id="txtTicketNewComment" contenteditable="" style="min-height:250px; max-height:250px;" placeholder="Type the note here ..."></div>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col-md order-1 order-md-0">
                        <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Close</button>
                    </div>
                    <div class="col-md order-1 order-md-0">
                        <button data-role="none" type="button" id="btnSaveTicketComment" class="btn btn-success btn-block btn-lg">Save Comment</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="popIssueTicket" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Issue Ticket</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div class="mb-3" id="lblTicketFYI"></div>
                <div id="divTabTicket" style="white-space:nowrap; overflow:auto;">
                    <ul class="nav nav-tabs mb-3" style="flex-wrap:nowrap;">
                        <li class="nav-item" role="presentation">
                            <button data-role="none" class="nav-link active" data-toggle="tab" data-target="#tabTicketDetails">Ticket Info</button>
                        </li>
                        <li class="nav-item" role="presentation">
                            <button data-role="none" class="nav-link" id="btnTabTicketComments" data-toggle="tab" data-target="#tabTicketComments">Comments</button>
                        </li>
                        <li class="nav-item" role="presentation">
                            <button data-role="none" class="nav-link" id="btnTabTicketHistory" data-toggle="tab" data-target="#tabTicketHistory">Edit Trail</button>
                        </li>
                        <li class="nav-item" role="presentation">
                            <button data-role="none" class="nav-link" id="btnTabTicketRMA" data-toggle="tab" data-target="#tabTicketRMA">RMA</button>
                        </li>
                        <li class="nav-item" role="presentation">
                            <button data-role="none" class="nav-link" id="btnTabTicketAttachments" data-toggle="tab" data-target="#tabTicketAttachments">Attachments</button>
                        </li>
                    </ul>
                </div>

                <div class="tab-content p-3">
                    <div id="tabTicketDetails" class="tab-pane fade show active">
                        <input data-role="none" type="text" id="txtTicketID" style="display:none" />
                        @*<div title="Save" id="btnSaveTicket" class="btn-trail btnSaveTicket"><span id="btnTicketSave" class="ticket finalsave s48"></span><br /><span>Save</span></div>
            <div title="Print Ticket" id="btnPrintSample" class="btn-trail"><span class="ticket print s48"></span><br /><span>Print</span></div>*@
                        <div id="divTicketStatus" class="row mb-3" style="display:none;">
                            <label class="col-md-3">Status:</label>
                            <div class="col-md">
                                <div class="form-check form-check-inline">
                                    <input data-role="none" class="form-check-input" type="radio" name="radTicketStatus" id="radTicketOpen" checked value="Unresolved">
                                    <label class="form-check-label" for="radTicketOpen">Unresolved</label>
                                </div>

                                <div class="form-check form-check-inline">
                                    <input data-role="none" class="form-check-input" type="radio" name="radTicketStatus" id="radTicketResolved" value="Resolved">
                                    <label class="form-check-label" for="radTicketResolved">Resolved</label>
                                </div>

                                <div class="form-check form-check-inline">
                                    <input data-role="none" class="form-check-input" type="radio" name="radTicketStatus" id="radTicketClose" value="Cancelled">
                                    <label class="form-check-label" for="radTicketClose">Cancelled</label>
                                </div>
                            </div>
                        </div>

                        <div id="divTicketStatusConfirmedBy" class="row mb-3" style="display:none;">
                            <label class="col-md-3 col-form-label"><span id="lblTicketStatus"></span> Confirmed By:</label>
                            <div class="col-md"><input data-role="none" type="text" class="form-control" id="txtTicketStatusConfirmBy" /></div>
                        </div>

                        <div id="divTicketNumber" class="row mb-3" style="display:none;">
                            <label class="col-md-3">Ticket Number:</label>
                            <div class="col-md"><b id="lblTicketNumber"></b></div>
                        </div>

                        <div id="divTicketClientList" class="row mb-3" style="display:none;">
                            <label class="col-md-3 col-form-label">Client Name: <span class="required">*</span></label>
                            <div class="col-md"><select id="selTicketClientName" data-role="none" class="form-select"></select></div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 col-form-label">FOWST Location:</label>
                            <div class="col-md">
                                <div class="row mb-3 align-items-center">
                                    <div class="col-auto">
                                        <input data-role="none" type="radio" class="form-check-input" name="radClientFowst" id="radNoClientFowst" />
                                    </div>
                                    <div class="col-auto">
                                        <label class="form-check-label" for="radNoClientFowst">No FOWST</label>
                                    </div>                                    
                                </div>
                                <div class="row mb-3 align-items-center">
                                    <div class="col-auto">
                                        <input data-role="none" type="radio" class="form-check-input" name="radClientFowst" id="radSelectFromClientFowst" />
                                    </div>
                                    <div class="col-auto">
                                        <label class="form-check-label" for="radSelectFromClientFowst">Browse Client's FOWST</label>
                                    </div>
                                    <div class="col"><button data-role="none" class="btn btn-secondary btn-block" id="btnClientFOWST">Search here</button></div>
                                </div>
                                <div class="row mb-3 align-items-center">
                                    <div class="col-auto">
                                        <input data-role="none" type="radio" class="form-check-input" name="radClientFowst" id="radFreeTypeClientFowst" />
                                    </div>
                                    <div class="col-auto">
                                        <label class="form-check-label" for="radFreeTypeClientFowst">Type client's FOWST name manually</label>
                                    </div>
                                    <div class="col"><input data-role="none" class="form-control" id="txtClientFowst" placeholder="Type client's FOWST location manually" /></div>
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 col-form-label">Subject: <span class="required">*</span></label>
                            <div class="col-md">
                                <select id="selTicketType" data-role="none" class="form-select">
                                    <option value="">Please Select</option>
                                    <option value="Bug Report">Bug Report</option>
                                    <option value="Claim Ticket">Claim Ticket</option>
                                    <option value="Customer Complaint">Customer Complaint</option>
                                    <option value="Hardware Repair">Hardware Repair</option>
                                    <option value="Need Answer to Question">Need Answer to Question</option>
                                    <option value="User Report">User Tutorial</option>
                                    <option value="Warranty Service">Warranty Service</option>
                                    <option value="Request for information">Request for information</option>
                                    <option value="Feature request">Feature request</option>
                                    <option value="Others">Others</option>
                                </select>
                            </div>
                            <div class="col-md" id="divTicketOthersField" style="display:none;">
                                <input type="text" class="form-control" id="txtTicketOthers" placeholder="Enter other ticket type" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 col-form-label">Product/Services:</label>
                            <div class="col-md">
                                <input type="text" id="txtTicketProductServices" data-role="none" class="form-control" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 col-form-label">Client Contact Person:</label>
                            <div class="col-md">
                                <input data-role="none" type="text" id="txtTicketContactPersonID" style="display:none" />
                                <input type="text" id="txtTicketContactPerson" data-role="none" class="form-control" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 col-form-label">Address to service in case of visit:</label>
                            <div class="col-md">
                                <input type="text" id="txtTicketAddress" data-role="none" class="form-control" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 col-form-label">Assigned To:<span class="required d-none">*</span></label>
                            <div class="col-md">
                                <input data-role="none" type="text" id="txtTicketAssignedToID" style="display:none" />
                                <button data-role="none" class="btn btn-secondary btn-block" id="btnTicketAssignedTo">Search Employee</button>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 col-form-label">Recommended Action:<span class="required d-none">*</span></label>
                            <div class="col-md">
                                <select id="selTicketRecommendedAction" data-role="none" class="form-select">
                                    <option value="">Please Select</option>
                                    <option value="Call Back">Call Back</option>
                                    <option value="Client Visit">Client Visit</option>
                                    <option value="For check-up/repair, then pick up by client">For check-up/repair, then to be picked up by client</option>
                                    <option value="For check-up/ repair, to be delivered to client">For check-up/ repair, then to be delivered to client</option>
                                    <option value="Return item to vendor">Return item to vendor</option>
                                    <option value="Others">Others</option>
                                </select>
                            </div>
                            <div class="col-md" id="divTicketOthersRecommendedField" style="display:none;">
                                <input type="text" class="form-control" id="txtTicketRecommendedActionOthers" placeholder="Enter other recommended action" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3">Best day to service:</label>
                            <div class="col-md">
                                <div class="form-check form-check-inline">
                                    <input data-role="none" class="form-check-input" type="checkbox" name="radTicketDays" id="radDayMon" value="Monday">
                                    <label class="form-check-label" for="radDayMon">Monday</label>
                                </div>

                                <div class="form-check form-check-inline">
                                    <input data-role="none" class="form-check-input" type="checkbox" name="radTicketDays" id="radDayTue" value="Tuesday">
                                    <label class="form-check-label" for="radDayTue">Tuesday</label>
                                </div>

                                <div class="form-check form-check-inline">
                                    <input data-role="none" class="form-check-input" type="checkbox" name="radTicketDays" id="radDayWed" value="Wednesday">
                                    <label class="form-check-label" for="radDayWed">Wednesday</label>
                                </div>

                                <div class="form-check form-check-inline">
                                    <input data-role="none" class="form-check-input" type="checkbox" name="radTicketDays" id="radDayThu" value="Thursday">
                                    <label class="form-check-label" for="radDayWed">Thursday</label>
                                </div>

                                <div class="form-check form-check-inline">
                                    <input data-role="none" class="form-check-input" type="checkbox" name="radTicketDays" id="radDayThu" value="Thursday">
                                    <label class="form-check-label" for="radDayWed">Thursday</label>
                                </div>

                                <div class="form-check form-check-inline">
                                    <input data-role="none" class="form-check-input" type="checkbox" name="radTicketDays" id="radDayFri" value="Friday">
                                    <label class="form-check-label" for="radDayWed">Friday</label>
                                </div>

                                <div class="form-check form-check-inline">
                                    <input data-role="none" class="form-check-input" type="checkbox" name="radTicketDays" id="radDaySat" value="Saturday">
                                    <label class="form-check-label" for="radDayWed">Saturday</label>
                                </div>

                                <div class="form-check form-check-inline">
                                    <input data-role="none" class="form-check-input" type="checkbox" name="radTicketDays" id="radDaySun" value="Sunday">
                                    <label class="form-check-label" for="radDayWed">Sunday</label>
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 col-form-label">Best time to service:</label>
                            <div class="col-md-3">
                                <button data-role="none" class="btn btn-secondary btn-block" id="btnTicketTimeFrom">Select Time</button>
                            </div>
                            <div class="col-md-1 text-center">
                                to
                            </div>
                            <div class="col-md-3">
                                <button data-role="none" class="btn btn-secondary btn-block" id="btnTicketTimeTo">Select Time</button>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 col-form-label">Due Date:<span class="required d-none">*</span></label>
                            <div class="col-md">
                                <button data-role="none" class="btn btn-secondary btn-block" id="btnTicketDueDate">Set Date</button>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 col-form-label">Remarks:</label>
                            <div class="col-md">
                                <textarea id="txtTicketNote" class="form-control" style="height:150px; max-height:150px; resize:none;"></textarea>
                            </div>
                        </div>

                        <div class="row mb-3" style="display:none;">
                            <label class="col-md-3" for="chbDelegateToMobiz">Delegate to Mobiz Client:</label>
                            <div class="col-md">
                                <input data-role="none" type="checkbox" class="form-check-input" id="chbDelegateToMobizClient" />
                                <br />
                                <i><b>Note:</b> If checked, it will allow <b name="lblClientName">Mobiz's Client</b> to view/edit this ticket.</i>
                            </div>
                        </div>

                        @*<div class="row mb-3" style="display:none;">
        <label class="col-md-3" for="chbDelegateToMobiz">Delegate to 1Mobiz:</label>
        <div class="col-md">
            <input data-role="none" type="checkbox" class="form-check-input" id="chbDelegateToMobiz" />
            <br />
            <i><b>Note:</b> If checked, we will send the report to the CRM of 1Mobiz</i>
        </div>
    </div>*@
                    </div>
                    <div id="tabTicketComments" class="tab-pane fade">
                        <div id="divTicketComments">
                            <div class="row mb-3">
                                <div class="offset-md-6 col-md-6" style="text-align:right;"><button data-role="none" class="btn btn-success" id="btnTicketAddComment">Add Comment</button></div>
                            </div>
                            
                            <ul id="listTicketComments" class="list-group"></ul>
                        </div>
                    </div>
                    <div id="tabTicketHistory" class="tab-pane fade">
                        <table data-role="none" style="display:table;" class="table table-light table-striped table-hover">
                            <thead>
                                <tr class="custom-theme-f">
                                    <th style="cursor:pointer;"> Modified By </th>
                                    <th style="cursor:pointer;"> Modified Date </th>
                                </tr>
                            </thead>
                            <tbody id="tbodyTicketsHistory"></tbody>
                        </table>
                    </div>
                    <div id="tabTicketRMA" class="tab-pane fade">
                        <div class="row">
                            <div class="col-sm">
                                <h5>Return Merchandise Authorization</h5>
                            </div>
                            <div class="col-sm-auto text-right">
                                <button data-role="none" id="btnAddRMA" class="btn btn-success">Add RMA</button>
                            </div>
                        </div>
                        <br />
                        <table id="tblDataTableRMA" data-role="none" style="display:table;" class="table table-light table-striped table-hover">
                            <thead>
                                <tr class="custom-theme-f">
                                    <th style="cursor:pointer;"> RMA # </th>
                                    <th style="cursor:pointer;"> Return To </th>
                                    <th style="cursor:pointer;"> RMA Date </th>
                                    <th style="cursor:pointer;"> WB or BL Date </th>
                                    <th style="cursor:pointer;"> No. of Items </th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody id="tbodyRMA"></tbody>
                        </table>
                    </div>
                    <div id="tabTicketAttachments" class="tab-pane fade">
                        <div class="row">
                            <div class="col-md text-right">
                                <button data-role="none" id="btnTicketFileUpload" title="Upload File" class="btn btn-success">
                                    Upload File
                                </button>
                                <input data-role="none" type="file" id="ticketfileUpload" accept=".xlsx,.xls,image/*,.doc, .docx,.ppt, .pptx,.txt,.pdf" multiple style="display:none;" />
                            </div>
                        </div>
                        <br />
                        <div class="table-responsive">
                            <table id="tblTicketAttachment" data-role="table" style="display:table; width:100%;" class="table table-light table-striped table-hover">
                                <thead>
                                    <tr class="custom-theme-f">
                                        <th style="width:20px !important;"> # </th>
                                        <th style="cursor:pointer;"> File Name </th>
                                        <th style="cursor:pointer; width:150px"> Date Uploaded </th>
                                        <th style="cursor:pointer; width:50px"> </th>
                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>                        
                    </div>
                </div>                
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col-md order-2 p-1 order-md-0">
                        <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                    </div>
                    <div class="col-md order-0 p-1 order-md-1">
                        <button data-role="none" type="button" print="0" class="btn btn-success btn-block btn-lg btnSaveTicket">Save</button>
                    </div>
                    <div class="col-md order-1 p-1 order-md-2">
                        <button data-role="none" type="button" print="1" class="btn btn-success btn-block btn-lg btnSaveTicket">Save and Print</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="popTicketHistory" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">History Details</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div class="mb-3">
                    <label class="form-label">Status:</label>
                    <input data-role="none" id="lblROTicketStatus" class="form-control" readonly />
                </div>

                <div class="mb-3">
                    <label class="form-label">Ticket Number:</label>
                    <input data-role="none" id="lblROTicketNumber" class="form-control" readonly />
                </div>

                <div class="mb-3">
                    <label class="form-label">Ticket Type:</label>
                    <input data-role="none" id="lblROTicketType" class="form-control" readonly />
                </div>

                <div class="mb-3">
                    <label class="form-label">Client Contact Person:</label>
                    <input data-role="none" id="lblROTicketContactPerson" class="form-control" readonly />
                </div>

                <div class="mb-3">
                    <label class="form-label">Address to service in case of visit:</label>
                    <input data-role="none" id="lblROTicketAddress" class="form-control" readonly />
                </div>

                <div class="mb-3">
                    <label class="form-label">Assigned To:</label>
                    <input data-role="none" id="lblROTicketAssignedTo" class="form-control" readonly />
                </div>

                <div class="mb-3">
                    <label class="form-label">Recommended Action:</label>
                    <input data-role="none" id="lblROTicketRecommendedAction" class="form-control" readonly />
                </div>

                <div class="mb-3">
                    <label class="form-label">Best day to service:</label>
                    <input data-role="none" id="lblROTicketServiceDays" class="form-control" readonly />
                </div>

                <div class="mb-3">
                    <label class="form-label">Best time to service:</label>
                    <input data-role="none" id="lblROTicketServiceTime" class="form-control" readonly />
                </div>

                <div class="mb-3">
                    <label class="form-label">Due Date:</label>
                    <input data-role="none" id="lblROTicketDueDate" class="form-control" readonly />
                </div>

                <div class="mb-3">
                    <label class="form-label">Remarks:</label>
                    <input data-role="none" id="lblROTicketRemarks" class="form-control" readonly />
                </div>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col-md order-1 order-md-0">
                        <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="popRMA" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-lg modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">RMA</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div style="white-space:nowrap; overflow:auto;">
                    <ul class="nav nav-tabs" role="tablist" style="flex-wrap:nowrap;">
                        <li class="nav-item" role="presentation">
                            <button data-role="none" class="nav-link active" data-toggle="tab" data-target="#tabRMADetails">RMA Details</button>
                        </li>
                        <li class="nav-item" role="presentation">
                            <button data-role="none" id="btnTabRMAEditTrail" class="nav-link" data-toggle="tab" data-target="#tabRMAEditTrail">Edit Trail</button>
                        </li>
                    </ul>
                </div>

                <div class="tab-content p-3">
                    <div id="tabRMADetails" class="tab-pane fade show active">
                        <div class="mb-3" style="display:none;">
                            <label class="form-label">RMA #:</label>
                            <input data-role="none" type="text" class="form-control" id="txtRMANo" readonly />
                        </div>

                        <div class="mb-3">
                            <label class="form-label">Return To: <span class="required">*</span></label>
                            <input data-role="none" type="text" class="form-control" id="txtRMAReturnTo" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label">RMA Date: <span class="required">*</span></label>
                            <input data-role="none" type="text" class="form-control" id="txtRMADate" readonly />
                        </div>

                        <h4>Shipping Info</h4>

                        <div class="mb-3">
                            <label class="form-label">Logistic Company:</label>
                            <input data-role="none" type="text" class="form-control" id="txtRMALogisticCompany" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label">WB or BL Ref:</label>
                            <input data-role="none" type="text" class="form-control" id="txtRMALogisticCompany" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label">WB or BL Date:</label>
                            <input data-role="none" type="text" class="form-control" id="txtRMAWBBlDate" readonly />
                        </div>

                        <div class="mb-3">
                            <label class="form-label">Mode of Transport:</label>
                            <select id="selRMATransport" data-role="none" class="form-select">
                                <option value="">Please Select</option>
                                <option value="Air">Air</option>
                                <option value="Land">Land</option>
                                <option value="Sea">Sea</option>
                            </select>
                        </div>

                        <div class="mb-3">
                            <label class="form-label">WB or BL Amount:</label>
                            <input data-role="none" type="text" class="form-control" id="txtRMAWBBlAmount" />
                        </div>

                        <div class="row mb-3 align-items-center">
                            <div class="col-md"><h4>Items to Return</h4></div>
                            <div class="col-md text-right">
                                <button data-role="none" id="btnAddRMAItems" class="btn btn-success">Add Item to Return</button>
                            </div>
                        </div>

                        <div class="table-responsive">
                            <table id="tblDataTableRMAItemsToReturn" data-role="none" style="display:table;" class="table table-light table-striped table-hover">
                                <thead>
                                    <tr class="custom-theme-f">
                                        <th style="cursor:pointer;"> Qty </th>
                                        <th style="cursor:pointer;"> Unit </th>
                                        <th style="cursor:pointer;"> Description </th>
                                        <th style="cursor:pointer;"> Serial No </th>
                                        <th style="cursor:pointer;"> Invoice Ref. # </th>
                                        <th style="cursor:pointer;"> Remarks </th>
                                        <th></th>
                                    </tr>
                                </thead>
                                <tbody id="tbodyRMAItemsToReturn"></tbody>
                            </table>
                        </div>                        
                    </div>
                    <div id="tabRMAEditTrail" class="tab-pane fade">
                        <div class="table-responsive">
                            <table id="tblDataTableRMAHistory" data-role="none" style="display:table;" class="table table-light table-striped table-hover">
                                <thead>
                                    <tr class="custom-theme-f">
                                        <th style="cursor:pointer;"> Modified By </th>
                                        <th style="cursor:pointer;"> Modified Date </th>
                                    </tr>
                                </thead>
                                <tbody id="tbodyRMAHistory"></tbody>
                            </table>
                        </div>                        
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col-md order-1 order-md-0">
                        <button data-role="none" type="button" class="btn btn-secondary btn-lg btn-block" data-dismiss="modal">Cancel</button>
                    </div>
                    <div class="col-md order-0 order-md-1">
                        <button data-role="none" type="button" id="btnSaveRMA" class="btn btn-success btn-lg btn-block">Save</button>
                    </div>
                </div>                
            </div>
        </div>
    </div>
</div>

<div id="popRMAItems" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Item Details</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div class="mb-3">
                    <label for="txtRMAItemsQty" class="form-label">Qty: <span class="required">*</span></label>
                    <input data-role="none" id="txtRMAItemsQty" class="form-control" />
                </div>

                <div class="mb-3">
                    <label for="txtRMAItemsUnit" class="form-label">Unit: <span class="required">*</span></label>
                    <input data-role="none" id="txtRMAItemsUnit" class="form-control" />
                </div>

                <div class="mb-3">
                    <label for="txtRMAItemDescription" class="form-label">Description: <span class="required">*</span></label>
                    <textarea data-role="none" class="form-control" id="txtRMAItemDescription" style="height:100px; max-height:100px; resize:none"></textarea>
                </div>

                <div class="mb-3">
                    <label for="txtRMAItemsSerialNo" class="form-label">Serial No: <span class="required">*</span></label>
                    <input data-role="none" id="txtRMAItemsSerialNo" class="form-control" />
                </div>

                <div class="mb-3">
                    <label for="txtRMAItemsInvoiceNo" class="form-label">Invoice Ref. #:</label>
                    <input data-role="none" id="txtRMAItemsInvoiceNo" class="form-control" />
                </div>

                <div class="mb-3">
                    <label for="txtRMAItemRemark" class="form-label">Remarks:</label>
                    <textarea data-role="none" class="form-control" id="txtRMAItemRemark" style="height:100px; max-height:100px; resize:none"></textarea>
                </div>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col-md order-1 order-md-0">
                        <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Close</button>
                    </div>
                    <div class="col-md order-0 order-md-1">
                        <button data-role="none" type="button" id="btnOkItem" class="btn btn-success btn-block btn-lg">Ok</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="popReadOnlyRMA" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">RMA History Details</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div class="mb-3">
                    <label for="lblRMAReturnTo" class="form-label">Return To:</label>
                    <input data-role="none" id="lblRMAReturnTo" class="form-control" readonly />
                </div>

                <div class="mb-3">
                    <label for="lblRMADate" class="form-label">RMA Date:</label>
                    <input data-role="none" id="lblRMADate" class="form-control" readonly />
                </div>

                <h4>Shipping Info</h4>

                <div class="mb-3">
                    <label for="lblRMALogisticCompany" class="form-label">Logistic Company:</label>
                    <input data-role="none" id="lblRMALogisticCompany" class="form-control" readonly />
                </div>

                <div class="mb-3">
                    <label for="lblRMAWBBLRef" class="form-label">WB or BL Ref:</label>
                    <input data-role="none" id="lblRMAWBBLRef" class="form-control" readonly />
                </div>

                <div class="mb-3">
                    <label for="lblRMAWBBLDate" class="form-label">WB or BL Date:</label>
                    <input data-role="none" id="lblRMAWBBLDate" class="form-control" readonly />
                </div>

                <div class="mb-3">
                    <label for="lblRMAModeOfTransport" class="form-label">Mode of Transport:</label>
                    <input data-role="none" id="lblRMAModeOfTransport" class="form-control" readonly />
                </div>

                <div class="mb-3">
                    <label for="lblRMAWBBLAmount" class="form-label">WB or BL Amount:</label>
                    <input data-role="none" id="lblRMAWBBLAmount" class="form-control" readonly />
                </div>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col-md order-1 order-md-0">
                        <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="popDownloadIssueTickets" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Download Issue Tickets</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div class="mb-3">
                    <label for="selDownloadClientTicketStatus" class="form-label">Status:</label>
                    <select data-role="none" class="form-select" id="selDownloadClientTicketStatus">
                        <option value="All">All</option>
                        <option value="Unresolved">Unresolved</option>
                        <option value="Resolved">Resolved</option>
                        <option value="Cancelled">Cancelled</option>
                    </select>
                </div>

                <div class="mb-3">
                    <label for="btnDownloadDateReported" class="form-label">Date Reported:</label>
                    <button data-role="none" class="btn btn-secondary btn-block" id="btnDownloadDateReported">Select Date</button>
                </div>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col">
                        <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                    </div>
                    <div class="col">
                        <button data-role="none" type="button" id="btnDownloadNowIssueTickets" class="btn btn-success btn-block btn-lg">Download Now</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>