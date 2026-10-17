define([
    ], function () {

        var mDeferred = $.Deferred();
        var mHtml;

        (function staticInit() {

            AjaxSend({
                ExactUrl: '/Scripts/sources/AutoDataArchiving/AutoDataArchiving.html',
                type: 'GET',
                dataType: 'text',
                success: function (vData) {
                    mHtml = vData;
                    mDeferred.resolve();
                }
            });

        })();

        CreateStringObject('App.controller.AutoDataArchiving', null, function () {

            var mOut = {};
            var mDate;

            var mElement = $(mHtml);
            mOut.element = mElement;

            mElement.find('[name=butCalendar]').click(function() {
                SingleCalendar({
                    ShowTime: true,
                    Click: function(e) {
                        var pDate = new Date(e);
                        mDate = e;
                        pDate = FormatDate(pDate, { Format: 'MMM d, yyyy' });
                        mElement.find('[name=butCalendar]').text(pDate);
                    }
                });
            });

            mElement.find('[name=butComputeYears]').click(function() {
                var pTxtYears = mElement.find('[name=txtYears]').val();
                pTxtYears = CInt2(pTxtYears);
                if (pTxtYears <= 0) {
                    NewMessageBox('Number of years must be a whole number greater than zero.', 'Invalid number of years');
                    return;
                }

                var pYear2 = new Date().getFullYear() - pTxtYears;
                var pDate2 = new Date(pYear2, 11, 31);
                mDate = pDate2;
                pDate2 = FormatDate(pDate2, { Format: 'MMM d, yyyy' });
                mElement.find('[name=butCalendar]').text(pDate2);
            });

            mElement.find('[name=butArchiveNow]').click(function() {
                if (mDate == null) {
                    NewMessageBox('Please choose a date first.', 'No selected date');
                    return;
                }
                var pDate = FormatDate(mDate, { Format: 'MMM d, yyyy' });

                NewMessageBox('Are you sure you archive records older than ' + pDate + '?', 'Archive', [
                    {
                        Text: 'Yes',
                        Click: function() {
                            ArchiveNow();
                        }
                    },
                    {
                        Text: 'No',
                        Click: function() {
                            
                        }
                    }
                ]);
            });

            function ArchiveNow() {
                mElement.find('[name=butArchiveNow]').text('Please Wait').prop('disabled', true);
                AjaxSend({
                    url: 'Archive/ArchiveNow',
                    data: {
                        Date2: FormatDate(mDate, { Format: 'yyyy/MM/dd' })
                    },
                    success: function(vData) {
                        if (vData.Error) {
                            NewMessageBox(vData.Error, '');
                            return;
                        }
                        var pMessage = 'The archive process has successfully completed. Number of affected records: ';
                        pMessage += AccountingFormat(vData.AffectedRecords, 0);
                        NewMessageBox(pMessage, 'Done');
                    },
                    complete: function() {
                        mElement.find('[name=butArchiveNow]').text('Archive Now').prop('disabled', false);
                    }
                });
            }

            return mOut;

        }); // END CreateStringObject('App.controller.PriceChange', null, function() {


        return mDeferred.promise();

    });
