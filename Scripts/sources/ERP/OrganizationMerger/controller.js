define([
    'common'
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/OrganizationMerger/view.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.OrganizationMerger.controller', null, function () {

        var mOut = {};
        var mElement = $(mHtml);
        var mDivStatus = mElement.find('[name=divStatus]');
        var mType;

        $('body').append(mElement);//.trigger('create');

        var mDivContents = mElement.find('[name=divContents]');

        mOut.Show = function(vParams) {
            vParams = $.extend({
                ProductID: null,
                ProductNo: null,
                ProductName: null,
                OrderController: null,
                AfterSave: function() {}
            }, vParams);
         
            mElement.modal('show');
            mType = 'Customers';
            GetData();
        };

        mOut.ShowSuppliers = function(vParams) {
            vParams = $.extend({
                ProductID: null,
                ProductNo: null,
                ProductName: null,
                OrderController: null,
                AfterSave: function() {}
            }, vParams);
         
            mElement.modal('show');
            mType = 'Suppliers';
            GetData();
        };

        mElement.find('[name=butDeleteCache]').click(function() {
            GetData(true);
        });

        mElement.find('[name=butRefreshList]').click(function() {
            GetData();
        });

        function GetData(vDeleteCache) {
            
            mDivContents.html('');
            Bs.Loading('show');
            AjaxSend({
                url: '/Organization/GetSimilar' + mType,
                data: {
                    DeleteCache: vDeleteCache
                },
                timeout: 8123,
                success: function(vData) {
                    DisplayData(vData);
                },
                error: function(vData) {
                    Bs.Loading('hide');
                    GetData();
                }
            });

        }

        function DisplayData(vData) {
            var pHtml = 'No. of similiar customers in cache: ' + AccountingFormat(vData.Items.length, 0) + '<br />' +
                'Cache date started: ' + FormatDate(vData.DateStarted, {Format: 'MM/dd/yyyy hh:mm:ss a'}) + '<br />' +
                'Cache date updated: ' + FormatDate(vData.DateUpdated, {Format: 'MM/dd/yyyy hh:mm:ss a'});
            if (vData.DateEnded != null)
                pHtml += '<br />Cache date ended: ' + FormatDate(vData.DateEnded, {Format: 'MM/dd/yyyy hh:mm:ss a'});
            if (CBool(vData.Busy))
                pHtml += '<br />The compare process is currently executing.';
            mDivStatus.html(pHtml);
            for (var p = 0; p <= vData.Items.length - 1; p++) {
                var pItem = vData.Items[p];
                pItem.RadioName = p;
                var pDivItem = $('<div>');
                pDivItem.attr('name', 'divGroup');
                pDivItem.css('border', '1px solid #b5b5b5');
                pDivItem.css('margin', '10px');
                pDivItem.css('padding', '10px');
                pDivItem.css('border-radius', '3px');
                pDivItem.append($('<div>').text('Check or mark the organization to retain. All other organizations in this group will be deactivated and their transactions will be attributed to the marked organization.'));
                pDivItem.append(CreateItemDiv(pItem));

                for (var pDuplicateIndex = 0; pDuplicateIndex <= pItem.Duplicates.length - 1; pDuplicateIndex++) {
                    var pDuplicate = pItem.Duplicates[pDuplicateIndex];
                    pDuplicate.RadioName = p;
                    pDivItem.append(CreateItemDiv(pDuplicate));
                }

                pDivItem.append(CreateItemDiv({
                    RadioName: p,
                    CompanyName: 'Retain all organizations in this group'
                }));
                pDivItem.find('input[type=radio]').data('ParentData', pItem);

                mDivContents.append(pDivItem);
            }
            Bs.Loading('hide');
            //$('.chbMerger').checkboxradio().checkboxradio('refresh');
        }

        function CreateItemDiv(vData) {
            var pOut = $('<div class="form-check">');
            var pInput = $('<input data-iconpos="notext" class="chbMerger form-check-input" data-role="none" type="radio">');
            var pContainer = $('<label class="form-check-label">');
            var pLabel = vData.CompanyName;
            pInput.attr('name', 'RadioName' + vData.RadioName);
            pInput.data('Data', vData);
            pContainer.append(pInput);
            pOut.append(pContainer);
            
            if (CStr2(vData.OrgID) != '') {
                //pLabel += ' ' + vData.OrgID
                pLabel += ' | Last updated: ' + vData.LastUpdated;
            }

            pOut.find('label').append(pLabel);

            return pOut;
        }

        mElement.find('[name=butRetain1st]').click(function() {

            // https://stackoverflow.com/a/7620596/334045
            mDivContents.find('[name=divGroup]').each(function() {
                $('input[type=radio]:first', this).attr('checked', true);
            });
            //$('.chbMerger').checkboxradio().checkboxradio('refresh');

        });

        mElement.find('[name=butSave]').click(function() {
            var pOut = {
                Items: []
            };
            var pSelected = mDivContents.find('input[type=radio]:checked');
            for (var p = 0; p <= pSelected.length - 1; p++) {
                var pInput = $(pSelected[p]);
                var pData = {
                    OrgID: pInput.data('Data').OrgID,
                    Duplicates: []
                };
                if (CStr2(pData.OrgID) == '')
                    continue;

                var pParentOrgID = pInput.data('ParentData').OrgID;
                if (pParentOrgID != pData.OrgID)
                    pData.Duplicates.push(pParentOrgID);

                var pDuplicates = pInput.data('ParentData').Duplicates;
                for (var pDuplicateIndex = 0; pDuplicateIndex <= pDuplicates.length - 1; pDuplicateIndex++) {
                    var pDuplicate = pDuplicates[pDuplicateIndex];
                    if (pDuplicate.OrgID == pData.OrgID)
                        continue;
                    pData.Duplicates.push(pDuplicate.OrgID);
                }
                pOut.Items.push(pData);
            }
            if (pOut.Items.length == 0) {
                NewMessageBox('Nothing to save');
                return;
            }

            NewMessageBox('Are you sure you want to modify the ' + pOut.Items.length + ' groups of organizations?', 'Modify', [
                {
                    Text: 'Yes',
                    Click: function() {
                        Save(pOut);
                    }
                },
                {
                    Text: 'No'
                }
            ]);
        });

        function Save(vData) {

            Bs.Loading('show');
            AjaxSend({
                url: '/Organization/MergeDuplicate' + mType,
                data: vData,
                success: function(vData) {
                    Bs.Loading('hide');
                    if (GetStringObject('Result', vData) == 'ok') {
                        NewMessageBox('The selected organizations were successfully merged.');
                        mDivContents.html('');
                        // GetData();
                    } else if (CStr2(GetStringObject('Error', vData)) != '') {
                        NewMessageBox(vData.Error);
                    }
                }
            });

        }

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});