require(['common',
    GetPathFormat('/Scripts3/Browser/LibraryFowst.js?crc=')
    ], function() {

    $(function () {

        var mCustomerAddressID;
        var mBranchID;
        var mFowstID;

        // BEGIN MainPage

        var mLvBranches;

        (function() {
            LoadAddresses();
        })();

        function LoadAddresses() {
            AjaxSend({
                url: 'PosSetup/Addresses',
                success: function (vData) {
                    Display(vData);
                },
                error: function (vError) {
                    alert('Network error');
                }
            }); // AjaxSend

            function Display(vData) {

                if (mLvBranches == undefined) {
                    mLvBranches = new CreateListView(); // the "new" keyword is important to prevent conflict with other instances of CreateListView
                    mLvBranches.Element.appendTo('#divBranches');
                    mLvBranches.OnItem = function(vDataRow, vItem, e) {
                        var pLnk = $('<a>')
                            .data('data', vDataRow)
                            .append($('<h3>')
                                .css('white-space','normal')
                                .text(vDataRow.Address)
                            ).append($('<p>').text('Branch: ' + vDataRow.BranchName))
                            .append($('<p>').text('FOWST Name: ' + vDataRow.FowstName))
                            .append($('<p>').text('FOWST Address: ' + vDataRow.FowstAddress))
                            // .append($('<p>').text('POS OR Counter: ' + vDataRow.POSORCounter))
                            .click(function(e) {
                                var pData = $(this).data('data');
                                e.preventDefault();
                                mCustomerAddressID = pData.ID;
                                mBranchID = pData.BranchID;
                                $('#spanAddress').text(pData.Address);
                                $('#spanBranch').text(pData.BranchName);
                                $('#spanFowstName').text(pData.FowstName);
                                // $('#txtPosOrCounter').val(pData.POSORCounter);
                                $.mobile.changePage('#pageDetails');
                                return false;
                            });

                        vItem.append(pLnk);
                    };
                }; // if (mLvBranches == undefined) {
                mLvBranches.SetData(vData.data.Rows);

            }; // function Display(vData) {
        }; // function LoadBranches() {

        // END MainPage

        (function pageDetails() {

            $('#butFowst').click(function() {

                Browser.LibraryFowst.AfterClick = function(e) {
                    mFowstID = e.ID;
                    $('#spanFowstName').text(e.FowstName);
                    // history.back();
                };
                Browser.LibraryFowst.Show();
            });

            $('#butSave').click(function() {

                if (CStr2(mBranchID).length == 0) {
                    MessageBox('Please select a branch first');
                    return;
                };

//                if ((!IsNumeric($('#txtPosOrCounter').val())) 
//                    || (CDec2($('#txtPosOrCounter').val()) < 0)) {

//                    MessageBox('POS OR Counter must be a valid number greater than or equal to zero');
//                    return;
//                };

                AjaxSend({
                    url: 'PosSetup/UpdateBranch',
                    data: {
                        BranchID: mBranchID,
                        FowstID: mFowstID
                    },
                    success: function (vData) {
                        if (vData.Result == 'ok') {
                            MessageBox('Branch and address were successfully linked', 'Saved', null, {
                                OnClose: function() {
                                    LoadAddresses();
                                    GotoFirstPage();
                                }
                            });
                        } else if (vData.Error) {
                            MessageBox(vData.Error);
                        };
                    },
                    error: function (vError) {
                        alert('Network error');
                    }
                }); // AjaxSend

            }); // $('#butSave').click

        })(); // pageDetails

    }); // $(function () {
});