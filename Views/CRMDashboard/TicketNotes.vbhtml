<script>
    require([GetPathFormat('/Scripts/sources/CRMDashboard/TicketNotes.js?crc=' + assetversion)]);
</script>

<div id="divLoadingTicketNotes" subpage="pageTicketNotes" style="display:block;"><div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div></div></div>

<div id="divMainTicketNotesPage" subpage="pageTicketNotes" style="display:none;">
    <div class="form-check mb-3">
        <label for="chbTicketNoteAsDefault" class="form-check-label">
            <input data-role="none" type="checkbox" id="chbTicketNoteAsDefault" value="1" class="form-check-input" checked />
            Set as default Note
        </label>
    </div>

    <textarea data-role="none" id="txtTicketNotes" class="form-control" style="height:300px; max-height:300px" placeholder="Type your notes here"></textarea>

    <div class="mt-5 mb-5 text-center">
        <button data-role="none" class="btn btn-secondary btn-lg" id="btnCancelTicketNote">Cancel</button>
        <button data-role="none" class="btn btn-success btn-lg" id="btnSaveTicketNote">Save</button>
    </div>
</div>