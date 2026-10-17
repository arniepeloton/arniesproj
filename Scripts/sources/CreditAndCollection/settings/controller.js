define([
    'promise!/Scripts/sources/CreditAndCollection/settings/GrantCredit/controller.js?v=' + assetversion
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/CreditAndCollection/settings/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.CreditAndCollection.settings.controller = ERP.CreditAndCollection.settings.controller(); // singleton
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.CreditAndCollection.settings.controller', null, function () {

        var mOut = {};
        var mElement;
        var mEsoaEarliestDate;
        var mSoaEarliestDate;

        mElement = $(mHtml);
        mOut.element = mElement;
        
        mElement.find('[name=butSave]').click(function () {
            if (!IsUrl($('[name="soalinkdomain"]').val())) {
                WebToast('Please enter valid SOA Link domain', 'top');
                return;
            }

            $(mElement.find('form')).trigger('submit');
        });

        mElement.find('form').submit(function(e) {
            var p;

            e.preventDefault();
            var formData = new FormData($(this)[0]);
            var pJson = ERP.CreditAndCollection.settings.GrantCredit.GetInputData();
            pJson.OrgID = sessionStorage.OrgID;
            pJson.EsoaEarliestDate = mEsoaEarliestDate;
            pJson.SoaEarliestDate = mSoaEarliestDate;
            formData.append('json', JSON.stringify(pJson));
            formData.append('SOAFootnote', mElement.find('[name="txtSOAFootnote"]').val())
            //formData.append('OrgID', sessionStorage.OrgID);
            //formData.append('IsActive', pChkActive.prop('checked'));
            $.ajax({
                url: '/CreditAndCollection/SaveSettings',
                type: 'POST',
                data: formData,
                async: false,
                cache: false,
                contentType: false,
                processData: false,
                success: function (vData) {
                    try {
                        vData = JSON.parse(vData);
                        if (vData.d)
                            vData = vData.d;
                    } catch (e) {
                        NewMessageBox('An error occured while parsing the server response to JSON: ' + e.message);
                        return;
                    }
                    if (vData.Error)
                        NewMessageBox(vData.Error);
                    else if (vData.Warning)
                        NewMessageBox(vData.Warning);
                    else if (vData.Result == 'ok')
                        NewMessageBox('The data was successfully saved.');
                    else
                        NewMessageBox('Unknown status. ref js8TWY3ZAGW9!');
                }
            });
        });

        mElement.find('[name=butEsoaEarliestDate]').click(function() {
            SingleCalendar({
                Click: function (e) {
                    mEsoaEarliestDate = e;
                    mElement.find('[name=butEsoaEarliestDate]').text(e);
                }
            });
        });

        mElement.find('[name=butSoaEarliestDate]').click(function() {
            SingleCalendar({
                Click: function (e) {
                    mSoaEarliestDate = e;
                    mElement.find('[name=butSoaEarliestDate]').text(e);
                }
            });
        });

        mElement.find('#btnCnCQuickRemarks').click(function () {
            Bs.Loading('show');
            require(['promise!/Scripts/sources/ERP/RemarksMessages/controller.js?v=' + assetversion], function () {
                Bs.Loading('hide');
                ERP.RemarksMessages.main.controller.show({ module: 'cnc', filter: 'SOA' });
            })
        })

        var mIsLoaded;

        mOut.Initialize = function() {
            $('#tabCacSettings').html('').append(mElement);
            $('#btntabCacSettings').click(function () {
                var pAccess;
                if (mIsLoaded) {
                } else {
                    if (proj.CacMode == 'AR')
                        pAccess = 'Access Credit and Collection Settings*';
                    else
                        pAccess = 'Accounts Payable (EAS) Settings*';
                    NewPromptUserAccess(pAccess, {
                        OnClickOKSuccess: function () {
                            AjaxSend({
                                url: 'CreditAndCollection/GetSettings',
                                success: function (vData) {
                                    mElement.find('[name="expirationdaysforsoalink"]').val(vData.ExpirationDaysForSOALink || 7);
                                    mElement.find('[name="soalinkdomain"]').val(vData.SOALinkDomainName);
                                    mElement.find('[name="soalinkinternalipaddress"]').val(vData.SOALinkDomainInternalIPAddress);
                                    mElement.find('[name="soalinkexternalipaddress"]').val(vData.SOALinkDomainExternalIPAddress);
                                    mElement.find('[name="txtSOAFootnote"]').val(vData.SOAFootnote);
                                    var pDate = ParseDate(vData.EsoaEarliestDate);
                                    if (pDate != null)
                                        mElement.find('[name=butEsoaEarliestDate]').text(FormatDate(pDate, { Format: 'MMM dd, yyyy' }));
                                    pDate = ParseDate(vData.SoaEarliestDate);
                                    if (pDate != null)
                                        mElement.find('[name=butSoaEarliestDate]').text(FormatDate(pDate, { Format: 'MMM dd, yyyy' }));
                                }
                            });

                            NewPromptUserAccess('Credit and Collection Settings > Grant Credit', {
                                OnClickOKSuccess: function () {
                                    mElement.find('[name=divGrantCredit]').append(ERP.CreditAndCollection.settings.GrantCredit.element);         
                                    //ERP.CreditAndCollection.settings.GrantCredit.element.trigger('create');
                                    //mOut.Load();
                                    ERP.CreditAndCollection.settings.GrantCredit.LoadData();
                                },
                                OnCancel: function() {
                                    //mOut.Load();

                                    //AjaxSend({
                                    //    url: 'CreditAndCollection/GetSettings',
                                    //    success: function (vData) {
                                    //        mElement.find('[name="expirationdaysforsoalink"]').val(vData.ExpirationDaysForSOALink || 7);
                                    //        mElement.find('[name="soalinkdomain"]').val(vData.SOALinkDomainName);
                                    //        mElement.find('[name=butEsoaEarliestDate]').text(vData.EsoaEarliestDate);
                                    //    }
                                    //});
                                }
                            });                            

                            $('#tabCacSettings').opentabmenu();
                        }
                    });
                    //mIsLoaded = true;
                }
            });
            
        };

        mOut.Load = function() {
        };

        mOut.Initialize();

        return mOut;
    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});