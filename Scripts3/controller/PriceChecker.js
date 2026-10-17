define([
    // '/Scripts3/model/PeerConnect.js?crc='
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts3/view/PriceChecker.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });

    })();

    $(function () {

        CreateStringObject('App.controller.PriceChecker', null, function () {

            var mOut = {};
            var mElement;
            var mTxtPriceCheckerSearch;
            var mLblPriceCheckProductDescription;
            var mLblPriceCheckerRetailPrice;

            mElement = $(mHtml);
            mOut.element = mElement;

            function Init() {
                if (OfflineMode) {
                    Android.UpdateSessionData({ OrgID: sessionStorage.OrgID });
                    Web.HandleOrganization(true);
                }
                mTxtPriceCheckerSearch = mElement.find('#txtPriceCheckerSearch');
                mLblPriceCheckProductDescription = mElement.find('#lblPriceCheckProductDescription');
                mLblPriceCheckerRetailPrice = mElement.find('#lblPriceCheckerRetailPrice');
                mElement.find('form').submit(function(e) {
                    e.preventDefault();
                    Search();
                    return false;
                });
            }
            
            mElement.find('[name=imgHome]').click(function () {
                mElement.find('[name=divMessage]').hide();
                mElement.find('[name=divPopPassword]').modal('show');
            });

            mElement.find('[name=divPopPassword] button[name=ok]').click(function() {
                var pUsername = mElement.find('[name=txtUsername]').val();
                var pPassword =  mElement.find('[name=txtPassword]').val();
                var pSql = 'SELECT * FROM LibraryPeopleUserAccount WHERE Username = ' + ReadyString(pUsername);
                var pDT = Sql.Execute(pSql);
                if (pDT.Rows.length == 0) {
                    mElement.find('[name=divMessage]').text('Invalid username/password').show();
                } else if (CBool(Android.VerifyHash(pPassword, pDT.Rows[0].UserPassword))) {
                    mElement.find('[name=divPopPassword]').modal('hide');
                    $('#pageHome').pageshow();
                    //setTimeout(function() {
                    //    history.back();
                    //}, 200);
                } else
                    mElement.find('[name=divMessage]').text('Invalid username/password').show();
                    
            });

            mElement.find('[name=divPopPassword] button[name=close]').click(function() {
                mElement.find('[name=divPopPassword]').modal('hide');
            });

            function Search() {
                var options = [{
                    Search: mTxtPriceCheckerSearch.val(),
                    DisplayText: [{ Column: 'ProductName' }],
                    Click: function (e) {
                        console.log('jsN2LVBEJ2M5');
                        var pPrice = Business.LibraryPIMProduct.GetRetailPrice(e.ID, sessionStorage.OrgID);
                        mLblPriceCheckerRetailPrice.text(AccountingFormat(pPrice));
                        mLblPriceCheckProductDescription.text(e.ProductName);
                        setTimeout(function () {
                            mTxtPriceCheckerSearch.select().focus();
                        }, 200);
                    }
                }]

                if (OfflineMode) {
                    options[0].Offline = true;
                }

                BrowsePopup('LibraryPIMProduct', 'Select Product', options);
            }

            function mButSecondScreenChangePinSave_Click() {
                var pPin = mTxtSecondScreenChangePinPin.val();
                if (pPin.length < 3) {
                    NewMessageBox('PIN must be longer than 2 characters.');
                    return;
                }
                if (pPin != mTxtSecondScreenChangePinConfirmPin.val()) {
                    NewMessageBox('PIN must be equal to confirm PIN.');
                    return;
                }
                mOut.model.SavePin(pPin);
                NewMessageBox('PIN was successfully saved.');
            }

            mElement.find('[name=butBarCode]').click(function () {
                if ((CBool(window.IsMobile)) && (!CBool(window.InWindows))) {
                    mobile.Scan({
                        Success: function (vData) {
                            mTxtPriceCheckerSearch.val(vData.Value);
                            Search();
                        }
                    });
                } else {
                    MessageBox('Scan Barcode / QR Code feature is only available on the mobile app',
                        'Scan Barcode / QR Code',
                        [
                            { Text: 'Ok' }
                        ]
                    );
                }
            });

            mOut.popup = function () {

                if ($('body #divPriceChecker').length == 0) {
                    $('body > div:eq(0)').append(mElement);
                    Init();
                    //$('body').trigger('create');
                }
                $('#divPriceChecker').pageshow();
                mElement.find('[name=divMessage]').text('');
                //$.mobile.changePage(mElement, {
                //    allowSamePageTransition: true
                //});
              
                setTimeout(function() {
                    mTxtPriceCheckerSearch.select().focus();
                }, 500);
            };
            
            return mOut;

        }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    }); // END $(function () {

    return mDeferred.promise();

});