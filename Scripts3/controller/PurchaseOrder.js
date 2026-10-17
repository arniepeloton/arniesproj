define(['/Scripts3/Browser/LibraryPIMProduct.js?crc=',
    '/Scripts3/Browser/LibraryOrganizationSupplier.js?crc='
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts3/view/PoPopup.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });

    })();

    $(function () {

        CreateStringObject('App.controller.PurchaseOrder', null, function () {

            var mOut = {};
            var mSelOrganization;
            var mSelOutlet;
            var mSelDepartment;
            var mBinID; // from mSelDepartment
            var mInputData; 
            var mOrgID;
            var mOrgName;
            var mNetSuiteSupplierFowstID;

            mOut.element = $(mHtml);
            mOut.ok = function(){}; // override this

            function InitNames() {
                mSelOrganization = mOut.element.find('#selOrganization');
                mSelOrganization
                    .unbind('change', mSelOrganization_change)
                    .empty()
                    .change(mSelOrganization_change)
                    .selectmenu('refresh');

                mSelOutlet = mOut.element.find('#selOutlet');
                mSelOutlet
                    .unbind('change', PopDepartmentCombo)
                    .empty()
                    .change(PopDepartmentCombo)
                    .selectmenu('refresh');

                mSelDepartment = mOut.element.find('#selDepartment');
                mSelDepartment
                    .unbind('change', mSelDepartment_change)
                    .empty()
                    .change(mSelDepartment_change)
                    .selectmenu('refresh');

                $('#butPurchaseOrderOk')
                    .unbind('click', butPurchaseOrderOk_click)
                    .click(butPurchaseOrderOk_click);
            }

            function butPurchaseOrderOk_click() {
                var pOutlet = mSelOutlet.find('option:selected');
                var pDepartment = mSelDepartment.find('option:selected');

                var pData = {
                    OrgID: mOrgID,
                    CompanyName: mOrgName,
                    FowstID: pOutlet.prop('value'),
                    FowstName: pOutlet.text(),
                    BinID: mBinID,
                    BinName: pDepartment.text(),
                    NetSuiteSupplierFowstID: mNetSuiteSupplierFowstID
                };
                if (!IsGuid(pData.FowstID)) {
                    NewMessageBox('Please select an outlet or FOWST');
                    return;
                }

                mOut.ok(pData);
                mOut.element.popup('close');
            } // END butPurchaseOrderOk_click

            mOut.popup = function(vData) {

                mNetSuiteSupplierFowstID = null;
                if (mOut.element.parent().length == 0) {
                    $.mobile.activePage.append(mOut.element);
                    $.mobile.activePage.trigger('create');
                }
                mInputData = $.extend({
                    Title: '',
                    LockedOrg: false,
                    LockedFowst: false
                }, vData);

                InitNames();
                PopOrgCombo();

                mOut.element.find('#txtTitlePopup').text(mInputData.Title);
                Repopup();

            };

            function Repopup() {

                mOut.element.popup('open');
                var pElement = mOut.element.closest('.ui-popup-container');
                if (pElement.length == 0)
                    console.trace('popup container for divRequestPrice not found');
                //else
                //    // know issue: pop up dimension will revert to default after browser window resize
                //    pElement.css({
                //        left: 0,
                //        width: '80%',
                //        margin: 'auto 10%',
                //        padding: 15
                //    });
            }

            function mSelOrganization_change() {
                
                var pOption = mSelOrganization.find('option:selected');
                var pSelectedOrg = pOption.prop('value');

                if (pSelectedOrg == 'third') {
                    var pBrowser = Browser.LibraryOrganizationSupplier;

                    pBrowser.InputData.IncludeCustomersAndSuppliers = 1;
                    pBrowser.Click = function(e) {
                        var pOption = mSelOrganization.find('option[value=third]');
                        mOrgName = e.CompanyName;
                        pOption.text('3rd Party (' + mOrgName + ')');
                        mSelOrganization.selectmenu('refresh');
                        mNetSuiteSupplierFowstID = e.NetSuiteSupplierFowstID;
                        mOrgID = e.ID;
                        PopOutletCombo();
                        setTimeout(Repopup, 100);
                    };
                    pBrowser.Show();
                } else {
                    mOrgID = sessionStorage.OrgID;
                    mOrgName = pOption.text();
                }
                PopOutletCombo();

            } // END mSelOrganization_change

            function PopOrgCombo() {
                
                var pOption;
                var pDR;

                mSelOrganization.empty();
                mOrgID = Web.OrgID();
                pDR = Business.LibraryOrganization.ByID(mOrgID);
                pDR = $.extend({}, pDR);
                if (CStr2(pDR.CompanyName).trim() == '')
                    pDR.CompanyName = 'own organization';
                mOrgName = pDR.CompanyName;
                mSelOrganization.append(
                    new Option(mOrgName)
                );
                mSelOrganization.append(
                    new Option('3rd Party', 'third')
                );
                mSelOrganization.selectmenu('enable');

                if (IsGuid(mInputData.OrgID)) {
                    if (mInputData.OrgID == sessionStorage.OrgID)
                        mSelOrganization.find('option[value=own]').prop('selected', true);
                    else {
                        mOrgID = mInputData.OrgID;
                        pDR = Business.LibraryOrganization.ByID(mOrgID);
                        pDR = $.extend({}, pDR);
                        mOrgName = pDR.CompanyName;

                        pOption = mSelOrganization.find('option[value=third]');
                        pOption.prop('selected', true);
                        pOption.text('3rd Party (' + mOrgName + ')');
                    }
                    if (mInputData.LockedOrg) {
                        mSelOrganization.selectmenu('disable');
                    }
                }
                mSelOrganization.selectmenu('refresh');
                PopOutletCombo();
            }

            function PopOutletCombo() {
                var pDR;
                // var pOrgID = mSelOrganization.find('option:selected').prop('value');
                var pOrgID = mOrgID;
                mSelOutlet.selectmenu('enable');
                mSelOutlet.empty().selectmenu('refresh');
                if (!IsGuid(pOrgID)) {
                    PopupReposition();
                    return;
                }
                var pDT = OfflineBrowser.LibraryCustomerFowst.GetData({
                    OrgID: pOrgID,
                    MaxRecords: Number.MAX_SAFE_INTEGER
                });
                if (pDT.Rows.length > 0)
                    console.log('jsYRQH579X9W!');
                mSelOutlet.append(
                    new Option('Please select')
                );
                for (var p = 0; p <= pDT.Rows.length - 1; p++) {
                    pDR = pDT.Rows[p];
                    var pOption = new Option(pDR.FowstName, pDR.ID);
                    mSelOutlet.append(pOption);
                }
                if (IsGuid(mInputData.FowstID)) {
                    mSelOutlet.find('option[value=' + mInputData.FowstID + ']').prop('selected', true);
                    mSelOutlet.selectmenu('refresh');
                    if (mInputData.LockedOrg) {
                        mSelOutlet.selectmenu('disable');
                    }
                    PopDepartmentCombo();
                }
                PopupReposition();
            }

            function PopDepartmentCombo() {
                var pDR;
                var pFowstID = mSelOutlet.find('option:selected').prop('value');
                mSelDepartment.empty().selectmenu('refresh');
                if (!IsGuid(pFowstID)) {
                    PopupReposition();
                    // NewMessageBox('jsB3UYTH9S98! FowstID must be GUID');
                    return;
                }
                var pDT = Business.LibraryFowstBin.GetByFowstID(pFowstID);
                if (pDT.Rows.length > 0)
                    console.log('jsYLBT67954H!');
                mSelDepartment.append(
                    new Option('Please select')
                );
                for (var p = 0; p <= pDT.Rows.length - 1; p++) {
                    pDR = pDT.Rows[p];
                    var pOption = new Option(pDR.Name, pDR.ID);
                    mSelDepartment.append(pOption);
                }

                if (CStr2(mInputData.BinName) != '') {
                    mSelDepartment.find('>option:contains(' + mInputData.BinName + ')').prop('selected', true);
                    mSelDepartment.selectmenu('refresh');
                }
                PopupReposition();
            }

            function mSelDepartment_change() {
                var pFowstID = mSelOutlet.find('option:selected').prop('value');
                var pName = mSelDepartment.find('option:selected').text();
                var pDT = Business.LibraryFowstBin.GetByName(pName, pFowstID);

                if (pDT.Rows.length > 0)
                    mBinID = pDT.Rows[0].ID;
                else
                    mBinID = null;
                PopupReposition();
            }

            mOut.SaveSupplier = function(vInvoiceID, vIU, vExecute) {
                var pIU;
                var pSql;

                if (!IsGuid(vInvoiceID))
                    return;

                if (vIU == null)
                    pIU = new InsertUpdate('TransInvoice');
                else
                    pIU = vIU;

                 if (IsGuid(sessionStorage.SupplierOrganizationID))
                    pIU.Add('SupplierID', ReadyString(sessionStorage.SupplierOrganizationID));
                else
                    pIU.Add('SupplierID', 'null');

                if (CStr2(sessionStorage.SupplierFowstID).trim() != '')
                    pIU.Add('SupplierFowstID', ReadyString(sessionStorage.SupplierFowstID));
                else
                    pIU.Add('SupplierFowstID', 'null');

                if (CStr2(sessionStorage.SupplierBinID).trim() != '')
                    pIU.Add('SupplierBinID', ReadyString(sessionStorage.SupplierBinID));
                else
                    pIU.Add('SupplierBinID', 'null');
                    
                if (vExecute) {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vInvoiceID));
                    Sql.ExecuteSql(pSql);
                }
                    
                return pIU;
            };

            mOut.SaveCustomer = function(vInvoiceID, vIU, vExecute, vNetSuiteSupplierFowstID) {
                var pIU;
                var pSql;

                if (!IsGuid(vInvoiceID))
                    return;

                if (vIU == null)
                    pIU = new InsertUpdate('TransInvoice');
                else
                    pIU = vIU;

                // todo: save customer ID
                if (CStr2(sessionStorage.CustomerOrganizationID).trim() != '')
                    pIU.Add('CustomerID', ReadyString(sessionStorage.CustomerOrganizationID));
                else
                    pIU.Add('CustomerID', 'null');

                if (CStr2(sessionStorage.CustomerFowstID).trim() != '')
                    pIU.Add('CustomerFowstID', ReadyString(sessionStorage.CustomerFowstID));
                else
                    pIU.Add('CustomerFowstID', 'null');

                if (CStr2(sessionStorage.CustomerBinID).trim() != '')
                    pIU.Add('CustomerBinID', ReadyString(sessionStorage.CustomerBinID));
                else
                    pIU.Add('CustomerBinID', 'null');

                if (IsGuid(vNetSuiteSupplierFowstID))
                    pIU.Add('SupplierFowstID', ReadyString(vNetSuiteSupplierFowstID));

                if (vExecute) {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vInvoiceID));
                    Sql.ExecuteSql(pSql);
                }
                
                return pIU;
            };

            mOut.SaveSupplierAndCustomer = function(vInvoiceID) {
                var pIU;

                pIU = mOut.SaveSupplier(vInvoiceID);
                mOut.SaveCustomer(vInvoiceID, pIU, true);
            };

            return mOut;

        }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    }); // END $(function () {

    return mDeferred.promise();

});