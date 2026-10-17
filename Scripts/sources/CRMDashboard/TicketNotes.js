require(['common'], 
    function () {
        $(document).ready(function () {
            var noteID = '';

            window.TicketNotes = {
                LoadData: function () {
                    LoadRequiredData();
                },
                ShowData: function () {
                    LoadRequiredData();
                }
            }

            LoadRequiredData();

            //Ticket Notes
            $('body')
            .on('click', '#btnCancelTicketNote', function (e) {
                window.location.hash = 'LibraryPage';
            })
            .on('click', '#btnSaveTicketNote', function (e) {
                if ($.trim($('#txtTicketNotes').val()) == "") {
                    WebToast('Please enter note');
                    return;
                }

                SaveTicketNotes();
            })

            function SaveTicketNotes() {
                var pData = {
                    noteID: noteID,
                    Note: $.trim($('#txtTicketNotes').val()),
                    IsDefault: $('#chbTicketNoteAsDefault').is(':checked'),
                    Today: new Date()
                }

                $('#btnSaveTicketNote, #btnCancelTicketNote').attr('disable', 1); // disable button to avoid duplicate row

                AjaxSend({
                    url: 'CRMDashboard/SaveTicketNotes',
                    data: pData,
                    success: function (e) {
                        $('#btnSaveTicketNote, #btnCancelTicketNote').removeAttr('disable');

                        if (e.Results == "saved") {
                            ClearFields();
                            window.location.hash = 'LibraryPage';
                            LibraryTable.TicketNotes();
                        } else {
                            NewMessageBox(e.Results);
                        }
                    }
                })
            }

            function LoadRequiredData() {
                $('#divMainTicketNotesPage').pageshow();
                ClearFields();

                if (sessionStorage.AddViewLibBCName == "View Note") {
                    GetTicketNoteData()
                }
            }

            function GetTicketNoteData() {
                AjaxSend({
                    url: 'CRMDashboard/GetTicketNoteData',
                    data: { ID: sessionStorage.ViewID },
                    success: function (vData) {
                        ClearFields()
                        var vRows = vData.Results.Rows[0];

                        noteID = vRows.ID;

                        $('#txtTicketNotes').val(vRows.Notes);
                        $('#chbTicketNoteAsDefault').prop('checked', CBool(vRows.IsDefault))//.checkboxradio('refresh');
                    }
                })
            }

            function ClearFields() {
                noteID = '';
                $('#txtTicketNotes').val('');
                $('#chbTicketNoteAsDefault').prop('checked', true)//.checkboxradio('refresh');
            }
        })
    }
)