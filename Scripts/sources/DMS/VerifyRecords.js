require(['common'], function () {

    $(document).on("mobileinit", function () {
        $.mobile.ajaxEnabled = false;
        $.mobile.loadingMessage = false;
    });

    require(['/Content/bootstrap-5.0.0/js/bootstrap.js'], function () {
        $(document).ready(function () {
            //ShowRecords()

            $('#formVerifyRecords').submit(function (e) {
                e.preventDefault();
                e.stopPropagation();
                ShowRecords();
            })

            function ShowRecords() {
                var tbodyVerifyDocs = $('#tbodyVerifyDocs');
                var arrDocs = [
                    {
                        DocumentID: '7SO7NSAA',
                        DocumentName: 'Document ',
                        DocumentCategory: 'Category 1',
                        DocumentType: 'Type 1',
                        Metadata: [
                            {
                                Author: 'Jose Mari Chan',
                                'Date Publised': 'May 2, 2005'
                            }
                        ]
                    },
                    {
                        DocumentID: 'CDWRKLLZ',
                        DocumentName: 'Document ',
                        DocumentCategory: 'Category 1',
                        DocumentType: 'Type 1',
                        Metadata: [
                            {
                                Author: 'Maria Leonor Theresa',
                                'Date Publised': 'March 2, 1992'
                            }
                        ]
                    },
                    {
                        DocumentID: 'KN36D4WY',
                        DocumentName: 'Document ',
                        DocumentCategory: 'Category 2',
                        DocumentType: 'Type 2',
                        Metadata: [
                            {
                                Author: 'Ronald Reagan',
                                'Date Publised': 'August 16, 1979'
                            }
                        ]
                    }
                ]

                tbodyVerifyDocs.empty()

                for (var i = 0; i < arrDocs.length; i++) {
                    var metadataname = Object.keys(arrDocs[i].Metadata[0]);
                    var metadata = '';

                    for (var m = 0; m < metadataname.length; m++) {
                        metadata += '<b>' + metadataname[m] + '</b>: ' + arrDocs[i].Metadata[0][metadataname[m]] + '<br>';
                    }

                    tbodyVerifyDocs.append($('<tr>')
                            .append($('<td>').text(i + 1 + '.'))
                            .append($('<td>').text(arrDocs[i].DocumentID))
                            .append($('<td>').append(arrDocs[i].DocumentCategory))
                            .append($('<td>').append(arrDocs[i].DocumentType))
                            .append($('<td>').html(metadata))
                            .append($('<td style="text-align:center;">')
                                .append($('<button class="btn btn-primary">').text('View Document')
                                    .data('data', arrDocs[i])
                                    .click(function () {
                                        var data = $(this).data('data');
                                        var metadataname = Object.keys(data.Metadata[0]);
                                        var metadata = '';

                                        for (var m = 0; m < metadataname.length; m++) {
                                            metadata += '<b>' + metadataname[m] + '</b>: ' + data.Metadata[0][metadataname[m]] + '<br>';
                                        }

                                        $('#iframeViewDocument').attr('src', location.origin + '/FileTemplates/' + data.DocumentID + '.pdf');
                                        $('#lblViewDocID').text(data.DocumentID);
                                        $('#metadata').html(metadata)
                                        $('#popupViewDocs').modal('show');
                                    })
                                )
                            )
                        )
                }
            }

            function GenerateDocumentID() {
                var randomChars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';
                var result = '';
                for (var i = 0; i < 8; i++) {
                    result += randomChars.charAt(Math.floor(Math.random() * randomChars.length));
                }
                return result;
            }
        })
    })
})