define([
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/PIM/pimSaveAs/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                // ERP.OrderProcessing.product.controller = ERP.OrderProcessing.product.controller(); // singleton
                // mHtml.trigger('create');
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('PIM.pimSaveAs.controller', null, function () {

        var mOut = {};
        var mElement;
        var mProductID;
        var mOnOk;
        var mOnCancel;
        var mProductNum;
        var mProductName;
        var mProductBrand;

        //$('body').append($(mHtml));
        //mElement = $('[idx="popOrderInput"]').attr('id', popupid); // popOrderInput only

        mElement = $(mHtml);
        mElement.attr('id', 'popPimSaveAs' + NewID());
        $('body').append(mElement);

        mOut.element = mElement;
        
       //mOut.element.trigger('create');        

        mOut.popup = function(vParams) {
            
            mProductID = vParams.ProductID;
            mProductNum = vParams.ProductNum.trim();
            mElement.find('[name=txtProductNum]').val(mProductNum);
            mProductName = vParams.ProductName.trim();
            mElement.find('[name=txtProductName]').val(mProductName);
            mProductBrand = vParams.ProductBrand.trim();
            mElement.find('[name=txtProductBrand]').val(mProductBrand);
            mOnOk = vParams.OnOk;
            mOnCancel = vParams.OnCancel;

            //mElement.css({ visibility: 'visible', display: 'block' });

            try {
                mElement.modal('show');
            } catch (ex) {
                // just ignore the error
                console.log(ex);
            }
        };        

        mElement.find('[name=butOk]').click(function() {
            verify(function(vPassed) {
                if (CBool(vPassed) == false)
                    return;


            });
        });

        function verify(vCallback) {
            if (mElement.find('[name=txtProductNum]').val().trim().toLowerCase() ==  mProductNum.toLowerCase()) {
                mElement.find('[name=divResult]').html('Product Number must be different <br>');
                vCallback(false);
                return;
            }

            if (mElement.find('[name=txtProductName]').val().trim().toLowerCase() ==  mProductName.toLowerCase()) {
                mElement.find('[name=divResult]').html('Product Name must be different <br>');
                vCallback(false);
                return;
            }

            if (mElement.find('[name=txtProductBrand]').val().trim().toLowerCase() == '') {
                mElement.find('[name=divResult]').html('Please enter the Brand name of the new Product <br>');
                vCallback(false);
                return;
            }

            if (mElement.find('[name=chbNewProduct]:checked').length == 0) {
                mElement.find('[name=divResult]').html('Please select the tagging of the new Product <br>');
                vCallback(false);
                return;
            }

            var pData = {
                SourceProductID: mProductID,
                ProductNum: mElement.find('[name=txtProductNum]').val(),
                ProductName: mElement.find('[name=txtProductName]').val(),
                ProductBrand: mElement.find('[name=txtProductBrand]').val(),
                TagAs: mElement.find('[name=chbNewProduct]:checked').val()
            };
            mElement.find('[name=divResult]').text('Validating...');
            AjaxSend({
                url: '/PIMProduct/SaveAs',
                data: pData,
                success: function(vData) {
                    if (vData.Existing != null) {
                        if (mElement.find('[name=txtProductNum]').val().trim().toLowerCase() ==  CStr2(vData.Existing.ProductNo).trim().toLowerCase()) {
                            mElement.find('[name=divResult]').text('Product with the same number already exist.');
                        } else
                            mElement.find('[name=divResult]').text('Product with the same name already exist.');
                        vCallback(false);
                        return;
                    } else if (vData.NewProductID) {
                        mElement.find('[name=divResult]').text('Product data was successfully saved.');
                        NewMessageBox('Product data was successfully saved', 'Saved As', null, {
                            Close: function() {
                                sessionStorage.ViewID = vData.NewProductID;
                                LibraryTable.CompanyWideProductListPage();
                                mElement.modal('hide');
                            }
                        });
                    }
                }
            });
        }

        //mElement.find('[name=butCancel]').click(function() {
        //    mElement.popup('close');
        //});

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});